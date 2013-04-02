(function () {
    'use strict';
    var currentSectionEntry = null;
    var zIndex = 101;

    function setZIndex(jQueryElem) {
        jQueryElem.css("z-index", ++zIndex);
    }

    function getCloneElem(event) {
        var targetElem = $(event.target);
        var cloneElem = $('<p class="alert alert-error"></p>');
        cloneElem.text(targetElem.text());
        cloneElem.height(18);
        cloneElem.width(150);
        return cloneElem;
    }

    var coevery = angular.module('coevery', []);

    coevery.directive('fdToolsSection', function () {
        return {
            template: '<p fd-draggable="div[fd-form]>.entry" class="alert alert-info">Section</p>',
            replace: true,
            link: function (scope, element, attrs) {
            }
        };
    })
        .directive('fdToolsControl', function () {
            return {
                template: '<p fd-draggable class="alert alert-info"></p>',
                replace: true,
                link: function (scope, element, attrs) {
                    switch (attrs.fdToolsControl) {
                        case 'text':
                            element.text('Textbox');
                            attrs.$set('data-key', 'form-text');
                            break;
                        case 'radio':
                            element.text('Radio List');
                            attrs.$set('data-key', 'form-radio');
                            break;
                        case 'checkbox':
                            element.text('Checkbox List');
                            attrs.$set('data-key', 'form-checkbox');
                            break;
                        case 'select':
                            element.text('Dropdown List');
                            attrs.$set('data-key', 'form-select');
                            break;
                        case 'textarea':
                            element.text('Textarea');
                            attrs.$set('data-key', 'form-textarea');
                            break;
                        default:
                            element.text('Textbox');
                            attrs.$set('data-key', 'form-tex');
                            break;
                    }
                }
            };
        })
        .directive('fdForm', function ($compile) {
            return {
                template: '<div><div class="entry"></div></div>',
                replace: true,
                link: function (scope, element, attrs) {
                    var id = newGuid();
                    attrs.$set('id', id);
                    $("#" + id + ">.entry").sortable({
                        items: "div[fd-section]:not(.sort-placeholder)",
                        placeholder: "sort-placeholder",
                        tolerance: "pointer",
                        scroll: false,
                        beforeStop: function (event, ui) {
                            if (ui.item.is('p')) {
                                var newItem = $('<div fd-section colum-count="2"></div>');
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
                template: '<div class="row-fluid"><section class="span12 widget"><header class="widget-header"><span class="title">&nbsp;</span></header><section class="widget-content form-container"><form fd-container-field class="form-horizontal entry"></form></section></section></div>',
                replace: true,
                link: function (scope, element, attrs) {
                    var id = newGuid();
                    attrs.$set('id', id);

                    $('#' + id + " header:first").dblclick(
                        function () {
                            var rowElem = $('#' + id + " form:first>.row-fluid");
                            var colums = rowElem.children();
                            if (colums.length == 1) {
                                $(colums[0]).removeClass('span12');
                                $(colums[0]).addClass('span6');
                                var newColum = $('<div fd-container-field class="span6 entry"></div>');
                                $compile(newColum)(scope);
                                rowElem.append(newColum);
                            } else {
                                var colum = $(colums[0]);
                                $(colums[1]).children().each(function () {
                                    colum.append(this);
                                });
                                $(colums[1]).remove();
                                $(colums[0]).removeClass('span6');
                                $(colums[0]).addClass('span12');
                            }
                        }
                    );

                    var entry = $('#' + id + " form.entry:first");
                    entry.droppable({
                        accept: '[fd-control], [fd-tools-control]',
                        tolerance: "pointer",
                        drop: function (event, ui) {
                            currentSectionEntry = null;

                            var markedColum = $(this).find('.marked, .markedAbove');
                            var dragItem;
                            if (ui.draggable.is('[fd-tools-control]')) {
                                var key = ui.draggable.data('key');
                                var formKey = key.substr("form-".length);
                                dragItem = $('<div fd-control="' + formKey + '"></div>');
                                $compile(dragItem)(scope);
                            } else {
                                dragItem = ui.draggable;
                            }

                            if (markedColum.children('[fd-control]').attr('id') == dragItem.attr('id')) {
                                return;
                            }
                            var dragField = $('[fd-form]:first').find('.dragging');
                            var dragColum = dragField.parent();
                            var markedRow = markedColum.parent();
                            var columIndex = markedRow.children().index(markedColum);
                            var colums = $(this).find('[fd-row]>[fd-colum]:nth-child(' + (columIndex + 1) + ')');

                            var filledColums = colums.has('[fd-control]:not(.dragging)');
                            if (filledColums.length == colums.length) {
                                var newRow = $('<div fd-row="' + attrs.columCount + '"></div>');
                                $(this).append(newRow);
                                $compile(newRow)(scope);
                                colums.splice(colums.length, 0, newRow.children()[columIndex]);
                            }
                            var fields = filledColums.children('[fd-control]');
                            var insertIndex = markedColum.is('.marked') ?
                                filledColums.index(markedColum) + 1 :
                                filledColums.index(markedColum);

                            fields.splice(insertIndex, 0, dragItem[0]);
                            for (var i = 0; i < fields.length; i++) {
                                $(colums[i]).append(fields[i]);
                            }

                            if (dragColum.length && colums.index(dragColum) == -1) {
                                var dragRow = dragColum.parent();
                                var dragColumIndex = dragRow.children().index(dragColum);
                                var dragColums = dragRow.nextAll().find('[fd-colum]:nth-child(' + (dragColumIndex + 1) + ')');
                                dragColums.splice(0, 0, dragColum[0]);
                                for (var j = 0; j < dragColums.length - 1; j++) {
                                    $(dragColums[j]).append($(dragColums[j + 1]).children());
                                }
                                var dragLastRow = dragColums.last().parent();
                                if (dragRow.siblings().length && dragLastRow.find('[fd-control]').length == 0) {
                                    dragLastRow.remove();
                                }
                            }
                        },
                        over: function (event, ui) {
                            currentSectionEntry = $(this);
                        },
                        out: function (event, ui) {
                            currentSectionEntry = null;
                        }
                    });

                    var emptyRow = $('<div fd-row="' + attrs.columCount + '"></div>');
                    entry.append(emptyRow);
                    $compile(emptyRow)(scope);
                }
            };
        })
        .directive('fdRow', function ($compile) {
            return {
                template: '<div class="row-fluid"></div>',
                replace: true,
                link: function (scope, element, attrs) {
                    var id = newGuid();
                    attrs.$set('id', id);
                    var columCount = parseInt(attrs.fdRow);
                    for (var i = 0; i < columCount; i++) {
                        var newCell = $('<div fd-colum></div>');
                        element.append(newCell);
                        $compile(newCell)(scope);
                    }
                }
            };
        })
        .directive('fdColum', function ($compile) {
            return {
                link: function (scope, element, attrs) {
                    var id = newGuid();
                    attrs.$set('id', id);

                    var columCount = parseInt(element.parent().attr('fd-row'));
                    var width = 12 / columCount;
                    element.addClass('span' + width);
                }
            };
        })
        .directive('fdControl', function ($compile) {
            return {
                template: '<div fd-draggable fd-hoverable class="control-group"><label class="form-label span3">title</label><div class="controls-row span9"></div></div>',
                replace: true,
                link: function (scope, element, attrs) {
                    var id = newGuid();
                    attrs.$set('id', id);
                    var formKey = attrs.fdControl;
                    var newItem;
                    switch (formKey) {
                        case "text":
                            newItem = $('<input type="text" class="span12"/>');
                            break;
                        case "radio":
                            newItem = $('<label class="radio span6"><input type="radio" name="radioOptions" checked/>option1</label><label class="radio span6"><input type="radio" name="radioOptions"/>option2</label>');
                            break;
                        case "checkbox":
                            newItem = $('<label class="checkbox span12"><input type="checkbox" />option1</label>');
                            break;
                        case "select":
                            newItem = $('<select class="span12"><option>option1</option><option>option2</option><option>option3</option></select>');
                            break;
                        case "textarea":
                            newItem = $('<textarea class="span12"></textarea>');
                            break;
                        default:
                            newItem = $('<input type="text" class="span12"/>');
                            break;
                    }
                    element.children('.controls-row').append(newItem);
                }
            };
        })
        .directive('fdContainerField', function ($compile) {
            return {
                link: function (scope, element, attrs) {
                    var parentRow;
                }
            };
        })
        .directive('fdDraggable', function () {
            var lastColum = null;
            
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
                if (lastColum) {
                    lastColum.removeClass('markedAbove');
                    lastColum.removeClass('marked');
                    lastColum = null;
                }
            }

            function setMark(above) {
                if (above) {
                    lastColum.removeClass('marked');
                    lastColum.addClass('markedAbove');
                } else {
                    lastColum.removeClass('markedAbove');
                    lastColum.addClass('marked');
                }
            }

            return {
                link: function (scope, element, attrs) {
                    var connectTo = attrs.fdDraggable;

                    element.css('cursor', 'move');
                    element.draggable({
                        revert: "invalid",
                        connectToSortable: connectTo,
                        helper: getCloneElem,
                        cursorAt: { left: -5, top: -5 },
                        tolerance: "pointer",
                        scroll: false,
                        start: function (event, ui) {
                            setZIndex(ui.helper);
                            $(event.target).css('opacity', '0.3');
                            $(event.target).addClass('dragging');
                        },
                        stop: function (event, ui) {
                            clearMark();
                            $(event.target).css('opacity', '1');
                            $(event.target).removeClass('dragging');
                        },
                        drag: function (event, ui) {
                            clearMark();
                            if (currentSectionEntry) {
                                setDragEffect(ui.helper, true);
                                var rows = currentSectionEntry.children('div[fd-row]');
                                var columCount = parseInt(currentSectionEntry.parents('[fd-section]:first').attr('colum-count'));
                                var columWidth = rows.width() / columCount;
                                var columIndex = Math.floor((event.clientX - rows.offset().left) / columWidth);
                                var rowIndex = Math.floor((event.clientY - rows.offset().top) / rows.height());
                                var colums = rows.find('[fd-colum]:nth-child(' + (columIndex + 1) + ')');
                                var above;
                                if ($(colums[rowIndex]).children().length) {
                                    var currentColum = $(colums[rowIndex]);
                                    var isPrev = event.clientY < currentColum.offset().top + rows.height() / 2;
                                    above = isPrev && rowIndex == 0;
                                    var index = isPrev && rowIndex ? rowIndex - 1 : rowIndex;
                                    lastColum = $(colums[index]);
                                } else {
                                    var filledColums = colums.has('[fd-control]');
                                    above = !filledColums.length;
                                    lastColum = filledColums.length ? filledColums.last() : colums.first();
                                }
                                setMark(above);
                            } else {
                                setDragEffect(ui.helper, false);
                            }
                        }
                    });
                }
            };
        })
        .directive('fdHoverable', function () {
            return {
                link: function (scope, element, attrs) {
                    element.hover(
                        function () {
                            $(this).addClass('highlight');
                        },
                        function () {
                            $(this).removeClass('highlight');
                        }
                    );
                }
            };
        });
})();