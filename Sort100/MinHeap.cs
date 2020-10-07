using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sort100
{
    /// <summary>
    /// Бинарное дерево (Min Heap) для поиска минимальных значений среди множеств упорядоченных объектов
    /// с последовательным доступом (Chunk). Сложность поиска O(log n).
    /// См. https://www.geeksforgeeks.org/binary-heap/ 
    /// </summary>
    internal sealed class MinHeap
    {
        private readonly IComparer<Entry> _comparer;
        private readonly EntryReader[] _readers;
        private int _size;

        public MinHeap(EntryReader[] readers, IComparer<Entry> comparer)
        {
            _comparer = comparer;
            _readers = new EntryReader[readers.Length];

            foreach (var reader in readers) 
                Add(reader);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetLeftChildIndex(int elementIndex) => 2 * elementIndex + 1;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetRightChildIndex(int elementIndex) => 2 * elementIndex + 2;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetParentIndex(int elementIndex) => (elementIndex - 1) / 2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasLeftChild(int elementIndex) => GetLeftChildIndex(elementIndex) < _size;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasRightChild(int elementIndex) => GetRightChildIndex(elementIndex) < _size;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsRoot(int elementIndex) => elementIndex == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Entry GetLeftChild(int elementIndex) => _readers[GetLeftChildIndex(elementIndex)].Current;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Entry GetRightChild(int elementIndex) => _readers[GetRightChildIndex(elementIndex)].Current;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Entry GetParent(int elementIndex) => _readers[GetParentIndex(elementIndex)].Current;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Swap(int index1, int index2)
        {
            var temp = _readers[index1];
            _readers[index1] = _readers[index2];
            _readers[index2] = temp;
        }
        
        private EntryReader Pop()
        {
            if (_size == 0)
                return null;

            var result = _readers[0];
            Swap(0, _size - 1);
            _size--;

            ReBalanceDown();

            return result;
        }

        private void Add(EntryReader entryReader)
        {
            if (_size == _readers.Length)
                throw new IndexOutOfRangeException();

            _readers[_size] = entryReader;
            _size++;

            ReBalanceUp();
        }

        public bool TryPopAndWriteMin(EntryWriter writer)
        {
            var readerWithMinEntry = Pop();
            if (readerWithMinEntry == null)
                return false;
            
            var minEntry = readerWithMinEntry.Current;
            
            readerWithMinEntry.ReadNext();
            if (readerWithMinEntry.Current != null)
                Add(readerWithMinEntry);
            
            if (minEntry != null)
                writer.Write(minEntry);
            return minEntry != null;
        }

        private void ReBalanceDown()
        {
            var index = 0;
            while (HasLeftChild(index))
            {
                var smallerIndex = GetLeftChildIndex(index);
                
                if (HasRightChild(index) && _comparer.Compare(GetRightChild(index), GetLeftChild(index)) < 0)
                    smallerIndex = GetRightChildIndex(index);

                if (_comparer.Compare(_readers[smallerIndex].Current, _readers[index].Current) >= 0)
                    break;

                Swap(smallerIndex, index);
                index = smallerIndex;
            }
        }

        private void ReBalanceUp()
        {
            var index = _size - 1;
            while (!IsRoot(index) && _comparer.Compare(_readers[index].Current, GetParent(index)) < 0)
            {
                var parentIndex = GetParentIndex(index);
                Swap(parentIndex, index);
                index = parentIndex;
            }
        }
    }
}