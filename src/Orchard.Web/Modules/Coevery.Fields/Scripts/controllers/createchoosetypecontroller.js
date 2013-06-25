'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'FieldCreateChooseTypeCtrl',
        ['$scope', 'logger', '$detour', '$stateParams',
            function ($scope, logger, $detour, $stateParams) {
                var entityName = $stateParams.EntityName;
                $scope.exit = function () {
                    $detour.transitionTo('EntityDetail.Fields', { Id: entityName });
                };

                $scope.next = function () {
                    if ($scope.fieldType) {
                        $detour.transitionTo('FieldCreateEditInfo', { EntityName: entityName, FieldTypeName: $scope.fieldType });
                    }
                };

                $scope.fieldType = $('#field-type-form input:first').val();
            }]
    ]);
});