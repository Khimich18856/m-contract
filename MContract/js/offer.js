if (document.body.clientWidth > 767) {
    $('.product__item').remove();
} else {
    $('.product__item_d').remove();
}

$('.offer__open .star').on('click', function() {
    $(this).toggleClass('selected');
});

$('.open-popup-offers').on('click', function() {
    $('.offer__popup-wrapper').removeClass('popup-closed');
});

$('.offer__popup-header .close').on('click', function() {
    $('.offer__popup-wrapper').addClass('popup-closed');
});

var itemsSimilar = document.querySelectorAll('.similar__block-non-slider .product__item');
var arraySimilar = itemsSimilar;

var itemsSimilarD = document.querySelectorAll('.similar__block-non-slider .product__item_d');
var arraySimilarD = itemsSimilarD;

var slider = document.querySelector('.similar__block-slider');
for (var i = 0; i < itemsSimilar.length; i++) {
    slider.appendChild(itemsSimilar[i]);
}
for (var c = 0; c < itemsSimilarD.length; c++) {
    slider.appendChild(itemsSimilarD[c]);
}

$('.similar__block-slider').slick({
    arrows: true,
    dots: false,
    slidesToShow: 2,
    responsive: [{
          breakpoint: 993,
          settings: {
           slidesToShow: 1,
           slidesToScroll: 1
         }}],
    appendArrows: $('.slider__arows'),
    prevArrow: '<button id="prev" type="button" class="btn btn-juliet"><i class="fa fa-chevron-left" aria-hidden="true"></i> </button>',
    nextArrow: '<button id="next" type="button" class="btn btn-juliet">  <i class="fa fa-chevron-right" aria-hidden="true"></i></button>'
});

var itemsSlider = document.querySelectorAll('.similar__block-slider .product__item');
var arraySlider = itemsSlider;

var itemsSliderD = document.querySelectorAll('.similar__block-slider .product__item_d');
var arraySliderD = itemsSliderD;


$('.similar__block-view .similar-view-btn').on('click', function() {
    $('.similar__block-view .selected').removeClass('selected');
    $(this).toggleClass('selected');
    let single = $('.similar__block-view .single');
    if (single.hasClass('selected')) {
        $('.similar__block-slider').addClass('slider-off');

        $('.slider__arows').hide();

        var noneSlider = document.querySelector('.similar__block-non-slider');

        for (var d = 0; d < arraySimilar.length; d++) {
            noneSlider.appendChild(arraySimilar[d]);
        }

        for (var e = 0; e < arraySimilarD.length; e++) {
            noneSlider.appendChild(arraySimilarD[e]);
        }
        

        $('.similar__block-non-slider').addClass('slider-off');

    } else {
        $('.slider__arows').show();
        $('.similar__block-non-slider').removeClass('slider-off');
        $('.similar__block-slider').removeClass('slider-off');
    }
});

$('.tab-images .img').on('click', function() {
    var count = $(this).index();
    var block = document.createElement('section');
    var container = document.createElement('div');
    var col = document.createElement('div');
    var imageWrapper = document.createElement('div');
    var closeBtn = document.createElement('div');
    var imageSlider = document.createElement('div');


    var collectionOfImg = document.querySelectorAll('.tab-content .img');
    var documentWindow = document.querySelector('.log-in_window');

    block.classList.add('image__high-wrapper');
    container.classList.add('container');
    container.classList.add('height-100');
    col.classList.add('col-sm-12');
    imageWrapper.classList.add('image__high-block');
    closeBtn.classList.add('close__btn');
    imageSlider.classList.add('image__high-slider');
    
    imageWrapper.append(imageSlider);
    imageWrapper.append(closeBtn);
    col.append(imageWrapper);
    container.append(col);
    block.append(container);

    for (var g = 0; g < collectionOfImg.length; g++) {
        var newImg = document.createElement('img');
        var div = document.createElement('div');
        div.classList.add('image-slide');
        var span = document.createElement('span');
        span.textContent = 'Позиция' + ' ' + (g + 1);
        newImg.src = collectionOfImg[g].querySelector('img').getAttribute('data-full-url');
        div.append(newImg);
        div.append(span);
        imageSlider.append(div);
    }

    documentWindow.prepend(block);

    $('.image__high-slider').slick({
        arrows: true,
        dots: false,
        slidesToShow: 1
    });

    $('.image__high-slider').slick('slickGoTo', count, true)

    closeBtn.addEventListener('click', function() {
        block.remove();
    });
});

$('.offer__description-text .item .img').on('click', function() {
    var count = $(this).index();
    var block = document.createElement('section');
    var container = document.createElement('div');
    var col = document.createElement('div');
    var imageWrapper = document.createElement('div');
    var closeBtn = document.createElement('div');
    var imageSlider = document.createElement('div');


    var collectionOfImg = document.querySelectorAll('.tab-content .img');
    var documentWindow = document.querySelector('.log-in_window');

    block.classList.add('image__high-wrapper');
    container.classList.add('container');
    container.classList.add('height-100');
    col.classList.add('col-sm-12');
    imageWrapper.classList.add('image__high-block');
    closeBtn.classList.add('close__btn');
    imageSlider.classList.add('image__high-slider');
    
    imageWrapper.append(imageSlider);
    imageWrapper.append(closeBtn);
    col.append(imageWrapper);
    container.append(col);
    block.append(container);

    for (var g = 0; g < collectionOfImg.length; g++) {
        var newImg = document.createElement('img');
        var div = document.createElement('div');
        div.classList.add('image-slide');
        var span = document.createElement('span');
        span.textContent = 'Позиция' + ' ' + (g + 1);
        newImg.src = collectionOfImg[g].querySelector('img').getAttribute('data-full-url');
        div.append(newImg);
        div.append(span);
        imageSlider.append(div);
    }

    documentWindow.prepend(block);

    $('.image__high-slider').slick({
        arrows: true,
        dots: false,
        slidesToShow: 1
    });

    var selectedItem = document.querySelector('.slick-active');
    selectedItem.classList.remove('slick-current');
    selectedItem.classList.remove('slick-active');

    var tappedItem = this.querySelector('img');
    var slider = document.querySelectorAll('.image__high-wrapper .image-slide');

    for (var f = 0; f < slider.length; f++) {
        if (!slider[f].classList.contains('slick-cloned')) {
            var currentItemSlide = slider[f].querySelector('img').src;
            if (currentItemSlide == tappedItem.src) {
                slider[f].classList.add('slick-current');
                slider[f].classList.add('slick-active');
            }
        }
    }

    $('.image__high-slider').slick('slickGoTo', count, true)
    

    closeBtn.addEventListener('click', function() {
        block.remove();
    });
});

$('.sort-list-item a').on('click', function(e) {
    e.preventDefault();
    var element = document.querySelector('.close-pop');
    element.textContent = $(this).text();
    $('.sort').removeClass('opened-menu');
});

$('.close-pop').on('click', function() {
    $('.sort').toggleClass('opened-menu');
});

$('.not-added').on('click', function(e) {
    e.preventDefault();
});

$('.buyFilter').on('click', function() {
    if ($(window).width() > 993) {
        $('.lk-product__item_d.buy-ad').show();
        $('.lk-product__item_d.sale-ad').hide();
    } else {
        $('.lk-product__item.sale-ad').hide();
        $('.lk-product__item.buy-ad').show();
    }
    if (!($('.buy-ad').length)) {
        $('.dnt-have-products').removeClass('closed');
    } else {
        $('.dnt-have-products').addClass('closed');
    }
});

$('.saleFilter').on('click', function() {
    if ($(window).width() > 993) {
        $('.lk-product__item_d.buy-ad').hide();
        $('.lk-product__item_d.sale-ad').show();
    } else {
        $('.lk-product__item.sale-ad').show();
        $('.lk-product__item.buy-ad').hide();
    }
    if (!($('.sale-ad').length)) {
        $('.dnt-have-products').removeClass('closed');
    } else {
        $('.dnt-have-products').addClass('closed');
    }
});

$('.allFilter').on('click', function() {
    if ($(window).width() > 993) {
        $('.lk-product__item_d.buy-ad').show();
        $('.lk-product__item_d.sale-ad').show();
    } else {
        $('.lk-product__item.sale-ad').show();
        $('.lk-product__item.buy-ad').show();
    }
    if (!($('.lk-product__item_d').length)) {
        $('.dnt-have-products').removeClass('closed');
    } else {
        $('.dnt-have-products').addClass('closed');
    }
});