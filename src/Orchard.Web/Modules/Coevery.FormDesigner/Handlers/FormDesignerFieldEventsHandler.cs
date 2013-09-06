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

        public void OnCreated(string etityName, string fieldName, bool isInLayout) {
            if (isInLayout) {
                _layoutManager.AddField(etityName, fieldName);
            }
        }

        public void OnDeleting(string etityName, string fieldName) {
            _layoutManager.DeleteField(etityName, fieldName);
        }
    }
}