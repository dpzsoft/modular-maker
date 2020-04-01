/**
 * Jttp专用类
 */
class JttpOperator {

    constructor(el) {
        if (typeof el === "object") {
            this._parent = el;
        } else {
            this._parent = document.body;
        }
        this._handlers = [];
        this._fails = [];
        this._errors = [];
        this._sid = "";
        if (localStorage.getItem("Mdr-Session-Id") !== null) this._sid = localStorage.getItem("Mdr-Session-Id");
        this._skey = "";
        if (localStorage.getItem("Mdr-Security-Key") !== null) this._skey = localStorage.getItem("Mdr-Security-Key");
    }

    /**
     * 获取符合样式名称的第一个元素
     * @param {String} className 样式名称
     * @returns {HTMLElement}
     */
    getClassElement(className) {
        var parent = this._parent;
        var els = parent.getElementsByClassName(className);
        if (els.length <= 0) return null;
        return els[0];
    }

    /**
     * 显示登录界面
     * @param {*} fn 
     */
    showLogin(fn) {
        document.body.innerHTML = "";
        // 创建登录界面
        var div = document.createElement("div");
        div.id = "ecp-login";
        document.body.append(div);
        if (typeof fn === "function") fn(div);
    }

    /**
     * 绑定失败事件
     */
    onApiFail(fn) {
        var fails = this._fails;
        fails[fails.length] = fn;
    };

    /**
     * 绑定失败事件
     * @param {*} fn 
     */
    onApiError(fn) {
        var errors = this._errors;
        errors[errors.length] = fn;
    };

    /**
     * 获取交互标识
     */
    get SessionID() {
        return this._sid;
    }

    /**
     * 设置交互标识
     */
    set SessionID(value) {
        this._sid = value;
        localStorage.setItem("Mdr-Session-Id", value);
    }

    /**
     * 获取交互密钥
     */
    get SecurityKey() {
        return this._skey;
    }

    /**
     * 设置交互标识
     */
    set SecurityKey(value) {
        this._skey = value;
        localStorage.setItem("Mdr-Security-Key", value);
    }

    /**
     * 获取一个签名
     * @param {*} salt 
     * @param {*} timestamp 
     * @param {*} attach 
     */
    GetSign(salt, timestamp, attach) {
        var str = "";
        str = str + "$type=md5";
        str = str + "$salt=" + salt;
        str = str + "$time=" + timestamp;
        str = str + "$token=" + this._sid;
        if (typeof attach === "string") str = str + "$attach=" + attach;
        str = str + "$key=" + this._skey;
        return dpz2.getMD5(str);
    }

    /**
     * 访问Api
     */
    postApi(url, args, fn, fnfail) {
        var salt = dpz2.getRndString(32);
        var ts = dpz2.getTimestamp();
        var that = this;
        $.ajax({
            //请求类型，这里为POST
            type: 'POST',
            //你要请求的api的URL
            url: url,
            //是否使用缓存
            cache: false,
            //数据类型，这里我用的是json
            dataType: "text",
            //必要的时候需要用JSON.stringify() 将JSON对象转换成字符串
            data: dpz2.jsonSerialize(args), //data: {key:value}, 
            //添加额外的请求头
            headers: {
                'Access-Control-Allow-Origin': '*',
                'Mdr-Session-Id': that._sid,
                'Mdr-Security-Type': 'md5',
                'Mdr-Security-Salt': salt,
                'Mdr-Security-Timestamp': ts,
                'Mdr-Security-Sign': that.GetSign(salt, ts)
            },
            //请求成功的回调函数
            success: function (data) {
                //函数参数 "data" 为请求成功服务端返回的数据
                var jttp = dpz2.jsonDeserialize(data);
                // 判断状态
                var res = parseInt(jttp.Result);
                if (res > 0) {
                    if (typeof fn == "function") fn(jttp);
                } else if (res == 0) {
                    var fails = that._fails;
                    for (var i = 0; i < fails.length; i++) {
                        if (typeof fails[i] === "function") fails[i](jttp);
                    }
                    if (typeof fnfail === "function") fnfail(jttp);
                } else {
                    var errors = that._errors;
                    for (var i = 0; i < errors.length; i++) {
                        if (typeof errors[i] === "function") errors[i](jttp);
                    }
                    if (typeof fnfail === "function") fnfail(jttp);
                }
            },
        });

    };

    /**
     * 绑定完成事件
     * @param {void} fn 
     */
    ready(fn) {
        var handlers = this._handlers;
        handlers[handlers.length] = fn;
    }

    /**
     * 执行绑定事件
     */
    execHandlers() {
        var handlers = this._handlers;
        for (var i = 0; i < handlers.length; i++) {
            try {
                if (typeof handlers[i] === "function") handlers[i]();
            } catch (ex) { console.error(ex); }
        }
        this._handlers = [];
    }

}

/**
 * Jttp操作器
 */
var $jttp = new JttpOperator();

dpz2.ready(function () {

    // 执行事件
    $jttp.execHandlers();

    // 处理错误信息
    $jttp.onApiError(function (e) {
        console.error(e);
    });

    // 处理失败信息
    $jttp.onApiFail(function (e) {
        console.warn(e);
    });

    // 获取所有的Header
    var getHeaders = function () {
        var req = new XMLHttpRequest();
        req.open('GET', document.location.href, false);
        req.send(null);
        var headerArr = req.getAllResponseHeaders().split('\n');
        var headers = {};
        headerArr.forEach(item => {
            if (item !== '') {
                var index = item.indexOf(':');
                var key = item.slice(0, index);
                var value = item.slice(index + 1).trim();
                headers[key] = value
            }

        })
        return headers;
    }

    // 创建交互信息
    var sessionCreate = function (sid) {
        // 申请交互密钥
        $jttp.postApi("/modular-project-common/Api/Session/Create", {}, function (jttp) {
            var data = jttp.Data;
            $jttp.SessionID = data.SessionID;
            $jttp.SecurityKey = data.SecurityKey;
        });
    };

    // 判断交互标识是否存在
    $jttp.postApi("/modular-project-common/Api/Session/Keep", {}, function (jttp) {
        var data = jttp.Data;
        // 标识无效则重新申请交互标识
        if (data.Enable <= 0) {
            sessionCreate();
        }
    });

});

$(function () {
    dpz2.execHandlers();
});