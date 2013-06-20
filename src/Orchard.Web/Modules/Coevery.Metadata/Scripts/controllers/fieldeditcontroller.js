function FieldEditCtrl($http, $timeout, $scope, logger, $state, $stateParams, $element) {
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

    $scope.viewItems = function () {
        $state.transitionTo('SubDetail', { Module: 'Metadata', Id: $stateParams.Id, SubModule: 'Field', View: 'Items', SubId: $stateParams.SubId });
    };
}

//@ sourceURL=Coevery.Metadata/fieldeditcontroller.js