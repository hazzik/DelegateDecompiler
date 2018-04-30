using System;
using System.Collections.Generic;
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

        public Address Clone(IDictionary<Address, Address> map)
        {
            if (map.ContainsKey(this))
                return map[this];
            var result = new Address { Expression = Expression };
            map[this] = result;
            return result;
        }

        public static Address Merge(Expression test, Address address1, Address address2,
            IDictionary<Tuple<Address, Address>, Address> map)
        {
            var addresses = Tuple.Create(address1, address2);
            if (map.ContainsKey(addresses))
                return map[addresses];
            Address result;
            if (address1.Expression == address2.Expression)
            {
                result = address1;
            }
            else
            {
                var left = address1.Expression ?? Expression.Default(address2.Type);
                var right = address2.Expression ?? Expression.Default(address1.Type);
                left = Processor.AdjustType(left, right.Type);
                right = Processor.AdjustType(right, left.Type);
                result = Expression.Condition(test, left, right);
            }
            map[addresses] = result;
            return result;
        }
    }
}