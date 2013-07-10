(function ($) {

    "use strict";
    $('#header #header-search .search-query')
				.add($('.dropdown-menu')
				.find(':input'))
				.on('click.template', function (e) {
				    e.stopPropagation();
				});
})(window.jQuery);
