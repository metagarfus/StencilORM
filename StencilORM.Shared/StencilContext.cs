using System;
using StencilORM.Metadata;
using StencilORM.Parsers;
using StencilORM.Reflection;

namespace StencilORM
{
    public static class StencilContext
    {
        public static IExprParser DefaultExprParser { get; set; } = ExprParser.Instance;
        public static IReflectionServices ReflectionServices { get; set; } = new ReflectionServices();
    }
}
