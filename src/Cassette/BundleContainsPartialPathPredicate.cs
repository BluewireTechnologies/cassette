using System;
using System.IO;
using Cassette.Utilities;

namespace Cassette
{
    abstract class BundleContainsPartialPathPredicate : IBundleVisitor
    {
        public static BundleContainsPartialPathPredicate CreateFor(string originalPath)
        {
            if (originalPath.IsUrl()) return new AbsolutePredicate(originalPath, originalPath);
            if (originalPath.StartsWith("~")) return new AbsolutePredicate(originalPath, originalPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            return new RelativePredicate(originalPath);
        }

        public bool Result { get; private set; }
        public abstract void Visit(Bundle bundle);
        public abstract void Visit(IAsset asset);

        class AbsolutePredicate : BundleContainsPartialPathPredicate
        {
            public AbsolutePredicate(string originalPath, string normalizedPath)
            {
                this.originalPath = originalPath;
                this.normalizedPath = normalizedPath;
            }

            readonly string originalPath;
            readonly string normalizedPath;

            public override void Visit(Bundle bundle)
            {
                if (Result) return;
                if (IsMatch(bundle.Path))
                {
                    Result = true;
                }
            }

            public override void Visit(IAsset asset)
            {
                if (Result) return;
                if (IsMatch(asset.Path) || IsPartialAssetPathMatch(asset.Path))
                {
                    Result = true;
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

            bool IsMatch(string path)
            {
                return PathUtilities.PathsEqual(path, normalizedPath);
            }
        }

        class RelativePredicate : BundleContainsPartialPathPredicate
        {
            public RelativePredicate(string path)
            {
                originalPath = path;
                originalPathWithoutTrailingSlashes = path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }

            readonly string originalPath;
            string localPath;

            readonly string originalPathWithoutTrailingSlashes;

            public override void Visit(Bundle bundle)
            {
                if (Result) return;

                // Within this bundle, match assets according to this resolved path:
                localPath = PathUtilities.CombineWithForwardSlashes(bundle.Path, originalPathWithoutTrailingSlashes);

                if (IsMatch(bundle.Path))
                {
                    Result = true;
                }
            }

            public override void Visit(IAsset asset)
            {
                if (Result) return;
                if (IsMatch(asset.Path) || IsPartialAssetPathMatch(asset.Path))
                {
                    Result = true;
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

            bool IsMatch(string path)
            {
                return PathUtilities.PathsEqual(path, localPath);
            }
        }
    }
}