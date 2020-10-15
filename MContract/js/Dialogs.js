$('.message__main-item .close').on('click', function() {
    $('.delete-msg').addClass('closed');
    $(this).parent().children('.delete-msg').removeClass('closed');
    $(this).parent().children('.delete-msg').children('.btns-block').children('.delete-btn.yes').on('click', function() {
        $(this).parent().parent().addClass('closed');
    });
    $(this).parent().children('.delete-msg').children('.btns-block').children('.delete-btn.no').on('click', function() {
        $(this).parent().parent().addClass('closed');
    });
});

$('.new-message').on('click', function() {
    $('.messgae__newchat-wrapper').removeClass('closed');
});

$('.messgae__newchat-header .close').on('click', function() {
    $('.messgae__newchat-wrapper').addClass('closed');
});