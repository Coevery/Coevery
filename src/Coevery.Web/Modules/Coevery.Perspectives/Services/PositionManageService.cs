using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.ContentManagement;
using Coevery.Core.Navigation.Models;

namespace Coevery.Perspectives.Services {
    public class PositionTreeModel {
        public int ParentId { get; set; }
        public string ParentPosition { get; set; }
        public int Level { get; set; }
        public bool IsLeaf { get; set; }
        public bool Expanded { get; set; }
    }
    public interface IPositionManageService : IDependency {

    }
    public class PositionManageService : IPositionManageService {
        private ICoeveryServices _service;
        public PositionManageService(ICoeveryServices service) {
            _service = service;
        }

        public MenuPart GetMenuItemFromPosition(string position) {
            if (string.IsNullOrWhiteSpace(position)) {
                return null;
            }

             //_service.ContentManager.Query(,);
            return null;
        }
        public PositionTreeModel ParseMenuPostion(string position) {
            return null;
        }
    }
}