'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'EditManyToManyCtrl',
        ['$scope', 'logger', '$detour', '$stateParams',
            function ($scope, logger, $detour, $stateParams) {
                
                $scope.save = function () {
                    ToggleReadonly(false);
                    var form = $('#manytomany-form');
                    $.ajax({
                        url: form.attr('action'),
                        type: form.attr('method'),
                        data: form.serializeArray(),
                        success: function () {
                            logger.success('success');
                        },
                        error: function (result) {
                            logger.error('Failed:\n' + result.responseText);
                        }
                    });
                    ToggleReadonly(true);
                };

                $scope.exit = function () {
                    $detour.transitionTo('EntityDetail.Relationships', { Id: $stateParams.EntityName });
                };
            }]
    ]);
});

function ToggleReadonly(condition) {
    $("input.primary-entity").prop('disabled', condition);
    $("#relation-name").prop('disabled', condition);
    $("input.related-entity").prop('disabled', condition);
}