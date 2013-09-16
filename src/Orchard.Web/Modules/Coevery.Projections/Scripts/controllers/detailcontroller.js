'use strict';
define(['core/app/detourService', 'Modules/Coevery.Projections/Scripts/services/projectiondataservice', 'Modules/Coevery.Projections/Scripts/services/viewmodeldataservice'], function(detour) {
    detour.registerController([
        'ProjectionDetailCtrl',
        ['$rootScope', '$scope', '$timeout', 'logger', '$detour', '$stateParams', '$resource','$http', 'projectionDataService', 'viewmodelDataService','$parse',
            function ($rootScope, $scope, $timeout, logger, $detour, $stateParams, $resource, $http, projectionDataService, viewmodelDataService, $parse) {
                var name = $stateParams.Id;
                $scope.mySelections = [];
                $scope.fieldCoumns = [];
                $scope.SelectedColumns = [];

                $scope.gridOptions = {
                    data: 'myData',
                    selectedItems: $scope.mySelections,
                    multiSelect: false,
                    enableRowSelection: false,
                    columnDefs: 'fieldCoumns'
                };
                
                angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);

                $scope.preview = function() {
                    $scope.fieldCoumns = [];
                    $scope.myData = [];
                    var jasondata = { };
                    for (var i = 0; i < $scope.SelectedColumns.length; i++) {
                        var fieldName = $scope.SelectedColumns[i].FieldName;
                        $scope.fieldCoumns[i] = { field: fieldName, displayName: fieldName };
                        jasondata[fieldName] = "data_" + fieldName;
                    }
                    if (i > 0) {
                    for (var j = 0; j < 5; j++) {
                        var newjason = { };
                        for (var filed in jasondata) {
                            newjason[filed] = jasondata[filed] + "_" + (j + 1);
                        }
                        $scope.myData.push(newjason);
                    }
                }
            };

                var validator = $("form[name=myForm]").validate({
                    errorClass: "inputError"
                });

                $scope.save = function () {
                    var form = $("form[name=myForm]");
                    if (!validator.form()) {
                        return null;
                    }
                    var pickListValue = '';
                    for (var i = 0; i < $scope.SelectedColumns.length; i++) {
                        var fieldName = $scope.SelectedColumns[i].FieldName;
                        pickListValue += fieldName + '$';
                    }
                    $('#picklist')[0].value = pickListValue;
                    
                    var promise = $http({
                        url: form.attr('action'),
                        method: "POST",
                        data: form.serialize() + '&submit.Save=Save',
                        headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                    }).then(function (response) {
                        logger.success('Save succeeded.');
                        return response;
                    }, function (reason) {
                        logger.error('Save Failed： ' + reason);
                    });
                    return promise;
                };

                $scope.saveAndView = function () {
                    var promise = $scope.save();
                    promise.then(function (response) {
                        var getter = $parse('id');
                        var id = getter(response.data);
                        if (id)
                            $detour.transitionTo('ProjectionEdit', { EntityName: $stateParams.EntityName, Id: id });
                    });
                };

                $scope.saveAndBack = function () {
                    var promise = $scope.save();
                    promise.then(function () {
                        $scope.exit();
                    }, function () {
                    });
                };
                
                $scope.change = function() {

                };

                $scope.exit = function() {
                    $detour.transitionTo('EntityDetail.Views', { Id: $stateParams.EntityName });
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

                $scope.dragStart = function(e, ui) {
                    ui.item.data('start', ui.item.index());
                };

                $scope.dragEnd = function(e, ui) {
                    var start = ui.item.data('start'),
                        end = ui.item.index();

                    $scope.SelectedColumns.splice(end, 0,
                        $scope.SelectedColumns.splice(start, 1)[0]);

                    $scope.$apply();
                };
                
                $('.sortable-list ul').sortable({
                    placeholder: 'placeholder',
                    forcePlaceholderSize: true,
                    start: $scope.dragStart,
                    update: $scope.dragEnd
                });
            }]
    ]);
});