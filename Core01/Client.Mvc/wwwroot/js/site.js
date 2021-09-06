// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function Site_Border() {

    var sel = $("#body_id");
    if (sel != undefined)
        sel.css("border", "3px solid blue");

    Ext.onReady(function () {
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
    });

}

