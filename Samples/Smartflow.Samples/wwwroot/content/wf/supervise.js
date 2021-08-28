/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 ********************************************************************
 */
$(function () {

    function Page(option) {
        this.setting = $.extend({}, option);
        this.init();
    }

    Page.prototype.init = function () {
        this.bind();
        this.load();
    }

    Page.prototype.bind = function () {
        var $this = this;
        $.each($this.setting.event, function (propertyName) {
            var selector = '#' + propertyName;
            $(selector).click(function () {
                $this.setting.event[propertyName].call($this);
            });
        });
    }

    Page.prototype.load = function () {
        var $this = this;
        var config = this.setting.config;
        var selector = '#' + config.id;
        util.table({
            elem: selector
            , toolbar: '#list-bar'
            , url: config.url
            , cols: [[
                { type: 'radio' }
                , { width: 50, type: 'numbers', sort: false, title: '序号', align: 'center', unresize: true }
                , { field: 'CategoryName', width: 120, title: '业务类型', sort: false, align: 'center' }
                , { field: 'OrganizationName', width: 130, title: '所属单位', sort: false, align: 'center' }
                , { field: 'Name', width: 120, title: '创建人', sort: false, align: 'center' }
                , {
                    field: 'Comment', title: '标题', minWidth: 140, align: 'left', event: 'jump', templet: function (d) {
                        return "<span class=\"jump-click\">" + d.Comment + "</span>";
                    }
                }
                , {
                    field: 'CreateTime', title: '创建时间', width: 200, align: 'center',
                    templet: function (d) { return layui.util.toDateString(d.CreateTime, 'yyyy.MM.dd HH:mm:ss'); }
                }
                , { field: 'StateName', width: 100, title: '运行状态', align: 'center', unresize: true }
                , {
                    field: 'NodeName', minWidth: 180, title: '执行节点', align: 'left', unresize: true, templet: function (d) {
                        if (d.State.toLowerCase() == 'reject') {
                            return d.NodeName + '(否决)';
                        } else {
                            return d.NodeName;
                        }
                    }
                }
            ]]
        });
        layui.table.on('tool(' + config.id + ')', function (obj) {
            var data = obj.data;
            var eventName = obj.event;
            if (eventName === 'jump') {
                $this.jump(data);
            }
        });
        layui.table.on('toolbar(' + config.id + ')', function (obj) {
            var data = obj.data;
            var eventName = obj.event;
            $this.setting.methods[eventName].call($this);
        });
    }

    Page.prototype.jump = function (obj) {
        var config = this.setting.config;
        var url = util.format(config.category, { id: obj.CategoryCode });
        util.ajaxWFService({
            url: url,
            type: 'get',
            success: function (data) {
                util.openDetailFullLayer(data.Url, {
                    id: obj.Key,
                    code: obj.CategoryCode,
                    instanceID: obj.InstanceID
                }, obj.CategoryName);
            }
        });
    }

    Page.prototype.delete = function (data) {
        var $this = this,
            config = $this.setting.config;
        var url = util.format(config.delete, { instanceID: data.InstanceID, categoryCode: data.CategoryCode, id: data.Key });

        util.ajaxWFService({
            url: url,
            type: 'post',
            dataType: 'text',
            success: function () {
                $this.refresh();
            }
        });
    }

    Page.prototype.reboot = function (data) {
        var $this = this,
            config = $this.setting.config;

        var url = util.format(config.reboot, {
            instanceID: data.InstanceID,
            id: data.Key,
            categoryCode: data.CategoryCode
        });

        util.ajaxWFService({
            url: url,
            type: 'post',
            dataType: 'text',
            success: function () {
                $this.refresh();
            }
        });
    }

    Page.prototype.refresh = function () {
        layui.table.reload(this.setting.config.id);
    }

    Page.check = function (id, callback) {
        var checkStatus = layui.table.checkStatus(id);
        var dataArray = checkStatus.data;
        if (dataArray.length == 0) {
            layer.msg(util.message.record);
        } else {
            var data = dataArray[0];
            callback && callback(data);
        }
    }

    var page = new Page({
        config: {
            id: 'supervise-table',
            url: 'api/setting/summary/supervise/page',
            delete: 'api/setting/summary/{instanceID}/{categoryCode}/delete/{id}',
            category: 'api/setting/category/{id}/info',
            reboot: 'api/smf/{instanceID}/{categoryCode}/reboot/{id}'
        },
        methods: {
            change: function () {
                Page.check('supervise-table', function (data) {
                    if (data.State.toLowerCase() === 'running') {
                        util.openWin('./uc.html?id=' + data.InstanceID + "&categoryCode=" + data.CategoryCode + "&destination=" + data.NodeID, data.CategoryName, 895, 660);
                    } else {
                        layer.msg(util.message.running);
                    }
                });
            },
            kill: function () {
                var $this = this;
                Page.check('supervise-table', function (data) {
                    if (data.State.toLowerCase() === 'running') {
                        util.confirm(util.message.kill, function () {
                            var url = util.format("api/smf/{categoryCode}/{instanceID}/kill/{destination}", { instanceID: data.InstanceID, categoryCode: data.CategoryCode, destination: data.NodeID });
                            util.ajaxWFService({
                                type: 'post',
                                url: url,
                                dataType: 'text',
                                success: function () {
                                    $this.refresh();
                                }
                            });
                        });
                    } else {
                        layer.msg(util.message.running);
                    }
                });
            },
            delete: function () {
                var $this = this;
                Page.check('supervise-table', function (data) {
                    util.confirm(util.message.delete, function () {
                        $this.delete(data);
                    });
                });
            },
            reboot: function () {
                var $this = this;
                Page.check('supervise-table', function (data) {
                    if (data.State.toLowerCase() === 'running') {
                        util.confirm('重新发起流程？', function () {
                            $this.reboot(data);
                        });
                    } else {
                        layer.msg(util.message.running);
                    }
                });
            }
        },
        event: {

        }
    });

    window.invoke = function () {
        page.refresh();
    }
});
