
function replaceIsoDateTimeWithLocalDateTime(objs) {
    if ($(objs).length > 0) {
        var offset = new Date().getTimezoneOffset() * 60000;
        $(objs).each(function () {
            localDate = new Date($.trim($(this).html()));
            var milliseconds = localDate.getMilliseconds();
            localDate.setMilliseconds(milliseconds - offset);
            var result = localDate.toLocaleString("ru-RU");
            if (result != 'Invalid date')
                $(this).html(result); 
        });
    }
};