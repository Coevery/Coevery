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

        public void OnDeleting(FieldEventsContext context) {
            _layoutManager.DeleteField(context.EtityName, context.FieldName);
        }
    }
}