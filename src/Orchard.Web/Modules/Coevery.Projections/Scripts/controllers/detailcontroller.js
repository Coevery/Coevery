'use strict';
define(['core/app/detourService', 'Modules/Coevery.Projections/Scripts/services/projectiondataservice', 'Modules/Coevery.Projections/Scripts/services/propertydataservice'], function (detour) {
    detour.registerController([
        'ProjectionDetailCtrl',
        ['$rootScope', '$scope', '$timeout', 'logger', '$state', '$stateParams', '$resource', '$http', 'projectionDataService', 'propertyDataService', '$parse', '$q',
            function ($rootScope, $scope, $timeout, logger, $state, $stateParams, $resource, $http, projectionDataService, propertyDataService, $parse, $q) {
                var name = $stateParams.Id;
                var deferred = $q.defer();
                var isInit = true;
                $scope.fieldCoumns = [];
                $scope.SelectedColumns = [];

                $scope.preview = function () {
                    $scope.fieldCoumns = [];
                    $scope.myData = [];
                    var jasondata = {};
                    for (var i = 0; i < $scope.SelectedColumns.length; i++) {
                        var fieldNames = $scope.SelectedColumns[i].FieldName.split(".");
                        $scope.fieldCoumns[i] = { name: fieldNames[1], label: fieldNames[1] };
                        jasondata[fieldNames[1]] = "data_" + fieldNames[1];
                    }
                    if (i > 0) {
                        for (var j = 0; j < 5; j++) {
                            var newjason = {};
                            for (var filed in jasondata) {
                                newjason[filed] = jasondata[filed] + "_" + (j + 1);
                            }
                            $scope.myData.push(newjason);
                        }
                    }
                    deferred.resolve();
                    $scope.changePreview();
                };

                $scope.changePreview = function () {
                    deferred.promise.then(function () {
                        $scope.gridOptions = {
                            colModel: $scope.fieldCoumns,
                            needReloading: !isInit
                        };
                        isInit = false;
                        angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);
                        $scope.gridOptions.datatype = "local";
                        $scope.gridOptions.data = $scope.myData;
                    });
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
                            $state.transitionTo('ProjectionEdit', { EntityName: $stateParams.EntityName, Id: id });
                    });
                    return promise;
                };

                $scope.saveAndBack = function () {
                    var promise = $scope.save();
                    promise.then(function () {
                        $scope.exit();
                    }, function () {
                    });
                    return promise;
                };

                $scope.exit = function () {
                    $state.transitionTo('EntityDetail.Views', { Id: $stateParams.EntityName });
                };

                $scope.addfield = function (fieldName, displayName) {
                    var selectedField = { FieldName: fieldName, DisplayName: displayName };
                    $scope.SelectedColumns.splice($scope.SelectedColumns.length, 0, selectedField);
                };

                $scope.removefield = function (index) {
                    $scope.SelectedColumns.splice(index, 1);
                };

                $scope.AddAll = function () {
                    $.each($('td.unselectedField'), function (i, v) {
                        var displayName = $(v).text();
                        var fieldName = $(v).attr('filed-type');
                        var exsitsItem = $($scope.SelectedColumns).filter(function () {
                            return this.FieldName == fieldName;
                        });
                        if (exsitsItem.length <= 0) {
                            $scope.addfield(fieldName, displayName);
                        }
                    });
                };

                $scope.LabelClass = function (fieldName) {
                    for (var i = 0; i < $scope.SelectedColumns.length; i++) {
                        if ($scope.SelectedColumns[i].FieldName == fieldName) return 'label';
                    }
                    return 'label hide';
                };

                $scope.ButtonStyle = function (fieldName) {
                    var selected = $.grep($scope.SelectedColumns, function (n, i) {
                        return n.FieldName == fieldName;
                    });
                    if (selected.length > 0) {
                        return { 'display': 'none' };
                    }
                    return { 'display': 'block' };
                };


                $scope.InitSeletedFieldData = function () {
                    var id = $stateParams.Id || -1;
                    var properties = propertyDataService.query({ id: id }, function () {
                        properties = properties || [];
                        $.each(properties, function (index, value) {
                            $scope.addfield(value.FieldName, value.DisplayName);
                        });
                    }, function () {

                    });
                };

                $scope.InitSeletedFieldData();

                $scope.dragStart = function (e, ui) {
                    ui.item.data('start', ui.item.index());
                };

                $scope.dragEnd = function (e, ui) {
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