'use strict';

var formElementTemplate = {
    text: '<div id="{0}" data-key="form-text"><label class="form-label span3">{1}</label> \
             <div class="controls-row span9"><input type="text" class="span12"/></div></div>',
    radio: '<div id="{0}" data-key="form-radio"><label class="form-label span3">{1}</label> \
             <div class="controls-row span9"><label class="radio span6"><input type="radio" name="radioOptions"' +
        ' checked/>option1</label><label class="radio span6"><input type="radio" name="radioOptions"/>option2</label></div></div>',
    checkbox: '<div id="{0}" data-key="form-checkbox"><label class="form-label span3">{1}</label> \
             <div class="controls-row span9"><label class="checkbox span12"><input type="checkbox" />option1</label></div></div>',
    select: '<div id="{0}" data-key="form-select"><label class="form-label span3">{1}</label> \
             <div class="controls-row span9"><select class="span12"><option>option1</option><option>option2</option><option>option3</option></select></div></div>',
    textarea: '<div id="{0}" data-key="form-textarea"><label class="form-label span3">{1}</label> \
             <div class="controls-row span9"><textarea class="span12"></textarea></div></div>'
};

var cellOperation = {
    mergeRight: "mergeRight",
    mergeLeft: "mergeLeft",
    split: "split",
    oneCell: "oneCell",
    twoCells: "twoCells",
    threeCells: "threeCells",
    fourCells: "fourCells",
    clear: "clear"
};

var cellMergeDirection = {
    left: "left",
    right: "right"
};

var zIndex = 101;
var cellContextMenu = null;
var rowContextMenu = null;

function setZIndex(jQueryElem) {
    jQueryElem.css("z-index", ++zIndex);
}

/*
 * Base control class
 */
function Control() {
    this.id = newGuid();
    this.controls = [];
    this.title = "Click to edit";
    this.parent = null;
    this.type = "Control";
}
Control.prototype.init = function (parent) {
    var thisControl = this;
    this.parent = parent instanceof Control ? parent : null;

    //insert dom to right place.
    var parentElem = parent instanceof Control ? "#" + parent.id : parent;
    var dom = this.buildDom();
    if (this.parent != null) {
        var controls = this.parent.controls;
        for (var i = 0; i < controls.length; i++) {
            if (controls[i].id == this.id) {
                if (i == 0) {
                    var entry = $(parentElem + ' .entry:first');
                    entry.prepend(dom);
                } else {
                    $("#" + controls[i - 1].id).after(dom);
                }
            }
        }
    } else {
        $(parentElem).append(dom);
    }

    //init child controls.
    $.each(this.controls, function (i, v) {
        v.init(thisControl);
    });
    thisControl.registerEvent();

    if (typeof this.onInit === "function") {
        this.onInit();
    }
};
Control.prototype.buildDom = function (parentElem) {
    throw "cannot invoke base control's buildDom function.";
};
Control.prototype.registerEvent = function () {
    throw "cannot invoke base control's registerEvent function.";
};
Control.prototype.addControl = function (control, index) {
    this.controls.Insert(control, index);
    control.init(this);
};
Control.prototype.addNext = function (control) {
    var index = this.parent.controls.indexOf(this) + 1;
    this.parent.addControl(control, index);
};
Control.prototype.addPrev = function (control) {
    var index = this.parent.controls.indexOf(this);
    this.parent.addControl(control, index);
};
Control.prototype.findItem = function (id) {
    if (this.id === id) {
        return this;
    } else {
        for (var i = 0; i < this.controls.length; i++) {
            var item = this.controls[i].findItem(id);
            if (item != null) {
                return item;
            }
        }
    }
    return null;
};
Control.prototype.remove = function (closeAnimation) {
    this.parent.controls.Remove(this);
    if (closeAnimation) {
        $("#" + this.id).remove();
    } else {
        $("#" + this.id).slideUp("fast", function () {
            $(this).remove();
        });
    }
};
Control.prototype.toXml = function (additionalAction) {
    var xml = "<Control xsi:type=\"" + this.type + "\">";
    xml += "<Title>" + this.title + "</Title>";
    xml += "<Controls>";
    $(this.controls).each(function () {
        xml += this.toXml();
    });
    xml += "</Controls>";
    if (additionalAction != undefined && additionalAction != null) {
        xml += additionalAction();
    }
    xml += "</Control>";
    return xml;
};
Control.prototype.createChild = function () {
    return null;
};
Control.prototype.updateFrom = function (obj) {
    this.title = obj.Title;
    for (var i = 0; i < obj.Controls.length; i++) {
        var child = this.createChild();
        child.updateFrom(obj.Controls[i]);
        this.controls.push(child);
    }
};
/*
 * Form class
 */
function Form(tab) {
    Control.call(this);
    this.title = "Click to input form name.";
    this.controls = [];
    if (tab != undefined) {
        this.controls.push(tab);
    }
    this.type = "Form";
}
Form.prototype = new Control();
Form.prototype.constructor = Form;
Form.prototype.buildDom = function () {
    var formDom = $('<div id="' + this.id + '" class="form"><div class="entry"></div></div>');
    //var headerHtml = '<h3 class="form-title"><label>' + this.title + '</label><input type="text" class="text-editor" /></h3>';
    //formDom.append($(headerHtml));
    return formDom;
};
Form.prototype.registerEvent = function () {
    var form = this;
    //TODO: should edit title
    //$(document).on("click", "#" + form.id + " .form-title label", function (e) {
    //    var title = $(this);
    //    var editor = title.next();
    //    editor.val(title.html().trim());
    //    title.hide();
    //    editor.show();
    //    editor.focus();
    //});
    //$(document).on("blur", "#" + form.id + " .form-title input", function (e) {
    //    var editor = $(this);
    //    var title = editor.prev();
    //    title.html(editor.val()).show();
    //    editor.hide();
    //});

    //$("#" + this.id + " h3.form-title input").keyup(function (e) {
    //    form.title = $(this).val();
    //});

    $("#" + form.id + ">.entry").sortable({
        items: "div.tab:not(.sort-placeholder)",
        placeholder: "sort-placeholder",
        beforeStop: function (event, ui) {
            var item = ui.item;
            var insertIndex = $("#" + form.id + ">.entry").children(":not(.sort-placeholder)").index(item);
            if (item.length > 0 && item.is("p")) {
                var newtab = new Tab(new Row(1));
                form.addControl(newtab, insertIndex);
                ui.item.replaceWith($("#" + newtab.id));
            } else {
                var id = item.attr("id");
                var ctrl = form.findItem(id);
                form.controls.Remove(ctrl);
                form.controls.Insert(ctrl, insertIndex);
            }
        }
    });
};
Form.prototype.toXml = function () {
    var xml = "<?xml version=\"1.0\"?>";
    xml += "<Form xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">";
    xml += "<Title>" + this.title + "</Title>";
    xml += "<Controls>";
    $(this.controls).each(function () {
        xml += this.toXml();
    });
    xml += "</Controls>";
    xml += "</Form>";
    return xml;
};
Form.prototype.createChild = function () {
    return new Tab();
};

/*
 * Tab class
 */
function Tab(row) {
    Control.call(this);
    this.title = "Click to input tab name";
    this.controls = [];
    if (row != null) {
        this.controls.push(row);
    }
    this.type = "Tab";
}
Tab.prototype = new Control();
Tab.prototype.constructor = Tab;
Tab.prototype.buildDom = function () {
    var tabDom = $('<div class="row-fluid tab" id="' + this.id +
        '"><section class="span12 widget"><header class="widget-header"><span class="title">&nbsp;</span>' +
        '</header><section class="widget-content form-container"><form class="form-horizontal entry"></form></section></section></div>');
    return tabDom;
};
Tab.prototype.registerEvent = function () {
    var tab = this;
    var tabElem = "#" + this.id;
    $(tabElem + " form.entry").sortable({
        items: "div.row1:not(.sort-placeholder)",
        placeholder: "sort-placeholder",
        connectWith: ".tab form.entry",
        receive: function (e, ui) {
            var item = $(ui.item);
            if (ui.sender != null) {
                var sender = ui.sender;
                var t = tabElem;
                var senderId = sender.attr("id");
                var senderTab = tab.parent.findItem(ui.sender.attr("id"));
                if (senderTab != null) {
                    var insertIndex = $("#" + tab.id + " form.entry").children(":not(.sort-placeholder)").index(item);
                    var id = item.attr("id");
                    var ctrl = senderTab.findItem(id);
                    senderTab.controls.Remove(ctrl);
                    tab.controls.Insert(ctrl, insertIndex);
                }
            }
            $("#" + tab.id + " form.entry").remove(".sort-placeholder");
        },
        beforeStop: function (event, ui) {
            var item = $(ui.item);
            var insertIndex = $("#" + tab.id + " form.entry").children(":not(.sort-placeholder)").index(item);
            if (item.length > 0 && item.is("p")) {
                var key = item.data("key");
                var colCount = parseInt(key.match(/\d/));
                var newRow = new Row(colCount > 0 ? colCount : 1);
                tab.addControl(newRow, insertIndex);
                item.replaceWith($("#" + newRow.id));
            } else {
                var id = item.attr("id");
                var ctrl = tab.findItem(id);
                tab.controls.Remove(ctrl);
                tab.controls.Insert(ctrl, insertIndex);
            }
        }
    });

    //$(tabElem + " h4.form-title input").keyup(function (e) {
    //    tab.title = $(this).val();
    //});

    //var deleteElem = $(tabElem + " > .delete");
    //$(tabElem).hover(function (e) {
    //    deleteElem.removeClass("del-red").addClass("del-gray").fadeIn("normal");
    //}, function (e) {
    //    deleteElem.fadeOut("normal");
    //});
    //deleteElem.hover(function (e) {
    //    deleteElem.removeClass("del-gray").addClass("del-red");
    //}, function (e) {
    //    deleteElem.removeClass("del-red").addClass("del-gray");
    //});

    //deleteElem.click(function (e) {
    //    tab.remove();
    //});
};
Tab.prototype.createChild = function () {
    return new Row(1);
};

/*
 * Row class
 */
function Row(colCount) {
    Control.call(this);
    this.colCount = colCount > 0 ? colCount : 1;
    this.controls = [];
    for (var i = 0; i < this.colCount; i++) {
        this.controls.push(new Cell(1));
    }
    this.type = "Row";
}
Row.prototype = new Control();
Row.prototype.constructor = Row;
Row.prototype.buildDom = function () {
    var rowDom = $('<div id="' + this.id + '" class="row-fluid row1"><div class="entry"></div></div>');
    return rowDom;
};
Row.prototype.registerEvent = function () {
};
Row.prototype.hasFormControls = function () {
    var hasCtrl = false;
    this.controls.Each(function (i, v) {
        if (v.controls.length > 0) {
            hasCtrl = true;
            return false;
        }
    });
    return hasCtrl;
};
Row.prototype.setHeight = function () {
    var cells = this.controls;
    var maxHeight = 0;
    cells.Each(function (i, item) {
        var itemElem = $("#" + item.id);
        itemElem[0].style.removeProperty("height");
        if (item.controls.length > 0) {
            var tempHeight = itemElem.height();
            maxHeight = tempHeight > maxHeight ? tempHeight : maxHeight;
        }
    });
    cells.Each(function (i, item) {
        $("#" + item.id).height(maxHeight);
    });
};
Row.prototype.updateColCount = function (colCount) {
    this.colCount = colCount;
};
Row.prototype.toXml = function () {
    var obj = this;
    return Control.prototype.toXml.call(this, function () {
        return "<ColCount>" + obj.colCount + "</ColCount>";
    });
};
Row.prototype.updateFrom = function (obj) {
    Control.prototype.updateFrom.call(this, obj);
    this.colCount = obj.ColCount;
};
Row.prototype.createChild = function () {
    return new Cell(1);
};
/*
 * Cell class
 */
function Cell(colCount) {
    Control.call(this);
    this.colCount = colCount > 0 ? colCount : 1;
    this.width = 12;
    this.controls = [];
    this.type = "Cell";
}
Cell.prototype = new Control();
Cell.prototype.constructor = Cell;
Cell.prototype.buildDom = function () {
    this.width = 12 / this.parent.colCount * this.colCount;
    var cellDom = $('<div id="' + this.id + '" class="cell span' + this.width + '"><section class="control-group entry"></section></div>');
    return cellDom;
};
Cell.prototype.registerEvent = function () {
    var cell = this;
    var cellDom = $("#" + this.id);
    cellDom.droppable({
        accept: "[data-key^=form]",
        tolerance: "pointer",
        over: function (event, ui) {
            $(this).addClass("drop-highlight");
        },
        out: function (event, ui) {
            $(this).removeClass("drop-highlight");
        },
        drop: function (event, ui) {
            $(this).removeClass("drop-highlight");

            var existItem = cell.controls[0];
            var form = cell.parent.parent.parent;
            var dragElement = $(ui.helper);
            var dragItem = form.findItem(dragElement.attr("id"));
            var originalCell = null;

            if (dragItem != null) {
                //if dragItem is itself, do nothing.
                if (existItem && existItem.id == dragItem.id) {
                    dragElement.animate({
                        top: 0,
                        left: 0
                    }, 500);
                    return;
                } else {
                    //else remove the dragItem's cell.
                    //dragItem.parent.removeControls(true);
                    originalCell = dragItem.parent;
                    $("#" + originalCell.id + ">.entry").html("");
                }
            } else {
                //add new control
                var key = dragElement.data("key");
                var formKey = key.substr("form-".length);
                switch (formKey) {
                    case "text": dragItem = new Textbox(); break;
                    case "radio": dragItem = new RadioList(); break;
                    case "checkbox": dragItem = new CheckboxList(); break;
                    case "select": dragItem = new DropdownList(); break;
                    case "textarea": dragItem = new Textarea(); break;
                    default: dragItem = new Textbox(); break;
                }
            }

            var ctrls = cell.parent.controls;

            if (cell.controls.length == 0) {
                cell.addControl(dragItem);
            } else {
                var row = cell.parent;
                var newRow = new Row(row.colCount);
                row.addPrev(newRow);
                var index = ctrls.indexOf(cell);

                for (var i = 1; i < cell.colCount; i++) {
                    newRow.controls[index].merge(cellMergeDirection.right);
                }
                newRow.controls[index].addControl(dragItem);
            }
            if (originalCell != null) {
                originalCell.removeControls();
            }
        }
    });

    //right click menu

    if (cellContextMenu == null) {
        cellContextMenu = new ContextMenu("cellcellContextMenu");
        cellContextMenu.items.push({
            text: "merge right",
            action: function (senderCell) {
                senderCell.merge(cellMergeDirection.right);
            },
            setDisable: function (senderCell) {
                this.disabled = !senderCell.canMergeRight();
            }
        });
        cellContextMenu.items.push({
            text: "merge left",
            action: function (senderCell) {
                senderCell.merge(cellMergeDirection.left);
            },
            setDisable: function (senderCell) {
                this.disabled = !senderCell.canMergeLeft();
            }
        });
        cellContextMenu.items.push({
            text: "split",
            action: function (senderCell) {
                senderCell.split();
            },
            setDisable: function (senderCell) {
                this.disabled = !senderCell.canSplit();
            }
        });
        cellContextMenu.items.push({
            text: "clear",
            action: function (senderCell) {
                senderCell.removeControls();
            },
            setDisable: function (senderCell) {
                this.disabled = !senderCell.canClear();
            }
        });
        cellContextMenu.items.push({
            text: "remove row",
            action: function (senderCell) {
                senderCell.parent.remove();
            },
            setDisable: function () {
                return false;
            }
        });
    }

    cellDom.bind("contextmenu", function (e) {
        for (var i = 0; i < cellContextMenu.items.length; i++) {
            var item = cellContextMenu.items[i];
            item.setDisable(cell);
        }
        cellContextMenu.show(e, cell);
        return false;
    });

};
Cell.prototype.removeControls = function () {
    this.controls.Clear();
    var cellEntry = $("#" + this.id + ">.entry");
    cellEntry.html("");

    this.parent.setHeight();
    return this;
};
Cell.prototype.changeColCount = function (newColCount) {
    var newWidth = 12 / this.parent.colCount * newColCount;
    $("#" + this.id).removeClass("span" + this.width).addClass("span" + newWidth);
    this.colCount = newColCount;
    this.width = newWidth;
};
Cell.prototype.menuSwitch = function () {
    var ret = {};
    var cell = this;
    var ctrls = cell.parent.controls;
    var index = ctrls.indexOf(cell);
    //merge left
    if (index > 0 && (ctrls[index - 1].controls.length == 0 || cell.controls.length == 0)) {
        ret[cellOperation.mergeLeft] = true;
    }
    //merge right
    if (index < ctrls.length - 1 && (ctrls[index + 1].controls.length == 0 || cell.controls.length == 0)) {
        ret[cellOperation.mergeRight] = true;
    }
    //split
    if (cell.colCount > 1) {
        ret[cellOperation.split] = true;
    }
    if (cell.controls.length > 0) {
        ret[cellOperation.clear] = true;
    }
    //todo: resplit row

    return ret;
};
Cell.prototype.split = function () {
    if (!this.canSplit()) {
        return;
    }
    var parentColCount = 0;
    var rowControls = this.parent.controls.length;

    switch (this.parent.colCount) {
        case 1:
            parentColCount = 2;
            break;
        case 2:
        case 4:
            parentColCount = rowControls < 2 ? 2 : 4;
            break;
        default:
            parentColCount = this.parent.colCount;
            break;
    }

    var oldColCount = this.colCount;
    this.parent.updateColCount(parentColCount);
    this.changeColCount(1);

    if (parentColCount == 4 && rowControls == 2) {
        if (oldColCount == 3) {
            this.addNext(new Cell(1));
        } else {
            var index = this.parent.controls.indexOf(this) == 0 ? 1 : 0;
            this.parent.controls[index].changeColCount(2);
        }
    } else if (parentColCount == 3 && rowControls == 1) {
        this.addNext(new Cell(1));
    }
    this.addNext(new Cell(1));
    this.parent.setHeight();
};
Cell.prototype.merge = function (direction) {
    var row = this.parent;
    var ctrls = row.controls;
    var index = ctrls.indexOf(this);

    var mergeCell = cellMergeDirection.left == direction ? ctrls[index - 1] : ctrls[index + 1];

    var colCount = this.colCount + mergeCell.colCount;
    if (row.colCount == colCount) {
        row.updateColCount(1);
        colCount = 1;
    }

    var formCtrl = this.controls.length > 0 ? this.controls[0] : mergeCell.controls[0];
    if (formCtrl) {
        this.controls.Clear();
        var cellEntry = $("#" + this.id + ">.entry");
        cellEntry.html("");
        this.addControl(formCtrl);
    }
    mergeCell.remove(true);
    this.changeColCount(colCount);
};
Cell.prototype.canMergeRight = function () {
    var index = this.parent.controls.indexOf(this);
    if (index >= this.parent.controls.length - 1) {
        return false;
    }
    var rightCell = this.parent.controls[index + 1];
    return rightCell.controls.length == 0;
};
Cell.prototype.canMergeLeft = function () {
    var index = this.parent.controls.indexOf(this);
    if (index == 0) {
        return false;
    }
    var leftCell = this.parent.controls[index - 1];
    return leftCell.controls.length == 0;
};
Cell.prototype.canSplit = function () {
    switch (this.parent.colCount) {
        case 1:
        case 2:
            return true;
        default:
            return this.colCount > 1;
    }
};
Cell.prototype.canClear = function () {
    return this.controls.length > 0;
};
Cell.prototype.toXml = function () {
    var obj = this;
    return Control.prototype.toXml.call(this, function () {
        return "<ColSpan>" + obj.colCount + "</ColSpan>";
    });
};
Cell.prototype.updateFrom = function (obj) {
    this.title = obj.Title;
    this.colCount = obj.ColSpan;
    for (var i = 0; i < obj.Controls.length; i++) {
        var control = obj.Controls[i];
        var child = null;
        switch (control.Type) {
            case "Textbox":
                child = new Textbox();
                break;
            case "RadioList":
                child = new RadioList();
                break;
            case "CheckboxList":
                child = new CheckboxList();
                break;
            case "DropdownList":
                child = new DropdownList();
                break;
            case "Textarea":
                child = new Textarea();
                break;
            default:
        }
        child.title = control.Title;
        this.controls.push(child);
    }
};
/*
 * Textbox class
 */
function FormControl() {
    Control.call(this);
    this.type = "Field";
};
FormControl.prototype = new Control();
FormControl.prototype.constructor = FormControl;
FormControl.prototype.registerDrag = function () {
    var formControl = this;
    $("#" + this.id).draggable({
        revert: "invalid",
        connectToSortable: ".cell",
        start: function (event, ui) {
            setZIndex(ui.helper);
        }
    });

    $("#" + this.id + " div.form-title input").keyup(function (e) {
        formControl.title = $(this).val();
    });

};
FormControl.prototype.onInit = function () {
    this.parent.parent.setHeight();
};
FormControl.prototype.registerEvent = function () {
    this.registerDrag();
    var obj = this;
    $("#" + this.id + " .text-editor").keyup(function () {
        obj.title = $(this).val();
    });
};

/*
 * Textbox class
 */
function Textbox() {
    FormControl.call(this);
    this.type = "Textbox";
}
Textbox.prototype = new FormControl();
Textbox.prototype.constructor = Textbox;
Textbox.prototype.buildDom = function () {
    var textDom = $(formElementTemplate["text"].format(this.id, this.title));

    return textDom;
};

/*
 * radio class
 */
function RadioList() {
    FormControl.call(this);
    this.type = "RadioList";
}
RadioList.prototype = new FormControl();
RadioList.prototype.constructor = RadioList;
RadioList.prototype.buildDom = function () {
    var radioDom = $(formElementTemplate["radio"].format(this.id, this.title));
    return radioDom;
};

/*
 * checkbox class
 */
function CheckboxList() {
    FormControl.call(this);
    this.type = "CheckboxList";
}
CheckboxList.prototype = new FormControl();
CheckboxList.prototype.constructor = CheckboxList;
CheckboxList.prototype.buildDom = function () {
    var checkboxDom = $(formElementTemplate["checkbox"].format(this.id, this.title));
    return checkboxDom;
};

/*
 * select class
 */
function DropdownList() {
    FormControl.call(this);
    this.type = "DropdownList";
}
DropdownList.prototype = new FormControl();
DropdownList.prototype.constructor = DropdownList;
DropdownList.prototype.buildDom = function () {
    var dropdownDom = $(formElementTemplate["select"].format(this.id, this.title));
    return dropdownDom;
};

/*
 * textarea class
 */
function Textarea() {
    FormControl.call(this);
    this.type = "Textarea";
}
Textarea.prototype = new FormControl();
Textarea.prototype.constructor = Textarea;
Textarea.prototype.buildDom = function (parentElem) {
    var textareaDom = $(formElementTemplate["textarea"].format(this.id, this.title));
    return textareaDom;
};

function getCloneElem(event) {
    var targetElem = $(event.target);
    var cloneElem = targetElem.clone();
    cloneElem.height(targetElem.height());
    return cloneElem;
}
/*
 * init toolboxes
 */
function initToolbox() {
    $("#toolboxes fieldset p[data-key=tab]").draggable({
        helper: getCloneElem,
        revert: "invalid",
        connectToSortable: ".form>.entry",
        start: function (event, ui) {
            setZIndex(ui.helper);
        }
    });

    $("#toolboxes fieldset p[data-key^=row]").draggable({
        helper: getCloneElem,
        revert: "invalid",
        connectToSortable: ".tab form.entry",
        start: function (event, ui) {
            setZIndex(ui.helper);
        }
    });

    $("#toolboxes fieldset p[data-key^=form-]").draggable({
        helper: getCloneElem,
        revert: "invalid",
        connectToSortable: ".cell",
        start: function (event, ui) {
            setZIndex(ui.helper);
        }
    });
}

function convertToClientForm(formObj) {
    var clientForm = new Form();
    clientForm.updateFrom(formObj);
    return clientForm;
}

var form = null;
$(function () {
    initToolbox();
    form = new Form(new Tab(new Row(1)));
    form.controls[0].controls[0].controls[0].controls.push(new Textbox());
    form.controls[1] = new Tab(new Row(1));
    form.controls[1].controls[0].controls[0].controls.push(new CheckboxList());
    form.controls[2] = new Tab(new Row(1));
    //var jsonForm = $("#hfJsonForm").val();
    //if (jsonForm == "") {
    //    form = new Form(new Tab(new Row(1)));
    //}
    //else {
    //    form = convertToClientForm($.parseJSON(jsonForm));
    //}
    form.init("#form-designer");


    //var $demoForm = $("#demoForm");
    //$("#btnPreview").click(function () {
    //    $("[name='xml']").val(form.toXml());
    //    $demoForm.attr("action", "/demo/preview");
    //    $demoForm.attr("target", "_blank");
    //    $demoForm.submit();
    //});

    //var $btnSave = $("#btnSave");
    //$btnSave.click(function () {
    //    $("[name='xml']").val(form.toXml());
    //    $demoForm.attr("action", "/demo/save");
    //    $demoForm.removeAttr("target");
    //    $demoForm.submit();
    //});
});