using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cassette.Utilities;

namespace Cassette.Scripts
{
    class DebugScriptBundleHtmlRenderer : IBundleHtmlRenderer<ScriptBundle>
    {
        public DebugScriptBundleHtmlRenderer(IUrlGenerator urlGenerator)
        {
            this.urlGenerator = urlGenerator;
        }

        readonly IUrlGenerator urlGenerator;

        public string Render(ScriptBundle bundle)
        {
            var conditionalRenderer = new ConditionalRenderer();
            return conditionalRenderer.Render(
                bundle.Condition,
                html => RenderScriptContent(bundle, html)
            );
        }

        void RenderScriptContent(ScriptBundle bundle, StringBuilder sb)
        {
            GetAssetUrls(bundle)
                .AppendWithSeparator(sb, Environment.NewLine, (b, url) =>
                    b.AppendFormat(HtmlConstants.ScriptHtml, url, bundle.HtmlAttributes.CombinedAttributes));
        }

        IEnumerable<string> GetAssetUrls(ScriptBundle bundle)
        {
            return bundle.Assets.Select(urlGenerator.CreateAssetUrl);
        }
    }
}
