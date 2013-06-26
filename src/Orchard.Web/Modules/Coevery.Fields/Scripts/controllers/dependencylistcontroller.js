'use strict';
define(['core/app/detourService', 'Modules/Coevery.Fields/Scripts/services/fielddependencydataservice'], function (detour) {
    detour.registerController([
        'FieldDependencyListCtrl',
        ['$scope', 'logger', '$detour', '$stateParams', '$resource', 'fieldDependencyDataService',
            function ($scope, logger, $detour, $stateParams, $resource, fieldDependencyDataService) {
                var entityName = $stateParams.EntityName;

                var fieldColumnDefs = [
                    { field: 'Id', displayName: 'Actions', width: 100, cellTemplate: '<div class="ngCellText" ng-class="col.colIndex()"><a>Edit</a>&nbsp;<a ng-click="delete(row.getProperty(col.field))">Delete</a></div>' },
                    { field: 'ControlFieldName', displayName: 'Control Field' },
                    { field: 'DependentFieldName', displayName: 'Dependent Field' }
                ];

                $scope.gridOptions = {
                    data: 'myData',
                    selectedItems: $scope.mySelections,
                    multiSelect: false,
                    enableColumnReordering: true,
                    columnDefs: fieldColumnDefs
                };

                $scope.add = function () {
                    $detour.transitionTo('FieldDependencyCreate', { EntityName: entityName });
                };
                $scope.back = function () {
                    $detour.transitionTo('EntityDetail.Fields', { Id: entityName });
                };
                $scope.delete = function (itemId) {
                    fieldDependencyDataService.delete({ Id: itemId });
                };

                $scope.getOptionItems = function () {
                    var items = fieldDependencyDataService.query({ EntityName: entityName }, function () {
                        $scope.myData = items;
                    }, function () {
                        logger.error("Get items failed.");
                    });
                };
                $scope.getOptionItems();
            }]
    ]);
});