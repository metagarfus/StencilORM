using System;
using StencilORM.Transaction;
using System.Collections.Generic;
using System.Linq;
using StencilORM.Metadata;
using StencilORM.Compilers;

namespace StencilORM.Queries
{
    public class QuerySource
    {
        public string TableName { get; }
        public Query TableQuery { get; }

        public QuerySource(string name)
        {
            this.TableName = name;
        }

        public QuerySource(Query query)
        {
            this.TableQuery = query;
        }
    }

    public class Query
    {
        // public ITransaction Transaction { get; private set; }
        public QuerySource TableSource { get; private set; }
        //public Query TableQuery { get; private set; }
        public string Alias { get; set; }
        //public Type SourceType { get; set; }
        //public Type ResultType { get; set; }
        public List<Column> Columns { get; private set; } = new List<Column>();
        public IAppendableExpr WhereExpr { get; private set; } = Expr.Empty;
        public List<Join> Joins { get; private set; } = new List<Join>();
        public List<Column> GroupByColumns { get; private set; } = new List<Column>();
        public IAppendableExpr GroupWhereExpr { get; private set; } = Expr.Empty;
        public List<OrderBy> OrderByExprs { get; private set; } = new List<OrderBy>();
        public int? RowsLimit { get; set; }
        public int RowsOffset { get; set; }
        public bool DistinctRecords { get; set; }
        // public List<string> ParamList { get; set; } = new List<string>();

        public Query(string tableName)
            : this(tableName, null)
        {
        }

        public Query(string tableName, string alias)
            : this(new QuerySource(tableName), alias)
        {
        }

        public Query(Query tableQuery)
            : this(tableQuery, null)
        {
        }

        public Query(Query tableQuery, string alias)
            : this(new QuerySource(tableQuery), alias)
        {
        }

        private Query(QuerySource tableSource, string alias)
        {
            this.TableSource = tableSource;
            this.Alias = alias;
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

        public Query SelectAs(IExpr column, string alias)
        {
            Columns.Add(new Column(column, alias));
            return this;
        }

        public Query SelectAs(string column, string alias)
        {
            Columns.Add(new Column(column, alias));
            return this;
        }

        public Query Where(IExpr expr)
        {
            this.WhereExpr = this.WhereExpr.And(expr);
            return this;
        }

        public Query Where(string expr)
        {
            this.WhereExpr = this.WhereExpr.And(Expr.Parse(expr));
            return this;
        }

        public Query WhereOr(IExpr expr)
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
            /*var outerKeysExprs = outerKeys.Select(x => (IExpr)new Variable(x)).ToArray();
            var innerKeysExprs = innerKeys.Select(x => (IExpr)new Variable(x)).ToArray();*/
            return Join(new Join(inner, outerKeys, innerKeys, leftJoin));
        }
        
        public Query Join(string inner, IExpr[] outerKeys, IExpr[] innerKeys, bool leftJoin)
        {
            return Join(new Join(inner, outerKeys, innerKeys, leftJoin));
        }
        
         public Query Join(string inner, string alias, IExpr[] outerKeys, IExpr[] innerKeys, bool leftJoin)
        {
            return Join(new Join(inner, alias, outerKeys, innerKeys, leftJoin));
        }

        public Query Join(string inner, string[] outerKeys, string[] innerKeys, bool leftJoin)
        {
            /*var outerKeysExprs = outerKeys.Select(x => (IExpr)new Variable(x)).ToArray();
            var innerKeysExprs = innerKeys.Select(x => (IExpr)new Variable(x)).ToArray();*/
            return Join(new Join(inner, outerKeys, innerKeys, leftJoin));
        }
        
        public Query Join(string inner, string alias, string[] outerKeys, string[] innerKeys, bool leftJoin)
        {
            return Join(new Join(inner, alias, outerKeys, innerKeys, leftJoin));
        }

        public Query InnerJoin(Query inner, IExpr[] outerKeys, IExpr[] innerKeys)
        {
            return Join(inner, outerKeys, innerKeys, false);
        }

        public Query InnerJoin(Query inner, string[] outerKeys, string[] innerKeys)
        {
            return Join(inner, outerKeys, innerKeys, false);
        }
        
        public Query InnerJoin(string inner, IExpr[] outerKeys, IExpr[] innerKeys)
        {
            return Join(inner, outerKeys, innerKeys, false);
        }
        
        public Query InnerJoin(string inner, string alias, IExpr[] outerKeys, IExpr[] innerKeys)
        {
            return Join(inner, alias, outerKeys, innerKeys, false);
        }

        public Query InnerJoin(string inner, string[] outerKeys, string[] innerKeys)
        {
            return Join(inner, outerKeys, innerKeys, false);
        }
        
         public Query InnerJoin(string inner, string alias, string[] outerKeys, string[] innerKeys)
        {
            return Join(inner, alias, outerKeys, innerKeys, false);
        }

        public Query LeftJoin(Query inner, IExpr[] outerKeys, IExpr[] innerKeys)
        {
            return Join(inner, outerKeys, innerKeys, true);
        }

        public Query LeftJoin(Query inner, string[] outerKeys, string[] innerKeys)
        {
            return Join(inner, outerKeys, innerKeys, true);
        }
        
        public Query LeftJoin(string inner, IExpr[] outerKeys, IExpr[] innerKeys)
        {
            return Join(inner, outerKeys, innerKeys, true);
        }
        
        public Query LeftJoin(string inner, string alias, IExpr[] outerKeys, IExpr[] innerKeys)
        {
            return Join(inner, alias, outerKeys, innerKeys, true);
        }

        public Query LeftJoin(string inner, string[] outerKeys, string[] innerKeys)
        {
            return Join(inner, outerKeys, innerKeys, true);
        }
        
        public Query LeftJoin(string inner, string alias, string[] outerKeys, string[] innerKeys)
        {
            return Join(inner, alias, outerKeys, innerKeys, true);
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

        public Query Having(IExpr expr)
        {
            this.GroupWhereExpr = this.GroupWhereExpr.And(expr);
            return this;
        }

        public Query Having(string expr)
        {
            this.GroupWhereExpr = this.GroupWhereExpr.And(Expr.Parse(expr));
            return this;
        }

        public Query HavingOr(IExpr expr)
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

        public Query Distinct(bool distinct)
        {
            this.DistinctRecords = distinct;
            return this;
        }

        public Query Distinct()
        {
            return Distinct(true);
        }

        public Query Offset(int offset)
        {
            this.RowsOffset = offset;
            return this;
        }

        /*  public Query Params(params string[] paramList)
          {
              this.ParamList.AddRange(paramList);
              return this;
          }*/


        public IEnumerable<T> Execute<T>(IQueryCompiler compiler, params Value[] parameters)
        {
            return compiler.Execute<T>(this, parameters);
        }

        public object Execute(IQueryCompiler compiler, Type type, params Value[] parameters)
        {
            return compiler.Execute(type, this, parameters);
        }

        public IEnumerable<string[]> Execute(IQueryCompiler compiler, params Value[] parameters)
        {
            return compiler.Execute(this, parameters);
        }

        public IPreparedStatement Prepare(IQueryCompiler compiler)
        {
            return compiler.Prepare(this);
        }

        public IEnumerable<T> Execute<T>(IConnectionSource source, params Value[] parameters)
        {
            return Execute<T>(source.QueryCompiler, parameters);
        }

        public object Execute(IConnectionSource source, Type type, params Value[] parameters)
        {
            return Execute(source.QueryCompiler, type, parameters);
        }

        public IEnumerable<string[]> Execute(IConnectionSource source, params Value[] parameters)
        {
            return Execute(source.QueryCompiler, parameters);
        }

        public IPreparedStatement Prepare(IConnectionSource source)
        {
            return Prepare(source.QueryCompiler);
        }

        public bool IsTrivial()
        {
            return (WhereExpr == null || WhereExpr.Type == ExprType.EMPTY)
                && (GroupWhereExpr == null || GroupWhereExpr.Type == ExprType.EMPTY)
                && !Columns.Any()
                && !OrderByExprs.Any()
                && !Joins.Any()
                && RowsLimit == null
                && RowsOffset == 0
                && !DistinctRecords
                && !GroupByColumns.Any();
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

        public Query()
           : base(MetadataResolver.TableName<T>(), null)
        {
        }

        public Query(string alias)
            : base(MetadataResolver.TableName<T>(), alias)
        {
        }
    }
}
