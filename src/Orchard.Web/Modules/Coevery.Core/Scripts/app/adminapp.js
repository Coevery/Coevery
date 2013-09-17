define(['angular-detour', 'core/app/formdesignerservice', 'core/directives/common'], function () {
    'use strict';

    var coevery = angular.module('coevery', ['ng', 'ngGrid', 'ngResource', 'agt.detour', 'ui.utils', 'coevery.formdesigner', 'coevery.grid', 'SharedServices', 'ui.bootstrap', 'coevery.common']);
    coevery.config(['$locationProvider', '$provide', '$detourProvider',
        function ($locationProvider, $provide, $detourProvider) {
            $detourProvider.loader = {
                lazy: {
                    enabled: true,
                    routeUrl: 'api/CoeveryCore/Route',
                    stateUrl: 'api/CoeveryCore/State',
                    routeParameter: 'isFront=false&r'
                },
                crossDomain: true,
                httpMethod: 'GET'
            };
        }]);

    coevery.value('$anchorScroll', angular.noop);

    coevery.run(['$rootScope', '$detour', '$stateParams', '$templateCache',
        function ($rootScope, $detour, $stateParams, $templateCache) {

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
            $rootScope.$on('$viewContentLoaded', function () {
                $(window).scrollTop(0);
            });

            $rootScope.i18nextOptions = {
                resGetPath: 'i18n/__ns_____lng__.json',
                lowerCaseLng: true,
                ns: 'resources-locale'
            };
            
            if (!String.prototype.format) {
                String.prototype.format = function () {
                    var args = arguments;
                    return this.replace(/{(\d+)}/g, function (match, number) {
                        return typeof args[number] != 'undefined'
                          ? args[number]
                          : match
                        ;
                    });
                };
            }

            $rootScope.cellLinkTemplate = function (cellvalue, options, rowObject) {
                var template = '<div class="gridCellText">' +
                    '<section class="row-actions hide">' +
                    '<span class="icon-edit edit-action" data-id={0} title="Edit"></span>' +
                    '<span class="icon-remove delete-action" data-id= {1} title="Delete"></span>' +
                    '{4}</section>' +
                    '<span class={3} data-id= {1} > {2} </span> </div>';
                if (!options.colModel.formatoptions) {
                    return template.format(options.rowId, options.rowId, cellvalue, '', '');
                }
                
                var editParams, viewStyle, defaultStyle;
                if (options.colModel.formatoptions.useType) {
                    editParams = JSON.stringify({ id: options.rowId , type: rowObject.Type });
                } else {
                    editParams = options.rowId;
                }
                if (options.colModel.formatoptions.hasView) {
                    viewStyle = 'btn-link view-action';
                } else {
                    viewStyle = '';
                }
                if (options.colModel.formatoptions.hasDefault) {
                    defaultStyle = '<span class="icon-tags default-action" data-id="' + options.rowId + '" title="Set Default"></span>';
                } else {
                    defaultStyle = '';
                }

                return template.format(editParams, options.rowId, cellvalue, viewStyle, defaultStyle);
            };

            $rootScope.defaultGridOptions = {
                datatype: "json",
                loadonce: true,
                pagerpos: "right",
                recordpos: "left",
                sortable: true,
                height: "100%",
                viewrecords: true,
                multiselect: true,
                multiboxonly: true,
                shrinkToFit: false,
                rowNum: 50,
                rowList: [50, 100, 200],
                loadui: "disable",
                jsonReader: {
                    repeatitems: false,
                    id: "0" //Get Id from first column
                },
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
