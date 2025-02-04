﻿/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 ********************************************************************
 */
(function (initialize) {

    function Tree(option) {
        this.option = option;
    }

    Tree.prototype.load = function (nx) {
        var $opt = this.option;
        var ajaxSettings = { url: $opt.url, type: 'get' };
        ajaxSettings.data = ajaxSettings.data || {};
        ajaxSettings.success = function (serverData) {
            var treeObj = $.fn.zTree.init($($opt.el), {
                check: {
                    enable: true
                },
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
            }, serverData);

            treeObj.expandAll(true);

            $.each(nx.organization, function () {
                var node = treeObj.getNodeByParam("ID", this.id, null);
                treeObj.checkNode(node, true, true);
            });
        };
        util.ajaxWFService(ajaxSettings);
    }

    Tree.prototype.set = function (nx) {
        var treeObj = $.fn.zTree.getZTreeObj("ztree");
        var nodes = treeObj.getCheckedNodes(true);
        var selectNodes = [];
        $.each(nodes, function () {
            if (!this.isParent) {
                selectNodes.push({ id: this.ID, name: this.Name });
            }
        });
        nx.organization = selectNodes;
    }

    initialize(function (option) {
        return new Tree(option);
    });

})(function (createInstance) {
    window.setting = createInstance({
        url: 'api/setting/organization/list',
        el: '#ztree'
    });
});