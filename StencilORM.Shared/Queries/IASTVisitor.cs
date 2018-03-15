using System;
using System.Text;
namespace StencilORM.Queries
{
    public interface IASTVisitor
    {
        void Process(Expr expr);
        void Process(Param param);
        void Process(Variable variable);
        void Process(If @if);
        void Process(Empty empty);
        void Process(Literal literal);
        void Process(Function function);
    }
    
    public interface IASTVisitor<T>
    {
        void Process(T data, Expr expr);
        void Process(T data, Param param);
        void Process(T data, Variable variable);
        void Process(T data, If @if);
        void Process(T data, Empty empty);
        void Process(T data, Literal literal);
        void Process(T data, Function function);
    }
    
    public interface IASTVisitor<T1, T2>
    {
        void Process(T1 data1, T2 data2, Expr expr);
        void Process(T1 data1, T2 data2, Param param);
        void Process(T1 data1, T2 data2, Variable variable);
        void Process(T1 data1, T2 data2, If @if);
        void Process(T1 data1, T2 data2, Empty empty);
        void Process(T1 data1, T2 data2, Literal literal);
        void Process(T1 data1, T2 data2, Function function);
    }
}
