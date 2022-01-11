
function selectorGrid_DataBound(vm_Base_params, page_guid, uri_Content) {
    try {
        vm_Base.initializeArguments(arguments);

        var jQgrid = $("#period_values_grid_" + page_guid);
        var grid = jQgrid.data("kendoGrid");
        var grid_ds_data = grid.dataSource.data();
        grid.select("tr:last");

        if (grid_ds_data.length > 0)
            grid_SelectRow_byUid(page_guid, grid_ds_data[grid_ds_data.length - 1].uid);

        periodValue_Save_Change_ButtomImage(page_guid, uri_Content, true);
    }
    catch (e) {
        if (Top().isAutoTestClient)
            Top().set_Auto_Test_Log(e);
        else
            throw e;
    }
}

function selector_Edit(e, page_guid) {
    var row = $(e.container).closest("tr");
    var rowIdx = row[0].rowIndex;
    var colIdx = $("td", row).index($(e.container));

    if (colIdx <= 0) {
        //Редактировать дату можно только у последней строки
        if (rowIdx <= e.sender.table.find("tr").length - 2) {
            e.container.find("input[name=DateBeg]").data("kendoDatePicker").enable(false);
        }
    }
    else 
    {
        if (e.model.PeriodType == "Selector") {
            if (colIdx == 2) {
                $("#period_values_grid_" + page_guid).data("kendoGrid").closeCell();
            }
        }
        if (e.model.PeriodType == "ComboBox")
        {
            var input = e.container.find("input").data("kendoDropDownList");
            if (input != undefined && (input.value() == "" || input.value() == undefined))
                input.value(e.model.Value);
        }
    }
}

var selector_edit_row_index;
var selector_edit_col_index;
function selectorGridCell_Change(page_guid, uri_Content, grid, row_el, rowIdx, colIdx) {
    //Редактировать дату можно только у последней строки
    if (rowIdx <= $("tr", grid.tbody).length - 2 && colIdx <= 0) {
        //messageWindow("Ошибка", "Редактировать дату можно только у последней строки");
        return;
    }
    if (rowIdx == selector_edit_row_index && colIdx == selector_edit_col_index) {
        return;
    }
    var row_uid = row_el.data("uid");

    var edit_mode = periodValue_Edit_Get_EditMode(page_guid);

    if (edit_mode) {

        if (selector_edit_col_index != null && selector_edit_col_index != undefined && selector_edit_col_index == 0) {
            //periodValue_Edit_Change_RefreshDate(page_guid, grid);
        }

        if (colIdx > 1)
            colIdx = 1;
        
        if (selector_edit_col_index == 0 && selector_edit_row_index > 1) {
            var grid_el = $("#period_values_grid_" + page_guid);
            var grid = grid_el.data("kendoGrid");
            var grid_ds = grid.dataSource;
            var grid_ds_view = grid_ds.view();

            var row_this_item = grid_ds_view[selector_edit_row_index];
            var row_prev_item = grid_ds_view[selector_edit_row_index - 1];

            if (row_this_item != undefined && row_prev_item != undefined) {
                row_this_item.PeriodDate = DateBeg_DateEnd(row_this_item.DateBeg, row_this_item.DateEnd);

                row_prev_item.DateEnd = row_this_item.DateBeg;
                row_prev_item.PeriodDate = DateBeg_DateEnd(row_prev_item.DateBeg, row_prev_item.DateEnd);
            }
        }

        selector_edit_row_index = rowIdx;
        selector_edit_col_index = colIdx;

        var row_td_val_el = row_el.find("td:eq(" + colIdx + ")");

        selector_edit_row_uid = row_uid;
        grid.editCell(row_td_val_el);
    }
}
function selectorGrid_Change(page_guid, uri_Content) {
    var grid = $("#period_values_grid_" + page_guid).data("kendoGrid");
    var row_el = grid.select();
    var row_uid = row_el.data("uid");

    if (selector_edit_row_uid != null && selector_edit_row_uid != row_uid) {

        var edit_mode = periodValue_Edit_Get_EditMode(page_guid);
        if (edit_mode) {

            selector_edit_row_uid = row_uid;

            //var row_td_val_el = row_el.find("td:eq(1)");
            //grid.editCell(row_td_val_el);

            //if (edit_mode) {
            //    periodValue_Edit_Change_LastRow(page_guid);
            //}
        }
    }
}

//***********************************************************************************
function selector_Click(sender, page_guid, guids_out, uri_Content, uri_Action, selector_type, selector_filter_dynamic) {

    // selector_type = 
    //      1 - selector from filter
    //      2 - selector from editor
    //      3 - editor
    //      4 - selector from period values

    clickItem.initialize(sender);

    var tab_name = "FilterTab";
    if (selector_type == 2 || selector_type == 3 || selector_type == 4) {
        tab_name = "EditTab";
    }

    var tab_content_el = $("#" + tab_name);
    if (tab_content_el.length == 0) {
        tab_name = "EditTabMain";
        tab_content_el = $("#" + tab_name);
        if (tab_content_el.length == 0)
            return false;
    }

    var item_val = $("#val_" + page_guid);
    item_val.attr("Changed", "Changed");
    var item_val_right = $("#valRight_" + page_guid);
    if (item_val_right != undefined)
        item_val_right.attr("Changed", "Changed");
    var data_param = uri_Action;
    var data_form = {};

    // для зависимости селекотра от других инпутов на странице
    if (is_true(item_val.attr("use_header_path"))) {
        formData.readInputs("myForm");
        data_param = formData.toUrl(data_param);
    }

    if (selector_filter_dynamic != undefined && selector_filter_dynamic != "")
        data_param = updateQueryStringParameter(data_param, "selector_filter_dynamic", selector_filter_dynamic);

    if (selector_type == 1 || selector_type == 2) {
        var selector_result = $(sender).attr("data-selector-result");
        var selector_value = item_val.attr("value");
        data_param = updateQueryStringParameter(data_param, "selector_result", selector_result);
        if (selector_result != undefined && selector_result == "Filter")
            data_form.selector_value = selector_value;
        else
            data_param = updateQueryStringParameter(data_param, "selector_value", selector_value);

        var content_type_from_field = $(sender).attr("data-content-type-from-field");
        if (content_type_from_field != undefined) {
            var selector_parent_guid = $(sender).attr("data-selector-parent-guid");
            var tabEdit_Content = $(".editor-field-list");
            if (tabEdit_Content.length > 0) {
                var content_type_source_field = tabEdit_Content.find("[data-content-type-source-field='" + content_type_from_field + "']");
                if (content_type_source_field.length > 0) {
                    content_type_source_field_guid = content_type_source_field.attr('page_guid');
                    var content_type_source_field_item = $("#val_" + content_type_source_field_guid);
                    var content_type_source_field_nsi_group_name = content_type_source_field_item.attr("nsi_group_name");
                    var content_type_source_field_nsi_table_name = content_type_source_field_item.attr("nsi_table_name");
                    var content_type_source_field_val = content_type_source_field_item.val();
                    if (content_type_source_field_val == "") {
                        var content_type_source_field_label = $("#label_" + content_type_source_field_guid).text().trim();
                        messageWindow("Ошибка", "Не задан атрибут [" + content_type_source_field_label + "]");
                        return;
                    }
                    else {
                        data_param = updateQueryStringParameter(data_param, "selector_content_type", content_type_from_field + "=" + content_type_source_field_val);
                        data_param = updateQueryStringParameter(data_param, "selector_content_table_name", content_type_source_field_nsi_table_name);
                        data_param = updateQueryStringParameter(data_param, "selector_content_group_name", content_type_source_field_nsi_group_name);
                    }
                }
            }
        }
    }

    if (selector_type == 3) {
        var parent_page_guid = $(sender).attr('data-page_guid');
        var buttSave = $("#buttSave_" + parent_page_guid);
        var save_mode = "False";
        if (buttSave.length > 0) {
            var buttSave_attr = buttSave.attr("data-save");
            if (buttSave_attr != undefined && buttSave_attr == 1)
                save_mode = "True";
        }

        var tab_edit = $("#TabEditContent_" + page_guid);

        data_param = updateQueryStringParameter(data_param, "selector_value", item_val.attr("value"));
        data_param = updateQueryStringParameter(data_param, "caption", $("#search_" + page_guid).attr("data-label"));
        data_param = updateQueryStringParameter(data_param, "save_mode", save_mode);
    }

    if (selector_type == 4) {
        var link_period_values_grid = $("#period_values_grid_" + page_guid);
        var period_values_grid = link_period_values_grid.data("kendoGrid");
        var row_el = $(period_values_grid.select());
        if (row_el.length == 0) {
            messageWindow("Ошибка", "Необходимо выделить строку");
            return;
        }
        data_param = updateQueryStringParameter(data_param, "title", "Период");
    }

    var current = $("#" + tab_name + " > ul > li.k-state-active");
    var kendoTabStrip = tab_content_el.data("kendoTabStrip");
    if (kendoTabStrip != undefined) {
        kendoTabStrip.enable(current, false);
        var tab_text = $(sender).data("label");
        
        kendoTabStrip.append({ text: tab_text, encoded: false, content: "<div class='is-fulled loader-spin'></div>" });
        var tab_activate = $("#" + tab_name + " > ul > li.k-last");
        kendoTabStrip.activateTab(tab_activate);

        if (data_param != undefined) {
            if (selector_type == 2) {
                var sss = 1;
            }
            $.ajax({
                url: data_param,
                type: "POST",
                data: data_form,
                success: function (data) {
                    setTimeout(function () {
                        if ($('#FilterTab > .k-content.k-state-active').length > 0)
                            $('#FilterTab > .k-content.k-state-active').html(data);
                        else
                            $('#EditTab > .k-content.k-state-active').html(data);
                    }, 500);
                }
            });
        }
    }
}

function selector_FastExecute() {
    formData.readInputs("myFilterForm_" + clickItem.page_guid);
    var ss = 1;

    $.ajax({
        url: clickItem.url_action,
        type: "POST",
        data: formData.formDataObject,
        contentType: false,
        processData: false,
        success: function (data) {
            var filterString = data.filter_string;
            if (filterString != undefined) {
                id = filterString;
                caption = filterString;
            }
            var page_guid = clickItem._page_guid;

            var link = $("#search_" + page_guid);
            var link_check = link.closest("tr").find('input[type="checkbox"]');
            link_check.prop("checked", true);

            var link_caption = $("#caption_" + page_guid);
            var link_val = $("#val_" + page_guid);

            link_val.attr("data-valfield-msg", false)
            link_val.removeAttr("disabled");
            link_val.attr("selector_executed", true);

            link_caption.val(caption);

            link_val.attr("selector_id", id);
            var result_enc = selector_Result(id, caption);
            link_val.val(result_enc);

            filterTab_Close(page_guid, clickItem.isSelectorFromFilter);
        }
    });

}

function selector_Execute(page_guid, guids_out, isSelectorFromFilter, soperation_id, data, grid_type) {

    var filterTab = $("#FilterTab");
    var filterPanel = $(".filterPanel");
    if (!is_true(clickItem.is_selector_from_filter) && filterTab.length == 0) {
        filterTab = $("#EditTab");
        if (filterTab.length == 0)
            filterTab = $("#EditTabMain");
    }

    var filterTab_TabStrip = filterTab.data("kendoTabStrip");

    if (filterTab.length == 0 || filterTab_TabStrip == null || clickItem.page_guid == undefined)
        return;

    var link = $("#search_" + clickItem.page_guid);
    if (clickItem.soperation_id == sop_38 && !link) {
        var page_guid_out = getGuidStr_FromGuidsOut_ByName(clickItem.guids_out, "Selector");
        link = $("#search_" + page_guid_out);
        if (link)
            clickItem.page_guid = page_guid_out;
    }

    if (link) {
        var id = undefined, caption = undefined;

        var linkResultType = link.attr("data-selector-result");
        // если тип результат селектора - фильтр, то не надо выбирать строку в гриде
        var jQgrid = $("#grid_" + clickItem.page_guid);
        if (linkResultType != undefined && linkResultType == "Filter") {
            var filterString = jQgrid.attr("filter_string");
            if (filterString != undefined) {
                id = filterString;
                caption = filterString;
            }
        }
        else {
            if (clickItem.soperation_id == sop_38) {
                id = data.pk_value;
                if (clickItem.text_property != undefined && data.new_item != undefined) {
                    caption = JSON.parse(data.new_item)[clickItem.text_property];
                }
                else
                    caption = data.name_value;
            }
            else {
                if (clickItem.grid_type == undefined)
                    clickItem.grid_type = "kendoGrid";
                var grid = jQgrid.data(clickItem.grid_type);
                if (grid == null)
                    return;

                var row = grid.select();
                if (row.length > 0) {
                    var grid_row_data = grid.dataItem(row);

                    var valField = link.data("val");
                    var captionField = jQgrid.attr("text_property");

                    id = grid_row_data.id;
                    caption = grid_row_data[captionField];
                    if (caption == undefined && $.isNumeric(grid_row_data.id))
                        caption = grid_row_data.id;
                }
            }
        }

        if (id != undefined) {
            var link_period_values_grid = $("#period_values_grid_" + clickItem.page_guid);

            if (link_period_values_grid.length == 0) {
                var link_check = link.closest("tr").find('input[type="checkbox"]');
                link_check.prop("checked", true);

                let link_caption = $("#caption_" + clickItem.page_guid);
                if (link_caption.length == 0)
                    link_caption = $("#caption_" + clickItem.selector_page_guid);
                let link_val = $("#val_" + clickItem.page_guid);
                if (link_val.length == 0)
                    link_val = $("#val_" + clickItem.selector_page_guid);

                link_val.attr("data-valfield-msg", false)
                link_val.removeAttr("disabled");
                link_val.attr("selector_executed", true);

                link_caption.val(caption);

                link_val.attr("selector_id", id);
                var result_enc = selector_Result(id, caption);
                link_val.val(result_enc);
            }
            else {
                var period_values_grid = link_period_values_grid.data("kendoGrid");

                var row_el = $(period_values_grid.select());
                var row_item = period_values_grid.dataItem(row_el);

                if (row_item != null) {
                    row_item.Value = id;
                    row_item.ValueStr = id.toString();
                    row_item.Name = caption;

                    period_values_grid.refresh();
                    grid_SelectRow_byUid(clickItem.page_guid, row_item.uid);
                }
            }
        }
    }

    filterTab_Close(clickItem.page_guid, clickItem.is_selector_from_filter);
    if (isMobile())
        filterPanel.data("kendoResponsivePanel").open();

    let selector = $("#val_" + clickItem.page_guid);
    if (selector.length == 0) {
        selector = $("#val_" + clickItem.selector_page_guid);
        clickItem.page_guid = clickItem.selector_page_guid;
    }
    if (selector.length > 0) {
        control_RefreshChilds(clickItem.page_guid);
    }
}

function selector_Result(id, caption) {
    if (id == "" && caption == "")
        return "";

    var xresult = "<root>";
    xresult += "    <id>" + id + "</id>";
    xresult += "    <text>" + caption + "</text>";
    xresult += "</root>";

    var result_enc = encodeURIComponent(xresult);
    return result_enc;
}

function selector_Clear(page_guid) {
    var link_val = $("#val_" + page_guid);
    var link_caption = $("#caption_" + page_guid);
    var link_val_right = $("#valRight_" + page_guid);

    link_caption.val("");
    link_val.val("[null]");
    link_val_right.val("");
    link_val.removeAttr("selector_id");

    /*Changed не убираем. Пустые значения тоже надо передавать*/
    //if ($('.ctrl-selector .ctrl-selector > #val_' + page_guid).length == 0)
    if (link_val.attr("Changed") != undefined)
        link_val.attr("Changed", "Changed");

    control_ClearAllChilds(page_guid);
   
}

function selector_Combo_onChange(sender, page_guid) {

    var sender_el = $(sender.element);
    var grid_el = sender_el.closest(".k-grid");
    if (grid_el.length == 0)
        return;
    var grid = grid_el.data("kendoGrid");

    var row_el = grid.select();
    var row_item = grid.dataItem(row_el);
    var sender_el_data = sender_el.data("kendoDropDownList");

    if (sender_el_data == undefined)
        sender_el_data = sender_el.data("kendoComboBox");
    if (sender_el_data == undefined)
        return;

    var sender_el_data_value = sender_el_data.value();

    row_item.ComboItemText = sender_el_data.text();
    row_item.ComboItemValue = sender_el_data_value;
    row_item.Name = sender_el_data_value;
    row_item.Value = sender_el_data_value;
    row_item.ValueStr = sender_el_data_value;
}

function selector_Masked_onChange(sender, page_guid) {

    var sender_el = $(sender.element);
    var grid_el = sender_el.closest(".k-grid");
    if (grid_el.length == 0) 
        return;
    var grid = grid_el.data("kendoGrid");

    var row_el = grid.select();
    var row_item = grid.dataItem(row_el);
    var sender_el_data = sender_el.data("kendoMaskedTextBox");
    var sender_el_data_value = sender_el_data.value();

    if (sender_el_data.options.unmaskOnPost != undefined && sender_el_data.options.unmaskOnPost == true)
        sender_el_data_value = sender_el_data.raw();

    row_item.Name = sender_el_data_value;
    row_item.Value = sender_el_data_value;
    row_item.ValueStr = sender_el_data_value;
}

function selector_Combo_onBind(sender, page_guid) {
    var sender_el = $(sender.element);
    var grid_el = sender_el.closest(".k-grid");
    var sender_el_data = sender_el.data("kendoDropDownList");
}

function selector_Select(sender, page_guid) {
    var parent_page_guid = $(sender.item).attr("page_guid");
    if (parent_page_guid != undefined)
    {
        var parent_value = $('#val_' + parent_page_guid).val();
        var parent_value_right = $('#valRight_' + parent_page_guid).val();
        var parent_caption = $('#caption_' + parent_page_guid).val();
        if (parent_caption != "") {
            $('#val_' + page_guid).val(parent_value);
            $('#val_' + page_guid).attr("Changed", "Changed");
            $('#caption_' + page_guid).val(parent_caption);
            $('#valRight_' + page_guid).val(parent_value_right);
        }
    }
}

function contextMenuSelectorCreate(page_guid) {
    //$("#contextMenuSelector_" + page_guid).removeClass('is-not-visible');
    $("#contextMenuSelector_" + page_guid).kendoContextMenu({
        target: "#select_" + page_guid,
        showOn: "click",
        select: function (e) {
            selector_Select(e, page_guid)
        }
    });
}

//***********************************************************************************
function periodValue_FindFrame(grid_name, page_guid, uri_Content, soperation_id, uri_path) {
    messageWindow_Close();

    for (var i = 0; i < frames.length; i++) {
        var grid = frames[i].$("#" + grid_name + page_guid);
        if (grid.length > 0) {
            if (soperation_id == sop_3)
                frames[i].periodValue_Save_Execute(grid_name, page_guid, uri_Content, uri_path);
            if (soperation_id == sop_4)
                frames[i].periodValue_Del_Execute(grid_name, page_guid, uri_Content);
            return;
        }
        else {
            for (var ii = 0; ii < frames[i].frames.length; ii++) {
                if (frames[i].frames[ii].$ != undefined) {
                    var grid = frames[i].frames[ii].$("#" + grid_name + page_guid);
                    if (grid.length > 0) {
                        if (soperation_id == sop_3)
                            frames[i].frames[ii].periodValue_Save_Execute(grid_name, page_guid, uri_Content, uri_path);
                        if (soperation_id == sop_4)
                            frames[i].frames[ii].periodValue_Del_Execute(grid_name, page_guid, uri_Content);
                        return;
                    }
                }
            }
        }
    }
}

function periodValue_Add() {
    var jQgrid = $("#period_values_grid_" + clickItem.page_guid);
    var grid = jQgrid.data("kendoGrid");
    var grid_ds = grid.dataSource;
    var grid_ds_view = grid_ds.view();

    var row_last_data = grid_ds_view[grid_ds_view.length - 1];

    var row_sel = grid.select();
    var row_sel_data = grid.dataItem(row_sel); 

    var date_new = new Date(Date.now());
    var yy0 = date_new.getFullYear(), mm0 = date_new.getMonth() + 1, dd0 = date_new.getDate();

    if (row_last_data != undefined) {
        var date_last = row_last_data.DateBeg;
        var yy1 = date_last.getFullYear(), mm1 = date_last.getMonth() + 1, dd1 = date_last.getDate();
        if (yy0 * 1000 + mm0 * 100 + dd0 <= yy1 * 1000 + mm1 * 100 + dd1) {
            date_new.setTime(date_last.getTime() + 86400000);
        }
    }
    var yy = date_new.getFullYear(), mm = date_new.getMonth() + 1, dd = date_new.getDate();

    var row_data = {};
    row_data.DateBeg = date_new;
    row_data.DateEnd = null;
    row_data.PeriodDate = DateBeg_DateEnd(row_data.DateBeg, row_data.DateEnd);
    row_data.Value = null;
    row_data.PeriodType = $(grid.element[0]).attr('data-selector-type');

    row_data.ComboItemValue = null;
    row_data.ComboItemText = null;

    if (vm_Base.selector_value != null && vm_Base.selector_text != null) {
        row_data.ComboItemValue = vm_Base.selector_value;
    }

    row_data.Value = null;
    row_data.ValueStr = "";
    row_data.Name = "";

    grid_ds.add(row_data);
    grid.refresh();

    periodValue_Edit_Change_LastRow(clickItem.page_guid);
    periodValue_Edit_Change_EditMode(clickItem.page_guid, vm_Base.url_Content, grid, grid.select(), true, 0);

    periodValue_Save_Change_ButtomImage(clickItem.page_guid, vm_Base.url_Content, true);
    return false;
}

function periodValue_Del() {

    //Проверка доступности у кнопок редактирования грида в редакторе
    if (is_true($("div.formAreaEdit").find("div.k-toolbar > a.k-button").attr("clickable")))
        return false;

    var jQgrid = $("#" + clickItem.grid_name + clickItem.page_guid);
    var grid = jQgrid.data("kendoGrid");
    var grid_ds_data = grid.dataSource.data();

    if (clickItem.is_ext_params == undefined && grid_ds_data.length <= 1) {
        messageWindow("Ошибка", "Нельзя удалять единственный период");
        return false;
    }

    var row_data = grid.dataItem(grid.select());
    if (row_data.DateEnd != null) {
        messageWindow("Ошибка", "Нельзя удалять закрытый период");
        return false;
    }

    confirmWindow("Удаление", "Удалить данную запись?"
        , 'periodValue_FindFrame("' + clickItem.grid_name + '","' + clickItem.page_guid + '", "' + vm_Base.url_Content + '", 4)'
        );

    return false;
}
function periodValue_Del_Execute(grid_name, page_guid, uri_Content) {
    var jQgrid = $("#" + grid_name + page_guid);
    var grid = jQgrid.data("kendoGrid");
    var grid_ds = grid.dataSource;
    var grid_ds_data = grid_ds.data();
    var row_data = grid.dataItem(grid.select());

    grid_ds.remove(row_data);

    var row_last_data = grid.dataItem(grid.items().last());

    if (grid_name == "period_values_grid_" && row_last_data != null) {
        row_last_data.DateEnd = null;
        row_last_data.PeriodDate = row_last_data.PeriodDate.substr(0, 10);
    }

    grid.refresh();

    if (grid_ds_data.length > 0)
        grid_SelectRow_byUid(page_guid, grid_ds_data[grid_ds_data.length - 1].uid);

    periodValue_Save_Change_ButtomImage(page_guid, uri_Content, true);

    return false;
}

var selector_edit_row_uid = null;
function periodValue_Edit() {
    var grid_el = $("#period_values_grid_" + clickItem.page_guid);
    var grid = grid_el.data("kendoGrid");

    var grid_ds = grid.dataSource;
    var grid_ds_data = grid_ds.data();

    var edit_mode = !periodValue_Edit_Get_EditMode(clickItem.page_guid);

    periodValue_Edit_Change_EditMode(clickItem.page_guid, vm_Base.url_Content, grid, grid.select(), edit_mode);

    periodValue_Save_Change_ButtomImage(clickItem.page_guid, vm_Base.url_Content, true);
    return false;
}

function periodValue_Save() {
    var buttSave = $("#buttSave_" + clickItem.page_guid);

    if (clickItem.save == 1) {
        confirmWindow("Сохранение", "Сохранить изменения в БД ?",
            'periodValue_FindFrame("period_values_grid_", "' + clickItem.page_guid + '", "' + vm_Base.url_Content + '", 3, "' + clickItem.uri_path + '")');
    }

    return false;
}
function periodValue_Save_Execute(grid_name, page_guid, uri_Content, uri_path) {
    var grid_el = $("#" + grid_name + page_guid);
    var grid = grid_el.data("kendoGrid");
    var grid_ds = grid.dataSource;
    var grid_ds_data = grid_ds.data();

    var options = {};
    options.url = uri_Content + "Home/Period_Save";
    options.type = "POST";

    options.data = {};
    options.data._isPostBack = "true";

    options.data.sconfig_id = -1;
    options.data.uri_path = uri_path;
    options.data.stable_id = grid_el.data("stable");
    options.data.soperation_id = 10024;
    options.data.nom = 0;
    options.data.parent_id = grid_el.data("parent");
    options.data.random = Math.floor(Math.random() * 100);

    options.data.items = [];
    for (var i = 0; i < grid_ds_data.length; i++) {
        var item = {};

        if (grid_name == "period_values_grid_") {
            var date_beg = grid_ds_data[i].DateBeg;
            item.DateBeg = date_beg.getFullYear() + "." + (date_beg.getMonth() + 1) + "." + date_beg.getDate();
            item.PeriodType = "TextBox";
            item.Value = grid_ds_data[i].Value;
            item.Value2 = grid_ds_data[i].Value2;

            if (grid_el.data("selector-type") == "DatePicker")
                item.Value = grid_ds_data[i].ValueD.getFullYear() + "." + (grid_ds_data[i].ValueD.getMonth() + 1) + "." + grid_ds_data[i].ValueD.getDate();
        }
        if (grid_name == "grid_") {
            item = grid_ds_data[i];
        }

        options.data.items.push(item);
    }

    options.success = function (data) {
        grid_ds.read();
        if (data.status == "error")
            messageWindow("Ошибка", data.message, data.error_text);
    };

    $.ajax(options);

    return false;
}

//***********************************************************************************
function periodValue_Edit_Get_EditMode(page_guid) {
    var buttEdit = $("#buttEdit_" + page_guid);
    var edit_mode = false
    if (buttEdit.attr("data-edit") != null && buttEdit.attr("data-edit") == 1)
        edit_mode = true;
    return edit_mode;
}

function periodValue_Edit_Change_LastRow(page_guid) {
    var grid_el = $("#period_values_grid_" + page_guid);
    var grid = grid_el.data("kendoGrid");

    var grid_ds = grid.dataSource;
    var grid_ds_view = grid_ds.view();

    var row_last_item = grid_ds_view[grid_ds_view.length - 1];
    row_last_item.PeriodDate = DateBeg_DateEnd(row_last_item.DateBeg, null);

    if (grid_ds_view.length >= 2) {
        var row_prev_item = grid_ds_view[grid_ds_view.length - 2];
        row_prev_item.DateEnd = row_last_item.DateBeg;
        row_prev_item.PeriodDate = DateBeg_DateEnd(row_prev_item.DateBeg, row_prev_item.DateEnd);
    }

}

function periodValue_Edit_Change_RefreshDate(page_guid, grid) {
    if (selector_edit_row_index < 1)
        return;

    var grid_ds = grid.dataSource;
    var grid_ds_view = grid_ds.view();

    var row_curr_item = grid_ds_view[selector_edit_row_index];
    row_curr_item.PeriodDate = DateBeg_DateEnd(row_curr_item.DateBeg, row_curr_item.DateEnd);

    var row_prev_item = grid_ds_view[selector_edit_row_index - 1];
    row_prev_item.DateEnd = row_curr_item.DateBeg;
    row_prev_item.PeriodDate = DateBeg_DateEnd(row_prev_item.DateBeg, row_prev_item.DateEnd);

    grid.refresh();
}
function periodValue_RefreshDate(e, page_guid, grid) {
    var row = $(e.container).closest("tr");
    var rowIdx = row[0].rowIndex;
    var grid_ds = grid.dataSource;
    var grid_ds_view = grid_ds.view();

    if (rowIdx != grid_ds_view.length - 1 || e.values.DateBeg === undefined)
        return;

    var row_curr_item = grid_ds_view[rowIdx];
    var row_prev_item = grid_ds_view[rowIdx - 1];

    if (row_prev_item != undefined) {
        if (e.values.DateBeg <= row_prev_item.DateBeg) {
            e.container.find("input[name=DateBeg]").data("kendoDatePicker").value(row_prev_item.DateEnd);
            return;
        }

        row_prev_item.DateEnd = row_curr_item.DateBeg;
        row_prev_item.PeriodDate = DateBeg_DateEnd(row_prev_item.DateBeg, e.values.DateBeg);//row_prev_item.DateEnd);
    }

    row_curr_item.PeriodDate = DateBeg_DateEnd(e.values.DateBeg, row_curr_item.DateEnd);
    grid.refresh();
}

function periodValue_Edit_Change_EditMode(page_guid, uri_Content, grid, jRow, edit_mode, colIdx) {
    var row_el = $(jRow);
    var row_item = grid.dataItem(row_el);

    var buttEdit = $("#buttEdit_" + page_guid);
    var buttEdit_img = buttEdit.find("img");

    if (selector_edit_col_index != null && selector_edit_col_index != undefined && selector_edit_col_index == 0) {
        periodValue_Edit_Change_RefreshDate(page_guid, grid);
    }

    if (edit_mode) {
        buttEdit.attr("data-edit", 1);
        buttEdit_img.attr("src", uri_Content + "Images/Operations/Large/Edit_Red.png");

        selector_edit_row_uid = row_el.data("uid");

        if (colIdx == undefined)
            colIdx = 1;

        var row_td_val_el = row_el.find("td:eq(" + colIdx + ")");
        grid.editCell($("#period_values_grid_" + page_guid + " td:eq(0)"));

    }
    else {
        buttEdit.removeAttr("data-edit");
        buttEdit_img.attr("src", uri_Content + "Images/Operations/Large/Edit_Blue.png");

        var row_td_val_el = row_el.find("td:eq(" + selector_edit_col_index + ")");
        grid.closeCell(row_td_val_el);

        selector_edit_row_uid = null;
        selector_edit_col_index = null;
        selector_edit_row_index = null;
    }
}

function periodValue_Save_Change_ButtomImage(page_guid, uri_Content, save_mode) {
    var buttSave = $("#buttSave_" + page_guid);
    var buttSave_img = buttSave.find("img");

    if (save_mode) {
        buttSave.attr("data-save", 1);
        buttSave.removeClass("k-state-disabled");
        buttSave_img.attr("src", uri_Content + "Images/Operations/Large/Save_Red.png");
    }
    else {
        buttSave.removeAttr("data-save");
        buttSave.addClass("k-state-disabled");
        buttSave_img.attr("src", uri_Content + "Images/Operations/Large/Save_Blue_Disable.png");
    }
}


