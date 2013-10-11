using System.Collections.Generic;
using System.Linq;
using Orchard;

namespace Coevery.Entities.Services {
    public interface IFieldToPropertyStateProvider:IDependency {
        string GetPropertyState(string fieldType, string filedName, IDictionary<string,string> customSettings);
        bool CanHandle(string fieldType);
    }
    public abstract class FieldToPropertyStateProvider : IFieldToPropertyStateProvider {
        protected IEnumerable<string> FieldTypeSet { get; set; }
        protected const string Format = @"<Form>
                  <Description>{0}</Description>
                  <LinkToContent>true</LinkToContent>
                  <ExcludeFromDisplay>false</ExcludeFromDisplay>
                  <CreateLabel>false</CreateLabel>
                  <Label></Label>
                  <Format>{1}</Format>
                  <CustomizePropertyHtml>false</CustomizePropertyHtml>
                  <CustomPropertyTag></CustomPropertyTag>
                  <CustomPropertyCss></CustomPropertyCss>
                  <NoResultText></NoResultText>
                  <ZeroIsEmpty>false</ZeroIsEmpty>
                  <HideEmpty>false</HideEmpty>
                  <TrimLength>false</TrimLength>
                  <MaxLength>0</MaxLength>
                  </Form>";
        public abstract string GetPropertyState(string fieldType, string filedName, IDictionary<string, string> customSettings);
        public virtual bool CanHandle(string fieldType) {
            return FieldTypeSet.Any(type => type == fieldType);
        }
    }
}