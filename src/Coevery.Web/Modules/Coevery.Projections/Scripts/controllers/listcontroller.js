'use strict';

define(['core/app/detourService',
        'Modules/Coevery.Projections/Scripts/services/projectiondataservice'], function(detour) {
            detour.registerController([
                'ProjectionListCtrl',
                ['$rootScope', '$scope', 'logger', '$state', '$resource', '$stateParams', 'projectionDataService', '$i18next',
                    function($rootScope, $scope, logger, $state, $resource, $stateParams, projectionDataService, $i18next) {
                        var columnDefs = [
                            { name: 'ContentId', label: $i18next('Content Id'), hidden: true ,key:true},
                            {
                                name: 'DisplayName',
                                label: $i18next('Display Name'),
                                formatter: $rootScope.cellLinkTemplate,
                                formatoptions: { hasDefault: true }
                            },
                            { name: 'Default', label: $i18next('Default') }];

                        $scope.gridOptions = {
                            url: "api/projections/Projection?id=" + $stateParams.Id,
                            colModel: columnDefs
                        };

                        angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);

                        $scope.exit = function() {
                            $state.transitionTo('EntityDetail.Fields', { Id: $stateParams.Id });
                        };

                        $scope['delete'] = function (id) {
                            var deleteView = id || $scope.selectedItems.length > 0 ? $scope.selectedItems[0] : null;
                            if (!deleteView) return;
                            projectionDataService['delete']({ Id: deleteView }, function() {
                                if ($scope.selectedItems.length != 0) {
                                    $scope.selectedItems.pop();
                                }
                                $scope.getAll();
                                logger.success($i18next('Delete the view successful.'));
                            }, function(result) {
                                logger.error($i18next("Failed to delete the view:") + result.data.Message);
                            });

                        };

                        $scope.add = function(category,type) {
                            $state.transitionTo('ProjectionCreate', { EntityName: $stateParams.Id, Category: category, Type: type });
                        };

                        $scope.edit = function(id) {
                            $state.transitionTo('ProjectionEdit', { EntityName: $stateParams.Id, Id: id });
                        };

                        $scope.setDefault = function(id) {
                            var result = projectionDataService.save({ Id: id, EntityType: $stateParams.Id }, function() {
                                if ($scope.selectedItems.length != 0) {
                                    $scope.selectedItems.pop();
                                }
                                $scope.getAll();
                            }, function() {

                            });
                        };

                        $scope.getAll = function() {
                            $("#viewList").jqGrid('setGridParam', {
                                datatype: "json"
                            }).trigger('reloadGrid');
                        };
                        $scope.refreshTab();
                    }]
            ]);
        });