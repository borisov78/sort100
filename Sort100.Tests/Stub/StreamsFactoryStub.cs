using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using Sort100.Interfaces;

namespace Sort100.Tests.Stub
{
    internal sealed class StreamsFactoryStub : IIOStreamsFactory
    {
        private readonly ConcurrentDictionary<string, string> _forReads = new ConcurrentDictionary<string, string>();
        private readonly ConcurrentDictionary<string, MemoryStream> _forWrites = new ConcurrentDictionary<string, MemoryStream>();

        private sealed class UnclosableMemoryStream : MemoryStream
        {
            public override void Close() { }
            protected override void Dispose(bool disposing) { }
        }
        
        
        public Stream GetForRead(string identifier, bool useCompression = false)
        {
            if (_forReads.TryGetValue(identifier, out var content))
            {
                var result = new MemoryStream();
                result.Write(Encoding.Default.GetBytes(content));
                result.Position = 0;
                return result;
            }

            if (_forWrites.TryGetValue(identifier, out var contentFromWrites))
            {
                contentFromWrites.Position = 0;
                return contentFromWrites;
            }
            
            throw new ArgumentException($"File '{identifier}' not found.");
        }

        public Stream GetForWrite(string identifier, bool useCompression = false)
        {
            var result = new UnclosableMemoryStream();
            _forWrites.TryAdd(identifier, result);
            return result;
        }

        public void SetUpInput(string identifier, string value)
        {
            _forReads.TryAdd(identifier, value);
        }
        
        public EntryReader[] SetUpInputs(params string[] inputs)
        {
            for (var i = 0; i < inputs.Length; i++)
                SetUpInput(i.ToString(), inputs[i]);
            return GetReaders();
        }

        private EntryReader[] GetReaders()
        {
            return _forReads.Select(input => new EntryReader(GetForRead(input.Key))).ToArray();
        }

        public void AssertOutput(string identifier, string expected)
        {
            if (!_forWrites.TryGetValue(identifier, out var stream))
                throw new ArgumentException($"File '{identifier}' not found.");
            var actual = Encoding.Default.GetString(stream.ToArray());
            if (string.Compare(actual, expected, StringComparison.Ordinal) != 0)
                throw new InvalidOperationException(
                    $"Strings is not equals. Actual:{Environment.NewLine}{actual}{Environment.NewLine}Expected:{Environment.NewLine}{expected}");
        }
    }
}