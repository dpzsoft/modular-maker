app.loading(function (obj) {

    obj.data.title = "${table.Title}管理";
    obj.data.list = [];
    obj.data.packageName = "${package.name}";
    obj.data.tableName = "${table.Name}";
    obj.data.tableFolder = "/" + obj.data.packageName + "/${table.Name}";
    obj.data.workFolder = obj.data.tableFolder + "/selector";

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
        key: ""
    };

});

app.ready(function (obj) {

    var pg = {};

    //加载列表
    pg.loadList = function (page) {
        //docker.wait.show("正在加载应用列表");

        if (dpz2.isNull(pg)) pg = obj.data.page;

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

        // 添加排序信息
        args["opt-order-field"] = obj.data.order.field;
        args["opt-order-sort"] = obj.data.order.sort;

        // 添加所有的筛选操作
        for (var k in obj.data.filter) {
            args["opt-filter-" + k] = obj.data.filter[k];
        }

        // 添加所有的参数信息
        for (var k in obj.data.Args) {
            if (typeof obj.data.Args[k] === "string" || typeof obj.data.Args[k] === "number" || typeof obj.data.Args[k] === "boolean")
                args["data-" + k] = obj.data.Args[k];
        }

        $$jttp.postApi("/" + obj.data.packageName + "/Api/${table.Name}/GetList", args, function (e) {
            obj.data.rows.pageSize = e.Data.PageSize;
            obj.data.rows.page = e.Data.Page;
            obj.data.rows.pageCount = e.Data.PageCount;
            obj.data.rows.rowCount = e.Data.RowCount;
            obj.data.list = e.List;
        });
    };

    obj.methods = {

        // 排序
        onOrder: function (field) {
            //docker.body.show(ycc.entityConfig.UrlEntrance, "/res/Authorization/Add/config.json");
            //console.log(field);
            if (obj.data.orderField === field) {
                switch (obj.data.orderType) {
                    case "asc":
                        obj.data.orderType = "desc";
                        break;
                    case "desc":
                        obj.data.orderField = "";
                        obj.data.orderType = "";
                        break;
                    default:
                        obj.data.orderType = "asc";
                        break;
                }
            } else {
                obj.data.orderField = field;
                obj.data.orderType = "asc";
            }
            pg.loadList(obj.data.rows.page);
        },

        // 排序
        onChangePage: function (page) {
            pg.loadList(page);
        },

        // 数据搜索
        onSearch: function () {
            obj.data.search.key = obj.data.search.input;
            pg.loadList(1);
        },

        // 清除搜素
        onSearchClear: function () {
            obj.data.search.key = "";
            pg.loadList(1);
        },

        // 选择事件
        onSelect: function (e, ob) {
            if (dpz2.isFunction(obj.data.Args.callback)) obj.data.Args.callback(ob);
            docker.dialog2.close();
        }
    };

    //pg.loadList();
    setTimeout(function () {
        pg.loadList(1);
    }, 10);

});