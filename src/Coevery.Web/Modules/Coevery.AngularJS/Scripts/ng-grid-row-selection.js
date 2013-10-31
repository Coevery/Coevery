function ngGridRowSelectionPlugin() {
    var self = this;
    self.grid = null;
    self.scope = null;
    self.init = function (scope, grid) {
        self.grid = grid;
        self.scope = scope;

        self.grid.rowFactory.rowConfig.beforeSelectionChangeCallback = function (row, event) {
            if (event && event.target) {
                var element = event.target || event;
                //check and make sure its not the bubbling up of our checked 'click' event 
                if (element.type == "checkbox" && element.parentElement.className == "ngSelectionCell ng-scope") {
                    return true;
                }
                if ($(element).hasClass("btn-link")) return false;
                if ($(element).parent().hasClass("row-actions")) return false;
                var selectionProvider = row.selectionProvider;
                selectionProvider.toggleSelectAll(false, true);
            }
            return true;
        };
        self.grid.rowFactory.rowConfig.afterSelectionChangeCallback = function () {
            var allSelected = self.grid.data.length == self.scope.selectionProvider.selectedItems.length;
            var header = grid.$root.find(".ngSelectionHeader");
            var targetCell = $(header).closest('.ngHeaderCell');
            var cellScope = angular.element(targetCell).scope();
            cellScope.allSelected = allSelected;
        };
    };
}