
function MetadataCtrl($scope, logger, $state, localize, $resource) {
    var cellTemplateString = '<div class="ngCellText" ng-class="col.colIndex()"><a href ="#/Metadata/{{row.entity.Name}}" class="ngCellText">{{row.entity.DisplayName}}</a></div>';
    $scope.mySelections = [];
    var metadata = MetadataContext($resource);
    var metadataGenerator = GenerateContext($resource);
    var metadataColumnDefs = [
        { field: 'Name', displayName: 'Actions', width: 100, cellTemplate: '<div class="ngCellText" ng-class="col.colIndex()"><a ng-click="edit(row.getProperty(col.field))">Edit</a>&nbsp;<a ng-click="delete(row.getProperty(col.field))">Remove</a></div>' },
        { field: 'DisplayName', displayName: localize.getLocalizedString('DisplayName'), cellTemplate: cellTemplateString },
        { field: 'IsDeployed', displayName: localize.getLocalizedString('IsDeployed') }];

    $scope.gridOptions = {
        data: 'myData',
        //enableCellSelection: true,
        //showSelectionCheckbox: true,
        selectedItems: $scope.mySelections,
        multiSelect: false,
        enableRowSelection: false,
        showColumnMenu: true,
        enableColumnResize: true,
        enableColumnReordering: true,
        //enableCellEdit: true,
        columnDefs: metadataColumnDefs
    };

    $scope.delete = function (entityName) {
        metadata.delete({ name: entityName }, function () {
            $scope.mySelections.pop();
            $scope.getAllMetadata();
            logger.success("Delete the metadata successful.");
        }, function () {
            logger.error("Failed to delete the metadata.");
        });
    };

    $scope.add = function () {
        $state.transitionTo('Create', { Module: 'Metadata' });
    };

    $scope.edit = function (entityName) {
        $state.transitionTo('Detail', { Module: 'Metadata', Id: entityName });
    };

    $scope.getAllMetadata = function () {
        var metadatas = metadata.query(function () {
            $scope.myData = metadatas;
        }, function () {
            logger.error("Failed to fetched Metadata.");
        });
    };

    $scope.generate = function () {
        metadataGenerator.save({ name: '' }, function() {
            $scope.getAllMetadata();
            logger.success("Generate metadata successful.");
        }, function() {
            logger.error("Failed to Generate the metadata.");
        });
    };


    $scope.getAllMetadata();
}

//@ sourceURL=Coevery.Metadata/listcontroller.js