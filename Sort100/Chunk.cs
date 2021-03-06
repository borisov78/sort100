﻿using System;
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
            // Для предотвращения фрагментации LOH берем буфера из пула
            Entries = _parentPool.Rent(capacity);
        }

        public void Add(Entry entry)
        {
            Entries[_position] = entry;
            _position++;
        }

        public ReadOnlySpan<Entry> ToSpan()
        {
            // Буфер может быть заполнен не полностью - поэтому на дальнейшую обработку отдаем "окно",
            // сделанное по актуальному размеру
            return Entries.AsSpan(0, _position);
        }

        public void Dispose()
        {
            _parentPool.Return(Entries);
        }
    }
}