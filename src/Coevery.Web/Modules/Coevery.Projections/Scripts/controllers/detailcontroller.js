'use strict';
define(['core/app/detourService', 'Modules/Coevery.Projections/Scripts/services/projectiondataservice'], function (detour) {
    detour.registerController([
        'ProjectionDetailCtrl',
        ['$rootScope', '$scope', '$timeout', 'logger', '$state', '$stateParams', '$resource', '$http', 'projectionDataService', '$parse', '$q',
            function ($rootScope, $scope, $timeout, logger, $state, $stateParams, $resource, $http, projectionDataService, $parse, $q) {
                var name = $stateParams.Id;
                var deferred = $q.defer();
                var isInit = true;
                $scope.fieldCoumns = [];
                $scope.SelectedColumns = [];

                $scope.preview = function () {
                    var pool = { list: [] };
                    $scope.$broadcast("getSelectedList", pool);

                    $scope.fieldCoumns = [];
                    $scope.myData = [];
                    var jasondata = {};
                    for (var i = 0; i < pool.list.length; i++) {
                        var fieldNames = pool.list[i].Value.split(".");
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

                    var promise = $http({
                        url: form.attr('action'),
                        method: "POST",
                        data: form.serialize() + '&submit.Save=Save',
                        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                        tracker: 'saveview'
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
                    promise && promise.then(function (response) {
                        var getter = $parse('id');
                        var id = getter(response.data);
                        if (id)
                            $state.transitionTo('ProjectionEdit', { EntityName: $stateParams.EntityName, Id: id });
                    });
                };

                $scope.saveAndBack = function () {
                    var promise = $scope.save();
                    promise && promise.then(function () {
                        $scope.exit();
                    }, function () {
                    });
                };

                $scope.exit = function () {
                    $state.transitionTo('EntityDetail.Views', { Id: $stateParams.EntityName });
                };

            }]
    ]);
});