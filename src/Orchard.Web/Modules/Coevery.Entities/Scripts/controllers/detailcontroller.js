'use strict';

define(['core/app/detourService', 'Modules/Coevery.Entities/Scripts/services/entitydataservice'], function (detour) {
    detour.registerController([
        'EntityDetailCtrl',
        ['$scope', 'logger', '$state', '$stateParams','entityDataService',
            function ($scope, logger, $state, $stateParams, entityDataService) {

                $scope.exit = function() {
                    $state.transitionTo('EntityList');
                };
                
                $scope.edit = function () {
                    $state.transitionTo('EntityEdit', { Id: $stateParams.Id });
                };

                $scope.delete = function () {
                    entityDataService.delete({ name: $stateParams.Id }, function () {
                        $state.transitionTo('EntityList');
                        logger.success("Delete the entity successful.");
                    }, function (reason) {
                        logger.error("Failed to delete the entity:" + reason);
                    });
                };

                $scope.formDesigner = function() {
                    $state.transitionTo('FormDesigner', { EntityName: $stateParams.Id });
                };
            }]
    ]);
});