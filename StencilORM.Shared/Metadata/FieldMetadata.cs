using System;
using System.Reflection;
using StencilORM.Annotations;

namespace StencilORM.Metadata
{
    public struct FieldMetadata
    {
        public string ColumnName {get; private set; }

        public DataType DataType {get; private set; }

        public string DefaultValue {get; private set; }

        public int Width {get; private set; }

        public bool CanBeNull {get; private set; }

        public bool Id {get; private set; }

        public bool GeneratedId {get; private set; }

        public string GeneratedIdSequence {get; private set; }

        public bool Foreign {get; private set; }

        public string UnknownEnumName {get; private set; }

        public bool ThrowIfNull {get; private set; }

        public bool Persisted {get; private set; }

        public string Format {get; private set; }

        public bool Unique {get; private set; }

        public bool UniqueCombo {get; private set; }

        public bool Index {get; private set; }

        public bool UniqueIndex {get; private set; }

        public string IndexName {get; private set; }

        public string UniqueIndexName {get; private set; }

        public bool AllowGeneratedIdInsert {get; private set; }

        public string ColumnDefinition {get; private set; }

        public bool Version {get; private set; }

        public string ForeignColumnName {get; private set; }

        public PropertyInfo PropertyInfo {get; private set; }

        public static FieldMetadata? NewFieldMetadata(PropertyInfo info)
        {
            var reflection = StencilContext.ReflectionServices;
            if (reflection == null)
                throw new MissingReflectionException();
            var attribute = reflection.GetCustomAttribute<DatabaseFieldAttribute>(info);
            if (attribute == null)
                return null;
            return new FieldMetadata()
            {
                PropertyInfo = info,
                ColumnName = StencilUtils.IsNullOrWhiteSpace(attribute.ColumnName) ? info.Name : attribute.ColumnName,
                DataType = attribute.DataType,
                DefaultValue = attribute.DefaultValue,
                Width = attribute.Width,
                CanBeNull = attribute.CanBeNull,
                Id = attribute.Id,
                GeneratedId = attribute.GeneratedId,
                GeneratedIdSequence = attribute.GeneratedIdSequence,
                Foreign = attribute.Foreign,
                UnknownEnumName = attribute.UnknownEnumName,
                ThrowIfNull = attribute.ThrowIfNull,
                Persisted = attribute.Persisted,
                Format = attribute.Format,
                Unique = attribute.Unique,
                UniqueCombo = attribute.UniqueCombo,
                Index = attribute.Index,
                UniqueIndex = attribute.UniqueIndex,
                IndexName = attribute.IndexName,
                UniqueIndexName = attribute.UniqueIndexName,
                AllowGeneratedIdInsert = attribute.AllowGeneratedIdInsert,
                ColumnDefinition = attribute.ColumnDefinition,
                Version = attribute.Version,
                ForeignColumnName = attribute.ForeignColumnName,
            };
        }
    }
}
