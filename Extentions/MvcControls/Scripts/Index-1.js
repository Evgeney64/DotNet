function Index_Ready(uri_Content, mainTab_IsAutoLoaded, mainTab_MenuLoad, mainTab_Text, uriPathStart, soperation_id, guids_out) {

    var options = {};
    options.url = uri_Content + "Home/IndexMainMenu";
    options.type = "GET";

    options.success = function (result) {
        
        var html = result.substr(15, result.length - 15).toString();

        var el = $('<div></div>');
        el.html(html);

        var left_pane = $('#left-pane', el);
        $('body').append(left_pane);

        IndexMainMenu_Ready(uri_Content, mainTab_IsAutoLoaded, mainTab_MenuLoad, mainTab_Text, uriPathStart, soperation_id, guids_out);
    };

    $.ajax(options);
}

function IndexMainMenu_Ready(uri_Content, mainTab_IsAutoLoaded, mainTab_MenuLoad, mainTab_Text, uriPathStart, soperation_id, guids_out) {

    var tabstrip = $("#tabstrip");
    var guid_out = null;
    if (guids_out != null && guids_out.split('=').length > 1)
        guid_out = guids_out.split('=')[1];

    if (mainTab_IsAutoLoaded == "True") {
        //var url = uri_Content + "Home/Main_Layout?sconfig_id=2049&stable_id=1&soperation_id=14&nom=0&uri_path=110_2049_0_1_14_0%3B";
        var url = null;
        if (soperation_id == sop_14)
            url = uri_Content + "Home/Main_Layout";
        if (soperation_id == sop_2 || soperation_id == sop_AddWizard)
            url = uri_Content + "Home/TabEdit_Layout";
        if (soperation_id == sop_27)
            url = uri_Content + "Home/DashBord";

        if (url != null) {
            url = updateQueryStringParameter(url, "uri_path", uriPathStart);
            url = updateQueryStringParameter(url, "maintab_isautoloaded", mainTab_IsAutoLoaded);

            mainTab_Open($("#tabstrip"), mainTab_Text, url, guid_out, guids_out);

            //$("#left-pane").data("kendoResponsivePanel").close();

            if (mainTab_MenuLoad == "False")
                return;
        }
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
        autoClose: false,
        //close: onClose
        //open: onOpen
    });
    // реагируем на свайпы
    $("#left-pane").kendoTouch({
        enableSwipe: true,
        swipe: function (e) {
            if (e.direction === "left") {
                $("#left-pane").data("kendoResponsivePanel").close();
            }
        }
    });
    // открываем меню с задержкой
    if (mainTab_IsAutoLoaded != "True") {
        setTimeout(function () {
            $("#left-pane").data("kendoResponsivePanel").open();
        }, 500);
    }

    // по любой ссылке в левом меню открываем таб с этой ссылкой
    hookLinks("#MainMenu", function (url, sender) {
        $("#left-pane").data("kendoResponsivePanel").close();

        /*При переходе к соответствующему списку, убираем значок новых элементов*/
        if (sender.parentElement.attributes["new_items_pic_type"] != null)
            if (sender.parentElement.attributes["need_new_items_pic"] != null > 0 && sender.parentElement.attributes["need_new_items_pic"].value == 'False') {
                $("li[new_items_pic_type='" + sender.parentElement.attributes["new_items_pic_type"].value + "']").attr('need_new_items_pic', 'True');
                $("li[new_items_pic_type='" + sender.parentElement.attributes["new_items_pic_type"].value + "'] > a > img.k-image-new-items").remove();
                $("li[new_items_pic_type='" + sender.parentElement.attributes["new_items_pic_type"].value + "'] > span > img.k-image-new-items").remove();
            }

        $("div.k-animation-container").css("display", "none");
        if (url.indexOf("modal") > 0 && !isMobile()) {
            modalWindow($(sender).text(), url);
        } else {
            /*Убираем из текста нажатого меню количество непрочитанных сообщений, чтобы оно не попало в заголовок*/
            if ($(sender).find("b.k-count-new-items").length > 0)
                $(sender).find("b.k-count-new-items").remove();
            var title = $(sender).text();
            mainTab_Open($("#tabstrip"), title, url, guid_out, guids_out);
        }

        return false;
    });
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
    hookLinks("#deal_content", function (url, sender) {
        OpenLink(url, sender, guids_out);
    });

    $(window).on("resize", function () {
        // Pavlov 06.02.2017
        // вся логика которая была здесь вынесена в отдельную функцию, которую можно вызывать без вызова метода $(window).resize()
        leftPaneWindowResize();
    });

    $(window).trigger('resize');

    // Pavlov 06.02.2017
    // обработчик click'ов любого места в документе
    $(document).click(function (e) {
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

}
//);

// Pavlov 06.02.2017
// это событие срабатывает при загрузке каждого iframe, делегирует заданный перечень событий главному документу
function onIframe_Load(ifr) {
    var eventlist = 'click dblclick \
                    keydown keypress keyup \
                    mousedown mouseup \
                    touchstart touchend touchcancel touchleave touchmove';

    var ifrHtml = $(ifr).contents().find('html');
    ifrHtml.on(eventlist, function (event) {
        $(document).trigger(event);
    });
};

// Pavlov 06.02.2017
// функция скрывает панель с главным меню
function hideMainMenu() {
    $('#left-pane').data('kendoResponsivePanel').close();
};

// Pavlov 06.02.2017
// функция переформатирует левую панель с главным меню и блоком информации по сделке в ЛИК
function leftPaneWindowResize() {
    if (window.innerWidth < 10000)
        $("#tabstrip > div.k-content").css("left", "0px");
    else
        $("#tabstrip > div.k-content").css("left", "345px");

    var height = window.innerHeight;
    var height_menu = 0;
    var height_dealinfo = 0;
    var height_deal_content = 0;
    if ($("#menu_block").length > 0)
        height_menu = $("#menu_block")[0].clientHeight;
    if ($("#dealinfo_block").length > 0) {
        if ($("#dealinfo_block div.tab_edit_content").length > 0)
            $("#dealinfo_block").css("height", $("#dealinfo_block div.tab_edit_content")[0].clientHeight)
        height_dealinfo = $("#dealinfo_block")[0].clientHeight;
    }
    if ($("#deal_content").length > 0)
        height_deal_content = $("#deal_content")[0].clientHeight;

    $("#empty_block").css("height", (height - height_menu - height_dealinfo - 55));
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
                            newb.setAttribute('class', 'k-count-new-items');
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