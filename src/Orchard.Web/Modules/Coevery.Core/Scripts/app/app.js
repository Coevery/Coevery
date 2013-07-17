define([], function () {
    'use strict';

    var coevery = angular.module('coevery', ['ng', 'ngGrid', 'ngResource', 'agt.couchPotato', 'ui.compat', 'ui.utils', 'coevery.layout', 'SharedServices']);
    coevery.config(['$stateProvider', '$routeProvider', '$urlRouterProvider', '$couchPotatoProvider', '$locationProvider', '$provide',
        function ($stateProvider, $routeProvider, $urlRouterProvider, $couchPotatoProvider, $locationProvider, $provide) {

            $stateProvider
                .state('List', {
                    url: '/{Module:[a-zA-Z]+}',
                    templateProvider: ['$http', '$stateParams', function ($http, $stateParams) {
                        var url = 'Coevery/' + $stateParams.Module + '/ViewTemplate/List/' + $stateParams.Module;
                        return $http.get(url).then(function (response) { return response.data; });
                    }],
                    resolve: {
                        dummy: ['$q', '$rootScope', '$stateParams', function ($q, $rootScope, $stateParams) {
                            return $couchPotatoProvider.resolveDependencies($q, $rootScope, ['core/controllers/listcontroller']);
                        }]
                    }
                })
                .state('Create', {
                    url: '/{Module:[a-zA-Z]+}/Create',
                    templateUrl: function (params) {
                        return "Coevery/" + params.Module + '/ViewTemplate/Create/' + params.Module;
                    },
                    resolve: {
                        dummy: ['$q', '$rootScope', '$stateParams', function ($q, $rootScope, $stateParams) {
                            return $couchPotatoProvider.resolveDependencies($q, $rootScope, ['core/controllers/detailcontroller']);
                        }]
                    }
                })
                .state('Detail', {
                    url: '/{Module:[a-zA-Z]+}/{Id:[0-9a-zA-Z]+}',
                    templateProvider: ['$http', '$stateParams', function ($http, $stateParams) {
                        var url = 'Coevery/' + $stateParams.Module + '/ViewTemplate/Edit/' + $stateParams.Id;
                        return $http.get(url).then(function (response) { return response.data; });
                    }],
                    resolve: {
                        dummy: ['$q', '$rootScope', '$stateParams', function ($q, $rootScope, $stateParams) {
                            return $couchPotatoProvider.resolveDependencies($q, $rootScope, ['core/controllers/detailcontroller']);
                        }]
                    }
                })
                .state('View', {
                    url: '/{Module:[a-zA-Z]+}/View/{Id:[0-9a-zA-Z]+}',
                    templateProvider: ['$http', '$stateParams', function ($http, $stateParams) {
                        var url = 'Coevery/' + $stateParams.Module + '/ViewTemplate/View/' + $stateParams.Id;
                        return $http.get(url).then(function (response) { return response.data; });
                    }],
                    resolve: {
                        dummy: ['$q', '$rootScope', '$stateParams', function ($q, $rootScope, $stateParams) {
                            return $couchPotatoProvider.resolveDependencies($q, $rootScope, ['core/controllers/viewcontroller']);
                        }]
                    }
                });
        }]);

    coevery.run(['$rootScope', '$state', '$stateParams', '$couchPotato',
        function ($rootScope, $state, $stateParams, $couchPotato) {
            //"cheating" so that couchPotato is available in requirejs
            //define modules -- we want run-time registration of components
            //to take place within those modules because it allows
            //for them to have their own dependencies also be lazy-loaded.
            //this is what requirejs is good at.
            coevery.couchPotato = $couchPotato;
            $rootScope.$state = $state;
            $rootScope.$stateParams = $stateParams;
            $rootScope.i18nextOptions = {
                resGetPath: 'i18n/__ns_____lng__.json',
                lowerCaseLng: true,
                ns: 'resources-locale'
            };

            function getGridMinHeight(currentGrid) {
                var findGrids = $(".gridStyle.ng-scope.ngGrid");
                var availHeight = window.innerHeight -
                    $("#header").outerHeight(true) -
                    $("#footer").outerHeight(true);
                var currentGridNumber = 0;
                if (isNaN(availHeight)) {
                    alert("Wrong variable used!");
                }

                for (var index = 0; index < findGrids.length; index++) {
                    var tempGrid = findGrids.eq(-index - 1);
                    availHeight -= tempGrid.height();
                    if (tempGrid.find(currentGrid) != 0) {
                        currentGridNumber = index + 1;
                    }
                }

                //Decide whether current grid can use auto minHight;
                if (availHeight < 0 || currentGridNumber > Math.ceil((availHeight - findGrids.last().offset().top) % 100)) {
                    return 150;
                }
                var minHeight = availHeight - currentGrid.offset().top + currentGrid.parent().height();
                if (minHeight < 200) {
                    minHeight = 200;
                }
                return minHeight;
            }

            $rootScope.defaultGridOptions = {
                plugins: [new ngGridFlexibleHeightPlugin({ minHeight: 0 }), new ngGridRowSelectionPlugin()],
                enableColumnResize: true,
                enableColumnReordering: true,
                enablePaging: true,
                showFooter: true,
                totalServerItems: "totalServerItems",
                footerTemplate: 'Coevery/CoeveryCore/GridTemplate/DefaultFooterTemplate'
            };
        }
    ]);

    return coevery;

});

$(function () {
    $('body').on("submit", 'form', function (event) {
        event.preventDefault();
    });
});
