'use strict';

define(['core/app/detourService', 'core/services/entitydataservice', 'core/services/gridcolumndataservice'], function(detour) {
    detour.registerController([
        'RelatedEntityListCtrl',
        ['$rootScope', '$scope', 'logger', '$state', '$stateParams', '$location', 'commonDataService', 'gridColumnDataService',
            function($rootScope, $scope, logger, $state, $stateParams, $location, commonDataService, gridColumnDataService) {
                var filters;
                $scope.toolButtonDisplay = false;
                $scope.isInit = true;
                $scope.definitionViews = [];
                $scope.columnDefs = [];

                //init pagingoption
                var pageSizes = [50, 100, 200];
                var currentPage = parseInt($location.$$search['Page'], 10);
                if (!currentPage) currentPage = 1;
                var pageSize = parseInt($location.$$search['Rows'], 10);
                if (!pageSize || pageSizes.indexOf(pageSize) < 0) pageSize = 50;

                var getPostData = function() {
                    return {
                        ViewId: $scope.viewId,
                        FilterGroupId: 0,
                        Filters: JSON.stringify(getFilters())
                    };
                };

                function getFilters() {
                    if (!filters) {
                        filters = [{
                            Type: $scope.partName + '.' + $scope.relationId + '.',
                            Category: $scope.partName + 'ContentFields',
                            FormData: [
                                { name: 'Operator', value: 'MatchesAny' },
                                { name: 'Value', value: $scope.$stateParams.Id }
                            ]
                        }];
                    }

                    return filters;
                }

                $scope.getPagedDataAsync = function() {
                    $scope.referenceList.setParam({ postData: getPostData() });
                    $scope.referenceList.reload();
                };

                $scope.$watch('filterOptions', function(newVal, oldVal) {
                    if (newVal !== oldVal) {
                        $scope.getPagedDataAsync();
                    }
                }, true);

                $scope.getRelatedData = function() {
                    var gridColumnQuery = gridColumnDataService.get({ contentType: $scope.entityTypeName, viewId: $scope.viewId }, function() {
                        $.each(gridColumnQuery.colModel, function(index, value) {
                            if (value.formatter) {
                                value.formatter = $rootScope[value.formatter];
                            }
                        });
                        if (!$scope.isInit) {
                            $scope.getPagedDataAsync();
                        } else {
                            var gridOptions = {
                                url: applicationBaseUrl + 'api/projections/entity/' + $scope.entityTypeName,
                                mtype: "post",
                                postData: getPostData(),
                                rowNum: pageSize,
                                rowList: pageSizes,
                                page: currentPage,
                                loadComplete: function(data) {
                                    currentPage = data.page;
                                    pageSize = data.records;
                                },
                                loadError: function(xhr, status, error) {
                                    logger.error("Failed to fetched records for " + $scope.entityTypeName + ":\n" + error);
                                }
                            };

                            $scope.gridOptions = angular.extend({}, $rootScope.defaultGridOptions, gridOptions, gridColumnQuery);
                        }
                    }, function() {
                    });
                };

                /*Grid Methods*/
                $scope.Refresh = function() {
                    $scope.getPagedDataAsync();
                };

                $rootScope['delete'] = function(id) {
                    var deleteRelationship = id || $scope.selectedItems.length > 0 ? $scope.selectedItems : null;
                    if (!deleteRelationship) {
                        logger.error('No data selected.');
                        return;
                    }
                    var ids;
                    if ($.isArray(deleteRelationship)) {
                        ids = deleteRelationship.join(",");
                    } else {
                        ids = deleteRelationship.toString();
                    }
                    commonDataService['delete']({ contentId: ids }, function() {
                        $scope.Refresh();
                        logger.success('Delete the relationship successful.');
                        $scope.entityId = [];
                        $scope.selectedItems = [];
                    }, function() {
                        logger.error('Failed to delete the relationship');
                    });
                };

                $scope.add = function() {
                    $state.transitionTo('Root.Menu.Create', { NavigationId: $stateParams.NavigationId, Module: $scope.entityTypeName });
                };

                $scope.edit = function(id) {
                    if (!id && $scope.selectedItems.length > 0) {
                        id = $scope.selectedItems[0];
                    }
                    $state.transitionTo('Root.Menu.Detail', { NavigationId: $stateParams.NavigationId, Module: $scope.entityTypeName, Id: id });
                };

                $scope.view = function(id) {
                    if (!id && $scope.selectedItems.length > 0) {
                        id = $scope.selectedItems[0];
                    }
                    $state.transitionTo('Root.Menu.View', { NavigationId: $stateParams.NavigationId, Module: $scope.entityTypeName, Id: id });
                };
            }]
    ]);
});