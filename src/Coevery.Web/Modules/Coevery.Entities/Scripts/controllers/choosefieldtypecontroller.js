'use strict';
define(['core/app/detourService'], function(detour) {
    detour.registerController([
        'ChooseFieldTypeCtrl',
        ['$scope', 'logger', '$stateParams', '$state',
            function($scope, logger, $stateParams, $state) {
                var entityName = $stateParams.Id;
                $scope.$parent.fieldInfo = null;
                
                $scope.$on('wizardGoNext', function (event, context) {
                    if ($scope.fieldType) {
                        context.stateParams = {
                            Id: entityName,
                            FieldTypeName: $scope.fieldType
                        };
                    } else {
                        context.cancel = true;
                        $("#error-tip").show();
                    }
                });
                
                $scope.fieldType = $('#field-type-form input[type=radio]:checked').val();
                $scope.radios = $('#field-type-form input[type=radio]');

                $scope.radios.click(function() {
                    $("#error-tip").hide();
                });
            }]
    ]);
});