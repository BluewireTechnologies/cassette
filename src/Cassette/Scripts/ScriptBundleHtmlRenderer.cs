namespace Cassette.Scripts
{
    class ScriptBundleHtmlRenderer : IBundleHtmlRenderer<ScriptBundle>
    {
        public ScriptBundleHtmlRenderer(IUrlGenerator urlGenerator)
        {
            this.urlGenerator = urlGenerator;
        }

        readonly IUrlGenerator urlGenerator;

        public string Render(ScriptBundle bundle)
        {
            var url = urlGenerator.CreateBundleUrl(bundle);

            var conditionalRenderer = new ConditionalRenderer();
            return conditionalRenderer.Render(bundle.Condition, html => html.AppendFormat(HtmlConstants.ScriptHtml, url, bundle.HtmlAttributes.CombinedAttributes));
        }
    }
}