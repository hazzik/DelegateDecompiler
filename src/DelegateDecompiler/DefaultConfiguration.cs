using System.Collections.Generic;
using System.Reflection;

namespace DelegateDecompiler
{
    public class DefaultConfiguration : Configuration
    {
        readonly HashSet<MemberInfo> computedMembers = new HashSet<MemberInfo>();

        public override bool ShouldDecompile(MemberInfo memberInfo)
        {
            return computedMembers.Contains(memberInfo) ||
                   memberInfo.GetCustomAttributes(typeof(DecompileAttribute), true).Length > 0 ||
                   memberInfo.GetCustomAttributes(typeof(ComputedAttribute), true).Length > 0;
        }

        public override void RegisterDecompileableMember(MemberInfo property)
        {
            computedMembers.Add(property);
        }
    }
}