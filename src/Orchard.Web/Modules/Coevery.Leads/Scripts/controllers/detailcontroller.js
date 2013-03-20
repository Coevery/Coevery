function LeadDetailCtrl($scope, logger, $state, $stateParams, $resource) {
    var Lead = LeadContext($resource);
    var id = $stateParams.Id;
    var isNew = id ? false : true;

    $scope.save = function() {
        if (isNew) {
            $scope.item.$save(function(u, putResponseHeaders) {
                isNew = false;
                logger.success("Create the lead successful.");
            }, function() {
                logger.error("Failed to create the lead.");
            });
        } else {
            $scope.item.$update(function(u, putResponseHeaders) {
                logger.success("Update the lead successful.");
            }, function() {
                logger.error("Failed to update the lead.");
            });
        }
    };

    $scope.change = function() {

    };

    $scope.exit = function() {
        $state.transitionTo('List', { Moudle: 'Leads' });
    };

    if (!isNew) {
        var lead = Lead.get({ leadId: id }, function() {
            $scope.item = lead;
        }, function() {
            logger.error("The lead does not exist.");
        });
    } else {
        $scope.item = new Lead();
    }
}