//'use strict';

//var coevery = angular.module('coevery', ['ng', 'ngGrid', 'ngResource', 'localization', 'ui.compat', 'coevery.layout'])
//    .config(['$stateProvider', function ($stateProvider) {

//        $stateProvider
//            .state('List', {
//                url: '/{Module:[a-zA-Z]+}',
//                templateUrl: function (params) {
//                    return "Coevery/" + params.Module + '/ViewTemplate/List/' + params.Module;
//                }
//            })
        
//            .state('Create', {
//                url: '/{Module:[a-zA-Z]+}/Create',
//                templateUrl: function (params) {
//                    return "Coevery/" + params.Module + '/ViewTemplate/Create/' + params.Module;
//                }
//            })
//            .state('Detail', {
//                url: '/{Module:[a-zA-Z]+}/{Id:[0-9a-zA-Z]+}',
//                templateUrl: function (params) {
//                    return "Coevery/" + params.Module + '/ViewTemplate/Edit/' + params.Id;
//                }
//            })
//            .state('SubCreate', {
//                url: '/{Module:[a-zA-Z]+}/{Id:[0-9a-zA-Z]+}/{SubModule:[a-zA-Z]+}/Create',
//                templateUrl: function (params) {
//                    return params.Module + '/' + params.SubModule + 'ViewTemplate/Create/' + params.Id;
//                }
//            })
//            .state('SubList', {
//                url: '/{Module:[a-zA-Z]+}/{Id:[0-9a-zA-Z]+}/{SubModule:[a-zA-Z]+}/{View:[a-zA-Z]+}',
//                templateProvider: ['$http', '$stateParams', function ($http, $stateParams) {
//                    var url = $stateParams.Module + '/' + $stateParams.SubModule + 'ViewTemplate/' + $stateParams.View + '/' + $stateParams.Id;
//                    return $http.get(url).then(function (response) { return response.data; });
//                }]
//            })
//            .state('SubDetail', {
//                url: '/{Module:[a-zA-Z]+}/{Id:[0-9a-zA-Z]+}/{SubModule:[a-zA-Z]+}/{View:[a-zA-Z]+}/{SubId:[0-9a-zA-Z]+}',
//                templateProvider: ['$http', '$stateParams', function ($http, $stateParams) {
//                    var url = $stateParams.Module + '/' + $stateParams.SubModule + 'ViewTemplate/' + $stateParams.View + '/' + $stateParams.Id + '?subId=' + $stateParams.SubId;
//                    return $http.get(url).then(function(response) { return response.data; });
//                }]
//            });
//    }])
//    .run(
//        ['$rootScope', '$state', '$stateParams',
//            function ($rootScope, $state, $stateParams) {
//                $rootScope.$state = $state;
//                $rootScope.$stateParams = $stateParams;
//            }]);


define(['angular-detour'], function () {
    'use strict';

    var coevery = angular.module('coevery', ['ng', 'ngGrid', 'ngResource', 'agt.detour', 'coevery.layout']);
    coevery.config(['$locationProvider', '$provide', '$detourProvider',
        function ($locationProvider, $provide, $detourProvider) {
            $detourProvider.loader = {
                lazy: {
                    enabled: true,
                    routeUrl: 'api/CoeveryCore/Route',
                    stateUrl: 'svc/getState'
                },
                crossDomain: true,
                httpMethod: 'GET'
            };
        }]);

    coevery.run(['$rootScope', '$detour', '$stateParams',
        function ($rootScope, $detour, $stateParams) {

            //"cheating" so that detour is available in requirejs
            //define modules -- we want run-time registration of components
            //to take place within those modules because it allows
            //for them to have their own dependencies also be lazy-loaded.
            //this is what requirejs is good at.

            //if not using any dependencies properties in detour states,
            //then this is not necessary
            coevery.detour = $detour;

            //the sample reads from the current $detour.state
            //and $stateParams in its templates
            //that it the only reason this is necessary
            $rootScope.$detour = $detour;
            $rootScope.$stateParams = $stateParams;
            
            $rootScope.i18nextOptions = {
                resGetPath: 'i18n/__ns_____lng__.json',
                lowerCaseLng: true,
                ns: 'resources-locale'
            };
        }
    ]);

    return coevery;

});

angular.module('coevery.layout', [])
    .directive('fdSection', function () {
        return {
            template: '<fieldset fd-section><legend class="title">Section Title</legend><div ng-transclude></div></fieldset>',
            replace: true,
            restrict: 'E',
            transclude: true
        };
    })
    .directive('fdRow', function () {
        return {
            template: '<div fd-row class="control-group" ng-transclude></div>',
            replace: true,
            restrict: 'E',
            transclude: true
        };
    })
    .directive('fdColumn', function () {
        return {
            template: '<div fd-column ng-transclude></div>',
            replace: true,
            restrict: 'E',
            transclude: true,
            link: function (scope, element, attrs) {
                var columnCount = parseInt(element.parents('[fd-section]:first').attr('section-columns'));
                var width = 12 / columnCount;
                element.addClass('span' + width);
            }
        };
    })
    .directive('fdField', function () {
        return {
            template: '<div fd-field></div>',
            replace: true,
            restrict: 'E',
            link: function (scope, element, attrs) {
                var template = $('script[type="text/ng-template"][id="' + attrs.fieldName + '.html"]');
                element.html(template.text());
            }
        };
    });

$(function () {
    $('body').on("submit", 'form', function (event) {
        event.preventDefault();
    });
});
