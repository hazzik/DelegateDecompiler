namespace DelegateDecompiler.EntityFramework.Tests.EfItems.Abstracts
{
#if EF_CORE
    public class WhiteShark : Fish<string>
    {
        public override string Species => "Carcharodon carcharias";
    }
#endif
}
