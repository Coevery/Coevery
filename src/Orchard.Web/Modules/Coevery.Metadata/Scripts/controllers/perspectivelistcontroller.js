//PerspectiveListCtrl.$inject = ['$rootScope', '$scope', 'logger', '$state', 'localize', '$resource', '$stateParams'];

function PerspectiveListCtrl($rootScope, $scope, logger, $state, localize, $resource, $stateParams) {
    
    var cellTemplateString = '<div class="ngCellText" ng-class="col.colIndex()"><a href ="#/Metadata/{{row.entity.Name}}" class="ngCellText">{{row.entity.DisplayName}}</a></div>';
    $scope.mySelections = [];
    var perspective = PerspectiveContext($resource);

    var perspectiveColumnDefs = [
        { field: 'Id', displayName: 'Actions', width: 100, cellTemplate: '<div class="ngCellText" ng-class="col.colIndex()"><a ng-click="edit(row.getProperty(col.field))">Edit</a>&nbsp;<a ng-click="delete(row.getProperty(col.field))">Remove</a></div>' },
        { field: 'Id', displayName: 'Id' },
        { field: 'DisplayName', displayName: localize.getLocalizedString('DisplayName'), cellTemplate: cellTemplateString }];

    $scope.gridOptions = {
        data: 'myData',
        selectedItems: $scope.mySelections,
        multiSelect: false,
        enableRowSelection: false,
        showColumnMenu: true,
        enableColumnResize: true,
        enableColumnReordering: true,
        columnDefs: perspectiveColumnDefs
    };

    $scope.delete = function (id) {
        perspective.delete({ Id: id }, function () {
            $scope.getAllPerspective();
            logger.success('Delete the ' + moduleName + ' successful.');
        }, function () {
            logger.error('Failed to delete the ' + moduleName);
        });
    };
    
    $scope.addPerspective = function () {
        $state.transitionTo('SubList', { Module: 'Metadata', SubModule: 'Perspective', Id: $stateParams.Id,subId:0, View: 'CreatePerspective' });
    };
    
    $scope.edit = function (id) {
        $state.transitionTo('SubList', { Module: 'Metadata', SubModule: 'Perspective', Id: id, View: 'EditPerspective' });
    };


    $scope.getAllPerspective = function () {
        var perspectives = perspective.query(function () {
            $scope.myData = perspectives;
        }, function () {
            logger.error("Failed to fetched Metadata.");
        });
    };
    $scope.getAllPerspective();
}

//@ sourceURL=Coevery.Metadata/perspectivelistcontroller.js