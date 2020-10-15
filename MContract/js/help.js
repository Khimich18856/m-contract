$('.select .default-item').on('click', function() {
    $('.list-of-select').toggleClass('closed');
});

$('.select .ico').on('click', function() {
    $('.list-of-select').toggleClass('closed');
});

$('.list-of-select li').on('click', function() {
    let thisText = $(this).text();
    $('.select .default-item').text(thisText);
    $('.list-of-select li.selected').removeClass('selected');
    $(this).addClass('selected');
    $('.list-of-select').toggleClass('closed');
});

$('.file-inp').on('change', function() {
    let fileName = $(this).val().split('/').pop().split('\\').pop();
    if (fileName) {
        $('.file-label').text(fileName);
    } else {
        $('.file-label').text('Файл не выбран');
    }
});