// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function Site_Border() {
    var but = $("#button");
    var button = but.data("kendoButton");
    let sss = 1;
    button.bind("click", function (e) {
        alert(e.event.target.tagName);
    });

    $("#primaryTextButton").kendoButton({
        click: function (e) {
            alert(e.event.target.tagName);
        }
    });
}
