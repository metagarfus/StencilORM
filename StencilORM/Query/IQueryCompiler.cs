using System;
using System.Collections.Generic;

namespace StencilORM.Query
{
    public interface IQueryCompiler
    {
        IEnumerable<T> Execute<T>(Query query, params Value[] parameters);
        dynamic Execute(Type type, Query query, params Value[] parameters);
        IEnumerable<string[]> Execute(Query query, params Value[] parameters);
        bool Update(Update query, out int rowsAltered, params Value[] parameters);
        bool Insert(Insert query, out int rowsAltered, params Value[] parameters);
        bool InsertOrUpdate(InsertOrUpdate query, out int rowsAltered, params Value[] parameters);
    }
}
