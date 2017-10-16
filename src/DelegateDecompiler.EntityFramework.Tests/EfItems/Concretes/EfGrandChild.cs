// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com
namespace DelegateDecompiler.EntityFramework.Tests.EfItems.Concretes
{
    public class EfGrandChild
    {

        public int EfGrandChildId { get; set; }

        public bool GrandChildBool { get; set; }

        public int GrandChildInt { get; set; }

        public double GrandChildDouble { get; set; }

        public string GrandChildString { get; set; }

        //-------------------------------------------
        //relationships

        public int EfChildId { get; set; }
    }
}
