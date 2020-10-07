using System;
using System.Runtime;
using Sort100.Interfaces;
using Sort100.Params;

namespace Sort100
{
    /// <summary>
    /// Объект-контейнер, имеющий все необходимое для выполнения всех фаз сортировки.
    /// </summary>
    public sealed class Sort
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly AlgParams _algParams;
        private readonly ISourceFileReader _sourceFileReader;
        private readonly IExternalMergeSort _externalMergeSort;
        private readonly ISortedChunksStorage _sortedChunksStorage;
        
        public Sort(IServiceProvider serviceProvider, AlgParams algParams, 
            ISourceFileReader sourceFileReader, ISortedChunksStorage sortedChunksStorage, IExternalMergeSort externalMergeSort)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _algParams = algParams ?? throw new ArgumentNullException(nameof(algParams));
            _sourceFileReader = sourceFileReader ?? throw new ArgumentNullException(nameof(sourceFileReader));
            _externalMergeSort = externalMergeSort ?? throw new ArgumentNullException(nameof(externalMergeSort));
            _sortedChunksStorage = sortedChunksStorage ?? throw new ArgumentNullException(nameof(sortedChunksStorage));
        }
        
        public void DoSort()
        {
            Console.WriteLine($"Start sorting. GC.IsServer: {GCSettings.IsServerGC} GC.LatencyMode: {GCSettings.LatencyMode}");
            Console.WriteLine("Init chunk processors..");
            for (var i = 0; i < _algParams.WorkersCount; i++)
            {
                var processor = (IChunkProcessor)_serviceProvider.GetService(typeof(IChunkProcessor));
                _sourceFileReader.RegisterChunkProcessor(processor);
            }
            Console.WriteLine("Split and sort chunks..");
            _sourceFileReader.ReadAndSplitChunks();
            Console.WriteLine("Init sorted chunks readers..");
            var sortedChunksReaders = _sortedChunksStorage.GetStoredChunkReaders();
            Console.WriteLine("Merge sort..");
            _externalMergeSort.Sort(sortedChunksReaders);
        }
    }
}