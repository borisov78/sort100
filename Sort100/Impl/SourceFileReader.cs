using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Sort100.Interfaces;
using Sort100.Params;

namespace Sort100.Impl
{
    internal sealed class SourceFileReader : ISourceFileReader
    {
        private readonly IOParams _ioParams;
        private readonly AlgParams _algParams;
        private readonly IIOStreamsFactory _streamsFactory;
        private readonly List<IChunkProcessor> _listeners = new List<IChunkProcessor>();
        private readonly ArrayPool<Entry> _chunksPool;

        public SourceFileReader(IOParams ioParams, AlgParams algParams, IIOStreamsFactory streamsFactory)
        {
            _ioParams = ioParams ?? throw new ArgumentNullException(nameof(ioParams));
            _algParams = algParams ?? throw new ArgumentNullException(nameof(algParams));
            _streamsFactory = streamsFactory ?? throw new ArgumentNullException(nameof(streamsFactory));
            _chunksPool = ArrayPool<Entry>.Create(_algParams.ChunkCapacity, 10);
        }

        private WaitHandle[] GetListenersWaitHandles()
        {
            return _listeners.Select(l => l.IsReady).ToArray();
        }

        private IChunkProcessor GetFirstReadyListener()
        {
            var waitHandles = GetListenersWaitHandles();
            var firstReadyIndex = WaitHandle.WaitAny(waitHandles);
            return _listeners[firstReadyIndex];            
        }

        private void WaitForAllListeners()
        {
            var waitHandles = GetListenersWaitHandles();
            WaitHandle.WaitAll(waitHandles);
        }

        public void ReadAndSplitChunks()
        {
            Chunk InitNewChunk()
            {
                return new Chunk(_chunksPool, _algParams.ChunkCapacity);
            }
            
            using var source =  _streamsFactory.GetForRead(_ioParams.SourceFilePath);
            using var entryReader = new EntryReader(source);

            var currentChunk = InitNewChunk();
            var currentChunkLength = 0;

            do
            {
                // Читаем строки исходного файла и заполняем текущий Chunk
                var charsRead = entryReader.ReadNext();
                if (charsRead > 0 && !currentChunk.IsFool)
                {
                    currentChunkLength += charsRead;
                    currentChunk.Add(entryReader.Current);
                }
                
                // Если достигнут максимально допустимый размер Сргтл или файл кончился - отдаем Chunk нга обработку
                if (currentChunk.IsFool 
                    || currentChunkLength >= _algParams.ChunkSizeInChars 
                    || entryReader.Current == null && currentChunkLength > 0)
                {
                    // Получаем первый свободный обработчик
                    var firstReadyProcessor = GetFirstReadyListener();
                    // Отдаем ему текущий Chunk, но окончания обработки не ждем - начинаем читать следюущие Chunk.
                    // Таким образом полчаем параллелизацию чтения исходного файла и сортировки + сохранения отдельных частей.
                    firstReadyProcessor.StartProcess(currentChunk);
                    currentChunk = InitNewChunk();
                    currentChunkLength = 0;
                }
            } while (entryReader.Current != null);

            // Исходнеый файл прочитан. Теперь ждем, когда будут обработаны оставшиеся чанки.
            WaitForAllListeners();
        }

        public void RegisterChunkProcessor(IChunkProcessor processor)
        {
            if (processor == null)
                throw new ArgumentNullException(nameof(processor));
            _listeners.Add(processor);
        }
    }
}
