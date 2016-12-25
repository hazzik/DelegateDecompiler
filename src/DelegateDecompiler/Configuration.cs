using System;
using System.Reflection;

namespace DelegateDecompiler
{
    public abstract class Configuration
    {
        private static readonly object locker = new object();
        private static volatile Configuration instance;

        internal static Configuration Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new DefaultConfiguration();
                        }
                    }
                }
                return instance;
            }
        }

        public static void Configure(Configuration cfg)
        {
            if (instance == null)
            {
                lock (locker)
                {
                    if (instance == null)
                    {
                        instance = cfg;
                        return;
                    }
                }
            }

            throw new InvalidOperationException("DelegateDecompiler has been configured already");
        }

        public abstract bool ShouldDecompile(MemberInfo memberInfo);

        public virtual void RegisterDecompileableMember(MemberInfo property)
        {
            throw new NotImplementedException("The method RegisterDecompileableMember of Configuration is not implemented. Please implement it in your custom configuration class.");
        }
    }
}