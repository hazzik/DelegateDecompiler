namespace DelegateDecompiler.EntityFramework.Tests.EfItems.Abstracts
{
    public interface IPerson
    {
        [Computed]
        string FullNameHandleNull { get; }
    }
}
