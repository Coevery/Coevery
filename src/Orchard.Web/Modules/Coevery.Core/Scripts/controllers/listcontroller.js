CommonCtrl.$inject = ['$rootScope', '$scope', 'logger', '$state', 'localize', '$resource', '$stateParams'];

function CommonCtrl($rootScope,$scope, logger, $state, localize, $resource,$stateParams) {
    var moduleName = $rootScope.$stateParams.Module;
    var module = CommonContext($rootScope, $resource);
    //var columnDefs = getColumnDefs(localize);
    var columnDefs = [{ field: 'Id', displayName: localize.getLocalizedString('Id') },
                { field: 'Topic', displayName: localize.getLocalizedString('Topic') },
                { field: 'StatusCode', displayName: localize.getLocalizedString('StatusCode') },
                { field: 'FirstName', displayName: localize.getLocalizedString('FirstName') },
                { field: 'LastName', displayName: localize.getLocalizedString('LastName') }];
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
                logger.success('Delete the '+moduleName+' successful.');
            }, function () {
                logger.error('Failed to delete the lead.');
            });
        }
    };

    $scope.add = function () {
        $state.transitionTo('Create', { Module: moduleName });
    };

    $scope.edit = function () {
        if ($scope.mySelections.length > 0) {
            $state.transitionTo('Detail', { Module: moduleName, Id: $scope.mySelections[0].ContentId });
        }
    };
    $scope.createnewview = function () {
        $state.transitionTo('SubCreate', { Module: 'Metadata', SubModule: 'Projection', Id: $stateParams.Module });
    };
    
    $scope.editview = function () {
        $state.transitionTo('SubDetail', { Module: 'Metadata', SubModule: 'Projection', View: 'Edit', Id: $stateParams.Module, SubId: viewId });
    };
    
    $scope.getAll = function () {
        var records = module.query(function () {
            $scope.myData = records;
        }, function () {
            logger.error("Failed to fetched records for " + moduleName);
        });
    };

    $scope.getAll();
}

//@ sourceURL=Coevery.Core/listcontroller.js