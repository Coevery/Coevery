define(['core/directives/common'], function () {
    'use strict';

    coevery.run(['$rootScope', '$state', '$stateParams', '$couchPotato',
        function ($rootScope, $state, $stateParams, $couchPotato) {
            //"cheating" so that couchPotato is available in requirejs
            //define modules -- we want run-time registration of components
            //to take place within those modules because it allows
            //for them to have their own dependencies also be lazy-loaded.
            //this is what requirejs is good at.
            coevery.detour = $couchPotato;
            
            $rootScope.$detour = $couchPotato;
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
                    '<span class="icon-edit" data-ng-click="edit(\'{0}\')" title="Edit"></span>' +
                    '<span class="icon-remove" co-delete-button confirm-message="You really want to delete this row?" ' +
                    'delete-action="delete(\'{0}\')" title="Delete"></span></section>' +
                    '<div>{1}</div> </div>';
                var viewStyle = cellvalue;
                if (!options.colModel.formatoptions) {
                    return template.format(options.rowId, viewStyle);
                }
                if (options.colModel.formatoptions.hasView) {
                    viewStyle = '<a class="btn-link" data-ng-click="view(\'' + options.rowId + '\')"> ' +
                        cellvalue + '</a>';
                }
                return template.format(options.rowId, viewStyle);
            };

            $rootScope.defaultGridOptions = {
                datatype: "json",
                pagerpos: "right",
                recordpos: "left",
                sortable: true,
                height: "100%",
                headertitles: true,
                viewrecords: true,
                multiselect: true,
                multiboxonly: true,
                autowidth: true,
                pginput: false,
                pgbuttons: false,
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