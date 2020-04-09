//-----------------------------------------------------------全局变量-----------------------------------------------------------------
var pageSize = 15;
//-----------------------------------------------------------数据源-------------------------------------------------------------------
//未寄送
var wjsStore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
        { name: 'shippingnoteid' },//订单ID
        { name: 'username' },//厂家
        { name: 'shippingnoteadddatetime' },//订单时间
        { name: 'shippingnotenumber' },//单号
        { name: 'goodsfromroute' },//起始地
        { name: 'goodstoroute' },//目的地
        { name: 'goodsreceiptplace' },//收货地址
        { name: 'consignee' },//收货方
        { name: 'consicontactname' },//收货联系人
        { name: 'consitelephonenumber' },//收货联系方式
        { name: 'descriptionofgoods' },//货物
        { name: 'totalnumberofpackages' },//数量
        { name: 'itemgrossweight' },//重量
        { name: 'cube' },//体积
        { name: 'vehicletyperequirement' },//车型
        { name: 'vehiclelengthrequirement' },//车长
        { name: 'vehiclelengthrequirementname' },//车长
        { name: 'carriername' },//承运方
        { name: 'istakegoods' },//是否提货
        { name: 'istakegoodsname' },//是否提货
        { name: 'isdelivergoods' },//是否送货
        { name: 'isdelivergoodsname' },//是否送货
        { name: 'actualcompanypay' },//预付运费
        { name: 'actualmoney' },//实际运费
        { name: 'memo' },//备注
        { name: 'shippingnotestatuscode' },//订单状态
        { name: 'shippingnotestatusname' },//订单状态
        { name: 'isabnormal' },
        { name: 'abnormalmemo' },
        { name: 'gpscompany' },
        { name: 'gpsdenno' },
        { name: 'doublepaynum' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getOrder(1, nPage);
    }
});
//已寄送
var yjsStore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
        { name: 'shippingnoteid' },//订单ID
        { name: 'username' },//厂家
        { name: 'shippingnoteadddatetime' },//订单时间
        { name: 'shippingnotenumber' },//单号
        { name: 'goodsfromroute' },//起始地
        { name: 'goodstoroute' },//目的地
        { name: 'goodsreceiptplace' },//收货地址
        { name: 'consignee' },//收货方
        { name: 'consicontactname' },//收货联系人
        { name: 'consitelephonenumber' },//收货联系方式
        { name: 'descriptionofgoods' },//货物
        { name: 'totalnumberofpackages' },//数量
        { name: 'itemgrossweight' },//重量
        { name: 'cube' },//体积
        { name: 'vehicletyperequirement' },//车型
        { name: 'vehiclelengthrequirement' },//车长
        { name: 'vehiclelengthrequirementname' },//车长
        { name: 'carriername' },//承运方
        { name: 'istakegoods' },//是否提货
        { name: 'istakegoodsname' },//是否提货
        { name: 'isdelivergoods' },//是否送货
        { name: 'isdelivergoodsname' },//是否送货
        { name: 'actualcompanypay' },//预付运费
        { name: 'actualmoney' },//实际运费
        { name: 'memo' },//备注
        { name: 'shippingnotestatuscode' },//订单状态
        { name: 'shippingnotestatusname' },//订单状态
        { name: 'isabnormal' },
        { name: 'abnormalmemo' },
        { name: 'gpscompany' },
        { name: 'gpsdenno' },
        { name: 'doublepaynum' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getOrder(2, nPage);
    }
});

var operiUserStore = Ext.create('Ext.data.Store', {//操作人，操作时间，备注
    fields: [
        { name: 'addtime' },
        { name: 'addusername' },
        { name: 'adduser' },
        { name: 'recordmemo' }
    ]
});

var receiptInfoStore = Ext.create('Ext.data.Store', {
    fields: [
        { name: 'receiptinfo' },
        { name: 'fjid' },
        { name: 'photoaccessaddress' },
        { name: 'receiptadditionalinformation' },
        { name: 'addtime' }
    ]
});
//-----------------------------------------------------------页面方法-----------------------------------------------------------------
//获取订单列表
function getOrder(type, nPage) {
    if (type == 1) {
        CS('CZCLZ.Order.GetReceiptList', function (retVal) {
            wjsStore.setData({
                data: retVal.dt,
                pageSize: pageSize,
                total: retVal.ac,
                currentPage: retVal.cp
            });

        }, CS.onError, nPage, pageSize, Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), Ext.getCmp("cx_changjia").getValue(), 1);
    } else {
        CS('CZCLZ.Order.GetReceiptList', function (retVal) {
            yjsStore.setData({
                data: retVal.dt,
                pageSize: pageSize,
                total: retVal.ac,
                currentPage: retVal.cp
            });

        }, CS.onError, nPage, pageSize, Ext.getCmp("cx_beg2").getValue(), Ext.getCmp("cx_end2").getValue(), Ext.getCmp("cx_changjia2").getValue(), 2);
    }
}

//寄送
function Send(orderId) {
    CS('CZCLZ.Order.UpIsSend', function (retVal) {
        getOrder(1, wjsStore.currentPage);
    }, CS.onError, orderId);
}

//查看操作历史
function ShowSendHis(orderId) {
    var win = new operiUserWin({ shippingnoteid: orderId });
    win.show(null, function () {
        CS('CZCLZ.Order.GetShippingRecordLine', function (retVal) {
            operiUserStore.loadData(retVal);
        }, CS.onError, orderId);
    });
}

//查看回执单附件
function FjShow(orderId) {
    var win = new receiptInfoWin({ shippingnoteid: orderId });
    win.show(null, function () {
        GetReceiptInfoLine(orderId);
    });
}

//获取回执单附件列表
function GetReceiptInfoLine(orderid) {
    CS('CZCLZ.Order.GetReceiptInfoList', function (retVal) {
        if (retVal) {
            receiptInfoStore.loadData(retVal);
        }
    }, CS.onError, orderid);
}
//-----------------------------------------------------回执单附件明细弹出界面---------------------------------------------------------------
Ext.define('receiptInfoWin', {
    extend: 'Ext.window.Window',

    height: document.documentElement.clientHeight,
    width: document.documentElement.clientWidth,
    layout: {
        type: 'fit'
    },
    title: '回执单附件',
    modal: true,

    initComponent: function () {
        var me = this;
        var id = me.shippingnoteid;
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
                            store: receiptInfoStore,
                            columns: [Ext.create('Ext.grid.RowNumberer'),
                            {
                                xtype: 'datecolumn',
                                dataIndex: 'addtime',
                                format: 'Y-m-d',
                                sortable: false,
                                menuDisabled: true,
                                width: 110,
                                text: '上传时间'
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'photoaccessaddress',
                                sortable: false,
                                menuDisabled: true,
                                text: "回单照片访问地址",
                                flex: 1
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'receiptadditionalinformation',
                                sortable: false,
                                menuDisabled: true,
                                width: 110,
                                text: "回单附加信息"
                            },
                            {
                                text: '操作',
                                dataIndex: 'receiptinfo',
                                width: 120,
                                sortable: false,
                                menuDisabled: true,
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    var str;
                                    str = "<a onclick='ShowReceiptInfoPhoto(\"" + record.data.photoaccessaddress + "\");'>查看</a>";
                                    return str;
                                }
                            }
                            ],
                            viewConfig: {

                            }
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
//------------------------------------------------------操作历史弹出界面----------------------------------------------------------------
Ext.define('operiUserWin', {
    extend: 'Ext.window.Window',

    height: 400,
    width: 500,
    layout: {
        type: 'fit'
    },
    title: '补单情况',
    modal: true,

    initComponent: function () {
        var me = this;
        var id = me.shippingnoteid;
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
                            store: operiUserStore,
                            columns: [Ext.create('Ext.grid.RowNumberer'),
                                {
                                    xtype: 'gridcolumn',
                                    dataIndex: 'addusername',
                                    sortable: false,
                                    menuDisabled: true,
                                    text: "操作员",
                                    width: 100,
                                },
                                {
                                    xtype: 'datecolumn',
                                    dataIndex: 'addtime',
                                    format: 'Y-m-d H:m:s',
                                    sortable: false,
                                    menuDisabled: true,
                                    width: 160,
                                    text: '操作时间'
                                },
                                {
                                    xtype: 'gridcolumn',
                                    dataIndex: 'recordmemo',
                                    sortable: false,
                                    menuDisabled: true,
                                    text: "操作备注",
                                    flex: 1
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

//查看回执单附件（照片）
function ShowReceiptInfoPhoto(photoHtml) {
    var win = new receiptInfoFjWin();
    win.show(null, function () {
        var html = "";
        html += '<div style="overflow-x: auto; overflow-y: auto; height: 680px; width:100%;">';
        html += '<table width="100%" border="0" cellspacing="0" cellpadding="0px" >';
        html += '  <tr>';
        html += '    <td align="center"><img width="200px" height="200px"  src="' + photoHtml + '"  width="100%"/></td>';
        html += '  </tr>';
        html += '</table></div>';
        Ext.getCmp('photoEl').setText(html, false);
    });
}
//-----------------------------------------------------------------回执单附件-----------------------------------------------------------------
Ext.define('receiptInfoFjWin', {
    extend: 'Ext.window.Window',
    height: document.documentElement.clientHeight,
    width: document.documentElement.clientWidth,
    layout: {
        type: 'fit'
    },
    modal: true,
    title: '回执单照片查看',
    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'form',
                bodyPadding: 10,
                items: [
                    {
                        id: 'photoEl',
                        html: '',
                        xtype: 'label',
                        height: '100%'
                    }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '关闭',
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
Ext.define('receiptView', {
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
                        if (n.title === "未寄送") {
                            getOrder(1, 1);
                        } else if (n.title === "已寄送") {
                            getOrder(2, 1);
                        }
                    }
                },
                items: [
                    {
                        xtype: 'panel',
                        layout: {
                            type: 'fit'
                        },
                        title: '未寄送',
                        items: [
                            {
                                xtype: 'gridpanel',
                                columnLines: 1,
                                border: 1,
                                store: wjsStore,
                                columns: [
                                    Ext.create('Ext.grid.RowNumberer'),
                                    {
                                        xtype: 'datecolumn',
                                        dataIndex: 'shippingnoteadddatetime',
                                        format: 'Y-m-d',
                                        sortable: false,
                                        menuDisabled: true,
                                        width: 110,
                                        text: '订单时间'
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'shippingnotenumber',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "单号",
                                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {

                                            return "<input value='" + value + "' style='border:0px;BACKGROUND-COLOR: transparent;'>";
                                        }
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'goodsfromroute',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "起始地"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'goodstoroute',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "目的地"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'goodsreceiptplace',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "收货地址"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'consignee',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "收货方"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'consicontactname',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "收货联系人"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'consitelephonenumber',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "收货联系方式"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'descriptionofgoods',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "货物"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'totalnumberofpackages',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "数量"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'itemgrossweight',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "重量"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'cube',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "体积"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'vehicletyperequirement',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "车型",
                                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                            var str = "";//车型（1：栏板车；2：厢车；）
                                            switch (value) {
                                                case "1":
                                                    str = "栏板车";
                                                    break;
                                                case "2":
                                                    str = "厢车";
                                                    break;
                                                default:
                                                    str = "";
                                            }

                                            return str;
                                        }
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'vehiclelengthrequirementname',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "车长"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'carriername',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "承运方"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'istakegoodsname',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "是否提货"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'isdelivergoodsname',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "是否送货"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'actualcompanypay',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "预付运费"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'actualmoney',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "实际运费"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'memo',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "备注"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'shippingnoteid',
                                        sortable: false,
                                        menuDisabled: true,
                                        width: 150,
                                        text: '操作',
                                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                            var str = "<a onclick='FjShow(\"" + value + "\");'>附件查看</a> || <a onclick='Send(\"" + value + "\");'>寄送</a>";
                                            return str;
                                        }
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
                                                id: 'cx_beg',
                                                xtype: 'datefield',
                                                fieldLabel: '订单时间',
                                                format: 'Y-m-d',
                                                labelWidth: 80,
                                                width: 210
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
                                                            getOrder(1, 1);
                                                        }
                                                    }
                                                ]
                                            }
                                        ]
                                    },
                                    {
                                        xtype: 'pagingtoolbar',
                                        displayInfo: true,
                                        store: wjsStore,
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
                        title: '已寄送',
                        items: [
                            {
                                xtype: 'gridpanel',
                                columnLines: 1,
                                border: 1,
                                store: yjsStore,
                                columns: [
                                    Ext.create('Ext.grid.RowNumberer'),
                                    {
                                        xtype: 'datecolumn',
                                        dataIndex: 'shippingnoteadddatetime',
                                        format: 'Y-m-d',
                                        sortable: false,
                                        menuDisabled: true,
                                        width: 110,
                                        text: '订单时间'
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'shippingnotenumber',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "单号",
                                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {

                                            return "<input value='" + value + "' style='border:0px;BACKGROUND-COLOR: transparent;'>";
                                        }
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'goodsfromroute',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "起始地"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'goodstoroute',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "目的地"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'goodsreceiptplace',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "收货地址"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'consignee',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "收货方"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'consicontactname',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "收货联系人"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'consitelephonenumber',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "收货联系方式"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'descriptionofgoods',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "货物"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'totalnumberofpackages',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "数量"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'itemgrossweight',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "重量"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'cube',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "体积"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'vehicletyperequirement',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "车型",
                                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                            var str = "";//车型（1：栏板车；2：厢车；）
                                            switch (value) {
                                                case "1":
                                                    str = "栏板车";
                                                    break;
                                                case "2":
                                                    str = "厢车";
                                                    break;
                                                default:
                                                    str = "";
                                            }

                                            return str;
                                        }
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'vehiclelengthrequirementname',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "车长"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'carriername',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "承运方"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'istakegoodsname',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "是否提货"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'isdelivergoodsname',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "是否送货"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'actualcompanypay',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "预付运费"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'actualmoney',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "实际运费"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'memo',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "备注"
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'shippingnoteid',
                                        sortable: false,
                                        menuDisabled: true,
                                        width: 150,
                                        text: '操作',
                                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                            var str = "<a onclick='FjShow(\"" + value + "\");'>附件查看</a> || <a onclick='ShowSendHis(\"" + value + "\");'>查看</a>";
                                            return str;
                                        }
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
                                                id: 'cx_beg2',
                                                xtype: 'datefield',
                                                fieldLabel: '订单时间',
                                                format: 'Y-m-d',
                                                labelWidth: 80,
                                                width: 210
                                            },
                                            {
                                                id: 'cx_end2',
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
                                                            getOrder(2, 1);
                                                        }
                                                    }
                                                ]
                                            }
                                        ]
                                    },
                                    {
                                        xtype: 'pagingtoolbar',
                                        displayInfo: true,
                                        store: yjsStore,
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
    new receiptView();


    getOrder(1, 1);

})