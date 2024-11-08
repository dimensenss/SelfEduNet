(function ($) {
    "use strict"


    /* 1. Proloder */
    $(window).on('load', function () {
        $('#preloader-active').fadeOut(0);
        $('body').css({
            'overflow': 'visible'
        });
    });

    /* 2. sticky And Scroll UP */
    $(window).on('scroll', function () {
        var scroll = $(window).scrollTop();
        if (scroll < 400) {
            $(".header-sticky").removeClass("sticky-bar");
            $('#back-top').fadeOut(500);
        } else {
            $(".header-sticky").addClass("sticky-bar");
            $('#back-top').fadeIn(500);
        }
    });

    // Scroll Up
    $('#back-top a').on("click", function () {
        $('body,html').animate({
            scrollTop: 0
        }, 800);
        return false;
    });


    /* 3. slick Nav */
// mobile_menu
    var menu = $('ul#navigation');
    if (menu.length) {
        menu.slicknav({
            prependTo: ".mobile_menu",
            closedSymbol: '+',
            openedSymbol: '-'
        });
    }
    ;


    /* 4. MainSlider-1 */

    // h1-hero-active
    function mainSlider() {
        var BasicSlider = $('.slider-active');
        BasicSlider.on('init', function (e, slick) {
            var $firstAnimatingElements = $('.single-slider:first-child').find('[data-animation]');
            doAnimations($firstAnimatingElements);

        });
        BasicSlider.on('beforeChange', function (e, slick, currentSlide, nextSlide) {
            var $animatingElements = $('.single-slider[data-slick-index="' + nextSlide + '"]').find('[data-animation]');
            doAnimations($animatingElements);

        });

        BasicSlider.slick({
            autoplay: true,
            autoplaySpeed: 4000,
            dots: false,
            fade: true,
            arrows: false,
            prevArrow: '<button type="button" class="slick-prev"><i class="ti-angle-left"></i></button>',
            nextArrow: '<button type="button" class="slick-next"><i class="ti-angle-right"></i></button>',
            responsive: [{
                breakpoint: 1024,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    infinite: true,
                }
            },
                {
                    breakpoint: 991,
                    settings: {
                        slidesToShow: 1,
                        slidesToScroll: 1,
                        arrows: false
                    }
                },
                {
                    breakpoint: 767,
                    settings: {
                        slidesToShow: 1,
                        slidesToScroll: 1,
                        arrows: false
                    }
                }
            ]
        });

        function doAnimations(elements) {
            var animationEndEvents = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
            elements.each(function () {
                var $this = $(this);
                var $animationDelay = $this.data('delay');
                var $animationType = 'animated ' + $this.data('animation');
                $this.css({
                    'animation-delay': $animationDelay,
                    '-webkit-animation-delay': $animationDelay
                });
                $this.addClass($animationType).one(animationEndEvents, function () {
                    $this.removeClass($animationType);
                });
            });
        }
    }

    mainSlider();


    /* 4. Testimonial Active*/
    var testimonial = $('.h1-testimonial-active');
    if (testimonial.length) {
        testimonial.slick({
            dots: true,
            infinite: true,
            speed: 1000,
            autoplay: true,
            loop: true,
            arrows: true,
            prevArrow: '<button type="button" class="slick-prev"><i class="ti-arrow-top-left"></i></button>',
            nextArrow: '<button type="button" class="slick-next"><i class="ti-arrow-top-right"></i></button>',
            slidesToShow: 1,
            slidesToScroll: 1,
            responsive: [
                {
                    breakpoint: 1024,
                    settings: {
                        slidesToShow: 1,
                        slidesToScroll: 1,
                        infinite: true,
                        dots: true,
                        arrow: true
                    }
                },
                {
                    breakpoint: 600,
                    settings: {
                        slidesToShow: 1,
                        slidesToScroll: 1,
                        arrow: true
                    }
                },
                {
                    breakpoint: 480,
                    settings: {
                        slidesToShow: 1,
                        slidesToScroll: 1,
                        arrow: true
                    }
                }
            ]
        });
    }


// Single Img slder
    $('.top-job-slider').slick({
        dots: false,
        infinite: true,
        autoplay: true,
        speed: 400,
        centerPadding: '60px',
        centerMode: true,
        focusOnSelect: true,
        arrows: false,
        prevArrow: '<button type="button" class="slick-prev"><i class="ti-angle-left"></i></button>',
        nextArrow: '<button type="button" class="slick-next"><i class="ti-angle-right"></i></button>',
        slidesToShow: 4,
        slidesToScroll: 1,
        responsive: [
            {
                breakpoint: 1400,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 1,
                    infinite: true,
                    dots: false,
                }
            },
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 1,
                    infinite: true,
                    dots: false,
                }
            },
            {
                breakpoint: 992,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 1,
                    infinite: true,
                    dots: false,
                }
            },
            {
                breakpoint: 768,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    arrows: false,
                    centerMode: false
                }
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    arrows: false,
                    centerMode: false
                }
            },
        ]
    });


// Single Img slder
    $('.team-active').slick({
        dots: false,
        infinite: true,
        autoplay: true,
        speed: 400,
        arrows: true,
        prevArrow: '<button type="button" class="slick-prev"><i class="ti-angle-left"></i></button>',
        nextArrow: '<button type="button" class="slick-next"><i class="ti-angle-right"></i></button>',
        slidesToShow: 4,
        slidesToScroll: 1,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 4,
                    slidesToScroll: 1,
                    infinite: true,
                    dots: false,
                }
            },
            {
                breakpoint: 992,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 1,
                    infinite: true,
                    dots: false,
                }
            },
            {
                breakpoint: 768,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    arrows: false
                }
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    arrows: false
                }
            },
        ]
    });


// courses-area
    $('.courses-actives').slick({
        dots: false,
        infinite: true,
        autoplay: true,
        speed: 400,
        arrows: true,
        prevArrow: '<button type="button" class="slick-prev"><i class="ti-angle-left"></i></button>',
        nextArrow: '<button type="button" class="slick-next"><i class="ti-angle-right"></i></button>',
        slidesToShow: 3,
        slidesToScroll: 1,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 1,
                    infinite: true,
                    dots: false,
                }
            },
            {
                breakpoint: 992,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 1,
                    infinite: true,
                    dots: false,
                }
            },
            {
                breakpoint: 768,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    arrows: false
                }
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                    arrows: false
                }
            },
        ]
    });


    // Brand Active
    $('.brand-active').slick({
        dots: false,
        infinite: true,
        autoplay: true,
        speed: 400,
        arrows: false,
        slidesToShow: 5,
        slidesToScroll: 1,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 4,
                    slidesToScroll: 3,
                    infinite: true,
                    dots: false,
                }
            },
            {
                breakpoint: 992,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 1,
                    infinite: true,
                    dots: false,
                }
            },
            {
                breakpoint: 768,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 1
                }
            },
            {
                breakpoint: 480,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1
                }
            },

            // You can unslick at a given breakpoint now by adding:
            // settings: "unslick"
            // instead of a settings object
        ]
    });


    /* 6. Nice Selectorp  */
    var nice_Select = $('select');
    if (nice_Select.length) {
        nice_Select.niceSelect();
    }

    /* 7. data-background */
    $("[data-background]").each(function () {
        $(this).css("background-image", "url(" + $(this).attr("data-background") + ")")
    });


    /* 10. WOW active */
    new WOW().init();

// 11. ---- Mailchimp js --------//
    function mailChimp() {
        $('#mc_embed_signup').find('form').ajaxChimp();
    }

    mailChimp();


// 12 Pop Up Img
    var popUp = $('.single_gallery_part, .img-pop-up');
    if (popUp.length) {
        popUp.magnificPopup({
            type: 'image',
            gallery: {
                enabled: true
            }
        });
    }
// 12 Pop Up Video
    var popUp = $('.popup-video');
    if (popUp.length) {
        popUp.magnificPopup({
            type: 'iframe'
        });
    }

    /* 13. counterUp*/
    $('.counter').counterUp({
        delay: 10,
        time: 3000
    });

    /* 14. Datepicker */
    $('#datepicker1').datepicker();

// 15. Time Picker
    $('#timepicker').timepicker();

//16. Overlay
    $(".snake").snakeify({
        speed: 200
    });


//17.  Progress barfiller

    $('#bar1').barfiller();
    $('#bar2').barfiller();
    $('#bar3').barfiller();
    $('#bar4').barfiller();
    $('#bar5').barfiller();
    $('#bar6').barfiller();

})(jQuery);
$(document).ready(function ($) {
    var successMessage = $("#jq-notification");
    var notification = $('#notification');
    var warning_notification = $('#warning-jq-notification');

    if (notification.length > 0) {
        setTimeout(function () {
            notification.alert('close');

        }, 3000);
    }
});


// $(document).ready(function () {
//
//     $('#catalog-search-form-main').on('submit', function (event) {
//         event.preventDefault(); // Останавливаем стандартное поведение формы
//
//         const form = $(this);
//         const formData = form.serializeArray();
//         const params = new URLSearchParams(window.location.search);
//
//         // Удаляем параметр 'q' из текущих параметров, если он есть
//         params.delete('q');
//
//         // Добавляем все параметры из формы, кроме 'q'
//         formData.forEach(({name, value}) => {
//             if (name !== 'q') {
//                 params.set(name, value);
//             }
//         });
//
//         // Добавляем новый параметр 'q' из формы
//         const searchQuery = form.find('input[name="q"]').val();
//         if (searchQuery) {
//             params.set('q', searchQuery);
//         }
//
//         // Создаем новый URL и перенаправляем
//         const newUrl = `${window.location.pathname}?${params.toString()}`;
//         window.location.href = newUrl;
//     });
//
//     const forms = $('#catalog-search-form-desktop, #catalog-search-form-mobile');
//
//     function updateSearch(params) {
//         const newUrl = `${window.location.pathname}?${params.toString()}`;
//         window.history.replaceState({}, '', newUrl);
//
//         $.ajax({
//             url: '/catalog/api/v1/search/',
//             type: 'GET',
//             data: params.toString(),
//             dataType: 'json',
//             success: function (data) {
//                 $('.courses-list').empty().html(data);
//                 updateFilterButtons(params);
//                 updateFormFromParams(params);
//             },
//             error: function (xhr, status, error) {
//                 console.error('Ошибка:', error);
//             }
//         });
//     }
//
//     function resetSelectFields(fieldName) {
//         const select = forms.find(`select[name="${fieldName}"]`);
//         const niceSelect = select.siblings('.nice-select');
//         niceSelect.find('.current').text('Всі') // Можно заменить на более общий текст, если нужно
//             .end().find('.option').removeClass('selected')
//             .filter('[data-value=""]').addClass('selected');
//         select.val('');
//     }
//
//     function updateFilterButtons(params) {
//         const filterContainer = $('.active-filters').empty();
//         let searchInputCleared = false;
//
//         params.forEach((value, key) => {
//             if (value && value !== '0') {
//                 let displayText = value;
//                 const element = $(`input[name="${key}"], select[name="${key}"]`);
//
//                 if (element.length) {
//                     // Для селектов ищем значение в опциях
//                     if (element.is('select')) {
//                         const option = element.find(`option[value="${value}"]`);
//                         displayText = option.data('btn-label') || option.text();
//                     } else {
//                         displayText = element.data('btn-label') || value;
//                         if (key.startsWith('price')) {
//                             displayText = `${element.data('btn-label')} ${value}`;
//                         }
//                     }
//                 }
//
//                 $('<button>', {
//                     type: 'button',
//                     class: 'button-4 m-0 me-2 mb-2',
//                     style: 'height: 35px',
//                     text: `${displayText} ✕`,
//                     click: function () {
//                         params.delete(key);
//                         updateSearch(params);
//                         if (key === 'q') {
//                             $('.search-input').val('');
//                             searchInputCleared = true;
//                         } else if (key === 'language' || key === 'difficulty') {
//                             resetSelectFields(key);
//                         }
//                     }
//                 }).appendTo(filterContainer);
//             }
//         });
//
//         if (searchInputCleared) $('.search-input').val('');
//     }
//
//
//     function updateFormFromParams(params) {
//         forms.find('input[type="text"], input[type="number"]').val('')
//             .end().find('input[type="checkbox"]').prop('checked', false);
//
//         params.forEach((value, key) => {
//             const field = forms.find(`[name="${key}"]`);
//             if (field.is('select')) {
//                 field.val(value);
//             } else if (field.attr('type') === 'checkbox') {
//                 field.prop('checked', true);
//             } else {
//                 field.val(value);
//             }
//         });
//
//         $('.search-input').val(params.get('q') || '');
//     }
//
//     forms.on('change', function (event) {
//         event.preventDefault();
//         const form = $(this);
//         const formData = form.serializeArray();
//         const params = new URLSearchParams(window.location.search);
//
//         form.find('input[name="price__gte"], input[name="price__lte"]').val(function (_, value) {
//             return value === '0' ? '' : value;
//         });
//
//         form.find('input[type="checkbox"]').each(function () {
//             const checkbox = $(this);
//             const name = checkbox.attr('name');
//             checkbox.is(':checked') ? params.set(name, checkbox.val()) : params.delete(name);
//         });
//
//         formData.forEach(({name, value}) => {
//             value && value !== '0' ? params.set(name, value) : params.delete(name);
//         });
//
//         updateSearch(params);
//     });
//
//     updateFilterButtons(new URLSearchParams(window.location.search));
//     updateFormFromParams(new URLSearchParams(window.location.search));
// });
//
//
// $(document).ready(function () {
//     // Привязываем обработчик для обеих форм
//     const forms = $('#teach-courses-search-form');
//     const userOwner = $('#teach-courses-search-form').find('#user_owner').val();
//
//     forms.on('submit', function (event) {
//         event.preventDefault();
//         const form = $(this); // Текущая форма
//
//         const params = new URLSearchParams(window.location.search);
//
//         const searchQuery = form.find('input[name="q"]').val();
//         if (searchQuery) {
//             params.set('q', searchQuery);
//             params.set('owner', userOwner);
//         } else if (searchQuery === '') {
//             params.delete('q');
//             params.delete('owner');
//         }
//
//         const selectElement = document.querySelector('select[name="status"]');
//         const selectedOption = selectElement.options[selectElement.selectedIndex];
//         const selectedValue = selectedOption.value;
//         params.set('status', selectedValue);
//
//
//
//         const newUrl = window.location.pathname + '?' + params.toString();
//         window.history.replaceState({}, '', newUrl);
//
//
//         // Отправляем AJAX запрос
//         $.ajax({
//             url: '/teach/api/v1/teach-search/',
//             type: 'GET',
//             data: params.toString(), // Передаем параметры URL в запросе
//             dataType: 'json',
//             success: function (data) {
//                 const coursesList = $('.courses-list');
//                 coursesList.empty();
//                 coursesList.html(data); // Обновляем контент
//             },
//             error: function (xhr, status, error) {
//                 console.error('Ошибка:', error);
//             }
//         });
//     });
// });
//
//
function sendAjaxRequest(url, params, successCallback) {
        $.ajax({
            url: url,
            type: 'GET',
            data: params.toString(),
            dataType: 'json',
            success: successCallback,
            error: function (xhr, status, error) {
                console.error('Ошибка:', error);
            }
        });
    }

    // Обновление URL без перезагрузки страницы
    function updateUrl(params) {
        const newUrl = `${window.location.pathname}?${params.toString()}`;
        window.history.replaceState({}, '', newUrl);
    }
    function serializeFormToParams(form) {
        const formData = form.serializeArray();
        const params = new URLSearchParams(window.location.search);

        formData.forEach(({name, value}) => {
            if (value && value !== '0') {
                params.set(name, value);
            } else {
                params.delete(name);
            }
        });

        return params;
    }
$(document).ready(function () {
    // Обработка сериализации формы и обновления URL


    // Общая функция для отправки AJAX-запросов


    // Обновление фильтров и полей формы
function updateFilterButtons(params) {
    const filterContainer = $('.active-filters').empty();
    let searchInputCleared = false;

    params.forEach((value, key) => {
        if (value && value !== '0') {
            let displayText = value;
            const element = $(`input[name="${key}"], select[name="${key}"]`);

            if (element.length) {
                // Для селектов ищем значение в опциях
                if (element.is('select')) {
                    const option = element.find(`option[value="${value}"]`);
                    displayText = option.data('btn-label') || option.text();
                } else if (element.attr('type') === 'checkbox') {
                    // Для чекбоксов используем data-btn-label
                    displayText = element.data('btn-label') || value;
                } else {
                    displayText = element.data('btn-label') || value;
                    if (key.startsWith('price')) {
                        displayText = `${element.data('btn-label')} ${value}`;
                    }
                }
            }

            $('<button>', {
                type: 'button',
                class: 'button-4 m-0 me-2 mb-2',
                style: 'height: 35px',
                text: `${displayText} ✕`,
                click: function () {
                    params.delete(key);
                    updateSearch(params);
                    if (key === 'q') {
                        $('.search-input').val('');
                        searchInputCleared = true;
                    } else if (key === 'language' || key === 'difficulty') {
                        resetSelectFields(key);
                    }
                }
            }).appendTo(filterContainer);
        }
    });

    if (searchInputCleared) $('.search-input').val('');
}


    // Обновление полей формы на основе параметров URL
    function updateFormFromParams(params) {
        forms.find('input, select').each(function () {
            const field = $(this);
            const value = params.get(field.attr('name')) || '';

            if (field.is('select')) {
                field.val(value);
            } else if (field.attr('type') === 'checkbox') {
                field.prop('checked', value === field.val());
            } else {
                field.val(value);
            }
        });

        $('.search-input').val(params.get('q') || '');
    }

    // Обновление поиска
    function updateSearch(params) {
        updateUrl(params);
        sendAjaxRequest('/catalog/api/v1/search/', params, function (data) {
            $('.courses-list').empty().html(data);
            updateFilterButtons(params);
            updateFormFromParams(params);
        });
    }

    const forms = $('#catalog-search-form-main, #catalog-search-form-desktop, #catalog-search-form-mobile');

    // Обработчик для всех форм
    forms.on('submit change', function (event) {
        event.preventDefault();
        const form = $(this);
        const params = serializeFormToParams(form);

        updateSearch(params);
    });

    // Инициализация фильтров и формы при загрузке
    const initialParams = new URLSearchParams(window.location.search);
    updateFilterButtons(initialParams);
    updateFormFromParams(initialParams);
});

// Обработка формы поиска по курсам для владельца
$(document).ready(function () {
    const forms = $('#teach-courses-search-form');
    const userOwner = $('#teach-courses-search-form').find('#user_owner').val();

    forms.on('submit', function (event) {
        event.preventDefault();
        const form = $(this);
        const params = serializeFormToParams(form);

        const searchQuery = form.find('input[name="q"]').val();
        if (searchQuery) {
            params.set('q', searchQuery);
            params.set('owner', userOwner);
        } else {
            params.delete('q');
            params.delete('owner');
        }

        const statusValue = form.find('select[name="status"]').val();
        params.set('status', statusValue);

        updateUrl(params);
        sendAjaxRequest('/teach/api/v1/teach-search/', params, function (data) {
            $('.courses-list').empty().html(data);
        });
    });
});
 (function () {
        'use strict'

        var forms = document.querySelectorAll('.needs-validation')

        Array.prototype.slice.call(forms)
            .forEach(function (form) {
                form.addEventListener('submit', function (event) {
                    if (!form.checkValidity()) {
                        event.preventDefault()
                        event.stopPropagation()
                    }

                    form.classList.add('was-validated')
                }, false)
            })
    })()