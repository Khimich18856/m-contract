var url_string = window.location.href;
var url = new URL(url_string);
var url_cats = url.searchParams.get("selectedCategoryIds");
var url_towns = url.searchParams.get("selectedTownIds");
var url_regions = url.searchParams.get("selectedRegionIds");

var openCategory = document.querySelector('.btn__category');
var categoryWindow = document.querySelector('.category__wrapper');
var regionWindow = document.querySelector('.region__wrapper');
var closeCategory = document.querySelector('.close__btn');
var openReg = document.querySelector('.btn__region');
var closeReg = document.querySelector('.close__btn-reg');
var regList = document.querySelector('.region__content-list-wrapper');
var openRegList = document.querySelector('.region__content-category-item');
var regMargin = document.querySelector('.region__content-rus');
var listOfSelectedCategory = document.querySelector('.category-items');

var clearButton = document.querySelector('.clear-category');

var listOfCategoty = document.querySelector('.category__content-list');

var countOfCategoryBtn = document.querySelector('.count-of-cats');

var countOfCategory = function () {
    return $('.category-items-item').length;
};

var emptyCategory = document.querySelector('.category-empty')

var regionsList = document.querySelectorAll('.region__content-list-li-ul-li');
var regionListEmpty = document.querySelector('.region-items');
var countOfRegsBtn = document.querySelector('.count-of-regs');
var regionEmpty = document.querySelector('.region-empty');
var listOfSelectedRegions = document.querySelector('.region-items');
var clearRegs = document.querySelector('.clear-regions');
var countOfRegions = 0;

//      have-category-item - категории есть ВЕШАТЬ НА btn__category
//           count-of-cats - КОЛ-ВО КАТЕГОРИЙ (1)
//dnthave-select-category  - НЕТ ВЫБРАННАЯ КАТЕГОРИЯ В СПИСКЕ
//   have-select-category  - ВЫБРАННАЯ КАТЕГОРИЯ В СПИСКЕ
var itemOfCategory = document.querySelectorAll('.third-level');

var newRegions = function() {
    var region = $('.full-region');
    var newIdR = [];
    for (var r = 0; r < region.length; r++) {
        var idRegion = region[r].getAttribute('data-region-id');
        newIdR.push(idRegion);
    }
    $('#selectedRegions').attr('value', newIdR);
    $('.region-items-item .ico').on('click', function() {
        var curText = $(this).parent().text();
        var selector = '.selected-region:contains("' + curText + '")';
        $(selector).removeClass('selected-region');
        $(this).parent().remove();
        if (!($('.region-items-item').length)) {
            openReg.classList.remove('have-region-item');
            regionEmpty.classList.remove('have-select-region');
            listOfSelectedRegions.classList.add('dnthave-select-region');
        }
        newRegions();
        newReg();
    });
}

var newReg = function() {
    var regs = $('.region');
    var newId = [];
    for (var l = 0; l < regs.length; l++) {
        var idCurrent = regs[l].getAttribute('data-town-id');
        newId.push(idCurrent);
    }
    newRegions();
    $('#selectedTownIds').attr('value', newId);
    $('.region-items-item .ico').on('click', function() {
        var curText = $(this).parent().text();
        var selector = '.selected-region:contains("' + curText + '")';
        $(selector).removeClass('selected-region');
        $(this).parent().remove();
        if (!($('.region-items-item'))) {
            openReg.classList.remove('have-region-item');
            regionEmpty.classList.remove('have-select-region');
            listOfSelectedRegions.classList.add('dnthave-select-region');
        }
        newRegions();
        newReg();
    });
};

var newCatsId = function() {
    var cats = $('.category-items-item');
    var newIdc = [];
    for (var l = 0; l < cats.length; l++) {
        var idCurrent = cats[l].getAttribute('data-category-id');
        newIdc.push(idCurrent);
    }
    $('#selectedCategoryIds').attr('value', newIdc);
    $('.category-items-item .ico').on('click', function() {
        var curText = $(this).parent().text();
        var selector = '.selected:contains("' + curText + '")';
        $(selector).removeClass('selected');
        $(this).parent().remove();
        countOfCategoryBtn.textContent = countOfCategory();
        if (!(countOfCategory() == 0)) {
            openCategory.classList.add('have-category-item');
            emptyCategory.classList.add('have-select-category');
            listOfSelectedCategory.classList.remove('dnthave-select-category');
        } else {
            openCategory.classList.remove('have-category-item');
            emptyCategory.classList.remove('have-select-category');
            listOfSelectedCategory.classList.add('dnthave-select-category');
        }
        newCatsId();
    });
}

function setCats (arrayOfCats) {
    for (var x = 0; x < arrayOfCats.length; x += 1) {
        var dataAttr = arrayOfCats[x];
        var selector = '.category__content' + ' ' + 'span[data-category-id="' + dataAttr + '"' + ']';
        $(selector).addClass('selected');
        if ($(selector).parent('.second-level')) {
            $(selector).parent().parent().parent().parent().children('.first-level').addClass('selected');
            $(selector).parent().parent().parent().parent().children('.category__content-item-ul').removeClass('off');
        }

        if ($(selector).parent('.third-level')) {
            $(selector).parent().parent().removeClass('off');
            $(selector).parent().parent().parent().children('.second-level').addClass('selected');
            $(selector).parent().parent().parent().parent().removeClass('off');
            $(selector).parent().parent().parent().parent().parent().children('.first-level').addClass('selected');
        }

        var newElementOfCatefory = document.createElement('div');
            newElementOfCatefory.classList.add('category-items-item');
            newElementOfCatefory.textContent = $(selector).text();
        var thisDataId = arrayOfCats[x];
            newElementOfCatefory.setAttribute('data-category-id', thisDataId);
        var ico = document.createElement('div');
            ico.classList.add('ico');
            newElementOfCatefory.prepend(ico);
            listOfSelectedCategory.prepend(newElementOfCatefory);
            newCatsId();

    }
    countOfCategoryBtn.textContent = countOfCategory();
    if (!(document.querySelectorAll('.selected').length == 0)) {
        openCategory.classList.add('have-category-item');
        emptyCategory.classList.add('have-select-category');
        listOfSelectedCategory.classList.remove('dnthave-select-category');
    } else {
        openCategory.classList.remove('have-category-item');
        emptyCategory.classList.remove('have-select-category');
        listOfSelectedCategory.classList.add('dnthave-select-category');
    }
}
if (url_cats) {
    var arr_cats = url_cats.split(',');
    setCats(arr_cats);
}

function setTowns (arrayOfTowns) {
    for (var z = 0; z < arrayOfTowns.length; z += 1) {
        var dataAttr = arrayOfTowns[z];
        var selector = '.region__content-list-wrapper' + ' ' + 'li[data-town-id="' + dataAttr + '"' + ']';
        $(selector).addClass('selected-region');
        $(selector).parent().removeClass('off');
        $(selector).parent().parent().children('.region-first-level').addClass('selected-region');

        countOfRegions++;

        var newElementOfRegion = document.createElement('div');
        newElementOfRegion.classList.add('region-items-item');
        newElementOfRegion.classList.add('region');
        newElementOfRegion.textContent = $(selector).text();
        newElementOfRegion.setAttribute('data-town-id', arrayOfTowns[z]);
        
        var ico = document.createElement('div');
            ico.classList.add('ico');
            newElementOfRegion.append(ico);
        regionListEmpty.prepend(newElementOfRegion);
        newRegions();
    }
    countOfRegsBtn.textContent = countOfRegions;
        
    if (!(countOfRegsBtn.textContent == 0)) {
        openReg.classList.add('have-region-item');
        regionEmpty.classList.add('have-select-region');
        listOfSelectedRegions.classList.remove('dnthave-select-region');
    } else {
        openReg.classList.remove('have-region-item');
        regionEmpty.classList.remove('have-select-region');
        listOfSelectedRegions.classList.add('dnthave-select-region');
    }
    newReg();
    newRegions();
}
if (url_towns) {
    var arr_towns = url_towns.split(',');
    setTowns(arr_towns);
}

function setRegs (arrayOfRegs) {
    for (var q = 0; q < arrayOfRegs.length; q += 1) {
        var dataAttr = arrayOfRegs[q];
        var selector = '.region__content-list-wrapper' + ' ' + 'span[data-region-id="' + dataAttr + '"' + ']';
        $(selector).addClass('selected-region');

        countOfRegions++;

        var newElementOfRegion = document.createElement('div');
        newElementOfRegion.classList.add('region-items-item');
        newElementOfRegion.classList.add('full-region');
        newElementOfRegion.textContent = $(selector).text();
        newElementOfRegion.setAttribute('data-region-id', arrayOfRegs[q]);
        var ico = document.createElement('div');
            ico.classList.add('ico');
            newElementOfRegion.append(ico);
        regionListEmpty.prepend(newElementOfRegion);
        newRegions();
    }
    countOfRegsBtn.textContent = countOfRegions;
        
    if (!(countOfRegsBtn.textContent == 0)) {
        openReg.classList.add('have-region-item');
        regionEmpty.classList.add('have-select-region');
        listOfSelectedRegions.classList.remove('dnthave-select-region');
    } else {
        openReg.classList.remove('have-region-item');
        regionEmpty.classList.remove('have-select-region');
        listOfSelectedRegions.classList.add('dnthave-select-region');
    }
    newReg();
    newRegions();
}

if (url_regions) {
    var arr_regions = url_regions.split(',');
    setRegs(arr_regions);
}

for (var i = 0; i < itemOfCategory.length; i++) {

    itemOfCategory[i].addEventListener('click', function() {
        this.classList.toggle('selected');
        if (this.classList.contains('selected')) {
            var newElementOfCatefory = document.createElement('div');
            newElementOfCatefory.classList.add('category-items-item');
            newElementOfCatefory.textContent = this.textContent;
            var thisDataId = this.getAttribute('data-category-id');
            newElementOfCatefory.setAttribute('data-category-id', thisDataId);
            var ico = document.createElement('div');
            ico.classList.add('ico');
            newElementOfCatefory.prepend(ico);
            listOfSelectedCategory.prepend(newElementOfCatefory);
        } else {
            var collectionOfSelectedItems = document.querySelectorAll('.category-items-item');
            for (var c = 0; c < collectionOfSelectedItems.length; c++) {
                if (this.textContent == collectionOfSelectedItems[c].textContent) {
                    collectionOfSelectedItems[c].remove();
                }
            }
        }

        countOfCategoryBtn.textContent = countOfCategory();
        

        if (!(document.querySelectorAll('.selected').length == 0)) {
            openCategory.classList.add('have-category-item');
            emptyCategory.classList.add('have-select-category');
            listOfSelectedCategory.classList.remove('dnthave-select-category');
        } else {
            openCategory.classList.remove('have-category-item');
            emptyCategory.classList.remove('have-select-category');
            listOfSelectedCategory.classList.add('dnthave-select-category');
        }
        newCatsId();
    });
}

$('.first-level span').on('click', function() {
    $(this).toggleClass('selected');

    if ($(this).hasClass('selected')) 
    {
        var newElementOfCatefory = document.createElement('div');
        newElementOfCatefory.classList.add('category-items-item');
        newElementOfCatefory.textContent = this.textContent;
        var thisDataId = this.getAttribute('data-category-id');
        newElementOfCatefory.setAttribute('data-category-id', thisDataId);
        var ico = document.createElement('div');
            ico.classList.add('ico');
            newElementOfCatefory.prepend(ico);
        listOfSelectedCategory.prepend(newElementOfCatefory);

        $(this).parent().children('.ico').on('click', function() {
            $(this).parent().removeClass('selected');
            $(this).parent().parent().children('.category__content-item-ul').addClass('off');
        });
    }
    else 
    {
        var collectionOfSelectedItems = document.querySelectorAll('.category-items-item');
        for (var c = 0; c < collectionOfSelectedItems.length; c++) {
            if (this.textContent == collectionOfSelectedItems[c].textContent) {
                collectionOfSelectedItems[c].remove();
            }
        }

        $(this).parent().children('.ico.no').on('click', function() {
            $(this).parent().addClass('selected');
            $(this).parent().parent().children('.category__content-item-ul').removeClass('off');
        });
        $(this).parent().children('.ico.yes').on('click', function() {
            $(this).parent().removeClass('selected');
            $(this).parent().parent().children('.category__content-item-ul').addClass('off');
        });
    }
    countOfCategoryBtn.textContent = countOfCategory();
    var lengthOfCat = countOfCategory();

    if (!(lengthOfCat == 0)) {
        openCategory.classList.add('have-category-item');
        emptyCategory.classList.add('have-select-category');
        listOfSelectedCategory.classList.remove('dnthave-select-category');
    } else {
        openCategory.classList.remove('have-category-item');
        emptyCategory.classList.remove('have-select-category');
        listOfSelectedCategory.classList.add('dnthave-select-category');
    }
    newCatsId();
});

$('.first-level .ico').on('click', function() {
    $(this).parent().toggleClass('selected');
    $(this).parent().parent().children('.category__content-item-ul').toggleClass('off');
    if ($(this).parent().hasClass('selected')) {
        $(this).parent().children('span').on('click', function() {
            $(this).parent().toggleClass('selected');
            $(this).parent().parent().children('.category__content-item-ul').toggleClass('off');
        });
    } else {
        $(this).parent().children('span').on('click', function() {
            $(this).toggleClass('selected');

            if ($(this).hasClass('selected')) 
            {

                var collectionOfSecLevel = $('.category__content-item-ul-li span');

                var collectionOfSelectedItems = document.querySelectorAll('.category-items-item');
                for (var c = 0; c < collectionOfSelectedItems.length; c++) {
                    for (var l = 0; l < collectionOfSecLevel.length; l++) {
                        if (collectionOfSecLevel[l].textContent == collectionOfSelectedItems[c].textContent) {
                            collectionOfSelectedItems[c].remove();
                            collectionOfSecLevel[l].classList.remove('selected');
                        }
                    }
                }
                var newElementOfCatefory = document.createElement('div');
                newElementOfCatefory.classList.add('category-items-item');
                newElementOfCatefory.textContent = this.textContent;
                var thisDataId = this.getAttribute('data-category-id');
                newElementOfCatefory.setAttribute('data-category-id', thisDataId);
                var ico = document.createElement('div');
                ico.classList.add('ico');
                newElementOfCatefory.prepend(ico);
                listOfSelectedCategory.prepend(newElementOfCatefory);
        
                $(this).parent().children('.ico').on('click', function() {
                    $(this).parent().removeClass('selected');
                    $(this).parent().parent().children('.category__content-item-ul').addClass('off');
                });
            }
            else 
            {
                var collectionOfSelectedItems = document.querySelectorAll('.category-items-item');
                for (var c = 0; c < collectionOfSelectedItems.length; c++) {
                    if (this.textContent == collectionOfSelectedItems[c].textContent) {
                        collectionOfSelectedItems[c].remove();
                    }
                }
        
                $(this).parent().children('.ico.no').on('click', function() {
                    $(this).parent().addClass('selected');
                    $(this).parent().parent().children('.category__content-item-ul').removeClass('off');
                });
                $(this).parent().children('.ico.yes').on('click', function() {
                    $(this).parent().removeClass('selected');
                    $(this).parent().parent().children('.category__content-item-ul').addClass('off');
                });
            }
        
            countOfCategoryBtn.textContent = countOfCategory();
        
            var lengthOfCat = countOfCategory();

            if (!(lengthOfCat == 0)) {
                openCategory.classList.add('have-category-item');
                emptyCategory.classList.add('have-select-category');
                listOfSelectedCategory.classList.remove('dnthave-select-category');
            } else {
                openCategory.classList.remove('have-category-item');
                emptyCategory.classList.remove('have-select-category');
                listOfSelectedCategory.classList.add('dnthave-select-category');
            }
        });
    }
});

$('.second-level span').on('click', function() {
    $(this).toggleClass('selected');

    if ($(this).hasClass('selected')) 
    {
        var newElementOfCatefory = document.createElement('div');
        newElementOfCatefory.classList.add('category-items-item');
        newElementOfCatefory.textContent = this.textContent;
        var thisDataId = this.getAttribute('data-category-id');
        newElementOfCatefory.setAttribute('data-category-id', thisDataId);
        var ico = document.createElement('div');
            ico.classList.add('ico');
            newElementOfCatefory.prepend(ico);
        listOfSelectedCategory.prepend(newElementOfCatefory);

        $(this).parent().children('.ico').on('click', function() {
            $(this).parent().removeClass('selected');
            $(this).parent().parent().children('.category__content-item-ul').addClass('off');
        });
    }
    else 
    {
        var collectionOfSelectedItems = document.querySelectorAll('.category-items-item');
        for (var c = 0; c < collectionOfSelectedItems.length; c++) {
            if (this.textContent == collectionOfSelectedItems[c].textContent) {
                collectionOfSelectedItems[c].remove();
            }
        }

        $(this).parent().children('.ico.no').on('click', function() {
            $(this).parent().addClass('selected');
            $(this).parent().parent().children('.category__content-item-ul').removeClass('off');
        });
        $(this).parent().children('.ico.yes').on('click', function() {
            $(this).parent().Add('selected');
            $(this).parent().parent().children('.category__content-item-ul').addClass('off');
        });
    }

    countOfCategoryBtn.textContent = countOfCategory();

    var lengthOfCat = countOfCategory();

    if (!(lengthOfCat == 0)) {
        openCategory.classList.add('have-category-item');
        emptyCategory.classList.add('have-select-category');
        listOfSelectedCategory.classList.remove('dnthave-select-category');
    } else {
        openCategory.classList.remove('have-category-item');
        emptyCategory.classList.remove('have-select-category');
        listOfSelectedCategory.classList.add('dnthave-select-category');
    }
    newCatsId();
});

$('.second-level .ico').on('click', function() {
    $(this).parent().toggleClass('selected');
    $(this).parent().parent().children('.category__content-item-ul-li-ul').toggleClass('off');
    if ($(this).parent().hasClass('selected')) { 
        $(this).parent().children('span').on('click', function() {
            $(this).parent().toggleClass('selected');
            $(this).parent().parent().children('.category__content-item-ul-li-ul').toggleClass('off');
        });
    } else {
        $(this).parent().children('span').on('click', function() {
            $(this).toggleClass('selected');

            if ($(this).hasClass('selected')) 
            {
                var newElementOfCatefory = document.createElement('div');
                newElementOfCatefory.classList.add('category-items-item');
                newElementOfCatefory.textContent = this.textContent;
                var thisDataId = this.getAttribute('data-category-id');
                newElementOfCatefory.setAttribute('data-category-id', thisDataId);
                var ico = document.createElement('div');
                ico.classList.add('ico');
                newElementOfCatefory.prepend(ico);
                listOfSelectedCategory.prepend(newElementOfCatefory);
        
                $(this).parent().children('.ico').on('click', function() {
                    $(this).parent().removeClass('selected');
                    $(this).parent().parent().children('.category__content-item-ul-li-ul').addClass('off');
                });
            }
            else 
            {
                var collectionOfSelectedItems = document.querySelectorAll('.category-items-item');
                for (var c = 0; c < collectionOfSelectedItems.length; c++) {
                    if (this.textContent == collectionOfSelectedItems[c].textContent) {
                        collectionOfSelectedItems[c].remove();
                    }
                }
        
                $(this).parent().children('.ico.no').on('click', function() {
                    $(this).parent().addClass('selected');
                    $(this).parent().parent().children('.category__content-item-ul-li-ul').removeClass('off');
                });
                $(this).parent().children('.ico.yes').on('click', function() {
                    $(this).parent().removeClass('selected');
                    $(this).parent().parent().children('.category__content-item-ul-li-ul').addClass('off');
                });
            }
        
            countOfCategoryBtn.textContent = countOfCategory();
        
            var lengthOfCat = countOfCategory();

            if (!(lengthOfCat == 0)) {
                openCategory.classList.add('have-category-item');
                emptyCategory.classList.add('have-select-category');
                listOfSelectedCategory.classList.remove('dnthave-select-category');
            } else {
                openCategory.classList.remove('have-category-item');
                emptyCategory.classList.remove('have-select-category');
                listOfSelectedCategory.classList.add('dnthave-select-category');
            }
        });
    }
});


clearButton.addEventListener('click', function() {
    var collectionOfSelectedItems = document.querySelectorAll('.category-items-item');
    for (var d = 0; d < collectionOfSelectedItems.length; d++) {
        collectionOfSelectedItems[d].remove();
    }
    var selectedItems = document.querySelectorAll('.selected');
    for (var v = 0; v < selectedItems.length; v++) {
        selectedItems[v].classList.remove('selected');
    }
    openCategory.classList.remove('have-category-item');
    emptyCategory.classList.remove('have-select-category');
    listOfSelectedCategory.classList.add('dnthave-select-category');
    $('.category__content-item-ul-li-ul').addClass('off');
    $('.category__content-item-ul').addClass('off');
    countOfCategoryBtn.textContent = countOfCategory();
});

openCategory.addEventListener('click', function() {
    categoryWindow.classList.remove('closed');
    regionWindow.classList.add('closed');
});

openReg.addEventListener('click', function() {
    regionWindow.classList.remove('closed');
    categoryWindow.classList.add('closed');
});

closeCategory.addEventListener('click', function() {
    categoryWindow.classList.add('closed');
});

closeReg.addEventListener('click', function() {
    regionWindow.classList.add('closed');
});

openRegList.addEventListener('click', function() {
    regList.classList.toggle('closed-regions');
    regMargin.classList.toggle('closed-margin');
});

for (var m = 0; m < regionsList.length; m ++) {
    regionsList[m].addEventListener('click', function() {
        this.classList.toggle('selected-region');
        if (this.classList.contains('selected-region')) {
            countOfRegions++;
            var newElementOfRegion = document.createElement('div');
            newElementOfRegion.classList.add('region-items-item');
            newElementOfRegion.classList.add('region');
            newElementOfRegion.textContent = this.textContent;
            var thisDataId = this.getAttribute('data-town-id');
            newElementOfRegion.setAttribute('data-town-id', thisDataId);
            var ico = document.createElement('div');
            ico.classList.add('ico');
            newElementOfRegion.append(ico);
            regionListEmpty.prepend(newElementOfRegion);
            newRegions();
        } else {
            countOfRegions--;
            var collectionOfSelectedRegs = document.querySelectorAll('.region-items-item');
            for (var c = 0; c < collectionOfSelectedRegs.length; c++) {
                if (this.textContent == collectionOfSelectedRegs[c].textContent) {
                    collectionOfSelectedRegs[c].remove();
                }
            }
            newRegions();
        }

        countOfRegsBtn.textContent = countOfRegions;
        
        if (!(countOfRegsBtn.textContent == 0)) {
            openReg.classList.add('have-region-item');
            regionEmpty.classList.add('have-select-region');
            listOfSelectedRegions.classList.remove('dnthave-select-region');
        } else {
            openReg.classList.remove('have-region-item');
            regionEmpty.classList.remove('have-select-region');
            listOfSelectedRegions.classList.add('dnthave-select-region');
        }
        newReg();
        newRegions();
    });
};

$('.region-first-level').on('click', function() {
    $(this).toggleClass('selected-region');
    $(this).parent().children('.region__content-list-li-ul').toggleClass('off');
    newRegions();
});

$('.select-all').on('click', function() {
    if ($(this).hasClass('selected-region')) {
        $(this).parent().children('.region__content-list-li-ul-li').removeClass('selected-region');
        var countOfThisLevel = $(this).parent().children('.region__content-list-li-ul-li').length;
        $(this).removeClass('selected-region');
        var collectionOfSelectedRegs = document.querySelectorAll('.region-items-item');
        for (var k = 0; k < countOfThisLevel; k++) {
            var currentElemReg = $(this).parent().children('.region__content-list-li-ul-li')[k];
            for (var c = 0; c < collectionOfSelectedRegs.length; c++) {
                if (currentElemReg.textContent == collectionOfSelectedRegs[c].textContent) {
                    collectionOfSelectedRegs[c].remove();
                    countOfRegions--;
                }
            }
        }
        countOfRegions++;
        countOfRegsBtn.textContent = countOfRegions;
        var newElementOfRegion = document.createElement('div');
        newElementOfRegion.classList.add('region-items-item');
        newElementOfRegion.classList.add('full-region');
        newElementOfRegion.textContent = $(this).parent().parent().children('.region-first-level').text();
        var newTownId = $(this).parent().parent().children('.region-first-level').attr('data-region-id');
        newElementOfRegion.setAttribute('data-region-id', newTownId);
        var ico = document.createElement('div');
            ico.classList.add('ico');
            newElementOfRegion.append(ico);
        regionListEmpty.prepend(newElementOfRegion);
        newReg();
        $(this).parent().addClass('off');
        $(this).parent().parent().children('.region-first-level').on('click', function() {
            $(this).removeClass('selected-region');
            $(this).parent().children('.region__content-list-li-ul').addClass('off');
            var textToDelete = $(this).text();
            var collectionOfSelectedRegs = document.querySelectorAll('.region-items-item');
            for (var c = 0; c < collectionOfSelectedRegs.length; c++) {
                if (textToDelete == collectionOfSelectedRegs[c].textContent) {
                    collectionOfSelectedRegs[c].remove();
                }
            }
            countOfRegions = $('.region-items-item').length;
            countOfRegsBtn.textContent = countOfRegions;

            if (!(countOfRegsBtn.textContent == 0)) {
                openReg.classList.add('have-region-item');
                regionEmpty.classList.add('have-select-region');
                listOfSelectedRegions.classList.remove('dnthave-select-region');
            } else {
                openReg.classList.remove('have-region-item');
                regionEmpty.classList.remove('have-select-region');
                listOfSelectedRegions.classList.add('dnthave-select-region');
            }
            newReg();
            newRegions();
            $(this).on('click', function(){
                $(this).addClass('selected-region');
                $(this).parent().children('.region__content-list-li-ul').removeClass('off');
                newRegions();
            });
        });
        newRegions();
    } else {
        $(this).removeClass('selected-region');
        $(this).parent().children('.region__content-list-li-ul-li').removeClass('selected-region');
        var thisFirstElemText = $(this).parent().parent().children('.region-first-level').text();
        var collectionOfSelectedRegs = document.querySelectorAll('.region-items-item');
        for (var c = 0; c < collectionOfSelectedRegs.length; c++) {
            if (thisFirstElemText == collectionOfSelectedRegs[c].textContent) {
                collectionOfSelectedRegs[c].remove();
            }
        }
        countOfRegsBtn.textContent = countOfRegions;

        if (!(countOfRegsBtn.textContent == 0)) {
            openReg.classList.add('have-region-item');
            regionEmpty.classList.add('have-select-region');
            listOfSelectedRegions.classList.remove('dnthave-select-region');
        } else {
            openReg.classList.remove('have-region-item');
            regionEmpty.classList.remove('have-select-region');
            listOfSelectedRegions.classList.add('dnthave-select-region');
        }
        $(this).parent().parent().children('.region-first-level').on('click', function() {
            $(this).toggleClass('selected-region');
            $(this).parent().children('.region__content-list-li-ul').toggleClass('off');
            newRegions();
        });
    }
});

clearRegs.addEventListener('click', function() {
    countOfRegions = 0;
    countOfRegsBtn.textContent = countOfCategory;
    var collectionOfSelectedRegs = document.querySelectorAll('.region-items-item');
    for (var k = 0; k < collectionOfSelectedRegs.length; k++) {
        collectionOfSelectedRegs[k].remove();
    }
    var selectedRegs = document.querySelectorAll('.selected-region');
    for (var a = 0; a < selectedRegs.length; a++) {
        selectedRegs[a].classList.remove('selected-region');
    }
    $('.region__content-list-li-ul').addClass('off');
    openReg.classList.remove('have-region-item');
    regionEmpty.classList.remove('have-select-region');
    listOfSelectedRegions.classList.add('dnthave-select-region');
    newReg();
    newRegions();
});

var lkOpen = document.querySelector('.button__lk');
var lkWindow = document.querySelector('.lk__wrapper');
var lkClose = document.querySelector('.close__btn-lk');
var lkOpenWindow = document.querySelector('.nav__lk');
var registeredUser = document.querySelector('.registered');

lkOpen.addEventListener('click', function() {
    lkWindow.classList.remove('closed-lk');
});

if (registeredUser) {
    registeredUser.onmouseover = function(e) {
        lkWindow.classList.remove('closed-lk');
    };
}

lkClose.addEventListener('click', function() {
    lkWindow.classList.add('closed-lk');
})

$(document).mouseup(function (e) {
    var container = $('.lk__wrapper');
    if (container.has(e.target).length === 0){
        container.addClass('closed-lk');
    }
});

$('.open-favour').on('click', function (e) {
    e.preventDefault();
    this.classList.toggle('selected');
});

$('.product__item-favour').on('click', function () {
    this.classList.toggle('selected');
});

$('.select-input').on('click', function() {
    $(this).children('.select-list').toggleClass('closed');
});

$('.select-list-item').on('click', function() {
    var clickedText = $(this).text();
    $(this).parent().parent().children('span').text(clickedText);
});

$('.false-enter input').attr('placeholder', 'Укажите форму собственности');

$('.region-input').on('click', function() {
    $('.select-region__wrapper').toggleClass('closed');
});

$(document).mouseup(function (e) {
    var container = $('.select-region__wrapper');
    if (container.has(e.target).length === 0){
        container.addClass('closed');
    }
});

$('.select-region__wrapper .select-region-first-level').on('click', function() {
    $(this).toggleClass('selected-region');
    $(this).parent().children('.select-region__content-list-li-ul').toggleClass('off');
    $('.region-input').on('click', function() {
        $('.select-region__wrapper').toggleClass('closed');
    });
});

$('.select-region__content-list-li-ul-li').on('click', function() {
    $(this).addClass('selected-region');
    $('.select-input.region-input span.resultCity').addClass('selected-region-list-item-span');
    var regionSelected = $('.selected-region-list-item-span');
    var clickedRegionText = $(this).text();
    regionSelected.text(clickedRegionText);
    $('.select-region__content-list-li-ul').addClass('off');
    $('.select-region-first-level').removeClass('selected-region');
    $('.select-region__content-list-li-ul-li').removeClass('selected-region');
    $('.select-region__wrapper').toggleClass('closed');
});

var number = 1;

$('.close').on('click', function() {
    $('.selected-region-list-item-span').removeClass('selected-region-list-item-span');
    $('.resultCity').text('');
    $('.region-input').on('click', function() {
        $('.select-region__wrapper').toggleClass('closed');
    });
});

var allId = '';

$('.inp__search .input__button').on('click', function() {
    var cateoriesInp = $('#selectedCategoryIds').val();
    var towns = $('#selectedTownIds').val();
    var regions = $('#selectedRegions').val();
    window.location.href = $('#siteUrl').val() + "/Ads/index/?selectedCategoryIds=" + cateoriesInp + "&selectedTownIds=" + towns + "&selectedRegionIds=" + regions;
});