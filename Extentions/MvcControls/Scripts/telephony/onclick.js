/*
$(document).ready(function () {
    $("body").on("click", "a[href^='tel:']", function (e) {
        e.preventDefault();
        $.get("../api/Telephony/Call", { to: $(e.target).attr("href").substring(4) });
        return false;
    });
});
*/