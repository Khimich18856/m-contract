$('.lk__hedaer-burger').on('click', function() {
    $('.lk__wrapper').removeClass('closed-lk');
});

$('.close__btn-lk').on('click', function() {
    $('.lk__wrapper').addClass('closed-lk');
});

$('.filter-name-mobile span').on('click', function() {
    $('.filter-block').toggleClass('closed');
});

$('.filter-item').on('click', function() {
    $(this).addClass('selected');
    $('.filter-item.selected').removeClass('selected');
    document.querySelector('.selected-filter-span').textContent = $(this).text();
    $('.filter-block').addClass('closed').delay(500);
});

$('.filter-list-desktop li').on('click', function() {
    $('.selected-filter').removeClass('selected-filter');
    $(this).addClass('selected-filter');
});

var editProfile = document.querySelectorAll('.lk-product__item-edit');

$('.lk-product__item-edit').on('click', function(e) {
    e.preventDefault();
    var parent = $(this).children('.lk-product__item-list-block')[0];
    parent.classList.toggle('lk-product__item-list-opened');
});

$('.actived-list').on('click', function() {
    $('.active-wrapper').removeClass('disabled');
    $('.active-wrapper').addClass('active');
    $(this).addClass('selected');
    $('.ended-list').removeClass('selected');
    $('.ended-wrapper').addClass('disabled');
    $('.ended-wrapper').removeClass('active');
    $('.filter-item .selected').removeClass('selected');
    $('.allFilter').addClass('selected');
    if ($('.active-wrapper .lk-product__item').length) {
        $('.dnt-have-products').addClass('closed');
    } else {
        $('.dnt-have-products').removeClass('closed');
    }
});

$('.ended-list').on('click', function() {
    $('.ended-wrapper').removeClass('disabled');
    $('.ended-wrapper').addClass('active');
    $(this).addClass('selected');
    $('.actived-list').removeClass('selected');
    $('.active-wrapper').addClass('disabled');
    $('.active-wrapper').removeClass('active');
    $('.filter-item .selected').removeClass('selected');
    $('.allFilter').addClass('selected');
    if ($('.ended-wrapper .lk-product__item').length) {
        $('.dnt-have-products').addClass('closed');
    } else {
        $('.dnt-have-products').removeClass('closed');
    }
});

$('.password-open').on('click', function() {
    $(this).toggleClass('show');
    if ($(this).siblings('.profile-input').attr('type') == 'password'){
        $(this).siblings('.profile-input').attr('type', 'text');
	} else {
        $(this).siblings('.profile-input').attr('type', 'password');
	}
});