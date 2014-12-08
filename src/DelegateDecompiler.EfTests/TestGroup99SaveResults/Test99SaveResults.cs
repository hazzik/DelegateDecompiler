// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using DelegateDecompiler.EfTests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EfTests.TestGroup99SaveResults
{
    class Test99SaveResults
    {
        [Test]
        public void EndRun()
        {
            MasterEnvironment.UpdateDocumentationIfLooksLikeAFullRun();
            //var markupResults = MasterEnvironment.ResultsAsMarkup( OutputVersions.Summary);
            //Console.Write(markupResults);
        }

    }
}
