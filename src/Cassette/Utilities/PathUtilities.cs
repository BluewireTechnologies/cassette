using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cassette.Utilities
{
    public static class PathUtilities
    {
        public static string Combine(params string[] paths)
        {
#if !NET35
            return Path.Combine(paths);
#else
            return paths.Aggregate(Path.Combine);
#endif
        }
    
        public static string CombineWithForwardSlashes(params string[] paths)
        {
            return Combine(paths).Replace('\\', '/');
        }


        /// <summary>
        /// Resolve relative components of paths to canonicalise the input.
        /// </summary>
        public static string NormalizePath(string path)
        {
            if (path.Length == 0) throw new ArgumentException("Path cannot be empty.", nameof(path));
            var slashes = new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
            var parts = path.Split(slashes, StringSplitOptions.RemoveEmptyEntries);

            var resolved = ResolveRelativePath(path, parts);

            var isNetworkSharePath = path[0] == '\\' && path[1] == '\\'; // StartsWith(@"\\");
            if (isNetworkSharePath)
            {
                return @"\\" + resolved.JoinStrings(@"\");
            }
            else
            {
                var returnPath = resolved.JoinStrings("/");
                return (path.StartsWithCharacter('/') ? "/" : "") + returnPath;
            }
        }

        static IEnumerable<string> ResolveRelativePath(string path, string[] parts)
        {
            var stack = new Stack<string>();
            foreach (var part in parts)
            {
                if (part.StartsWithCharacter('.'))
                {
                    if (part.Length == 1) continue;
                    if (part[1] == '.' && part.Length == 2)
                    {
                        if (stack.Count <= 0) throw new ArgumentException($"Too many \"..\" in the path \"{path}\".");
                        stack.Pop();
                        continue;
                    }
                }
                stack.Push(part);
            }
            return stack.Reverse();
        }

        public static string AppRelative(string path)
        {
            if (path.IsUrl()) return path;
            if (path.Length == 0) return NormalizePath("~/");

            if (path.StartsWithCharacter('~')) return NormalizePath(path);
            if (path.StartsWithCharacter('/')) return NormalizePath("~" + path);
            return NormalizePath("~/" + path);
        }

        public static string AppRelative(string basePath, string path)
        {
            if (path.Length == 0) return NormalizePath(basePath);
            if (path.IsUrl()) return path;
            if (path.StartsWithCharacter('~')) return NormalizePath(path);
            if (path.StartsWithCharacter('/')) return NormalizePath("~" + path);
            return NormalizePath(CombineWithForwardSlashes(basePath, path));
        }
    }
}