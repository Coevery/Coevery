'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'EditManyToManyCtrl',
        ['$scope', 'logger', '$detour', '$stateParams',
            function ($scope, logger, $detour, $stateParams) {
                $scope.showPrimaryList = true;
                $scope.showRelatedList = true;
                
                $scope.save = function () {
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
                };

                $scope.exit = function () {
                    $detour.transitionTo('EntityDetail.Relationships', { Id: $stateParams.EntityName });
                };
            }]
    ]);
});