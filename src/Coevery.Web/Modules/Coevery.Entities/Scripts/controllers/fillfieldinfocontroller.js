'use strict';
define(['core/app/detourService'], function(detour) {
    detour.registerController([
        'FillFieldInfoCtrl',
        ['$scope', 'logger', '$stateParams', '$state', '$http',
            function($scope, logger, $stateParams, $state, $http) {
                var entityName = $stateParams.Id;
                if ($scope.$parent.fieldInfo) {
                    $scope.fieldInfo = $scope.$parent.fieldInfo;
                } else {
                    $scope.fieldInfo = {};
                }

                var checkValid = function(form) {
                    var validator = form.validate();
                    if (!validator) {
                        return false;
                    }
                    if (!validator.form()) {
                        return false;
                    }
                    if (!validator.element("#inputFieldName")) {
                        return false;
                    }
                    return true;
                };

                $scope.$on('wizardGoBack', function(event, context) {
                    context.stateParams = {
                        Id: entityName
                    };
                });

                $scope.$on('wizardGoNext', function(event, context) {
                    if (!checkValid($("#field-info-form"))) {
                        context.cancel = true;
                        return;
                    }

                    context.stateParams = {
                        Id: entityName,
                        FieldTypeName: $stateParams.FieldTypeName
                    };

                    var form = $('#field-info-form');
                    $scope.fieldInfo.fieldName = $("#inputFieldName").val();
                    $scope.fieldInfo.formData = form.serialize();
                    $scope.fieldInfo.formAction = form.attr('action');
                    $scope.fieldInfo.formMethod = form.attr('method');

                    $scope.$parent.fieldInfo = $scope.fieldInfo;
                });
            }]
    ]);
});