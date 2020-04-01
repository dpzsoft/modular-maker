$(function () {
    // 判断宿主是否存在
    if (typeof host === "object") {
        window.alert = function (text) {
            host.ShowMessage(text);
        }
    }
});