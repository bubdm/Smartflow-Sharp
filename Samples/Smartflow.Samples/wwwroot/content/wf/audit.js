(function (initialize) {

    function Audit(option) {
        this.user = util.getUser();
        this.setting = option;
        this.bridge = {};
        this.init();
    }

    Audit.prototype.openAuditWindow = function () {
        var $this = this,
            setting = $this.setting,
            url = util.format($this.setting.url, { instanceID: $this.bridge.instanceID, actorID: $this.user.ID });
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
                    code: $this.bridge.code,
                    instanceID: $this.bridge.instanceID,
                    id: $this.bridge.id
                });
            }
        });
    };

    Audit.prototype.setCurrent = function () {
        var $this = this,
            setting = $this.setting,
            url = util.format($this.setting.url, { instanceID: $this.bridge.instanceID, actorID: $this.user.ID });
        util.ajaxWFService({
            type: 'get',
            url: url,
            success: function (serverData) {
                var button = setting.button;
                $(button).val(serverData.Name);
                if (serverData.Category.toLowerCase() == 'end') {
                    $(button).hide();
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

    Audit.prototype.init = function () {
        var $this = this, url = util.format($this.setting.bridge, { id: this.setting.key });
        util.ajaxWFService({
            type: 'get',
            url: url,
            success: function (s) {
                $this.bridge = Object.create({
                    id: s.Key,
                    code: s.CategoryCode,
                    instanceID: s.InstanceID
                });
                $this.setCurrent();
                $this.bind();
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
            var url = util.process + $this.bridge.instanceID;
            util.openWin(url, '流程图', window.screen.availWidth - 100, window.screen.availHeight - 160);
        });
    }

    initialize(function (option) {
        return new Audit(option);
    });

})(function (createInstance) {
    $.Audit = function (option) {

        var settting = {
            url: 'api/smf/{instanceID}/node/{actorID}',
            bridge: 'api/setting/bridge/{id}/info'
        };

        return createInstance(Object.assign(option, settting));
    }
});

