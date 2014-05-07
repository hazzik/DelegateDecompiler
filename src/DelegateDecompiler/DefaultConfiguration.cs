using System.Reflection;

namespace DelegateDecompiler
{
    public class DefaultConfiguration : Configuration
    {
        public override bool ShouldDecompile(MemberInfo memberInfo)
        {
            return memberInfo.GetCustomAttributes(typeof (DecompileAttribute), true).Length > 0 ||
                   memberInfo.GetCustomAttributes(typeof (ComputedAttribute), true).Length > 0;
        }
    }
}