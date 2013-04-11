CommonDetailCtrl.$inject = ['$rootScope','$scope', 'logger', '$state'];

function CommonDetailCtrl($rootScope, $scope, $timeout, logger, $state) {

    var moduleName = $rootScope.$stateParams.Module;

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

    $scope.change = function() {

    };

    $scope.exit = function() {
        $state.transitionTo('List', { Module: moduleName });
    };
}

//@ sourceURL=Coevery.Core/detailcontroller.js