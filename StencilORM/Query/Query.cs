using System;
using StencilORM.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace StencilORM.Query
{
    public class Query
    {
        // public ITransaction Transaction { get; private set; }
        public string TableName { get; private set; }
        public string Alias { get; set; }
        // public Type SourceType { get; set; }
        //public Type ResultType { get; set; }
        public List<Column> Columns { get; private set; } = new List<Column>();
        public IAppendableExpr WhereExpr { get; private set; }
        public List<Join> Joins { get; private set; } = new List<Join>();
        public List<Column> GroupByColumns { get; private set; } = new List<Column>();
        public IAppendableExpr GroupWhereExpr { get; private set; }
        public List<OrderBy> OrderByExprs { get; private set; } = new List<OrderBy>();
        public int RowsLimit { get; set; }
        public int RowsOffset { get; set; }
        public List<string> ParamList { get; set; } = new List<string>();

        public Query(string tableName)
            : this(/*null, */tableName, null/*, null, null*/)
        {
        }


        /* public Query(string tableName, string alias)
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
         }*/

        public Query(/*ITransaction transaction, */string tableName, string alias/*, Type sourceType, Type resultType*/)
        {
            //this.Transaction = transaction;
            this.TableName = tableName;
            this.Alias = alias;
            /*this.SourceType = sourceType;
            this.ResultType = resultType;*/
        }

        public static implicit operator Query(string name)
        {
            return new Query(name);
        }

        public Query Select(params Column[] columns)
        {
            Columns.AddRange(columns);
            return this;
        }

        public Query Select(params IExpr[] columns)
        {
            foreach (var item in columns)
                Columns.Add(new Column(item));
            return this;
        }

        public Query Select(params string[] columns)
        {
            foreach (var item in columns)
                Columns.Add(new Column(item));
            return this;
        }

        public Query Where(Expr expr)
        {
            this.WhereExpr = this.WhereExpr.And(expr);
            return this;
        }

        public Query Where(string expr)
        {
            this.WhereExpr = this.WhereExpr.And(Expr.Parse(expr));
            return this;
        }

        public Query WhereOr(Expr expr)
        {
            this.WhereExpr = this.WhereExpr.Or(expr);
            return this;
        }

        public Query WhereOr(string expr)
        {
            this.WhereExpr = this.WhereExpr.Or(Expr.Parse(expr));
            return this;
        }

        public Query Join(Join join)
        {
            this.Joins.Add(join);
            return this;
        }

        public Query Join(Query inner, IExpr[] outerKeys, IExpr[] innerKeys, bool leftJoin)
        {
            return Join(new Join(inner, outerKeys, innerKeys, leftJoin));
        }

        public Query Join(Query inner, string[] outerKeys, string[] innerKeys, bool leftJoin)
        {
            var outerKeysExprs = outerKeys.Select(x => (IExpr)new Variable(x)).ToArray();
            var innerKeysExprs = innerKeys.Select(x => (IExpr)new Variable(x)).ToArray();
            return Join(new Join(inner, outerKeysExprs, innerKeysExprs, leftJoin));
        }

        public Query InnerJoin(Query inner, IExpr[] outerKeys, IExpr[] innerKeys)
        {
            return Join(inner, outerKeys, innerKeys, false);
        }

        public Query InnerJoin(Query inner, string[] outerKeys, string[] innerKeys)
        {
            return Join(inner, outerKeys, innerKeys, false);
        }

        public Query LeftJoin(Query inner, IExpr[] outerKeys, IExpr[] innerKeys)
        {
            return Join(inner, outerKeys, innerKeys, true);
        }

        public Query LeftJoin(Query inner, string[] outerKeys, string[] innerKeys)
        {
            return Join(inner, outerKeys, innerKeys, true);
        }

        public Query GroupBy(params Column[] columns)
        {
            GroupByColumns.AddRange(columns);
            return this;
        }

        public Query GroupBy(params IExpr[] columns)
        {
            foreach (var item in columns)
                GroupByColumns.Add(new Column(item));
            return this;
        }

        public Query GroupBy(params string[] columns)
        {
            foreach (var item in columns)
                GroupByColumns.Add(new Column(item));
            return this;
        }

        public Query Having(Expr expr)
        {
            this.GroupWhereExpr = this.GroupWhereExpr.And(expr);
            return this;
        }

        public Query Having(string expr)
        {
            this.GroupWhereExpr = this.GroupWhereExpr.And(Expr.Parse(expr));
            return this;
        }

        public Query HavingOr(Expr expr)
        {
            this.GroupWhereExpr = this.GroupWhereExpr.Or(expr);
            return this;
        }

        public Query HavingOr(string expr)
        {
            this.GroupWhereExpr = this.GroupWhereExpr.Or(Expr.Parse(expr));
            return this;
        }

        public Query OrderBy(params OrderBy[] orderByExprs)
        {
            OrderByExprs.AddRange(orderByExprs);
            return this;
        }

        public Query OrderBy(bool descending, params IExpr[] orderByExprs)
        {
            foreach (var item in orderByExprs)
                OrderByExprs.Add(new OrderBy(descending, item));
            return this;
        }

        public Query OrderBy(bool descending, params string[] orderByExprs)
        {
            foreach (var item in orderByExprs)
                OrderByExprs.Add(new OrderBy(descending, item));
            return this;
        }

        public Query Limit(int limit)
        {
            this.RowsLimit = limit;
            return this;
        }

        public Query Offset(int offset)
        {
            this.RowsOffset = offset;
            return this;
        }

        public Query Params(params string[] paramList)
        {
            this.ParamList.AddRange(paramList);
            return this;
        }


        public IEnumerable<T> Execute<T>(IQueryCompiler compiler, params Value[] parameters)
        {
            return compiler.Execute<T>(this, parameters);
        }

        public dynamic Execute(IQueryCompiler compiler, Type type, params Value[] parameters)
        {
            return compiler.Execute(type, this, parameters);
        }

        public IEnumerable<string[]> Execute(IQueryCompiler compiler, params Value[] parameters)
        {
            return compiler.Execute(this, parameters);
        }

        public IEnumerable<T> Execute<T>(IConnectionSource source, params Value[] parameters)
        {
            return Execute<T>(source.QueryCompiler, parameters);
        }

        public dynamic Execute(IConnectionSource source, Type type, params Value[] parameters)
        {
            return Execute(source.QueryCompiler, type, parameters);
        }

        public IEnumerable<string[]> Execute(IConnectionSource source, params Value[] parameters)
        {
            return Execute(source.QueryCompiler, parameters);
        }

        public IEnumerable<T> Execute<T>(params Value[] parameters)
        {
            return Execute<T>(StencilContext.ConnectionSource, parameters);
        }

        public dynamic Execute(Type type, params Value[] parameters)
        {
            return Execute(StencilContext.ConnectionSource, type, parameters);
        }

        public IEnumerable<string[]> Execute(params Value[] parameters)
        {
            return Execute(StencilContext.ConnectionSource, parameters);
        }
    }

    public class Query<T> : Query
    {
        /*public Query(string tableName) 
            : base(null, tableName, null, typeof(T), null)
        {
        }


        public Query(string tableName, string alias) 
            : base(null, tableName, alias, typeof(T), null)
        {
        }*/

        public Query(string alias)
            : base(MetadataResolver.TableName<T>(), alias)
        {
        }
    }
}
