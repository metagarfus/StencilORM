using System;
using System.Reflection;
using System.Collections.Generic;
using StencilORM.Annotations;
using StencilORM.Query;
using System.Linq;

namespace StencilORM.Metadata
{
    public static class MetadataResolver
    {
        private static IDictionary<Type, TableMetadata> Tables { get; } = new Dictionary<Type, TableMetadata>();

        public static TableMetadata EnsureMetadata(Type type)
        {
            if (Tables.TryGetValue(type, out TableMetadata metadata))
                return metadata;
            var newMetadata = TableMetadata.NewTableMetadata(type);
            if (newMetadata == null)
                throw new Exception(string.Format("Type '{0}' is not a valid table type. " +
                                                  "Did you forget a DatabaseTable annotation?", type.Name));
            metadata = newMetadata.Value;
            Tables[type] = metadata;
            return metadata;
        }

        public static string TableName<T>()
        {
            var metadata = EnsureMetadata(typeof(T));
            return metadata.TableName;
        }

        internal static Set[] Sets<T>(T value, string[] columns)
        {
            var metadata = EnsureMetadata(typeof(T));
            if (columns == null || columns.Length == 0)
                return FullSets(metadata, value);
            else
                return PartialSets(metadata, value, columns);
        }

        private static Set[] PartialSets<T>(TableMetadata metadata, T value, string[] columns)
        {
            IList<Set> sets = new List<Set>();
            foreach (var item in metadata.Fields)
            {
                if (!columns.Contains(item.Key))
                    continue;
                sets.Add(NewSet(value, item));
            }
            return sets.ToArray();
        }

        private static Set[] FullSets<T>(TableMetadata metadata, T value)
        {
            Set[] sets = new Set[metadata.Fields.Count];
            int n = 0;
            foreach (var item in metadata.Fields)
            {
                sets[n++] = NewSet(value, item);
            }
            return sets;
        }

        private static Set NewSet<T>(T value, KeyValuePair<string, FieldMetadata> item)
        {
            return new Set(item.Key, new Literal(item.Value.PropertyInfo.GetValue(value)));
        }

        internal static IExpr IdentityExpr<T>(T value)
        {
            var metadata = EnsureMetadata(typeof(T));
            IAppendableExpr result = Expr.Empty;
            foreach (var item in metadata.Keys)
            {
                var keyName = (Variable) item.Key;
                var keyValue = new Literal(item.Value.PropertyInfo.GetValue(value));
                result = result.And(Expr.Eq(keyName, keyValue));
            }
            return result;
        }
    }
}
