function ItemsCtrl($http, $timeout, $scope, logger, $state, $stateParams, $resource) {
    var entityName = $stateParams.Id;
    var fieldName = $stateParams.SubId;
    var OptionItems = $resource(
         'api/metadata/OptionItems',
         {},
         { update: { method: 'PUT' } });

    var optionColumnDefs = [
        { field: 'ContentId', displayName: 'Actions', width: 100, cellTemplate: '<div class="ngCellText" ng-class="col.colIndex()"><a>Delete</a></div>' },
        { field: 'Value', displayName: 'Value' },
        { field: 'IsDefault', displayName: 'Is Default', cellTemplate: '<div class="ngCellText" ng-class="col.colIndex()"><span ng-cell-text><input type="checkbox" ng-checked="row.entity[col.field]"></span></div>' }
    ];

    $scope.gridOptions = {
        data: 'myData',
        multiSelect: false,
        columnDefs: optionColumnDefs
    };

    $scope.add = function () {

    };
    $scope.back = function () {
        $state.transitionTo('SubDetail', { Module: 'Metadata', Id: $stateParams.Id, SubModule: 'Field', View: 'Edit', SubId: $stateParams.SubId });
    };
    $scope.getAllField = function () {
        var metaData = OptionItems.query({ FieldName: fieldName, EntityName: entityName }, function () {
            $scope.myData = metaData;
        }, function () {
            logger.error("The metadata does not exist.");
        });
    };
    $scope.getAllField();
    //$scope.save = function () {
    //    var form = $element.find('#field-info-form');
    //    $.ajax({
    //        url: form.attr('action'),
    //        type: form.attr('method'),
    //        data: form.serialize() + '&submit.Save=Save',
    //        success: function (result) {
    //            logger.success('Success');
    //        }
    //    });
    //};
}

//@ sourceURL=Coevery.Metadata/itemscontroller.js