$('.company__list-item .close').on('click', function() {
    $('.company__list-item-delete').addClass('closed');
    $(this).parent().children('.company__list-item-delete').removeClass('closed');
});

$('.company__list-item-delete .btns-block .yes').on('click', function() {
    var elemId = $(this).parent().parent().parent().attr('id');
    deleteFromRegularClients(elemId);
    $(this).parent().parent().addClass('closed');
    $(this).parent().parent().parent().remove();
});

$('.company__list-item-delete .btns-block .no').on('click', function() {
    $(this).parent().parent().addClass('closed');
});