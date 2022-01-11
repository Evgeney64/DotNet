
function multicombo_onChange(sender, page_guid) {
    //var sender_el = $(sender.element);
    //var link_tr = sender_el.closest("tr");
    //var link_check = link_tr.find("input[type=checkbox]");
    //var link_value = link_tr.find("input[name]");
    var link_check = $('#check_' + page_guid);
    var link_value = $('#val_' + page_guid);

    link_check.prop("checked", true);
    //combo_Checked(page_guid);

    $(link_value[1]).prop("value", link_value.data("kendoMultiSelect").dataItems());
    var is_checked = link_check.prop("checked");
    if (is_checked == true) {
        link_value.attr("Changed", "Changed");
    }
    else {
        link_value.removeAttr("Changed");

    }
    //messageWindow("combo_onChange", page_guid, link_tr.html());
}

function combo_onChange(sender, page_guid, ev) {
    var link_check = $('#check_' + page_guid);
    var link_value = $('#val_' + page_guid);
    var link_value2 = $('#val2_' + page_guid);

    var sender_el = $(sender.element);
    if (sender_el[0].value == "" && ev == "dataBound")
        return;

    link_check.prop("checked", true);

    $(link_value[1]).prop("value", sender_el[0].value);
    if (link_value.length > 0 && link_value.attr("name").indexOf(".Changed") == -1) {
        link_value.attr("name", link_value.attr("name") + ".Changed");
    }
    if (link_value.attr("Changed") == undefined || link_value.attr("Changed") == "") {
        link_value.attr("Changed", "Changed");
    }

    $(link_value2[1]).prop("value", sender_el[0].value);
    if (link_value2.length > 0 && link_value2.attr("name").indexOf(".Changed") == -1) {
        link_value2.attr("name", link_value2.attr("name") + ".Changed");
    }
    if (link_value2.attr("Changed") == undefined || link_value2.attr("Changed") == "") {
        link_value2.attr("Changed", "Changed");
    }

    /*Обновление выпадающих списков каскадных combobox'ов (при наличии атрибутов Nom в конфиге) */
    if (link_value.attr("cascade_noms") != undefined && link_value.attr("cascade_noms") != "")
        combo_CascadeNoms(link_value.attr("cascade_noms"));

    /*Очистка выбранных значений дочерних каскадных combobox'ов при измении значения родительского*/
    control_ClearAllChildValues(page_guid);

    control_RefreshChilds(page_guid);
}

//Обработчик нужен только для каскадных combobox'ов, т.к. событие Open (в отличие от Change) вызывается перед контроллером
function combo_onOpen(sender, page_guid, ev) {
    var link_check = $('#check_' + page_guid);
    var link_value = $('#val_' + page_guid);
    var link_value2 = $('#val2_' + page_guid);

    link_check.prop("checked", true);

    if (link_value.length > 0 && link_value.attr("name").indexOf(".Changed") == -1) {
        link_value.attr("name", link_value.attr("name") + ".Changed");
        link_value.attr("Changed", "Changed");
    }

    if (link_value2.length > 0 && link_value2.attr("name").indexOf(".Changed") == -1) {
        link_value2.attr("name", link_value2.attr("name") + ".Changed");
        link_value2.attr("Changed", "Changed");
    }

    if (link_value.data('kendoComboBox') != undefined)
        link_value.data('kendoComboBox').dataSource.read();
}

function combo_Checked(page_guid) {
    var link_check = $('#check_' + page_guid);
    var link_value = $('#val_' + page_guid);
    var is_checked = link_check.prop("checked");
    //link_value.prop("disabled", is_checked);
    if (is_checked == true) {
        //link_value.removeProp("disabled", "disabled");
        link_value.removeAttr("disabled");
    }
    else {
        //link_value.prop("disabled", true);
        link_value.attr("disabled", "disabled");
    }

    //messageWindow(step, page_guid, link_check.parent().parent().html());
}

function multiBox_SelectValue(sender) {
    var kendoMultiSelect = sender;
    var value = kendoMultiSelect.input.val();
    if (value != "") {
        var valueExists = kendoMultiSelect.dataSource.data().some(function (item) {
            return item.Value == value;
        })

        if (!valueExists) {
            kendoMultiSelect.dataSource.add({
                Text: value,
                Value: value,
            });
        }

        var values = kendoMultiSelect.value();
        values.push(value);
        kendoMultiSelect.value(values);
        if (kendoMultiSelect.options.autoClose) {
            kendoMultiSelect.close();
        }
        kendoMultiSelect.trigger("change");

        return true;
    }
    return false;
}
function multiBox_onInit(sender, page_guid, isNumeric) {
    var kendoMultiSelect = sender;
    kendoMultiSelect.input.keydown(function (e) {
        if (isNumeric === true) {
            if (!e.ctrlKey && !e.metaKey && !e.altKey && e.key.length === 1) {
                var text = $(this).val() + e.key;
                if (!$.isNumeric(text)) {
                    e.preventDefault();
                }
            }
        }
        if (e.keyCode === 9) {
            var result = multiBox_SelectValue(kendoMultiSelect);
            if (result) {
                e.preventDefault();
            }
        }
    });
    kendoMultiSelect.input.change(function (e) {
        var result = multiBox_SelectValue(kendoMultiSelect);
    });
}

function datePickerRangeBeg_onChange(sender, page_guid) {
    var kendoDatePickerBeg = sender;
    var kendoDatePickerEnd = $('#' + kendoDatePickerBeg.element.attr('idvalend')).data('kendoDatePicker');
    if (kendoDatePickerEnd) {
        var dateBeg = kendoDatePickerBeg.value()
        if (dateBeg) {
            kendoDatePickerEnd.min(dateBeg);
        }
        else {
            kendoDatePickerEnd.min(new Date(1900, 1, 1));
        }
    }

    control_Click(page_guid);
}
function datePickerRangeEnd_onChange(sender, page_guid) {
    var kendoDatePickerEnd = sender;
    var kendoDatePickerBeg = $('#' + kendoDatePickerEnd.element.attr('idvalbeg')).data('kendoDatePicker');
    if (kendoDatePickerBeg) {
        var dateEnd = kendoDatePickerEnd.value()
        if (dateEnd) {
            kendoDatePickerBeg.max(dateEnd);
        }
        else {
            kendoDatePickerBeg.max(new Date(2100, 1, 1));
        }
    }

    control_Click(page_guid);
}

// string Events *******************************************************************************************
function control_Checked(page_guid) {
    var link_check = $('#check_' + page_guid);
    var link_value = $('#val_' + page_guid);
    var link_checkbox_value = $('#checkbox_' + page_guid);
    var is_checked = link_check.prop("checked");

    if (is_checked != true) {
        link_value.removeAttr("Changed");
        link_checkbox_value.removeAttr("Changed");
    }
    else {
        link_value.attr("Changed", "Changed");
        link_checkbox_value.attr("Changed", "Changed");
    }
}

function checkbox_Checked(page_guid) {
    var link_checkbox = $('#checkbox_' + page_guid);
    var link_check = $('#check_' + page_guid);
    if (link_checkbox.attr("checked") == null) {
        link_check.attr("checked", "checked");
        link_check.prop("checked", true);
        link_checkbox.attr("checked", "checked");
        link_checkbox.prop("checked", true);
    }
    else {
        link_check.removeAttr("checked");
        link_checkbox.removeAttr("checked");
    }
    if (link_checkbox.attr("Changed") == undefined)
        link_checkbox.attr("Changed", "Changed");
}

function control_Click(page_guid, reverse) {
    var link_check = $('#check_' + page_guid);
    //link_check.prop("checked", true);
    var checked = reverse ? !link_check.is(':checked') : true;
    link_check.prop("checked", checked);

    var control = $("#val_" + page_guid);
    if (control.length > 0) {
        if (control.attr("name") != undefined && control.attr("name").indexOf(".Changed") == -1)
            control.attr("name", control.attr("name") + ".Changed");
        control.attr("Changed", "Changed");
    }

    var controlRight = $("#valRight_" + page_guid);
    if (controlRight.length > 0) {
        if (controlRight.attr("name") != undefined && controlRight.attr("name").indexOf(".Changed") == -1)
            controlRight.attr("name", controlRight.attr("name") + ".Changed");
        controlRight.attr("Changed", "Changed");
    }

    var control2 = $("#val2_" + page_guid);
    if (control2.length > 0) {
        if (control2.attr("name") != undefined && control2.attr("name").indexOf(".Changed") == -1)
            control2.attr("name", control2.attr("name") + ".Changed");
        control2.attr("Changed", "Changed");
    }
}

/*Обновление содержимого combobox'ов*/
function combo_CascadeNoms(noms) {
    var arr_noms = noms.split(",");
    for (let i = 0; i <= arr_noms.length-1; i++) {
        var combo = $('[nom="' + arr_noms[i] + '"]');
        if (combo.length > 0) {
            kcombo = combo.data('kendoComboBox');
            if (kcombo != undefined)
                kcombo.dataSource.read();
        }
    }
}

/*Обновление значений зависимых контролов*/
function control_RefreshChilds(page_guid) {
    let control = $('#val_' + page_guid);
    if (control.length == 0)
        return;

    let has_childs = control.attr("has_childs");

    if (has_childs === "true") {
        let data = {};
        let url;
        url = vm_Base.url_Content + "Home/GetChildControls";

        data.nom = control.attr("nom");
        data.value = control_GetValue(page_guid);
        if (control.attr("selector_id") != undefined)
            data.selector_id = control.attr("selector_id");
        data.uri_path = control.attr("uri_path");

        $('span [nom_parent=' + data.nom + ']').map(function (i, child) {
            control_EmptyDiv_SetValue(child.getAttribute("page_guid"), "Загрузка...");
        });

        $.ajax({
            url: url,
            type: "POST",
            data: data,
            success: function (result) {
                if (result.status != "success")
                    return;

                if (result.data == undefined || result.data.length == 0) {
                    control_ClearAllChilds(undefined, result.nom);
                    return;
                }
                    result.data.map(function (item) {
                        if (item.Nom != undefined) {
                            var edit_tabs_count = $('#EditTab .k-content[role="tabpanel"]').length;
                            var child_control = $('#EditTab-' + edit_tabs_count + ' input[nom=' + item.Nom + '], .k-window-content input[nom=' + item.Nom + '] ');

                            if (child_control.length > 0) {
                                let control_page_guid = child_control.attr("page_guid");
                                control_SetValue(control_page_guid, item.Value, item.Text);

                                control_EmptyDiv_SetValue(control_page_guid, "");
                            }
                        }
                    });
            }
        });
    }
}

/*Универсальная функция присваивания значений контролам*/
function control_SetValue(page_guid, value, text) {
    let control = $('#val_' + page_guid);
    if (control.length > 0) {
        control_Checked(page_guid);

        if (control.attr("controltype") == "Selector") {
            control.attr("selector_id", value);
            $('#caption_' + page_guid).val(text);
            control.val(selector_Result(value, text));
            return;
        }

        if (control.attr("controltype") == "ComboBox" /*|| control.attr("controltype") == "CascadedComboBox"*/) {
            control.data("kendoComboBox").setDataSource(value);
        }

        control.val(value);
    }
}

/*Универсальная функция очистки только выбранных значений контролов*/
function control_ClearValue(page_guid) {
    let control = $('#val_' + page_guid);
    if (control.length > 0) {

        if (control.attr("controltype") == "ComboBox" || control.attr("controltype") == "CascadedComboBox") {
            control.data("kendoComboBox").value("");
            return;
        }

        control.val(undefined);
    }
}

/*Универсальная функция получения значения контрола*/
function control_GetValue(page_guid) {
    let control = $('#val_' + page_guid);
    if (control.length > 0) {
        if (control.attr("controltype") == "ComboBox") {
            var kendoComboBox = $(control).data("kendoComboBox");
            return kendoComboBox.value();
        }

        if (control.attr("controltype") == "DropDownTree") {
            var kendoDropDownTree = $(control).data("kendoDropDownTree");
            return kendoDropDownTree.value();
        }

        return control.attr("value");
    }
}

/*Отображение рядом с контролом текста*/
/*(в будущем добавить иконки (загрузка, ожидание и т.п.)*/
function control_EmptyDiv_SetValue(page_guid, value) {
    let empty_div = $('#empty_div_' + page_guid);
    if (empty_div.length > 0) {
        empty_div.text(value);
    }
}

/*Очистка зависимых контролов*/
function control_ClearAllChilds(page_guid, nom) {
    let control;
    if (page_guid != undefined)
        control = $('#val_' + page_guid);
    if (nom != undefined)
        control = $('[nom=' + nom + ']');

    if (control.length == 1) {
        if (control.attr("nom") != undefined) {
            $('span [nom_parent=' + control.attr("nom") + ']').map(function (i, child) {
                control_EmptyDiv_SetValue(child.getAttribute("page_guid"), "");

                control_SetValue(child.getAttribute("page_guid"), "", "");
            });
        }
        if (control.attr("page_guid") != undefined) {
            $('span [parent_combo_guid=' + control.attr("page_guid") + ']').map(function (i, child) {
                control_SetValue(child.getAttribute("page_guid"), "", "");
            });
            $('span [parent_combo_guid_second=' + control.attr("page_guid") + ']').map(function (i, child) {
                control_SetValue(child.getAttribute("page_guid"), "", "");
            });
        }
    }
}

/*Очистка только выбранных значений у зависимых контролов*/
function control_ClearAllChildValues(page_guid, nom) {
    let control;
    if (page_guid != undefined)
        control = $('#val_' + page_guid);
    if (nom != undefined)
        control = $('[nom=' + nom + ']');

    if (control.length == 1) {
        if (control.attr("nom") != undefined) {
            $('span [nom_parent=' + control.attr("nom") + ']').map(function (i, child) {
                control_ClearValue(child.getAttribute("page_guid"));
            });
        }
        if (control.attr("page_guid") != undefined) {
            $('span [parent_combo_guid=' + control.attr("page_guid") + ']').map(function (i, child) {
                control_ClearValue(child.getAttribute("page_guid"));
            });
            $('span [parent_combo_guid_second=' + control.attr("page_guid") + ']').map(function (i, child) {
                control_ClearValue(child.getAttribute("page_guid"));
            });
        }
    }
}

/*Работа 6983. Функция для передачи параметров контрол GetCascadeComboItems. Пока не используется, не получается передать в функцию все guid'ы*/
function filterComboItems123(page_guid, parent_combo_guid, parent_combo_guid_second) {
    return;
    var control = $("#val_" + page_guid);
    if (control.length == 0)
        return;
    var control_parent = $("#val_" + parent_combo_guid);
    var control_parent_second = $("#val_" + parent_combo_guid_second);
    var fk_value;
    var fk_value_second;
    var fk_value_checked = "True";
    var fk_value_second_checked = "True";

    if (control_parent.length > 0)
        fk_value = control_parent.val();
    if (control_parent_second.length > 0)
        fk_value_second = control_parent_second.val();

    if ($("#check_" + parent_combo_guid).length > 0)
        fk_value_checked = $("#check_" + parent_combo_guid).prop("checked");
    if ($("#check_" + parent_combo_guid_second).length > 0)
        fk_value_second_checked = $("#check_" + parent_combo_guid_second).prop("checked");

    var filter_string = "";
    if (control.data("kendoComboBox") != undefined)
        filter_string = control.data("kendoComboBox").input.val();

    return {
        uri_path: control.attr("uri_path"),
        fk_value: fk_value, 
        fk_value_checked: fk_value_checked,
        fk_value_second: fk_value_second,
        fk_value_second_checked: fk_value_second_checked,
        default_value: control.val(),
        filter_string: filter_string,
        init_value: control.val(),
        random_value: getRandomValue(),
        fk_name: control.attr("fk_name"),
        table_name: control.attr("table_name"),
        sql_string: control.attr("sql_string"),
        top: control.attr("top")
    };
}

// DealInfo *******************************************************************************************
var deal_info_change_combo_item_first = 1;
var deal_info_ndepartment_id = 0;
function deal_info_change_combo_item(sender, vm_Base_params, url_Content, controller_name, uri_path) { /*, guids_out, page_guid, has_main_menu_sections, _deal_info_ndepartment_id, main_page_title*/
    //initialize(arguments);
    vm_Base.initializeArguments(arguments);

    if (deal_info_change_combo_item_first == 1) {
        deal_info_change_combo_item_first = 0;
        deal_info_ndepartment_id = vm_Base._deal_info_ndepartment_id;
        return;
    }

    var sender_el = $(sender.element);
    var deal_id = sender_el[0].value;
    if (deal_id == "")
        deal_id = sender_el[0].defaultValue;//$("#val_" + page_guid + "_listbox").find("li.k-state-hover")[0].attr("add_to_dealinfo_combo_id");
    if (deal_id != "") {
        deal_info_set_content(deal_id, vm_Base.url_Content, vm_Base.guids_out, vm_Base.page_guid, vm_Base.uri_path, 0, vm_Base.has_main_menu_sections);
    }

    mainTab_Close_All();

    vm_Base.url_Action = vm_Base.url_Content + controller_name;
    vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "deal_id", deal_id);
    vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "get_deal_info", true);
    vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "get_deal_info_first", 0);
    vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "uri_path", vm_Base.uri_path);
    vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "guids_out", vm_Base.guids_out);
    vm_Base.url_Action = updateQueryStringParameter(vm_Base.url_Action, "temp_value", getRandomValue());

    var tabstrip = $("#tabstrip");
    if (vm_Base.main_page_title == undefined)
        vm_Base.main_page_title = "Главная страница";
    if (isMobile()) {
        remove_mobile_view("3");
        create_new_mobile_view(vm_Base.main_page_title, "", 3);
        //create_new_mobile_view("Главная страница", "", 3);
        tabstrip = $("#mobile_view3");
    }

    vm_Base.title = "Главная страница";
    vm_Base.mainTab_IsAutoLoaded = "True";
    mainTab_Open();
}

function deal_info_set_content(deal_id, uri_Action_prefix, guids_out, page_guid, uri_path, is_first, has_main_menu_sections) {

    var uri_Action = uri_Action_prefix + "Home/GetDealInfo?";
    uri_Action = updateQueryStringParameter(uri_Action, "deal_id", deal_id);
    uri_Action = updateQueryStringParameter(uri_Action, "uri_path", uri_path);
    uri_Action = updateQueryStringParameter(uri_Action, "guids_out", guids_out);
    uri_Action = updateQueryStringParameter(uri_Action, "index_guid", page_guid);
    uri_Action = updateQueryStringParameter(uri_Action, "get_deal_info_first", is_first);
    uri_Action = updateQueryStringParameter(uri_Action, "temp_value", getRandomValue());

    var options_deal_info = {};
    options_deal_info.url = uri_Action;
    options_deal_info.type = "POST";

    options_deal_info.success = function (result_deal_info) {
        if (is_first == 1) {
            var combo_for_dealnum = Top().$('#deal_content');

            combo_for_dealnum.empty();
            combo_for_dealnum.html(result_deal_info);

            if (typeof Top == 'function')
                Top().leftPaneWindowResize();
        }
        else {

            if (has_main_menu_sections == 1 && result_deal_info["deal_info_ndepartment_id"] != undefined) {
                var _deal_info_ndepartment_id = result_deal_info["deal_info_ndepartment_id"].Value;
                var _deal_info_ndepartment_id = result_deal_info["deal_info_ndepartment_id"].Value;
                if (deal_info_ndepartment_id != 0 && deal_info_ndepartment_id != _deal_info_ndepartment_id) {
                    var _uri_path = result_deal_info["uri_path"].Value;

                    var uri_Action1 = uri_Action_prefix + "Home/MainMenu?";
                    uri_Action1 = updateQueryStringParameter(uri_Action1, "deal_id", deal_id);
                    //uri_Action1 = updateQueryStringParameter(uri_Action1, "main_menu_section", result_deal_info["main_menu_section"].Value);
                    //uri_Action1 = updateQueryStringParameter(uri_Action1, "ndepartment_id", deal_ndepartment_id);
                    uri_Action1 = updateQueryStringParameter(uri_Action1, "uri_path", _uri_path);
                    uri_Action1 = updateQueryStringParameter(uri_Action1, "guids_out", guids_out);
                    uri_Action1 = updateQueryStringParameter(uri_Action1, "temp_value", getRandomValue());

                    var options_main_menu = {};
                    options_main_menu.url = uri_Action1;
                    options_main_menu.type = "GET";

                    options_main_menu.success = function (result_main_menu) {
                        var main_menu_content = $('#menu_block_vertical');
                        if (main_menu_content.length == 0)
                            main_menu_content = $('#menu_block_horizontal');
                        if (main_menu_content.length > 0) {
                            main_menu_content.empty();
                            main_menu_content.html(result_main_menu);
                        }
                    }
                    $.ajax(options_main_menu);
                }

                deal_info_ndepartment_id = _deal_info_ndepartment_id;
            }

            var deal_content = Top().$('#deal_content');

            var els = deal_content.find("[deal_info='1']");
            if (els.length > 0) {
                for (var i = 0; i < els.length; i++) {
                    var el = $(els[i]);

                    var deal_info_type = el.attr("deal_info_type");
                    var deal_info_name = el.attr("deal_info_name");
                    var deal_info = result_deal_info[deal_info_name];

                    if (deal_info.InVisibility) {
                        if (el.hasClass("is-not-visible") == false)
                            el.addClass("is-not-visible");
                    }
                    else {
                        if (el.hasClass("is-not-visible"))
                            el.removeClass("is-not-visible");
                    }

                    if (deal_info_type != undefined) {
                        if (deal_info_type == "Image")
                            continue;
                        
                        if (result_deal_info.hasOwnProperty(deal_info_name) != null && result_deal_info[deal_info_name] != null) {
                            
                            if (deal_info_type == "TextBox" || deal_info_type == "Numeric") {
                                if ($('#val_' + el.attr('page_guid')).data('kendoNumericTextBox') != undefined)
                                    $('#val_' + el.attr('page_guid')).data('kendoNumericTextBox').value(deal_info.Text);
                                else
                                    $('#val_' + el.attr('page_guid')).val(deal_info.Text);
                            }

                            if (deal_info_type == "Link" || deal_info_type == "Label")
                                el.text(deal_info.Text);
                            if (deal_info_type == "QRCode")
                                //QRcode теперь обновляется целиком из контроллера. От QR-кода telerik'а пришлось отказаться из-за проблем с кодировкой русских символов
                                $('#qr-code').html(deal_info.Value);
                                //$('#val_' + el.attr('page_guid')).data('kendoQRCode').value(deal_info.Text)

                            if (deal_info.Url != null) {
                                var href = deal_info.Url.replace("~/", uri_Action_prefix);
                                el.attr("href", href);
                            }

                            if (deal_info.link_disabled != null)
                                el.attr("link_disabled", deal_info.link_disabled);

                            if (deal_info.report_id != null)
                                el.attr("report_id", deal_info.report_id);
                            else
                                el.attr("report_id", 0);
                        }
                    }
                }
            }
        }
    };

    $.ajax(options_deal_info);
}
