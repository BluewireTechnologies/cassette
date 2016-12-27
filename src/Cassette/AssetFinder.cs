using Cassette.Utilities;

namespace Cassette
{
    class AssetFinder : IBundleVisitor
    {
        readonly string pathToFind;

        public AssetFinder(string pathToFind)
        {
            this.pathToFind = pathToFind;
        }

        public IAsset FoundAsset { get; private set; }

        public bool Visit(Bundle bundle) => FoundAsset == null; // Shortcircuit; already found.

        public void Visit(IAsset asset)
        {
            if (FoundAsset != null) return;

            if (PathUtilities.PathsEqual(asset.Path, pathToFind))
            {
                FoundAsset = asset;
            }
        }
    }
}
