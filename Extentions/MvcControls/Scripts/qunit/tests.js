/**
 * Created by lazareveugene on 27.05.16.
 */
QUnit.test('isMobile()', function (assert) {
    assert.ok($(window.top).width() > 0, 'Ширина нормальная: ' + $(window.top).width());
    if ($(window.top).width() < 769) {
        assert.equal(isMobile(), true, 'Определено как мобильная версия');
    } else {
        assert.equal(isMobile(), false, 'Определено как десктопная версия');
    }
});

QUnit.test('setTitle()', function (assert) {
    $("<div id='title'></div>").appendTo($("body"));
    $("<div id='tabstrip'></div>").data("kendoTabStrip", {
        select: function () {
            return {
                text: function () {
                    return "test"
                }
            }
        }
    }).appendTo($("body"));

    setTitle();

    assert.equal($("#title").text(),"test", "Заголовок установлен");
});