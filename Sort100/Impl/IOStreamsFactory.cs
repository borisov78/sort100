using System;
using System.IO;
using System.IO.Compression;
using Sort100.Interfaces;
using Sort100.Params;

namespace Sort100.Impl
{
    internal sealed class IOStreamsFactory : IIOStreamsFactory
    {
        private readonly IOParams _ioParams;
        
        public IOStreamsFactory(IOParams ioParams)
        {
            _ioParams = ioParams ?? throw new ArgumentNullException(nameof(ioParams));
        }

        public Stream GetForRead(string filePath, bool useCompression = false)
        {
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None,
                _ioParams.ReadBufferSizeInBytes, FileOptions.SequentialScan);
            // Performance optimization: временные файлы будем сохранять и читать с быстрой компрессией.
            // На утилизацию CPU почти не влияет, но на ~60% уменьшает размер файлов (экономия операций ввода-вывода).
            return useCompression
                ? (Stream) new BrotliStream(fileStream, CompressionMode.Decompress, false)
                : fileStream;
        }

        public Stream GetForWrite(string filePath, bool useCompression = false)
        {
            var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None,
                _ioParams.WriteBufferSizeInBytes, FileOptions.SequentialScan);
            return useCompression
                ? (Stream) new BrotliStream(fileStream, CompressionLevel.Fastest, false)
                : fileStream;
        }
    }
}