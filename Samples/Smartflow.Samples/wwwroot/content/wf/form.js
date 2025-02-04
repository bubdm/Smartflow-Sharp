﻿/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 ********************************************************************
 */
; (function (initialize) {

    function Form(option) {
        this.option = option;
    }

    Form.prototype.load = function (nx) {
        var $opt = this.option;
        var form = layui.form;
        form.val($opt.el, {
            name: nx.name,
            url: nx.url
        });
    }

    Form.prototype.set = function (nx) {
        var $opt = this.option;
        var form = layui.form.val($opt.el);
        nx.name = form.name;
        nx.url = form.url;
        if (nx.brush) {
            nx.brush.text(nx.name);
        }
    }

    initialize(function (option) {
        return new Form(option);
    });

})(function (createInstance) {
    window.setting = createInstance({
        el: 'form-bus'
    });
});