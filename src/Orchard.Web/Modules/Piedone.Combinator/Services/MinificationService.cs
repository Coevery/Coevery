
using Microsoft.Ajax.Utilities;

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
            return new Minifier().MinifyJavaScript(javaScript);
        }
    }
}