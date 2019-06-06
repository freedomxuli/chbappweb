var pageSize = 15;
var cx_yhm;
//************************************数据源*****************************************
var store = createSFW4Store({
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
//************************************数据源*****************************************

//************************************页面方法***************************************


function DataBind(nPage) {
    CS('CZCLZ.YKMag.GetYKHBList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_oilcardcode").getValue(), Ext.getCmp("cx_oiltransfercode").getValue(), Ext.getCmp("cx_yhzh").getValue(), Ext.getCmp("cx_zt").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
}


//************************************页面方法***************************************

//************************************弹出界面***************************************
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
                        name:'transfertype',
                        fieldLabel: '划拨类型',
                        editable: false,
                        allowBlank: false,
                        store: Ext.create('Ext.data.Store', {
                            fields: [
                                { name: 'val' },
                                { name: 'txt' }
                            ],
                            data: [
                                { 'val': 0, 'txt': '干线运输' },
                                { 'val': 1, 'txt': '油品销售' }]

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

//************************************弹出界面***************************************

//************************************主界面*****************************************
Ext.onReady(function() {
    Ext.define('YKView', {
        extend: 'Ext.container.Viewport',

        layout: {
            type: 'fit'
        },

        initComponent: function() {
            var me = this;
            me.items = [
                {
                    xtype: 'gridpanel',
                    id: 'usergrid',
                    title: '',
                    store: store,
                    columnLines:true,
                //    selModel: Ext.create('Ext.selection.CheckboxModel', {

                //}),
                columns: [Ext.create('Ext.grid.RowNumberer'),
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'zcxm',
                            sortable: false,
                            menuDisabled: true,
                            text: "转出人员",
                            flex:1
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
                             format:'Y-m-d H:i:s',
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
                                            xtype: 'combobox',
                                            id: 'cx_zt',
                                            width: 220,
                                            fieldLabel: '是否为查货宝划拨',
                                            editable: false,
                                            labelWidth: 110,
                                            store: Ext.create('Ext.data.Store', {
                                                fields: [
                                                   { name: 'val' },
                                                   { name: 'txt' }
                                                ],
                                                data: [
                                                     { 'val': "", 'txt': '全部' },
                                                    { 'val': 0, 'txt': '是' },
                                                    { 'val': 1, 'txt': '否' }
                                                ]
                                            }),
                                            queryMode: 'local',
                                            displayField: 'txt',
                                            valueField: 'val',
                                            value: ''
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
                                                        if (privilege("加油模块_油卡划拨_查看")) {
                                                            DataBind(1);
                                                        }
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
                                                        if (privilege("加油模块_油卡划拨_导出")) {
                                                            DownloadFile("CZCLZ.YKMag.GetYKHBListToFile", "油卡划拨.xls", Ext.getCmp("cx_oilcardcode").getValue(), Ext.getCmp("cx_oiltransfercode").getValue(), Ext.getCmp("cx_yhzh").getValue(), Ext.getCmp("cx_zt").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
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

    new YKView();
    DataBind();
})
//************************************主界面*****************************************