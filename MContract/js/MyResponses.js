$('.lk-product__item-favour').on('click', function(e) {
    e.preventDefault();
    $(this).parent().parent().children('.lk-product__item_d-delete').removeClass('closed');
});

$('.open-favour').on('click', function(e) {
    e.preventDefault();
    $(this).parent().children('.lk-product__item_d-delete').removeClass('closed');
});

$('.lk-product__item_d-delete .delete-btn').on('click', function(e) {
    e.preventDefault();
    $(this).parent().parent().addClass('closed');
});