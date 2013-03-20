metadata.controller('MetadataDetailCtrl', function ($scope, logger, $location, $routeParams, metadata) {
    var name = $routeParams.name;
    var isNew = (name || name == '') ? false : true;
    
    
    $scope.save = function () {
        if (isNew) {
            $scope.item.$save(function (u, putResponseHeaders) {
                isNew = false;
                $scope.NameDisabled = true;
                logger.success("Create the metadata successful.");
            }, function () {
                logger.error("Failed to create the metadata.");
            });
        } else {
            $scope.item.$update(function (u, putResponseHeaders) {
                logger.success("Update the metadata successful.");
            }, function () {
                logger.error("Failed to update the metadata.");
            });
        }
    };

    $scope.change = function () {

    };

    $scope.exit = function () {
        $location.path('');
    };
    if (!isNew) {
        var metaData = metadata.get({ name: name }, function () {
            $scope.NameDisabled = true;
            $scope.item = metaData;
        }, function () {
            logger.error("The metadata does not exist.");
        });
    } else {
        $scope.NameDisabled = false;
        $scope.item = new metadata();
    }
});