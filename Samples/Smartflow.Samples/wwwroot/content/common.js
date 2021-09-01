(function () {
    window.util = {
        prefix: 'http://localhost:59095/',
        smf: 'http://localhost:8097/',
        process: 'http://localhost:59095/wf/image.html?id=',
        pending: 'http://localhost:59095/wf/pending.html?id=',
        actor: 'http://localhost:59095/wf/actorSelect.html',
        carbon: 'http://localhost:59095/wf/userSelect.html',
        ajaxService: function (settings) {
            var url = util.prefix + settings.url;
            var defaultSettings = $.extend({
                dataType: 'json',
                type: 'post',
                contentType: 'application/json',
                cache: false,
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert('流程发生异常,请与系统管理员联系');
                }
            }, settings, { url: url });
            $.ajax(defaultSettings);
        },
        ajaxWFService: function (settings) {
            var url = util.smf + settings.url;
            var defaultSettings = $.extend({
                dataType: 'json',
                type: 'post',
                contentType: 'application/json',
                cache: false,
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert('流程发生异常,请与系统管理员联系');
                }
            }, settings, { url: url });
            $.ajax(defaultSettings);
        },
        confirm: function (message, callback) {
            window.top.layer.confirm(message, {
                title: util.title,
                btn: ['确定', '取消']
            }, function (index) {
                callback & callback();
                window.top.layer.close(index);
            }, function (index) {
                window.top.layer.close(index);
            });
        },
        message: {
            record: '请选择记录',
            running: '请选择正在运行中流程',
            success: '操作成功',
            kill: '确定终止流程',
            delete: '确定删除'
        },
        serialize: function (obj) {
            var arr = [];
            for (var p in obj) {
                arr.push(p + '=' + obj[p]);
            }
            return arr.join('&');
        },
        create: function (option) {
            var url = util.prefix + option.url;
            var defaultSettings = {
                type: 2,
                content: url,
                title: false,
                closeBtn: 1,
                shade: [0.6],
                shadeClose: false,
                area: [option.width + 'px', option.height + 'px'],
                offset: 'auto'
            };

            var setting = $.extend({}, option);
            $.each(['width', 'height', 'url'], function (i, name) {
                if (setting[name]) {
                    delete setting[name];
                }
            });
            var index = 0;
            if (setting.host === 'window') {
                delete setting[host];
                index = layer.open($.extend(defaultSettings, setting));
            } else {
                index = window.top.layer.open($.extend(defaultSettings, setting));
            }
            return index;
        },
        openLayer: function (wnd, args) {
            if ($.isPlainObject(wnd)) {
                var url = !!args ? wnd.url + '?' + util.serialize(args) : wnd.url;
                util.create($.extend({}, wnd, { url: url }));
            } else {
                util.create({
                    title: '窗体弹多大，由你来决定',
                    width: 600,
                    url: wnd,
                    height: 420
                });
            }
        },
        openFullLayer: function (name, args, title) {
            var wnd = window.top.util.windows[name];
            var url = !!args ? wnd.url + '?' + util.serialize(args) : wnd.url;
            var option = $.extend({}, wnd, { url: util.prefix + url });
            var defaultSettings = {
                type: 2,
                content: url,
                title: false,
                closeBtn: 1,
                shade: [0.6],
                shadeClose: false,
                offset: 'auto'
            };
            var setting = $.extend({}, option);
            $.each(['width', 'height', 'url'], function (i, name) {
                if (setting[name]) {
                    delete setting[name];
                }
            });
            var index = window.top.layer.open($.extend(defaultSettings, setting, { title: title }));
            window.top.layer.full(index);
        },
        openDetailFullLayer: function (url, args, title) {
            var url = !!args ? url + '?' + util.serialize(args) : url;
            var option = $.extend({}, { url: util.prefix + url });
            var defaultSettings = {
                type: 2,
                content: url,
                title: false,
                closeBtn: 1,
                shade: [0.6],
                shadeClose: false,
                offset: 'auto'
            };
            var setting = $.extend({}, option);
            var index = window.top.layer.open($.extend(defaultSettings, setting, { title: title }));
            window.top.layer.full(index);
        },
        isEmpty: function (value) {
            return (value == '' || !value) ? false : true;
        },
        openWin: function (url, title, width, height) {
            var h = height || 720;
            var w = width || 1000;
            var top = (window.screen.availHeight - h) / 2;
            var left = (window.screen.availWidth - w) / 2;
            window.open(url, title, "width=" + w + ", height=" + h + ",top=" + top + ",left=" + left + ",titlebar=no,menubar=no,scrollbars=yes,resizable=yes,status=yes,toolbar=no,location=no");
        },
        doQuery: function (name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
            var rw = window.location.search.substr(1).match(reg);
            return (rw != null) ? rw[2] : false;
        },
        getUser: function () {
            return JSON.parse(Base64.fromBase64(window.localStorage.getItem('ticket')));
        },
        image: function (id, code) {
            var url = util.format("api/setting/bridge/{categoryCode}/{id}/single", { categoryCode: code, id: id });
            util.ajaxService({
                type: 'get',
                url: url,
                success: function (serverData) {
                    util.openWin(util.process + serverData.InstanceID);
                }
            });
        },
        table: function (option) {
            var url = util.smf + option.url;
            var setting = {
                page: true
                , defaultToolbar: false
                , method: 'post'
                , contentType: 'application/json'
                , cellMinWidth: 80
                , parseData: function (res) {
                    return {
                        "code": (res.Code == 200 ? 0 : res.Code),
                        "msg": (res.Code == 200 ? '' : res.Data),
                        "count": res.Total,
                        "data": res.Data
                    };
                }
            };
            layui.table.render($.extend(setting, option, { url: url }));
        },
        format: function (url, params) {
            var r = url;
            for (var propertyName in params) {
                r = r.replace("{" + propertyName + "}", params[propertyName]);
            }
            return r;
        },
        refresh: function () {
            var ifr = window.top.document.getElementById('ifrmContent').contentWindow;
            if (ifr.invoke) {
                ifr.invoke();
            }
        },
        convertToActor: function (arr) {
            var resultSet = [];
            $.each(arr, function () {
                resultSet.push({
                    id: this['ID'],
                    name: this['Name']
                });
            });
            return resultSet;
        }
    };
})();