NavigationItemDetailCtrl.$inject = ['$rootScope', '$scope', 'logger', '$state', 'localize', '$resource', '$stateParams'];

function NavigationItemDetailCtrl($rootScope, $scope, logger, $state, localize, $resource, $stateParams) {
    $scope.exit = function () {
        $state.transitionTo('SubList', { Module: 'Metadata', SubModule: 'Perspective', Id: $stateParams.Id, View: 'EditPerspective' });
    };

    $scope.save = function () {
        $.ajax({
            url: myForm.action,
            type: myForm.method,
            data: $(myForm).serialize() + '&submit.Save=Save',
            success: function (result) {
                logger.success("Perspective Saved.");
            }
        });
    };
}

//@ sourceURL=Coevery.Metadata/navigationitemdetailcontroller.js