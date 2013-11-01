using System.Collections.Generic;
using System.Linq;
using Coevery.Common.Extensions;
using Coevery;
using Coevery.ContentManagement;
using Coevery.Forms.Services;
using Coevery.Projections.Descriptors.Property;
using Coevery.Projections.Services;

namespace Coevery.Projections.Services
{
    public interface ICompareContentService : IDependency {
        bool CompareContent(ContentItem preContentItem, ContentItem newContentItem);
        Dictionary<string, string> Defferences { get; }
    }

    public class CompareContentService : ICompareContentService {
        public Dictionary<string, string> Defferences {
            get { return _defferences; }
        }

        private readonly Dictionary<string, string> _defferences = new Dictionary<string, string>();
        private const string IsAuditKey = "CoeveryTextFieldSettings.IsAudit";
        private readonly IProjectionManager _projectionManager;

        public CompareContentService(IProjectionManager projectionManager) {
            _projectionManager = projectionManager;
        }

        public bool CompareContent(ContentItem preContentItem, ContentItem newContentItem) {
            _defferences.Clear();
            var part = newContentItem.Parts.FirstOrDefault(p => p.PartDefinition.Name == preContentItem.ContentType.ToPartName());
            if (part == null) {
                return false;
            }
            var fieldContext = new PropertyContext {
                State = FormParametersHelper.ToDynamic(string.Empty)
            };
            string category = newContentItem.ContentType.ToPartName() + "ContentFields";
            var allFielDescriptors = _projectionManager.DescribeProperties().Where(p => p.Category == category).SelectMany(x => x.Descriptors).ToList();
            foreach (var field in part.Fields) {
                if (!field.PartFieldDefinition.Settings.ContainsKey(IsAuditKey)) {
                    continue;
                }
                bool isAudit = bool.Parse(field.PartFieldDefinition.Settings[IsAuditKey]);
                if (!isAudit) {
                    continue;
                }
                if (_defferences.Keys.Contains(field.Name)) {
                    continue;
                }
                var descriptor = allFielDescriptors.FirstOrDefault(d => d.Name.Text == field.Name + ":Value");
                if (descriptor == null) {
                    continue;
                }
                var preValue = descriptor.Property(fieldContext, preContentItem);
                var newValue = descriptor.Property(fieldContext, newContentItem);
                if (preValue != null && !string.IsNullOrEmpty(preValue.ToString())
                    && (newValue == null || string.IsNullOrEmpty(newValue.ToString()))) {
                    _defferences.Add(field.Name, string.Format("Deleted {1} in {0}.", field.Name, preValue));
                }
                else if (preValue != null && newValue.ToString() != preValue.ToString() ||
                         preValue == null && newValue != null && !string.IsNullOrEmpty(newValue.ToString())) {
                    _defferences.Add(field.Name, string.Format("Changed {0} from {1} to {2}.", field.Name, preValue, newValue));
                }
            }
            return _defferences.Count == 1;
        }
    }
}