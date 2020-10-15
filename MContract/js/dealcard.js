$('.rules-title').on('click', function() {
    $('.rules-wrapper').removeClass('closed');
});

$('.rules-wrapper .close-btn').on('click', function() {
    $('.rules-wrapper').addClass('closed');
});

$('.delete__draft .delete__draft-flex .delete__draft-btn.yes').on('click', function(e) {
    e.preventDefault();
    $('.delete__draft').addClass('closed');
});

$('.delete__draft .delete__draft-flex .delete__draft-btn.no').on('click', function(e) {
    e.preventDefault();
    $('.delete__draft').addClass('closed');
});

$('.rate-delete').on('click', function(e) {
    e.preventDefault();
    $('.delete__draft').removeClass('closed');
});