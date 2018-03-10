using System;
namespace StencilORM
{
    public static class StencilUtils
    {
        public static bool IsNullOrWhiteSpace(string str)
        {
            return string.IsNullOrEmpty(str);
        }
    }
}
