UserViewCtrl.$inject = ['$rootScope', '$scope', 'logger', '$state', 'localize', '$resource', '$stateParams'];

function UserViewCtrl($rootScope, $scope, logger, $state, localize, $resource,$stateParams) {
    var moduleName = 'Projection';
    var module = UserViewContext($resource);
    var columnDefs = getColumnDefs(localize);
    $scope.mySelections = [];

    $scope.gridOptions = {
        data: 'myData',
        showSelectionCheckbox: true,
        selectedItems: $scope.mySelections,
        multiSelect: true,
        enableColumnResize: true,
        enableColumnReordering: true,
        columnDefs: columnDefs
    };

    $scope.delete = function (id) {
        module.delete({ Id: id }, function () {
            $scope.getAll();
            logger.success('Delete the ' + moduleName + ' successful.');
        }, function () {
            logger.error('Failed to delete the ' + moduleName);
        });
    };

    $scope.add = function () {
        $state.transitionTo('SubCreate', { Module: 'Metadata', SubModule: 'Projection', Id: $stateParams.Id });
    };

    $scope.edit = function (id) {
        $state.transitionTo('SubDetail', { Module: 'Metadata', SubModule: 'Projection', View: 'Edit', Id: $stateParams.Id, SubId: id });
    };

    $scope.getAll = function () {
        var records = module.query({Name:$stateParams.Id},function () {
            $scope.myData = records;
        }, function () {
            logger.error("Failed to fetched projections for " + $stateParams.Id);
        });
    };

    $scope.getAll();
}

//@ sourceURL=Coevery.Metadata/userviewlistcontroller.js