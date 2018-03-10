using System;
namespace StencilORM.Query
{
    public struct Column
    {
        public IExpr Value { get; set; }
        public string Alias { get; set; }

        public Column(string name)
            : this(new Variable(name), null)
        {
        }

        public Column(IExpr value)
           : this(value, null)
        {
        }

        public Column(string name, string alias)
           : this(new Variable(name), alias)
        {
        }

        public Column(IExpr Value, string alias)
        {
            this.Value = Value;
            this.Alias = alias;
        }

        public static implicit operator Column(Expr expr)
        {
            return new Column(expr);
        }

        public static implicit operator Column(Variable expr)
        {
            return new Column(expr);
        }

        public static implicit operator Column(Literal expr)
        {
            return new Column(expr);
        }

        public static implicit operator Column(string name)
        {
            return new Column(Expr.Parse(name));
        }
    }
}
