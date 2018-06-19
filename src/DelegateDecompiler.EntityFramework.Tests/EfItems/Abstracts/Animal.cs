using System.Collections.Generic;
using System.Linq;

namespace DelegateDecompiler.EntityFramework.Tests.EfItems.Abstracts
{
    public abstract class Animal : LivingBeeing
    {
        [Computed]
        public abstract bool IsPet { get; }

        [Computed]
        public bool IsAdoptedBy(IEnumerable<Person> persons)
        {
            return persons.Any(p => this.IsAdoptedBy(p));
        }

        [Computed]
        public virtual bool IsAdoptedBy(Person person)
        {
            return person.Animals.Contains(this);
        }
    }
}