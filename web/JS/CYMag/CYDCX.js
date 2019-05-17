var pageSize = 15;
var cx_yhm;
//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
        { name: 'carriageid' },
        { name: 'carriagecode' },
        { name: 'carriagetime' },
        { name: 'carriagefromprovince' },
        { name: 'carriagefromcity' },
        { name: 'carriagetoprovince' },
        { name: 'carriagetocity' },
        { name: 'driverid' },
        { name: 'carriagepoints' },
        { name: 'carriageoilmoney' },
        { name: 'carriagemoney' },
        { name: 'status' },
        { name: 'carriagestatus' },
        { name: 'memo' },
        { name: 'ispayinsurance' },
        { name: 'insuranceid' },
        { name: 'insurancemoney' },
        { name: 'addtime' },
        { name: 'adduser' },
        { name: 'userid' },
        { name: 'isoilpay' },
        { name: 'ismoneypay' },
        { name: 'isarrive' },
        { name: 'ismoneynewpay' },
        { name: 'sjzh' },
        { name: 'sjxm' },
        { name: 'sjdh' },
        { name: 'sjcarnumber' },
        { name: 'zx' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        DataBind(nPage);
    }
});
//************************************数据源*****************************************

//************************************页面方法***************************************


function DataBind(nPage) {
    CS('CZCLZ.CYMag.GetCYDList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_carriagecode").getValue(), Ext.getCmp("cx_UserXM").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
}


function QR(carriageid, carriagestatus) {
    CS('CZCLZ.CYMag.QR', function (retVal) {
        DataBind(1);
    }, CS.onError, carriageid, carriagestatus);
}

function TH(carriageid, carriagestatus) {
    CS('CZCLZ.CYMag.TH', function (retVal) {
        DataBind(1);
    }, CS.onError, carriageid, carriagestatus);
}

function WC(carriageid, carriagestatus) {
    CS('CZCLZ.CYMag.WC', function (retVal) {
        DataBind(1);
    }, CS.onError, carriageid, carriagestatus);
}
function JJ(carriageid, carriagestatus) {
    var yjwin = new yjWin({ carriageid: carriageid,carriagestatus:carriagestatus });
    yjwin.show();
}

function YKDK(carriageid, carriagestatus) {
    Ext.MessageBox.confirm("提示", "是否油卡打款？", function (obj) {
        if (obj == "yes") {
            CS('CZCLZ.CYMag.YKDK', function (retVal) {
                DataBind(1);
            }, CS.onError, carriageid, carriagestatus);
        }
    });
}

function XJDK(carriageid, carriagestatus) {
    if (privilege("承运模块_承运单查询_打款")) {
        Ext.MessageBox.confirm("提示", "是否现付打款？", function (obj) {
            if (obj == "yes") {
                var passwin = new PassWin({ carriageid: carriageid, carriagestatus: carriagestatus, type: 0 });
                passwin.show();
            }
        });
    }
}

function YSFDK(carriageid, carriagestatus) {
    if (privilege("承运模块_承运单查询_打款")) {
        Ext.MessageBox.confirm("提示", "是否验收付打款？", function (obj) {
            if (obj == "yes") {
                var passwin = new PassWin({ carriageid: carriageid, carriagestatus: carriagestatus, type: 1 });
                passwin.show();
            }
        });

    }
}
function CKBD(carriageid) {
    CS('CZCLZ.CYMag.getInsure', function (retVal) {
        if (retVal) {
            var result = retVal.evalJSON();
            if (result.success) { window.open(result.url) } else {
                Ext.MessageBox.alert('提示', "查看失败");
            }
        }
    }, CS.onError, carriageid);
}


//************************************页面方法***************************************

//************************************弹出界面***************************************
Ext.define('yjWin', {
    extend: 'Ext.window.Window',

    height: 150,
    width: 400,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '退回意见',
    initComponent: function () {
        var me = this;
        var carriageid = me.carriageid;
        var carriagestatus = me.carriagestatus;
        me.items = [
            {
                xtype: 'form',
                id: 'yjform',
                bodyPadding: 10,
                items: [
                     {
                         xtype: 'textareafield',
                         id: 'yj',
                         name: 'yj',
                         labelWidth: 70,
                         fieldLabel: '意见',
                         allowBlank:false,
                         anchor: '100%'
                     }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '确认',
                        iconCls: 'dropyes',
                        handler: function () {
                            CS('CZCLZ.CYMag.JJ', function (retVal) {
                                if (retVal) {
                                    DataBind(1);
                                    Ext.MessageBox.alert('提示', "拒绝成功！");
                                }
                            }, CS.onError, carriageid, carriagestatus, Ext.getCmp("yj").getValue());
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

Ext.define('PassWin', {
    extend: 'Ext.window.Window',

    height: 120,
    width: 350,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '确认密码',
    initComponent: function () {
        var me = this;
        var carriageid = me.carriageid;
        var carriagestatus = me.carriagestatus;
        var type = me.type;
        me.items = [
            {
                xtype: 'form',
                id: 'passform',
                bodyPadding: 10,
                items: [
                     {
                         xtype: 'textfield',
                         id: 'password',
                         name: 'password',
                         labelWidth: 70,
                         fieldLabel: '密码',
                         allowBlank: false,
                         inputType: 'password',
                         anchor: '100%'
                     }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '确认',
                        iconCls: 'dropyes',
                        handler: function () {
                            CS('CZCLZ.CYMag.QRMM', function (retVal) {
                                if (retVal) {
                                    if (type == 0) {
                                        CS('CZCLZ.CYMag.XJDK', function (retVal) {
                                            DataBind(1);
                                            this.up('window').close();
                                        }, CS.onError, carriageid, carriagestatus);
                                    } else if (type == 1) {
                                        CS('CZCLZ.CYMag.YSFDK', function (retVal) {
                                            DataBind(1);
                                            this.up('window').close();
                                        }, CS.onError, carriageid, carriagestatus);
                                    }
                                } else {
                                    Ext.MessageBox.alert('提示', "密码错误！");
                                    Ext.getCmp("password").setValue()
                                }
                            }, CS.onError, Ext.getCmp("password").getValue());
                            
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
    Ext.define('CYDView', {
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
                    columns: [Ext.create('Ext.grid.RowNumberer'),//专线名称，司机手机、车牌、是否保险、保费、是否油卡付款、是否现金付款
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'carriageid',
                            sortable: false,
                            menuDisabled: true,
                            text: "操作",
                            width: 150,
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                var str = "";
                                if (record.data.carriagestatus == 10) {
                                    str += " <a onclick='QR(\"" + value + "\",\"" + record.data.carriagestatus + "\");'>确认</a>";
                                } else if (record.data.carriagestatus == 11)
                                    str += " <a onclick='TH(\"" + value + "\",\"" + record.data.carriagestatus + "\");'>退回</a> ";
                                 if ((record.data.carriagestatus == 30 || record.data.carriagestatus == 40 || record.data.carriagestatus == 50) && record.data.isoilpay == 0) {
                                    str += " <a onclick='YKDK(\"" + value + "\",\"" + record.data.carriagestatus + "\");'>油卡打款</a>";
                                }
                                 if ((record.data.carriagestatus == 30 || record.data.carriagestatus == 40 || record.data.carriagestatus == 50) && record.data.ismoneypay == 0) {
                                    str += " <a onclick='XJDK(\"" + value + "\",\"" + record.data.carriagestatus + "\");'>现付打款</a>";
                                 } if ((record.data.carriagestatus >= 50) && record.data.ismoneynewpay == 0) {
                                     str += " <a onclick='YSFDK(\"" + value + "\",\"" + record.data.carriagestatus + "\");'>验收付打款</a>";
                                 } if (record.data.carriagestatus == 50 && record.data.isoilpay == 1 && record.data.ismoneypay == 1 && record.data.ismoneynewpay == 1) {
                                    str += " <a onclick='WC(\"" + value + "\",\"" + record.data.carriagestatus + "\");'>完成</a>";
                                 }

                                 if (record.data.carriagestatus >=30 ) {
                                     str += " <a onclick='CKBD(\"" + value + "\");'>查看保单</a> ";
                                 }
                                return str;
                            }
                        },
                         {
                             xtype: 'gridcolumn',
                             dataIndex: 'carriageid',
                             sortable: false,
                             menuDisabled: true,
                             text: "拒绝",
                             width: 80,
                             renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                 var str = "";
                                 if (record.data.carriagestatus == 10) {
                                     str += " <a onclick='JJ(\"" + value + "\",\"" + record.data.carriagestatus + "\");'>拒绝</a>";
                                 } else if (record.data.carriagestatus == 11)
                                     str += " <a onclick='JJ(\"" + value + "\",\"" + record.data.carriagestatus + "\");'>拒绝</a> ";
                                 return str;
                             }
                         },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'zx',
                            sortable: false,
                            menuDisabled: true,
                            text: "上游客户（专线名称）",
                            width: 200
                        },
                        {
                            xtype: 'datecolumn',
                            dataIndex: 'carriagetime',
                            sortable: false,
                            menuDisabled: true,
                            text: "运输日期",
                            format: 'Y-m-d H:i:s',
                            width: 200
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'carriagefromprovince',
                            sortable: false,
                            menuDisabled: true,
                            text: "起运地",
                            width: 150,
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                return record.data.carriagefromprovince + record.data.carriagefromcity;

                            }
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'carriagefromprovince',
                            sortable: false,
                            menuDisabled: true,
                            text: "目的地",
                            width: 150,
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                return record.data.carriagetoprovince + record.data.carriagetocity;

                            }
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'carriagepoints',
                            sortable: false,
                            menuDisabled: true,
                            text: "支付券额",
                            width: 100
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'sjzh',
                            sortable: false,
                            menuDisabled: true,
                            text: "下游司机",
                            width: 100
                        },
                         {
                             xtype: 'gridcolumn',
                             dataIndex: 'sjxm',
                             sortable: false,
                             menuDisabled: true,
                             text: "司机姓名",
                             width: 100
                         },
                         {
                             xtype: 'gridcolumn',
                             dataIndex: 'sjdh',
                             sortable: false,
                             menuDisabled: true,
                             text: "电话",
                             width: 100
                         },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'sjcarnumber',
                            sortable: false,
                            menuDisabled: true,
                            text: "车牌",
                            width: 100
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'carriageoilmoney',
                            sortable: false,
                            menuDisabled: true,
                            text: "油卡",
                            width: 100
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'carriagemoney',
                            sortable: false,
                            menuDisabled: true,
                            text: "现金",
                            width: 100
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'insurancemoney',
                            sortable: false,
                            menuDisabled: true,
                            text: "保费",
                            width: 100,
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                var str = "";
                                if (value != null && value != "") {
                                    str = value/100
                                }
                                return str;
                            }
                        },
                        //{
                        //    xtype: 'gridcolumn',
                        //    dataIndex: 'ispayinsurance',
                        //    sortable: false,
                        //    menuDisabled: true,
                        //    text: "是否保险",
                        //    width: 200,
                        //    renderer: function (v, m) {
                        //        str = "";
                        //        if (v == 0) {
                        //            str = "否";
                        //        } else if (v == 1) {
                        //            str = "是";
                        //        }
                        //        return str;
                        //    }
                        //},
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'isoilpay',
                            sortable: false,
                            menuDisabled: true,
                            text: "是否油卡付款",
                            width: 100,
                            renderer: function (v, m) {
                                str = "";
                                if (v == 0) {
                                    str = "否";
                                } else if (v == 1) {
                                    str = "是";
                                }
                                return str;
                            }
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'ismoneypay',
                            sortable: false,
                            menuDisabled: true,
                            text: "是否现金付款",
                            width: 100,
                            renderer: function (v, m) {
                                str = "";
                                if (v == 0) {
                                    str = "否";
                                } else if (v == 1) {
                                    str = "是";
                                }
                                return str;
                            }
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'carriagestatus',
                            sortable: false,
                            menuDisabled: true,
                            text: "订单状态",
                            width: 200,
                            renderer: function (v, m) {
                                str = "";
                                if (v == 0) {
                                    str = "已申请待后台审核";
                                } else if (v == 10) {
                                    str = "司机确认待后台审核";
                                } else if (v == 11) {
                                    str = "司机拒绝待后台审核";
                                } else if (v == 20) {
                                    str = "后台已审核待专线付款";
                                } else if (v == 21) {
                                    str = "后台拒绝申请";
                                } else if (v == 30) {
                                    str = "专线支付券额运输开始";
                                } else if (v == 40) {
                                    str = "司机确认到货待专线确认";
                                } else if (v == 50) {
                                    str = "专线确认到货待后台结款";
                                } else if (v == 90) {
                                    str = "后台确认结款，承运完成";
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
                                            id: 'cx_carriagecode',
                                            width: 160,
                                            labelWidth: 60,
                                            fieldLabel: '承运单号'
                                        },
                                        {
                                            xtype: 'textfield',
                                            id: 'cx_UserXM',
                                            width: 160,
                                            labelWidth: 60,
                                            fieldLabel: '承运专线'
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
                                                    text: '导出',
                                                    iconCls: 'view',
                                                    handler: function () {
                                                        DownloadFile("CZCLZ.CYMag.GetCYDListToFile", "承运单.xls", Ext.getCmp("cx_carriagecode").getValue(), Ext.getCmp("cx_UserXM").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
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

    new CYDView();

   
    DataBind();
})
//************************************主界面*****************************************