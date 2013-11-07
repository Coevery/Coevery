'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'CreateManyToManyCtrl',
        ['$scope', 'logger', '$state', '$stateParams', '$http', '$parse',
            function ($scope, logger, $state, $stateParams, $http, $parse) {
                
                $scope.showPrimaryList = true;
                $scope.showRelatedList = true;
                
                var validator = $("#manytomany-form").validate({
                    errorClass: "inputError"
                });

                $scope.save = function () {
                    $("input.primary-entity").prop('disabled', false);
                    var form = $('#manytomany-form');
                    if (!validator.form()) {
                        return null;
                    }

                    var promise = $http({
                        url: form.attr('action'),
                        method: form.attr('method'),
                        data: form.serialize(),
                        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                        tracker: 'saverelation'
                    }).then(function (response) {
                        logger.success('success');
                        $("input.primary-entity").prop('disabled', true);
                        return response;
                    }, function (result) {
                        logger.error('Failed:\n' + result);
                        $("input.primary-entity").prop('disabled', true);
                    });
                    return promise;
                };

                $scope.exit = function () {
                    $state.transitionTo('EntityDetail.Relationships', { Id: $stateParams.EntityName });
                };

                $scope.saveAndView = function () {
                    var promise = $scope.save();
                    promise && promise.then(function (response) {
                        var getter = $parse('relationId');
                        var relationId = getter(response.data);
                        if (relationId)
                            $state.transitionTo('EditManyToMany', { EntityName: $stateParams.EntityName, RelationId: relationId });
                    });
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
