;(function($, window) {
    $(function() {
        var sidebar = $("#navigation");
        sidebar.css("height", $(window).height() - $("#nav-selector").outerHeight() - 110);
        $('#navigation').perfectScrollbar();

        $(window).resize(function() {
            sidebar.css("height", $(window).height() - $("#nav-selector").outerHeight() - 110);
            $('#navigation').perfectScrollbar('update');
        });
    });
})(jQuery, window);