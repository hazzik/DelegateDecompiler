using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DelegateDecompiler.EntityFramework.Tests.EfItems.Abstracts
{
    public class Cat : Feline
    {
        public override string Species => base.Species + " silvestris";

        public override bool IsPet => true;

        public static Expression<Func<Cat, bool>> IsAdopted(IEnumerable<Person> persons)
            => c => persons.Any(p => p.Animals.OfType<Cat>().Contains(c));
    }
}