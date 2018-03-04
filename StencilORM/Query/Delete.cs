using System;
namespace StencilORM.Query
{
    public class Delete
    {
        public string TableName { get; private set; }
        public IAppendableExpr WhereExpr { get; private set; }



        public Delete(string tableName)
        {
            this.TableName = tableName;
        }

        public Delete Where(Expr expr)
        {
            this.WhereExpr = this.WhereExpr.And(expr);
            return this;
        }

        public Delete Where(string expr)
        {
            this.WhereExpr = this.WhereExpr.And(Expr.Parse(expr));
            return this;
        }

        public Delete WhereOr(Expr expr)
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

        public bool Execute(IConnectionSource source, out int rowsAltered, params Value[] parameters)
        {
            return Execute(source.QueryCompiler, out rowsAltered, parameters);
        }

        public bool Execute(out int rowsAltered, params Value[] parameters)
        {
            return Execute(StencilContext.ConnectionSource, out rowsAltered, parameters);
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
