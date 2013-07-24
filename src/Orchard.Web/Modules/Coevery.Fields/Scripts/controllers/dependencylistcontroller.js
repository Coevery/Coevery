'use strict';
define(['core/app/detourService', 'Modules/Coevery.Fields/Scripts/services/fielddependencydataservice'], function (detour) {
    detour.registerController([
        'FieldDependencyListCtrl',
        ['$rootScope', '$scope', 'logger', '$detour', '$stateParams', '$resource', 'fieldDependencyDataService',
            function ($rootScope, $scope, logger, $detour, $stateParams, $resource, fieldDependencyDataService) {

                var cellTemplateString = '<div class="ngCellText" ng-class="col.colIndex()" title="{{COL_FIELD}}">' +
          '<span class="btn-link" ng-click="view(row.entity.Name)">{{COL_FIELD}}</span>' +
          '<ul class="row-actions pull-right hide">' +
          '<li class="icon-edit" ng-click="edit(row.entity.Name)" title="Edit"></li>' +
          '<li class="icon-remove" ng-click="delete(row.entity.Name)" title="Delete"></li>' +
          '</ul>' +
          '</div>';
                var entityName = $stateParams.EntityName;

                var fieldColumnDefs = [
                    { field: 'ControlFieldName', displayName: 'Control Field', cellTemplate: cellTemplateString },
                    { field: 'DependentFieldName', displayName: 'Dependent Field' }
                ];

                $scope.pagingOptions = {
                    pageSizes: [250, 500, 1000],
                    pageSize: 250,
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
                    $detour.transitionTo('FieldDependencyCreate', { EntityName: entityName });
                };
                $scope.back = function () {
                    $detour.transitionTo('EntityDetail.Fields', { Id: entityName });
                };
                $scope.delete = function (itemId) {
                    fieldDependencyDataService.delete({ Id: itemId }, function() {
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