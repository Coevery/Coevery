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
        { field: 'Name', displayName: 'Actions', width: 100, cellTemplate: '<div class="ngCellText" ng-class="col.colIndex()"><a ng-click="edit(row.getProperty(col.field))">Edit</a></div>' },
        { field: 'DisplayName', displayName: 'Field Label' },
        { field: 'Name', displayName: 'Field Name' },
        { field: 'Type', displayName: 'Type' },
        { field: 'FieldType', displayName: 'Field Type' },
        { field: 'ControlField', displayName: 'Control Field' }
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
    
    var relationshipColumnDefs = [
      { field: 'Name', displayName: 'Actions', width: 100, cellTemplate: '<div class="ngCellText" ng-class="col.colIndex()"><a>Edit</a>&nbsp;<a>Delete</a></div>' },
      { field: 'Name', displayName: 'Relationship Name' },
      { field: 'PrimaryEntity', displayName: 'Primary Entity' },
      { field: 'RelatedEntity', displayName: 'Related Entity' },
      { field: 'Type', displayName: 'Type' }
    ];
    $scope.relationshipGrid = {
        data: 'relationships',
        multiSelect: false,
        enableColumnReordering: true,
        columnDefs: relationshipColumnDefs
    };
    $scope.relationships = [
        { Name: 'Leads_Accounts', PrimaryEntity: 'Lead', RelatedEntity: 'Account', Type: 'One to Many' },
        { Name: 'Leads_Opportunities', PrimaryEntity: 'Lead', RelatedEntity: 'Opportunity', Type: 'One to Many' },
        { Name: 'Leads_Users', PrimaryEntity: 'Lead', RelatedEntity: 'User', Type: 'Many to Many' }
    ];
   
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
    $scope.editOneToMany = function() {
        $state.transitionTo('SubList', { Module: 'Metadata', Id: name, SubModule: 'Relationship', View: 'EditOneToMany' });
    };
    $scope.editManyToMany = function () {
        $state.transitionTo('SubList', { Module: 'Metadata', Id: name, SubModule: 'Relationship', View: 'EditManyToMany' });
    };
    $scope.listViewDesigner = function() {
        $state.transitionTo('SubList', { Module: 'Metadata', Id: name, SubModule: 'Projection', View: 'List' });
    }; 
    $scope.formDesigner = function () {
        location.href = 'Metadata/FormDesignerViewTemplate/Index/' + name;
    }; 
    
    $scope.getAllField = function () {
        var metaData = metadata.get({ name: name }, function () {
            $scope.item = metaData;
            $scope.myData = metaData.Fields;
            $.each($scope.myData, function() {
                $.extend(this, { Type: 'System Field' });
            });
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