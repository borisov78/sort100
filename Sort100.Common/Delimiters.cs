using System;
using System.Text;

namespace Sort100.Common
{
    public class Delimiters
    {
        public const string PartsDelimiter = ".";
        public const string WordDelimiter = " ";
        public static readonly string LineDelimiter = Environment.NewLine;
        
        public readonly byte[] PartsDelimiterBytes;
        public readonly byte[] WordDelimiterBytes;
        public readonly byte[] LineDelimiterBytes;

        public Delimiters(Encoding encoding)
        {
            PartsDelimiterBytes = encoding.GetBytes(PartsDelimiter);
            WordDelimiterBytes = encoding.GetBytes(WordDelimiter);
            LineDelimiterBytes = encoding.GetBytes(LineDelimiter);
        }
    }
}