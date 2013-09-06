'use strict';

define(['core/app/detourService', 'Modules/Coevery.Entities/Scripts/services/entitydataservice'], function (detour) {
    detour.registerController([
        'EntityDetailCtrl',
        ['$scope', 'logger', '$detour', '$stateParams','entityDataService',
            function ($scope, logger, $detour, $stateParams, entityDataService) {

                $scope.exit = function() {
                    $detour.transitionTo('EntityList');
                };
                
                $scope.edit = function () {
                    $detour.transitionTo('EntityEdit', { Id: $stateParams.Id });
                };

                $scope.delete = function() {
                    entityDataService.delete({ name: $stateParams.Id }, function () {
                        $detour.transitionTo('EntityList');
                        logger.success("Delete the entity successful.");
                    }, function (reason) {
                        logger.error("Failed to delete the entity." + reason);
                    });
                };
                
                $scope.formDesigner = function() {
                    $detour.transitionTo('FormDesigner', { EntityName: $stateParams.Id });
                };
            }]
    ]);
});