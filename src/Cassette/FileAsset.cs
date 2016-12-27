using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Cassette.Caching;
using Cassette.IO;
using Cassette.Utilities;

namespace Cassette
{
    public class FileAsset : AssetBase
    {
        public FileAsset(IFile sourceFile, Bundle parentBundle)
        {
            this.parentBundle = parentBundle;
            this.sourceFile = sourceFile;

            hash = new Lazy<byte[]>(ComputeHash); // Avoid file IO until the hash is actually needed.
        }

        readonly Bundle parentBundle;
        readonly IFile sourceFile;
        readonly Lazy<byte[]> hash;
        readonly List<AssetReference> references = new List<AssetReference>();

        public override string Path
        {
            get { return sourceFile.FullPath; }
        }

        public override byte[] Hash
        {
            get { return hash.Value; }
        }

        public override Type AssetCacheValidatorType
        {
            get { return typeof(FileAssetCacheValidator); }
        }

        public override IEnumerable<AssetReference> References
        {
            get { return references; }
        }

        public override void AddReference(string assetRelativePath, int lineNumber)
        {
            if (assetRelativePath.IsUrl())
            {
                AddUrlReference(assetRelativePath, lineNumber);
            }
            else
            {
                var appRelativeFilename = PathUtilities.AppRelative(sourceFile.Directory.FullPath, assetRelativePath);
                AddBundleReference(appRelativeFilename, lineNumber);
            }
        }

        void AddBundleReference(string appRelativeFilename, int lineNumber)
        {
            var type = parentBundle.ContainsPath(appRelativeFilename)
                           ? AssetReferenceType.SameBundle
                           : AssetReferenceType.DifferentBundle;
            references.Add(new AssetReference(Path, appRelativeFilename, lineNumber, type));
        }

        void AddUrlReference(string url, int sourceLineNumber)
        {
            references.Add(new AssetReference(Path, url, sourceLineNumber, AssetReferenceType.Url));
        }

        public override void AddRawFileReference(string relativeFilename)
        {
            var appRelativeFilename = PathUtilities.AppRelative(sourceFile.Directory.FullPath, relativeFilename);

            var alreadyExists = references.Any(r => r.ToPath.Equals(appRelativeFilename, StringComparison.OrdinalIgnoreCase));
            if (alreadyExists) return;

            references.Add(new AssetReference(Path, appRelativeFilename, -1, AssetReferenceType.RawFilename));
        }

        byte[] ComputeHash()
        {
            using (var sha1 = SHA1.Create())
            using (var stream = OpenStream())
            {
                return sha1.ComputeHash(stream);
            }
        }

        protected override Stream OpenStreamCore()
        {
            return sourceFile.OpenRead();
        }
    }
}