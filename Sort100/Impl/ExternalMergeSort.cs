using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using Sort100.Interfaces;
using Sort100.Params;

namespace Sort100.Impl
{
    internal sealed class ExternalMergeSort : IExternalMergeSort
    {
        private readonly IOParams _ioParams;
        private readonly AlgParams _algParams;
        private readonly IComparer<Entry> _entryComparer;
        private readonly IIOStreamsFactory _streamsFactory;
        
        public ExternalMergeSort(IOParams ioParams, AlgParams algParams, IComparer<Entry> entryComparer, 
            IIOStreamsFactory streamsFactory)
        {
            _ioParams = ioParams ?? throw new ArgumentNullException(nameof(ioParams));
            _algParams = algParams ?? throw new ArgumentNullException(nameof(algParams));
            _entryComparer = entryComparer ?? throw new ArgumentNullException(nameof(entryComparer));
            _streamsFactory = streamsFactory ?? throw new ArgumentNullException(nameof(streamsFactory));
        }

        public void Sort(EntryReader[] sortedChunksReaders)
        {
            static void WithReaders(ReadOnlySpan<EntryReader> readers, Action<EntryReader> action)
            {
                foreach (var reader in readers)
                    action(reader);
            }
            
            var readers = sortedChunksReaders.ToArray();
            using var output = _streamsFactory.GetForWrite(_ioParams.ResultFilePath);
            using var entryWriter = new EntryWriter(output, _algParams.Encoding, _ioParams.WriteBufferSizeInBytes);
            
            // Инициализируем читателей отсортированных пачек объектов первыми значениями
            WithReaders(readers, reader =>
            {
                reader.ReadNext();
            });
            // Инициализируем бинарное дерево
            var minHeap = new MinHeap(sortedChunksReaders, _entryComparer);
            // Извлекаем минимальные значения и пишем их в результирующий файл
            long totalEntriesWritten = 0;
            while (minHeap.TryGetMin(out var minEntry))
            {
                entryWriter.Write(minEntry);
                /*totalEntriesWritten++;
                if (totalEntriesWritten % 1000000 == 0)
                {
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                    GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                    GC.Collect();  
                }*/
            }
            // Закрываем читающие потоки
            WithReaders(readers, reader => reader.Dispose());
        }

    }
}