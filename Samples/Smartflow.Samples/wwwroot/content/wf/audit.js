(function (factory) {

    function Audit(option) {
        this.user = util.getUser();
        this.setting = $.extend({
            url: 'api/smf/{instanceID}/node/{actorID}',
            bridge: 'api/setting/bridge/{id}/info'
        }, option);

        var $this = this;
        this.init(this.setting.key, function (s) {
            $this.setting.key = s.Key;
            $this.setting.code = s.CategoryCode;
            $this.setting.instanceID = s.InstanceID;
            $this.setCurrent();
            $this.bind();
        });
    }

    Audit.prototype.openAuditWindow = function () {
        var $this = this,
            setting = $this.setting,
            url = util.format($this.setting.url, { instanceID: setting.instanceID, actorID: $this.user.ID });
        util.ajaxWFService({
            type: 'get',
            url: url,
            success: function (r) {
                var mth = r && r.Category.toLowerCase() === 'form';
                util.openLayer((mth ? r.Url : {
                    title: '审批窗口',
                    url: 'WF/auditWindow.html',
                    width: 600,
                    height: 420
                }), {
                    code: setting.code,
                    instanceID: setting.instanceID,
                    id: setting.key
                });
            }
        });
    };

    Audit.prototype.setCurrent = function () {
        var $this = this,
            setting = $this.setting,
            url = util.format($this.setting.url, { instanceID: setting.instanceID, actorID: $this.user.ID });
        util.ajaxWFService({
            type: 'get',
            url: url,
            success: function (serverData) {
                var button = setting.button;
                $(button).val(serverData.Name);
                setting.name = serverData.Name;
                if (serverData.Category.toLowerCase() == 'end') {
                    $(setting.button).hide();
                } else {
                    if (!serverData.HasAuth) {
                        $(button)
                            .addClass("layui-disabled")
                            .attr("disabled", "disabled");
                    }
                }
            }
        });
    };

    Audit.prototype.init = function (id, callback) {
        var $this = this;
        var url = util.format($this.setting.bridge, { id: id });
        util.ajaxWFService({
            type: 'get',
            url: url,
            success: function (s) {
                callback && callback(s);
            }
        });
    }

    Audit.prototype.bind = function () {
        var $this = this,
            setting = $this.setting;
        $(setting.button).click(function () {
            $this.openAuditWindow();
        });

        $(setting.image).click(function () {
            var url = util.process + setting.instanceID;
            util.openWin(url, '流程图', window.screen.availWidth - 100, window.screen.availHeight - 160);
        });
    }

    factory(function (option) {
        return new Audit(option);
    });

})(function (instance) {
    $.Audit = instance;
});

