define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'SubViewCtrl',
        ['$rootScope', '$scope', 'logger', '$state', '$stateParams',
            function ($rootScope, $scope, logger, $state, $stateParams) {
                var moduleName = $stateParams.Module;

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
            }]
    ]);
});

