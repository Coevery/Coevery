'use strict';

define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'RelationshipsCtrl',
        ['$rootScope', '$scope', 'logger', '$detour', '$resource', '$stateParams',
            function ($rootScope, $scope, logger, $detour, $resource, $stateParams) {
                var relationshipDataService = $resource('api/relationship/Relationship');

                var relationshipColumnDefs = [
                    { name: 'ContentId', label: 'Content Id', hidden: true },
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
                    colModel: relationshipColumnDefs
                };

                angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);
                
                $scope.getAllRelationship = function () {
                    $("#relationList").jqGrid('setGridParam', {
                        datatype: "json"
                    }).trigger('reloadGrid');
                };

                $scope.createOneToMany = function () {
                    $detour.transitionTo('CreateOneToMany', { EntityName: $stateParams.Id });
                };
                $scope.createManyToMany = function () {
                    $detour.transitionTo('CreateManyToMany', { EntityName: $stateParams.Id });
                };
                $scope.edit = function (paramString) {
                    var params = JSON.parse(paramString);
                    if (params.Type == "OneToMany") {
                        $detour.transitionTo('EditOneToMany', { EntityName: $stateParams.Id, RelationId: params.ContentId });
                    } else if (params.Type == "ManyToMany") {
                        $detour.transitionTo('EditManyToMany', { EntityName: $stateParams.Id, RelationId: params.ContentId });
                    }
                };
                $scope.delete = function (contentId) {
                    $scope.relationshipId = contentId;
                    $('#myModalRelationship').modal({
                        backdrop: 'static',
                        keyboard: true
                    });
                };

                $scope.deleteRelationship = function () {
                    $('#myModalRelationship').modal('hide');
                    relationshipDataService.delete({ RelationshipId: $scope.relationshipId }, function () {
                        $scope.getAllRelationship();
                        logger.success("Delete the relationship successful.");
                    }, function (reason) {
                        logger.error("Failed to delete the relationship:" + reason);
                    });
                };
            }]
    ]);
});