// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DelegateDecompiler.EfTests.Helpers
{
    static class SplitterExtension
    {
        private static readonly Regex Reg = new Regex("([a-z,0-9](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", RegexOptions.Compiled);

        private static readonly Tuple<string,string>[] ExceptionsToSplitting = new Tuple<string,string>[]
        {
            new Tuple<string, string>("ToString", "To String"),
            new Tuple<string, string>("DateTime", "Date Time"),
        };

        /// <summary>
        /// This splits up a string based on capital letters
        /// e.g. "MyAction" would become "My Action" and "My10Action" would become "My10 Action"
        /// </summary>
        /// <param name="str"></param>
        /// <param name="addBoldMarkDown">If true then surrounds the fina output with **, i.e. bold MarkDown</param>
        /// <returns></returns>
        public static string SplitCamelCase(this string str, bool addBoldMarkDown = false)
        {
            var splitTest = Reg.Replace(str, "$1 ");
            var splitString = ExceptionsToSplitting.Aggregate(splitTest, (current, tuple) => current.Replace(tuple.Item2, tuple.Item1));
            return addBoldMarkDown ? "**" + splitString + "**" : splitString;
        }
    }
}
