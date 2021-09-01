/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 ********************************************************************
 */
(function (initialize) {

    Configuration.controlSelectors = {
        node_veto: {
            type: 'checkbox',
            parse: function (id) {
                return $.SMF.getNodeById(id);
            },
            invoke: function (nx) {
                var veto = parseInt(nx.veto, 10);
                $('#node_veto').prop('checked', veto === 1);
            }
        },
        node_back_select: {
            type: 'select',
            parse: function (id) {
                return $.SMF.getNodeById(id);
            },
            invoke: function (nx) {
                layui.form.val('form_node',{ node_back_select: nx.back });
            }
        },
        node_action: {
            title: '自定义动作',
            type: 'box',
            width: '900px',
            height: '680px',
            url: './actionSelect.html',
            parse: function (id) {
                return $.SMF.getNodeById(id);
            }
        },
        node_carbon: {
            title: '抄送',
            type: 'box',
            width: '900px',
            height: '680px',
            url: './userSelect.html',
            parse: function (id) {
                return $.SMF.getNodeById(id);
            }
        },
        node_actor: {
            title: '参与者',
            type: 'box',
            width: '900px',
            height: '680px',
            url: './actorSelect.html',
            parse: function (id) {
                return $.SMF.getNodeById(id);
            }
        },
        node_extra: {
            field: 'extra',
            parse: function (id) {
                return $.SMF.getNodeById(id);
            },
            invoke: function (nx) {
                $('#node_extra').val(nx.extra);
            }
        },
        node_name: {
            field: 'name',
            parse: function (id) {
                return $.SMF.getNodeById(id);
            },
            invoke: function (nx) {
                $('#node_name').val(nx.name);
            }
        }
    };

    Configuration.findElementById = function (o) {
        var id = $(o).attr('id');
        var controlSelector = Configuration.controlSelectors[id];
        return {
            element: this.element,
            descriptor: controlSelector
        };
    };

    Configuration.show = function (settings) {
        $.each(settings, function (i, propertyName) {
            var selector = '#' + propertyName;
            $(selector).show();
            $(selector).siblings().each(function () {
                $(this).hide();
            });
        });
    };

    Configuration.getDOMFrame = function (dom) {
        var frameId = dom.find("iframe").attr('id');
        return document.getElementById(frameId).contentWindow;
    };

    Configuration.open = function (nx, descriptor) {
        var settings = {
            type: 2,
            title: descriptor.title,
            area: [descriptor.width, descriptor.height],
            shade: 0.8,
            closeBtn: 1,
            shadeClose: false,
            content: [descriptor.url, 'no']
        };

        if (!!descriptor.btn) {
            settings.btn = descriptor.btn;
        }

        if (!!descriptor.yes) {
            settings.yes = function (index,dom) {
                descriptor.yes.call(this, nx, index, dom);
            }
        }

        settings.cancel = function (index, dom) {
            var frameContent = Configuration.getDOMFrame(dom);
            frameContent.setting.set(nx);
            if (descriptor.invoke) {
                descriptor.invoke(nx);
            }
        };

        settings.success = function (dom, index) {
            var frameContent = Configuration.getDOMFrame(dom);
            frameContent.setting.load(nx);
        };

        layer.open(settings);
    };

    function Configuration(option) {
        this.option = $.extend({}, option);
        this.init();
        this.bind();
    }

    Configuration.prototype.init = function () {
        var $this = this;
        var id = util.doQuery('id');
        $("#drawing").SMF({
            container: this.option.container,
            dblClick: function (nx) {
                $this.element = nx;
                $this.selectTab.call($this, nx);
            }
        });

        if (id) {
            var url = util.format($this.option.url, { id: id});
            util.ajaxWFService({
                url: url,
                type: 'Get',
                success: function (serverData) {
                    $.SMF.getComponentById($this.option.container)
                        .import(serverData.Resource);
                }
            });
        } else {
            $.each(['start', 'end'], function (i, value) {
                $.SMF
                    .getComponentById($this.option.container)
                    .create(value, false);
            });
        }

        //渲染所有表单
        layui.form.render();
    };

    Configuration.prototype.bind = function () {
        var $this = this;
        for (var propertyName in Configuration.controlSelectors) {
            var selector = '#' + propertyName,
                sel = Configuration.controlSelectors[propertyName];
            if (sel.type === 'checkbox') {
                $(selector).click(function () {
                    var result = Configuration.findElementById.call($this, this);
                    result.element.veto = $(this).is(':checked') ? 1 : 0;
                });
            } else if (sel.type === 'box') {
                $(selector).click(function () {
                    var result = Configuration.findElementById.call($this, this);
                    Configuration.open(result.element, result.descriptor);
                });
            }
            else if (propertyName == 'node_back_select') {
                layui.form.on('select(node_back_select)', function (data) {
                    var result = Configuration.findElementById.call($this, $("#node_back_select"));
                    result.element.back = data.value;
                });
            }
            else {
                $(selector).keyup(function () {
                    var result = Configuration.findElementById.call($this, this);
                    var text = $(this).val();
                    if (result.descriptor.field === "extra") {
                        result.element.extra = text;
                    } else {
                        result.element.name = text;
                        if (result.element.brush) {
                            result.element.brush.text(text);
                        }
                    }
                });
            }
        }
        this.bindConstraint();
    };

    Configuration.prototype.selectTab = function (nx) {
        var $this = this,
            category = nx.category.toLowerCase();
        if (category === 'node') {
            Configuration.show([$this.option.node]);
            $this.loadConstraint(function () {
                $.each(nx.rule, function () { $('#' + this.id).attr("checked", true); });
                layui.form.render(null, $this.option.constraintID);
            });
            var controls = ['node_veto', 'node_name','node_extra', 'node_carbon','node_back_select'];
            $.each(controls, function (i, propertyName) {
                if (Configuration.controlSelectors[propertyName].invoke) {
                    Configuration.controlSelectors[propertyName].invoke(nx, $this);
                }
            });
        } else {
            Configuration.show([$this.option.help]);
            if (category === 'line') {
                layer.prompt({
                    formType: 0,
                    value: nx.name,
                    title: '跳转',
                    btn: ['确定']
                }, function (value, index, elem) {
                    nx.name = value;
                    layer.close(index);
                });
            }
            else if (category === 'form') {
                Configuration.open(nx, {
                    title: '属性',
                    width: '400px',
                    height: '220px',
                    url: './form.html',
                    btn: ['确定'],
                    yes: function (nx, index, dom) {
                        var frameContent = Configuration.getDOMFrame(dom);
                        frameContent.setting.set(nx);
                        layer.close(index);
                    }
                });
            }
            else if (category === 'end') {
                Configuration.open(nx, {
                    title: '自定义动作',
                    width: '900px',
                    height: '680px',
                    url: './actionSelect.html'
                });
            }
            else if (category === 'dynamic') {
                Configuration.open(nx, {
                    title: '自定义动作',
                    width: '900px',
                    height: '680px',
                    url: './actionSelect.html'
                });
            }
            else if (category === 'decision') {
                Configuration.open(nx, {
                    title: '属性',
                    width: '900px',
                    height: '680px',
                    url: './decision.html'
                });
            }
        }
    };

    Configuration.prototype.prompt = function (elementId, instance) {
        var ht = $("#" + elementId).html(),
            $this = this;

        layer.open({
            title: '流程信息',
            type: 1,
            closeBtn: 1,
            area: ['520px', '350px'],
            anim: 2,
            shadeClose: false,
            content: ht,
            btnAlign: 'c',
            btn: ['确定'],
            success: function (dom, index) {
                var form = layui.form;
                $this.loadCategory(function () {
                    form.render(null, 'layui_flow_info');
                    var id = util.doQuery('id');
                    if (id) {
                        $this.set(id);
                    }
                });
            },
            yes: function (index, dom) {
                var form = layui.form,
                    formData = form.val('layui_flow_info'),
                    id = util.doQuery('id');
                if (id) {
                    formData.NID = id;
                }

                formData.Resource = instance.export();
                util.ajaxWFService({
                    dataType:'text',
                    url: $this.option.save,
                    data: JSON.stringify(formData),
                    success: function () {
                        layer.closeAll();
                        if (window.opener && window.opener.invoke) {
                            window.opener.invoke();
                        }
                        window.close();
                    }
                });
            }
        });
    }

    Configuration.prototype.set = function (id) {
        var url = util.format(this.option.url, { id: id });
        var $this = this;
        util.ajaxWFService({
            url: url,
            type: 'Get',
            success: function (serverData) {
                var form = layui.form;
                $this.select(serverData.CategoryCode, 'ztree');
                form.val('layui_flow_info', serverData);
            }
        });
    }

    Configuration.prototype.select = function (id, treeId) {
        var treeObj = $.fn.zTree.getZTreeObj(treeId);
        var nodes = treeObj.getNodesByParam("NID", id, null);
        if (nodes.length > 0) {
            var n = nodes[0];
            treeObj.selectNode(n);
            $('#txtCateName').val(n.Name);
        }
    }

    Configuration.prototype.loadCategory = function (callback) {
        var url = this.option.categoryUrl,
            id = '#' + this.option.categoryId;
        util.ajaxWFService({
            url: url,
            type: 'GET',
            success: function (serverData) {
                var treeObj = $.fn.zTree.init($(id), {
                    beforeClick: function (id, node) {
                        return !node.isParent;
                    },
                    callback: {
                        onClick: function (event, id, node) {
                            $("#hidCategoryCode").val(node.NID);
                            $("#txtCategoryName").val(node.Name);
                        },
                        onDblClick: function (event, id, node) {
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
                callback && callback();
            }
        });
    }

    Configuration.prototype.loadConstraint = function (callback) {
        var $this = this,
            url = $this.option.constraintUrl,
            id = '#' + $this.option.constraintID;
        util.ajaxWFService({
            url: url,
            type: 'GET',
            success: function (serverData) {
                var htmlArray = [];
                $.each(serverData, function () {
                    htmlArray.push("<div class=\"layui-form-item\" style=\"margin-bottom:0\"><input type=\"checkbox\" id=\"" + this.NID + "\" name=\"" + this.NID + "\" title =\"" + this.Name + "\" /></div>");
                });

                $(id).html(htmlArray.join(''));
                layui.form.render(null, $this.option.constraintID);
                callback && callback();
            }
        });
    }

    Configuration.prototype.bindConstraint = function () {
        var $this = this;
        layui.form.on('checkbox', function (data) {
            var nx = $this.element,
                formData = layui.form.val($this.option.constraintID);
            nx.rule.length = 0;
            for (var property in formData) {
                var title = $('#' + property).attr('title');
                nx.rule.push({
                    id: property,
                    name: title
                });
            }
        });
    }

    Configuration.prototype.save = function () {
        var instance = $.SMF.getComponentById(this.option.container);
        if (instance.validate()) {
            this.prompt('smartflow_s_info', instance);
        } else {
            alert('流程图不符合流程定义规则');
        }
    }

    initialize(function (option) {
        return new Configuration(option);
    });

})(function (createInstance) {
    window.design = {
        Configuration: createInstance
    };
});