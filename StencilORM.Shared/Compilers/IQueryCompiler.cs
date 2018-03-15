using System;
using System.Collections.Generic;
using StencilORM.Queries;

namespace StencilORM.Compilers
{
    public interface IQueryCompiler
    {
        IEnumerable<T> Execute<T>(Query query, params Value[] parameters);
        object Execute(Type type, Query query, params Value[] parameters);
        IEnumerable<string[]> Execute(Query query, params Value[] parameters);
        bool Execute(Insert query, out int rowsAltered, params Value[] parameters);
        bool Execute(Update query, out int rowsAltered, params Value[] parameters);
        bool Execute(Delete query, out int rowsAltered, params Value[] parameters);

        IPreparedStatement Prepare(Query query);
        IPreparedStatement Prepare(Insert query);
        IPreparedStatement Prepare(Update query);
        IPreparedStatement Prepare(Delete query);
        
        bool CreateOrAlter<T>();

       /* bool Update(Update query, out int rowsAltered, params Value[] parameters);
        bool Insert(Insert query, out int rowsAltered, params Value[] parameters);
        bool InsertOrUpdate(InsertOrUpdate query, out int rowsAltered, params Value[] parameters);*/
    }
}