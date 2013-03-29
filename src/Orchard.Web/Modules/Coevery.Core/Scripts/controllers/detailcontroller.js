CommonDetailCtrl.$inject = ['$rootScope','$scope', 'logger', '$state', '$stateParams', '$resource'];

function CommonDetailCtrl($rootScope,$scope, logger, $state, $stateParams, $resource) {

    var module = CommonContext($rootScope,$resource);
    var id = $stateParams.Id;
    var isNew = id ? false : true;

    $scope.save = function () {
        if (isNew) {
            $scope.item.$save(function (u, putResponseHeaders) {
                isNew = false;
                logger.success("Create the " + contentTypeName + " successful.");
            }, function () {
                logger.error("Failed to create the " + contentTypeName + ".");
            });
        } else {
            $scope.item.$update(function (u, putResponseHeaders) {
                logger.success("Update the " + contentTypeName + " successful.");
            }, function () {
                logger.error("Failed to update the " + contentTypeName);
            });
        }
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