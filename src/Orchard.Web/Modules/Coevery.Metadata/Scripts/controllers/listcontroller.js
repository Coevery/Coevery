metadata.controller('MetadataCtrl', function ($scope, logger, $location,localize, metadata) {
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


    $scope.$on("localizeResourcesUpdates", function () {
        for (var colIndex = 0; colIndex < $scope.gridOptions.$gridScope.columns.length; colIndex++) {
            $scope.gridOptions.$gridScope.columns[colIndex].displayName
                = localize.getLocalizedString($scope.gridOptions.$gridScope.columns[colIndex].field);
        }
    });
    
    $scope.delete = function () {
        if ($scope.mySelections.length > 0) {
            metadata.delete({ name: $scope.mySelections[0].Name }, function () {
                $scope.mySelections.pop();
                $scope.getAllMetadata();
                logger.success("Delete the metadata successful.");
            }, function () {
                logger.error("Failed to delete the metadata.");
            });
        }
    };

    $scope.add = function () {
        $location.path('Create');
    };
    
    $scope.OpenFieldList = function () {
        $location.path('FieldList/' + $scope.mySelections[0].Name);
    };

    $scope.edit = function () {
        if ($scope.mySelections.length > 0) {
            $location.path('Edit/'+$scope.mySelections[0].Name);
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