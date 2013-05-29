OneToManyDetailCtrl.$inject = ['$rootScope', '$scope', 'logger', '$state', 'localize', '$resource', '$stateParams'];

function OneToManyDetailCtrl($rootScope, $scope, logger, $state, localize, $resource, $stateParams) {
    $scope.recordDeleteBehavior = 1;
    $scope.showRelatedList = true;
    $scope.$watch('required', function(newValue) {
        if (newValue && $scope.recordDeleteBehavior == 1) {
            $scope.recordDeleteBehavior = 2;
        }
    });
    $scope.exit = function () {
        $state.transitionTo('Detail', { Module: 'Metadata', Id: $stateParams.Id });
    };
}

//@ sourceURL=Coevery.Metadata/onetomanydetailcontroller.js