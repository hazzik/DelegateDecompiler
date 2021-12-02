namespace DelegateDecompiler.EntityFramework.Tests.EfItems.Abstracts
{
#if EF_CORE
    public abstract class Fish : LivingBeeing
    {
    }

    public abstract class Fish<T> : Fish
    {
    }
#endif
}
