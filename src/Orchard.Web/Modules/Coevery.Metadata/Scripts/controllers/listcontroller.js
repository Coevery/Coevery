function MetadataCtrl($scope, logger, $state, localize, $resource) {
    
    $scope.mySelections = [];
    var metadata = MetadataContext($resource);
    var metadataColumnDefs = [{ field: 'DisplayName', displayName: localize.getLocalizedString('DisplayName') }];
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
        columnDefs: metadataColumnDefs
    };

    $scope.delete = function() {
        if ($scope.mySelections.length > 0) {
            metadata.delete({ name: $scope.mySelections[0].Name }, function() {
                $scope.mySelections.pop();
                $scope.getAllMetadata();
                logger.success("Delete the metadata successful.");
            }, function() {
                logger.error("Failed to delete the metadata.");
            });
        }
    };

    $scope.add = function() {
        $state.transitionTo('Create', { Moudle: 'Metadata' });
    };

    $scope.OpenFieldList = function() {
        $state.transitionTo('FieldList', { Moudle: 'Metadata', name: $scope.mySelections[0].Name });
    };

    $scope.edit = function() {
        if ($scope.mySelections.length > 0) {
            $state.transitionTo('Detail', { Moudle: 'Metadata', Id: $scope.mySelections[0].Name });
        }
    };

    $scope.getAllMetadata = function() {
        var metadatas = metadata.query(function() {
            $scope.myData = metadatas;
        }, function() {
            logger.error("Failed to fetched Metadata.");
        });
    };

    $scope.getAllMetadata();
}