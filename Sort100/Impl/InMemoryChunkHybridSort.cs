using System;
using System.Collections.Generic;
using Sort100.Interfaces;

namespace Sort100.Impl
{
    /// <summary>
    /// In memory сортировка пачки объектов с помощью стандартной гибридной сортировки netcore (для маленьких множеств
    /// - insertion sort, для больших - heapsort). Сложность O(n log(n)).
    /// </summary>
    internal sealed class InMemoryChunkHybridSort : IInMemoryChunkSort
    {
        private readonly IComparer<Entry> _entryComparer;
        
        public InMemoryChunkHybridSort(IComparer<Entry> entryComparer)
        {
            _entryComparer = entryComparer ?? throw new ArgumentNullException(nameof(entryComparer));
        }
        
        public void Sort(Chunk chunk)
        {
            Array.Sort(chunk.Entries, 0, chunk.Length, _entryComparer);
        }
    }
}