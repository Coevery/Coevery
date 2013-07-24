'use strict';

define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'RelationshipsCtrl',
        ['$rootScope', '$scope', 'logger', '$detour', '$stateParams',
            function ($rootScope, $scope, logger, $detour, $stateParams) {

                var cellTemplateString = '<div class="ngCellText" ng-class="col.colIndex()" title="{{COL_FIELD}}">' +
           '<span class="btn-link" ng-click="edit(row.entity.Name)">{{COL_FIELD}}</span>' +
           '<ul class="row-actions pull-right hide">' +
           '<li class="icon-edit" ng-click="edit(row.entity.Name)" title="Edit"></li>' +
           '<li class="icon-remove" ng-click="delete(row.entity.Name)" title="Delete"></li>' +
           '</ul>' +
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

                $scope.relationships = [
                    { Name: 'Leads_Accounts', PrimaryEntity: 'Lead', RelatedEntity: 'Account', Type: 'One to Many' },
                    { Name: 'Leads_Opportunities', PrimaryEntity: 'Lead', RelatedEntity: 'Opportunity', Type: 'One to Many' },
                    { Name: 'Leads_Users', PrimaryEntity: 'Lead', RelatedEntity: 'User', Type: 'Many to Many' }
                ];

                $scope.editOneToMany = function () {
                    $detour.transitionTo('EditOneToMany', { EntityName: $stateParams.Id });
                };
                $scope.editManyToMany = function () {
                    $detour.transitionTo('EditManyToMany', { EntityName: $stateParams.Id });
                };
            }]
    ]);
});