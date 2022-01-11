// entity ChangeStatus / Save *******************************************************************************************
var entity_status = 1;

function entity_Save(vm_Base_params, url_Action, url_Content) {
    if (url_Action != undefined) {
        vm_Base.url_Action = url_Action;
        clickItem.url_action = url_Action;
    }

    if (is_true(clickItem.add_from_selector))
        formData.readInputs("myForm", "add_from_selector");
    else
        formData.readInputs("myForm");

    if (entity_status <= 0)
        return;

    var $buttSave = $("#val_" + vm_Base.page_guid);
    if ($buttSave.hasClass("k-state-disabled"))
        return;

    /*Проверка. Если нажимаем кнопку "Сохранить" не на последней вкладке визарда, то выводится сообщение об ошибке*/
    if (is_true($("#WizardTab_" + clickItem.page_guid).attr("is-wizard")) && !is_true(clickItem.is_toolbar_control)) {
        if ($("#WizardTab_" + clickItem.page_guid + " li.k-last.k-state-active" + "[role='tab']").length == 0) {
            messageWindow("Ошибка", "Сохранение возможно только на последней вкладке!");
            return;
        }
    }
    button_SetReadOnly($buttSave, true);

    if (formData.errorMessage != undefined) {
        messageWindow("Ошибка", formData.errorMessage);
        button_SetReadOnly($buttSave, false);
        return;
    }

    if (clickItem.grid_selected_rows > 0) {
        // 1 - Выделенные строки
        // 2 - Весь список
        // 3 - Оба варианта (явно в коде)
        formData.append("grid_selected_rows", clickItem.grid_selected_rows);
        if (clickItem.grid_selected_rows == 1 || clickItem.grid_selected_rows == 3) {
            if (!isPageGuidEmpty(vm_Base.GuidChildTabLast))
                formData.append("parent_ids", parent.getRowId_ByGuid(vm_Base.GuidChildTabLast, vm_Base, clickItem.grid_selected_rows));
            else {
                formData.append("parent_ids", parent.getRowId_ByGuid(clickItem.parent_guid, vm_Base, clickItem.grid_selected_rows));
            }
        }

        if (clickItem.grid_selected_rows == 2 || clickItem.grid_selected_rows == 3) {
            var grid_filter_string = parent.$("#grid_" + clickItem.parent_guid);
            if (grid_filter_string.length > 0)
                formData.append("filter_string_encode", grid_filter_string.attr("filter_string"));
        }
    }

    if (is_true(clickItem.this_grid_selected_row)) {
        if ($("div[grid_stable_id=\"" + clickItem.refresh_grid + "\"]").length > 0) {
            let curr_grid = $("div[grid_stable_id=\"" + clickItem.refresh_grid + "\"]");
            let grid_guid = curr_grid[0].getAttribute("page_guid");
            if (grid_guid != null)
                formData.append("parent_id", getRowId_ByGuid(grid_guid));
        }
    }

    // Гасим "дискету" на время обработки
    entity_ChangeStatus("entity_Save-1", vm_Base.url_Content, vm_Base.page_guid, -10);

    if ($buttSave.length > 0) {
        if ($buttSave.attr("is_dealinfo_refresh") != undefined && is_true($buttSave.attr("is_dealinfo_refresh"))) {
            formData.append("is_dealinfo_refresh", true);
        }
    }

    $.ajax({
        url: clickItem.url_action,
        type: "POST",
        data: formData.formDataObject,
        cache: false,
        contentType: false,
        processData: false,
        success: function (data) {
            entity_Save_OnCompleted(data);
        }
    });

    return;
}

function entity_ChangeStatus(sender, url_Content, page_guid, is_initialize) {
    //is_initialize 0-отключено, 1-включено, -10-отключена "дискета", 10-включена "дискета

    if (clickItem.change_status_forbidden == 1)
        return;

    var buttSave = $("#buttSave_" + clickItem.page_guid);
    if ((sender == "entity_Save-1" || sender == "entity_Save_OnCompleted-2")
        && buttSave.hasClass("k-state-disabled"))
        return;

    var buttSave_img = buttSave.find("img");
    var button_save_hint = buttSave.attr("button_save_hint");

    var buttEdit = $("#buttChangeStatus_" + clickItem.page_guid);
    var buttEdit_img = buttEdit.find("img");
    var button_edit_hint = buttEdit.attr("button_edit_hint");
    var button_edit_hint2 = buttEdit.attr("button_edit_hint2");

    if (is_initialize == -10) {
        buttEdit_img.attr("src", vm_Base.url_Content + "Images/Operations/Large/Edit_Red_Disable.png");
        buttSave_img.attr("src", vm_Base.url_Content + "Images/Operations/Large/Save_Red_Disable.png");
        buttSave_img.removeAttr("title");

        entity_status = -1;
        buttSave.removeAttr("data-save");
        return;
    }
    if (is_initialize == 10) {
        buttSave_img.attr("src", vm_Base.url_Content + "Images/Operations/Large/Save_Red.png");
        buttSave_img.attr("title", button_save_hint);

        entity_status = 1;
        buttSave.attr("data-save", 1);
        return;
    }

    let border = document.getElementById("border");
    if (border == null)
        return;

    if (buttSave.attr("data-save") == 1)
        entity_status = 1;
    else
        entity_status = 0;

    let div_formAreaEdit = $("div.formAreaEdit");
    if (is_initialize != undefined && is_initialize)
        entity_status = 1;

    if (entity_status == 0) {
        entity_status = 1;

        buttSave.removeClass("k-state-disabled")

        $(border).addClass("is-red-bordered")
        $(border).removeClass("is-silver-bordered")

        buttEdit_img.attr("src", vm_Base.url_Content + "Images/Operations/Large/Edit_Red.png");
        buttSave_img.attr("src", vm_Base.url_Content + "Images/Operations/Large/Save_Red.png");

        buttEdit_img.attr("title", button_edit_hint2);
        buttSave_img.attr("title", button_save_hint);

        buttSave.attr("data-save", 1);

        $.each($("input, textarea"), function () {
            if ($(this).attr("readonly") != undefined && $(this).attr("readonly") == "readonly") {
            }
            else {
                $(this).removeAttr("disabled");
                if ($(this).data("kendoMaskedTextBox") != undefined)
                    $(this).data("kendoMaskedTextBox").enable(true);
            }
        });

        //Доступность кнопок грида в редакторе. Со свойством "disabled" почему-то не работало
        div_formAreaEdit.find("div.k-toolbar > a.k-button").removeAttr("not_clickable");

        //Изменение цвета у всех кнопок редактирования гридов
        div_formAreaEdit.find("div.k-toolbar > a.k-button[operation=add]").find("img").attr("src", vm_Base.url_Content + "Images/Operations/Small/Add_Blue.png");
        div_formAreaEdit.find("div.k-toolbar > a.k-button[operation=del]").find("img").attr("src", vm_Base.url_Content + "Images/Operations/Small/Del_Blue.png");
        div_formAreaEdit.find("div.k-toolbar > a.k-button[operation=edit]").find("img").attr("src", vm_Base.url_Content + "Images/Operations/Small/Edit_Blue.png");
    }
    else {
        entity_status = 0;

        buttSave.addClass("k-state-disabled")

        $(border).addClass("is-silver-bordered")
        $(border).removeClass("is-red-bordered");

        buttEdit_img.attr("src", vm_Base.url_Content + "Images/Operations/Large/Edit_Blue.png");
        buttSave_img.attr("src", vm_Base.url_Content + "Images/Operations/Large/Save_Blue_Disable.png");

        buttEdit_img.attr("title", button_edit_hint);
        buttSave_img.removeAttr("title");

        buttSave.removeAttr("data-save");

        $.each($("input, textarea"), function () {
            if ($(this).attr("readonly") != undefined && $(this).attr("readonly") == "readonly") {
            }
            else {
                $(this).attr("disabled", "disabled");
            }
        });

        div_formAreaEdit.find("div.k-toolbar > a.k-button").attr("not_clickable", "true");
        div_formAreaEdit.find("div.k-toolbar > a.k-button[operation=add]").find("img").attr("src", vm_Base.url_Content + "Images/Operations/Small/Add_Blue_Disabled.png");
        div_formAreaEdit.find("div.k-toolbar > a.k-button[operation=del]").find("img").attr("src", vm_Base.url_Content + "Images/Operations/Small/Del_Blue_Disabled.png");
        div_formAreaEdit.find("div.k-toolbar > a.k-button[operation=edit]").find("img").attr("src", vm_Base.url_Content + "Images/Operations/Small/Edit_Blue_Disabled.png");
    }
}

function entity_Save_OnCompleted(data) {
    button_SetReadOnly("#val_" + vm_Base.page_guid, false);

    clickItem.new_item = data.new_item;
    vm_Base.pk_field = data.pk_field;
    if (data.fk_field != undefined)
        vm_Base.fk_field = data.fk_field;
    if (data.refresh_grid != undefined)
        vm_Base.refresh_grid = data.refresh_grid;

    if (clickItem.soperation_id != undefined && vm_Base.soperation_id != clickItem.soperation_id)
        vm_Base.soperation_id = clickItem.soperation_id;

    if (clickItem.parent_guid == undefined)
        clickItem.parent_guid = vm_Base.parent_guid;

    if (data.refresh_full != null && data.refresh_full != undefined) {
        vm_Base.refresh_full = data.refresh_full;
    }

    if (data.status === "error") {
        if (data.captcha_is_exists) {
            if (window[data.captcha_refresh] != undefined) {
                window[data.captcha_refresh]();
            }
            if (window[data.captcha_clear] != undefined) {
                window[data.captcha_clear]();
            }
        }
        messageWindow("Ошибка", data.message, data.error_text);
        entity_ChangeStatus("entity_Save_OnCompleted-1", vm_Base.url_Content, vm_Base.page_guid, 10);
        return 0;
    }

    if (data.is_restart) {
        window.parent.location.assign(window.parent.location + "Home/Index");
        return 1;
    }

    if (data.status === "success_message") {
        messageWindow(data.title, data.message, data.error_text);
    }

    if (vm_Base.TabEditSave_Content == 'True') {
        $("#TabEdit_ToolBar_" + vm_Base.page_guid).html("");
        $("#TabEditContent_" + vm_Base.page_guid).html(data);
        document.getElementById("TabEditContent_" + vm_Base.page_guid).style.borderWidth = 0;
        return 1;
    }

    if (vm_Base.TabEditSave_Redirect != undefined && vm_Base.TabEditSave_Redirect != '' && vm_Base.TabEditSave_Redirect != '0') {
        window.parent.location.assign(window.parent.location + "Home/Index?&nom=" + vm_Base.TabEditSave_Redirect);
        return 1;
    }

    if (vm_Base.soperation_id == sop_AddWizard) vm_Base.soperation_id = sop_2;
    if (vm_Base.soperation_id == sop_EditWizard) vm_Base.soperation_id = sop_3;

    if (vm_Base.soperation_id == sop_38) { // Добавить из селектора
        if (clickItem.selector_page_guid != undefined) {
            !is_true(clickItem.is_selector_from_filter);
            selector_Execute(clickItem.selector_page_guid, vm_Base.guids_out, "false", vm_Base.soperation_id, data);
        }
        else {
            !is_true(clickItem.is_selector_from_filter);
            selector_Execute(vm_Base.page_guid, vm_Base.guids_out, "false", vm_Base.soperation_id, data);
        }
        entity_status = 1;
        return;
    }

    var operation_isexecution = getUriParam_ByName(clickItem.url_action, "operation_isexecution");

    // Обновление гридов родительского или у соседних фреймов
    if ((vm_Base.soperation_id == sop_2 // Добавить
        || vm_Base.soperation_id == sop_3 // Корректировать
        || vm_Base.soperation_id == sop_12 // Добавление объекта к родительскому
        || vm_Base.soperation_id == sop_13 // Добавление дочернего объекта
        || vm_Base.soperation_id == sop_26 // Добавление дочернего объекта по схеме
        || vm_Base.soperation_id == sop_29 // Добавить (визард)
    )
    ) {
        var source_grid_guid_str = getGuidStr_FromGuidsOut(clickItem.guids_out, 'last-1');
        var source_grid_guid = source_grid_guid_str.split('_')[1];
        var source_name = source_grid_guid_str.split('_')[0];
        var rowRefreshed = false;

        /*Обновление грида в родительском фрейме*/
        var source_grid = null;
        if (source_name == 'MainTab' || data.refresh_grid == "parent") {
            source_grid = window.parent.$("#grid_" + clickItem.parent_guid);
            if ((vm_Base.soperation_id == sop_2    
                || vm_Base.soperation_id == sop_3
                || vm_Base.soperation_id == sop_12
                || vm_Base.soperation_id == sop_26
            )
                && !is_true(vm_Base.grid_no_refresh)
                && operation_isexecution == null
            ) {
                window.parent.vm_Base.refresh_full = data.refresh_full;
                if (data.refresh_grid == "parent") {
                    window.parent.vm_Base.pk_field = data.fk_field;
                }
                window.parent.grid_RefreshRow(clickItem.parent_guid, clickItem.soperation_id, data.new_item, data.pk_field, data.refresh_full);
                rowRefreshed = true;
            }
        }

        /*Обновление гридов в соседних фреймах*/
        if (!isMobile()) {
            if (((source_name == 'ChildTab' || vm_Base.soperation_id == sop_13) && !is_true(vm_Base.is_child_editor))
                || (source_name == 'MainTab'
                    && (data.refresh_grid != undefined
                        || is_true(clickItem.save_in_main_tab))
                    )
            ) {
                var frames = window.parent.frames;

                for (var i = 0; i < frames.length; i++) {
                    var $frame = $(frames[i].frameElement);

                    if (data.refresh_grid != undefined && $frame.attr("target_stable_id") == data.refresh_grid) {
                        if (frames[i].frameElement.id != "")
                            frames[i].grid_RefreshRow($frame.attr("page_guid"), sop_8, data.new_item, data.pk_field, data.refresh_full);
                    }
                    else {
                        var stableId = getUrlVars(clickItem.url_action)["stable_id"];
                        if (frames[i].frameElement.id != "") {
                            source_grid = frames[i].$("#grid_" + source_grid_guid);
                            if (vm_Base.soperation_id == sop_13) {
                                source_grid = frames[i].$("div[grid_stable_id=\"" + stableId + "\"]");
                                source_grid_guid = source_grid.attr("page_guid");
                            }

                            if (source_grid.length > 0) {
                                var parentId = source_grid.attr("parent_id");

                                if (vm_Base.soperation_id == sop_13 || vm_Base.soperation_id == sop_26
                                    || vm_Base.soperation_id == sop_3
                                    || vm_Base.soperation_id == sop_EditWizard
                                    || vm_Base.soperation_id == sop_2
                                    || vm_Base.soperation_id == sop_12
                                    || vm_Base.soperation_id == sop_AddWizard
                                ) {
                                    //Работа 5952. Убрал пока. Из-за этого коряво обновляется строка в гриде, где есть картинки в ячейках
                                    //frames[i].grid_RefreshRow(source_grid_guid, vm_Base.soperation_id, data.new_item, data.pk_field);
                                    frames[i].vm_Base.selectAfterAdd = true;
                                    if (data.new_item != undefined)
                                        frames[i].vm_Base.new_item_json = JSON.parse(data.new_item);;
                                    frames[i].vm_Base.pk_field = data.pk_field;
                                    frames[i].gridRefresh(source_grid_guid);
                                }
                            }
                        }
                    }
                }
            }
            else if (!rowRefreshed)
                grid_RefreshRow(vm_Base.parent_guid, vm_Base.soperation_id, data.new_item, data.pk_field, data.refresh_full);
        }
        else {
            current_view = Top().$(this.parent.document.location.hash);
            if (current_view.length > 0) {
                if (current_view.attr("view_parent") != undefined) {
                    parent_view = Top().$(current_view.attr("view_parent"));
                    source_grid = parent_view.find("iframe")[0].contentWindow.$("div.k-listview");
                    if (source_grid.length > 0) {
                        source_grid.data("kendoListView").dataSource.read();
                    }
                }
            }
        }

        /*Поиск и обновление грида в этом же редакторе*/
        if (clickItem.client_controller == "TabEditPage" && data.refresh_grid != undefined)
        {
            if ($("div[grid_stable_id=\"" + data.refresh_grid + "\"]").length > 0)
            {
                let grid = $("div[grid_stable_id=\"" + data.refresh_grid + "\"]");
                grid.data("kendoGrid").dataSource.read();
            }
        }
    }

    /*Обновление панели оперативной информации по сделке*/
    if (data.is_dealinfo_refresh == true) {
        let combo_for_deals = Top().$('[controltype="ComboBox_ForDeals"]');
        if (combo_for_deals.length > 0) {
            let deal_id = $(combo_for_deals)[0].defaultValue;
            vm_Base.guids_out = getGuidStr_FromGuidsOut(vm_Base.guids_out, 'first')
            deal_info_set_content(deal_id, vm_Base.url_Content, vm_Base.guids_out, vm_Base.page_guid, vm_Base.uri_path_out, 1, vm_Base.has_main_menu_sections);
        }
    }

    // Редактор не закрывается
    if (data.tab_close == false) {
        entity_status = 1;

        if (vm_Base.soperation_id != sop_2 && vm_Base.soperation_id != sop_12)
            entity_ChangeStatus("entity_Save_OnCompleted-2", vm_Base.url_Content, vm_Base.page_guid, 1);

        /*Редактор не закрывается, но обновить заголовок текущей вкладки надо (кроме редактора конфигураций)*/
        if (is_true(clickItem.save_in_main_tab)
            && data.name_value != null && data.stable_name != "SYS_CONFIG"
            && clickItem.mask_title == undefined)
            window.parent.$('#tabstrip li.k-state-active .tab-title-left').text(data.name_value);

        /*Работа 7131. Если была указана маска для заголовка вкладки, то после радакирования заголовок перерисовываем с учетом этой маски*/
        if (clickItem.mask_title != undefined) {
            var title = get_TabTitle_From_Mask(clickItem.mask_title);
            if (title != undefined)
                window.parent.$('#tabstrip li.k-state-active .tab-title-left').text(title);
        }

        return 1;
    }

    // Закрываем редактор
    if (is_true(clickItem.save_in_main_tab) && data.tab_close) {
        var tabstrip = $("#tabstrip");
        var parent_guid = vm_Base.parent_guid;
        tabstrip = Top().$("#tabstrip");
        var parent_tabstrip = parent.$("#tabstrip");

        var parent_frame = Top().$('#MainTabFrame_' + parent_guid);
        if (parent_frame.length > 0
            // Роев пока отключено (06.08.2019)
            && 1 == 2
        ) {

            var buttons_edit = parent_frame[0].contentWindow.$('.butt_edit');
            if (buttons_edit.length > 0) {
                $(buttons_edit[0]).attr("is_auto_open", "True");
                $(buttons_edit[0]).attr("auto_selected_row", "Last");
                parent_frame[0].contentWindow.vm_Base.auto_selected_row = data.pk_value;
                var parent_grid = parent_frame[0].contentWindow.$('.grid');
                if (parent_grid.length > 0)
                    $(parent_grid[0]).attr("is_autotabs_opened", "False");

                /*Реализация перерисовки редактора из режима добавления в режим редактирования без закрытия всего таба (пока отключена, работает плохо)*/
                //if ($(buttons_edit[0]).attr("action") != undefined) {
                //    var edit_action = $(buttons_edit[0]).attr("action");
                //    //edit_action = edit_action.replace("MainEdit", "TabEdit");
                //    //var editor_content = $("#editor_content");
                //    //editor_content.html("<div class='loader-spin is-fulled'></div>");
                //    var body = $("body");
                //    body.html("<div class='loader-spin is-fulled'></div>");
                //    $.ajax({
                //        url: edit_action,
                //        type: "POST",
                //        data: formData.toFormData(),
                //        contentType: false,
                //        processData: false,
                //        success: function (data) {
                //            //editor_content.html(data);
                //            body.html(data);
                //        }
                //    });
                //}
            }
        }

        tabStrip_RemoveTab(Top().$("#tabstrip"), "last", undefined, "last");
    }
    else {
        if (!is_true(vm_Base.is_child_editor)) {
            let parent_guid = vm_Base.mainTab_guid;
            var tabstrip = window.parent.$('[data-role="tabstrip"].childTabs');
            let ul_li = tabstrip.find('.k-item.k-state-active');
            window.parent.tabStrip_RemoveTab(tabstrip, "this", ul_li, "this", parent_guid);
        }
        else
            filterTab_Close(vm_Base.page_guid, 'false');
    }

    if (vm_Base != undefined && vm_Base.soperation_id == sop_Save) {
        entity_status = 1;
    }

    return 1;
}

function entity_ModalSave(vm_Base_params, url_Action, title, linkTarget_MainTab, wthoutDlg) { //wthoutDlg вызов команды напрямую из меню, без диалогов (anton)
    
    /*Костыль на случай старых вызовов без инициализации clickItem*/
    if (vm_Base_params != undefined)
        clickItem.fromVmBase = true;

    clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "temp_value", getRandomValue());

    //initialize(arguments);
    vm_Base.initializeArguments(arguments);
    formData.readInputs("myModalForm");

    if (clickItem.wthoutdlg == undefined && is_true(clickItem.close_after_error)) {

        var btnRun = Top().modWindow.element.find("button.k-primary");
        if (btnRun)
            btnRun.prop("disabled", true);

        // для kendoUnpload input
        var fileInputControl = $("#fileInput").data("kendoUpload");
        var content_name = "ModalWindow_";

        if (formData.errorMessage != undefined) {
            messageWindow("Ошибка", formData.errorMessage);
            if (btnRun)
                btnRun.prop("disabled", false);
            return;
        }

        clickItem.url_action = formData.toUrl(clickItem.url_action);

        if (fileInputControl) {
            var files = fileInputControl.getFiles();
            if (files.length > 0) {
                for (var i = 0; i < files.length; i++)
                    formData.append("fileInput_" + i, files[i].rawFile);

                $.ajax({
                    url: clickItem.url_action,
                    type: "POST",
                    data: formData.formDataObject,//formData.toFormData(),
                    contentType: false,
                    processData: false,
                    success: function (data) {
                        // Pavlov 09.03.2017
                        entity_ModalSave_Completed(data); /*, page_guid, soperation_id*/
                    }
                });
                return;
            }
        }
    }
    else if (clickItem.wthoutdlg != undefined) {
        //parent_id
        var row_id = getRowId_ByGuid(vm_Base.page_guid, vm_Base);
        if (row_id == null && isOperationsForRow(vm_Base.soperation_id) == true) {
            messageWindow("Ошибка", "Выберите строку.");
            return null;
        }
        var parent_id = row_id;
        if (row_id != null) {
            var index_semicolon = row_id.indexOf(";");
            if (index_semicolon >= 0) {
                parent_id = row_id.substring(0, index_semicolon);
            }
        }

        if (parent_id == null)
            parent_id = 0; //Когда в списке нет записей

        var options = {};
        clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "parent_id", parent_id);
    }

    if (formData != undefined)
        formData.append("Operation", "EntitySave");

    if (vm_Base.soperation_id == sop_8) {// SysOperation_Enum.Exec
        // Выбор из списка
        // 182	65	1	Выделенные строки
        // 183	65	2	Весь список
        //DOCUMENT_RELATION.TEMP_FIELD_1.1.DropDownList.0.64=1&
        //DOCUMENT_RELATION.TEMP_FIELD_2.2.DropDownList.0.65=2

        var filter_string = null;
        var parent_grid = getGrid_FromFrames(vm_Base.page_guid, vm_Base.clientController_Prev);
        if (parent_grid != null && parent_grid.attr("filter_string") != undefined) {
            filter_string = parent_grid.attr("filter_string");
            if (filter_string != null)
                formData.append("filter_string_encode", encodeURIComponent(filter_string));
        }
        formData.append("parent_ids", getRowId_ByGuid_FromFrames(vm_Base.page_guid, vm_Base.clientController_Prev));
    }

    if (vm_Base.soperation_id == sop_25 || vm_Base.soperation_id == sop_1) {// SysOperation_Enum.AddWithPreview
        var ss = typeof (formData);
        Top().modWindow.close();
        if (is_true(clickItem.link_target_maintab) || is_true(clickItem.linkTarget_MainTab)) {
            clickItem.fromVmBase = false;
            clickItem.url_action = formData.toUrl(clickItem.url_action);
            mainTab_Open();
        }
        else {
            vm_Base.soperation_id = sop_2;
            Top().previewWindow_FindFrame();
        }
        return;
    }

    $.ajax({
        url: clickItem.url_action,
        type: "POST",
        data: formData.formDataObject,//formData.toFormData(),
        contentType: false,
        processData: false,
        success: function (data) {
            entity_ModalSave_Completed(data);
        }
    });
}

function entity_ModalSave_Completed(data) {

    if (clickItem.wthoutdlg != undefined) {
        if (data.status === "error") {
            messageWindow("Ошибка", data.message, data.error_text);
        }
        else {
            if (data.status === "success" || data.status === "success_message") {
                messageWindow(data.title, data.message);

            } else if (data.status === "file_download") {
                location.href = data.url;
            }

            if (data.refresh_grid == "this")
            {
                let page_guid = $(".k-grid").attr("id").split("_")[1];
                gridRefresh(page_guid);
            }
        }     
        return;
    }

    try {
        if (data.status === "error") {
            Top().modWindow.save_result = 0;
            messageWindow("Ошибка", data.message, data.error_text);

            if (!is_true(clickItem.close_after_error))
                return;
        }
        else if (data.status === "success_result" || data.status === "file_download") {
            location.href = data.url;
            Top().modWindow.save_result = 1;
        }
        else {
            Top().modWindow.save_result = 0;

            var soperation_id = vm_Base.soperation_id;
            if (data.refresh_grid != null && data.refresh_grid != undefined) {
                Top().modWindow.refresh_grid = data.refresh_grid;
            }
            if (data.refresh_full != null && data.refresh_full != undefined) {
                Top().modWindow.refresh_full = data.refresh_full;
                vm_Base.refresh_full = data.refresh_full;
            }

            if (data.status != "success_without_message" && data.message != undefined)
                messageWindow(data.title, data.message);

            Top().modWindow.save_result = 1;

            /*Обновление панели оперативной информации по сделке*/
            if (data.is_dealinfo_refresh == true) {
                let combo_for_deals = Top().$('[controltype="ComboBox_ForDeals"]');
                if (combo_for_deals.length > 0) {
                    let deal_id = $(combo_for_deals)[0].defaultValue;
                    vm_Base.guids_out = getGuidStr_FromGuidsOut(vm_Base.guids_out, 'first')
                    deal_info_set_content(deal_id, vm_Base.url_Content, vm_Base.guids_out, vm_Base.page_guid, vm_Base.uri_path, 1, vm_Base.has_main_menu_sections);
                }
            }
        }
        Top().modWindow.soperation_id = soperation_id;
        if (data.soperation_id1 != undefined)
            Top().modWindow.soperation_id = data.soperation_id1;
        if (soperation_id == sop_2
            || soperation_id == sop_3
            || soperation_id == sop_12
            || soperation_id == sop_13
        ) {
            Top().modWindow.new_item = data.new_item;
            Top().modWindow.pk_field = data.pk_field;
            Top().modWindow.fk_field = data.fk_field;
        }

        Top().modWindow.close();
        Top().modWindow = undefined;
    }
    catch (e) {
        modWindow.soperation_id = vm_Base.soperation_id;
        if (data.status === "error") {
            modWindow.save_result = 0;
            messageWindow("Ошибка", data.message, data.error_text);
        }
        else {
            if (data.status === "success_message") {
                modWindow.save_result = 0;
                messageWindow(data.title, data.message);
            } else if (data.status === "file_download") {
                location.href = data.url;
            } modWindow.save_result = 1;
        }
        modWindow.close();
        modWindow = undefined;
    }
}

function entitySubGroup_Exec(sender, vm_Base_params, url, page_guid, soperation_id) {
    //initialize(arguments);
    vm_Base.initializeArguments(arguments);

    var group = undefined;
    if ($("#val_" + page_guid).attr("group") != undefined)
        group = $("#val_" + page_guid).attr("group");

    //var content = "<div id=\"delete-confirmation\" type=\"text/x-kendo-template\">" +
    //            "<p class=\"delete-message\">Are you sure?</p>" +
    //            "<button class=\"delete-confirm k-button\">Yes</button>" +
    //            "<button class=\"delete-cancel k-button\">No</button>" +
    //            "</div>";

    //var options = {};
    //options.url = url;
    //options.type = "POST";

    //options.data = {};
    //options.data.Operation = "EntitySave";

    formData.readInputs("myForm", group);
    formData.append("Operation", "EntitySave");
    //var message = entity_ReadData(options, null, page_guid, null, group);

    button_SetReadOnly(sender, true);

    //options.success = function (data) {
    if ($("#val_" + page_guid).attr("nalgorithm_ids") != undefined) {
        var nalgorithm_ids = $("#val_" + page_guid).attr("nalgorithm_ids");
        for (t = 0; t <= nalgorithm_ids.split(",").length - 1; t++) {
            formData.append("nalgorithm_id", nalgorithm_ids.split(",")[t]);

            $.ajax({
                url: url,
                type: "POST",
                data: formData.formDataObject,//formData.toFormData(),
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data.status === "error") {
                        messageWindow("Ошибка", data.message, data.error_text);
                        button_SetReadOnly(sender, false);
                        return 0;
                    }
                    else if (data.status === "success_message") {
                        messageWindow(data.title, data.message);
                    }
                    else if (data.status === "success_result") {
                        var data_result = data.result;
                        for (i = 0; i < Object.keys(data_result).length; i++) {
                            var property = Object.keys(data_result)[i];
                            var value = data_result[Object.keys(data_result)[i]];
                            var input = $("[sub_group_output='true'][group='" + group + "'][property='" + property + "']");

                            if (value != undefined && input.length > 0) {
                                if (input.data("kendoNumericTextBox") != undefined) {
                                    input.data("kendoNumericTextBox").value(value);
                                    if (input.attr("name").indexOf(".Changed") == -1) {
                                        input.attr("name", input.attr("name") + ".Changed");
                                        input.attr("Changed", "Changed");
                                    }
                                    continue;
                                }

                                if (input.data("kendoChart") != undefined) {
                                    var seriesData = undefined;
                                    for (j = 0; j < value.split("|").length - 1; j++) {
                                        var val1 = value.split("|")[j].split(";")[0];
                                        var val2 = value.split("|")[j].split(";")[1];

                                        val2 = val2.replace(",", ".");

                                        if (seriesData == undefined)
                                            seriesData = [{
                                                cat_field: val1,
                                                value: val2
                                            }];
                                        else
                                            seriesData.push({
                                                cat_field: val1,
                                                value: val2
                                            });
                                    }

                                    var series_name = input.data("kendoChart").options.series[0].name;
                                    var series_type = input.data("kendoChart").options.series[0].type;
                                    var tooltip = input.data("kendoChart").options.tooltip;
                                    var legend = input.data("kendoChart").options.legend;

                                    input.kendoChart({
                                        dataSource: {
                                            data: seriesData
                                        },
                                        series: [{
                                            type: series_type,
                                            field: "value",
                                            categoryField: "cat_field",
                                            name: series_name
                                        }],
                                        tooltip: tooltip,
                                        legend: legend
                                    });

                                    continue;
                                }

                                input.text(value);
                            }
                        }
                    }

                    button_SetReadOnly(sender, false);
                }
            });
        }
    }
}

function findInHelpTree(vm_Base_params, url_Action) {

    //initialize(arguments);
    vm_Base.initializeArguments(arguments);
    formData.readInputs("myForm");

    var btnRun = Top().modWindow.element.find("button.k-primary");
    if (btnRun) {
        btnRun.prop("disabled", true);
    };

    if (formData.errorMessage != undefined) {
        messageWindow("Ошибка", formData.errorMessage);
        return;
    }

    findNextInHelpTree(vm_Base_params, url_Action)

    try {
        Top().modWindow.close();
        Top().modWindow = undefined;
    }
    catch (e) {
        modWindow.close();
        modWindow = undefined;
    }
}

function findNextInHelpTree(vm_Base_params, url_Action) {
    //initialize(arguments);
    vm_Base.initializeArguments(arguments);
    let myForm = "myForm";
    if (vm_Base.clientController == "ModalWindow")
        myForm = "myModalForm";
    formData.readInputs(myForm);

    var grid = getGrid_FromFrames(vm_Base.page_guid, vm_Base.clientController_Prev);

    if (grid != null) {

        if (url_Action.indexOf("parent_id") < 0) {
            var row_id = getRowId_ByGuid(vm_Base.page_guid, vm_Base);
            if (row_id == null) {
                messageWindow("Ошибка", "Выберите строку.");
                return;
            }
            url_Action = addToUrl(url_Action, "parent_id", row_id);
        }

        formData.append("Operation", "EntitySave")

        $.ajax({
            url: url_Action,
            type: "POST",
            data: formData.formDataObject,//formData.toFormData(),
            contentType: false,
            processData: false,
            success: function (data) {
                if (data.nextId !== undefined) {
                    var kendoName = "kendoTreeList";
                    var filterString = "<Filter><Item TypeRelation='AllParentsAndChildren' /></Filter>";
                    grid.attr("filter_string", filterString);
                    grid.data("loaded", false);
                    var kendoGrid = grid.data(kendoName);
                    kendoGrid.dataSource.transport.options.read.url = addToUrl(kendoGrid.dataSource.transport.options.read.url, "parent_id", data.nextId);
                    kendoGrid.dataSource.read();
                }
                else if (data.status === "error") {
                    messageWindow("Ошибка", data.message, data.error_text);
                }
                else {
                    messageWindow(data.title, data.message);
                }
            }
        });
    }
}

// **************************************************************************************************
function entity_ReadData_EditableGrid(sender, grid_items) {

    var grid = $(sender).data("kendoGrid");
    var grid_ds = grid.dataSource;
    var grid_ds_data = grid_ds.data();

    var grid_page_nom = $(sender).attr("grid_page_nom");
    var grid_stable_id = $(sender).attr("grid_stable_id");
    var grid_stable_name = $(sender).attr("grid_stable_name");

    for (var i = 0; i < grid_ds_data.length; i++) {
        var item = {};
        // ($Роев)
        var suffix = ";" + grid_page_nom + ";" + grid_stable_id + ";" + i;

        if (grid_stable_name == "POINT_METER_SHOW_VALUE") {
            if ((grid_ds_data[i].VALUE != grid_ds_data[i].ValueOld) && (grid_ds_data[i].VALUE)) {
                grid_items.push({ Value: "CALC_ITEM_ID=" + grid_ds_data[i].CALC_ITEM_ID_str + suffix });
                grid_items.push({ Value: "POINT_METER_ID=" + grid_ds_data[i].POINT_METER_ID_str + suffix });
                grid_items.push({ Value: "NMETER_PARAM_ID=" + grid_ds_data[i].NMETER_PARAM_ID + suffix });
                grid_items.push({ Value: "DATE_BEG=" + grid_ds_data[i].DateBeg + suffix });
                grid_items.push({ Value: "VALUE=" + grid_ds_data[i].VALUE + suffix });
            }
        }
        else if (grid_stable_name == "EQUIPMENT_METER") {
            if (grid_ds_data[i].NMETER_NAME != grid_ds_data[i].NmeterNameOld) {

                var equipment_id = grid_ds_data[i].EQUIPMENT_ID;
                if (equipment_id == undefined)
                    equipment_id = 0;

                grid_items.push({ Value: "EQUIPMENT_ID=" + equipment_id + suffix });
                grid_items.push({ Value: "NMETER_NAME=" + grid_ds_data[i].NMETER_NAME + suffix });
            }
        }
        else if (grid_stable_name == "NSI_CALC_MATRIX") {
            if (grid_ds_data[i].NCALC_MATRIX_NAME != grid_ds_data[i].NcalcMarixNameOld
                || grid_ds_data[i].NCALC_MATRIX_LIST != grid_ds_data[i].NcalcMarixListOld
            ) {
                grid_items.push({ Value: "NCALC_MATRIX_ID=" + grid_ds_data[i].NCALC_MATRIX_ID + suffix });

                if (grid_ds_data[i].NCALC_MATRIX_NAME != grid_ds_data[i].NcalcMarixNameOld)
                    grid_items.push({ Value: "NCALC_MATRIX_NAME=" + grid_ds_data[i].NCALC_MATRIX_NAME + suffix });

                if (grid_ds_data[i].NCALC_MATRIX_LIST != grid_ds_data[i].NcalcMarixListOld)
                    grid_items.push({ Value: "NCALC_MATRIX_LIST=" + grid_ds_data[i].NCALC_MATRIX_LIST + suffix });
            }
        }
    }
    return true;
}

// **************************************************************************************************
function previewWindow_FindFrame() {
    var frames = window.parent.frames;
    if (vm_Base.clientController_Prev == "MainTable") {
        for (var i = 0; i < frames.length; i++) {
            var grid = frames[i].$("#grid_" + vm_Base.page_guid);
            if (grid.length > 0) {
                //frames[i].clickItem.initializeFromVmBase();
                frames[i].clickItem.url_action = clickItem.url_action;
                frames[i].previewWindow_Execute();
                return;
            }
        }
    }
    if (vm_Base.clientController_Prev == "TabTable") {
        for (var i = 0; i < frames.length; i++) {
            var frames1 = frames[i].frames;
            for (var i1 = 0; i1 < frames1.length; i1++) {
                var grid1 = frames1[i1].$("#grid_" + vm_Base.page_guid);

                //Временное решение. Если грид открыт через операцию 23, то модальный диалог его не сможет найти.
                //Добавил поиск по атрибуту page_guid23_prev
                if (grid1.length == 0)
                    grid1 = frames1[i1].$("div[page_guid23_prev='" + vm_Base.page_guid + "']");

                if (grid1.length > 0) {
                    //frames1[i1].clickItem.initializeFromVmBase();
                    frames1[i1].clickItem.url_action = clickItem.url_action;
                    frames1[i1].previewWindow_Execute();
                    return;
                }
            }
        }
    }
}

function previewWindow_Execute() {

    var row_id = getRowId_ByGuid(Top().vm_Base.page_guid, Top().vm_Base, Top().vm_Base.grid_selected_rows);
    if (row_id == null && isOperationsForRow(Top().vm_Base.soperation_id) == true) {
        messageWindow("Ошибка", "Выберите строку.");
        return;
    }

    if (row_id != null) {
        // Выделенные строки
        if (Top().vm_Base.grid_selected_rows == 1) {
            var index_semicolon = row_id.indexOf(";");
            if (index_semicolon >= 0) {
                var parent_id = row_id.substring(0, index_semicolon);
                Top().vm_Base.url_Action = updateQueryStringParameter(Top().vm_Base.url_Action, "parent_id", parent_id);

                Top().vm_Base.url_Action = updateQueryStringParameter(Top().vm_Base.url_Action, "parent_ids", row_id);
            }
            else
                Top().vm_Base.url_Action = updateQueryStringParameter(Top().vm_Base.url_Action, "parent_id", row_id);
        }
    }

    childTab_Open(undefined, /*Top().vm_Base.vm_Base_params,*/ Top().vm_Base.url_Action, Top().vm_Base.title/*, Top().vm_Base.soperation_id, 1*/);  //Top().vm_Base.page_guid, Top().vm_Base.guids_out, 
}

// **************************************************************************************************
function entity_Message(content, parent_guid, page_guid) {

    var content_parent = parent.$("html ").html();
    messageWindow("parent", parent_guid, content_parent);

    var content_this = $("html").html();
    messageWindow("this", page_guid, content_this);
}

function tab_Selected(url_Content, page_guid, isWizard) {

    var wizardTab_data = $("#WizardTab_" + page_guid).data("kendoTabStrip");
    var selectedTabIndex = wizardTab_data.select().index();
    var tabCount = wizardTab_data.items().length;

    var toolBar = $("#TabEdit_ToolBar_" + page_guid);
    if (toolBar != undefined) {
        var toolBar_data = toolBar.data("kendoMenu");
        if (toolBar_data != undefined) {
            toolBar_data.enable("#buttBack_" + page_guid, (selectedTabIndex > 0));
            toolBar_data.enable("#buttForward_" + page_guid, (selectedTabIndex < tabCount - 1));
        }
    }

    var val_submit = $("#val_" + page_guid);
    var val_prev = $("#val_prev_" + page_guid);
    var val_next = $("#val_next_" + page_guid);
    if (val_prev.length > 0 && val_next.length > 0) {

        // val_prev ............................................
        if (selectedTabIndex > 0) {
            val_prev.removeClass("k-state-disabled");

            val_prev.removeClass("is-button-silver");
            val_prev.addClass("is-button-blue");
        }
        else {
            val_prev.removeClass("is-button-blue");
            val_prev.addClass("is-button-silver");

            if (val_prev.hasClass("k-state-disabled") == false)
                val_prev.addClass("k-state-disabled");
        }

        // val_next & val_submit ............................................
        if (selectedTabIndex < tabCount - 1) {
            // не последний Tab
            val_next.removeClass("k-state-disabled");
            val_next.removeClass("is-button-silver");
            val_next.addClass("is-button-blue");

            if (val_submit != null) {
                val_submit.removeClass("is-button-gold");
                val_submit.addClass("is-button-gold-silver");
                if (val_submit.hasClass("k-state-disabled") == false)
                    val_submit.addClass("k-state-disabled");
            }
        }
        else {
            // последний Tab
            val_next.removeClass("is-button-blue");
            val_next.addClass("is-button-silver");
            if (val_next.hasClass("k-state-disabled") == false)
                val_next.addClass("k-state-disabled");

            if (val_submit != null) {
                val_submit.removeClass("k-state-disabled");
                val_submit.removeClass("is-button-gold-silver");
                val_submit.addClass("is-button-gold");
            }
        }
    }

    if (isWizard) {
        wizardTab_data.enable(wizardTab_data.items(), false);
        wizardTab_data.enable(wizardTab_data.select(), true);
    }
    entity_Arrow_Image(url_Content, page_guid, selectedTabIndex, tabCount);
}

function entity_Next() {

    if (entity_status == -1)
        return;

    /*Работа 6203. Проверка на заполнение обязательных полей при переходе на следующую страницу визарда*/
    formData.readInputs("myForm");
    if (formData.errorMessage != undefined) {
        messageWindow("Ошибка", formData.errorMessage);
        return;
    }

    var val_next = $("#val_next_" + clickItem.page_guid);
    if (val_next != null && val_next.hasClass("k-state-disabled"))
        return;
    if (val_next.length > 0)
        val_next.addClass("k-state-disabled");

    var tabStrip = $("#WizardTab_" + clickItem.page_guid).data("kendoTabStrip");
    var tabCount = tabStrip.items().length;
    var selectedTabIndex = tabStrip.select().index();
    if (selectedTabIndex + 1 >= tabCount)
        return;

    var TabEdit_ButtonReturn = $("#buttBack_" + clickItem.page_guid);
    if (TabEdit_ButtonReturn.length > 0)
        TabEdit_ButtonReturn.removeClass("k-state-hover k-state-focused");

    var TabEdit_ButtonEnter = $("#buttForward_" + clickItem.page_guid);
    if (TabEdit_ButtonEnter.length > 0)
        TabEdit_ButtonEnter.removeClass("k-state-hover k-state-focused");

    if (clickItem.is_wizard) {
        var url = tabStrip.options.contentUrls[selectedTabIndex + 1];
        //if (_url != undefined)
        //    url = _url;

        formData.readInputs("myForm");
        if (formData.errorMessage != undefined) {
            if (val_next.length > 0)
                val_next.removeClass("k-state-disabled");
            messageWindow("Ошибка", formData.errorMessage);
            return;
        }

        entity_status = -1;
        $.ajax({
            url: url,
            type: "POST",
            data: formData.formDataObject,//formData.toFormData(),
            contentType: false,
            processData: false,
            success: function (data) {

                entity_status = 1;

                if (val_next.length > 0)
                    val_next.removeClass("k-state-disabled");

                if (data.status === "error") {
                    if (data.captcha_is_exists) {
                        if (window[data.captcha_refresh] != undefined) {
                            window[data.captcha_refresh]();
                        }
                        if (window[data.captcha_clear] != undefined) {
                            window[data.captcha_clear]();
                        }
                    }
                    messageWindow("Ошибка", data.message, data.error_text);
                }
                else {
                    $(tabStrip.contentElement(selectedTabIndex + 1)).html(data);
                    tabStrip.select(selectedTabIndex + 1);
                }
            }
        });
    }
    else {
        tabStrip.select(selectedTabIndex + 1);
    }
}

function entity_Prev(/*url_Content, page_guid, isWizard*/) {

    var val_prev = $("#val_prev_" + clickItem.page_guid);
    if (val_prev != null && val_prev.hasClass("k-state-disabled"))
        return;

    var tabStrip = $("#WizardTab_" + clickItem.page_guid).data("kendoTabStrip");
    var selectedTabIndex = tabStrip.select().index();
    if (selectedTabIndex == 0)
        return;

    var TabEdit_ButtonReturn = $("#buttBack_" + clickItem.page_guid);
    if (TabEdit_ButtonReturn.length > 0)
        TabEdit_ButtonReturn.removeClass("k-state-hover k-state-focused");

    var TabEdit_ButtonEnter = $("#buttForward_" + clickItem.page_guid);
    if (TabEdit_ButtonEnter.length > 0)
        TabEdit_ButtonEnter.removeClass("k-state-hover k-state-focused");

    tabStrip.select(selectedTabIndex - 1);
    if (clickItem.is_wizard) {
        var url = tabStrip.options.contentUrls[selectedTabIndex];

        formData.readInputs("myForm", -1);
        formData.append("funcName", "entity_Prev");
        /*Работа 6999. Не понял зачем при переходе на предыдущую вкладку проверять контролы на заполнение*/
        //if (formData.errorMessage != undefined) {
        //    if (val_next.length > 0)
        //        val_next.removeClass("k-state-disabled");
        //    messageWindow("Ошибка", formData.errorMessage);
        //    return;
        //}
        var data = formData.formDataObject;
        var sss = 1;

        $.ajax({
            url: url,
            type: "POST",
            data: formData.formDataObject,
            contentType: false,
            processData: false,
            success: function (data) {
            }
        });

        $(tabStrip.contentElement(selectedTabIndex)).children().remove();
    }
}

function entity_Arrow_Image(url_Content, page_guid, selectedTabIndex, tabCount) {

    var buttReturn = $("#buttBack_" + page_guid);
    if (selectedTabIndex > 0) {
        buttReturn.find("img").attr("src", url_Content + "Images/Operations/Large/ArrowBack.png");
    }
    else {
        buttReturn.find("img").attr("src", url_Content + "Images/Operations/Large/ArrowBack_Disable.png");
    }

    var buttEnter = $("#buttForward_" + page_guid);
    if (selectedTabIndex < tabCount - 1) {
        buttEnter.find("img").attr("src", url_Content + "Images/Operations/Large/ArrowForward.png");
    }
    else {
        buttEnter.find("img").attr("src", url_Content + "Images/Operations/Large/ArrowForward_Disable.png");
    }
}

function report_generate(url, content, soperation_id, report_format, sendToPrint, page_guid) {
    if (entity_status == 1) {
        initialize(arguments);
        formData.readInputs("myForm");

        if (formData.errorMessage != undefined) {
            messageWindow("Ошибка", formData.errorMessage);
            return;
        }
        formData.append("report_format", clickItem.report_format);
        formData.append("sendToPrint", is_true(clickItem.send_to_print));

        // Для 201 операции УРЛ передается параемтром, для кнопок на форме вызова отчета в элементе
        var urlPost = clickItem.url_action;
        if (soperation_id == sop_ReportDirect) {
            var parent_id = getRowId_ByGuid(clickItem.page_guid);
            formData.append("parent_id", parent_id);
            urlPost = url;
        }
        //formData.append("temp_value", getRandomValue());

        var formAreaEdit = $("#TabEditContent_" + clickItem.page_guid);
        kendo.ui.progress(formAreaEdit, true);

        $.ajax({
            url: urlPost,
            type: "POST",
            data: formData.formDataObject,//formData.toFormData(),
            contentType: false,
            processData: false,
            success: function (data) {
                var formAreaEdit = $("#TabEditContent_" + clickItem.page_guid);
                kendo.ui.progress(formAreaEdit, false);
                if (data.status === "error") {
                    messageWindow("Ошибка", data.message, data.error_text);
                }
                else if (data.status === "success") {
                    if (clickItem.soperation_id == sop_Report) {
                    }

                    var tabStrip = $("#WizardTab_" + clickItem.page_guid).data("kendoTabStrip");
                    var selectedTabIndex = 0;
                    var tabCount = 1;
                    if (tabStrip != undefined) {
                        selectedTabIndex = tabStrip.select().index();
                        tabCount = tabStrip.items().length;
                    }
                    //messageWindow("Отчеты", "Отчет сформирован");

                    
                    if (is_true(clickItem.send_to_print)) {
                        messageWindow("Печать", "Отчет отправлен на принтер.");
                    }
                    else {
                        downloadUrl = "TabReportDownloadResult?reportResultFileName=" + data.reportResultFileName;
                        if (!window.open(downloadUrl)) {
                            var hiddenIFrameID = 'hiddenDownloader';
                            var link = document.createElement('a');
                            link.setAttribute('href', downloadUrl);
                            link.setAttribute('download', data.reportResultFileNameBase);
                            onload = link.click();
                        }
                    }

                    // Закрываем tab
                    if (clickItem.soperation_id == sop_Report || clickItem.soperation_id == sop_ReportExport) {
                        //window.parent.childTab_Close(vm_Base.parent_guid, clickItem.page_guid);
                    }
                } else {
                    messageWindow("Ошибка", "Произошла неуловимая ошибка при попытке сгенерировать отчет", "");
                }
            }
        });
    }
}

// **************************************************************************************************
var rows_count;
var curr_row_ind;
var curr_column;
var pk_column;
var pk_prop_name;
var save_message;
var arr_prop = [];
var arr_formulas = [];
var arr_readonly = [];
var arr_readonly_rows = [];
var arr_validations = [];
var arr_formats = [];
var arr_unvisible_columns = [];
var arr_rows_height = [];
var all = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M",
            "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
            "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM",
            "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ"];

function entity_SpreadSheet_Read(options) {
    let url_action;
    let spreadsheet = find_SpreadSheet();
    if (spreadsheet == undefined || spreadsheet.length == 0)
        return null;

    //let spreadsheet_data = spreadsheet.data("kendoSpreadsheet");
    if (is_true(spreadsheet.attr('is_spreadsheet_empty'))) {
        return null;
    }

    url_action = $("#spreadsheet")[0].getAttribute("url_action");

    $.ajax({
        url: url_action,
        dataType: "json",
        success: function (result) {
            var spreadsheet = $("#spreadsheet").data("kendoSpreadsheet");
            var sheet = spreadsheet.activeSheet();

            var array = [];
            var arr_header = [];

            sheet.range("A2:AZ2").forEachCell(function (row, column, cellProperties) {
                if (cellProperties.formula != undefined)
                    arr_formulas[column] = cellProperties.formula.print();
                if (cellProperties.enable == false)
                    arr_readonly[column] = false;
                if (cellProperties.validation != undefined)
                    arr_validations[column] = cellProperties.validation;
                if (cellProperties.format != undefined)
                    arr_formats[column] = cellProperties.format;
            });
            arr_prop = sheet.range("A2:AZ2").values()[0];
            for (let key in arr_prop) {
                arr_header.push(arr_prop[key]);
            }
            pk_column = get_Column_ByPropName(pk_prop_name);

            let row_ind = 2;
            result.Data.map(function (el) {
                var arr_content = [];
                for (let key in arr_prop) {
                    arr_content.push(el[arr_prop[key]]);
                }
                array.push(arr_content);

                if (el.ResultId == 1)
                    arr_readonly_rows.push(row_ind);
                row_ind++;
            });
            array.push(arr_header);
            rows_count = array.length;

            sheet.range("A2:AZ" + array.length + 1).values(array);
            sheet.hideRow(array.length);

            for (let key in arr_formulas) {
                let input = arr_formulas[key];
                let output = "";
                let column = all[key];

                for (let i = 2; i < array.length + 1; i++) {
                    sheet.range(column + i).formula(input.replace(/a/g, i));
                };
            };

            for (let key in arr_readonly) {
                let column = all[key];
                sheet.range(column + "2:" + column + array.length).enable(false);
            }

            for (let key in arr_validations) {
                let column = all[key];
                sheet.range(column + "2:" + column + array.length).validation(arr_validations[key]);

                if (arr_validations[key].dataType == "list") {
                    let values = sheet.range(column + "2:" + column + array.length).values();
                    for (let j in values) {
                        if (values[j] == "") {
                            sheet.range(column + (Number(j) + 2)).value(null);
                            continue;
                        }

                        for (let i in arr_validations[key].from_value.data[0]) {
                            let item = arr_validations[key].from_value.data[0][i];
                            let ind = item.indexOf(". ");
                            if (item.substring(0, ind)== values[j]) {
                                sheet.range(column + (Number(j) + 2)).value(item);
                                break;
                            }
                        }
                    }     
                }

                if (arr_formats[key] != undefined)
                    sheet.range(column + "2:" + column + array.length).format(arr_formats[key]);
            }

            for (let key in arr_readonly_rows) {
                curr_row_ind = arr_readonly_rows[key];
                disable_Row(true);
            }

            set_Unvisible_Columns();
        },
        error: function (result) {
            var spreadsheet = $("#spreadsheet").data("kendoSpreadsheet");
            var sheet = spreadsheet.activeSheet();

            /*Если SpreadSheet должен быть пустой*/
            if (!is_true($("#spreadsheet").attr('is_spreadsheet_empty')))
                messageWindow("Ошибка загрузки", "Ошибка загрузки. Список пуст");

            sheet.range("A2:AZ3").clear();
        }
    });
}

function entity_SpreadSheet_Read_Modal_run(nalgorithm_id, page_guid) {
    formData.readInputs("myModalForm");

    Top().frames[0].entity_SpreadSheet_Read_Modal(nalgorithm_id, page_guid, formData);
}

function entity_SpreadSheet_Read_Modal(nalgorithm_id, page_guid) {
    formData.readInputs("myModalForm");

    let url_action;
    var spreadsheet = find_SpreadSheet(page_guid);

    if (spreadsheet == undefined || spreadsheet.length == 0)
        return null;

    url_action = spreadsheet[0].getAttribute("url_action");
    if (nalgorithm_id != undefined && typeof (nalgorithm_id) == "number")
        url_action = updateQueryStringParameter(url_action, "grid_nalgorithm_id", nalgorithm_id);

    let fd = undefined;
    if (formData.keys.length != 0)
        fd =  formData.formDataObject;

    arr_prop = [];

    Top().modWindow.close();

    $.ajax({
        url: url_action,
        dataType: "json",
        data: fd,
        type: "POST",
        contentType: false,
        processData: false,
        success: function (result) {
            if (result.Data == undefined)
                return null;

            var spreadsheet = find_SpreadSheet();//Top().frames[0].$("#spreadsheet").data("kendoSpreadsheet");
            var sheet = spreadsheet.data("kendoSpreadsheet").activeSheet();

            sheet.unhideRow(1);

            var array = [];
            var arr_header = [];

            sheet.range("A2:AZ2").forEachCell(function (row, column, cellProperties) {
                if (cellProperties.formula != undefined)
                    arr_formulas[column] = cellProperties.formula.print();
                if (cellProperties.enable == false)
                    arr_readonly[column] = false;
                if (cellProperties.validation != undefined)
                    arr_validations[column] = cellProperties.validation;
                if (cellProperties.format != undefined)
                    arr_formats[column] = cellProperties.format;
            });

            if (arr_prop.length == 0)
                arr_prop = sheet.range("A2:AZ2").values()[0];
            for (let key in arr_prop) {
                arr_header.push(arr_prop[key]);
            }
            pk_column = get_Column_ByPropName(pk_prop_name);

            let row_ind = 2;
            result.Data.map(function (el) {
                var arr_content = [];
                for (let key in arr_prop) {
                    if (el[arr_prop[key]] != undefined && typeof (el[arr_prop[key]]) == "string") {
                        if (el[arr_prop[key]].indexOf('\n') != -1) {
                            let row_height = el[arr_prop[key]].split('\n').length;
                            if (arr_rows_height[row_ind] == undefined || arr_rows_height[row_ind] < row_height)
                                arr_rows_height[row_ind] = el[arr_prop[key]].split('\n').length;
                        }
                    }
                    arr_content.push(el[arr_prop[key]]);
                }
                array.push(arr_content);

                if (el.ResultId == 1)
                    arr_readonly_rows.push(row_ind);
                row_ind++;
            });
            array.push(arr_header);
            rows_count = array.length;

            sheet.range("A2:AZ" + array.length + 1).values(array);
            sheet.hideRow(array.length);

            for (let key in arr_formulas) {
                let input = arr_formulas[key];
                let output = "";
                let column = all[key];

                for (let i = 2; i < array.length + 1; i++) {
                    sheet.range(column + i).formula(input.replace(/a/g, i));
                };
            };

            for (let key in arr_readonly) {
                let column = all[key];
                sheet.range(column + "2:" + column + array.length).enable(false);
            }

            for (let key in arr_validations) {
                let column = all[key];
                sheet.range(column + "2:" + column + array.length).validation(arr_validations[key]);

                if (arr_validations[key].dataType == "list") {
                    let values = sheet.range(column + "2:" + column + array.length).values();
                    for (let j in values) {
                        if (values[j] == "") {
                            sheet.range(column + (Number(j) + 2)).value(null);
                            //arr_combo_items.push(null);
                            continue;
                        }

                        for (let i in arr_validations[key].from_value.data[0]) {
                            let item = arr_validations[key].from_value.data[0][i];
                            let ind = item.indexOf(". ");
                            if (item.substring(0, ind) == values[j]) {
                                sheet.range(column + (Number(j) + 2)).value(item);
                                break;
                            }
                        }
                    }
                }

                if (arr_formats[key] != undefined)
                    sheet.range(column + "2:" + column + array.length).format(arr_formats[key]);
            }

            for (let key in arr_formats) {
                let column = all[key];
                let format = arr_formats[key];
                sheet.range(column + "2:" + column + array.length).format(format);
            }

            for (let key in arr_readonly_rows) {
                curr_row_ind = arr_readonly_rows[key];
                disable_Row(true);
            }

            for (let key in arr_rows_height) {
                sheet.rowHeight(Number(key)-1, arr_rows_height[key] * 15);
            }

            set_Unvisible_Columns();
        },
        error: function (result) {
            var spreadsheet = find_SpreadSheet();
            var sheet = spreadsheet.data("kendoSpreadsheet").activeSheet();

            /*Если SpreadSheet должен быть пустой*/
            if (!is_true($("#spreadsheet").attr('is_spreadsheet_empty')))
                messageWindow("Ошибка загрузки", "Ошибка загрузки. Список пуст");

            sheet.range("A2:AZ3").clear();
        }
    });
}

function entity_SpreadSheet_Save() {
    $("#spreadsheet").data("kendoSpreadsheet").activeSheet().dataSource.sync();
}

function entity_SpreadSheet_Submit(e) {
    var spreadsheet = $("#spreadsheet").data("kendoSpreadsheet");
    var sheet = spreadsheet.activeSheet();
    var data = spreadsheet.toJSON();

    $.ajax({
        type: "POST",
        url: clickItem.url_action,
        data: JSON.stringify(data),
        cache: false,
        contentType: false,
        processData: false,
        success: function (data) {
            /*Разобраться с этим. Глобальные массивы должны быть в одном месте*/
            if (arr_prop.length == 0 && Top().arr_prop.length > 0)
                arr_prop = Top().arr_prop;

            for (let i = 0; i < data.length; i++) {
                curr_row_ind = data[i]["Id"] + 1;//get_RowInd_ById(data[i]["id"]);

                curr_column = get_Column_ByPropName("Result");
                sheet.range(curr_column + curr_row_ind).value(data[i]["resultStr"]);

                curr_column = get_Column_ByPropName("ResultSave");
                sheet.range(curr_column + curr_row_ind).value(data[i]["resultCheckStr"]);

                curr_column = get_Column_ByPropName("ResultMask");
                sheet.range(curr_column + curr_row_ind).value(data[i]["resultMaskStr"]);

                curr_column = get_Column_ByPropName("PointMeterShowId");
                sheet.range(curr_column + curr_row_ind).value(data[i]["newId"]);

                curr_column = get_Column_ByPropName("ResultId");
                sheet.range(curr_column + curr_row_ind).value(data[i]["resultId"]);

                if (data[i]["resultId"] == 1)
                    disable_Row(true);
            }

            if (!clickItem.is_save) {
                if (data.message == undefined)
                    save_message = "Ведомость разнесена";
                else
                    save_message = data.message;
                if ($('#buttSave_' + clickItem.page_guid).length > 0)
                    $('#buttSave_' + clickItem.page_guid).click();
                else {
                    data.message = "Операция выполнена";
                    data.status = "success_message";
                    entity_Save_OnCompleted(data);
                }
            }
            else {
                let str = data.message;
                if (save_message != undefined)
                    data.message = save_message + str;
                entity_Save_OnCompleted(data);
                save_message = undefined;
            }
        },
        error: function (xhr, httpStatusMessage, customErrorMessage) {
            alert(customErrorMessage);
        }
    });
}

function entity_SpreadSheet_Exec_Operation() {
    var parent_id = get_SelectedRowId_SpreadSheet("PointMeterShowId");
    if (parent_id != undefined) {
        if (clickItem.url_action != undefined) {
            clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "parent_id", parent_id);
         
            $.ajax({
                type: "POST",
                url: clickItem.url_action,
                success: function (data) {
                    if (data.status == "success") {
                        let spreadsheet = $("#spreadsheet").data("kendoSpreadsheet");
                        let sheet = spreadsheet.activeSheet();

                        if (clickItem.soperation_id == 4) {
                            curr_column = get_Column_ByPropName("Result");
                            sheet.range(curr_column + curr_row_ind).value(null);

                            curr_column = get_Column_ByPropName("ResultSave");
                            sheet.range(curr_column + curr_row_ind).value(null);

                            curr_column = get_Column_ByPropName("ResultMask");
                            sheet.range(curr_column + curr_row_ind).value(null);

                            curr_column = get_Column_ByPropName("PointMeterShowId");
                            sheet.range(curr_column + curr_row_ind).value(null);

                            curr_column = get_Column_ByPropName("ResultId");
                            sheet.range(curr_column + curr_row_ind).value(null);

                            disable_Row(false);
                        }

                        if (clickItem.soperation_id == 8) {
                            curr_column = get_Column_ByPropName("ResultMask");
                            if (data["resultMask"] != undefined)
                                sheet.range(curr_column + curr_row_ind).value("Маска");
                            else
                                sheet.range(curr_column + curr_row_ind).value(null);
                        }
                    }

                    if (data.message == undefined)
                        save_message = "Операция выполнена";
                    else
                        save_message = data.message;
                    $('#buttSave_' + clickItem.page_guid).click();
                }
            });
        }
    }
    else
        messageWindow("Ошибка", "Не удалось определить код строки");
}

function get_SelectedRowId_SpreadSheet(column) {
    var parent_id;
    var spreadsheet = $("#spreadsheet").data("kendoSpreadsheet");
    curr_row_ind = spreadsheet.activeSheet().selection()._ref.topLeft.row + 1;
    if (column == undefined)
        parent_id = spreadsheet.activeSheet().range(pk_column + curr_row_ind).value();
    else {
        curr_column = get_Column_ByPropName(column);
        parent_id = spreadsheet.activeSheet().range(curr_column + curr_row_ind).value();
    }
    return parent_id;
}

function get_RowInd_ById(id, propName) {
    let spreadsheet = $("#spreadsheet").data("kendoSpreadsheet");
    let column = pk_column;
    let ind;
    if (propName != undefined)
        column = get_Column_ByPropName(propName);
    spreadsheet.activeSheet().range(column + "2:" + column + rows_count).forEachCell(function (row, column, cellProperties) {
        if (cellProperties.value === String(id)) {
            ind =  row+1;
        }
    });
    return ind;
}

function get_Column_ByPropName(propName) {
    for (let key in arr_prop) {
        if (arr_prop[key] === propName)
            return all[key];
    }
}

function disable_Row(disable) {
    let spreadsheet = $("#spreadsheet").data("kendoSpreadsheet");
    let sheet = spreadsheet.activeSheet();

    if (disable) {
        sheet.range("A" + curr_row_ind + ":AZ" + curr_row_ind).enable(!disable);
    }
    else {
        sheet.range("A" + curr_row_ind + ":AZ" + curr_row_ind).forEachCell(function (row, column, cellProperties) {
            if (arr_readonly[column] == undefined) {
                let columnName = all[column];
                sheet.range(columnName + curr_row_ind).enable(true);
            }
        });
    }
}

function set_Unvisible_Columns() {
    let spreadsheet = find_SpreadSheet();
    if (spreadsheet == undefined)
        return null;

    let spreadsheet_data = spreadsheet.data("kendoSpreadsheet");
    let sheet = spreadsheet_data.activeSheet();

    for (let key in arr_unvisible_columns) {
        sheet.hideColumn(arr_unvisible_columns[key]);
    }
}

function find_SpreadSheet(page_guid) {
    let spreadsheet = undefined;
    if ($("#spreadsheet").length != 0)
        return $("#spreadsheet");

    let count = Top().frames.length;
    for (let i = 0; i < count; i++) {
        if (page_guid != undefined) {
            if (Top().frames[i].$("#spreadsheet[page_guid='" + page_guid + "']").length != 0)
                return Top().frames[i].$("#spreadsheet[page_guid='" + page_guid + "']");
        }
        else {
            if (Top().frames[i].$("#spreadsheet").length != 0)
                return Top().frames[i].$("#spreadsheet");
        }
    }

    return null;
}

function export_SpreadSheet() {
    let spreadsheet = find_SpreadSheet();
    spreadsheet.data("kendoSpreadsheet").saveAsExcel();
}

// **************************************************************************************************
//function DocumentStorageDownload(documentStorageId) {
//    if (entity_status == 1) {
//        var options = {};
//        options.url = "Home/";
//        options.type = "POST";
//        options.data = {};
//        options.data["id"] = documentStorageId;
//        var message = null;
//        $.each($("input[type='checkbox']"), function () {
//            var check_page_guid = $(this).attr("page_guid");
//            var value = $("#val_" + check_page_guid);
//            var value_checkbox = $("#checkbox_" + check_page_guid);
//            /*Комбобокс для даты*/
//            options.data[value.attr("name")] = value.val();
//            if (value_checkbox.length > 0) {
//                if (value_checkbox.attr("checked") == "checked")
//                    options.data[value_checkbox.attr("name")] = "on";
//                else options.data[value_checkbox.attr("name")] = "off";
//            }
//            if (value.attr("isrequired") != undefined && value.val() == "") {
//                message = "Не задано поле [" + value.attr("title") + "]";
//                return;
//            }
//        })
//        if (message != null) {
//            messageWindow("Ошибка", message);
//            return;
//        }
//        options.success = function (data) {
//            if (data.status === "error") {
//                messageWindow("Ошибка", data.message, data.error_text);
//            }
//            else if (data.status === "success") {
//                var tabStrip = $("#WizardTab_" + page_guid).data("kendoTabStrip");
//                var selectedTabIndex = 0;
//                var tabCount = 1;
//                if (tabStrip != undefined) {
//                    selectedTabIndex = tabStrip.select().index();
//                    tabCount = tabStrip.items().length;
//                }
//                //$(tabStrip.contentElement(selectedTabIndex + 1)).append(result);
//                //tabStrip.select(selectedTabIndex + 1);
//                //window.open('~/Content/test.PDF');
//                //                /window.open(data);
//                messageWindow("Отчеты", "Отчет сформирован");
//                downloadUrl = "DownloadFile_Execute?id=" + data.documentStorageId;
//                //downloadUrl = "TabReportDownloadResult?id=" + data.reportResultFileName;
//                //if (!window.open(downloadUrl))
//                //{
//                //location.href = downloadUrl;
//                // }
//                //window.location.assign("TabReportDownloadResult?reportResultFileName=" + data.reportResultFileName);
//                //window.location.href = "TabReportDownloadResult?reportResultFileName=" + data.reportResultFileName;
//                //http://stackoverflow.com/questions/1066452/easiest-way-to-open-a-download-window-without-navigating-away-from-the-page/7208039
//                /*
//                var hiddenIFrameID = 'hiddenDownloader',
//                iframe = document.getElementById(hiddenIFrameID);
//                if (iframe === null) {
//                    iframe = document.createElement('iframe');
//                    iframe.id = hiddenIFrameID;
//                    iframe.style.display = 'none';
//                    document.body.appendChild(iframe);
//                }
//                iframe.src = downloadUrl;
//                //http://javascript.ru/forum/events/48870-kak-skachat-fajjl-cherez-js.html
//                */
//                if (!window.open(downloadUrl)) {
//                    var hiddenIFrameID = 'hiddenDownloader';
//                    var link = document.createElement('a');
//                    link.setAttribute('href', downloadUrl);
//                    //link.setAttribute('download', data.reportResultFileName + "." + report_format);
//                    link.setAttribute('download', data.reportResultFileNameBase);
//                    onload = link.click();
//                }
//                // Закрываем tab
//                if (soperation_id == sop_Report || soperation_id == sop_ReportExport) {
//                    //window.parent.childTab_Close(parent_guid, page_guid);
//                }
//            } else {
//                messageWindow("Ошибка", "Произошла неуловимая ошибка при попытке сгенерировать отчет", "");
//            }
//        };
//        $.ajax(options);
//    }
//}