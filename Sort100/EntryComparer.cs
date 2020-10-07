using System;
using System.Collections.Generic;

namespace Sort100
{
    public class EntryComparer : IComparer<Entry>
    {
        public int Compare(Entry x, Entry y)
        {
            // Стратегия сравнения: сначала сравниваем строковые части, если равны - сравниваем численные части
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;

            // Performance optimization: не создаем новых строк, работаем со спанами, построенными над исходной строкой
            var xStringPart = x.Raw.AsSpan().Slice(x.IndexOfStartStringPart);
            var yStringPart = y.Raw.AsSpan().Slice(y.IndexOfStartStringPart);
            var stringPartComparison = xStringPart.CompareTo(yStringPart, StringComparison.Ordinal);
            
            return stringPartComparison != 0 
                ? stringPartComparison 
                : Comparer<int>.Default.Compare(x.NumberPart, y.NumberPart);
        }
    }
}