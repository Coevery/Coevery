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
                    $("#footer").outerHeight(true) -
                    $(".navbar.navbar-fixed-bottom").outerHeight(true);
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
                if (currentGridNumber > Math.floor((availHeight - findGrids.last().offset().top) % 100)) {
                    return 0;
                }
                var minHeight = availHeight - currentGrid.offset().top + currentGrid.parent().height();
                if (minHeight < 100) {
                    minHeight = 100;
                }
                return minHeight;
            }

            $rootScope.defaultGridOptions = {
                plugins: [new ngGridFlexibleHeightPlugin({ minHeight: getGridMinHeight })],
                //multiSelect: false,
                //enableRowSelection: false,
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
            template: '<fieldset fd-section class="data-section"><header><legend><h5>Section Title</h5></legend></header><div ng-transclude></div></fieldset>',
            replace: true,
            restrict: 'E',
            transclude: true,
            link: function (scope, element, attrs) {
                var sectionHeader = element.find('header h5');
                sectionHeader.text(attrs.sectionTitle);
            }
        };
    })
    .directive('fdRow', function () {
        return {
            template: '<div fd-row class="data-row clearfix" ng-transclude></div>',
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
