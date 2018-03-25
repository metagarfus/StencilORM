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
            var attributes = type.GetCustomAttributes(false);
            return GetAttribute<T>(attributes);
        }

        public T GetCustomAttribute<T> (PropertyInfo info) where T : Attribute
        {
            var attributes = info.GetCustomAttributes(false);
            return GetAttribute<T>(attributes);
        }

        public IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return type.GetProperties();
        }

        public object GetValue<T>(KeyValuePair<string, FieldMetadata> item, T value)
        {
            return item.Value.PropertyInfo.GetValue(value, null);
        }

        private static T GetAttribute<T>(object[] attributes) where T : Attribute
        {
            if (attributes == null)
                return null;
            foreach (var item in attributes)
            {
                var attribute = item as T;
                if (attribute != null)
                    return attribute;
            }
            return null;
        }
    }
}
