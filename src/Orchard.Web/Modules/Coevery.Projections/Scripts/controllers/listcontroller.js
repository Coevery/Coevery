'use strict';

define(['core/app/detourService',
        'Modules/Coevery.Projections/Scripts/services/projectiondataservice'], function (detour) {
            detour.registerController([
                'ProjectionListCtrl',
                ['$rootScope', '$scope', 'logger', '$detour', '$resource', '$stateParams', 'projectionDataService',
                    function ($rootScope, $scope, logger, $detour, $resource, $stateParams, projectionDataService) {

                        var t = function (str) {
                            var result = i18n.t(str);
                            return result;
                        };

                        var actionTemplate = '<div class="ngCellText" ng-class="col.colIndex()" title="{{COL_FIELD}}">' +
                            '<span>{{COL_FIELD}}</span>' +
                            '<ul class="row-actions pull-right hide">' +
                            '<li class="icon-edit" ng-click="edit(row.entity.ContentId)" title="Edit"></li>' +
                            '<li class="icon-remove" ng-click="delete(row.entity.ContentId)" title="Delete"></li>' +
                            '<li class="icon-tags" ng-click="setDefault(row.entity.ContentId)" title="Set Default"></li>' +
                            '</ul>' +
                            '</div>';
                        var columnDefs = [
                            { field: 'DisplayName', displayName: t('DisplayName'), cellTemplate: actionTemplate },
                            { field: 'EntityType', displayName: t('EntityType') },            
                            { field: 'Default', displayName: t('Default') }];
                        $scope.mySelections = [];
                        $scope.pagingOptions = {
                            pageSizes: [50, 100, 200],
                            pageSize: 50,
                            currentPage: 1
                        };

                        $scope.gridOptions = {
                            data: 'myData',
                            enablePaging: true,
                            showFooter: true,
                            multiSelect: true,
                            enableRowSelection: true,
                            showSelectionCheckbox: true,
                            selectedItems: $scope.mySelections,
                            columnDefs: columnDefs,
                            pagingOptions: $scope.pagingOptions
                        };

                        angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);

                        $scope.exit = function () {
                            $detour.transitionTo('EntityDetail.Fields', { Id: $stateParams.EntityName });
                        };

                        $scope.delete = function (id) {
                            projectionDataService.delete({ Id: id }, function () {
                                if ($scope.mySelections.length != 0) {
                                    $scope.mySelections.pop();
                                }
                                $scope.getAll();
                                logger.success('Delete the ' + id + ' successful.');
                            }, function () {
                                logger.error('Failed to delete the ' + id);
                            });
                        };

                        $scope.add = function () {
                            $detour.transitionTo('ProjectionCreate', { EntityName: $stateParams.Id });
                        };

                        $scope.edit = function (id) {
                            $detour.transitionTo('ProjectionEdit', { EntityName: $stateParams.Id, Id: id });
                        };

                        $scope.setDefault = function (id) {
                            var result = projectionDataService.save({ Id: id, EntityType: $stateParams.Id }, function () {
                                if ($scope.mySelections.length != 0) {
                                    $scope.mySelections.pop();
                                }
                                $scope.getAll();
                            }, function () {

                            });
                        };

                        $scope.getAll = function () {
                            var records = projectionDataService.query({ Name: $stateParams.Id }, function () {
                                $scope.myData = records;
                            }, function () {
                                logger.error("Failed to fetched projections for " + $stateParams.EntityName);
                            });
                        };

                        $scope.getAll();
                    }]
            ]);
        });