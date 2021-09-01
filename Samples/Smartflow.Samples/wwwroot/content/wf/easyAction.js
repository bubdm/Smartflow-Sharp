/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 ********************************************************************
 */
; (function (initialize) {

    function EasyAction(option) {
        this.option = option;
    }

    EasyAction.prototype.load = function (nx) {
        var $opt = this.option,
            form = layui.form;
        this.loadDataSource(function () {
            if (nx.command) {
                $($opt.selectEl).val(nx.command.id);
                form.render('select');
            }
        });
    }

    EasyAction.prototype.loadDataSource = function (callback) {
        var $opt = this.option;
        util.ajaxWFService({
            url: $opt.url,
            type: 'get',
            success: function (serverData) {
                var htmlArray = [];
                htmlArray.push("<option value=\"\"></option>");
                $.each(serverData, function () {
                    htmlArray.push("<option value='" + this.ID + "'>" + this.Name + "</option>");
                });

                $($opt.selectEl).html(htmlArray.join(''));
                layui.form.render('select');
                callback && callback();
            }
        });
    }

    EasyAction.prototype.set = function (nx) {
        var $opt = this.option, form = layui.form.val($opt.el);

        nx.command = $.extend(nx.command || {}, form);
    }

    initialize(function (option) {
        return new EasyAction(option);
    });

})(function (createInstance) {
    window.setting = createInstance({
        el: 'form_normal',
        selectEl:'#node_normal_select',
        url: 'api/setting/database-source/list',
    });
});