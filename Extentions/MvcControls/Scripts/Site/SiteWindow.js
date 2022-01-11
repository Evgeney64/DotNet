
// modalWindow *******************************************************************************************
var modWindow;

function menuModalWindow_Verify(vm_Base_params, sender, _title, url, page_guid, guids_out, soperation_id) {
    //initialize(arguments, true);
    vm_Base.initializeArguments(arguments);

    var row_id = getRowId_ByGuid(page_guid);
    if (row_id == null && isOperationsForRow(soperation_id) == true) {
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
    url = updateQueryStringParameter(url, "parent_id", parent_id);
    url = updateQueryStringParameter(url, "is_operation_rule", 1);
    options.url = url;
    options.type = "POST";

    options.success = function (data) {
        if (data.status === "error") {
            messageWindow("Ошибка", data.message, data.error_text);
        }
        else {
            menuModalWindow(vm_Base_params, sender, _title, url, page_guid, guids_out, soperation_id);
        }
    };

    $.ajax(options);
}

function menuModalWindow(vm_Base_params, sender, _title, url_Action, page_guid, guids_out, soperation_id, _width, _height) {

    vm_Base.initializeArguments(arguments);

    var row_id = String(getRowId_ByGuid(clickItem.page_guid, vm_Base));
    if (row_id == "null" || row_id == "undefined")
        row_id = null;

    if (row_id == null && (is_true(vm_Base.editor_in_main_tab) || is_true(vm_Base.save_in_main_tab))) {
        if ($(clickItem.sender).attr("parent_id") != undefined) {
            row_id = $(clickItem.sender).attr("parent_id");
            parent_id = row_id;
        }
        if ($(clickItem.sender).attr("row_id") != undefined) {
            row_id = $(clickItem.sender).attr("row_id");
            parent_id = row_id;
        }
    }

    if (clickItem.row_id != undefined)
        row_id = clickItem.row_id;

    /*Сделать эту проверку для каждой кнопки отдельно. У spreadsheet могут появиться модальные операции со строками*/
    let is_spreadsheet_toolbar = false;
    if ($('.k-menu[is_spreadsheet_toolbar="True"]').length > 0)
        is_spreadsheet_toolbar = true;

    if (row_id == null && isOperationsForRow(clickItem.soperation_id) == true && !is_spreadsheet_toolbar) {
        messageWindow("Ошибка", "Выберите строку.");
        return;
    }
    if (clickItem.soperation_id == sop_12 && vm_Base.parent_id == 0) {
        messageWindow("Ошибка", "Основной объект еще не существует.");
        return;
    }

    if (row_id != null)
        clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "parent_id", row_id);
    clickItem.url_action = mainGrid_Parameters(vm_Base, clickItem.url_action, clickItem.soperation_id);

    var grid = $("#grid_" + vm_Base.page_guid);
    if (grid != undefined && grid.attr("parent_id") != undefined && clickItem.soperation_id == 12) {
        clickItem.url_action = addToUrl(clickItem.url_action, "grand_parent_id", grid.attr("parent_id"));
        vm_Base.selectAfterAdd = true;
    }

    if ($(clickItem.sender).attr("volume_values") != undefined)
        clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "volume_values", $(clickItem.sender).attr("volume_values"));
    if ($(clickItem.sender).attr("value_type") != undefined)
        clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "value_type", $(clickItem.sender).attr("value_type"));

    if (row_id != null) {
        var parent_id = row_id;
        var index_semicolon = row_id.indexOf(";");
        if (index_semicolon >= 0) {
            parent_id = row_id.substring(0, index_semicolon);
        }
        clickItem.url_action = updateQueryStringParameter(clickItem.url_action, "parent_id", parent_id);
    }

    if (_width == undefined || _width < 300) _width = 300;
    if (_height == undefined || _height < 100) _height = 100;
    modalWindow(clickItem.title, clickItem.url_action, _width, _height, undefined, clickItem.page_guid);
}

// Динамический контент окна
function modalWindow(_title, url, _width, _height, _top, main_table_guid, add_uri_Action) {
    if (_width == undefined || _width < 300) _width = 300;
    if (_height == undefined || _height < 100) _height = 100;

    if (add_uri_Action == 1)
        _title += " (" + uri_Action + ")";

    var div = "<div></div>";

    Top().modWindow = Top()
        .$(div)
        .kendoWindow({
            content: url,
            title: _title,
            visible: false,
            modal: true,
            iframe: false,

            minWidth: 300,
            minHeight: _height,

            close: function (ev) {
                modalWindow_OnClose(ev, main_table_guid);
            },

            deactivate: function () {
                this.destroy();
            }
        })
        .data("kendoWindow").center();

    var wrapper = Top().modWindow.wrapper;

    if (_top == undefined) {
        wrapper.css({ top: _top });
    }

    Top().modWindow.open();
}

function modalWindow_OnClose(ev, page_guid) {
    if (ev.sender.save_result == 1) {

        gridRefresh_AfterOperation(
            page_guid,
            ev.sender.soperation_id,
            ev.sender.new_item,
            ev.sender.pk_field,
            ev.sender.refresh_grid,
            ev.sender.refresh_full,
            ev.sender.fk_field
        );
    }
}

function modalWindow_Close(parent_guid) {
    try {
        if (Top().modWindow) {
            Top().modWindow.close();
            Top().modWindow = undefined;
        }
    }
    catch (e) {
        try {
            if (parent.modWindow) {
                parent.modWindow.close();
                parent.modWindow = undefined;
            }
        }
        catch (e) {
            if (modWindow) {
                modWindow.close();
                modWindow = undefined;
            }
        }
    }
}

// messageWindow *******************************************************************************************

var messWindow;
function messageWindow(_title, message_title, message_text, _height, _width) {
    //if (_width == undefined || _width < 300) _width = 300;
    //if (_height == undefined || _height < 100) _height = 100;

    var content = "";

    message_title = message_title ? message_title : '';
    var message_titles = message_title ? message_title.split('\n') : [];
    if (message_titles.length > 0) {
        message_title = "";
        for (var i = 0; i < message_titles.length; i++) {
            message_title += "<div>" + message_titles[i] + "</div>"
        }
    }

    var funcClose = "messageWindow_Close();";

    if (message_text == null || message_text == undefined || message_text == "") {
        content =
            "<div>" +
            "   <div class='modal-window-content-textarea is-fulled is-width-fulled is-not-bordered is-not-scrolled'>" + message_title.trim() + "</div>" +
            "   <div class='k-buttons-panel is-width-fulled'>" +
            "       <a class='ctrl-modal-window-ok-button k-button k-primary is-align-right' onclick='" + funcClose + "'>OK</a>" +
            "   </div>" +
            "</div>";
    }
    else {
        message_text = message_text ? message_text : '';
        var message_texts = message_text ? message_text.split('\n') : [];
        if (message_texts.length > 0) {
            message_text = "";
            for (var i = 0; i < message_texts.length; i++) {
                message_text += "<div>" + message_texts[i] + "</div>"
            }
        }
        content =
            "<div>" +
            "   <div class='modal-window-content-textarea is-fulled is-width-fulled is-not-bordered is-not-scrolled'>" + message_title.trim() +
            "   <p>" +
            "   <div class='modal-window-content-textarea-detail is-width-fulled is-bordered is-scrolled'>" + message_text.trim() + "</div></div>" +
            "   <div class='k-buttons-panel is-width-fulled'>" +
            "       <input type='checkbox' onclick='messageWindow_CheckDetail(this);'>Детали</input>" +
            "       <a class='k-button k-primary is-align-right' onclick='" + funcClose + "'>Закрыть</a>" +
            "   </div>" +
            "</div>"
        ;
    }
    //if (Top().isAutoTestNew) {
    //    if (Top().isErrorAutoTest) {
    //        content += "<script>Top().exec_Custom_Event('MainMenu', 'autoClickModalOk')</script>";  //$($('iframe')[0]).
    //    }
    //    else
    //        content += "<script>$('iframe')[0].contentWindow.exec_Custom_Event('FilterTab', 'autoClickModalOk')</script>";  //$($('iframe')[0]).
    //}
    if (Top().isAutoTestClient) {
        content += "<script>click_Auto_Test_Modal()</script> ";
    }

    Top().messWindow = Top()
        .$(content)
        .kendoWindow({
            //width: _width + "px",
            //height: _height + "px",
            title: _title,
            visible: false,
            resizable: false,
            modal: true,
            close: messageWindow_Close,
            actions: ["Close"]
        })
        .data("kendoWindow");

    Top().messWindow.center().open();
}

function messageWindow_CheckDetail(evt) {
    //var parent_div = $(evt).parent().parent();
    //var textarea = $(evt).parent().parent().children().eq(1);

    //var height = 100;
    if ($(evt).prop("checked")) {
        //var window_height = $(parent_div).data('window_height');
        //var textarea_height = $(textarea).data('textarea_height');
        //height = textarea_height + window_height;

        //$(textarea).show();

        $("div.modal-window-content-textarea-detail").show(); //attr("display", "block")
    }
    else {
        $("div.modal-window-content-textarea-detail").hide(); //.attr("display", "none")
        //$(textarea).hide();
    }
    //Top().$(".k-window").height(height);
    //messageWindow("", "textarea_height", textarea_height);
}

function messageWindow_Close(e) {
    if (Top().messWindow || Top().$('.k-widget.k-window').length > 0) {

        /*Удаляем элемент из DOM принудительно. Иначе остается куча невидимых старых диалогов*/
        //Top().$('body')[0].removeChild(Top().document.querySelector('.k-widget.k-window'));
        Top().$('body')[0].removeChild(Top().$('.k-widget.k-window')[Top().$('.k-widget.k-window').length - 1]);
        Top().messWindow.close();
        Top().messWindow = undefined;
    }
    if (Top().document.querySelector('.k-overlay') != null) 
        Top().$('body')[0].removeChild(Top().document.querySelector('.k-overlay'))
}

function confirmWindow(_title, message_title, callback_Ok) {

    var _content = "";
    if (message_title != undefined && callback_Ok != undefined) {
        _content =
            "<div>" +
            "   <div><p>" + message_title.trim() + "<p><p></div>" +
            "   <div class='k-buttons-panel is-width-fulled'>" +
            "       <a class='k-button k-cancel' onclick='messageWindow_Close();'>Закрыть</a>" +
            "       &nbsp;&nbsp;&nbsp;&nbsp;" +
            "       <a class='k-button k-primary is-align-right' onclick='" + callback_Ok + ";'>Выполнить</a>";
        "   </div>" +
            "</div>";
    }
    else {
        _content = _title;
    }
    try {
        Top().messWindow = Top().$(_content)
            .kendoWindow({
                title: _title,
                visible: false,
                resizable: false,
                modal: true,
                actions: ["Close"],
            })
            .data("kendoWindow");

        Top().messWindow.center().open();
    }
    catch (e) {
        parent.messWindow = parent.$(_content)
            .kendoWindow({
                title: _title,
                visible: false,
                resizable: false,
                modal: true,
                actions: ["Close"],
            })
            .data("kendoWindow");

        parent.messWindow.center().open();
    }
}

function get_simple_modalWindow(_title, url, content) {
    var window_content = url;
    if (url == undefined)
        window_content = $(content);

    var kendoWindow =
        $("<div />").kendoWindow({
            title: _title,
            modal: true,
            resizable: false,

            minWidth: 300,
            minHeight: 100,
        });

    kendoWindow.data("kendoWindow")
        .content(window_content)
        .center().open();

    return kendoWindow;
}

function exec_modalWindow(_title, url, content) {
    var modalWindow = get_simple_modalWindow(_title, url, content);
    if (modalWindow != undefined)
    {
        //modalWindow.open();

        modalWindow
        .find(".delete-confirm,.delete-cancel")
            .click(function () {
                if ($(this).hasClass("delete-confirm")) {
                    modalWindow.data("kendoWindow").close();

                    var event = document.createEvent('Event');
                    event.initEvent('dialog_OK', true, true)
                    document.dispatchEvent(event);
                }
                if ($(this).hasClass("delete-cancel")) {
                    modalWindow.data("kendoWindow").close();
                }
            })
        .end()
    }
}

// configWindow *******************************************************************************************
var mainTableFilterConfigs = [];
function configWindow_Visibility(/*page_guid, uri_path, config_show, guids_out*/) { //vm_Base_params, url_Content, config_show
    //initialize(arguments, true);
    //vm_Base.initializeArguments(arguments);

    var filterConfigPanel = $("#MainFilterTable_Config_" + clickItem.page_guid);

    if (isMobile()) {
        create_filterConfigPanel(clickItem.page_guid);
    }

    if (clickItem.config_show == 1) {
        var mainTableFilterConfig_isLoaded = 0;
        for (let i = 0; i < mainTableFilterConfigs.length; i++) {
            if (mainTableFilterConfigs[i] == clickItem.page_guid) {
                mainTableFilterConfig_isLoaded = 1;
                break;
            }
        }
        if (mainTableFilterConfig_isLoaded == 0) {
            mainTableFilterConfig_Open();
            mainTableFilterConfigs.push(clickItem.page_guid);
        }

        $("#MainFilterTable_Filter_" + clickItem.page_guid).hide();
        $("#MainFilterTable_Config_" + clickItem.page_guid).show();
        if (filterConfigPanel.data("kendoResponsivePanel")) {
            filterConfigPanel.data("kendoResponsivePanel").open();
        }
    }
    else {
        $("#MainFilterTable_Filter_" + clickItem.page_guid).show();
        $("#MainFilterTable_Config_" + clickItem.page_guid).hide();
        if (filterConfigPanel.data("kendoResponsivePanel")) {
            filterConfigPanel.data("kendoResponsivePanel").close();
        }
    }
}

var confWindow;
//var configWindow_CheckedNodes = [];
function configWindow_CheckNode(evt) {
    var input = $(evt.node).find("input");
    var checked_attr = input.attr('checked');
    if (checked_attr == undefined) {
        input.attr('checked', "checked");
    }
    else {
        input.attr('checked', null);
    }
}

function configWindow_Execute(/*page_guid, url_Action, uri_path*/) {  //vm_Base_params, url_Action
    //initialize(arguments, true);
    //vm_Base.initializeArguments(arguments);
    formData.readInputs("myFilterConfigForm_" + clickItem.page_guid);

    //formData = createNewFormData();
    formData.append("_isPostBack", true);
    formData.append("Operation", "ConfigSave1");
    //formData.append("soperation_id", 10022);
    //formData.append("random_value", getRandomValue());
    //formData.append("uri_path", vm_Base.uri_path);

    var config_div = $("#MainFilterTable_Config_" + clickItem.page_guid);
    var config_inputs = config_div.find("input");

    for (var i = 0; i < config_inputs.length; i++) {
        var checked_attr = $(config_inputs[i]).attr('checked');
        if (checked_attr != undefined) {
            formData.append(config_inputs[i].value, true);
        }
    }

    $.ajax({
        url: clickItem.url_action,
        type: "POST",
        data: formData.formDataObject,//formData.toFormData(),
        contentType: false,
        processData: false,
        success: function (data) { 
            mainTableFilter_Refresh(/*clickItem.page_guid*/);
        }
    });

    configWindow_Visibility(/*vm_Base.page_guid, vm_Base.uri_path, 0*/);
}

function configWindow_Close() {
    if (confWindow) {
        confWindow.close();
        confWindow = undefined;
    }
}

function configWindow_Ready() {
    //Отключаем checkbox'ы у автополей
    $('li[data-isauto="True"]').find('input').attr("disabled", "disabled");
    //Делаем серый фон у автополей
    $('li[data-isauto="True"]').find('span.k-in').attr("style", "background-color: lightgray");
    //Делаем невидимым checkbox на всехуровнях дерева
    $('li[data-parent="True"]').find('input').attr("style", "display: none");
    //Удаляем атрибут "невидимости" на дочерних уровнях дерева (ничего лучше пока не придумал)
    $('li[data-child="True"]').find('input[style="display: none"]').removeAttr("style");
}

function link_modalWindow(sender, vm_Base_params, url, page_guid, title) {
    if ($("#val_" + page_guid).attr("group") != undefined) {
        var group = $("#val_" + page_guid).attr("group");
        url = updateQueryStringParameter(url, "control_group", group);

        //initialize(arguments);
        vm_Base.initializeArguments(arguments);
        formData.readInputs("myForm", group);

        url = formData.toUrl(url);
        //var options = {};
        //options.data = {};
        //var message = entity_ReadData(options, null, page_guid, null, group);
        //var options_data = options.data;
        //$.each(options.data, function (data, options_data) {
        //    url = updateQueryStringParameter(url, data, options_data);
        //})
    }
    modalWindow(title, url);
}

function modalEditorVolumeValue(e, page_guid)
{
    var rows = $('#rows');
    if (rows.length > 0)
    {
        let calendar = $('#val_' + page_guid).data("kendoCalendar");
        if (calendar == undefined || calendar.value() == null)
            return;

        let nom = 1;
        if ($('.row:last').length > 0)
            nom = Number($('.row:last').attr('nom')) + 1;
        let month = Number(calendar.value().getMonth()) + 1;
        let value_str = kendo.toString(calendar.value(), "dd.MM.yyyy");//calendar.value().getDate() + "." + month + "." + calendar.value().getFullYear()
        let h =
            "<div id='row" + nom + "' class='row' nom='" + nom + "' style='height: 30px; width: 90%;'>"  //is-width-full
                + "<div class='is-align-left' style='height: 30px; width: 75px;'><span id='modal_row_text' name='VOLUME_VALUE.VALUE." + nom + ".Date.Editor.0' class='ctrl-input' value='" + value_str + "' changed='Changed' istext='True'>" //is-align-left
                    + value_str
                + "</span></div>"
                + "<div class='is-align-left' style='height: 30px; width: 85px;'><input id='row_input_time" + nom + "' name='VOLUME_VALUE.VALUE." + nom + ".Time.Editor.0' class='k-textbox k-header ctrl-input Tiny ' changed='Changed' value='0000'></input></div>" //is-align-left
                //+ "<input id='row_input_value" + nom + "' name='VOLUME_VALUE.VALUE." + nom + ".VALUE.Editor.0' class='k-textbox k-header ctrl-input Tiny is-align-left' value='' changed='Changed'></input>"
                + "<div class='is-align-left' style='height: 30px; width: 21px;'><span class='k-link k-icon k-i-close ' onclick=\"modalEditorVolumeValueRemove('row" + nom + "')\" style='margin-top: 5px;'></span></div>" //is-align-left
                + "<div class='is-align-left' style='height: 30px; width: 21px;'><span class='k-link k-icon k-i-arrow-e ' onclick=\"modalEditorVolumeValueShowDetail('" + nom + "')\" style='margin-top: 5px;'></span></div>" //is-align-left
                + "<div class='is-align-left' style='height: 30px; width: 6px;'><input id='row_input_detail_hidden" + nom + "' style='visibility: hidden; width: 6px;' value=''/></div>"
            + "</div>";
        rows.append(h);
        $('#row_input_time' + nom).kendoMaskedTextBox({
            mask: "h0:m0",
            rules: { h: /[0-2]/, m: /[0-5]/}
        });
        $('#bottom input.k-textbox').val($('.row').length);
    }
}
function modalEditorVolumeValueRemove(row)
{
    if ($('#' + row).length > 0)
        $('#' + row).remove();
    $('#bottom input.k-textbox').val($('.row').length);
}
function modalEditorVolumeValueShowDetail(row_nom) {
    if ($('#row_input_detail_hidden' + row_nom).length > 0) {
        $('#row_input_detail').text($('#row_input_detail_hidden' + row_nom).val());
    }
}   

function clickItem_Exec(sender) {
    clickItem.initialize(sender);
    clickItem.click();
}

