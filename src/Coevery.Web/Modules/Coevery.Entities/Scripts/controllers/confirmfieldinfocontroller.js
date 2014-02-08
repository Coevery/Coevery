'use strict';
define(['core/app/detourService'], function(detour) {
    detour.registerController([
        'ConfirmFieldInfoCtrl',
        ['$scope', 'logger', '$stateParams', '$state', '$http',
            function($scope, logger, $stateParams, $state, $http) {
                var entityName = $stateParams.Id,
                    fieldType = $stateParams.FieldTypeName;

                $scope.fieldInfo = $scope.$parent.fieldInfo;
                $scope.addInLayout = true;

                $scope.$on('wizardGoBack', function(event, context) {
                    context.stateParams = {
                        Id: entityName,
                        FieldTypeName: fieldType
                    };
                });

                $scope.$on('wizardComplete', function(event, context) {
                    $http({
                        url: $scope.fieldInfo.formAction,
                        method: $scope.fieldInfo.formMethod,
                        data: $scope.fieldInfo.formData + '&' + $('#AddInLayout').serialize(),
                        headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                    }).then(function() {
                        logger.success('success');
                    }, function(reason) {
                        logger.error('Failed:\n' + reason.data);
                    });
                });
            }]
    ]);
});