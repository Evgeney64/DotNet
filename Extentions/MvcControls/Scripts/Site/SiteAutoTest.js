var doubleEvent = 0; //признак, который не позволяет событию срабатывать несколько раз. 
var clickItems;
var countClickItems;
var currClickItem;
var currClickItemNom = 0;
var isAutoTestNew = false; //признак автотест включен/выключен
var isErrorAutoTest = false;
var isEventListenersSet = false;

function initialize_AutoTest() {
    //set_AutoTest_Event_Listeneres('MainMenu');
    document.getElementById('MainMenu').addEventListener("autoTestNextStep", function (e) {
        doubleEvent = 0;
        currClickItemNom++;
        run_AutoTest_One_Step();
    });
    document.getElementById('MainMenu').addEventListener("autoClickTabClose", function (e) {
        run_AutoTest_One_Step_Close();
    });
    document.getElementById('MainMenu').addEventListener("autoClickModalOk", function (e) {
        $('.ctrl-modal-window-ok-button')[$('.ctrl-modal-window-ok-button').length - 1].click();
    });
    isEventListenersSet = true;

    turn_on_AutoTest(true);
    //run_AutoTest(1);
}

function start_AutoTest() {
    initialize_AutoTest();
    clickItems = get_Click_Items(1);
    countClickItems = clickItems.length;
}

function run_AutoTest(level) {
    clickItems = get_Click_Items(level);
    countClickItems = clickItems.length;
    run_AutoTest_One_Step();
}

function get_Click_Items(level) {
    currClickItemNom = 0;
    if (level == 1)
        return $('#MainMenu li[for_autotest="True"] span.k-link');
    if (level == 2) {
        //currClickItemNom = 0;
        return $('a.ctrl-toolbar-button-forward');
    }
    if (level == 3) {
        doubleEvent = 1;
        return $('li[for_autotest="True"] span.k-link');//$($('ul[role="menu"]')[1]).find('li.k-item span.k-link');
    }
}

function turn_on_AutoTest(on) {
    if (on) {
        Top().isAutoTestNew = true;
        //isAutoTestNew = true;
        doubleEvent = 0;
        $('#autotest_label').removeClass("is-not-visible");
        $('#autotest_label').addClass("is-visible");
        $('#turn_autotest_button_on').addClass("is-not-visible");
        $('#turn_autotest_button_on').removeClass("is-visible");
        $('#turn_autotest_button_off').removeClass("is-not-visible");
        $('#turn_autotest_button_off').addClass("is-visible");
    }
    else
    {
        Top().isAutoTestNew = false;
        //isAutoTestNew = false;
        $('#turn_autotest_button_off').removeClass("is-visible");
        $('#turn_autotest_button_off').addClass("is-not-visible");
        $('#turn_autotest_button_on').addClass("is-visible");
        $('#turn_autotest_button_on').removeClass("is-not-visible");
        //set_AutoTest_Error(undefined, true);
    }
}

function run_AutoTest_One_Step() {
    if (currClickItemNom == countClickItems) {
        if (window === Top()) {
            turn_on_AutoTest(false);
            return;
        }
        else {
            currClickItemNom++;
            Top().exec_Custom_Event("MainMenu", "autoClickTabClose");
            return;
        }
    }

    set_AutoTest_CurrClickItem(clickItems[currClickItemNom]);
    if (currClickItem != undefined) {
        currClickItem.click();
        //currClickItemNom++;
        currClickItem = clickItems[currClickItemNom];
    }
}

function set_AutoTest_CurrClickItem(item) {
    for (let i = 0; i < clickItems.length; i++)
    {
        if (item === clickItems[i])
        {
            currClickItem = item;
            currClickItemNom = i;
            return;
        }
    }
}

function run_AutoTest_One_Step_Close() {
    if ($('div.tab-title-right span.k-i-close').length != 0) {
        $('div.tab-title-right span.k-i-close')[0].click();
    }
    else
        Top().exec_Custom_Event("MainMenu", "autoTestNextStep");
}

function run_AutoTest_One_Step_ModalWindow_Ok() {
    $('.ctrl-modal-window-ok-button')[$('.ctrl-modal-window-ok-button').length - 1].click();
}

function exec_Custom_Event(div_name, event_name, event_levet, html, div_type) {
    if (Top().isAutoTestNew) {
        setTimeout(function () {
            var autoTestEvent = new CustomEvent(event_name, { detail: { level: event_levet, html: html, div_type: div_type } });
            document.getElementById(div_name).dispatchEvent(autoTestEvent);
        }, 1500);
    }
}

function test_serv_error(is_child, controller) {
    if (Top().check_AutoTest_Logging() == false || Top().isAutoTestNew == false)
        return;

    setTimeout(function () {
        let error = $('.serv_error');
        if (error.length == 0)
            error = $('iframe')[0].contentDocument.body.getElementsByClassName('serv_error')
        if (error.length > 0)
            set_AutoTest_Error({ stack: error[0].innerText, type: 'server', controller: controller });
        if(is_child == 0)
            exec_Custom_Event("FilterTab", "autoClickTabClose", 3);
        else
            run_AutoTest_One_Step_Close(); //по-хорошему тут надо вызывать событие закрытия таба, а не функцию

    }, 1000);
}

function set_AutoTest_Error(e, end) {
    Top().isErrorAutoTest = true;
    if (e != undefined && e.type != 'server')
        messageWindow("Ошибка в скриптах", e.stack);

    var options = {};
    options.url = vm_Base.url_Content + "Home/Logging";
    options.url = updateQueryStringParameter(options.url, "browser", Top().get_Browser_Type());
    options.url = updateQueryStringParameter(options.url, "isAutoTestLogging", check_AutoTest_Logging());
    if (vm_Base.uri_path_out != undefined)
        options.url = updateQueryStringParameter(options.url, "uri_path", vm_Base.uri_path_out);
    if (end == true) {
        options.url = updateQueryStringParameter(options.url, "end_auto_test", true);
        options.url = updateQueryStringParameter(options.url, "error_auto_test", "Тест окончен!");
    }
    else {
        if (e != undefined) {
            options.data = updateQueryStringParameter(options.url, "error_auto_test_text", e.stack);
            if (currClickItem != undefined && currClickItem.innerText != "")
                options.url = updateQueryStringParameter(options.url, "error_auto_test_item", currClickItem.innerText);
            else {
                if (e.controller != undefined)
                    options.url = updateQueryStringParameter(options.url, "error_auto_test_item", e.controller);
            }
        }
        else {
            options.url = updateQueryStringParameter(options.url, "div_type", vm_Base.div_type);
            options.data = updateQueryStringParameter(options.url, "html", vm_Base.html);
        }
    }
    options.type = "POST";
    $.ajax(options);

    vm_Base.html = undefined;
}

function set_AutoTest_Event_Listeneres(div_name) {
    if (Top().isAutoTestNew && !isEventListenersSet && document.getElementById(div_name) != null) {
        document.getElementById(div_name).addEventListener("autoTestRunLevelTest", function (e) {
            if (e.detail.html != undefined) {
                e.detail.html = e.detail.html.replace(/"/g, "'");
                vm_Base.html = get_Html_For_AutoTest(e.detail.html);
                vm_Base.div_type = e.detail.div_type;
                set_AutoTest_Error(undefined, false);
            }

            run_AutoTest(e.detail.level);
        });
        document.getElementById(div_name).addEventListener("autoTestNextStep", function (e) {
            currClickItemNom++;
            run_AutoTest_One_Step();
        });
        document.getElementById(div_name).addEventListener("autoClickTabClose", function (e) {
            Top().doubleEvent = 0;

            if (e.detail.level == 3)
                parent.run_AutoTest_One_Step_Close();
            else
                run_AutoTest_One_Step_Close();
        });
        document.getElementById(div_name).addEventListener("autoClickModalOk", function (e) {
            Top().doubleEvent = 0;
            parent.run_AutoTest_One_Step_ModalWindow_Ok();
        });
        isEventListenersSet = true;
    }
}

function get_Html_For_AutoTest(html, str_beg, str_end) {
    if (str_beg == undefined) {
        str_beg = "href='";
        html = get_Html_For_AutoTest(html, str_beg, "'");
        str_beg = " id='";
        html = get_Html_For_AutoTest(html, str_beg, "'");
        str_beg = " name='";
        html = get_Html_For_AutoTest(html, str_beg, "'");
        str_beg = "page_guid='";
        html = get_Html_For_AutoTest(html, str_beg, "'");
        str_beg = "parent_guid='";
        html = get_Html_For_AutoTest(html, str_beg, "'")
        str_beg = "guids_out='";
        html = get_Html_For_AutoTest(html, str_beg, "'");
        str_beg = "guids-out='";
        html = get_Html_For_AutoTest(html, str_beg, "'");
        str_beg = "data-uid='";
        html = get_Html_For_AutoTest(html, str_beg, "'");
        str_beg = "data-tabchildtoolbar='";
        html = get_Html_For_AutoTest(html, str_beg, "'");
        str_beg = "aria-activedescendant='";
        html = get_Html_For_AutoTest(html, str_beg, "'");
        str_beg = "aria-owns='";
        html = get_Html_For_AutoTest(html, str_beg, "'");
        str_beg = "aria-controls='";
        html = get_Html_For_AutoTest(html, str_beg, "'");
        str_beg = "data-marker='";
        html = get_Html_For_AutoTest(html, str_beg, "'");
        str_beg = "data-fk_field='";
        html = get_Html_For_AutoTest(html, str_beg, "'");
        str_beg = "data-is_main='";
        html = get_Html_For_AutoTest(html, str_beg, "'");
        str_beg = "data-maintabletabchild='";
        html = get_Html_For_AutoTest(html, str_beg, "'");
        str_beg = "for='";
        html = get_Html_For_AutoTest(html, str_beg, "'");
        str_beg = "onchange('";
        html = get_Html_For_AutoTest(html, str_beg, ");'");
        str_beg = "onclick('";
        html = get_Html_For_AutoTest(html, str_beg, ");'");
        str_beg = "onchange='";
        html = get_Html_For_AutoTest(html, str_beg, ");'");
        str_beg = "onclick='";
        html = get_Html_For_AutoTest(html, str_beg, ");'");
        str_beg = "<script>";
        html = get_Html_For_AutoTest(html, str_beg, "</script>");
        str_beg = "k-progress k-complete";
        html = get_Html_For_AutoTest(html, str_beg, "'");
        str_beg = "k-complete k-progress";
        html = get_Html_For_AutoTest(html, str_beg, "'");
        return html;
    }
    else {
        while (true) {
            if (html.indexOf(str_beg) == -1)
                break;
            let pos1 = html.indexOf(str_beg);
            let str1 = html.substring(0, pos1);// + str.length + 2);
            let pos2 = html.substring(pos1 + str_beg.length).indexOf(str_end);
            let str2 = html.substring(pos1 + str_beg.length + pos2 + str_end.length);
            html = str1 + str2;
        }
        return html;
    }
}

function check_AutoTest_Logging() {
    if (Top().$('#checkbox_auto_test_logging')[0].checked)
        return true;
    else
        return false;
}

(function () {
    if (window.CustomEvent.length != undefined)
        return false;
    if (typeof window.CustomEvent === "function")
        return false; //If not IE

    function CustomEvent(event, params) {
        params = params || { bubbles: false, cancelable: false, detail: undefined };
        var evt = document.createEvent('CustomEvent');
        evt.initCustomEvent(event, params.bubbles, params.cancelable, params.detail);
        return evt;
    }

    CustomEvent.prototype = window.Event.prototype;

    window.CustomEvent = CustomEvent;
})();

