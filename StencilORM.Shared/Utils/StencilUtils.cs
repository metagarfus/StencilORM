using System;
using System.Linq;

namespace StencilORM.Utils
{
    public static class StencilUtils
    {
        public static bool IsNullOrWhiteSpace(string str)
        {
            return str.IsEmpty() || str.Trim().IsEmpty();
        }
    }
}
