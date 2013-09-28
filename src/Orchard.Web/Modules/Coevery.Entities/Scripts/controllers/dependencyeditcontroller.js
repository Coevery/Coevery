'use strict';
define(['core/app/detourService', 'Modules/Coevery.Fields/Scripts/services/fielddependencydataservice', 'Modules/Coevery.Fields/Scripts/services/optionitemsdataservice'], function (detour) {
    detour.registerController([
        'FieldDependencyEditCtrl',
        ['$scope', 'logger', '$state', '$stateParams', '$resource', 'fieldDependencyDataService', 'optionItemsDataService',
            function ($scope, logger, $state, $stateParams, $resource, fieldDependencyDataService, optionItemsDataService) {
                var entityName = $stateParams.EntityName;

                var dependentFieldItems = optionItemsDataService.query({
                    EntityName: entityName,
                    FieldName: $scope.dependentField
                }, function () {
                    $scope.dependentFieldItems = dependentFieldItems;
                }, function () {
                    logger.error("Get items failed.");
                });

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

                $scope.exit = function () {
                    $state.transitionTo('FieldDependencyList', { EntityName: entityName });
                };
                
                $scope.save = function () {
                    var value = "[";
                    var outerIndex = 0;
                    $.each($scope.controlFieldItems, function () {
                        if (outerIndex != 0) {
                            value += ",";
                        }
                        outerIndex++;
                        value += "{ControlFieldValue:'" + this.Id + "',DependentFieldValue:[";
                        var innerIndex = 0;
                        $.each($('input:checked[name=' + this.Value + ']'), function () {
                            if (innerIndex != 0) {
                                value += ",";
                            }
                            innerIndex++;
                            value += "'"+ this.value +"'";
                        });
                        value += "]}";
                    });
                    value += "]";

                    $.ajax({
                        type: 'POST',
                        contentType: 'application/json',
                        url: 'api/fields/FieldDependency?EntityName=' + entityName + 
                            '&ControlFieldName=' + $scope.controlField + 
                            '&DependentFieldName=' + $scope.dependentField,
                        data: value,
                        success: function () {
                            logger.success('Save success');
                        },
                        error: function (result) {
                            logger.error('Save failed:' + result.responseText);
                        }
                    });              
                };
            }]
    ]);
});