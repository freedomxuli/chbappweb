var pageSize = 15;
var cx_yhm;
//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
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
        { name: 'UserName' },
        { name: 'STAIR' },
        { name: 'UserTel' },
        { name: 'isinvoice' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        DataBind(nPage);
    }
});
//************************************数据源*****************************************

//************************************页面方法***************************************


function DataBind(nPage) {
    CS('CZCLZ.YKMag.GetYKDDList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_cardNo").getValue(),
        Ext.getCmp("cx_orderId").getValue(), Ext.getCmp("cx_zt").getValue(),
        Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(),
        Ext.getCmp("cx_isinvoice").getValue(), Ext.getCmp("cx_transfertype").getValue(), Ext.getCmp("cx_stair").getValue()
    );
}

//************************************页面方法***************************************

//************************************弹出界面***************************************



//************************************弹出界面***************************************

//************************************主界面*****************************************
Ext.onReady(function () {
    Ext.define('YKView', {
        extend: 'Ext.container.Viewport',

        layout: {
            type: 'fit'
        },

        initComponent: function () {
            var me = this;
            me.items = [
                {
                    xtype: 'gridpanel',
                    id: 'ykGrid',
                    title: '',
                    store: store,
                    columnLines: true,
                    selModel: Ext.create('Ext.selection.CheckboxModel', {
                        checkOnly: true
                    }),
                    columns: [Ext.create('Ext.grid.RowNumberer'),
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'cardNo',
                        sortable: false,
                        menuDisabled: true,
                        text: "加油卡号",
                        width: 200
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'STAIR',
                        sortable: false,
                        menuDisabled: true,
                        text: "一级划拨账户",
                        width: 100
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'oilNum',
                        sortable: false,
                        menuDisabled: true,
                        text: "加油量（L）",
                        width: 100
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'Price',
                        sortable: false,
                        menuDisabled: true,
                        text: "单价（元/L）",
                        align: 'right',
                        width: 100
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'money',
                        sortable: false,
                        menuDisabled: true,
                        text: "消费总金额",
                        align: 'right',
                        width: 100
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'oilType',
                        sortable: false,
                        menuDisabled: true,
                        text: "油品类型（号）",
                        width: 120
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'oilName',
                        sortable: false,
                        menuDisabled: true,
                        text: "油品名称",
                        width: 100
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'oilLevel',
                        sortable: false,
                        menuDisabled: true,
                        text: "油品等级",
                        width: 90
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'status',
                        sortable: false,
                        menuDisabled: true,
                        text: "订单状态",
                        width: 90,
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
                        dataIndex: 'isinvoice',
                        sortable: false,
                        menuDisabled: true,
                        text: "是否开票",
                        width: 100,
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            if (value == 1) {
                                return "已开";
                            } else if (value == 0) {
                                return "未开";
                            } else if (value == 2) {
                                return "已收票";
                            }
                        }
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'oilordercode',
                        sortable: false,
                        menuDisabled: true,
                        text: "订单号",
                        width: 180
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'orderId',
                        sortable: false,
                        menuDisabled: true,
                        text: "找油网的交易流水号",
                        width: 180
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'UserTel',
                        sortable: false,
                        menuDisabled: true,
                        text: "用户手机号",
                        width: 120
                    },
                    {
                        xtype: 'datecolumn',
                        dataIndex: 'addtime',
                        sortable: false,
                        menuDisabled: true,
                        format: 'Y-m-d H:i:s',
                        width: 180,
                        align: 'center',
                        text: "时间"
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
                                    id: 'cx_cardNo',
                                    width: 160,
                                    labelWidth: 60,
                                    fieldLabel: '加油卡号'
                                },
                                {
                                    xtype: 'textfield',
                                    id: 'cx_orderId',
                                    width: 250,
                                    labelWidth: 110,
                                    fieldLabel: '找油网交易流水号'
                                },
                                {
                                    xtype: 'combobox',
                                    id: 'cx_zt',
                                    width: 140,
                                    fieldLabel: '状态',
                                    editable: false,
                                    labelWidth: 50,
                                    store: Ext.create('Ext.data.Store', {
                                        fields: [
                                            { name: 'val' },
                                            { name: 'txt' }
                                        ],
                                        data: [
                                            { 'val': "", 'txt': '全部' },
                                            { 'val': 0, 'txt': '待付款' },
                                            { 'val': 1, 'txt': '支付成功' },
                                            { 'val': 2, 'txt': '交易取消' },
                                            { 'val': 3, 'txt': '交易撤销' }]

                                    }),
                                    queryMode: 'local',
                                    displayField: 'txt',
                                    valueField: 'val',
                                    value: ''
                                },
                                {
                                    xtype: 'combobox',
                                    id: 'cx_isinvoice',
                                    width: 180,
                                    fieldLabel: '是否开票',
                                    editable: false,
                                    labelWidth: 60,
                                    store: Ext.create('Ext.data.Store', {
                                        fields: [
                                            { name: 'val' },
                                            { name: 'txt' }
                                        ],
                                        data: [
                                            { 'val': "", 'txt': '全部' },
                                            { 'val': 0, 'txt': '未开' },
                                            { 'val': 1, 'txt': '已开' }]

                                    }),
                                    queryMode: 'local',
                                    displayField: 'txt',
                                    valueField: 'val',
                                    value: ''
                                }
                            ]
                        },
                        {
                            xtype: 'toolbar',
                            dock: 'top',
                            items: [
                                {
                                    xtype: 'combobox',
                                    id: 'cx_transfertype',
                                    width: 160,
                                    fieldLabel: '划拨类型',
                                    editable: false,
                                    labelWidth: 60,
                                    store: Ext.create('Ext.data.Store', {
                                        fields: [
                                            { name: 'val' },
                                            { name: 'txt' }
                                        ],
                                        data: [
                                            { 'val': "", 'txt': '全部' },
                                            { 'val': 0, 'txt': '干线运输' },
                                            { 'val': 1, 'txt': '油品销售' }]

                                    }),
                                    queryMode: 'local',
                                    displayField: 'txt',
                                    valueField: 'val',
                                    value: ''
                                },
                                {
                                    xtype: 'textfield',
                                    id: 'cx_stair',
                                    width: 250,
                                    labelWidth: 110,
                                    fieldLabel: '一级划拨账户'
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
                                                if (privilege("加油模块_油卡订单查询_查看")) {
                                                    DataBind(1);
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
                                            iconCls: 'view',
                                            text: '导出',
                                            handler: function () {
                                                if (privilege("加油模块_油卡订单查询_导出")) {
                                                    DownloadFile("CZCLZ.YKMag.GetYKDDListToFile", "油卡订单.xls",
                                                        Ext.getCmp("cx_cardNo").getValue(), Ext.getCmp("cx_orderId").getValue(),
                                                        Ext.getCmp("cx_zt").getValue(), Ext.getCmp("cx_beg").getValue(),
                                                        Ext.getCmp("cx_end").getValue(), Ext.getCmp("cx_isinvoice").getValue(),
                                                        Ext.getCmp("cx_transfertype").getValue(), Ext.getCmp("cx_stair").getValue()
                                                    );
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
                                            text: '开票',
                                            id: 'kaipiao',
                                            handler: function () {
                                                if (privilege("加油模块_油卡订单查询_开票")) {
                                                    var grid = Ext.getCmp('ykGrid');
                                                    var gx = grid.getSelectionModel().getSelection();
                                                    if (gx.length == 0) {
                                                        Ext.Msg.show({
                                                            title: '提示',
                                                            msg: "请勾选需要开票的油卡订单。",
                                                            buttons: Ext.MessageBox.OK,
                                                            icon: Ext.MessageBox.INFO
                                                        });
                                                        return;
                                                    }

                                                    var arr = [];
                                                    var bo = false;
                                                    var msg = "";
                                                    for (var i = 0; i < gx.length; i++) {
                                                        if (gx[i].data.status != 1) {
                                                            //0：待付款；1：支付成功；2：交易取消；3：交易撤销；
                                                            var ts = "";
                                                            switch (gx[i].data.status) {
                                                                case 0: ts = '待付款'
                                                                    break;
                                                                case 2: ts = '交易取消'
                                                                    break;
                                                                case 3: ts = '交易撤销'
                                                                    break;
                                                                default: ts = '支付成功';
                                                            }
                                                            msg = "加油卡号[" + gx[i].data.cardNo + "]" + ts + '，不能开票。';
                                                            bo = true;
                                                            break;
                                                        }
                                                        arr.push(gx[i].data);
                                                    }
                                                    if (bo) {
                                                        Ext.Msg.show({
                                                            title: '提示',
                                                            msg: msg,
                                                            buttons: Ext.MessageBox.OK,
                                                            icon: Ext.MessageBox.INFO
                                                        });
                                                    } else {
                                                        CS('CZCLZ.YKMag.Ykkp', function (retVal) {
                                                            Ext.Msg.show({
                                                                title: '提示',
                                                                msg: "开票成功。",
                                                                buttons: Ext.MessageBox.OK,
                                                                icon: Ext.MessageBox.INFO
                                                            });
                                                            DataBind();
                                                        }, CS.onError, arr);
                                                    }
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
    if (!privilegeBo("加油模块_油卡订单查询_开票")) {
        Ext.getCmp('kaipiao').disable(true);
    }
    DataBind();
})
//************************************主界面*****************************************