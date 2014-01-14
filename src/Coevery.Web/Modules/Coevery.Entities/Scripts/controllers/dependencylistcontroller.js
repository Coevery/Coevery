'use strict';
define(['core/app/detourService', 'Modules/Coevery.Fields/Scripts/services/fielddependencydataservice'], function (detour) {
    detour.registerController([
        'FieldDependencyListCtrl',
        ['$rootScope', '$scope', 'logger', '$state', '$stateParams', '$resource', 'fieldDependencyDataService',
            function ($rootScope, $scope, logger, $state, $stateParams, $resource, fieldDependencyDataService) {

                var cellTemplateString = '<div class="ngCellText" ng-class="col.colIndex()" title="{{COL_FIELD}}">' +
                    '<ul class="row-actions pull-right hide">' +
                    '<li class="icon-edit" ng-click="edit(row.entity.Id)" title="Edit"></li>' +
                    '<li class="icon-remove" ng-click="delete(row.entity.Id)" title="Delete"></li>' +
                    '</ul>' +
                    '<span class="btn-link" ng-click="edit(row.entity.Id)">{{COL_FIELD}}</span>' +
                    '</div>';
                var entityName = $stateParams.EntityName;

                var fieldColumnDefs = [
                    { field: 'ControlFieldName', displayName: 'Control Field', cellTemplate: cellTemplateString },
                    { field: 'DependentFieldName', displayName: 'Dependent Field' }
                ];

                $scope.pagingOptions = {
                    pageSizes: [50, 100, 200],
                    pageSize: 50,
                    currentPage: 1
                };

                $scope.gridOptions = {
                    data: 'myData',
                    selectedItems: $scope.mySelections,
                    multiSelect: false,
                    enableRowSelection: false,
                    enableColumnReordering: true,
                    columnDefs: fieldColumnDefs,
                    pagingOptions: $scope.pagingOptions
                };
                angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);

                $scope.add = function () {
                    $state.transitionTo('FieldDependencyCreate', { EntityName: entityName });
                };
                $scope.back = function () {
                    $state.transitionTo('EntityDetail.Fields', { Id: entityName });
                };
                $scope.edit = function (itemId) {
                    $state.transitionTo('FieldDependencyEdit', { EntityName: entityName, DependencyID: itemId });
                };
                $scope['delete'] = function (itemId) {
                    fieldDependencyDataService['delete']({ Id: itemId }, function () {
                        logger.success('Delete success.');
                        $scope.getOptionItems();
                    });
                };

                $scope.getOptionItems = function () {
                    var items = fieldDependencyDataService.query({ EntityName: entityName }, function () {
                        $scope.totalServerItems = items.length;
                        $scope.myData = items;
                    }, function () {
                        logger.error("Get items failed.");
                    });
                };
                $scope.getOptionItems();
            }]
    ]);
});