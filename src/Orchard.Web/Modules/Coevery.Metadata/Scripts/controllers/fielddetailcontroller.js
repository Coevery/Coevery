function FieldDetailCtrl($http, $timeout, $scope, logger, $state, $stateParams, $element) {
    $scope.exit = function () {
        $state.transitionTo('Detail', { Module: 'Metadata', Id: $stateParams.Id });
    };

    $scope.next = function () {
        if ($scope.fieldType) {
            $state.transitionTo('SubDetail', { Module: 'Metadata', Id: $stateParams.Id, SubModule: 'Field', View: 'EditFieldInfo', SubId: $scope.fieldType });
        }
    };

    $scope.fieldType = $('#field-type-form input:first').val();
}

//@ sourceURL=Coevery.Metadata/fielddetailcontroller.js