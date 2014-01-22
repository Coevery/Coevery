'use strict';

define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'RelationshipsCtrl',
        ['$rootScope', '$scope', 'logger', '$state', '$q', '$resource', '$stateParams',
            function ($rootScope, $scope, logger, $state, $q, $resource, $stateParams) {
                var relationshipDataService = $resource(applicationBaseUrl + 'api/relationship/Relationship');
                var defer = $q.defer();
                $scope.$watch("selectedRow", function(newValue) {
                    if (newValue) {
                        defer.resolve();
                    }
                });

                var relationshipColumnDefs = [
                    { name: 'ContentId', label: 'Content Id', hidden: true, key: true },
                    {
                        name: 'Name', label: 'Relationship Name',
                        formatter: $rootScope.cellLinkTemplate,
                        formatoptions: { editRow: true }
                    },
                    { name: 'PrimaryEntity', label: 'Primary Entity' },
                    { name: 'RelatedEntity', label: 'Related Entity' },
                    { name: 'Type', label: 'Type' }
                ];

                $scope.gridOptions = {
                    url: "api/relationship/Relationship?entityName=" + $stateParams.Id,
                    rowIdName: 'ContentId',
                    colModel: relationshipColumnDefs
                };

                angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);
                
                $scope.getAllRelationship = function () {
                    $("#relationList").jqGrid('setGridParam', {
                        datatype: "json"
                    }).trigger('reloadGrid');
                };

                $scope.createOneToMany = function () {
                    $state.transitionTo('CreateOneToMany', { EntityName: $stateParams.Id });
                };
                $scope.createManyToMany = function () {
                    $state.transitionTo('CreateManyToMany', { EntityName: $stateParams.Id });
                };
                $scope.edit = function () {
                    defer.promise.then(function () {
                        var params = $scope.selectedRow;
                        if (!params) {
                            logger.error("No relation selected!");
                        }
                        if (params.Type === "OneToMany") {
                            $state.transitionTo('EditOneToMany', { EntityName: $stateParams.Id, RelationId: params.ContentId });
                        } else if (params.Type === "ManyToMany") {
                            $state.transitionTo('EditManyToMany', { EntityName: $stateParams.Id, RelationId: params.ContentId });
                        }
                    });
                };
                $scope['delete'] = function (contentId) {
                    var deleteRelationship = contentId || $scope.selectedItems.length > 0 ? $scope.selectedItems[0] : null;
                    if (!deleteRelationship) return;
                    
                    relationshipDataService['delete']({ RelationshipId: deleteRelationship }, function () {
                        $scope.getAllRelationship();
                        logger.success("Delete the relationship successful.");
                    }, function (reason) {
                        logger.error("Failed to delete the relationship:" + reason);
                    });
                };
                $scope.refreshTab();
            }]
    ]);
});