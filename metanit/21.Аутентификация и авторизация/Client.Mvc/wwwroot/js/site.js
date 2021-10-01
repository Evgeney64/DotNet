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

function Site_Accordion1() {
    var ss = 1;
    Site_Border();
}

function JwtToken() {
    var tokenKey = "accessToken";

    // отпавка запроса к контроллеру AccountController для получения токена
    async function getTokenAsync() {

        // получаем данные формы и фомируем объект для отправки
        const formData = new FormData();
        formData.append("grant_type", "password");
        formData.append("username", document.getElementById("emailLogin").value);
        formData.append("password", document.getElementById("passwordLogin").value);

        // отправляет запрос и получаем ответ
        const response = await fetch("/token", {
            method: "POST",
            headers: { "Accept": "application/json" },
            body: formData
        });
        // получаем данные 
        const data = await response.json();

        // если запрос прошел нормально
        if (response.ok === true) {

            // изменяем содержимое и видимость блоков на странице
            document.getElementById("userName").innerText = data.username;
            document.getElementById("userInfo").style.display = "block";
            document.getElementById("loginForm").style.display = "none";
            // сохраняем в хранилище sessionStorage токен доступа
            sessionStorage.setItem(tokenKey, data.access_token);
            console.log(data.access_token);
        }
        else {
            // если произошла ошибка, из errorText получаем текст ошибки
            console.log("Error: ", response.status, data.errorText);
        }
    };
    // отправка запроса к контроллеру ValuesController
    async function getData(url) {
        const token = sessionStorage.getItem(tokenKey);

        const response = await fetch(url, {
            method: "GET",
            headers: {
                "Accept": "application/json",
                "Authorization": "Bearer " + token  // передача токена в заголовке
            }
        });
        if (response.ok === true) {

            const data = await response.json();
            alert(data)
        }
        else
            console.log("Status: ", response.status);
    };

    // получаем токен
    document.getElementById("submitLogin").addEventListener("click", e => {

        e.preventDefault();
        getTokenAsync();
    });

    // условный выход - просто удаляем токен и меняем видимость блоков
    document.getElementById("logOut").addEventListener("click", e => {

        e.preventDefault();
        document.getElementById("userName").innerText = "";
        document.getElementById("userInfo").style.display = "none";
        document.getElementById("loginForm").style.display = "block";
        sessionStorage.removeItem(tokenKey);
    });


    // кнопка получения имя пользователя  - /api/values/getlogin
    document.getElementById("getDataByLogin").addEventListener("click", e => {

        e.preventDefault();
        getData("/api/values/getlogin");
    });

    // кнопка получения роли  - /api/values/getrole
    document.getElementById("getDataByRole").addEventListener("click", e => {

        e.preventDefault();
        getData("/api/values/getrole");
    });
}