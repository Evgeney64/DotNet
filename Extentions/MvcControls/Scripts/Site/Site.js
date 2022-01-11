kendo.culture("ru-RU");

var sop_0 = 0;      // Не определено
var sop_1 = 1;      // Просмотр
var sop_61 = 61;    // Просмотр-навигация
var sop_2 = 2;      // Добавить
var sop_3 = 3;      // Корректировать
var sop_4 = 4;      // Удалить
var sop_5 = 5;      // Открыть список
var sop_6 = 6;      // Фильтр
var sop_Save = 7;      //
var sop_8 = 8;      // Выполнить

var sop_12 = 12;    // Добавление объекта к родительскому
var sop_13 = 13;    // Добавление дочернего объекта
var sop_14 = 14;    // MainList_From_MainMenu
var sop_15 = 15;    // Перенести одну строку в Главный список
var sop_16 = 16;    // Перенести все строки в Главный список

var sop_19 = 19;    // Открыть список (дерево)
var sop_191 = 191;    // Открыть главный список (дерево)
var sop_192 = 192;    // Открыть главный список (дерево) без фильтра
var sop_193 = 193;    // 

var sop_23 = 23;    // Открыть окно независимого списка
var sop_25 = 25;    // Добавить с предварительным диалогом
var sop_26 = 26;    // Добавление дочернего объекта по схеме
var sop_27 = 27;    // DashBord
var sop_29 = 29;    // Добавить (визард)
var sop_35 = 35;    // Добавление дочернего объекта (визард)
var sop_38 = 38;    // Добавить из селектора
var sop_41 = 41;    // Открыть строку-список из ячейки грида
var sop_44 = 44;    // Добавить связь между объектами
var sop_49 = 49;    // Добавление связей содержимого документов (3 окна)
var sop_54 = 54;    // Пересоздавать зависимый список

var sop_SelectorFromFilter = 601;
var sop_SelectorFromEditor = 602;
var sop_SelectorFromPeriodValues = 603;

var sop_AddWizard = 29;         // Добавить (визард)
var sop_EditWizard = 30;        // Корректировать (визард)
var sop_Report = 200;           // Вызов отчета
var sop_ReportDirect = 201;     // Вызов отчета без диалога
var sop_ReportExport = 235;     // Экспорт отчета
var sop_296 = 296;              // PointMeterShowValue_Import
//-----------------------------------------------------------------------------------------

var auto_test_delay = 2000;
var auto_test_delay5 = 5000;
var auto_test_delay10 = 10000;

var tabstrips_height = "";
var hold_mm = 0;
var uri_Action = null;

var isParentWindow = true;

// Is methods *******************************************************************************************

// Опреации для которых требуется наличие текущей строки
function isOperationsForRow(soperation_id) {
    if (soperation_id == sop_3 || soperation_id == sop_13
        || soperation_id == sop_4
        || soperation_id == sop_5
        || soperation_id == sop_8
        || soperation_id == sop_15
        || soperation_id == sop_16
        || soperation_id == sop_19
        || soperation_id == sop_26
        || soperation_id == sop_EditWizard
        || soperation_id == sop_35
        || soperation_id == sop_Report
        || soperation_id == sop_ReportExport
        || soperation_id == sop_296
    )
        return true;
    else
        return false;
}

// Группа операций добавления для сходных действий на клиенте
function isOperationsAdd(soperation_id) {
    if (soperation_id == sop_2
        || soperation_id == sop_AddWizard
        || soperation_id == sop_12
        || soperation_id == sop_13
        || soperation_id == sop_26
    )
        return true;
    else
        return false;
}

function isMobile() {
    /*Работа 7346. Верстка под мобильные устройства пока отключена*/
    return false;

    try {
        if (Top()) {
            return $(Top()).width() < 769;
        }
    } catch (err) {
        return $(window).width() < 769;
    }
}

function isNumeric(n) {
    return !isNaN(parseFloat(n)) && isFinite(n);
}

function isLik(uri_path_out) {
    var uri_path_110 = getUriPath_FromStr_ByType(uri_path_out, 110, ';');
    if (uri_path_110 != null && uri_path_110.IsLIK == 1)
        return true;
    return false;
}

function isAutoTest(uri_path_out) {
    var uri_path_110 = getUriPath_FromStr_ByType(uri_path_out, 110, ';');
    if (uri_path_110 != null && uri_path_110.IsAutoTest == 1)
        return true;
    return false;
}

function isPageGuidEmpty(guid) {
    if (guid == "00000000-0000-0000-0000-000000000000" || guid == undefined)
        return true;
    else
        return false;
}

// *******************************************************************************************
function setTitle() {
    $("#title").text($("#tabstrip").data("kendoTabStrip").select().text());

    moveTabstrip();
}

/*Двигаем содержимое вкладок правее, если главное меню остается*/
function moveTabstrip() {
    if (window.innerWidth >= 10000) {
        if ($("#tabstrip > div.k-content").length > 0)
            $("#tabstrip > div.k-content").css("left", "245px");
        //if ($("#tabstrip > ul").length > 0)
        //    $("#tabstrip > ul").css("left", "400px");
    }
}

// Converters *******************************************************************************************
function updateQueryStringParameter(uri, key, value) {
    if (value || value === "") {
        var re = new RegExp("([?&])" + key + "=.*?(&|$)", "i");
        var separator = uri.indexOf('?') !== -1 ? "&" : "?";

        if (uri.match(re)) {
            var re_par = '$1' + key + "=" + encodeURIComponent(value) + '$2';
            var uri2 = uri.replace(re, '$1' + key + "=" + encodeURIComponent(value) + '$2');
            return uri2;
        }
        else {
            return uri + separator + key + "=" + encodeURIComponent(value);
        }
    } else {
        if (key != undefined) {
            var result = uri;
            $.each(key.split("&"), function () {
                var pair = this.split("=");
                if (pair.length > 1) {
                    result = updateQueryStringParameter(result, pair[0], pair[1]);
                } else {
                    result = updateQueryStringParameter(result, pair[0], "");
                }
            });
            return result;
        }
    }
}

function addToUrl(url, key, val) {
    if (url.indexOf(key) >= 0) {
        return updateQueryStringParameter(url, key, val);
    }

    if (val) {
        url += (url.indexOf('?') > -1 ? "&" : "?") + key + "=" + encodeURIComponent(val);
    } else {
        if (key != undefined) {
            $.each(key.split("&"), function () {
                var pair = this.split("=");
                url += (url.indexOf('?') > -1 ? "&" : "?") + pair[0] + "=" + pair[1];
            });
        }
    }
    return url;
}

// select Item *******************************************************************************************
function findInParent(selector, windowHandler) {
    var win = windowHandler ? windowHandler : window;
    var jqObj = win.$(selector);
    if (jqObj.length > 0) {
        return jqObj;
    }
    return win.parent === win ? jqObj : findInParent(selector, win.parent);
}

var tree_node_selected;
function selectTreeNode(sender, page_guid) {
    var tree = $("#FilterTree_" + page_guid);

    //tree_node_selected = tree.find("li[aria-selected=true]");
    if (tree.data("kendoTreeView"))
        tree_node_selected = tree.data("kendoTreeView").select();
}

function getUrlVars(url) {
    var vars = [], hash;
    var hashes = url.slice(url.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}

// Строка типа 20.04.2018 - 30.05.2018
function DateBeg_DateEnd(date_beg, date_end) {
    var str = ""
    var yy0 = date_beg.getFullYear(), mm0 = date_beg.getMonth() + 1, dd0 = date_beg.getDate();
    var date_beg_str = (100 + dd0).toString().substr(1, 2) + "." + (100 + mm0).toString().substr(1, 2) + "." + yy0;
    str = date_beg_str + " - ";

    if (date_end != null) {
        var yy1 = date_end.getFullYear(), mm1 = date_end.getMonth() + 1, dd1 = date_end.getDate();
        var date_end_str = (100 + dd1).toString().substr(1, 2) + "." + (100 + mm1).toString().substr(1, 2) + "." + yy1;
        str += date_end_str;
    }
    return str;
}

function getSoperation_ByUrl(url) {
    //messageWindow("url", "", url);
    var url_paths = url.split('?');
    var urls = url_paths[1].split('&');
    for (var i = 0; i < urls.length; i++) {
        var values = urls[i].split('=');
        if (values[0] == "soperation_id")
            return values[1];
    }
    return null;
}

function getParameterByName(name) {
    var match = RegExp('[?&]' + name + '=([^&]*)').exec(window.location.search);
    return match && decodeURIComponent(match[1].replace(/\+/g, ' '));
}

function linksChangeColor(page_guid) {
    var jQgrid = $("#grid_" + page_guid);

    if (jQgrid.find("tr.k-state-selected a#gridCellLink").length > 0) {
        var color = getComputedStyle(jQgrid.find("tr.k-state-selected")[0], "")["color"];
        var curr_color = "rgb(0, 102, 204)";//jQgrid.find("tr.k-state-selected a#gridCellLink")[0].style.color;
        jQgrid.find("tr.k-state-selected a#gridCellLink")[0].style.color = color;
        jQgrid.find("tr[aria-selected='false'] a#gridCellLink").each(function () {
            $(this)[0].style.color = curr_color;
        });
    }
}

// Guid *******************************************************************************************
function getGuidStr_FromGuidsOut(guids_out, num) {
    var guid_str_out = '';
    var count = guids_out.split(';').length;
    for (var i = 0; i < count; i++) {
        var guid_str = guids_out.split(';')[i];
        switch (num) {
            case 'first': {
                if (i == 0 /*&& guid_str.split('=').length > 1*/)
                    guid_str_out = guid_str;//guid_str.split('=')[1];
                break;
            }
            case 'last': {
                if (i == count - 1 /*&& guid_str.split('=').length > 1*/)
                    guid_str_out = guid_str;//guid_str.split('=')[1];
                break;
            }
            case 'last-1': {
                if (i == count - 2 /*&& guid_str.split('=').length > 1*/)
                    guid_str_out = guid_str;//guid_str.split('=')[1];
                break;
            }
        }
    }

    //var guids_str = guid_str_out.split('=');
    //if (guids_str.length == 2)
    //    guid_str_out = guids_str[1];
    var guid = guid_str_out.split('=')[1];
    var name = guid_str_out.split('=')[0].split('_')[0];

    return name + '_' + guid;
}

function getGuidStr_FromGuidsOut_ByName(guids_out, name) {
    var guid_str_out = '';
    var guids_str = guids_out.split(';');
    var count = guids_str.length;
    for (var i = count - 1; i >= 0; i--) {
        var guid_str_part_value = guids_str[i].split('=');
        var guid_str_parts = guid_str_part_value[0].split('_');
        if (guid_str_parts[0] == name) {
            var guids_str_out = guids_str[i].split('=');
            return guids_str_out[1];
        }
    }
    return "";
}

function setGuidsStr(guids_in, guid) {
    var guids_str = guids_in.split(';');
    var count = guids_str.length;
    for (var i = count - 1; i >= 0; i--) {
        var guid_str_part_name = guids_str[i].split('=')[0];
        var guid_str_part_value = guids_str[i].split('=')[1];
        if (guid_str_part_name == guid.split('=')[0]) {
            guids_str[i] = guid;
            return guids_str.toString().replace(/,/g, ";");
        }
    }
    guids_str[count] = guid;
    return guids_str.toString().replace(/,/g, ";");
}

// UriPath *******************************************************************************************
function getUriParam_ByName(uri, name) {
    var uris = uri.split('?');
    if (uris.length < 2)
        return null;

    var uri_paths = uris[1].split('&');
    if (uri_paths.length == 0)
        return null;

    var count = uri_paths.length;
    for (var i = count - 1; i >= 0; i--) {
        var uri_params = uri_paths[i].split('=');
        if (uri_params.length == 2 && uri_params[0] == name) {
            return uri_params[1];
        }
    }
    return null;
}

function getUriPath_ByType(url, type) {
    var uri_paths = getUriParam_ByName(url, "uri_path");
    if (uri_paths != null) {
        if (uri_paths.indexOf('%3B') >= 0)
            return getUriPath_FromStr_ByType(uri_paths, type, '%3B');
        if (uri_paths.indexOf(';') >= 0)
            return getUriPath_FromStr_ByType(uri_paths, type, ';');
    }
    return null;
}

function getUriPath_FromStr_First(uri_path_out) {
    var uri_paths = uri_path_out.split(';');
    if (uri_paths.length == 0)
        return null;

    var uri_path_first = uri_paths[0];
    return getUriPath_Init(uri_path_last);
}

function getUriPath_FromStr_Last(uri_path_out) {
    var uri_paths = uri_path_out.split(';');
    if (uri_paths.length == 0)
        return null;

    var uri_path_last = uri_paths[uri_paths.length - 1];
    if (uri_path_last == "" && uri_paths.length >= 2)
        uri_path_last = uri_paths[uri_paths.length - 2];

    return getUriPath_Init(uri_path_last);
}

function getUriPath_FromStr_ByType(uri_path_out, type, splitter) {
    if (splitter == undefined)
        splitter = ';';

    var uri_paths = uri_path_out.split(splitter);
    if (uri_paths.length == 0)
        return null;

    var count = uri_paths.length;
    for (var i = count - 1; i >= 0; i--) {
        var uri_params = uri_paths[i].split('_');
        if (uri_params[0] == type) {
            return getUriPath_Init(uri_paths[i]);
        }
    }
    return null;
}

function getUriPath_Soperation(uri_path_out, type, splitter) {
    var soperation_id = 0;
    var uri_path_110 = getUriPath_FromStr_ByType(uri_path_out, type, splitter);
    if (uri_path_110 != null)
        soperation_id = uri_path_110.SoperationId;
    return soperation_id;
}

function getUriPath_Init(uri_path_out) {
    var _uri_paths = uri_path_out.split('_');
    var type = _uri_paths[0];

    var uri_path = {
        SysConfigType: [],
        SconfigId: [],
        SectionId: [],
        PageId: [],
        Nom: [],

        StableId: [],
        TargetStableId: [],
        TargetViewId: [],

        SoperationId: [],
        SoperationId1: [],
        ControlType: [],

        IsLIK: [],
        IsAutoTest: [],
        AutoTest_Nom0: [],
        AutoTest_Nom1: [],
    };

    uri_path.SysConfigType = _uri_paths[0];
    uri_path.SconfigId = _uri_paths[1];
    uri_path.SectionId = _uri_paths[2];
    uri_path.Nom = _uri_paths[3];

    if (type == 110) {
        uri_path.StableId = _uri_paths[4];
        uri_path.TargetStableId = _uri_paths[4];
        uri_path.TargetViewId = _uri_paths[5];

        uri_path.SoperationId = _uri_paths[6];
        uri_path.SoperationId1 = _uri_paths[8];

        uri_path.IsLIK = _uri_paths[9];
        uri_path.IsAutoTest = _uri_paths[10];
        uri_path.AutoTest_Nom0 = _uri_paths[11];
        uri_path.AutoTest_Nom1 = _uri_paths[12];
    }
    else if (type == 111) {
        uri_path.StableId = _uri_paths[4];
        uri_path.TargetStableId = _uri_paths[5];
        uri_path.TargetViewId = _uri_paths[6];

        uri_path.SoperationId = _uri_paths[9];
        uri_path.SoperationId1 = _uri_paths[11];
    }
    else if (type == 101 || type == 108) {
        uri_path.StableId = _uri_paths[5];
        uri_path.TargetStableId = _uri_paths[6];
        uri_path.TargetViewId = _uri_paths[7];

        uri_path.SoperationId = _uri_paths[8];
        uri_path.ControlType = _uri_paths[9];
    }
    else if (type == 102 || type == 120) {
        uri_path.PageId = _uri_paths[4];
        uri_path.StableId = _uri_paths[5];
        uri_path.TargetStableId = _uri_paths[6];
        uri_path.TargetViewId = _uri_paths[7];

        uri_path.SoperationId = _uri_paths[9];
        uri_path.SoperationId1 = _uri_paths[10];
        uri_path.ControlType = _uri_paths[11];
    }
    return uri_path;
}

// *******************************************************************************************
function getRandomValue() {
    var min = 1;
    var max = 100000;
    var rand = Math.floor(Math.random() * (max - min)) + min;
    return 100000 + rand;
}

//Меняются иконки стрелочек. После обновления скриптов они почему-то перевернулись
function change_arrows() {
    let arrows = $('.k-combobox .k-select>.k-icon.k-i-arrow-60-down, .k-dropdown-wrap .k-select>.k-icon.k-i-arrow-60-down');
    for (let i = 0; i < arrows.length; i++) {
        $(arrows[i]).removeClass('k-i-arrow-60-down');
        $(arrows[i]).addClass('k-i-arrow-s');
    }


    $('.k-icon.k-i-calendar').click(function () {
        setTimeout(function () {
            arrows = $('.k-calendar .k-nav-prev>.k-icon.k-i-arrow-60-left');
            for (let i = 0; i < arrows.length; i++) {
                $(arrows[i]).removeClass('k-i-arrow-60-down');
                $(arrows[i]).addClass('k-i-arrow-s');
            }
        }, 100);
    });
}

// Pavlov 06.02.2017
// функция скрывает панель с главным меню
function hideMainMenu() {
    if (window.hold_mm == 0) {
        if ($('#left-pane').data('kendoResponsivePanel') != undefined)
            $('#left-pane').data('kendoResponsivePanel').close();
        $('#left-pane').addClass("is-not-visible");
    }
    else {
        $('#left-pane').addClass("k-rpanel-animate");
        $('#left-pane').addClass("k-rpanel-expanded");
        $('#left-pane').removeClass("is-not-visible");
    }

    if ($("#main_header").length > 0 && $("#main_header").data("kendoResponsivePanel") != undefined)
        $("#main_header").data("kendoResponsivePanel").open();
}

function holdMainMenu(url_content) {
    if ($('#hold_mm_span').attr("isMobile") == undefined) {
        if (window.hold_mm == 0) {
            $("#hold_mm_pic").attr("src", /*window.location.href*/ url_content + "Images/Operations/Small/Fix_On.png")
            window.hold_mm = 1;
            $("#left-pane").removeClass("k-rpanel-left");
            $("#left-pane").addClass("is-menu-relative");
            $("#left-pane").removeClass("is-menu-absolute");
        }
        else {
            $("#hold_mm_pic").attr("src", /*window.location.href*/ url_content + "Images/Operations/Small/Fix_Off.png")
            window.hold_mm = 0;
            $("#left-pane").addClass("k-rpanel-left");
            $("#left-pane").addClass("is-menu-absolute");
            $("#left-pane").removeClass("is-menu-relative");
        }
        if ($("#tabstrip").length > 0)
            leftPaneWindowResize();

        var options = {};
        options.url = url_content + "Home/TabEditSave";
        options.type = "POST";

        options.data = {};
        options.data.save_operation = "SaveHoldMainMenu";
        options.data.hold_main_menu = window.hold_mm;
        $.ajax(options);
    }
}

//Функция возвращает высоту предыдущего tabstrip'а, либо текущего, если такая есть
function getTabStripHeight(tabstrip_id) {
    var last_height;
    var name = tabstrip_id.split("_")[0];

    if (Top().tabstrips_height != undefined) {
        try {
            var arr = Top().tabstrips_height.split(",");
        }
        catch (e) {
            try {
                var arr = parent.tabstrips_height.split(",");
            }
            catch (e) {
                var arr = tabstrips_height.split(",");
            }
        }
        for (var i = 0; i < arr.length; i++) {
            //if (name == 'tabstrip' || name == 'FilterTab' ||)
            if (arr[i].split(":")[0].trim() == name)
                last_height = arr[i].split(":")[1];
        }
        //if (last_height == undefined)
            //last_height = arr[arr.length - 1].split(":")[1];
    }

    return last_height;
}

//Функция сохраняет высоту текущего tabstrip'а в массив
function setTabStripHeight(tabstrip_id, height) {
    var name = tabstrip_id.split("_")[0];
    if (height != undefined && !isNaN(height)) {
        try {
            if (Top().tabstrips_height.indexOf(name + ":") != -1)
                return;

            if (Top().tabstrips_height.length == 0)
                Top().tabstrips_height = name + ":" + height;
            else
                Top().tabstrips_height = Top().tabstrips_height + ", " + name + ":" + height;
        }
        catch (e) {
            try {
                if (parent.tabstrips_height.indexOf(name) != -1)
                    return;

                if (parent.tabstrips_height.length == 0)
                    parent.tabstrips_height = name + ":" + height;
                else
                    parent.tabstrips_height = parent.tabstrips_height + ", " + name + ":" + height;
            }
            catch (e) {
                if (tabstrips_height.indexOf(name) != -1)
                    return;

                if (tabstrips_height.length == 0)
                    tabstrips_height = name + ":" + height;
                else
                    tabstrips_height = tabstrips_height + ", " + name + ":" + height;
            }
        }
    }
}

function getActionName_BySoperation(uri_Content, soperation_id) {
    var url = uri_Content + "Home/Main_Layout";

    if (soperation_id == sop_2 || soperation_id == sop_12 || soperation_id == sop_3 || soperation_id == sop_AddWizard) {
        url = uri_Content + "Home/TabEdit_Layout";
        //url = uri_Content + "Home/TabEdit";
    }
    if (soperation_id == sop_27)
        url = uri_Content + "Home/DashBord";

    return url;
}

//Функция возвращает самый верхний window
function Top() {
    function _top(wnd) {
        //Поиск родителя уровнем выше, для случаев, когда сайт в iframe. Выдает ошибку, если уровень выше и есть главный iframe. 
        //Работает везде, кроме Мазилы.
        try {
            if (wnd.parent.Error().message != "" || wnd.parent.isParentWindow === undefined)
               return wnd;
        }
        catch (e) {
            return wnd;
        }

        //Тоже самое, что и блок выше. Работает только в Мазиле.
        if (wnd == wnd.parent) {
            return wnd;
        }

        if (wnd.name.indexOf("|") > 0) {
            return wnd;
        }
        if (wnd.name.indexOf("bitrix24") > 0) {
            return wnd;
        }
        try {
            return _top(wnd.parent);
        } catch (e) {
            return wnd;
        }
    }

    return _top(window);
}

function button_SetReadOnly(sender, disable) {
    if (disable) {
        $(sender).addClass("k-state-disabled");
        if ($(sender).attr("click_func") == undefined)
            $(sender).attr("click_func", $(sender).attr("onclick"));
        $(sender).attr("onclick", "return false");
    }
    else {
        $(sender).removeClass("k-state-disabled");
        if ($(sender).attr("click_func") != undefined)
            $(sender).attr("onclick", $(sender).attr("click_func"));
        $(sender).removeAttr("click_func");
    }
}

function is_true(val) {
    if (val === undefined || val === null || val === "")
        return false;
    if (typeof(val) === "boolean")
        return val;
    if (val === 0)
        return false;
    if (val === 1)
        return true;
    if (val.toLowerCase() === "false")
        return false;
    if (val.toLowerCase() === "true")
        return true;
    return false;
}
// **********************************************************************************
function view_open(view_name, left, right) {
    if (left)
        kendo.mobile.application.navigate("#" + view_name, 'slide:left');
    if (right)
        kendo.mobile.application.navigate("#" + view_name, 'slide:right');
    if (left == undefined && right == undefined)
        kendo.mobile.application.navigate("#" + view_name)

    if ($("#" + view_name).length > 0) {
        $("div[data-role='view']").attr("current", "false");
        $("#" + view_name).attr("current", "true");
        if ($("#" + view_name).attr("title") != undefined)
            $("#" + view_name).find("#view_title")[0].textContent = "->  " + $("#" + view_name).attr("title");
    }
}

function view_change(left, right) {
    var mobile_app = $("#mobile_app");
    if (mobile_app.length == 0)
        mobile_app = Top().$("#mobile_app");
    var current_view = mobile_app.find("div[data-role='view'][current='true']");
    var current_num = parseInt(current_view.attr("num"), 10);
    var view_count = mobile_app.find("div[data-role='view']").length;

    if (current_view.length > 0) {
        var view_name = "mobile_view";
        if (right) {
            if (current_num - 1 != 0)
                view_name += (current_num - 1);
            else
                view_name += view_count;
        }
        if (left) {
            if (current_num + 1 <= view_count)
                view_name += (current_num + 1);
            else
                view_name += 1;
        }

        view_open(view_name, left, right);
    }
}

function create_new_mobile_view(title, view_parent_name, new_view_num) {
    var new_view_name = undefined;
    var mobile_app = $("#mobile_app");
    if (mobile_app.length == 0)
        mobile_app = Top().$("#mobile_app");
    if (mobile_app.length > 0) {
        var view_count = mobile_app.find("div[data-role='view']").length;
        new_view_num = parseInt(mobile_app.find("div[data-role='view']")[view_count - 1].getAttribute("num")) + 1;
        if (new_view_num == undefined)
            new_view_num = parseInt(view_count) + 1;
        new_view_name = "mobile_view" + new_view_num;
        var new_view_html = "<div id=\"" + new_view_name + "\" num=\"" + new_view_num + "\" data-role=\"view\" style=\"display: none;\" data-transition=\"slide\" data-layout=\"mobile_layout\" title=\"" + title + "\" view_parent=\"" + view_parent_name + "\">";
        mobile_app.append(new_view_html + "</div>");

        if (new_view_num != 3) {
            mobile_app.find("#inboxActions > li.km-actionsheet-cancel").remove();
            mobile_app.find("#inboxActions").append("<li id=\"inboxActions" + new_view_num + "\"><span class=\"k-link\" onclick=\"view_open('" + new_view_name + "');\">" + title + "</span>" +
                "<span class=\"k-link ctrl-mobile-button ctrl-mobile-button-cancel\" onclick=\"remove_mobile_view('" + new_view_num + "');\">" +
                "<img class=\"k-image\" alt=\"image\" src=\"Images/Operations/Small/Close_.png\">" +
            "</span></li>");
            mobile_app.find("#inboxActions").append("<li class=\"km-actionsheet-cancel\"><a href=\"#\">Cancel</a></li>");
        }
    }
    return new_view_name;
}

function remove_mobile_view(view_id) {
    var mobile_app = $("#mobile_app");
    if (mobile_app.length == 0)
        mobile_app = Top().$("#mobile_app");
    if (mobile_app.length > 0) {
        mobile_app.find("#mobile_view" + view_id).remove();
        if (view_id != 3) {
            mobile_app.find("li#inboxActions" + view_id).remove();
            view_open('mobile_view1');
        }
    }
}

// **********************************************************************************
function export_html_to_pdf() {
    kendo.drawing.drawDOM(".content-export-to-pdf", {
        forcePageBreak: ".report-content-div",
        paperSize: "auto",
        margin: "1cm",
        multiPage: true
    }).then(function (group) {
        kendo.drawing.pdf.saveAs(group, "Report.pdf");
    });
}

function createFileEditor(editor_id) {
    $("#" + editor_id).kendoEditor({
        tools: [
            "insertImage",
            "insertFile"
        ],
        fileBrowser: {
            messages: {
                dropFilesHere: "Drop files here"
            },
            transport: {
                read: "/Home/FileRead",
                destroy: {
                    url: "/Home/FileDestroy",
                    type: "POST"
                },
                create: {
                    url: "/Home/FileCreate",
                    type: "POST"
                },
                uploadUrl: "/Home/FileUpload"//,
                //fileUrl: "/Home/File?fileName={0}"
            }
        }
    });
}

// **********************************************************************************
/*Обработчки кликов. Вешается на каждый фрейм. При клике убирает гл. меню*/
function onIframe_Load(ifr) {
    var ifrHtml = $(ifr).contents().find('html');
    ifrHtml.on("click", function (event) {
        Top().hideMainMenu();
    });
}

/*Убираем все обработки фрейма, скрывающие гл. меню, иначе будет утечка памяти*/
function onIframe_Event_Remove(ifr) {
    if (ifr.length > 0) {
        ifr.off("load");
        ifr[0].removeAttribute("onload");
        var ifrHtml = ifr.contents().find('html');
        ifrHtml.off("click");
        ifrHtml[0].removeAttribute("onload");
    }
}

// **********************************************************************************
function get_Browser_Type() {
    if (!!window.opera || navigator.userAgent.indexOf(' OPR/') >= 0) return "opera";
    if (!!window.chrome) return "chrome";
    if (typeof document !== 'undefined' && !!document.documentMode) return "explorer";
    if (typeof window.InstallTrigger !== 'undefined') return "firefox";
    if (/^((?!chrome|android).)*safari/i.test(navigator.userAgent)) return "safari";
    return "none";
}

// **********************************************************************************
/*Создание DropDownTree 2 варианта*/
function dropDownTreeCreate() {
    var dropdown = $("#dropdown").kendoDropDownList({
        dataSource: [{ text: "", value: "" }],
        dataTextField: "text",
        dataValueField: "value",
        open: function (e) {
            // If the treeview is not visible, then make it visible.
            if (!$treeviewRootElem.hasClass("k-custom-visible")) {
                $treeviewRootElem.slideToggle('fast', function () {
                    dropdown.close();
                    $treeviewRootElem.addClass("k-custom-visible");
                });
            }
        }
    }).data("kendoDropDownList");
    var $dropdownRootElem = $(dropdown.element).closest("span.k-dropdown");

    var treeview = $("#treeview").kendoTreeView({
        select: function (e) {
            // When a node is selected, display the text for the node in the dropdown and hide the treeview.
            $dropdownRootElem.find("span.k-input").text($(e.node).children("div").text());
            $treeviewRootElem.slideToggle('fast', function () {
                $treeviewRootElem.removeClass("k-custom-visible");
            });
        }
    }).data("kendoTreeView");
    var $treeviewRootElem = $(treeview.element).closest("div.k-treeview");

    // Hide the treeview.
    $treeviewRootElem
        .width($dropdownRootElem.width())
        .css({ "border": "1px solid grey", "display": "none", "position": "absolute" });

    // Position the treeview so that it is below the dropdown.
    $treeviewRootElem
        .css({ "top": $dropdownRootElem.position().top + $dropdownRootElem.height(), "left": $dropdownRootElem.position().left });

    $(document).click(function (e) {
        // Ignore clicks on the treetriew.
        if ($(e.target).closest("div.k-treeview").length == 0) {
            // If visible, then close the treeview.
            if ($treeviewRootElem.hasClass("k-custom-visible")) {
                $treeviewRootElem.slideToggle('fast', function () {
                    $treeviewRootElem.removeClass("k-custom-visible");
                });
            }
        }
    });
}

function dropDownTreeCreateTelerik(action) {

    homogeneous = new kendo.data.HierarchicalDataSource({
        transport: {
            read: {
                url: action,
                dataType: "jsonp"
            }
        },
        schema: {
            model: {
                id: "Id",
                hasChildren: "hasChildren"
            }
        }
    });

    $("#treeview").kendoDropDownTree({
        placeholder: "Select ...",
        dataSource: homogeneous,
        dataTextField: "NCALC_TYPE_NAME",
        height: "auto"//,
        //dataSource: [
        //    {
        //        text: "Furniture", expanded: true, items: [
        //            { text: "Tables & Chairs" },
        //            { text: "Sofas" },
        //            { text: "Occasional Furniture" }
        //        ]
        //    },
        //    {
        //        text: "Decor", items: [
        //            { text: "Bed Linen" },
        //            { text: "Curtains & Blinds" },
        //            { text: "Carpets" }
        //        ]
        //    }
        //]
    });
}
