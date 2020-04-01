var pg = {};

pg.postApi = function (api, group, item, name) {
    $(".info").css({ display: "block" });
    $jttp.postApi("/modular-project-manage/api/Builder/" + api, { Group: group, Item: item, Name: name }, function (e) {
        alert(e.Message);
        $(".info").css({ display: "none" });
    }, function (e) {
        alert(e.Message);
        $(".info").css({ display: "none" });
    });
}

pg.buildController = function (group, item, name) {
    pg.postApi("BuildController", group, item, name);
}

pg.buildAddForm = function (group, item, name) {
    pg.postApi("BuildAddForm", group, item, name);
}

pg.buildEditForm = function (group, item, name) {
    pg.postApi("BuildEditForm", group, item, name);
}

pg.buildViewForm = function (group, item, name) {
    pg.postApi("BuildViewForm", group, item, name);
}

pg.buildList = function (group, item, name) {
    pg.postApi("BuildList", group, item, name);
}

pg.buildSelector = function (group, item, name) {
    pg.postApi("BuildSelector", group, item, name);
}

$(function () {

});