if (typeof $.connection !== 'undefined') {
    if ($.connection.callEventsHub !== undefined) {
        $.connection.callEventsHub.client.onCall = function (data) {
            console.info("onCall");
            if ("Notification" in window) {
                var notification = new Notification("Телефонный звонок", {
                    //tag: "telephony",
                    icon: "./Images/Dashbord/tsb.png",
                    body: data,
                    actions: [{
                        action: "pickup", title: "Принять"
                    }, {
                        action: "hangup", title: "Отклонить"
                    }]
                });
            }

            /*В соответствующие пункты меню добавляем значок новых элементов*/
            var li = $("li[need_new_items_pic='True']");
            if (li.length > 0) {
                var a = $("li[need_new_items_pic='True'] > a");
                if (a.length > 0) {
                    var newImg1 = document.createElement('img');
                    newImg1.setAttribute('class', 'k-image image-new-items is-align-left');
                    newImg1.setAttribute('alt', 'image');
                    newImg1.setAttribute('src', li.attr("new_items_pic_src"));
                    a.append(newImg1);
                }

                var span = $("li[need_new_items_pic='True'] > span");
                if (span.length > 0) {
                    var newImg2 = document.createElement('img');
                    newImg2.setAttribute('class', 'k-image image-new-items is-align-left');
                    newImg2.setAttribute('alt', 'image');
                    newImg2.setAttribute('src', li.attr("new_items_pic_src"));
                    span.append(newImg2);
                }
                $("li[new_items_pic_type='Calls']").attr('need_new_items_pic', 'False');
            }
            //setTimeout(notification.close.bind(notification), 60000);
        };
        $.connection.callEventsHub.client.proxy = function (data) {
        };
    }

    if ($.connection.taskHub !== undefined) {
        var task = $.connection.taskHub;
        task.client.executionStatusChanged = function (data) {
            if ("Notification" in window) {
                var notification = new Notification("Пакетные операции", {
                    tag: "taskExecution" + data.ExecutionId,
                    icon: "./Images/Dashbord/tsb.png",
                    body: data.Info
                });
            }
        };
    }

    if ($.connection.eventHub !== undefined) {
        var event = $.connection.eventHub;
        event.client.notify = function (data) {
            if ("Notification" in window) {
                var notification = new Notification("Внутреннее уведомление", {
                    tag: "eventId" + getRandomValue(),
                    //tag: "eventId" + data.EventId,
                    icon: "./Images/Dashbord/tsb.png",
                    body: data.Info
                });
                self.registration.showNotification("Notification", notification);
            }
        };
    }

    $.connection.hub.reconnected(function () {
        console.info("reconnected");
        $.connection.callEventsHub.server.register();
    });
    console.info("start");
    //For Debug use {transport: 'longPolling', waitForPageLoad: false}
    $.connection.hub.start({ waitForPageLoad: false }, function () {
        console.info("started");
        $.connection.callEventsHub.server.register();
    });
}

if ("Notification" in window) {
    if (Notification.permission === "granted") {
    }
    else if (Notification.permission === "denied") {
    }
    else {
        Notification.requestPermission(function (e) {
            //var notification = new Notification("Телефонный звонок", {
            //    tag: "telephony",
            //    icon: "./Images/Dashbord/tsb.png",
            //    body: "Так мы будем показывать входящие и пропущенные звонки.",
            //    actions: [{
            //        action: "pickup", title: "Принять"
            //    }, {
            //        action: "hangup", title: "Отклонить"
            //    }]
            //});
            //var notification = new Notification("Пакетные операции", {
            //    tag: "taskExecution",
            //    icon: "./Images/Dashbord/tsb.png",
            //    body: "А вот так информацию о статусе выполнения пакетных операций."
            //});
            //setTimeout(notification.close.bind(notification), 60000);
        });
    }
}
// поддерживается ли технология в браузере
if ('serviceWorker' in navigator) {
    // регистрируем фоновую службу
    navigator.serviceWorker.register('serviceworker.js')
        .then(function (registration) {
            if (!registration) {
                return Promise.reject("Не удалось настроить фоновую службу");
            }
            return registration;
        })
        // ждем когда завершится установка
        .then(function () { return navigator.serviceWorker.ready; })
        .then(function (registration) {
            // поддерживает ли браузер получение пушей
            if (!('PushManager' in window)) {
                return Promise.reject("Пуш сообщения не поддерживаются в браузере");
            }
            // подписываемся на получение пушей
            return registration.pushManager.getSubscription().then(function (subscription) {
                // если уже есть подписка то возвращаем ее
                if (subscription) {
                    return subscription;
                }

                // запрашиваем открытый ключ сервера
                return fetch("Push/getPushPublicKey")
                    .then(function (raw) { return raw.text(); })
                    .then(function (vapidPublicKey) {
                        if (!vapidPublicKey) {
                            return Promise.reject();
                        }

                        const convertedVapidKey = urlBase64ToUint8Array(vapidPublicKey);

                        // делаем запрос на подписку
                        return registration.pushManager.subscribe({
                            // пока поддерживается получение только пушей с уведомлениями
                            userVisibleOnly: true,
                            applicationServerKey: convertedVapidKey
                        }).catch(function (error) {
                            // если еще не разрешен показ уведомлений
                            if (error.name === "NotAllowedError") {
                                // делаем запрос на показ уведомлений
                                Notification.requestPermission().then(function (permission) {
                                    if (permission === "granted") {
                                        // если разрешили то перезагружаемся
                                        window.reload();
                                    }
                                });
                            }
                            return Promise.reject();
                        });
                    });
            });
        })
        // передаем полученный токен на сервер
        .then(function (subscription) {
            if (!subscription) {
                return Promise.reject();
            }

            return fetch("Push/SetPushToken", {
                method: "POST",
                headers: {
                    "Content-type": "application/x-www-form-urlencoded"
                },
                body: NVP({
                    type: "WebPush",
                    user: "seeAuth",
                    token: JSON.stringify(subscription)
                })
            });
        });
}

function NVP(obj) {
    return Object.keys(obj).map(function (key) {
        return [
            encodeURIComponent(key),
            encodeURIComponent(obj[key]),
        ].join("=");
    }).join("&");
}

function urlBase64ToUint8Array(base64String) {
    let padding = '='.repeat((4 - base64String.length % 4) % 4);
    let base64 = (base64String + padding)
        .replace(/\-/g, '+')
        .replace(/_/g, '/');

    let rawData = window.atob(base64);
    let outputArray = new Uint8Array(rawData.length);

    for (let index = 0; index < rawData.length; ++index) {
        outputArray[index] = rawData.charCodeAt(index);
    }

    return outputArray;
}