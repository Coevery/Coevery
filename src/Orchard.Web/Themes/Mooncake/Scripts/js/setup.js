/*
 * MoonCake v1.3.1 - Template Setup JS
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

;(function( $, window, document, undefined ) {


    $(document).ready(function(e) {

        // Restrict any other content beside numbers
        $(":input[data-accept=numbers]").keydown(function(event) {

            // Allow: backspace, delete, tab, escape, and enter
            if (event.keyCode == 46 || event.keyCode == 8 || event.keyCode == 9 || event.keyCode == 27 || event.keyCode == 13 ||
                // Allow: Ctrl+A
                (event.keyCode == 65 && event.ctrlKey === true) ||
                // Allow: home, end, left, right
                (event.keyCode >= 35 && event.keyCode <= 39)) {
                // let it happen, don't do anything
                return;
            } else {
                // Ensure that it is a number and stop the keypress
                if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                    event.preventDefault();
                }
            }
        });

        // Initialize Bootstrap Tooltips
        $.fn.tooltip && $('[rel="tooltip"]').tooltip();

        // Initialize Bootstrap Popovers
        $.fn.popover && $('[rel="popover"]').popover();

        // Style checkboxes and radios
        $.fn.uniform && $(':radio.uniform, :checkbox.uniform').uniform();

        // IE Placeholder
        $.fn.placeholder && $('[placeholder]').placeholder();

        // Bootstrap Dropdown Workaround for touch devices
        $(document).on('touchstart.dropdown.data-api', '.dropdown-menu', function(e) { e.stopPropagation(); });

        // Gridview row menu
        $(document)
            .on('mouseover.dropdown-menu', '.ngGrid .ngRow', function(e) {
                $(this).find('.row-actions').removeClass('hide');
            });
        $(document)
            .on('mouseout.dropdown-menu', '.ngGrid .ngRow', function(e) {
                $(this).find('.row-actions').addClass('hide');
            });
    });

}) (jQuery, window, document);