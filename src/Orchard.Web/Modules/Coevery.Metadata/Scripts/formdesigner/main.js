//$(function () {
    'use strict';

    function LayoutContext($resource) {
        return $resource(
            '/OrchardLocal/api/metadata/layout/:id',
            { id: '@id' },
            { update: { method: 'PUT' } });
    }

    function getHelper() {
        var helper = $('<p class="alert alert-error"></p>');
        var text = $(this).find('.title:first').text();
        helper.text(text);
        helper.height(18);
        helper.width(150);
        return helper;
    }

    function setDragEffect(item, valid) {
        var itemElem = $(item);
        if (valid) {
            itemElem.removeClass('alert-error');
            itemElem.addClass('alert-info');
        } else {
            itemElem.removeClass('alert-info');
            itemElem.addClass('alert-error');
        }
    }
    
    function clearMark(item) {
        var itemElem = $(item);
        itemElem.removeClass('markedAbove');
        itemElem.removeClass('marked');
    }

    function setMark(item, above) {
        var itemElem = $(item);
        if (above) {
            itemElem.removeClass('marked');
            itemElem.addClass('markedAbove');
        } else {
            itemElem.removeClass('markedAbove');
            itemElem.addClass('marked');
        }
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
    coevery.directive('fdToolsSection', function() {
        return {
            template: '<p fd-tools-section fd-draggable class="alert alert-info"><span class="title"></span></p>',
            replace: true,
            restrict: 'E',
            link: function(scope, element, attrs) {
                var columnCount = attrs.sectionColumns;
                var titleElem = element.children();
                titleElem.text(columnCount + '-Column Section');
            }
        };
    })
        .directive('fdToolsField', function ($rootScope) {
            $rootScope.inLayoutFields || ($rootScope.inLayoutFields = []);
            $rootScope.$watch(function(scope) {
                return scope.inLayoutFields.join(',');
            }, function(newValue, oldValue) {
                var currentFields = newValue ? newValue.split(',') : [];
                var oldFields = oldValue ? oldValue.split(',') : [];
                if (newValue.length == oldValue.length) {
                    setFieldsUsed(currentFields);
                    return;
                }
                var differentFields;
                if (currentFields.length > oldFields.length) {
                    differentFields = getDifferentItems(currentFields, oldFields);
                    setFieldsUsed(differentFields);
                } else {
                    differentFields = getDifferentItems(oldFields, currentFields);
                    setFieldsUnused(differentFields);
                }

                function setFieldsUsed(fields) {
                    for (var j = 0; j < fields.length; j++) {
                        var fieldItem = $('[fd-tools-field][field-name=' + fields[j] + ']');
                        fieldItem.css('opacity', '0.3');
                        fieldItem[0].disableDraggable();
                    }
                }

                function setFieldsUnused(fields) {
                    for (var j = 0; j < fields.length; j++) {
                        var fieldItem = $('[fd-tools-field][field-name=' + fields[j] + ']');
                        fieldItem.css('opacity', '1');
                        fieldItem[0].enableDraggable();
                    }
                }

                function getDifferentItems(items, source) {
                    var differentItems = [];
                    for (var j = 0; j < items.length; j++) {
                        $.inArray(items[j], source) == -1 && differentItems.push(items[j]);
                    }
                    return differentItems;
                }
            });
            
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
        .directive('fdForm', function($compile) {
            return {
                template: '<div fd-form class="row-fluid"><section class="span12 widget"><section class="widget-content form-container"><form class="form-horizontal" ng-transclude></form></section></section></div>',
                replace: true,
                restrict: 'E',
                transclude: true,
                link: function(scope, element, attrs) {
                    var id = newGuid();
                    attrs.$set('id', id);

                    var lastMarked = null;
                    element.find('form:first').droppable({
                        accept: '[fd-section], [fd-tools-section]',
                        tolerance: "pointer",
                        drop: function(event, ui) {
                            scope.$root.dragHandler = null;

                            var markedSection = $(this).find('.marked, .markedAbove').closest('[fd-section]');
                            var dragItem;
                            if (ui.draggable.is('[fd-tools-section]')) {
                                dragItem = $('<fd-section></fd-section>');
                                dragItem.attr('section-columns', ui.draggable.attr('section-columns'));
                                $compile(dragItem)(scope);
                            } else {
                                dragItem = ui.draggable;
                            }

                            if (markedSection.attr('id') == dragItem.attr('id')) {
                                clearMark(lastMarked);
                                lastMarked = null;
                                return;
                            }
                            
                            if (!markedSection.length) {
                                $(this).append(dragItem);
                                clearMark(lastMarked);
                                lastMarked = null;
                                return;
                            }

                            if (markedSection.is('.marked')) {
                                markedSection.after(dragItem);
                            } else {
                                markedSection.before(dragItem);
                            }
                            clearMark(lastMarked);
                            lastMarked = null;
                        },
                        over: function(event, ui) {
                            setDragEffect(ui.helper, true);
                            scope.$root.dragHandler = function(dragEvent, dragUi) {
                                markSection({
                                    x: dragEvent.pageX,
                                    y: dragEvent.pageY
                                });
                            };
                            scope.$root.dragHandler(event, ui);
                        },
                        out: function(event, ui) {
                            scope.$root.dragHandler = null;
                            setDragEffect(ui.helper, false);
                            clearMark(lastMarked);
                            lastMarked = null;
                        }
                    });

                    function markSection(position) {
                        var sections = element.find('[fd-section]');
                        var above;

                        if (!sections.length) {
                            lastMarked = element;
                            setMark(lastMarked, true);
                        }

                        for (var i = 0; i < sections.length; i++) {
                            var section = $(sections[i]);
                            var sectionOffsetY = section.offset().top;
                            var sectionHeight = section.height();
                            var result = position.y >= sectionOffsetY && position.y < sectionOffsetY + sectionHeight ?
                                (position.y < sectionOffsetY + sectionHeight / 2 ? true : false) :
                                null;
                            if (result != null) {
                                clearMark(lastMarked);
                                above = result && i == 0;
                                var index = result && i ? i - 1 : i;
                                lastMarked = above ? $(sections[index]).find('legend:first') : sections[index];
                                setMark(lastMarked, above);
                                break;
                            }
                        }
                    }
                }
            };
        })
        .directive('fdSection', function($compile) {
            return {
                template: '<fieldset fd-draggable drag-handle="legend" fd-section><legend class="title">Section Title</legend><div fd-field-container ng-transclude></div></fieldset>',
                replace: true,
                restrict: 'E',
                transclude: true,
                link: function(scope, element, attrs) {
                    var id = newGuid();
                    attrs.$set('id', id);

                    element.find('legend:first').dblclick(function () {
                        var section = $(this).parents('[fd-section]:first');
                        scope.$root.currentSection = {
                            section: section,
                            columns: section.attr('section-columns')
                        };
                        scope.$root.$apply();
                        $('#sectionPropertiesDialog').modal({ backdrop: 'static' });
                    });
                    
                    scope.$root.$watch(function () {
                        return element.attr('section-columns');
                    }, function (newValue, oldValue) {
                        if (newValue == oldValue) {
                            return;
                        }
                        var section = element;
                        var rows = section.find('[fd-row]');
                        var columnCount = parseInt(newValue);
                        
                        if (columnCount == 1) {
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
                        } else {
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
        .directive('fdRow', function($compile) {
            return {
                template: '<div fd-row class="control-group" ng-transclude></div>',
                replace: true,
                restrict: 'E',
                transclude: true,
                link: function(scope, element, attrs) {
                    var id = newGuid();
                    attrs.$set('id', id);
                    adjustRowHeight(element);
                }
            };
        })
        .directive('fdColumn', function($compile) {
            return {
                template: '<div fd-column ng-transclude></div>',
                replace: true,
                restrict: 'E',
                transclude: true,
                link: function(scope, element, attrs) {
                    var id = newGuid();
                    attrs.$set('id', id);

                    var columnCount = parseInt(element.parents('[fd-section]:first').attr('section-columns'));
                    var width = 12 / columnCount;
                    element.addClass('span' + width);
                }
            };
        })
        .directive('fdField', function($compile) {
            return {
                template: '<div fd-field fd-hoverable fd-draggable class="clearfix"></div>',
                replace: true,
                restrict: 'E',
                link: function(scope, element, attrs) {
                    var id = newGuid();
                    attrs.$set('id', id);
                    scope.$root.inLayoutFields.push(attrs.fieldName);

                    var template = $('script[type="text/ng-template"][id="' + attrs.fieldName + '.html"]').text();
                    template = template.replace(/<(input|select|textarea)/ig, '<$1 disabled');
                    element.html(template);
                    var control = element.find('.control');
                    control.removeClass('span12');
                    control.addClass('span9');
                    control.after('<div class="span3 tools"></div>');

                    var propertyItem = $('<fd-field-tool-property></fd-field-tool-property>');
                    element.find('.tools').append(propertyItem);
                    $compile(propertyItem)(scope);
                    element.find('.tools').hide();

                    changeToAlwaysOnLayout(attrs.fieldAlwaysOnLayout != null);
                    changeToRequired(attrs.fieldRequired != null);

                    
                    element[0].removeListener = scope.$root.$watch(function () {
                        return element.attr('field-required');
                    }, function(newValue) {
                        changeToRequired(newValue != null);
                    });

                    element[0].hoverOverHandler = function() {
                        $(this).addClass('highlight');
                        $(this).find('.tools').show();
                    };
                    element[0].hoverOutHandler = function() {
                        $(this).removeClass('highlight');
                        $(this).find('.tools').hide();
                    };
                    
                    function changeToRequired(required) {
                        if (required) {
                            if (!element.find('.required-image').length) {
                                var img = $('<img class="required-image" src="/OrchardLocal/Modules/Coevery.Metadata/Styles/formdesigner/images/required12.gif">');
                                element.find('.title').prepend(img);
                            }
                        } else {
                            element.find('.required-image').remove();
                        }
                    }
                    
                    function changeToAlwaysOnLayout(required) {
                        if (required) {
                            if (!element.find('.layout-image').length) {
                                var img = $('<img class="layout-image" src="/OrchardLocal/Modules/Coevery.Metadata/Styles/formdesigner/images/alwaysdisplay12.gif">');
                                element.find('.title').prepend(img);
                            }
                            element.find('[fd-field-tool-remove]').remove();
                        } else {
                            element.find('.layout-image').remove();
                            
                            var removeItem = $('<fd-field-tool-remove></fd-field-tool-remove>');
                            element.find('.tools').append(removeItem);
                            $compile(removeItem)(scope);
                        }
                    }
                }
            };
        })
        .directive('fdFieldToolRemove', function($compile) {
            return {
                template: '<div fd-field-tool-remove fd-hoverable class="remove"></div>',
                replace: true,
                restrict: 'E',
                link: function(scope, element, attrs) {
                    element[0].hoverOverHandler = function() {
                        $(this).addClass('highlight');
                    };
                    element[0].hoverOutHandler = function() {
                        $(this).removeClass('highlight');
                    };

                    element.click(function() {
                        var column = $(this).parents('[fd-column]:first');
                        var field = $(this).parents('[fd-field]:first');
                        field[0].removeListener && field[0].removeListener();
                        var fieldIndex = $.inArray(field.attr('field-name'), scope.$root.inLayoutFields);
                        scope.$root.inLayoutFields.splice(fieldIndex, 1);
                        scope.$root.$apply();
                        field.remove();
                        moveNextColumns(column);
                    });
                }
            };
        })
        .directive('fdFieldToolProperty', function($compile) {
            return {
                template: '<div fd-field-tool-property fd-hoverable class="property"></div>',
                replace: true,
                restrict: 'E',
                link: function(scope, element, attrs) {
                    element[0].hoverOverHandler = function() {
                        $(this).addClass('highlight');
                    };
                    element[0].hoverOutHandler = function() {
                        $(this).removeClass('highlight');
                    };

                    element.click(function () {
                        var field = $(this).parents('[fd-field]:first');
                        scope.$root.currentField = {
                            field: field,
                            readonly: field.attr('field-readonly') != null,
                            required: field.attr('field-required') != null
                        };
                        scope.$root.$apply();
                        $('#fieldPropertiesDialog').modal({ backdrop: 'static' });
                    });
                }
            };
        })
        .directive('fdFieldPropertiesDialog', function($compile) {
            return {
                template: '<div id="fieldPropertiesDialog" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true"><div class="modal-header"><button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h3 id="myModalLabel">Field Properties</h3></div><div class="modal-body"><p><label class="checkbox"><input type="checkbox" ng-model="currentField.readonly"/><strong>Read-Only</strong></label></p><p><label class="checkbox"><input type="checkbox" ng-model="currentField.required"/><strong>Required</strong></label></p></div><div class="modal-footer"><button class="btn" data-dismiss="modal" aria-hidden="true">Cancel</button><button class="btn btn-primary">Ok</button></div></div>',
                replace: true,
                restrict: 'E',
                link: function (scope, element, attrs) {
                    var rootScope = scope.$root;
                    element.find('.btn-primary').click(function(parameters) {
                        rootScope.currentField.readonly
                            ? rootScope.currentField.field.attr('field-readonly', '')
                            : rootScope.currentField.field.removeAttr('field-readonly');
                        rootScope.currentField.required
                            ? rootScope.currentField.field.attr('field-required', '')
                            : rootScope.currentField.field.removeAttr('field-required');

                        scope.$root.$apply();
                        $('#fieldPropertiesDialog').modal('hide');
                    });
                }
            };
        })
    .directive('fdSectionPropertiesDialog', function ($compile) {
        return {
            template: '<div id="sectionPropertiesDialog" class="modal hide fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true"><div class="modal-header"><button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button><h3 id="myModalLabel">Section Properties</h3></div><div class="modal-body"><p><label class="radio"><input type="radio" ng-model="currentSection.columns" value="1"/><strong>1-Columns</strong></label></p><p><label class="radio"><input type="radio" ng-model="currentSection.columns" value="2"/><strong>2-Columns</strong></label></p></div><div class="modal-footer"><button class="btn" data-dismiss="modal" aria-hidden="true">Cancel</button><button class="btn btn-primary">Ok</button></div></div>',
            replace: true,
            restrict: 'E',
            link: function(scope, element, attrs) {
                var rootScope = scope.$root;
                element.find('.btn-primary').click(function(parameters) {
                    rootScope.currentSection.section.attr('section-columns', rootScope.currentSection.columns);
                    scope.$root.$apply();
                    $('#sectionPropertiesDialog').modal('hide');
                });
            }
        };
    })
        .directive('fdFieldContainer', function($compile) {
            return {
                restrict: 'A',
                link: function(scope, element, attrs) {
                    var lastColumn = null;

                    element.droppable({
                        accept: '[fd-field], [fd-tools-field]',
                        tolerance: "pointer",
                        drop: function(event, ui) {
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
                                scope.$root.$apply();
                            } else {
                                dragItem = ui.draggable;
                            }

                            if (markedColumn.children('[fd-field]').attr('id') == dragItem.attr('id')) {
                                clearMark(lastColumn);
                                lastColumn = null;
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
                            clearMark(lastColumn);
                            lastColumn = null;
                        },
                        over: function(event, ui) {
                            setDragEffect(ui.helper, true);
                            scope.$root.dragHandler = function(dragEvent, dragUi) {
                                markColumn({
                                    x: dragEvent.pageX,
                                    y: dragEvent.pageY
                                });
                            };
                            scope.$root.dragHandler(event, ui);
                        },
                        out: function(event, ui) {
                            scope.$root.dragHandler = null;
                            setDragEffect(ui.helper, false);
                            clearMark(lastColumn);
                            lastColumn = null;
                        }
                    });

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
                                clearMark(lastColumn);
                                if (column.children('[fd-field]').length) {
                                    above = result && i == 0;
                                    var index = result && i ? i - 1 : i;
                                    lastColumn = columns[index];
                                } else {
                                    var filledColumns = columns.has('[fd-field]');
                                    above = !filledColumns.length;
                                    lastColumn = filledColumns.length ? filledColumns.last() : columns.first();
                                }
                                setMark(lastColumn, above);
                                break;
                            }
                        }
                    }
                }
            };
        })
        .directive('fdDraggable', function() {
            var zIndex = 101;

            function setZIndex(jQueryElem) {
                jQueryElem.css('z-index', ++zIndex);
            }

            return {
                restrict: 'A',
                link: function(scope, element, attrs) {
                    element.css('cursor', 'move');
                    element.draggable({
                        revert: "invalid",
                        connectToSortable: attrs.dragConnectToSortable,
                        handle: attrs.dragHandle,
                        helper: getHelper,
                        cursorAt: { left: -5, top: -5 },
                        tolerance: "pointer",
                        //scroll: false,
                        start: function(event, ui) {
                            scope.$root.enableHover = false;
                            setZIndex(ui.helper);
                            $(event.target).addClass('dragging');
                        },
                        stop: function(event, ui) {
                            scope.$root.enableHover = true;
                            $(event.target).removeClass('dragging');
                        },
                        drag: function(event, ui) {
                            if (scope.$root.dragHandler) {
                                scope.$root.dragHandler(event, ui);
                            }
                        }
                    });

                    element[0].disableDraggable = function() {
                        element.css('cursor', 'default');
                        $(this).draggable("disable");
                    };
                    element[0].enableDraggable = function () {
                        element.css('cursor', 'move');
                        $(this).draggable("enable");
                    };
                }
            };
        })
        .directive('fdHoverable', function() {
            return {
                restrict: 'A',
                link: function(scope, element, attrs) {
                    scope.$root.enableHover = true;
                    element.hover(
                        function() {
                            scope.$root.enableHover && this.hoverOverHandler && this.hoverOverHandler();
                        },
                        function() {
                            this.hoverOutHandler && this.hoverOutHandler();
                        }
                    );
                }
            };
        })
        .directive('fdSaveLayoutButton', function($resource, $location) {
            return {
                //template: '<button class="btn">Save</button>',
                //replace: true,
                //restrict: 'E',
                link: function(scope, element, attrs) {
                    var url = $location.absUrl();
                    var moduleId = url.substr(url.lastIndexOf('/') + 1);
                    element.click(function() {
                        var layoutString = '';
                        var sections = $('[fd-form]').find('[fd-section]');
                        sections.each(function() {
                            if ($(this).find('[fd-field]').length) {
                                var columnCount = $(this).attr('section-columns');
                                layoutString += '<fd-section section-columns="' + columnCount + '">';
                                var rows = $(this).find('[fd-row]');
                                rows.each(function() {
                                    layoutString += '<fd-row>';
                                    var columns = $(this).find('[fd-column]');
                                    columns.each(function() {
                                        layoutString += '<fd-column>';
                                        var field = $(this).find('[fd-field]');
                                        if (field.length) {
                                            var settings = '';
                                            field.attr('field-required') != null && (settings += ' field-required');
                                            field.attr('field-readonly') != null && (settings += ' field-readonly');
                                            layoutString += '<fd-field field-name="' + field.attr('field-name') + '"' + settings + '></fd-field>';
                                        }
                                        layoutString += '</fd-column>';
                                    });
                                    layoutString += '</fd-row>';
                                });
                                layoutString += '</fd-section>';
                            }
                        });

                        $.post('/OrchardLocal/api/metadata/layout/' + moduleId, { id: moduleId, layout: layoutString }, function() {
                            console.log('success');
                        });
                    });
                }
            };
        });

    //angular.bootstrap($('[ng-app]'), ['coevery.layout']);
    //angular.bootstrap($('fd-form'), ['coevery.layout']);
//});


//@ sourceURL=Coevery.Metadata/main.js