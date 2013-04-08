function FieldCtrl($scope, logger, $state, $stateParams, localize, $resource) {
    
    var name = $stateParams.Id;
    var field = FieldContext($resource);
    var metadata = MetadataContext($resource);
    $scope.mySelections = [];
    var fieldColumnDefs = [{ field: 'DisplayName', displayName: localize.getLocalizedString('DisplayName') },
                           { field: 'FieldTypeDisplayName', displayName: localize.getLocalizedString('FieldTypeDisplayName') }];
    
    $scope.gridOptions = {
        data: 'myData',
        selectedItems: $scope.mySelections,
        multiSelect: false,
        showColumnMenu: true,
        enableColumnResize: true,
        enableColumnReordering: true,
        columnDefs: fieldColumnDefs
    };

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
        $state.transitionTo('List', { Module: 'Metadata' });
    };
    
    $scope.add = function () {
        $state.transitionTo('SubCreate', { Module: 'Metadata', Id: name, SubModule: 'Field', View: 'Detail' });
    };

    $scope.edit = function () {
        if ($scope.mySelections.length > 0) {
            $state.transitionTo('SubDetail', { Module: 'Metadata', Id: name, SubModule: 'Field', View: 'Detail', SubId: $scope.mySelections[0].Name });
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
}