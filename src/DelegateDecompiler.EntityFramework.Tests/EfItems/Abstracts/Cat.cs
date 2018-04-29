﻿using System.Linq;

namespace DelegateDecompiler.EntityFramework.Tests.EfItems.Abstracts
{
    public class Cat : Feline
    {
        public override string Species => base.Species + " silvestris";

        public override bool IsPet => true;

        [Computed]
        public bool IsAdopted(IQueryable<Person> persons)
        {
            return persons.Any(p => p.Animals.OfType<Cat>().Contains(this));
        }
    }
}