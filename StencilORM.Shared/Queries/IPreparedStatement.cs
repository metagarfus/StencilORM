using System;
using System.Collections.Generic;

namespace StencilORM.Queries
{
    public interface IPreparedStatement
    {  
        IEnumerable<T> Execute<T>(params Value[] parameters);
        object Execute(Type type, params Value[] parameters);
        IEnumerable<string[]> Execute(params Value[] parameters);
        bool Execute(out int rowsAltered, params Value[] parameters);
    }
}
