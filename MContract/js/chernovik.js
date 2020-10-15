var newElementOfTd = $('.position-elem').clone();

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

$('.lk-product__item-close').on('click', function(e) {
    e.preventDefault();
    $(this).parent().parent().children('.delete__draft-cur').removeClass('closed');
    $('.delete__draft').addClass('closed');
});

$('.delete__draft-cur .delete__draft-cur-btn.no').on('click', function(e) {
    e.preventDefault();
    $(this).parent().parent().parent().addClass('closed');
});

$('.delete__draft-cur .delete__draft-cur-btn.yes').on('click', function(e) {
    e.preventDefault();
    var adId = $(this).parent().parent().parent().parent().attr('id');
    $(this).parent().parent().parent().parent().remove();
    $(this).parent().parent().parent().addClass('closed');
    deleteDraft(adId);
});

$('input[readonly="readonly"]').removeAttr('readonly');

var i = 1;

function newElem() {
    i++;
    $(newElementOfTd).find('td:first-of-type').text(i);
    var newClone = newElementOfTd.clone();
    $('table tr').last().before(newClone);
    
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
        if ($(this).parent().hasClass('selected')) {
            $(this).parent().parent().children('.category__content-item-ul').addClass('off');
            $(this).parent().removeClass('selected');
        } else {
            $(this).parent().parent().children('.category__content-item-ul').removeClass('off');
            $(this).parent().addClass('selected');
        }
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
    $('input[readonly="readonly"]').removeAttr('readonly');
}

$('td.add').on('click', function() {
    newElem()
});