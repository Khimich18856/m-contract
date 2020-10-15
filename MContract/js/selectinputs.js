$('.regionInput').on('click', function() {
    $('.region__wrapper').toggleClass('closed');
    $('.category__wrapper').addClass('closed');
});

$('.regionInput').on('input', function() {
    var text = $(this).val();
    if (text.length > 2) {
        var selector = '.region-first-level:contains(' + '"' + text + '"' + ')';
        $('.region-first-level').hide();
        $(selector).show();
    } else {
        $('.region-first-level').show();
    }
});

$('.region-first-level').on('click', function() {
    $('.region-items-item').remove();
    var newRegText = $(this).text();
    $('.region-empty').addClass('have-select-region');
    var newReg = $('<div class="region-items-item"></div>');
    newReg.text(newRegText);
    $('.regionInput').val(newRegText);
    $('.region-items').prepend(newReg);
    $('.region-items').removeClass('dnthave-select-region');
    $('.region-first-level').show();
});

$('.clear-regions').on('click', function() {
    $('.region-items-item').remove();
    $('.region-empty').removeClass('have-select-region');
    $('.region-items').addClass('dnthave-select-region');
    $('.regionInput').val('');
});

$('.close__btn-reg').on('click', function() {
    $('.region__wrapper').toggleClass('closed');
});

$('.first-level span').on('click', function() {
    if ($(this).parent().parent().parent().parent().parent().parent().find('.category-items-item').length) {
        newElem();
        $('.category__wrapper').addClass('closed');
        $(this).parent().parent().parent().parent().parent().parent().find('.category-items-item').remove();
    } else {
        $(this).parent().parent().parent().parent().parent().parent().find('.category-items-item').remove();
        var catText = $(this).text();
        var newCat = $('<div class="category-items-item"></div>')
        newCat.text(catText);
        $(this).parent().parent().parent().parent().parent().parent().find('.category-items').prepend(newCat);
        $(this).parent().parent().parent().parent().parent().parent().find('.category-empty').addClass('have-select-category');
        $(this).parent().parent().parent().parent().parent().parent().find('.category-items').removeClass('dnthave-select-category');
        $(this).parent().parent().parent().parent().parent().parent().parent().find('.catInput').val(catText);
    }
});

$('.second-level span').on('click', function() {
    if ($(this).parent().parent().parent().parent().parent().parent().parent().parent().find('.category-items-item').length) {
        newElem();
        $('.category__wrapper').addClass('closed');
        $(this).parent().parent().parent().parent().parent().parent().parent().parent().find('.category-items-item').remove();
    } else {
        $(this).parent().parent().parent().parent().parent().parent().parent().parent().find('.category-items-item').remove();
        var catText = $(this).text();
        var newCat = $('<div class="category-items-item"></div>')
        newCat.text(catText);
        $(this).parent().parent().parent().parent().parent().parent().parent().find('.category-items').prepend(newCat);
        $(this).parent().parent().parent().parent().parent().parent().parent().find('.category-empty').addClass('have-select-category');
        $(this).parent().parent().parent().parent().parent().parent().parent().find('.category-items').removeClass('dnthave-select-category');
        $(this).parent().parent().parent().parent().parent().parent().parent().parent().find('.catInput').val(catText);
    }
});

$('.third-level').on('click', function() {
    if ($(this).parent().parent().parent().parent().parent().parent().parent().parent().parent().find('.category-items-item').length) {
        newElem();
        $('.category__wrapper').addClass('closed');
        $(this).parent().parent().parent().parent().parent().parent().parent().parent().parent().find('.category-items-item').remove();
    } else {
        $(this).parent().parent().parent().parent().parent().parent().parent().parent().parent().find('.category-items-item').remove();
        var catText = $(this).text();
        var newCat = $('<div class="category-items-item"></div>')
        newCat.text(catText);
        $(this).parent().parent().parent().parent().parent().parent().parent().parent().find('.category-items').prepend(newCat);
        $(this).parent().parent().parent().parent().parent().parent().parent().parent().find('.category-empty').addClass('have-select-category');
        $(this).parent().parent().parent().parent().parent().parent().parent().parent().find('.category-items').removeClass('dnthave-select-category');
        $(this).parent().parent().parent().parent().parent().parent().parent().parent().parent().find('.catInput').val(catText);
    }
});

$('.first-level .ico').on('click', function() {
    $(this).parent().parent().children('.category__content-item-ul').toggleClass('off');
    $(this).parent().toggleClass('selected');
});

$('.second-level .ico').on('click', function() {
    $(this).parent().parent().children('.category__content-item-ul-li-ul').toggleClass('off');
    $(this).parent().toggleClass('selected');
});

$('.clear-category').on('click', function() {
    $(this).parent().parent().parent().find('.category-empty').removeClass('have-select-category');
    $(this).parent().parent().parent().find('.category-items').addClass('dnthave-select-category');
    $(this).parent().parent().parent().find('.selected').removeClass('selected');
    $(this).parent().parent().parent().find('.category__content-item-ul').addClass('off');
    $(this).parent().parent().parent().find('.category__content-item-ul-li-ul').addClass('off');
    $(this).parent().parent().parent().parent().find('.catInput').val('');
});

$('.catInput').on('click', function() {
    $('.region__wrapper').addClass('closed');
    $('.category__wrapper').addClass('closed');
    $(this).parent().children('.category__wrapper').toggleClass('closed');
});

$('.category__header .close__btn').on('click', function() {
    $(this).parent().parent().addClass('closed');
});