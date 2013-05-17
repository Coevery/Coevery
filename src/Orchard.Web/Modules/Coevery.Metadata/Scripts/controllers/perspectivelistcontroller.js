PerspectiveListCtrl.$inject = ['$rootScope', '$scope', 'logger', '$state', 'localize', '$resource', '$stateParams'];

function PerspectiveListCtrl($rootScope, $scope, logger, $state, localize, $resource, $stateParams) {
    $scope.addPerspective = function () {
        $state.transitionTo('SubList', { Module: 'Metadata', SubModule: 'Perspective', Id: $stateParams.Id, View: 'EditPerspective' });
    };
    $scope.addNavigationItem = function() {
        $state.transitionTo('SubList', { Module: 'Metadata', SubModule: 'Perspective', Id: $stateParams.Id, View: 'EditNavigationItem' });
    };
}

//@ sourceURL=Coevery.Metadata/perspectivelistcontroller.js