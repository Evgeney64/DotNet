function grid_ReadData(page_guid, guids_out, uri_path) {
    var mainTable_grid = $("#grid_" + page_guid);
    var filter_string = mainTable_grid.attr("filter_string");
    return {
        guids_out: guids_out,
        uri_path: uri_path,
        filter_string: filter_string,
    };
}

function grid_GetTreeData(page_guid, guids_out, uri_path) {
    return function (e) {
        let id = e.id;

        let dataItem = undefined;
        if (id != undefined)
            dataItem = $("#grid_" + page_guid).data("kendoTreeList").dataSource.get(id);
        
        let mainTable_grid = $("#grid_" + page_guid);
        let filter_string = mainTable_grid.attr("filter_string");

        return {
            treeDataItem: (dataItem != undefined) ? JSON.stringify(dataItem) : "",
            guids_out: guids_out,
            uri_path: uri_path,
            filter_string: filter_string,
        };
    };
};

function grid_DataBound(vm_Base_params, page_guid) {
    try {
        vm_Base.initializeArguments(arguments);

        var jQgrid = $('#grid_' + page_guid);
        var grid_data = jQgrid.data("kendoGrid");

        var is_select = grid_SelectRow_fromVmBase(page_guid);
        if (!is_select)
            grid_data.select("tbody tr:eq(0)");

        var row = grid_data.select(1);

        if (vm_Base.clientController != "TabEditPage")
            vm_Base.soperation_id = getUriPath_Soperation(vm_Base.uri_path_out, 101, ';');
        var soperation_id = vm_Base.soperation_id;

        if (vm_Base.soperation_id != sop_SelectorFromFilter
            && vm_Base.soperation_id != sop_SelectorFromEditor
            && vm_Base.soperation_id != sop_SelectorFromPeriodValues
            && vm_Base.selector_type_from == "None"
            && (vm_Base.soperation_id != sop_5 || (vm_Base.soperation_id == sop_5 && is_true(vm_Base.grid_has_grandchild)))
            && row.length > 0
            && vm_Base.clientController != "TabEdit"
            && !isGridGrandchild()
        ) {
            var grid = $("#grid_" + vm_Base.page_guid);
            if (!is_true(grid.attr("is_autotabs_opened")))
                grid_AutoChildTabs(grid);
        }

        /*Работа 6102. Меняем в ячейках грида с контекстным меню значения null на пустую строку*/
        if ($(".grid-cell-btn").length > 0) {
            $(".grid-cell-btn").each(function (i, item) {
                let span = $(item).parent();
                if (span.length > 0) {
                    let html = span.html();
                    if (span[0].outerText === "null")      
                        span.html(html.replace("null", ""));
                    if (span[0].outerText === "undefined")
                        span.html(html.replace("undefined", ""));
                }
            });
        }

        /*Работа 6045. По двойному клику на гриде открываем редактор*/
        if ($('.k-grid-content').attr("is_dblclick") == undefined) {
            $('.k-grid-content').on("dblclick", function (e) {
                let button_edit = $('.butt_edit');
                if (button_edit.length > 0)
                    button_edit[0].click();
            });
            $('.k-grid-content').attr("is_dblclick", true);
        }

        /*Работа 6299. Некоторым строкам и тексту в них присваиваются цвета*/
        if (jQgrid.attr("colored_rows_field") != undefined) {
            grid_SetColor(page_guid);
        }

        /*Работа 6945. После обновления грида надо выбрать нужную строку*/
        //if (vm_Base.isNeedSelect) {
        //    var row_uid = grid_Get_RowGuid_ByField(vm_Base.new_item_json, vm_Base.pk_field);
        //    if (row_uid != undefined)
        //        grid_SelectRow_byUid(vm_Base.page_guid, row_uid);
        //    vm_Base.isNeedSelect = false;
        //}

        if (Top().isAutoTestClient == true) {
            setTimeout(function () {
                if (vm_Base.clientController == "MainFilter")
                    return;

                if (grid_data.dataSource.total() == 0)
                    this.isGridEmpty = true;
                else
                    this.isGridEmpty = false;
                
                if (soperation_id == 6 || soperation_id == 191 || soperation_id == 192 || soperation_id == 14) {
                    if (vm_Base.IsFilterExists == "0")
                        Top().end_Auto_Test_Item(1);
                    Top().end_Auto_Test_Item(2);
                }
                else {
                    Top().end_Auto_Test_Item();
                }
            }, 500);
        }
    }
    catch (e) {
        if (Top().isAutoTestClient)
            Top().set_Auto_Test_Log(e);
        else
            throw e;
    }
}

function grid_DataBound_ControllerError(e, page_guid) {
    var grid = $('#grid_' + page_guid);
    var grid_content = grid.find('.k-grid-content');
    grid_content.html(e.errors);
}

function gridTreeList_DataBound(vm_Base_params, page_guid) {
    try {
        //initialize(arguments, true);
        vm_Base.initializeArguments(arguments);

        var jQgrid = $('#grid_' + page_guid);
        //if (Top().isAutoTestNew && Top().doubleEvent != 1) {
        //    Top().doubleEvent = 1;
        //    if (vm_Base.soperation_id == 6 || vm_Base.soperation_id == 191 || vm_Base.soperation_id == 192 || vm_Base.soperation_id == 14)
        //        setTimeout(function () { exec_Custom_Event("FilterTab", "autoTestRunLevelTest", 3); }, 500);
        //    else
        //        setTimeout(function () { parent.exec_Custom_Event("FilterTab", "autoClickTabClose"); }, 500);
        //}

        if (Top().isAutoTestClient == true) {
            setTimeout(function () {
                if (vm_Base.soperation_id == 192 || vm_Base.soperation_id == 14)
                    Top().end_Auto_Test_Item(1);

                if (vm_Base.soperation_id == 6 || vm_Base.soperation_id == 191 || vm_Base.soperation_id == 192 || vm_Base.soperation_id == 14)
                    Top().end_Auto_Test_Item(2);
                else
                    Top().end_Auto_Test_Item();
            }, 500);
        }

        // раскрываем первый узел дерева
        var childTreeList = jQgrid.data("kendoTreeList");
        if (childTreeList) {
            var firstNode = $("#grid_" + vm_Base.page_guid + " tbody>tr:eq(0)");
            if (firstNode.length > 0) {
                if (is_true(vm_Base.is_auto_expand)) {
                    var isExpanded = $(firstNode).attr("aria-expanded");
                    if (typeof isExpanded === 'undefined' || isExpanded !== 'true') {
                        childTreeList.expand(firstNode);
                        return;
                    };
                };

                // пробуем найти строку с меткой IsMarked и выбираем ее, иначе выбираем первую строку
                if (!jQgrid.data("loaded")) {
                    jQgrid.data("loaded", true);

                    var flag = false;
                    var dataItems = childTreeList.dataSource.data();
                    $.each(dataItems, function (i, dataItem) {
                        if (dataItem.IsMarked === true) {
                            var item = childTreeList.itemFor(dataItem);
                            childTreeList.select(item);
                            flag = true;
                            return false;
                        }
                    });
                    if (!flag) {
                        childTreeList.select(firstNode);
                    }
                }

                if (vm_Base.soperation_id != sop_SelectorFromFilter
                    && vm_Base.soperation_id != sop_SelectorFromEditor
                    && vm_Base.soperation_id != sop_SelectorFromPeriodValues
                    && vm_Base.selector_type_from == "None"
                    //&& row.length > 0
                ) {
                    var grid = $("#grid_" + vm_Base.page_guid);
                    if (grid.attr("is_autotabs_opened") == undefined)
                        grid_AutoChildTabs(grid);
                }
            };
        };

        /*Работа 6045. По двойному клику на гриде открываем редактор*/
        if ($('.k-grid-content').attr("is_dblclick") == undefined) {
            $('.k-grid-content').on("dblclick", function (e) {
                let button_edit = $('.butt_edit');
                if (button_edit.length > 0)
                    button_edit[0].click();
            });
            $('.k-grid-content').attr("is_dblclick", true);
        }
    }
    catch (e) {
        if (Top().isAutoTestClient)
            Top().set_Auto_Test_Log(e);
            //set_AutoTest_Error(e);
        else
            throw e;
    }
}

function grid_AutoChildTabs(grid) {
    //Для автотеста не надо открывать автотабы. Он с ними работает 
    if (Top().isAutoTestClient == true)
        return;

    var uri_path_main_menu = getUriPath_FromStr_ByType(vm_Base.uri_path_out, 110);
    //var uri_path_main_grid = getUriPath_FromStr_ByType(vm_Base.uri_path_out, 101);
    var uri_path_main_grid = getUriPath_FromStr_Last(vm_Base.uri_path_out);
    if (uri_path_main_menu == null || uri_path_main_grid == null)
        return;

    /*Работа 6044. Добавил возможность открытия атвотабов во втором уровне для зависимых списков*/
    if (vm_Base.soperation_id == sop_5
        || vm_Base.soperation_id == sop_6
        || vm_Base.soperation_id == sop_14
        || vm_Base.soperation_id == sop_16
        || vm_Base.soperation_id == sop_191
        || vm_Base.soperation_id == sop_192
        || vm_Base.soperation_id == sop_3
        || vm_Base.soperation_id == sop_2
        || vm_Base.soperation_id == sop_26
    ) {
        var autoOpenMenus = $('[is_auto_open="True"] span.k-link');
        for (var j = 0; j < autoOpenMenus.length; j++) {
            if (vm_Base.auto_selected_row != undefined)
            {
                var item = grid.data("kendoGrid").dataSource.get(vm_Base.auto_selected_row);
                var tr = $("[data-uid='" + item.uid + "']", grid.data("kendoGrid").tbody);
                grid.data("kendoGrid").select(tr);
                vm_Base.auto_selected_row = undefined;
            }

            //if (vm_Base.soperation_id == sop_3 || vm_Base.soperation_id == 2 || vm_Base.soperation_id == 5)
            if (vm_Base.clientController == "TabEdit")
                autoOpenMenus[j].setAttribute("mainTabEdit", true);
            autoOpenMenus[j].click();
        }
        if (autoOpenMenus.length > 0 && grid != undefined) //Загрузка может происходить много раз. Автотабы должны открываться только один
            grid.attr("is_autotabs_opened", "True");
    }
}

function grid_DataBound_InitProgress(page_guid, field, fieldCount, fieldTotal) {
    // функция для установки значений прогресс бара
    var jQgrid = $('#grid_' + page_guid);

    var setProgress = function (id, count, total, init) {
        var progressBarElement = jQgrid.find("#progress_" + field + id);
        if (progressBarElement != null) {
            if (init) {
                eval(progressBarElement.siblings("script").last().html());
                progressBarElement.width("95%");
            }
            var progressBar = progressBarElement.data("kendoProgressBar");
            if (progressBar != undefined) {
                progressBar.options.max = total;
                progressBar.value(count);
            }
        }
    };
    // инициализация прогресс-бара
    jQgrid.data("kendoGrid").dataSource.data().forEach(function (e) {
        setProgress(e.id, e.get(fieldCount), e.get(fieldTotal), true);
    });
    // подключение к хабу
    var task = $.connection.taskHub;
    task.client.executionDetailStatusChanged = function (data) {
        if (data.Status == 4 || data.Status == 6)
        {
            setProgress(data.ExecutionId, data.Count, data.Total, false);
        }
    };
    $.connection.hub.start();
}

var main_grid_row_uid = null;
function grid_Change(page_guid, is_from_tab) {

    /*Если открыли грид из селектора в редакторе в MainTab, то зависимые списки не надо обновлять*/
    /*Добавлено дополнительное условие vm_Base.soperation_id != sop_5. Если у карточки есть зависимые списки с 3м уровнем*/
    if ((vm_Base.mainTab_sconfig_type_id == "102" && vm_Base.soperation_id != sop_5 && vm_Base.soperation_id != sop_13)
        || vm_Base.SOperation == "SelectorFromFilter"
        || vm_Base.SOperation == "SelectorFromEditor"
    ) {
        return;
    }

    var row_id = getRowId_ByGuid(page_guid);

    //if (main_grid_row_uid != row_id) {
    //    main_grid_row_uid = row_id;

        var frames = window.frames;
        if (row_id != null && frames.length > 0) {
            for (var i = 0; i < frames.length; i++) {

                var $frame = $(frames[i].frameElement);

                /*Работа 6044. Дочерний список обновляем только в момент активации вкладки*/
                if ($frame.parent().length > 0) {
                    if ($frame.parent()[0].className.indexOf('k-state-active') == -1)
                        continue;
                }

                var fr_page_guid = $frame.attr("page_guid");
                if (fr_page_guid == undefined)
                    continue;
                var fr_soperation_id = $frame.attr("soperation_id");

                if (fr_soperation_id == sop_5
                    || fr_soperation_id == sop_19
                    || fr_soperation_id == sop_54
                    || fr_soperation_id == sop_14
                ) {
                    if (frames[i].$ == undefined)
                        return;

                    var child_grid = frames[i].$("#grid_" + fr_page_guid);
                    var child_form = frames[i].$("form").parent();

                    var child_grid_data = null;
                    if (child_grid.length > 0) {
                        var kendoObject = child_grid.attr("kendoObject");
                        child_grid_data = child_grid.data(kendoObject);

                        if (is_from_tab && child_grid.attr("parent_id") == row_id)
                            continue;

                        if (child_grid_data.dataSource != null) {
                            if (child_grid.attr("is_fixing_grid") == undefined) {

                                if (fr_soperation_id == sop_54) {
                                    var uri_action = child_grid.attr("uri_action");
                                    if (uri_action != undefined) {
                                        var options = {};
                                        options.url = uri_action;
                                        options.type = "POST";

                                        options.data = {};
                                        options.data.parent_id = row_id;
                                        options.data.list_reopen = 1;

                                        options.success = function (data) {
                                            child_form.html("");
                                            child_form.html(data);

                                            //child_grid.html("");
                                            //child_grid.html(data);
                                        };
                                        $.ajax(options);
                                    }
                                }
                                else if (fr_soperation_id != sop_23) {
                                    //Если выбрано несколько строк, зависимый грид отображаем только для последней
                                    if (typeof (row_id) == "string" && row_id.split(";").length > 1)
                                        row_id = row_id.split(";")[row_id.split(";").length - 2];

                                    child_grid.attr("parent_id", row_id);
                                    var parent_id = row_id;
                                    var filter_string_child = child_grid.attr("filter_string");
                                    if (parent_id != "" && parent_id != undefined) {
                                        child_grid_data.dataSource.transport.options.read.url = addToUrl(child_grid_data.dataSource.transport.options.read.url, "parent_id", parent_id);
                                        child_grid_data.options.dataSource.transport.read.url = addToUrl(child_grid_data.dataSource.transport.options.read.url, "parent_id", parent_id);
                                    }
                                    if (filter_string_child != "" && filter_string_child != undefined) {
                                        child_grid_data.dataSource.transport.options.read.url = addToUrl(child_grid_data.dataSource.transport.options.read.url, "filter_string", filter_string_child);
                                        child_grid_data.options.dataSource.transport.read.url = addToUrl(child_grid_data.dataSource.transport.options.read.url, "filter_string", filter_string_child);
                                    }

                                    if (fr_soperation_id == sop_19)
                                        child_grid_data.dataSource.read();
                                    else
                                        child_grid_data.dataSource.page(1);
                                }
                            }
                        }
                    }
                }
                else if (fr_soperation_id == sop_61) {
                    if (is_true($frame.attr("tab_from_info_link")))
                        continue;

                    var url = $frame.attr("src");
                    url = updateQueryStringParameter(url, "parent_id", row_id);
                    url = updateQueryStringParameter(url, "temp_value", getRandomValue());
                    url = url.replace("Home/TabEdit_Layout", "Home/Info");

                    var $tab_content = frames[i].$("#TabEditContent_" + fr_page_guid);

                    var options = {};
                    options.url = url;
                    
                    options.success = function (data) {
                        if (fr_soperation_id == sop_61) {
                            $tab_content.html("");
                            if (data.status == "success" && data.data != undefined && data.data != "")
                                $tab_content.html(data.data);
                            else
                                $tab_content.html(data);
                        }
                    };
                    $.ajax(options);
                }
            }
        }

        //Цикл по незагруженным вкладкам, чтобы обновить там текущий parent_id
        var divs_empty = $('.div_empty');
        if (row_id != null && divs_empty.length > 0) {
            for (var i = 0; i < divs_empty.length; i++) {
                var src = $(divs_empty[i]).attr('src');
                $(divs_empty[i]).attr('src', updateQueryStringParameter(src, 'parent_id', row_id))
            }
        }
    //}

    linksChangeColor(page_guid);
}

function getRowId_ByGuid(page_guid, grid_selected_rows) {

    var row;
    var $grid = null;
    var editor = $('#editor_content');

    var $targetFrame = $("#ChildTabFrame_" + page_guid);
    if ($targetFrame.length > 0) {
        //Для грида внутри фрейма почему-то не работает .data("kendoGrid")
        return $targetFrame[0].contentWindow.getRowId_ByGuid(page_guid, grid_selected_rows);
    }
    else {
        $grid = $("#grid_" + page_guid);
        if (isMobile())
            $grid = $("#listView_" + page_guid);
    };
    if ($grid.length != 0) {
        var kendoObject = "kendoGrid";
        if (isMobile())
            kendoObject = "kendoListView";
        else if ($grid.data("role") == "treelist")
            kendoObject = "kendoTreeList";
        var grid_data = $grid.data(kendoObject);

        if (kendoObject == "kendoGrid")
            row = grid_data.select(1);
        else // kendoListView, kendoTreeList
            row = grid_data.select();
    }
    
    /*Для редакторов из карточки*/
    /*Если грид в редакторе, то код строки берется из атрибутов редактора*/
    if (/*row == undefined && */editor.length != 0 && editor.attr('parent_id') != undefined && editor.attr('parent_id') != "") {
        //if (editor.attr('parent_id') != undefined)
            return editor.attr('parent_id');
    }

    /*Для копки в строке грида*/
    if (row == undefined) {
        if (vm_Base != undefined && vm_Base.sender != undefined && vm_Base.sender.getAttribute("row_id") != null)
            return vm_Base.sender.getAttribute("row_id");
    }

    if (row == undefined || row.length === 0)
        return null;
    
    var grid_data_row = grid_data.dataItem(row);
    vm_Base.curr_id = grid_data_row.id;
    //vm_Base.new_item_json = grid_data_row;
    // Объект (для SYS_TABLE_TREE)
    //if (row.length == 1
    //    && grid_data_row.STABLE_ID != null && grid_data_row.ID != null) {
    //    return grid_data_row;
    //}

    var rows_count = 1;
    if (grid_selected_rows != undefined && grid_selected_rows == 2) {
        // Весь список
        rows_count = grid_data.items().length;
    }
    else if ($grid.attr("multi_select") != undefined && $grid.attr("multi_select")) {
        // Выделенные строки
        rows_count = row.length;
    }

    if (rows_count > 1) {
        var ids = "";
        for (var i = 0; i < rows_count; i++) {
            var id = grid_data.dataItem(row[i]).id;
            ids = ids + id + ";";
        }
        return ids;
    }
    else {
        // Одна строка
        return String(grid_data_row.id);
    }
}

function get_Grid_ColumnValue(column_name) {
    if (vm_Base != undefined && vm_Base.sender != undefined && vm_Base.sender.getAttribute("row_id") != null)
        return vm_Base.sender.getAttribute("row_id");

    let $grid = $("#grid_" + vm_Base.page_guid);
    if (isMobile())
        $grid = $("#listView_" + vm_Base.page_guid);
    if ($grid.length == 0)
        return null;

    let kendoObject = "kendoGrid";
    if (isMobile())
        kendoObject = "kendoListView";
    else if ($grid.data("role") == "treelist")
        kendoObject = "kendoTreeList";
    let grid_data = $grid.data(kendoObject);

    let row;
    if (kendoObject == "kendoGrid")
        row = grid_data.select(1);
    else // kendoListView, kendoTreeList
        row = grid_data.select();
    if (row.length === 0)
        return null;

    let grid_data_row = grid_data.dataItem(row);

    if (grid_data_row != null) {
        let first_column_name = column_name
        if (column_name == null)
            first_column_name = grid_data.columns[0].field;
        return grid_data_row[first_column_name];
    }
    return "";
}

function getRowId_ByGuid_0(page_guid) {

    var row_id = getRowId_ByGuid(page_guid);
    if (row_id == null)
        return null;

    var row_id0 = row_id;
    var index_semicolon = row_id.indexOf(";");
    if (index_semicolon >= 0) {
        row_id0 = row_id.substring(0, index_semicolon);
    }
    return row_id0;
}


function getRowId_ByGuid_FromFrames(page_guid, clientController_Prev, grid_selected_rows) {
    var frames = window.parent.frames;
    if (clientController_Prev == "MainTable") {
        for (var i = 0; i < frames.length; i++) {
            var grid = frames[i].$("#grid_" + page_guid);
            if (grid.length > 0) {
                var row_id = frames[i].getRowId_ByGuid(page_guid, grid_selected_rows);
                return row_id;
            }
        }
    }
    if (clientController_Prev == "TabTable") {
        for (var i = 0; i < frames.length; i++) {
            var frames1 = frames[i].frames;
            if (frames1.length > 0) {
                for (var i1 = 0; i1 < frames1.length; i1++) {
                    try {
                        if (frames[i1] !== undefined && frames[i1].$ !== undefined) {
                            var grid1 = frames1[i1].$("#grid_" + page_guid);
                            if (grid1.length > 0) {
                                var row_id = frames1[i1].getRowId_ByGuid(page_guid, grid_selected_rows);
                                return row_id;
                            }
                        }
                    } catch (e) { }
                }
            }
        }
    }
}

function getGrid_FromFrames(page_guid, clientController_Prev) {
    var frames = window.parent.frames;
    if (clientController_Prev == "MainTable") {
        for (var i = 0; i < frames.length; i++) {
            var grid = frames[i].$("#grid_" + page_guid);
            if (grid.length > 0)
                return grid;
        }
    }
    if (clientController_Prev == "TabTable") {
        for (var i = 0; i < frames.length; i++) {
            var frames1 = frames[i].frames;
            if (frames1.length > 0) {
                for (var i1 = 0; i1 < frames1.length; i1++) {
                    if (frames1[i1].length > 0) {
                        var grid1 = frames1[i1].$("#grid_" + page_guid);
                        if (grid1.length > 0)
                            return grid1;
                    }
                }
            }
        }
    }
}

function grid_RefreshRow(parent_guid, soperation_id, new_item_str, pk_field, refresh_full, fk_field) {
    let $grid = $("#grid_" + parent_guid);
    let grid = $grid.data("kendoGrid");
    if (grid == null || grid == undefined) {
        grid = $grid.data("kendoTreeList");
    }

    if (grid == null || grid == undefined)
        return;

    let grid_ds = grid.dataSource;
    let grid_ds_data = grid_ds.data();

    let row = null;
    let row_uid = null;
    let new_item_json = undefined;
    if (new_item_str != undefined)
        new_item_json = JSON.parse(new_item_str);
    if (new_item_json != undefined)
        vm_Base.new_item_json = new_item_json;

    // Добавить || Корректировать
    if ((soperation_id == sop_2
        || soperation_id == sop_3
        || soperation_id == sop_12
        || soperation_id == sop_13
        || soperation_id == sop_26
        || soperation_id == 115
        )
        && (new_item_json != undefined)
        ) {

        if (is_true(refresh_full)) {
            grid.dataSource.read();
            vm_Base.isNeedSelect = true;
            return;
        }

        if (vm_Base.refresh_grid == "parent" && vm_Base.fk_field != undefined)
            row_uid = grid_Get_RowGuid_ByField(new_item_json, fk_field);
        else
            row_uid = grid_Get_RowGuid_ByField(new_item_json, pk_field);
        row = $grid.find("tr[data-uid='" + row_uid + "']");

        if (soperation_id == sop_2
            || soperation_id == sop_12
            || soperation_id == sop_13
            || soperation_id == sop_26
        ) {
            if ($grid.data("kendoTreeList") != undefined) {
                grid.dataSource.pushCreate(new_item_json);
            }
            else {
                grid.dataSource.insert(0, new_item_json);
                grid_SelectRow_byUid(parent_guid, grid_ds_data[0].uid);
            }
            return;
        }

        if (row != null && new_item_json != null) {

            var cells = $(row).children('td[role="gridcell"]');

            for (var i = 0; i < grid.columns.length; i++) {
                var column = grid.columns[i];
                var cell = cells.eq(i);

                var fieldValue = null;
                if (column.template != undefined) {
                    fieldValue = column.template;
                    if (typeof (fieldValue) == 'function') {
                        // Дерево
                        fieldValue = fieldValue(new_item_json);
                        var count = cell.find('span.k-i-none, span.k-i-expand, span.k-i-collapse').length;
                        if (count > 0) {
                            if ($(fieldValue)[0] != undefined && $(fieldValue)[0].id == "cell_template" && cell.find('#cell_template') != undefined) {
                                cell.find('#cell_template').html($(fieldValue).html());
                            }
                            else {  //старая реализация. Оставил на всякий случай. Может где-то еще используется
                                var el_nextSibling = cell.find('span.k-i-none, span.k-i-expand, span.k-i-collapse')[count - 1].nextSibling;
                                if (el_nextSibling) {
                                    el_nextSibling.outerHTML = fieldValue;
                                } else {
                                    cell.html(cell.find('span.k-i-none, span.k-i-expand, span.k-i-collapse')[count - 1].outerHTML + fieldValue);
                                }
                            }
                        }
                        else
                            cell.html(fieldValue);
                    }
                    else {
                        var kendoTemplate = kendo.template(column.template);
                        cell.html(kendoTemplate(new_item_json));
                        /*
                        // Grid
                        if (1 == 1) {
                            //var $kendoOutput;
                            //with (new_item_json) {
                            //    $kendoOutput = "<span><img src='/" + (new_item_json.ProductImageSrc) + "' />&nbsp&nbsp" + (new_item_json.NPRODUCT_NAME) + "</span>";
                            //}
                            //fieldValue = $kendoOutput;

                            // <span><img src='/#=data.ProductImageSrc#' />&nbsp&nbsp#=data.NPRODUCT_NAME#</span>
                            // ограничение в 100 параметров в шаблоне
                            var beg = 0;
                            //while (1 == 1) {
                            for (var j = 0; j < 100; j++) {
                                var beg = fieldValue.indexOf('#=data.', beg);
                                if (beg < 0)
                                    break;
                                var end = fieldValue.indexOf('#', beg + 1);
                                if (end <= beg + 7)
                                    break;

                                var template_field = fieldValue.substring(beg + 7, end);
                                var substr_old = fieldValue.substring(beg, end + 1);
                                var substr_new = new_item_json[template_field];
                                fieldValue = fieldValue.replace(substr_old, substr_new);

                                beg++;
                            }
                            if (fieldValue != null)
                                cell.html(fieldValue);
                        }*/
                    }
                }
                else {
                    fieldValue = new_item_json[column.field];
                    if (fieldValue != null)
                        cell.html(fieldValue);
                }
            }

            grid_SelectRow_byUid(parent_guid, row_uid);
        }
    }

    // Удалить
    if (soperation_id == sop_4) {
        row = grid.select();
        let row_data = grid.dataItem(row);

        if (row_data != null) {
            var row_index = row[0].sectionRowIndex;

            grid.removeRow(row);
            grid_ds.remove(row_data);

            if (row_index >= grid_ds_data.length)
                row_index = grid_ds_data.length - 1;

            if ($grid.data("kendoTreeList") != undefined) {
                for (var i = 0; i < grid_ds_data.length; i++) {
                    if (grid_ds_data[i].id == row_data.ParentId)
                        row_index = i;
                }
            }

            if (grid_ds_data[row_index] != undefined)
                grid_SelectRow_byUid(parent_guid, grid_ds_data[row_index].uid);
        }
    }

    if (soperation_id == sop_8 || (soperation_id == 115 && new_item_str == undefined)) {
        vm_Base.selectAfterAdd = true;
        grid.dataSource.read();
    }
}

function grid_SelectRow_byUid(parent_guid, uid) {
    //На случай если какая-то строка уже была выделена 
    if ($('#grid_' + parent_guid).length == 0)
        return;

    grid = $('#grid_' + parent_guid).data("kendoGrid");
    if (grid == null || grid == undefined)
        grid = $('#grid_' + parent_guid).data("kendoTreeList");
    if (grid.options.selectable != false) {
        grid.clearSelection();
        var row_sel = grid.table.find("[data-uid=" + uid + "]");
        grid.select(row_sel);
        grid.table.find("[data-uid=" + uid + "]").click();
    }
}

function grid_SelectRow_fromVmBase(page_guid) {
    let grid = $("#grid_" + page_guid);
    if (grid.length > 0) {

        let grid_ds = undefined;
        if (grid.data("kendoGrid") != undefined)
            grid_ds = grid.data("kendoGrid").dataSource;
        if (grid.data("kendoTreeList") != undefined)
            grid_ds = grid.data("kendoTreeList").dataSource;
        if (grid_ds == undefined)
            return false;

        if (vm_Base.pk_field == undefined)
            vm_Base.pk_field = grid.attr("pk_field");

        var new_item_json = undefined;
        new_item_json = vm_Base.new_item_json;
        if (new_item_json == undefined && window.parent != undefined)
            new_item_json = window.parent.vm_Base.new_item_json;

        let grid_ds_data = grid_ds.data();

        var pk_value = undefined;
        if (new_item_json != null && vm_Base.pk_field)
            pk_value = new_item_json[vm_Base.pk_field];
        if (pk_value == undefined)
            pk_value = vm_Base.curr_id;
        if (pk_value == undefined)
            return false;

        for (let i = 0; i < grid_ds_data.length; i++) {
            let row_ind = grid_ds_data[i];
            if (row_ind[vm_Base.pk_field] == pk_value) {
                row_uid = row_ind.uid;
                grid_SelectRow_byUid(page_guid, row_uid);
                return true;
            }
        }
    }
    return false;
}

function grid_SelectRow_First(page_guid, guids_out, uri_path_out, uri_Content, mainTab_Text, is_last) {

    var grid = $('#grid_' + page_guid)

    if (!grid.data("loaded")) {
        grid.data("loaded", true);

        var kendoObject = grid.attr("kendoObject");
        var page_guid = grid.attr("page_guid");
        var grid_data = grid.data(kendoObject);
        if (grid_data != null) {
            if (kendoObject == "kendoGrid")
                grid_data.select("tr:eq(0)");
            else if (kendoObject == "kendoTreeList") {
                var firstNode = $("#grid_" + page_guid + " tbody>tr:eq(0)");
                grid_data.select(firstNode);
            }
        }
    }
    //if (isAutoTest(uri_path_out)) {
    //    mainTab_Open_AutoTest(guids_out, uri_path_out, uri_Content, mainTab_Text, is_last);
    //}
}

function grid_SelectRow_Last(grid) {
    if (!grid.data("loaded")) {
        grid.data("loaded", true);
        var totalPages = grid.data("kendoGrid").dataSource.totalPages();
        if (totalPages > 1) {
            grid.data("kendoGrid").dataSource.page(totalPages);
        }
        setTimeout(function () {
            grid.find(".k-grid-content").animate({ scrollTop: grid.find('.k-grid-content').prop("scrollHeight") }, 1000);
            grid.data("kendoGrid").select("tr:last-child");
        }, 500);
    }
}

function grid_SelectRows_Move_Add(page_guid, page_guid23, uri_path) {
    var grid = $("#grid_" + page_guid);
    var grid_data = grid.data("kendoGrid");

    var rows = grid_data.select();

    var frames = window.frames;
    for (var i = 0; i < frames.length; i++) {
        var source_grid = frames[i].$("#grid_" + page_guid23);
        if (source_grid.length > 0) {
            var filter_list = source_grid.attr("filter_list");
            var filter_lists = [];
            if (filter_list == undefined)
                filter_lists.push(0);
            else
                filter_lists = filter_list.split(',');

            for (var j = 0; j < rows.length; j++) {
                var row_data = grid_data.dataItem(rows[j]);
                if (filter_lists.indexOf(row_data.id) == -1)
                    filter_lists.push(row_data.id);
            }
            filter_list = filter_lists.join();
            source_grid.attr("filter_list", filter_list);
            source_grid.attr("filter_string", "<Filter />");
            var source_grid_data = frames[i].$("#grid_" + page_guid23).data("kendoGrid");

            var jdata_param = {};
            jdata_param["uri_path"] = uri_path;
            jdata_param["filter_list"] = filter_list;
            source_grid_data.dataSource.read(jdata_param);
        }
    }
}

function grid_SelectRows_Move_Del(page_guid, uri_path) {
    var grid = $("#grid_" + page_guid);
    var filter_list = grid.attr("filter_list");
    if (filter_list == undefined)
        return;

    var grid_data = grid.data("kendoGrid");
    var rows = grid_data.select();
    var row_ids = [];
    for (var j = 0; j < rows.length; j++) {
        var row_data = grid_data.dataItem(rows[j]);
        row_ids.push(row_data.id);
    }

    var filter_lists_curr = filter_list.split(',');
    var filter_lists = [];
    filter_lists.push(0);
    for (var j = 0; j < filter_lists_curr.length; j++) {
        if (row_ids.indexOf(filter_lists_curr[j]) == -1)
            filter_lists.push(filter_lists_curr[j]);
    }
    filter_list = filter_lists.join();
    grid.attr("filter_list", filter_list);

    var jdata_param = {};
    jdata_param["uri_path"] = uri_path;
    jdata_param["filter_list"] = filter_list;
    grid_data.dataSource.read(jdata_param);
}

function gridRefresh(page_guid, type) {
    //var frames = window.frames;

    var jQgrid = $("#grid_" + page_guid);

    var grid = jQgrid.data("kendoGrid");
    if (grid == undefined)
        grid = jQgrid.data("kendoTreeList");

    var jdata_param = {};
    var parent_guid = jQgrid.attr("parent_guid");
    if (parent_guid != undefined && parent_guid != page_guid) {
        var jQgrid_par = parent.$("#grid_" + parent_guid);
        if (jQgrid_par.length > 0) {
            var grid_par = jQgrid_par.data("kendoGrid");
            var row_par = grid_par.select();
            var row_data_par = grid_par.dataItem(row_par);

            jdata_param["RequestType"] = "POST";
            jdata_param["parent_id"] = row_data_par.id;
            grid.dataSource.read(jdata_param);
        }
        else
            grid.dataSource.read();
    }
    else
        grid.dataSource.read(jdata_param);
    return;
}

function gridRefresh_AfterOperation(page_guid, soperation_id, new_item_str, pk_field, refresh_grid, refresh_full, fk_field) {
    if (soperation_id == sop_2
        || soperation_id == sop_3
        || soperation_id == sop_4
        || soperation_id == sop_8
        || soperation_id == sop_12
    ) {
        if (refresh_grid == "parent") {
            vm_Base.selectAfterAdd = true;
            vm_Base.pk_field = pk_field;
            if (new_item_str != undefined)
                vm_Base.new_item_json = JSON.parse(new_item_str);
            window.parent.vm_Base.selectAfterAdd = true;
            window.parent.vm_Base.pk_field = fk_field;
            window.parent.grid_RefreshRow(window.parent.vm_Base.page_guid, soperation_id, new_item_str, fk_field, refresh_full);
            grid_RefreshRow(page_guid, soperation_id, new_item_str, pk_field, refresh_full);
        }
        else
            grid_RefreshRow(page_guid, soperation_id, new_item_str, pk_field, refresh_full);
    }

    //else if ((soperation_id == sop_8 || soperation_id == 13 || soperation_id == 115) && refresh_grid != undefined) {
    if (refresh_grid == "this") {
        grid_RefreshRow(page_guid, soperation_id, new_item_str, pk_field, refresh_full);
    }
}

function grid_ExportExcel(page_guid) {
    var grid = $("#grid_" + page_guid).data("kendoGrid");
    if (grid == undefined)
        grid = $("#grid_" + page_guid).data("kendoTreeList");
    if (grid != undefined)
        grid.saveAsExcel();
};

function mainGrid_Parameters(vm_Base, url, soperation_id) {
   
    /*if (soperation_id == sop_12)*/ {
        var main_stable_id = 0;
        var main_parent_id = 0;
        if (vm_Base.mainTab_sconfig_type_id == 101) {
            if (parent != null) {
                if (vm_Base.mainTab_guid != null && vm_Base.mainTab_guid != undefined
                    && vm_Base.mainTab_StableId != null && vm_Base.mainTab_StableId != undefined) {

                    main_stable_id = vm_Base.mainTab_StableId;
                    var main_parent_id = parent.getRowId_ByGuid(vm_Base.mainTab_guid, vm_Base);
                }
            }
        }
        else if (vm_Base.mainTab_sconfig_type_id == 102) {
            main_stable_id = vm_Base.mainTab_StableId;
            main_parent_id = vm_Base.parent_id;
        }
        if (main_stable_id > 0 && main_parent_id > 0) {
            url = updateQueryStringParameter(url, "main_stable_id", main_stable_id);
            url = updateQueryStringParameter(url, "main_parent_id", main_parent_id);
        }
    }
    return url;
}

function grid_Get_RowGuid_ByField(new_item_json, pk_field) {
    let $grid = $("#grid_" + vm_Base.page_guid);
    let grid = $grid.data("kendoGrid");
    if (grid == null || grid == undefined) {
        grid = $grid.data("kendoTreeList");
    }

    if (grid == null || grid == undefined)
        return;

    let grid_ds = grid.dataSource;
    let grid_ds_data = grid_ds.data();

    var pk_value = new_item_json[pk_field];
    for (var i = 0; i < grid_ds_data.length; i++) {
        var row_ind = grid_ds_data[i];
        if (row_ind[pk_field] == pk_value) {
            return row_uid = row_ind.uid;
        }
    }
}

/*Функция сначала добавляет в разметку css-стили для заданных цветов фона и текста. 
А затем строкам грида, у которых значение заданного поля удовлетворяет заданному условию, присваивает эти css-стили */
function grid_SetColor(page_guid) {
    let jQgrid = $('#grid_' + page_guid);

    if (jQgrid.length > 0) {
        let grid_data = jQgrid.data("kendoGrid");
        let field = jQgrid.attr("colored_rows_field");
        let condition_value = jQgrid.attr("colored_rows_condition_value");
        let color = jQgrid.attr("colored_rows_color");
        let color_font = jQgrid.attr("colored_rows_color_font");

        if (field != undefined && condition_value != undefined && color != undefined && grid_data != undefined) {
            let style_string = "<style> .colored-rows { background-color: " + color + "; } </style>";
            if (color_font != undefined)
                style_string += "<style> .colored-rows-font { color: " + color_font + "; } </style>";
            $('head').append(style_string);

            let rows = $("tbody").children();

            for (var j = 0; j < rows.length; j++) {
                var row = $(rows[j]);
                var dataItem = grid_data.dataItem(row);

                if (dataItem != undefined && dataItem[field] != "" && dataItem[field] != undefined && get_Grid_SetColor_Condition(page_guid, dataItem[field], condition_value)) {
                    let row_css = "";
                    if (row.attr("class") != undefined)
                        row_css = row.attr("class");
                    row_css += " colored-rows";

                    if (color_font != undefined)
                        row_css += " colored-rows-font";

                    row.attr("class", row_css);
                }
            }
        }
    }
}
/*Работа 7143. Добавлены настраиваемые в конфиге условия*/
function get_Grid_SetColor_Condition(page_guid, val1, val2) {
    if (val1 == undefined || val2 == undefined)
        return false;

    let jQgrid = $('#grid_' + page_guid);

    if (jQgrid.length > 0) {
        let condition = jQgrid.attr("colored_rows_condition");
        if (condition == undefined)
            return val1 == val2;
        else {
            if (condition == "!=")
                return val1 != val2;
            if (condition == ">")
                return val1 > val2;
            if (condition == "<")
                return val1 < val2;
            if (condition == ">=")
                return val1 >= val2;
            if (condition == "<=")
                return val1 <= val2;
            if (condition == "=" || condition == "==")
                return val1 == val2;
        }
    }
    return false;
}
//***********************************************************************************
function gridEditable_Add(grid_name, page_guid, uri_Content) {
    var jQgrid = $("#" + grid_name + "_" + page_guid);
    var grid = jQgrid.data("kendoGrid");
    var grid_ds = grid.dataSource;
    var grid_ds_view = grid_ds.view();

    var id_max = 0;
    for (var i = 0; i < grid_ds_view.length; i++) {
        var id = grid_ds_view[i].GridRowId;
        if (id != null && id != undefined && id > id_max)
            id_max = id;
    }

    var row_last_data = grid_ds_view[grid_ds_view.length - 1];

    var row_sel = grid.select();
    var row_sel_data = grid.dataItem(row_sel);

    var row_data = {};
    row_data.GridRowId = id_max + 1;

    grid_ds.add(row_data);
    grid.refresh();

    grid.select("tr:last-child");
}

function gridEditable_Del(grid_name, page_guid, uri_Content) {
    var jQgrid = $("#" + grid_name + "_" + page_guid);
    var grid = jQgrid.data("kendoGrid");
    var grid_ds = grid.dataSource;
    var grid_ds_data = grid_ds.data();

    if (grid.select().length == 0) {
        messageWindow("Ошибка", "Не выбрана строка");
        return false;
    }

    var row_data = grid.dataItem(grid.select());

    grid_ds.remove(row_data);
    grid.refresh();

    grid.select("tr:last-child");
}

var curr_source_tree_node = null;
var curr_source_parent_tree_node = null;
var curr_target_tree_node = null;
var curr_source_tree_node_cut = false;

function grid_TreeNodeCopy(page_guid, stable_id, cut) {    
    var grid = $("#grid_" + page_guid).data("kendoTreeList");
    if (!grid) {        
        messageWindow('Ошибка', 'Не найдено дерево');
        return;
    };
    var row = grid.select();
    if (!row) {        
        messageWindow('', 'Встаньте на строку, которую нужно скопировать');
        return;
    };
    var dataItem = grid.dataItem(row);
    if (!dataItem) {        
        messageWindow('Ошибка', 'Не найдены данные в строке');
        return;
    };
    //
    curr_source_tree_node = dataItem.id.toString();
    curr_source_parent_tree_node = dataItem.parentId.toString();
    curr_source_tree_node_cut = cut;
    messageWindow('', 'Строка скопирована');
};

function grid_TreeNodePaste(page_guid, main_table_page_guid, stable_id, action_url, parent_stable_id) {
    if (!curr_source_tree_node) {        
        messageWindow('', 'Сначала скопируйте строку');
        return;
    }
    //
    var grid = $("#grid_" + page_guid).data("kendoTreeList");
    if (!grid) {        
        messageWindow('Ошибка', 'Не найдено дерево');
        return;
    };
    //
    var row = grid.select();
    var dataItem = grid.dataItem(row);
    curr_target_tree_node = dataItem ? dataItem.id.toString() : null;
    //
    var parent_id = null;
    if (main_table_page_guid) {
        if ((parent) && (parent.frameElement)) {
            var main_frame = parent.frameElement.contentWindow.$;
            var element = $(parent.frameElement).contents().find('#grid_' + main_table_page_guid)[0]
            var grid_main = main_frame.data(element, 'kendoTreeList')
            if (grid_main) {                
                var row_main = grid_main.select();
                var dataItem_main = grid_main.dataItem(row_main);
                if (dataItem_main)
                    parent_id = dataItem_main.id.toString();
            };
        }        
    };
    //
    $.ajax({
        type: 'POST',        
        url: action_url,
        data: JSON.stringify(
            {
                stable_id: stable_id,
                source_node_str: curr_source_tree_node,
                source_parent_node_str: curr_source_parent_tree_node,
                target_node_str: curr_target_tree_node,
                parent_id_str: parent_id, 
                parent_stable_id: parent_stable_id,
                is_del: curr_source_tree_node_cut
            }),
        contentType: 'application/json; charset=windows-1251',
        success: function (response) {
            if ((response == null) || (response.status === "error")) {                
                messageWindow('Ошибка',
                    (response == null ? 'Ошибка при попытке вставить строку' : (response.message ? response.message : 'Ошибка при попытке вставить строку')),
                    (response == null ? null : (response.error_text ? response.error_text : null))
                    );
            } else {                
                messageWindow('', response.message ? response.message : 'Строка вставлена');
                if (response.new_item) {
                    grid.dataSource.add(response.new_item)
                };
                if ((curr_source_tree_node) && (curr_source_tree_node_cut)) {
                    grid.dataSource.remove(grid.dataSource.get(curr_source_tree_node));
                };
                curr_source_tree_node = null;
                curr_source_parent_tree_node = null;
                curr_target_tree_node = null;
            };
        },
        error: function (response) {            
            messageWindow('Ошибка', 'Ошибка при вставке строки');
        }
    });
};

function TabRelation_AddRelation(page_guid, url, page_guid_left, page_guid_right, grand_parent_id) {

    var grid_left = $("#grid_" + page_guid_left);
    var grid_right = $("#grid_" + page_guid_right);

    var left_tree_row_id = parent.getRowId_ByGuid(grid_left.attr("parent_guid"));
    if (left_tree_row_id == null) {
        messageWindow("Ошибка", "Выберите основной объект.");
        return;
    }
    var right_tree_row_id = getRowId_ByGuid(page_guid_right);
    if (right_tree_row_id == null) {
        messageWindow("Ошибка", "Выберите дочерний объект.");
        return;
    }

    confirmWindow("Операция", "Создать связь узлов ?"
        , 'tabRelation_AddRelation_FindFrame("' + url + '", "' + page_guid_left + '", "' + grand_parent_id + '", "' + left_tree_row_id + '", "' + right_tree_row_id + '")'
        );
}

function tabRelation_AddRelation_FindFrame(url, page_guid, grand_parent_id, left_tree_row_id, right_tree_row_id) {
    messageWindow_Close();

    for (var i = 0; i < frames.length; i++) {
        var grid = frames[i].$("#grid_" + page_guid);
        if (grid.length > 0) {
            frames[i].tabRelation_AddRelation_Exec(url, page_guid, grand_parent_id, left_tree_row_id, right_tree_row_id);
            return;
        }
        else {
            for (var ii = 0; ii < frames[i].frames.length; ii++) {
                var grid = frames[i].frames[ii].$("#grid_" + page_guid);
                if (grid.length > 0) {
                    frames[i].frames[ii].tabRelation_AddRelation_Exec(url, page_guid, grand_parent_id, left_tree_row_id, right_tree_row_id);
                    return;
                }
            }
        }
    }
}

function tabRelation_AddRelation_Exec(url, page_guid_left, grand_parent_id, left_tree_row_id, right_tree_row_id) {
    messageWindow_Close();

    var options = {};
    options.url = url;
    options.type = "POST";

    options.data = {};
    options.data.Operation = "EntitySave";

    options.data["DOCUMENT_RELATION.DOCUMENT_ID.0"] = grand_parent_id;
    options.data["DOCUMENT_RELATION.DOCUMENT_ITEM_ID.0"] = left_tree_row_id;
    options.data["DOCUMENT_RELATION.CHILD_ID.0"] = right_tree_row_id;
    options.data["DOCUMENT_RELATION.NDOCUMENT_RELATION_ID.0"] = "24";

    options.success = function (data) {
        var grid_left = $("#grid_" + page_guid_left);
        grid_left.data("kendoGrid").dataSource.read();
    };

    $.ajax(options);
}

function grid_SaveConfig(page_guid, url) {
    var kendoGrid = $("#grid_" + page_guid).data("kendoGrid");
    if (kendoGrid != undefined) {
        var configGridData = {
            items: [],
            sort: [],
        };
        for (var i = 0; i < kendoGrid.columns.length; i++) {
            configGridData.items.push({
                field: kendoGrid.columns[i].field,
                hidden: kendoGrid.columns[i].hidden,
                width: kendoGrid.columns[i].width,
            });
        }
        var columnSort = kendoGrid.dataSource.sort();
        for (var i = 0; i < columnSort.length; i++) {
            configGridData.sort.push({
                field: columnSort[i].field,
                desc: columnSort[i].dir == "desc",
            });
        }

        $.ajax({
            url: url,
            type: 'POST',
            dataType: 'json',
            data: {
                configGridData: JSON.stringify(configGridData)
            }
        });
    }
}

function gridVolumeValueShowColumns(e, page_guid)
{
    var grid = $('#grid_' + page_guid);
    if (grid.length > 0) {
        var grid_data = grid.data("kendoGrid");
        grid_data.select("tr:eq(0)");
        var row = grid_data.select(1);

        if (e.sender._data[0] == undefined)
            return;

        let graph = e.sender._data[0].VOLUME_GRAPH;
        let type = e.sender._data[0].VOLUME_TYPE;
        //В зависимости от типа показываем нужный столбец с единицами измерения
        if (graph == 1 || graph == 2 || graph == 3) {
            grid.data('kendoGrid').showColumn(6);
        }
        else
            grid.data('kendoGrid').showColumn(7);

        if (type == 3) {
            grid.data('kendoGrid').showColumn(0); //Год
            grid.data('kendoGrid').showColumn(1); //Месяц
            grid.data('kendoGrid').showColumn(4); //Источник
            grid.data('kendoGrid').showColumn(5); //Маска
            grid.data('kendoGrid').showColumn(6); //М3
            grid.data('kendoGrid').showColumn(7); //Кг
            grid.data('kendoGrid').showColumn(8); //Вывозы
            grid.data('kendoGrid').showColumn(9); //Итого
            grid.data('kendoGrid').showColumn(10); //Разница с Маджента
            grid.data('kendoGrid').showColumn(11); //Фамилия ИО
        }

        //для графика "Помесячный объем" показываем столбец "Месяц"
        if (graph == 2) {
            grid.data('kendoGrid').showColumn(1);
        }
        //для графика "Среднемесячный погодовой объем" показываем столбцы "Год","Месяц" и значения
        if (graph == 3) {
            grid.data('kendoGrid').showColumn(0);
            grid.data('kendoGrid').showColumn(1);
        }     
        //для графика "Заданные дни недели" показываем столбец "Дни недели"
        if (graph == 7) {
            grid.data('kendoGrid').showColumn(2);

            var values = "";
            $.each($('.volume_value_row'), function () {
                if ($(this).attr("volume_value_id") != undefined && $(this).attr("value") != undefined /*&& $(this).attr("value") != ""*/)
                    values += $(this).attr("volume_value_id") + "-" + $(this).attr("value") + ",";
            });
            if (values != "")
                $('.volume_value_row').attr("volume_values", values);

        }
        //для графика "Заданные даты" показываем столбец "DATE_BEG"
        if (graph == 8) {
            grid.data('kendoGrid').showColumn(3);
        }
    }
}
//***********************************************************************************
function ChartResize() {
    if ($('#chart').length > 0) {
        var height = window.innerHeight - 30;
        var width = window.innerWidth - 30;
        $('#chart').attr("style", "height: " + height + "px; width: " + width + "px;")
    }
}

/*Функция для определения является ли грид внучатным (3й уровень) */
function isGridGrandchild() {
    if (window.parent.frameElement != null && is_true(window.parent.frameElement.getAttribute("is_child_tab")) && window.parent.$('.k-grid[grid_has_grandchild="True"]').length > 0)
        return true;
    else
        return false;
}

//***********************************************************************************
//Исправление бага, при котором пропадала возможность расширения столбцов грида после изменения масштаба браузера
kendo.ui.Grid.prototype._positionColumnResizeHandle = function () {
    var that = this,
        indicatorWidth = that.options.columnResizeHandleWidth,
        lockedHead = that.lockedHeader ? that.lockedHeader.find("thead:first") : $();

    that.thead.add(lockedHead).on("mousemove" + ".kendoGrid", "th", function (e) {
        var th = $(this);
        if (th.hasClass("k-group-cell") || th.hasClass("k-hierarchy-cell")) {
            return;
        }
        that._createResizeHandle(th.closest("div"), th);
    });
};