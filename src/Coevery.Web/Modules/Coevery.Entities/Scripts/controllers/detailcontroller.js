'use strict';

define(['core/app/detourService', 'Modules/Coevery.Entities/Scripts/services/entitydataservice'], function (detour) {
    detour.registerController([
        'EntityDetailCtrl',
        ['$scope', 'logger', '$state', '$stateParams', 'entityDataService', '$http',
            function ($scope, logger, $state, $stateParams, entityDataService, $http) {
                $scope.refreshTab = function() {
                    $scope.showField = $state.includes('EntityDetail.Fields');
                    $scope.showRelation = $state.includes('EntityDetail.Relationships');
                    $scope.showView = $state.includes('EntityDetail.Views');
                };
                
                $scope.exit = function () {
                    $state.transitionTo('EntityList');
                };

                $scope.edit = function () {
                    $state.transitionTo('EntityEdit', { Id: $stateParams.Id });
                };

                $scope['delete'] = function (id) {
                    entityDataService['delete']({ name: id }, function () {
                        $state.transitionTo('EntityList');
                        logger.success("Delete the entity successful.");
                    }, function (reason) {
                        logger.error("Failed to delete the entity:" + reason);
                    });
                };

                $scope.formDesigner = function () {
                    $state.transitionTo('FormDesigner', { EntityName: $stateParams.Id });
                };
                
                $scope.publish = function () {
                    $http.get('Entities/SystemAdmin/Publish/' + $stateParams.Id).then(function () {
                        logger.success("Publish the entity successful.");
                    });
                };
            }]
    ]);
});