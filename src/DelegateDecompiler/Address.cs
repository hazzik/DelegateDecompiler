using System;
using System.Linq.Expressions;

namespace DelegateDecompiler
{
    class Address
    {
        public Expression Expression { get; set; }

        public Type Type
        {
            get { return Expression.Type; }
        }

        public static implicit operator Expression(Address address)
        {
            return address.Expression;
        }

        public static implicit operator Address(Expression expression)
        {
            return new Address {Expression = expression};
        }
    }
}