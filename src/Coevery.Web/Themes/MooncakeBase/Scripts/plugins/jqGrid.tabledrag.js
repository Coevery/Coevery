/**
 * @file
 * jQuery Tabledrag plugin. Drag and drop table rows with field manipulation.
 * Most code is copied verbatim from Drupal core, misc/tabledrag.js.
 * @license GPL V2 
 * @author Wouter Admiraal <wadmiraal@connect-i.ch>
 */

; (function ($, undefined) {

    // Use the Drupal namespace to facilitate the fork.
    var Drupal = {};
    function getColumnSelector(tabeId, columnName) {
        return '[aria-describedby="' + tabeId + '_' + columnName + '"]';
    }
    
    function columnValueSelector(selector,value) {
        return 'td'+ selector + '[title="'+ (value || '') + '"]';
    }

    function treeDisplay(settings, element) {
        $('tr.' + settings.draggableClass, element).each(function () {
            var row = $(this);
            var field = $(getColumnSelector(settings.tableId, settings.group.columnName), row);
            if (field.length) {
                for (var i = 1, len = parseInt(field.text(), 10) - settings.initialLevel ; i <= len; i++) {
                    $('td:first', row).prepend(Drupal.theme('tableDragIndentation'));
                }
            }
        });
    }
    /**
     * Plugin definition.
     */
    $.fn.tableDrag = function (settings) {
        settings = $.extend({
            tableId: '',
            draggableClass: 'jqgrow',
            initialLevel: 0,
            group: {
                columnName: 'Level',
                depthLimit: 3
            },
            weight: {
                columnName: 'Weight',
            },
            parent: {
                columnName: 'Parent',
                sourceColumnName: 'Id',
            }
        }, settings || {});

        this.each(function () {
            var table = new Drupal.tableDrag(this, settings);

            // Indent each row.
            treeDisplay(settings,this);
        });
    };

    /**
     * Constructor for the tableDrag object. Provides table and field manipulation.
     *
     * @param table
     *   DOM object for the table to be made draggable.
     * @param tableSettings
     *   Settings for the table.
     */
    Drupal.tableDrag = function (table, tableSettings) {
        var self = this;

        // Required object variables.
        this.table = table;
        this.$table = $(table); // Cache the jQuery object.
        this.tableSettings = tableSettings;
        this.dragObject = null; // Used to hold information about a current drag operation.
        this.rowObject = null; // Provides operations for row manipulation.
        this.oldRowElement = null; // Remember the previous element.
        this.oldY = 0; // Used to determine up or down direction from last mouse move.
        this.changed = false; // Whether anything in the entire table has changed.
        this.maxDepth = 0; // Maximum amount of allowed parenting.
        this.rtl = $(this.table).css('direction') == 'rtl' ? -1 : 1; // Direction of the table.

        // Configure the scroll settings.
        this.scrollSettings = { amount: 4, interval: 50, trigger: 70 };
        this.scrollInterval = null;
        this.scrollY = 0;
        this.windowHeight = 0;

        // Check this table's settings to see if there are parent relationships in
        // this table. For efficiency, large sections of code can be skipped if we
        // don't need to track horizontal movement and indentations.
        this.indentEnabled = !!tableSettings.parent.columnName;
        this.maxDepth = this.indentEnabled ? tableSettings.group.depthLimit : this.maxDepth;

        if (this.indentEnabled) {
            this.indentCount = 1; // Total width of indents, set in makeDraggable.
            // Find the width of indentations to measure mouse movements against.
            // Because the table doesn't need to start with any indentations, we
            // manually append 2 indentations in the first draggable row, measure
            // the offset, then remove.
            var indent = Drupal.theme('tableDragIndentation');
            var testRow = $('<tr/>').addClass(this.tableSettings.draggableClass).appendTo(table);
            var testCell = $('<td/>').appendTo(testRow).prepend(indent).prepend(indent);
            this.indentAmount = $('.indentation', testCell).get(1).offsetLeft - $('.indentation', testCell).get(0).offsetLeft;
            testRow.remove();
        }

        // Make each applicable row draggable.
        // Match immediate children of the parent element to allow nesting.
        $('> tr.' + this.tableSettings.draggableClass + ', > tbody > tr.' + this.tableSettings.draggableClass, table).each(function () { self.makeDraggable(this); });

        // Add mouse bindings to the document. The self variable is passed along
        // as event handlers do not have direct access to the tableDrag object.
        $(document).bind('mousemove', function (event) { return self.dragRow(event, self); });
        $(document).bind('mouseup', function (event) { return self.dropRow(event, self); });
    };

    /**
     * Find the target used within a particular row and group.
     */
    Drupal.tableDrag.prototype.rowSettings = function (group, row) {
        if (this.tableSettings[group].columnName !== undefined) {
            var field = $(getColumnSelector(this.tableSettings.tableId, this.tableSettings[group].columnName), row);
            if (field.length) {
                // Return a copy of the row settings.
                var rowSettings = this.tableSettings[group];
                rowSettings.relationship = group == 'group' ? 'group' : (group == 'parent' ? 'parent' : (group == 'weight' ? 'sibling' : 'self'));
                rowSettings.action = group == 'group' ? 'depth' : (group == 'parent' ? 'match' : (group == 'weight' ? 'order' : 'order'));
                return rowSettings;
            }
        }
    };

    /**
     * Take an item and add event handlers to make it become draggable.
     */
    Drupal.tableDrag.prototype.makeDraggable = function (item) {
        var self = this;

        // Create the handle.
        var handle = $('<a href="#" class="tabledrag-handle"><div class="handle">&nbsp;</div></a>').attr('title', Drupal.t('Drag to re-order'));
        // Insert the handle after indentations (if any).
        if ($('td:first .indentation:last', item).length) {
            $('td:first .indentation:last', item).after(handle);
            // Update the total width of indentation in this entire table.
            self.indentCount = Math.max($('.indentation', item).length, self.indentCount);
        }
        else {
            $('td:first', item).prepend(handle);
        }

        // Add hover action for the handle.
        handle.hover(function () {
            self.dragObject == null ? $(this).addClass('tabledrag-handle-hover') : null;
        }, function () {
            self.dragObject == null ? $(this).removeClass('tabledrag-handle-hover') : null;
        });

        // Add the mousedown action for the handle.
        handle.mousedown(function (event) {
            // Create a new dragObject recording the event information.
            self.dragObject = {};
            self.dragObject.initMouseOffset = self.getMouseOffset(item, event);
            self.dragObject.initMouseCoords = self.mouseCoords(event);
            if (self.indentEnabled) {
                self.dragObject.indentMousePos = self.dragObject.initMouseCoords;
            }

            // If there's a lingering row object from the keyboard, remove its focus.
            if (self.rowObject) {
                $('a.tabledrag-handle', self.rowObject.element).blur();
            }

            // Create a new rowObject for manipulation of this row.
            self.rowObject = new self.row(item, 'mouse', self.indentEnabled, self.maxDepth, true, self.tableSettings.draggableClass);

            // Save the position of the table.
            self.table.topY = $(self.table).offset().top;
            self.table.bottomY = self.table.topY + self.table.offsetHeight;

            // Add classes to the handle and row.
            $(this).addClass('tabledrag-handle-hover');
            $(item).addClass('drag');

            // Set the document to use the move cursor during drag.
            $('body').addClass('drag');
            if (self.oldRowElement) {
                $(self.oldRowElement).removeClass('drag-previous');
            }

            // Hack for IE6 that flickers uncontrollably if select lists are moved.
            if (navigator.userAgent.indexOf('MSIE 6.') != -1) {
                $('select', this.table).css('display', 'none');
            }

            // Hack for Konqueror, prevent the blur handler from firing.
            // Konqueror always gives links focus, even after returning false on mousedown.
            self.safeBlur = false;

            // Call optional placeholder function.
            self.onDrag();
            return false;
        });

        // Prevent the anchor tag from jumping us to the top of the page.
        handle.click(function () {
            return false;
        });

        // Similar to the hover event, add a class when the handle is focused.
        handle.focus(function () {
            $(this).addClass('tabledrag-handle-hover');
            self.safeBlur = true;
        });

        // Remove the handle class on blur and fire the same function as a mouseup.
        handle.blur(function (event) {
            $(this).removeClass('tabledrag-handle-hover');
            if (self.rowObject && self.safeBlur) {
                self.dropRow(event, self);
            }
        });

        // Add arrow-key support to the handle.
        handle.keydown(function (event) {
            // If a rowObject doesn't yet exist and this isn't the tab key.
            if (event.keyCode != 9 && !self.rowObject) {
                self.rowObject = new self.row(item, 'keyboard', self.indentEnabled, self.maxDepth, true, self.tableSettings.draggableClass);
            }

            var keyChange = false;
            switch (event.keyCode) {
                case 37: // Left arrow.
                case 63234: // Safari left arrow.
                    keyChange = true;
                    self.rowObject.indent(-1 * self.rtl);
                    break;
                case 38: // Up arrow.
                case 63232: // Safari up arrow.
                    var previousRow = $(self.rowObject.element).prev('tr').not('.jqgfirstrow').get(0);
                    while (previousRow && $(previousRow).is(':hidden')) {
                        previousRow = $(previousRow).prev('tr').get(0);
                    }
                    if (previousRow) {
                        self.safeBlur = false; // Do not allow the onBlur cleanup.
                        self.rowObject.direction = 'up';
                        keyChange = true;

                        if ($(item).is('.tabledrag-root')) {
                            // Swap with the previous top-level row.
                            var groupHeight = 0;
                            while (previousRow && $('.indentation', previousRow).length) {
                                previousRow = $(previousRow).prev('tr').not('.jqgfirstrow').get(0);
                                groupHeight += $(previousRow).is(':hidden') ? 0 : previousRow.offsetHeight;
                            }
                            if (previousRow) {
                                self.rowObject.swap('before', previousRow);
                                // No need to check for indentation, 0 is the only valid one.
                                window.scrollBy(0, -groupHeight);
                            }
                        }
                        else if (self.table.tBodies[0].rows[0] != previousRow || $(previousRow).is('.' + self.tableSettings.draggableClass)) {
                            // Swap with the previous row (unless previous row is the first one
                            // and undraggable).
                            self.rowObject.swap('before', previousRow);
                            self.rowObject.interval = null;
                            self.rowObject.indent(0);
                            window.scrollBy(0, -parseInt(item.offsetHeight, 10));
                        }
                        handle.get(0).focus(); // Regain focus after the DOM manipulation.
                    }
                    break;
                case 39: // Right arrow.
                case 63235: // Safari right arrow.
                    keyChange = true;
                    self.rowObject.indent(1 * self.rtl);
                    break;
                case 40: // Down arrow.
                case 63233: // Safari down arrow.
                    var nextRow = $(self.rowObject.group).filter(':last').next('tr').not('.jqgfirstrow').get(0);
                    while (nextRow && $(nextRow).is(':hidden')) {
                        nextRow = $(nextRow).next('tr').get(0);
                    }
                    if (nextRow) {
                        self.safeBlur = false; // Do not allow the onBlur cleanup.
                        self.rowObject.direction = 'down';
                        keyChange = true;

                        if ($(item).is('.tabledrag-root')) {
                            // Swap with the next group (necessarily a top-level one).
                            var groupHeight = 0;
                            var nextGroup = new self.row(nextRow, 'keyboard', self.indentEnabled, self.maxDepth, false, self.tableSettings.draggableClass);
                            if (nextGroup) {
                                $(nextGroup.group).each(function () {
                                    groupHeight += $(this).is(':hidden') ? 0 : this.offsetHeight;
                                });
                                var nextGroupRow = $(nextGroup.group).filter(':last').get(0);
                                self.rowObject.swap('after', nextGroupRow);
                                // No need to check for indentation, 0 is the only valid one.
                                window.scrollBy(0, parseInt(groupHeight, 10));
                            }
                        }
                        else {
                            // Swap with the next row.
                            self.rowObject.swap('after', nextRow);
                            self.rowObject.interval = null;
                            self.rowObject.indent(0);
                            window.scrollBy(0, parseInt(item.offsetHeight, 10));
                        }
                        handle.get(0).focus(); // Regain focus after the DOM manipulation.
                    }
                    break;
            }

            if (self.rowObject && self.rowObject.changed == true) {
                $(item).addClass('drag');
                if (self.oldRowElement) {
                    $(self.oldRowElement).removeClass('drag-previous');
                }
                self.oldRowElement = item;
                self.restripeTable();
                self.onDrag();
            }

            // Returning false if we have an arrow key to prevent scrolling.
            if (keyChange) {
                return false;
            }
        });

        // Compatibility addition, return false on keypress to prevent unwanted scrolling.
        // IE and Safari will suppress scrolling on keydown, but all other browsers
        // need to return false on keypress. http://www.quirksmode.org/js/keys.html
        handle.keypress(function (event) {
            switch (event.keyCode) {
                case 37: // Left arrow.
                case 38: // Up arrow.
                case 39: // Right arrow.
                case 40: // Down arrow.
                    return false;
            }
        });
    };

    /**
     * Mousemove event handler, bound to document.
     */
    Drupal.tableDrag.prototype.dragRow = function (event, self) {
        if (self.dragObject) {
            self.currentMouseCoords = self.mouseCoords(event);

            var y = self.currentMouseCoords.y - self.dragObject.initMouseOffset.y;
            var x = self.currentMouseCoords.x - self.dragObject.initMouseOffset.x;

            // Check for row swapping and vertical scrolling.
            if (y != self.oldY) {
                self.rowObject.direction = y > self.oldY ? 'down' : 'up';
                self.oldY = y; // Update the old value.

                // Check if the window should be scrolled (and how fast).
                var scrollAmount = self.checkScroll(self.currentMouseCoords.y);
                // Stop any current scrolling.
                clearInterval(self.scrollInterval);
                // Continue scrolling if the mouse has moved in the scroll direction.
                if (scrollAmount > 0 && self.rowObject.direction == 'down' || scrollAmount < 0 && self.rowObject.direction == 'up') {
                    self.setScroll(scrollAmount);
                }

                // If we have a valid target, perform the swap and restripe the table.
                var currentRow = self.findDropTargetRow(x, y);
                if (currentRow) {
                    if (self.rowObject.direction == 'down') {
                        self.rowObject.swap('after', currentRow, self);
                    }
                    else {
                        self.rowObject.swap('before', currentRow, self);
                    }
                    self.restripeTable();
                }
            }

            // Similar to row swapping, handle indentations.
            if (self.indentEnabled) {
                if (!self.indentAmount) {
                    var indent = Drupal.theme('tableDragIndentation');
                    var testRow = $('<tr/>').addClass(this.tableSettings.draggableClass).appendTo(this.table);
                    var testCell = $('<td/>').appendTo(testRow).prepend(indent).prepend(indent);
                    this.indentAmount = $('.indentation', testCell).get(1).offsetLeft - $('.indentation', testCell).get(0).offsetLeft;
                    testRow.remove();
                }
                var xDiff = self.currentMouseCoords.x - self.dragObject.indentMousePos.x;
                // Set the number of indentations the mouse has been moved left or right.
                var indentDiff = Math.round(xDiff / self.indentAmount * self.rtl);
                // Indent the row with our estimated diff, which may be further
                // restricted according to the rows around this row.
                var indentChange = self.rowObject.indent(indentDiff);
                // Update table and mouse indentations.
                self.dragObject.indentMousePos.x += self.indentAmount * indentChange * self.rtl;
                self.indentCount = Math.max(self.indentCount, self.rowObject.indents);
            }

            return false;
        }
    };

    /**
     * Mouseup event handler, bound to document.
     * Blur event handler, bound to drag handle for keyboard support.
     */
    Drupal.tableDrag.prototype.dropRow = function (event, self) {
        // Drop row functionality shared between mouseup and blur events.
        if (self.rowObject != null) {
            var droppedRow = self.rowObject.element;

            // The row is already in the right place so we just release it.
            if (self.rowObject.changed == true) {
                // Update the fields in the dropped row.
                self.updateFields(droppedRow);

                // If a setting exists for affecting the entire group, update all the
                // fields in the entire dragged group.

                if (!!self.tableSettings.group.columnName) {
                    for (var n in self.rowObject.children) {
                        self.updateField(self.rowObject.children[n], "group");
                    }
                }

                if (self.changed == false) {
                    self.changed = true;
                }
            }
            if (self.indentEnabled) {
                self.removeIndentClasses();
            }
            if (self.oldRowElement) {
                $(self.oldRowElement).removeClass('drag-previous');
            }
            $(droppedRow).removeClass('drag').addClass('drag-previous');
            self.oldRowElement = droppedRow;
            self.onDrop();
            self.rowObject = null;
        }

        // Functionality specific only to mouseup event.
        if (self.dragObject != null) {
            $('.tabledrag-handle', droppedRow).removeClass('tabledrag-handle-hover');

            self.dragObject = null;
            $('body').removeClass('drag');
            clearInterval(self.scrollInterval);

            // Hack for IE6 that flickers uncontrollably if select lists are moved.
            if (navigator.userAgent.indexOf('MSIE 6.') != -1) {
                $('select', this.table).css('display', 'block');
            }
        }
    };

    /**
     * Get the mouse coordinates from the event (allowing for browser differences).
     */
    Drupal.tableDrag.prototype.mouseCoords = function (event) {
        if (event.pageX || event.pageY) {
            return { x: event.pageX, y: event.pageY };
        }
        return {
            x: event.clientX + document.body.scrollLeft - document.body.clientLeft,
            y: event.clientY + document.body.scrollTop - document.body.clientTop
        };
    };

    /**
     * Given a target element and a mouse event, get the mouse offset from that
     * element. To do this we need the element's position and the mouse position.
     */
    Drupal.tableDrag.prototype.getMouseOffset = function (target, event) {
        var docPos = $(target).offset();
        var mousePos = this.mouseCoords(event);
        return { x: mousePos.x - docPos.left, y: mousePos.y - docPos.top };
    };

    /**
     * Find the row the mouse is currently over. This row is then taken and swapped
     * with the one being dragged.
     *
     * @param x
     *   The x coordinate of the mouse on the page (not the screen).
     * @param y
     *   The y coordinate of the mouse on the page (not the screen).
     */
    Drupal.tableDrag.prototype.findDropTargetRow = function (x, y) {
        var rows = $(this.table.tBodies[0].rows).not(':hidden');
        for (var n = 0; n < rows.length; n++) {
            var row = rows[n];
            var indentDiff = 0;
            var rowY = $(row).offset().top;
            // Because Safari does not report offsetHeight on table rows, but does on
            // table cells, grab the firstChild of the row and use that instead.
            // http://jacob.peargrove.com/blog/2006/technical/table-row-offsettop-bug-in-safari.
            if (row.offsetHeight == 0) {
                var rowHeight = parseInt(row.firstChild.offsetHeight, 10) / 2;
            }
                // Other browsers.
            else {
                var rowHeight = parseInt(row.offsetHeight, 10) / 2;
            }

            // Because we always insert before, we need to offset the height a bit.
            if ((y > (rowY - rowHeight)) && (y < (rowY + rowHeight))) {
                if (this.indentEnabled) {
                    // Check that this row is not a child of the row being dragged.
                    for (var n in this.rowObject.group) {
                        if (this.rowObject.group[n] == row) {
                            return null;
                        }
                    }
                }
                else {
                    // Do not allow a row to be swapped with itself.
                    if (row == this.rowObject.element) {
                        return null;
                    }
                }

                // Check that swapping with this row is allowed.
                if (!this.rowObject.isValidSwap(row)) {
                    return null;
                }

                // We may have found the row the mouse just passed over, but it doesn't
                // take into account hidden rows. Skip backwards until we find a draggable
                // row.
                while ($(row).is(':hidden') && $(row).prev('tr').is(':hidden')) {
                    row = $(row).prev('tr').not('.jqgfirstrow').get(0);
                }
                return row;
            }
        }
        return null;
    };

    /**
     * After the row is dropped, update the table fields according to the settings
     * set for this table.
     *
     * @param changedRow
     *   DOM object for the row that was just dropped.
     */
    Drupal.tableDrag.prototype.updateFields = function (changedRow) {
        for (var group in { group: 1, parent: 1, weight: 1 }) {
            // Each group may have a different setting for relationship, so we find
            // the source rows for each separately.
            this.updateField(changedRow, group);
        }
    };

    /**
     * After the row is dropped, update a single table field according to specific
     * settings.
     *
     * @param changedRow
     *   DOM object for the row that was just dropped.
     * @param group
     *   The settings group on which field updates will occur.
     */
    Drupal.tableDrag.prototype.updateField = function (changedRow, group) {
        var rowSettings = this.rowSettings(group, changedRow);

        // Set the row as its own target.
        if (rowSettings.relationship == 'self' || rowSettings.relationship == 'group') {
            var sourceRow = changedRow;
        }
            // Siblings are easy, check previous and next rows.
        else if (rowSettings.relationship == 'sibling') {
            var previousRow = $(changedRow).prev('tr').not('.jqgfirstrow').get(0);
            var nextRow = $(changedRow).next('tr').not('.jqgfirstrow').get(0);
            var sourceRow = changedRow;
            if ($(previousRow).is('.' + this.tableSettings.draggableClass)
                && $(getColumnSelector(this.tableSettings.tableId, this.tableSettings[group].columnName), previousRow).length) {
                if (this.indentEnabled) {
                    if ($('.indentations', previousRow).length == $('.indentations', changedRow)) {
                        sourceRow = previousRow;
                    }
                }
                else {
                    sourceRow = previousRow;
                }
            }
            else if ($(nextRow).is('.' + this.tableSettings.draggableClass)
                && $(getColumnSelector(this.tableSettings.tableId, this.tableSettings[group].columnName), nextRow).length) {
                if (this.indentEnabled) {
                    if ($('.indentations', nextRow).length == $('.indentations', changedRow)) {
                        sourceRow = nextRow;
                    }
                }
                else {
                    sourceRow = nextRow;
                }
            }
        }
            // Parents, look up the tree until we find a field not in this group.
            // Go up as many parents as indentations in the changed row.
        else if (rowSettings.relationship == 'parent') {
            var previousRow = $(changedRow).prev('tr').not('.jqgfirstrow');
            while (previousRow.length && $('.indentation', previousRow).length >= this.rowObject.indents) {
                previousRow = previousRow.prev('tr').not('.jqgfirstrow');
            }
            // If we found a row.
            if (previousRow.length) {
                sourceRow = previousRow[0];
            }
                // Otherwise we went all the way to the left of the table without finding
                // a parent, meaning this item has been placed at the root level.
            else {
                // Use the first row in the table as source, because it's guaranteed to
                // be at the root level. Find the first item, then compare this row
                // against it as a sibling.
                sourceRow = $(this.table).find('tr.' + this.tableSettings.draggableClass + ':first').get(0);
                if (sourceRow == this.rowObject.element) {
                    sourceRow = $(this.rowObject.group[this.rowObject.group.length - 1]).next('tr.' + this.tableSettings.draggableClass).get(0);
                }
                var useSibling = true;
            }
        }

        // Because we may have moved the row from one category to another,
        // take a look at our sibling and borrow its sources and targets.
        this.copyDragClasses(sourceRow, changedRow, group);
        rowSettings = this.rowSettings(group, changedRow);

        // In the case that we're looking for a parent, but the row is at the top
        // of the tree, copy our sibling's values.
        if (useSibling) {
            rowSettings.relationship = 'sibling';
            rowSettings.source = rowSettings.columnName;
        }

        var targetClass = getColumnSelector(this.tableSettings.tableId, rowSettings.columnName);
        var targetElement = $(targetClass, changedRow);

        // Check if a target element exists in this row.
        if (targetElement) {
            var sourceClass = getColumnSelector(this.tableSettings.tableId, (rowSettings.relationship == 'parent' ? rowSettings.sourceColumnName : rowSettings.columnName));
            var sourceElement = $(sourceClass, sourceRow);
            var table = $(this.table);
            var weight;
            var weightColumnName = this.tableSettings.weight.columnName;
            switch (rowSettings.action) {
                case 'depth':
                    // Get the depth of the target row.
                    var record = table.getLocalRow($(changedRow).attr("id"));
                    var originalLevel = record[this.tableSettings.group.columnName];
                    var originalParent = record[this.tableSettings.parent.columnName];
                    var newLevel = $('.indentation', changedRow).length + this.tableSettings.initialLevel;   
                    record[this.tableSettings.group.columnName] = newLevel;
                    targetElement.text(newLevel);
                    targetElement.attr("title", newLevel);
                    //update previous level's row
                    var levelSelector = targetClass;
                    var parentSelector = getColumnSelector(this.tableSettings.tableId, this.tableSettings.parent.columnName);
                    var previousLevelRow = $("tr." + this.tableSettings.draggableClass, this.table).not(changedRow)
                        .has(columnValueSelector(parentSelector, originalParent))
                        .has(columnValueSelector(levelSelector, originalLevel)).get(0);

                    if (previousLevelRow) {
                        this.updateField(previousLevelRow, 'weight');
                    }
                    break;
                case 'match':
                    // Update the value.
                    var parentValue = sourceElement.attr("title");
                    var currentrecord = table.getLocalRow($(changedRow).attr("id"));
                    currentrecord[this.tableSettings.parent.columnName] = parentValue;
                    targetElement.text(parentValue);
                    targetElement.attr("title", parentValue);
                    break;
                case 'order':
                    var siblings;
                    if (changedRow != this.rowObject.element) {
                        var currentRow = new this.row(changedRow, 'mouse', this.indentEnabled, this.maxDepth, true, this.tableSettings.draggableClass);
                        siblings = currentRow.findSiblings(rowSettings);
                    } else {
                        siblings = this.rowObject.findSiblings(rowSettings);
                    }
                    weight = 1;
                    // Assume a numeric input field.
                    $(targetClass, siblings).each(function () {
                        var id = $(this).parent("tr").attr("id");
                        var temprecord = table.getLocalRow(id);
                        temprecord[weightColumnName] = weight;
                        $(this).text(weight);
                        $(this).attr("title", weight);
                        weight++;
                    });
                    break;
            }
        }
    };

    /**
     * Copy all special tableDrag classes from one row's form elements to a
     * different one, removing any special classes that the destination row
     * may have had.
     */
    Drupal.tableDrag.prototype.copyDragClasses = function (sourceRow, targetRow, group) {
        var sourceElement = $(getColumnSelector(this.tableSettings.tableId, this.tableSettings[group].columnName), sourceRow);
        var targetElement = $(getColumnSelector(this.tableSettings.tableId, this.tableSettings[group].columnName), targetRow);
        if (sourceElement.length && targetElement.length) {
            targetElement[0].className = sourceElement[0].className;
        }
    };

    Drupal.tableDrag.prototype.checkScroll = function (cursorY) {
        var de = document.documentElement;
        var b = document.body;

        var windowHeight = this.windowHeight = window.innerHeight || (de.clientHeight && de.clientWidth != 0 ? de.clientHeight : b.offsetHeight);
        var scrollY = this.scrollY = (document.all ? (!de.scrollTop ? b.scrollTop : de.scrollTop) : (window.pageYOffset ? window.pageYOffset : window.scrollY));
        var trigger = this.scrollSettings.trigger;
        var delta = 0;

        // Return a scroll speed relative to the edge of the screen.
        if (cursorY - scrollY > windowHeight - trigger) {
            delta = trigger / (windowHeight + scrollY - cursorY);
            delta = (delta > 0 && delta < trigger) ? delta : trigger;
            return delta * this.scrollSettings.amount;
        }
        else if (cursorY - scrollY < trigger) {
            delta = trigger / (cursorY - scrollY);
            delta = (delta > 0 && delta < trigger) ? delta : trigger;
            return -delta * this.scrollSettings.amount;
        }
    };

    Drupal.tableDrag.prototype.setScroll = function (scrollAmount) {
        var self = this;

        this.scrollInterval = setInterval(function () {
            // Update the scroll values stored in the object.
            self.checkScroll(self.currentMouseCoords.y);
            var aboveTable = self.scrollY > self.table.topY;
            var belowTable = self.scrollY + self.windowHeight < self.table.bottomY;
            if (scrollAmount > 0 && belowTable || scrollAmount < 0 && aboveTable) {
                window.scrollBy(0, scrollAmount);
            }
        }, this.scrollSettings.interval);
    };

    Drupal.tableDrag.prototype.restripeTable = function () {
        // :even and :odd are reversed because jQuery counts from 0 and
        // we count from 1, so we're out of sync.
        // Match immediate children of the parent element to allow nesting.
        $('> tbody > tr.' + this.tableSettings.draggableClass + ':visible, > tr.' + this.tableSettings.draggableClass + ':visible', this.table)
          .removeClass('odd even')
          .filter(':odd').addClass('even').end()
          .filter(':even').addClass('odd');
    };

    /**
     * Stub function. Allows a custom handler when a row begins dragging.
     */
    Drupal.tableDrag.prototype.onDrag = function () {
        this.$table.trigger('tabledrag:dragrow', this);
        return null;
    };

    /**
     * Stub function. Allows a custom handler when a row is dropped.
     */
    Drupal.tableDrag.prototype.onDrop = function () {
        this.$table.trigger('tabledrag:droprow', this);
        return null;
    };

    /**
     * Constructor to make a new object to manipulate a table row.
     *
     * @param tableRow
     *   The DOM element for the table row we will be manipulating.
     * @param method
     *   The method in which this row is being moved. Either 'keyboard' or 'mouse'.
     * @param indentEnabled
     *   Whether the containing table uses indentations. Used for optimizations.
     * @param maxDepth
     *   The maximum amount of indentations this row may contain.
     * @param addClasses
     *   Whether we want to add classes to this row to indicate child relationships.
     */
    Drupal.tableDrag.prototype.row = function (tableRow, method, indentEnabled, maxDepth, addClasses, draggableClass) {
        this.element = tableRow;
        this.method = method;
        this.group = [tableRow];
        this.groupDepth = $('.indentation', tableRow).length;
        this.changed = false;
        this.table = $(tableRow).closest('table').get(0);
        this.indentEnabled = indentEnabled;
        this.maxDepth = maxDepth;
        this.direction = ''; // Direction the row is being moved.
        this.draggableClass = draggableClass;
        this.rowId = $(tableRow).attr("id");

        if (this.indentEnabled) {
            this.indents = $('.indentation', tableRow).length;
            this.children = this.findChildren(addClasses);
            this.group = $.merge(this.group, this.children);
            // Find the depth of this entire group.
            for (var n = 0; n < this.group.length; n++) {
                this.groupDepth = Math.max($('.indentation', this.group[n]).length, this.groupDepth);
            }
        }
    };

    /**
     * Find all children of rowObject by indentation.
     *
     * @param addClasses
     *   Whether we want to add classes to this row to indicate child relationships.
     */
    Drupal.tableDrag.prototype.row.prototype.findChildren = function (addClasses) {
        var parentIndentation = this.indents;
        var currentRow = $(this.element, this.table).next('tr.' + this.draggableClass);
        var rows = [];
        var child = 0;
        while (currentRow.length) {
            var rowIndentation = $('.indentation', currentRow).length;
            // A greater indentation indicates this is a child.
            if (rowIndentation > parentIndentation) {
                child++;
                rows.push(currentRow[0]);
                if (addClasses) {
                    $('.indentation', currentRow).each(function (indentNum) {
                        if (child == 1 && (indentNum == parentIndentation)) {
                            $(this).addClass('tree-child-first');
                        }
                        if (indentNum == parentIndentation) {
                            $(this).addClass('tree-child');
                        }
                        else if (indentNum > parentIndentation) {
                            $(this).addClass('tree-child-horizontal');
                        }
                    });
                }
            }
            else {
                break;
            }
            currentRow = currentRow.next('tr.' + this.draggableClass);
        }
        if (addClasses && rows.length) {
            $('.indentation:nth-child(' + (parentIndentation + 1) + ')', rows[rows.length - 1]).addClass('tree-child-last');
        }
        return rows;
    };

    /**
     * Ensure that two rows are allowed to be swapped.
     *
     * @param row
     *   DOM object for the row being considered for swapping.
     */
    Drupal.tableDrag.prototype.row.prototype.isValidSwap = function (row) {
        if (this.indentEnabled) {
            var prevRow, nextRow;
            if (this.direction == 'down') {
                prevRow = row;
                nextRow = $(row).next('tr').not('.jqgfirstrow').get(0);
            }
            else {
                prevRow = $(row).prev('tr').not('.jqgfirstrow').get(0);
                nextRow = row;
            }
            this.interval = this.validIndentInterval(prevRow, nextRow);

            // We have an invalid swap if the valid indentations interval is empty.
            if (this.interval.min > this.interval.max) {
                return false;
            }
        }

        // Do not let an un-draggable first row have anything put before it.
        if (this.table.tBodies[0].rows[0] == row && $(row).is(':not(.' + this.draggableClass + ')')) {
            return false;
        }

        return true;
    };

    /**
     * Perform the swap between two rows.
     *
     * @param position
     *   Whether the swap will occur 'before' or 'after' the given row.
     * @param row
     *   DOM element what will be swapped with the row group.
     */
    Drupal.tableDrag.prototype.row.prototype.swap = function (position, row) {
        // @todo Drupal.detachBehaviors(this.group, Drupal.settings, 'move');
        $(row)[position](this.group);
        // @todo Drupal.attachBehaviors(this.group, Drupal.settings);
        this.changed = true;
        this.onSwap(row);
    };

    /**
     * Determine the valid indentations interval for the row at a given position
     * in the table.
     *
     * @param prevRow
     *   DOM object for the row before the tested position
     *   (or null for first position in the table).
     * @param nextRow
     *   DOM object for the row after the tested position
     *   (or null for last position in the table).
     */
    Drupal.tableDrag.prototype.row.prototype.validIndentInterval = function (prevRow, nextRow) {
        var minIndent, maxIndent;

        // Minimum indentation:
        // Do not orphan the next row.
        minIndent = nextRow ? $('.indentation', nextRow).length : 0;

        // Maximum indentation:
        if (!prevRow || $(prevRow).is(':not(.' + this.draggableClass + ')') || $(this.element).is('.tabledrag-root')) {
            // Do not indent:
            // - the first row in the table,
            // - rows dragged below a non-draggable row,
            // - 'root' rows.
            maxIndent = 0;
        }
        else {
            // Do not go deeper than as a child of the previous row.
            maxIndent = $('.indentation', prevRow).length + ($(prevRow).is('.tabledrag-leaf') ? 0 : 1);
            // Limit by the maximum allowed depth for the table.
            if (this.maxDepth) {
                maxIndent = Math.min(maxIndent, this.maxDepth - (this.groupDepth - this.indents));
            }
        }

        return { 'min': minIndent, 'max': maxIndent };
    };

    /**
     * Indent a row within the legal bounds of the table.
     *
     * @param indentDiff
     *   The number of additional indentations proposed for the row (can be
     *   positive or negative). This number will be adjusted to nearest valid
     *   indentation level for the row.
     */
    Drupal.tableDrag.prototype.row.prototype.indent = function (indentDiff) {
        // Determine the valid indentations interval if not available yet.
        if (!this.interval) {
            var prevRow = $(this.element).prev('tr').not('.jqgfirstrow').get(0);
            var nextRow = $(this.group).filter(':last').next('tr').not('.jqgfirstrow').get(0);
            this.interval = this.validIndentInterval(prevRow, nextRow);
        }

        // Adjust to the nearest valid indentation.
        var indent = this.indents + indentDiff;
        indent = Math.max(indent, this.interval.min);
        indent = Math.min(indent, this.interval.max);
        indentDiff = indent - this.indents;

        for (var n = 1; n <= Math.abs(indentDiff) ; n++) {
            // Add or remove indentations.
            if (indentDiff < 0) {
                $('.indentation:first', this.group).remove();
                this.indents--;
            }
            else {
                $('td:first', this.group).prepend(Drupal.theme('tableDragIndentation'));
                this.indents++;
            }
        }
        if (indentDiff) {
            // Update indentation for this row.
            this.changed = true;
            this.groupDepth += indentDiff;
            this.onIndent();
        }

        return indentDiff;
    };

    /**
     * Find all siblings for a row, either according to its subgroup or indentation.
     * Note that the passed-in row is included in the list of siblings.
     *
     * @param settings
     *   The field settings we're using to identify what constitutes a sibling.
     */
    Drupal.tableDrag.prototype.row.prototype.findSiblings = function (rowSettings) {
        var siblings = [];
        var directions = ['prev', 'next'];
        var rowIndentation = this.indents;
        for (var d = 0; d < directions.length; d++) {
            var checkRow = $(this.element)[directions[d]]();
            while (checkRow.length) {
                // Check that the sibling contains a similar target field.
                if ($('.' + rowSettings.target, checkRow)) {
                    // Either add immediately if this is a flat table, or check to ensure
                    // that this row has the same level of indentation.
                    if (this.indentEnabled) {
                        var checkRowIndentation = $('.indentation', checkRow).length;
                    }

                    if (!(this.indentEnabled) || (checkRowIndentation == rowIndentation)) {
                        siblings.push(checkRow[0]);
                    }
                    else if (checkRowIndentation < rowIndentation) {
                        // No need to keep looking for siblings when we get to a parent.
                        break;
                    }
                }
                else {
                    break;
                }
                checkRow = $(checkRow)[directions[d]]();
            }
            // Since siblings are added in reverse order for previous, reverse the
            // completed list of previous siblings. Add the current row and continue.
            if (directions[d] == 'prev') {
                siblings.reverse();
                siblings.push(this.element);
            }
        }
        return siblings;
    };

    /**
     * Remove indentation helper classes from the current row group.
     */
    Drupal.tableDrag.prototype.removeIndentClasses = function () {
        var tdList = $("tr." + this.tableSettings.draggableClass + " td:first-child", this.table).has("div.indentation");
        for (var n = 0 ; n < tdList.length; n++) {
            $('.indentation', tdList[n])
              .removeClass('tree-child')
              .removeClass('tree-child-first')
              .removeClass('tree-child-last')
              .removeClass('tree-child-horizontal');
        }
    };

    /**
     * Stub function. Allows a custom handler when a row is indented.
     */
    Drupal.tableDrag.prototype.row.prototype.onIndent = function () {
        return null;
    };

    /**
     * Stub function. Allows a custom handler when a row is swapped.
     */
    Drupal.tableDrag.prototype.row.prototype.onSwap = function (swappedRow) {
        return null;
    };

    /**
     * Dummy implementation of Drupal.t(). 
     * The Drupal.t() function is Drupal's localization function.
     * @todo - use in the future to localize the plugin.
     */
    Drupal.t = function (string) {
        return string;
    };

    /**
     * Verbatim copy of Drupal.theme().
     * Drupal.theme() is a function that allows to quickly re-use small snippets
     * of HTML markup.
     */
    Drupal.theme = function (func) {
        var args = Array.prototype.slice.apply(arguments, [1]);

        return (Drupal.theme[func] || Drupal.theme.prototype[func]).apply(this, args);
    };

    Drupal.theme.prototype.tableDragChangedMarker = function () {
        return '<span class="warning tabledrag-changed">*</span>';
    };

    Drupal.theme.prototype.tableDragIndentation = function () {
        return '<div class="indentation">&nbsp;</div>';
    };


})(jQuery);
