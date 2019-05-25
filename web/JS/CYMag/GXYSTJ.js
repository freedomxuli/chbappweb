//-----------------------------------------------------------全局变量-----------------------------------------------------------------

//-----------------------------------------------------------数据源-------------------------------------------------------------------
var gxysStore = Ext.create('Ext.data.Store', {
    fields: ['UserXM', 'YK_SY']
});
//-----------------------------------------------------------页面方法-----------------------------------------------------------------
function DataBind() {
    var mc = Ext.getCmp('cx_userxm').getValue();
    var dq = Ext.getCmp('cx_dqmc').getValue();

    CS('CZCLZ.YKMag.GetGxysTj', function (retVal) {
        console.log(retVal);
        gxysStore.loadData(retVal.dt);
        Ext.getCmp('tj').setText("干线运输各个专线的总账剩余：" + retVal.gxye);
    }, CS.onError, mc, dq, 0);
}

function ShowLine() {
    var win = new lineWin();
    win.show(null, function () {
        
    })
}

//-----------------------------------------------------------弹出界面-----------------------------------------------------------------
Ext.define('lineWin', {
    extend: 'Ext.window.Window',

    height: document.documentElement.clientHeight,
    width: document.documentElement.clientWidth,
    layout: {
        type: 'fit'
    },
    title: '专线油卡明细记录',
    modal: true,

    initComponent: function () {
        var me = this;
        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'tabpanel',
                    activeTab: 1,
                    items: [
                        {
                            xtype: 'panel',
                            layout: {
                                type: 'fit'
                            },
                            hidden: true,
                            items: [
                                {
                                    xtype: 'gridpanel',
                                    columnLines: 1,
                                    border: 1,
                                    store: xszbstore,
                                    columns: [
                                        {
                                            xtype: 'datecolumn',
                                            dataIndex: 'rq',
                                            format: 'Y年m月d日',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 150,
                                            text: '日期'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'gqje',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '期初金额'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'wsyje',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '油卡划拨',
                                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                                var str;
                                                str = "<a onclick='CKXSMX(\"" + userId + "\",\"" + record.data.rq + "\");'>" + value + "</a>";
                                                return str;
                                            }
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'zje',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '油卡消耗',
                                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                                var str;
                                                str = "<a onclick='CKXSMX(\"" + userId + "\",\"" + record.data.rq + "\");'>" + value + "</a>";
                                                return str;
                                            }
                                        },
                                        {
                                            text: '剩余金额',
                                            dataIndex: 'zje',
                                            width: 250,
                                            sortable: false,
                                            menuDisabled: true
                                        }
                                    ],
                                    dockedItems: [
                                        {
                                            xtype: 'toolbar',
                                            dock: 'top',
                                            items: [
                                                {
                                                    xtype: 'textfield',
                                                    id: 'cx_userxm',
                                                    width: 160,
                                                    labelWidth: 60,
                                                    fieldLabel: '专线'
                                                },
                                                {
                                                    xtype: 'textfield',
                                                    id: 'cx_dqmc',
                                                    width: 160,
                                                    labelWidth: 60,
                                                    fieldLabel: '年份'
                                                }
                                            ]
                                        }
                                    ]
                                }
                            ]
                        }

                    ],
                    buttonAlign: 'center',
                    buttons: [
                        {
                            text: '关闭',
                            handler: function () {
                                me.close();
                            }
                        }
                    ]
                }
            ]
        });

        me.callParent(arguments);
    }

});
//-----------------------------------------------------------界    面-----------------------------------------------------------------
Ext.define('myView', {
    extend: 'Ext.container.Viewport',

    layout: {
        type: 'fit'
    },

    initComponent: function () {
        var me = this;
        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'gridpanel',
                    store: gxysStore,
                    columnLines: true,
                    columns: [
                        Ext.create('Ext.grid.RowNumberer'),
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'UserXM',
                            sortable: false,
                            menuDisabled: true,
                            text: "专线名称",
                            flex: 1
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'YK_SY',
                            sortable: false,
                            menuDisabled: true,
                            text: "油卡剩余金额",
                            flex: 1,
                            renderer: function (v, s, r) {
                                return '<a href="javascript:void(0);" onclick="ShowLine(\'' + v + '\')">' + v + '</a>';
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
                                    id: 'cx_userxm',
                                    width: 160,
                                    labelWidth: 60,
                                    fieldLabel: '专线名称'
                                },
                                {
                                    xtype: 'textfield',
                                    id: 'cx_dqmc',
                                    width: 160,
                                    labelWidth: 60,
                                    fieldLabel: '归属地'
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
                                                DataBind();
                                            }
                                        }
                                    ]
                                }
                            ]
                        },
                        {
                            xtype: 'toolbar',
                            dock: 'top',
                            items: [
                                {
                                    xtype: 'label',
                                    id: 'tj'
                                }
                            ]
                        }
                    ]
                }
            ]
        });
        me.callParent(arguments);
    }
});

Ext.onReady(function () {
    new myView();
    DataBind();
})