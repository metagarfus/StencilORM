using System;
using System.Collections.Generic;
namespace StencilORM.Utils
{
    public static class ListExtensions
    {
        public static void ListForEach<T>(this IList<T> list, Action<T> action)
        {
            if (list == null)
                return;
            foreach (var item in list)
                action(item);
        }
    }
}
