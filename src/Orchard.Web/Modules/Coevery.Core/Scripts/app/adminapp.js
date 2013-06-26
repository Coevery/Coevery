define(['angular-detour'], function () {
    'use strict';

    var coevery = angular.module('coevery', ['ng', 'ngGrid', 'ngResource', 'agt.detour', 'ui.utils', 'coevery.layout']);
    coevery.config(['$locationProvider', '$provide', '$detourProvider',
        function ($locationProvider, $provide, $detourProvider) {
            $detourProvider.loader = {
                lazy: {
                    enabled: true,
                    routeUrl: 'api/CoeveryCore/Route',
                    stateUrl: 'api/CoeveryCore/State'
                },
                crossDomain: true,
                httpMethod: 'GET'
            };
        }]);

    coevery.value('$anchorScroll', angular.noop);

    coevery.run(['$rootScope', '$detour', '$stateParams', '$templateCache',
        function($rootScope, $detour, $stateParams, $templateCache) {

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
            
            $rootScope.defaultGridOptions = {
                data: 'myData',
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
