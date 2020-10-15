$('.lk-product__item-favour').on('click', function(e) {
    e.preventDefault();
    $(this).toggleClass('selected');
});

$('.open-favour').on('click', function(e) {
    e.preventDefault();
    $(this).toggleClass('selected');
});

$('.clear-draft-wrapper').on('click', function() {
    $('.delete__draft-cur').addClass('closed');
    $('.delete__draft').toggleClass('closed');

    $('.delete__draft-btn').on('click', function() {
        var clicked = $(this)[0];
        if (clicked.classList.contains('yes')) {
            $('.lk-product__item_d').remove();
            $('.lk-product__item').remove();
            $('.delete__draft').addClass('closed');
            document.querySelector('.clear-draft-wrapper').textContent = 'Черновик пуст';
            $('.clear-draft-wrapper').addClass('empty-draft');
            $('.empty-draft').on('click', function() {
                $('.delete__draft').remove();
            });
        } else {
            $('.delete__draft').addClass('closed');
        }
    })
});