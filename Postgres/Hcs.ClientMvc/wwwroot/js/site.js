function IndexScript() {
    Ext.onReady(function () {
        var panel = Ext.create('Ext.Panel', {
            title: 'ГИС ЖКХ',
            width: 500,
            height: 150,
            padding: 10,
            bodyPadding: 5,
            items: [
                {
                    xtype: 'textfield',
                    fieldLabel: 'Параметр',
                    id: 'txtParam',
                    height: 20,
                    fieldStyle: 'background-color: #fff; font-weight: bold; width:350px;'
                }, {
                    xtype: 'textfield',
                    fieldLabel: 'Результат',
                    id: 'txtResult',
                    height: 20,
                    fieldStyle: 'background-color: #ddd; font-weight: bold; width:350px;'
                }, {
                    xtype: 'button',
                    text: 'Выполнить',
                    id: 'passData',
                    height: 30,
                    width: 450,
                    margin: '5 0 0 5',
                    //labelStyle: 'font-weight: bold',
                    style: {
                        'color': 'red',
                        'font-size': '15px',
                        'font-weight': 'bold'
                    },
                    handler: function () {
                        panel.getComponent('txtParam').setValue("executing...");
                        //var execute = $(panel.getComponent('txtResult'));
                        //sss1.style.visibility = "hidden";
                        //execute.css('visibility', 'hidden;');
                        //execute.css('visibility', 'collapse');
                        $(panel.getComponent('passData')).css('visibility', 'hidden');
                        Ext.Ajax.request({
                            url: 'Home/TestGenPostgr',
                            success: function (response, options) {
                                if (response != null) {
                                    panel.getComponent('txtParam').setValue("");
                                    var value = response.responseText;
                                    HtmlScript(value);
                                    panel.getComponent('txtResult').setValue(value);
                                }
                            },
                            failure: function (response, options) {
                                alert("Ошибка: " + response.statusText);
                            }
                        });
                    }
                }
            ],
            renderTo: Ext.getBody()
        });
    });
}

function TestScript(value) {
    var value_str = value_in.replaceAll('&quot;', '"');
    var value = JSON.parse(value_str);
    var value_html = "<p>" + value.ValueGuid + "</p><p>---</p><p>" + value.ValueStr + "</p>";
    HtmlScript(value_html);
}

function HtmlScript(value) {
    var sss = 1;

    Ext.create('Ext.Panel', {
        width: 1500,
        height: 500,
        padding: 10,
        layout: 'border',
        items: [{
            xtype: 'panel',
            title: 'Результат операции',
            html: value,
            region: 'center',
            margin: '5 5 5 5',
            style: {
                'font-color': 'red',
                'font-size': '15px',
                'font-weight': 'bold'
            },
        }],
        renderTo: Ext.getBody()
    });

}

function IndexScriptAjax() {
    Ext.onReady(function () {

        Ext.Ajax.request({
            url: 'Home/Test',
            success: function (response, options) {
                var objAjax = Ext.decode(response.responseText); // декодируем полученные json-объекты
                // устанавливаем для каждого свойства декодированное значение
                panel.getComponent('txtName').setValue(objAjax.firstname);
                panel.getComponent('txtSurname').setValue(objAjax.lastname);
                panel.getComponent('txtCompany').setValue(objAjax.company);
            },
            failure: function (response, options) {
                alert("Ошибка: " + response.statusText);
            }
        });
    });
}
