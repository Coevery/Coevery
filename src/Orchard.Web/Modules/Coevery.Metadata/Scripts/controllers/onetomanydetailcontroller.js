'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'EditOneToManyCtrl',
        ['$scope', 'logger', '$detour', '$stateParams',
            function ($scope, logger, $detour, $stateParams) {
                $scope.recordDeleteBehavior = 1;
                $scope.showRelatedList = true;
                $scope.$watch('required', function (newValue) {
                    if (newValue && $scope.recordDeleteBehavior == 1) {
                        $scope.recordDeleteBehavior = 2;
                    }
                });
                $scope.exit = function () {
                    $detour.transitionTo('EntityDetail.Relationships', { Id: $stateParams.EntityName });
                };
            }]
    ]);
});