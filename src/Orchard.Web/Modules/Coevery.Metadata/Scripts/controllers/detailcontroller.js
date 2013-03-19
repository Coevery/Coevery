metadata.controller('MetadataDetailCtrl', function ($scope, logger, $location, $routeParams, metadata) {
    var name = $routeParams.name;
    var isNew = name? false : true;

    $scope.save = function () {
        if (isNew) {
            $scope.item.$save(function (u, putResponseHeaders) {
                isNew = false;
                logger.success("Create the lead successful.");
            }, function () {
                logger.error("Failed to create the lead.");
            });
        } else {
            $scope.item.$update(function (u, putResponseHeaders) {
                logger.success("Update the lead successful.");
            }, function () {
                logger.error("Failed to update the lead.");
            });
        }
    };

    $scope.change = function () {

    };

    $scope.exit = function () {
        $location.path('List');
    };

    if (!isNew) {
        var metaData = metadata.get({ name: name }, function () {
            $scope.item = metaData;
        }, function () {
            logger.error("The metadata does not exist.");
        });
    } else {
        $scope.item = new metadata();
    }
});