define(['core/app/couchPotatoService', 'core/services/commondataservice'], function (couchPotato) {
    couchPotato.registerController([
        'GeneralViewCtrl',
        ['$timeout', '$rootScope', '$scope', 'logger', '$state', '$stateParams', '$element', 'commonDataService',
            function ($timeout, $rootScope, $scope, logger, $state, $stateParams, $element, commonDataService) {
                var moduleName = $rootScope.$stateParams.Module;
                $scope.moduleName = moduleName;

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

                $scope.exit = function () {
                    $state.transitionTo('List', { Module: moduleName });
                };
            }]
    ]);
});