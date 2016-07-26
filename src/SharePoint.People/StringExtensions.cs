using System;

namespace SharePoint.People
{
    internal static class StringExtensions
    {
        internal static string[] SplitEmpty(this string s, Char c)
        {
            if (string.IsNullOrEmpty(s))
                return null;
            return s.Split(new[] { c }, StringSplitOptions.RemoveEmptyEntries);

        }
    }
}