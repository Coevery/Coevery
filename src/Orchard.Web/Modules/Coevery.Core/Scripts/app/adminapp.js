define(['angular-detour'], function () {
    'use strict';

    var coevery = angular.module('coevery', ['ng', 'ngGrid', 'ngResource', 'agt.detour', 'coevery.layout']);
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

            $templateCache.put("footerTemplate.html",
                //"<div class=\"dt_footer\" style=\"background-color: rgb(242,246,255); border: 1px solid #d4d4d4;\">"+
                //"    <div class=\"row-fluid\">"+
                //"        <div class=\"span6\">"+
                //"            <div class=\"dataTables_info\" id=\"demo-dtable-03_info\">"+
                //"                Showing {{(pagingOptions.currentPage -1)*pagingOptions.pageSize + 1}} to"+
                //"                {{pagingOptions.totalServerItems <= pagingOptions.currentPage*pagingOptions.pageSize?pagingOptions.totalServerItems:pagingOptions.currentPage*pagingOptions.pageSize}}"+
                //"                of {{pagingOptions.totalServerItems}} entries"+
                //"            </div>"+
                //"        </div>"+
                //"        <div class=\"span6\">"+
                //"            <div class=\"dataTables_paginate paging_bootstrap pagination\">"+
                //"                <ul>"+
                //"                    <li ng-class=\"{'prev disabled': pagingOptions.currentPage == pagingOptions.pageNumber[0]}\"><a href=\"#\" ng-click=\"pageBackward()\">← Previous</a></li>"+
                //"                    <li ng-repeat=\"page in pagingOptions.pageNumber\" ng-class=\"{'active': pagingOptions.currentPage == {{page}}}\">"+
                //"                        <a href=\"#\" ng-click=\"GoToPage({{page}})\">{{page}}</a></li>"+
                //"                    <li class=\"next\" ng-class=\"{'prev disabled': pagingOptions.currentPage == pagingOptions.pageNumber[pagingOptions.pageNumber.length-1]}\"><a href=\"#\" ng-click=\"pageForward()\">Next → </a></li>"+
                //"                </ul>"+
                //"            </div>"+
                //"        </div>"+
                //"   </div>" +
                //"</div>"

                "<div ng-show=\"showFooter\" class=\"ngFooterPanel\" ng-class=\"{'ui-widget-content': jqueryUITheme, 'ui-corner-bottom': jqueryUITheme}\" ng-style=\"footerStyle()\">" +
                "    <div class=\"ngTotalSelectContainer\" >" +
                "        <div class=\"ngFooterTotalItems\" ng-class=\"{'ngNoMultiSelect': !multiSelect}\" >" +
                "            <span class=\"ngLabel\">{{i18n.ngTotalItemsLabel}} {{maxRows()}}</span><span ng-show=\"filterText.length > 0\" class=\"ngLabel\">({{i18n.ngShowingItemsLabel}} {{totalFilteredItemsLength()}})</span>" +
                "        </div>" +
                "        <div class=\"ngFooterSelectedItems\" ng-show=\"multiSelect\">" +
                "            <span class=\"ngLabel\">{{i18n.ngSelectedItemsLabel}} {{selectedItems.length}}</span>" +
                "        </div>" +
                "    </div>" +
                "    <div class=\"ngPagerContainer\" style=\"float: right; margin-top: 10px;\" ng-show=\"enablePaging\" ng-class=\"{'ngNoMultiSelect': !multiSelect}\">" +
                "        <div style=\"float:left; margin-right: 10px;\" class=\"ngRowCountPicker\">" +
                "            <span style=\"float: left; margin-top: 3px;\" class=\"ngLabel\">{{i18n.ngPageSizeLabel}}</span>" +
                "            <select style=\"float: left;height: 27px; width: 100px\" ng-model=\"pagingOptions.pageSize\" >" +
                "                <option ng-repeat=\"size in pagingOptions.pageSizes\">{{size}}</option>" +
                "            </select>" +
                "        </div>" +
                "        <div style=\"float:left; margin-right: 10px; line-height:25px;\" class=\"ngPagerControl\" style=\"float: left; min-width: 135px;\">" +
                "            <button class=\"ngPagerButton\" ng-click=\"pageToFirst()\" ng-disabled=\"cantPageBackward()\" title=\"{{i18n.ngPagerFirstTitle}}\"><div class=\"ngPagerFirstTriangle\"><div class=\"ngPagerFirstBar\"></div></div></button>" +
                "            <button class=\"ngPagerButton\" ng-click=\"pageBackward()\" ng-disabled=\"cantPageBackward()\" title=\"{{i18n.ngPagerPrevTitle}}\"><div class=\"ngPagerFirstTriangle ngPagerPrevTriangle\"></div></button>" +
                "            <input class=\"ngPagerCurrent\" type=\"number\" style=\"width:50px; height: 24px; margin-top: 1px; padding: 0 4px;\" ng-model=\"pagingOptions.currentPage\"/>" +
                "            <button class=\"ngPagerButton\" ng-click=\"pageForward()\" ng-disabled=\"cantPageForward()\" title=\"{{i18n.ngPagerNextTitle}}\"><div class=\"ngPagerLastTriangle ngPagerNextTriangle\"></div></button>" +
                "            <button class=\"ngPagerButton\" ng-click=\"pageToLast()\" ng-disabled=\"cantPageToLast()\" title=\"{{i18n.ngPagerLastTitle}}\"><div class=\"ngPagerLastTriangle\"><div class=\"ngPagerLastBar\"></div></div></button>" +
                "        </div>" +
                "    </div>" +
                "</div>"
            );
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
