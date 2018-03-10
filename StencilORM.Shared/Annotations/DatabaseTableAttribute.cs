using System;
namespace StencilORM.Annotations
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class DatabaseTableAttribute : Attribute
    {
        public string TableName { get; set; } = "";

        public DatabaseTableAttribute()
        {
        }
    }
}
