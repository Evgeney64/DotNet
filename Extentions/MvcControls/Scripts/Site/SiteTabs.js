//MainTabs*******************************************************************************************
function mainTab_Open(title, url_Action) {
    try {
        vm_Base.initializeArguments(arguments);

        var uri_path_grid = getUriPath_ByType(clickItem.url_action, 101);
        var uri_path_menu = getUriPath_ByType(clickItem.url_action, 111);
        /*Из-за автооткрытия вкладок, у vm_Base остается операция последней открытой вкладки*/
        //if (uri_path_menu != null)
            //clickItem.soperation_id = uri_path_menu.SoperationId;

        if (uri_path_grid == null || clickItem.soperation_id == 19)
            uri_path_grid = getUriPath_ByType(clickItem.url_action, 108);
        var soperation_id = 0;
        if (uri_path_grid != null)
            soperation_id = uri_path_grid.SoperationId;
        if (is_true(vm_Base.editor_in_main_tab) && uri_path_menu != null)
            soperation_id = uri_path_menu.SoperationId;

        var uri_path_dashbord = getUriPath_ByType(clickItem.url_action, 130);

        var main_tab_guid = kendo.guid();
        var main_tab_guid_value = "MainTab=" + main_tab_guid;

        let guids_out = vm_Base.guids_out;
        if (guids_out != undefined)
            guids_out = setGuidsStr(guids_out, main_tab_guid_value);

        clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "main_tab_open", 1);
        clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "guids_out", guids_out);

        if (vm_Base.page_guid != undefined
            && uri_path_grid != null
            && soperation_id != sop_41 // Открыть строку-список из ячейки грида
        ) {
            var row_id = getRowId_ByGuid(vm_Base.page_guid, vm_Base);
            if (row_id != null)
                clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "parent_id", row_id);
            clickItem.url_action = mainGrid_Parameters(vm_Base, clickItem.url_action, uri_path_menu.SoperationId);
        }

        //Осинных. Проверка срабатывает только для открытия карточки в MainTab
        if (row_id == null
            && is_true(vm_Base.editor_in_main_tab)
            && soperation_id != sop_0
            && soperation_id != sop_2
            && soperation_id != sop_12) {
            messageWindow("Ошибка", "Выберите строку.");
            return null;
        }

        var onIframe_Load_str = "";
        var uri_path_110 = getUriPath_ByType(clickItem.url_action, 110);
        if (uri_path_110 != null && uri_path_110.IsAutoTest == false) {
            if (vm_Base.is_onIframe_Load == undefined || vm_Base.is_onIframe_Load == 1)
                onIframe_Load_str = " onload='onIframe_Load(this);'";
        }
        // отключено для HelpOpen. почему-то ругается
        var uri_path = getUriParam_ByName(clickItem.url_action, "uri_path");
        var uri_path_last = getUriPath_FromStr_Last(uri_path);
        //var _content = "<iframe id='MainTableSplitter_" + main_tab_guid + "' src='" + clickItem.url_action + "' " + onIframe_Load_str + "></iframe>";
        var _content =
            "<iframe " +
                " id='MainTabFrame_" + main_tab_guid + "'" +
                " class='flexbox-item-full loader-spin '" +
                " page_guid='" + main_tab_guid + "'" +
                " src='" + clickItem.url_action + "'" +
                " uri_path='" + uri_path + "'" +
                " is_main_tab='True'" +
                " soperation_id=" + uri_path_last.SoperationId +
                " stable_id=" + uri_path_last.StableId +
                onIframe_Load_str +
            " ></iframe>";

        if ((vm_Base.is_auto_test == undefined || vm_Base.is_auto_test == false)
            && soperation_id != 0 // Открыть строку-список из ячейки грида
            && soperation_id != sop_41 // Открыть строку-список из ячейки грида
            && soperation_id != sop_19 // Открыть список (дерево)
            && soperation_id != sop_191 // Открыть главный список (дерево)
            && soperation_id != sop_192 // Открыть главный список (дерево) без фильтра
            && soperation_id != sop_6
            && soperation_id != sop_5
            && uri_path_dashbord == null
            ) {
            if (is_true(vm_Base.editor_in_main_tab))
                Top().leftPaneWindowResize();
            else
                leftPaneWindowResize();
        }

        var main_tab_class = "";
        if (is_true(vm_Base.mainTab_IsAutoLoaded)) {
            main_tab_class = "main-tab";
            clickItem.title = vm_Base.main_page_title;  //Убрать. Передается слишком много переменных для заголовка
            if (clickItem.title == undefined)
                clickItem.title = vm_Base.mainTab_Text;
        }

        if (clickItem.mask_title != undefined) {
            var title = get_TabTitle_From_Mask(clickItem.mask_title);
            if (title != undefined)
                clickItem.title = title;
        }
        var vm_Base_title = clickItem.title;

        if (soperation_id != sop_2
            && soperation_id != sop_12
            && is_true(vm_Base.MainTabTitle_FromGrid)
            && clickItem.mask_title == undefined) {
            var column_value = get_Grid_ColumnValue(vm_Base.mainTab_TitleField);
            if (vm_Base.mainTab_TitlePrefix != null && vm_Base.mainTab_TitlePrefix != "")
                column_value = vm_Base.mainTab_TitlePrefix + column_value;

            if (column_value != undefined)
                vm_Base_title = column_value;
        }

        if (!isMobile() ) {
            vm_Base.mainTabstrip.data("kendoTabStrip").append([{
                text: "<span id='MainTab_" + main_tab_guid + "' class='" + main_tab_class + "'><div class='tab-title-left'>" + vm_Base_title + '</div>' +
                //text: "<span id='MainTab_" + main_tab_guid + "' >" + vm_Base.title +
                    '<div class="tab-title-right"><span class="k-icon k-i-close" onClick="mainTab_Close(this, \'' + main_tab_guid + '\')"></span></div></span>',
                encoded: false,
                content: _content,
            }]);
            tabStrip_ActivateLastTab(vm_Base.mainTabstrip);

            mainTableSplitter_LastPane_Collapse(main_tab_guid, true);
        } else {
            vm_Base.mainTabstrip.append(_content);

            if (!is_true(vm_Base.mainTab_IsAutoLoaded) && vm_Base.mainTabstrip.length > 0)
                setTimeout(function () { view_open(vm_Base.mainTabstrip[0].id) }, 10);
        }
    }
    catch(e) {
        if (Top().isAutoTestClient)
            Top().set_Auto_Test_Log(e);
            //set_AutoTest_Error(e);
        else
            throw e;
    }
}

function mainTab_Open_Verify(title, url_Action) {

    vm_Base.initializeArguments(arguments);

    var row_id = getRowId_ByGuid(vm_Base.page_guid, vm_Base);
    if (row_id == null) {
        messageWindow("Ошибка", "Выберите строку.");
        return null;
    }
    var parent_id = row_id;
    var index_semicolon = row_id.indexOf(";");
    if (index_semicolon >= 0) {
        parent_id = row_id.substring(0, index_semicolon);
    }

    var options = {};
    clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "parent_id", parent_id);
    clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "is_operation_rule", 1);
    options.url = clickItem.url_action;
    options.type = "POST";

    options.success = function (data) {
        if (data.status === "error") {
            messageWindow("Ошибка", data.message, data.error_text);
        }
        else {
            mainTab_Open();
        }
    };

    $.ajax(options);
}

function mainTab_Close(sender, main_tab_guid) {
    try {
        //  по нажатию на крестик закрываем таб
        var tabstrip = $(sender).closest(".k-tabstrip");

        if (tabstrip.find(".k-tabstrip-items").length > 0 && tabstrip.find(".k-tabstrip-items").data("kendoResponsivePanel")) {
            tabstrip.find(".k-tabstrip-items").data("kendoResponsivePanel").close();
        }
        var item = $(sender).closest(".k-item");

        //if (Top().isAutoTestNew) {
        //    Top().exec_Custom_Event("MainMenu", "autoTestNextStep");
        //}
        
        if (tabstrip.data("kendoTabStrip") != null) {
            tabstrip.data("kendoTabStrip").remove(item.index());
            tabStrip_ActivateLastTab(tabstrip);
        }
        return false;
    }
    catch (e) {
        if (Top().isAutoTestClient)
            Top().set_Auto_Test_Log(e);
            //set_AutoTest_Error(e);
        else
            throw e;
    }
}

function mainTab_Close_All() {
    var tabstrip = $("*").closest(".k-tabstrip");
    var items = tabstrip.find(".k-item");

    if (items.length > 0) {
        do {
            tabstrip.find(".k-tabstrip-items").data("kendoResponsivePanel").close();
            tabstrip.data("kendoTabStrip").remove(items.index());
            items = tabstrip.find(".k-item");
        }
        while (items.length > 0);
    }

    return false;
}

function mainTab_Open_From_List(sender, url_action) { //tabstrip, vm_Base_params, url_action

    //initialize(arguments);
    vm_Base.initializeArguments(arguments, true);
    
    /*Работа 6533. Дублируем заголовок дочернего таба для открываемого главного таба*/
    if (sender != undefined && sender.getAttribute("title") != undefined && sender.getAttribute("title") != "")
        vm_Base.title = sender.getAttribute("title");

    var grid = $("#grid_" + vm_Base.page_guid);
    if (grid.length > 0) {
        var filter_string = grid.attr("filter_string");
        clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "filter_string", filter_string);

        var filter_list = grid.attr("filter_list");
        if (filter_list != undefined)
            clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "filter_list", filter_list);
    }
    if (vm_Base.is_child_list_int != undefined && vm_Base.is_child_list_int == 1) {
        var row_id = window.parent.getRowId_ByGuid_0(vm_Base.parent_guid);
        var row_id1 = getRowId_ByGuid_0(vm_Base.page_guid);
        if (row_id == null || row_id1 == null) {
            messageWindow("Ошибка", "Выберите строку.");
            return null;
        }
        clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "parent_id", row_id);
        clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "parent_id1", row_id1);
    }

    mainTab_Open_From_Source(/*tabstrip*/);
}

function mainTab_Open_From_Row(url_Action) { //tabstrip, vm_Base_params, url_Action
    //initialize(arguments);
    vm_Base.initializeArguments(arguments);

    var row_id = getRowId_ByGuid_0(vm_Base.page_guid);
    if (row_id == null) {
        messageWindow("Ошибка", "Выберите строку.");
        return null;
    }
    clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "parent_id", row_id);

    mainTab_Open_From_Source(/*tabstrip*/);
}

function mainTab_Open_From_TreeRelation(url_Action) {  //tabstrip, vm_Base_params, url_Action
    //initialize(arguments);
    vm_Base.initializeArguments(arguments);

    var parent_id = window.parent.getRowId_ByGuid_0(vm_Base.parent_guid);
    var parent_id1 = getRowId_ByGuid_0(clickItem.page_guid);
    if (parent_id == null || parent_id1 == null) {
        messageWindow("Ошибка", "Выберите строку.");
        return null;
    }

    var options = {};
    clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "parent_id", parent_id);
    clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "parent_id1", parent_id1);
    //clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "is_operation_rule", 1);
    clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "is_operation_rule_verify", 1);
    clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "temp_value", getRandomValue());

    options.url = clickItem.url_action;
    options.type = "POST";

    options.success = function (data) {
        if (data.status === "error") {
            messageWindow("Ошибка", data.message, data.error_text);
        }
        else {
            clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "parent_id", parent_id);
            clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "parent_id1", parent_id1);
            mainTab_Open_From_Source(/*tabstrip*/);
        }
    };

    $.ajax(options);

}

function mainTab_Open_From_Source(url_Action) { //tabstrip, vm_Base_params, url_Action
    //initialize(arguments, true);
    vm_Base.initializeArguments(arguments);

    var main_tab_guid = kendo.guid();

    var _content = "<iframe id='MainTabFrame_" + main_tab_guid + "' class='flexbox-item-full loader-spin' src='" + clickItem.url_action + "' ></iframe>";

    vm_Base.mainTabstrip.data("kendoTabStrip").append([{
        text: "<span id='MainTab_" + main_tab_guid + "'><div class='tab-title-left'>" + clickItem.title + '</div>' +
        //text: "<span id='MainTab_" + main_tab_guid + "' >" + vm_Base.title +
            '<div class="tab-title-right"><span class="k-icon k-i-close" onClick="mainTab_Close(this, \'' + main_tab_guid + '\')"></span></div></span>',
        encoded: false,
        content: _content
    }]);
    vm_Base.mainTabstrip.data("kendoTabStrip").activateTab(vm_Base.mainTabstrip.find("ul li:last-child"));
    $(window).trigger('resize');
}

function mainTab_Open_For_ExecLinkUrl(/*tabstrip, title, url, page_guid*/) {
    let tabstrip = findInParent("#tabstrip");
    let data_param = clickItem.url_action;

    let main_tab_guid = kendo.guid();
    let _content = "<iframe id='MainTabFrame_" + main_tab_guid + "' class='flexbox-item-full loader-spin' src='" + data_param + "'></iframe>";

    tabstrip.data("kendoTabStrip").append([{
        text: "<span id='MainTab_" + main_tab_guid + "'><div class='tab-title-left'>" + clickItem.title + '</div>' +
        //text: "<span id='MainTab_" + main_tab_guid + "' >" + title +
            '<div class="tab-title-right"><span class="k-icon k-i-close" onClick="mainTab_Close(this, \'' + main_tab_guid + '\')"></span></div></span>',
        encoded: false,
        content: _content
    }]);
    tabstrip.data("kendoTabStrip").activateTab(tabstrip.find("ul li:last-child"));
    $(window).trigger('resize');
}

function modalWindow_ConfigOpen(sender, title, url_Action, page_guid, guids_out, target_param) {
    //initialize(arguments);
    vm_Base.initializeArguments(arguments);

    if (target_param == "ContextClean") {
        modalWindow_Close();
        modalWindow($(sender).text(), vm_Base.url_Action);
        return;
    }

    var Qgrid = $("#grid_" + page_guid);
    var grid = Qgrid.data("kendoGrid");

    var row = grid.select();
    if (row.length == 0)
        return;

    var SconfigId = grid.dataItem(row[0]).SconfigId;
    var SectionId = grid.dataItem(row[0]).SectionId;
    vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, target_param, 1);
    vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "parent_id", SconfigId);
    vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "SectionId", SectionId);

    mainTab_Open(/*$("#tabstrip")*/); //, title, vm_Base.url_Action, page_guid, guids_out);
    modalWindow_Close();
}


//ChildTabs*******************************************************************************************
function getChildEditTab()
{
    var tab_content_el = $("#EditTab");
    if (tab_content_el.length == 0) {
    }
}

function childTab_Open(sender, url_Action, title, soperation_id) { //sender, vm_Base_params, url_Action, title, soperation_id, add_after_preview
    try {
        //initialize(arguments, true);
        vm_Base.initializeArguments(arguments);

        /*Работа 6609. В некоторых случаях (напр. когда в редакторе есть грид) текущий page_guid слетает. Берем его из кнопки*/
        if (clickItem.sender != undefined && clickItem.sender.getAttribute("data_page_guid") != undefined && vm_Base.page_guid != clickItem.sender.getAttribute("data_page_guid"))
            vm_Base.page_guid = clickItem.sender.getAttribute("data_page_guid");

        if (clickItem.soperation_id == sop_12 && vm_Base.parent_id == 0) {
            messageWindow("Ошибка", "Основной объект еще не существует.");
            return;
        }

        if (Top().isAutoTestClient == true) {
            Top().set_Auto_Test_Curr_Item($(clickItem.sender).find("span.k-link")[0], 3);
        }

        row_id = getRowId_ByGuid(vm_Base.page_guid, vm_Base);
        if (row_id == "null")
            row_id == null;

        if (row_id == null) {
            if ($(clickItem.sender).attr("parent_id") != undefined) {
                row_id = $(clickItem.sender).attr("parent_id");
                parent_id = row_id;
            }
        }
        if (row_id == null && vm_Base.GuidChildTabLast != undefined)
            row_id = getRowId_ByGuid(vm_Base.GuidChildTabLast, vm_Base);

        if (row_id == null && isOperationsForRow(clickItem.soperation_id) == true) {
            messageWindow("Ошибка", "Выберите строку.");
            return null;
        }

        if (isMobile()) {
            vm_Base.tab = Top().$("#" + create_new_mobile_view(clickItem.title, this.parent.document.location.hash));//findInParent("#tabstrip");
            if (vm_Base.tab == undefined)
                return
        }
        else {
            vm_Base.tab = $("#MainTableTabChild_" + vm_Base.page_guid);
            if (vm_Base.tab.length == 0)
                vm_Base.tab = $("#EditTab");
            if (vm_Base.tab.length == 0)
                vm_Base.tab = $("#FilterTab");
            if (vm_Base.tab.length == 0 && is_true(vm_Base.save_in_main_tab))
                vm_Base.tab = window.parent.$("#tabstrip");
        }

        var parent_id = row_id;
        if (clickItem.soperation_id == 23)
            parent_id == undefined;

        clickItem.url_action = addToUrl(clickItem.url_action, "child_tab_open", 1);

        if (row_id != undefined) {
            var index_semicolon = row_id.toString().indexOf(";");
            if (index_semicolon >= 0) {
                parent_id = row_id.substring(0, index_semicolon);
                clickItem.url_action = addToUrl(clickItem.url_action, "parent_ids", row_id);
            }
            if (parent_id != undefined)
                clickItem.url_action = addToUrl(clickItem.url_action, "parent_id", parent_id);
        }

        clickItem.url_action = addToUrl(clickItem.url_action, "guids_out", vm_Base.guids_out);
        clickItem.url_action = addToUrl(clickItem.url_action, "title", clickItem.title);
        clickItem.url_action = addToUrl(clickItem.url_action, "debug", 0);

        var grid = $("#grid_" + vm_Base.page_guid);
        if (grid.attr("parent_id") != undefined && clickItem.soperation_id == 12) {
            clickItem.url_action = addToUrl(clickItem.url_action, "grand_parent_id", grid.attr("parent_id"));
            vm_Base.selectAfterAdd = true;
        }

        if (clickItem.sender != undefined && is_true(clickItem.sender.getAttribute("open_grandchild_grid"))) {
            if ($('#TabTableTabChild_' + vm_Base.page_guid).length > 0)
                vm_Base.tab = $('#TabTableTabChild_' + vm_Base.page_guid);
        }

        clickItem.url_action = mainGrid_Parameters(vm_Base, clickItem.url_action, clickItem.soperation_id);

        // !!! так можно передать в контроллер всю строку грида или дерева, на которой стояли при открытии таба
        //var kendoObject = "kendoGrid";
        //if (grid.data("role") == "treelist")
        //	kendoObject = "kendoTreeList";
        //var gridDataItem = grid.data(kendoObject).dataSource.get(row_id);
        //clickItem.url_action = addToUrl(clickItem.url_action, "gridDataItem", JSON.stringify(gridDataItem));

        return childTab_EditOpen();
    }
    catch (e) {
        if (Top().isAutoTestClient)
            Top().set_Auto_Test_Log(e);
        else
            throw e;
    }
}

function childTab_Open_Verify(sender, url_Action, title, soperation_id) { //sender, vm_Base_params, url_Action, title, soperation_id, add_after_preview
    //initialize(arguments);
    vm_Base.initializeArguments(arguments);

    var row_id = getRowId_ByGuid(vm_Base.page_guid);
    if (row_id == "null")
        row_id == null;

    if (row_id == null) {
        if ($(clickItem.sender).attr("parent_id") != undefined) {
            row_id = $(clickItem.sender).attr("parent_id");
            parent_id = row_id;
        }
    }

    if (row_id == null
        && clickItem.soperation_id != sop_2
        && clickItem.soperation_id != sop_12
        && clickItem.soperation_id != sop_25
    ) {
        messageWindow("Ошибка", "Выберите строку.");
        return null;
    }
    var parent_id = row_id;
    if (row_id != null) {
        var index_semicolon = row_id.indexOf(";");
        if (index_semicolon >= 0)
            parent_id = row_id.substring(0, index_semicolon);
    }

    var options = {};
    clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "parent_id", parent_id);
    clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "is_operation_rule", 1);
    //options.url = url;
    options.url = clickItem.url_action;
    options.type = "POST";

    options.success = function (data) {
        if (data.status === "error") {
            messageWindow("Ошибка", data.message, data.error_text);
        }
        else {
            clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "is_operation_rule", 0);
            childTab_Open(sender, /*vm_Base_params, */clickItem.url_action, clickItem.title/*, soperation_id, add_after_preview*/);
        }
    };

    $.ajax(options);
}

function childTab_EditOpen(url_Action, title, page_guid, guids_out, tab, soperation_id) {
    //initialize(arguments, true);
    if (clickItem.sender == undefined) {
        vm_Base.initializeArguments(arguments);
    }

    if (!isMobile()) {
        mainTableSplitter_LastPane_Collapse(vm_Base.page_guid, false);

        var kendoTabStrip = vm_Base.tab.data("kendoTabStrip");
        if (kendoTabStrip == null) {
            vm_Base.tab = window.parent.$(".childTabs.k-tabstrip-top");
            if (vm_Base.tab.length > 0) {
                vm_Base.mainTableTabChild_guid = vm_Base.tab.attr("id").replace("MainTableTabChild_", "");
                kendoTabStrip = vm_Base.tab.data("kendoTabStrip");
            }
        }
        if (kendoTabStrip == null)
            return;
    }

    var tab_guid = kendo.guid();
    if (clickItem.url_action.indexOf("TabEdit_Layout") > 0)
        var tab_guid_value = "ChildEditTab=" + tab_guid;
    else
        var tab_guid_value = "ChildTab=" + tab_guid;
    guids_out = vm_Base.guids_out;
    if (guids_out != undefined)
        guids_out += ";" + tab_guid_value;
    else
        guids_out = tab_guid_value;

    clickItem.url_action = addToUrl(clickItem.url_action, "guids_out", guids_out);

    if (isMobile())
        clickItem.url_action = addToUrl(clickItem.url_action, "is_mobile", isMobile());

    var uri_path = getUriParam_ByName(clickItem.url_action, "uri_path");
    var uri_path_last = getUriPath_FromStr_Last(uri_path);

    if (clickItem.soperation_id == undefined)
        clickItem.soperation_id = uri_path_last.SoperationId;

    //var _content = document.createElement('iframe');
    //_content.id = "ChildTabFrame_" + tab_guid;
    //_content.src = clickItem.url_action;
    //_content.dataset = Top().formData_new.toFormData();
    var _content =
        "<iframe " +
            " id='ChildTabFrame_" + tab_guid + "'" +
            " class='flexbox-item-full'" +
            " page_guid='" + tab_guid + "'" +
            " src='" + clickItem.url_action + "'" +
            " uri_path='" + uri_path + "'" +
            " is_child_tab='True'" +
            " soperation_id=" + clickItem.soperation_id +
            " stable_id=" + uri_path_last.StableId +
            " target_stable_id=" + uri_path_last.TargetStableId +
            " onload='onIframe_Load(this)'" +
        " ></iframe>";
        //var tab = vm_Base.tab;
        //$.ajax({
        //    url: clickItem.url_action,
        //    type: "POST",
        //    data: formData.toFormData(),
        //    contentType: false,
        //    processData: false,
        //    success: function (data) {
    if (!isMobile()) {
        var butt_close = '<span class="k-icon k-i-close" onClick="childTab_Close(\'' + (vm_Base.mainTableTabChild_guid != undefined ? vm_Base.mainTableTabChild_guid : vm_Base.page_guid) + '\',\'' + tab_guid + '\')"></span>';
        if ($(clickItem.sender).find("span.k-link").attr("mainTabEdit") != undefined)
            butt_close = '<span not_visible="true" class="k-icon k-i-close" onClick="childTab_Close(\'' + vm_Base.mainTableTabChild_guid + '\',\'' + tab_guid + '\')"></span>';

        kendoTabStrip.append([{
            text: "<span id='TabChild_" + tab_guid + "' ><div class='tab-title-left'>" + clickItem.title + '</div>' +
                '<div class="tab-title-right">' + butt_close + '</div></span>',
            encoded: false,
            //content:  _content// data
            content: "<div id='div_empty'" +
                        " class='div_empty' " +
                        " page_guid='" + tab_guid + "'" + 
                        " src='" + clickItem.url_action + "'" +
                        " uri_path='" + uri_path + "'" +
                        " soperation_id=" + clickItem.soperation_id +
                        " stable_id=" + uri_path_last.StableId +
                        " target_stable_id=" + uri_path_last.TargetStableId +
                        " tab_from_info_link='" + (clickItem.sender != undefined && clickItem.sender.className.indexOf("k-i-redo") != -1 ? 'True' : 'False') + "'" +
                        "></div>"
        }]);

        if (kendoTabStrip._events.select == undefined)
            kendoTabStrip.bind("select", childTab_Selected);
        if (kendoTabStrip._events.activate == undefined)
            kendoTabStrip.bind("activate", childTab_Selected);
        //if (vm_Base.tab.find('div.k-state-active').length == 0)
            kendoTabStrip.activateTab(vm_Base.tab.find("ul li:last-child"));

        vm_Base.tab_guid = tab_guid;
    } else {
        vm_Base.tab.append(_content);
        setTimeout(function () { Top().view_open(vm_Base.tab[0].id) }, 100);
    }

    //    }
    //});
    vm_Base.tab = undefined;
    return tab_guid;
}

function childTab_Selected(e) {

    if (e.sender.element.attr("page_guid") == undefined)
        return;

    var tab = $('#MainTableTabChild_' + e.sender.element.attr("page_guid"));
    if (tab.length == 0)
        tab = $('#TabTableTabChild_' + e.sender.element.attr("page_guid"));
    if (tab.length == 0)
        tab = $('#TabTableTabChild_' + vm_Base.page_guid);

    if (tab.length > 0) {
        var tab_content = tab.find('div.k-state-active');
        if (tab_content.length > 0 && tab_content.find('iframe').length == 0) {
            setTimeout(function () {
                var div_empty = tab_content.find('#div_empty');

                var content = "<iframe " +
                    " id='ChildTabFrame_" + $(div_empty).attr('page_guid') + "'" +
                    " page_guid='" + $(div_empty).attr('page_guid') + "'" +
                    " class ='flexbox-item-full'" +
                    " src='" + $(div_empty).attr('src') + "'" +
                    " uri_path='" + $(div_empty).attr('uri_path') + "'" +
                    " is_child_tab='True'" +
                    " soperation_id=" + $(div_empty).attr('soperation_id') +
                    " stable_id=" + $(div_empty).attr('stable_id') +
                    " target_stable_id=" + $(div_empty).attr('target_stable_id') +
                    " onload='onIframe_Load(this)'" +
                    " tab_from_info_link='" + $(div_empty).attr('tab_from_info_link') + "'" +
                " ></iframe>";

                tab_content.find('#div_empty').remove();
                tab.find('div.k-state-active').append(content);
            }, 200);
        }
        else {
            /*Работа 6044. Дочерний список обновляем только в момент активации вкладки*/
            grid_Change(vm_Base.page_guid, true);
        }
    }
}

function childTab_Close(mainTableTabChild_guid, tabChild_guid) {
    try {
        var tab = $("#MainTableTabChild_" + mainTableTabChild_guid);
        if (tab.length == 0) {
            tab = window.$(".childTabs.k-tabstrip-top");
            if (isMobile && tab.length == 0)
                tab = window.$(".k-tabstrip-top");
            if (tab.length != 0 && tab.attr("page_guid") != undefined)
                mainTableTabChild_guid = tab.attr("page_guid");//tab.attr("id").replace("MainTableTabChild_", "");
        }
        var ul_li = tab.find("ul").find("#TabChild_" + tabChild_guid).closest(".k-item");

        /*Убираем обработчки для фрейма. Из-за этого происходит серьезная утечка памяти*/
        onIframe_Event_Remove($('*').find('#ChildTabFrame_' + tabChild_guid));
        tabStrip_RemoveTab(tab, "this", ul_li);
        tabStrip_ActivateLastTab(tab)

        //Если кроме текущей, нет других вкладок, двигаем сплиттер
        if (tab.find("li.k-item[role='tab']").length == 0) {
            mainTableSplitter_LastPane_Collapse(mainTableTabChild_guid, true);
        }

        //if (Top().isAutoTestNew) {
        //    if ($('iframe').length == 0)
        //        exec_Custom_Event("FilterTab", "autoTestNextStep");
        //}
    }
    catch (e) {
        if (Top().isAutoTestClient)
            Top().set_Auto_Test_Log(e);
            //set_AutoTest_Error(e);
        else
            throw e;
    }
}

/*Вызывается при закрытии фильтра программно. Убраны лишние анимации для табов. */
function childTab_Close_From_Filter(mainTableTabChild_guid, tabChild_guid) {

    var tab = $("#MainTableTabChild_" + mainTableTabChild_guid);
    var ul_li = tab.find("ul").find("#TabChild_" + tabChild_guid).closest(".k-item");

    if (ul_li.length > 0)
        tab.data("kendoTabStrip").remove(ul_li);

    $("#" + openedTabs[tabChild_guid]).find(".fa-check-square-o").remove();
    delete openedTabs[tabChild_guid];

    if (frames.length == 0) {
        mainTableSplitter_LastPane_Collapse(mainTableTabChild_guid, true);
    }
}

function tabChildGrid_AddEdit_Click(page_guid, page_guid_grid
    //, sconfig_id, stable_id
    , operation_id
    , title, Parent_StableId, parent_id) {
    //Проверка доступности у кнопок редактирования грида в редакторе
    //messageWindow("", "", $('*').html());
    if (is_true($("div.formAreaEdit").find("div.k-toolbar > a.k-button").attr("not_clickable")))
        return false;

    var tab_edit = $("#TabEditContent_" + page_guid);
    var data_param = "/Home/TabEdit?=";

    //data_param = updateQueryStringParameter(data_param, "soperation_id", operation_id);

    var row_id = getRowId_ByGuid(page_guid_grid);
    if (operation_id == 3)
        data_param = updateQueryStringParameter(data_param, "parent_id", row_id);
    if (operation_id == 13)
        data_param = updateQueryStringParameter(data_param, "parent_id", parent_id);

    //data_param = updateQueryStringParameter(data_param, "IsChildEditor", true);

    var tab_content_el = $("#EditTab");
    var current = $("#EditTab > ul > li.k-state-active");
    tab_content_el.data("kendoTabStrip").enable(current, false);
    tab_content_el.data("kendoTabStrip").append(
        {
            text: title + "  <i class=\"fa fa-angle-left\" aria-hidden=\"true\"></i>",
            encoded: false,
            contentUrl: data_param
        });

    tab_content_el.data("kendoTabStrip").activateTab($("#EditTab > ul > li.k-last"));

    return false;
}

function tabChildGrid_Del_Click(url, width, height, page_guid) {
    if (is_true($("div.formAreaEdit").find("div.k-toolbar > a.k-button").attr("not_clickable")))
        return false;
    //var options = {};
    //options.url = url;

    var row_id = getRowId_ByGuid(page_guid);
    url = updateQueryStringParameter(url, "parent_id", row_id);
    //options.type = "POST";

    //$.ajax(options);
    modalWindow("Удаление", url, width, height, undefined, page_guid);
}

//***********************************************************************************
function TabTree_ExpandAll(page_guid) {
    var tabtree = $("#TabTree_" + page_guid).data("kendoTreeView");
    var buttExpAll = $("#TabTree_ButtonExpAll_" + page_guid);
    var buttExpAll_img = buttExpAll.find("img");

    if (!is_true(buttExpAll.attr("expand-all"))) {
        buttExpAll_img.attr("src", "/Images/Operations/Large/Expander_Open.png");
        buttExpAll.attr("expand-all", "true");
        tabtree.expand(".k-item");
    }
    else {
        buttExpAll_img.attr("src", "/Images/Operations/Large/Expander_Close.png");
        buttExpAll.attr("expand-all", "false");
        tabtree.collapse(".k-item");
    }
}

function TabTree_NodeEdit(url, parent_guid, page_guid, guids_out, soperation_id) {

    var tabtree = $("#TabTree_" + page_guid);
    var item = tabtree.find("li[aria-selected=true]");
    var selected = tabtree.data("kendoTreeView").select();
    var text = item.attr("type-name");//$("#TabTree_" + page_guid).data("kendoTreeView").dataItem(selected).text;

    if (soperation_id == sop_3) {
        url = updateQueryStringParameter(url, "StableTreeId", item.attr("stable-id"));
        url = updateQueryStringParameter(url, "parent_id", item.attr("data-id"));
        var tab = parent.$("#MainTableTabChild_" + parent_guid);
        childTab_EditOpen(url, "Редактор-Up", parent_guid, guids_out, tab)
    }
}

function TabTree_to_(tabstrip, vm_Base_params, new_guid, soperation_id) {
    //initialize(arguments);
    vm_Base.initializeArguments(arguments);

    var tabtree = $("#TabTree_" + vm_Base.page_guid);
    var item = tabtree.find("li[aria-selected=true]");
    var selected = tabtree.data("kendoTreeView").select();
    vm_Base.title = item.attr("type-name");//$("#TabTree_" + vm_Base.page_guid).data("kendoTreeView").dataItem(selected).text;

    if (soperation_id == sop_25) {
        vm_Base.url_Action = "/Home/MainTable?=";
        vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "sconfig_id", "0");
        vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "stable_id", item.attr("stable-id"));
        vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "soperation_id", soperation_id);
        vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "nom", "0");
        vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "page_guid", new_guid);
        vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "parent_id", item.attr("data-id"));//.split('-')[1]);
        vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "_isPostBack", true);
        vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "_iframe", 1);

        mainTab_Open(/*tabstrip*/); //, text, vm_Base.url_Action, vm_Base.page_guid, vm_Base.guids_out);
    }

    if (soperation_id == sop_3) {
        vm_Base.url_Action = "/Home/TabEdit_Layout?=";
        //var data_param = "/Home/TabEdit?=";
        vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "sconfig_id", "0");
        vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "stable_id", item.attr("stable-id"));
        vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "soperation_id", soperation_id);
        vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "nom", "0");
        vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "page_guid", new_guid);
        vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "parent_id", item.attr("data-id"));

        vm_Base.tab = parent.$("#MainTableTabChild_" + vm_Base.parent_guid);
        vm_Base.title = "Редактор-Up";
        childTab_EditOpen(); //vm_Base.url_Action, vm_Base.title, vm_Base.parent_guid, vm_Base.guids_out, vm_Base.tab
    }
}

//***********************************************************************************
function filterTab_Close(page_guid, isSelectorFromFilter) {
    if (clickItem.is_selector_from_filter == undefined)
        clickItem.is_selector_from_filter = isSelectorFromFilter

    var filterTab = $("#FilterTab");
    if (!is_true(clickItem.is_selector_from_filter) && filterTab.length == 0) {
        filterTab = $("#EditTab");
        if (filterTab.length == 0)
            filterTab = $("#EditTabMain");
    }

    var filterTab_TabStrip = filterTab.data("kendoTabStrip");

    if (filterTab.length > 0 && filterTab_TabStrip != null) {

        var grid = $("#period_values_grid_" + clickItem.page_guid);
        if (grid.length > 0) {
            var data = grid.data("kendoGrid").dataSource.data();
            if (data.length > 0) {
                var value = data[data.length - 1].Value;
                var period = data[data.length - 1].PeriodDate;

                var text = "";
                var text2 = "";
                if (data[data.length - 1].PeriodType == "Selector") {
                    text = data[data.length - 1].Name;
                    text2 = data[data.length - 1].Value2;
                }

                if (data[data.length - 1].PeriodType == "ComboBox")
                    text = data[data.length - 1].ComboItemText;

                if (data[data.length - 1].PeriodType == "TextBox")
                    text = data[data.length - 1].Value;

                if (data[data.length - 1].PeriodType == "Masked")
                    text = data[data.length - 1].Value;

                if (data[data.length - 1].PeriodType == "DatePicker") {
                    value = data[data.length - 1].ValueDStr;
                    text = data[data.length - 1].ValueDStr;
                }
            }
        }

        var old = filterTab_TabStrip.select();

        if (old.prev().length > 0) {
            filterTab_TabStrip.enable(old.prev(), true);
            filterTab_TabStrip.activateTab(old.prev());
            filterTab_TabStrip.remove(old);

            $(window).trigger('resize');

            $("div.formAreaEdit").find("div.k-toolbar > a.k-button").removeAttr("not_clickable");
            $("div.formAreaEdit").find("div.k-toolbar > a.k-button[operation=add]").find("img").attr("src", "/Images/Operations/Small/Add_Blue.png");
            $("div.formAreaEdit").find("div.k-toolbar > a.k-button[operation=del]").find("img").attr("src", "/Images/Operations/Small/Del_Blue.png");
            $("div.formAreaEdit").find("div.k-toolbar > a.k-button[operation=edit]").find("img").attr("src", "/Images/Operations/Small/Edit_Blue.png");
        }

        /*В заголовке едитора отображаем последнюю строку из таблицы*/
        if (value != null) {
            $("#caption_" + clickItem.page_guid).attr("value", value);
            if ($("#caption_" + clickItem.page_guid).length > 0) {
                $("#caption_" + clickItem.page_guid)[0].value = text;
            }
            if ($("#captionRight_" + clickItem.page_guid).length > 0) {
                $("#captionRight_" + clickItem.page_guid).attr("value", text2);
                $("#captionRight_" + clickItem.page_guid)[0].value = text2;
            }
            $("#val_" + clickItem.page_guid).attr("value", value);
            /*Обновление контролов быстрого редактирования*/
            if ($("#val2_" + clickItem.page_guid).length > 0) {
                $("#val2_" + clickItem.page_guid).attr("value", value);
                $("#val2_" + clickItem.page_guid)[0].value = value;

                if ($("#val2_" + clickItem.page_guid).data("kendoComboBox") != undefined)
                    $("#val2_" + clickItem.page_guid).data("kendoComboBox").value(value);
                if ($("#val2_" + clickItem.page_guid).data("kendoDropDownList") != undefined)
                    $("#val2_" + clickItem.page_guid).data("kendoDropDownList").value(value);
                if ($("#val2_" + clickItem.page_guid).data("kendoNumericTextBox") != undefined)
                    $("#val2_" + clickItem.page_guid).data("kendoNumericTextBox").value(value);
            }

            $("#label_period_" + clickItem.page_guid).text(period);
            $("#search_" + clickItem.page_guid).attr("data-parent_id", value);
        }
    }

    mainTableFilterConfigs.pop(clickItem.page_guid);
    $('#MainFilterTable_Filter_' + clickItem.page_guid).remove();
    $('#MainFilterTable_Config_' + clickItem.page_guid).remove();
    var div_filter = $('.filter-panel');
        div_filter.css("display", "block");

    return false;
}

//***********************************************************************************
function link_Click(sender, page_guid, url_Content, url_Action, title) {
    //initialize(arguments);
    vm_Base.initializeArguments(arguments);

    if ($("#tabstrip").length == 0)
        mainTab_Open(/*Top().$("#tabstrip")*/); //, undefined, title, url_Action);
    return false;
}

function link_mainTab_Open(sender, vm_Base_params, url_Action, page_guid, title, editor_in_main_tab) {
    //initialize(arguments);
    vm_Base.initializeArguments(arguments);

    if (is_true(vm_Base.is_Lik))
        vm_Base.mainTab_IsAutoLoaded = "False";

    var row_id = getRowId_ByGuid(vm_Base.page_guid);
    if (row_id != null)
        vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "parent_id", row_id);

    var link_disabled = $(sender).attr("link_disabled");
    if (link_disabled != undefined && link_disabled == '1')
        return;

    mainTab_Open(/*Top().$("#tabstrip")*/);
    //childTab_Open(sender, vm_Base_params, url_Action, title)
}

function link_Execute(sender, vm_Base_params, url_Action, page_guid, browser_tab, direct_link, is_eval) {
    //initialize(arguments, true);
    vm_Base.initializeArguments(arguments);

    var link_disabled = $(clickItem.sender).attr("link_disabled");
    if (link_disabled != undefined && link_disabled == '1')
        return;

    /*Работа 7127. Проверка на пустую внешнюю ссылку*/
    if ((is_true(clickItem.direct_link) || clickItem.direct_link) && (clickItem.url_action == "" || clickItem.url_action == undefined)) {
        var message = "";
        if (clickItem.title != "" && clickItem.title != undefined)
            message = "Поле [" + clickItem.title + "] у ссылки не заполнено";
        else
            message = "Ссылка пустая";
        messageWindow("Ошибка", message);
        return false;
    }

    let group = undefined;
    if ($("#val_" + clickItem.page_guid).attr("group") != undefined)
        group = $("#val_" + clickItem.page_guid).attr("group");
    /*Работа 6446*/
    if (vm_Base.clientController == "ModalWindow")
        formData.readInputs("myModalForm", group);
    else
        formData.readInputs("myForm", group);
    if (!is_true(clickItem.direct_link))
        clickItem.url_action = formData.toUrl(clickItem.url_action);
    /**/

    if (is_true(clickItem.direct_link)) {
	    Top().window.open(clickItem.url_action);
		return false;
	}

	if (vm_Base.clientController == "ModalWindow")
	    Top().modWindow.close();

	let row_id = $(clickItem.sender).attr("row_id");
	if (row_id != undefined && row_id > 0)
	    clickItem.url_action = addToUrl(clickItem.url_action, "id", row_id);

	let report_id = $(clickItem.sender).attr("report_id");
    if (report_id != undefined && report_id > 0)
        clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "id", report_id);

    var parent_id = getRowId_ByGuid(clickItem.page_guid);
    if (parent_id != undefined)
        formData.append("parent_id", parent_id);
    else
        parent_id = getRowId_ByGuid_FromFrames(clickItem.page_guid, vm_Base.clientController_Prev);
    if (parent_id != undefined)
        formData.append("parent_id", parent_id);
    
    //Осинных. Переход по ссылке. Сначала вызывается PrepareDownloadFile (редко другой контроллер). Если он не возвращает ошибку,
    //то за ним вызывается DownloadFile, который и сохраняет файл, либо прямой переход по ссылке
    $.ajax({
        url: clickItem.url_action,
        type: "POST",
        data: formData.formDataObject,//formData.toFormData(),
        contentType: false,
        processData: false,
        success: function (data) {
            if (data.status === "error") {
                messageWindow("Ошибка", data.message, data.error_text, undefined, 450);
            }
            if (data.status === "success") {
                if (data.url != undefined)
                    Top().window.open(data.url);
                else
                    Top().window.open(clickItem.url_action.replace("Prepare", ""));
            }
            if (data.status === "file_download" && data.file != undefined) {
                var sampleArr = base64ToArrayBuffer(data.file);
                blob = new Blob([sampleArr], { type: data.file_type });
                if (blob) {
                    if (navigator.msSaveOrOpenBlob) {
                        navigator.msSaveBlob(blob, data.file_name);
                    } else {
                        var url = window.URL.createObjectURL(blob);
                        a = document.createElement("a");
                        document.body.appendChild(a);
                        a.style = "display: none";
                        a.href = url;
                        a.download = data.file_name;
                        a.click();
                        window.URL.revokeObjectURL(url);
                    }
                }
            }
        }
    });
}

function base64ToArrayBuffer(base64) {
    var binaryString = window.atob(base64);
    var binaryLen = binaryString.length;
    var bytes = new Uint8Array(binaryLen);
    for (var i = 0; i < binaryLen; i++) {
        var ascii = binaryString.charCodeAt(i);
        bytes[i] = ascii;
    }
    return bytes;
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

// **********************************************************************************
function mainMenu_Click(sender, vm_Base_params, url_Action, main_table_tab_guid, title, url_Content, guids_out) { //, is_onIframe_Load, menu_inline
    try {
        clickItem.fromMainMenu = true;
        vm_Base.initializeArguments(arguments);
        //initialize(arguments, true);

        //if ($(sender).attr("autotesting") != undefined) {
        //    initialize_AutoTest();
        //    clickItems = get_Click_Items(1);
        //    countClickItems = clickItems.length;
        //    return;
        //}
        //if (Top().isAutoTestNew) {
        //    set_AutoTest_CurrClickItem($(sender).find("span.k-link")[0]);
        //    currClickItem = clickItems[currClickItemNom];
        //}

        if (Top().isAutoTestClient == true) {
            Top().set_Auto_Test_Items(1);
            Top().set_Auto_Test_Curr_Item($(sender).find("span.k-link")[0], 1);
        }

        if ((vm_Base.url_Action.indexOf("ModalWindow") > 0 && !isMobile()) || vm_Base.menu_inline) {
        }
        else
            hideMainMenu();
        //$("#left-pane").data("kendoResponsivePanel").close();

        /*При переходе к соответствующему списку, убираем значок новых элементов*/
        if (sender.parentElement.attributes["new_items_pic_type"] != null)
            if (sender.parentElement.attributes["need_new_items_pic"] != null > 0 && sender.parentElement.attributes["need_new_items_pic"].value == 'False') {
                $("li[new_items_pic_type='" + sender.parentElement.attributes["new_items_pic_type"].value + "']").attr('need_new_items_pic', 'True');
                $("li[new_items_pic_type='" + sender.parentElement.attributes["new_items_pic_type"].value + "'] > a > img.image-new-items").remove();
                $("li[new_items_pic_type='" + sender.parentElement.attributes["new_items_pic_type"].value + "'] > span > img.image-new-items").remove();
            }

        $("div.k-animation-container").css("display", "none");

        if (vm_Base.title == undefined)
            vm_Base.title = $(sender).text();
        if (vm_Base.url_Action.indexOf("ModalWindow") > 0 && !isMobile()) {
        
            modalWindow(vm_Base.title, vm_Base.url_Action);
        } else {
            /*Убираем из текста нажатого меню количество непрочитанных сообщений, чтобы оно не попало в заголовок*/
            if ($(sender).find("b.count-new-items").length > 0)
                $(sender).find("b.count-new-items").remove();

            vm_Base.title = $(sender).text();
            //if (_title != undefined)
            //    title = _title;

            var tabstrip = "tabstrip";

            if (isMobile()) {
                tabstrip = create_new_mobile_view(vm_Base.title);
                if (tabstrip == undefined)
                    return
                //if ($("#mobile_app").length > 0) {
                //    var view_count = $("#mobile_app div[data-role='view']").length;
                //    tabstrip = "mobile_view_main" + view_count;
                //    var new_view_html = "<div id=\"" + tabstrip + "\" data-role=\"view\" style=\"display: none;\" data-transition=\"slide\" data-layout=\"mobile_layout\">";
                //    $("#mobile_app").append(new_view_html + "</div>");
                //}
            }

            vm_Base.mainTab_IsAutoLoaded = undefined;
            if (is_true($(sender).attr("is_default_page")))
                vm_Base.mainTab_IsAutoLoaded = "True";

            vm_Base.url_Action = addToUrl(vm_Base.url_Action, "_iframe", 1);

            //if (isMobile() && tabstrip != "tabstrip") {
            //    $("#inboxActions > li.km-actionsheet-cancel").remove();
            //    $("#inboxActions").append("<li><span id=\"" + view_count + "\" class=\"k-link\" onclick=\"view_open('" + tabstrip + "');\">" + vm_Base.title + "</span></li>");
            //    $("#inboxActions").append("<li class=\"km-actionsheet-cancel\"><a href=\"#\">Cancel</a></li>");
            //}

            mainTab_Open(); //$("#" + tabstrip), vm_Base.title, vm_Base.url_Action, vm_Base.main_table_tab_guid, guids_out, is_onIframe_Load, undefined, vm_Base.mainTab_IsAutoLoaded);
        }
    }
    catch(e) {
        if (Top().isAutoTestClient)
            Top().set_Auto_Test_Log(e);
        //set_AutoTest_Error(e);
    else
        throw e;
    }
}

function mainMenu_Click_Verify(sender, vm_Base_params, url_Action, main_table_tab_guid, title) { //guid_out, guids_out, is_onIframe_Load, menu_inline
    //initialize(arguments);
    vm_Base.initializeArguments(arguments);

    var options = {};
    vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "is_operation_rule", 1);
    options.url = vm_Base.url_Action;
    options.type = "POST";

    options.success = function (data) {
        if (data.status == "error") {
            messageWindow("Ошибка", data.message, data.error_text);
        }
        else {
            mainMenu_Click(sender); //, vm_Base.url_Action, guid_out, guids_out, is_onIframe_Load, title, menu_inline);
        }
    };
    $.ajax(options);
}

//***********************************************************************************
function kendoContextMenu_Exec(page_guid) {
    var contextMenu = $("#contextMenuGrid_" + page_guid);
    var grid = $("#grid_" + page_guid);
    var sss1 = 1;

    contextMenu.kendoContextMenu({
        target: "#grid_" + page_guid,
        filter: ".grid-cell-btn",
        showOn: "click",
        select: function (e) {

            var controller_name = "Home/TabEdit_Layout/";
            //var controller_name = "Home/TabEdit/";
            if ($(e.item).data("list") == "list") {
                controller_name = "Home/MainTable/";
            }

            var cell_target = $(e.target);

            var url = uri_Content + controller_name;

            url = addToUrl(url, "_isPostBack", true);
            url = addToUrl(url, "page_guid", page_guid);
            url = addToUrl(url, "fk_field", cell_target.data("fkfield"));
            url = addToUrl(url, "parent_id", cell_target.closest("td").prev().text());

            var title = cell_target.data("tabtitle");

            if ($(e.item).data("list") == "list") {

                url = addToUrl(url, "is_main", is_main);
                url = addToUrl(url, "_iframe", 1);
                url = addToUrl(url, "context_menu_operation", sop_15);  // ListItem

                mainTab_Open(/*findInParent("#tabstrip"), undefined, */title, url/*, page_guid*/);

            } else {
                url = addToUrl(url, "context_menu_operation", sop_3);   // EditItem

                var tab = $("#MainTableTabChild_" + page_guid);
                return childTab_EditOpen(url, "Редактор-Up", page_guid, null, tab);
            }
        }
    });
}

function contextMenuSelect(page_guid, guids_out, uri_Content, e) {
    var controller_name = "Home/TabEdit_Layout/";
    //var controller_name = "Home/TabEdit/";
    if ($(e.item).data("list") == "list") {
        controller_name = "Home/MainTable/";
    }

    var cell_target = $(e.target);

    var url = uri_Content + controller_name;

    url = addToUrl(url, "_isPostBack", true);
    url = addToUrl(url, "page_guid", cell_target.data("page_guid"));
    url = addToUrl(url, "guids_out", guids_out);
    url = addToUrl(url, "grid_cell_content", cell_target.data("grid_cell_content"));
    url = addToUrl(url, "fk_field", cell_target.data("fk_field"));
    url = addToUrl(url, "uri_path", cell_target.data("uri_path"));
    url = addToUrl(url, "parent_id", cell_target.data("parent_id"));

    var title = cell_target.data("title");

    if ($(e.item).data("list") == "list") {

        url = addToUrl(url, "is_main", cell_target.data("is_main"));
        url = addToUrl(url, "_iframe", 1);
        url = addToUrl(url, "context_menu_operation", sop_15);  // ListItem

        mainTab_Open(/*findInParent("#tabstrip"), undefined, */title, url/*, page_guid, guids_out*/);

    } else {
        url = addToUrl(url, "context_menu_operation", sop_3);   // EditItem

        var tab = $("#MainTableTabChild_" + page_guid);

        return childTab_EditOpen(url, title, page_guid, guids_out, tab);
    }
}

// **********************************************************************************
function onSeriesClick(e/*, page_guid*/) {
    var chart = e.sender.element;//$("#chart");
    if (chart.length == 0) {
        messageWindow("Ошибка", "Ошибка привязки графика");
        return;
    }

    var url = chart.attr("url");
    var page_guid = chart.attr("page_guid");
    var guids_out = chart.attr("guids_out");
    url = updateQueryStringParameter(url, "uri_path", chart.attr("uri_path"));
    url = updateQueryStringParameter(url, "guids_out", guids_out);
    url = updateQueryStringParameter(url, "parent_id", chart.attr("parent_id"));

    if (e.series.name != null) {
        if (e.series.name == 'Оплачено')
            url = updateQueryStringParameter(url, "chart_name", "credit");
        else {
            if (e.series.name == 'Начислено')
                url = updateQueryStringParameter(url, "chart_name", "debet");
            else
                return;
        }
    }

    if (e.category != null)
        url = updateQueryStringParameter(url, "chart_category", e.category);

    url = updateQueryStringParameter(url, "chart_request", "true");

    mainTab_Open(/*findInParent("#tabstrip"), undefined, */e.series.name, url/*, page_guid, guids_out*/);
}

function get_TabTitle_From_Mask(mask) {
    let str = mask;
    let str_out = str;
    let field;
    let field_value;

    while (str_out.indexOf('[') != -1) {
        field = str_out.substring(mask.indexOf('[') + 1, mask.indexOf(']'));

        if (clickItem.new_item != undefined)
            field_value = JSON.parse(clickItem.new_item)[field];
        else   
            field_value = get_Grid_ColumnValue(field);

        if (field_value != undefined)
            str_out = str_out.replace("[" + field + "]", field_value);
        else break;
    }
    if (str != str_out)
        return str_out;

    if (clickItem.maintable_titlefield != undefined) {
        field_value = get_Grid_ColumnValue(clickItem.maintable_titlefield);
        if (field_value != undefined)
            return field_value;
    }

    field_value = get_Grid_ColumnValue("id");
    if (field_value != undefined)
        return field_value;

    return undefined;
}