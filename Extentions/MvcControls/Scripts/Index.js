//<doc>
//<it> <#-20/> Общая структура скриптов папки Site:</it>
//</it>
//<it> <#-20/> Index.js - скрипт, запускающийся в самом начале после создания вьюхи Index. Создаются главные панели, заголовок, их обработчки и пр.</it>
//</it>
//<it> <#-20/> Site.js - скрипт содержит глобальные переменные и общие функции, которые могут быть использованы в любом месте. Это функции для работы с url, uri_path, функции определения высоты различных контейнеров.</it>
//</it>
//<it> <#-20/> SiteEntity.js - функции для работы с данными, считывание этих данных с контролов, сохранение и отрисовка контролов, которые отображают эти данные.</it>
//</it>
//<it> <#-20/> SiteGrid.js - функции для работы с гридами, деревьями и списками.</it>
//</it>
//<it> <#-20/> SiteInputEvent.js - обработчики событий у контролов ввода.</it>
//</it>
//<it> <#-20/> SiteMainTableFilter.js - функции для работы с главным фильтром.</it>
//</it>
//<it> <#-20/> SiteReady.js - функции, которые вызываются сразу после созданию вьюх (событие ready).</it>
//</it>
//<it> <#-20/> SiteSelector.js - функции для работы с селекторами и гридами этих селекторов.</it>
//</it>
//<it> <#-20/> SiteTabs.js - функции для работы с вкладками, а так же открытие этих вкладок разными контролами.</it>
//</it>
//<it> <#-20/> SiteWindow.js - функции для работы с модальными окнами.</it>
//</it>
//</doc>

var isAutoTestClient = false;
var MainMenu_dataSource;

function Index_Ready(
    uri_Content, 
    mainTab_IsAutoLoaded, 
    mainTab_MenuLoad, 
    mainTab_Text, 
    //uriPathStart, 
    soperation_id, 
    guids_out,
    uri_path_out,
    deal_id,
    isLIK_DealInfo,
    holdMainMenu,
    indexParams
    ) {

    uri_Action = uri_Content;

    // Загрузка "Стартовой страницы"
    //mainTab_IsAutoLoaded = "True";
    if (mainTab_IsAutoLoaded == "True" /*|| isAutoTest(uri_path_out)*/) {

        if (mainTab_IsAutoLoaded == "True")
            autoStartPage(uri_Content, mainTab_IsAutoLoaded, mainTab_Text, soperation_id, guids_out, uri_path_out, indexParams);
        else if (isAutoTest(uri_path_out))
            mainTab_Open_AutoTest(uri_Content, mainTab_Text, guids_out, uri_path_out, 0, "Index_Ready");

        if (mainTab_MenuLoad == "False")
            return;
    }

    // Загрузка контейнера DealInfo
    if (isLIK_DealInfo == "True") {
        var guid_out = null;
        if (guids_out.split('=').length > 1)
            guid_out = guids_out.split('=')[1];
        deal_info_set_content(deal_id, uri_Content, guids_out, guid_out, uri_path_out, 1);
    }

    $("#tabstrip .k-tabstrip-items").kendoResponsivePanel({
        breakpoint: 769,
        orientation: "right",
        toggleButton: ".k-rpanel-toggle"
    })
    $("#tabstrip .k-tabstrip-items").kendoTouch({
        enableSwipe: true,
        swipe: function (e) {
            if (e.direction === "right") {
                $("#tabstrip .k-tabstrip-items").data("kendoResponsivePanel").close();
            }
        }
    });

    //function onClose() {
    //    if (window.innerWidth < 1920)
    //        $("#tabstrip > div.k-content").css("left", "400px");
    //}

    $("#left-pane").kendoResponsivePanel({
        breakpoint: 10000,
        orientation: "left",
        toggleButton: ".k-lpanel-toggle",
        //autoClose: false,
        close: onHideMainMenu,
        open: onShowMainMenu
    });
    $("#main_header").kendoResponsivePanel({
        breakpoint: 1000,
        orientation: "top",
        //toggleButton: ".k-lpanel-toggle",
        //autoClose: false,
        //close: onClose
        //open: onOpen
    });

    // реагируем на свайпы
    $("#left-pane").kendoTouch({
        enableSwipe: true,
        swipe: function (e) {
            if (e.direction === "left") {
                //$("#left-pane").data("kendoResponsivePanel").close();
                hideMainMenu();
            }
        }
    });
    if (isMobile()) {
        $("#mobile_app").kendoTouch({
            enableSwipe: true,
            swipe: function (e) {
                if (e.direction === "right") {
                    view_change(false, true);
                }
                if (e.direction === "left") {
                    view_change(true, false);
                }
            }
        });
    }

    // открываем меню с задержкой
    if (mainTab_IsAutoLoaded != "True") {
        setTimeout(function () {
            if ($("#left-pane").data("kendoResponsivePanel") != undefined)
                $("#left-pane").data("kendoResponsivePanel").open();
        }, 500);
    }

    // по любой ссылке в левом меню открываем таб с этой ссылкой
    // перенесено в метод mainMenu_Click()
    //hookLinks("#MainMenu", function (url, sender) {
    //    $("#left-pane").data("kendoResponsivePanel").close();

    //    /*При переходе к соответствующему списку, убираем значок новых элементов*/
    //    if (sender.parentElement.attributes["new_items_pic_type"] != null)
    //        if (sender.parentElement.attributes["need_new_items_pic"] != null > 0 && sender.parentElement.attributes["need_new_items_pic"].value == 'False') {
    //            $("li[new_items_pic_type='" + sender.parentElement.attributes["new_items_pic_type"].value + "']").attr('need_new_items_pic', 'True');
    //            $("li[new_items_pic_type='" + sender.parentElement.attributes["new_items_pic_type"].value + "'] > a > img.k-image-new-items").remove();
    //            $("li[new_items_pic_type='" + sender.parentElement.attributes["new_items_pic_type"].value + "'] > span > img.k-image-new-items").remove();
    //        }

    //    $("div.k-animation-container").css("display", "none");
    //    if (url.indexOf("modal") > 0 && !isMobile()) {
    //        modalWindow111($(sender).text(), url);
    //    } else {
    //        /*Убираем из текста нажатого меню количество непрочитанных сообщений, чтобы оно не попало в заголовок*/
    //        if ($(sender).find("b.k-count-new-items").length > 0)
    //            $(sender).find("b.k-count-new-items").remove();
    //        var title = $(sender).text();
    //        mainTab_Open($("#tabstrip"), title, url, guid_out, guids_out);
    //    }

    //    return false;
    //});


    $("#left-pane").mouseleave(function (url, sender) {
        /*Получаем разметку элемента на который перешел курсор с панели меню*/
        var hover_element = $(url.relatedTarget).attr("class");
        /*Получаем разметку комбобокса с панели меню*/
        var html_of_combobox = "";
        if ($("body > div.k-animation-container")[0] != null)
            html_of_combobox = $("body > div.k-animation-container")[0].outerHTML;

        /*Если курсор перешел не на комбобокс, скрываем меню*/
        if (html_of_combobox.indexOf(hover_element) == -1) {

            // Pavlov 06.02.2017
            // убрал это, меню будет скрываться только по клику - либо на пункт меню, либо на любое другое место в документе

            /*
            $("#left-pane").data("kendoResponsivePanel").close();
            $("div.k-animation-container").css("display", "none");
            */
        }
    });

    /*Открываем табы по ссылкам в области детализации сделки*/
    //hookLinks("#deal_content", function (url, sender) {
    //    OpenLink(url, sender, guids_out);
    //});

    $(window).on("resize", function () {
        // Pavlov 06.02.2017
        // вся логика которая была здесь вынесена в отдельную функцию, которую можно вызывать без вызова метода $(window).resize()
        leftPaneWindowResize();
    });

    $(window).trigger('resize');

    // Pavlov 06.02.2017
    // обработчик click'ов любого места в документе
    $(document).click(function (e) {
        // для деревьев - при клике на область дерева, не являющуюся строкой, снимаем выделение
        var tree = $(e.target).parents('.k-treelist').first();
        if (tree.length > 0) {            
            var row = $(e.target).parents('tr[role="row"]').first();
            if (row.length <= 0) {                
                tree.find('.k-grid-content tr[role="row"].k-state-selected').removeClass('k-state-selected');
            };
        };
        //
        var dontHideMenu = $(e.target).closest('a').hasClass('k-lpanel-toggle');
        //dontHideMenu = dontHideMenu || ($(e.target).parents('#menu_block').length > 0);
        //dontHideMenu = dontHideMenu || ($(e.target).parents('#dealinfo_block').length > 0);
        dontHideMenu = dontHideMenu || ($(e.target).parents('#left-pane').length > 0);
        dontHideMenu = dontHideMenu || ($(e.target).closest('span').hasClass('combobox-for-deals-item'));
        dontHideMenu = dontHideMenu || ($(e.target).children('span').first().hasClass('combobox-for-deals-item'));

        if (dontHideMenu)
            return;

        hideMainMenu();
    });

    if (holdMainMenu == 1) {
        $("#hold_mm_pic").attr("src", uri_Content + "Images/Operations/Small/Fix_On.png")
        window.hold_mm = 1;
        $("#left-pane").removeClass("k-rpanel-left");
        $("#left-pane").addClass("is-menu-relative");
    }
    else {
        $("#left-pane").addClass("is-menu-absolute");
    }

    //Меняются иконки стрелочек у меню. После обновления скриптов они почему-то перевернулись
    let count = $('#MainMenu .k-icon.k-i-arrow-60-right').length;
    for (let i = 0; i < count; i++) {
        $($('#MainMenu .k-icon')[i]).removeClass('k-i-arrow-60-right');
        $($('#MainMenu .k-icon')[i]).addClass('k-i-arrow-e');
    }
}
//);

// Pavlov 06.02.2017
// функция переформатирует левую панель с главным меню и блоком информации по сделке в ЛИК
function leftPaneWindowResize() {
    if ($("#main_header").length > 0) {
        var height = window.innerHeight;
        var height_menu = 0;
        var height_combo = 0;
        var height_dealinfo = 0;
        var height_deal_content = 0;
        if ($("#menu_block_vertical").length > 0)
            height_menu = $("#menu_block_vertical")[0].clientHeight;
        if ($("#combo_for_dealnum").length > 0)
            height_combo = $("#combo_for_dealnum")[0].clientHeight;
        if ($("#dealinfo_block").length > 0) {
            if ($("#tab_edit_content").length > 0)
                $("#dealinfo_block").css("height", $("#tab_edit_content")[0].clientHeight);
            height_dealinfo = $("#dealinfo_block")[0].clientHeight;
        }
        if ($("#deal_content").length > 0)
            height_deal_content = $("#deal_content")[0].clientHeight;

        if (height_dealinfo > (height - height_menu - height_combo))
            $("#dealinfo_block").css("height", (height - height_menu - height_combo));

        if ((height_menu + height_combo) == 0)
            $("#dealinfo_block").css("height", height);
    }
};

/*Асинхронный вызов сервиса для определения количества новых записей для нужных строк меню*/
/*Формируется строка из пар типа "nom:stable_id;" и передается в сервис. Сервис возвращает строку из пар типа "nom:count;"*/
/*По nom находится нужная строка меню и к тексту добавляется count*/
function onOpen() {
    var xhr = getXmlHttp();
    var li = $("li[need_new_items_count='True']");

    if (li.length > 0) {
        var tables = '';
        for (var i = 0; i < li.length; i++) {
            tables = tables + li[i].attributes.getNamedItem("nom").value + ":" + li[i].attributes.getNamedItem("stable_id").value + ";";
        }
        var url = window.location.href + '/Home/GetCountNewItems';
        url = addToUrl(url, 'tables', tables);
        url = addToUrl(url, 'r', Math.random());

        xhr.open('GET', url , true);
        xhr.send(null);

        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4) {
                if (xhr.status == 200 && xhr.responseText != '') {
                    var str_arr = xhr.responseText.split(';');
                    for (var j = 0; j < str_arr.length-1; j++) {
                        var str = str_arr[j].split(':');
                        var nom = str[0];
                        var count = str[1];

                        var a = $("li[need_new_items_count='True'][nom='" + nom + "'] > a");
                        var oldb = $("li[need_new_items_count='True'][nom='" + nom + "'] > a > b");
                        if (a.length > 0 && (oldb.length == 0 || oldb.innerText != '+' + xhr.responseText) && count != '0') {
                            var newb = document.createElement('b');
                            newb.setAttribute('class', 'count-new-items is-red-colored is-align-left');
                            newb.textContent = '+' + count;//xhr.responseText;

                            if (oldb.length == 0)
                                a.append(newb);
                            else
                                oldb.text('+' + count);//xhr.responseText);
                        }
                    }
                }
            }
        }
    }
};
function onShowMainMenu() {
    if (window.hold_mm == 1)
        return;
    setTimeout(function() {
        $('#left-pane').removeClass("is-not-visible");
    }, 200);
}
function onHideMainMenu() {
    if (window.hold_mm == 1)
        return;
    setTimeout(function() {
        $('#left-pane').addClass("is-not-visible");
    }, 500);
}


function getXmlHttp() {
    var xmlhttp;
    try {
        xmlhttp = new ActiveXObject("Msxml2.XMLHTTP");
    } catch (e) {
        try {
            xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
        } catch (E) {
            xmlhttp = false;
        }
    }
    if (!xmlhttp && typeof XMLHttpRequest != 'undefined') {
        xmlhttp = new XMLHttpRequest();
    }
    return xmlhttp;
}

function onOpen(e) {
    $("#inboxActions").data("kendoMobileActionSheet").open();
}

// **********************************************************************************
function mobile_main_menu_read(e) {

    var Id = e.view.params.parent;
    if (Id == 0)
        return false;
    var backButton = $('#employee_back');
    var navBar = $('#employee_navbar').data('kendoMobileNavBar');
    var listView = $('#main_menu_list_view');
    

    if (Id != undefined) {
        if (listView.length > 0) {
            var data = listView.data('kendoMobileListView').dataSource;
            data.fetch(function () {
                var item = data.get(Id);
                if (item) {
                    backButton.show();
                    listView.data('kendoMobileListView').setDataSource(item.MobileMainMenuItemChilds);
                } else {
                    setTimeout(function() {
                        kendo.mobile.application.navigate("#:back");
                    }, 0);
                }
            });
        }
    }
    else {
        if (listView.length > 0) {
            if (window.MainMenu_dataSource != undefined) {
                listView.data('kendoMobileListView').setDataSource(window.MainMenu_dataSource);
            } else {
                window.MainMenu_dataSource = listView.data('kendoMobileListView').dataSource;
            }
        }
    }

    var menuBlock = $('#menu_block_vertical');
    var menuHeader = $('#mobile_view_header');
    if (menuBlock)
        menuBlock.css("height", listView[0].clientHeight + menuHeader[0].clientHeight);
}

//(function () {
//    if (Top().window.CustomEvent.length != undefined)
//        return false;
//    if (typeof Top().window.CustomEvent === "function")
//        return false; //If not IE

//    function CustomEvent(event, params) {
//        params = params || { bubbles: false, cancelable: false, detail: undefined };
//        var evt = document.createEvent('CustomEvent');
//        evt.initCustomEvent(event, params.bubbles, params.cancelable, params.detail);
//        return evt;
//    }

//    CustomEvent.prototype = Top().window.Event.prototype;

//    Top().window.CustomEvent = CustomEvent;
//})();