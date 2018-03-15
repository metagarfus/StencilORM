using System;
using System.Collections.Generic;
using System.Linq;
using StencilORM.Queries;

namespace StencilORM.Compilers
{
    public class PreparedInsertOrUpdate : IPreparedStatement
    {
        public IPreparedStatement Update { get; set; }
        public IPreparedStatement Insert { get; set; }
        public IPreparedStatement Exists { get; set; }
    
        public PreparedInsertOrUpdate()
        {
        }

        private bool ExecuteExists(Value[] parameters)
        {
            return Exists.Execute<bool>(parameters).FirstOrDefault();
        }    

        public IEnumerable<T> Execute<T>(params Value[] parameters)
        {
            if (ExecuteExists(parameters))
                return Update.Execute<T>(parameters);
            return Insert.Execute<T>(parameters);
        }

        public object Execute(Type type, params Value[] parameters)
        {
            if (ExecuteExists(parameters))
                return Update.Execute(type, parameters);
            return Insert.Execute(type, parameters);
        }

        public IEnumerable<string[]> Execute(params Value[] parameters)
        {
            if (ExecuteExists(parameters))
                return Update.Execute(parameters);
            return Insert.Execute(parameters);
        }

        public bool Execute(out int rowsAltered, params Value[] parameters)
        {
            if (ExecuteExists(parameters))
                return Update.Execute(out rowsAltered, parameters);
            return Insert.Execute(out rowsAltered, parameters);
        }
    }
}
