'use strict';

define(['core/app/detourService', 'Modules/Coevery.Entities/Scripts/services/entitydataservice', 'Modules/Coevery.Entities/Scripts/services/fielddataservice'], function (detour) {
    detour.registerController([
        'FieldsCtrl',
        ['$rootScope', '$scope', 'logger', '$detour', '$stateParams', '$dialog', 'entityDataService', 'fieldDataService',
            function ($rootScope, $scope, logger, $detour, $stateParams, $dialog, entityDataService, fieldDataService) {
                var cellTemplateString = '<div class="ngCellText" ng-class="col.colIndex()" title="{{COL_FIELD}}">' +
            '<span class="btn-link" ng-click="view(row.entity.Name)">{{COL_FIELD}}</span>' +
            '<ul class="row-actions pull-right hide">' +
            '<li class="icon-edit" ng-click="edit(row.entity.Name)" title="Edit"></li>' +
            '<li class="icon-remove" ng-click="delete(row.entity.Name)" title="Delete"></li>' +
            '</ul>' +
            '</div>';

                var entityName = $stateParams.Id;
                var fieldColumnDefs = [
                    {
                        field: 'DisplayName', displayName: 'Field Label'
                    },
                    { field: 'Name', displayName: 'Field Name', cellTemplate: cellTemplateString },
                    { field: 'Type', displayName: 'Type' },
                    { field: 'FieldType', displayName: 'Field Type' },
                    { field: 'ControlField', displayName: 'Control Field' }
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
                    data: 'myData',
                    multiSelect: false,
                    enableRowSelection: false,
                    showSelectionCheckbox: false,
                    selectedItems: $scope.selectedItems,
                    columnDefs: fieldColumnDefs,
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
                        $scope.getAllField();
                        logger.success("Delete the field successful.");
                    }, function () {
                        logger.error("Failed to delete the field.");
                    });
                };

                $scope.edit = function (fieldName) {
                    $detour.transitionTo('FieldEdit', { EntityName: entityName, FieldName: fieldName });
                };
                $scope.gotoDependency = function () {
                    $detour.transitionTo('FieldDependencyList', { EntityName: entityName });
                };

                $scope.getAllField = function () {
                    var metaData = entityDataService.get({ name: entityName }, function () {
                        $scope.item = metaData;
                        $scope.myData = metaData.Fields;
                        $.each($scope.myData, function () {
                            var type = this.IsSystemField ? 'System Field' : 'User Field';
                            $.extend(this, { Type: type });
                        });
                        $scope.userFields = [
                            { DisplayName: 'Full Name', Name: 'FullName', FieldType: 'Input Field' }
                        ];
                    }, function () {
                        logger.error("The metadata does not exist.");
                    });
                };
                $scope.getAllField();
            }]
    ]);
});
