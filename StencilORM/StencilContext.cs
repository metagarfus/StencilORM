using System;
using StencilORM.Parsers;

namespace StencilORM
{
    public static class StencilContext
    {
        public static IConnectionSource ConnectionSource { get; set; }
        public static IExprParser DefaultExprParser { get; set; } = ExprParser.Instance;
    }
}
