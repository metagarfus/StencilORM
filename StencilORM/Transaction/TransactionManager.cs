using System;
namespace StencilORM.Transaction
{
    public static class TransactionManager
    {
        public static T CallInTransaction<T>(IConnectionSource source, Func<IConnectionSource, T> runInsideTransaction)
        {
            using (ITransaction transaction = source.StartTransaction())
            {
                var result = runInsideTransaction(source);
                transaction.Commit();
                return result;
            }
        }
    }
}
