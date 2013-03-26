

var ContextMenu = function (id) {
    this.id = id == undefined ? "contextMenu" + new Date().getTime() : id;
    this.items = new Array();
    this.initialized = false;
    this.currentSender = null;
};

ContextMenu.prototype.hide = function () {
    $("#" + this.id).hide();
};

ContextMenu.prototype.show = function (e, sender) {
    var obj = this;
    obj.currentSender = sender;
    if (this.initialized == false) {
        obj.items = fixItems(obj.items);
        init();
        this.initialized = true;
    }
    var $menu = $("#" + this.id);
    $menu.css("top", e.pageY);
    $menu.css("left", e.pageX - 25);
    $menu.css("z-index", 101);

    var docSize = {
        width: document.documentElement.offsetWidth,
        height: document.documentElement.offsetHeight
    };


    var maxWidth = docSize.width - $menu[0].offsetWidth;
    var maxHeight = docSize.height - $menu[0].offsetHeight;

    $menu[0].offsetTop > maxHeight && ($menu.css("top", maxHeight));
    $menu[0].offsetLeft > maxWidth && ($menu.css("left", maxWidth));

    $menu.find("li").each(function () {
        var $li = $(this);
        if ($li.data("data").disabled) {
            $li.addClass("disable");
        }
        else {
            $li.removeClass("disable");
        }
    });

    $menu.show();

    function fixItems(items) {
        var newItems = new Array();
        for (var i = 0; i < items.length; i++) {
            newItems.push($.extend(new MenuItem(), items[i]));
        }
        return newItems;
    }

    function init() {
        var $m = $("<div id='" + obj.id + "' class='context-menu'></div>");
        $m.appendTo("body");
        var $ul = $("<ul></ul>");
        $ul.appendTo($m);
        addItemsToUl($ul, obj.items);
        $(document).mousedown(function (evt) {
            if ($(evt.target).parents('#' + obj.id).length === 0) {
                obj.hide();
            }
        });

        function addItemsToUl(ul, items) {
            var $list = $(ul);
            $(items).each(function () {
                var item = this;
                var $item = $("<li data-oper='" + item.operation + "'>" + item.text + "</li>");
                $item.appendTo($list);
                $item.data("data", item);
                if (item.action != null) {
                    $item.click(function () {
                        if (item.disabled) {
                            return;
                        }
                        item.action(obj.currentSender);
                        obj.hide();
                    });
                }
                if (item.children.length > 0) {
                    var $children = $("<ul></ul>");
                    $children.appendTo($item);
                    addItemsToUl($children, item.children);
                }
            });
        }
    }
};

var MenuItem = function () {
    this.text = "";
    this.operation = "";
    this.action = null;
    this.disabled = false;
    this.setDisable = null;
    this.children = new Array();
};
