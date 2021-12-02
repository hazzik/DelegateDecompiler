namespace DelegateDecompiler.EntityFramework.Tests.EfItems.Abstracts
{
#if EF_CORE
    public class AtlanticCod : Fish<int>
    {
        public override string Species => "Gadus morhua";
    }
#endif
}
