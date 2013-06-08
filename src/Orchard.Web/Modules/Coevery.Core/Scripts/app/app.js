define([], function () {
    'use strict';

    var coevery = angular.module('coevery', ['agt.couchPotato', 'ui.compat', 'coevery.layout']);
    coevery.config(['$stateProvider', '$routeProvider', '$urlRouterProvider', '$couchPotatoProvider', '$locationProvider', '$provide',
        function($stateProvider, $routeProvider, $urlRouterProvider, $couchPotatoProvider, $locationProvider, $provide) {
            //comment out the decorator function for html5mode
            //uncomment the decorator function for force hash(bang) mode
            // $provide.decorator('$sniffer', function($delegate) {
            //   $delegate.history = false;
            //   return $delegate;
            // });
            //$locationProvider.html5Mode(true);


            $urlRouterProvider
                .when('/c?id', '/contacts/:id')
                .otherwise('/');

            $routeProvider
                .when('/user/:id', {
                    redirectTo: '/contacts/:id'
                });

            $stateProvider
                .state('List', {
                    url: '/{Module:[a-zA-Z]+}',
                    templateUrl: function(params) {
                        return "Coevery/" + params.Module + '/ViewTemplate/List/' + params.Module;
                    },
                    resolve: {
                        dummy: ['$q', '$rootScope', '$stateParams', function ($q, $rootScope, $stateParams) {
                            return $couchPotatoProvider.resolveDependencies($q, $rootScope, ['core/controllers/listcontroller']);
                        }]
                    }
                })
                .state('Create', {
                    parent: 'List',
                    url: '/Create',
                    templateUrl: function(params) {
                        return "Coevery/" + params.Module + '/ViewTemplate/Create/' + params.Module;
                    },
                    resolve: {
                        dummy: ['$stateParams', function($stateParams) {
                            return $couchPotatoProvider.resolveDependencies([$stateParams.Module + '/Scripts/Controllers/detailcontroller']);
                        }]
                    }
                })
                .state('Detail', {
                    parent: 'List',
                    url: '/{Id:[0-9a-zA-Z]+}',
                    templateUrl: function(params) {
                        return "Coevery/" + params.Module + '/ViewTemplate/Edit/' + params.Id;
                    }
                });
            //.state('SubCreate', {
            //    parent: 'Detail',
            //    url: '/{SubModule:[a-zA-Z]+}/Create',
            //    templateUrl: function(params) {
            //        return params.Module + '/' + params.SubModule + 'ViewTemplate/Create/' + params.Id;
            //    }
            //})
            //.state('SubList', {
            //    parent: 'Detail',
            //    url: '/{SubModule:[a-zA-Z]+}/{View:[a-zA-Z]+}',
            //    templateProvider: ['$http', '$stateParams', function($http, $stateParams) {
            //        var url = $stateParams.Module + '/' + $stateParams.SubModule + 'ViewTemplate/' + $stateParams.View + '/' + $stateParams.Id;
            //        return $http.get(url).then(function(response) { return response.data; });
            //    }]
            //})
            //.state('SubDetail', {
            //    parent: 'SubList',
            //    url: '/{SubId:[0-9a-zA-Z]+}',
            //    templateProvider: ['$http', '$stateParams', function($http, $stateParams) {
            //        var url = $stateParams.Module + '/' + $stateParams.SubModule + 'ViewTemplate/' + $stateParams.View + '/' + $stateParams.Id + '?subId=' + $stateParams.SubId;
            //        return $http.get(url).then(function(response) { return response.data; });
            //    }]
            //});
        }]);

    coevery.run(['$rootScope', '$state', '$stateParams', '$couchPotato',
        function($rootScope, $state, $stateParams, $couchPotato) {
            //"cheating" so that couchPotato is available in requirejs
            //define modules -- we want run-time registration of components
            //to take place within those modules because it allows
            //for them to have their own dependencies also be lazy-loaded.
            //this is what requirejs is good at.
            coevery.couchPotato = $couchPotato;
            $rootScope.$state = $state;
            $rootScope.$stateParams = $stateParams;
        }
    ]);

    return coevery;

});

function SetActiveMenu(module) {
    if (!$(".menu>#" + module).hasClass("active")) {
        $(".menu>li.active").removeClass("active");
        var $li = $(".menu>#" + module);
        $li.addClass("active");
        if (!$li.parent()) return;
        var $liParent = $li.parent();
        if ($liParent[0] && $("#navigation #" + $liParent[0].id).css('display') != 'block') {
            $("#navigation .menu").css('display', 'none');
            $("#navigation #" + $liParent[0].id).css('display', 'block');
            $("#FirstMenu .btn-group #FirstMenuText").text($liParent[0].id);
        }
    }
}


angular.module('coevery.layout', [])
    .directive('fdSection', function() {
        return {
            template: '<fieldset fd-section><legend class="title">Section Title</legend><div ng-transclude></div></fieldset>',
            replace: true,
            restrict: 'E',
            transclude: true
        };
    })
    .directive('fdRow', function() {
        return {
            template: '<div fd-row class="control-group" ng-transclude></div>',
            replace: true,
            restrict: 'E',
            transclude: true
        };
    })
    .directive('fdColumn', function() {
        return {
            template: '<div fd-column ng-transclude></div>',
            replace: true,
            restrict: 'E',
            transclude: true,
            link: function(scope, element, attrs) {
                var columnCount = parseInt(element.parents('[fd-section]:first').attr('section-columns'));
                var width = 12 / columnCount;
                element.addClass('span' + width);
            }
        };
    })
    .directive('fdField', function() {
        return {
            template: '<div fd-field></div>',
            replace: true,
            restrict: 'E',
            link: function(scope, element, attrs) {
                var template = $('script[type="text/ng-template"][id="' + attrs.fieldName + '.html"]');
                element.html(template.text());
            }
        };
    });

$(function() {
    $('body').on("submit", 'form', function(event) {
        event.preventDefault();
    });
});
