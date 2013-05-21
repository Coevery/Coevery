ManyToManyDetailCtrl.$inject = ['$rootScope', '$scope', 'logger', '$state', 'localize', '$resource', '$stateParams'];

function ManyToManyDetailCtrl($rootScope, $scope, logger, $state, localize, $resource, $stateParams) {
    $scope.showPrimaryList = true;
    $scope.showRelatedList = true;
    $scope.exit = function () {
        $state.transitionTo('Detail', { Module: 'Metadata', Id: $stateParams.Id });
    };
}

//@ sourceURL=Coevery.Metadata/manytomanydetailcontroller.js