using System;
using System.Collections.Generic;
using System.Linq;
using Coevery.Entities.Events;
using Coevery.FormDesigner.Services;

namespace Coevery.FormDesigner.Handlers {
    public class FormDesignerFieldEventsHandler : IFieldEvents {
        private readonly ILayoutManager _layoutManager;

        public FormDesignerFieldEventsHandler(ILayoutManager layoutManager) {
            _layoutManager = layoutManager;
        }

        public void OnCreated(FieldCreatedContext context) {
            if (context.IsInLayout) {
                _layoutManager.AddField(context.EtityName, context.FieldName);
            }
        }

        public void OnDeleting(FieldDeletingContext context) {
            _layoutManager.DeleteField(context.EtityName, context.FieldName);
        }
    }
}