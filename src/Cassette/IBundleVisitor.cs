namespace Cassette
{
    /// <summary>
    /// A visitor that traverses a bundle and optionally its assets.
    /// </summary>
    public interface IBundleVisitor
    {
        bool Visit(Bundle bundle);

        void Visit(IAsset asset);
    }
}
