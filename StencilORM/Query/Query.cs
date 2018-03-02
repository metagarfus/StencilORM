using System;
using StencilORM.Transaction;
using System.Collections.Generic;

namespace StencilORM.Query
{
    public class Query
    {
        public ITransaction Transaction { get; private set; }
        public string TableName { get; set; }
        public string Alias { get; set; }
        public Type SourceType { get; set; }
        public Type ResultType { get; set; }
        public List<Column> SelectedColumns { get; set; } = new List<Column>();

        public Query(string tableName)
            : this(null, tableName, null, null, null)
        {
        }


        public Query(string tableName, string alias)
            : this(null, tableName, alias, null, null)
        {
        }

        public Query(string tableName, Type sourceType, Type resultType)
            : this(null, tableName, null, sourceType, resultType)
        {
        }

        public Query(string tableName, string alias, Type sourceType, Type resultType)
            : this(null, tableName, alias, sourceType, resultType)
        {
        }

        public Query(ITransaction transaction, string tableName, string alias, Type sourceType, Type resultType)
        {
            this.Transaction = transaction;
            this.TableName = tableName;
            this.Alias = alias;
            this.SourceType = sourceType;
            this.ResultType = resultType;
        }

        public Query Select(params Column[] columns)
        {
            SelectedColumns.AddRange(columns);
            return this;
        }

        public Query Select(params IExpr[] columns)
        {
            foreach(var item in columns)
                SelectedColumns.Add(new Column(item));
            return this;
        }

        public Query Select(params string[] columns)
        {
            foreach (var item in columns)
                SelectedColumns.Add(new Column(item));
            return this;
        }
    }

    public class Query<T> : Query
    {
        public Query(string tableName) 
            : base(null, tableName, null, typeof(T), null)
        {
        }


        public Query(string tableName, string alias) 
            : base(null, tableName, alias, typeof(T), null)
        {
        }
    }
}
