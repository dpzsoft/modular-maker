/**
 * dpz2入口
 */
var dpz2 = {};

/**
 * 套件版本号
 */
dpz2.Version = "1.0.1912.1";

/**
 * 判断值是否为对象
 * @param {any} obj 待判断对象
 * @return {Boolean} 是否为对象判断结果
 */
dpz2.isObject = function (obj) { return typeof val === "object"; }

/**
 * 判断对象是否为数组
 * @param {any} obj 待判断对象
 * @return {Boolean} 对象为数组结果
 */
dpz2.isArray = function (obj) { return Object.prototype.toString.call(obj) === '[object Array]'; };

/**
 * 判断对象是否为函数
 * @param {any} obj 待判断对象
 * @return {Boolean} 对象为函数结果
 */
dpz2.isFunction = function (obj) { return typeof obj === 'function'; };

/**
 * 判断对象是否为空
 * @param {any} obj 待判断对象
 * @return {Boolean} 对象为空判断结果
 */
dpz2.isNull = function (obj) {
    switch (typeof obj) {
        case "undefined":
            return true;
        case "object":
            return obj === null;
        default:
            return false;
    }
};

/**
 * 判断对象是否为空字符串
 * @param {any} obj 待判断对象
 * @return {Boolean} 对象为空判断结果
 */
dpz2.isNoneString = function (obj) {
    switch (typeof obj) {
        case "undefined":
            return true;
        case "object":
            return obj === null;
        case "string":
            return obj === "";
        default:
            return false;
    }
};

/**
 * 判断对象是否为空字符串
 * @param {any} obj 待判断对象
 * @return {Boolean} 对象为空判断结果
 */
dpz2.isEmpty = function (obj) {
    switch (typeof obj) {
        case "undefined":
            return true;
        case "object":
            return obj === null;
        case "string":
            return obj === "";
        default:
            return false;
    }
};

/**
 * 生成一个随机字符串
 * @param {Number} len 字符串长度
 */
dpz2.getRndString = function (len) {
    len = len || 32;
    var $chars = 'QWERTYUIOPASDFGHJKLZXCVBNM1234567890qwertyuiopasdfghjklzxcvbnm';
    var maxPos = $chars.length;
    var res = '';
    for (var i = 0; i < len; i++) {
        res += $chars.charAt(Math.floor(Math.random() * maxPos));
    }
    return res;
}

/**
 * 获取时间戳
 * @param {*} date 日期
 */
dpz2.getTimestamp = function (date) {
    if (typeof data === "undefined") {
        return Math.floor((new Date()).getTime() / 1000);
    } else {
        return Math.floor((new Date(date)).getTime() / 1000);
    }
}

/**
 * UTF8编码操作
 */
class UTF8Encoding {
    /**
     * 将字符串内容转化为字节数组
     * @param {string} str 字符串
     * @returns {Array} 字节数组
     * */
    getBytes(str) {
        var back = [];
        var byteSize = 0;
        for (var i = 0; i < str.length; i++) {
            var code = str.charCodeAt(i);
            if (0x00 <= code && code <= 0x7f) {
                byteSize += 1;
                back.push(code);
            }
            else if (0x80 <= code && code <= 0x7ff) {
                byteSize += 2;
                back.push((192 | (31 & (code >> 6))));
                back.push((128 | (63 & code)));
            }
            else if ((0x800 <= code && code <= 0xd7ff)
                || (0xe000 <= code && code <= 0xffff)) {
                byteSize += 3;
                back.push((224 | (15 & (code >> 12))));
                back.push((128 | (63 & (code >> 6))));
                back.push((128 | (63 & code)));
            }
        }
        for (i = 0; i < back.length; i++) {
            back[i] &= 0xff;
        }
        return back;
    };

    /**
    * 将字节数组内容转化为字符串
    * @param {Array} arr 字节数组
    * @returns {string} 字符串
    * */
    getString(arr) {
        if (typeof arr === 'string') {
            return arr;
        }
        var UTF = '', _arr = arr;
        for (var i = 0; i < _arr.length; i++) {
            var one = _arr[i].toString(2), v = one.match(/^1+?(?=0)/);
            if (v && one.length == 8) {
                var bytesLength = v[0].length;
                var store = _arr[i].toString(2).slice(7 - bytesLength);
                for (var st = 1; st < bytesLength; st++) {
                    store += _arr[st + i].toString(2).slice(2);
                }
                UTF += String.fromCharCode(parseInt(store, 2));
                i += bytesLength - 1;
            }
            else {
                UTF += String.fromCharCode(_arr[i]);
            }
        }
        return UTF;
    };
}

/**
 * UTF8编码操作器
 */
dpz2.utf8 = new UTF8Encoding();

/**
 * 获取字符串MD5值
 * @param {String} s 字符串
 */
dpz2.getMD5 = function (s) {
    var objMD5 = {};
    objMD5.hexcase = 1; /* hex output format. 0 - lowercase; 1 - uppercase */
    objMD5.b64pad = ""; /* base-64 pad character. "=" for strict RFC compliance */
    objMD5.chrsz = 8; /* bits per input character. 8 - ASCII; 16 - Unicode */
    /*
    * These are the functions you'll usually want to call
    * They take string arguments and return either hex or base-64 encoded strings
    */
    objMD5.GetHexMD5 = function (s) {
        //var bytes = dykEncode.Utf8.GetBytes(s);
        var utf8Encoding = new UTF8Encoding();
        var bytes = utf8Encoding.getBytes(s);
        return objMD5.binl2hex(objMD5.core_md5(objMD5.arr2binl(bytes), bytes.length * objMD5.chrsz));
        //return objMD5.binl2hex(objMD5.core_md5(objMD5.str2binl(s), s.length * objMD5.chrsz));
    };
    objMD5.GetB64MD5 = function (s) { return objMD5.binl2b64(objMD5.core_md5(objMD5.str2binl(s), s.length * objMD5.chrsz)); };
    objMD5.GetHexHMacMD5 = function (key, data) { return objMD5.binl2hex(objMD5.core_hmac_md5(key, data)); };
    objMD5.GetB64HMacMD5 = function (key, data) { return objMD5.binl2b64(objMD5.core_hmac_md5(key, data)); };
    /* Backwards compatibility - same as hex_md5() */
    objMD5.calcMD5 = function (s) { return objMD5.binl2hex(objMD5.core_md5(objMD5.str2binl(s), s.length * objMD5.chrsz)); };
    /*
    * Perform a simple self-test to see if the VM is working
    */
    objMD5.md5_vm_test = function () {
        return hex_md5("abc") == "900150983cd24fb0d6963f7d28e17f72";
    };
    /*
    * Calculate the MD5 of an array of little-endian words, and a bit length
    */
    objMD5.core_md5 = function (x, len) {
        /* append padding */
        x[len >> 5] |= 0x80 << ((len) % 32);
        x[(((len + 64) >>> 9) << 4) + 14] = len;
        var a = 1732584193;
        var b = -271733879;
        var c = -1732584194;
        var d = 271733878;
        for (var i = 0; i < x.length; i += 16) {
            var olda = a;
            var oldb = b;
            var oldc = c;
            var oldd = d;
            a = objMD5.md5_ff(a, b, c, d, x[i + 0], 7, -680876936);
            d = objMD5.md5_ff(d, a, b, c, x[i + 1], 12, -389564586);
            c = objMD5.md5_ff(c, d, a, b, x[i + 2], 17, 606105819);
            b = objMD5.md5_ff(b, c, d, a, x[i + 3], 22, -1044525330);
            a = objMD5.md5_ff(a, b, c, d, x[i + 4], 7, -176418897);
            d = objMD5.md5_ff(d, a, b, c, x[i + 5], 12, 1200080426);
            c = objMD5.md5_ff(c, d, a, b, x[i + 6], 17, -1473231341);
            b = objMD5.md5_ff(b, c, d, a, x[i + 7], 22, -45705983);
            a = objMD5.md5_ff(a, b, c, d, x[i + 8], 7, 1770035416);
            d = objMD5.md5_ff(d, a, b, c, x[i + 9], 12, -1958414417);
            c = objMD5.md5_ff(c, d, a, b, x[i + 10], 17, -42063);
            b = objMD5.md5_ff(b, c, d, a, x[i + 11], 22, -1990404162);
            a = objMD5.md5_ff(a, b, c, d, x[i + 12], 7, 1804603682);
            d = objMD5.md5_ff(d, a, b, c, x[i + 13], 12, -40341101);
            c = objMD5.md5_ff(c, d, a, b, x[i + 14], 17, -1502002290);
            b = objMD5.md5_ff(b, c, d, a, x[i + 15], 22, 1236535329);
            a = objMD5.md5_gg(a, b, c, d, x[i + 1], 5, -165796510);
            d = objMD5.md5_gg(d, a, b, c, x[i + 6], 9, -1069501632);
            c = objMD5.md5_gg(c, d, a, b, x[i + 11], 14, 643717713);
            b = objMD5.md5_gg(b, c, d, a, x[i + 0], 20, -373897302);
            a = objMD5.md5_gg(a, b, c, d, x[i + 5], 5, -701558691);
            d = objMD5.md5_gg(d, a, b, c, x[i + 10], 9, 38016083);
            c = objMD5.md5_gg(c, d, a, b, x[i + 15], 14, -660478335);
            b = objMD5.md5_gg(b, c, d, a, x[i + 4], 20, -405537848);
            a = objMD5.md5_gg(a, b, c, d, x[i + 9], 5, 568446438);
            d = objMD5.md5_gg(d, a, b, c, x[i + 14], 9, -1019803690);
            c = objMD5.md5_gg(c, d, a, b, x[i + 3], 14, -187363961);
            b = objMD5.md5_gg(b, c, d, a, x[i + 8], 20, 1163531501);
            a = objMD5.md5_gg(a, b, c, d, x[i + 13], 5, -1444681467);
            d = objMD5.md5_gg(d, a, b, c, x[i + 2], 9, -51403784);
            c = objMD5.md5_gg(c, d, a, b, x[i + 7], 14, 1735328473);
            b = objMD5.md5_gg(b, c, d, a, x[i + 12], 20, -1926607734);
            a = objMD5.md5_hh(a, b, c, d, x[i + 5], 4, -378558);
            d = objMD5.md5_hh(d, a, b, c, x[i + 8], 11, -2022574463);
            c = objMD5.md5_hh(c, d, a, b, x[i + 11], 16, 1839030562);
            b = objMD5.md5_hh(b, c, d, a, x[i + 14], 23, -35309556);
            a = objMD5.md5_hh(a, b, c, d, x[i + 1], 4, -1530992060);
            d = objMD5.md5_hh(d, a, b, c, x[i + 4], 11, 1272893353);
            c = objMD5.md5_hh(c, d, a, b, x[i + 7], 16, -155497632);
            b = objMD5.md5_hh(b, c, d, a, x[i + 10], 23, -1094730640);
            a = objMD5.md5_hh(a, b, c, d, x[i + 13], 4, 681279174);
            d = objMD5.md5_hh(d, a, b, c, x[i + 0], 11, -358537222);
            c = objMD5.md5_hh(c, d, a, b, x[i + 3], 16, -722521979);
            b = objMD5.md5_hh(b, c, d, a, x[i + 6], 23, 76029189);
            a = objMD5.md5_hh(a, b, c, d, x[i + 9], 4, -640364487);
            d = objMD5.md5_hh(d, a, b, c, x[i + 12], 11, -421815835);
            c = objMD5.md5_hh(c, d, a, b, x[i + 15], 16, 530742520);
            b = objMD5.md5_hh(b, c, d, a, x[i + 2], 23, -995338651);
            a = objMD5.md5_ii(a, b, c, d, x[i + 0], 6, -198630844);
            d = objMD5.md5_ii(d, a, b, c, x[i + 7], 10, 1126891415);
            c = objMD5.md5_ii(c, d, a, b, x[i + 14], 15, -1416354905);
            b = objMD5.md5_ii(b, c, d, a, x[i + 5], 21, -57434055);
            a = objMD5.md5_ii(a, b, c, d, x[i + 12], 6, 1700485571);
            d = objMD5.md5_ii(d, a, b, c, x[i + 3], 10, -1894986606);
            c = objMD5.md5_ii(c, d, a, b, x[i + 10], 15, -1051523);
            b = objMD5.md5_ii(b, c, d, a, x[i + 1], 21, -2054922799);
            a = objMD5.md5_ii(a, b, c, d, x[i + 8], 6, 1873313359);
            d = objMD5.md5_ii(d, a, b, c, x[i + 15], 10, -30611744);
            c = objMD5.md5_ii(c, d, a, b, x[i + 6], 15, -1560198380);
            b = objMD5.md5_ii(b, c, d, a, x[i + 13], 21, 1309151649);
            a = objMD5.md5_ii(a, b, c, d, x[i + 4], 6, -145523070);
            d = objMD5.md5_ii(d, a, b, c, x[i + 11], 10, -1120210379);
            c = objMD5.md5_ii(c, d, a, b, x[i + 2], 15, 718787259);
            b = objMD5.md5_ii(b, c, d, a, x[i + 9], 21, -343485551);
            a = objMD5.safe_add(a, olda);
            b = objMD5.safe_add(b, oldb);
            c = objMD5.safe_add(c, oldc);
            d = objMD5.safe_add(d, oldd);
        }
        return Array(a, b, c, d);
    };
    /*
    * These functions implement the four basic operations the algorithm uses.
    */
    objMD5.md5_cmn = function (q, a, b, x, s, t) {
        return objMD5.safe_add(objMD5.bit_rol(objMD5.safe_add(objMD5.safe_add(a, q), objMD5.safe_add(x, t)), s), b);
    };
    objMD5.md5_ff = function (a, b, c, d, x, s, t) {
        return objMD5.md5_cmn((b & c) | ((~b) & d), a, b, x, s, t);
    };
    objMD5.md5_gg = function (a, b, c, d, x, s, t) {
        return objMD5.md5_cmn((b & d) | (c & (~d)), a, b, x, s, t);
    };
    objMD5.md5_hh = function (a, b, c, d, x, s, t) {
        return objMD5.md5_cmn(b ^ c ^ d, a, b, x, s, t);
    };
    objMD5.md5_ii = function (a, b, c, d, x, s, t) {
        return objMD5.md5_cmn(c ^ (b | (~d)), a, b, x, s, t);
    };
    /*
    * Calculate the HMAC-MD5, of a key and some data
    */
    objMD5.core_hmac_md5 = function (key, data) {
        var bkey = objMD5.str2binl(key);
        if (bkey.length > 16)
            bkey = objMD5.core_md5(bkey, key.length * objMD5.chrsz);
        var ipad = Array(16), opad = Array(16);
        for (var i = 0; i < 16; i++) {
            ipad[i] = bkey[i] ^ 0x36363636;
            opad[i] = bkey[i] ^ 0x5C5C5C5C;
        }
        var hash = objMD5.core_md5(ipad.concat(objMD5.str2binl(data)), 512 + data.length * objMD5.chrsz);
        return objMD5.core_md5(opad.concat(hash), 512 + 128);
    };
    /*
    * Add integers, wrapping at 2^32. This uses 16-bit operations internally
    * to work around bugs in some JS interpreters.
    */
    objMD5.safe_add = function (x, y) {
        var lsw = (x & 0xFFFF) + (y & 0xFFFF);
        var msw = (x >> 16) + (y >> 16) + (lsw >> 16);
        return (msw << 16) | (lsw & 0xFFFF);
    };
    /*
    * Bitwise rotate a 32-bit number to the left.
    */
    objMD5.bit_rol = function (num, cnt) {
        return (num << cnt) | (num >>> (32 - cnt));
    };
    /*
    * Convert a string to an array of little-endian words
    * If chrsz is ASCII, characters >255 have their hi-byte silently ignored.
    */
    objMD5.str2binl = function (str) {
        //Unicode方式
        //var bin = Array();
        //var mask = (1 << objMD5.chrsz) - 1;
        //for (var i = 0; i < str.length * objMD5.chrsz; i += objMD5.chrsz)
        //    bin[i >> 5] |= (str.charCodeAt(i / objMD5.chrsz) & mask) << (i % 32);
        //return bin;
        var utf8 = new DykJsDevelopmentKitEncodeUtf8();
        var bytes = utf8.GetBytes(str);
        var bin = Array();
        var mask = (1 << objMD5.chrsz) - 1;
        for (var i = 0; i < bytes.length * objMD5.chrsz; i += objMD5.chrsz)
            bin[i >> 5] |= (bytes[i / objMD5.chrsz] & mask) << (i % 32);
        return bin;
        //return dykEncode.Utf8.GetBytes(str);
    };
    objMD5.arr2binl = function (arr) {
        //Unicode方式
        //var bin = Array();
        //var mask = (1 << objMD5.chrsz) - 1;
        //for (var i = 0; i < str.length * objMD5.chrsz; i += objMD5.chrsz)
        //    bin[i >> 5] |= (str.charCodeAt(i / objMD5.chrsz) & mask) << (i % 32);
        //return bin;
        //var bytes = dykEncode.Utf8.GetBytes(str);
        var bin = Array();
        var mask = (1 << objMD5.chrsz) - 1;
        for (var i = 0; i < arr.length * objMD5.chrsz; i += objMD5.chrsz)
            bin[i >> 5] |= (arr[i / objMD5.chrsz] & mask) << (i % 32);
        return bin;
        //return dykEncode.Utf8.GetBytes(str);
    };
    /*
    * Convert an array of little-endian words to a hex string.
    */
    objMD5.binl2hex = function (binarray) {
        var hex_tab = objMD5.hexcase ? "0123456789ABCDEF" : "0123456789abcdef";
        var str = "";
        for (var i = 0; i < binarray.length * 4; i++) {
            str += hex_tab.charAt((binarray[i >> 2] >> ((i % 4) * 8 + 4)) & 0xF) +
                hex_tab.charAt((binarray[i >> 2] >> ((i % 4) * 8)) & 0xF);
        }
        return str;
    };
    /*
    * Convert an array of little-endian words to a base-64 string
    */
    objMD5.binl2b64 = function (binarray) {
        var tab = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        var str = "";
        for (var i = 0; i < binarray.length * 4; i += 3) {
            var triplet = (((binarray[i >> 2] >> 8 * (i % 4)) & 0xFF) << 16)
                | (((binarray[i + 1 >> 2] >> 8 * ((i + 1) % 4)) & 0xFF) << 8)
                | ((binarray[i + 2 >> 2] >> 8 * ((i + 2) % 4)) & 0xFF);
            for (var j = 0; j < 4; j++) {
                if (i * 8 + j * 6 > binarray.length * 32)
                    str += objMD5.b64pad;
                else
                    str += tab.charAt((triplet >> 6 * (3 - j)) & 0x3F);
            }
        }
        return str;
    };

    //return objMD5;
    return objMD5.GetHexMD5(s);
}

/**
 * 网络套件
 * */
class Network {

    /**
     * 获取系统的HTTP请求器
     */
    getHttpRequest() {
        var http_request = false;
        //开始初始化XMLHttpRequest对象
        if (window.XMLHttpRequest) {
            http_request = new XMLHttpRequest();
            if (http_request.overrideMimeType) {
                http_request.overrideMimeType("text/xml");
            }
        }
        else if (window.ActiveXObject) {
            try {
                http_request = new ActiveXObject("Msxml2.XMLHttp");
            }
            catch (e) {
                http_request = new ActiveXobject("Microsoft.XMLHttp");
            }
        }
        if (!http_request) {
            return null;
        }
        return http_request;
    };

    /**
     * 监听ajax状态变更
     * @param {*} http_request 
     * @param {*} fsuccess 
     * @param {*} ferror 
     */
    ajaxStateChange(http_request, fsuccess, ferror) {
        if (http_request.readyState === 4) {
            // alert(http_request.status);
            if (http_request.status === 200) {
                var reqStr = http_request.responseText;

                if (isNoneString(reqStr)) return;
                if (fsuccess)
                    fsuccess(reqStr);
            }
            else {
                //alert("您所请求的页面不正常！");
                if (ferror)
                    ferror(http_request.status, "");
            }
        }
        else {
            if (ferror)
                ferror(http_request.readyState, "");
        }
    };

    /**
     * 从文件框上传文件
     * @param {any} url 上传地址
     * @param {any} fileobj 文件对象
     * @param {any} fprocess 进度调用时的回调
     * @param {any} fsuccess 成功时的回调
     * @param {any} ferror 错误时的回调
     * @returns {void}
     */
    upload(url, fileobj, fprocess, fsuccess, ferror) {
        var fd = new FormData();
        //var fileobj = document.getElementById(fileid);
        if (!fileobj) {
            alert("未找到文件选框");
            return;
        }
        if (fileobj.files.length <= 0) {
            alert("请先选择一个上传文件");
            return;
        }
        fd.append("file1", fileobj.files[0]);

        var http_request = getHttpRequest();
        http_request.onreadystatechange = function () {
            ajaxStateChange(http_request, fsuccess, ferror);
        };
        http_request.upload.onprogress = function (evt) {
            if (fprocess)
                fprocess(evt);
        };
        try {
            //确定发送请求方式，URL，及是否同步执行下段代码
            http_request.open("Post", url, true);
            //http_request.setRequestHeader("Content-type", "multipart/form-data");
            // post 方法必须有的函数2 => 传值的方法 .send( 'name=value' );
            http_request.send(fd);
        }
        catch (e) {
            if (ferror)
                ferror(0, e);
        }
    };

    /**
     * 上传文件
     * @param {any} fnselect 文件选择时的回调
     * @param {any} fprocess 进度调用时的回调
     * @param {any} fsuccess 成功时的回调
     * @param {any} ferror 错误时的回调
     */
    uploadFile(fnselect, fprocess, fsuccess, ferror) {
        var input = document.createElement("input");
        input.name = "dpz_file_input";
        input.type = "file";
        input.style.display = "none";
        document.body.appendChild(input);

        input.addEventListener("change", function () {
            if (fnselect) {
                fnselect({
                    input: input,
                    name: input.files[0].name,
                    upload: function (url) {
                        if (isNoneString(url)) return;
                        //alert(url);
                        //console.log(url);
                        res.upload(url, input, fprocess, function (responseText) {
                            //document.body.removeChild(input);
                            input.remove();
                            if (fsuccess)
                                fsuccess(responseText);
                        }, function (status, msg) {
                            //document.body.removeChild(input);
                            input.remove();
                            if (ferror)
                                ferror(status, msg);
                        });
                    }
                });
            }
            else {
                //document.body.removeChild(input);
                input.remove();
            }
        });

        //console.log(input);
        //input.click();
        // 模拟input点击事件
        var evt = new MouseEvent("click", {
            bubbles: false,
            cancelable: true,
            view: window
        });
        input.dispatchEvent(evt);
    };
}

/**
 * 网络操作器
 */
dpz2.net = new Network();

/**
 * 上传文件
 * @param {any} url 提交地址
 * @param {any} fsuccess 回调地址
 */
dpz2.upload = function (url, fsuccess) {
    var network = new Network();
    network.uploadFile(function (ob) {
        ob.upload(url);
    }, null, fsuccess, null);
};

/**
 * 表单管理
 * */
class FormManager {

    constructor(el) {
        this._el = el;
    };

    getValues() {
        var i = 0;
        var ele = this._el;
        var res = {};

        //处理所有的Input
        var inputs = ele.getElementsByTagName("input");
        for (i = 0; i < inputs.length; i++) {
            var input = inputs[i];
            if (!isNull(input.name)) {
                if (input.name !== "") {
                    res[input.name] = input.value;
                }
            }
        }

        //处理所有的texteare
        var txts = ele.getElementsByTagName("textarea");
        for (i = 0; i < txts.length; i++) {
            var txt = txts[i];
            if (!isNull(txt.name)) {
                if (txt.name !== "") {
                    res[txt.name] = txt.value;
                }
            }
        }

        //处理所有的texteare
        var selects = ele.getElementsByTagName("select");
        for (i = 0; i < selects.length; i++) {
            var select = selects[i];
            if (!isNull(select.name)) {
                if (select.name !== "") {
                    res[select.name] = select.value;
                }
            }
        }

        return res;
    };
}

/**
 * 获取一个表单管理器
 */
dpz2.getForm = function (el) { return new FormManager(el); }

/**
 * 获取一个表单管理器
 */
dpz2.getFormById = function (id) {
    var el = document.getElementById(id);
    return new FormManager(el);
}

/**
 * json对象序列化
 * @param {*} obj 
 */
dpz2.jsonSerialize = function (obj) {
    var res = "";
    var my = this;
    if (typeof obj === "object") {
        //为对象
        if (isArray(obj)) {
            //为数组
            for (p in obj) {
                if (typeof obj[p] !== "function") {
                    if (res !== "")
                        res += ",";
                    res += my.getString(obj[p]);
                }
            }
            return "[" + res + "]";
        }
        else {
            //不为数组
            for (p in obj) {
                if (typeof obj[p] !== "function") {
                    if (res !== "")
                        res += ",";
                    res += "\"" + p + "\":";
                    res += my.getString(obj[p]);
                }
            }
            return "{" + res + "}";
        }
    }
    else {
        return "\"" + obj + "\"";
    }
}

/**
 * json字符串反序列化
 * @param {*} str 
 */
dpz2.jsonDeserialize = function (str) {
    try {
        var obj = eval('(' + str + ')');
        return obj;
    }
    catch (e) {
        console.error("转换对象发生异常!");
        console.error(str);
        return {};
    }
}

var handlers = []

/**
 * 绑定完成事件
 * @param {void} fn 
 */
dpz2.ready = function (fn) {
    handlers[handlers.length] = fn;
}

/**
 * 执行绑定事件
 */
dpz2.execHandlers = function () {
    for (var i = 0; i < handlers.length; i++) {
        try {
            if (typeof handlers[i] === "function") handlers[i]();
        } catch (ex) { console.error(ex); }
    }
    handlers = [];
}

/**
 * 大胖子动态加载器
 * */
class DynamicLoader {

    constructor() {
        this._box = {
            styles: [],
            scripts: [],
            images: []
        };
    }

    /**
    * 添加一个Css文件
    * @param {string} id 唯一标识符
    * @param {string} url 地址
    */
    addCss(id, url) {
        var idx = this._box.styles.length;
        this._box.styles[idx] = { id: id, url: url };
    };

    /**
    * 添加一个Js文件
    * @param {string} id 唯一标识符
    * @param {string} url 地址
    */
    addJs(id, url) {
        var idx = this._box.scripts.length;
        this._box.scripts[idx] = { id: id, url: url };
    };

    /**
    * 添加一张图片
    * @param {string} url 地址
    */
    addImage(url) {
        var idx = this._box.images.length;
        this._box.images[idx] = url;
    };

    /**
    * 加载一个样式文件
    * @param {string} id  唯一标识符
    * @param {string} url 地址
    * @param {void} fn 回调函数
    */
    loadCss(id, url, fn) {

        if (url.indexOf("?") > 0) {
            url += "&rnd=" + Math.random();
        } else {
            url += "?rnd=" + Math.random();
        }

        var js = document.getElementById(id);
        if (!js) {
            js = document.createElement("link");
            js.id = id;
            js.href = url;
            js.rel = "stylesheet";

            js.onload = js.onreadystatechange = function () {
                if (!this.readyState || this.readyState === 'loaded' || this.readyState === 'complete') {
                    //alert('done');
                    if (fn) fn();
                }
                js.onload = js.onreadystatechange = null;
            };

            document.head.appendChild(js);
            //while (!X.Configs.Completed[name]) { }
        } else {
            if (fn) fn();
        }
    };

    /**
    * 加载一个脚本文件
    * @param {string} id  唯一标识符
    * @param {string} url 地址
    * @param {void} fn 回调函数
    */
    loadJs(id, url, fn) {
        //var id = "X_" + name
        if (url.indexOf("?") > 0) {
            url += "&rnd=" + Math.random();
        } else {
            url += "?rnd=" + Math.random();
        }

        var js = document.getElementById(id);
        if (!js) {
            js = document.createElement("script");
            js.id = id;
            js.src = url;

            js.onload = js.onreadystatechange = function () {
                if (!this.readyState || this.readyState === 'loaded' || this.readyState === 'complete') {
                    // 执行专用入口绑定函数
                    execHandlers();
                    if (fn) fn();
                }
                js.onload = js.onreadystatechange = null;
            };

            document.head.appendChild(js);
            //while (!X.Configs.Completed[name]) { }
        } else {
            if (fn) fn();
        }
    };

    /**
    * 动态加载图片
    * @param {string} url 地址
    * @param {void} fun 回调函数
    * @param {void} errfun 发生错误时的回调函数
    */
    loadImage(url, fun, errfun) {
        var Img = new Image();

        Img.onerror = function () {
            if (errfun) errfun({ Url: url, Image: Img });
            if (fun) fun({ Url: url, Image: Img });
        };

        Img.onload = function () {
            if (fun) fun({ Url: url, Image: Img });
        };

        Img.src = url;
    };

    /**
    * 执行加载
    * @param {void} fn 回调函数
    */
    load(fn) {

        var my = this._box;
        var idx = 0;

        //加载所有的样式文件
        var loadStyles = function (fnStyles) {
            if (idx >= my.styles.length) {
                if (typeof fnStyles === "function") fnStyles();
                return;
            }

            //加载样式
            res.loadCss(my.styles[idx].id, my.styles[idx].url, function () {
                idx++;
                loadStyles(fnStyles);
            });
        };

        //加载所有的图片文件
        var loadImages = function (fnImages) {
            if (idx >= my.images.length) {
                if (typeof fnImages === "function") fnImages();
                return;
            }

            //加载样式
            res.loadImage(my.images[idx], function () {
                idx++;
                loadImages(fnImages);
            });
        };

        //加载所有的脚本文件
        var loadScripts = function (fnScripts) {
            if (idx >= my.scripts.length) {
                if (typeof fnScripts === "function") fnScripts();
                return;
            }

            //加载样式
            res.loadJs(my.scripts[idx].id, my.scripts[idx].url, function () {
                idx++;
                loadScripts(fnScripts);
            });
        };

        // 执行加载
        loadStyles(function () {
            idx = 0;
            loadImages(function () {
                idx = 0;
                loadScripts(function () {
                    my.styles = [];
                    my.images = [];
                    my.scripts = [];
                    if (typeof fn === "function") fn();
                });
            });
        });
    };

}

/**
 * 创建一个加载器
 */
dpz2.createLoader = function () { return new DynamicLoader(); }

/**
 * 动态加载一个Css文件
 * @param {String} id 
 * @param {String} url 
 * @param {void} fn 
 */
dpz2.loadCss = function (id, url, fn) {
    var loader = new DynamicLoader();
    loader.loadCss(id, url, fn);
}

/**
 * 动态加载一个js文件
 * @param {String} id 
 * @param {String} url 
 * @param {void} fn 
 */
dpz2.loadJs = function (id, url, fn) {
    var loader = new DynamicLoader();
    loader.loadJs(id, url, fn);
}

/**
 * 动态加载图片
 * @param {String} url 
 * @param {void} fn 
 */
dpz2.loadImage = function (url, fn) {
    var loader = new DynamicLoader();
    loader.loadImage(url, fn);
}