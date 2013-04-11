UserViewCtrl.$inject = ['$rootScope', '$scope', 'logger', '$state', 'localize', '$resource', '$stateParams'];

function UserViewCtrl($rootScope, $scope, logger, $state, localize, $resource,$stateParams) {
    var moduleName = 'Projection';
    var module = UserViewContext($resource);
    var columnDefs = getColumnDefs(localize);
    $scope.mySelections = [];

    $scope.gridOptions = {
        data: 'myData',
        //enableCellSelection: true,
        //enableRowSelection: false,
        //showSelectionCheckbox: true,
        selectedItems: $scope.mySelections,
        multiSelect: false,
        showColumnMenu: true,
        enableColumnResize: true,
        enableColumnReordering: true,
        //enableCellEdit: true,
        columnDefs: columnDefs
    };

    $scope.delete = function () {
        if ($scope.mySelections.length > 0) {
            module.delete({ contentType: $scope.mySelections[0].ContentId }, function () {
                $scope.mySelections.pop();
                $scope.getAll();
                logger.success('Delete the ' + moduleName + ' successful.');
            }, function () {
                logger.error('Failed to delete the ' + moduleName);
            });
        }
    };

    $scope.add = function () {
        $state.transitionTo('SubCreate', { Module: 'Metadata', SubModule: 'Projection', Id: $stateParams.Id });
    };

    $scope.edit = function () {
        if ($scope.mySelections.length > 0) {
            $state.transitionTo('SubDetail', { Module: 'Metadata', SubModule: 'Projection', View: 'Edit', Id: $stateParams.Id, SubId: $scope.mySelections[0].ContentId });
        }
    };

    $scope.getAll = function () {
        var records = module.query(function () {
            $scope.myData = records;
        }, function () {
            logger.error("Failed to fetched projections for " + $stateParams.Id);
        });
    };

    $scope.getAll();
}

//@ sourceURL=Coevery.Metadata/userviewlistcontroller.js