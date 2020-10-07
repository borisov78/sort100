using System.Text;

namespace Sort100.Tests.Helpers
{
    internal static class EntryHelper
    {
        public static string Generate(params int[] entries)
        {
            var sb = new StringBuilder();
            foreach (var entry in entries)
                sb.Append(entry.ToString()).Append(". 0").AppendLine();
            return sb.ToString();
        }
    }
}