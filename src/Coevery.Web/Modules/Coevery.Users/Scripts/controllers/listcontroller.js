'use strict';
define(['core/app/detourService', 'Modules/Coevery.Users/Scripts/services/userdataservice'], function(detour) {
    detour.registerController([
        'UserListCtrl',
        ['$rootScope', '$scope', 'logger', '$state', '$stateParams', 'userDataService', '$http',
            function($rootScope, $scope, logger, $state, $stateParams, userDataService, $http) {
                var t = function(str) {
                    var result = i18n.t(str);
                    return result;
                };

                var columnDefs = [
                    { name: 'Id', label: t('Id'), hidden: true },
                    {
                        name: 'UserName',
                        label: t('User Name'),
                        formatter: $rootScope.cellLinkTemplate,
                        formatoptions: { hasView: true }
                    },
                    { name: 'Email', label: t('Email') },
                    { name: 'RegistrationStatus', label: t('Registration Status') }
                ];

                $scope.gridOptions = {
                    url: "api/users/User",
                    colModel: columnDefs
                };

                angular.extend($scope.gridOptions, $rootScope.defaultGridOptions);

                $scope.addUser = function() {
                    $state.transitionTo('UserCreate');
                };

                $scope.edit = function(id) {
                    $state.transitionTo('UserEdit', { Id: id });
                };

                $scope.view = function(id) {
                    $scope.edit(id);
                };

                $scope.approve = function(id) {
                    $http({
                        url: 'SystemAdmin/Users/Approve/' + id,
                        tracker: 'approveUser'
                    }).then(function(response) {
                        $scope.getAllUsers();
                        logger.success('Approve succeeded');
                        return response;
                    }, function(reason) {
                        logger.error('Approve Failed： ' + reason.data);
                    });
                };

                $scope.disable = function(id) {
                    $http({
                        url: 'SystemAdmin/Users/Moderate/' + id,
                        tracker: 'disableUser'
                    }).then(function(response) {
                        $scope.getAllUsers();
                        logger.success('Disable succeeded');
                        return response;
                    }, function(reason) {
                        logger.error('Disable Failed： ' + reason.data);
                    });
                };

                $scope.delete = function() {
                    var id = $scope.selectedItems.length > 0 ? $scope.selectedItems[0] : null;
                    if (!id) return;
                    userDataService.delete({ id: id }, function() {
                        $scope.getAllUsers();
                        logger.success('Delete the user successful.');
                    }, function(result) {
                        logger.error('Failed to delete the user:' + result.data);
                    });
                };

                $scope.getAllUsers = function() {
                    $("#userList").jqGrid('setGridParam', {
                        datatype: "json"
                    }).trigger('reloadGrid');
                };
            }]
    ]);
});