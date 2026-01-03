/* ==============
 ========= js documentation ==========================

 * theme name: Itify
 * version: 1.0
 * description: Software Development & It Solutions
 * author: Pixelaxis
 * author-url: https://themeforest.net/user/pixelaxis

    ==================================================

     01. get device with and initial scroll position
     -------------------------------------------------
     02. preloader
     -------------------------------------------------
     03. data background
     -------------------------------------------------
     04. data before background
     -------------------------------------------------
     05. custom cursor
     -------------------------------------------------
     06. following cursor animated button
     -------------------------------------------------
     07. scroll to top with progress
     -------------------------------------------------
     08. offcanvas nav
     -------------------------------------------------
     09. on scroll effects
     -------------------------------------------------
     10. on resize effects
     -------------------------------------------------
     11. footer copyright year
     -------------------------------------------------
     12. odometer counter
     -------------------------------------------------
     13. video popup
     -------------------------------------------------
     14. testimonial slider
     -------------------------------------------------
     15. vanilla tilt animation
     -------------------------------------------------
     16. latest post slider
     -------------------------------------------------
     17. sponsor slider
     -------------------------------------------------
     18. case study slider
     -------------------------------------------------
     19. gallery slider
     -------------------------------------------------
     20. transform slider
     -------------------------------------------------
     21. team slider
     -------------------------------------------------
     22. case study tab
     -------------------------------------------------
     23. blog post filter
     -------------------------------------------------
     24. register gsap
     -------------------------------------------------
     25. gsap null config
     -------------------------------------------------
     26. target section with gsap
     -------------------------------------------------
     27. parallax image with gsap
     -------------------------------------------------
     28. story image hover animation
     -------------------------------------------------
     29. service banner animation
     -------------------------------------------------
     30. career banner animation
     -------------------------------------------------
     31. img fade animation
     -------------------------------------------------
     32. fade top gsap animation
     -------------------------------------------------
     33. fade left gsap animation
     -------------------------------------------------
     34. fade right gsap animation
     -------------------------------------------------
     35. fade bottom gsap animation
     -------------------------------------------------
     36. appear down
     -------------------------------------------------
     37. title animation
     -------------------------------------------------
     38. footer shape animation
     -------------------------------------------------
     39. lenis smooth scroll

    ==================================================
============== */

(function ($) {
  "use strict";

  jQuery(document).ready(function () {
    /**
     * ======================================
     * 01. get device with and initial scroll position
     * ======================================
     */
    let device_width = window.innerWidth;
    let initialScroll = $(window).scrollTop();

    /**
     * ======================================
     * 02. preloader
     * ======================================
     */

    if ($("#preloader").length > 0) {
      $("#preloader").hide();
    }

    /**
     * ======================================
     * 03. data background
     * ======================================
     */
    $("[data-background]").each(function () {
      var backgroundImages = $(this).attr("data-background").split(",");
      var cssValue = backgroundImages
        .map(function (image) {
          return 'url("' + image.trim() + '")';
        })
        .join(",");

      $(this).css("background-image", cssValue);
    });

    /**
     * ======================================
     * 04. data before background
     * ======================================
     */
    $("[data-before-image]").each(function () {
      var backgroundImage = $(this).attr("data-before-image");
      $(this).css("position", "relative");
      $(this)
        .prepend('<div class="before-image"></div>')
        .find(".before-image")
        .css({
          "background-image": 'url("' + backgroundImage + '")',
        });
    });

    /**
     * ======================================
     * 05. custom cursor
     * ======================================
     */
    if ($(".mouseCursor").length > 0) {
      function itCursor() {
        var myCursor = jQuery(".mouseCursor");
        if (myCursor.length) {
          if ($("body")) {
            const e = document.querySelector(".cursor-inner"),
              t = document.querySelector(".cursor-outer");
            let n,
              i = 0,
              o = !1;
            (window.onmousemove = function (s) {
              o ||
                (t.style.transform =
                  "translate(" + s.clientX + "px, " + s.clientY + "px)"),
                (e.style.transform =
                  "translate(" + s.clientX + "px, " + s.clientY + "px)"),
                (n = s.clientY),
                (i = s.clientX);
            }),
              $("body").on(
                "mouseenter",
                "button, a, .cursor-pointer",
                function () {
                  e.classList.add("cursor-hover"),
                    t.classList.add("cursor-hover");
                }
              ),
              $("body").on(
                "mouseleave",
                "button, a, .cursor-pointer",
                function () {
                  ($(this).is("a", "button") &&
                    $(this).closest(".cursor-pointer").length) ||
                    (e.classList.remove("cursor-hover"),
                    t.classList.remove("cursor-hover"));
                }
              ),
              (e.style.visibility = "visible"),
              (t.style.visibility = "visible");
          }
        }
      }
      itCursor();
    }

    $("h1, h2, h3, h4, h5, h6, .cur-lg").on("mouseenter", function () {
      $(".mouseCursor").addClass("cursor-big");
    });

    $("h1, h2, h3, h4, h5, h6, .cur-lg").on("mouseleave", function () {
      $(".mouseCursor").removeClass("cursor-big");
    });

    /**
     * ======================================
     * 06. following cursor animated button
     * ======================================
     */
    if ($(".btn-anim").length > 0) {
      const ELEMENTS = $(".btn-anim");

      ELEMENTS.each(function () {
        const element = $(this);

        element.on("mouseenter", function (e) {
          const span = element.find("span");
          span.css({
            left: e.pageX - element.offset().left + "px",
            top: e.pageY - element.offset().top + "px",
          });
        });

        element.on("mouseleave", function (e) {
          const span = element.find("span");
          span.css({
            left: e.pageX - element.offset().left + "px",
            top: e.pageY - element.offset().top + "px",
          });
        });
      });
    }

    /**
     * ======================================
     * 07. scroll to top with progress
     * ======================================
     */
    if ($(".progress-wrap").length > 0) {
      var progressPath = document.querySelector(".progress-wrap path");
      var pathLength = progressPath.getTotalLength();
      progressPath.style.transition = progressPath.style.WebkitTransition =
        "none";
      progressPath.style.strokeDasharray = pathLength + " " + pathLength;
      progressPath.style.strokeDashoffset = pathLength;
      progressPath.getBoundingClientRect();
      progressPath.style.transition = progressPath.style.WebkitTransition =
        "stroke-dashoffset 10ms linear";
      var updateProgress = function () {
        var scroll = $(window).scrollTop();
        var height = $(document).height() - $(window).height();
        var progress = pathLength - (scroll * pathLength) / height;
        progressPath.style.strokeDashoffset = progress;
      };
      updateProgress();
      $(window).scroll(updateProgress);
      var offset = 50;
      var duration = 1500;
      $(window).on("scroll", function () {
        if ($(this).scrollTop() > offset) {
          $(".progress-wrap").addClass("active-progress");
        } else {
          $(".progress-wrap").removeClass("active-progress");
        }
      });
      $(".progress-wrap").on("click", function (event) {
        event.preventDefault();
        $("html, body").animate(
          {
            scrollTop: 0,
          },
          duration
        );
        return false;
      });

      if (initialScroll >= 50) {
        $(".progress-wrap").addClass("active-progress");
      }
    }

    /**
     * ======================================
     * 08. offcanvas nav
     * ======================================
     */
    $(".open-offcanvas-nav").on("click", function () {
      $(".nav-fade").each(function (i) {
        $(this).css("animation-delay", 1.8 + 0.2 * 1 * i + "s");
      });

      $(".offcanvas-menu").addClass("show-offcanvas-menu");
      $(".offcanvas-menu__wrapper").removeClass("nav-fade-active");
      $("body").addClass("body-active");
    });

    $(".close-offcanvas-menu").on("click", function () {
      setTimeout(function () {
        $(".offcanvas-menu").removeClass("show-offcanvas-menu");
      }, 900);
      $(".offcanvas-menu__wrapper").addClass("nav-fade-active");
      $("body").removeClass("body-active");
      $(".navbar__sub-menu").slideUp(300);
      $(".navbar__dropdown-label").removeClass("navbar__item-active");
    });

    if ($(".offcanvas-nav").length) {
      $(".offcanvas-menu .navbar__dropdown-label").on("click", function () {
        $(this).parent().siblings().find(".navbar__sub-menu").slideUp(300);
        $(this)
          .parent()
          .siblings()
          .find(".navbar__dropdown-label")
          .removeClass("navbar__item-active");
        $(this).siblings(".navbar__sub-menu").slideToggle(300);
        $(this).toggleClass("navbar__item-active");
      });
    }

    /**
     * ======================================
     * 09. on scroll effects
     * ======================================
     */
    $(window).on("scroll", function () {
      var scroll = $(window).scrollTop();
      if (scroll < 50) {
        $(".primary-navbar").removeClass("navbar-active");
      } else {
        $(".primary-navbar").addClass("navbar-active");
      }
    });

    if (initialScroll >= 50) {
      $(".primary-navbar").addClass("navbar-active");
    }

    /**
     * ======================================
     * 10. on resize effects
     * ======================================
     */
    $(window).on("resize", function () {
      $("body").removeClass("body-active");

      // offcanvas nav
      setTimeout(function () {
        $(".offcanvas-menu").removeClass("show-offcanvas-menu");
      }, 900);
      $(".offcanvas-menu__wrapper").addClass("nav-fade-active");
      $(".navbar__sub-menu").slideUp(300);
      $(".navbar__dropdown-label").removeClass("navbar__item-active");
    });

    /**
     * ======================================
     * 11. footer copyright year
     * ======================================
     */
    if ($("#copyrightYear").length > 0) {
      $("#copyrightYear").text(new Date().getFullYear());
    }

    /**
     * ======================================
     * 12. odometer counter
     * ======================================
     */
    $(".odometer").appear(function (e) {
      var odo = $(".odometer");
      odo.each(function () {
        var countNumber = $(this).attr("data-odometer-final");
        $(this).html(countNumber);
      });
    });

    /**
     * ======================================
     * 13. video popup
     * ======================================
     */
    if (document.querySelector(".video-btn") !== null) {
      $(".video-btn").magnificPopup({
        disableOn: 768,
        type: "iframe",
        mainClass: "mfp-fade",
        removalDelay: 160,
        preloader: false,
        fixedContentPos: false,
      });
    }

    /**
     * ======================================
     * 14. testimonial slider
     * ======================================
     */
    $(".tp-testimonial-slider")
      .not(".slick-initialized")
      .slick({
        infinite: true,
        autoplay: true,
        focusOnSelect: false,
        slidesToShow: 1,
        speed: 1000,
        autoplaySpeed: 4000,
        slidesToScroll: 1,
        arrows: true,
        prevArrow: $(".prev-testimonial"),
        nextArrow: $(".next-testimonial"),
        dots: true,
        appendDots: $(".tp-testimonial-pagination"),
        centerMode: false,
        customPaging: function (slider, i) {
          var imageUrls = [
            "assets/images/testimonial/avatar-one.png",
            "assets/images/testimonial/avatar-two.png",
            "assets/images/testimonial/avatar-three.png",
            "assets/images/testimonial/avatar-four.png",
            "assets/images/testimonial/avatar-five.png",
          ];
          return '<img src="' + imageUrls[i] + '" alt="Dot ' + i + '">';
        },
      });

    var totalSlides = $(".tp-testimonial-slider").slick("getSlick").slideCount;

    $(".total-slide").text(totalSlides);

    $(".tp-testimonial-slider").on(
      "afterChange",
      function (event, slick, currentSlide) {
        $(".current-slide").text(currentSlide + 1);
      }
    );

    /**
     * ======================================
     * 15. vanilla tilt animation
     * ======================================
     */

    // button animation
    let topylotilt = document.querySelectorAll(".btn-anim");

    if (topylotilt) {
      VanillaTilt.init(document.querySelectorAll(".btn-anim"), {
        max: 15,
        speed: 3000,
        perspective: 400,
      });
    }

    // card animation
    let topytilt = document.querySelectorAll(".topy-tilt");

    if (topytilt) {
      VanillaTilt.init(document.querySelectorAll(".topy-tilt"), {
        max: 5,
        speed: 3000,
      });
    }

    /**
     * ======================================
     * 16. latest post slider
     * ======================================
     */
    $(".tp-lp-slider").not(".slick-initialized").slick({
      infinite: true,
      autoplay: true,
      focusOnSelect: false,
      slidesToShow: 1,
      speed: 1000,
      autoplaySpeed: 4000,
      slidesToScroll: 1,
      arrows: false,
      dots: false,
      variableWidth: true,
      centerMode: false,
    });

    /**
     * ======================================
     * 17. sponsor slider
     * ======================================
     */
    $(".sponsor__slider")
      .not(".slick-initialized")
      .slick({
        infinite: true,
        autoplay: true,
        focusOnSelect: true,
        slidesToShow: 6,
        speed: 1000,
        autoplaySpeed: 3000,
        slidesToScroll: 1,
        arrows: false,
        dots: false,
        centerMode: true,
        centerPadding: "0px",
        responsive: [
          {
            breakpoint: 1700,
            settings: {
              slidesToShow: 4,
            },
          },
          {
            breakpoint: 1400,
            settings: {
              slidesToShow: 3,
            },
          },
          {
            breakpoint: 992,
            settings: {
              slidesToShow: 2,
            },
          },
          {
            breakpoint: 576,
            settings: {
              slidesToShow: 1,
            },
          },
        ],
      });

    /**
     * ======================================
     * 18. case study slider
     * ======================================
     */
    $(".tp-study-slider")
      .not(".slick-initialized")
      .slick({
        infinite: true,
        autoplay: true,
        focusOnSelect: false,
        slidesToShow: 1,
        speed: 1000,
        autoplaySpeed: 4000,
        slidesToScroll: 1,
        arrows: true,
        prevArrow: $(".prev-study"),
        nextArrow: $(".next-study"),
        dots: false,
        variableWidth: true,
        centerMode: false,
      });

    /**
     * ======================================
     * 19. gallery slider
     * ======================================
     */
    $(".tp-gallery__single").magnificPopup({
      delegate: "a",
      type: "image",
      mainClass: "mfp-img-mobile",
      gallery: {
        enabled: true,
        navigateByImgClick: true,
        preload: [0, 1],
      },
    });

    $(".tp-gallery-slider").not(".slick-initialized").slick({
      infinite: true,
      autoplay: true,
      focusOnSelect: false,
      slidesToShow: 1,
      speed: 1000,
      autoplaySpeed: 4000,
      slidesToScroll: 1,
      arrows: false,
      dots: false,
      variableWidth: true,
      centerMode: false,
    });

    /**
     * ======================================
     * 20. transform slider
     * ======================================
     */
    $(".transform-slider").not(".slick-initialized").slick({
      infinite: true,
      autoplay: true,
      focusOnSelect: false,
      slidesToShow: 1,
      speed: 1000,
      autoplaySpeed: 4000,
      slidesToScroll: 1,
      arrows: false,
      dots: false,
      variableWidth: true,
      centerMode: false,
    });

    /**
     * ======================================
     * 21. team slider
     * ======================================
     */
    $(".team-slider")
      .not(".slick-initialized")
      .slick({
        infinite: true,
        autoplay: true,
        focusOnSelect: false,
        slidesToShow: 1,
        speed: 1000,
        autoplaySpeed: 4000,
        slidesToScroll: 1,
        arrows: true,
        prevArrow: $(".prev-team"),
        nextArrow: $(".next-team"),
        dots: false,
        centerMode: false,
      });

    var totalSlides = $(".team-slider").slick("getSlick").slideCount;

    $(".total-slide-t").text(totalSlides);

    $(".team-slider").on("afterChange", function (event, slick, currentSlide) {
      $(".current-slide-t").text(currentSlide + 1);
    });

    /**
     * ======================================
     * 22. case study tab
     * ======================================
     */
    $(".c-tab-single").hide();
    $(".c-tab-single:first").show();
    $(".study-btn").on("click", function () {
      $(".study-btn").removeClass("study-btn-active");
      $(this).addClass("study-btn-active");
      $(".c-tab-single").hide();
      var activePlan = $(this).data("target");
      $(activePlan).fadeIn(500);
      return false;
    });

    /**
     * ======================================
     * 23. blog post filter
     * ======================================
     */
    function masonryMain() {
      if ($(".masonry-grid").length) {
        var $grid = $(".masonry-grid").isotope({
          layoutMode: "fitRows",
          itemSelector: ".grid-item-main",
        });

        var filterFns = {
          all: function () {
            return true;
          },
        };

        $(".post-filter__wrapper").on("click", "button", function () {
          var filterValue = $(this).attr("data-filter");
          filterValue = filterFns[filterValue] || filterValue;
          $grid.isotope({
            filter: filterValue,
          });
        });

        $(".post-filter__wrapper").each(function (i, buttonGroup) {
          var $buttonGroup = $(buttonGroup);
          $buttonGroup.on("click", "button", function () {
            $buttonGroup.find(".active").removeClass("active");
            $(this).addClass("active");
          });
        });

        $grid.isotope({
          transitionDuration: "1200ms",
        });
      }
    }

    masonryMain();

    /**
     * ======================================
     * 24. register gsap
     * ======================================
     */
    gsap.registerPlugin(ScrollTrigger, ScrollToPlugin, SplitText);

    /**
     * ======================================
     * 25. gsap null config
     * ======================================
     */
    gsap.config({
      nullTargetWarn: false,
      debug: false,
    });

    /**
     * ======================================
     * 26. target section with gsap
     * ======================================
     */
    $('a[href^="#"]').on("click", function (event) {
      event.preventDefault();

      var target = $(this).attr("href");

      gsap.to(window, {
        scrollTo: {
          y: target,
          offsetY: 100,
        },
        duration: 1.5,
        ease: "power3.inOut",
      });
    });

    /**
     * ======================================
     * 27. parallax image with gsap
     * ======================================
     */
    var imageParallax = document.querySelectorAll(".parallax-image");
    if (imageParallax.length > 0) {
      $(".parallax-image").each(function () {
        $(this).wrap(
          '<div class="parallax-image-wrap"><div class="parallax-image-inner"></div></div>'
        );
        $(".parallax-image-wrap").css({
          overflow: "hidden",
        });

        var $animImageParallax = $(this);
        var $aipWrap = $animImageParallax.parents(".parallax-image-wrap");
        var $aipInner = $aipWrap.find(".parallax-image-inner");

        let tl_ImageParallax = gsap.timeline({
          scrollTrigger: {
            trigger: $aipWrap,
            start: "top bottom",
            end: "bottom top",
            scrub: true,
            onEnter: () => animImgParallaxRefresh(),
          },
        });
        tl_ImageParallax.to($animImageParallax, {
          yPercent: 30,
          ease: "none",
        });

        function animImgParallaxRefresh() {
          tl_ImageParallax.scrollTrigger.refresh();
        }

        let tl_aipZoomIn = gsap.timeline({
          scrollTrigger: {
            trigger: $aipWrap,
            start: "top 99%",
          },
        });
        tl_aipZoomIn.from($aipInner, {
          duration: 1.5,
          autoAlpha: 0,
          scale: 1.2,
          ease: Power2.easeOut,
          clearProps: "all",
        });
      });
    }

    /**
     * ======================================
     * 28. story image hover
     * ======================================
     */
    $(".tp-story-thumb:not(:first-child)").hide();

    $(".tp-story__single").on("mouseenter", function () {
      if ($(this).hasClass("active")) {
        return;
      }

      var index = $(this).index();
      gsap.to(".tp-story-thumb", {
        opacity: 0,
        scale: 0,
        duration: 0.4,
        onComplete: function () {
          $(".tp-story-thumb").hide();
          $(".tp-story-thumb").eq(index).show();
          gsap.fromTo(
            ".tp-story-thumb",
            {
              opacity: 0,
              scale: 0,
            },
            {
              opacity: 1,
              scale: 1,
              duration: 0.4,
            }
          );
        },
      });

      $(".tp-story__single").removeClass("active");
      $(this).addClass("active");
    });

    $(".tp-story__single").on("mouseleave", function () {
      gsap.to(".tp-story-thumb", {
        opacity: 1,
        scale: 1,
        duration: 0.4,
      });
    });

    /**
     * ======================================
     * 29. service banner animation
     * ======================================
     */

    if ($(".service-banner").length > 0) {
      var tl = gsap.timeline({
        scrollTrigger: {
          trigger: ".service-banner",
          start: "center center",
          end: "+=100%",
          scrub: true,
          pin: false,
        },
      });
      tl.to(".thumb-one", {
        opacity: 0.3,
        y: "-100%",
        zIndex: -1,
        duration: 2,
      });
      tl.to(
        ".thumb-two",
        {
          opacity: 0.3,
          scale: 2,
          y: "100%",
          zIndex: -1,
          duration: 2,
        },
        0
      );
    }

    /**
     * ======================================
     * 30. career banner animation
     * ======================================
     */

    if ($(".career-banner").length > 0) {
      var tl = gsap.timeline({
        scrollTrigger: {
          trigger: ".career-banner",
          start: "center center",
          end: "+=100%",
          scrub: true,
          pin: false,
        },
      });
      tl.to(".cp-banner-thumb", {
        opacity: 0.1,
        y: "40%",
        duration: 2,
      });
      tl.to(
        ".career-banner",
        {
          "--scale": 3,
          duration: 2,
        },
        0
      );
    }

    /**
     * ======================================
     * 31. img fade animation
     * ======================================
     */
    if ($(".fade-img").length > 0) {
      if (device_width >= 992) {
        gsap.utils.toArray(".fade-img").forEach((el) => {
          var tl = gsap.timeline({
            scrollTrigger: {
              trigger: el,
              start: "center center",
              end: "+=40%",
              scrub: 1,
              pin: false,
              invalidateOnRefresh: true,
            },
          });
          tl.to(el, {
            y: "120px",
            zIndex: "-1",
            duration: 1,
          });
        });
      }
    }

    /**
     * ======================================
     * 32. fade top gsap animation
     * ======================================
     */
    if ($(".fade-wrapper").length > 0) {
      $(".fade-wrapper").each(function () {
        var section = $(this);
        var fadeItems = section.find(".fade-top");

        fadeItems.each(function (index, element) {
          var delay = index * 0.15;

          gsap.set(element, {
            opacity: 0,
            y: 100,
          });

          ScrollTrigger.create({
            trigger: element,
            start: "top 100%",
            end: "bottom 20%",
            scrub: 0.5,
            onEnter: function () {
              gsap.to(element, {
                opacity: 1,
                y: 0,
                duration: 1,
                delay: delay,
              });
            },
            once: true,
          });
        });
      });
    }

    /**
     * ======================================
     * 33. fade left gsap animation
     * ======================================
     */
    if ($(".fade-wrapper").length > 0) {
      $(".fade-wrapper").each(function () {
        var section = $(this);
        var fadeItems = section.find(".fade-left");

        fadeItems.each(function (index, element) {
          var delay = index * 0.15;

          gsap.set(element, {
            opacity: 0,
            x: 100,
          });

          ScrollTrigger.create({
            trigger: element,
            start: "top 100%",
            end: "bottom 20%",
            scrub: 0.5,
            onEnter: function () {
              gsap.to(element, {
                opacity: 1,
                x: 0,
                duration: 1,
                delay: delay,
              });
            },
            once: true,
          });
        });
      });
    }

    /**
     * ======================================
     * 34. fade right gsap animation
     * ======================================
     */
    if ($(".fade-wrapper").length > 0) {
      $(".fade-wrapper").each(function () {
        var section = $(this);
        var fadeItems = section.find(".fade-right");

        fadeItems.each(function (index, element) {
          var delay = index * 0.15;

          gsap.set(element, {
            opacity: 0,
            x: -100,
          });

          ScrollTrigger.create({
            trigger: element,
            start: "top 100%",
            end: "bottom 20%",
            scrub: 0.5,
            onEnter: function () {
              gsap.to(element, {
                opacity: 1,
                x: 0,
                duration: 1,
                delay: delay,
              });
            },
            once: true,
          });
        });
      });
    }

    /**
     * ======================================
     * 35. fade bottom gsap animation
     * ======================================
     */
    if ($(".fade-wrapper").length > 0) {
      $(".fade-wrapper").each(function () {
        var section = $(this);
        var fadeItems = section.find(".fade-bottom");

        fadeItems.each(function (index, element) {
          var delay = index * 0.15;

          gsap.set(element, {
            opacity: 0,
            y: -100,
          });

          ScrollTrigger.create({
            trigger: element,
            start: "top 100%",
            end: "bottom 20%",
            scrub: 0.5,
            onEnter: function () {
              gsap.to(element, {
                opacity: 1,
                y: 0,
                duration: 1,
                delay: delay,
              });
            },
            once: true,
          });
        });
      });
    }

    /**
     * ======================================
     * 36. appear down
     * ======================================
     */
    $(".appear-down").each(function () {
      const section = $(this);
      gsap.fromTo(
        section,
        {
          scale: 0.8,
          opacity: 0,
        },
        {
          scale: 1,
          opacity: 1,
          duration: 1.5,
          scrollTrigger: {
            trigger: section[0],
            scrub: 1,
            start: "top bottom",
            end: "bottom center",
            markers: false,
          },
        }
      );
    });

    /**
     * ======================================
     * 37. title animation
     * ======================================
     */
    if ($(".title-anim").length > 0) {
      let char_come = gsap.utils.toArray(".title-anim");
      char_come.forEach((char_come) => {
        let split_char = new SplitText(char_come, {
          type: "chars, words",
          lineThreshold: 0.5,
        });
        const tl2 = gsap.timeline({
          scrollTrigger: {
            trigger: char_come,
            start: "top 90%",
            end: "bottom 60%",
            scrub: false,
            markers: false,
            toggleActions: "play none none none",
          },
        });
        tl2.from(split_char.chars, {
          duration: 0.8,
          x: 70,
          autoAlpha: 0,
          stagger: 0.03,
        });
      });
    }

    /**
     * ======================================
     * 38. footer shape animation
     * ======================================
     */
    if ($(".footer").length > 0) {
      gsap.set(".foot-fade", {
        x: -100,
        opacity: 0,
      });

      ScrollTrigger.batch(".foot-fade", {
        start: "-100px bottom",
        onEnter: (elements) =>
          gsap.to(elements, {
            x: 0,
            opacity: 1,
            stagger: 0.3,
          }),
      });
    }

    /**
     * ======================================
     * 39. lenis smooth scroll
     * ======================================
     */
    const lenis = new Lenis();

    gsap.ticker.add(function (time) {
      lenis.raf(time * 500);
    });
    gsap.ticker.lagSmoothing(0);

    ScrollTrigger.update();
  });
})(jQuery);
