using System;
using System.Collections.Generic;
using System.Reflection;
using StencilORM.Metadata;

namespace StencilORM.Reflection
{
    public class ReflectionServices : IReflectionServices
    {
        public object CreateInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public T GetCustomAttribute<T>(Type type, bool inherit) where T : Attribute
        {
            return type.GetTypeInfo().GetCustomAttribute<T>(false);
        }

        public T GetCustomAttribute<T> (PropertyInfo info) where T : Attribute
        {
            return info.GetCustomAttribute<T>();
        }

        public IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return type.GetRuntimeProperties();
        }

        public object GetValue<T>(KeyValuePair<string, FieldMetadata> item, T value)
        {
            return item.Value.PropertyInfo.GetValue(value);
        }
    }
}
