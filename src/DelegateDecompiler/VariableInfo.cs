using System;
using System.Linq.Expressions;

namespace DelegateDecompiler
{
    class VariableInfo
    {
        public VariableInfo(Type type)
        {
            Type = type;
            Address = new Address
            {
                Expression = Expression.Variable(type)
            };
        }

        public Type Type { get; set; }

        public Address Address { get; set; }
    }
}