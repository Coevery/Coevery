(function ($) {

    "use strict";
    $(document)
        .on('mouseover.dropdown-menu', '.ngGrid .ngRow', function (e) {
            $(this).find('.row-actions').removeClass('hide');
        });
    $(document)
   .on('mouseout.dropdown-menu', '.ngGrid .ngRow', function (e) {
       $(this).find('.row-actions').addClass('hide');
   });
    $(document)
        .on("click.navmenu", '#wrapper #sidebar #navigation ul.menu > li', function (e) {
            var navToggle = $('#header #header-right #nav-toggle');
            if (navToggle.css('display') != 'block') return;
            var navigation = $('#wrapper #sidebar #navigation');
            navigation.removeClass('in');
            navigation.css('height', 0);
        });
})(window.jQuery);
