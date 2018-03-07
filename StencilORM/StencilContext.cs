﻿using System;
using StencilORM.Metadata;
using StencilORM.Parsers;

namespace StencilORM
{
    public static class StencilContext
    {
        public static IExprParser DefaultExprParser { get; set; } = ExprParser.Instance;
    }
}
