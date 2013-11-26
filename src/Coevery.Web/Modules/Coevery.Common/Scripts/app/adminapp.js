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

            if (!String.prototype.format) {
                String.prototype.format = function () {
                    var args = arguments;
                    return this.replace(/\{(\d+)\}/g, function (match, number) {
                        return typeof args[number] !== 'undefined'
                          ? args[number]
                          : match
                        ;
                    });
                };
            }

            $rootScope.cellLinkTemplate = function (cellvalue, options, rowObject) {
                var template = '<div class=\"gridCellText\">{0}<div>{1}</div></div>';
                var cellbuttonTemplate = '<section class=\"row-actions hide\">' +
                        '<span class=\"icon-edit\" data-ng-click=\"edit(\'{0}\')\" title=\"Edit\"></span>' +
                        '<span class=\"icon-remove\" co-delete-button confirm-message=\"You really want to delete this row?\" ' +
                        'delete-action=\"delete(\'{0}\')\" title=\"Delete\"></span>{1}</section>';
                var viewStyle = cellvalue, param = [options.rowId], defaultStyle = '',
                    foramtOptions = options.colModel.formatoptions, result;
                if (!foramtOptions) {
                    result = template.format(cellbuttonTemplate.format(options.rowId, ''), viewStyle);
                    return result;
                }
                if (foramtOptions.hasView) {
                    viewStyle = '<a class=\"btn-link\" data-ng-click=\"view(\'{0}\')\"> ' +
                        cellvalue + '</a>';
                }
                //'paramAttrs' define the param attributes passed to the edit and delete function
                if (!!foramtOptions.paramAttrs) {
                    param = [];
                    for (var index = 0; index < foramtOptions.paramAttrs.length; index++) {
                        param.push(rowObject[foramtOptions.paramAttrs[index]]);
                    }
                }
                if (!!foramtOptions.hasDefault) {
                    defaultStyle = '<span class=\"icon-tags\" data-ng-click=\"setDefault(\'' + options.rowId + '\')\" title=\"Set Default\"></span>';
                }
                result = !!foramtOptions.noCellButton
                    ? template.format('', viewStyle.format(param.join('\',\'')))
                    : template.format(cellbuttonTemplate.format(param.join('\'\,\''), defaultStyle), viewStyle.format(param.join('\'\,\'')));
                return result;
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
