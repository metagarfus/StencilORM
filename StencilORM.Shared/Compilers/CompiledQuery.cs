using System;
namespace StencilORM.Compilers
{
    public struct CompiledQuery<T>
    {
        public string Query { get; set; }
        public T State { get; set; }

        public CompiledQuery(string query, T state)
        {
            this.Query = query;
            this.State = state;
        }
    }
}
