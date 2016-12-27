using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cassette.BundleProcessing;
using Cassette.Utilities;

namespace Cassette
{
    public class AssetCollection : IEnumerable<IAsset>
    {
        private readonly object lock_indexed = new object();
        private List<IAsset> inner;
        private volatile AssetIndex indexes;

        public AssetCollection() : this(new List<IAsset>())
        {
        }

        public AssetCollection(IEnumerable<IAsset> assets)
        {
            inner = assets.ToList();
        }

        public int Count => inner.Count;

        public void Add(IAsset asset)
        {
            inner.Add(asset);
            lock(lock_indexed) indexes = null;
        }

        public void ReplaceWith(IAsset asset)
        {
            Replace(new List<IAsset> { asset });
        }

        public void ReplaceWith(IEnumerable<IAsset> assets)
        {
            Replace(assets.ToList());
        }

        public IAsset FindByPath(string path)
        {
            return GetIndexes().FindByPath(path);
        }

        public bool ContainsPath(string path)
        {
            return GetIndexes().ContainsPath(path);
        }

        public bool ContainsPathPrefix(string pathPrefix)
        {
            return GetIndexes().ContainsPathPrefix(pathPrefix);
        }

        public IEnumerator<IAsset> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private AssetIndex GetIndexes()
        {
            lock (lock_indexed)
            {
                if (indexes != null) return indexes;
                var visitor = new IndexingVisitor();
                foreach (var asset in inner) asset.Accept(visitor);
                indexes = new AssetIndex(visitor.IndexByPath);
                return indexes;
            }
        }

        class IndexingVisitor : IBundleVisitor
        {
            public Dictionary<string, IAsset> IndexByPath { get; } = new Dictionary<string, IAsset>(new CaseInsensitivePathEqualityComparer());

            public bool Visit(Bundle bundle) => true;

            public void Visit(IAsset asset)
            {
                if (asset is ConcatenatedAsset) return;
                IndexByPath.Add(asset.Path, asset);
            }
        }

        private void Replace(List<IAsset> assets)
        {
            lock (lock_indexed)
            {
                inner = assets;
                indexes = null;
            }
        }
    }
}
