using System;
using System.IO;
using System.Text;

namespace Sort100
{
    internal sealed class EntryWriter : IDisposable
    {
        private readonly StreamWriter _streamWriter;
       
        public EntryWriter(Stream innerStream, Encoding encoding, int bufferSize)
        {
            _streamWriter = new StreamWriter(innerStream, encoding, bufferSize);
        }

        public void Write(Entry entry)
        {
            _streamWriter.WriteLine(entry.Raw);
        }
        
        public void WriteAll(Chunk chunk)
        {
            foreach (var entry in chunk.ToSpan())
                Write(entry);
            _streamWriter.Flush();
        }
        
        public void Dispose()
        {
            _streamWriter?.Dispose();
        }
    }
}