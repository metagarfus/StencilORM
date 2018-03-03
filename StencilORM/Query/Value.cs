using System;
namespace StencilORM.Query
{
    public class Value
    {
        public string Name { get; set; }
        public IExpr Expr { get; set; }

        public Value(string name, IExpr expr)
        {
            this.Name = name;
            this.Expr = expr;
        }
    }
}
