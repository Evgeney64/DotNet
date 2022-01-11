if (!!window.name && window.name.indexOf("|") > 0) {
    var a = document.createElement('script');
    var m = document.getElementsByTagName('script')[0];
    a.async = 1;
    a.onload = function () {
        if (typeof BX24 !== 'undefined' && BX24 !== null) {
            BX24.init(function () {
                if (BX24.placement.info().options.url &&
                    window.location.href.indexOf(BX24.placement.info().options.url) === -1) {
                    window.location.href = BX24.placement.info().options.url;
                }
                document.documentElement.style.height = "800px";
                BX24.fitWindow();
                function setup() {
                    $.connection.evalHub.client.eval = function (command) {
                        console.info("eval");
                        try {
                            eval(command);
                        } catch (e) {
                            console.error(e);
                        }
                    };
                    console.info("start");
                    function rerun() {
                        var hub = {};
                        if (hub = $.connection.hub.stop()) {
                            //For Debug use {transport: 'longPolling', waitForPageLoad: false}
                            hub.start({}, function () {
                                console.info("started");
                            });
                        } else {
                            setTimeout(rerun, 1000);
                        }
                    }
                    rerun();

                }
                if (typeof $.connection !== 'undefined') {
                    setup();
                } else {
                    setTimeout(setup, 1000);
                }
            });

        }
    };
    a.src = window.location.protocol + "//api.bitrix24.com/api/v1/";
    m.parentNode.insertBefore(a, m);
}