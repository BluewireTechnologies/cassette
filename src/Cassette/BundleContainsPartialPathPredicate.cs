using System;
using System.IO;
using Cassette.Utilities;

namespace Cassette
{
    abstract class BundleContainsPartialPathPredicate
    {
        public static BundleContainsPartialPathPredicate CreateFor(string originalPath)
        {
            if (String.IsNullOrEmpty(originalPath)) throw new ArgumentException(nameof(originalPath));
            if (originalPath.IsUrl()) return new AbsolutePredicate(originalPath, originalPath);
            if (originalPath.StartsWithCharacter('~')) return new AbsolutePredicate(originalPath, originalPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            return new RelativePredicate(originalPath);
        }
        
        public abstract bool EvaluateFor(Bundle bundle);

        class AbsolutePredicate : BundleContainsPartialPathPredicate
        {
            public AbsolutePredicate(string originalPath, string normalizedPath)
            {
                this.originalPath = originalPath;
                this.normalizedPath = normalizedPath;
            }

            readonly string originalPath;
            readonly string normalizedPath;

            public override bool EvaluateFor(Bundle bundle)
            {
                if (new CaseInsensitivePathEqualityComparer().Equals(bundle.Path, normalizedPath)) return true;
                if (bundle.Assets.ContainsPath(normalizedPath)) return true;
                if (bundle.Assets.ContainsPathPrefix(originalPath)) return true;

                return false;
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
            readonly string originalPathWithoutTrailingSlashes;

            public override bool EvaluateFor(Bundle bundle)
            {
                // Within this bundle, match assets according to this resolved path:
                var localPath = PathUtilities.CombineWithForwardSlashes(bundle.Path, originalPathWithoutTrailingSlashes);

                if (new CaseInsensitivePathEqualityComparer().Equals(bundle.Path, localPath)) return true;
                if (bundle.Assets.ContainsPath(localPath)) return true;
                if (bundle.Assets.ContainsPathPrefix(originalPath)) return true;

                return false;
            }
        }
    }
}
