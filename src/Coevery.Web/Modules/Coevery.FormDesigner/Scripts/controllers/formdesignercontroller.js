'use strict';
define(['core/app/detourService'], function (detour) {
    detour.registerController([
        'FormDesignerCtrl',
        ['$scope', 'logger', '$state', '$stateParams','$http',
            function ($scope, logger, $state, $stateParams, $http) {
                $scope.exit = function () {
                    $state.transitionTo('EntityDetail.Fields', { Id: $stateParams.EntityName });
                };

                $scope.save = function() {
                    var sections = $('.fd-form').find('.fd-section');
                    var layoutObject = [];
                    sections.each(function() {
                        if ($(this).find('.fd-field').length) {
                            var columnCount = $(this).attr('section-columns'),
                                width = $(this).attr('section-columns-width'),
                                sectionTitle = $(this).attr('section-title'),
                                section = {
                                    SectionColumns: columnCount,
                                    SectionColumnsWidth: width,
                                    SectionTitle: sectionTitle,
                                    Rows: []
                                },
                                rows = $(this).find('.fd-row');

                            layoutObject.push(section);
                            rows.each(function() {
                                var row = {
                                    Columns: [],
                                    IsMerged: $(this).hasClass('merged-row')
                                };
                                section.Rows.push(row);
                                var columns = $(this).find('.fd-column');
                                columns.each(function() {
                                    var column = { };
                                    row.Columns.push(column);
                                    var field = $(this).find('.fd-field');
                                    if (field.length) {
                                        column.Field = {
                                            FieldName: field.attr('field-name'),
                                        };
                                    }
                                });
                            });
                        }
                    });

                    var promise = $http({
                        url: 'api/formdesigner/layout/' + $stateParams.EntityName,
                        method: "POST",
                        data: JSON.stringify(layoutObject),
                        headers: { 'Content-Type': 'application/json' },
                        tracker: 'saveform'
                    }).then(function () {
                        logger.success('Save succeeded.');
                    }, function (reason) {
                        logger.error('Save Failed： ' + reason.data);
                    });
                    return promise;
                };

                
                $scope.saveAndBack = function () {
                    var promise = $scope.save();
                    promise && promise.then(function () {
                        $scope.exit();
                    });
                };
            }]
    ]);
});

setTimeout(function () {
    $('#test').affix({
        offset: {
            top: function () {
                //var height = $(window).height() - 71 - 90;
                //return $('#form-designer').height() > height ? 71 : 1000;
                return $('#form-designer').height() > $('#test').height() ? 71 : 1000;
            },
        }
    });
}, 100);