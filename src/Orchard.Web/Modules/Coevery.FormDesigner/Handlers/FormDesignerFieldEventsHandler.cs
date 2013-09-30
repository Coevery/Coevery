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

        public void OnCreated(string entityName, string fieldName, bool isInLayout) {
            if (isInLayout) {
                _layoutManager.AddField(entityName, fieldName);
            }
        }

        public void OnDeleting(string entityName, string fieldName) {
            _layoutManager.DeleteField(entityName, fieldName);
        }
    }
}