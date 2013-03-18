

using Microsoft.Ajax.Utilities;
using Yahoo.Yui.Compressor;

namespace Piedone.Combinator.Services
{
    /// <summary>
    /// Wraps YUI compressor
    /// </summary>
    public class MinificationService : IMinificationService
    {
        public string MinifyCss(string css) {
            return new Minifier().MinifyStyleSheet(css);

        }

        public string MinifyJavaScript(string javaScript) {
            return javaScript;
            //return new Minifier().MinifyJavaScript(javaScript);
        }

        public string MinifyYahooJavaScript(string javaScript) {
            return javaScript;
            //return new JavaScriptCompressor().Compress(javaScript);
        }
    }
}