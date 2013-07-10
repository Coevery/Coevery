'use strict';

define(['core/app/detourService', 'Modules/Coevery.Entities/Scripts/services/entitydataservice', 'Modules/Coevery.Entities/Scripts/services/fielddataservice'], function (detour) {
    detour.registerController([
        'FieldsCtrl',
        ['$rootScope', '$scope', 'logger', '$location', '$detour', '$stateParams', 'entityDataService', 'fieldDataService',
            function ($rootScope, $scope, logger, $location, $detour, $stateParams, entityDataService, fieldDataService) {

                var entityName = $stateParams.Id;
                var fieldColumnDefs = [
                    { field: 'Name', displayName: 'Actions', width: 100, cellTemplate: '<div class="ngCellText" ng-class="col.colIndex()"><a ng-click="edit(row.getProperty(col.field))">Edit</a>&nbsp;<a ng-hide="row.getProperty(\'IsSystemField\')" ng-click="delete(row.getProperty(col.field))">Delete</a></div>' },
                    { field: 'DisplayName', displayName: 'Field Label' },
                    { field: 'Name', displayName: 'Field Name' },
                    { field: 'Type', displayName: 'Type' },
                    { field: 'FieldType', displayName: 'Field Type' },
                    { field: 'ControlField', displayName: 'Control Field' }
                ];

                $scope.$on('toStep2', function (event, fieldInfo) {
                    $scope.$broadcast('toStep2Done');
                    $location.url("/Entities/" + entityName.toString() + "/Create/" + fieldInfo);
                });

                $scope.$on('toStep1', function () {
                    $scope.$broadcast('toStep1Done');
                    $location.url("/Entities/" + entityName.toString() + "/Create");                  
                });

                $scope.gridOptions = {
                    data: 'myData',
                    columnDefs: fieldColumnDefs
                };

                angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);

                var deleteField;
                $scope.delete = function (fieldName) {
                    deleteField = fieldName;
                    $('#myModal').modal({
                        backdrop: 'static',
                        keyboard: true
                    });
                };

                $scope.add = function () {
                    $detour.transitionTo('EntityDetail.Fields.Create', { Id: entityName });
                };

                $scope.deleteField = function () {
                    $('#myModal').modal('hide');

                    fieldDataService.delete({ name: deleteField, parentname: entityName }, function () {
                        $scope.getAllField();
                        logger.success("Delete the field successful.");
                    }, function () {
                        logger.error("Failed to delete the field.");
                    });
                };

                $scope.edit = function (fieldName) {
                    $detour.transitionTo('FieldEdit', { EntityName: entityName, FieldName: fieldName });
                };
                $scope.gotoDependency = function () {
                    $detour.transitionTo('FieldDependencyList', { EntityName: entityName });
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
