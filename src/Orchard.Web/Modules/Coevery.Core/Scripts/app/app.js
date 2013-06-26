define([], function () {
    'use strict';

    var coevery = angular.module('coevery', ['ng', 'ngGrid', 'ngResource', 'agt.couchPotato', 'ui.compat','ui.utils', 'coevery.layout']);
    coevery.config(['$stateProvider', '$routeProvider', '$urlRouterProvider', '$couchPotatoProvider', '$locationProvider', '$provide',
        function($stateProvider, $routeProvider, $urlRouterProvider, $couchPotatoProvider, $locationProvider, $provide) {

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
                    url: '/{Module:[a-zA-Z]+}/Create',
                    templateUrl: function(params) {
                        return "Coevery/" + params.Module + '/ViewTemplate/Create/' + params.Module;
                    },
                    resolve: {
                        dummy: ['$q', '$rootScope', '$stateParams', function($q, $rootScope, $stateParams) {
                            return $couchPotatoProvider.resolveDependencies($q, $rootScope, ['core/controllers/detailcontroller']);
                        }]
                    }
                })
                .state('Detail', {
                    url: '/{Module:[a-zA-Z]+}/{Id:[0-9a-zA-Z]+}',
                    templateUrl: function(params) {
                        return "Coevery/" + params.Module + '/ViewTemplate/Edit/' + params.Id;
                    },
                    resolve: {
                        dummy: ['$q', '$rootScope', '$stateParams', function ($q, $rootScope, $stateParams) {
                            return $couchPotatoProvider.resolveDependencies($q, $rootScope, ['core/controllers/detailcontroller']);
                        }]
                    }
                });
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
            $rootScope.i18nextOptions = {
                resGetPath: 'i18n/__ns_____lng__.json',
                lowerCaseLng: true,
                ns: 'resources-locale'
            };
            
            $rootScope.defaultGridOptions = {
                plugins: [new ngGridFlexibleHeightPlugin({ minHeight: 200 })],
                multiSelect: false,
                enableRowSelection: false,
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

//function SetActiveMenu(module) {
//    if (!$(".menu>#" + module).hasClass("active")) {
//        $(".menu>li.active").removeClass("active");
//        var $li = $(".menu>#" + module);
//        $li.addClass("active");
//        if (!$li.parent()) return;
//        var $liParent = $li.parent();
//        if ($liParent[0] && $("#navigation #" + $liParent[0].id).css('display') != 'block') {
//            $("#navigation .menu").css('display', 'none');
//            $("#navigation #" + $liParent[0].id).css('display', 'block');
//            $("#FirstMenu .btn-group #FirstMenuText").text($liParent[0].id);
//        }
//    }
//}


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
