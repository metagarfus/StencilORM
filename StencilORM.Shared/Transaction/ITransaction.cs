using System;
namespace StencilORM.Transaction
{
    public interface ITransaction : IDisposable
    {
        void Commit();
    }
}
