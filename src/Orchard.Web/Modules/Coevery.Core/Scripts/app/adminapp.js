define(['core/app/formdesignerservice', 'core/directives/common'], function () {
    'use strict';
    
    coevery.value('$anchorScroll', angular.noop);

    coevery.run(['$rootScope', '$couchPotato', '$stateParams', '$templateCache',
        function ($rootScope, $couchPotato, $stateParams, $templateCache) {

            //"cheating" so that detour is available in requirejs
            //define modules -- we want run-time registration of components
            //to take place within those modules because it allows
            //for them to have their own dependencies also be lazy-loaded.
            //this is what requirejs is good at.

            //if not using any dependencies properties in detour states,
            //then this is not necessary
            coevery.detour = $couchPotato;

            //the sample reads from the current $detour.state
            //and $stateParams in its templates
            //that it the only reason this is necessary
            $rootScope.$detour = $couchPotato;
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
                    '<span class="icon-edit" data-ng-click="edit(\'{0}\')" title="Edit"></span>' +
                    '<span class="icon-remove" co-delete-button confirm-message="You really want to delete this row?" ' +
                    'delete-action="delete(\'{0}\')" title="Delete"></span>{2}</section>' +
                    '<div>{1}</div> </div>';
                var viewStyle = cellvalue, defaultStyle='';
                if (!options.colModel.formatoptions) {
                    return template.format(options.rowId, viewStyle, '');
                }
                if (options.colModel.formatoptions.hasView) {
                    viewStyle = '<a class="btn-link" data-ng-click="view(\'' + options.rowId + '\')"> ' +
                        cellvalue + '</a>';
                } 
                if (options.colModel.formatoptions.hasDefault) {
                    defaultStyle = '<span class="icon-tags" data-ng-click="setDefault(\'' + options.rowId + '\')" title="Set Default"></span>';
                }
                return template.format(options.rowId, viewStyle, defaultStyle);
            };

            $rootScope.defaultGridOptions = {
                datatype: "json",
                loadonce: true,
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
                rowNum: 50,
                rowList: [50, 100, 200],
                //loadui: "disable",
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
