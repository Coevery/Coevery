'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'CreateOneToManyCtrl',
        ['$scope', 'logger', '$detour', '$stateParams', '$http',
            function ($scope, logger, $detour, $stateParams, $http) {
                $scope.showRelatedList = true;
                $scope.$watch('required', function (newValue) {
                    if (newValue && $scope.recordDeleteBehavior == 1) {
                        $scope.recordDeleteBehavior = 2;
                    }
                });
                $scope.save = function () {
                    $("input.primary-entity").prop('disabled', false);
                    var form = $('#onetomany-form');                  
                    if (!checkValid(form)) {
                        return null;
                    }
                    var promise = $http({
                        url: form.attr('action'),
                        method: form.attr('method'),
                        data: form.serialize(),
                        headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                    }).then(function () {
                        logger.success('success');
                        $("input.primary-entity").prop('disabled', true);
                    }, function (result) {
                        logger.error('Failed:\n' + result.responseText);
                        $("input.primary-entity").prop('disabled', true);
                    });
                    return promise;
                };

                $scope.exit = function () {
                    $detour.transitionTo('EntityDetail.Relationships', { Id: $stateParams.EntityName });
                };
                
                $scope.saveAndBack = function () {
                    var promise = $scope.save();
                    promise && promise.then(function () {
                        $scope.exit();
                    });
                };
            }]
    ]);
});

function checkValid(form) {
    var validator = form.validate();
    if (!validator) {
        return false;
    }
    if (!validator.form()) {
        return false;
    }
    return true;
};