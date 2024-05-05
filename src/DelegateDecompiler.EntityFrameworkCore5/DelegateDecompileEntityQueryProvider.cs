using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace DelegateDecompiler.EntityFrameworkCore;

[SuppressMessage("Usage", "EF1001:Internal EF Core API usage.")]
class DelegateDecompileEntityQueryProvider(IQueryCompiler queryCompiler) : EntityQueryProvider(queryCompiler)
{
    public override TResult Execute<TResult>(Expression expression) =>
        base.Execute<TResult>(expression.Decompile().Optimize());

    public override object Execute(Expression expression) =>
        base.Execute(expression.Decompile().Optimize());

    public override TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default) =>
        base.ExecuteAsync<TResult>(expression.Decompile().Optimize(), cancellationToken);
}