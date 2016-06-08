using System;
using System.IO;
using Cassette.Utilities;

namespace Cassette
{
    class BundleContainsPartialPathPredicate : IBundleVisitor
    {
        public BundleContainsPartialPathPredicate(string path)
        {
            originalPath = path;
        }

        readonly string originalPath;
        string normalizedPath;
        bool isFound;

        public bool Result
        {
            get { return isFound; }
        }

        void IBundleVisitor.Visit(Bundle bundle)
        {
            if (isFound) return;
            normalizedPath = originalPath.IsUrl() ? originalPath : NormalizePath(originalPath, bundle);

            if (IsMatch(bundle.Path))
            {
                isFound = true;
            }
        }

        void IBundleVisitor.Visit(IAsset asset)
        {
            if (isFound) return;
            if (IsMatch(asset.Path) || IsPartialAssetPathMatch(asset.Path))
            {
                isFound = true;
            }
        }

        /// <summary>
        /// Looking for "~/bundle/sub" can match "~/bundle/sub/asset.js"
        /// </summary>
        bool IsPartialAssetPathMatch(string assetPath)
        {
            if (assetPath.Length < originalPath.Length) return false;

            return assetPath.StartsWith(originalPath, StringComparison.OrdinalIgnoreCase);
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