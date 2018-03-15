using System;
using StencilORM.Metadata;
using StencilORM.Compilers;

namespace StencilORM.Queries
{
    public class Delete
    {
        public string TableName { get; private set; }
        public IAppendableExpr WhereExpr { get; private set; } = Expr.Empty;

        public Delete(string tableName)
        {
            this.TableName = tableName;
        }

        public Delete Where(IExpr expr)
        {
            this.WhereExpr = this.WhereExpr.And(expr);
            return this;
        }

        public Delete Where(string expr)
        {
            this.WhereExpr = this.WhereExpr.And(Expr.Parse(expr));
            return this;
        }

        public Delete WhereOr(IExpr expr)
        {
            this.WhereExpr = this.WhereExpr.Or(expr);
            return this;
        }

        public Delete WhereOr(string expr)
        {
            this.WhereExpr = this.WhereExpr.Or(Expr.Parse(expr));
            return this;
        }

        public bool Execute(IQueryCompiler compiler, out int rowsAltered, params Value[] parameters)
        {
            return compiler.Execute(this, out rowsAltered, parameters);
        }

        public IPreparedStatement Prepare(IQueryCompiler compiler)
        {
            return compiler.Prepare(this);
        }

        public bool Execute(IConnectionSource source, out int rowsAltered, params Value[] parameters)
        {
            return Execute(source.QueryCompiler, out rowsAltered, parameters);
        }
        
        public IPreparedStatement Prepare(IConnectionSource source)
        {
            return Prepare(source.QueryCompiler);
        }
    }

    public class Delete<T> : Update
    {
        public Delete()
            : base(MetadataResolver.TableName<T>())
        {
        }

        public Delete(T value, params string[] columns)
           : base(MetadataResolver.TableName<T>())
        {
            Where(MetadataResolver.IdentityExpr<T>(value));
        }
    }
}
