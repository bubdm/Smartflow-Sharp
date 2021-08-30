/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 ********************************************************************
 */
(function (factory) {

    function Record(option) {
        this.setting = $.extend({}, option);
        this.init();
    }

    Record.prototype.init = function () {
        var $this = this,
            setting = $this.setting;

        this.loadBrigdge(setting.key, function (s) {
            $this.load(s.InstanceID);
        });
    }

    Record.prototype.load = function (instanceID) {
        var $this = this,
            setting = this.setting,
            url = util.format($this.setting.urls.url, { instanceID: instanceID });
        util.ajaxWFService({
            url: url,
            type: 'get',
            success: function (serverData) {
                var htmlArray = [];
                $.each(serverData, function () {
                    var el = setting.templet;
                    htmlArray.push(
                        el.replace(/{{Name}}/ig, setting.Type ? this.OrganizationName : this.Name)
                            .replace(/{{Comment}}/ig, this.Comment)
                            .replace(/{{CreateTime}}/ig, this.CreateTime ? layui.util.toDateString(this.CreateTime, 'yyyy.MM.dd HH:mm') : '')
                            .replace(/{{Sign}}/ig, util.isEmpty(this.Url) ? '' : "<image src=\"" + this.Url + "\" />")
                            .replace(/{{AuditUserName}}/ig, this.AuditUserName)
                    );
                });
                $(setting.id).html(htmlArray.join(''));
                setting.done && setting.done(serverData);
            }
        });
    }

    Record.prototype.loadBrigdge = function (id, callback) {
        var $this = this,
            url = util.format($this.setting.urls.bridge, { id: id });
        util.ajaxWFService({
            type: 'get',
            url: url,
            success: function (s) {
                callback && callback(s);
            }
        });
    }

    Record.prototype.refresh = function () {
        layui.table.reload(this.setting.config.id);
    };

    factory(function (option) {
        return new Record(option);
    });

})(function (createInstance) {
    $.Record = function (option) {
        var templet = "<tr><td class=\"flow-node\" rowspan=\"2\">{{Name}}</td><td class=\"flow-message\">{{Comment}}</td><tr><td colspan=\"2\" class=\"flow-sign\">{{CreateTime}}&nbsp;&nbsp;&nbsp;{{AuditUserName}}</td></tr></tr>";
        return createInstance(Object.assign({
            templet: templet,
            id: 'table.record-table',
            urls: {
                url: 'api/setting/record/{instanceID}/list',
                bridge: 'api/setting/bridge/{id}/info'
            }
        }, option));
    }
});
