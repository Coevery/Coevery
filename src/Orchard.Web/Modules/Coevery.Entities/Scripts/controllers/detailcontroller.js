'use strict';

define(['core/app/couchPotatoService', 'Modules/Coevery.Entities/Scripts/services/entitydataservice', 'Modules/Coevery.Entities/Scripts/services/fielddataservice'], function(couchPotato) {
    couchPotato.registerController([
        'EntityListCtrl',
        ['$scope', 'logger', '$state', '$stateParams', '$resource', 'entityDataService', 'fieldDataService',
            function($scope, logger, $state, $stateParams, entityDataService, fieldDataService) {
                var name = $stateParams.Id;

                var fieldColumnDefs = [
                    { field: 'Name', displayName: 'Actions', width: 100, cellTemplate: '<div class="ngCellText" ng-class="col.colIndex()"><a ng-click="edit(row.getProperty(col.field))">Edit</a>&nbsp;<a ng-hide="row.getProperty(\'IsSystemField\')" ng-click="delete(row.getProperty(col.field))">Delete</a></div>' },
                    { field: 'DisplayName', displayName: 'Field Label' },
                    { field: 'Name', displayName: 'Field Name' },
                    { field: 'Type', displayName: 'Type' },
                    { field: 'FieldType', displayName: 'Field Type' },
                    { field: 'ControlField', displayName: 'Control Field' }
                ];

                $scope.gridOptions = {
                    data: 'myData',
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

                var deleteField;
                $scope.delete = function(fieldName) {
                    deleteField = fieldName;
                    $('#myModal').modal({
                        backdrop: 'static',
                        keyboard: true
                    });
                };
                $scope.deleteField = function() {
                    $('#myModal').modal('hide');
                    fieldDataService.delete({ name: deleteField, parentname: name }, function () {
                        $scope.getAllField();
                        logger.success("Delete the field successful.");
                    }, function() {
                        logger.error("Failed to delete the field.");
                    });
                };

                $scope.exit = function() {
                    $state.transitionTo('List', { Module: 'Metadata' });
                };

                $scope.add = function() {
                    $state.transitionTo('SubCreate', { Module: 'Metadata', Id: name, SubModule: 'Field', View: 'Create' });
                };

                $scope.edit = function(fieldName) {
                    $state.transitionTo('SubDetail', { Module: 'Metadata', Id: name, SubModule: 'Field', View: 'Edit', SubId: fieldName });
                };

                $scope.gotoDependency = function() {
                    $state.transitionTo('SubList', { Module: 'Metadata', Id: name, SubModule: 'Field', View: 'DependencyList' });
                };
                $scope.editOneToMany = function() {
                    $state.transitionTo('SubList', { Module: 'Metadata', Id: name, SubModule: 'Relationship', View: 'EditOneToMany' });
                };
                $scope.editManyToMany = function() {
                    $state.transitionTo('SubList', { Module: 'Metadata', Id: name, SubModule: 'Relationship', View: 'EditManyToMany' });
                };
                $scope.listViewDesigner = function() {
                    $state.transitionTo('SubList', { Module: 'Metadata', Id: name, SubModule: 'Projection', View: 'List' });
                };
                $scope.formDesigner = function() {
                    location.href = 'Metadata/FormDesignerViewTemplate/Index/' + name;
                };

                $scope.getAllField = function() {
                    var metaData = entityDataService.get({ name: name }, function () {
                        $scope.item = metaData;
                        $scope.myData = metaData.Fields;
                        $.each($scope.myData, function() {
                            var type = this.IsSystemField ? 'System Field' : 'User Field';
                            $.extend(this, { Type: type });
                        });
                        $scope.userFields = [
                            { DisplayName: 'Full Name', Name: 'FullName', FieldType: 'Input Field' }
                        ];
                    }, function() {
                        logger.error("The metadata does not exist.");
                    });
                };
                $scope.getAllField();
            }]
    ]);
});