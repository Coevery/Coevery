'use strict';
define(['core/app/detourService', 'app'], function (detour, app) {
    detour.registerController([
        'FormDesignerCtrl',
        ['$scope', 'logger', '$detour', '$stateParams',
            function ($scope, logger, $detour, $stateParams) {
                $scope.exit = function() {
                    $detour.transitionTo('EntityDetail.Relationships', { Id: $stateParams.EntityName });
                };

            }]
    ]);
});

setTimeout(function () {
    $('#test').affix({
        offset: {
            top: function () {
                //var height = $(window).height() - 71 - 90;
                //return $('#form-designer').height() > height ? 71 : 1000;
                return $('#form-designer').height() > $('#test').height() ? 71 : 1000;
            },
        }
    });
}, 100);