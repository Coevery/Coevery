using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Projections.Descriptors.Property;
using Orchard.Projections.Models;
using Orchard.Projections.ViewModels;

namespace Coevery.Core {

    public interface ICompareContentService : IDependency {
        bool CompareContent(ContentItem preContentItem, ContentItem newContentItem);
        Dictionary<string, string> Defferences { get; }
    }

    public class CompareContentService : ICompareContentService
    {
        public Dictionary<string, string> Defferences {
            get { return _defferences; }
        }

        Dictionary<string,string> _defferences = new Dictionary<string, string>();
        private readonly string isAuditKey = "CoeveryTextFieldSettings.IsAudit";
        public bool CompareContent(ContentItem preContentItem, ContentItem newContentItem)
        {
            _defferences.Clear();
            var prePart = preContentItem.Parts.FirstOrDefault(p => p.PartDefinition.Name.Contains(preContentItem.ContentType));
            var part = newContentItem.Parts.FirstOrDefault(p => p.PartDefinition.Name.Contains(preContentItem.ContentType));
            if (prePart == null || part == null) return false;
            foreach (var field in part.Fields) {
                if (!field.PartFieldDefinition.Settings.ContainsKey(isAuditKey)) continue;
                bool isAudit = bool.Parse(field.PartFieldDefinition.Settings[isAuditKey]);
                if (!isAudit) continue;
                var preField = prePart.Fields.FirstOrDefault(f => f.Name == field.Name);
                if (preField == null) continue;
                var property = field.GetType().GetProperty("Value");
                var preProperty = preField.GetType().GetProperty("Value");
                if (property != null && preProperty != null) {
                    var newValue = property.GetValue(field, null);
                    var preValue = preProperty.GetValue(preField,null);
                    if (newValue == null && preValue == null) continue;
                    if (_defferences.Keys.Contains(field.Name)) continue;
                    if (preValue != null && !string.IsNullOrEmpty(preValue.ToString())
                        && (newValue == null || string.IsNullOrEmpty(newValue.ToString()))) {
                            _defferences.Add(field.Name, string.Format("Deleted {1} in {0}.", field.Name, preValue));
                    }
                    else if (preValue != null && newValue.ToString() != preValue.ToString() || 
                        preValue == null && newValue != null && !string.IsNullOrEmpty(newValue.ToString()))
                    {
                        _defferences.Add(field.Name, string.Format("Changed {0} from {1} to {2}.",field.Name,preValue,newValue));
                    }
                }
            }
            return _defferences.Count == 1;
        }
    }
}