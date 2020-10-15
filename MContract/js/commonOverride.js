/*
for (var i = 0; i < itemOfCategory.length; i++) {
    itemOfCategory[i].removeEventListener('click');
}
*/

$('.first-level, .second-level, .third-level').on("click", function () {
    this.classList.toggle('selected');
    if (this.classList.contains('selected')) {
        countOfCategory++;
        var newElementOfCatefory = document.createElement('div');
        newElementOfCatefory.classList.add('category-items-item');
        newElementOfCatefory.textContent = this.textContent;
        listOfSelectedCategory.prepend(newElementOfCatefory);
    } else {
        countOfCategory--;
        var collectionOfSelectedItems = document.querySelectorAll('.category-items-item');
        for (var c = 0; c < collectionOfSelectedItems.length; c++) {
            if (this.textContent == collectionOfSelectedItems[c].textContent) {
                collectionOfSelectedItems[c].remove();
            }
        }
    }

    countOfCategoryBtn.textContent = countOfCategory;


    if (!(document.querySelectorAll('.selected').length == 0)) {
        openCategory.classList.add('have-category-item');
        emptyCategory.classList.add('have-select-category');
        listOfSelectedCategory.classList.remove('dnthave-select-category');
    } else {
        openCategory.classList.remove('have-category-item');
        emptyCategory.classList.remove('have-select-category');
        listOfSelectedCategory.classList.add('dnthave-select-category');
    }
});

$('.first-level').off('click');

$('.second-level').off('click');

$(".first-level-expand").on('click', function (e) {
    if ($(this).hasClass('expanded'))
        $(this).html('+');
    else
        $(this).html('-');

    $(this).toggleClass('expanded');

    $(this).parent().children('.category__content-item-ul').toggleClass('off');
});

$(".second-level-expand").on('click', function (e) {
    if ($(this).hasClass('expanded'))
        $(this).html('+');
    else
        $(this).html('-');

    $(this).toggleClass('expanded');

    $(this).parent().children('.category__content-item-ul-li-ul').toggleClass('off');
});

$(".first-level").on("click", function () {
    $(this).toggleClass('selected');
});

$(".second-level").on("click", function () {
    $(this).toggleClass('selected');
});

$(document).off("mouseup");

$(document).mouseup(function (e) {
    var container = $('.lk__wrapper').not(".only-desktop .lk__wrapper, .only__pc .lk__wrapper");
    if (container.has(e.target).length === 0) {
        container.addClass('closed-lk');
    }
});