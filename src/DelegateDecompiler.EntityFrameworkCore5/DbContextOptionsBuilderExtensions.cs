using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace DelegateDecompiler.EntityFrameworkCore;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder AddDelegateDecompiler(this DbContextOptionsBuilder builder) =>
        builder.ReplaceService<IAsyncQueryProvider, DelegateDecompileEntityQueryProvider>();
}