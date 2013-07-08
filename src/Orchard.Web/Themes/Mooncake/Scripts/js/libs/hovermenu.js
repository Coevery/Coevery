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

})(window.jQuery);
