/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 ********************************************************************
 */
; (function (initialize) {

    function Condition(option) {
        this.option = option;
    }

    Condition.prototype.load = function (nx) {
        var form = layui.form, $this = this;
        var $opt = $this.option;

        $this.loadDataSource(function () {
            if (nx.command) {
                $($opt.selectEl).val(nx.command.id);
                $($opt.selectEl).val(nx.command.text);
                form.render('select');
            }
        });

        Condition.dynamicGenControl(nx);
    }

    Condition.prototype.set = function (nx) {
        var form = layui.form.val('form-decision');
        nx.command = $.extend(nx.command || {}, form);
        Condition.dynamicGetControl(nx);
    }

    Condition.prototype.loadDataSource = function (callback) {
        var $opt = this.option;
        util.ajaxWFService({
            url: $opt.url,
            type: 'GET',
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

    Condition.dynamicGenControl = function (nx) {
        var LC = nx.getTransitions();
        if (LC.length > 0) {
            var template = document.getElementById("common_expression").innerHTML,
                ele = [];
            $.each(LC, function (i) {
                ele.push(template.replace(/{{name}}/, this.name)
                    .replace(/{{expression}}/, this.expression)
                    .replace(/{{id}}/, this.$id)
                );
            });
            $("#form_expression").html(ele.join(''));
            layui.form.render(null, 'form_expression');
        }
    }

    Condition.dynamicGetControl = function (nx) {
        var controls = $("#form_expression").find("textarea");
        $.each(controls, function () {
            var input = $(this);
            nx.set({
                id: input.attr("name"),
                expression: input.val()
                    .replace(/\r\n/g, ' ')
                    .replace(/\n/g, ' ')
                    .replace(/\s/g, ' ')
            });
        });
    }

    initialize(function (option) {
        return new Condition(option);
    });

})(function (createInstance) {
    window.setting = createInstance({
        url: 'api/setting/database-source/list',
        selectEl: '#decision_select'
    });
});