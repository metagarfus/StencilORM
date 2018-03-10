using System;
using StencilORM.Query;

namespace StencilORM.Parsers
{
    public interface IExprParser
    {
        IExpr Parse(string source);
    }
}
