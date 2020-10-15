/*if (window.location.hash!='#lastMessage') {
    $(document).ready(function() {
        window.hashName = window.location.hash;
        window.location.hash = '#lastMessage';
        $('html').animate({scrollTop: $(window.hashName).offset().top}, 10, function() {
            window.location.hash = window.hashName;
        });
    });
}*/

$('.message__header .edit').on('click', function() {
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

$('.toBottom').hide();

var countOfMessages = document.querySelectorAll('.message-in').length;

$('.message__main').on('mousewheel DOMMouseScroll', function(event){
    var course = event.originalEvent.wheelDelta;
    if(course  > 0 && countOfMessages > 7){
      $('.toBottom').show();
  }
  else{
      $('.toBottom').hide();
  }
});

/*$('.message-send-btn').on('click', function() {
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
        countOfMessages++;
        $('.message__main').on('mousewheel DOMMouseScroll', function(event){
            var course = event.originalEvent.wheelDelta;
            if(course  > 0 && countOfMessages > 7){
              $('.toBottom').show();
          }
          else{
              $('.toBottom').hide();
          }
        });
    }
    $('html,body').stop().animate({ scrollTop: '1000' }, 1000);
});*/