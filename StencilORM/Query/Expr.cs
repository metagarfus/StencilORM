using System;
using StencilORM.Parsers;

namespace StencilORM.Query
{
    public enum ExprType
    {
        LITERAL,
        EXPR,
        VARIABLE,
        PARAM,
        FUNCTION,
        IF,
        EMPTY,
    }

    public interface IExpr
    {
        ExprType Type { get; }
        IExpr Negate();
    }

    public struct Function : IExpr
    {
        public ExprType Type => ExprType.FUNCTION;

        public IExpr[] Args { get; set; }

        public Function(params IExpr[] args)
        {
            this.Args = args;
        }

        public IExpr Negate()
        {
            throw new NotSupportedException("Parameters cannot be negated");
        }
    }

    public struct Param : IExpr
    {
        public ExprType Type => ExprType.PARAM;

        public string Name { get; set; }

        public Param(string name)
        {
            this.Name = name;
        }

        public static implicit operator Param(string name)
        {
            return new Param(name);
        }

        public IExpr Negate()
        {
            throw new NotSupportedException("Parameters cannot be negated");
        }
    }

    public struct Variable : IExpr
    {
        public ExprType Type => ExprType.VARIABLE;

        public string Name { get; set; }

        public string Scope { get; set; }

        public Variable(string name)
        {
            string[] parts = (name ?? "").Split('.');
            if (parts.Length > 2)
                throw new Exception("Variables names cannot contain more than two '.'");
            if (parts.Length == 2) {
                Scope = parts[0];
                Name = parts[1];
            } else {
                Scope = null;
                Name = parts[0];
            }
        }

        public static implicit operator Variable(string name)
        {
            return new Variable(name);
        }

        public IExpr Negate()
        {
            return Expr.Mul((Literal)(-1), this);
        }
    }

    public struct Literal : IExpr
    {
        public ExprType Type => ExprType.LITERAL;

        public object Value { get; set; }

        public static implicit operator Literal(int value)
        {
            return new Literal
            {
                Value = value
            };
        }

        public static implicit operator Literal(long value)
        {
            return new Literal
            {
                Value = value
            };
        }

        public static implicit operator Literal(double value)
        {
            return new Literal
            {
                Value = value
            };
        }

        public static implicit operator Literal(float value)
        {
            return new Literal
            {
                Value = value
            };
        }


        public static implicit operator Literal(decimal value)
        {
            return new Literal
            {
                Value = value
            };
        }

        public static implicit operator Literal(bool value)
        {
            return new Literal
            {
                Value = value
            };
        }

        public static implicit operator Literal(string value)
        {
            return new Literal
            {
                Value = value
            };
        }

        public IExpr Negate()
        {
            if (Value is short)
                return (Literal)(-((short) Value));
            if (Value is int)
                return (Literal)(-((int)Value));
            if (Value is long)
                return (Literal)(-((long)Value));
            if (Value is float)
                return (Literal)(-((float)Value));
            if (Value is double)
                return (Literal)(-((double)Value));
            if (Value is decimal)
                return (Literal)(-((decimal)Value));
            if (Value is bool)
                return (Literal)(!((bool)Value));
            return Expr.Mul((Literal)(-1), this);
        }
    }

    public struct If : IExpr
    {
        public ExprType Type => ExprType.IF;

        public IExpr Condition { get; set; }

        public IExpr ThenExpr { get; set; }

        public IExpr ElseExpr { get; set; }

        public If(IExpr condition, IExpr thenExpr, IExpr elseExpr)
        {
            this.Condition = condition;
            this.ThenExpr = thenExpr;
            this.ElseExpr = elseExpr;
        }

        public IExpr Negate()
        {
            return Expr.Mul((Literal)(-1), this);
        }
    }

    public interface IAppendableExpr : IExpr {
        IAppendableExpr And(IExpr right);

        IAppendableExpr Or(IExpr right);
    }

    public struct Empty : IAppendableExpr
    {
        public ExprType Type => ExprType.EMPTY;

        public IAppendableExpr And(IExpr right)
        {
            return VerifyAppendable(right);
        }

        public IAppendableExpr Or(IExpr right)
        {
            return VerifyAppendable(right);
        }

        private IAppendableExpr VerifyAppendable(IExpr right)
        {
            var appendable = right as IAppendableExpr;
            if (appendable == null)
                throw new Exception("Expression is null or non-appendable");
            return appendable;
        }

        public IExpr Negate()
        {
            return this;
        }
    }

    public struct Expr : IAppendableExpr
    {
        public ExprType Type => ExprType.EXPR;

        public IExpr Left { get; set; }
        public IExpr Right { get; set; }
        public Operation Operation { get; set; }

        public static readonly Empty Empty = new Empty();

        public IExpr Negate()
        {
            return Expr.Mul((Literal)(-1), this);
        }

        public static Expr NewExpr(Operation operation, IExpr left, IExpr right)
        {
            return new Expr
            {
                Operation = operation,
                Left = left,
                Right = right,
            };
        }

        public IAppendableExpr And(IExpr right)
        {
            return Expr.And(this, right);
        }

        public IAppendableExpr Or(IExpr right)
        {
            return Expr.Or(this, right);
        }

        public static Expr Add(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.ADD, Left, right);
        }

        public static Expr Sub(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.SUB, Left, right);
        }

        public static Expr Mul(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.MUL, Left, right);
        }

        public static Expr Div(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.DIV, Left, right);
        }

        public static Expr Gt(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.GT, Left, right);
        }

        public static Expr Lt(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.LT, Left, right);
        }

        public static Expr GtE(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.GTE, Left, right);
        }

        public static Expr LtE(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.LTE, Left, right);
        }

        public static Expr Eq(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.EQUALS, Left, right);
        }

        public static Expr NotEq(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.NOTEQUALS, Left, right);
        }

        public static Expr And(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.AND, Left, right);
        }

        public static Expr Or(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.OR, Left, right);
        }

        public static Expr In(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.IN, Left, right);
        }

        public static Expr NotIn(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.NOTIN, Left, right);
        }

        public static Expr Not(IExpr Left)
        {
            return NewExpr(Operation.NOT, Left, null);
        }

        public static Expr IsNull(IExpr Left)
        {
            return NewExpr(Operation.ISNULL, Left, null);
        }

        public static Expr NotNull(IExpr Left)
        {
            return NewExpr(Operation.NOTNULL, Left, null);
        }
        /*
        public static Expr As(IExpr Left)
        {
            return NewExpr(Operation.AS, Left, null);
        }*/

        public static IExpr Parse(string source)
        {
            return StencilContext.DefaultExprParser.Parse(source);
        }
    }
}
