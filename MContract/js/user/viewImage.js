function prepareImageViewElements() {
    $("body").css("position", "relative");
    var imageViewContainer = $("<div class='imageViewContainer'>")[0];
    $("body").append(imageViewContainer);
    var topbarHeight = $(".navbar-fixed-top").height();
    var containerHeight = $(window).height() > $("body").height() ? $(window).height() : $("body").height();
    containerHeight -= topbarHeight;
    $(".imageViewContainer").css({
        "display": "none",
        "backdrop-filter": "blur(2px)",
        "justify-content": "center",
        "align-items": "center",
        "background-color": "rgba(128, 128, 128, 0.5)",
        "position": "fixed",
        "height": containerHeight,
        "width": "100%",
        "top": topbarHeight,
        "left": "0"
    });
    var imageViewLeftButtonContainer = $("<div class='imageViewLeftButtonContainer'>")[0];
    $(".imageViewContainer").append(imageViewLeftButtonContainer);
    $(".imageViewLeftButtonContainer").css({
        "position": "absolute",
        "z-index": "auto",
        "height": "calc(100vh - 50px)",
        "width": "50%",
        "display": "flex",
        "flex-direction": "row",
        "justify-content": "flex-start",
        "align-items": "center",
        "top": "0",
        "left": "0"
    });
    var imageViewLeftButton = $("<img class='imageViewLeftButton'>")[0];
    $(".imageViewLeftButtonContainer").append(imageViewLeftButton);
    $(".imageViewLeftButton").attr("src", "/ico/ArrowRight.svg").css({
        "height": "auto",
        "width": "10%",
        "-moz-transform": "rotate(180deg)",
        "-webkit-transform": "rotate(180deg)",
        "-ms-transform": "rotate(180deg)",
        "transform": "rotate(180deg)" 
    });
    var imageViewRightButtonContainer = $("<div class='imageViewRightButtonContainer'>")[0];
    $(".imageViewContainer").append(imageViewRightButtonContainer);
    $(".imageViewRightButtonContainer").css({
        "position": "absolute",
        "z-index": "auto",
        "height": "calc(100vh - 50px)",
        "width": "50%",
        "display": "flex",
        "flex-direction": "row",
        "justify-content": "flex-end",
        "align-items": "center",
        "top": "0",
        "right": "0"
    });
    var imageViewRightButton = $("<img class='imageViewRightButton'>")[0];
    $(".imageViewRightButtonContainer").append(imageViewRightButton);
    $(".imageViewRightButton").attr("src", "/ico/ArrowRight.svg").css({
        "height": "auto",
        "width": "10%"
    });
    var closeImageView = $("<img class='closeImageView'>")[0];
    $(".imageViewContainer").append(closeImageView);
    $(".closeImageView").attr("src", "/ico/CircledX.svg").css({
        "position": "absolute",
        "z-index": "auto",
        "height": "5%",
        "width": "auto",
        "top": "5%",
        "right": "5%"
    });
    var imageViewFlexContainer = $("<div class='imageViewFlexContainer'>")[0];
    $(".imageViewContainer").append(imageViewFlexContainer);
    $(".imageViewFlexContainer").css({
        "display": "flex",
        "justify-content": "center",
        "align-items": "center",
        "width": "100%",
        "height": "calc(100vh - 50px)"
    });
    var imageView = $("<img class='imageView'>")[0];
    $(".imageViewFlexContainer").append(imageView)
    $(".imageView").css({
        "height": "80%",
        "width": "80%",
        "object-fit": "contain"
    });
    $(".imageViewContainer .closeImageView").on("click", function () {
        $(".imageViewContainer").hide();
    });
    $(document).on("keyup", function (event) {
        if ($(".imageViewContainer:visible").length != 0) {
            if ($(".imageViewLeftButtonContainer:visible").length != 0) {
                if (event.which == 37) {
                    event.preventDefault();
                    $(".imageViewLeftButtonContainer").trigger("click");
                }
            }
            if ($(".imageViewRightButtonContainer:visible").length != 0) {
                if (event.which == 39) {
                    event.preventDefault();
                    $(".imageViewRightButtonContainer").trigger("click");
                }
            }
        }
    });
}

function showImageView(obj) {
    $(".imageViewContainer .imageView").attr("src", $(obj).attr("data-url"));
    var objClass = $(obj).attr("class");
    var indexOfObj = parseInt($(obj).parent().children("[class=" + objClass + "]").index(obj));
    var indexOfLast = parseInt($(obj).parent().children("[class=" + objClass + "]").length) - 1;
    if (indexOfObj == 0) {
        $(".imageViewContainer .imageViewLeftButtonContainer").hide();
    } else {
        $(".imageViewContainer .imageViewLeftButtonContainer").show();
        $(".imageViewLeftButtonContainer").off("click");
        $(".imageViewLeftButtonContainer").on("click", function () {
            showImageView($(obj).prev("[class=" + objClass + "]"));
        });
    }
    if (indexOfObj == indexOfLast) {
        $(".imageViewContainer .imageViewRightButtonContainer").hide();
    } else {
        $(".imageViewContainer .imageViewRightButtonContainer").show();
        $(".imageViewRightButtonContainer").off("click");
        $(".imageViewRightButtonContainer").on("click", function () {
            showImageView($(obj).next("[class=" + objClass + "]"));
        });
    }
    $(".imageViewContainer").show();
    return true;
}