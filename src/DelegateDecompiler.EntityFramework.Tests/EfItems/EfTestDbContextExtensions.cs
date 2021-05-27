using System.Linq;

namespace DelegateDecompiler.EntityFramework.Tests.EfItems
{
    public static class EfTestDbContextExtensions
    {
        [Computed]
        public static int GetFirstChildIdByParent(this EfTestDbContext context, int parentId)
        {
            return context.EfChildren.Where(e => e.EfParentId == parentId).Select(e => e.EfChildId).FirstOrDefault();
        }
    }
}
