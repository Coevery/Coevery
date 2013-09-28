'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'FieldCreateChooseTypeCtrl',
        ['$scope', 'logger', '$stateParams', '$state',
            function ($scope, logger, $stateParams, $state) {
                var entityName = $stateParams.Id;
                $scope.fieldType = $('#field-type-form input[type=radio]:checked').val();
                $scope.radios = $('#field-type-form input[type=radio]');

                $scope.exit = function () {
                    $state.transitionTo('EntityDetail.Fields', { Id: entityName });
                };

                $scope.next = function () {
                    if ($scope.fieldType) {
                        $scope.$emit('toStep2', $scope.fieldType);
                    } else {
                        $("#error-tip").show();
                    }
                };

                $scope.$on('$destroy', function () {
                    if ($state.current.name != 'EntityDetail.Fields.CreateEditInfo' && $scope.$parent.dialog != null) {
                        $scope.closeDialog();
                    }
                });
                
                $scope.openDialog();

                $scope.radios.click(function() {
                    $("#error-tip").hide();
                });
            }]
    ]);
});