var filter_execute_status = 1;
function mainTableFilter_Execute() {
    try {
        formData.readInputs("myFilterForm_" + clickItem.page_guid);

        let butt_forward = $("#buttForward_" + clickItem.page_guid);
        if (butt_forward.length > 0 && butt_forward.attr("is_started") != null)
            return;

        if (filter_execute_status = 0)
            return;

        if (formData.errorMessage != undefined) {
            messageWindow("Ошибка", formData.errorMessage);
            return;
        }

        let tree_node_selected_id = null;
        if (is_true(vm_Base.isFilterForTree_Int)) {
            if (tree_node_selected == undefined) {
                messageWindow("Ошибка", "Не выбран узел");
                return;
            }
            tree_node_selected_id = tree_node_selected.data("id");
            if (tree_node_selected_id == 0) {
                messageWindow("Ошибка", tree_node_selected.attr("click_verify"));
                return;
            }
        }

        mainTableFilter_Execute_ChangeStatus(0);

        let filterPanel = $("#MainFilterTable_Filter_" + clickItem.page_guid);
        let mainTable_grid = $("#grid_" + clickItem.page_guid);
        let mainFilterTable_Content = $("#MainFilterTable_Content_" + clickItem.page_guid);
        if (mainFilterTable_Content.length == 0)
            mainFilterTable_Content = $('#table_content');

        formData.append("guids_out", clickItem.guids_out);

        var isSelectorFromFilter = filterPanel.attr("data-filter-selector");
        if (filterPanel.data("kendoResponsivePanel")) {
            filterPanel.data("kendoResponsivePanel").close();
        }

        if (is_true(vm_Base.isFilterForTree_Int) && tree_node_selected_id != null) {
            formData.append("StableTreeId", tree_node_selected_id);
        }

        //  если нас открыли по ссылке
        if (getParameterByName("id")) {
            formData.append("id", getParameterByName("id"));
            mainTable_CloseFilterPane();
        }

        if (mainTable_grid.length == 0 || is_true(vm_Base.isFilterForTree_Int)) {
            mainFilterTable_Content.empty();
            mainFilterTable_Content.addClass("loader-spin");
        }

        var uri_paths = [];
        uri_paths = clickItem.uri_path.split(';');
        var uri_path_pars = [];
        uri_path_pars = uri_paths[uri_paths.length - 2].split('_');
        var kendoName = "kendoGrid";
        if (uri_path_pars[0] == 108)
            kendoName = "kendoTreeList";

        var fileInput = $("#fileInput");
        if (fileInput.length > 0) {
            var fileInputControl = fileInput.data("kendoUpload");
            var files = fileInputControl.getFiles();
            if (files.length > 0) {
                for (var i = 0; i < files.length; i++)
                    formData.append("fileInput_" + i, files[i].rawFile);

                formData.append("upload_header_path", fileInput.attr("selectorheaderpath"));
                formData.append("guids_out", clickItem.guids_out);

                formData.append("save_operation", 5);
                if (mainTable_grid.length == 0 || is_true(vm_Base.isFilterForTree_Int)) {
                    clickItem.url_action = clickItem.url_action.replace("MainTable", "MainTablePost");
                }
                else {
                    clickItem.url_action = clickItem.url_action.replace("MainTable", "TabEditSave");
                }

                $.ajax({
                    url: clickItem.url_action,
                    type: "POST",
                    data: formData.formDataObject,
                    contentType: false,
                    processData: false,
                    success: function (result) {
                        if (mainTable_grid.length == 0 || vm_Base.isFilterForTree_Int == "1")
                            mainTableFilter_Execute_OnCompleted_First(result, filterPanel, mainFilterTable_Content);
                        else
                            mainTableFilter_Execute_OnCompleted_Second(result, mainTable_grid, kendoName);

                        mainTableFilter_Execute_ChangeStatus(1);
                    }
                });

                return;
            }
        }

        if (mainTable_grid.length == 0 || is_true(vm_Base.isFilterForTree_Int) || mainTable_grid.find('#panelbar_controller_error').length > 0) {
            formData.append("is_mobile", isMobile());
            $.ajax({
                url: clickItem.url_action,
                type: "POST",
                data: formData.toFormDataObject(),
                contentType: false,
                processData: false,
                success: function (result) {
                    mainTableFilter_Execute_OnCompleted_First(result, filterPanel, mainFilterTable_Content);
                    mainTableFilter_Execute_ChangeStatus(1);
                }
            });

        }
        else {
            var grid = mainTable_grid.data(kendoName);
            if (grid == undefined) {
                if (uri_path_pars[0] == 108)
                    grid = mainTable_grid.data(kendoName);
            }

            if (grid != undefined && grid.dataSource != null) {
                clickItem.url_action = clickItem.url_action.replace("MainTable", "TabEditSave");
                formData.append("save_operation", 5);
                $.ajax({
                    url: clickItem.url_action,
                    type: "POST",
                    data: formData.formDataObject,
                    contentType: false,
                    processData: false,
                    success: function (result) {
                        mainTableFilter_Execute_OnCompleted_Second(result, mainTable_grid, kendoName);
                        mainTableFilter_Execute_ChangeStatus(1);
                    }
                });
            }
        }

        return false;
    }
    catch (e) {
        if (Top().isAutoTestClient)
            Top().set_Auto_Test_Log(e);
        else
            throw e;
    }
}

function mainTableFilter_AddExecute(url_Action, page_guid) {
    var mainFilterTable_Content = $("#MainFilterTable_Content_" + page_guid);
    mainFilterTable_Content.empty();
    mainFilterTable_Content.addClass("loader-spin");

    var options = {};
    options.data = {};

    options.url = url_Action;
    options.type = "GET";
    options.data["is_mobile"] = isMobile();

    options.success = function (result) {

        mainFilterTable_Content.html(result);
        mainFilterTable_Content.removeClass("loader-spin");

        mainTable_CloseFilterPane();
    };

    $.ajax(options);

    return false;
}

function mainTableFilter_Execute_ChangeStatus(filter_execute_status) {

    var filter_execute_button = $("#buttForward_" + vm_Base.page_guid);
    var filter_execute_button_img = filter_execute_button.find("img");
    if (filter_execute_status == 1) {
        filter_execute_button_img.attr("src", vm_Base.url_Content + "Images/Operations/Large/ArrowForward_Green.png");
    }
    else {
        filter_execute_button_img.attr("src", vm_Base.url_Content + "Images/Operations/Large/ArrowForward_Disable.png");
    }
}

function mainTableFilter_Execute_OnCompleted_First(result, filterPanel, mainFilterTable_Content) {
    filterPanel.removeClass("loader-spin");

    let page_guid = clickItem.page_guid;
    let selector_header_path = clickItem.selector_header_path;
    mainFilterTable_Content.html(result);
    mainFilterTable_Content.removeClass("loader-spin");

    if (selector_header_path == undefined || selector_header_path === "")
        mainTable_CloseFilterPane();
    else {
        mainTableSplitter_LastPane_Collapse(page_guid, true);
        clickItem.selector_header_path = undefined;
    }
}

function mainTableFilter_Execute_OnCompleted_Second(result, mainTable_grid, kendoName) {

    mainTable_grid.attr("filter_string", result.filter_string);
    mainTable_grid.data("loaded", false);

    var grid = mainTable_grid.data(kendoName);
    grid.dataSource.transport.options.read.data["guids_out"] = clickItem.guids_out;
    grid.dataSource.transport.options.read.data["uri_path"] = clickItem.uri_path;
    grid.dataSource.transport.options.read.data["filter_string"] = result.filter_string;

    if (kendoName == "kendoGrid")
        grid.dataSource.page(1);
    else if (kendoName == "kendoTreeList")
        grid.dataSource.read();

    grid.dataSource.read();
    grid.refresh();

    if (clickItem.selector_header_path == undefined || clickItem.selector_header_path === "")
        mainTable_CloseFilterPane();
    else
        mainTableSplitter_LastPane_Collapse(clickItem.page_guid, true);
}

function mainTableFilter_Refresh(page_guid) {
    let filter_form = $("#myFilterForm_" + clickItem.page_guid);

    filter_form.empty();
    filter_form.addClass("loader-spin");

    let options = {};
    options.url = vm_Base.url_Content + "Home/MainFilter";
    options.type = "GET";

    options.data = {};
    options.data._isPostBack = "true";

    if (vm_Base.parent_id != undefined)
        options.data.parent_id = vm_Base.parent_id;
    options.data.guids_out = vm_Base.guids_out;
    options.data.uri_path = vm_Base.uri_path;
    if (vm_Base.selector_filter_dynamic != undefined && vm_Base.selector_filter_dynamic != "")
        options.data.selector_filter_dynamic = vm_Base.selector_filter_dynamic;
    options.data.random_value = getRandomValue();

    if (vm_Base.selector_content_type != undefined && vm_Base.selector_content_type != "")
        options.data.selector_content_type = vm_Base.selector_content_type;

    if (vm_Base.selector_result != undefined && vm_Base.selector_result != "")
        options.data.selector_result = vm_Base.selector_result;

    if (vm_Base.selector_value != undefined && vm_Base.selector_value != "")
        options.data.selector_value = vm_Base.selector_value;

    options.success = function (result) {
        filter_form.html(result);
        filter_form.removeClass("loader-spin");
    };

    $.ajax(options);
}

function mainTableFilter_Cancel() {
    //Проверка если меню disabled. Почему-то клик на таком меню все равно срабатывает
    if ($('#buttCancel_' + clickItem.page_guid).length > 0 && $('#buttCancel_' + clickItem.page_guid).attr('class').indexOf('k-state-disabled') != -1)
        return;

    clickItem.is_selector_from_filter = is_true(clickItem.is_selector_from_filter);

    filterTab_Close(clickItem.page_guid, clickItem.isSelectorFromFilter);

    return false;
}

function mainTableFilter_AddCancel(page_guid, selector_guid, guids_out) {
    var filterPanel = $("#MainFilterTable_Filter_" + selector_guid);
    var editorToolBal = $("#TabEdit_ToolBar_" + page_guid);
    var editorContent = $("#TabEditContent_" + page_guid);
    var mainSplitter_closest = filterPanel.closest(".mainSplitter");

    if (mainSplitter_closest.length > 0
        && editorToolBal.length > 0
        && editorContent.length > 0
        && mainSplitter_closest.data("kendoSplitter")
        && mainSplitter_closest.find(".k-pane:first").hasClass("k-state-collapsed")
        ) {
        mainSplitter_closest.data("kendoSplitter").expand(".k-pane:first");

        editorToolBal.parent().empty();
        editorContent.parent().empty();
    }

    return false;
}

function mainTableFilterConfig_Open() {
    var filter_content = $("#MainFilterTable_Config_" + clickItem.page_guid);

    filter_content.empty();
    filter_content.addClass("loader-spin");

    var options = {};
    options.url = vm_Base.url_Content + "Home/MainFilterConfig";
    options.type = "GET";

    options.data = {};

    options.data.page_guid = clickItem.page_guid;
    options.data.guids_out = vm_Base.guids_out;
    options.data.uri_path = clickItem.uri_path;

    options.success = function (result) {
        filter_content.html(result);
        filter_content.removeClass("loader-spin");
    };

    $.ajax(options);
}

function mainTable_OpenFilterPane(page_guid, filter_from_button) {
    var filterButton = $("#buttSearch_" + clickItem.page_guid);
    var filterPanel = $("#MainFilterTable_Filter_" + clickItem.page_guid);
    if (filterPanel.length > 0 && clickItem.filter_from_button != undefined)
        filterPanel.attr("filter_from_button", "True");

    if (isMobile()) {
        if ($("#filter_content").length > 0) {
            $("#filter_content").attr("style", "");
        }
    }

    var mainTableSplitter = $("#MainTableSplitter_" + clickItem.page_guid);
    var mainSplitter_closest = mainTableSplitter.closest(".mainSplitter");
    //var mainSplitter_closest = $("#MainFilterTableSplitter_" + page_guid);

    var filterSplitter = $("#MainFilterTableSplitter_" + clickItem.page_guid);
    if (filterSplitter.length > 0)
        filterSplitter.focus();

    if (mainSplitter_closest.length > 0) {
        if (mainSplitter_closest.find(".filterPanel").data("kendoResponsivePanel")) {
            if (mainSplitter_closest.find(".filterPanel").hasClass("k-rpanel-expanded")) {
                mainSplitter_closest.find(".filterPanel").data("kendoResponsivePanel").close();
                filterButton.attr("title", filterButton.attr("title_hint2"));
            } else {
                mainSplitter_closest.find(".filterPanel").data("kendoResponsivePanel").open();
                filterButton.attr("title", filterButton.attr("title_hint"));
            }
        }

        if (mainSplitter_closest.data("kendoSplitter")) {
            if (mainSplitter_closest.find(".k-pane:first").hasClass("k-state-collapsed")) {
                mainSplitter_closest.data("kendoSplitter").expand(".k-pane:first");
                filterButton.attr("title", filterButton.attr("title_hint2"));
            } else {
                mainSplitter_closest.data("kendoSplitter").collapse(".k-pane:first");
                filterButton.attr("title", filterButton.attr("title_hint"));
            }
        }
    }
}

function mainTable_CloseFilterPane() {
    var filterPanel = $("#MainFilterTable_Filter_" + clickItem.page_guid);
    if (filterPanel.data("kendoResponsivePanel")) {
        filterPanel.data("kendoResponsivePanel").close();
    }

    if (isMobile()) {
        if ($("#filter_content").length > 0) {
            $("#filter_content").attr("style", "display: none;");
        }
    }

    var mainFilterTable_Splitter = $("#MainFilterTableSplitter_" + clickItem.page_guid);
    var mainFilterTable_Splitter_data = mainFilterTable_Splitter.data("kendoSplitter");
    if (mainFilterTable_Splitter_data) {
        mainFilterTable_Splitter_data.collapse(".k-pane:first");
    }

    var collapse = true;
    /*Если есть открытые вкладки при вызове фильтра их не закрываем*/
    if (filterPanel.attr("filter_from_button") != undefined
        && mainFilterTable_Splitter.find("div.childTabs > div.k-content").length > 0)
        collapse = false;
    mainTableSplitter_LastPane_Collapse(clickItem.page_guid, collapse);
}

function mainTableSplitter_LastPane_Collapse(page_guid, collapse) {
    var mainTableSplitter = $("#MainTableSplitter_" + page_guid);
    if (mainTableSplitter.length == 0 && clickItem.sender != undefined) {
        if (clickItem.sender.attributes != undefined && is_true(clickItem.sender.getAttribute("open_grandchild_grid"))) {
            mainTableSplitter = $('#TabTableSplitter_' + page_guid);
        }
    }

    if (mainTableSplitter.length != 0) {
        var mainTableSplitter_data = mainTableSplitter.data("kendoSplitter");
        if (mainTableSplitter_data) {
            if (collapse == true)
                mainTableSplitter_data.collapse(".k-pane:last");
            else
                mainTableSplitter_data.expand(".k-pane:last");
        }
    }
}

function mainTableFilter_Checkbox_Clear() {
    if (clickItem._page_guid == undefined)
        return;

    let changes = $('#myFilterForm_' + clickItem._page_guid + ' [changed="Changed"]');
    if (changes.length > 0) {
        for (let i = 0; i < changes.length; i++)
        {
            let page_guid = $(changes[i]).attr("page_guid");
            if (page_guid != undefined) {
                $('#check_' + page_guid).click();
            }
        }
    }
}