jQuery(document).ready(function ($) {
    alert('MyAds.js started');
    if (document.body.clientWidth > 767) {
        alert('document.body.clientWidth > 767');
        alert('.lk-product__item.length = ' + $('.lk-product__item').length);
        $('.lk-product__item').remove();
    } else {
        alert('document.body.clientWidth <= 767');
        alert('.lk-product__item_d.length = ' + $('.lk-product__item_d').length);
        $('.lk-product__item_d').remove();
    }
});