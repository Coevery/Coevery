function MetadataDetailCtrl($scope, logger, localize, $state, $stateParams, $resource) {
    var name = $stateParams.Id;
    var field = FieldContext($resource);
    var metadata = MetadataContext($resource);
    //$scope.mySelections = [];
    //var fieldColumnDefs = [
    //    { field: 'DisplayName', displayName: localize.getLocalizedString('DisplayName') },
    //    { field: 'FieldType', displayName: localize.getLocalizedString('FieldTypeDisplayName') }
    //];

    var fieldColumnDefs = [
        { field: 'Name', displayName: 'Actions', width: 100, cellTemplate: '<button class="btn btn-small" ng-click="edit(row.getProperty(col.field))"><i class="icon-pencil"></i></button>' },
        { field: 'DisplayName', displayName: 'Field Label' },
        { field: 'Name', displayName: 'Field Name' },
        { field: 'FieldType', displayName: 'Field Type' }
    ];

    $scope.gridOptions = {
        data: 'myData',
        //selectedItems: $scope.mySelections,
        multiSelect: false,
        enableColumnReordering: true,
        columnDefs: fieldColumnDefs
    };
    var userFieldColumnDefs = [
        { field: 'Name', displayName: 'Actions', width: 100, cellTemplate: '<button class="btn btn-small"><i class="icon-pencil"></i></button><button class="btn btn-small"><i class="icon-remove"></i></button>' },
        { field: 'DisplayName', displayName: 'Field Label' },
        { field: 'Name', displayName: 'Field Name' },
        { field: 'FieldType', displayName: 'Field Type' },
        { field: 'ControlField', displayName: 'Control Field' }
    ];
    $scope.userFields = {
        data: 'userFields',
        multiSelect: false,
        enableColumnReordering: true,
        columnDefs: userFieldColumnDefs
    };
   
    //$scope.delete = function () {
    //    if ($scope.mySelections.length > 0) {
    //        field.delete({ name: $scope.mySelections[0].Name, parentname: name }, function () {
    //            $scope.mySelections.pop();
    //            $scope.getAllField();
    //            logger.success("Delete the field successful.");
    //        }, function () {
    //            logger.error("Failed to delete the field.");
    //        });
    //    }
    //};

    $scope.exit = function () {
        $state.transitionTo('List', { Module: 'Metadata' });
    };

    $scope.add = function () {
        $state.transitionTo('SubCreate', { Module: 'Metadata', Id: name, SubModule: 'Field', View: 'Create' });
    };

    $scope.edit = function (fieldName) {
        $state.transitionTo('SubDetail', { Module: 'Metadata', Id: name, SubModule: 'Field', View: 'Edit', SubId: fieldName });
    };

    $scope.gotoDependency = function() {
        $state.transitionTo('SubList', { Module: 'Metadata', Id: name, SubModule: 'Field', View: 'DependencyList' });
    };

    $scope.getAllField = function () {
        var metaData = metadata.get({ name: name }, function () {
            $scope.item = metaData;
            $scope.myData = metaData.Fields;
            $scope.userFields = [
                { DisplayName: 'Full Name', Name: 'FullName', FieldType: 'Input Field' }
            ];
        }, function () {
            logger.error("The metadata does not exist.");
        });
    };
    $scope.getAllField();
}

//@ sourceURL=Coevery.Metadata/detailcontroller.js