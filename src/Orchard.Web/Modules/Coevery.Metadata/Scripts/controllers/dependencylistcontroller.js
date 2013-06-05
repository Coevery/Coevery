function DependencyListCtrl($scope, logger, localize, $state, $stateParams, $resource) {
    $('.step1').hide();
    $('.step2').hide();

    var fieldColumnDefs = [
        { field: 'ContentId', displayName: 'Actions', width: 100, cellTemplate: '<div class="ngCellText" ng-class="col.colIndex()"><a>Edit</a>&nbsp;<a>Delete</a></div>' },
        { field: 'ControlField', displayName: 'Control Field' },
        { field: 'DependentField', displayName: 'Dependent Field' },
        { field: 'ModifiedBy', displayName: 'Modified By' }
    ];

    $scope.gridOptions = {
        data: 'myData',
        selectedItems: $scope.mySelections,
        multiSelect: false,
        enableColumnReordering: true,
        columnDefs: fieldColumnDefs
    };

    $scope.myData = [
        { ControlField: 'IsClosed', DependentField: 'Status', ModifiedBy: 'Zhang Junnan, 2013-5-15' }
    ];
    $scope.controlFields = [
        { name: 'IsClosed' },
        { name: 'Rating' },
        { name: 'Industry' }
    ];
    $scope.dependentFields = [
        { name: 'Sport' },
        { name: 'Country' }
    ];

    $scope.add = function() {
        $('.step0').hide();
        $('.step1').show();
    };
    $scope.back = function() {
        $state.transitionTo('Detail', { Module: 'Metadata', Id: $stateParams.Id });
    };
    $scope.next = function() {
        $('.step1').hide();
        $('.step2').show();
    };
    $scope.prev = function() {
        $('.step1').show();
        $('.step2').hide();
    };
    $scope.exit = function() {
        $('.step0').show();
        $('.step1').hide();
    };
}

//@ sourceURL=Coevery.Metadata/dependencylistcontroller.js