/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 ********************************************************************
 */
; (function (initialize) {

    function Transfer(option) {
        this.option = option;
        this.init();
        this.bind();
    }

    Transfer.prototype.set = function (nx) {
        var table = layui.table,
            cacheData = table.cache.right;
        var carbonArray = [];

        $(cacheData).each(function () {
            var self = this;
            carbonArray.push({
                id: self.ID,
                name: self.Name
            });
        });

        nx.carbon = carbonArray;
    }
    Transfer.prototype.load = function (rData) {
        var table = layui.table,
            $this = this,
            $opt = $this.option;
        var url = util.format($opt.right.url, {
            instanceID: $opt.instanceID,
            nodeID: $opt.destination
        });

        var actors = [];
        $.each(rData, function () {
            actors.push(this.ID);
        });

        util.table({
            elem: $opt.left.el
            , url: $opt.left.url
            , height: 'full-90'
            , page: { layout: ['prev', 'page', 'next', 'count'] }
            , where: {
                arg: JSON.stringify({ actor: actors.join(',') })
            }
            , cols: [[
                { checkbox: true, fixed: true }
                , { field: 'ID', title: 'ID', hide: true }
                , { field: 'Name', title: '用户名', width: 120, align: 'left' }
                , {
                    title: '部门名称', templet: function (d) {
                        return d.Data.OrgName;
                    }
                }
            ]]
        });
        util.table({
            elem: $opt.right.el
            , url: url
            , height: 'full-90'
            , page: false
            , method: 'get'
            , cols: [[
                { checkbox: true, fixed: true }
                , { field: 'ID', title: 'ID', hide: true }
                , { field: 'Name', title: '用户名', width: 120, align: 'center' }
                , { field: 'OrganizationName', title: '部门名称' }
            ]]
        });
        var ls = 'checkbox(' + $opt.left.filter + ')',
            rs = 'checkbox(' + $opt.right.filter + ')';
        var mulitSelector = [{ selector: ls, filter: 'left', id: 'div.arrow-right' }, { selector: rs, filter: 'right', id: 'div.arrow-left' }];
        $.each(mulitSelector, function () {
            var info = this;
            table.on(info.selector, function (obj) {
                var checkStatus = table.checkStatus(info.filter), data = checkStatus.data;
                var methodName = data.length > 0 ? 'removeClass' : 'addClass';
                $(info.id)[methodName]('layui-btn-disabled');
            });
        });
    }
    Transfer.prototype.init = function () {
        var $this = this, $opt = $this.option;
        var url = util.format($opt.right.url, { instanceID: $opt.instanceID, nodeID: $opt.destination });
        util.ajaxWFService({
            url: url,
            type: 'get',
            success: function (r) {
                $this.load(r.Data);
            }
        });

        util.ajaxWFService({
            type: 'get',
            url: $this.option.tree.url,
            success: function (serverData) {
                $this.renderTree(serverData);
            }
        });
    }
    Transfer.prototype.renderTree = function (data) {
        var $this = this,
            $opt = $this.option;
        $.fn.zTree.init($($opt.tree.el), {
            callback: $opt.tree.callback,
            data: {
                key: {
                    name: 'Name'
                },
                simpleData: {
                    enable: true,
                    idKey: 'ID',
                    pIdKey: 'ParentID',
                    rootPId: 0
                }
            }
        }, data);
    }
    Transfer.prototype.bind = function () {
        var $this = this,
            $opt = $this.option;
        for (let propertyName in $opt.event) {
            const selector = '#' + propertyName;
            $(selector).click(function (e) {
                $opt.event[propertyName].call($this, this, e);
            });
        }
    }
    initialize(function (option) {
        return new Transfer(option);
    });

})(function (createInstance) {

    var id = util.doQuery("id");
    var destination = util.doQuery("destination");
    var categoryCode = util.doQuery('categoryCode');
    var instance = createInstance({
        instanceID: id,
        destination: destination,
        categoryCode: categoryCode,
        left: {
            el: 'table.left',
            url: 'api/setting/actor/page',
            filter: 'left'
        },
        right: {
            el: 'table.right',
            url: 'api/setting/actor/{instanceID}/user/{nodeID}/list',
            filter: 'right'
        },
        tree: {
            el: 'div.ztree',
            url: 'api/setting/organization/list',
            callback: {
                onClick: function (event, treeId, node) {
                    $("input.node-text").val(node.Name);
                    $("input.node-value").val(node.ID);
                }
            }
        },
        event: {
            toLeft: function (o) {
                var $this = $(o),
                    $opt = this.option,
                    key = $("input.input-key").val();
                if (!$this.hasClass('layui-btn-disabled')) {
                    var checkStatus = layui.table.checkStatus($opt.right.filter);
                    removeData(checkStatus.data);
                    if (checkStatus.data.length > 0) {
                        removeData(checkStatus.data);
                        var cacheData = layui.table.cache.right;
                        var actors = [];
                        $.each(cacheData, function () {
                            actors.push(this.ID);
                        });
                        deletePending(checkStatus.data, function () {
                            var config = {
                                page: { curr: 1 },
                                where: {
                                    arg: JSON.stringify({ actor: actors.join(','), searchKey: key })
                                }
                            };
                            layui.table.reload($opt.left.filter, config);
                            layui.table.reload($opt.right.filter, { page: false });
                            $this.addClass('layui-btn-disabled');
                        });
                    }
                    $this.addClass('layui-btn-disabled');
                }

                function removeData(selectDataArray) {
                    var cacheData = layui.table.cache.right;
                    $.each(selectDataArray, function () {
                        for (var i = 0, len = cacheData.length; i < len; i++) {
                            var c = cacheData[i];
                            if (this.ID == c.ID) {
                                cacheData.splice(i, 1);
                                break;
                            }
                        }
                    });
                }

                function deletePending(data, callback) {
                    var actors = [];
                    $.each(data, function () {
                        actors.push(this.ID);
                    });
                    util.ajaxWFService({
                        url: 'api/setting/pending/delete',
                        type: 'post',
                        dataType: 'text',
                        data: JSON.stringify({
                            ID: id,
                            NodeID: destination,
                            ActorIDs: actors.join(',')
                        }),
                        success: function () {
                            callback && callback();
                        }
                    });
                }

            },
            toRight: function (o) {
                var $this = $(o),
                    $opt = this.option,
                    key = $("input.input-key").val();
                if (!$this.hasClass('layui-btn-disabled')) {
                    var checkStatus = layui.table.checkStatus($opt.left.filter), data = checkStatus.data;
                    if (checkStatus.data.length > 0) {
                        $.each(checkStatus.data, function () {
                            actors.push(this.ID);
                        });
                        addPending(checkStatus.data, function () {
                            var config = {
                                page: { curr: 1 },
                                where: { arg: JSON.stringify({ searchKey: key, actor: actors.join(',') }) }
                            };
                            layui.table.reload($opt.left.filter, config);
                            layui.table.reload($opt.right.filter, { page: false });
                        });
                    }
                    $this.addClass('layui-btn-disabled');
                }

                function addPending(data, callback) {
                    var actors = [];
                    $.each(data, function () {
                        actors.push(this.ID);
                    });
                    util.ajaxWFService({
                        url: 'api/setting/pending/persistent',
                        type: 'post',
                        dataType: 'text',
                        data: JSON.stringify({
                            ID: id,
                            NodeID: destination,
                            ActorIDs: actors.join(','),
                            CategoryCode: categoryCode
                        }),
                        success: function () {
                            callback && callback();
                        }
                    });
                }
            },
            reload: function () {
                var $this = this,
                    key = $("input.input-key").val(),
                    code = $("input.node-value").val(),
                    cacheData = layui.table.cache.right,
                    actors = [];

                $.each(cacheData, function () {
                    actors.push(this.ID);
                });

                var searchCondition = {
                    actor: actors.join(','),
                    orgCode: code,
                    searchKey: code
                };

                $('div.arrow-right').addClass('layui-btn-disabled');
                layui.table.reload($this.option.left.filter, {
                    page: { curr: 1 },
                    where: {
                        arg: JSON.stringify(searchCondition)
                    }
                });
            }
        }
    });

    window.setting = instance;
    $('div.zc-ztree').hover(function () { }, function () {
        $(this).hide();
    });
});
