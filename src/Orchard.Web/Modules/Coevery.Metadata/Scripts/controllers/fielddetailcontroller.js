function FieldDetailCtrl($scope, logger, $state, $stateParams, $resource) {
    $scope.save = function () {
        $.ajax({
            url: myForm.action,
            type: myForm.method,
            data: $(myForm).serialize() + '&submit.Save=Save',
            success: function (result) {
                $state.transitionTo('Detail', { Module: 'Metadata', Id: $stateParams.Id });
            }
        });
    };

    $scope.exit = function() {
        $state.transitionTo('Detail', { Module: 'Metadata', Id: $stateParams.Id });
    };
}

//@ sourceURL=Coevery.Metadata/fielddetailcontroller.js