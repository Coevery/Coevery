using Orchard;

namespace Piedone.Combinator.Services
{
    public interface IMinificationService : IDependency {
        string MinifyCss(string css);
        string MinifyJavaScript(string javaScript);
    }
}
