'use strict';

//Row.prototype.setHeight = function () {
//    var cells = this.controls;
//    var maxHeight = 0;
//    cells.Each(function (i, item) {
//        var itemElem = $("#" + item.id);
//        itemElem[0].style.removeProperty("height");
//        if (item.controls.length > 0) {
//            var tempHeight = itemElem.height();
//            maxHeight = tempHeight > maxHeight ? tempHeight : maxHeight;
//        }
//    });
//    cells.Each(function (i, item) {
//        $("#" + item.id).height(maxHeight);
//    });
//};

(function () {
    var currentSectionEntry = null;

    var zIndex = 101;

    function setZIndex(jQueryElem) {
        jQueryElem.css("z-index", ++zIndex);
    }

    function getCloneElem(event) {
        var targetElem = $(event.target);
        var cloneElem = $('<p class="alert alert-info"></p>');
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
                template: '<p fd-draggable="div[fd-section] form.entry" class="alert alert-info"></p>',
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
                        //cursor: 'move',
                        tolerance: "pointer",
                        scroll: false,
                        beforeStop: function (event, ui) {
                            if (ui.item.is('p')) {
                                var newItem = $('<div fd-section></div>');
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

                    $('#' + id + " form.entry:first").droppable({
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
                                var newRow = $('<div fd-row="1"></div>');
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

                    var newRow = $('<div fd-row="1"></div>');
                    $('#' + id + " form.entry:first").append(newRow);
                    $compile(newRow)(scope);
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

                    //element.sortable({
                    //    items: "div[fd-control]:not(.sort-placeholder)",
                    //    placeholder: "sort-placeholder",
                    //    connectWith: "div[fd-section] form.entry",
                    //    cursor: 'move',
                    //    tolerance: "pointer",
                    //    helper: getCloneElem,
                    //    scroll: false,
                    //    stop: function (event, ui) {

                    //        function moveField(field, row) {
                    //            if (field.parent().is('[fd-row]')) {
                    //                var index = field.parent().children().index(field);
                    //                switch (index) {
                    //                    case 0:
                    //                        field.parent().before(row);
                    //                        break;
                    //                    case 1:
                    //                        field.parent().after(row);
                    //                        break;
                    //                    default:
                    //                        break;
                    //                }
                    //            } else {
                    //                field.after(row);
                    //            }
                    //            row.append(field);
                    //        }

                    //        if (ui.item.is('[fd-tools-control]')) {
                    //            var key = ui.item.data('key');
                    //            var formKey = key.substr("form-".length);
                    //            var newItem = $('<div fd-control="' + formKey + '"></div>');
                    //            $compile(newItem)(scope);
                    //            ui.item.replaceWith(newItem);
                    //            var newRow = $('<div fd-row></div>');
                    //            $compile(newRow)(scope);

                    //            moveField(newItem, newRow);
                    //        } else {
                    //            if (ui.item.parent().attr('id') != parentRow.attr('id')) {
                    //                moveField(ui.item, parentRow);
                    //            }

                    //            ui.item.fadeTo('normal', 1);
                    //        }
                    //    },
                    //    start: function (event, ui) {
                    //        if (ui.item.is('[fd-control]')) {
                    //            parentRow = ui.item.parent();
                    //            ui.item.css('opacity', '0.3');
                    //            ui.item.show();
                    //        }
                    //    }
                    //});
                }
            };
        })
        .directive('fdDraggable', function () {
            var lastColum = null;

            function clearMark() {
                if (lastColum != null) {
                    lastColum.removeClass('markedAbove');
                    lastColum.removeClass('marked');
                    lastColum = null;
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
                        //cursor: 'move',
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
                            if (currentSectionEntry != null) {
                                ui.helper.removeClass('alert-error');
                                ui.helper.addClass('alert-info');
                                var rows = currentSectionEntry.children('div[fd-row]');
                                for (var i = 0; i < rows.length; i++) {
                                    var row = $(rows[i]);
                                    var rowOffset = row.offset();
                                    if (rowOffset.top <= event.clientY
                                        && event.clientY <= rowOffset.top + row.height()) {
                                        var above = event.clientY <= rowOffset.top + row.height() / 2 ? true : false;
                                        var colums = row.children('[fd-colum]');

                                        var columWidth = colums.first().width();
                                        var index = Math.floor((event.clientX - rowOffset.left) / columWidth);
                                        lastColum = $(colums[index]);

                                        if (above) {
                                            lastColum.removeClass('marked');
                                            lastColum.addClass('markedAbove');
                                        } else {
                                            lastColum.removeClass('markedAbove');
                                            lastColum.addClass('marked');
                                        }

                                        break;
                                    }
                                }
                            } else {
                                ui.helper.removeClass('alert-info');
                                ui.helper.addClass('alert-error');
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