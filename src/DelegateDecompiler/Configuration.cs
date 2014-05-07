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
            if (instance != null)
                throw new InvalidOperationException("DelegateDecompiler has been configured already");
            
            instance = cfg;
        }

        public abstract bool ShouldDecompile(MemberInfo memberInfo);
    }
}