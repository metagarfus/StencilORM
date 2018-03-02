using System;
namespace StencilORM.Query
{
    public struct OrderBy
    {
        public Expr Expr { get; set; }
        public bool Descending { get; set; }
    }
}
