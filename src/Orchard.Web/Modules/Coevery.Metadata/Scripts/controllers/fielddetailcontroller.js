metadata.controller('FieldDetailCtrl', function ($scope, logger, $location, $routeParams, field) {
    var name = $routeParams.name;
    var params = $scope.$eval(name);
    var parentname = params[0].parentname;
    name = params[0].name;
    var isNew = (name || name == '') ? false : true;
    
    $scope.save = function () {
        if (isNew) {
            $scope.item.$save(function (u, putResponseHeaders) {
                isNew = false;
                logger.success("Create the field successful.");
            }, function () {
                logger.error("Failed to create the field.");
            });
        } else {
            $scope.item.$update(function (u, putResponseHeaders) {
                logger.success("Update the field successful.");
            }, function () {
                logger.error("Failed to update the field.");
            });
        }
    };

    $scope.exit = function () {
        $location.path('FieldList/' + parentname);
    };
    if (!isNew) {
        var fielddata = field.get({ name: name, parentname: parentname }, function () {
            $scope.inputNameEable = true;
            $scope.item = fielddata;
        }, function () {
            logger.error("The metadata does not exist.");
        });
    } else {
        $scope.inputNameEable = false;
        $scope.item = new field();
        $scope.item.ParentName = parentname;
    }
    
});