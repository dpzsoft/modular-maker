// 加载中
app.loading(function (obj) {
    obj.data.packageName = "${package.name}";
    obj.data.tableName = "${table.Name}";
    obj.data.tableFolder = "/" + obj.data.packageName + "/${table.Name}";
    obj.data.workFolder = obj.data.tableFolder + "/edit";
    // 定义默认图像
    obj.data.img = {};
    // 定义表单
    obj.data.form = {}; 
    // 填充表单
    if (!dpz2.isNull(obj.data.Args.data)) {
        for (var k in obj.data.Args.data) {
            obj.data.form[k] = obj.data.Args.data[k];
        }
    }
});

// 加载完毕
app.ready(function (obj) {
    // Vue绑定事件
    obj.methods = {
        // 修改保存
        onEditSave: function (e) {
            var args = obj.data.form;
            args.ID = obj.data.Args.ID;
            // 清理默认图像填充
            for (var k in obj.data.img) {
                if (args[k] === obj.data.img[k]) args[k] = "";
            }
            // 提交API
            $$jttp.postApi("/" + obj.data.packageName + "/Api/${table.Name}/EditSave", args, function () {
                docker.delayed.show("操作成功", 2000);
                docker.dialog.close();
                // 执行回调函数
                if (dpz2.isFunction(obj.data.Args.callback)) obj.data.Args.callback(args);
            }, function (jttp) {
                alert(jttp.Message);
            });
        },
        // 取消按钮
        onCancel: function (e) {
            docker.dialog.close();
        },
        // 可选框勾选
        onChecked: function (e, name) {
            //console.log(name);
            if (obj.data.form[name] === "1") {
                obj.data.form[name] = "0";
            } else {
                obj.data.form[name] = "1";
            }
        },
        // 上传文件
        onUpload: function (e, name) {
            //console.log(name);
            dpz2.upload(ycc.entityConfig.UrlEntrance + "/Api/Files/Upload?sid=" + ycc.entityConfig.SessionID, function (e) {
                var jttp = dpz2.data.json.parse(e);
                var status = parseInt(jttp.Header.Status);
                if (status > 0) {
                    obj.data.form[name] = ycc.entityConfig.UrlEntrance + jttp.Data.Path;
                } else {
                    if (jttp.Message !== "") alert(jttp.Message);
                }
            });
        },
        // 上传文件清理
        onUploadClear: function (name) {
            obj.data.form[name] = obj.data.img[name];
        },
        // 选择日期
        onDayPicked: function (e, name) {
            docker.datePicker.showDay(null, null, obj.data.form[name], function (e) {
                obj.data.form[name] = e;
            });
        },
        // 调用选择框
        onPicked: function (e, name, title) {
            var args = {};
            // 回调函数
            args["callback"] = function (ob) {
                console.log(ob);
            };
            docker.dialog2.show(obj.data.tableFolder + "/selector/config.json", {
                hasHeader: true,
                toTop: true,
                width: 750,
                height: 500,
                title: "选择 " + title,
                args: args
            });
        },
    };
});