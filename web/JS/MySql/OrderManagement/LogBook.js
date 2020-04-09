//-----------------------------------------------------------全局变量-----------------------------------------------------------------
var pageSize = 15;
//-----------------------------------------------------------数据源-------------------------------------------------------------------
//询价工作日志
var storeOffer = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
        { name: 'changjia' },//厂家
        { name: 'operatorname' },//操作员
        { name: 'businessname' },//业务员
        { name: 'status' },//询价状态
        { name: 'addtime' },//操作时间
        { name: 'recordmemo' }//操作内容
    ],
    onPageChange: function (sto, nPage, sorters) {
        getLog(1, nPage);
    }
});
//订单工作日志
var storeOrder = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
        { name: 'changjia' },//厂家
        { name: 'operatorname' },//操作员
        { name: 'businessname' },//业务员
        { name: 'status' },//订单状态
        { name: 'addtime' },//操作时间
        { name: 'recordmemo' }//操作内容
    ],
    onPageChange: function (sto, nPage, sorters) {
        getLog(2, nPage);
    }
});
//-----------------------------------------------------------页面方法-----------------------------------------------------------------
//获取列表
function getLog(type, nPage) {
    if (type == 1) {
        CS('CZCLZ.Order.GetLogByPage', function (retVal) {
            storeOffer.setData({
                data: retVal.dt,
                pageSize: pageSize,
                total: retVal.ac,
                currentPage: retVal.cp
            });
        }, CS.onError, nPage, pageSize, Ext.getCmp("cx_changjia").getValue(), Ext.getCmp("cx_czy").getValue(), Ext.getCmp("cx_ywy").getValue(), 1);
    } else {
        CS('CZCLZ.Order.GetLogByPage', function (retVal) {
            storeOrder.setData({
                data: retVal.dt,
                pageSize: pageSize,
                total: retVal.ac,
                currentPage: retVal.cp
            });
        }, CS.onError, nPage, pageSize, Ext.getCmp("cx_changjia2").getValue(), Ext.getCmp("cx_czy2").getValue(), Ext.getCmp("cx_ywy2").getValue(), 2);
    }
}

//-----------------------------------------------------------界    面-----------------------------------------------------------------
Ext.define('logView', {
    extend: 'Ext.container.Viewport',

    layout: {
        type: 'fit'
    },

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'tabpanel',
                activeTab: 0,
                listeners: {
                    'tabchange': function (t, n) {
                        console.log(n);
                        if (n.title === "询价工作日志") {
                            getLog(1, 1);
                        } else if (n.title === "订单工作日志") {
                            getLog(2, 1);
                        }
                    }
                },
                items: [
                    {
                        xtype: 'panel',
                        layout: {
                            type: 'fit'
                        },
                        title: '询价工作日志',
                        items: [
                            {
                                xtype: 'gridpanel',
                                columnLines: 1,
                                border: 1,
                                store: storeOffer,
                                columns: [
                                    Ext.create('Ext.grid.RowNumberer'),
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'changjia',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "厂家"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'operatorname',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "操作员"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'businessname',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "业务员"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'status',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "询价状态",
                                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                            var str = "";//报价单状态，（0：已咨询；1：已下单；2：已过期；）
                                            switch (value) {
                                                case 0:
                                                    str = "已咨询";
                                                    break;
                                                case 1:
                                                    str = "已下单";
                                                    break;
                                                case 2:
                                                    str = "已过期";
                                                    break;
                                                default:
                                                    str = "";
                                            }

                                            return str;
                                        }
                                    },
                                    {
                                        xtype: 'datecolumn',
                                        dataIndex: 'addtime',
                                        format: 'Y-m-d H:m:s',
                                        sortable: false,
                                        menuDisabled: true,
                                        width: 180,
                                        text: '操作时间'
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'recordmemo',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "操作内容",
                                        flex:1
                                    }
                                ],
                                dockedItems: [
                                    {
                                        xtype: 'toolbar',
                                        dock: 'top',
                                        items: [
                                            {
                                                xtype: 'textfield',
                                                id: 'cx_changjia',
                                                width: 140,
                                                labelWidth: 50,
                                                fieldLabel: '厂家'
                                            },
                                            {
                                                xtype: 'textfield',
                                                id: 'cx_czy',
                                                width: 140,
                                                labelWidth: 50,
                                                fieldLabel: '操作员'
                                            },
                                            {
                                                xtype: 'textfield',
                                                id: 'cx_ywy',
                                                width: 140,
                                                labelWidth: 50,
                                                fieldLabel: '业务员'
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
                                                            getLog(1, 1);
                                                        }
                                                    }
                                                ]
                                            },
                                            {
                                                xtype: 'buttongroup',
                                                title: '',
                                                items: [
                                                    {
                                                        xtype: 'button',
                                                        text: '导出',
                                                        handler: function () {
                                                            DownloadFile("CZCLZ.Order.ExportLog", "询价工作日志.xls", Ext.getCmp("cx_changjia").getValue(), Ext.getCmp("cx_czy").getValue(), Ext.getCmp("cx_ywy").getValue(), 1);
                                                        }
                                                    }
                                                ]
                                            }
                                        ]
                                    },
                                    {
                                        xtype: 'pagingtoolbar',
                                        displayInfo: true,
                                        store: storeOffer,
                                        dock: 'bottom'
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        xtype: 'panel',
                        layout: {
                            type: 'fit'
                        },
                        title: '订单工作日志',
                        items: [
                            {
                                xtype: 'gridpanel',
                                columnLines: 1,
                                border: 1,
                                store: storeOrder,
                                columns: [
                                    Ext.create('Ext.grid.RowNumberer'),
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'changjia',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "厂家"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'operatorname',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "操作员"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'businessname',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "业务员"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'status',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "询价状态",
                                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                            var str = "";//订单状态代码，（【未完成】0：已下单；10：提货中；20：待出发；30：在途；）（【已完成】40：待验收付款；90：已验收付款；）（【异常】21：差额待确认；）
                                            switch (value) {
                                                case 0:
                                                    str = "已下单";
                                                    break;
                                                case 10:
                                                    str = "提货中";
                                                    break;
                                                case 20:
                                                    str = "待出发";
                                                    break;
                                                case 30:
                                                    str = "在途";
                                                    break;
                                                case 40:
                                                    str = "待验收付款";
                                                    break;
                                                case 90:
                                                    str = "已验收付款";
                                                    break;
                                                case 21:
                                                    str = "差额待确认";
                                                    break;
                                                default:
                                                    str = "";
                                            }

                                            return str;
                                        }
                                    },
                                    {
                                        xtype: 'datecolumn',
                                        dataIndex: 'addtime',
                                        format: 'Y-m-d H:m:s',
                                        sortable: false,
                                        menuDisabled: true,
                                        width: 180,
                                        text: '操作时间'
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'recordmemo',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "操作内容",
                                        flex: 1
                                    }
                                ],
                                dockedItems: [
                                    {
                                        xtype: 'toolbar',
                                        dock: 'top',
                                        items: [
                                            {
                                                xtype: 'textfield',
                                                id: 'cx_changjia2',
                                                width: 140,
                                                labelWidth: 50,
                                                fieldLabel: '厂家'
                                            },
                                            {
                                                xtype: 'textfield',
                                                id: 'cx_czy2',
                                                width: 140,
                                                labelWidth: 50,
                                                fieldLabel: '操作员'
                                            },
                                            {
                                                xtype: 'textfield',
                                                id: 'cx_ywy2',
                                                width: 140,
                                                labelWidth: 50,
                                                fieldLabel: '业务员'
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
                                                            getLog(2, 1);
                                                        }
                                                    }
                                                ]
                                            },
                                            {
                                                xtype: 'buttongroup',
                                                title: '',
                                                items: [
                                                    {
                                                        xtype: 'button',
                                                        text: '导出',
                                                        handler: function () {
                                                            DownloadFile("CZCLZ.Order.ExportLog", "订单工作日志.xls", Ext.getCmp("cx_changjia2").getValue(), Ext.getCmp("cx_czy2").getValue(), Ext.getCmp("cx_ywy2").getValue(), 2);
                                                        }
                                                    }
                                                ]
                                            }
                                        ]
                                    },
                                    {
                                        xtype: 'pagingtoolbar',
                                        displayInfo: true,
                                        store: storeOrder,
                                        dock: 'bottom'
                                    }
                                ]
                            }
                        ]
                    }
                ]

            }
        ];
        me.callParent(arguments);
    }
});

Ext.onReady(function () {
    new logView();


    getLog(1, 1);

})