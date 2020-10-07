using System;
using System.Buffers;

namespace Sort100
{
    public sealed class Chunk: IDisposable
    {
        private readonly ArrayPool<Entry> _parentPool;
        public Entry[] Entries { get; }
        private int _position;
        public bool IsFool => _position >= Entries.Length - 1;
        public int Length => _position;


        public Chunk(ArrayPool<Entry> parentPool, int capacity)
        {
            _parentPool = parentPool;
            Entries = _parentPool.Rent(capacity);
        }


        public void Add(Entry entry)
        {
            Entries[_position] = entry;
            _position++;
        }

        public ReadOnlySpan<Entry> ToSpan()
        {
            return Entries.AsSpan(0, _position);
        }

        public void Dispose()
        {
            _parentPool.Return(Entries);
        }
    }
}