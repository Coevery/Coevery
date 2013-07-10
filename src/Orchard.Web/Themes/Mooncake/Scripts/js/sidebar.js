;(function($, window) {
    $(function() {
        var sidebar = $("#sidebarItems");
        sidebar.css("height", $(window).height() - $("#nav-selector").height() - 114);
        //sidebar.css("width", $("#sidebar").width());
        sidebar.css("overflow","hidden");
        $('#sidebarItems').perfectScrollbar();

        $(window).resize(function() {
            sidebar.css("height", $(window).height() - $("#nav-selector").height() - 114);
            $('#sidebarItems').perfectScrollbar('update');
        });
    });
})(jQuery, window);