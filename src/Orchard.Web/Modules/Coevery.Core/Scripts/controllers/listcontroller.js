CommonCtrl.$inject = ['$rootScope', '$scope', 'logger', '$state', 'localize', '$resource'];

function CommonCtrl($rootScope,$scope, logger, $state, localize, $resource) {
    var moduleName = $rootScope.$stateParams.Module;
    var module = CommonContext($rootScope,$resource);
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
        columnDefs: [{ field: 'Id', displayName: localize.getLocalizedString('Id') },
                { field: 'Topic', displayName: localize.getLocalizedString('Topic') },
                { field: 'StatusCode', displayName: localize.getLocalizedString('StatusCode') },
                { field: 'FirstName', displayName: localize.getLocalizedString('FirstName') },
                { field: 'LastName', displayName: localize.getLocalizedString('LastName') }]
    };

    $scope.delete = function () {
        if ($scope.mySelections.length > 0) {
            module.delete({ leadId: $scope.mySelections[0].LeadId }, function () {
                $scope.mySelections.pop();
                $scope.getAllLeads();
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