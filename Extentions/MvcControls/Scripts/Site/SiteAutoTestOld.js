function autoStartPage(
    url_Content,
    mainTab_IsAutoLoaded,
    mainTab_Text,
    soperation_id,
    guids_out,
    uri_path_out,
    indexParams
) {

    vm_Base.initializeArguments(arguments);
    //initialize(arguments);

    vm_Base.url_Action = getActionName_BySoperation(url_Content, soperation_id);
    if (isAutoTest(uri_path_out) && isLik(uri_path_out) == false)
        vm_Base.url_Action = url_Content + "Home/Main_Layout";

    if (vm_Base.url_Action == null)
        return;

    vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "uri_path", uri_path_out);
    vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "maintab_isautoloaded", mainTab_IsAutoLoaded);

    if (indexParams != null)
        vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "index_params", indexParams);

    var tabstrip = $("#tabstrip");
    if (tabstrip.length == 0)
        tabstrip = Top().$("#tabstrip");

    var guid_out = null;
    if (guids_out.split('=').length > 1)
        guid_out = guids_out.split('=')[1];

    mainTab_Open(/*tabstrip, undefined, */mainTab_Text, vm_Base.url_Action/*, guid_out, guids_out, undefined, undefined, mainTab_IsAutoLoaded*/);
}

function mainTab_Open_AutoTest(url_Content, mainTab_Text, guids_out, uri_path_out, is_last, from_where) {
    //if (is_last == 1) {
    //    var ss = 1;
    //}

    initialize(arguments);

    if (isAutoTest(uri_path_out)) {
        setTimeout(function () {

            var tabstrip = $("#tabstrip");
            if (tabstrip.length == 0)
                tabstrip = Top().$("#tabstrip");

            if (is_last == 0) {

                var guid_Index = getGuidStr_FromGuidsOut_ByName(guids_out, "Index");
                guids_out = "Index=" + guid_Index + ";";

                var uri_paths = uri_path_out.split(';');
                var soperation_id = getUriPath_Soperation(uri_path_out, 110, ';');
                var url = getActionName_BySoperation(url_Content, soperation_id);

                url = updateQueryStringParameter(url, "guids_out", guids_out);
                url = updateQueryStringParameter(url, "uri_path", uri_paths[0]);
                url = updateQueryStringParameter(url, "maintab_isautotest", 1);

                mainTab_Open(/*tabstrip, undefined, */mainTab_Text, url/*, guid_Index, guids_out, undefined, true*/);

                if (from_where == undefined || from_where != "Index_Ready") {
                    if (isLik(uri_path_out)) {
                        if (tabstrip.data("kendoTabStrip").items().length > 2)
                            tabStrip_RemoveTab(tabstrip, "second");
                    }
                    else
                        tabStrip_RemoveTab(tabstrip, "first");
                }
            }
            if (is_last == 1) {
                setTimeout(function () {
                    messageWindow("Сообщение", "Авто-тестирование закончено");

                    var options = {};
                    options.url = url_Content + "Home/Logging";
                    options.url = updateQueryStringParameter(options.url, "log_type", 1);
                    options.type = "POST";
                    $.ajax(options);


                    if (isLik(uri_path_out))
                        tabStrip_RemoveTab(tabstrip, "second");
                    else
                        tabStrip_RemoveTab(tabstrip, "first");

                    if (tabstrip.data("kendoTabStrip").items().length > 0) {
                        if (tabstrip.find("ul li:last-child").length > 0) {
                            tabstrip.data("kendoTabStrip").activateTab(tabstrip.find("ul li:last-child"));
                        }
                        //tabStrip_ActivateLastTab(tabstrip);
                    }

                }, auto_test_delay);
            }

        }, auto_test_delay);
    }
}

function childTab_Open_AutoTest(
    url_Action,
    title,
    page_guid, guids_out,
    soperation_id,

    uri_Content,
    mainTab_Text,
    uri_path_out,
    is_last,

    from_where
) {
    initialize(arguments);

    var sss = 0;
    setTimeout(function () {

        var row_id = getRowId_ByGuid(page_guid);
        if (row_id == null && isOperationsForRow(soperation_id) == true) {
            mainTab_Open_AutoTest(uri_Content, mainTab_Text, guids_out, uri_path_out, is_last, from_where)
            return;
        }

        vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "maintab_isautotest", 1);

        childTab_Open(undefined/*, undefined*/, vm_Base.url_Action, title/*, soperation_id*/);  //page_guid, guids_out,

    }, auto_test_delay);
}

function tabStrip_RemoveTab(tabstrip, tab_num, ul_li, tab_num_activate, parent_guid) {
    var kendoTabStrip = tabstrip.data("kendoTabStrip");
    if (kendoTabStrip == undefined)
        return;
    var tabNums = kendoTabStrip.items().length;

    var tabNum = null;
    if (tab_num == "first")
        tabNum = 0;
    else if (tab_num == "second")
        tabNum = 1;
    else if (tab_num == "last")
        tabNum = tabNums - 1;
    else if (tab_num == "this" && ul_li != undefined && ul_li.length > 0) {
        kendoTabStrip.remove(ul_li);
        //return;
    }

    // Удаляем tab
    if (tabNum != null) {
        kendoTabStrip.remove(tabNum);
    }

    if (tabstrip.find("ul li:last-child").length == 0 && parent_guid != undefined)
        mainTableSplitter_LastPane_Collapse(parent_guid, true);

    // Активируем tab
    if (tab_num_activate != undefined) {
        var selector_tab = null;
        if (tab_num == undefined || tab_num_activate == "last")
            selector_tab = "ul li:last-child";
        else if (tab_num_activate == "this")
            selector_tab = "ul li:last-child";
        else if (tab_num_activate == "first")
            selector_tab = "ul li:first-child";

        if (tabstrip.find(selector_tab).length > 0)
            kendoTabStrip.activateTab(tabstrip.find(selector_tab));
    }
}

function tabStrip_ActivateLastTab(tabstrip, tab_num) {

    var selector_tab = null;
    if (tab_num == undefined || tab_num == "last")
        selector_tab = "ul li:last-child";
    else if (tab_num == "this")
        selector_tab = "ul li:last-child";
    else if (tab_num == "first")
        selector_tab = "ul li:first-child";

    if (tabstrip.find(selector_tab).length > 0)
        tabstrip.data("kendoTabStrip").activateTab(tabstrip.find(selector_tab));
}
