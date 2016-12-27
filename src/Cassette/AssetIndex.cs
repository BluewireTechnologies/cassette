using System;
using System.Collections.Generic;
using System.Linq;
using Cassette.Utilities;

namespace Cassette
{
    public class AssetIndex
    {
        private readonly IDictionary<string, IAsset> indexByPath;
        private readonly List<string> orderByPath;

        public AssetIndex(IDictionary<string, IAsset> indexByPath)
        {
            this.indexByPath = indexByPath;
            orderByPath = indexByPath.Keys.ToList();
            orderByPath.Sort(new CaseInsensitivePathEqualityComparer());
        }

        public IAsset FindByPath(string path)
        {
            IAsset asset;
            if (indexByPath.TryGetValue(path, out asset)) return asset;
            return null;
        }

        public bool ContainsPath(string path)
        {
            return indexByPath.ContainsKey(path);
        }

        public bool ContainsPathPrefix(string pathPrefix)
        {
            var index = orderByPath.BinarySearch(pathPrefix, new CaseInsensitivePathEqualityComparer());
            if (index >= 0) return true; // Exact match found.

            var nextLargestIndex = ~index;
            if (nextLargestIndex < orderByPath.Count)
            {
                // First item which compares larger than the requested pathPrefix may start with it.
                var possibleMatch = orderByPath[nextLargestIndex];
                return possibleMatch.StartsWith(pathPrefix, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}
