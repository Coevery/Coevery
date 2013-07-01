;(function($, window) {
    $(function() {
        var sidebar = $("#sidebar");
        sidebar.css("height", $(window).height() - 153);
        $('#sidebar').perfectScrollbar();

        $(window).resize(function() {
            sidebar.css("height", $(window).height() - 153);
            $('#Demo').perfectScrollbar('update');
        });
    });
})(jQuery, window);