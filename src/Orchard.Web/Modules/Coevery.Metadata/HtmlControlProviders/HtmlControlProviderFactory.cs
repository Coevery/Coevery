using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;
using Orchard.ContentManagement.MetaData.Models;

namespace Coevery.Metadata.HtmlControlProviders
{
    public interface IHtmlControlProviderFactory : IDependency
    {
        IHtmlControlProvider GetHtmlControlProvider(ContentPartFieldDefinition fieldDefinition);
    }

    public class HtmlControlProviderFactory:IHtmlControlProviderFactory
    {

        public IHtmlControlProvider GetHtmlControlProvider(ContentPartFieldDefinition fieldDefinition)
        {
            string filedName = fieldDefinition.FieldDefinition.Name;
            IHtmlControlProvider re = null;
            switch (filedName)
            {
                case "TextField":
                   re = new TextFiledProvider(fieldDefinition);
                    break;
                case "EnumrationField":
                     re = new EnumrationFieldProvider(fieldDefinition);
                    break;
                case "BooleanField":
                      re = new BooleanFieldProvider(fieldDefinition);
                    break;
                case "NumericField":
                      re = new NumericFieldProvider(fieldDefinition);
                    break;
                case "DateTimeField":
                      re = new DateTimeFieldProvider(fieldDefinition);
                    break;
                default:
                   throw  new TypeUnloadedException(filedName + "'s htmlcontrol provider not found");
            }

            return re;
        }
    }
}