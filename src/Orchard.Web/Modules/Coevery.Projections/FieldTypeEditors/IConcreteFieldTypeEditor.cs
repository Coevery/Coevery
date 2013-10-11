using System;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Localization;
using Orchard.Projections.Descriptors.Filter;
using Orchard.Projections.Descriptors.SortCriterion;
using Orchard.Projections.FieldTypeEditors;
using Orchard.Utility.Extensions;

namespace Coevery.Projections.FieldTypeEditors {

    public delegate void Filter(FilterContext context, IFieldTypeEditor fieldTypeEditor, string storageName, Type storageType, ContentPartDefinition part, ContentPartFieldDefinition field);

    public delegate void Sort(SortCriterionContext context, IFieldTypeEditor fieldTypeEditor, string storageName, Type storageType, ContentPartDefinition part, ContentPartFieldDefinition field);

    public interface IConcreteFieldTypeEditor : IFieldTypeEditor {
        bool CanHandle(string fieldTypeName, Type storageType);
        void ApplyFilter(FilterContext context, string storageName, Type storageType, ContentPartDefinition part, ContentPartFieldDefinition field);
        void ApplySortCriterion(SortCriterionContext context, string storageName, Type storageType, ContentPartDefinition part, ContentPartFieldDefinition field);
    }

    public abstract class ConcreteFieldTypeEditorBase : IConcreteFieldTypeEditor {

        public abstract bool CanHandle(string fieldTypeName, Type storageType);

        public virtual void ApplyFilter(FilterContext context, string storageName, Type storageType, ContentPartDefinition part, ContentPartFieldDefinition field) {
            var propertyName = String.Join(".", part.Name, field.Name, storageName ?? "");

            // use an alias with the join so that two filters on the same Field Type wont collide
            var relationship = GetFilterRelationship(propertyName.ToSafeName());

            // generate the predicate based on the editor which has been used
            Action<IHqlExpressionFactory> predicate = GetFilterPredicate(context.State);

            // combines the predicate with a filter on the specific property name of the storage, as implemented in FieldIndexService
            Action<IHqlExpressionFactory> andPredicate = x => x.And(y => y.Eq("PropertyName", propertyName), predicate);

            // apply where clause
            context.Query = context.Query.Where(relationship, andPredicate);
        }

        public virtual void ApplySortCriterion(SortCriterionContext context, string storageName, Type storageType, ContentPartDefinition part, ContentPartFieldDefinition field) {
            var ascending = (bool)context.State.Sort;
            var propertyName = String.Join(".", part.Name, field.Name, storageName ?? "");

            // use an alias with the join so that two filters on the same Field Type wont collide
            var relationship = GetFilterRelationship(propertyName.ToSafeName());

            // generate the predicate based on the editor which has been used
            Action<IHqlExpressionFactory> predicate = y => y.Eq("PropertyName", propertyName);

            // combines the predicate with a filter on the specific property name of the storage, as implemented in FieldIndexService

            // apply where clause
            context.Query = context.Query.Where(relationship, predicate);

            // apply sort
            context.Query = ascending
                ? context.Query.OrderBy(relationship, x => x.Asc("Value"))
                : context.Query.OrderBy(relationship, x => x.Desc("Value"));
        }

        public virtual bool CanHandle(Type storageType) {
            return false;
        }

        public virtual string FormName {
            get { return null; }
        }

        public abstract Action<IHqlExpressionFactory> GetFilterPredicate(dynamic formState);

        public abstract LocalizedString DisplayFilter(string fieldName, string storageName, dynamic formState);

        public abstract Action<IAliasFactory> GetFilterRelationship(string aliasName);
    }

}