using System;

namespace DelegateDecompiler
{
    class VariableInfo
    {
        public VariableInfo(Type type)
        {
            Type = type;
            Address = new Address
            {
                Expression = ExpressionHelper.Default(type)
            };
        }

        public Type Type { get; set; }

        public Address Address { get; set; }
    }
}