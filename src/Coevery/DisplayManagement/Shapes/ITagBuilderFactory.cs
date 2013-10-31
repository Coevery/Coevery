using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Coevery.DisplayManagement.Shapes {
    public interface ITagBuilderFactory : IDependency {
        CoeveryTagBuilder Create(dynamic shape, string tagName);
    }

    public class CoeveryTagBuilder : TagBuilder {
        public CoeveryTagBuilder(string tagName) : base(tagName) { }

        public IHtmlString StartElement { get { return new HtmlString(ToString(TagRenderMode.StartTag)); } }
        public IHtmlString EndElement { get { return new HtmlString(ToString(TagRenderMode.EndTag)); } }
    }

    public class TagBuilderFactory : ITagBuilderFactory {
        public CoeveryTagBuilder Create(dynamic shape, string tagName) {
            var tagBuilder = new CoeveryTagBuilder(tagName);
            tagBuilder.MergeAttributes(shape.Attributes, false);
            foreach (var cssClass in shape.Classes ?? Enumerable.Empty<string>())
                tagBuilder.AddCssClass(cssClass);
            if (!string.IsNullOrEmpty(shape.Id))
                tagBuilder.GenerateId(shape.Id);
            return tagBuilder;
        }
    }
}
