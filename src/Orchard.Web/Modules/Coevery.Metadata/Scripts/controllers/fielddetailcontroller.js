
function FieldDetailCtrl($http, $timeout, $scope, logger, $state, $stateParams) {
    $scope.save = function () {
        $.ajax({
            url: $scope.myForm.action,
            type: $scope.myForm.method,
            data: $($scope.myForm).serialize() + '&submit.Save=Save',
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