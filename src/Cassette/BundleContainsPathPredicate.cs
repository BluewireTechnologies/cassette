using System;
using System.Diagnostics;
using System.IO;
using Cassette.Utilities;

namespace Cassette
{
    class BundleContainsPathPredicate
    {
        public BundleContainsPathPredicate(string path)
        {
            if (String.IsNullOrEmpty(path)) throw new ArgumentException(nameof(path));
            originalPath = path;
        }

        readonly string originalPath;

        public bool EvaluateFor(Bundle bundle)
        {
            var normalizedPath = originalPath.IsUrl() ? originalPath : NormalizePath(originalPath, bundle);

            if (new CaseInsensitivePathEqualityComparer().Equals(bundle.Path, normalizedPath)) return true;
            return bundle.Assets.ContainsPath(normalizedPath);
        }

        static string NormalizePath(string path, Bundle bundle)
        {
            var trimmedPath = path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (trimmedPath.Length == 0) return bundle.Path;
            if (trimmedPath.StartsWithCharacter('~')) return trimmedPath;
            return PathUtilities.CombineWithForwardSlashes(bundle.Path, trimmedPath);
        }
    }
}
