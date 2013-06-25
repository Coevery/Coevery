function DependencyListCtrl($scope, logger, localize, $state, $stateParams, $resource) {
    var entityName = $stateParams.Id;
    var FieldDependency = $resource(
        'api/metadata/FieldDependency',
        {},
        { update: { method: 'PUT' } }
    );

    var fieldColumnDefs = [
        { field: 'Id', displayName: 'Actions', width: 100, cellTemplate: '<div class="ngCellText" ng-class="col.colIndex()"><a>Edit</a>&nbsp;<a ng-click="delete(row.getProperty(col.field))">Delete</a></div>' },
        { field: 'ControlFieldName', displayName: 'Control Field' },
        { field: 'DependentFieldName', displayName: 'Dependent Field' }
    ];

    $scope.gridOptions = {
        data: 'myData',
        selectedItems: $scope.mySelections,
        multiSelect: false,
        enableColumnReordering: true,
        columnDefs: fieldColumnDefs
    };

    $scope.add = function () {
        $state.transitionTo('SubList', { Module: 'Metadata', Id: $stateParams.Id, SubModule: 'Field', View: 'CreateDependency' });
    };
    $scope.back = function () {
        $state.transitionTo('Detail', { Module: 'Metadata', Id: $stateParams.Id });
    };
    $scope.delete = function (itemId) {
        FieldDependency.delete({ Id: itemId });
    };

    $scope.getOptionItems = function () {
        var items = FieldDependency.query({ EntityName: entityName }, function () {
            $scope.myData = items;
        }, function () {
            logger.error("Get items failed.");
        });
    };
    $scope.getOptionItems();
}

//@ sourceURL=Coevery.Metadata/dependencylistcontroller.js