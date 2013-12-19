using System;
using Coevery.ContentManagement;
using Coevery.Projections.ModelBinding;

namespace Coevery.Projections.PropertyEditors {
    /// <summary>
    /// Coordinated all available <see cref="IPropertyEditor"/> to apply specific formatting on a model binding property
    /// </summary>
    public interface IPropertyFormater : IDependency {

        /// <summary>
        /// Returns the form for a specific type
        /// </summary>
        string GetForm(Type type);

        /// <summary>
        /// Formats the value based on the Form state, for a specific type
        /// </summary>
        dynamic Format(Type type, object value, dynamic formState);

        dynamic Format(ContentField field, Type type, object value, dynamic formState);
    }
}