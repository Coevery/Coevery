'use strict';

define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'RelationshipsCtrl',
        ['$rootScope', '$scope', 'logger', '$detour', '$stateParams',
            function ($rootScope, $scope, logger, $detour, $stateParams) {

                var cellTemplateString = '<div class="ngCellText" ng-class="col.colIndex()" title="{{COL_FIELD}}">' +
                    '<ul class="row-actions pull-right hide">' +
                    '<li class="icon-edit" ng-click="edit(row.entity.Name)" title="Edit"></li>' +
                    '<li class="icon-remove" ng-click="delete(row.entity.Name)" title="Delete"></li>' +
                    '</ul>' +
                    '<span class="btn-link" ng-click="edit(row.entity.Name)">{{COL_FIELD}}</span>' +
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

                $scope.relationships = [];

                $scope.getAllRelationship = function() {

                    $.ajax({
                        type: 'Get',
                        url: 'api/relationship/Relationship/Get?EntityName=' + $stateParams.Id,
                        success: function (result) {
                            alert(result);
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
                $scope.edit = function () {

                };
                $scope.delete = function () {

                };

                $scope.getAllRelationship();
            }]
    ]);
});