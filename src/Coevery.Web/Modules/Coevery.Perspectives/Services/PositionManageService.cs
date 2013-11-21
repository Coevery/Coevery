using System;
using System.Collections.Generic;
using System.Linq;
using Coevery.ContentManagement;
using Coevery.Core.Navigation.Models;
using Coevery.Core.Navigation.ViewModels;
using Coevery.Perspectives.Models;

namespace Coevery.Perspectives.Services {
    public interface IPositionManageService : IDependency {
        PositionTreeModel ParseMenuPostion(string position, int perspectiveId);
    }

    public class PositionManageService : IPositionManageService {
        private readonly IContentManager _contentManager;
        public PositionManageService(IContentManager contentManager) {
            _contentManager = contentManager;
        } 

        public PositionTreeModel ParseMenuPostion(string position, int perspectiveId) {
            var menu = GetMenuPartFromPosition(position);
            var perspective = _contentManager.Get<PerspectivePart>(perspectiveId);
            if (menu == null || perspective == null) {
                return null;
            }
            var lastPosition = menu.MenuPosition.LastIndexOf('.');
            int currentLevel;
            int weight;
            string parentPosition;
            if (lastPosition < 0) {
                currentLevel = 1;
                parentPosition = null;
                int.TryParse(position, out weight);
            }
            else {
                var positions = menu.MenuPosition.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
                currentLevel = positions.Length;
                int.TryParse(positions.Last(), out weight);
                parentPosition = menu.MenuPosition.Substring(0, lastPosition);
            }
            var parentMenu = GetMenuPartFromPosition(parentPosition);
            return new PositionTreeModel {
                ParentId = parentMenu != null ? parentMenu.Id : (int?)null,
                ParentPosition = parentPosition,
                Level = currentLevel,
                Order = weight,
                Expanded = true,
            };
        }

        private MenuPart GetMenuPartFromPosition(string position) {
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