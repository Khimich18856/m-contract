document.addEventListener("DOMContentLoaded", function () {
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

    var countOfCategory = 0;

    var emptyCategory = document.querySelector('.category-empty')

    //      have-category-item - категории есть ВЕШАТЬ НА btn__category
    //           count-of-cats - КОЛ-ВО КАТЕГОРИЙ (1)
    //dnthave-select-category  - НЕТ ВЫБРАННАЯ КАТЕГОРИЯ В СПИСКЕ
    //   have-select-category  - ВЫБРАННАЯ КАТЕГОРИЯ В СПИСКЕ
    var itemOfCategory = document.querySelectorAll('.category__content-item');

    for (var i = 0; i < itemOfCategory.length; i++) {

        itemOfCategory[i].addEventListener('click', function () {
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
    }


    clearButton.addEventListener('click', function () {
        countOfCategory = 0;
        countOfCategoryBtn.textContent = countOfCategory;
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
    });

    openCategory.addEventListener('click', function () {
        categoryWindow.classList.remove('closed');
        regionWindow.classList.add('closed');
    });

    openReg.addEventListener('click', function () {
        regionWindow.classList.remove('closed');
        categoryWindow.classList.add('closed');
    });

    closeCategory.addEventListener('click', function () {
        categoryWindow.classList.add('closed');
    });

    closeReg.addEventListener('click', function () {
        regionWindow.classList.add('closed');
    });

    openRegList.addEventListener('click', function () {
        regList.classList.toggle('closed-regions');
        regMargin.classList.toggle('closed-margin');
    });

    var regionsList = document.querySelectorAll('.region__content-list-li');
    var regionListEmpty = document.querySelector('.region-items');
    var countOfRegsBtn = document.querySelector('.count-of-regs');
    var regionEmpty = document.querySelector('.region-empty');
    var listOfSelectedRegions = document.querySelector('.region-items');
    var clearRegs = document.querySelector('.clear-regions');
    var countOfRegions = 0;

    for (var m = 0; m < regionsList.length; m++) {
        regionsList[m].addEventListener('click', function () {
            this.classList.toggle('selected-region');
            if (this.classList.contains('selected-region')) {
                countOfRegions++;
                var newElementOfRegion = document.createElement('div');
                newElementOfRegion.classList.add('region-items-item');
                newElementOfRegion.textContent = this.textContent;
                regionListEmpty.prepend(newElementOfRegion);
            } else {
                countOfRegions--;
                var collectionOfSelectedRegs = document.querySelectorAll('.region-items-item');
                for (var c = 0; c < collectionOfSelectedRegs.length; c++) {
                    if (this.textContent == collectionOfSelectedRegs[c].textContent) {
                        collectionOfSelectedRegs[c].remove();
                    }
                }
            }

            countOfRegsBtn.textContent = countOfRegions;

            if (!(document.querySelectorAll('.selected-region').length == 0)) {
                openReg.classList.add('have-region-item');
                regionEmpty.classList.add('have-select-region');
                listOfSelectedRegions.classList.remove('dnthave-select-region');
            } else {
                openReg.classList.remove('have-region-item');
                regionEmpty.classList.remove('have-select-region');
                listOfSelectedRegions.classList.add('dnthave-select-region');
            }
        });
    };

    clearRegs.addEventListener('click', function () {
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
        openReg.classList.remove('have-region-item');
        regionEmpty.classList.remove('have-select-region');
        listOfSelectedRegions.classList.add('dnthave-select-region');
    });

    var lkOpen = document.querySelector('.button__lk');
    var lkWindow = document.querySelector('.lk__wrapper');
    var lkClose = document.querySelector('.close__btn-lk');
    var lkOpenWindow = document.querySelector('.nav__lk');

    lkOpen.addEventListener('click', function () {
        lkWindow.classList.remove('closed-lk');
    });

    lkOpenWindow.addEventListener('click', function () {
        lkWindow.classList.remove('closed-lk');
    });

    lkClose.addEventListener('click', function () {
        lkWindow.classList.add('closed-lk');
    })

    $(document).mouseup(function (e) {
        var container = $('.lk__wrapper');
        if (container.has(e.target).length === 0) {
            container.addClass('closed-lk');
        }
    });
});