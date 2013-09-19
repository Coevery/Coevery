using Orchard.Projections.FieldTypeEditors;

namespace Coevery.Projections.FieldTypeEditors {
    public interface ILogicFieldTypeEditor : IFieldTypeEditor {
        bool CanHandle(string fieldTypeName);  
        bool NeedApplyFilter { get; }
        void ApplyFilter(dynamic context);
    }
}