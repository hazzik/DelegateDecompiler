namespace DelegateDecompiler.EntityFramework.Tests.EfItems.Abstracts
{
    public abstract class Animal : LivingBeeing
    {
        public Person Owner { get; set; }

        [Computed]
        public abstract bool IsPet { get; }
    }
}
