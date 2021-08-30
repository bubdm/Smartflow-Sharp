/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 ********************************************************************
 */
; (function (factory) {

    factory({
        load: function (nx) {
            var form = layui.form;
            form.val('form-bus', {
                name: nx.name,
                url: nx.url
            });
        },
        set: function (nx) {
            var form = layui.form.val('form-bus');
            nx.name = form.name;
            nx.url = form.url;
            if (nx.brush) {
                nx.brush.text(nx.name);
            }
        }
    });

})(function (option) {
    window.setting = option;
});