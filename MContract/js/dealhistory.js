$('.clear-btn').on('click', function() {
    $('.draft-history').removeClass('closed');
});

$('.draft-history .delete__draft-btn.no').on('click', function(e) {
    e.preventDefault();
    $('.draft-history').addClass('closed');
});

$('.draft-history .delete__draft-btn.yes').on('click', function(e) {
    e.preventDefault();
    $('.draft-history').addClass('closed');
    $('.product__item').remove();
    $('.product__item_d').remove();
});

$('.product__item-close').on('click', function() {
    $('.draft-deal').removeClass('closed');
    var currentDeal = $(this).parent().parent();
    $('.draft-deal .delete__draft-btn.yes').on('click', function(e) {
        e.preventDefault();
        currentDeal.remove();
        $('.draft-deal').addClass('closed');
    });
    $('.draft-deal .delete__draft-btn.no').on('click', function(e) {
        e.preventDefault();
        $('.draft-deal').addClass('closed');
    });
});

$('.period-btn').on('click', function() {
    $(this).parent().children('.filter-item-list').toggleClass('closed');
    $(this).parent().children('.filter-item-list li').on('click', function() {
        $(this).addClass('selected');
    });
})

$('.filter-item-list li').on('click', function() {
    $(this).parent().children('.selected').removeClass('selected');
});

$('.period-btn-open').on('click', function() {
    $('.filter-item-list-date').removeClass('closed');
    $(this).parent().addClass('closed');
});

$('.filter-item-list-date .date-btns .cancel').on('click', function() {
    $('.filter-item-list-date').addClass('closed');
});

$('.filter-item-list-date .date-btns .yes').on('click', function() {
    $('.filter-item-list-date').addClass('closed');
});

$('.select-date-year').on('click', function() {
    $('.filter-item-list-date').addClass('closed');
    $('.select-date-year-wrapper').removeClass('closed');
})

$('.select-date-year-wrapper .date-btn.cancel').on('click', function() {
    $('.select-date-year-wrapper').addClass('closed');
});

$('.select-date-year-wrapper .date-btn.yes').on('click', function() {
    $('.select-date-year-wrapper').addClass('closed');
});


$('.item-list .mounth-item').on('click', function() {
    $('.mounth-item.selected').removeClass('selected');
    $(this).addClass('selected');
});

$('.item-list .year-item').on('click', function() {
    $('.year-item.selected').removeClass('selected');
    $(this).addClass('selected');
});

$('.simple-filter').on('click', function() {
    $('.simple-filter.active').removeClass('active');
    $(this).addClass('active');
});

$('.mounth-btn-open').on('click', function() {
    $(this).parent().addClass('closed');
    $('.select-date-year-wrapper').removeClass('closed');
});

$('.calendar-btn-open').on('click', function() {
    $(this).parent().addClass('closed');
    $('.select-date-calenar').removeClass('closed');
});

$('.calendar-btn.cancel').on('click', function() {
    $('.select-date-calenar').addClass('closed');
});

$('.calendar-btn.yes').on('click', function() {
    $('.select-date-calenar').addClass('closed');
});

$('.selected-filter .ico').on('click', function() {
    $(this).parent().removeClass('selected-filter');
});

$('.category-btn').on('click', function() {
    $(this).parent().children('.filter-item-list').removeClass('closed');
    $(this).parent().children('.filter-item-list').children().on('click', function() {
        var newCatText = $(this).text();
        console.log(newCatText);
        $('.category-filter').text(newCatText);
        $(this).parent().parent().children('.filter-item-list').addClass('closed');
        $(this).parent().parent().addClass('selected-filter');
        $('.selected-filter .ico').on('click', function() {
            $(this).parent().removeClass('selected-filter');
        });
    });
});

$('.counter-btn').on('click', function() {
    $('.find__counter-wrapper').removeClass('closed');
});

$('.counter-list-item').on('click', function() {
    $('.find__counter-wrapper').addClass('closed');
});

$('.mobile__filter .close').on('click', function() {
    $('header').removeClass('filter-opened');
    var countOfFilter = $('.mobile__filter-li.active').length;
    countOfFilter += $('.selected-filter').length;
    $('.count-of-filters').text(countOfFilter);
});

$('.filter-btn').on('click', function() {
    $('header').addClass('filter-opened');
});

$('.simple-desc-filter').on('click', function() {
    $('.simple-desc-filter.active').removeClass('active');
    $(this).addClass('active');
});

$('.btn-period-desk').on('click', function() {
    $('.desk-filter-item-list').addClass('closed');
    $('.desk-filter-item-list-date').addClass('closed');
    $('.desk-select-date-year-wrapper').addClass('closed');
    $('.desk-select-date-calenar').addClass('closed');
    $('.desk-find__counter-wrapper').addClass('closed');
    $(this).parent().children('.desk-filter-item-list-date').removeClass('closed');
});

$('.period-btn-open-desk').on('click', function() {
    $('.desk-filter-item-list').addClass('closed');
    $('.desk-select-date-year-wrapper').addClass('closed');
    $('.desk-select-date-calenar').addClass('closed');
    $('.desk-find__counter-wrapper').addClass('closed');
    $('.desk-filter-item-list-date').removeClass('closed');
    $(this).parent().addClass('closed');
});

$('.mounth-btn-open-desk').on('click', function() {
    $('.desk-filter-item-list').addClass('closed');
    $('.desk-filter-item-list-date').addClass('closed');
    $('.desk-select-date-year-wrapper').addClass('closed');
    $('.desk-select-date-calenar').addClass('closed');
    $('.desk-find__counter-wrapper').addClass('closed');
    $(this).parent().addClass('closed');
    $('.desk-select-date-year-wrapper').removeClass('closed');
});

$('.desk-filter-item-list-date .select-date-year').on('click', function() {
    $(this).parent().addClass('closed');
    $('.desk-select-date-year-wrapper').removeClass('closed');
});

$('.desk-filter-item-list-date .date-btns .cancel').on('click', function() {
    $(this).parent().parent().addClass('closed');
    $('.desk-select-date-calenar').addClass('closed');
});

$('.desk-filter-item-list-date .date-btns .yes').on('click', function() {
    $(this).parent().parent().addClass('closed');
    $('.desk-select-date-calenar').addClass('closed');
    var newText = $(this).parent().parent().children('.date-select-wrapper').children('#datefromDesk').val() + ' - ' + $(this).parent().parent().children('.date-select-wrapper').children('#datetoDesk').val();
    $('#dateInputs').text(newText);
    $('.filter-desk-text.btn-period-desk').parent().addClass('have-selected-desk');
    $('.filter-item-desk.have-list.have-selected-desk .ico').on('click', function() {
        $(this).parent().removeClass('have-selected-desk');
        $('#dateInputs').text('');
    });
});

$('.desk-select-date-year-wrapper .date-btn.cancel').on('click', function() {
    $('.desk-select-date-year-wrapper').addClass('closed');
});

$('.desk-select-date-year-wrapper .date-btn.yes').on('click', function() {
    $('.desk-select-date-year-wrapper').addClass('closed');
});

$('.calendar-btn-open-desk').on('click', function() {
    $('.desk-filter-item-list').addClass('closed');
    $('.desk-filter-item-list-date').addClass('closed');
    $('.desk-select-date-year-wrapper').addClass('closed');
    $('.desk-select-date-calenar').addClass('closed');
    $('.desk-find__counter-wrapper').addClass('closed');
    $(this).parent().addClass('closed');
    $('.desk-select-date-calenar').removeClass('closed');
});

$('.desk-select-date-calenar .calendar-btns .cancel').on('click', function() {
    $('.desk-select-date-calenar').addClass('closed');
});

$('.desk-select-date-calenar .calendar-btns .yes').on('click', function() {
    $('.desk-select-date-calenar').addClass('closed');
});

$('.btn-cats-desk').on('click', function() {
    $('.desk-filter-item-list').addClass('closed');
    $('.desk-filter-item-list-date').addClass('closed');
    $('.desk-select-date-year-wrapper').addClass('closed');
    $('.desk-select-date-calenar').addClass('closed');
    $('.desk-find__counter-wrapper').addClass('closed');
    $(this).parent().children('.desk-filter-item-list').removeClass('closed');
});

$('.cats-list-desktop li').on('click', function() {
    var newCatsText = $(this).text();
    $(this).parent().addClass('closed');
    $('.selected-desk-text').text(newCatsText);
    $(this).parent().parent().addClass('have-selected-desk');
    $('.have-selected-desk .ico').on('click', function() {
        $(this).parent().removeClass('have-selected-desk');
    });
});

$('.btn-counter-desk').on('click', function() {
    $('.desk-filter-item-list').addClass('closed');
    $('.desk-filter-item-list-date').addClass('closed');
    $('.desk-select-date-year-wrapper').addClass('closed');
    $('.desk-select-date-calenar').addClass('closed');
    $('.desk-find__counter-wrapper').removeClass('closed');
});

$(document).mouseup(function (e) {
    var container = $('.desk-find__counter-wrapper');
    if (container.has(e.target).length === 0){
        container.addClass('closed');
    }
});

$('.close-deal').on('click', function(e) {
    e.preventDefault();
    var elementNeedDel = $(this).parent();
    elementNeedDel.append($('.draft-deal'));
    $('.draft-deal').removeClass('closed');


    $('.draft-deal .delete__draft-btn.yes').on('click', function(c) {
        c.preventDefault();
        $('.deals__wrapper').append($('.draft-deal'));
        elementNeedDel.remove();
        $('.draft-deal').addClass('closed');
    });
    $('.draft-deal .delete__draft-btn.no').on('click', function(c) {
        c.preventDefault();
        $('.deals__wrapper').append($('.draft-deal'));
        $('.draft-deal').addClass('closed');
    });
});

$('.date-select-wrapper input').on('focus', function() {
    $(this).parent().parent().parent().children('.desk-select-date-calenar').removeClass('closed');
});

