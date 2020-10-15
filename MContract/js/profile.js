$('.edit-password-btn').on('click', function() {
    $('.lk-main').removeClass('lk-opened');
    $('.lk-main').addClass('lk-closed');
    $('.lk-password').removeClass('lk-closed');
})

$('.profile__edit-back-btn').on('click', function() {
    $('.lk-main').removeClass('lk-closed');
    $('.lk-password').addClass('lk-closed');
    $('.lk-main').addClass('lk-opened');
})

$('.profile__edit-delete-btn').on('click', function() {

    $('.delete__draft').removeClass('closed');
    $('.delete__draft-btn').on('click', function() {
        var clicked = $(this)[0];
        if (clicked.classList.contains('yes')) {
            $('.delete__draft').addClass('closed');
        } else {
            $('.delete__draft').addClass('closed');
        }
    })
});

$('.profile__edit-back-btn-reg').on('click', function() {
    $('.lk-edit').addClass('lk-closed');
    $('.lk-main').removeClass('lk-closed');
    $('.lk-main').addClass('lk-opened');
});

$('.edit-profile-btn').on('click', function() {
    $('.lk-edit').removeClass('lk-closed');
    $('.lk-main').addClass('lk-closed');
    $('.lk-main').removeClass('lk-opened');
});

$('.profile-btn.find').on('click', function() {
    $('.region__wrapper').removeClass('closed');
});

$('.close__btn-reg'). on('click', function() {
    $('.region__wrapper').addClass('closed');
});

var regionsList = document.querySelectorAll('.region__content-list-li-ul-li');

for (var m = 0; m < regionsList.length; m ++) {
    regionsList[m].addEventListener('click', function() {
        this.classList.add('selected-region');
        var selectedText = this.textContent;
        var parentText = this.parentElement.parentElement.children[0].textContent;

        var newText = parentText + ', ' + selectedText;

        $('.region__content-rus').addClass('closed');
        $('.region__content-list-wrapper').addClass('closed');
        $('.region__content-another').addClass('closed');
        $('.selected-region-main').removeClass('closed');
        $('.selected-region-list').removeClass('closed');

        $('.region__selected-category-item span').text(newText);
    });
};

$('.region__selected-category-item .ico-close').on('click', function() {
    $('.region__content-rus').removeClass('closed');
    $('.region__content-list-wrapper').removeClass('closed');
    $('.region__content-another').removeClass('closed');
    $('.selected-region-main').addClass('closed');
    $('.selected-region-list').addClass('closed');
    $('.selected-region').removeClass('selected-region');
    $('.region__content-list-li-ul').addClass('off');
});

$('.region-first-level').on('click', function() {
    $(this).toggleClass('selected-region');
    $(this).parent().children('.region__content-list-li-ul').toggleClass('off');
});