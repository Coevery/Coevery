metadata.controller('MetadataCtrl', function ($scope, logger, $location, metadata) {
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
        columnDefs:metadataColumnDefs
    };

    $scope.delete = function () {
        if ($scope.mySelections.length > 0) {
            metadata.delete({ leadId: $scope.mySelections[0].Name }, function () {
                $scope.mySelections.pop();
                $scope.getAllMetadata();
                logger.success("Delete the lead successful.");
            }, function () {
                logger.error("Failed to delete the lead.");
            });
        }
    };

    $scope.add = function () {
        $location.path('Detail');
    };

    $scope.edit = function () {
        if ($scope.mySelections.length > 0) {
            $location.path($scope.mySelections[0].Name);
        }
    };

    $scope.getAllMetadata = function () {
        var metadatas = metadata.query(function () {
            $scope.myData = metadatas;
        }, function() {
            logger.error("Failed to fetched Metadata.");
        });
    };

    $scope.getAllMetadata();
});