using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.ContentManagement;
using Coevery.Core.Navigation.Models;
using Coevery.Perspectives.Models;

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
        private readonly IContentManager _contentManager;
        public PositionManageService(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        public PositionTreeModel ParseMenuPostion(string position, int perspectiveId) {
            var menu = GetMenuItemFromPosition(position);
            var perspective = _contentManager.Get<PerspectivePart>(perspectiveId);
            if (menu == null || perspective == null) {
                return null;
            }
            var lastPosition = menu.MenuPosition.LastIndexOf('.');
            int currentLevel;
            string parentPosition;
            if (lastPosition < 0) {
                currentLevel = 1;
                parentPosition = null;
            }
            else {
                currentLevel = menu.MenuPosition.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Length;
                parentPosition = menu.MenuPosition.Substring(0, lastPosition);
            }
            var parentMenu = GetMenuItemFromPosition(parentPosition);
            return new PositionTreeModel {
                ParentId = parentMenu.Id,
                ParentPosition = parentPosition,
                Level = currentLevel,
                IsLeaf = currentLevel == perspective.CurrentLevel,
                Expanded = true,
            };
        }



        private MenuPart GetMenuItemFromPosition(string position) {
            if (string.IsNullOrWhiteSpace(position)) {
                return null;
            }

            var result = _contentManager.Query<MenuPart, MenuPartRecord>().Where(menu => menu.MenuPosition == position).ForPart<MenuPart>().List();
            if (result == null || result.Count() != 1) {
                return null;
            }
            return result.FirstOrDefault();
        }
    }
}