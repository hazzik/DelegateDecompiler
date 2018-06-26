namespace DelegateDecompiler.EntityFramework.Tests.EfItems.Abstracts
{
    public abstract class Animal : LivingBeeing
    {
        [Computed]
        public abstract bool IsPet { get; }

        [Computed]
        public virtual bool IsAdoptedBy(Person person)
        {
            return person.Animals.Contains(this);
        }
    }
}
