using System;
namespace StencilORM.Queries
{
    public struct Set
    {
        public string Name { get; set; }
        public IExpr Value { get; set; }

        public Set(string name, IExpr value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
