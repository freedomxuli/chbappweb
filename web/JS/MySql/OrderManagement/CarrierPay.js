//-------------------------------------------------------全局变量----------------------------------------------------------
var pageSize = 15;
//-------------------------------------------------------数据源------------------------------------------------------------
var storeCarrierPay = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
        { name: 'id' },
        { name: 'shippingnotenumber' },
        { name: 'changjia' },
        { name: 'carriername' },
        { name: 'paymoney' },
        { name: 'paymemo' },
        { name: 'paystatus' },
        { name: 'tuoyunorder' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        GetCarrierPay(nPage);
    }
});

var storeAdd = Ext.create('Ext.data.Store', {
    fields: [
        { name: 'shippingnoteid' },
        { name: 'changjia' },
        { name: 'shippingnotenumber' },
        { name: 'carriername' },
        { name: 'actualwaymoney' },
        { name: 'drivername' },
        { name: 'price' },
        { name: 'actualdrivermoney' },
        { name: 'consignmentdatetime' },
        { name: 'goodstoroutename' },
        { name: 'paysummoney' },
        { name: 'id' }


    ]
});
//-------------------------------------------------------页面方法----------------------------------------------------------
//获取订单列表
function GetCarrierPay(nPage) {
    CS('CZCLZ.Order.GetCarrierPayByPage', function (retVal) {
        storeCarrierPay.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });

    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_changjia").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), Ext.getCmp("cx_shippingnotenumber").getValue(), Ext.getCmp("cx_paystatus").getValue());
}

var myWin;
//新增选择
function SelOrder(orderid, yfje, price, id) {
    if (privilege("一键发包模块_订单管理-承运商付款申请_新增")) {
        myWin = new Ext.Window({
            extend: 'Ext.window.Window',
            viewModel: {
                type: 'mywindow'
            },
            width: 400,
            height: 300,
            layout: 'fit',
            title: "编辑",
            modal: true,
            items: [
                {
                    xtype: 'form',
                    bodyPadding: 10,
                    id: 'addForm',
                    items: [
                        {
                            xtype: 'numberfield',
                            name: 'paymoney',
                            labelWidth: 100,
                            fieldLabel: '申请付款金额',
                            allowBlank: false,
                            minValue: 0,
                            maxValue: price,
                            anchor: '100%'
                        },
                        {
                            xtype: 'label',
                            text: '申请金额不能大于：' + price

                        },
                        {
                            xtype: 'textareafield',
                            name: 'paymemo',
                            labelWidth: 100,
                            fieldLabel: '申请付款备注',
                            allowBlank: false,
                            minValue: 0,
                            anchor: '100%'
                        }
                    ],
                    buttonAlign: 'center',
                    buttons: [
                        {
                            text: '保存',
                            iconCls: 'dropyes',
                            handler: function () {
                                let form = Ext.getCmp('addForm');
                                if (form.form.isValid()) {
                                    var values = form.form.getValues(false);
                                    CS('CZCLZ.Order.SaveOrderCarrierPay', function (retVal) {
                                        if (retVal) {
                                            GetCarrierPay(1);
                                            Ext.MessageBox.alert('提示', "申请成功！");
                                            myWin.close();
                                        } else {
                                            GetCarrierPay(1);
                                            Ext.MessageBox.alert('提示', "申请金额超出应付款，申请失败！");
                                            myWin.close();
                                        }
                                    }, CS.onError, orderid, values, yfje == 'null' ? 0 : yfje, id);
                                }
                            }
                        }
                    ]
                }
            ]
        });
        myWin.show();
    }
}

//提交财务申请付款
function SubmitDriverPay(id) {
    if (privilege("一键发包模块_订单管理-承运商付款申请_提交财务申请付款")) {
        Ext.MessageBox.confirm('添加提示', '是否要提交财务申请付款!', function (obj) {
            if (obj == "yes") {
                CS('CZCLZ.Order.UpdateOrderCarrierPaySubmit', function (retVal) {
                    if (retVal) {
                        GetCarrierPay(1);
                    }
                }, CS.onError, [id]);
            } else {
                return;
            }
        });
    }
}

//财务确认付款
function ConfirmDriverPay(id) {
    if (privilege("一键发包模块_订单管理-承运商付款申请_财务确认付款")) {
        Ext.MessageBox.confirm('添加提示', '是否要财务确认付款!', function (obj) {
            if (obj == "yes") {
                CS('CZCLZ.Order.UpdateOrderCarrierPayConfirm', function (retVal) {
                    if (retVal) {
                        GetCarrierPay(1);
                    }
                }, CS.onError, [id]);
            } else {
                return;
            }
        });
    }
}
//-------------------------------------------------------新增弹出界面-------------------------------------------------------
Ext.define('addDriverOrderPayWin', {
    extend: 'Ext.window.Window',

    height: document.documentElement.clientHeight,
    width: document.documentElement.clientWidth,
    layout: {
        type: 'fit'
    },
    title: '新增',
    modal: true,
    initComponent: function () {
        var me = this;
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
                            store: storeAdd,
                            columns: [Ext.create('Ext.grid.RowNumberer'),
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'changjia',
                                sortable: false,
                                menuDisabled: true,
                                text: "厂家",
                                flex: 1
                            },
                            {
                                xtype: 'datecolumn',
                                dataIndex: 'consignmentdatetime',
                                sortable: false,
                                menuDisabled: true,
                                text: "订单时间",
                                format: 'Y-m-d H:i:s',
                                flex: 1
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'goodstoroutename',
                                sortable: false,
                                menuDisabled: true,
                                text: "收货地址",
                                flex: 1
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'shippingnotenumber',
                                sortable: false,
                                menuDisabled: true,
                                text: "订单号",
                                width: 180,
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    return "<input value='" + value + "' style='border:0px;BACKGROUND-COLOR: transparent;'>";
                                }
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'carriername',
                                sortable: false,
                                menuDisabled: true,
                                text: "承运商/专线或整车司机",
                                width: 180
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'actualwaymoney',
                                sortable: false,
                                menuDisabled: true,
                                text: "承运商金额",
                                width: 100
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'paysummoney',
                                sortable: false,
                                menuDisabled: true,
                                text: "已申请金额",
                                flex: 1
                            },
                            /*{
                                xtype: 'gridcolumn',
                                dataIndex: 'drivername',
                                sortable: false,
                                menuDisabled: true,
                                text: "装车司机",
                                width: 100
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'actualdrivermoney',
                                sortable: false,
                                menuDisabled: true,
                                text: "司机金额",
                                width: 100
                            },*/
                            {
                                text: '操作',
                                dataIndex: 'shippingnoteid',
                                width: 80,
                                sortable: false,
                                menuDisabled: true,
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    var str;
                                    str = "<a onclick='SelOrder(\"" + value + "\",\"" + record.data.actualwaymoney + "\",\"" + record.data.price + "\",\"" + record.data.id + "\");'>选择</a>";
                                    return str;
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
                                            id: 'cx_addshippingnotenumber',
                                            width: 140,
                                            labelWidth: 50,
                                            fieldLabel: '订单号',

                                        },
                                        {
                                            xtype: 'textfield',
                                            id: 'cx_addchangjia',
                                            width: 140,
                                            labelWidth: 50,
                                            fieldLabel: '厂家'
                                        },
                                        {
                                            id: 'addsearch_beg',
                                            xtype: 'datefield',
                                            fieldLabel: '订单时间',
                                            format: 'Y-m-d',
                                            labelWidth: 80,
                                            width: 200
                                        },
                                        {
                                            id: 'addsearch_end',
                                            xtype: 'datefield',
                                            format: 'Y-m-d',
                                            fieldLabel: '至',
                                            labelWidth: 20,
                                            width: 150
                                        },
                                        {
                                            xtype: 'textfield',
                                            id: 'addsearch_are',
                                            labelWidth: 80,
                                            width: 200,
                                            fieldLabel: '收货地址'
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
                                                        CS('CZCLZ.Order.GetAddOrderList', function (retVal) {
                                                            if (retVal) {
                                                                storeAdd.loadData(retVal);
                                                            }
                                                        }, CS.onError, Ext.getCmp("cx_addshippingnotenumber").getValue(), Ext.getCmp("cx_addchangjia").getValue(), Ext.getCmp("addsearch_beg").getValue(), Ext.getCmp("addsearch_end").getValue(), Ext.getCmp("addsearch_are").getValue());
                                                    }
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
//-------------------------------------------------------主界面------------------------------------------------------------
Ext.define('DriverPayView', {
    extend: 'Ext.container.Viewport',

    layout: {
        type: 'fit'
    },
    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'gridpanel',
                id: 'orderPayGrid',
                store: storeCarrierPay,
                columnLines: true,
                selModel: Ext.create('Ext.selection.CheckboxModel', {
                    singleSelect: true
                }),
                columns: [
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'changjia',
                        sortable: false,
                        menuDisabled: true,
                        text: "厂家",
                        flex: 1
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'shippingnotenumber',
                        sortable: false,
                        menuDisabled: true,
                        text: "订单号",
                        width: 180,
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {

                            return "<input value='" + value + "' style='border:0px;BACKGROUND-COLOR: transparent;'>";
                        }
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'tuoyunorder',
                        sortable: false,
                        menuDisabled: true,
                        text: "托运协议编号",
                        width: 100
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'carriername',
                        sortable: false,
                        menuDisabled: true,
                        text: "承运商姓名",
                        width: 100
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'paymoney',
                        sortable: false,
                        menuDisabled: true,
                        text: "承运商申请付款金额",
                        width: 140
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'paymemo',
                        sortable: false,
                        menuDisabled: true,
                        text: "申请备注",
                        width: 100
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'paystatus',
                        sortable: false,
                        menuDisabled: true,
                        text: "承运商付款状态",
                        width: 150,
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            var str = "";//付款状态（0：未付款，客服未申请；10：未付款，客服已申请；11：未付款，总经理审核；20：未付款，操作已提交财务；90：财务已付款；）
                            switch (value) {
                                case 0:
                                    str = "未付款，客服未申请";
                                    break;
                                case 10:
                                    str = "未付款，客服已申请";
                                    break;
                                case 11:
                                    str = "未付款，总经理审核";
                                    break;
                                case 20:
                                    str = "未付款，操作已提交财务";
                                    break;
                                default:
                                    str = "财务已付款";
                            }

                            return str;
                        }
                    },
                    {
                        text: '操作',
                        dataIndex: 'id',
                        width: 130,
                        sortable: false,
                        menuDisabled: true,
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            var str = "";
                            if (record.data.paystatus == 10) {
                                str += "<a onclick='SubmitDriverPay(\"" + value + "\");'>提交财务申请付款</a>";
                            } else if (record.data.paystatus == 20) {
                                str += "<a onclick='ConfirmDriverPay(\"" + value + "\");'>财务确认付款</a>";
                            }
                            return str;
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
                                id: 'cx_changjia',
                                width: 140,
                                labelWidth: 50,
                                fieldLabel: '厂家'
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
                                xtype: 'textfield',
                                id: 'cx_shippingnotenumber',
                                width: 160,
                                labelWidth: 50,
                                fieldLabel: '订单号'
                            },
                            {
                                xtype: 'combobox',
                                id: 'cx_paystatus',
                                fieldLabel: '付款状态',//付款状态（0：未付款，客服未申请；10：未付款，客服已申请；11：未付款，总经理审核；20：未付款，操作已提交财务；90：财务已付款；）
                                labelWidth: 60,
                                width: 210,
                                editable: false,
                                store: Ext.create('Ext.data.Store', {
                                    fields: [
                                        { name: 'value' },
                                        { name: 'name' }
                                    ],
                                    data: [
                                        { 'value': '', 'name': '全部' },
                                        { 'value': '0', 'name': '未付款，客服未申请' },
                                        { 'value': '10', 'name': '未付款，客服已申请' },
                                        { 'value': '11', 'name': '未付款，总经理审核' },
                                        { 'value': '20', 'name': '未付款，操作已提交财务' },
                                        { 'value': '90', 'name': '财务已付款' }
                                    ]
                                }),
                                queryMode: 'local',
                                displayField: 'name',
                                valueField: 'value',
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
                                            GetCarrierPay(1);
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
                                        iconCls: 'add',
                                        text: '新增',
                                        handler: function () {
                                            let win = new addDriverOrderPayWin();
                                            win.show(null, function () {
                                                storeAdd.removeAll();
                                            });
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
                                        text: '批量提交财务',
                                        handler: function () {
                                            if (privilege("一键发包模块_订单管理-承运商付款申请_批量提交财务")) {
                                                var grid = Ext.getCmp("orderPayGrid");
                                                var rds = grid.getSelectionModel().getSelection();

                                                let lis = [];
                                                for (var i = 0; i < rds.length; i++) {
                                                    if (rds[i].get("paystatus") == 10) {
                                                        lis.push(rds[i].get("id"));
                                                    }
                                                }
                                                if (lis.length == 0) {
                                                    Ext.Msg.show({
                                                        title: '提示',
                                                        msg: '请选择申请数据提交财务!',
                                                        buttons: Ext.MessageBox.OK,
                                                        icon: Ext.MessageBox.INFO
                                                    });
                                                    return;
                                                }
                                                Ext.MessageBox.confirm('添加提示', '是否要批量提交财务!', function (obj) {
                                                    if (obj == "yes") {
                                                        CS('CZCLZ.Order.UpdateOrderCarrierPaySubmit', function (retVal) {
                                                            if (retVal) {
                                                                GetCarrierPay(1);
                                                            }
                                                        }, CS.onError, lis);
                                                    } else {
                                                        return;
                                                    }
                                                });
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
                                        text: '批量财务确认',
                                        handler: function () {
                                            if (privilege("一键发包模块_订单管理-承运商付款申请_批量财务确认")) {
                                                var grid = Ext.getCmp("orderPayGrid");
                                                var rds = grid.getSelectionModel().getSelection();

                                                let lis = [];
                                                for (var i = 0; i < rds.length; i++) {
                                                    if (rds[i].get("paystatus") == 20) {
                                                        lis.push(rds[i].get("id"));
                                                    }
                                                }
                                                if (lis.length == 0) {
                                                    Ext.Msg.show({
                                                        title: '提示',
                                                        msg: '请选择申请数据财务确认!',
                                                        buttons: Ext.MessageBox.OK,
                                                        icon: Ext.MessageBox.INFO
                                                    });
                                                    return;
                                                }
                                                Ext.MessageBox.confirm('添加提示', '是否要批量财务确认!', function (obj) {
                                                    if (obj == "yes") {
                                                        CS('CZCLZ.Order.UpdateOrderCarrierPayConfirm', function (retVal) {
                                                            if (retVal) {
                                                                GetCarrierPay(1);
                                                            }
                                                        }, CS.onError, lis);
                                                    } else {
                                                        return;
                                                    }
                                                });
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
                        store: storeCarrierPay,
                        dock: 'bottom'
                    }
                ]
            }
        ];
        me.callParent(arguments);
    }
});


Ext.onReady(function () {
    new DriverPayView();

    GetCarrierPay(1);
})