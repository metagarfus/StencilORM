using System;
namespace StencilORM.Metadata
{
    public class MissingReflectionException : Exception
    {
        public MissingReflectionException()
            : base("Missing reflection services.")
        {
        }
    }
}
