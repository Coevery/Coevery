
function FieldDetailCtrl($http, $scope, logger, $state, $stateParams) {
    $scope.save = function () {
        $http({
            url: myForm.action,
            method: myForm.method,
            data: $(myForm).serialize() + '&submit.Save=Save'
        }).success(function(data, status) {
            $state.transitionTo('Detail', { Module: 'Metadata', Id: $stateParams.Id });
        }).error(function () {
            
        });
        //$.ajax({
        //    url: myForm.action,
        //    type: myForm.method,
        //    data: $(myForm).serialize() + '&submit.Save=Save',
        //    success: function (result) {
        //        $state.transitionTo('Detail', { Module: 'Metadata', Id: $stateParams.Id });
        //    }
        //});
    };

    $scope.exit = function() {
        $state.transitionTo('Detail', { Module: 'Metadata', Id: $stateParams.Id });
    };
}

//@ sourceURL=Coevery.Metadata/fielddetailcontroller.js