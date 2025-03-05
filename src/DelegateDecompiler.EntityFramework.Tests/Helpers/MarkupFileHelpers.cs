// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System;
using System.IO;

namespace DelegateDecompiler.EntityFramework.Tests.Helpers
{
    internal static class MarkupFileHelpers
    {
        private const string MarkupDirectoryName = "GeneratedDocumentation";

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
            if (searchPattern.Contains(Path.DirectorySeparatorChar.ToString()))
            {
                //Has subdirectory in search pattern, so change directory
                directory = Path.Combine(directory, searchPattern.Substring(0, searchPattern.LastIndexOf(Path.DirectorySeparatorChar)));
                searchPattern = searchPattern.Substring(searchPattern.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            }

            return Directory.GetFiles(directory, searchPattern);
        }

        public static string GetMarkupFileDirectory()
        {
            string path;
            if (GetMarkupFileDirectory(Environment.CurrentDirectory, out path) ||
                GetMarkupFileDirectory(Path.GetDirectoryName(new Uri(typeof(MarkupFileHelpers).Assembly.Location).LocalPath), out path))
            {
                return path;
            }

            throw new Exception("bad news guys. Not the expected path");
        }

        static bool GetMarkupFileDirectory(string pathToManipulate, out string path)
        {
            string[] configs = ["release", "debug"];
            string[] frameworks = ["", "net45", "net8.0", "net9.0", "net10.0"];

            foreach (var conf in configs)
            {
                foreach (var platform in frameworks)
                {
                    var ending = Path.Combine("bin", conf, platform);

                    if (pathToManipulate.EndsWith(ending, StringComparison.InvariantCultureIgnoreCase))
                    {
                        path = Path.Combine(
                            pathToManipulate.Substring(0, pathToManipulate.Length - ending.Length),
                            MarkupDirectoryName);
                        return true;
                    }
                }
            }

            path = null;
            return false;
        }
    }
}
