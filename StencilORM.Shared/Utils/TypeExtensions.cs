using System;
namespace StencilORM.Utils
{
    public static class TypeExtensions
    {
        public static bool Is<T>(this object value) where T : struct
        {
            var type = value.GetType();
            return type == typeof(T) || type == typeof(T?);
        }
    }
}
