'use strict';
define(['core/app/detourService', 'Modules/Coevery.Entities/Scripts/services/optionitemsdataservice'], function (detour) {
    detour.registerController([
        'ItemsCtrl',
        ['$rootScope', '$scope', 'logger', '$detour', '$stateParams', '$resource', 'optionItemsDataService',
            function ($rootScope, $scope, logger, $detour, $stateParams, $resource, optionItemsDataService) {
                var entityName = $stateParams.EntityName;
                var fieldName = $stateParams.FieldName;

                var optionColumnDefs = [
                    { field: 'Id', displayName: 'Actions', width: 100, cellTemplate: '<div class="ngCellText" ng-class="col.colIndex()"><a ng-click="edit(row.entity)">Edit</a>&nbsp;<a ng-click="delete(row.getProperty(col.field))">Delete</a></div>' },
                    { field: 'Value', displayName: 'Value' },
                    { field: 'IsDefault', displayName: 'Is Default', cellTemplate: '<div class="ngSelectionCell" ng-class="col.colIndex()"><span ng-cell-text><input type="checkbox" ng-checked="row.entity[col.field]" disabled></span></div>' }
                ];

                $scope.pagingOptions = {
                    pageSizes: [50, 100, 200],
                    pageSize: 50,
                    currentPage: 1
                };
                $scope.totalServerItems = 2;
                $scope.gridOptions = {
                    data: 'myData',
                    multiSelect: false,
                    enableRowSelection: false,
                    columnDefs: optionColumnDefs,
                    pagingOptions: $scope.pagingOptions
                };
                angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);

                $scope.add = function () {
                    $scope.itemValue = '';
                    $scope.itemIsDefault = false;
                    $scope.editFunc = createItemFunc;
                    $('#editModal').modal({
                        backdrop: 'static',
                        keyboard: true
                    });
                };
                var editOptionItem;
                $scope.edit = function (item) {
                    editOptionItem = item;
                    $scope.itemValue = item.Value;
                    $scope.itemIsDefault = item.IsDefault;
                    $scope.editFunc = editItemFunc;
                    $('#editModal').modal({
                        backdrop: 'static',
                        keyboard: true
                    });
                };
                function createItemFunc() {
                    $('#editModal').modal('hide');
                    var newItem = new optionItemsDataService();
                    newItem.Value = $scope.itemValue;
                    newItem.IsDefault = $scope.itemIsDefault;
                    newItem.$save({
                        EntityName: entityName,
                        FieldName: fieldName
                    }, function () {
                        $scope.getOptionItems();
                        logger.success("Add the item successful.");
                    }, function () {
                        logger.error("Failed to add the item.");
                    });
                }
                function editItemFunc() {
                    $('#editModal').modal('hide');
                    editOptionItem.Value = $scope.itemValue;
                    editOptionItem.IsDefault = $scope.itemIsDefault;
                    editOptionItem.$update({
                        Id: editOptionItem.Id
                    }, function () {
                        $scope.getOptionItems();
                        logger.success("Update the item successful.");
                    }, function () {
                        logger.error("Failed to update the item.");
                    });
                }

                var deleteItemId;
                $scope.delete = function (itemId) {
                    deleteItemId = itemId;
                    $('#deleteModal').modal({
                        backdrop: 'static',
                        keyboard: true
                    });
                };
                $scope.deleteItem = function () {
                    $('#deleteModal').modal('hide');
                    optionItemsDataService.delete({ Id: deleteItemId }, function () {
                        $scope.getOptionItems();
                        logger.success("Delete the item successful.");
                    }, function () {
                        logger.error("Failed to delete the item.");
                    });
                };

                $scope.getOptionItems = function () {
                    var items = optionItemsDataService.query({ EntityName: entityName, FieldName: fieldName }, function () {
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