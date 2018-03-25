using System;
using StencilORM.Metadata;

namespace StencilORM.Queries
{
    public struct Value
    {
        public DataType DataType { get; set; }
        public string Name { get; set; }
        public IExpr Expr { get; set; }

        public Value(string name, object expr)
            : this(name, new Literal(expr))
        {
        }

        public Value(string name, IExpr expr)
            : this(DataType.UNKNOWN, name, expr)
        {
        }
        
        public Value(DataType dataType, string name, object expr)
            : this(dataType, name, new Literal(expr))
        {
        }

        public Value(DataType dataType, string name, IExpr expr)
        {
            this.DataType = dataType;
            this.Name = name;
            this.Expr = expr;
        }
    }
}
