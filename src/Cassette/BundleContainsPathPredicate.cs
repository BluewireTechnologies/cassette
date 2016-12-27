using System.IO;
using Cassette.Utilities;

namespace Cassette
{
    class BundleContainsPathPredicate
    {
        public BundleContainsPathPredicate(string path)
        {
            originalPath = path;
        }

        readonly string originalPath;

        public bool EvaluateFor(Bundle bundle)
        {
            var normalizedPath = originalPath.IsUrl() ? originalPath : NormalizePath(originalPath, bundle);

            if (new CaseInsensitivePathEqualityComparer().Equals(bundle.Path, normalizedPath)) return true;
            return bundle.Assets.ContainsPath(normalizedPath);
        }

        string NormalizePath(string path, Bundle bundle)
        {
            path = path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (path.StartsWith("~"))
            {
                return path;
            }
            else
            {
                return PathUtilities.CombineWithForwardSlashes(bundle.Path, path);
            }
        }
    }
}
