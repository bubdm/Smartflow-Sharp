(function (factory) {

    function AuditWindow(option) {
        var $this = this;
        var instanceID = util.doQuery("instanceID");
        var code = util.doQuery('code');
        this.back = "NID_BACK_ID_80_11";
        this.sender = "NID_BACK_ID_80_12";
        this.veto = "NID_REJECT_ID_80_11";
        this.user = util.getUser();
        this.nodeSetting = {
            group: [],
            organization: [],
            actor: [],
            carbon: []
        };
        this.ActorIndex = 0;
        this.settings = $.extend({
            instanceID: instanceID,
            code: code
        }, option);
        $.each(['_bindSelect', '_initEvent'], function (index, propertyName) {
            $this[propertyName]();
        });
    }

    AuditWindow.prototype.close = function () {
        parent.layer.closeAll();
        if (window.top.util.refresh) {
            window.top.util.refresh();
        }
    };

    AuditWindow.prototype.actorSelect = function (id, callback) {
        var $this = this,
            setting = $this.settings,
            url = util.format(setting.actorUrl, {
                instanceID: setting.instanceID,
                destination: id
            });
        util.ajaxWFService({
            url: url,
            type: 'get',
            success: function (data) {
                $this.nodeSetting.actor = util.convertToActor(data.Actor);
                $this.nodeSetting.group = util.convertToActor(data.Group);
                $this.nodeSetting.organization = util.convertToActor(data.Organization);
                $this.nodeSetting.carbon = util.convertToActor(data.Carbon);
                callback && callback();
            }
        });
    }

    AuditWindow.prototype.show = function (id, callback) {
        var $this = this,
            setting = $this.settings;
        if (id == $this.back || id == $this.sender) {
            $("#next_audit_user").hide();
        } else {
            var url = util.format(setting.showUrl, {
                instanceID: setting.instanceID,
                destination: id
            });
            util.ajaxWFService({
                url: url,
                type: 'get',
                dataType: 'text',
                success: function (nodeType) {
                    var type = nodeType.toLowerCase();
                    if ($.inArray(type, ['decision', 'start', 'fork', 'merge', 'end']) > -1) {
                        $("#next_audit_user").hide();
                    }
                    else {
                        $this.actorSelect(id);
                        $("#next_audit_user").show();
                    }
                }
            });
        }
    }

    AuditWindow.prototype._bindSelect = function () {
        var $this = this, setting = $this.settings,
            url = util.format(setting.select, {
                instanceID: setting.instanceID,
                actorID: $this.user.ID
            });

        util.ajaxWFService({
            url: url,
            type: 'get',
            success: function (serverData) {
                var htmlArray = [];
                $.each(serverData, function () {
                    htmlArray.push("<option value='" + this.Code + "'>" + this.Name + "</option>");
                });
                $(setting.selectOption).html(htmlArray.join(''));
                $this.show($(setting.selectOption).val());
                layui.form.render();
            }
        });
    };

    AuditWindow.prototype._initEvent = function () {
        var $this = this;
        $($this.settings.buttonId).click(function () {
            var data = layui.form.val('next_form'),
                message = data.message;
            var n = $this.nodeSetting;
            var result = $("#next_audit_user").is(":hidden");
            if (!result && n.group.length == 0 && n.actor.length == 0 && n.organization.length == 0) {
                alert('请选择审批人');
            } else if (util.isEmpty(message)) {
                $(this).attr("disabled", "disabled");
                $this.jump(message, data.to);
            } else {
                alert($this.settings.tips);
            }
        });

        var form = layui.form;
        form.on('select(operate)', function (data) {
            if (data.value === 'NID_REJECT_ID_80_11') {
                $("#next_audit_user").hide();
            } else {
                $this.show(data.value);
            }
        });

        $($this.settings.actorID).click(function () {
            $this.bindActor();
        });

        $($this.settings.carbonID).click(function () {
            parent.layer.close($this.ActorIndex);
            $this.ActorIndex = util.create({
                width: 900,
                height: 680,
                title: '抄送人',
                content: [util.carbon],
                cancel: function (index, dom) {
                    var frameContent = AuditWindow.getDOMFrame(dom);
                    frameContent.setting.set($this.nodeSetting);
                    var arr = ($this.nodeSetting.carbon || []);
                    if (arr.length > 0) {
                        var cc = [];
                        $.each(arr, function () {
                            cc.push(this.name);
                        });
                        $($this.settings.carbonID).text(cc.join('，'));
                    }
                },
                success: function (dom, index) {
                    var frameContent = AuditWindow.getDOMFrame(dom);
                    frameContent.setting.load($this.nodeSetting);
                }
            });
        });
    };

    AuditWindow.getDOMFrame = function (dom) {
        var frameId = dom.find("iframe").attr('id');
        return parent.document.getElementById(frameId).contentWindow;
    };

    AuditWindow.prototype.bindActor = function () {
        var $this = this;
        parent.layer.close($this.ActorIndex);
        $this.ActorIndex = parent.util.create({
            width: 900,
            height: 680,
            title: '参与者',
            content: [util.actor],
            cancel: function (index, dom) {
                var frameContent = AuditWindow.getDOMFrame(dom);
                frameContent.setting.set($this.nodeSetting);
            },
            success: function (dom, index) {
                var frameContent = AuditWindow.getDOMFrame(dom);
                frameContent.setting.load($this.nodeSetting);
            }
        });
    }

    AuditWindow.prototype.jump = function (message, transition) {
        var $this = this, url;
        var settings = this.settings,
            result = $this.user,
            param = JSON.stringify({
                instanceID: settings.instanceID,
                actorID: result.ID,
                message: message,
                data: {
                    CategoryCode: settings.code,
                    UUID: result.ID,
                    Name: result.Name,
                    Group: convertStrs($this.nodeSetting.group),
                    Actor: convertStrs($this.nodeSetting.actor),
                    Carbon: convertStrs($this.nodeSetting.carbon),
                    Organization: convertStrs($this.nodeSetting.organization)
                }
            });

        if (transition === $this.back) {
            url = "api/smf/back";
        }
        else if (transition === $this.sender) {
            url = "api/smf/sender";
        }
        else if (transition === $this.veto) {
            url = "api/smf/veto";
        }
        else {
            url = "api/smf/next";
        }
        util.ajaxWFService({
            url: url,
            data: param,
            dataType: 'text',
            success: function () {
                $this.close();
            }
        });
    }

    function convertStrs(arr) {
        var data = [];
        $.each(arr, function () {
            data.push(this.id);
        });

        return data.join(',');
    }

    factory(function (option) {
        return new AuditWindow(option);
    });

})(function (instance) {
    $.AuditWindow = instance;
});

