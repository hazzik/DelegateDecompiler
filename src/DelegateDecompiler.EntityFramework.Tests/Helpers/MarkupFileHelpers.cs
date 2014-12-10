// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System;
using System.IO;

namespace DelegateDecompiler.EntityFramework.Tests.Helpers
{
    internal static class MarkupFileHelpers
    {
        private const string MarkupDirectoryName = @"\GeneratedDocumentation";

        //-------------------------------------------------------------------

        public static string SearchMarkupForSingleFileReturnFilePath(string searchPattern)
        {
            var fileList = SearchMarkupForManyFilesReturnFilePaths(searchPattern);

            if (fileList.Length != 1)
                throw new FileNotFoundException(string.Format("The searchString {0} found {1} file. Either not there or ambiguous",
                    searchPattern, fileList.Length));

            return fileList[0];
        }

        public static string SearchMarkupForSingleFileReturnContent(string searchPattern)
        {
            return File.ReadAllText(SearchMarkupForSingleFileReturnFilePath(searchPattern));
        }

        public static void WriteTextToFileInMarkup(string filenameWithExtension, string content)
        {
            File.WriteAllText(Path.Combine(GetMarkupFileDirectory(), filenameWithExtension), content);
        }

        public static string[] SearchMarkupForManyFilesReturnFilePaths(string searchPattern = "")
        {
            var directory = GetMarkupFileDirectory();
            if (searchPattern.Contains(@"\"))
            {
                //Has subdirectory in search pattern, so change directory
                directory = Path.Combine(directory, searchPattern.Substring(0, searchPattern.LastIndexOf('\\')));
                searchPattern = searchPattern.Substring(searchPattern.LastIndexOf('\\')+1);
            }

            string[] fileList = Directory.GetFiles(directory, searchPattern);

            return fileList;
        }

        public static string GetMarkupFileDirectory(string alternateTestDir = MarkupDirectoryName)
        {
            string pathToManipulate = Environment.CurrentDirectory;
            const string debugEnding = @"\bin\debug";
            const string releaseEnding = @"\bin\release";

            if (pathToManipulate.EndsWith(debugEnding, StringComparison.InvariantCultureIgnoreCase))
                return pathToManipulate.Substring(0, pathToManipulate.Length - debugEnding.Length) + alternateTestDir;
            if (pathToManipulate.EndsWith(releaseEnding, StringComparison.InvariantCultureIgnoreCase))
                return pathToManipulate.Substring(0, pathToManipulate.Length - releaseEnding.Length) + alternateTestDir;   
                
            throw new Exception("bad news guys. Not the expected path");

        }

    }
}
