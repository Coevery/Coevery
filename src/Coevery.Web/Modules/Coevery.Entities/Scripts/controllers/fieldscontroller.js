'use strict';

define(['core/app/detourService', 'Modules/Coevery.Entities/Scripts/services/entitydataservice', 'Modules/Coevery.Entities/Scripts/services/fielddataservice'], function(detour) {
    detour.registerController([
        'FieldsCtrl',
        ['$rootScope', '$scope', 'logger', '$state', '$stateParams', '$dialog', 'fieldDataService',
            function($rootScope, $scope, logger, $state, $stateParams, $dialog, fieldDataService) {
                $scope.$parent.showField = true;
                $scope.selectedItems = [];
                var entityName = $stateParams.Id;
                var fieldColumnDefs = [
                    { name: 'Id', label: 'Id', hidden: true },
                    { name: 'Name', label: 'Field Name', formatter: $rootScope.cellLinkTemplate, key: true },
                    { name: 'DisplayName', label: 'Field Label' },
                    { name: 'FieldType', label: 'Field Type' },
                    { name: 'Type', label: 'Type' },
                    //{ name: 'ControlField', label: 'Control Field' }
                ];

                $scope.gridOptions = {
                    url: "api/entities/field?name=" + $scope.metaId,
                    rowIdName: "Name",
                    colModel: fieldColumnDefs
                };
                angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);
                $scope['delete'] = function () {
                    var deleteField = $scope.selectedItems.length > 0 ? $scope.selectedItems : null;
                    if (!deleteField) return;
                    fieldDataService['delete']({ name: deleteField, entityName: entityName }, function () {
                        $scope.selectedItems = [];
                        logger.success("Delete the field successful.");
                        $scope.getAllField();
                    }, function(reason) {
                        logger.error("Failed to delete the field:" + reason.Message);
                    });
                };

                $scope.edit = function(fieldName) {
                    $state.transitionTo('FieldEdit.Items', { EntityName: entityName, FieldName: fieldName });
                };
                $scope.gotoDependency = function() {
                    $state.transitionTo('FieldDependencyList', { EntityName: entityName });
                };

                $scope.getAllField = function() {
                    $("#fieldList").jqGrid('setGridParam', {
                        datatype: "json"
                    }).trigger('reloadGrid');
                };

                $scope.refreshTab();

                //Dialog action
                $scope.wizardOptions = {
                    states: [
                        'EntityDetail.Fields.Create',
                        'EntityDetail.Fields.CreateFillInfo',
                        'EntityDetail.Fields.CreateConfirmInfo'
                    ],
                    title: 'Add new Field',
                    closeFunc: closeWizard,
                    completeFunc: completeFunc
                };

                function closeWizard() {
                    $state.transitionTo('EntityDetail.Fields', { Id: entityName });
                }

                function completeFunc() {
                    setTimeout($scope.getAllField, 200);
                }

                $scope.add = function() {
                    $state.transitionTo('EntityDetail.Fields.Create', { Id: entityName });
                };
            }]
    ]);
});