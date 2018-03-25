using System;
using System.Reflection;
using System.Collections.Generic;
using StencilORM.Metadata;

namespace StencilORM.Reflection
{
    public interface IReflectionServices
    {
        T GetCustomAttribute<T>(Type type, bool inherit) where T : Attribute;
        T GetCustomAttribute<T> (PropertyInfo info) where T : Attribute;
        IEnumerable<PropertyInfo> GetProperties (Type type);
        object GetValue<T>(KeyValuePair<string, FieldMetadata> item, T value);
        object CreateInstance(Type type);
    }
}
