'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'FieldEditCtrl',
        ['$scope', 'logger', '$detour', '$stateParams',
            function ($scope, logger, $detour, $stateParams) {
                var entityName = $stateParams.EntityName;
                var fieldName = $stateParams.FieldName;
                
                $scope.exit = function () {
                    $detour.transitionTo('EntityDetail.Fields', { Id: entityName });
                };

                $scope.save = function (isBack) {
                    var form = $('#field-info-form');
                    $.ajax({
                        url: form.attr('action'),
                        type: form.attr('method'),
                        data: form.serialize(),
                        success: function (result) {
                            //logger.success('Success');
                            if (isBack)
                                $scope.exit();
                        }
                    });
                };

                $scope.viewItems = function () {
                    $detour.transitionTo('FieldItems', { EntityName: entityName, FieldName: fieldName });
                };
            }]
    ]);
});