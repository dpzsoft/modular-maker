var pg = {};

pg.buildMainPage = function (group, item, path) {
    $(".info").css({ display: "block" });
    $jttp.postApi("/modular-project-manage/api/Builder/BuildMainPage", { Group: group, Item: item, Path: path }, function (e) {
        alert(e.Message);
        location.reload(true);
    }, function (e) {
        alert(e.Message);
        $(".info").css({ display: "none" });
    });
}

$(function () {

});