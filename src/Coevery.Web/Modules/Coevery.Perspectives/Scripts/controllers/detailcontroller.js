'use strict';
define(['core/app/detourService',
        'Modules/Coevery.Perspectives/Scripts/services/perspectivedataservice',
        'Modules/Coevery.Perspectives/Scripts/services/navigationdataservice'], function (detour) {
            detour.registerController(['PerspectiveDetailCtrl', ['$rootScope', '$timeout', '$scope', 'logger', '$state', '$stateParams',
                    '$resource', 'perspectiveDataService', 'navigationDataService', '$i18next',
                    function ($rootScope, $timeout, $scope, logger, $state, $stateParams,
                        $resource, perspectiveDataService, navigationDataService, $i18next) {
                        var perpectiveId = $stateParams.Id;

                        $scope.exit = function () {
                            $state.transitionTo('PerspectiveList');
                        };

                        $scope.save = function () {
                            var tempForm = $("form[name=myForm]");
                            $.ajax({
                                url: tempForm.action,
                                type: tempForm.method,
                                data: tempForm.serialize() + '&submit.Save=Save',
                                success: function (result) {
                                    logger.success("Perspective Saved.");
                                }
                            });
                        };

                        var navigationColumnDefs = [{
                            name: 'Id',
                            label: $i18next('Id'),
                            key: true
                        }, {
                            name: 'DisplayName',
                            label: $i18next('DisplayName'),
                            formatter: $rootScope.cellLinkTemplate,
                            //'paramAttrs' define the param attributes passed to the edit and delete function 
                            formatoptions: { hasView: true, paramAttrs: ['Id', 'NavigationType'] }
                        }, { name: 'Type', label: $i18next('Type')},
                            { name: 'Description', label: $i18next('Description')},
                            { name: 'NavigationType', label: $i18next('NavigationType'), hidden: true },
                            { name: 'Parent', label: $i18next('Parent'), hidden: true },
                            { name: 'Weight', label: $i18next('Weight'), hidden: true },
                            { name: 'Level', label: $i18next('Level'), hidden: true },
                            { name: 'IsLeaf', label: $i18next('Is Leaf'), hidden: true }
                        ];

                        var gridOptions = {
                            url: "api/perspectives/Navigation?id=" + perpectiveId,
                            colModel: navigationColumnDefs,
                            rowIdName: 'Id',
                            nestedDrag: true,
                            initialLevel: 1,
                            cmTemplate: {
                                sortable: false
                            }
                        };
                        angular.extend(gridOptions, $rootScope.defaultGridOptions);
                        gridOptions.multiselect = false;
                        gridOptions.sortable = false;
                        $scope.gridOptions = gridOptions;

                        $scope.addNavigationItem = function (type) {
                            $state.transitionTo('CreateNavigationItem', { Id: perpectiveId, Type: type});
                        };

                        $scope.edit = function (id,type) {
                            $state.transitionTo('EditNavigationItem', { Id: perpectiveId, NId: id, Type: type });
                        };

                        $scope.view = $scope.edit;

                        $scope.editPerspective = function () {
                            $state.transitionTo('PerspectiveEdit', { Id: perpectiveId });
                        };

                        $scope.saveDeployment = function () {
                            var postdata = [];
                            var depth = 1;
                            var getPosition = function (parent, order) {
                                if (!parent) {
                                    return order;
                                }
                                var parentRecord = $scope.navigationList.getRow(parent);
                                return getPosition(parentRecord.Parent, parentRecord.Weight) + "." + order;
                            };
                            $scope.navigationList.getParam("data").forEach(function (element, index, array) {
                                var currentLevel = element.Level;
                                depth = currentLevel > depth ? currentLevel : depth;
                                var position = getPosition(element.Parent, element.Weight);
                                postdata.push({
                                    NavigationId: element.Id,
                                    Position: position.toString()
                                });
                            });
                            navigationDataService.save({ Id: perpectiveId, Positions: postdata, Depth: depth }, function (data) {
                                logger.success($i18next('Save layout successful.'));
                                $scope.getAllNavigationdata();
                            }, function (response) {
                                logger.error($i18next('Failed to save layout:' + response.data.Message));
                            });
                        };

                        $scope['delete'] = function (id, type) {
                            perspectiveDataService['delete']({ Id: id }, function () {
                                $scope.getAllNavigationdata();
                                logger.success($i18next('Delete the navigation successful.'));
                            }, function (result) {
                                logger.error($i18next('Failed to delete the navigation:' + result));
                            });
                        };

                        $scope.deletePerspective = function () {
                            perspectiveDataService['delete']({ id: perpectiveId }, function () {
                                $scope.exit();
                                logger.success($i18next('Delete the perspective successful.'));
                            }, function (result) {
                                logger.error($i18next('Failed to delete the perspective:') + result.data.Message);
                            });
                        };


                        $scope.getAllNavigationdata = function (needReconstruct) {
                            if (needReconstruct) {
                                var reloadOptions = {
                                    needReloading: true
                                };
                                angular.extend(reloadOptions, gridOptions);
                                $scope.gridOptions = reloadOptions;
                                return;
                            }
                            $scope.navigationList.setParam({
                                datatype: "json"
                            });
                            $scope.navigationList.reload();
                        };
                    }]
            ]);
        });