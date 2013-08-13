'use strict';

define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'RelationshipsCtrl',
        ['$rootScope', '$scope', 'logger', '$detour', '$stateParams',
            function ($rootScope, $scope, logger, $detour, $stateParams) {

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

                $scope.relationshipGridOptions = {
                    data: 'relationships',
                    columnDefs: relationshipColumnDefs
                };

                angular.extend($scope.relationshipGridOptions, $rootScope.defaultGridOptions);
                $scope.getAllRelationship = function() {

                    $.ajax({
                        type: 'Get',
                        url: 'api/relationship/Relationship/Get?EntityName=' + $stateParams.Id,
                        success: function (result) {
                            $scope.relationships = result;
                        },
                        error: function (result) {
                            logger.error('Get relationships failed:' + result.responseText);
                        }
                    });
                };

                $scope.createOneToMany = function () {
                    $detour.transitionTo('CreateOneToMany', { EntityName: $stateParams.Id });
                };
                $scope.createManyToMany = function () {
                    $detour.transitionTo('CreateManyToMany', { EntityName: $stateParams.Id });
                };
                $scope.edit = function (cotentId, type) {
                    $detour.transitionTo('EditRelationship', { Id: cotentId, Type: type });
                };
                $scope.delete = function (contentId) {
                    $.ajax({
                        type: 'Get',
                        url: 'api/relationship/Relationship/Delete?RelationshipId=' + contentId,
                        success: function () {
                            logger.success("Delete relationship success!");
                        },
                        error: function (result) {
                            logger.error('Delete relationship failed:' + result.responseText);
                        }
                    });
                };

                $scope.getAllRelationship();
            }]
    ]);
});