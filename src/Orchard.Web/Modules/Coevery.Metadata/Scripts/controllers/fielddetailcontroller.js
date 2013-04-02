function FieldDetailCtrl($scope, logger, $state, $stateParams, $resource) {
    var parentname = $stateParams.Id;
    var name = $stateParams.SubId;
    var isNew = name? false : true;
    var field = FieldContext($resource);
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
        $state.transitionTo('SubList', { Module: 'Metadata', Id: $stateParams.Id, SubModule: 'Field', View: 'List' });
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
    
}