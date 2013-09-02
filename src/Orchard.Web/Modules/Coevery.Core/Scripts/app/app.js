define(['angular-detour'], function () {
    'use strict';

    var coevery = angular.module('coevery', ['ng', 'ngGrid', 'ngResource', 'agt.detour', 'ui.compat', 'ui.utils', 'coevery.layout', 'SharedServices', 'angular-underscore']);
    coevery.config(['$detourProvider', '$provide',
        function ($detourProvider, $provide) {        
            $detourProvider.loader = {
                lazy: {
                    enabled: true,
                    routeUrl: 'api/CoeveryCore/Route',
                    stateUrl: 'api/CoeveryCore/State',
                    routeParameter: 'isFront=true&r'
                },
                crossDomain: true,
                httpMethod: 'GET'
            };

            //        $stateProvider
            //            .state('List', {
            //                url: '/{Module:[a-zA-Z]+}',
            //                templateProvider: ['$http', '$stateParams', function ($http, $stateParams) {
            //                    var url = 'Coevery/' + $stateParams.Module + '/ViewTemplate/List/' + $stateParams.Module;
            //                    return $http.get(url).then(function (response) { return response.data; });
            //                }],
            //                resolve: {
            //                    dummy: ['$q', '$rootScope', '$stateParams', function ($q, $rootScope, $stateParams) {
            //                        return $couchPotatoProvider.resolveDependencies($q, $rootScope, ['core/controllers/listcontroller']);
            //                    }]
            //                }
            //            })
            //            .state('Create', {
            //                url: '/{Module:[a-zA-Z]+}/Create',
            //                templateUrl: function (params) {
            //                    return "Coevery/" + params.Module + '/ViewTemplate/Create/' + params.Module;
            //                },
            //                resolve: {
            //                    dummy: ['$q', '$rootScope', '$stateParams', function ($q, $rootScope, $stateParams) {
            //                        return $couchPotatoProvider.resolveDependencies($q, $rootScope, ['core/controllers/detailcontroller']);
            //                    }]
            //                }
            //            })
            //            .state('Detail', {
            //                url: '/{Module:[a-zA-Z]+}/{Id:[0-9a-zA-Z]+}',
            //                templateProvider: ['$http', '$stateParams', function ($http, $stateParams) {
            //                    var url = 'Coevery/' + $stateParams.Module + '/ViewTemplate/Edit/' + $stateParams.Id;
            //                    return $http.get(url).then(function (response) { return response.data; });
            //                }],
            //                resolve: {
            //                    dummy: ['$q', '$rootScope', '$stateParams', function ($q, $rootScope, $stateParams) {
            //                        return $couchPotatoProvider.resolveDependencies($q, $rootScope, ['core/controllers/detailcontroller']);
            //                    }]
            //                }
            //            })
            //            .state('View', {
            //                url: '/{Module:[a-zA-Z]+}/View/{Id:[0-9a-zA-Z]+}',
            //                templateProvider: ['$http', '$stateParams', function ($http, $stateParams) {
            //                    var url = 'Coevery/' + $stateParams.Module + '/ViewTemplate/View/' + $stateParams.Id;
            //                    return $http.get(url).then(function (response) { return response.data; });
            //                }],
            //                resolve: {
            //                    dummy: ['$q', '$rootScope', '$stateParams', function ($q, $rootScope, $stateParams) {
            //                        return $couchPotatoProvider.resolveDependencies($q, $rootScope, ['core/controllers/viewcontroller']);
            //                    }]
            //                }
            //            });
        }
    ]);

    coevery.run(['$rootScope', '$state', '$stateParams', '$detour',
        function ($rootScope, $state, $stateParams, $detour) {
            //"cheating" so that couchPotato is available in requirejs
            //define modules -- we want run-time registration of components
            //to take place within those modules because it allows
            //for them to have their own dependencies also be lazy-loaded.
            //this is what requirejs is good at.
            coevery.detour = $detour;
            
            $rootScope.$detour = $detour;
            $rootScope.$state = $state;
            $rootScope.$stateParams = $stateParams;
            $rootScope.i18nextOptions = {
                resGetPath: 'i18n/__ns_____lng__.json',
                lowerCaseLng: true,
                ns: 'resources-locale'
            };

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
