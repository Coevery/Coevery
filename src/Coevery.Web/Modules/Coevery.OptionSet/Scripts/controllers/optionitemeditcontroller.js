'use strict';

define(['core/app/detourService'], function(detour) {
    detour.registerController([
        'OptionItemsCtrl',
        ['$rootScope', '$scope', 'logger', '$state', '$resource', '$stateParams','optionItemDataService','$q',
            function ($rootScope, $scope, logger, $state, $resource, $stateParams, optionItemDataService, $q) {
                var defer = $q.defer();
                $scope.$watch("selectedRow", function (newValue) {
                    if (newValue) {
                        defer.resolve();
                    }
                });

                var optionColumnDefs = [
                    { name: 'Id', label: 'Id', sorttype:'int', hidden: true },
                    {
                        name: 'Name', label: 'Value', 
                        formatter: $rootScope.cellLinkTemplate,
                        formatoptions: { editRow: true }
                    },
                    { name: 'Selectable', label: 'Selectable',},
                    { name: 'Weight', label: 'Weight', sorttype: 'int' }
                ];

                $scope.gridOptions = {
                    url: "api/OptionSet/OptionItem/?optionSetId=" + $scope.optionSetId,
                    rowIdName: "Id",
                    colModel: optionColumnDefs
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

                $scope.edit = function (id) {
                    defer.promise.then(function() {
                        var item = $scope.selectedRow[0];
                        $scope.itemId = item.Id;
                        $scope.itemValue = item.Name;
                        $scope.itemSelectable = item.Selectable;
                        $scope.itemWeight = item.Weight;
                        $scope.editFunc = editItemFunc;
                        $('#editModal').modal({
                            backdrop: 'static',
                            keyboard: true
                        });
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
                        }, function(response) {
                            logger.error("Failed to add:\n" + response.data.Text);
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

                $scope['delete'] = function (itemId) {
                    var deleteItem = itemId || $scope.selectedItems.length > 0 ? $scope.selectedItems[0] : null;
                    if (!deleteItem) return;
                    optionItemDataService.delete({ Id: deleteItem }, function () {
                        $scope.getOptionItems();
                        logger.success("Delete the item successful.");
                    }, function () {
                        logger.error("Failed to delete the item.");
                    });
                };

                $scope.getOptionItems = function () {
                    $("#itemList").jqGrid('setGridParam', {
                        datatype: "json"
                    }).trigger('reloadGrid');
                };
            }
        ]
    ]);
});
