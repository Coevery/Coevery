using System;
using System.Linq;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Common.Fields;

namespace Coevery.Metadata.HtmlControlProviders
{
    public interface IHtmlControlProvider
    {
        string GenerateHtmlContrlDesc();
    }

    public abstract class ControlProviderBase : IHtmlControlProvider
    {
        protected ContentPartFieldDefinition _field;
        private const string ControlDescTemp = "<fd-tools-field field-type=\"{0}\" field-display-name =\"{1}\" field-name=\"{2}\" {3}></fd-tools-field>";

        public string GenerateHtmlContrlDesc()
        {
            string filedType = _field.FieldDefinition.Name;
            string controlType = GetControlType(filedType);
            string settingPartten = GenerateControlpattern();

            return string.Format(ControlDescTemp, controlType, _field.DisplayName, _field.Name, settingPartten);
        }

        public virtual string GenerateControlpattern()
        {
            string settingPartten = string.Empty;
            var requiredKeys = _field.Settings.Keys.Where(t => t.EndsWith("Required"));
            foreach (var requiredKey in requiredKeys)
            {
                bool required = bool.Parse(_field.Settings[requiredKey]);
                if (required)
                {
                    settingPartten += " Required";
                    break;
                }
            }
            return settingPartten;
        }

        private string GetControlType(string filedType)
        {
            switch (filedType)
            {
                case "TextField":
                    return "text";
                case "EnumrationField":
                    return "enumration";
                case "BooleanField":
                    return "checkbox";
                case "NumericField":
                    return "number";
                case "DateTimeField":
                    return "datetime";
                default:
                    return "text";
            }
        }
    }

}