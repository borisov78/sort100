using System;
using System.Threading;
using System.Threading.Tasks;
using Sort100.Interfaces;

namespace Sort100.Impl
{
    internal sealed class ChunkProcessor : IChunkProcessor
    {
        private readonly AutoResetEvent _ready = new AutoResetEvent(true);
        public WaitHandle IsReady => _ready;
        private readonly ISortedChunksStorage _sortedChunksStorage;
        private readonly IInMemoryChunkSort _inMemoryChunkSort;

        public ChunkProcessor(ISortedChunksStorage sortedChunksStorage, IInMemoryChunkSort inMemoryChunkSort)
        {
            _sortedChunksStorage = sortedChunksStorage ?? throw new ArgumentNullException(nameof(sortedChunksStorage));
            _inMemoryChunkSort = inMemoryChunkSort ?? throw new ArgumentNullException(nameof(inMemoryChunkSort));
        }

        public void StartProcess(Chunk chunk)
        {
            _ready.Reset();
            // Запускаем обработку пачки объектов в отдельном потоке.
            // Когда обработка закончится - сигнализируем о готовности к приему следующей порции объектов.
            Task.Factory.StartNew(() =>
            {
                try
                {
                    _inMemoryChunkSort.Sort(chunk);
                    _sortedChunksStorage.StoreSortedChunk(chunk);
                    chunk.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    _ready.Set();
                }
            }, TaskCreationOptions.LongRunning);
        }
    }
}