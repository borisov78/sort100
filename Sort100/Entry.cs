using System;
using Sort100.Common;

namespace Sort100
{
    public sealed class Entry
    {
        public int NumberPart { get; }
        
        public int IndexOfStartStringPart { get; }

        public string Raw { get; }

        private Entry(string raw, int numberPart, int indexOfStartStringPath)
        {
            NumberPart = numberPart;
            IndexOfStartStringPart = indexOfStartStringPath;
            Raw = raw;
        }

        public static Entry Parse(string line)
        {
            var lineSpan = line.AsSpan();
            var delimiterIndex = lineSpan.IndexOf(Delimiters.PartsDelimiter);
            if (delimiterIndex < 1)
                throw new ArgumentException($"Incorrect input string: '{new string(line)}'");
            var numberPart = lineSpan.Slice(0, delimiterIndex);
            return new Entry(line, int.Parse(numberPart), delimiterIndex + 2);
        }
    }
}