function NavigationDetailCtrl($scope,$timeout, logger, $state, $stateParams, $resource) {
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
    
    $scope.exit = function () {
        $state.transitionTo('List', { Module: 'CoeveryNavigation'});
    };
    
}

//@ sourceURL=Coevery.CoeveryNavigation/navigationdetailcontroller.js