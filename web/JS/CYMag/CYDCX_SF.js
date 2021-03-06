﻿inline_include("approot/r/js/jquery-1.6.4.js");
//-----------------------------------------------------------全局变量-----------------------------------------------------------------
var pageSize = 15;
var cx_yhm;
//-----------------------------------------------------------数据源-------------------------------------------------------------------
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
        { name: 'carriagemoneynew' },
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
        { name: 'zx' },
        { name: 'isinvoice' },
        { name: 'modetype' },
        { name: 'modecoefficient' },
        { name: 'caruser' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        DataBind(nPage);
    }
});

//-----------------------------------------------------------页面方法-----------------------------------------------------------------
function DataBind(nPage) {
    CS('CZCLZ.CYMagSf.GetCYDList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_carriagecode").getValue(), Ext.getCmp("cx_UserXM").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), Ext.getCmp("cx_isinvoice").getValue(), Ext.getCmp("cx_carriagestatus").getValue(), Ext.getCmp("cx_ismoneypay").getValue());
}

//操作-确认
function QR(carriageid, carriagestatus, userid) {
    CS('CZCLZ.CYMagSf.QR', function (retVal) {
        if (retVal) {
            $.ajax({
                url: "http://jeremyda.fun:8010/api/zhaoyou/payAndInsure115",
                type: 'post',
                dataType: 'json',
                headers: { 'Content-Type': 'application/json' },
                data: JSON.stringify({
                    tradeCode: "payAndInsure",
                    carriageid: carriageid,
                    carriagestatus: 30,
                    userid: userid
                }),
                success: function (data) {
                    try {
                        if (data.success) {
                            Ext.MessageBox.alert('提示', "确认成功！");
                            DataBind(1);
                        }
                        else {
                            CS('CZCLZ.CYMagSf.QRTH', function (ret) {
                                if (ret) {
                                    Ext.MessageBox.alert('提示', data.details);
                                    DataBind(1);
                                }
                            }, CS.onError, carriageid, carriagestatus);
                        }
                    }
                    catch (e) {
                        CS('CZCLZ.CYMagSf.QRTH', function (ret) {
                            if (ret) {
                                Ext.MessageBox.alert('提示', data.details);
                                DataBind(1);
                            }
                        }, CS.onError, carriageid, carriagestatus);
                    }
                },
                error: function (j, t, e) {
                    CS('CZCLZ.CYMagSf.QRTH', function (ret) {
                        if (ret) {
                            console.log(j);
                            console.log("发生未知错误，错误:" + t, e);
                            DataBind(1);
                        }
                    }, CS.onError, carriageid, carriagestatus);

                }
            });
        }
    }, CS.onError, carriageid, carriagestatus);


}

function TH(carriageid, carriagestatus) {
    CS('CZCLZ.CYMagSf.TH', function (retVal) {
        DataBind(1);
    }, CS.onError, carriageid, carriagestatus);
}

function WC(carriageid, carriagestatus) {
    CS('CZCLZ.CYMagSf.WC', function (retVal) {
        DataBind(1);
    }, CS.onError, carriageid, carriagestatus);
}

function JJ(carriageid, carriagestatus) {
    var yjwin = new yjWin({ carriageid: carriageid, carriagestatus: carriagestatus });
    yjwin.show();
}

function YKDK(carriageid, carriagestatus) {
    Ext.MessageBox.confirm("提示", "是否油卡打款？", function (obj) {
        if (obj == "yes") {
            CS('CZCLZ.CYMagSf.YKDK', function (retVal) {
                DataBind(1);
            }, CS.onError, carriageid, carriagestatus);
        }
    });
}

function XJDK(carriageid, carriagestatus) {
    if (privilege("承运模块_三方承运单查询_打款")) {
        Ext.MessageBox.confirm("提示", "是否现付打款？", function (obj) {
            if (obj == "yes") {
                var passwin = new PassWin({ carriageid: carriageid, carriagestatus: carriagestatus, type: 0 });
                passwin.show();
            }
        });
    }
}

function XJDK1(carriageid, carriagestatus) {
    if (privilege("承运模块_三方承运单查询_打款")) {
        Ext.MessageBox.confirm("提示", "是否现付确认？", function (obj) {
            if (obj == "yes") {
                var passwin = new PassWin({ carriageid: carriageid, carriagestatus: carriagestatus, type: 3 });
                passwin.show();
            }
        });
    }
}

function YSFDK(carriageid, carriagestatus) {
    if (privilege("承运模块_三方承运单查询_打款")) {
        Ext.MessageBox.confirm("提示", "是否验收付打款？", function (obj) {
            if (obj == "yes") {
                var passwin = new PassWin({ carriageid: carriageid, carriagestatus: carriagestatus, type: 1 });
                passwin.show();
            }
        });

    }
}

function YSFDK1(carriageid, carriagestatus) {
    if (privilege("承运模块_三方承运单查询_打款")) {
        Ext.MessageBox.confirm("提示", "是否验收付确认？", function (obj) {
            if (obj == "yes") {
                var passwin = new PassWin({ carriageid: carriageid, carriagestatus: carriagestatus, type: 4 });
                passwin.show();
            }
        });

    }
}

function CKBD(carriageid) {
    CS('CZCLZ.CYMagSf.getInsure', function (retVal) {
        if (retVal) {
            var result = retVal.evalJSON();
            if (result.success) { window.open(result.url) } else {
                Ext.MessageBox.alert('提示', "查看失败");
            }
        }
    }, CS.onError, carriageid);
}

function QRDD(carriageid, carriagestatus) {
    if (privilege("承运模块_三方承运单查询_确认到达")) {
        Ext.MessageBox.confirm("提示", "是否确认到达确认？", function (obj) {
            if (obj == "yes") {
                CS('CZCLZ.CYMagSf.QRDD', function (retVal) {
                    DataBind(1);
                }, CS.onError, carriageid, carriagestatus);
            }
        });

    }
}

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
                            CS('CZCLZ.CYMagSf.JJ', function (retVal) {
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
                            CS('CZCLZ.CYMagSf.QRMM', function (retVal) {
                                if (retVal) {
                                    if (type == 0) {
                                        CS('CZCLZ.CYMagSf.XJDK', function (ret) {
                                            if (ret) {
                                                DataBind(1);
                                                Ext.MessageBox.alert('提示', "打款成功！");
                                            }
                                        }, CS.onError, carriageid, carriagestatus);
                                    } else if (type == 1) {
                                        CS('CZCLZ.CYMagSf.YSFDK', function (ret) {
                                            if (ret) {
                                                DataBind(1);
                                                Ext.MessageBox.alert('提示', "打款成功！");
                                            }
                                        }, CS.onError, carriageid, carriagestatus);
                                    } else if (type == 3) {
                                        CS('CZCLZ.CYMagSf.XJDK1', function (ret) {
                                            if (ret) {
                                                DataBind(1);
                                                Ext.MessageBox.alert('提示', "现付确认成功！");
                                            }
                                        }, CS.onError, carriageid, carriagestatus);
                                    } else if (type == 4) {
                                        CS('CZCLZ.CYMagSf.YSFDK1', function (ret) {
                                            if (ret) {
                                                DataBind(1);
                                                Ext.MessageBox.alert('提示', "验收付确认成功！");
                                            }
                                        }, CS.onError, carriageid, carriagestatus);
                                    }

                                } else {
                                    Ext.MessageBox.alert('提示', "密码错误！");
                                    Ext.getCmp("password").setValue()
                                }
                            }, CS.onError, Ext.getCmp("password").getValue());
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
Ext.define('CYDView', {
    extend: 'Ext.container.Viewport',

    layout: {
        type: 'fit'
    },

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'gridpanel',
                id: 'cyGrid',
                title: '',
                store: store,
                columnLines: true,
                selModel: Ext.create('Ext.selection.CheckboxModel', {
                    checkOnly: true
                }),
                columns: [
                    Ext.create('Ext.grid.RowNumberer'),//专线名称，司机手机、车牌、是否保险、保费、是否油卡付款、是否现金付款
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'carriageid',
                        sortable: false,
                        menuDisabled: true,
                        text: "操作",
                        width: 250,
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            var str = "";
                            if (record.data.carriagestatus == 10) {
                                str += " <a onclick='QR(\"" + value + "\",\"" + record.data.carriagestatus + "\",\"" + record.data.userid + "\");'>确认</a>";
                            } else if (record.data.carriagestatus == 11)
                                str += " <a onclick='TH(\"" + value + "\",\"" + record.data.carriagestatus + "\");'>退回</a> ";
                            if (record.data.modetype == 1) {
                                if ((record.data.carriagestatus == 30 || record.data.carriagestatus == 40 || record.data.carriagestatus == 50) && record.data.isoilpay == 0) {
                                    str += " <a onclick='YKDK(\"" + value + "\",\"" + record.data.carriagestatus + "\");'>油卡打款</a>";
                                }
                            }
                            if ((record.data.carriagestatus == 30 || record.data.carriagestatus == 40 || record.data.carriagestatus == 50) && record.data.ismoneypay == 0) {
                                str += " <a onclick='XJDK(\"" + value + "\",\"" + record.data.carriagestatus + "\");'>现付打款</a>";
                            }
                            if ((record.data.carriagestatus == 30 || record.data.carriagestatus == 40 || record.data.carriagestatus == 50) && record.data.ismoneypay == 0) {
                                str += " <a onclick='XJDK1(\"" + value + "\",\"" + record.data.carriagestatus + "\");'>现付确认</a>";
                            }
                            if (record.data.modetype == 1) {
                                if ((record.data.carriagestatus >= 50) && record.data.ismoneynewpay == 0) {
                                    str += " <a onclick='YSFDK(\"" + value + "\",\"" + record.data.carriagestatus + "\");'>验收付打款</a>";
                                }
                                if ((record.data.carriagestatus >= 50) && record.data.ismoneynewpay == 0) {
                                    str += " <a onclick='YSFDK1(\"" + value + "\",\"" + record.data.carriagestatus + "\");'>验收付确认</a>";
                                }
                            }

                            if ((record.data.carriagestatus == 30 || record.data.carriagestatus == 40 || record.data.carriagestatus == 50) && record.data.isarrive == 0) {
                                str += " <a onclick='QRDD(\"" + value + "\",\"" + record.data.carriagestatus + "\");'>确认到达</a>";
                            }

                            if (record.data.modetype == 1) {
                                if (record.data.carriagestatus == 50 && record.data.isoilpay == 1 && record.data.ismoneypay == 1 && record.data.ismoneynewpay == 1) {
                                    str += " <a onclick='WC(\"" + value + "\",\"" + record.data.carriagestatus + "\");'>完成</a>";
                                }
                            } else if (record.data.modetype == 2) {
                                if (record.data.carriagestatus == 50 && record.data.ismoneypay == 1) {
                                    str += " <a onclick='WC(\"" + value + "\",\"" + record.data.carriagestatus + "\");'>完成</a>";
                                }
                            }
                            if (record.data.carriagestatus >= 30) {
                                str += " <a onclick='CKBD(\"" + value + "\");'>查看保单</a> ";
                            }
                            if (record.data.carriagestatus >= 30 && record.data.carriagestatus < 90) {
                                var href = "http://47.110.134.105:8010/location?carriageid=" + value;
                                str += " <a href=" + href + " target='_blank'>查看轨迹</a> ";
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
                        dataIndex: 'isinvoice',
                        sortable: false,
                        menuDisabled: true,
                        text: "票据状态",
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
                        dataIndex: 'zx',
                        sortable: false,
                        menuDisabled: true,
                        text: "上游客户（三方名称）",
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
                        text: "支付运费",
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
                        text: "司机现金",
                        width: 100
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'carriagemoneynew',
                        sortable: false,
                        menuDisabled: true,
                        text: "验收付现金",
                        width: 100,
                        hidden: true
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
                                str = value / 100
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
                        hidden: true,
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
                         dataIndex: 'carriagecode',
                         sortable: false,
                         menuDisabled: true,
                         text: "承运单号",
                         width: 200
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
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'caruser',
                        sortable: false,
                        menuDisabled: true,
                        text: "车主账号",
                        width: 100,
                        hidden: true
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
                                        { 'val': 1, 'txt': '已开' },
                                        { 'val': 2, 'txt': '已收票' }]

                                }),
                                queryMode: 'local',
                                displayField: 'txt',
                                valueField: 'val',
                                value: ''
                            },
                            {
                                xtype: 'combobox',
                                id: 'cx_carriagestatus',
                                width: 250,
                                fieldLabel: '订单状态',
                                editable: false,
                                labelWidth: 60,
                                store: Ext.create('Ext.data.Store', {
                                    fields: [
                                        { name: 'val' },
                                        { name: 'txt' }
                                    ],
                                    data: [
                                        { 'val': "", 'txt': '全部' },
                                        { 'val': 0, 'txt': '已申请待后台审核' },
                                        { 'val': 10, 'txt': '司机确认待后台审核' },
                                        { 'val': 11, 'txt': '司机拒绝待后台审核' },
                                        { 'val': 20, 'txt': '后台已审核待专线付款' },
                                        { 'val': 21, 'txt': '后台拒绝申请' },
                                        { 'val': 30, 'txt': '专线支付券额运输开始' },
                                        { 'val': 40, 'txt': '司机确认到货待专线确认' },
                                        { 'val': 50, 'txt': '专线确认到货待后台结款' },
                                        { 'val': 90, 'txt': '后台确认结款，承运完成' }]

                                }),
                                queryMode: 'local',
                                displayField: 'txt',
                                valueField: 'val',
                                value: ''
                            },
                            {
                                xtype: 'combobox',
                                id: 'cx_ismoneypay',
                                width: 180,
                                fieldLabel: '是否支付',
                                editable: false,
                                labelWidth: 60,
                                store: Ext.create('Ext.data.Store', {
                                    fields: [
                                        { name: 'val' },
                                        { name: 'txt' }
                                    ],
                                    data: [
                                        { 'val': "", 'txt': '全部' },
                                        { 'val': "1", 'txt': '已支付现付' },
                                        { 'val': "0", 'txt': '未支付现付' }]

                                }),
                                queryMode: 'local',
                                displayField: 'txt',
                                valueField: 'val',
                                value: ''
                            },
                            {
                                xtype: 'buttongroup',
                                items: [
                                    {
                                        xtype: 'button',
                                        iconCls: 'search',
                                        text: '查询',
                                        handler: function () {
                                            if (privilege("承运模块_三方承运单查询_查看")) {
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
                                        text: '导出',
                                        iconCls: 'view',
                                        handler: function () {
                                            if (privilege("承运模块_三方承运单查询_导出")) {
                                                DownloadFile("CZCLZ.CYMagSf.GetCYDListToFile", "承运单.xls", Ext.getCmp("cx_carriagecode").getValue(), Ext.getCmp("cx_UserXM").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), Ext.getCmp("cx_isinvoice").getValue(), Ext.getCmp("cx_carriagestatus").getValue(), Ext.getCmp("cx_ismoneypay").getValue());
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
                                        text: '开票',
                                        id: 'kaipiao',
                                        handler: function () {
                                            if (privilege("承运模块_三方承运单查询_开票")) {
                                                var grid = Ext.getCmp('cyGrid');
                                                var gx = grid.getSelectionModel().getSelection();
                                                if (gx.length == 0) {
                                                    Ext.Msg.show({
                                                        title: '提示',
                                                        msg: "请勾选需要开票的承运单。",
                                                        buttons: Ext.MessageBox.OK,
                                                        icon: Ext.MessageBox.INFO
                                                    });
                                                    return;
                                                }

                                                var arr = [];
                                                for (var i = 0; i < gx.length; i++) {
                                                    arr.push(gx[i].data);
                                                }
                                                CS('CZCLZ.CYMagSf.Cykp', function (retVal) {
                                                    Ext.Msg.show({
                                                        title: '提示',
                                                        msg: "开票成功。",
                                                        buttons: Ext.MessageBox.OK,
                                                        icon: Ext.MessageBox.INFO
                                                    });
                                                    DataBind();
                                                }, CS.onError, arr, 1);
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
                                        text: '收票',
                                        id: 'shoupiao',
                                        handler: function () {
                                            if (privilege("承运模块_三方承运单查询_收票")) {
                                                var grid = Ext.getCmp('cyGrid');
                                                var gx = grid.getSelectionModel().getSelection();
                                                if (gx.length == 0) {
                                                    Ext.Msg.show({
                                                        title: '提示',
                                                        msg: "请勾选需要收票的承运单。",
                                                        buttons: Ext.MessageBox.OK,
                                                        icon: Ext.MessageBox.INFO
                                                    });
                                                    return;
                                                }

                                                var arr = [];
                                                for (var i = 0; i < gx.length; i++) {
                                                    arr.push(gx[i].data);
                                                }
                                                CS('CZCLZ.CYMagSf.Cykp', function (retVal) {
                                                    Ext.Msg.show({
                                                        title: '提示',
                                                        msg: "收票成功。",
                                                        buttons: Ext.MessageBox.OK,
                                                        icon: Ext.MessageBox.INFO
                                                    });
                                                    DataBind();
                                                }, CS.onError, arr, 2);
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

Ext.onReady(function () {
    new CYDView();
    if (!privilegeBo("承运模块_三方承运单查询_开票")) {
        Ext.getCmp('kaipiao').disable(true);
    }

    DataBind();
})