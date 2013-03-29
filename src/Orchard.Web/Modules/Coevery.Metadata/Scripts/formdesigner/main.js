'use strict';

//var remove = function (closeAnimation) {
//    this.parent.controls.Remove(this);
//    if (closeAnimation) {
//        $("#" + this.id).remove();
//    } else {
//        $("#" + this.id).slideUp("fast", function () {
//            $(this).remove();
//        });
//    }
//};

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
        //.directive('fdToolsRow', function () {
        //    return {
        //        template: '<p fd-draggable="[fd-tab] form.entry" class="alert alert-info"></p>',
        //        replace: true,
        //        link: function (scope, element, attrs) {
        //            switch (attrs.fdToolsRow) {
        //                case '1':
        //                    element.text('One Columns');
        //                    attrs.$set('data-colum', '1');
        //                    break;
        //                case '2':
        //                    element.text('Two Columns');
        //                    attrs.$set('data-colum', '2');
        //                    break;
        //                default:
        //                    element.text('One Columns');
        //                    attrs.$set('data-colum', '1');
        //                    break;
        //            }
        //        }
        //    };
        //})
        .directive('fdToolsControl', function () {
            return {
                template: '<p fd-draggable="div[fd-section] form>.row-fluid>.entry" class="alert alert-info"></p>',
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
                template: '<div class="row-fluid"><section class="span12 widget"><header class="widget-header"><span class="title">&nbsp;</span></header><section class="widget-content form-container"><form class="form-horizontal"><div class="row-fluid"><div fd-container-field class="span12 entry"></div></div></form></section></section></div>',
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
                                rowElem.append(newColum);
                                $compile(newColum)(scope);
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
        //.directive('fdRow', function ($compile) {
        //    return {
        //        template: '<div class="row-fluid"><div class="entry"></div></div>',
        //        replace: true,
        //        link: function (scope, element, attrs) {
        //            var id = newGuid();
        //            attrs.$set('id', id);
        //            var colum = parseInt(attrs.fdRow);
        //            var entry = $('#' + attrs.id + '>.entry');
        //            for (var i = 0; i < colum; i++) {
        //                var newItem = $('<div fd-cell="1"></div>');
        //                entry.append(newItem);
        //                $compile(newItem)(scope);
        //            }
        //        }
        //    };
        //})
        //.directive('fdCell', function ($compile) {
        //    return {
        //        template: '<div fd-hoverable class="cell"><section class="control-group entry"></section></div>',
        //        replace: true,
        //        link: function (scope, element, attrs) {
        //            var id = newGuid();
        //            attrs.$set('id', id);

        //            var cellColum = parseInt(attrs.fdCell);
        //            var row = element.parents('[fd-row]:first');
        //            var rowColum = parseInt(row.attr('fd-row'));
        //            var width = 12 / rowColum * cellColum;
        //            element.addClass('span' + width);
        //            var entry = $('#' + id + '>.entry');

        //            element.droppable({
        //                accept: "[fd-tools-control],[fd-control]",
        //                tolerance: "pointer",
        //                cursor: 'move',
        //                over: function (event, ui) {
        //                    $(this).addClass("drop-highlight");
        //                },
        //                out: function (event, ui) {
        //                    $(this).removeClass("drop-highlight");
        //                },
        //                drop: function (event, ui) {
        //                    $(this).removeClass("drop-highlight");
        //                    if (entry.children().length > 0) {
        //                        ui.helper.animate({
        //                            top: 0,
        //                            left: 0
        //                        }, 500);
        //                        return;
        //                    }

        //                    if (ui.draggable.is('p')) {
        //                        var key = ui.draggable.data('key');
        //                        var formKey = key.substr("form-".length);
        //                        var newItem = $('<div fd-control="' + formKey + '"></div>');
        //                        entry.append(newItem);
        //                        $compile(newItem)(scope);
        //                        return;
        //                    } else {
        //                        entry.append(ui.draggable);
        //                        ui.draggable.css('left', 0);
        //                        ui.draggable.css('top', 0);
        //                    }
        //                }
        //            });
        //        }
        //    };
        //})
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
                    element.sortable({
                        items: "div[fd-control]:not(.sort-placeholder)",
                        placeholder: "sort-placeholder",
                        connectWith: "div[fd-section] form>.row-fluid>.entry",
                        cursor: 'move',
                        tolerance: "pointer",
                        helper: getCloneElem,
                        scroll: false,
                        stop: function (event, ui) {
                            if (ui.item.is('[fd-tools-control]')) {
                                var key = ui.item.data('key');
                                var formKey = key.substr("form-".length);
                                var newItem = $('<div fd-control="' + formKey + '"></div>');
                                ui.item.replaceWith(newItem);
                                $compile(newItem)(scope);
                            } else {
                                ui.item.fadeTo('normal', 1);
                            }
                        },
                        start: function (event, ui) {
                            if (ui.item.is('[fd-control]')) {
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