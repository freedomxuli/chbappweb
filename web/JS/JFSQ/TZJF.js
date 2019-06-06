//

var pageSize = 15;
var cx_yhm;
//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'UserID' },
       { name: 'UserName' },
       { name: 'UserXM' },
       { name: 'zed' },
       { name: 'kyed' },
       { name: 'kkfed' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getList(nPage);
    }
});
//************************************数据源*****************************************
//专线名、总额度、可用额度、可开放额度、操作列
//************************************页面方法***************************************
function getList(nPage) {
    CS('CZCLZ.JFSQMag.GetTZList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_yhm").getValue(), Ext.getCmp("cx_xm").getValue());
}


function tz(id) {

    var r = store.findRecord("UserID", id).data;
    if (privilege("申请运费券_调整申请额度_编辑")) {

        if (r.kyed <= 0) {
            Ext.MessageBox.alert('提示', '暂无可扣除的运费券！');
        } else {
            var win = new addWin();
            win.show(null, function () {
                Ext.getCmp("UserID").setValue(id);
                Ext.getCmp("MaxPoint").setValue(r.kyed);

                var str = "可扣除额度:" + r.kyed;
                Ext.getCmp("zspoint").setText(str);
            });
        }
    }
}
//************************************页面方法***************************************

//************************************弹出界面***************************************
Ext.define('addWin', {
    extend: 'Ext.window.Window',

    height: 190,
    width: 400,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '扣除运费券',

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'form',
                id: 'addform',
                bodyPadding: 10,
                items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: 'ID',
                        id: 'UserID',
                        name: 'UserID',
                        hidden: true
                    },
                    {
                        xtype: 'numberfield',
                        id: 'MaxPoint',
                        name: 'MaxPoint',
                        hidden: true
                    },
                    {
                        xtype: 'numberfield',
                        fieldLabel: '分数',
                        id: 'points',
                        name: 'points',
                        allowBlank: false,
                        allowDecimals: false,
                        allowNegative: false,
                        minValue: 1,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textareafield',
                        fieldLabel: '备注',
                        anchor: '100%',
                        id: 'memo',
                        name: 'memo'
                    },
                     {
                         xtype: "label",
                         id: "zspoint",
                         text: "",
                         style: "padding-left:100px;"
                     }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '确定',
                        iconCls: 'dropyes',
                        handler: function () {
                            var point = Ext.getCmp("points").getValue();
                            if (point < 1) {
                                Ext.Msg.show({
                                    title: '提示',
                                    msg: '扣除的运费券必须大于1',
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.INFO
                                });
                                return;
                            }

                            var maxpoint = Ext.getCmp("MaxPoint").getValue();
                            if (point > maxpoint) {
                                Ext.Msg.show({
                                    title: '提示',
                                    msg: '扣除的运费券不得超过其所有运费券',
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.INFO
                                });
                                return;
                            }
                            var form = Ext.getCmp('addform');
                            if (form.form.isValid()) {
                                var values = form.form.getValues(false);
                                console.log(values);
                                var me = this;
                                CS('CZCLZ.JFSQMag.SaveKCYFQ', function (retVal) {
                                    if (retVal) {
                                        me.up('window').close();
                                        getList(1);
                                        Ext.MessageBox.alert('提示', '扣除成功！');
                                    }
                                }, CS.onError, values);

                            }
                        }
                    },
                     {
                         text: '取消',
                         iconCls: 'back',
                         handler: function () {
                             this.up('window').close();
                         }
                     }
                ]
            }
        ];
        me.callParent(arguments);
    }
});
//************************************弹出界面***************************************

//************************************主界面*****************************************
Ext.onReady(function () {
    Ext.define('JFSQView', {
        extend: 'Ext.container.Viewport',

        layout: {
            type: 'fit'
        },

        initComponent: function () {
            var me = this;
            me.items = [
                {
                    xtype: 'gridpanel',
                    id: 'JFSQGrid',
                    store: store,
                    
                    columnLines: 1,
                    border: 1,
                    columns: [Ext.create('Ext.grid.RowNumberer'),
                         {
                             xtype: 'gridcolumn',
                             dataIndex: 'UserXM',
                             sortable: false,
                             menuDisabled: true,
                             flex:1,
                             text: '物流名称'
                         },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'UserName',
                            sortable: false,
                            menuDisabled: true,
                            width: 100,
                            text: '用户名'
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'zed',
                            sortable: false,
                            menuDisabled: true,
                            width: 100,
                            text: '总额度'
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'kyed',
                            sortable: false,
                            menuDisabled: true,
                            width: 100,
                            text: '可用额度'
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'kkfed',
                            sortable: false,
                            menuDisabled: true,
                            width: 100,
                            text: '可开放额度'
                        },
                        {
                            xtype: 'gridcolumn',
                            sortable: false,
                            menuDisabled: true,
                            dataIndex: 'UserID',
                            text: '操作',
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                return  "<a href='JavaScript:void(0)' onclick='tz(\"" + value + "\")'>扣除</a>";
                            }
                        }
                    ],
                    viewConfig: {

                    },
                    dockedItems: [
                        {
                            xtype: 'toolbar',
                            dock: 'top',
                            items: [
                                {
                                    xtype: 'textfield',
                                    id: 'cx_xm',
                                    width: 140,
                                    labelWidth: 60,
                                    fieldLabel: '物流名称'
                                },
                                {
                                    xtype: 'textfield',
                                    id: 'cx_yhm',
                                    width: 140,
                                    labelWidth: 60,
                                    fieldLabel: '用户名'
                                },
                                {
                                    xtype: 'buttongroup',
                                    title: '',
                                    items: [
                                        {
                                            xtype: 'button',
                                            iconCls: 'search',
                                            text: '查询',
                                            handler: function () {
                                                if (privilege("申请运费券_调整申请额度_查看")) {
                                                    getList(1);
                                                }
                                            }
                                        }
                                    ]
                                }
                            ]
                        },
                        {
                            xtype: 'pagingtoolbar',
                            displayInfo: true,
                            store: store,
                            dock: 'bottom'
                        }
                    ]
                }
            ];
            me.callParent(arguments);
        }
    });

    new JFSQView();

    getList();
})
//************************************主界面*****************************************