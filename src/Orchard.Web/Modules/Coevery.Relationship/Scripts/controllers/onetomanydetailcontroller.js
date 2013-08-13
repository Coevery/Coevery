'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'CreateOneToManyCtrl',
        ['$scope', 'logger', '$detour', '$stateParams',
            function ($scope, logger, $detour, $stateParams) {

                $scope.showRelatedList = true;
                $scope.$watch('required', function (newValue) {
                    if (newValue && $scope.recordDeleteBehavior == 1) {
                        $scope.recordDeleteBehavior = 2;
                    }
                });
                $scope.save = function () {
                    $("input.primary-entity").prop('disabled', false);
                    var form = $('#onetomany-form');
                    $.ajax({
                        url: form.attr('action'),
                        type: form.attr('method'),
                        data: form.serializeArray(),
                        success: function () {
                            logger.success('success');
                            $("input.primary-entity").prop('disabled', true);
                        },
                        error: function (result) {
                            logger.error('Failed:\n' + result.responseText);
                            $("input.primary-entity").prop('disabled', true);
                        }
                    });
                };

                $scope.exit = function () {
                    $detour.transitionTo('EntityDetail.Relationships', { Id: $stateParams.EntityName });
                };
            }]
    ]);
});