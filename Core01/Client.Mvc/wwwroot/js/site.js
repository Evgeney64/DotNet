// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function Site_Border() {

    var sel = $("#body_id");
    if (sel != undefined)
        sel.css("border", "3px solid blue");

    Ext.create('Ext.Panel', {
        width: 500,
        height: 360,
        padding: 10,
        layout: 'border',
        items: [{
            xtype: 'panel',
            title: 'Центральная панель',
            html: 'Центральная панель',
            region: 'center',
            margin: '5 5 5 5'
        }, {
            xtype: 'panel',
            title: 'Верхняя панель',
            html: 'Верхняя панель',
            region: 'north',
            height: 80
        }, {
            xtype: 'panel',
            title: 'Нижняя панель',
            html: 'Нижняя панель',
            region: 'south',
            height: 80
        }, {
            xtype: 'panel',
            title: 'Левая панель',
            html: 'Левая панель',
            region: 'west',
            width: 100
        }, {
            xtype: 'panel',
            title: 'Правая панель',
            html: 'Правая панель',
            region: 'east',
            width: 120
        }],
        renderTo: Ext.getBody()
    });

}

function Site_Border1() {
    Ext.create('Ext.Panel', {
        width: 500,
        height: 360,
        padding: 10,
        layout: 'border',
        items: [{
            xtype: 'panel',
            title: 'Центральная панель',
            html: 'Центральная панель',
            region: 'center',
            margin: '5 5 5 5'
        }, {
            xtype: 'panel',
            title: 'Верхняя панель',
            html: 'Верхняя панель',
            region: 'north',
            height: 80
        } ],
        renderTo: Ext.getBody()
    });

}

function Site_Accordion(data) {

    var data_str = data.replaceAll('&quot;', '"');
    //var data_str1 = JSON.stringify(data_str);
    var users = $.parseJSON(data_str);
    var ss1 = 1;

    for (var user in users) {
        //var checkBox = "<input type='checkbox' data-price='" + key.Price + "' name='" + key.Name + "' value='" + key.ID + "'/>" + key.Name + "<br/>";
        //$(checkBox).appendTo('#modifiersDiv');
        ss1 = ss1 + 1;
    };
    var ss2 = 1;

    Ext.create('Ext.Panel', {
        title: 'Таблица',
        width: 500,
        height: 200,
        layout: 'accordion',
        items: [
            {
                xtype: 'panel',
                title: 'Л. Толстой',
                html: 'Произведения Л. Толстого: ....'
            },
            {
                xtype: 'panel',
                title: 'Ф. Достоевский',
                html: 'Произведения Ф. Достоевского: ...'
            },
            {
                xtype: 'panel',
                title: 'И. Тургенев',
                html: 'Произведения И. Тургенева: ...'
            }],
        renderTo: Ext.getBody()
    });

}
