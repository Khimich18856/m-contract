$('.close-position').on('click', function() {
    var element = $(this).parent().parent();

    $('.delete__draft').removeClass('closed');

    $('.delete__draft-btn').on('click', function(e) {
        e.preventDefault();
        var clicked = $(this)[0];
        if (clicked.classList.contains('yes')) {
            element.hide();
            $('.delete__draft').addClass('closed');
        } else {
            $('.delete__draft').addClass('closed');
        }
    })
});

$('.delete').on('click', function() {
    var element = $(this).parent();

    $('.delete__draft').removeClass('closed');

    $('.delete__draft-btn').on('click', function(e) {
        e.preventDefault();
        var clicked = $(this)[0];
        if (clicked.classList.contains('yes')) {
            element.hide();
            $('.delete__draft').addClass('closed');
        } else {
            $('.delete__draft').addClass('closed');
        }
    })
});

$('.cat-span').on('click', function() {
    $('.filter-item-list').addClass('closed');
    $(this).parent().children('.filter-item-list').removeClass('closed');
});

$('.filter-item-list li').on('click', function() {
    $(this).parent().children('.selected').removeClass('selected');
    $(this).addClass('selected');
    var newCatText = $(this).text();
    $(this).parent().parent().children('.cat-span').text(newCatText);
    $(this).parent().addClass('closed');
});