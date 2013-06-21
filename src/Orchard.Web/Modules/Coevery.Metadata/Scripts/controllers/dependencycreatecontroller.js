function DependencyCreateCtrl($scope, logger, localize, $state, $stateParams, $resource) {
    var entityName = $stateParams.Id;
    var FieldDependency = $resource(
        'api/metadata/FieldDependency',
        {},
        { update: { method: 'PUT' } }
    );
    var OptionItems = $resource(
        'api/metadata/OptionItems',
        {},
        { update: { method: 'PUT' } }
    );

    $('.step2').hide();
    $scope.next = function () {
        if ($('option[value=' + $scope.controlField + ']').attr('field_type') == 'BooleanField') {
            $scope.controlFieldItems = [{ Value: 'True', Id: 'True' }, { Value: 'False', Id: 'False' }];
        } else {
            var controlFieldItems = OptionItems.query({
                EntityName: entityName,
                FieldName: $scope.controlField
            }, function () {
                $scope.controlFieldItems = controlFieldItems;
            }, function () {
                logger.error("Get items failed.");
            });
        }

        var dependentFieldItems = OptionItems.query({
            EntityName: entityName,
            FieldName: $scope.dependentField
        }, function () {
            $scope.dependentFieldItems = dependentFieldItems;
        }, function () {
            logger.error("Get items failed.");
        });
        $('.step1').hide();
        $('.step2').show();
    };
    $scope.prev = function () {
        $('.step1').show();
        $('.step2').hide();
    };
    $scope.exit = function () {
        $state.transitionTo('SubList', { Module: 'Metadata', Id: $stateParams.Id, SubModule: 'Field', View: 'DependencyList' });
    };
    $scope.save = function () {
        var test = new FieldDependency();
        var dependencies = [];
        $.each($scope.controlFieldItems, function () {
            var dependentFieldValue = '';
            $.each($('input:checked[name=' + this.Value + ']'), function () {
                dependentFieldValue += $(this).attr('value') + ';';
            });
            dependencies.push({
                ControlFieldValue: this.Id,
                DependentFieldValue: dependentFieldValue
            });
        });
        test.Dependencies = dependencies;
        test.$save({
            EntityName: entityName,
            ControlFieldName: $scope.controlField,
            DependentFieldName: $scope.dependentField
        });
        //FieldDependency.save({
        //    Entity: entityName,
        //    ControlField: $scope.controlField,
        //    DependentField: $scope.dependentField
        //}, dependencies);
    };
}

//@ sourceURL=Coevery.Metadata/dependencylistcontroller.js