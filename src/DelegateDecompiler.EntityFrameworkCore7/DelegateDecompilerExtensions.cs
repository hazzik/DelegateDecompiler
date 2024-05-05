using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DelegateDecompiler.EntityFrameworkCore;

public static class DelegateDecompilerExtensions
{
    sealed class DelegateDecompilerInterceptor : IQueryExpressionInterceptor
    {
        public Expression QueryCompilationStarting(Expression queryExpression, QueryExpressionEventData eventData)
        {
            return DecompileExpressionVisitor.Decompile(queryExpression);
        }
    }

    public static DbContextOptionsBuilder AddDelegateDecompiler(this DbContextOptionsBuilder builder)
        => builder.AddInterceptors(new DelegateDecompilerInterceptor());
}