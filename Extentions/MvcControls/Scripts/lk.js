/**
 * Created by azureuser on 28.09.2016.
 */

$(document).ready(function () {

    $("#main-content").kendoSortable({
        filter: ">div",
        cursor: "move",
        placeholder: placeholder,
        hint: hint,
        holdToDrag: true
    });
    if ($(window).width() < 600) {
        $("#sidebar")
            .kendoResponsivePanel({
                breakpoint: 601,
                orientation: "left"
            });
    }

    //collapse
    $(".panel-wrap").on("click", "span.k-i-minimize", function (e) {
        var contentElement = $(e.target).closest(".widget");
        contentElement.removeClass("expanded");
        var complete = function () {
            // Animation complete.
            $(e.target)
                .removeClass("k-i-minimize")
                .addClass("k-i-maximize");
            $.each($(".widget"), function (index, item) {
                $(item).show();
            });
            $(document).scrollTop(contentElement.position().top);
            $("body, html").scrollTop(contentElement.position().top);
            $("body").removeClass("is-not-scrolled");
            $(window).resize();
        };
        if ($(window).width() > 600) {
            var width = "300px";
            if (contentElement.hasClass("wide")) {
                width = "642px";
            }
            contentElement.animate({
                width: width,
                height: "300px"
            }, 1000, complete);
        } else {
            contentElement.animate({
                height: "300px"
            }, 1000, complete);
        }
    });

    //expand
    $(".panel-wrap").on("click", "span.k-i-maximize", function (e) {
        var contentElement = $(e.target).closest(".widget");
        $("body").addClass("is-not-scrolled");
        contentElement.addClass("expanded");
        $.each($(".widget"), function (index, item) {
            if (!$(item).hasClass("expanded")) {
                $(item).hide();
            }
        });
        var complete = function () {
            // Animation complete.
            $(e.target)
                .removeClass("k-i-maximize")
                .addClass("k-i-minimize");
            $(document).scrollTop(contentElement.position().top);
            $("body, html").scrollTop(contentElement.position().top);
            $(window).resize();
        };
        if ($(window).width() > 600) {
            contentElement.animate({
                width: $(document).width() - 100,
                height: $(document).height() - 49
            }, 1000, complete);
        } else {
            contentElement.animate({
                height: screen.height - 6
            }, 1000, complete);
        }
    });

    //in tab
    $(".panel-wrap").on("click", "span.k-i-restore", function (e) {
        var contentElement = $(e.target).closest(".widget");
        var url = "/Home/" + contentElement.find("iframe").attr("src");
        var url_params = url.split("&");
        var guids_out = null;
        for (var i = 0; i < url_params.length; i++) {
            var url_param = url_params[i].split("=");
            if (url_param.length > 0) {
                if (url_param[0] == "guids_out")
                    guids_out = url_params[i].substr(10, url_params[i].length - 10);
            }
        }
        url = url.replace("TabTable", "Main_Layout");
        url = addToUrl(url, "from_dashbord_to_maintable", true);

        Top().mainTab_Open(/*Top().$("#tabstrip"), undefined, */contentElement.find("h3").text(), url/*, null, guids_out*/);
    });

});

function placeholder(element) {
    return element.clone().addClass("placeholder");
}

function hint(element) {
    return element.clone().addClass("hint")
        .height(element.height())
        .width(element.width());
}