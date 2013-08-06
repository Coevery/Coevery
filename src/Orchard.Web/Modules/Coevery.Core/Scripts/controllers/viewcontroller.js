define(['core/app/couchPotatoService', 'core/services/historyservice'], function (couchPotato) {
    couchPotato.registerController([
        'GeneralViewCtrl',
        ['$timeout', '$rootScope', '$scope', 'logger', '$state', '$stateParams', '$element', 'historyService',
            function ($timeout, $rootScope, $scope, logger, $state, $stateParams, $element, historyService) {
                var moduleName = $stateParams.Module;
                var id = $stateParams.Id;
                $scope.moduleName = moduleName;

                // activities
                $scope.openActivitiesOptions = {
                    data: 'activities',
                    multiSelect: true,
                    enableRowSelection: true,
                    showSelectionCheckbox: true,
                    columnDefs: [
                        { field: 'subject', displayName: 'Subject' },
                        { field: 'status', displayName: 'Status' }
                    ]
                };
                angular.extend($scope.openActivitiesOptions, $rootScope.defaultGridOptions);

                $scope.activities = [
                    { subject: 'Confirm', status: 'Open' },
                    { subject: 'Meeting', status: 'Open' },
                    { subject: 'Call', status: 'Open' }
                ];

                // note
                $scope.notesOptions = {
                    data: 'notes',
                    multiSelect: true,
                    enableRowSelection: true,
                    showSelectionCheckbox: true,
                    columnDefs: [
                        { field: 'title', displayName: 'Title' },
                        { field: 'body', displayName: 'Body' }
                    ]
                };
                angular.extend($scope.notesOptions, $rootScope.defaultGridOptions);


                $scope.notes = [
                    { title: 'Lead Way', body: 'Buy the website.' },
                    { title: 'Confirm', body: 'Make sure.' },
                    { title: 'Failed', body: 'Unfortunately!' }
                ];
                
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


                $scope.exit = function () {
                    $state.transitionTo('List', { Module: moduleName });
                };
                $scope.edit = function () {
                    $state.transitionTo('Detail', { Module: moduleName, Id: id });
                };
            }]
    ]);
});