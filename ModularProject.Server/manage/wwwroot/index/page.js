var pg = {};

pg.openPages = function (group, item) {
    $(".info").css({ display: "block" });
    $jttp.postApi("/modular-project-manage/api/tools/openpages", { Group: group, Item: item }, function (e) {
        alert("使用VS编辑Pages命令执行成功!");
        $(".info").css({ display: "none" });
    }, function (e) {
        alert(e.Message);
        $(".info").css({ display: "none" });
    });
}

pg.openController = function (group, item) {
    $(".info").css({ display: "block" });
    $jttp.postApi("/modular-project-manage/api/tools/OpenController", { Group: group, Item: item }, function (e) {
        alert("使用VS编辑Controller命令执行成功!");
        $(".info").css({ display: "none" });
    }, function (e) {
        alert(e.Message);
        $(".info").css({ display: "none" });
    });
}

pg.buildController = function (group, item) {
    $(".info").css({ display: "block" });
    $jttp.postApi("/modular-project-manage/api/Maker/BuildController", { Group: group, Item: item }, function (e) {
        alert(e.Message);
        $(".info").css({ display: "none" });
    }, function (e) {
        alert(e.Message);
        $(".info").css({ display: "none" });
    });
}

pg.buildPackage = function (group, item) {
    $(".info").css({ display: "block" });
    $jttp.postApi("/modular-project-manage/api/Maker/BuildPackage", { Group: group, Item: item }, function (e) {
        alert(e.Message);
        location.reload(true);
    }, function (e) {
        alert(e.Message);
        $(".info").css({ display: "none" });
    });
}

pg.buildPages = function (group, item) {
    $(".info").css({ display: "block" });
    $jttp.postApi("/modular-project-manage/api/Maker/BuildPages", { Group: group, Item: item }, function (e) {
        alert(e.Message);
        $(".info").css({ display: "none" });
    }, function (e) {
        alert(e.Message);
        $(".info").css({ display: "none" });
    });
}

$(function () {

});