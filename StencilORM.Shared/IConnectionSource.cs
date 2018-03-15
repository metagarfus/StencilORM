using System;
using StencilORM.Transaction;
using StencilORM.Queries;
using StencilORM.Compilers;

namespace StencilORM
{
    public interface IConnectionSource
    {
        ITransaction StartTransaction();
        IQueryCompiler QueryCompiler { get; }
    }
}
