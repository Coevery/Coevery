'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'FieldCreateChooseTypeCtrl',
        ['$scope', 'logger', '$stateParams', '$detour',
            function ($scope, logger, $stateParams, $detour) {
                var entityName = $stateParams.Id;
                $scope.fieldType = $('#field-type-form input[type=radio]:checked').val();

                $scope.exit = function () {
                    $detour.transitionTo('EntityDetail.Fields', { Id: entityName });
                };

                $scope.next = function () {
                    if ($scope.fieldType) {
                        $scope.$emit('toStep2', $scope.fieldType);
                    }
                };

                $scope.$on('$destroy', function () {
                    if ($detour.current.name != 'EntityDetail.Fields.CreateEditInfo') {
                        $scope.closeDialog();
                    }
                });
                
                $scope.openDialog();
            }]
    ]);
});