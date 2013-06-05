'use strict';

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
            $(".btn-toolbar .btn-group #firstMenu").text($liParent[0].id);
        }
    }
}

var coevery = angular.module('coevery', ['ng', 'ngGrid', 'ngResource', 'localization', 'ui.compat', 'coevery.layout'])
    .value('$anchorScroll', angular.noop)
    .config(['$stateProvider', function ($stateProvider) {
        $stateProvider
            .state('List', {
                url: '/{Module:[a-zA-Z]+}',
                templateUrl: function (params) {
                    SetActiveMenu(params.Module);
                    return "Coevery/" + params.Module + '/ViewTemplate/List/' + params.Module;
                }
            })
        
            .state('Create', {
                url: '/{Module:[a-zA-Z]+}/Create',
                templateUrl: function (params) {
                    return "Coevery/" + params.Module + '/ViewTemplate/Create/' + params.Module;
                }
            })
            .state('Detail', {
                url: '/{Module:[a-zA-Z]+}/{Id:[0-9a-zA-Z]+}',
                templateUrl: function (params) {
                    return "Coevery/" + params.Module + '/ViewTemplate/Edit/' + params.Id;
                }
            })
            .state('SubCreate', {
                url: '/{Module:[a-zA-Z]+}/{Id:[0-9a-zA-Z]+}/{SubModule:[a-zA-Z]+}/Create',
                templateUrl: function (params) {
                    return params.Module + '/' + params.SubModule + 'ViewTemplate/Create/' + params.Id;
                }
            })
            .state('SubList', {
                url: '/{Module:[a-zA-Z]+}/{Id:[0-9a-zA-Z]+}/{SubModule:[a-zA-Z]+}/{View:[a-zA-Z]+}',
                templateProvider: ['$http', '$stateParams', function ($http, $stateParams) {
                    var url = $stateParams.Module + '/' + $stateParams.SubModule + 'ViewTemplate/' + $stateParams.View + '/' + $stateParams.Id;
                    return $http.get(url).then(function (response) { return response.data; });
                }]
            })
            .state('SubDetail', {
                url: '/{Module:[a-zA-Z]+}/{Id:[0-9a-zA-Z]+}/{SubModule:[a-zA-Z]+}/{View:[a-zA-Z]+}/{SubId:[0-9a-zA-Z]+}',
                templateProvider: ['$http', '$stateParams', function ($http, $stateParams) {
                    var url = $stateParams.Module + '/' + $stateParams.SubModule + 'ViewTemplate/' + $stateParams.View + '/' + $stateParams.Id + '?subId=' + $stateParams.SubId;
                    return $http.get(url).then(function(response) { return response.data; });
                }]
            });
    }])
    .run(
        ['$rootScope', '$state', '$stateParams',
            function ($rootScope, $state, $stateParams) {
                $rootScope.$state = $state;
                $rootScope.$stateParams = $stateParams;
            }]);

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
