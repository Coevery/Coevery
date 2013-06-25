'use strict';

define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'RelationshipsCtrl',
        ['$scope', 'logger', '$detour', '$stateParams',
            function ($scope, logger, $detour, $stateParams) {

                var relationshipColumnDefs = [
                    { field: 'Name', displayName: 'Actions', width: 100, cellTemplate: '<div class="ngCellText" ng-class="col.colIndex()"><a>Edit</a>&nbsp;<a>Delete</a></div>' },
                    { field: 'Name', displayName: 'Relationship Name' },
                    { field: 'PrimaryEntity', displayName: 'Primary Entity' },
                    { field: 'RelatedEntity', displayName: 'Related Entity' },
                    { field: 'Type', displayName: 'Type' }
                ];
                $scope.relationshipGrid = {
                    data: 'relationships',
                    multiSelect: false,
                    enableColumnReordering: true,
                    columnDefs: relationshipColumnDefs
                };
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