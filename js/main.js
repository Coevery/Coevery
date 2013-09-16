function is_touch_device() {
return !!('ontouchstart' in window);
}
$(window).load(function(){
	$(".menu").sticky({ topSpacing: 0 });
});
	
jQuery(function ($) {

    var slides = [// Slideshow Images
        { image: '/images/slide1.jpg' },
        { image: '/images/slide4.jpg' },
        { image: '/images/slide3.jpg' }
    ];

    var windowWidth = $(window).width();
    if (windowWidth < 960) {
        slides = [
            { image: '/images/slide1_960.jpg' },
            { image: '/images/slide4_960.jpg' },
            { image: '/images/slide3_960.jpg' }
        ];
    }
    else if (windowWidth < 1350) {
        slides = [
            { image: '/images/slide1_1350.jpg' },
            { image: '/images/slide4_1350.jpg' },
            { image: '/images/slide3_1350.jpg' }
        ];
    }

    $.supersized({
        // Functionality
        slideshow: 1,			// Slideshow on/off
        autoplay: 1,			// Slideshow starts playing automatically
        start_slide: 1,			// Start slide (0 is random)
        stop_loop: 0,			// Pauses slideshow on last slide
        random: 0,			// Randomize slide order (Ignores start slide)
        slide_interval: 8000,		// Length between transitions
        transition: 1, 			// 0-None, 1-Fade, 2-Slide Top, 3-Slide Right, 4-Slide Bottom, 5-Slide Left, 6-Carousel Right, 7-Carousel Left
        transition_speed: 1000,		// Speed of transition
        new_window: 1,			// Image links open in new window/tab
        pause_hover: 0,			// Pause slideshow on hover
        keyboard_nav: 1,			// Keyboard navigation on/off
        performance: 1,			// 0-Normal, 1-Hybrid speed/quality, 2-Optimizes image quality, 3-Optimizes transition speed // (Only works for Firefox/IE, not Webkit)
        image_protect: 1,			// Disables image dragging and right click with Javascript

        // Size & Position						   
        min_width: 0,			// Min width allowed (in pixels)
        min_height: 0,			// Min height allowed (in pixels)
        vertical_center: 1,			// Vertically center background
        horizontal_center: 1,			// Horizontally center background
        fit_always: 0,			// Image will never exceed browser width or height (Ignores min. dimensions)
        fit_portrait: 1,			// Portrait images will not exceed browser height
        fit_landscape: 0,			// Landscape images will not exceed browser width

        // Components							
        slide_links: 'blank',	// Individual links for each slide (Options: false, 'num', 'name', 'blank')
        thumb_links: 1,			// Individual thumb links for each slide
        thumbnail_navigation: 0,			// Thumbnail navigation
        slides: slides,

        // Theme Options			   
        progress_bar: 1,			// Timer for each slide							
        mouse_scrub: 0
    });
    setTimeout($("#defaultslide").fadeOut("slow").remove(), 3000);
});

jQuery(document).ready(function() {
	$('.menu ul li a, .cta-left a').click(function() {
		$('html, body').animate({scrollTop: $(this.hash).offset().top}, 1000);
		return false;
	});
	$('.menu ul li').removeClass('active').find('a[href="#"]').addClass('active');
    
	$("<select />").appendTo(".nav");

	$("<option />", {
	    "selected": "selected",
	    "value": "",
	    "text": "Go to..."
	}).appendTo("nav select");

    // Populate dropdown with menu items
	$(".nav li").each(function () {

	    var depth = $(this).parents('ul').length - 1;

	    var indent = '';
	    if (depth > 0) { indent = ' - '; }
	    if (depth > 1) { indent = ' - - '; }
	    if (depth > 2) { indent = ' - - -'; }
	    if (depth > 3) { indent = ' - - - -'; }


	    var el = $(this).children('a');
	    $("<option />", {
	        "value": el.attr("href"),
	        "text": (indent + el.text()),
	        "class": "scroll",
	    }).appendTo(".nav select");
	});

	$(".nav select").change(function () {
	    var full_url = $(this).val();
	    var parts = full_url.split("#");
	    var trgt = parts[1];
	    var target_offset = jQuery("#" + trgt).offset();
	    var target_top = target_offset.top;
	    jQuery('html, body').animate({ scrollTop: target_top }, 900);
	});

	jQuery('.view-image').fancybox({
	    'titlePosition': 'over'
	});
});
$(window).bind('scroll resize', function() {	
	var currentSection = null;
	$('.rowd').each(function(){
		var element = $(this).attr('id');		
		if($(window).scrollTop() >= $('#'+element).offset().top - 70)
		{
			currentSection = element;
		}
	});
$('.menu ul li').removeClass('active').find('a[href="#'+currentSection+'"]').parent().addClass('active');
});