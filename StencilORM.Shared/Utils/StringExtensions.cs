using System;
using System.Collections.Generic;
using System.Text;

namespace StencilORM.Utils
{
    public static class StringExtensions
    {
        public static StringBuilder AppendJoin<T>(this StringBuilder builder, string separator,
                                         IEnumerable<T> enumerable, Action<StringBuilder, T> generator)
        {
            int n = 0;
            foreach (var item in enumerable)
            {
                if (n != 0)
                    builder.Append(", ");
                generator(builder, item);
                n++;
            }
            return builder;
        }
        
        public static bool IsEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
    }
}
