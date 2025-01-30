using System;
using System.Linq;
using Cassette.BundleProcessing;

namespace Cassette.HtmlTemplates
{
    public class WrapJavaScriptHtmlTemplates : IBundleProcessor<HtmlTemplateBundle>
    {
        public void Process(HtmlTemplateBundle bundle)
        {
            if (bundle.Assets.Count == 0) return;
            if (bundle.Assets.Count > 1) throw new ArgumentException("WrapJavaScriptHtmlTemplates should only process a bundle where the assets have been concatenated.", "bundle");

            var transformer = new WrapJavaScriptHtmlTemplatesTransformer(bundle.ContentType);
            bundle.Assets.Single().AddAssetTransformer(transformer);
        }
    }
}