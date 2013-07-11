;(function($, window) {
    $(function() {
        var nav = $("#navigation");
        //nav.css("height", $(window).height() - $("#nav-selector").outerHeight() - 110);
        $('#navigation').perfectScrollbar();

        //$(window).resize(function() {
        //    nav.css("height", $(window).height() - $("#nav-selector").outerHeight() - 110);
        //    $('#navigation').perfectScrollbar('update');
        //    $('#navigation').perfectScrollbar('destroy');
        //});
    });
})(jQuery, window);