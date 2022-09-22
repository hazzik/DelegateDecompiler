// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com
using DelegateDecompiler.EntityFramework.Tests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup99SaveResults
{
    class Test99SaveResults
    {
#if !DEBUG
        [Test]
        public void EndRun()
        {
            MasterEnvironment.UpdateDocumentationIfLooksLikeAFullRun();
            //var markupResults = MasterEnvironment.ResultsAsMarkup( OutputVersions.Summary);
            //Console.Write(markupResults);
        }
#endif
    }
}
