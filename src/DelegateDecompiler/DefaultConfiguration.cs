using System.Collections.Generic;
using System.Reflection;

namespace DelegateDecompiler
{
    public class DefaultConfiguration : Configuration
    {
        private readonly HashSet<MemberInfo> computedMembers = new HashSet<MemberInfo>();

        public override bool ShouldDecompile(MemberInfo memberInfo)
        {
            // favor lookup to reflection
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