using System;
using System.Collections.Generic;

namespace StencilORM.Metadata
{
    public struct TableMetadata
    {
        public string TableName { get; protected set; }
        public IDictionary<string, PropertyInfo> Fields { get; protected set; }

        public TableMetadata()
        {
        }
    }
}
