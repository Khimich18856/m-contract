$('.buyFilter').on('click', function() {
    if ($(window).width() > 993) {
        $('.lk-product__item_d.buy-ad').show();
        $('.lk-product__item_d.sale-ad').hide();
    } else {
        $('.lk-product__item.sale-ad').hide();
        $('.lk-product__item.buy-ad').show();
    }
    if (!($('.active .buy-ad').length)) {
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
    if (!($('.active .sale-ad').length)) {
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
    if (!($('.active .lk-product__item'))) {
        $('.dnt-have-products').removeClass('closed');
    } else {
        $('.dnt-have-products').addClass('closed');
    }
});

$('.lk__main-select-item').on('click', function() {
    $('.filter-list-desktop .selected-filter').removeClass('selected-filter');
    $('.filter-list-desktop .allFilter').addClass('selected-filter');
    $('.filter-list .selected').removeClass('selected');
    $('.filter-list .allFilter').addClass('selected');
    if ($(window).width() > 993) {
        $('.lk-product__item_d.buy-ad').show();
        $('.lk-product__item_d.sale-ad').show();
    } else {
        $('.lk-product__item.sale-ad').show();
        $('.lk-product__item.buy-ad').show();
    }
    if (!($('.active .lk-product__item'))) {
        $('.dnt-have-products').removeClass('closed');
    } else {
        $('.dnt-have-products').addClass('closed');
    }
});

if (!($('.lk-product__item'))) {
    $('.dnt-have-products').removeClass('closed');
} else {
    $('.dnt-have-products').addClass('closed');
}