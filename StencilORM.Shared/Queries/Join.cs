using System;
namespace StencilORM.Queries
{
    public struct Join
    {
        //public Query Outer { get; set; }
        public QuerySource Inner { get; set; }
        public string Alias { get; set; }
        public IExpr On { get; set; }
        public bool LeftJoin { get; set; }

        public Join(string inner, string[] outerKeys, string[] innerKeys, bool leftJoin)
            : this(inner, null, outerKeys, innerKeys, leftJoin)
        {
        }

        public Join(string inner, string alias, string[] outerKeys, string[] innerKeys, bool leftJoin)
            : this(inner, alias, ZipOn(outerKeys, innerKeys), leftJoin)
        {
        }

        public Join(string inner, IExpr[] outerKeys, IExpr[] innerKeys, bool leftJoin)
            : this(inner, null, outerKeys, innerKeys, leftJoin)
        {
        }

        public Join(string inner, string alias, IExpr[] outerKeys, IExpr[] innerKeys, bool leftJoin)
           : this(inner, alias, ZipOn(outerKeys, innerKeys), leftJoin)
        {
        }

        public Join(string inner, IExpr on, bool leftJoin)
                : this(inner, null, on, leftJoin)
        {
        }

        public Join(string inner, string alias, IExpr on, bool leftJoin)
                : this(new QuerySource(inner), alias, on, leftJoin)
        {
        }

        public Join(Query inner, string[] outerKeys, string[] innerKeys, bool leftJoin)
            : this(inner, null, outerKeys, innerKeys, leftJoin)
        {
        }

        public Join(Query inner, string alias, string[] outerKeys, string[] innerKeys, bool leftJoin)
           : this(inner, alias, ZipOn(outerKeys, innerKeys), leftJoin)
        {
        }

        public Join(Query inner, IExpr[] outerKeys, IExpr[] innerKeys, bool leftJoin)
            : this(inner, null, outerKeys, innerKeys, leftJoin)
        {
        }

        public Join(Query inner, string alias, IExpr[] outerKeys, IExpr[] innerKeys, bool leftJoin)
            : this(inner, alias, ZipOn(outerKeys, innerKeys), leftJoin)
        {
        }

        public Join(Query inner, IExpr on, bool leftJoin)
                : this(inner, null, on, leftJoin)
        {
        }

        public Join(Query inner, string alias, IExpr on, bool leftJoin)
                : this(new QuerySource(inner), alias, on, leftJoin)
        {
        }

        private Join(QuerySource inner, string alias, IExpr on, bool leftJoin)
        {
            Inner = inner;
            Alias = alias;
            On = on;
            LeftJoin = leftJoin;
        }

        private static IAppendableExpr ZipOn(string[] outerKeyNames, string[] innerKeyNames)
        {
            IExpr[] outerKeys = new IExpr[outerKeyNames?.Length ?? 0];
            IExpr[] innerKeys = new IExpr[innerKeyNames?.Length ?? 0];
            if (outerKeyNames != null)
            {
                int len = outerKeyNames.Length;
                for (int n = 0; n < len; n++)
                    outerKeys[n] = new Variable(outerKeyNames[n]);
            }

            if (innerKeyNames != null)
            {
                int len = innerKeyNames.Length;
                for (int n = 0; n < len; n++)
                    innerKeys[n] = new Variable(innerKeyNames[n]);
            }

            return ZipOn(outerKeys, innerKeys);
        }

        private static IAppendableExpr ZipOn(IExpr[] outerKeys, IExpr[] innerKeys)
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
