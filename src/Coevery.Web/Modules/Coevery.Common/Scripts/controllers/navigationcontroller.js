define(['core/app/detourService', 'core/services/navigationdataservice'], function(detour) {
    detour.registerController([
        'NavigationCtrl',
        ['$scope', '$stateParams', 'logger',
            function ($scope, $stateParams, logger) {
                $('#navigation').perfectScrollbar();
                var navigationId = $stateParams.NavigationId;

                $scope.setcurrmenu = function (id, text) {
                    $scope.currentMenuId = id;
                    $scope.currentMenuText = text;
                };        
            }]
    ]);
});