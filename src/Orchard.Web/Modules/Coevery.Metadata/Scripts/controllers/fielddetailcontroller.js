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
                $scope.NameDisabled = true;
                $scope.FieldTypeNameDisabled = true;
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
        $scope.NameDisabled = true;
        $scope.FieldTypeNameDisabled = true;
        var editfielddata = field.get({ name: name, parentname: parentname }, function () {
            $scope.item = editfielddata;
            var isExsits = false;
            for (var i = 0; i < editfielddata.FieldTypes.length; i++) {
                if (editfielddata.FieldTypeName.Name == editfielddata.FieldTypes[i].Name) {
                    $scope.item.FieldTypeName = editfielddata.FieldTypes[i];
                    isExsits = true;
                    break;
                }
            }
            if (!isExsits) $scope.item.FieldTypeName = null;
        }, function () {
            logger.error("The metadata does not exist.");
        });
    } else {
        $scope.NameDisabled = false;
        $scope.FieldTypeNameDisabled = false;
        var addfielddata = field.get({parentname: parentname }, function () {
            $scope.item = addfielddata;
            if (addfielddata.FieldTypes.length > 0) {
                $scope.item.FieldTypeName = addfielddata.FieldTypes[0];
            }
        }, function () {
            $scope.item = new field();
            $scope.item.ParentName = parentname;
        });
       
        
    }
    
});