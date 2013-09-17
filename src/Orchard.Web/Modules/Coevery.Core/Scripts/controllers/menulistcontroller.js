define(['core/app/detourService', 'core/services/menudataservice'], function (detour) {
    detour.registerController([
        'MenuListCtrl',
        ['$scope', '$detour', '$stateParams', 'menuDataService', 'logger',
            function ($scope, $detour, $stateParams, menuDataService, logger) {
                var menuId = $stateParams.Navigation;
                if (menuId == undefined) menuId = 0;
                $scope.InitMenu = function () {
                    var menus = menuDataService.query({ menuId: menuId }, function () {
                        $scope.temp = menus;
                    }, function () {
                        logger.error("Failed to fetched filters for " + moduleName);
                    });
                }
                $scope.InitMenu();
            }]
    ]);
});