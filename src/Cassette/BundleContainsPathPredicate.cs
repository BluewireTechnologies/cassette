using System.IO;
using Cassette.Utilities;

namespace Cassette
{
    class BundleContainsPathPredicate : IBundleVisitor
    {
        public BundleContainsPathPredicate(string path)
        {
            originalPath = path;
        }

        readonly string originalPath;
        string normalizedPath;
        bool isFound;

        public bool Result => isFound;

        bool IBundleVisitor.Visit(Bundle bundle)
        {
            if (isFound) return false; // Shortcircuit; already found.
            normalizedPath = originalPath.IsUrl() ? originalPath : NormalizePath(originalPath, bundle);

            if (IsMatch(bundle.Path))
            {
                isFound = true;
            }
            return true;
        }

        void IBundleVisitor.Visit(IAsset asset)
        {
            if (isFound) return;
            if (IsMatch(asset.Path))
            {
                isFound = true;
            }
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

        bool IsMatch(string path)
        {
            return PathUtilities.PathsEqual(path, normalizedPath);
        }
    }
}