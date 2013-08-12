'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'FieldCreateChooseTypeCtrl',
        ['$scope', 'logger', '$stateParams', '$detour',
            function ($scope, logger, $stateParams, $detour) {
                var entityName = $stateParams.Id;
                $scope.fieldType = $('#field-type-form input[type=radio]:checked').val();
                $scope.radios = $('#field-type-form input[type=radio]');

                $scope.exit = function () {
                    $detour.transitionTo('EntityDetail.Fields', { Id: entityName });
                };

                $scope.next = function () {
                    if ($scope.fieldType) {
                        $scope.$emit('toStep2', $scope.fieldType);
                    } else {
                        $("#error-tip").show();
                    }
                };

                $scope.$on('$destroy', function () {
                    if ($detour.current.name != 'EntityDetail.Fields.CreateEditInfo' && $scope.$parent.dialog != null) {
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