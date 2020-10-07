using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Sort100.Helpers;
using Sort100.Interfaces;
using Sort100.Params;

namespace Sort100.Impl
{
    internal sealed class SortedChunksStorage : ISortedChunksStorage, IDisposable
    {
        private readonly IOParams _ioParams;
        private readonly AlgParams _algParams;
        private readonly IIOStreamsFactory _streamsFactory;
        private readonly ConcurrentBag<string> _storedChunks = new ConcurrentBag<string>();

        public SortedChunksStorage(IOParams ioParams, AlgParams algParams, IIOStreamsFactory streamsFactory)
        {
            _ioParams = ioParams ?? throw new ArgumentNullException(nameof(ioParams));
            _algParams = algParams ?? throw new ArgumentNullException(nameof(algParams));
            _streamsFactory = streamsFactory ?? throw new ArgumentNullException(nameof(streamsFactory));
        }

        public void StoreSortedChunk(Chunk chunk)
        {
            var tmpFilePath = Guid.NewGuid().ToString("N") + ".tmp";
            if (!string.IsNullOrEmpty(_ioParams.TempDir))
            {
                if (!Directory.Exists(_ioParams.TempDir))
                    Directory.CreateDirectory(_ioParams.TempDir);
                tmpFilePath = Path.Combine(_ioParams.TempDir, tmpFilePath);
            }
            
            _storedChunks.Add(tmpFilePath);

            using var output = _streamsFactory.GetForWrite(tmpFilePath, true);
            using var entryWriter = new EntryWriter(output, _algParams.Encoding, _ioParams.WriteBufferSizeInBytes);
            entryWriter.WriteAll(chunk);
            output.Flush();
        }

        public EntryReader[] GetStoredChunkReaders()
        {
            var result = new List<EntryReader>();
            foreach (var tmpFilePath in _storedChunks)
            {
                var source = _streamsFactory.GetForRead(tmpFilePath, true);
                result.Add(new EntryReader(source, true));
            }

            return result.ToArray();
        }

        public void Dispose()
        {
            foreach (var tmpFilePath in _storedChunks)
                FileHelper.SafeDeleteFile(tmpFilePath);
            FileHelper.SafeDeleteDirectory(_ioParams.TempDir);
        }
    }
}