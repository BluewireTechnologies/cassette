using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cassette.Utilities
{
    /// <summary>
    /// Utility methods for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a new stream containing the contents of the string, using UTF-8 encoding.
        /// The stream's Position property is set to zero.
        /// </summary>
        /// <param name="s">The string to convert into a stream.</param>
        /// <returns>A new stream.</returns>
        public static Stream AsStream(this string s)
        {
            var source = new MemoryStream();
            var writer = new StreamWriter(source);
            writer.Write(s);
            writer.Flush();
            source.Position = 0;
            return source;
        }

        public static bool StartsWithCharacter(this string s, char c)
        {
            if (s.Length == 0) return false;
            return s[0] == c;
        }

        public static bool IsUrl(this string s)
        {
            if (s.Length < 2) return false;
            if (s[0] == '/' && s[1] == '/') return true;
            if (s.Length < 6) return false;
            if (s[4] == ':' && s.StartsWith("http:", StringComparison.OrdinalIgnoreCase)) return true;
            if (s[5] == ':' && s.StartsWith("https:", StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        public static string JoinStrings(this IEnumerable<string> strings, string separator)
        {
            using (var iter = strings.GetEnumerator())
            {
                if (!iter.MoveNext()) return string.Empty;

                var sb = new StringBuilder();
                sb.Append(iter.Current);
                while (iter.MoveNext())
                {
                    sb.Append(separator);
                    sb.Append(iter.Current);
                }
                return sb.ToString();
            }
        }

        public static void AppendWithSeparator<T>(this IEnumerable<T> values, StringBuilder builder, string separator, Action<StringBuilder, T> append)
        {
            using (var iter = values.GetEnumerator())
            {
                if (!iter.MoveNext()) return;

                append(builder, iter.Current);
                while (iter.MoveNext())
                {
                    builder.Append(separator);
                    append(builder, iter.Current);
                }
            }
        }

        public static bool IsNullOrWhiteSpace(this string s)
        {
#if NET35
            return String.IsNullOrEmpty(s) || s.Trim().Length == 0;
#else
            return String.IsNullOrWhiteSpace(s);
#endif
        }
    }
}