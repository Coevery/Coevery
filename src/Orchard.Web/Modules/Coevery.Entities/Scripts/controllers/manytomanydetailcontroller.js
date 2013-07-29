'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'EditManyToManyCtrl',
        ['$scope', 'logger', '$detour', '$stateParams',
            function ($scope, logger, $detour, $stateParams) {
                $scope.showPrimaryList = true;
                $scope.showRelatedList = true;
                $scope.exit = function () {
                    $detour.transitionTo('EntityDetail.Relationships', { Id: $stateParams.EntityName });
                };
            }]
    ]);
});