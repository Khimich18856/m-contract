$('.m-chat__main .edit-msg').on('click', function(e) {
    e.preventDefault();
    $(this).parent().children('.edit-wrapper').toggleClass('closed');
});

$('.m-chat__current').hide();

$('.edit-wrapper .delete').on('click', function() {
    $(this).parent().parent().toggleClass('closed');
    $(this).parent().parent().parent().children('.delete-msg').removeClass('closed');
});

$('.m-chat__main-item .img').on('click', function() {
    $('.m-chat__current').show();
});

$('.m-chat__main-item .info').on('click', function() {
    $('.m-chat__current').show();
});

$('.message__header .back').on('click', function() {
    $('.m-chat__current').hide();
});

$('.edit-wrapper .full').on('click', function() {
    $(this).parent().parent().toggleClass('closed');
});

$('.edit-current .delete').on('click', function() {
    $('.delete-msg-cerrnt-all').removeClass('closed');
});

$('.delete-msg-cerrnt-all .delete-btn').on('click', function() {
    $('.delete-msg-cerrnt-all').addClass('closed');
});

$('.delete-msg .delete-btn.yes').on('click', function() {
    $(this).parent().parent().toggleClass('closed');
    let itemId = $(this).parent().parent().parent().attr('data-chatId');
    alert(itemId);
    $(this).parent().parent().parent().hide();
});

$('.delete-msg .delete-btn.no').on('click', function() {
    $(this).parent().parent().toggleClass('closed');
});

$('.m-chat__header-clrd .clear').on('click', function() {
    $('.delete-msg-all').removeClass('closed');
});

$('.delete-msg-all .delete-btn.yes').on('click', function() {
    $(this).parent().parent().toggleClass('closed');
    $('.m-chat__main-item').hide();
});

$('.delete-msg-all .delete-btn.no').on('click', function() {
    $(this).parent().parent().toggleClass('closed');
});

$('.m-chat__header-btns .notice').on('click', function() {
    $(this).toggleClass('notice-off');
});

$('.m-chat__header-btns .close').on('click', function() {
    $('.m-chat').addClass('closed');
    $('.m-chat-btn').show();
});

$('.m-chat-btn').on('click', function() {
    $(this).hide();
    $('.m-chat').removeClass('closed');
});



$('.message__header .edit-current').on('click', function() {
    $('.edit-list').removeClass('closed');
    $('.edit-item').on('click', function() {
        setTimeout(function (){
            $('.edit-list').addClass('closed');
        }, 10);
    });
});

$(document).mouseup(function (e) {
    var container = $('.edit-list');
    if (container.has(e.target).length === 0){
        container.addClass('closed');
    }
});

$(document).mouseup(function (e) {
    var container = $('.message-list');
    if (container.has(e.target).length === 0){
        container.addClass('closed');
    }
});

$('.message-in .add').on('click', function() {
    $('.message-list').addClass('closed');
    $(this).children('.message-list').removeClass('closed');
});

if ($(window).width() > 766) {
    $('textarea').each(function () {
        this.setAttribute('style', 'height:' + (this.scrollHeight) + 'px;overflow-y:hidden;');
      }).on('input', function () {
        this.style.height = 'auto';
        this.style.height = (this.scrollHeight) + 'px';
      });
}

$('.toBottom').on('click', function() {
    window.hashName = window.location.hash;
    window.location.hash = '#lastMessage';
    $('.message__main').animate({scrollTop: $('#lastMessage').offset().top}, 10, function() {
        window.location.hash = window.hashName;
    });
});

var countOfMessages = document.querySelectorAll('.message-in').length;

$('.message-send-btn').on('click', function() {
    var messageText = $('#message').val();

    var fullDate = new Date();

    var twoDigitMonth = ((fullDate.getMonth().length+1) === 1)? (fullDate.getMonth()+1) : '0' + (fullDate.getMonth()+1);
    var currentDate =  fullDate.getDate() + "." + twoDigitMonth + '.' + fullDate.getFullYear();

    currentDate = currentDate + ', ' + fullDate.getHours() + '.' + fullDate.getMinutes() + '.' + fullDate.getSeconds();

    if (messageText) {
        $('#lastMessage').removeAttr("id");
        $('.message-in.out').removeAttr("id");
        var newMessageBlock = $('<div class="message-in out" id="#lastMessage"> ' +
                                '<div class="message-info">' + 
                                    '<div class="date">' + 
                                        currentDate + 
                                    '</div> '+
                                    '<div class="add">'+
                                        '<ul class="message-list closed"><li class="message-item">Пункт меню</li><li class="message-item">Удалить сообщение</li><li class="message-item">Пункт меню</li></ul>' +
                                    '</div> '+
                                '</div> '+
                                '<div class="message-text">' +
                                    messageText +
                                '</div>' +
                                '<div class="status send">' + 
                                '</div>' +
                            ' </div>');
        $('.message__main-date.last').append(newMessageBlock);
        $('#message').val("");
        $('#message').height('46');
        var scroll = $('.message__main').height();
        $('.message__main').animate({ scrollTop: scroll }, 1000);
        $('html,body').stop().animate({ scrollDown: scroll }, 1000);
    }

});

$('.m-chat__find-allbase').hide();
$('.m-chat__find').hide();

$('#findChat').on('input', function() {
    var value = $('#findChat').val().length;
    if (value) {
        $('.m-chat__find-main .letter').hide();
        $('.m-chat__find-allbase').show();
    } else {
        $('.m-chat__find-main .letter').show();
        $('.m-chat__find-allbase').hide();
    }
});

$('.m-chat__find-header .back').on('click', function() {
    $('.m-chat__find').hide();
});

$('.m-chat__header-find .new-message').on('click', function() {
    $('.m-chat__find').show();
});
