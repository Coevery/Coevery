metadata.controller('FieldCtrl', function ($scope, logger, $location, $routeParams, localize, metadata, field) {
    
    var name = $routeParams.name;

    $scope.mySelections = [];
    var fieldColumnDefs = [{ field: 'DisplayName', displayName: 'DisplayName' }];
    
    $scope.gridOptions = {
        data: 'myData',
        selectedItems: $scope.mySelections,
        multiSelect: false,
        showColumnMenu: true,
        enableColumnResize: true,
        enableColumnReordering: true,
        columnDefs: fieldColumnDefs
    };

    $scope.$on("localizeResourcesUpdates", function () {
        for (var colIndex = 0; colIndex < $scope.gridOptions.$gridScope.columns.length; colIndex++) {
            $scope.gridOptions.$gridScope.columns[colIndex].displayName
                = localize.getLocalizedString($scope.gridOptions.$gridScope.columns[colIndex].field);
        }
    });
    
    $scope.delete = function () {
        if ($scope.mySelections.length > 0) {
            field.delete({ name: $scope.mySelections[0].Name, parentname: name }, function () {
                $scope.mySelections.pop();
                $scope.getAllField();
                logger.success("Delete the field successful.");
            }, function () {
                logger.error("Failed to delete the field.");
            });
        }
    };

    $scope.exit = function () {
        $location.path('');
    };
    
    $scope.add = function () {
        var params = '[{parentname:"' + name + '"}]';
        $location.path('FieldList/CreateField/' + params);
    };

    $scope.edit = function () {
        if ($scope.mySelections.length > 0) {
            var params = '[{parentname:"' + name + '",name:"' + $scope.mySelections[0].Name + '"}]';
            $location.path('FieldList/EditField/' + params);
        }
    };

    
    
    $scope.getAllField = function () {
        var metaData = metadata.get({ name: name }, function () {
            $scope.item = metaData;
            $scope.myData = metaData.Fields;
        }, function () {
            logger.error("The metadata does not exist.");
        });
    };
    $scope.getAllField();
});