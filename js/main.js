function is_touch_device() {
return !!('ontouchstart' in window);
}
$(window).load(function(){
	$(".menu").sticky({ topSpacing: 0 });
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