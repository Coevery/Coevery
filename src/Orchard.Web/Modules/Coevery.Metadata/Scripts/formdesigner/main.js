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
                        cursor: 'move',
                        tolerance: "pointer",
                        scroll: false,
                        beforeStop: function (event, ui) {
                            if (ui.item.is('p')) {
                                var newItem = $('<div fd-section></div>');
                                $compile(newItem)(scope);
                                ui.item.replaceWith(newItem);
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
                    //element.droppable({
                    //    accept: '[fd-control],[fd-tools-control]',
                    //    hoverClass: 'drop-highlight',
                    //    tolerance: "pointer",
                    //});
                }
            };
        })
        .directive('fdCell', function ($compile) {
            return {
                link: function (scope, element, attrs) {
                    var id = newGuid();
                    attrs.$set('id', id);

                }
            };
        })
        .directive('fdControl', function ($compile) {
            return {
                template: '<div fd-hoverable class="control-group"><label class="form-label span3">title</label><div class="controls-row span9"></div></div>',
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

                    element.css('cursor', 'move');
                }
            };
        })
        .directive('fdContainerField', function ($compile) {
            return {
                link: function (scope, element, attrs) {
                    var parentRow;

                    element.sortable({
                        items: "div[fd-control]:not(.sort-placeholder)",
                        placeholder: "sort-placeholder",
                        connectWith: "div[fd-section] form.entry",
                        cursor: 'move',
                        tolerance: "pointer",
                        helper: getCloneElem,
                        scroll: false,
                        stop: function (event, ui) {

                            function moveField(field, row) {
                                if (field.parent().is('[fd-row]')) {
                                    var index = field.parent().children().index(field);
                                    switch (index) {
                                        case 0:
                                            field.parent().before(row);
                                            break;
                                        case 1:
                                            field.parent().after(row);
                                            break;
                                        default:
                                            break;
                                    }
                                } else {
                                    field.after(row);
                                }
                                row.append(field);
                            }

                            if (ui.item.is('[fd-tools-control]')) {
                                var key = ui.item.data('key');
                                var formKey = key.substr("form-".length);
                                var newItem = $('<div fd-control="' + formKey + '"></div>');
                                $compile(newItem)(scope);
                                ui.item.replaceWith(newItem);
                                var newRow = $('<div fd-row></div>');
                                $compile(newRow)(scope);

                                moveField(newItem, newRow);
                            } else {
                                if (ui.item.parent().attr('id') != parentRow.attr('id')) {
                                    moveField(ui.item, parentRow);
                                }

                                ui.item.fadeTo('normal', 1);
                            }
                        },
                        start: function (event, ui) {
                            if (ui.item.is('[fd-control]')) {
                                parentRow = ui.item.parent();
                                ui.item.css('opacity', '0.3');
                                ui.item.show();
                            }
                        }
                    });
                }
            };
        })
        .directive('fdDraggable', function () {
            return {
                link: function (scope, element, attrs) {
                    var connectTo = attrs.fdDraggable;
                    element.css('cursor', 'move');
                    element.draggable({
                        revert: "invalid",
                        connectToSortable: connectTo,
                        helper: getCloneElem,
                        cursor: 'move',
                        tolerance: "pointer",
                        scroll: false,
                        start: function (event, ui) {
                            setZIndex(ui.helper);
                        },
                        stop: function (event, ui) {

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