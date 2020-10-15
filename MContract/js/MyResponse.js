$('.main-btn.refresh').on('click', function() {
    $('.select-date-calenar').removeClass('closed');
});

$('.calendar-btn').on('click', function() {
    $('.select-date-calenar').addClass('closed');
});

$('.delete-btn').on('click', function() {
    $('.delete__draft').removeClass('closed');
});

$('.delete__draft-btn').on('click', function() {
    $('.delete__draft').addClass('closed');
});

$('.delete-add').on('click', function(e) {
    e.preventDefault();
    $('.delete__draft').removeClass('closed');
});