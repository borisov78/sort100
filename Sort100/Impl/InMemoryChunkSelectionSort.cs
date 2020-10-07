using System;
using System.Collections.Generic;
using Sort100.Interfaces;

namespace Sort100.Impl
{
    /// <summary>
    /// Вариация на тему сортировки выбором для одного чанка.
    /// Имеет квадратичную сложность и для больших файлов неприменимо.
    /// Только для экспериментальныхэ целей. 
    /// </summary>
    internal sealed class InMemoryChunkSelectionSort : IInMemoryChunkSort
    {
        private readonly IComparer<Entry> _entryComparer;
        
        public InMemoryChunkSelectionSort(IComparer<Entry> entryComparer)
        {
            _entryComparer = entryComparer ?? throw new ArgumentNullException(nameof(entryComparer));
        }
        
        public void Sort(Chunk chunk)
        {
            void Swap(int index1, int index2)
            {
                var tmp = chunk.Entries[index1];
                chunk.Entries[index1] = chunk.Entries[index2];
                chunk.Entries[index2] = tmp;
            }
            
            var lowBoundIndex = 0;
            var hiBoundIndex = chunk.Length - 1;

            while (lowBoundIndex < hiBoundIndex)
            {
                var maxEntryIndex = lowBoundIndex;
                var minEntryIndex = hiBoundIndex;
                
                for (var curIndex = lowBoundIndex; curIndex <= hiBoundIndex; curIndex++)
                {
                    if (_entryComparer.Compare(chunk.Entries[maxEntryIndex], chunk.Entries[curIndex]) > 0)
                        maxEntryIndex = curIndex;
                    else if (_entryComparer.Compare(chunk.Entries[minEntryIndex], chunk.Entries[curIndex]) < 0)
                        minEntryIndex = curIndex;
                }
                
                // max
                Swap(lowBoundIndex, maxEntryIndex);
                
                // min
                Swap(hiBoundIndex, minEntryIndex);
                
                // уменьшаем окно
                lowBoundIndex++;
                hiBoundIndex--;
            }
        }
    }
}