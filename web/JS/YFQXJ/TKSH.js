//-----------------------------------------------------------全局变量-----------------------------------------------------------------
var pageSize = 15;

//-----------------------------------------------------------数据源-------------------------------------------------------------------
var cardStore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
        { name: 'fhrmc' },
        { name: 'points' },
        { name: 'addtime' },
        { name: 'zxmc' },
        { name: 'mycardId' },
        { name: 'status' },
        { name: 'OrderCode' },
        { name: 'mycardId' },
        { name: 'tktime' },
        { name: 'tkmoney' },
        { name: 'qlx' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getList(nPage);
    }
});
//-----------------------------------------------------------页面方法-----------------------------------------------------------------
function getList(nPage) {
    CS('CZCLZ.XJMag.GetTkshList', function (retVal) {
        cardStore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, {
        'cx_beg': Ext.getCmp("cx_beg").getValue(),
        'cx_end': Ext.getCmp("cx_end").getValue(),
        'cx_fhrzh': Ext.getCmp("cx_fhrzh").getValue(),
        'cx_zxmc': Ext.getCmp("cx_zxmc").getValue(),
        'cx_istk': Ext.getCmp("cx_istk").getValue()
    });
}


function getListEXCELOUT(nPage) {


    DownloadFile("CZCLZ.XJMag.GetTkshListOutList", "退款审核数据下载.xls", {
        'cx_beg': Ext.getCmp("cx_beg").getValue(),
        'cx_end': Ext.getCmp("cx_end").getValue(),
        'cx_fhrzh': Ext.getCmp("cx_fhrzh").getValue(),
        'cx_zxmc': Ext.getCmp("cx_zxmc").getValue(),
        'cx_istk': Ext.getCmp("cx_istk").getValue()
    });

 
}

//审核退款
function shtk(id) {
    var win = new iWin({ mycardId: id });
    win.show();
}
//-----------------------------------------------------------弹出界面-----------------------------------------------------------------
Ext.define('iWin', {
    extend: 'Ext.window.Window',

    height: 150,
    width: 400,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '密码确认',
    initComponent: function () {
        var me = this;
        var mycardId = me.mycardId;
        me.items = [
            {
                xtype: 'form',
                id: 'yjform',
                bodyPadding: 10,
                items: [
                    {
                        xtype: 'textfield',
                        id: 'passworld',
                        name: 'passworld',
                        labelWidth: 70,
                        inputType: 'password',
                        fieldLabel: '密码',
                        allowBlank: false,
                        anchor: '100%'
                    }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '确认',
                        iconCls: 'dropyes',
                        handler: function () {
                            CS('CZCLZ.XJMag.VerifyPassWorld', function (retVal) {
                                if (retVal) {
                                    getList(1);
                                    Ext.MessageBox.alert('提示', "退款成功！");
                                } else {
                                    Ext.MessageBox.alert('提示', "密码错误！");
                                }
                            }, CS.onError, mycardId, Ext.getCmp("passworld").getValue());
                            this.up('window').close();
                        }
                    },
                    {
                        text: '取消',
                        iconCls: 'close',
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
//-----------------------------------------------------------界    面-----------------------------------------------------------------
Ext.define('iView', {
    extend: 'Ext.container.Viewport',

    layout: {
        type: 'fit'
    },

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'gridpanel',
                store: cardStore,
                columnLines: 1,
                border: 1,
                columns: [
                    {
                        xtype: 'gridcolumn',
                        sortable: false,
                        menuDisabled: true,
                        dataIndex: 'mycardId',
                        text: '操作',
                        width: 120,
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            if (record.data.status == 2) {
                                str = "<a href='JavaScript:void(0)' onclick='shtk(\"" + value + "\")'>审核通过并退款</a>";
                                return str;
                            }
                        }
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'OrderCode',
                        sortable: false,
                        menuDisabled: true,
                        flex: 1,
                        text: '订单号'
                    },
                    {
                        xtype: 'numbercolumn',
                        dataIndex: 'points',
                        sortable: false,
                        menuDisabled: true,
                        width: 150,
                        text: '申请退款券额'
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'fhrmc',
                        sortable: false,
                        menuDisabled: true,
                        width: 150,
                        text: '发货人账号'
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'zxmc',
                        sortable: false,
                        menuDisabled: true,
                        width: 150,
                        text: '专线名称'
                    },
                    {
                        xtype: 'datecolumn',
                        dataIndex: 'addtime',
                        sortable: false,
                        menuDisabled: true,
                        width: 90,
                        text: '购买时间',
                        format: 'Y-m-d',
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'status',
                        sortable: false,
                        menuDisabled: true,
                        width: 90,
                        text: '退款状态',
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            if (value == 2) {
                                return "退款审核中";
                            } else if (value == 3) {
                                return "已退款";
                            }
                        }
                    },
                    {
                        xtype: 'datecolumn',
                        dataIndex: 'tktime',
                        sortable: false,
                        menuDisabled: true,
                        width: 90,
                        text: '退款时间',
                        format: 'Y-m-d'
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'tkmoney',
                        sortable: false,
                        menuDisabled: true,
                        width: 150,
                        text: '退款金额'
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'qlx',
                        sortable: false,
                        menuDisabled: true,
                        width: 150,
                        text: '券类型'
                    },
                ],
                viewConfig: {

                },
                dockedItems: [
                    {
                        xtype: 'toolbar',
                        dock: 'top',
                        items: [
                            {
                                xtype: 'datefield',
                                id: 'cx_beg',
                                fieldLabel: '起始日期',
                                width: 180,
                                format: 'Y-m-d',
                                labelWidth: 60
                            },
                            {
                                xtype: 'datefield',
                                id: 'cx_end',
                                fieldLabel: '截止日期',
                                width: 180,
                                format: 'Y-m-d',
                                labelWidth: 60
                            },
                            {
                                xtype: 'textfield',
                                id: 'cx_fhrzh',
                                labelWidth: 80,
                                width: 180,
                                fieldLabel: '发货人账号'
                            },
                            {
                                xtype: 'textfield',
                                id: 'cx_zxmc',
                                labelWidth: 60,
                                width: 160,
                                fieldLabel: '专线名称'
                            },
                            {
                                xtype: 'combobox',
                                id: 'cx_istk',
                                width: 180,
                                fieldLabel: '退款状态',
                                editable: false,
                                labelWidth: 60,
                                store: Ext.create('Ext.data.Store', {
                                    fields: [
                                        { name: 'val' },
                                        { name: 'txt' }
                                    ],
                                    data: [
                                        { 'val': "", 'txt': '全部' },
                                        { 'val': 2, 'txt': '退款审核中' },
                                        { 'val': 3, 'txt': '已退款' }]

                                }),
                                queryMode: 'local',
                                displayField: 'txt',
                                valueField: 'val',
                                value: ''
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
                                            if (privilege("运费券下架_退款审核_查看")) {
                                                getList(1);
                                            }
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
                                        iconCls: 'search',
                                        text: 'EXCEL导出',
                                        handler: function () {
                                            if (privilege("运费券下架_退款审核_查看")) {
                                                getListEXCELOUT(1);
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
                        store: cardStore,
                        dock: 'bottom'
                    }
                ]
            }
        ];
        me.callParent(arguments);
    }
});

Ext.onReady(function () {
    new iView();
    getList(1);
})