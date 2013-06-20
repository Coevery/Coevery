'use strict';

define(['core/app/detourService', 'Modules/Coevery.Entities/Scripts/services/entitydataservice', 'Modules/Coevery.Entities/Scripts/services/fielddataservice'], function (detour) {
    detour.registerController([
        'FieldsCtrl',
        ['$scope', 'logger', '$detour', '$stateParams', 'entityDataService', 'fieldDataService',
            function ($scope, logger, $detour, $stateParams, entityDataService, fieldDataService) {

                var fieldColumnDefs = [
                    { field: '', displayName: 'Actions', width: 100, cellTemplate: '<div class="ngCellText" ng-class="col.colIndex()"><a ng-click="edit(row.getProperty(col.field))">Edit</a>&nbsp;<a ng-hide="row.getProperty(\'IsSystemField\')" ng-click="delete(row.getProperty(col.field))">Delete</a></div>' },
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

                var deleteField;
                $scope.delete = function (fieldName) {
                    deleteField = fieldName;
                    $('#myModal').modal({
                        backdrop: 'static',
                        keyboard: true
                    });
                };

                var entityName = $stateParams.Id;

                $scope.deleteField = function () {
                    $('#myModal').modal('hide');

                    fieldDataService.delete({ name: deleteField, parentname: entityName }, function () {
                        $scope.getAllField();
                        logger.success("Delete the field successful.");
                    }, function () {
                        logger.error("Failed to delete the field.");
                    });
                };


                $scope.add = function () {
                    $detour.transitionTo('SubCreate', { Module: 'Metadata', Id: entityName, SubModule: 'Field', View: 'Create' });
                };

                $scope.edit = function (fieldName) {
                    $detour.transitionTo('SubDetail', { Module: 'Metadata', Id: entityName, SubModule: 'Field', View: 'Edit', SubId: fieldName });
                };

                $scope.gotoDependency = function () {
                    $detour.transitionTo('SubList', { Module: 'Metadata', Id: entityName, SubModule: 'Field', View: 'DependencyList' });
                };

                $scope.getAllField = function () {
                    var metaData = entityDataService.get({ name: entityName }, function () {
                        $scope.item = metaData;
                        $scope.myData = metaData.Fields;
                        $.each($scope.myData, function () {
                            var type = this.IsSystemField ? 'System Field' : 'User Field';
                            $.extend(this, { Type: type });
                        });
                        $scope.userFields = [
                            { DisplayName: 'Full Name', Name: 'FullName', FieldType: 'Input Field' }
                        ];
                    }, function () {
                        logger.error("The metadata does not exist.");
                    });
                };
                $scope.getAllField();
            }]
    ]);
});