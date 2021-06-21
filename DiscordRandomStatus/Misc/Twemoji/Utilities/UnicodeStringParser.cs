// Code from Twemoji.NET by Petr Sedláček https://github.com/petrspelos/twemojiNet

using System;
using System.Collections.Generic;
using System.Linq;

namespace TwemojiNet.Utilities
{
    internal static class UnicodeStringParser
    {
        public static IEnumerable<string> ToCodePoints(string str)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            var codePoints = new List<int>(str.Length);
            for (int i = 0; i < str.Length; i++)
            {
                codePoints.Add(Char.ConvertToUtf32(str, i));
                if (Char.IsHighSurrogate(str[i]))
                    i += 1;
            }

            return codePoints.Select(i => $"{i:X4}".ToLower());
        }

        public static bool IsValidEmoji(string codepoint)
        {
            return Constants.StandardCodepoints.Contains(codepoint);
        }
    }
}
