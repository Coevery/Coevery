'use strict';

define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'EntityDetailCtrl',
        ['$scope', 'logger', '$detour', '$stateParams', '$resource',
            function ($scope, logger, $detour, $stateParams) {

                var entityName = $stateParams.Id;

                $scope.exit = function() {
                    $detour.transitionTo('EntityList');
                };

                $scope.listViewDesigner = function() {
                    $detour.transitionTo('ProjectionList', { EntityName: entityName });
                };
                $scope.formDesigner = function() {
                    location.href = 'Metadata/FormDesignerViewTemplate/Index/' + entityName;
                };
            }]
    ]);
});