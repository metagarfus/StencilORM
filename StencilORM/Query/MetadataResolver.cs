using System;
namespace StencilORM.Query
{
    public class MetadataResolver
    {
        public MetadataResolver()
        {
        }

        public static string TableName<T>()
        {
            return null;
        }

        internal static Set[] Sets<T>(T value, string[] columns)
        {
            throw new NotImplementedException();
        }

        internal static Expr IdentityExpr<T>(T value)
        {
            throw new NotImplementedException();
        }
    }
}
