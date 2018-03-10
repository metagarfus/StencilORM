using System;
using System.Collections.Generic;
using StencilORM.Metadata;

namespace StencilORM.Query
{
    public class Insert
    {
        public string TableName { get; private set; }
        public List<Set> Sets { get; private set; } = new List<Set>();

        public Insert(string tableName)
        {
            this.TableName = tableName;
        }

        public Insert Set(params Set[] sets)
        {
            this.Sets.AddRange(sets);
            return this;
        }

        public Insert Set(string name, IExpr value)
        {
            return Set(new Set(name, value));
        }
        public bool Execute(IQueryCompiler compiler, out int rowsAltered, params Value[] parameters)
        {
            return compiler.Execute(this, out rowsAltered, parameters);
        }

        public bool Execute(IConnectionSource source, out int rowsAltered, params Value[] parameters)
        {
            return Execute(source.QueryCompiler, out rowsAltered, parameters);
        }
    }

    public class Insert<T> : Insert
    {
        public Insert()
            : base(MetadataResolver.TableName<T>())
        {
        }

        public Insert(T value, params string[] columns)
           : base(MetadataResolver.TableName<T>())
        {
            Set(MetadataResolver.Sets<T>(value, columns));
        }
    }
}
