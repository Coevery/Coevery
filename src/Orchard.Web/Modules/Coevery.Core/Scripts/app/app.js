define(['angular-detour', 'core/directives/common'], function () {
    'use strict';

    var coevery = angular.module('coevery', ['ng', 'ngGrid', 'ngResource', 'agt.detour', 'ui.compat', 'ui.utils', 'coevery.layout', 'coevery.grid', 'SharedServices', 'angular-underscore', 'coevery.common', 'coevery.filter']);
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

            $rootScope.$on('$viewContentLoaded', function () {
               
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
                    '<span class="icon-remove delete-action" data-id= \"{1}\" title="Delete"></span>' +
                    '</section>' +
                    '<span class=\"{3}\" data-id= {1} > {2} </span> </div>';
                if (!options.colModel.formatoptions) {
                    return template.format(options.rowId, options.rowId, cellvalue, '');
                }

                var editParams, viewStyle;
                if (options.colModel.formatoptions.editRow) {
                    editParams = JSON.stringify(rowObject);
                } else {
                    editParams = options.rowId;
                }
                if (options.colModel.formatoptions.hasView) {
                    viewStyle = 'btn-link view-action';
                } else {
                    viewStyle = '';
                }

                return template.format(editParams, options.rowId, cellvalue, viewStyle);
            };

            $rootScope.defaultGridOptions = {
                datatype: "json",
                pagerpos: "right",
                recordpos: "left",
                sortable: true,
                height: "100%",
                viewrecords: true,
                multiselect: true,
                multiboxonly: true,
                autowidth: true,
                loadui: "disable",
                jsonReader: {
                    page: "page",
                    total: "totalPages",
                    records: "totalRecords",
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

/*Abondoned code
$rootScope.defaultGridOptions = {
                plugins: [new ngGridFlexibleHeightPlugin({ minHeight: 0 }), new ngGridRowSelectionPlugin()],
                enableColumnResize: true,
                enableColumnReordering: true,
                enablePaging: true,
                showFooter: true,
                totalServerItems: "totalServerItems",
                footerTemplate: 'Coevery/CoeveryCore/GridTemplate/DefaultFooterTemplate'
            };
*/