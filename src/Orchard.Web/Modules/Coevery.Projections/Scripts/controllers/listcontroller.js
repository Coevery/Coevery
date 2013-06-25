'use strict';

define(['core/app/detourService',
        'Modules/Coevery.Projections/Scripts/services/projectiondataservice'], function(detour) {
            detour.registerController([
                'ProjectionListCtrl',
                ['$rootScope', '$scope', 'logger', '$detour', '$resource', '$stateParams', 'projectionDataService',
                    function($rootScope, $scope, logger, $detour, $resource, $stateParams, projectionDataService) {

                        var t = function(str) {
                            var result = i18n.t(str);
                            return result;
                        };
                        var actionTemplate = '<div class="ngCellText" ng-class="col.colIndex()"><a ng-click="edit(row.getProperty(col.field))">Edit</a>';
                        actionTemplate += '&nbsp;<a ng-click="delete(row.getProperty(col.field))">Delete</a>';
                        actionTemplate += '&nbsp;<a ng-click="setDefault(row.getProperty(col.field))">Default</a></div>';
                        var columnDefs = [
                            { field: 'ContentId', displayName: 'Actions', width: 130, cellTemplate: actionTemplate },
                            { field: 'ContentId', displayName: t('Id') },
                            { field: 'EntityType', displayName: t('EntityType') },
                            { field: 'DisplayName', displayName: t('DisplayName') },
                            { field: 'Default', displayName: t('Default') }];
                        $scope.mySelections = [];

                        $scope.gridOptions = {
                            data: 'myData',
                            //showSelectionCheckbox: true,
                            selectedItems: $scope.mySelections,
                            multiSelect: true,
                            enableColumnResize: true,
                            enableColumnReordering: true,
                            columnDefs: columnDefs
                        };

                        $scope.exit = function () {
                            //$detour.transitionTo('EntityDetail', { Id: $stateParams.EntityName });
                            location.href = 'SystemAdmin#/Entities/' + $stateParams.EntityName;
                        };
                        
                        $scope.delete = function(id) {
                            projectionDataService.delete({ Id: id }, function() {
                                $scope.getAll();
                                logger.success('Delete the ' + id + ' successful.');
                            }, function() {
                                logger.error('Failed to delete the ' + id);
                            });
                        };

                        $scope.add = function() {
                            $detour.transitionTo('ProjectionCreate', { EntityName: $stateParams.EntityName });
                        };

                        $scope.edit = function(id) {
                            $detour.transitionTo('ProjectionEdit', { EntityName: $stateParams.EntityName, Id: id });
                        };

                        $scope.setDefault = function (id) {
                            var result = projectionDataService.save({ Id: id, EntityType: $stateParams.EntityName }, function () {
                                $scope.getAll();
                            }, function () {
                                
                            });
                           
                        };

                        $scope.getAll = function() {
                            var records = projectionDataService.query({ Name: $stateParams.EntityName }, function () {
                                $scope.myData = records;
                            }, function() {
                                logger.error("Failed to fetched projections for " + $stateParams.EntityName);
                            });
                        };

                        $scope.getAll();
                    }]
            ]);
        });