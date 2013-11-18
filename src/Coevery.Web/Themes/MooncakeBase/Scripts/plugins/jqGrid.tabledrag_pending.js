/**
 * @file
 * jQuery Tabledrag plugin. Drag and drop table rows with field manipulation.
 * Most code is copied verbatim from Drupal core, misc/tabledrag.js.
 * @license GPL V2 
 * @author Wouter Admiraal <wadmiraal@connect-i.ch>
 */

; (function ($, undefined) {

    // Use the jqGrid namespace to facilitate the fork.
    var jqGrid = {};

    /**
     * Plugin definition.
     */
    $.fn.tableDrag = function (settings) {
        settings = $.extend({
            draggableClass: 'jqgrow',
            depthLimit: 3,
            idAttr: 'Id'
        }, settings || {});

        this.each(function () {
            var table = new jqGrid.tableDrag(this, settings);
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
    jqGrid.tableDrag = function (table, tableSettings) {
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
        this.rtl = $(this.table).parent(".ui-jqgrid").attr('dir') == 'rtl' ? -1 : 1; // Direction of the table.
        this.treeReader = this.$table.getGridParam("treeReader");
        this.gridData = this.$table.getGridParam("data");

        // Configure the scroll settings.
        this.scrollSettings = { amount: 4, interval: 50, trigger: 70 };
        this.scrollInterval = null;
        this.scrollY = 0;
        this.windowHeight = 0;

        // Check this table's settings to see if there are parent relationships in
        // this table. For efficiency, large sections of code can be skipped if we
        // don't need to track horizontal movement and indentations.
        this.maxDepth = tableSettings.depthLimit;

        // Make each applicable row draggable.
        // Match immediate children of the parent element to allow nesting.
        $('> tr.' + this.tableSettings.draggableClass + ', > tbody > tr.' + this.tableSettings.draggableClass, table).each(function () { self.makeDraggable(this); });

        // Add mouse bindings to the document. The self variable is passed along
        // as event handlers do not have direct access to the tableDrag object.
        $(document).bind('mousemove', function (event) { return self.dragRow(event, self); });
        $(document).bind('mouseup', function (event) { return self.dropRow(event, self); });
    };

    /**
     * Take an item and add event handlers to make it become draggable.
     */
    jqGrid.tableDrag.prototype.makeDraggable = function (item) {
        var self = this;

        // Create the handle.
        var handle = $('<a href="#" class="tabledrag-handle"><div class="handle">&nbsp;</div></a>').attr('title', jqGrid.t('Drag to re-order'));
        // Insert the handle after indentations (if any).
        if ($('td:first:visible', item).length) {
            $('td:first', item).after(handle);
        }
        else {
            $('td:eq(1)', item).prepend(handle);
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

            // If there's a lingering row object from the keyboard, remove its focus.
            if (self.rowObject) {
                $('a.tabledrag-handle', self.rowObject.element).blur();
            }

            // Create a new rowObject for manipulation of this row.
            self.rowObject = new self.row(item, 'mouse', self.maxDepth, self.tableSettings.draggableClass);

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
                self.rowObject = new self.row(item, 'keyboard', self.maxDepth, self.tableSettings.draggableClass);
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
                    var previousRow = $(self.rowObject.element).prev('tr').get(0);
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
                                previousRow = $(previousRow).prev('tr').get(0);
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
                    var nextRow = $(self.rowObject.group).filter(':last').next('tr').get(0);
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
                            var nextGroup = new self.row(nextRow, 'keyboard', self.maxDepth, self.tableSettings.draggableClass);
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
            return undefined;
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
            return undefined;
        });
    };

    /**
     * Mousemove event handler, bound to document.
     */
    jqGrid.tableDrag.prototype.dragRow = function (event, self) {
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
            return false;
        }
        return undefined;
    };

    /**
     * Mouseup event handler, bound to document.
     * Blur event handler, bound to drag handle for keyboard support.
     */
    jqGrid.tableDrag.prototype.dropRow = function (event, self) {
        // Drop row functionality shared between mouseup and blur events.
        var droppedRow = null;
        if (self.rowObject != null) {
            droppedRow = self.rowObject.element;
            // The row is already in the right place so we just release it.
            if (self.rowObject.changed == true) {
                // Update the fields in the dropped row.
                //@todo: self.updateFields(droppedRow);

                if (self.changed == false) {
                    self.changed = true;
                }
            }

            if (self.oldRowElement) {
                $(self.oldRowElement).removeClass('drag-previous');
            }
            $(droppedRow).removeClass('drag').addClass('drag-previous');
            self.oldRowElement = droppedRow;
            self.currentMouseCoords = self.mouseCoords(event);
            self.onDrop();
            self.rowObject = null;
        }

        // Functionality specific only to mouseup event.
        if (self.dragObject != null) {
            $('.tabledrag-handle', droppedRow).removeClass('tabledrag-handle-hover');

            self.dragObject = null;
            $('body').removeClass('drag');
            clearInterval(self.scrollInterval);

        }
    };

    /**
     * Get the mouse coordinates from the event (allowing for browser differences).
     */
    jqGrid.tableDrag.prototype.mouseCoords = function (event) {
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
    jqGrid.tableDrag.prototype.getMouseOffset = function (target, event) {
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
    jqGrid.tableDrag.prototype.findDropTargetRow = function (x, y) {
        var rows = $(this.table.tBodies[0].rows).not(':hidden');
        for (var n = 0; n < rows.length; n++) {
            var row = rows[n];
            var rowY = $(row).offset().top;
            var rowHeight;
            // Because Safari does not report offsetHeight on table rows, but does on
            // table cells, grab the firstChild of the row and use that instead.
            // http://jacob.peargrove.com/blog/2006/technical/table-row-offsettop-bug-in-safari.
            if (row.offsetHeight == 0) {
                rowHeight = parseInt(row.firstChild.offsetHeight, 10) / 2;
            }
                // Other browsers.
            else {
                rowHeight = parseInt(row.offsetHeight, 10) / 2;
            }

            // Because we always insert before, we need to offset the height a bit.
            if ((y > (rowY - rowHeight)) && (y < (rowY + rowHeight))) {
                if (this.indentEnabled) {
                    // Check that this row is not a child of the row being dragged.
                    for (var m in this.rowObject.group) {
                        if (this.rowObject.group[m] == row) {
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
                    row = $(row).prev('tr').get(0);
                }
                return row;
            }
        }
        return null;
    };

    /**
     * Copy all special tableDrag classes from one row's form elements to a
     * different one, removing any special classes that the destination row
     * may have had.
     */
    jqGrid.tableDrag.prototype.copyDragClasses = function (sourceRow, targetRow, group) {
        var sourceElement = $('.' + group, sourceRow);
        var targetElement = $('.' + group, targetRow);
        if (sourceElement.length && targetElement.length) {
            targetElement[0].className = sourceElement[0].className;
        }
    };

    jqGrid.tableDrag.prototype.checkScroll = function (cursorY) {
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
        return undefined;
    };

    jqGrid.tableDrag.prototype.setScroll = function (scrollAmount) {
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

    jqGrid.tableDrag.prototype.restripeTable = function () {
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
    jqGrid.tableDrag.prototype.onDrag = function () {
        this.$table.trigger('tabledrag:dragrow', this);
        return null;
    };

    /**
     * Stub function. Allows a custom handler when a row is dropped.
     */
    jqGrid.tableDrag.prototype.onDrop = function () {
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
     * @param maxDepth
     *   The maximum amount of indentations this row may contain.
     */
    jqGrid.tableDrag.prototype.row = function (tableRow, method, maxDepth, draggableClass) {
        this.element = tableRow;
        this.method = method;
        this.rowId = $(tableRow).attr("Id");
        this.group = [tableRow];
        this.changed = false;
        this.table = $(tableRow).closest('table').get(0);
        this.maxDepth = maxDepth;
        this.direction = ''; // Direction the row is being moved.
        this.draggableClass = draggableClass;
        this.rowRecord = $(this.table).getLocalRow(this.rowId);

        var table = $(this.table);
        var ids = [this.rowId];
        var childrenRecords = table.getNodeChildren(this.rowRecord);
        var children = [];
        if (!!childrenRecords.length) {
            childrenRecords.forEach(function (value) {
                children.push(table.getInd(value._id_, true));
                ids.push(value._id_);
            });
        };
        this.group = $.merge(this.group, children);
        this.groupIds = ids;
    };

    /**
     * Ensure that two rows are allowed to be swapped.
     *
     * @param row
     *   DOM object for the row being considered for swapping.
     */
    //todo: check swap validation
    jqGrid.tableDrag.prototype.row.prototype.isValidSwap = function (row) {
        if (this.indentEnabled) {
            var prevRow, nextRow;
            if (this.direction == 'down') {
                prevRow = row;
                nextRow = $(row).next('tr').get(0);
            }
            else {
                prevRow = $(row).prev('tr').get(0);
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
    jqGrid.tableDrag.prototype.row.prototype.swap = function (position, row) {
        // @todo jqGrid.detachBehaviors(this.group, jqGrid.settings, 'move');
        $(row)[position](this.group);
        // @todo jqGrid.attachBehaviors(this.group, jqGrid.settings);
        this.changed = true;
        this.onSwap(row);
    };

    /**
     * Find all siblings for a row, either according to its subgroup or indentation.
     * Note that the passed-in row is included in the list of siblings.
     *
     * @param settings
     *   The field settings we're using to identify what constitutes a sibling.
     */
    jqGrid.tableDrag.prototype.row.prototype.findSiblings = function (rowSettings) {
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
 * Dummy implementation of Drupal.t(). 
 * The jqGrid.t() function is jqGrid's localization function.
 * @todo - use in the future to localize the plugin.
 */
    jqGrid.t = function (string) {
        return string;
    };

    /**
     * Stub function. Allows a custom handler when a row is swapped.
     */
    jqGrid.tableDrag.prototype.row.prototype.onSwap = function (swappedRow) {
        return null;
    };

})(jQuery);
