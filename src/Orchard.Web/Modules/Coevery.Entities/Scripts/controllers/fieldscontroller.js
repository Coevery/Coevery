'use strict';

define(['core/app/detourService', 'Modules/Coevery.Entities/Scripts/services/entitydataservice', 'Modules/Coevery.Entities/Scripts/services/fielddataservice'], function (detour) {
    detour.registerController([
        'FieldsCtrl',
        ['$rootScope', '$scope', 'logger', '$detour', '$stateParams', '$dialog', 'fieldDataService',
            function ($rootScope, $scope, logger, $detour, $stateParams, $dialog, fieldDataService) {

                $scope.selectedItems = [];
                var entityName = $stateParams.Id;
                var fieldColumnDefs = [
                    { name: 'Name', label: 'Field Name', width: 170, formatter: $rootScope.cellLinkTemplate },
                    {
                        name: 'DisplayName', label: 'Field Label', width: 180
                    },
                    { name: 'FieldType', label: 'Field Type', width: 180 },
                    { name: 'Type', label: 'Type' ,width: 170},
                    { name: 'ControlField', label: 'Control Field',width: 170 }
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
                    url: "api/entities/field?name=" + entityName,
                    colModel: fieldColumnDefs
                };

                angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);

                //Dialog action

                $scope.$on('toStep2', function (event, fieldInfo) {
                    $detour.transitionTo('EntityDetail.Fields.CreateEditInfo', { Id: entityName, FieldTypeName: fieldInfo });
                });

                $scope.$on('toStep1', function () {
                    $detour.transitionTo('EntityDetail.Fields.Create', { Id: entityName });
                });

                $scope.dialog = null;

                $scope.$watch('dialog._open', function (newValue, oldValue) {
                    if (newValue == false && oldValue == true &&
                        ($detour.current.name == 'EntityDetail.Fields.Create' ||
                            $detour.current.name == 'EntityDetail.Fields.CreateEditInfo')) {
                        $detour.transitionTo('EntityDetail.Fields', { Id: entityName });
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
                    $detour.transitionTo('EntityDetail.Fields.Create', { Id: entityName });
                };

                var deleteField;
                $scope.delete = function (fieldName) {
                    deleteField = fieldName;
                    $('#myModal').modal({
                        backdrop: 'static',
                        keyboard: true
                    });
                };

                $scope.deleteField = function () {
                    $('#myModal').modal('hide');
                    fieldDataService.delete({ name: deleteField, parentname: entityName }, function () {
                        logger.success("Delete the field successful.");
                        $scope.getAllField();
                    }, function (reason) {
                        logger.error("Failed to delete the field:" + reason);
                    });
                };

                $scope.edit = function (fieldName) {
                    $detour.transitionTo('FieldEdit.Items', { EntityName: entityName, FieldName: fieldName });
                };
                $scope.gotoDependency = function () {
                    $detour.transitionTo('FieldDependencyList', { EntityName: entityName });
                };

                $scope.getAllField = function () {
                    $("#fieldList").jqGrid('setGridParam', {
                        datatype: "json"
                    }).trigger('reloadGrid');
                };
            }]
    ]);
});
