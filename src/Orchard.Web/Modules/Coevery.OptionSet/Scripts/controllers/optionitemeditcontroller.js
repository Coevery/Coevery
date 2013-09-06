'use strict';

define(['core/app/detourService'], function(detour) {
    detour.registerController([
        'OptionItemsCtrl',
        ['$rootScope', '$scope', 'logger', '$detour', '$resource', '$stateParams','optionItemDataService',
            function ($rootScope, $scope, logger, $detour, $resource, $stateParams, optionItemDataService) {
                
                var optionColumnDefs = [
                    { field: 'Id', displayName: 'Actions', width: 100, cellTemplate: '<div class="ngCellText" ng-class="col.colIndex()"><a ng-click="edit(row.entity)">Edit</a>&nbsp;<a ng-click="delete(row.getProperty(col.field))">Delete</a></div>' },
                    { field: 'Name', displayName: 'Value' },
                    { field: 'Selectable', displayName: 'Selectable' },
                    { field: 'Weight', displayName: 'Weight' }
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

                $scope.add = function() {
                    $scope.itemValue = '';
                    $scope.itemSelectable = true;
                    $scope.editFunc = createItemFunc;
                    $('#editModal').modal({
                        backdrop: 'static',
                        keyboard: true
                    });
                };

                $scope.edit = function(item) {
                    $scope.itemId = item.Id;
                    $scope.itemValue = item.Name;
                    $scope.itemSelectable = item.Selectable;
                    $scope.itemWeight = item.Weight;
                    $scope.editFunc = editItemFunc;
                    $('#editModal').modal({
                        backdrop: 'static',
                        keyboard: true
                    });
                };

                function createItemFunc() {
                    $('#editModal').modal('hide');
                    optionItemDataService.save({
                            optionSetId: $scope.optionSetId,
                            name: $scope.itemValue,
                            selectable: $scope.itemSelectable,
                            weight: $scope.itemWeight
                        }, function() {
                            $scope.getOptionItems();
                        }, function() {
                            logger.error("Failed to add the item.");
                        });
                }

                function editItemFunc() {
                    $('#editModal').modal('hide');
                    optionItemDataService.update({
                            id: $scope.itemId,
                            name: $scope.itemValue,
                            selectable: $scope.itemSelectable,
                            weight: $scope.itemWeight
                        }, function() {
                            $scope.getOptionItems();
                            //logger.success("Update the item successful.");
                        }, function() {
                            logger.error("Failed to update the item.");
                        });
                }

                $scope.delete = function(itemId) {
                    $scope.itemId = itemId;
                    $('#deleteModal').modal({
                        backdrop: 'static',
                        keyboard: true
                    });
                };
                $scope.deleteItem = function() {
                    $('#deleteModal').modal('hide');
                    optionItemDataService.delete({ Id: $scope.itemId }, function () {
                        $scope.getOptionItems();
                        logger.success("Delete the item successful.");
                    }, function() {
                        logger.error("Failed to delete the item.");
                    });
                };

                $scope.getOptionItems = function () {
                    var items = optionItemDataService.query({ optionSetId: $scope.optionSetId }, function () {
                        $scope.totalServerItems = items.length;
                        $scope.myData = items;
                    }, function() {
                        logger.error("Get items failed.");
                    });
                };

                $scope.getOptionItems();
            }
        ]
    ]);
});
