using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Microsoft.AspNetCore.Csp
{
    [HtmlTargetElement("script")]
    public class NoncedScriptTagHelper : TagHelper
    {
        private readonly INonce _nonce;

        public NoncedScriptTagHelper(INonce nonce)
        {
            _nonce = nonce;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            //base.Process(context, output);
            output.TagName = "script";
            output.Attributes.SetAttribute("debug-nonce", _nonce.GetValue());
            output.Attributes.SetAttribute("nonce", _nonce.GetValue());
        }
    }
}
