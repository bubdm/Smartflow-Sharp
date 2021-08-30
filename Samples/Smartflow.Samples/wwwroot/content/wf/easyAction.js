/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 ********************************************************************
 */
; (function (factory) {

    function loadAction(nx) {


        var form = layui.form;
        loadDataSource(function () {
            if (nx.command) {
                $("#node_normal_select").val(nx.command.id);
                form.render('select');
                $("#node_normal_script").val(nx.command.text);
            }
        });
    }

    function loadDataSource(callback) {
        util.ajaxWFService({
            url: 'api/setting/database-source/list',
            type: 'get',
            success: function (serverData) {
                var htmlArray = [];

                htmlArray.push("<option value=\"\"></option>");
                $.each(serverData, function () {
                    htmlArray.push("<option value='" + this.ID + "'>" + this.Name + "</option>");
                });

                $('#node_normal_select').html(htmlArray.join(''));
                layui.form.render(null, 'form_normal');
                callback && callback();
            }
        });
    }

    factory({
        load: loadAction,
        set: function (nx) {
            var form = layui.form.val('form_normal');
            nx.command = $.extend(nx.command || {}, form);
        }
    });

})(function (option) {
    window.setting = option;
});