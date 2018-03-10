using System;
namespace StencilORM.Query
{
    public struct OrderBy
    {
        public IExpr Expr { get; set; }
        public bool Descending { get; set; }

        public OrderBy(bool descending, string name)
            : this(descending, new Variable(name))
        { 
        }

        public OrderBy(bool descending, IExpr expr)
        {
            this.Expr = expr;
            this.Descending = descending;
        }
    }
}
