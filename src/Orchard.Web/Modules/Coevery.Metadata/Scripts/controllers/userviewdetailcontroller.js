function UserViewDetailCtrl($scope, logger, $state, $stateParams, $resource) {
    var name = $stateParams.Id;
    var metadata = UserViewContext($resource);
    var isNew = (name || name == '') ? false : true;


    $scope.save = function () {
        $.ajax({
            url: myForm.action,
            type: myForm.method,
            data: $(myForm).serialize() + '&submit.Save=Save',
            success: function (result) {
                $state.transitionTo('List', { Module: moduleName });
            }
        });
    };

    $scope.change = function () {

    };

    $scope.exit = function () {
        $state.transitionTo('UserViewList', { Module: 'Metadata' });
    };
   
}

//@ sourceURL=Coevery.Metadata/userviewDetailcontroller.js