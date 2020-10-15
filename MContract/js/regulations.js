$('.rule__btn').on('click', function() {
    $('.rule__select').toggleClass('closed');
});

$('.rule__select-item').on('click', function() {
    $('.rule__select-item').removeClass('selected');
    $(this).addClass('selected');
    let newText = $(this).text();
    $('.rule__btn span').text(newText);
    $('.rule__select').toggleClass('closed');
});