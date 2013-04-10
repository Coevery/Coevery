(function () {
    'use strict';

    function LayoutContext($resource) {
        return $resource(
            '/OrchardLocal/api/metadata/layout/:id',
            { id: '@id' },
            { update: { method: 'PUT' } });
    }

    function adjustRowsHeight(rows) {
        for (var i = 0; i < rows.length; i++) {
            adjustRowHeight(rows[i]);
        }
    }

    function adjustRowHeight(row) {
        var columns = $(row).children('[fd-column]');
        var maxColumnHeight = 0;
        var maxFieldHeight = 0;
        for (var i = 0; i < columns.length; i++) {
            columns[i].style.removeProperty('height');
            var column = $(columns[i]);
            if (column.children('[fd-field]').length) {
                column.children('[fd-field]')[0].style.removeProperty('height');
                var columnHeight = column.height();
                maxColumnHeight = Math.max(maxColumnHeight, columnHeight);
                maxFieldHeight = Math.max(maxFieldHeight, column.children('[fd-field]').height());
            }
        }
        if (maxColumnHeight) {
            columns.each(function () {
                var borderHeight = $(this).is('.marked, .markedAbove') ? 3 : 0;
                $(this).height(maxColumnHeight - borderHeight);
                $(this).children('[fd-field]').height(maxFieldHeight);
            });
        }
    }

    function moveNextColumns(column) {
        var row = column.parent();
        var rows = row.nextAll().andSelf();
        var columnIndex = row.children().index(column);
        var columns = rows.find('[fd-column]:nth-child(' + (columnIndex + 1) + ')');
        for (var i = 0; i < columns.length - 1; i++) {
            $(columns[i]).append($(columns[i + 1]).children());
        }
        var lastRow = columns.last().parent();
        if (row.siblings().length && lastRow.find('[fd-field]').length == 0) {
            lastRow.remove();
        }
        adjustRowsHeight(rows);
    }

    function createNewRow(columnCount) {
        var columnsString = '';
        for (var i = 0; i < columnCount; i++) {
            columnsString += '<fd-column></fd-column>';
        }
        return $('<fd-row>' + columnsString + '</fd-row>');
    }

    var coevery = angular.module('coevery', ['ngResource']);

    coevery.directive('fdToolsSection', function () {
        return {
            template: '<p fd-tools-section fd-draggable="[fd-form]" class="alert alert-info"><span class="title"></span></p>',
            replace: true,
            restrict: 'E',
            link: function (scope, element, attrs) {
                var columnCount = attrs.sectionColumns;
                var titleElem = element.children();
                titleElem.text('Section' + columnCount);
            }
        };
    })
        .directive('fdToolsField', function () {
            return {
                template: '<p fd-tools-field fd-draggable class="alert alert-info"><span class="title"></span></p>',
                replace: true,
                restrict: 'E',
                link: function (scope, element, attrs) {
                    var titleElem = element.children();
                    titleElem.text(attrs.fieldDisplayName);
                }
            };
        })
        .directive('fdForm', function ($compile) {
            return {
                template: '<div fd-form ng-transclude></div>',
                replace: true,
                restrict: 'E',
                transclude: true,
                link: function (scope, element, attrs) {
                    var id = newGuid();
                    attrs.$set('id', id);
                    element.sortable({
                        items: '[fd-section]:not(.sort-placeholder)',
                        placeholder: 'sort-placeholder',
                        tolerance: 'pointer',
                        scroll: false,
                        beforeStop: function (event, ui) {
                            if (ui.item.is('p')) {
                                var newItem = $('<fd-section></fd-section>');
                                newItem.attr('section-columns', ui.item.attr('section-columns'));
                                ui.item.replaceWith(newItem);
                                $compile(newItem)(scope);
                            }
                        }
                    });
                }
            };
        })
        .directive('fdSection', function ($compile) {
            return {
                template: '<div fd-section class="row-fluid"><section class="span12 widget"><header class="widget-header light"><span class="title">&nbsp;</span></header><section class="widget-content form-container"><form fd-field-container class="form-horizontal" ng-transclude></form></section></section></div>',
                replace: true,
                restrict: 'E',
                transclude: true,
                link: function (scope, element, attrs) {
                    var id = newGuid();
                    attrs.$set('id', id);

                    element.find('header:first').dblclick(function () {
                        var section = $(this).parents('[fd-section]');
                        var rows = section.find('[fd-row]');
                        var columnCount = parseInt(section.attr('section-columns'));

                        if (columnCount == 1) {
                            section.attr('section-columns', 2);
                            var columns = rows.children('[fd-column]');
                            columns.each(function () {
                                var item = $(this);
                                item.removeClass('span12');
                                item.addClass('span6');
                            });
                            rows.each(function () {
                                var row = $(this);
                                var newColumn = $('<fd-column></fd-column>');
                                row.append(newColumn);
                                $compile(newColumn)(scope);
                            });
                        } else {
                            section.attr('section-columns', 1);
                            var rightColumns = rows.children('[fd-column]:nth-child(2)');
                            var rightFields = rightColumns.find('[fd-field]');
                            var leftColumns = rows.children('[fd-column]:nth-child(1)');
                            var leftEmptyColumns = leftColumns.filter(':not(:has([fd-field]))');
                            rightFields.each(function (i) {
                                if (i < leftEmptyColumns.length) {
                                    $(leftEmptyColumns[i]).append(this);
                                } else {
                                    var newRow = createNewRow(1);
                                    section.find('[fd-field-container]').append(newRow);
                                    $compile(newRow)(scope);
                                    newRow.children('[fd-column]').append(this);
                                }
                            });
                            leftColumns.each(function () {
                                var item = $(this);
                                item.removeClass('span6');
                                item.addClass('span12');
                            });
                            rightColumns.remove();
                        }

                        adjustRowsHeight(section.find('[fd-row]'));
                    });

                    if (!element.find('[fd-row]').length) {
                        var columnCount = parseInt(attrs.sectionColumns);
                        var emptyRow = createNewRow(columnCount);
                        element.find('[fd-field-container]:first').append(emptyRow);
                        $compile(emptyRow)(scope);
                    }
                }
            };
        })
        .directive('fdRow', function ($compile) {
            return {
                template: '<div fd-row class="row-fluid" ng-transclude></div>',
                replace: true,
                restrict: 'E',
                transclude: true,
                link: function (scope, element, attrs) {
                    var id = newGuid();
                    attrs.$set('id', id);
                    adjustRowHeight(element);
                }
            };
        })
        .directive('fdColumn', function ($compile) {
            return {
                template: '<div fd-column ng-transclude></div>',
                replace: true,
                restrict: 'E',
                transclude: true,
                link: function (scope, element, attrs) {
                    var id = newGuid();
                    attrs.$set('id', id);

                    var columnCount = parseInt(element.parents('[fd-section]:first').attr('section-columns'));
                    var width = 12 / columnCount;
                    element.addClass('span' + width);
                }
            };
        })
        .directive('fdField', function ($compile) {
            return {
                template: '<div fd-field fd-hoverable fd-draggable></div>',
                replace: true,
                restrict: 'E',
                link: function (scope, element, attrs) {
                    var id = newGuid();
                    attrs.$set('id', id);
                    //    case 'select':
                    //        newItem = $('<select class="span9"><option></option><option>option2</option><option>option3</option></select><div class="span3 tools"></div>');
                    //        break;
                    //    case 'textarea':
                    //        newItem = $('<textarea class="span9"></textarea><div class="span3 tools"></div>');
                    //        break;

                    var template = $('script[type="text/ng-template"][id="' + attrs.fieldName + '.html"]');
                    element.html(template.text());
                    var control = element.find('.control');
                    control.removeClass('span12');
                    control.addClass('span9');
                    control.after('<div class="span3 tools"></div>');

                    var propertyItem = $('<fd-field-tool-property></fd-field-tool-property>');
                    element.find('.tools').append(propertyItem);
                    $compile(propertyItem)(scope);
                    element.find('.tools').hide();
                    if (attrs.fieldAlwaysOnLayout == undefined) {
                        var removeItem = $('<fd-field-tool-remove></fd-field-tool-remove>');
                        element.find('.tools').append(removeItem);
                        $compile(removeItem)(scope);
                    } else {
                        var img = $('<img src="/OrchardLocal/Modules/Coevery.Metadata/Styles/formdesigner/images/alwaysdisplay12.gif">');
                        element.find('.title').prepend(img);
                    }
                    if (attrs.fieldRequired != undefined) {
                        var img = $('<img src="/OrchardLocal/Modules/Coevery.Metadata/Styles/formdesigner/images/required12.gif">');
                        element.find('.title').prepend(img);
                    }

                    element[0].hoverOverHandler = function () {
                        $(this).addClass('highlight');
                        $(this).find('.tools').show();
                    };
                    element[0].hoverOutHandler = function () {
                        $(this).removeClass('highlight');
                        $(this).find('.tools').hide();
                    };
                }
            };
        })
        .directive('fdFieldToolRemove', function ($compile) {
            return {
                template: '<div fd-field-tool-remove fd-hoverable class="remove"></div>',
                replace: true,
                restrict: 'E',
                link: function (scope, element, attrs) {
                    element[0].hoverOverHandler = function () {
                        $(this).addClass('highlight');
                    };
                    element[0].hoverOutHandler = function () {
                        $(this).removeClass('highlight');
                    };

                    element.click(function () {
                        var column = $(this).parents('[fd-column]:first');
                        $(this).parents('[fd-field]:first').remove();
                        moveNextColumns(column);
                    });
                }
            };
        })
        .directive('fdFieldToolProperty', function ($compile) {
            return {
                template: '<div fd-field-tool-property fd-hoverable class="property"></div>',
                replace: true,
                restrict: 'E',
                link: function (scope, element, attrs) {
                    element[0].hoverOverHandler = function () {
                        $(this).addClass('highlight');
                    };
                    element[0].hoverOutHandler = function () {
                        $(this).removeClass('highlight');
                    };

                    element.click(function () {

                    });
                }
            };
        })
        .directive('fdFieldContainer', function ($compile) {
            return {
                restrict: 'A',
                link: function (scope, element, attrs) {
                    var lastColumn = null;

                    element.droppable({
                        accept: '[fd-field], [fd-tools-field]',
                        tolerance: "pointer",
                        drop: function (event, ui) {
                            scope.$root.dragHandler = null;

                            var markedColumn = $(this).find('.marked, .markedAbove');
                            var dragItem;
                            if (ui.draggable.is('[fd-tools-field]')) {
                                var required = ui.draggable.attr('required');
                                if (required != null) {
                                    dragItem = $('<fd-field field-required></fd-field>');
                                } else {
                                    dragItem = $('<fd-field></fd-field>');
                                }

                                dragItem.attr('field-name', ui.draggable.attr('field-name'));
                                $compile(dragItem)(scope);
                            } else {
                                dragItem = ui.draggable;
                            }

                            if (markedColumn.children('[fd-field]').attr('id') == dragItem.attr('id')) {
                                clearMark();
                                return;
                            }

                            var dragField = $('[fd-form]:first').find('.dragging');
                            var dragColumn = dragField.parent();
                            var markedRow = markedColumn.parent();
                            var columnIndex = markedRow.children().index(markedColumn);
                            var columns = $(this).find('[fd-row]>[fd-column]:nth-child(' + (columnIndex + 1) + ')');

                            var filledColumns = columns.has('[fd-field]:not(.dragging)');
                            if (filledColumns.length == columns.length) {
                                var columnCount = parseInt($(this).parents('[fd-section]:first').attr('section-columns'));
                                var newRow = createNewRow(columnCount);
                                $(this).append(newRow);
                                $compile(newRow)(scope);
                                columns.splice(columns.length, 0, newRow.children()[columnIndex]);
                            }

                            var fields = filledColumns.children('[fd-field]');
                            var insertIndex = markedColumn.is('.marked') ?
                                filledColumns.index(markedColumn) + 1 :
                                filledColumns.index(markedColumn);

                            fields.splice(insertIndex, 0, dragItem[0]);
                            for (var i = 0; i < fields.length; i++) {
                                $(columns[i]).append(fields[i]);
                            }

                            if (dragColumn.length && columns.index(dragColumn) == -1) {
                                moveNextColumns(dragColumn);
                            }

                            adjustRowsHeight($(this).children('[fd-row]'));
                            clearMark();
                        },
                        over: function (event, ui) {
                            setDragEffect(ui.helper, true);
                            scope.$root.dragHandler = function (dragEvent, dragUi) {
                                markColumn({
                                    x: dragEvent.pageX,
                                    y: dragEvent.pageY
                                });
                            };
                            scope.$root.dragHandler(event, ui);
                        },
                        out: function (event, ui) {
                            scope.$root.dragHandler = null;
                            setDragEffect(ui.helper, false);
                            clearMark();
                        }
                    });

                    function setDragEffect(item, valid) {
                        if (valid) {
                            item.removeClass('alert-error');
                            item.addClass('alert-info');
                        } else {
                            item.removeClass('alert-info');
                            item.addClass('alert-error');
                        }
                    }

                    function clearMark() {
                        if (lastColumn) {
                            lastColumn.removeClass('markedAbove');
                            lastColumn.removeClass('marked');
                            lastColumn = null;
                        }
                    }

                    function setMark(above) {
                        if (above) {
                            lastColumn.removeClass('marked');
                            lastColumn.addClass('markedAbove');
                        } else {
                            lastColumn.removeClass('markedAbove');
                            lastColumn.addClass('marked');
                        }
                    }

                    function markColumn(position) {
                        var rows = element.children('[fd-row]');
                        var columnCount = parseInt(element.parents('[fd-section]:first').attr('section-columns'));
                        var columnWidth = rows.width() / columnCount;
                        var columnIndex = Math.floor((position.x - rows.offset().left) / columnWidth);
                        var columns = rows.find('[fd-column]:nth-child(' + (columnIndex + 1) + ')');
                        var above;

                        for (var i = 0; i < columns.length; i++) {
                            var column = $(columns[i]);
                            var columnOffsetY = column.offset().top;
                            var columnHeight = column.height();
                            var result = position.y >= columnOffsetY && position.y < columnOffsetY + columnHeight ?
                                (position.y < columnOffsetY + columnHeight / 2 ? true : false) :
                                null;
                            if (result != null) {
                                clearMark();
                                if (column.children('[fd-field]').length) {
                                    above = result && i == 0;
                                    var index = result && i ? i - 1 : i;
                                    lastColumn = $(columns[index]);
                                } else {
                                    var filledColumns = columns.has('[fd-field]');
                                    above = !filledColumns.length;
                                    lastColumn = filledColumns.length ? filledColumns.last() : columns.first();
                                }
                                setMark(above);
                                break;
                            }
                        }
                    }
                }
            };
        })
        .directive('fdDraggable', function () {
            var zIndex = 101;

            function setZIndex(jQueryElem) {
                jQueryElem.css('z-index', ++zIndex);
            }

            return {
                restrict: 'A',
                link: function (scope, element, attrs) {
                    var connectTo = attrs.fdDraggable;

                    element.css('cursor', 'move');
                    element.draggable({
                        revert: "invalid",
                        connectToSortable: connectTo,
                        helper: getHelper,
                        cursorAt: { left: -5, top: -5 },
                        tolerance: "pointer",
                        scroll: false,
                        start: function (event, ui) {
                            scope.$root.enableHover = false;
                            setZIndex(ui.helper);
                            $(event.target).css('opacity', '0.3');
                            $(event.target).addClass('dragging');
                        },
                        stop: function (event, ui) {
                            scope.$root.enableHover = true;
                            $(event.target).css('opacity', '1');
                            $(event.target).removeClass('dragging');
                        },
                        drag: function (event, ui) {
                            if (scope.$root.dragHandler) {
                                scope.$root.dragHandler(event, ui);
                            }
                        }
                    });

                    function getHelper() {
                        var helper = $('<p class="alert alert-error"></p>');
                        var text = $(this).find('.title:first').text();
                        helper.text(text);
                        helper.height(18);
                        helper.width(150);
                        return helper;
                    }
                }
            };
        })
        .directive('fdHoverable', function () {
            return {
                restrict: 'A',
                link: function (scope, element, attrs) {
                    scope.$root.enableHover = true;
                    element.hover(
                        function () {
                            if (scope.$root.enableHover) {
                                this.hoverOverHandler();
                            }
                        },
                        function () {
                            this.hoverOutHandler();
                        }
                    );
                }
            };
        })
        .directive('fdSaveLayoutButton', function ($resource, $location) {
            return {
                template: '<button class="btn">Save</button>',
                replace: true,
                restrict: 'E',
                link: function (scope, element, attrs) {
                    var url = $location.absUrl();
                    var moduleId = url.substr(url.lastIndexOf('/') + 1);
                    var Layout = LayoutContext($resource);
                    element.click(function () {
                        var layoutString = '';
                        var sections = $('[fd-form]').find('[fd-section]');
                        sections.each(function () {
                            if ($(this).find('[fd-field]').length) {
                                var columnCount = $(this).attr('section-columns');
                                layoutString += '<fd-section section-columns="' + columnCount + '">';
                                var rows = $(this).find('[fd-row]');
                                rows.each(function () {
                                    layoutString += '<fd-row>';
                                    var columns = $(this).find('[fd-column]');
                                    columns.each(function () {
                                        layoutString += '<fd-column>';
                                        var field = $(this).find('[fd-field]');
                                        if (field.length) {
                                            layoutString += '<fd-field field-name="' + field.attr('field-name') + '"></fd-field>';
                                        }
                                        layoutString += '</fd-column>';
                                    });
                                    layoutString += '</fd-row>';
                                });
                                layoutString += '</fd-section>';
                            }
                        });

                        Layout.save({ id: moduleId, layout: layoutString }, function () {
                            console.log('success');
                        }, function () {
                            console.log('failed');
                        });
                    });
                }
            };
        });
})();