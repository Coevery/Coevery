'use strict';

define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'FieldEditCtrl',
        ['$scope', 'logger', '$state', '$stateParams', '$http',
            function ($scope, logger, $state, $stateParams, $http) {
                var entityName = $stateParams.EntityName;
                
                var validator = $("form[name=myForm]").validate({
                    errorClass: "inputError"
                });

                $scope.exit = function () {
                    $state.transitionTo('EntityDetail.Fields', { Id: entityName });
                };

                $scope.save = function () {
                    if (!validator.form()) {
                        return null;
                    }
  
                    var form = $("form[name=myForm]");
                    var promise = $http({
                        url: form.attr('action'),
                        method: "POST",
                        data: form.serialize() + '&submit.Save=Save',
                        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                        tracker: 'savefield'
                    }).then(function () {
                        logger.success('Save succeeded.');
                    }, function (reason) {
                        logger.error('Save Failed： ' + reason.data);
                    });
                    return promise;
                };

                $scope.saveAndBack = function () {
                    var promise = $scope.save();
                    promise && promise.then(function () {
                        $scope.exit();
                    }, function () {
                    });
                };
            }]
    ]);
});