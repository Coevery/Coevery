
function FieldDetailCtrl($http, $timeout, $scope, logger, $state, $stateParams, $element) {
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
}

//@ sourceURL=Coevery.Metadata/fielddetailcontroller.js