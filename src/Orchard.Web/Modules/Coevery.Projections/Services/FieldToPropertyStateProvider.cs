using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;

namespace Coevery.Projections.Services {
    public interface IFieldToPropertyStateProvider:IDependency {
        string GetPropertyState(string fieldType, string filedName, object displaySettings);
    }
    public class FieldToPropertyStateProvider : IFieldToPropertyStateProvider {
        public virtual string GetPropertyState(string fieldType, string filedName, object displaySettings) {
            const string format = @"<Form>
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
            string formatOption = null;
            switch (fieldType) {
                case "DateField":
                    formatOption = "d";
                    break;
                case "DatetimeField":
                    formatOption = "g";
                    break;
            }
            return string.Format(format, filedName, formatOption);
        }
    }
}