
using System.ComponentModel.DataAnnotations;

namespace DelegateDecompiler.EntityFramework.Tests.EfItems.Abstracts
{
    public abstract class LivingBeeing
    {
        [Key]
        public int Id { get; set; }

        [Computed]
        public abstract string Species { get; }

        public int Age { get; set; }
    }
}
