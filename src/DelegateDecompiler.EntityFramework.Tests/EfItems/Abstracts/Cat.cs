namespace DelegateDecompiler.EntityFramework.Tests.EfItems.Abstracts
{
    public class Cat : Feline
    {
        public override string Species => base.Species + " domesticus";
        public override bool IsPet => true;
    }
}