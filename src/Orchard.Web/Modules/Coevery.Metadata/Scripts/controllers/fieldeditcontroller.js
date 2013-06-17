function FieldEditCtrl($http, $timeout, $scope, logger, $state, $stateParams, $element) {
    $('.itemsview').hide();

    var fieldColumnDefs = [
        { field: 'ContentId', displayName: 'Actions', width: 100, cellTemplate: '<div class="ngCellText" ng-class="col.colIndex()"><a>Delete</a></div>' },
        { field: 'Value', displayName: 'Value', enableCellEdit: true },
        { field: 'IsDefault', displayName: 'Is Default', cellTemplate: '<div class="ngCellText" ng-class="col.colIndex()"><span ng-cell-text><input type="checkbox" ng-checked="row.entity[col.field]"></span></div>' },
        { field: 'ModifiedBy', displayName: 'Modified By' }
    ];

    $scope.gridOptions = {
        data: 'myData',
        multiSelect: false,
        columnDefs: fieldColumnDefs
    };

    $scope.myData = [
        { Value: 'China', ModifiedBy: 'John 2013/5/14', IsDefault: true },
        { Value: 'America', ModifiedBy: 'John 2013/5/14' }
    ];

    $scope.exit = function () {
        $state.transitionTo('Detail', { Module: 'Metadata', Id: $stateParams.Id });
    };

    $scope.save = function () {
        var form = $element.find('#field-info-form');
        $.ajax({
            url: form.attr('action'),
            type: form.attr('method'),
            data: form.serialize() + '&submit.Save=Save',
            success: function (result) {
                logger.success('Success');
            }
        });
    };

    $scope.add = function () {

    };

    $scope.change = function () {

    };

    $scope.viewItems = function () {
        $('.editview').hide();
        $('.itemsview').show();
    };

    $scope.back = function () {
        $('.editview').show();
        $('.itemsview').hide();
    };
}

//@ sourceURL=Coevery.Metadata/fieldeditcontroller.js