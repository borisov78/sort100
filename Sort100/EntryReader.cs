using System;
using System.IO;

namespace Sort100
{
    public sealed class EntryReader : IDisposable
    {
        private readonly Stream _innerStream;
        private readonly StreamReader _streamReader; 
        
        public EntryReader(Stream innerStream, bool closeInnerStream = false)
        {
            if (closeInnerStream)
                _innerStream = innerStream;
            _streamReader = new StreamReader(innerStream, true);
        }
        
        public int ReadNext()
        {
            var line = _streamReader.ReadLine();
            if (string.IsNullOrEmpty(line))
            {
                Current = null;
                return 0;
            }

            Current = Entry.Parse(line);
            return line.Length;
        }

        public Entry Current { get; private set; }
        
        public void Dispose()
        {
            _streamReader?.Dispose();
            _innerStream?.Dispose();
        }
    }
}