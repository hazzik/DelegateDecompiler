namespace DelegateDecompiler.EntityFramework.Tests.EfItems.Abstracts
{
    public abstract class Animal : LivingBeeing
    {
        [Computed]
        public abstract bool IsPet { get; }
    }
}
