//-----------------------------------------------------------全局变量-----------------------------------------------------------------
var pageSize = 15;

//-----------------------------------------------------------数据源-------------------------------------------------------------------
var gxysStore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
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
    ],
    onPageChange: function (sto, nPage, sorters) {
        DataBind(nPage);
    }
});

//-----------------------------------------------------------页面方法-----------------------------------------------------------------
function DataBind(nPage) {
    CS('CZCLZ.YKMag.GetYKHBList', function (retVal) {
        gxysStore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_oilcardcode").getValue(), Ext.getCmp("cx_oiltransfercode").getValue(), Ext.getCmp("cx_yhzh").getValue(), Ext.getCmp("cx_zt").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
}

//-----------------------------------------------------------界    面-----------------------------------------------------------------
Ext.define('GXYSView', {
    extend: 'Ext.container.Viewport',

    layout: {
        type: 'fit'
    },

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'gridpanel',
                id: 'usergrid',
                store: gxysStore,
                columnLines: true,
                columns: [
                    Ext.create('Ext.grid.RowNumberer'),
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
                viewConfig: {

                },
                dockedItems: [
                    {
                        xtype: 'toolbar',
                        dock: 'top',
                        items: [
                            {
                                xtype: 'textfield',
                                id: 'cx_oilcardcode',
                                width: 160,
                                labelWidth: 60,
                                fieldLabel: '油卡编号'
                            },
                            {
                                xtype: 'textfield',
                                id: 'cx_oiltransfercode',
                                width: 160,
                                labelWidth: 60,
                                fieldLabel: '划拨编号'
                            },
                            {
                                xtype: 'textfield',
                                id: 'cx_yhzh',
                                width: 160,
                                labelWidth: 60,
                                fieldLabel: '用户账号'
                            },
                            {
                                id: 'cx_beg',
                                xtype: 'datefield',
                                fieldLabel: '时间',
                                format: 'Y-m-d',
                                labelWidth: 40,
                                width: 170
                            },
                            {
                                id: 'cx_end',
                                xtype: 'datefield',
                                format: 'Y-m-d',
                                fieldLabel: '至',
                                labelWidth: 20,
                                width: 150
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
                                            DataBind(1);
                                        }
                                    }
                                ]
                            },
                            {
                                xtype: 'buttongroup',
                                items: [
                                    {
                                        xtype: 'button',
                                        iconCls: 'add',
                                        text: '新增',
                                        handler: function () {
                                            CS('CZCLZ.YKMag.GetVisionList', function (retVal) {
                                                if (retVal) {
                                                    var win = new addWin();
                                                    win.show(null, function () {
                                                        Ext.getCmp("carriageoil").setValue(retVal[0]["carriageoil"]);
                                                    })
                                                }
                                            }, CS.onError);

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
                                        iconCls: 'view',
                                        text: '导出',
                                        handler: function () {
                                            //DownloadFile("CZCLZ.YKMag.GetYKHBListToFile", "油卡划拨.xls", Ext.getCmp("cx_oilcardcode").getValue(), Ext.getCmp("cx_oiltransfercode").getValue(), Ext.getCmp("cx_yhzh").getValue(), Ext.getCmp("cx_zt").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
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

new GXYSView();
DataBind();