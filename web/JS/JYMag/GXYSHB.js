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
    CS('CZCLZ.YKMag.GetHBList', function (retVal) {
        gxysStore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_oilcardcode").getValue(), Ext.getCmp("cx_oiltransfercode").getValue(), Ext.getCmp("cx_yhzh").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), 0);
}

//-----------------------------------------------------------界    面-----------------------------------------------------------------
Ext.define('addWin', {
    extend: 'Ext.window.Window',

    height: 300,
    width: 400,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '新增',

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
                        id: 'Oiltransferid',
                        name: 'Oiltransferid',
                        hidden: true
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '划拨用户手机号',
                        id: 'inuserzh',
                        name: 'inuserzh',
                        anchor: '100%'
                    },
                    {
                        xtype: 'numberfield',
                        fieldLabel: '划拨费用',
                        id: 'money',
                        name: 'money',
                        allowBlank: false,
                        allowDecimals: false,
                        allowNegative: false,
                        minValue: 1,
                        anchor: '100%'
                    },
                    {
                        xtype: 'numberfield',
                        id: 'carriageoil',
                        name: 'carriageoil',
                        hidden: true
                    },
                    {
                        xtype: 'combobox',
                        id: 'transfertype',
                        name: 'transfertype',
                        fieldLabel: '划拨类型',
                        editable: false,
                        allowBlank: false,
                        store: Ext.create('Ext.data.Store', {
                            fields: [
                                { name: 'val' },
                                { name: 'txt' }
                            ],
                            data: [
                                { 'val': 0, 'txt': '干线运输' }]

                        }),
                        queryMode: 'local',
                        displayField: 'txt',
                        valueField: 'val',
                        value: 0,
                        anchor: '100%'
                    }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '确定',
                        iconCls: 'dropyes',
                        handler: function () {
                            var point = Ext.getCmp("money").getValue();
                            if (point < 1) {
                                Ext.Msg.show({
                                    title: '提示',
                                    msg: '划拨油卡费用必须大于0',
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.INFO
                                });
                                return;
                            }

                            var carriageoil = Ext.getCmp("carriageoil").getValue();
                            if (point > carriageoil) {
                                Ext.Msg.show({
                                    title: '提示',
                                    msg: '划拨总油量不得超过其所有运费券',
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.INFO
                                });
                                return;
                            }
                            var form = Ext.getCmp('addform');
                            if (form.form.isValid()) {
                                var values = form.form.getValues(false);
                                var me = this;
                                CS('CZCLZ.YKMag.ADDHB', function (retVal) {
                                    if (retVal) {
                                        DataBind(1);
                                        me.up('window').close();
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

//-----------------------------------------------------------界    面-----------------------------------------------------------------
Ext.define('GXYSView', {
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
                                                DownloadFile("CZCLZ.YKMag.GetHBListToFile", "干线运输划拨.xls", Ext.getCmp("cx_oilcardcode").getValue(), Ext.getCmp("cx_oiltransfercode").getValue(), Ext.getCmp("cx_yhzh").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), 0);
                                            }
                                        }
                                    ]
                                }
                            ]
                        },
                        {
                            xtype: 'pagingtoolbar',
                            displayInfo: true,
                            store: gxysStore,
                            dock: 'bottom'
                        }
                    ]
                }
            ]
        });
        me.callParent(arguments);
    }
});

Ext.onReady(function () {
    new GXYSView();
    DataBind();
})