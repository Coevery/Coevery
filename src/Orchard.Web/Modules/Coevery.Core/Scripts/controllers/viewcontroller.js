define(['core/app/detourService', 'core/services/historyservice'], function (detour) {
    detour.registerController([
        'GeneralViewCtrl',
        ['$timeout', '$rootScope', '$scope', 'logger', '$state', '$stateParams', 'historyService',
            function ($timeout, $rootScope, $scope, logger, $state, $stateParams, historyService) {
                var navigationId = $stateParams.NavigationId;
                var moduleName = $stateParams.Module;
                var id = $stateParams.Id;
                $scope.moduleName = moduleName;
                
                // histories
                $scope.historiesOptions = {
                    data: 'histories',
                    multiSelect: true,
                    enableRowSelection: true,
                    showSelectionCheckbox: true,
                    columnDefs: [
                        { field: 'Date', displayName: 'Date' },
                        { field: 'User', displayName: 'User' },
                        { field: 'Action', displayName: 'Action' }
                    ]
                };
                angular.extend($scope.historiesOptions, $rootScope.defaultGridOptions);
                var histories = historyService.query({ contentId: id}, function () {
                    $scope.histories = histories;
                }, function () {
                    logger.error("Failed to fetched records for " + moduleName);
                });

                $scope.generateChildController = function(childName) {
                    this[childName] = function(items) {

                    };
                };

                $scope.exit = function () {
                    //if (window.history.length > 1)
                    //    window.history.back();
                    //else
                    $state.transitionTo('List', { NavigationId: navigationId, Module: moduleName });
                };
                $scope.edit = function () {
                    $state.transitionTo('Detail', { NavigationId: navigationId, Module: moduleName, Id: id });
                };
            }]
    ]);
});

