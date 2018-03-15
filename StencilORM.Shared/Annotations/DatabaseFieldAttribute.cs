using System;
using StencilORM.Metadata;

namespace StencilORM.Annotations
{
    public class DatabaseFieldAttribute : Attribute
    {
        public string ColumnName { get; set; } = "";

        public DataType DataType { get; set; } = DataType.UNKNOWN;

        public string DefaultValue { get; set; } = "";

        public int Width { get; set; } = 0;

        public bool CanBeNull { get; set; } = true;

        public bool Id { get; set; } = false;

        public bool GeneratedId { get; set; } = false;

        public string GeneratedIdSequence { get; set; } = "";

        public bool Foreign { get; set; } = false;

        public string UnknownEnumName { get; set; } = "";

        public bool ThrowIfNull { get; set; } = false;

        public bool Persisted { get; set; } = true;

        public string Format { get; set; } = "";

        public bool Unique { get; set; } = false;

        public bool UniqueCombo { get; set; } = false;

        public bool Index { get; set; } = false;

        public bool UniqueIndex { get; set; } = false;

        public string IndexName { get; set; } = "";

        public string UniqueIndexName { get; set; } = "";

        public bool AllowGeneratedIdInsert { get; set; } = false;

        public string ColumnDefinition { get; set; } = "";

        public bool Version { get; set; } = false;

        public string ForeignColumnName { get; set; } = "";

        public DatabaseFieldAttribute()
        {
        }
    }
}
