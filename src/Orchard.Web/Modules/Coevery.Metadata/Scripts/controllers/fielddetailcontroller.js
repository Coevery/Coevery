function FieldDetailCtrl($http, $timeout, $scope, logger, $state, $stateParams, $element) {
    $scope.fieldType = 'Input';
   
    $scope.save = function () {
        $.ajax({
            url: $element.attr('action'),
            type: $element.attr('method'),
            data: $element.serialize() + '&submit.Save=Save',
            success: function (result) {
                $timeout($scope.exit, 0);
            }
        });
    };

    $scope.exit = function() {
        $state.transitionTo('Detail', { Module: 'Metadata', Id: $stateParams.Id });
    };

    $scope.next = function() {
        $state.transitionTo('SubDetail', { Module: 'Metadata', Id: $stateParams.Id, SubModule: 'Field', View: 'EditFieldInfo', SubId: $scope.fieldType });
    };
}

//@ sourceURL=Coevery.Metadata/fielddetailcontroller.js