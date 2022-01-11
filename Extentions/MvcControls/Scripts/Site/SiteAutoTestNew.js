var AutoTest = {
    items: [],
};
var AutoTestItems = {
    length: 0
};
var AutoTestItem = {
    childAutoTestItems: { length: 0 }
};
var CurrLevelItems = {
    length: 0
}
var CurrLevelItem = {
    level: undefined,
    currNom: undefined,
    title: undefined
}

function set_Curr_Level_Items(level, currNom){
    for (var i=0; i <= CurrLevelItems.length-1; i++)
    {
        if (CurrLevelItems[i].level == level) {
            CurrLevelItems[i].currNom = currNom;
            return;
        }
    }
    CurrLevelItem = {};
    CurrLevelItem.level = level;
    CurrLevelItem.currNom = currNom;
    CurrLevelItems[CurrLevelItems.length] = CurrLevelItem;
    CurrLevelItems.length++;
}
var currLevelItem = function(level) {
    for (var i=0; i <= CurrLevelItems.length-1; i++)
    {
        if (CurrLevelItems[i].level == level)
            return CurrLevelItems[i].currNom;
    }
    return undefined;
}
var currLevel = function () {
    return CurrLevelItems[CurrLevelItems.length - 1].level;
}

function set_Auto_Test_Items(level) {
    let items = [];

    items = get_Auto_Test_Click_Items(level);

    if (items == undefined || items.length == undefined || items.length == 0)
        return;
    if (level == 1)
        AutoTestItems.length = items.length;
    //if (level == 2 || level == 3)
        //items = $('iframe')[0].contentWindow.get_Click_Items(level)

    for (var i = 0; i <= items.length-1; i++) {
        AutoTestItem = {};
        AutoTestItem.nom = i;
        AutoTestItem.level = level;
        AutoTestItem.clickItem = items[i];
        if (items.isLevelEmpty == true)
            AutoTestItem.isLevelEmpty = true;
        else
            AutoTestItem.isLevelEmpty = false;
        //AutoTestItem.closeClickItem = get_Auto_Test_Close_Click_Items(level);
        if (items[i].innerText != "")
            AutoTestItem.title = items[i].innerText;
        else
            AutoTestItem.title = items[i].title;

        if (level == 1) {
            AutoTestItems[i] = AutoTestItem;
            //AutoTestItems.length++;
        }
        if (level > 1) {
            if (oneAutoTestItem(level - 1, currLevelItem(level - 1)).childAutoTestItems == undefined)
                oneAutoTestItem(level - 1, currLevelItem(level - 1)).childAutoTestItems = {};
            oneAutoTestItem(level - 1, currLevelItem(level - 1)).childAutoTestItems[i] = AutoTestItem;
            if (oneAutoTestItem(level - 1, currLevelItem(level - 1)).childAutoTestItems.length != undefined)
                oneAutoTestItem(level - 1, currLevelItem(level - 1)).childAutoTestItems.length++;
            else
                oneAutoTestItem(level - 1, currLevelItem(level - 1)).childAutoTestItems.length = 1;
        }    
    }

    set_Curr_Level_Items(level, 0);
}
function set_Auto_Test_Close_Items(level) {
    if (currLevelItem(level) == undefined)
        oneAutoTestItem(level, 0).closeClickItem = get_Auto_Test_Close_Click_Items(level);
    else
        oneAutoTestItem(level, currLevelItem(level)).closeClickItem = get_Auto_Test_Close_Click_Items(level);
}
var oneAutoTestItem = function(level, nom) {
    if (level == 1) {
        for (var j = 0; j <= AutoTestItems.length - 1; j++) {
            if (AutoTestItems[j].nom == nom)
                return AutoTestItems[j];
        }
    }
    else {
        return get_Auto_Test_Child_Items_Recur(AutoTestItems, level, 0);
    }
}
var countAutoTestItems = function (level) {
    if (level == 1)
        return AutoTestItems.length;
    else {
        return oneAutoTestItem(level - 1, currLevelItem(level - 1)).childAutoTestItems.length;
    }
}
var countAutoTestChildItems = function (level) {
    //if (level == 1)
    //    return AutoTestItems.length;
    //else {
        let nom = 0;
        if (currLevelItem(level) != undefined)
            nom = currLevelItem(level);
   
        if (oneAutoTestItem(level, nom).childAutoTestItems == undefined)
            return 0;
        else
            return oneAutoTestItem(level, nom).childAutoTestItems.length;
    //}
}

function set_Auto_Test_Curr_Item(item, level) {
    for (let i = 0; i < get_Auto_Test_Click_Items(level).length; i++) {
        if (item === get_Auto_Test_Click_Items(level)[i]) {
            set_Curr_Level_Items(level, i);
            return;
        }
    }
}

function get_Auto_Test_Child_Items_Recur(items, level, curr) {
    let c = curr;
    for (var j = 0; j <= items.length - 1; j++) {

        if (items[j].level == CurrLevelItems[c].level
            && items[j].nom == CurrLevelItems[c].currNom) {

            if (items[j].level == level && items[j].nom == CurrLevelItems[c].currNom)
                return items[j];
            else {
                //if (items[j].childAutoTestItems != undefined) {
                    c = c + 1;
                    return get_Auto_Test_Child_Items_Recur(items[j].childAutoTestItems, level, c);
                //}
                //else
                    //return items[j];
            }
        }
    }
}

function get_Auto_Test_Click_Items(level) {
    if (level == 1) {
        if ($('#MainMenu li[for_autotest="True"] span.k-link').length > 0)
            return $('#MainMenu li[for_autotest="True"] span.k-link');
        else
            return get_Auto_Test_Empty_Item();
    }
    if (level == 2) {
        if ($('iframe').length == 0)
            return null;

        if ($('iframe')[0].contentWindow.$('a.ctrl-toolbar-button-forward, li.ctrl-menu-button-forward').length == 0)
            return get_Auto_Test_Empty_Item();
        else
            return $('iframe')[0].contentWindow.$('a.ctrl-toolbar-button-forward, li.ctrl-menu-button-forward');
    }
    if (level == 3) {
        if ($('iframe')[0].contentWindow.isGridEmpty == false)
            return $('iframe')[0].contentWindow.$('li[for_autotest="True"] span.k-link');
        else
            return $('iframe')[0].contentWindow.$('li[for_autotest="True"] span.k-link:first');
    }
    if (level == 4) {
        if ($('iframe')[0].contentWindow.$('iframe').length == 0)
            return undefined;

        if ($('iframe')[0].contentWindow.$('iframe')[0].contentWindow.isGridEmpty == false)
            //return $('iframe')[0].contentWindow.$('iframe')[0].contentWindow.$('li[for_autotest="True"] span.k-link');
        //else
            return $('iframe')[0].contentWindow.$('iframe')[0].contentWindow.$('li[for_autotest="True"] span.k-link:first');
    }
}

function get_Auto_Test_Empty_Item() {
    let emptyItems = {
        isLevelEmpty: true,
        length: 1
    };
    emptyItems[0] = { innerText: "Пустой уровень" };
    return emptyItems;
}

function get_Auto_Test_Close_Click_Items(level) {
    if (level == 1)
        return $('div.tab-title-right span.k-i-close')[0];
    if (level == 2 || level == 3)
        return $('iframe')[0].contentWindow.$('div.tab-title-right span.k-i-close')[0];
    if (level == 4)
        return $('iframe')[0].contentWindow.$('div.tab-title-right span.k-i-close')[1];
}

function click_Auto_Test_Modal() {
    if (Top().isAutoTestClient == false)
        return;

    setTimeout(function () {
        if ($('.ctrl-modal-window-ok-button').length > 0)
            $('.ctrl-modal-window-ok-button')[$('.ctrl-modal-window-ok-button').length - 1].click();
        if ($('div.k-window span.k-i-close').length > 0)
            $('div.k-window span.k-i-close')[0].click();

        end_Auto_Test_Item();
    }, 500);
}

function close_Auto_Test_Curr_Item(level) {
    if (oneAutoTestItem(level, currLevelItem(level)).closeClickItem != undefined)
        oneAutoTestItem(level, currLevelItem(level)).closeClickItem.click();
}

function close_Auto_Test_Other_Items(level){
    var items;
    if (level == 1)
        items = $('div.tab-title-right span.k-i-close');
    if (level == 2 || level == 3)
        items = $('iframe')[0].contentWindow.$('div.tab-title-right span.k-i-close');
    if (level == 4)
        items = $('iframe')[0].contentWindow.$('div.tab-title-right span.k-i-close');

    if (items.length > 0) {
        $.each(items, function () {
            if (this !== oneAutoTestItem(1, currLevelItem(1)).closeClickItem) {
                $(this).click();
            }
        });
    }
}

function end_Auto_Test_Item(level) {
    if (level == undefined)
        level = currLevel();

    //тут будет запись в лог
    set_Auto_Test_Log();
    set_Auto_Test_Close_Items(level);

    //if (currLevelItem(level) == undefined || currLevelItem(level + 1) == undefined) {
    //    set_Auto_Test_Items(level);
    //}

    if (currLevelItem(level + 1) == undefined || countAutoTestChildItems(level) == 0) {
        set_Auto_Test_Items(level + 1);
    }

    if (countAutoTestChildItems(level) > 0) { //если есть дочерние элементы
        //переход глубже на уровень
        set_Curr_Level_Items(level + 1, 0);
        if (AutoTestItem.isLevelEmpty != true)
            oneAutoTestItem(level + 1, currLevelItem(level + 1)).clickItem.click();
    }
    else { //иначе берем следующий элемент этого уровня
        if (CurrLevelItems[level - 1].currNom < countAutoTestItems(level) - 1) { //если это не последний элемент
            //кликаем следующий элемент
            close_Auto_Test_Curr_Item(level);
            set_Curr_Level_Items(level, currLevelItem(level) + 1);
            oneAutoTestItem(level, currLevelItem(level)).clickItem.click();
        }
        else { //иначе возвращаемся на уровень выше
            close_Auto_Test_Curr_Item(level);
            CurrLevelItems[level - 1] = undefined;
            CurrLevelItems.length--;
            end_Auto_Test_Level(level - 1);
        }   
    }
}

function end_Auto_Test_Level(level) {
    if (level == undefined)
        level = currLevel();

    if (CurrLevelItems[level - 1].currNom < countAutoTestItems(level) - 1) { //если на этом уровне это не последний элемент
        close_Auto_Test_Curr_Item(level);
        //кликаем следующий элемент
        set_Curr_Level_Items(level, currLevelItem(level) + 1);
        oneAutoTestItem(level, currLevelItem(level)).clickItem.click();
    }
    else { //иначе поднимаемся снова на уровень выше
        close_Auto_Test_Curr_Item(level);
        CurrLevelItems[level - 1] = undefined;
        CurrLevelItems.length--;
        if (level > 1) //если это не первый уровень
            end_Auto_Test_Level(level - 1);
        else //иначе заканчиваем автотест
            turn_On_Auto_Test(false);
    }
}

function set_Auto_Test_Log(e, end) {
    Top().isErrorAutoTest = true;
    if (e != undefined && e.type != 'server')
        messageWindow("Ошибка в скриптах", e.stack);

    var options = {};
    options.url = Top().location.pathname + "Home/Logging";
    options.url = updateQueryStringParameter(options.url, "browser", Top().get_Browser_Type());
    options.url = updateQueryStringParameter(options.url, "isAutoTestLogging", check_AutoTest_Logging());
    if (vm_Base.uri_path_out != undefined)
        options.url = updateQueryStringParameter(options.url, "uri_path", vm_Base.uri_path_out);
    if (end == true) {
        options.url = updateQueryStringParameter(options.url, "end_auto_test", true);
        options.url = updateQueryStringParameter(options.url, "auto_test_text", "Автотест окончен!");
    }
    else {
        if (e != undefined) {
            options.data = updateQueryStringParameter(options.url, "error_auto_test_text", e.stack);
            if (currClickItem != undefined && currClickItem.innerText != "")
                options.url = updateQueryStringParameter(options.url, "error_auto_test_item", currClickItem.innerText);
            if (e.controller != undefined)
                options.url = updateQueryStringParameter(options.url, "error_auto_test_item", e.controller);
            if (vm_Base.title != undefined && vm_Base.title != "")
                options.url = updateQueryStringParameter(options.url, "error_auto_test_item", vm_Base.title);
        }
        else {
            if (vm_Base.auto_test_text != undefined)
                options.data = updateQueryStringParameter(options.url, "auto_test_text", vm_Base.auto_test_text);
        //    options.url = updateQueryStringParameter(options.url, "div_type", vm_Base.div_type);
        //    options.data = updateQueryStringParameter(options.url, "html", vm_Base.html);
        }
    }
    options.type = "POST";
    $.ajax(options);

    vm_Base.html = undefined;
    vm_Base.auto_test_text = undefined;
}

function set_Auto_Test_Server_Error(is_child, controller) {
    if (Top().isAutoTestClient == false)
        return;

    setTimeout(function () {
        let error = $('.serv_error');
        if (error.length == 0)
            error = $('iframe')[0].contentDocument.body.getElementsByClassName('serv_error')
        if (error.length > 0)
            Top().set_Auto_Test_Log({ stack: error[0].innerText, type: 'server', controller: controller });

        //if (soperation_id == 192 || soperation_id == 14)
        //    Top().end_Auto_Test_Item(1);

        if (is_child == 0) {
            Top().end_Auto_Test_Item(1);
            Top().end_Auto_Test_Item();
        }
        else
            Top().end_Auto_Test_Item();

    }, 1000);
}

function turn_On_Auto_Test(on) {
    if (on) {
        Top().isAutoTestClient = true;
        $('#autotest_label').removeClass("is-not-visible");
        $('#autotest_label').addClass("is-visible");
        $('#turn_autotest_button_on').addClass("is-not-visible");
        $('#turn_autotest_button_on').removeClass("is-visible");
        $('#turn_autotest_button_off').removeClass("is-not-visible");
        $('#turn_autotest_button_off').addClass("is-visible");

        vm_Base.auto_test_text = "Автотест запущен!";
        set_Auto_Test_Log();
    }
    else {
        Top().isAutoTestClient = false;
        $('#turn_autotest_button_off').removeClass("is-visible");
        $('#turn_autotest_button_off').addClass("is-not-visible");
        $('#turn_autotest_button_on').addClass("is-visible");
        $('#turn_autotest_button_on').removeClass("is-not-visible");

        vm_Base.auto_test_text = "Автотест остановлен!";
        set_Auto_Test_Log();
    }
}

//function click_Auto_Test_Item(level, sender) {
//    //if (currLevelItem(level) == undefined)
//    //set_Auto_Test_Curr_Item($(sender).find("span.k-link")[0], level);
//    //if (level != 1 || oneAutoTestItem(level, currLevelItem(level)) == undefined) //{
//    //set_Auto_Test_Items(level);
//    //AutoTestItems[0].clickItem.click();
//    //}    
//}

