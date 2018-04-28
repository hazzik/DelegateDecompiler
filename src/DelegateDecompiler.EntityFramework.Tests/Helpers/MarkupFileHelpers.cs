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
            string path;
            if (GetMarkupFileDirectory(Environment.CurrentDirectory, alternateTestDir, out path) ||
                GetMarkupFileDirectory(Path.GetDirectoryName(new Uri(typeof(MarkupFileHelpers).Assembly.CodeBase).LocalPath), alternateTestDir, out path))
            {
                return path;
            }

            throw new Exception("bad news guys. Not the expected path");
        }

        static bool GetMarkupFileDirectory(string pathToManipulate, string alternateTestDir, out string path)
        {
            var endings = new[]
            {
                @"\bin\debug",
                @"\bin\debug\net45",
                @"\bin\debug\netcoreapp2.0",
                @"\bin\release",
                @"\bin\release\net45",
                @"\bin\release\netcoreapp2.0"
            };
            foreach (var ending in endings)
            {
                if (pathToManipulate.EndsWith(ending, StringComparison.InvariantCultureIgnoreCase))
                {
                    path = pathToManipulate.Substring(0, pathToManipulate.Length - ending.Length) + alternateTestDir;
                    return true;
                }
            }
            path = null;
            return false;
        }
    }
}
