namespace DelegateDecompiler.EntityFramework.Tests.EfItems.Abstracts
{
    public class Cat : Feline
    {
        public override string Species => base.Species + " silvestris";

        public override bool IsPet => true;

        public override bool IsAdoptedBy(Person person)
        {
            //simple override to check wether call to base method are supported
            return base.IsAdoptedBy(person);
        }
    }
}