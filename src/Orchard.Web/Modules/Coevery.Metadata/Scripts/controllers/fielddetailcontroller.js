
function FieldDetailCtrl($http, $timeout, $scope, logger, $state, $stateParams) {
    $scope.save = function () {
        $.ajax({
            url: myForm.action,
            type: myForm.method,
            data: $(myForm).serialize() + '&submit.Save=Save',
            success: function (result) {
                $timeout($scope.exit, 0);
            }
        });
    };

    $scope.exit = function() {
        $state.transitionTo('Detail', { Module: 'Metadata', Id: $stateParams.Id });
    };
}

//@ sourceURL=Coevery.Metadata/fielddetailcontroller.js