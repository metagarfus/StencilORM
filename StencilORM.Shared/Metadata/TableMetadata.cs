using System;
using System.Collections.Generic;
using System.Reflection;
using StencilORM.Annotations;
using StencilORM.Utils;

namespace StencilORM.Metadata
{
    public struct TableMetadata
    {
        public Type TableType { get; private set; }
        public string TableName { get; private set; }
        public IDictionary<string, FieldMetadata> Fields { get; private set; }
        public IDictionary<string, FieldMetadata> Keys { get; private set; }

        public object CreateIntance()
        {
            return StencilContext.ReflectionServices.CreateInstance(TableType);
        }

        public static TableMetadata? NewTableMetadata(Type type)
        {
            var reflection = StencilContext.ReflectionServices;
            if (reflection == null)
                throw new MissingReflectionException();
            var table = reflection.GetCustomAttribute<DatabaseTableAttribute>(type, false);
            if (table == null)
                return null;
            IDictionary<string, FieldMetadata> fields = new Dictionary<string, FieldMetadata>();
            IDictionary<string, FieldMetadata> keys = new Dictionary<string, FieldMetadata>();
            foreach (PropertyInfo info in reflection.GetProperties(type))
            {
                var newField = FieldMetadata.NewFieldMetadata(info);
                if (newField == null || StencilUtils.IsNullOrWhiteSpace(newField.Value.ColumnName))
                    continue;
                var field = newField.Value;
                fields[field.ColumnName] = field;
                if (field.Id)
                    keys[field.ColumnName] = field;
            }
            return new TableMetadata
            {
                TableName = StencilUtils.IsNullOrWhiteSpace(table.TableName) ? type.Name : table.TableName,
                Fields = fields,
                Keys = keys
            };
        }


    }
}
