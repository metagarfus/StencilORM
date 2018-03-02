using System;
namespace StencilORM.Query
{
    public struct Join
    {
        public Query Outer { get; set; }
        public Query Inner { get; set; }
        public IExpr On { get; set; }

        public Join(Query outer, Query inner, Expr[] outerKeys, Expr[] innerKeys)
            : this(outer, inner, ZipOn(outerKeys, innerKeys))
        {
        }

        public Join(Query outer, Query inner, IExpr on)
        {
            Outer = outer;
            Inner = inner;
            On = on;
        }

        private static IAppendableExpr ZipOn(Expr[] outerKeys, Expr[] innerKeys)
        {
            if (outerKeys.Length != innerKeys.Length)
                throw new Exception("Inner and Outer key selectors must specify the same number of keys");

            IAppendableExpr on = Expr.Empty;
            for (int n = 0; n < outerKeys.Length && n < innerKeys.Length; n++)
            {
                on = on.And(Expr.Eq(outerKeys[n], innerKeys[n]));
            }

            return on;
        }
    }
}
