using System;
using System.Collections.Generic;
using StencilORM.Metadata;

namespace StencilORM.Query
{
    public class Update
    {
        public string TableName { get; private set; }
        public List<Set> Sets { get; private set; } = new List<Set>();
        public IAppendableExpr WhereExpr { get; private set; } = Expr.Empty;

        public bool CreateNewIfNone { get; set; }

        public Update(string tableName)
        {
            this.TableName = tableName;
        }

        public Update InsertOrUpdate(bool createNewIfNone)
        {
            this.CreateNewIfNone = createNewIfNone;
            return this;
        }

        public Update InsertOrUpdate()
        {
            return InsertOrUpdate(true);
        }

        public Update Set(params Set[] sets)
        {
            this.Sets.AddRange(sets);
            return this;
        }

        public Update Set(string name, IExpr value)
        {
            return Set(new Set(name, value));
        }

        public Update Where(IExpr expr)
        {
            this.WhereExpr = this.WhereExpr.And(expr);
            return this;
        }

        public Update Where(string expr)
        {
            this.WhereExpr = this.WhereExpr.And(Expr.Parse(expr));
            return this;
        }

        public Update WhereOr(IExpr expr)
        {
            this.WhereExpr = this.WhereExpr.Or(expr);
            return this;
        }

        public Update WhereOr(string expr)
        {
            this.WhereExpr = this.WhereExpr.Or(Expr.Parse(expr));
            return this;
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

    public class Update<T> : Update
    {
        public Update()
            : base(MetadataResolver.TableName<T>())
        {
        }

        public Update(T value, params string[] columns)
           : base(MetadataResolver.TableName<T>())
        {
            Set(MetadataResolver.Sets<T>(value, columns));
            Where(MetadataResolver.IdentityExpr<T>(value));
        }
    }
}
