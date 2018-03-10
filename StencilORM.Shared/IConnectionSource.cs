using System;
using StencilORM.Transaction;
using StencilORM.Query;
namespace StencilORM
{
    public interface IConnectionSource
    {
        ITransaction StartTransaction();
        IQueryCompiler QueryCompiler { get; }
    }
}
