'use strict';
define(['core/app/detourService', 'Modules/Coevery.Fields/Scripts/services/fielddependencydataservice', 'Modules/Coevery.Fields/Scripts/services/optionitemsdataservice'], function (detour) {
    detour.registerController([
        'FieldDependencyCreateCtrl',
        ['$scope', 'logger', '$detour', '$stateParams', '$resource', 'fieldDependencyDataService', 'optionItemsDataService',
            function ($scope, logger, $detour, $stateParams, $resource, fieldDependencyDataService, optionItemsDataService) {
                var entityName = $stateParams.EntityName;

                $('.step2').hide();
                $scope.next = function () {
                    if ($('option[value=' + $scope.controlField + ']').attr('field_type') == 'BooleanField') {
                        $scope.controlFieldItems = [{ Value: 'True', Id: 'True' }, { Value: 'False', Id: 'False' }];
                    } else {
                        var controlFieldItems = optionItemsDataService.query({
                            EntityName: entityName,
                            FieldName: $scope.controlField
                        }, function () {
                            $scope.controlFieldItems = controlFieldItems;
                        }, function () {
                            logger.error("Get items failed.");
                        });
                    }

                    var dependentFieldItems = optionItemsDataService.query({
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
                    $detour.transitionTo('FieldDependencyList', { EntityName: entityName });
                };
                $scope.save = function () {
                    var test = new fieldDependencyDataService();
                    var value = '';
                    $.each($scope.controlFieldItems, function () {
                        var dependentFieldValue = '';
                        $.each($('input:checked[name=' + this.Value + ']'), function () {
                            var prefix = dependentFieldValue ? '&' : '';
                            dependentFieldValue += prefix + $(this).attr('value');
                        });
                        value += this.Id + '=' + dependentFieldValue + ';';
                    });
                    test.Value = value;
                    test.$save({
                        EntityName: entityName,
                        ControlFieldName: $scope.controlField,
                        DependentFieldName: $scope.dependentField
                    }, function () {
                        logger.success('Save success.');
                    });
                };
            }]
    ]);
});