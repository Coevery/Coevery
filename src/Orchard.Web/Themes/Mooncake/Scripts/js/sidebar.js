;(function($, window) {
    $(function() {
        var sidebar = $("#sidebar");
        sidebar.css("height", $(window).height() - 114);
        $('#sidebar').perfectScrollbar();

        $(window).resize(function() {
            sidebar.css("height", $(window).height() - 114);
            $('#Demo').perfectScrollbar('update');
        });
    });
})(jQuery, window);