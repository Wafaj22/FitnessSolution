(function($) {

    "use strict";

    /*
	|--------------------------------------------------------------------------
	| Template Name: Premise
	|--------------------------------------------------------------------------
	|--------------------------------------------------------------------------
	| TABLE OF CONTENTS:
	|--------------------------------------------------------------------------
	|
	| ## Scripts initialization
	| ## Preloader
	| ## Navigation
	| ## Sticky Header
	| ## Scroll Up
	| ## Portfolio
	| ## Owl Carousel
	| ## Smooth Scroll
	| ## Form Validator
	| ## Portfolio Load More
    | ## Accordian
	| ## Tamjid Counter
	|
	*/

    /*--------------------------------------------------------------
    ## Scripts initialization
    --------------------------------------------------------------*/
    $(window).on('load', function() {
        $(window).trigger("scroll");
        $(window).trigger("resize");
        preloader_setup();
    });

    $(document).ready(function() {
        $(window).trigger("resize");
        menu_toggle();
        scroll_up_setup();
        portfolio_ms_setup();
        owl_carousel_setup();
        smooth_scroll_setup();
        form_validator_setup();
        portfolio_loadmore_setup();
        accordian_setup();
        counterUp();
        new WOW().init();
        $('#parallax-bg').parallax("50%", 0.2);
    });

    $(window).on('resize', function() {
        portfolio_ms_setup();
    });

    $(window).on('scroll', function() {
        sticky_header_setup();
    });

    /*--------------------------------------------------------------
    ## Preloader
    --------------------------------------------------------------*/
    function preloader_setup() {

        $("body").imagesLoaded(function() {
            $(".p-preloader-wave").fadeOut();
            $("#p-preloader").delay(200).fadeOut("slow").remove();
        });

    }


    /*--------------------------------------------------------------
    ## Navigation
    --------------------------------------------------------------*/
    function menu_toggle() {

        $(".menu-toggle").on('click', function() {

            if ($(this).hasClass('toggle-on')) {

                $('.header-wrap').toggleClass("full-screen-nav-transition");
                setTimeout(function() {
                    $('.header-wrap').toggleClass("full-screen-nav");
                }, 300);

            } else {

                $('.header-wrap').toggleClass("full-screen-nav");
                setTimeout(function() {
                    $('.header-wrap').toggleClass("full-screen-nav-transition");
                }, 300);

            }


            $(this).toggleClass("toggle-on");

        });

        //Mobile Menu Toggleing
        $(".mm-togle").on('click', function() {
            $(this).siblings('.sub-menu').slideToggle(300);
        });

    }


    /*--------------------------------------------------------------
    ## Sticky Header
    --------------------------------------------------------------*/
    function sticky_header_setup() {

        if ($(window).scrollTop() > 10) {
            $('.navbar-collapse').addClass('sticky');
        } else {
            $('.navbar-collapse').removeClass('sticky');
        }

    }



    /*--------------------------------------------------------------
    ## Scroll Up
    --------------------------------------------------------------*/
    function scroll_up_setup() {

        $('#scroll-to-up').on('click', function(e) {
            e.preventDefault();
            $('html,body').animate({
                scrollTop: 0
            }, 700);
        });

    }


    /*--------------------------------------------------------------
    ## Portfolio
    --------------------------------------------------------------*/
    function portfolio_ms_setup() {

        $('.portfolio').isotope({
            itemSelector: '.portfolio-item',
            transitionDuration: '0.60s',
            masonry: {
                percentPosition: true
            }
        });

        /* Active Class of Portfolio*/
        $('.portfolio-filter ul li').on('click', function(event) {
            $(this).siblings('.active').removeClass('active');
            $(this).addClass('active');
            event.preventDefault();
        });

        /*=== Portfolio filtering ===*/
        $('.portfolio-filter ul').on('click', 'a', function() {
            var filterElement = $(this).attr('data-filter');
            $(this).parents(".portfolio-filter").next().isotope({
                filter: filterElement
            });
        });

    }

    /*--------------------------------------------------------------
    ## Media
    --------------------------------------------------------------*/
    function media_ms_setup() {

        $('.widget').isotope({
            itemSelector: '.media',
            transitionDuration: '0.60s',
            masonry: {
                percentPosition: true
            }
        });

        /* Active Class of Portfolio*/
        $('.portfolio-filter ul li').on('click', function(event) {
            $(this).siblings('.active').removeClass('active');
            $(this).addClass('active');
            event.preventDefault();
        });

        /*=== Portfolio filtering ===*/
        $('.portfolio-filter ul').on('click', 'a', function() {
            var filterElement = $(this).attr('data-filter');
            $(this).parents(".portfolio-filter").next().isotope({
                filter: filterElement
            });
        });

    }


    /*--------------------------------------------------------------
    ## Owl Carousel
    --------------------------------------------------------------*/
    function owl_carousel_setup() {

        $("#blog-slider").owlCarousel({
            singleItem: true,
            pagination: false,
            navigation: true,
            navigationText: ['<i class="fa fa-angle-left"></i>', '<i class="fa fa-angle-right"></i>'],
            transitionStyle: "fade",
            autoPlay: true
        });

        $("#testimonial").owlCarousel({
            singleItem: true,
            pagination: true,
            paginationSpeed: 100,
            navigation: false,
            transitionStyle: "fade",
            autoPlay: true
        });

    }


    /*--------------------------------------------------------------
    ## Owl Carousel
    --------------------------------------------------------------*/
    function owl_carousel_setup() {

        $("#blog-slider").owlCarousel({
            singleItem: true,
            pagination: false,
            navigation: true,
            navigationText: ['<i class="fa fa-angle-left"></i>', '<i class="fa fa-angle-right"></i>'],
            transitionStyle: "fade",
            autoPlay: true
        });

        $("#testimonial").owlCarousel({
            singleItem: true,
            pagination: true,
            paginationSpeed: 100,
            navigation: false,
            transitionStyle: "fade",
            autoPlay: true
        });

    }


    /*--------------------------------------------------------------
    ## Smooth Scroll
    --------------------------------------------------------------*/
    function smooth_scroll_setup() {

        if (typeof smoothScroll == 'object') {
            smoothScroll.init();
        }

    }


    /*--------------------------------------------------------------
    ## Form Validator
    --------------------------------------------------------------*/
    function validateEmail(email) {

        var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(email);

    }

    function form_validator_setup() {

        $('#contact').on('submit', function(event) {
            event.preventDefault();

            var formData = {
                'name': $('#contact [name=name]').val(),
                'email': $('#contact [name=email]').val(),
                'phone': $('#contact [name=phone]').val(),
                'website': $('#contact [name=website]').val(),
                'message': $('#contact [name=message]').val(),
            };

            var valid = true;
            if (!formData.name) {
                $('#contact [name=name]').focus();
                //alert('Name required.');
                valid = false;
            }

            if (!(formData.email && validateEmail(formData.email))) {
                $('#contact [name=email]').focus();
                //alert('Email required.');
                valid = false;
            }

            if (!formData.message) {
                $('#contact [name=message]').focus();
                // alert('Message required.');
                valid = false;
            }

            if (valid == false) {
                return;
            }

            $('#contact [type=submit]').text('Sending..');
            $.ajax({
                    type: $('#contact').attr('method'), // define the type of HTTP verb we want to use (POST for our form)
                    url: $('#contact').attr('action'), // the url where we want to POST
                    data: formData, // our data object
                })
                .done(function(data) {
                    $('#contact')[0].reset();
                    $('#contact [type=submit]').text(data);
                    setTimeout(function() {
                        $('#contact [type=submit]').text('Send Message');
                    }, 4000);
                });

        }); //end form submit handler

    }


    /*--------------------------------------------------------------
    ## Portfolio Load More
    --------------------------------------------------------------*/
    function no_more_portfolio($button) {

        $button.text('No more portfolio item');

        setTimeout(function() {
            $button.slideUp(300);
        }, 4000);

    }

    function portfolio_loadmore_setup() {

        $(document).on('click', ".load-more-btn", function() {
            var load_more_button = $(this);
            var loaded = parseInt($(this).attr('data-loaded'));
            var maxload = parseInt($(this).attr('data-maxload'));

            if (maxload <= loaded) {
                no_more_portfolio(load_more_button);
                return;
            }

            load_more_button.text("Loading...");
            $.ajax({
                'url': $(this).data('source'),
                'success': function(response) {
                    var $items = $(response);

                    $('#portfolio_box')
                        .append($items)
                        .isotope('appended', $items);

                    loaded++;
                    load_more_button
                        .attr('data-loaded', loaded)
                        .text("Load more");

                    if (maxload <= loaded) {
                        no_more_portfolio(load_more_button);
                    }
                }
            });
        });

    }

    /*--------------------------------------------------------------
    ## Accordian
    --------------------------------------------------------------*/
    function accordian_setup() {

        $(".acc-btn").on('click', function() {
            $(this).siblings('.acc-body').slideToggle("200");
            $(this).children('.icon-arrows_circle_plus').toggleClass("rotate-plus")
        });

    }

    /*--------------------------------------------------------------
    ## Tamjid Counter
    --------------------------------------------------------------*/
    function counterUp() {

        $('.counter').tamjidCounter({
            duration: 3000,
            delay: 0,
            easing: 'swing'
        });

    }


})(jQuery); // End of use strict