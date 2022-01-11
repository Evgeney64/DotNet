var state = 0;

function mainTableFilter_Ready(vm_Base_params, url_Content, uri_path) {
    try {
        set_AutoTest_Event_Listeneres('FilterTab');
        vm_Base.initializeArguments(arguments);
        mainTable_reopen = 1;

        if (isMobile())
            create_filterPanel(vm_Base.page_guid);

        if (!isMobile()) {
            $("#MainFilterTableSplitter_" + vm_Base.page_guid).kendoSplitter({
                orientation: "horisontal",
                panes: [
                    { collapsible: true, size: "360px", scrollable: false },
                    { collapsible: false, scrollable: false }]
            });
        }

        mainTableFilter_Refresh(vm_Base.page_guid);
        var filterPanel = $("#MainFilterTable_Filter_" + vm_Base.page_guid + ".filterPanel");

        setTimeout(function () {
            setTimeout(function () {
                if (filterPanel.data("kendoResponsivePanel")) {
                    filterPanel.data("kendoResponsivePanel").open();
                }
                if ($("#MainFilterTableSplitter_" + vm_Base.page_guid).data("kendoSplitter")) {
                    $("#MainFilterTableSplitter_" + vm_Base.page_guid).data("kendoSplitter").expand(".k-pane:first");
                }

            }, 500);
        }, 400);
    }
    catch (e) {
        if (Top().isAutoTestClient)
            Top().set_Auto_Test_Log(e);
        else
            throw e;
    }
}

function mainFilter_Ready(vm_Base_params, url_Content, uri_path) {
    try {
        vm_Base.initializeArguments(arguments);

        let butt_forward = $(".filter-panel #buttForward_" + vm_Base.page_guid);
        if (butt_forward.length > 0)
            butt_forward.removeAttr("is_started");

        if (isAutoTest(vm_Base.uri_path)) {
            setTimeout(function () {
                var url_Action = vm_Base.url_Content + "Home/MainTable";
                url_Action = updateQueryStringParameter(url_Action, "uri_path", vm_Base.uri_path);
                url_Action = updateQueryStringParameter(url_Action, "guids_out", vm_Base.guids_out);

                mainTableFilter_Execute(vm_Base_params, url_Action, vm_Base.url_Content, vm_Base.uri_path);

            }, auto_test_delay);
        }
        let uri_Action = vm_Base.url_Content + "Home/MainTable";
        uri_Action = updateQueryStringParameter(uri_Action, "uri_path", vm_Base.uri_path);
        uri_Action = updateQueryStringParameter(uri_Action, "guids_out", vm_Base.guids_out);

        if (!is_true(vm_Base.IsFilterExists)) {
            mainTableFilter_Execute(vm_Base_params, uri_Action, vm_Base.url_Content, vm_Base.uri_path);
            mainTable_CloseFilterPane();
        }

        // событие выборка фильтра по нажатию <Enter>  !!! Пока скрыто из-за редактируемого грида (грид обновляется каждый раз)
        var filterSplitter = $("#MainFilterTableSplitter_" + vm_Base.page_guid);
        if (filterSplitter.length > 0 && 1 == 2) {
            filterSplitter.keydown(function (event) {
                                        //Кроме нажатия Enter при вводе номера страницы грида 
                if (event.which == 13 && $(event.target).parent()[0].className.indexOf('k-pager-input') == -1) { 
                    event.preventDefault();
                    mainTableFilter_Execute(vm_Base.vm_Base_params, uri_Action, url_Content, vm_Base.uri_path_out, true);
                }
            });
            filterSplitter.focus();
        }

        if (Top().isAutoTestClient == true)
            setTimeout(function () {
                Top().end_Auto_Test_Item(1);
            }, 1000);
    }
    catch (e) {
        if (Top().isAutoTestClient)
            Top().set_Auto_Test_Log(e);
        else
            throw e;
    }
}

function mainTable_Ready(vm_Base_params, url_Content) {
    try {
        set_AutoTest_Event_Listeneres('FilterTab');
        vm_Base.initializeArguments(arguments);

        if (isMobile() && $("#MainTableSplitter_" + vm_Base.page_guid).data("kendoSplitter") != undefined) {
            $("#MainTableSplitter_" + vm_Base.page_guid).data("kendoSplitter").remove("#MainTableSplitter_" + vm_Base.page_guid + " .k-pane:last");
        }

        /*После окончания загрузки содержимого панелей, показываем их. Чтобы не было видно скачков разметки по мере загрузки содержимого*/
        if ($(".k-splitter > .k-pane.is-invisibility").length > 0)
            $(".k-splitter > .k-pane.is-invisibility").removeClass("is-invisibility");

        /***Drag&Drop for grids (пока убрано!)***/
        if (vm_Base.parent_guid != vm_Base.page_guid && 1 == 0) {
            var grid1 = window.parent.$("#grid_" + vm_Base.parent_guid).data("kendoGrid");
            var grid2 = $("#grid_" + vm_Base.page_guid).data("kendoGrid");

            $(grid1.element).kendoDraggable({
                filter: "tr",
                hint: function (e) {
                    var item = $('<div class="k-grid k-widget" style="background-color: DarkOrange; color: black;"><table><tbody><tr>' + e.html() + '</tr></tbody></table></div>');
                    return item;
                },
                group: "gridGroup1",
            });

            $(grid2.element).kendoDraggable({
                filter: "tr",
                hint: function (e) {
                    var item = $('<div class="k-grid k-widget" style="background-color: MediumVioletRed; color: black;"><table><tbody><tr>' + e.html() + '</tr></tbody></table></div>');
                    return item;
                },
                group: "gridGroup2",
            });

            grid1.table.kendoDropTarget({
                drop: function (e) {
                    var dataItem = grid2.DataSource.getByUid(e.draggable.currentTarget.data("uid"));
                    grid2.dataSource.remove(dataItem);
                    grid1.dataSource.add(dataItem);
                },
                group: "gridGroup2",
            });

            grid2.table.kendoDropTarget({
                drop: function (e) {
                    var dataItem = grid1.DataSource.getByUid(e.draggable.currentTarget.data("uid"));
                    grid1.DataSource.remove(dataItem);
                    grid2.DataSource.add(dataItem);
                },
                group: "gridGroup1",
            });
        }

        var childGrid = $("#grid_" + vm_Base.page_guid);
        var kendoObject = childGrid.attr("kendoObject");
        var childGridData = childGrid.data(kendoObject);

        $("#MainTable_MenuOper_" + vm_Base.page_guid).kendoPopup({
            anchor: $("#menu_toggle_MainTable_MenuOper_" + vm_Base.page_guid),
        }).removeClass("k-group");

        // прячем панелбар в всплывающее окно
        $("#MainTable_MenuList_" + vm_Base.page_guid).kendoPopup({
            anchor: $("#menu_toggle_MainTable_MenuList_" + vm_Base.page_guid),
        }).removeClass("k-group");

        // контекстное меню
        $("#contextMenuGrid_" + vm_Base.page_guid).kendoContextMenu({
            target: "#grid_" + vm_Base.page_guid,
            filter: ".grid-cell-btn",
            showOn: "click",
            select: function (e) {

                let grid = $("#grid_" + vm_Base.page_guid).data("kendoGrid");
                if (grid == undefined)
                    grid = $("#grid_" + vm_Base.page_guid).data("kendoTreeList")

                if (grid != undefined) {
                    grid.clearSelection();
                    grid.select($(e.target).closest("tr"));
                }

                $($(e)[0].target).attr("list", $($(e)[0].item).attr("data-list"));
                clickItem_Exec($(e)[0].target);
            }
        });

        $("#contextMenu2Grid_" + vm_Base.page_guid).kendoContextMenu({
            target: "#grid_" + vm_Base.page_guid,
            filter: ".grid-cell-menu-btn",
            showOn: "click",
            select: function (e) {
                let grid = $("#grid_" + vm_Base.page_guid).data("kendoGrid");
                if (grid != undefined) {
                    grid.clearSelection();
                    grid.select($(e.target).closest("tr"));
                }

                let item = $(e.item);
                if (item.data("nom") != null)
                {
                    let nom = item.data("nom");
                    if ($('td > li[nom=' + nom + ']').length > 0)
                        $('td > li[nom=' + nom + ']').click()
                }
            }
        });

        if (is_true(vm_Base.is_context_toolbar)) {
            $("#contextToolBar_" + vm_Base.page_guid).kendoContextMenu({
                target: "#grid_" + vm_Base.page_guid,
                filter: ".k-grid-content",
                //showOn: "click",
                open: function (e) {
                    let grid = $("#grid_" + vm_Base.page_guid).data("kendoGrid");
                    if (grid != undefined) {
                        grid.clearSelection();
                        grid.select($(e.event.target).closest("tr"));
                    }
                },
                select: function (e) {
                    let item = $(e.item);
                    if (item.data("nom") != null) {
                        let nom = item.data("nom");
                        if ($('td > li[nom=' + nom + ']').length > 0)
                            $('td > li[nom=' + nom + ']').click()
                    }
                }
            });
        }

        if (vm_Base.selector_header_path == undefined || vm_Base.selector_header_path === "")
            mainTableSplitter_LastPane_Collapse(vm_Base.page_guid, true);

        //Этот код прерывает отправку формы. Submit иногда срабатывает после нажатия enter при вводе номера страницы в пейджинге грида
        childGrid.keydown(function (event) {
            if (event.which == 13 && $(event.target).parent()[0].className.indexOf('k-pager-input') == 0) {
                event.preventDefault();
            }
        });
    }
    catch (e) {
        if (Top().isAutoTestClient)
            Top().set_Auto_Test_Log(e);
            //set_AutoTest_Error(e);
        else
            throw e;
    }
}

function childTabEdit_Ready(page_guid, url_Content) {
    entity_status = 1;
    //entity_ChangeStatus(uri_Content, page_guid, true);
}

function dashBord_Ready(vm_Base_params, url_Content) {
    //initialize(arguments);
    vm_Base.initializeArguments(arguments);

    if (isAutoTest(vm_Base.uri_path_out))
        mainTab_Open_AutoTest(vm_Base.url_Content, vm_Base.mainTab_Text, vm_Base.guids_out, vm_Base.uri_path_out, vm_Base.is_last, "dashBord_Ready");

    window.parent.$("iframe").removeClass("loader-spin");
}

function tabEdit_Layout_Ready() {
    var html1 = $("*").html();
    var sss = 0;
    $(window).resize(function () {
        $("#EditTab > ul").width(0);
    }); 
}

function tabEdit_Ready(vm_Base_params, url_Content) {
    try {
        //initialize(arguments);
        vm_Base.initializeArguments(arguments);

        if (window.frameElement != undefined)
            $(window.frameElement).removeClass('loader-spin');

        if ($('#EditTab').length > 0)
            $('#EditTab').removeClass('loader-spin');

        change_arrows();

        var tab_name = 'FilterTab';

        if (Top().isAutoTestClient == true) {
            setTimeout(function () {
                if (vm_Base != undefined && is_true(vm_Base.editor_in_main_tab)) {
                    Top().end_Auto_Test_Item(1);
                    Top().end_Auto_Test_Item(2);
                }
                if (vm_Base != undefined && !is_true(vm_Base.editor_in_main_tab)) {
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

function tabMainEdit_Ready(vm_Base_params, url_Content) {
    try {
        //initialize(arguments);
        vm_Base.initializeArguments(arguments);

        var tab_name = 'FilterTab';

        if (Top().isAutoTestClient == true) {
            setTimeout(function () {
                if (vm_Base != undefined && is_true(vm_Base.editor_in_main_tab)) {
                    if (is_true(vm_Base.MainTabTitle_FromGrid)) {
                        Top().end_Auto_Test_Item();
                        Top().close_Auto_Test_Other_Items(1);
                    }
                    else {
                        Top().end_Auto_Test_Item(1);
                        Top().end_Auto_Test_Item(2);
                    }
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

function modalWindow_Ready() {
    /*Работа 6002. Центрирование модального диалога после загрузки содержимого*/
    if ($(".k-window-content").length > 0) {
        let i = $(".k-window-content").length - 1;
        let dlg = $($(".k-window-content")[i]).data("kendoWindow");
        if (dlg != undefined)
            dlg.center().open();
    }

    if (Top().isAutoTestClient == true) { 
        setTimeout(function () { 
            click_Auto_Test_Modal();
        }, 500);
    }
}

//***********************************************************************************
function contextMenu_Click_From_Grid() {
    let controller_name = "Home/TabEdit_Layout/";
    if (clickItem.list == "list") {
        controller_name = "Home/MainTable/";
    }

    let grid = $("#grid_" + vm_Base.page_guid).data("kendoGrid");

    if ((!grid) || (grid.length <= 0))
        grid = $("#grid_" + vm_Base.page_guid).data("kendoTreeList");

    let cell_target = $(clickItem.sender);
    let row = cell_target.closest("tr");
    let row_data = grid.dataItem(row);

    var parent_id = row_data[cell_target.data("fkfield")];
    if (parent_id == null) {
        messageWindow("Ошибка", "Для данного объекта код не задан");
        return;
    }

    if ($("#grid_" + vm_Base.page_guid).attr("guids_out") != undefined)
        vm_Base.guids_out = $("#grid_" + vm_Base.page_guid).attr("guids_out");

    clickItem.url_action = vm_Base.url_Content + controller_name;

    var stable_id = 0;
    if (cell_target.data("content") != null)
        stable_id = cell_target.data("content");
    if (stable_id == 0 && cell_target.data("anytablefield") != null) {
        stable_id = row_data[cell_target.data("anytablefield")]
    }
    if (stable_id == 0) {
        messageWindow("Ошибка", "Не задан тип объекта");
        return;
    }
    clickItem.url_action = addToUrl(clickItem.url_action, "uri_path", cell_target.data("uripath"));
    clickItem.url_action = addToUrl(clickItem.url_action, "grid_cell_content", stable_id);
    clickItem.url_action = addToUrl(clickItem.url_action, "parent_id", parent_id);

    if (cell_target.data("anytablenamefield") != undefined && cell_target.data("anytablenamefield") != "")
        clickItem.title = row_data[cell_target.data("anytablenamefield")];
    if (clickItem.title == undefined)
        clickItem.title = vm_Base.title;

    if (clickItem.list == "list") {
        vm_Base.editor_in_main_tab = "False";
        clickItem.url_action = addToUrl(clickItem.url_action, "_iframe", 1);
        mainTab_Open();
    } else {
        vm_Base.tab = $("#MainTableTabChild_" + vm_Base.page_guid);
        return childTab_EditOpen();
    }
}

function contextMenu_Click_From_Grid_old(e) {
    let controller_name = "Home/TabEdit_Layout/";
    if ($(e.item).data("list") == "list") {
        controller_name = "Home/MainTable/";
    }

    let grid = $("#grid_" + vm_Base.page_guid).data("kendoGrid");
    // Pavlov 10.01.2017
    // мы находимся либо в гриде, либо в дереве
    if ((!grid) || (grid.length <= 0))
        grid = $("#grid_" + vm_Base.page_guid).data("kendoTreeList");

    let cell_target = $(e.target);
    let row = cell_target.closest("tr");
    let row_data = grid.dataItem(row);

    var parent_id = row_data[cell_target.data("fkfield")];
    if (parent_id == null) {
        messageWindow("Ошибка", "Для данного объекта код не задан");
        return;
    }

    if ($("#grid_" + vm_Base.page_guid).attr("guids_out") != undefined)
        vm_Base.guids_out = $("#grid_" + vm_Base.page_guid).attr("guids_out");

    vm_Base.url_Action = url_Content + controller_name;

    var stable_id = 0;
    if (cell_target.data("content") != null)
        stable_id = cell_target.data("content");
    if (stable_id == 0 && cell_target.data("anytablefield") != null) {
        stable_id = row_data[cell_target.data("anytablefield")]
    }
    if (stable_id == 0) {
        messageWindow("Ошибка", "Не задан тип объекта");
        return;
    }
    vm_Base.url_Action = addToUrl(vm_Base.url_Action, "uri_path", cell_target.data("uripath"));
    vm_Base.url_Action = addToUrl(vm_Base.url_Action, "grid_cell_content", stable_id);
    vm_Base.url_Action = addToUrl(vm_Base.url_Action, "parent_id", parent_id);

    vm_Base.title = cell_target.data("tabtitle");
    if (vm_Base.title.length == 0)
        vm_Base.title = row_data[cell_target.data("anytablenamefield")]

    if ($(e.item).data("list") == "list") {
        vm_Base.editor_in_main_tab = "False";
        vm_Base.url_Action = addToUrl(vm_Base.url_Action, "_iframe", 1);
        mainTab_Open();
    } else {
        vm_Base.tab = $("#MainTableTabChild_" + vm_Base.page_guid);
        return childTab_EditOpen();
    }
}
//***********************************************************************************
function draggableOnDragStart(e) {
    window.parent.$("#draggable").addClass("hollow");
    $("#droptarget").text("Drop here.");
    $("#droptarget").removeClass("painted");
}

function droptargetOnDragEnter(e) {
    $("#droptarget").text("Now drop...");
    $("#droptarget").addClass("painted");
}

function droptargetOnDragLeave(e) {
    $("#droptarget").text("Drop here.");
    $("#droptarget").removeClass("painted");
}

function droptargetOnDrop(e) {
    $("#droptarget").text("You did great!");
    window.parent.$("#draggable").removeClass("hollow");
}

function draggableOnDragEnd(e) {
    var draggable = window.parent.$("#draggable");

    if (!draggable.data("kendoDraggable").dropped) {
        // drag ended outside of any droptarget
        $("#droptarget").text("Try again!");
    }
    draggable.removeClass("hollow");
}

// *******************************************************************************************
function contextMenuCreate(page_guid, guids_out, uri_Content) {
    $("#contextMenuEditor1").kendoContextMenu({
        target: "a.k-icon.k-i-group",
        select: function (e) {
            contextMenuSelect(page_guid, guids_out, uri_Content, e)
        }
    });
    $("#contextMenuEditor2").kendoContextMenu({
        target: "a.k-icon.k-i-search",
        select: function (e) {
            contextMenuSelect(page_guid, guids_out, uri_Content, e)
        }
    });
}

// *******************************************************************************************
function create_filterPanel(page_guid) {
    var filterPanel = $("#MainFilterTable_Filter_" + page_guid); // + ".filterPanel"
    filterPanel.kendoResponsivePanel({
        breakpoint: 769,
        orientation: "left",
        toggleButton: ".k-lpanel-toggle",
        autoClose: false
    });

    // реагируем на свайп
    filterPanel.kendoTouch({
        enableSwipe: true,
        swipe: function (e) {
            if (e.direction === "left") {
                filterPanel.data("kendoResponsivePanel").close();
                $("#buttForward_" + page_guid).trigger("click");
            }
        }
    });
}

function create_filterConfigPanel(page_guid) {
    var filterConfigPanel = $("#MainFilterTable_Config_" + page_guid);
    filterConfigPanel.kendoResponsivePanel({
        breakpoint: 769,
        orientation: "left",
        toggleButton: ".k-lpanel-toggle",
        autoClose: false
    });

    // реагируем на свайп
    filterConfigPanel.kendoTouch({
        enableSwipe: true,
        swipe: function (e) {
            if (e.direction === "right") {
                filterConfigPanel.data("kendoResponsivePanel").close();
                $("#buttConfigCancel_" + page_guid).trigger("click");
            }
        }
    });
}

// *******************************************************************************************
function editor_SetReadOnly(page_guid) {
    var editor = $("#val_" + page_guid).data("kendoEditor");
    var editorBody = $(editor.body);

    // make readonly
    editorBody.removeAttr("contenteditable").find("a").on("click.readonly", false);

    // make editable
    //editorBody.attr("contenteditable", true).find("a").off("click.readonly");
}

function controlItemValue_Ready(page_guid, source) {
    var iframe = $('#iframe_' + page_guid);
    iframe.contents().find('body').html(source);
}

function setupXmlArea(id, content) {

    //var docSpec={
    //    onchange: function(){
    //        console.log("I been changed now!")
    //    },
    //    validate: function(obj){
    //        console.log("I be validatin' now!")
    //    },
    //    elements: {
    //        "EDIT": {
    //            menu: [{
    //                caption: "Добавить <ITEM>",
    //                action: Xonomy.newElementChild,
    //                actionParameter: "<ITEM/>"
    //            }]
    //        },
    //        "ITEM": {
    //            menu: [{
    //                caption: "Добавить @Field=\"\"",
    //                action: Xonomy.newAttribute,
    //                actionParameter: { name: "Field", value: "" },
    //                hideIf: function(jsElement){
    //                    return jsElement.hasAttribute("Field");
    //                }
    //            }, {
    //                caption: "Add @label2=\"m\"",
    //                action: Xonomy.newAttribute,
    //                actionParameter: { name: "label2", value: "m" },
    //                hideIf: function (jsElement) {
    //                    return jsElement.hasAttribute("label2");
    //                }
    //            }, {
    //                caption: "Удалить этот <ITEM>",
    //                action: Xonomy.deleteElement
    //            }, {
    //                caption: "Новый <ITEM> перед",
    //                action: Xonomy.newElementBefore,
    //                actionParameter: "<ITEM/>"
    //            }, {
    //                caption: "Новый <ITEM> после",
    //                action: Xonomy.newElementAfter,
    //                actionParameter: "<ITEM/>"
    //            }],
    //            canDropTo: ["Field"],
    //            attributes: {
    //                "Field": {
    //                    asker: Xonomy.askString,
    //                    menu: [{
    //                        caption: "Удалить этот @Field",
    //                        action: Xonomy.deleteAttribute
    //                    }]
    //                },
    //                "label2": {
    //                    asker: Xonomy.askPicklist,
    //                    askerParameter: [
    //                        {value: "m", caption: "male"},
    //                        {value: "f", caption: "female"}
    //                    ],
    //                    menu: [{
    //                        caption: "Delete this @label2",
    //                        action: Xonomy.deleteAttribute
    //                    }]
    //                }
    //            }
    //        }
    //    }
    //};

    var docSpec={
        onchange: function(){
            console.log("I been changed now!")
        },
        validate: function(obj){
            console.log("I be validatin' now!")
        },
        elements: {
            "EDIT": {
                menu: [{
                    caption: "Append an <EXPANDER>",
                    action: Xonomy.newElementChild,
                    actionParameter: "<EXPANDER/>"
                }]
            },
            "EXPANDER": {
                menu: [{
                    caption: "Add @Name=\"something\"",
                    action: Xonomy.newAttribute,
                    actionParameter: { name: "Name", value: "something" },
                    hideIf: function(jsElement){
                        return jsElement.hasAttribute("Name");
                    }
                }, {
                    caption: "Delete this <EXPANDER>",
                    action: Xonomy.deleteElement
                }, {
                    caption: "New <EXPANDER> before this",
                    action: Xonomy.newElementBefore,
                    actionParameter: "<EXPANDER/>"
                }, {
                    caption: "New <EXPANDER> after this",
                    action: Xonomy.newElementAfter,
                    actionParameter: "<EXPANDER/>"
                }],
                canDropTo: ["EDIT"],
                attributes: {
                    "Name": {
                        asker: Xonomy.askString,
                        menu: [{
                            caption: "Delete this @Name",
                            action: Xonomy.deleteAttribute
                        }]
                    }
                }
            }
        }
    };



    var div = document.getElementById(id);
    var xml = $(div).html(); //$(div).val();
    //var xml = "<EDIT><EXPANDER Name='NDOCUMENT_REPORT_ID' /><EXPANDER Name='REPORT_ID' /><EXPANDER Name='NDOCUMENT_ID' /></EDIT>";
    if ($('#' + id).length > 0 /*&& content != ''*/)
    {
        Xonomy.render(xml, div, docSpec);
    }
}

function visualTemplate(options) {
    var dataviz = kendo.dataviz;
    var g = new dataviz.diagram.Group();
    var dataItem = options.dataItem;

    if (dataItem != undefined) {
        if (dataItem.type == "Circle") {
            g.append(new dataviz.diagram.Circle({
                width: dataItem.width,
                height: dataItem.height,
                radius: dataItem.radius,
                stroke: {
                    width: 2,
                    color: "#586477"
                },
                fill: dataItem.Color
            }));
            g.append(new dataviz.diagram.TextBlock({
                x: 50,
                y: 15,
                text: dataItem.Text
            }));
        };
        if (dataItem.type == "Rhombus") {
            g.append(new dataviz.diagram.Path({
                data: "M 70 0 L 140 30 L 70 60 L 0 30 z",
                width: dataItem.width,
                height: dataItem.height,
                stroke: {
                    width: 2,
                    color: "#586477"
                },
                fill: dataItem.Color
            }));
            var layout = new dataviz.diagram.Layout(new dataviz.diagram.Rect(75, 30, dataItem.width-135, dataItem.height-30), {
                //alignContent: "center",
                spacing: 4
            });

            g.append(layout);

            var texts = dataItem.Text.split(" ");
            for (var i = 0; i < texts.length; i++) {
                layout.append(new dataviz.diagram.TextBlock({
                    text: texts[i]
                }));
            }
            layout.reflow();
            //g.append(new dataviz.diagram.TextBlock({
            //    x: 50,
            //    y: 25,
            //    text: dataItem.Text
            //}));
        };
        if (dataItem.type == "Rectangle") {
            g.append(new dataviz.diagram.Rectangle({
                width: dataItem.width,
                height: dataItem.height,
                stroke: {
                    width: 2,
                    color: "#586477"
                },
                fill: dataItem.Color
            }));
            if (dataItem.DOCUMENT_ITEM != undefined && dataItem.DOCUMENT_ITEM != "")
                g.append(new dataviz.diagram.Rectangle({
                    width: 8,
                    height: dataItem.height,
                    fill: "#2372ea",
                    stroke: {
                        width: 0
                    }
                }));

            var layout = new dataviz.diagram.Layout(new dataviz.diagram.Rect(10, 10, dataItem.width, dataItem.height), {
                //alignContent: "center",
                spacing: 4
            });

            g.append(layout);

            var texts = dataItem.Text.split(" ");
            for (var i = 0; i < texts.length; i++) {
                layout.append(new dataviz.diagram.TextBlock({
                    text: texts[i]
                }));
            }
            layout.reflow();
        }
        if (dataItem.type == "Text")
        {
            //g.append(new dataviz.diagram.TextBlock({
            //    width: dataItem.width,
            //    height: dataItem.height,
            //    text: dataItem.Text
            //}));
            var layout = new dataviz.diagram.Layout(new dataviz.diagram.Rect(0, 0, dataItem.width, dataItem.height), {
                //alignContent: "center",
                spacing: 4
            });

            g.append(layout);

            var texts = dataItem.Text.split(" ");
            for (var i = 0; i < texts.length; i++) {
                layout.append(new dataviz.diagram.TextBlock({
                    text: texts[i],
                }));
            }
            layout.reflow();
        }
    }

    return g;
}

function onDataBound(e) {
    var that = this;
    setTimeout(function () {
        that.bringIntoView(that.shapes);
    }, 0);
}

function createDiagram(parent_id, uri_path, stable_id, url_content) {
    var serviceRoot = url_content + "Home";
    var url = "?";
    url = addToUrl(url, 'parent_id', parent_id);
    url = addToUrl(url, 'uri_path', uri_path);
    url = addToUrl(url, 'stable_id', stable_id);

    var shapesDataSource = {
        batch: false,
        transport: {
            read: {
                url: serviceRoot + "/CustomizeDiagramShapes" + url,
                dataType: "jsonp"
            },
            update: {
                url: serviceRoot + "/UpdateShape" + url,
                dataType: "jsonp"
            },
            destroy: {
                url: serviceRoot + "/DestroyShape" + url,
                dataType: "jsonp"
            },
            create: {
                url: serviceRoot + "/UpdateShape" + url,
                dataType: "jsonp"
            },
            parameterMap: function (options, operation) {
                if (operation !== "read") {
                    return { models: kendo.stringify(options.models || [options]) };
                }
            }
        },
        schema: {
            model: {
                id: "id",
                fields: {
                    id: { from: "Id", type: "number", editable: false },
                    Text: { type: "string" },
                    x: { from: "X", type: "number"/*, editable: false*/ },
                    y: { from: "Y", type: "number"/*, editable: false*/ },
                    type: { from: "Type", type: "string", editable: false },
                    width: { from: "Width", type: "number"/*, editable: false*/ },
                    height: { from: "Height", type: "number"/*, editable: false*/ },
                    radius: { from: "Radius", type: "number" },
                    DOCUMENT_ITEM: { from: "DOCUMENT_ITEM", type: "string" },
                    Color: {
                        type: "string", defaultValue: "#e8eff7",
                        parse: function (val) {
                            return (val == "" || val == undefined) ? "#e8eff7" : val;
                        }
                    }
                }
            }
        }
    };

    var connectionsDataSource = {
        batch: false,
        transport: {
            read: {
                url: serviceRoot + "/CustomizeDiagramConnections" + url,
                dataType: "jsonp"
            },
            update: {
                url: serviceRoot + "/UpdateConnection" + url,
                dataType: "jsonp"
            },
            destroy: {
                url: serviceRoot + "/DestroyConnection" + url,
                dataType: "jsonp"
            },
            create: {
                url: serviceRoot + "/UpdateConnection" + url,
                dataType: "jsonp"
            },
            parameterMap: function (options, operation) {
                if (operation !== "read") {
                    return { models: kendo.stringify(options.models || [options]) };
                }
            }
        },
        schema: {
            model: {
                id: "id",
                fields: {
                    id: { from: "Id", type: "number", editable: false },
                    from: { from: "From", type: "number" },
                    to: { from: "To", type: "number" },
                    fromConnector: { from: "FromConnector", type: "string" },
                    toConnector: { from: "ToConnector", type: "string" },
                    fromX: { from: "FromX", type: "number" },
                    fromY: { from: "FromY", type: "number" },
                    toX: { from: "ToX", type: "number" },
                    toY: { from: "ToY", type: "number" }
                }
            }
        }
    };

    $("#diagram").kendoDiagram({
        dataSource: shapesDataSource,
        connectionsDataSource: connectionsDataSource,
        layout: false,
        shapeDefaults: {
            visual: visualTemplate,
            connectors: [{ name: "auto" }, { name: "top" }, { name: "left" }, { name: "bottom" }, { name: "right" }],
            editable: {
                tools: [
                    {
                        name: "edit"
                    },
                    {
                        name: "delete"
                    },
                    {
                        type: "button",
                        icon: "refresh",
                        click: function () {
                            var diagram = $("#diagram").getKendoDiagram();
                            var selected = diagram.select();
                            selected[0].redrawVisual();
                        }
                    }
                ]
            }
        },
        connectionDefaults: {
            stroke: {
                color: "#586477",
                width: 2
            }
        },
        dataBound: onDataBound,
        editable: {
            shapeTemplate: kendo.template($("#popup-editor").html()),
            tools: [
                    {
                        name: "diagram-rectangle-btn",
                        type: "button",
                        click: function () {
                            addShapeToDiagram("Rectangle", 200, 80);
                            //moveTextShapesToEnd();
                        }
                    },
                    {
                        name: "diagram-circle-btn",
                        type: "button",
                        click: function () {
                            addShapeToDiagram("Circle", 180, 60, 60);
                            //moveTextShapesToEnd();
                        }
                    }
                    , {
                        name: "diagram-text-btn",
                        type: "button",
                        click: function () {
                            addShapeToDiagram("Text", 150, 30);
                            //moveTextShapesToEnd();
                        }
                    }
                    , {
                        name: "diagram-rhombus-btn",
                        type: "button",
                        click: function () {
                            addShapeToDiagram("Rhombus", 200, 70);
                            //moveTextShapesToEnd();
                        }
                    }
                    , {
                        type: "button",
                        text: "Open",
                        click: function (options) {
                            var diagram = $("#diagram").getKendoDiagram();
                            var selected = diagram.select();
                            var uir_path;
                            var guids_out;
                            var parent_id;
                            var paragraph_id;
                            var page_guid;

                            var url = serviceRoot + "/TabEdit_Layout";
                            if ($('#diagram').attr("uri_path").length > 0)
                                uri_path = $('#diagram').attr("uri_path");
                            if ($('#diagram').attr("guids_out").length > 0)
                                guids_out = $('#diagram').attr("guids_out");
                            if ($('#diagram').attr("stable_id").length > 0)
                                stable_id = $('#diagram').attr("stable_id");
                            if (selected[0].dataItem.DOCUMENT_ITEM != undefined) {
                                parent_id = selected[0].dataItem.DOCUMENT_ITEM.split("-")[0];
                                paragraph_id = selected[0].dataItem.DOCUMENT_ITEM.split("-")[1];
                            }
                            if ($('#diagram').attr("page_guid").length > 0)
                                page_guid = $('#diagram').attr("page_guid");

                            url = addToUrl(url, "uri_path", uri_path);
                            url = addToUrl(url, "guids_out", guids_out);
                            url = addToUrl(url, "title", "Просмотр");
                            url = addToUrl(url, "parent_id", parent_id);
                            url = addToUrl(url, "paragraph_id", paragraph_id);
                            url = addToUrl(url, 'stable_id', stable_id);

                            if (parent_id != undefined && parent_id != "")
                                childTab_EditOpen(url, "Корректировать", page_guid, guids_out, $("#MainTableTabChild_" + page_guid))
                        }
                    }
            ]}
    });
}

function addShapeToDiagram(type, width, height, radius) {
    var diagram = $("#diagram").getKendoDiagram();
    var selected = diagram.select();
    var x = 0;
    var y = 0;
    if (selected != undefined && selected.length > 0) {
        x = selected[0].dataItem.x - 50;
        y = selected[0].dataItem.y - 50;
    }
    diagram.dataSource.add({ id: 0, type: type, Text: "Текст", x: x, y: y, width: width, height: height, radius: radius });
    diagram.dataSource.sync();
    diagram.bringIntoView(diagram.shapes);
}

function moveTextShapesToEnd() {
    var diagram = $("#diagram").getKendoDiagram();
    var length = diagram.dataSource.data().length;
    for (var i = 0; i < length-1; i++)
    {
        var item = diagram.dataSource.data()[i];
        if (item.type == "Text") {
            diagram.dataSource.add({
                id: 0, type: "Text", Text: item.Text,
                width: item.width, height: item.height, radius: item.radius,
                x: item.x, y: item.y
            });
            diagram.dataSource.remove(item);
        }
    }
    diagram.dataSource.sync();
    diagram.bringIntoView(diagram.shapes);
}

function editorScrollTo(page_guid, id) {
    var editor = $('#val_' + page_guid).data("kendoEditor");
    var theElement = editor.window.document.getElementById(id);
    var position; 
    var selectedPosX = 0;
    var selectedPosY = 0;

    if (id != undefined && id != "" && theElement != undefined) {
        setTimeout(function () {
            position = theElement.getBoundingClientRect();
            selectedPosY = position.top - 25;
            //theElement.scrollIntoView(top);
            editor.window.scrollTo(0, selectedPosY);
        }, 100);
    }
}

function get_lik_modal_window(nalgorithm_id) {
    if (nalgorithm_id == undefined || nalgorithm_id == 0)
        return;

    var url = vm_Base.url_Content + "Home/ExecAlgorithm";
    url = addToUrl(url, "nalgorithm_id", nalgorithm_id);
    url = addToUrl(url, "uri_path", vm_Base.uri_path);

    $.ajax({
        url: url,
        type: "POST",
        contentType: false,
        processData: false,
        success: function (data) {
            if (data.error)
                return;
            if (data.text != undefined)
                messageWindow("Сообщение:", data.text);
        }
    })
}