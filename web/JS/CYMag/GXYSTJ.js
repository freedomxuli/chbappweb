//-----------------------------------------------------------全局变量-----------------------------------------------------------------


//-----------------------------------------------------------数据源-------------------------------------------------------------------
var gxysStore = Ext.create('Ext.data.Store', {
    fields: ['UserID', 'UserXM', 'YK_SY']
});

var linestore = Ext.create('Ext.data.Store', {
    fields: ['ZXID', 'RQ', 'QS', 'YK_QC', 'YK_HB', 'YK_XH', 'YK_SY']
});

var ykhbStore = Ext.create('Ext.data.Store', {
    fields: [
        { name: 'addtime' },
        { name: 'oiltransfercode' },
        { name: 'oilcardcode' },
        { name: 'outuserid' },
        { name: 'money' },
        { name: 'zcxm' },
        { name: 'zczh' },
        { name: 'inuserid' },
        { name: 'zrxm' },
        { name: 'zrzh' }
    ]
});

var ykxhStore = Ext.create('Ext.data.Store', {
    fields: [
        { name: 'oilorderid' },
        { name: 'cardNo' },
        { name: 'oilNum' },
        { name: 'Price' },
        { name: 'money' },
        { name: 'oilType' },
        { name: 'oilName' },
        { name: 'oilLevel' },
        { name: 'status' },
        { name: 'oilordercode' },
        { name: 'orderId' },
        { name: 'addtime' },
        { name: 'UserTel' },
        { name: 'UserName' }
    ]
});
//-----------------------------------------------------------页面方法-----------------------------------------------------------------
function DataBind() {
    var mc = Ext.getCmp('cx_userxm').getValue();
    var dq = Ext.getCmp('cx_dqmc').getValue();

    CS('CZCLZ.YKMag.GetYkTj', function (retVal) {
        gxysStore.loadData(retVal.dt);
        Ext.getCmp('tj').setText("干线运输各个专线的总账剩余：" + retVal.gxye);
    }, CS.onError, mc, dq, 0);
}

function ShowLine(zxid, zxxm) {
    var win = new lineWin({ zxid: zxid });
    win.show(null, function () {
        Ext.getCmp('zx').setValue(zxxm);
        Ext.getCmp('ny').setValue(new Date());
        CS('CZCLZ.YKMag.GetLine', function (retVal) {
            linestore.loadData(retVal);
        }, CS.onError, zxid, Ext.getCmp('ny').getValue(), 0);
    })
}


function ykhb(zxid, rq) {
    var win = new ykhbWin({ zxid: zxid, rq: rq });
    win.show(null, function () {
        CS('CZCLZ.YKMag.GetYkhbLine', function (retVal) {
            ykhbStore.loadData(retVal);
        }, CS.onError, zxid, rq, 0);
    });
}

function ykxh(zxid, rq) {
    var win = new ykxhWin({ zxid: zxid, rq: rq });
    win.show(null, function () {
        CS('CZCLZ.YKMag.GetYkxhLine', function (retVal) {
            ykxhStore.loadData(retVal);
        }, CS.onError, zxid, rq, 0);
    });
}
//---------------------------------------------------------油卡消耗界面---------------------------------------------------------------
Ext.define('ykxhWin', {
    extend: 'Ext.window.Window',

    height: document.documentElement.clientHeight,
    width: document.documentElement.clientWidth,
    layout: {
        type: 'fit'
    },
    title: '油卡订单明细记录',
    modal: true,

    initComponent: function () {
        var me = this;
        var zxid = me.zxid;
        var rq = me.rq;
        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'panel',
                    layout: {
                        type: 'fit'
                    },
                    items: [
                        {
                            xtype: 'gridpanel',
                            columnLines: 1,
                            border: 1,
                            store: ykxhStore,
                            columns: [Ext.create('Ext.grid.RowNumberer'),
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'cardNo',
                                sortable: false,
                                menuDisabled: true,
                                text: "加油卡号",
                                flex: 1
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'oilNum',
                                sortable: false,
                                menuDisabled: true,
                                text: "加油量"
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'Price',
                                sortable: false,
                                menuDisabled: true,
                                text: "单价"
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'money',
                                sortable: false,
                                menuDisabled: true,
                                text: "消费总金额"
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'oilType',
                                sortable: false,
                                menuDisabled: true,
                                text: "油品类型"
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'oilName',
                                sortable: false,
                                menuDisabled: true,
                                text: "油品名称"
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'oilLevel',
                                sortable: false,
                                menuDisabled: true,
                                text: "油品等级"
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'status',
                                sortable: false,
                                menuDisabled: true,
                                text: "订单状态",
                                renderer: function (v, m) {
                                    str = "";
                                    if (v == 0) {
                                        str = "待付款";
                                    } else if (v == 1) {
                                        str = "支付成功";
                                    } else if (v == 2) {
                                        str = "交易取消";
                                    } else if (v == 3) {
                                        str = "交易撤销";
                                    }
                                    return str;
                                }
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'oilordercode',
                                sortable: false,
                                menuDisabled: true,
                                text: "订单号",
                                flex: 1
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'orderId',
                                sortable: false,
                                menuDisabled: true,
                                text: "找油网的交易流水号",
                                flex: 1
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'UserName',
                                sortable: false,
                                menuDisabled: true,
                                text: "用户手机号"
                            },
                            {
                                xtype: 'datecolumn',
                                dataIndex: 'addtime',
                                sortable: false,
                                menuDisabled: true,
                                format: 'Y-m-d H:i:s',
                                width: 180,
                                text: "时间"
                            }
                            ],
                            dockedItems: [
                                {
                                    xtype: 'toolbar',
                                    dock: 'top',
                                    items: [
                                        {
                                            xtype: 'button',
                                            iconCls: 'view',
                                            text: '导出',
                                            handler: function () {
                                                DownloadFile("CZCLZ.YKMag.GetYkxhLineToFile", "油卡订单.xls", zxid, rq, 0);
                                            }
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

//---------------------------------------------------------油卡划拨界面---------------------------------------------------------------
Ext.define('ykhbWin', {
    extend: 'Ext.window.Window',

    height: document.documentElement.clientHeight,
    width: document.documentElement.clientWidth,
    layout: {
        type: 'fit'
    },
    title: '油卡划拨明细记录',
    modal: true,

    initComponent: function () {
        var me = this;
        var zxid = me.zxid;
        var rq = me.rq;
        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'panel',
                    layout: {
                        type: 'fit'
                    },
                    items: [
                        {
                            xtype: 'gridpanel',
                            columnLines: 1,
                            border: 1,
                            store: ykhbStore,
                            columns: [Ext.create('Ext.grid.RowNumberer'),
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'zcxm',
                                sortable: false,
                                menuDisabled: true,
                                text: "转出人员",
                                flex: 1
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'zrzh',
                                sortable: false,
                                menuDisabled: true,
                                text: "转入人员账号",
                                flex: 1
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'zrxm',
                                sortable: false,
                                menuDisabled: true,
                                text: "转入人员名称",
                                flex: 1
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'money',
                                sortable: false,
                                menuDisabled: true,
                                text: "转出油卡金额",
                                flex: 1
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'oilcardcode',
                                sortable: false,
                                menuDisabled: true,
                                text: "油卡编号",
                                flex: 1
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'oiltransfercode',
                                sortable: false,
                                menuDisabled: true,
                                text: "油卡划拨编号",
                                flex: 1
                            },
                            {
                                xtype: 'datecolumn',
                                dataIndex: 'addtime',
                                sortable: false,
                                menuDisabled: true,
                                format: 'Y-m-d H:i:s',
                                text: "时间",
                                flex: 1
                            }
                            ],
                            dockedItems: [
                                {
                                    xtype: 'toolbar',
                                    dock: 'top',
                                    items: [
                                        {
                                            xtype: 'button',
                                            iconCls: 'view',
                                            text: '导出',
                                            handler: function () {
                                                DownloadFile("CZCLZ.YKMag.GetYkhbLineToFile", "油卡划拨.xls", zxid, rq, 0);
                                            }
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

//-----------------------------------------------------专线油卡剩余金额界面-----------------------------------------------------------
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
        var zxid = me.zxid;
        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'panel',
                    layout: {
                        type: 'fit'
                    },
                    items: [
                        {
                            xtype: 'gridpanel',
                            columnLines: 1,
                            border: 1,
                            store: linestore,
                            columns: [
                                {
                                    xtype: 'gridcolumn',
                                    dataIndex: 'QS',
                                    sortable: false,
                                    menuDisabled: true,
                                    width: 90,
                                    align: 'center',
                                    text: '日期'
                                },
                                {
                                    xtype: 'gridcolumn',
                                    dataIndex: 'YK_QC',
                                    sortable: false,
                                    menuDisabled: true,
                                    width: 130,
                                    text: '期初金额',
                                    align: 'right'
                                },
                                {
                                    xtype: 'gridcolumn',
                                    dataIndex: 'YK_HB',
                                    sortable: false,
                                    menuDisabled: true,
                                    width: 160,
                                    text: '油卡划拨（包含转出）',
                                    align: 'right',
                                    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                        var str = '<a href="javascript:void(0);" onclick="ykhb(\'' + record.data.ZXID + '\',\'' + record.data.RQ + '\')">' + value + '</a>';
                                        return str;
                                    }
                                },
                                {
                                    xtype: 'gridcolumn',
                                    dataIndex: 'YK_XH',
                                    sortable: false,
                                    menuDisabled: true,
                                    width: 160,
                                    text: '油卡消耗',
                                    align: 'right',
                                    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                        var str = '<a href="javascript:void(0);" onclick="ykxh(\'' + record.data.ZXID + '\',\'' + record.data.RQ + '\')">' + value + '</a>';
                                        return str;
                                    }
                                },
                                {
                                    text: '剩余金额',
                                    align: 'right',
                                    dataIndex: 'YK_SY',
                                    width: 130,
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
                                            id: 'zx',
                                            width: 160,
                                            labelWidth: 60,
                                            fieldLabel: '专线',
                                            readOnly: true
                                        },
                                        {
                                            xtype: 'monthfield',
                                            fieldLabel: '日期',
                                            editable: false,
                                            width: 150,
                                            labelWidth: 30,
                                            labelAlign: 'right',
                                            format: 'Y年m月',
                                            id: 'ny'
                                        },
                                        {
                                            xtype: 'button',
                                            iconCls: 'search',
                                            text: '查询',
                                            handler: function () {
                                                if (privilege("承运模块_干线运输统计表_查看")) {
                                                    CS('CZCLZ.YKMag.GetLine', function (retVal) {
                                                        linestore.loadData(retVal);
                                                    }, CS.onError, zxid, Ext.getCmp('ny').getValue(), 0);
                                                }
                                            }
                                        },
                                        {
                                            xtype: 'button',
                                            iconCls: 'view',
                                            text: '导出',
                                            handler: function () {
                                                if (privilege("承运模块_干线运输统计表_导出")) {
                                                    DownloadFile("CZCLZ.YKMag.GetLineToFile", "专线油卡明细.xls", zxid, Ext.getCmp('ny').getValue(), 0);
                                                }
                                            }
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
                                return '<a href="javascript:void(0);" onclick="ShowLine(\'' + r.data.UserID + '\',\'' + r.data.UserXM + '\')">' + v + '</a>';
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
                                                if (privilege("承运模块_干线运输统计表_查看")) {
                                                    DataBind();
                                                }
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