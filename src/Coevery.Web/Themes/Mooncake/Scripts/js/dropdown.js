(function($) {

    "use strict";

    var change = function(e) {

        var $parent,
            $menu,
            $toggle,
            selector,
            text = '',
            $items;

        $menu = $(this).closest('.dropdown-menu');

        $toggle = $menu.parent().find('[data-label-placement]');

        if (!$toggle || !$toggle.length) {
            $toggle = $menu.parent().find(toggle);
        }

        if (!$toggle || !$toggle.length || $toggle.data('placeholder') === false)
            return; // do nothing, no control

        ($toggle.data('placeholder') == undefined && $toggle.data('placeholder', $.trim($toggle.text())));
        text = $.data($toggle[0], 'placeholder');

        $items = $menu.find('li > input:checked');

        if ($items.length) {
            text = [];
            $items.each(function() {
                var str = $(this).parent().find('label').eq(0),
                    label = str.find('.data-label');

                if (label.length) {
                    var p = $('<p></p>');
                    p.append(label.clone());
                    str = p.html();
                } else {
                    str = str.html();
                }


                str && text.push($.trim(str));
            });

            text = text.length < 4 ? text.join(', ') : text.length + ' selected';
        }

        var caret = $toggle.find('.caret');

        $toggle.html(text || '&nbsp;');
        $toggle.attr("title", $toggle.text());
        if (caret.length)
            $toggle.append(' ') && caret.appendTo($toggle);

    };

    $(document)
        .on('click.dropdown-menu', function(e) {
            e.stopPropagation();
        })
        .on('click.dropdown-menu', '.dropdown-menu > li > input[type="checkbox"] ~ label, .dropdown-menu > li > input[type="checkbox"], .dropdown-menu.noclose > li', function(e) {
            e.stopPropagation();
        })
        .on('change.dropdown-menu', '.dropdown-menu > li > input[type="checkbox"], .dropdown-menu > li > input[type="radio"]', change);
})(window.jQuery);
