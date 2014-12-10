// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System;
using System.IO;
using DelegateDecompiler.EntityFramework.Tests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup01Infrastructure
{
    class Test15MarkupFileAccess
    {

        [Test]
        public void TestCheckRandomNamedFileIsNotThere()
        {

            //SETUP

            //ATTEMPT
            var ex =
                Assert.Throws<FileNotFoundException>(
                    () => MarkupFileHelpers.SearchMarkupForSingleFileReturnFilePath(Guid.NewGuid().ToString()));

            //VERIFY
        }

        [Test]
        public void TestCheckDocumentationHeaderTextIsThere()
        {

            //SETUP

            //ATTEMPT
            var filepaths = MarkupFileHelpers.SearchMarkupForManyFilesReturnFilePaths(MasterEnvironment.HeaderTestFilename);

            //VERIFY
            filepaths.Length.ShouldEqual(1);
        }

        [Test]
        public void TestReadDocumentationHeaderTextOk()
        {

            //SETUP

            //ATTEMPT
            var content = MarkupFileHelpers.SearchMarkupForSingleFileReturnContent(MasterEnvironment.HeaderTestFilename);

            //VERIFY
            content.Length.ShouldBeGreaterThan(10);
        }

        [Test]
        public void TestWriteTestFileAndReadOk()
        {

            //SETUP
            const string testFilename = "Test file - please ignore.txt";
            const string testContent = "This is a Unit Test of the MarkupFileHelpers";

            //ATTEMPT
            MarkupFileHelpers.WriteTextToFileInMarkup(testFilename, testContent);
            var content = MarkupFileHelpers.SearchMarkupForSingleFileReturnContent(testFilename);

            //VERIFY
            content.ShouldEqual(testContent);
        }

    }
}
