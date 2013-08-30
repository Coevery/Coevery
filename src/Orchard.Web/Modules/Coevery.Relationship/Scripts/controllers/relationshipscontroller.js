'use strict';

define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'RelationshipsCtrl',
        ['$rootScope', '$scope', 'logger', '$detour', '$resource', '$stateParams',
            function ($rootScope, $scope, logger, $detour, $resource, $stateParams) {
                var relationshipDataService = $resource('api/relationship/Relationship');

                var cellTemplateString = '<div class="ngCellText" ng-class="col.colIndex()" title="{{COL_FIELD}}">' +
                    '<ul class="row-actions pull-right hide">' +
                    '<li class="icon-edit" ng-click="edit(row.entity.ContentId, row.entity.Type)" title="Edit"></li>' +
                    '<li class="icon-remove" ng-click="delete(row.entity.ContentId)" title="Delete"></li>' +
                    '</ul>' +
                    '<span class="btn-link" ng-click="edit(row.entity.ContentId, row.entity.Type)">{{COL_FIELD}}</span>' +
                    '</div>';

                var relationshipColumnDefs = [
                    { field: 'Name', displayName: 'Relationship Name', cellTemplate: cellTemplateString },
                    { field: 'PrimaryEntity', displayName: 'Primary Entity' },
                    { field: 'RelatedEntity', displayName: 'Related Entity' },
                    { field: 'Type', displayName: 'Type' }
                ];

                $scope.selectedItems = [];
                $scope.relationshipGridOptions = {
                    data: 'relationships',
                    selectedItems: $scope.selectedItems,
                    columnDefs: relationshipColumnDefs
                };

                angular.extend($scope.relationshipGridOptions, $rootScope.defaultGridOptions);
                $scope.getAllRelationship = function () {
                    var items = relationshipDataService.query({ EntityName: $stateParams.Id }, function () {
                        if (items == null || items.toLowerCase == "null") {
                            return;
                        }
                        $scope.totalServerItems = items.length;
                        $scope.relationships = items;
                    }, function () {
                        logger.error('Get relationships failed');
                    });
                };

                $scope.createOneToMany = function () {
                    $detour.transitionTo('CreateOneToMany', { EntityName: $stateParams.Id });
                };
                $scope.createManyToMany = function () {
                    $detour.transitionTo('CreateManyToMany', { EntityName: $stateParams.Id });
                };
                $scope.edit = function (contentId, type) {
                    if (type == "OneToMany") {
                        $detour.transitionTo('EditOneToMany', { EntityName: $stateParams.Id, RelationId: contentId });
                    } else if(type == "ManyToMany") {
                        $detour.transitionTo('EditManyToMany', { EntityName: $stateParams.Id, RelationId: contentId });
                    }
                };
                $scope.delete = function (contentId) {
                    relationshipDataService.delete({ RelationshipId: contentId }, function () {
                        $scope.getAllRelationship();
                        logger.success("Delete the item successful.");
                    }, function (result) {
                        logger.error("Failed to delete the relationship:" + result.data.Message);
                    });
                };
            }]
    ]);
});