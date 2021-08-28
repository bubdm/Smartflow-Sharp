/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 ********************************************************************
 */
; (function () {

    function set(nx) {

        var table = layui.table,
            cacheData = table.cache.table_right;
        var actorArray = [];

        $(cacheData).each(function () {
            var self = this;
            actorArray.push({
                id: self.ID,
                name: self.Name
            });
        });

        nx.actor = actorArray;
    }

    function load(rData, id, destination) {

        var table = layui.table;
        var url = util.format("api/setting/actor/{instanceID}/user/{nodeID}/list", {
            instanceID: id,
            nodeID: destination
        });
        var actors = [];
        $.each(rData, function () {
            actors.push(this.ID);
        });
        util.table({
            elem: '#table_left'
            , url: 'api/setting/actor/page'
            , method: 'post'
            , height: 569
            , page: {
                layout: ['prev', 'page', 'next']
            }
            , where: {
                arg: JSON.stringify({ actor: actors.join(',') })
            }
            , cellMinWidth: 80 //全局定义常规单元格的最小宽度，layui 2.2.1 新增
            , cols: [[
                { checkbox: true, fixed: true }
                , { field: 'ID', title: 'ID', hide: true }
                , { field: 'Name', title: '姓名', width: 120, align: 'left' }
                , {
                    title: '部门名称', templet: function (d) {
                        return d.Data.OrgName;
                    }
                }
            ]]
        });
        util.table({
            elem: '#table_right'
            , url: url
            , height: 569
            , cellMinWidth: 80 
            , page: false
            , method: 'get'
            , cols: [[
                { checkbox: true, fixed: true }
                , { field: 'ID', title: 'ID', hide: true }
                , { field: 'Name', title: '姓名', width: 120, align: 'center' }
                , { field: 'OrganizationName', title: '部门名称' }
            ]]
        });

        table.on('checkbox(table_left)', function (obj) {
            //左边表格点击触发
            var checkStatus = table.checkStatus('table_left'), data = checkStatus.data;
            var methodName = data.length > 0 ? 'removeClass' : 'addClass';
            $('#right_table_1')[methodName]('layui-btn-disabled');
        });
        table.on('checkbox(table_right)', function (obj) {
            //右边表格点击触发
            var checkStatus = table.checkStatus('table_right'), data = checkStatus.data;
            var methodName = data.length > 0 ? 'removeClass' : 'addClass';
            $('#left_table_1')[methodName]('layui-btn-disabled');
        });
    }

    window.setting = {
        load: load,
        set: set
    };
})();

$(function () {
    var id = util.doQuery("id");
    var destination = util.doQuery("destination");
    var categoryCode = util.doQuery('categoryCode');
    var url = util.format("api/setting/actor/{instanceID}/user/{nodeID}/list", {
        instanceID: id,
        nodeID: destination
    });
    util.ajaxWFService({
        url: url,
        type: 'get',
        success: function (r) {
            setting.load(r.Data, id, destination);
        }
    });

    $('#reload').on('click', function () {
        var key = $('#title').val();
        var cacheData = layui.table.cache.table_right;
        var actors = [];
        $.each(cacheData, function () {
            actors.push(this.ID);
        });
        $("#right_table_1").addClass('layui-btn-disabled');
        layui.table.reload('table_left', {
            page: { curr: 1 },
            where: {
                arg: JSON.stringify({ actor: actors.join(','), searchKey: key })
            }
        });
    });

    $("#left_table_1").click(function () {
        var $this = $(this);
        var key = $('#title').val();
        if (!$this.hasClass('layui-btn-disabled')) {
            var checkStatus = layui.table.checkStatus('table_right');
            if (checkStatus.data.length > 0) {
                removeData(checkStatus.data);
                var cacheData = layui.table.cache.table_right;
                var actors = [];
                $.each(cacheData, function () {
                    actors.push(this.ID);
                });
                deletePending(checkStatus.data, function () {
                    layui.table.reload('table_left', {
                        page: { curr: 1 },
                        where: {
                            arg: JSON.stringify({ actor: actors.join(','), searchKey: key })
                        }
                    });
                    layui.table.reload('table_right', { page: false });
                });
            }
            $this.addClass('layui-btn-disabled');
        }
    });

    $("#right_table_1").click(function () {
        var $this = $(this);
        var key = $('#title').val();
        if (!$this.hasClass('layui-btn-disabled')) {
            var checkStatus = layui.table.checkStatus('table_left');
            if (checkStatus.data.length > 0) {
                var cacheData = layui.table.cache.table_right;
                var actors = [];
                $.each(cacheData, function () {
                    actors.push(this.ID);
                });

                if (checkStatus.data.length > 0) {

                    $.each(checkStatus.data, function () {
                        actors.push(this.ID);
                    });
                    addPending(checkStatus.data, function () {
                        layui.table.reload('table_left', {
                            page: { curr: 1 },
                            where: {
                                arg: JSON.stringify({ actor: actors.join(','), searchKey: key })
                            }
                        });
                        layui.table.reload('table_right', { page: false });
                    });
                }
            }

            $this.addClass('layui-btn-disabled');
        }
    });

    function deletePending(data, callback) {
        var actors = [];
        $.each(data, function () {
            actors.push(this.ID);
        });
        util.ajaxWFService({
            url: 'api/setting/pending/delete',
            type: 'post',
            dataType:'text',
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

    function removeData(selectDataArray) {
        var cacheData = layui.table.cache.table_right;
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
});