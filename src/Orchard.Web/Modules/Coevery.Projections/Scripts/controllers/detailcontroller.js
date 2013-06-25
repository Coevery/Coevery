'use strict';
define(['core/app/detourService', 'Modules/Coevery.Projections/Scripts/services/projectiondataservice', 'Modules/Coevery.Projections/Scripts/services/viewmodeldataservice'], function(detour) {
    detour.registerController([
        'ProjectionDetailCtrl',
        ['$timeout', '$scope', 'logger', '$detour', '$stateParams', '$resource', 'projectionDataService', 'viewmodelDataService',
            function($timeout, $scope, logger, $detour, $stateParams, $resource, projectionDataService, viewmodelDataService) {
                var name = $stateParams.Id;
                $scope.mySelections = [];
                $scope.fieldCoumns = [];
                $scope.SelectedColumns = [];

                $scope.gridOptions = {
                    data: 'myData',
                    selectedItems: $scope.mySelections,
                    multiSelect: false,
                    showColumnMenu: true,
                    enableColumnResize: true,
                    enableColumnReordering: true,
                    columnDefs: 'fieldCoumns'
                };

                $scope.preview = function() {
                    $scope.fieldCoumns = [];
                    for (var i = 0; i < $scope.SelectedColumns.length; i++) {
                        var fieldName = $scope.SelectedColumns[i].FieldName;
                        $scope.fieldCoumns[i] = { field: fieldName, displayName: fieldName };
                    }

                };

                $scope.save = function() {
                    var pickListValue = '';
                    for (var i = 0; i < $scope.SelectedColumns.length; i++) {
                        var fieldName = $scope.SelectedColumns[i].FieldName;
                        pickListValue += fieldName + '$';
                    }
                    $('#picklist')[0].value = pickListValue;
                    $.ajax({
                        url: myForm.action,
                        type: myForm.method,
                        data: $(myForm).serialize() + '&submit.Save=Save',
                        success: function(result) {
                            logger.success("Layout Saved.");
                        }
                    });
                };

                $scope.change = function() {

                };

                $scope.exit = function() {
                    $detour.transitionTo('ProjectionList', { EntityName: $stateParams.EntityName });
                };

                $scope.addfield = function(fieldName) {
                    var selectedField = { FieldName: fieldName };
                    $scope.SelectedColumns.splice($scope.SelectedColumns.length, 0, selectedField);

                };

                $scope.removefield = function(index) {
                    $scope.SelectedColumns.splice(index, 1);
                };

                $scope.AddAll = function() {
                    $.each($('td[name = "unselectedField"]'), function (i, v) {
                        var fieldName = $(v).text();
                        var exsitsItem = $($scope.SelectedColumns).filter(function () {
                            return this.FieldName == fieldName;
                        }).first();
                        if (exsitsItem.length <= 0) {
                            $scope.addfield(fieldName);
                        }
                    });
                };

                $scope.LabelClass = function(fieldName) {
                    for (var i = 0; i < $scope.SelectedColumns.length; i++) {
                        if ($scope.SelectedColumns[i].FieldName == fieldName) return 'label';
                    }
                    return 'label hide';
                };

                $scope.ButtonStyle = function(fieldName) {
                    for (var i = 0; i < $scope.SelectedColumns.length; i++) {
                        if ($scope.SelectedColumns[i].FieldName == fieldName)
                            return { 'display': 'none' };
                    }
                    return { 'display': 'block' };
                };


                $scope.InitSeletedFieldData = function() {
                    var viewModel = viewmodelDataService.query({ id: $stateParams.Id }, function() {
                        for (var i = 0; i < viewModel.length; i++) {
                            $scope.addfield(viewModel[i].FieldName);
                        }
                    }, function() {

                    });
                };
                $scope.InitSeletedFieldData();
            }]
    ]);
});