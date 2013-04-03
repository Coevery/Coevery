CommonDetailCtrl.$inject = ['$rootScope','$scope', 'logger', '$state', '$stateParams', '$resource'];

function CommonDetailCtrl($rootScope,$scope, logger, $state, $stateParams, $resource) {
    var moduleName = $rootScope.$stateParams.Module;
    var module = CommonContext($rootScope,$resource);
    var id = $stateParams.Id;
    var isNew = id ? false : true;

    $scope.save = function () {
        debugger;
        $.ajax({
            url: myForm.action,
            type: myForm.method,
            data: $(myForm).serialize(),
            success: function (result) {
                debugger;
                $state.transitionTo('List', { Module: moduleName });
            }
        });
    };

    $scope.change = function() {

    };

    $scope.exit = function() {
        $state.transitionTo('List', { Module: moduleName });
    };

    if (!isNew) {
        var lead = module.get({ leadId: id }, function() {
            $scope.item = lead;
        }, function() {
            logger.error("The lead does not exist.");
        });
    } else {
        $scope.item = new module();
    }
}