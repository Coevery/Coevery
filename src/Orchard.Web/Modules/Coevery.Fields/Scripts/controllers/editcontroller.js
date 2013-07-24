'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'FieldEditCtrl',
        ['$scope', 'logger', '$detour', '$stateParams','$http',
            function ($scope, logger, $detour, $stateParams,$http) {
                var entityName = $stateParams.EntityName;
                var fieldName = $stateParams.FieldName;
                
                $scope.exit = function () {
                    $detour.transitionTo('EntityDetail.Fields', { Id: entityName });
                };

                $scope.save = function () {
                    var form = angular.element(myForm);
                    var promise = $http({
                        url: form.attr('action'),
                        method: "POST",
                        data: form.serialize() + '&submit.Save=Save',
                        headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                    }).then(function () {
                        logger.success('Save succeeded.');
                    }, function (reason) {
                        logger.error('Save Failed： ' + reason);
                    });
                    return promise;
                };
                
                $scope.saveAndBack = function () {
                    var promise = $scope.save();
                    promise.then(function () {
                        $scope.exit();
                    }, function () {
                    });
                };

                $scope.viewItems = function () {
                    $detour.transitionTo('FieldItems', { EntityName: entityName, FieldName: fieldName });
                };
            }]
    ]);
});