namespace DelegateDecompiler.EntityFramework.Tests.EfItems.Abstracts
{
#if EF_CORE
    public abstract class Fish : LivingBeeing
    {
        [Computed]
        public abstract string Group { get; }
    }

    public abstract class Fish<T> : Fish
    {
        public override string Group => "Fish";
    }
#endif
}
