'use strict';
define(['core/app/detourService', 'Modules/Coevery.Roles/Scripts/services/roledataservice'], function(detour) {
    detour.registerController([
        'RoleListCtrl',
        ['$rootScope', '$scope', 'logger', '$state', '$stateParams', 'roleDataService','$i18next',
            function ($rootScope, $scope, logger, $state, $stateParams, roleDataService, $i18next) {
                var columnDefs = [
                    { name: 'Id', label: $i18next('Id'), hidden: true, key: true },
                    {
                        name: 'Name',
                        label: $i18next('Name'),
                        formatter: $rootScope.cellLinkTemplate,
                        formatoptions: { hasView: true }
                    }
                ];

                $scope.gridOptions = {
                    url: "api/roles/Role",
                    colModel: columnDefs
                };

                angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);

                $scope.addRole = function() {
                    $state.transitionTo('RoleCreate');
                };

                $scope.edit = function(id) {
                    $state.transitionTo('RoleEdit', { Id: id });
                };

                $scope.view = function(id) {
                    $scope.edit(id);
                };

                $scope['delete'] = function () {
                    var id = $scope.selectedItems.length > 0 ? $scope.selectedItems[0] : null;
                    if (!id) return;
                    roleDataService.delete({ id: id }, function() {
                        $scope.getAllRoles();
                        logger.success($i18next('Delete the role successful.'));
                    }, function(result) {
                        logger.error($i18next('Failed to delete the role:') + result.data);
                    });
                };

                $scope.getAllRoles = function() {
                    $("#roleList").jqGrid('setGridParam', {
                        datatype: "json"
                    }).trigger('reloadGrid');
                };
            }]
    ]);
});