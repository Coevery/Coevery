PerspectiveCreateCtrl.$inject = ['$rootScope', '$scope', 'logger', '$state', 'localize', '$resource', '$stateParams'];

function PerspectiveCreateCtrl($rootScope, $scope, logger, $state, localize, $resource, $stateParams) {
    
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
    
    $scope.exit = function () {
        $state.transitionTo('SubList', { Module: 'Metadata', SubModule: 'Perspective', Id: $stateParams.Id, View: 'List' });
    };
}

//@ sourceURL=Coevery.Metadata/perspectivecreatecontroller.js