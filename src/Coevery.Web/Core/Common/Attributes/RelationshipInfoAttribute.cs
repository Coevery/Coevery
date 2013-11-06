using System;

namespace Coevery.Core.Common.Attributes {
    [AttributeUsage(AttributeTargets.Class)]
    public class RelationshipInfoAttribute : Attribute {
        private readonly string _entityName;
        private readonly bool _isPrimary;

        public RelationshipInfoAttribute(string entityName, bool isPrimary) {
            _entityName = entityName;
            _isPrimary = isPrimary;
        }

        public string EntityName {
            get { return _entityName; }
        }

        public bool IsPrimary {
            get { return _isPrimary; }
        }
    }
}