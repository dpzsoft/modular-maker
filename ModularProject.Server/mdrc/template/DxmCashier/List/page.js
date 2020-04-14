// 加载中
app.loading(function (obj) {
    obj.data.title = "${table.Title}管理";
    obj.data.list = [];
    obj.data.packageName = "${package.name}";
    obj.data.tableName = "${table.Name}";
    obj.data.tableFolder = "/" + obj.data.packageName + "/${table.Name}";
    obj.data.workFolder = obj.data.tableFolder + "/list";
    // 按钮状态
    obj.data.buttons = {
        add: true,
        refresh: true
    }
    // 数据统计专用
    obj.data.statistics = {
        AutoIndex: "小计",
    @table.Fields{
        ${field.Name}: "",};
        RowOperate:""
    };
    // 数据行信息专用
    obj.data.rows = {
        pageSize: 10,
        page: 1,
        pageCount: 0,
        rowCount: 0
    };
    // 排序专用
    obj.data.order = {
        field: "",
        sort: ""
    };
    // 搜索专用
    obj.data.search = {
        input: "",
        key: "",
        start: "不限",
        end: "不限",
        hasDate: true,
        enable: true,
        worked: false
    };
    // 筛选专用
    obj.data.filter = {};
    // 筛选器
    obj.data.filters = [];
    // 访问源
    obj.data.referrer = {
        url: "",
        title: "",
        args: {}
    };
    // 设置访问源
    if (!dpz2.isNull(obj.data.Args)) {
        if (!dpz2.isNull(obj.data.Args.referrer)) {
            obj.data.referrer.url = obj.data.Args.referrer.url;
            obj.data.referrer.title = obj.data.Args.referrer.title;
            obj.data.referrer.args = obj.data.Args.referrer.args;
        };
    }
});

// 加载完毕
app.ready(function (obj) {
    // 建立页面主对象
    var pg = {};
    //加载列表
    pg.loadList = function (page) {
        docker.wait.show("正在加载数据列表");
        if (dpz2.isNull(page)) page = obj.data.page;
        var args = {
            pageSize: obj.data.rows.pageSize,
            page: page,
            order: obj.data.order.field,
            orderSort: obj.data.order.sort,
            key: obj.data.search.key
        };
        // 添加分页信息
        args["opt-page"] = page;
        args["opt-page-size"] = obj.data.rows.pageSize;
        // 添加搜索信息
        args["opt-search-key"] = obj.data.search.key;
        // 添加日期区间
        if (obj.data.search.hasDate) {
            args["opt-search-start"] = obj.data.search.start;
            args["opt-search-end"] = obj.data.search.end;
        }
        // 添加排序信息
        args["opt-order-field"] = obj.data.order.field;
        args["opt-order-sort"] = obj.data.order.sort;
        // 添加所有的筛选操作
        for (var k in obj.data.filter) {
            args["opt-filter-" + k] = obj.data.filter[k];
        }
        // 添加所有的参数信息
        for (var k in obj.data.Args) {
            args["data-" + k] = obj.data.Args[k];
        }
        // 提交API
        $$jttp.postApi("/" + obj.data.packageName + "/Api/${table.Name}/GetList", args, function (jttp) {
            obj.data.rows.pageSize = jttp.Data.PageSize;
            obj.data.rows.page = jttp.Data.Page;
            obj.data.rows.pageCount = jttp.Data.PageCount;
            obj.data.rows.rowCount = jttp.Data.RowCount;
            obj.data.list = jttp.List;
            for (var k in jttp.Data) {
                obj.data.statistics[k] = jttp.Data[k];
            }
            docker.wait.hide();
        }, function () { docker.wait.hide(); });

    };
    // Vue绑定事件
    obj.methods = {
        // 添加
        onAdd: function (e) {
            var args = {};
            args.data = {};
            // 添加所有的参数信息
            for (var k in obj.data.Args) {
                args.data[k] = obj.data.Args[k];
            }
            // 添加回调函数
            args.callback = function (ob) {
                obj.methods.onRefresh();
            };
            docker.dialog.show(obj.data.tableFolder + "/add/config.json", {
                hasHeader: true,
                toTop: true,
                width: 750,
                height: 500,
                title: "添加${table.Title}",
                args: args
            });
        },
        // 修改
        onEdit: function (e, ob) {
            var args = {};
            args.data = {};
            // 赋值数据对象
            for (var k in ob) {
                args.data[k] = "" + ob[k];
            }
            // 添加回调函数
            args.callback = function (ob) {
                obj.methods.onRefresh();
            };
            docker.dialog.show(obj.data.tableFolder + "/edit/config.json", {
                hasHeader: true,
                toTop: true,
                width: 750,
                height: 500,
                title: "修改${table.Title} - [" + ob.Name + "]",
                args: args
            });
        },
        // 查看详情
        onView: function (e, ob) {
            var args = {};
            args.data = {};
            // 赋值数据对象
            for (var k in ob) {
                args.data[k] = "" + ob[k];
            }
            docker.dialog.show(obj.data.tableFolder + "/view/config.json", {
                hasHeader: true,
                toTop: true,
                width: 750,
                height: 500,
                title: "查看详情 - [" + ob.Name + "]",
                args: args
            });
        },
        // 返回操作
        onBack: function (e) {
            var args = obj.data.referrer.args;
            docker.body.show(obj.data.referrer.url, args);
        },
        // 刷新列表
        onRefresh: function (e) {
            pg.loadList(obj.data.rows.page);
        },
        // 删除
        onDelete: function (e, ob) {
            if (confirm("确定要删除项目 \"" + ob.Name + "\" 吗?")) {
                $$jttp.postApi("/" + obj.data.packageName + "/Api/${table.Name}/DeleteSave", { ID: ob.ID }, function (jttp) {
                    docker.delayed.show("操作成功", 2000);
                    pg.loadList(obj.data.rows.page);
                }, function (jttp) {
                    alert(jttp.Message);
                });
            }
        },
        // 排序
        onOrder: function (e, field) {
            if (obj.data.order.field === field) {
                switch (obj.data.order.sort) {
                    case "asc":
                        obj.data.order.sort = "desc";
                        break;
                    case "desc":
                        obj.data.order.field = "";
                        obj.data.order.sort = "";
                        break;
                    default:
                        obj.data.order.sort = "asc";
                        break;
                }
            } else {
                obj.data.order.field = field;
                obj.data.order.sort = "asc";
            }
            pg.loadList(obj.data.rows.page);
        },
        // 设置勾选状态
        onCheckChangeSave: function (e, field, ob) {
            $$jttp.postApi("/" + obj.data.packageName + "/Api/${table.Name}/CheckChangeSave", { ID: ob.ID, field: field }, function (jttp) {
                ob[field] = "" + jttp.Data[field];
            }, function (jttp) {
                alert(jttp.Message);
            });
        },
        // 列表单元编辑事件
        onCellEdit: function (e, field, ob) {
            $$jttp.postApi("/" + obj.data.packageName + "/Api/${table.Name}/FieldEditSave", { ID: ob.ID, field: field, value: ob[field] }, function (jttp) {
                pg.loadList(obj.data.rows.page);
            }, function (jttp) {
                alert(jttp.Message);
            });
        },
        // 跳转页面
        onChangePage: function (page) {
            pg.loadList(page);
        },
        // 数据搜索
        onSearch: function () {
            obj.data.search.key = obj.data.search.input;
            obj.data.search.input = "";
            obj.data.search.worked = true;
            pg.loadList(1);
        },
        // 数据搜索重置
        onSearchReset: function () {
            obj.data.search.key = "";
            obj.data.search.input = "";
            obj.data.search.start = "不限";
            obj.data.search.end = "不限";
        },
        // 清除搜素
        onSearchClear: function () {
            obj.data.search.key = "";
            obj.data.search.input = "";
            obj.data.search.start = "不限";
            obj.data.search.end = "不限";
            obj.data.search.worked = false;
            pg.loadList(1);
        },
        // 选择开始日期
        onSelectDateStart: function () {
            docker.datePicker.showDay(null, null, obj.data.search.start, function (e) {
                obj.data.search.start = e;
            });
        },
        // 选择开始日期
        onSelectDateEnd: function () {
            docker.datePicker.showDay(null, null, obj.data.search.end, function (e) {
                obj.data.search.end = e;
            });
        },
        // 筛选
        onFilter: function (e, field, value) {
            obj.data.filter[field] = value;
            pg.loadList(1);
        },
    };

    //pg.loadList();
    setTimeout(function () { pg.loadList(1); }, 10);

});
