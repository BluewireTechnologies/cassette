﻿using System.Linq;
using Cassette.Scripts;
using Cassette.Scripts.Manifests;
using Cassette.Stylesheets;
using Cassette.Stylesheets.Manifests;
using Should;
using Xunit;

namespace Cassette.Manifests
{
    public class CassetteManifest_Test
    {
        [Fact]
        public void NewCassetteManifestHasEmptyButNotNullBundleManifestList()
        {
            var manifest = new CassetteManifest();
            manifest.BundleManifests.ShouldNotBeNull();
            manifest.BundleManifests.ShouldBeEmpty();
        }

        [Fact]
        public void EmptyManifestsAreEqual()
        {
            var manifest1 = new CassetteManifest();
            var manifest2 = new CassetteManifest();
            manifest1.ShouldEqual(manifest2);
        }

        [Fact]
        public void CassetteManifestsWithDifferentNumberOfBundleManifestsAreNotEqual()
        {
            var manifest1 = new CassetteManifest();
            var manifest2 = new CassetteManifest
            {
                BundleManifests = { new ScriptBundleManifest { Path = "~", Hash = new byte[0] } }
            };
            manifest1.ShouldNotEqual(manifest2);
        }

        [Fact]
        public void CassetteManifestsAreEqualIfBundleManifestsAreEqual()
        {
            var manifest1 = new CassetteManifest
            {
                BundleManifests = { new ScriptBundleManifest { Path = "~", Hash = new byte[0] } }
            };
            var manifest2 = new CassetteManifest
            {
                BundleManifests = { new ScriptBundleManifest { Path = "~", Hash = new byte[0] } }
            };
            manifest1.ShouldEqual(manifest2);
        }

        [Fact]
        public void CreateBundlesReturnsOneBundlePerBundleManifest()
        {
            var manifest = new CassetteManifest(new BundleManifest[]
            {
                new ScriptBundleManifest { Path = "~/js", Hash = new byte[0] },
                new StylesheetBundleManifest { Path = "~/css", Hash = new byte[0] }
            });

            var bundles = manifest.CreateBundles().ToArray();

            bundles.Length.ShouldEqual(2);
            bundles[0].ShouldBeType<ScriptBundle>();
            bundles[1].ShouldBeType<StylesheetBundle>();
        }
    }
}