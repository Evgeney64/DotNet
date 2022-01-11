// not used *******************************************************************************************
function addButtonToMenuToolbar(pane, text, cb) {
    if (pane.find('.k-toolbar:first').length > 0) {
        $(pane.find('.k-toolbar:first').eq(0)).data("kendoToolBar").add({
            type: "button",
            text: text,
            click: cb
        });
    }
    if (pane.find('.k-menu:first').length > 0) {
        $(pane.find('.k-menu:first').eq(0)).data("kendoMenu").append({
            text: text
        });
        $(pane.find('.k-menu:first').eq(0)).data("kendoMenu").bind("select", function (e) {
            if ($(e.item).children(".k-link").text() === text) {
                cb();
            }
        });
    }
}

// функция выдергивание параметров из строки адреса
function getParameterByName(name) {
    var match = RegExp('[?&]' + name + '=([^&]*)').exec(window.location.search);
    return match && decodeURIComponent(match[1].replace(/\+/g, ' '));
}

function onMenuClick(event) {
    event.preventDefault();
    var link = $(event.item).find(">a");
    if (link.length === 1) {
        modalWindow("title", $(event.item).find(">a").attr("href"));
    }
}

var prev = undefined;
// функция открытия нового уровня меню
function openMobileMenu(menuId, event) {
    if (event) {
        event.preventDefault();
        event.stopPropagation();
    }
    if (prev) {
        prev.data("kendoMobileActionSheet").close();
    }
    setTimeout(function () {
        prev = $("#" + menuId);
        prev.data("kendoMobileActionSheet").open();
    }, 100);
    return false;
}

function closeMainTableTab_ByTitle(title, page_guid) {
    var tab = $("#MainTableTabChild_" + page_guid);
    tab.data("kendoTabStrip").remove(tab.find("ul li:contains('" + title + "')"));
    if (tab.find("ul li:last-child").length > 0) {
        tab.data("kendoTabStrip").activateTab(tab.find("ul li:last-child"));
    }
    $("#" + openedTabs[title]).find(".fa-check-square-o").remove();
    delete openedTabs[title];
}

function openFilterPane(page_guid) {
    if (window.parent.$(".filterPanel").data("kendoResponsivePanel")) {
        if (window.parent.$(".filterPanel").hasClass("k-rpanel-expanded")) {
            window.parent.$(".filterPanel").data("kendoResponsivePanel").close();
        } else {
            window.parent.$(".filterPanel").data("kendoResponsivePanel").open();
        }
    }
    if (window.parent.$("#TabTable_Content_" + page_guid).closest(".mainSplitter").data("kendoSplitter")) {
        if (window.parent.$("#TabTable_Content_" + page_guid).closest(".mainSplitter").find(".k-pane:first").hasClass("k-state-collapsed")) {
            window.parent.$("#TabTable_Content_" + page_guid).closest(".mainSplitter").data("kendoSplitter").expand(".k-pane:first");
        } else {
            window.parent.$("#TabTable_Content_" + page_guid).closest(".mainSplitter").data("kendoSplitter").collapse(".k-pane:first");
        }
    }
}


function closeActiveTab() {
    var tabstrip = $("#tabstrip");
    if (tabstrip.length > 0) {
        if (tabstrip.find("li.k-state-active").length > 0) {
            tabstrip.data("kendoTabStrip").remove(tabstrip.find("li.k-state-active"));
            tabstrip.data("kendoTabStrip").activateTab(tabstrip.find("ul li:last-child"));
        }
    }
}

function closeMe(parent_guid) {
    //messageWindow("closeMe", "");
    if (Top().modWindow) {
        Top().modalWindow_Close();
    } else {
        window.parent.closeActiveTab();
    }
}
