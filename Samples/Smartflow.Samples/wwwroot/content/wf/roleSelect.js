/********************************************************************
 License: https://github.com/chengderen/Smartflow/blob/master/LICENSE 
 Home page: http://www.smartflow-sharp.com
 ********************************************************************
 */
; (function (factory) {

    function loadGroup(nx) {
        var ajaxSettings = { url: 'api/setting/group/list', type: 'get' };
        ajaxSettings.data = ajaxSettings.data || {};

        ajaxSettings.success = function (serverData) {

            var leftDataSource = [], rightDataSource = [];

            $.each(serverData, function () {
                leftDataSource.push({
                    value: this.ID,
                    title: this.Name,
                    disabled: '',
                    checked: ''
                });
            });

            $.each(nx.group, function () {
                rightDataSource.push(this.id);
            });

            var transfer = layui.transfer;

            //基础效果
            transfer.render({
                elem: '#transfer'
                , title: ['待选择', '已选择']
                , data: leftDataSource
                , value: rightDataSource
                , height: 530
                , width: 391
                , id: 'rightGroup'
            });
        };

        util.ajaxWFService(ajaxSettings);
    }

    function setGroup(nx) {

        var transfer = layui.transfer,
            rightData = transfer.getData('rightGroup');

        var roleArray = [];

        $(rightData).each(function () {
            var self = this;
            roleArray.push({
                id: self.value,
                name: self.title
            });
        });

        nx.group = roleArray;
    }

    factory({
        load: loadGroup,
        set: setGroup
    });

})(function (option) {
    window.setting = option;
});