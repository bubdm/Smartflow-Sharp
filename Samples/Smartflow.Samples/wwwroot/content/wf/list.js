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
        this.loadCategory();
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
            ,toolbar:'#list-bar'
            , url: config.url
            , cols: [[
                { type: 'radio' }
                , { width: 60, type: 'numbers', sort: false, title: '序号', align: 'center', unresize: true }
                , { field: 'Name', width: 240, title: '名称', align: 'left' }
                , { field: 'CategoryName', width: 100, title: '业务类型', sort: false, align: 'center' }
                , { field: 'Status', width: 120, title: '状态', align: 'center', templet: config.templet.checkbox, unresize: true }
                , { field: 'Memo', title: '备注', minWidth: 120, align: 'left' }
            ]]
        });

        layui.form.on(config.checkbox, function (obj) {
            var id = $(obj.elem).attr('code');
            var useState = (obj.elem.checked ? 1 : 0);
            var url = util.format("api/setting/structure/{id}/update/{status}", { id: id, status: useState });
            util.ajaxWFService({
                type: 'post',
                url: url,
                dataType:'text',
                success: function () {
                    layui.table.reload(config.id);
                }
            });
        });

        layui.table.on('toolbar(' + config.id + ')', function (obj) {
            var data = obj.data;
            var eventName = obj.event;
            $this.setting.methods[eventName].call($this);
        });
    }

    Page.prototype.loadCategory = function () {
        var url = this.setting.config.categoryUrl,
            id = '#' + this.setting.config.categoryId;
        util.ajaxWFService({
            url: url,
            type: 'GET',
            success: function (serverData) {
                var treeObj = $.fn.zTree.init($(id), {
                    callback: {
                        beforeClick: function (id, node) {
                            return !node.isParent;
                        },
                        onClick: function (event, id, node) {
                            $("#hidCategoryCode").val(node.NID);
                            $("#txtCategoryName").val(node.Name);
                        },
                        onDblClick: function () {
                            $("#hidCategoryCode").val(node.NID);
                            $("#txtCategoryName").val(node.Name);
                            $("#zc").hide();
                        }
                    },
                    data: {
                        key: {
                            name: 'Name'
                        },
                        simpleData: {
                            enable: true,
                            idKey: 'NID',
                            pIdKey: 'ParentID',
                            rootPId: 0
                        }
                    }
                }, serverData);
                var nodes = treeObj.getNodesByFilter(function (node) { return node.level == 0; });
                if (nodes.length > 0) {
                    treeObj.expandNode(nodes[0]);
                }
            }
        });
    }

    Page.prototype.refresh = function () {
        layui.table.reload(this.setting.config.id);
    }

    Page.prototype.search = function (searchCondition) {
        layui.table.reload(this.setting.config.id, searchCondition);
    }

    Page.prototype.delete = function (id) {
        var $this = this;
        var url = util.format(this.setting.config.delete, { id: id});
        util.ajaxWFService({
            url: url,
            dataType: 'text',
            type: 'post',
            success: function () {
                layer.closeAll();
                $this.refresh();
            }
        });
    }

    Page.check = function (id, callback) {
        var checkStatus = layui.table.checkStatus(id);
        var dataArray = checkStatus.data;
        if (dataArray.length == 0) {
            layer.msg('请选择记录');
        } else {
            callback && callback(dataArray[0]);
        }
    }

    Page.open = function (url) {
        var h = window.screen.availHeight;
        var w = window.screen.availWidth;
        window.open(url, '流程设计器', "width=" + w + ", height=" + h + ",top=0,left=0,titlebar=no,menubar=no,scrollbars=yes,resizable=yes,status=yes,toolbar=no,location=no");
    }

    var page = new Page({
        config: {
            id: 'struct-table',
            url: 'api/setting/structure/query/page',
            templet: {
                checkbox: '#struct-col-checkbox'
            },
            checkbox: 'checkbox(form-status)', 
            delete: 'api/setting/structure/{id}/delete',
            categoryUrl: 'api/setting/category/list',
            categoryId: 'ztree'
        },
        methods: {
            add: function () {
                Page.open('./design.html');
            },
            edit: function () {
                Page.check('struct-table', function (data) {
                    Page.open('./design.html?id=' + data.NID);
                });
            },
            delete: function () {
                var $this = this;
                Page.check('struct-table', function (data) {
                    util.confirm(util.message.delete, function () {
                        $this.delete(data.NID);
                    });
                });
            }
        },
        event: {
            search: function () {
                var searchCondition = layui.form.val('form-search');
                var config = {
                    page: { curr: 1 },
                    where: {
                        arg: JSON.stringify(searchCondition)
                    }
                };
                this.search(config);
            }
        }
    });

    window.invoke = function () {
        page.refresh();
    }
});
