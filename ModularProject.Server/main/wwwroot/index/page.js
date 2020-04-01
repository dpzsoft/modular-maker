var pg = {};

// 跳转对象缓存
pg.navId = '';

// 跳转对象
pg.navTo = function (id, url) {
    if (pg.navId !== id) {
        pg.navId = id;
        $("#frm_main").attr({ src: url });
        $("dd").css({ backgroundColor: "", color: "" });
        $("#dd_" + id).css({ backgroundColor: "#fff", color: "#333" });
    }
}

// 打开分类
pg.open = function (id) {
    $("#dl_" + id + "_close").css({ display: "none" });
    $("#dl_" + id + "_open").css({ display: "block" });
}

// 关闭分类
pg.close = function (id) {
    $("#dl_" + id + "_open").css({ display: "none" });
    $("#dl_" + id + "_close").css({ display: "block" });
}

$(function () {

    let userCacheName = "Cache-User";
    var user = "";
    if (localStorage.getItem(userCacheName) !== null) user = localStorage.getItem(userCacheName);
    $("txtUser").val(user);

    // 预加载所有图像
    var loader = dpz2.createLoader();
    loader.addImage("/modular-project-manage/index/image/left.png");
    loader.addImage("/modular-project-manage/index/image/right.png");
    loader.addImage("/modular-project-manage/index/image/exit.png");
    loader.load();

    // 收起菜单效果处理
    $("#lnk_menu_hide").mouseover(function () {
        $("#img_menu_hide2").css({ display: "none" });
        $("#img_menu_hide").css({ display: "block" });
        //$("#img_menu_hide").attr({ src: "/modular-project-manage/index/image/left.png" });
    }).mouseout(function () {
        $("#img_menu_hide").css({ display: "none" });
        $("#img_menu_hide2").css({ display: "block" });
        //$("#img_menu_hide").attr({ src: "/modular-project-manage/index/image/left_b.png" });
    }).click(function () {
        $("#switch_hide").css({ display: "none" });
        $(".right").css({ width: "100%" });
        $(".left").animate({ width: "0px" }, 200, function () {
            $("#switch_show").css({ display: "block" });
        });
    });

    // 显示菜单效果处理
    $("#lnk_menu_show").mouseover(function () {
        $("#img_menu_show2").css({ display: "none" });
        $("#img_menu_show").css({ display: "block" });
    }).mouseout(function () {
        $("#img_menu_show").css({ display: "none" });
        $("#img_menu_show2").css({ display: "block" });
    }).click(function () {
        $("#switch_show").css({ display: "none" });
        $(".left").animate({ width: "220px" }, 200, function () {
            $("#switch_hide").css({ display: "block" });
            $(".right").css({ width: "calc(100% - 220px)" });
        });
    });

    // 退出效果处理
    $("#lnk_exit").mouseover(function () {
        $("#img_exit2").css({ display: "none" });
        $("#img_exit").css({ display: "block" });
        //$("#img_exit").attr({ src: "/modular-project-manage/index/image/exit.png" });
    }).mouseout(function () {
        $("#img_exit").css({ display: "none" });
        $("#img_exit2").css({ display: "block" });
        //$("#img_exit").attr({ src: "/modular-project-manage/index/image/exit_b.png" });
    }).click(function () {
        // 提交数据
        $jttp.postApi("/modular-project-manage/Api/UserInfo/Logout", {}, function (e) {
            location.href = "/dxm-cashier-manager-login/index";
        }, function (e) {
            alert(e.Message);
        });
    });

    // 列表项效果
    $("dd").mouseover(function () {
        var id = this.id;
        if (id !== "dd_" + pg.navId) {
            $(this).css({ color: "#ff6d00" });
        }
    }).mouseout(function () {
        var id = this.id;
        if (id !== "dd_" + pg.navId) {
            $(this).css({ color: "" });
        }
    })

});