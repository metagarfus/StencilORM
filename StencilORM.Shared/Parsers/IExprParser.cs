using System;
using StencilORM.Queries;

namespace StencilORM.Parsers
{
    public interface IExprParser
    {
        IExpr Parse(string source);
    }
}
