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
using Orchard.Forms.Services;
using Orchard.Projections.Descriptors.Property;
using Orchard.Projections.Models;
using Orchard.Projections.Services;
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
        private readonly IProjectionManager _projectionManager;
        public CompareContentService(IProjectionManager projectionManager) {
            _projectionManager = projectionManager;
        }

        public bool CompareContent(ContentItem preContentItem, ContentItem newContentItem)
        {
            _defferences.Clear();
            var part = newContentItem.Parts.FirstOrDefault(p => p.PartDefinition.Name.Contains(preContentItem.ContentType));
            if (part == null) return false;
            var fieldContext = new PropertyContext
            {
                State = FormParametersHelper.ToDynamic(string.Empty)
            };
            string category = newContentItem.ContentType + "ContentFields";
            var allFielDescriptors = _projectionManager.DescribeProperties().Where(p => p.Category == category).SelectMany(x => x.Descriptors).ToList();
            foreach (var field in part.Fields) {
                if (!field.PartFieldDefinition.Settings.ContainsKey(isAuditKey)) continue;
                bool isAudit = bool.Parse(field.PartFieldDefinition.Settings[isAuditKey]);
                if (!isAudit) continue;
                if (_defferences.Keys.Contains(field.Name)) continue;
                var descriptor = allFielDescriptors.FirstOrDefault(d => d.Name.Text == field.Name + ":Value");
                if (descriptor == null) continue;
                var preValue = descriptor.Property(fieldContext, preContentItem);
                var newValue = descriptor.Property(fieldContext, newContentItem);
                if (preValue != null && !string.IsNullOrEmpty(preValue.ToString())
                    && (newValue == null || string.IsNullOrEmpty(newValue.ToString())))
                {
                    _defferences.Add(field.Name, string.Format("Deleted {1} in {0}.", field.Name, preValue));
                }
                else if (preValue != null && newValue.ToString() != preValue.ToString() ||
                    preValue == null && newValue != null && !string.IsNullOrEmpty(newValue.ToString()))
                {
                    _defferences.Add(field.Name, string.Format("Changed {0} from {1} to {2}.", field.Name, preValue, newValue));
                }
            }
            return _defferences.Count == 1;
        }
    }
}