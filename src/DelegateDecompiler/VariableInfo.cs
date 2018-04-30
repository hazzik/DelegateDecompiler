using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DelegateDecompiler
{
    class VariableInfo
    {
        public VariableInfo(Type type)
        {
            Type = type;
            Address = new Address();
        }

        public Type Type { get; set; }

        public Address Address { get; set; }

        public VariableInfo Clone(IDictionary<Address, Address> addressMap)
        {
            return new VariableInfo(Type)
            {
                Address = Address.Clone(addressMap)
            };
        }
    }
}