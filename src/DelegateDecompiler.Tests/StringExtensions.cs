namespace DelegateDecompiler.Tests;

internal static class StringExtensions
{
    public static string NormalizeLineEndings(this string s) => s.Replace("\r\n", "\n");
}