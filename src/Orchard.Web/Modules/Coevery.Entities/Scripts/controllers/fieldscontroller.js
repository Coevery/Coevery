'use strict';

define(['core/app/detourService', 'Modules/Coevery.Entities/Scripts/services/entitydataservice', 'Modules/Coevery.Entities/Scripts/services/fielddataservice'], function (detour) {
    detour.registerController([
        'FieldsCtrl',
        ['$rootScope', '$scope', 'logger', '$state', '$stateParams', '$dialog', 'fieldDataService',
            function ($rootScope, $scope, logger, $state, $stateParams, $dialog, fieldDataService) {
                $scope.$parent.showField = true;
                $scope.selectedItems = [];
                var entityName = $stateParams.Id;
                $scope.idAttr = "Name"; //The attribute represent the id of a row
                var fieldColumnDefs = [
                    { name: 'Name', label: 'Field Name', formatter: $rootScope.cellLinkTemplate },
                    { name: 'Id', label: 'Id', hidden: true },
                    {
                        name: 'DisplayName', label: 'Field Label'
                    },
                    { name: 'FieldType', label: 'Field Type' },
                    { name: 'Type', label: 'Type' },
                    { name: 'ControlField', label: 'Control Field' }
                ];

                function test() {
                    return $('#modalTemplate').children();
                }
                $scope.opts = {
                    backdrop: true,
                    backdropFade: true,
                    dialogFade: true,
                    backdropClick: false,
                    keyboard: true,
                    template: test, // OR: templateUrl: 'path/to/view.html',
                };

                $scope.gridOptions = {
                    url: "api/entities/field?name=" + $scope.metaId,
                    colModel: fieldColumnDefs
                };

                angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);

                //Dialog action

                $scope.$on('toStep2', function (event, fieldInfo) {
                    $state.transitionTo('EntityDetail.Fields.CreateEditInfo', { Id: entityName, FieldTypeName: fieldInfo });
                });

                $scope.$on('toStep1', function () {
                    $state.transitionTo('EntityDetail.Fields.Create', { Id: entityName });
                });

                $scope.dialog = null;

                $scope.$watch('dialog._open', function (newValue, oldValue) {
                    if (newValue == false && oldValue == true &&
                        ($state.current.name == 'EntityDetail.Fields.Create' ||
                            $state.current.name == 'EntityDetail.Fields.CreateEditInfo')) {
                        $state.transitionTo('EntityDetail.Fields', { Id: entityName });
                    }
                });

                $scope.openDialog = function () {
                    if ($scope.dialog) {
                        $scope.dialog.modalEl.html(test());
                    } else {
                        $scope.dialog = $dialog.dialog($scope.opts);
                        $scope.dialog.open();
                    }
                };
                $scope.closeDialog = function () {
                    $scope.dialog.close();
                    $scope.dialog = null;
                };

                //Page action

                $scope.add = function () {
                    $state.transitionTo('EntityDetail.Fields.Create', { Id: entityName });
                };

                $scope.delete = function () {
                    var deleteField = $scope.selectedItems.length > 0 ? $scope.selectedItems[0] : null;
                    if (!deleteField) return;
                    fieldDataService.delete({ name: deleteField, entityName: entityName }, function () {
                        $scope.selectedItems = [];
                        logger.success("Delete the field successful.");
                        $scope.getAllField();
                    }, function (reason) {
                        logger.error("Failed to delete the field:" + reason.Message);
                    });
                };

                $scope.edit = function (fieldName) {
                    $state.transitionTo('FieldEdit.Items', { EntityName: entityName, FieldName: fieldName });
                };
                $scope.gotoDependency = function () {
                    $state.transitionTo('FieldDependencyList', { EntityName: entityName });
                };

                $scope.getAllField = function () {
                    $("#fieldList").jqGrid('setGridParam', {
                        datatype: "json"
                    }).trigger('reloadGrid');
                };

                $scope.refreshTab();
            }]
    ]);
});
