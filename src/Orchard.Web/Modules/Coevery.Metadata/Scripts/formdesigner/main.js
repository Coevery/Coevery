(function () {
    'use strict';

    function adjustColumnHeight(rows) {
        for (var i = 0; i < rows.length; i++) {
            var row = $(rows[i]);
            var columns = row.children('[fd-column]');
            var maxColumnHeight = 0;
            var maxFieldHeight = 0;
            for (var j = 0; j < columns.length; j++) {
                columns[j].style.removeProperty('height');
                var column = $(columns[j]);
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
        adjustColumnHeight(rows);
    }

    var coevery = angular.module('coevery', []);

    coevery.directive('fdToolsSection', function () {
        return {
            template: '<p fd-tools-section fd-draggable="[fd-form]>.entry" class="alert alert-info"><span class="title"></span></p>',
            replace: true,
            restrict: 'E',
            link: function (scope, element, attrs) {
                var columnCount = attrs.sectionColumns;
                var titleElem = element.children();
                titleElem.text('Section' + columnCount);
            }
        };
    })
        .directive('fdToolsControl', function () {
            return {
                template: '<p fd-tools-control fd-draggable class="alert alert-info"><span class="title"></span></p>',
                replace: true,
                restrict: 'E',
                link: function (scope, element, attrs) {
                    var titleElem = element.children();
                    switch (attrs.fieldType) {
                        case 'text':
                            titleElem.text('Textbox');
                            break;
                        case 'radio':
                            titleElem.text('Radio List');
                            break;
                        case 'checkbox':
                            titleElem.text('Checkbox List');
                            break;
                        case 'select':
                            titleElem.text('Dropdown List');
                            break;
                        case 'textarea':
                            titleElem.text('Textarea');
                            break;
                        default:
                            titleElem.text('Textbox');
                            break;
                    }
                }
            };
        })
        .directive('fdForm', function ($compile) {
            return {
                template: '<div fd-form><div class="entry"></div></div>',
                replace: true,
                restrict: 'E',
                link: function (scope, element, attrs) {
                    var id = newGuid();
                    attrs.$set('id', id);
                    $('#' + id + '>.entry').sortable({
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
                template: '<div fd-section class="row-fluid"><section class="span12 widget"><header class="widget-header light"><span class="title">&nbsp;</span></header><section class="widget-content form-container"><form fd-field-container class="form-horizontal entry"></form></section></section></div>',
                replace: true,
                restrict: 'E',
                link: function (scope, element, attrs) {
                    var id = newGuid();
                    attrs.$set('id', id);

                    $('#' + id + ' header:first').dblclick(function () {
                        var container = $('#' + id + ' [fd-field-container]:first');
                        var rows = container.children('[fd-row]');
                        var columnCount = rows.first().children('[fd-column]').length;

                        if (columnCount == 1) {
                            $('#' + id).attr('section-columns', 2);
                            var columns = rows.children('[fd-column]');
                            columns.each(function () {
                                var item = $(this);
                                item.removeClass('span12');
                                item.addClass('span6');
                            });
                            rows.each(function () {
                                var row = $(this);
                                row.attr('row-columns', 2);
                                var newColumn = $('<fd-column></fd-column>');
                                row.append(newColumn);
                                $compile(newColumn)(scope);
                            });
                        } else {
                            $('#' + id).attr('section-columns', 1);
                            var rightColumns = rows.children('[fd-column]:nth-child(2)');
                            var rightFields = rightColumns.find('[fd-field]');
                            var leftColumns = rows.children('[fd-column]:nth-child(1)');
                            var leftEmptyColumns = leftColumns.filter(':not(:has([fd-field]))');
                            rightFields.each(function (i) {
                                if (i < leftEmptyColumns.length) {
                                    $(leftEmptyColumns[i]).append(this);
                                } else {
                                    var newRow = $('<fd-row row-columns="1"></fd-row>');
                                    container.append(newRow);
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

                        adjustColumnHeight(container.children('[fd-row]'));
                    });

                    var emptyRow = $('<fd-row></fd-row>');
                    emptyRow.attr('row-columns', element.attr('section-columns'));
                    $('#' + id + ' [fd-field-container]:first').append(emptyRow);
                    $compile(emptyRow)(scope);
                }
            };
        })
        .directive('fdRow', function ($compile) {
            return {
                template: '<div fd-row class="row-fluid"></div>',
                replace: true,
                restrict: 'E',
                link: function (scope, element, attrs) {
                    var id = newGuid();
                    attrs.$set('id', id);
                    var columnCount = parseInt(attrs.rowColumns);
                    for (var i = 0; i < columnCount; i++) {
                        var newColumn = $('<fd-column></fd-column>');
                        element.append(newColumn);
                        $compile(newColumn)(scope);
                    }
                }
            };
        })
        .directive('fdColumn', function ($compile) {
            return {
                template: '<div fd-column></div>',
                replace: true,
                restrict: 'E',
                link: function (scope, element, attrs) {
                    var id = newGuid();
                    attrs.$set('id', id);

                    var columnCount = parseInt(element.parent().attr('row-columns'));
                    var width = 12 / columnCount;
                    element.addClass('span' + width);
                }
            };
        })
        .directive('fdField', function ($compile) {
            return {
                template: '<div fd-field fd-hoverable fd-draggable class="control-group"><label class="form-label title span3">title</label><div class="controls-row span9"></div></div>',
                replace: true,
                restrict: 'E',
                link: function (scope, element, attrs) {
                    var id = newGuid();
                    attrs.$set('id', id);
                    var type = attrs.fieldType;
                    var newItem;
                    switch (type) {
                        case 'text':
                            newItem = $('<input type="text" class="span9"/><div class="span3 tools"></div>');
                            break;
                        case 'radio':
                            newItem = $('<label class="radio span4"><input type="radio" name="radioOptions" checked/>option1</label><label class="radio span4"><input type="radio" name="radioOptions"/>option2</label><div class="span4 tools"></div>');
                            break;
                        case 'checkbox':
                            newItem = $('<label class="checkbox span9"><input type="checkbox" />option1</label><div class="span3 tools"></div>');
                            break;
                        case 'select':
                            newItem = $('<select class="span9"><option>option1</option><option>option2</option><option>option3</option></select><div class="span3 tools"></div>');
                            break;
                        case 'textarea':
                            newItem = $('<textarea class="span9"></textarea><div class="span3 tools"></div>');
                            break;
                        default:
                            newItem = $('<input type="text" class="span9"/><div class="span3 tools"></div>');
                            break;
                    }
                    element.children('.controls-row').append(newItem);
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
                        accept: '[fd-field], [fd-tools-control]',
                        tolerance: "pointer",
                        drop: function (event, ui) {
                            scope.dragHandler = null;

                            var markedColumn = $(this).find('.marked, .markedAbove');
                            var dragItem;
                            if (ui.draggable.is('[fd-tools-control]')) {
                                var type = ui.draggable.attr('field-type');
                                dragItem = $('<fd-field field-always-on-layout field-required></fd-field>');
                                dragItem.attr('field-type', type);
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
                                var newRow = $('<fd-row></fd-row>');
                                newRow.attr('row-columns', $(this).parents('[fd-section]:first').attr('section-columns'));
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

                            adjustColumnHeight($(this).children('[fd-row]'));
                            clearMark();
                        },
                        over: function (event, ui) {
                            setDragEffect(ui.helper, true);
                            scope.dragHandler = function (dragEvent, dragUi) {
                                markColumn({
                                    x: dragEvent.pageX,
                                    y: dragEvent.pageY
                                });
                            };
                            scope.dragHandler(event, ui);
                        },
                        out: function (event, ui) {
                            scope.dragHandler = null;
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
                            scope.enableHover = false;
                            setZIndex(ui.helper);
                            $(event.target).css('opacity', '0.3');
                            $(event.target).addClass('dragging');
                        },
                        stop: function (event, ui) {
                            scope.enableHover = true;
                            $(event.target).css('opacity', '1');
                            $(event.target).removeClass('dragging');
                        },
                        drag: function (event, ui) {
                            if (scope.dragHandler) {
                                scope.dragHandler(event, ui);
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
                    element.hover(
                        function () {
                            if (scope.enableHover) {
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
        .directive('fdSaveLayoutButton', function () {
            return {
                template: '<button class="btn">Save</button>',
                replace: true,
                restrict: 'E',
                link: function (scope, element, attrs) {
                    element.click(function () {
                        var layout = '<?xml version="1.0"?>';
                        layout += '<Form xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">';
                        var sections = $('[fd-form]').find('[fd-section]');
                        sections.each(function () {
                            if ($(this).find('[fd-field]').length) {
                                layout += '<Section columns="' + $(this).attr('section-columns') + '">';
                                var rows = $(this).find('[fd-row]');
                                rows.each(function () {
                                    layout += '<Row>';
                                    var columns = $(this).find('[fd-column]');
                                    columns.each(function () {
                                        layout += '<Column>';
                                        var field = $(this).find('[fd-field]');
                                        if (field.length) {
                                            layout += '<Field type="' + field.attr('field-type') + '"></Field>';
                                        }
                                        layout += '</Column>';
                                    });
                                    layout += '</Row>';
                                });
                                layout += '</Section>';
                            }
                        });
                        layout += '</Form>';
                        console.log(layout);
                    });
                }
            };
        });
})();