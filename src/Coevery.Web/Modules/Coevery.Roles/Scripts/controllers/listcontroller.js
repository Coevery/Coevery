'use strict';
define(['core/app/detourService', 'Modules/Coevery.Roles/Scripts/services/roledataservice'], function(detour) {
    detour.registerController([
        'RoleListCtrl',
        ['$rootScope', '$scope', 'logger', '$state', '$stateParams', 'roleDataService',
            function($rootScope, $scope, logger, $state, $stateParams, roleDataService) {
                var t = function(str) {
                    var result = i18n.t(str);
                    return result;
                };

                var columnDefs = [
                    { name: 'Id', label: t('Id'), hidden: true },
                    {
                        name: 'Name',
                        label: t('Name'),
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

                $scope.delete = function() {
                    var id = $scope.selectedItems.length > 0 ? $scope.selectedItems[0] : null;
                    if (!id) return;
                    roleDataService.delete({ id: id }, function() {
                        $scope.getAllRoles();
                        logger.success('Delete the role successful.');
                    }, function(result) {
                        logger.error('Failed to delete the role:' + result.data);
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