using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cassette
{
    public class AssetCollection : IEnumerable<IAsset>
    {
        private List<IAsset> inner;

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
        }

        public void ReplaceWith(IAsset asset)
        {
            Replace(new List<IAsset> { asset });
        }

        public void ReplaceWith(IEnumerable<IAsset> assets)
        {
            Replace(assets.ToList());
        }

        public IEnumerator<IAsset> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Replace(List<IAsset> assets)
        {
            inner = assets;
        }
    }
}
