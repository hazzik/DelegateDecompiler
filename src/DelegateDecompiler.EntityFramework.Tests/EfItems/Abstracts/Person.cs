using System;
using System.Collections.Generic;

namespace DelegateDecompiler.EntityFramework.Tests.EfItems.Abstracts
{
    public class Person : LivingBeeing
    {
        public override string Species => "Human";

        public string Name { get; set; }

        public DateTime Birthdate { get; set; }

        public ICollection<Animal> Animals { get; set; }
    }
}