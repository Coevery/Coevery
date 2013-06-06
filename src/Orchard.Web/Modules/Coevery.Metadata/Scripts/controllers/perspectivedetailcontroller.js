PerspectiveDetailCtrl.$inject = ['$rootScope', '$scope', 'logger', '$state', 'localize', '$resource', '$stateParams'];

function PerspectiveDetailCtrl($rootScope, $scope, logger, $state, localize, $resource, $stateParams) {
    
    var cellTemplateString = '<div class="ngCellText" ng-class="col.colIndex()"><a href ="#/Metadata/{{row.entity.Name}}" class="ngCellText">{{row.entity.DisplayName}}</a></div>';
    $scope.mySelections = [];
    //var metadata = MetadataContext($resource);

    var metadataColumnDefs = [
        { field: 'Name', displayName: 'Actions', width: 100, cellTemplate: '<div class="ngCellText" ng-class="col.colIndex()"><a ng-click="edit(row.getProperty(col.field))">Edit</a>&nbsp;<a ng-click="delete(row.getProperty(col.field))">Remove</a></div>' },
        { field: 'DisplayName', displayName: localize.getLocalizedString('DisplayName'), cellTemplate: cellTemplateString }];

    $scope.gridOptions = {
        data: 'myData',
        selectedItems: $scope.mySelections,
        multiSelect: false,
        showColumnMenu: true,
        enableColumnResize: true,
        enableColumnReordering: true,
        //enableCellEdit: true,
        columnDefs: metadataColumnDefs
    };

    $scope.exit = function() {
        $state.transitionTo('SubList', { Module: 'Metadata',SubModule:'Perspective', Id: $stateParams.Id, View: 'List' });
    };
    
    $scope.addNavigationItem = function() {
        $state.transitionTo('SubList', { Module: 'Metadata', SubModule: 'Perspective', Id: $stateParams.Id, View: 'EditNavigationItem' });
    };
    
    $scope.edit = function () {
        $state.transitionTo('SubList', { Module: 'Metadata', SubModule: 'Perspective', Id: $stateParams.Id, View: 'EditNavigationItem' });
    };
    
    $scope.delete = function (entityName) {
        logger.success("Delete  successful.");
    };
    
    $scope.getAllMetadata = function () {
        var metadatas = [{ DisplayName: 'Home' }, { DisplayName: 'Leads' }, { DisplayName: 'Accounts' }];
        $scope.myData = metadatas;
    };

    $scope.getAllMetadata();
}

//@ sourceURL=Coevery.Metadata/perspectivedetailcontroller.js