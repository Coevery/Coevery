/*
 * MoonCake v1.3.1 - Template JS
 *
 * This file is part of MoonCake, an Admin template build for sale at ThemeForest.
 * For questions, suggestions or support request, please mail me at maimairel@yahoo.com
 *
 * Development Started:
 * July 28, 2012
 * Last Update:
 * December 07, 2012
 *
 */

; (function ($, window, document, undefined) {

    var MoonCake = function (document) {
        this.document = $(document);
    }

    MoonCake.prototype = {

        version: '1.0',

        defaults: {
            showSidebarToggleButton: true,
            fixedSidebar: false
        },

        init: function (options) {

            this.options = $.extend({}, this.defaults, options);

            this.bindEventHandlers();

            this.updateSidebarNav($('#sidebar #navigation > ul > li.active').first(), true);

            this.options.fixedSidebar && $.fn.affix && $('#sidebar').affix({
                offset: {
                    top: 0
                }
            });

            return this;
        },

        ready: function (fn) {
            this.document.ready($.proxy(function () {
                fn.call(this.document, this);
            }, this));

            return this;
        },

        bindEventHandlers: function () {
            // Search and Dropdown-menu inputs
            $('#header #header-search .search-query')
				.add($('.dropdown-menu')
				.find(':input'))
				.on('click.template', function (e) {
				    e.stopPropagation();
				});

            var self = this;
           
            $('#sidebar #navigation > ul > li')
				.on('click.template', ' > a, > span', function (e) {
				    $('#navigation').collapse('hide');
				    e.stopPropagation();
				});

            $('#nav-selector .dropdown-menu')
                .on('click.template', ' > li', function(e) {
                    var target = $(this).attr('data-nav-target');
                    var parent = $(this).attr('data-nav-parent');
                    $(parent).find('.collapse').collapse({ toggle: false });
                    $(target).collapse({ toggle: false });
                    if ($(this).attr('data-nav-target') != '#'+$("#first-menu-title").attr('title')) {
                        $(parent).find('.collapse').collapse('hide');
                    }
                    $(target).collapse('show');
                });

            // Collapsible Boxes
            $('.widget .widget-header [data-toggle=widget]')
			.each(function (i, element) {
			    var p = $(this).parents('.widget');
			    if (!p.children('.widget-inner-wrap').length) {
			        p.children(':not(.widget-header)')
						.wrapAll($('<div></div>').addClass('widget-inner-wrap'));
			    }
			}).on('click', function (e) {
			    var p = $(this).parents('.widget');
			    if (p.hasClass('collapsed')) {
			        p.removeClass('collapsed')
						.children('.widget-inner-wrap').hide().slideDown(250);
			    } else {
			        p.children('.widget-inner-wrap').slideUp(250, function () {
			            p.addClass('collapsed');
			        });
			    }
			    e.preventDefault();
			});
        },

        updateSidebarNav: function (nav, init) {
            var hasInnerNav = !!nav.children('.inner-nav').length;
            nav
				.siblings().removeClass('active open')
			.end()
				.addClass('active').toggleClass('open');

            !init && $('#content')
				.toggleClass('sidebar-minimized', !hasInnerNav);

            $('#sidebar-toggle')
				.toggleClass('disabled', !hasInnerNav)
				.toggleClass('toggled', $('#content').hasClass('sidebar-minimized'));

            nav = nav.children('.inner-nav').get(0);
            $('#wrapper #sidebar #navigation > ul')
				.css('minHeight', nav ? $(nav).outerHeight() : '');
        }
    };

    $.template = new MoonCake(document).ready(function (template) {

        template.init($('body').data());

    });


})(jQuery, window, document);