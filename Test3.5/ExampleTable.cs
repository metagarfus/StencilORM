using System;
using StencilORM.Annotations;

namespace Test
{
    [DatabaseTable(TableName = "MyTable")]
    public class ExampleTable
    {
        [DatabaseField(Id = true)]
        public Guid Key { get; set; }

        [DatabaseField(ColumnName = "Description")]
        public string Name { get; set; }

        [DatabaseField]
        public DateTime Date { get; set; }

        public ExampleTable()
        {
        }
    }
}
