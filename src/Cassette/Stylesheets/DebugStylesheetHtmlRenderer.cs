using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cassette.Utilities;

namespace Cassette.Stylesheets
{
    class DebugStylesheetHtmlRenderer : IBundleHtmlRenderer<StylesheetBundle>
    {
        public DebugStylesheetHtmlRenderer(IUrlGenerator urlGenerator)
        {
            this.urlGenerator = urlGenerator;
        }

        readonly IUrlGenerator urlGenerator;

        public string Render(StylesheetBundle bundle)
        {
            var conditionalRenderer = new ConditionalRenderer();
            return conditionalRenderer.Render(bundle.Condition, 
                html => RenderStylesheetContent(bundle, html));
        }

        void RenderStylesheetContent(StylesheetBundle bundle, StringBuilder sb)
        {
            GetAssetUrls(bundle)
                .AppendWithSeparator(sb, Environment.NewLine, (b, url) =>
                    b.AppendFormat(HtmlConstants.LinkHtml, url, bundle.HtmlAttributes.CombinedAttributes));
        }

        IEnumerable<string> GetAssetUrls(StylesheetBundle bundle)
        {
            return bundle.Assets.Select(urlGenerator.CreateAssetUrl);
        }
    }
}
