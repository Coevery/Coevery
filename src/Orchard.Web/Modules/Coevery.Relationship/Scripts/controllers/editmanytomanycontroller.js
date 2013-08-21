'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'EditManyToManyCtrl',
        ['$scope', 'logger', '$detour', '$stateParams', '$http',
            function ($scope, logger, $detour, $stateParams, $http) {
                
                $scope.save = function () {
                    ToggleReadonly(false);
                    var form = $('#manytomany-form');
                    var promise = $http({
                        url: form.attr('action'),
                        method: form.attr('method'),
                        data: form.serialize(),
                        headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                    }).then(function () {
                        logger.success('success');
                    }, function (result) {
                        logger.error('Failed:\n' + result.responseText);
                    });
                    ToggleReadonly(true);
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

function ToggleReadonly(condition) {
    $("input.primary-entity").prop('disabled', condition);
    $("#relation-name").prop('disabled', condition);
    $("input.related-entity").prop('disabled', condition);
}