using System;
using System.Text;
using System.Collections.Generic;
using StencilORM.Parsers;
using StencilORM.Metadata;

namespace StencilORM.Queries
{
    public enum ExprType
    {
        LITERAL,
        EXPR,
        VARIABLE,
        PARAM,
        FUNCTION,
        IF,
        // SUBQUERY,
        EMPTY,
    }

    public interface IExpr
    {
        ExprType Type { get; }
        IExpr Negate();
        void Visit(IASTVisitor visitor);
        void Visit<T>(T data, IASTVisitor<T> visitor);
        void Visit<T1, T2>(T1 data1, T2 data2, IASTVisitor<T1, T2> visitor);
    }

    public struct Function : IExpr
    {
        public ExprType Type => ExprType.FUNCTION;

        public string Name { get; set; }

        public IExpr[] Args { get; set; }

        public Function(string name, params IExpr[] args)
        {
            this.Name = name;
            this.Args = args;
        }

        public IExpr Negate()
        {
            throw new NotSupportedException("Parameters cannot be negated");
        }

        public void Visit(IASTVisitor visitor)
        {
            visitor.Process(this);
        }

        public void Visit<T>(T data, IASTVisitor<T> visitor)
        {
            visitor.Process(data, this);
        }
        
        public void Visit<T1, T2>(T1 data1, T2 data2, IASTVisitor<T1, T2> visitor)
        {
            visitor.Process(data1, data2, this);
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

        public void Visit(IASTVisitor visitor)
        {
            visitor.Process(this);
        }

        public void Visit<T>(T data, IASTVisitor<T> visitor)
        {
            visitor.Process(data, this);
        }
        
        public void Visit<T1, T2>(T1 data1, T2 data2, IASTVisitor<T1, T2> visitor)
        {
            visitor.Process(data1, data2, this);
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
            if (parts.Length == 2)
            {
                Scope = parts[0];
                Name = parts[1];
            }
            else
            {
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

        public void Visit(IASTVisitor visitor)
        {
            visitor.Process(this);
        }

        public void Visit<T>(T data, IASTVisitor<T> visitor)
        {
            visitor.Process(data, this);
        }
        
        public void Visit<T1, T2>(T1 data1, T2 data2, IASTVisitor<T1, T2> visitor)
        {
            visitor.Process(data1, data2, this);
        }
    }

    public struct Literal : IExpr
    {
        public ExprType Type => ExprType.LITERAL;

        public DataType DataType { get; set; }

        public object Value { get; set; }

        public Literal(object value)
            : this(DataType.UNKNOWN, value)
        {
        }

        public Literal(DataType dataType, object value)
        {
            this.DataType = dataType;
            this.Value = value;
        }

        public static implicit operator Literal(int? value)
        {
            return new Literal
            {
                Value = value
            };
        }

        public static implicit operator Literal(long? value)
        {
            return new Literal
            {
                Value = value
            };
        }

        public static implicit operator Literal(double? value)
        {
            return new Literal
            {
                Value = value
            };
        }

        public static implicit operator Literal(float? value)
        {
            return new Literal
            {
                Value = value
            };
        }


        public static implicit operator Literal(decimal? value)
        {
            return new Literal
            {
                Value = value
            };
        }

        public static implicit operator Literal(bool? value)
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

        public static implicit operator Literal(DateTime? value)
        {
            return new Literal
            {
                Value = value
            };
        }

        public static implicit operator Literal(DateTimeOffset? value)
        {
            return new Literal
            {
                Value = value
            };
        }

        public static implicit operator Literal(Guid? value)
        {
            return new Literal
            {
                Value = value
            };
        }

        public static implicit operator Literal(Query value)
        {
            return new Literal
            {
                Value = value
            };
        }

        public IExpr Negate()
        {
            if (Value is short)
                return (Literal)(-((short)Value));
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

        public void Visit(IASTVisitor visitor)
        {
            visitor.Process(this);
        }

        public void Visit<T>(T data, IASTVisitor<T> visitor)
        {
            visitor.Process(data, this);
        }
        
        public void Visit<T1, T2>(T1 data1, T2 data2, IASTVisitor<T1, T2> visitor)
        {
            visitor.Process(data1, data2, this);
        }
    }

    /* public struct SubQuery : IExpr
     { 
         public ExprType Type => ExprType.LITERAL;

         public Query Query { get; set; }

         public static implicit operator SubQuery(Query query)
         {
             return new SubQuery
             {
                 Query = query
             };
         }

         public IExpr Negate()
         {
             return Expr.Mul((Literal)(-1), this);
         }
     }*/

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

        public void Visit(IASTVisitor visitor)
        {
            visitor.Process(this);
        }

        public void Visit<T>(T data, IASTVisitor<T> visitor)
        {
            visitor.Process(data, this);
        }
        
        public void Visit<T1, T2>(T1 data1, T2 data2, IASTVisitor<T1, T2> visitor)
        {
            visitor.Process(data1, data2, this);
        }
    }

    public interface IAppendableExpr : IExpr
    {
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

        public void Visit(IASTVisitor visitor)
        {
            visitor.Process(this);
        }

        public void Visit<T>(T data, IASTVisitor<T> visitor)
        {
            visitor.Process(data, this);
        }
        
        public void Visit<T1, T2>(T1 data1, T2 data2, IASTVisitor<T1, T2> visitor)
        {
            visitor.Process(data1, data2, this);
        }
    }

    public struct Expr : IAppendableExpr
    {
        public ExprType Type => ExprType.EXPR;

        public IExpr Left { get; set; }
        public IExpr Right { get; set; }
        public Operation Operation { get; set; }

        public static readonly Empty Empty = new Empty();

        public bool IsBinaryOperation()
        {
            return !(Operation == Operation.NOT
                  || Operation == Operation.EXISTS
                  || Operation == Operation.IN
                  || Operation == Operation.NOTIN);
        }

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

        public static Expr Add(IExpr Left, Literal right)
        {
            return NewExpr(Operation.ADD, Left, right);
        }

        public static Expr Sub(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.SUB, Left, right);
        }

        public static Expr Sub(IExpr Left, Literal right)
        {
            return NewExpr(Operation.SUB, Left, right);
        }

        public static Expr Mul(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.MUL, Left, right);
        }

        public static Expr Mul(IExpr Left, Literal right)
        {
            return NewExpr(Operation.MUL, Left, right);
        }

        public static Expr Div(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.DIV, Left, right);
        }

        public static Expr Div(IExpr Left, Literal right)
        {
            return NewExpr(Operation.DIV, Left, right);
        }

        public static Expr Gt(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.GT, Left, right);
        }

        public static Expr Gt(Variable Left, Literal right)
        {
            return NewExpr(Operation.GT, Left, right);
        }

        public static Expr Lt(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.LT, Left, right);
        }

        public static Expr Lt(Variable Left, Literal right)
        {
            return NewExpr(Operation.LT, Left, right);
        }

        public static Expr GtE(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.GTE, Left, right);
        }

        public static Expr GtE(Variable Left, Literal right)
        {
            return NewExpr(Operation.GTE, Left, right);
        }

        public static Expr LtE(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.LTE, Left, right);
        }

        public static Expr LtE(Variable Left, Literal right)
        {
            return NewExpr(Operation.LTE, Left, right);
        }

        public static Expr Eq(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.EQUALS, Left, right);
        }

        public static Expr Eq(Variable Left, Literal right)
        {
            return NewExpr(Operation.EQUALS, Left, right);
        }

        public static Expr NotEq(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.NOTEQUALS, Left, right);
        }

        public static Expr NotEq(Variable Left, Literal right)
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

        public static Expr Exists(IExpr Left)
        {
            return NewExpr(Operation.EXISTS, Left, null);
        }

        public static Expr Exists<T>(IEnumerable<T> left)
        {
            return NewExpr(Operation.EXISTS, new Literal { Value = left }, null);
        }

        public static Expr Exists(Query left)
        {
            return NewExpr(Operation.EXISTS, (Literal)left, null);
        }

        public static Expr In(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.IN, Left, right);
        }

        public static Expr In<T>(Variable Left, IEnumerable<T> right)
        {
            return NewExpr(Operation.IN, Left, new Literal { Value = right });
        }

        public static Expr In(Variable Left, Query right)
        {
            return NewExpr(Operation.IN, Left, (Literal)right);
        }

        public static Expr NotIn(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.NOTIN, Left, right);
        }

        public static Expr NotIn<T>(Variable Left, IEnumerable<T> right)
        {
            return NewExpr(Operation.NOTIN, Left, new Literal { Value = right });
        }

        public static Expr NotIn(Variable Left, Query right)
        {
            return NewExpr(Operation.NOTIN, Left, (Literal)right);
        }

        public static Expr Not(IExpr Left)
        {
            return NewExpr(Operation.NOT, Left, null);
        }

        public static Expr IsNull(IExpr Left)
        {
            return NewExpr(Operation.ISNULL, Left, null);
        }

        public static Expr IsNull(Variable Left)
        {
            return NewExpr(Operation.ISNULL, Left, null);
        }

        public static Expr NotNull(IExpr Left)
        {
            return NewExpr(Operation.NOTNULL, Left, null);
        }

        public static Expr NotNull(Variable Left)
        {
            return NewExpr(Operation.NOTNULL, Left, null);
        }

        public static Expr Concat(IExpr Left, IExpr right)
        {
            return NewExpr(Operation.CONCAT, Left, right);
        }

        public static Expr Concat(IExpr Left, Literal right)
        {
            return NewExpr(Operation.CONCAT, Left, right);
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

        public void Visit(IASTVisitor visitor)
        {
            visitor.Process(this);
        }

        public void Visit<T>(T data, IASTVisitor<T> visitor)
        {
            visitor.Process(data, this);
        }
        
        public void Visit<T1, T2>(T1 data1, T2 data2, IASTVisitor<T1, T2> visitor)
        {
            visitor.Process(data1, data2, this);
        }
    }
}
