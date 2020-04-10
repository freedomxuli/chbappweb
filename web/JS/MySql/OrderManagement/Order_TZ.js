//-------------------------------------------------------全局变量----------------------------------------------------------
var pageSize = 15;
var opersUserPageSize = 15;
var carrierPageSize = 15;
var purPage = 1;
//-------------------------------------------------------数据源------------------------------------------------------------
var storeOrder = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
        { name: 'usernamea' },//操作员
        { name: 'userCZY' },//操作员
                { name: 'userGYS' },//操作员
        { name: 'userZCSJ' },//操作员


        { name: 'userGYS_username' },
        { name: 'userGYS_vehiclenumber' },
        { name: 'userGYS_identitydocumentnumber' },

        { name: 'userZCSJ_username' },
        { name: 'userZCSJ_vehiclenumber' },
        { name: 'userZCSJ_identitydocumentnumber' },
        { name: 'userZCSJ_usertel' },

         { name: 'vehicletype' },
          { name: 'vehiclelength' },


 

        { name: 'operatorid' },//订单ID
        { name: 'shippingnoteid' },//订单ID
        { name: 'username' },//厂家
        { name: 'shippingnoteadddatetime' },//订单时间
        { name: 'shippingnotestatuscode' },//订单状态
        { name: 'shippingnotestatusname' },//订单状态
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
        //{ name: 'carriername' },//承运方
        { name: 'istakegoods' },//是否提货
        { name: 'istakegoodsname' },//是否提货
        { name: 'isdelivergoods' },//是否送货
        { name: 'isdelivergoodsname' },//是否送货
        { name: 'totalmonetaryamount' },//预付运费
        { name: 'actualcompanypay' },//企业实际运费
        { name: 'actualmoney' },//实际运费
        { name: 'memo' },//备注
        { name: 'actualnooilmoney' },//实际未用油

        //{ name: 'isabnormal' },
        { name: 'abnormalmemo' },
        { name: 'gpscompany' },
        { name: 'gpsdenno' },
        { name: 'doublepaynum' },
        { name: 'carrierid' },//供应商
        { name: 'carriername' },//供应商
        { name: 'driverid' },//承运司机
        { name: 'cysj' },//承运司机
        { name: 'takegoodsdriver' }, //装车司机
        { name: 'takegoodsdrivername' }, //装车司机
        { name: 'offerid' },
        { name: 'isabnormal' },

        { name: 'estimatemoney' },
        { name: 'actualmoney' },
        { name: 'actualnooilmoney' },
        { name: 'actualcompanypay' },
        { name: 'actualcompletemoney' },
        { name: 'actualtaxmoney' },
        { name: 'actualcostmoney' },




        { name: 'actualmoneystatus' },
        { name: 'tuoyunorder' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        GetOrder(nPage);
    }
});

var sel_store = createSFW4Store({
    data: [],
    pageSize: opersUserPageSize,
    total: 1,
    currentPage: 1,
    fields: [
        { name: 'UserID' },
        { name: 'UserName' },
        { name: 'Password' },
        { name: 'roleId' },
        { name: 'roleName' },
        { name: 'UserXM' },
        { name: 'ClientKind' },
        { name: 'Address' },
        { name: 'UserTel' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        sel_getUser(nPage);
    }
});

var doublePayStore = Ext.create('Ext.data.Store', {
    fields: [
        { name: 'addtime' },
        { name: 'companydoublepay' },
        { name: 'paystatus' },
        { name: 'memo' },
        { name: 'doublepayid' }
    ]
});

var locationInfoStore = Ext.create('Ext.data.Store', {
    fields: [
        { name: 'locationid' },
        { name: 'vehiclenumber' },
        { name: 'drivertel' },
        { name: 'drivername' },
        { name: 'getlocationtime' },
        { name: 'address' },
        { name: 'memo' }
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

var planStore = Ext.create('Ext.data.Store', {
    fields: [
        { name: 'shippingnotestatuscode' },
        { name: 'username' },
        { name: 'recordtype' },
        { name: 'recordmemo' }
    ]
});

var OrderUsersStore = createSFW4Store({
    data: [],
    pageSize: carrierPageSize,
    total: 1,
    currentPage: 1,
    fields: [
        { name: 'userid' },
        { name: 'username' },
        { name: 'usertel' },
        { name: 'usertype' },
        { name: 'seltype' },
        { name: 'carriertel' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        GetUserByPage(nPage);
    }
});
//-------------------------------------------------------页面方法----------------------------------------------------------
//获取订单列表
function GetOrder(nPage) {
    purPage = nPage;
    CS('CZCLZ.Order.GetOrderList', function (retVal) {
        storeOrder.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });

    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), Ext.getCmp("cx_changjia").getValue(), Ext.getCmp("cx_shippingnotenumber").getValue(), Ext.getCmp("cx_gys").getValue(), Ext.getCmp("cx_zcsj").getValue(), Ext.getCmp("cx_cysj").getValue());
}

//指定操作员
function EditOperatorUser(orderid) {
    if (privilege("一键发包模块_订单管理-订单列表_指定操作员")) {
        var c_window = new Ext.Window({
            extend: 'Ext.window.Window',
            viewModel: {
                type: 'mywindow'
            },
            autoShow: true,
            height: 500,
            id: "operatorUserWin",
            width: 500,
            layout: 'fit',
            title: "指定操作员",
            modal: true,
            items: [
                {
                    xtype: 'panel',
                    layout: 'column',
                    bodyStyle: 'overflow-x:hidden;overflow-y:auto;',
                    buttonAlign: 'center',
                    layout: 'fit',
                    buttons: [
                        {
                            text: '保存',
                            handler: function () {
                                var grid = Ext.getCmp("operatorUserGrid");
                                var rds = grid.getSelectionModel().getSelection();
                                if (rds.length == 0) {
                                    Ext.Msg.show({
                                        title: '提示',
                                        msg: '请选择最少1位操作员!',
                                        buttons: Ext.MessageBox.OK,
                                        icon: Ext.MessageBox.INFO
                                    });
                                    return;
                                }

                                Ext.MessageBox.confirm('添加提示', '是否要添加数据!', function (obj) {
                                    if (obj == "yes") {
                                        var idlist = [];
                                        for (var n = 0, len = rds.length; n < len; n++) {
                                            var rd = rds[n];

                                            idlist.push(rd.get("UserID") + "," + rd.get("UserXM"));
                                        }
                                        CS('CZCLZ.Order.UpdateOrderOperatorList', function (retVal) {
                                            if (retVal) {
                                                GetOrder(purPage);
                                                Ext.getCmp("operatorUserWin").close();

                                            }
                                        }, CS.onError, orderid, idlist);
                                    } else {
                                        return;
                                    }
                                });
                            }
                        },
                        {
                            text: '关闭',
                            handler: function () {
                                Ext.getCmp("operatorUserWin").close();
                            }
                        }
                    ],
                    items: [
                        {
                            xtype: 'gridpanel',
                            border: 1,
                            id: 'operatorUserGrid',
                            columnLines: true,
                            columnWidth: 1,
                            store: sel_store,
                            selModel: Ext.create('Ext.selection.CheckboxModel', {
                                singleSelect: true
                            }),
                            columns: [
                                Ext.create('Ext.grid.RowNumberer', { width: 40 }),
                                {
                                    dataIndex: 'UserName',
                                    text: '账号',
                                    flex: 1,
                                    align: "center",
                                    sortable: false,
                                    menuDisabled: true
                                },
                                {
                                    dataIndex: 'UserXM',
                                    flex: 1,
                                    text: '名称',
                                    align: "center",
                                    sortable: false,
                                    menuDisabled: true
                                },
                                {
                                    dataIndex: 'UserID',
                                    hidden: true,
                                    text: '',
                                    align: "center",
                                    sortable: false,
                                    menuDisabled: true
                                }
                            ],
                            dockedItems: [
                                {
                                    xtype: 'toolbar',
                                    dock: 'top',
                                    items: [

                                        {
                                            xtype: 'textfield',
                                            id: 'sel_yhm',
                                            width: 140,
                                            labelWidth: 50,
                                            fieldLabel: '用户名'
                                        },
                                        {
                                            xtype: 'textfield',
                                            id: 'sel_xm',
                                            width: 160,
                                            labelWidth: 70,
                                            fieldLabel: '真实姓名'
                                        },
                                        {
                                            xtype: 'button',
                                            text: '查询',
                                            handler: function () {
                                                sel_getUser(1);
                                            }
                                        }

                                    ]
                                },
                                {
                                    xtype: 'pagingtoolbar',
                                    displayInfo: true,
                                    store: sel_store,
                                    dock: 'bottom'
                                }
                            ]
                        }
                    ]
                }
            ]
        });
        c_window.show();
        sel_getUser(1);
    }
}

//获取操作员
function sel_getUser(nPage) {
    CS('CZCLZ.YHGLClass.GetUserList', function (retVal) {
        sel_store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });

    }, CS.onError, nPage, pageSize, "0c11bf4b-d1ca-4e91-b384-504c29991076", Ext.getCmp("sel_yhm").getValue(), Ext.getCmp("sel_xm").getValue());
}

//异常解决
function EditAbnormal(orderid, isab) {
    var titl = isab == "null" ? "填写异常" : (isab == 0 ? "查看异常" : "处理异常");

    var r = storeOrder.findRecord("shippingnoteid", orderid).data;
    var Win = new abnormalWin({ shippingnoteid: orderid, title: titl });
    Win.show(null, function () {
        if (isab == "null") {
            Ext.getCmp('errorBtn').hide();
            Ext.getCmp('errorSaveBtn').show();
        } else if (isab == 1) {
            Ext.getCmp('errorBtn').show();
            Ext.getCmp('errorSaveBtn').hide();
        } else if (isab == 0) {
            Ext.getCmp('errorBtn').hide();
            Ext.getCmp('errorSaveBtn').hide();
        }
        var form = Ext.getCmp('abnormalForm');
        form.form.setValues(r);
    });
}

//填写GBS信息
function EditGPS(orderid) {
    var r = storeOrder.findRecord("shippingnoteid", orderid).data;
    var Win = new gbsWin({ shippingnoteid: orderid });
    Win.show(null, function () {
        var form = Ext.getCmp('gbsForm');
        form.form.setValues(r);
    });

}

//查看轨迹
function SearchGJ(orderid) {
    CS('CZCLZ.Order.GetOrderGJ', function (retVal) {
        console.log(retVal);
        window.open(retVal);
    }, CS.onError, orderid);
}

//查看补单情况
function ShowDoublePay(orderid) {
    var win = new doublePayWin({ shippingnoteid: orderid });
    win.show(null, function () {
        CS('CZCLZ.Order.GetDoublePayLine', function (retVal) {
            doublePayStore.loadData(retVal);
        }, CS.onError, orderid);
    });
}

//证据查看
function ShowDoublePayFj(doublepayid) {
    var fjwin = new payFjWin();
    fjwin.show(null, function () {
        CS('CZCLZ.Order.GetDoublePayFj', function (retVal) {
            var html = "";
            html += '<div style="overflow-x: auto; overflow-y: auto; height: 680px; width:100%;">';
            html += '<table width="100%" border="0" cellspacing="0" cellpadding="0px" >';
            for (var i = 0; i < retVal.length; i++) {
                var type = retVal[i]["type"];
                var mc = retVal[i]["mc"];
                var url = retVal[i]["url"];

                if (i % 2 == 0)
                    html += '  <tr>';

                if (type == 1) {
                    html += '    <td align="center"><img width="200px" height="200px"  src="' + url + '"  width="100%" alt="' + mc + '" /></td>';
                } else if (type == 2) {
                    html += '    <td align="center"><video width="520" height="440" controls>';
                    html += '<source src="' + url + '" type="video/mp4">';
                    html += '您的浏览器不支持 HTML5 video 标签。';
                    html += '</video></td>';
                }

                if (i % 2 != 0)
                    html += '  </tr>';

            }
            html += '</table></div>';
            Ext.getCmp('picorvideo').setText(html, false);
        }, CS.onError, doublepayid);

    });
}

//填写定位信息
function EditLocationInfoLine(orderid) {
    if (privilege("一键发包模块_订单管理-订单列表_填写定位信息")) {
        var win = new locationInfoWin({ shippingnoteid: orderid });
        win.show(null, function () {
            CS('CZCLZ.Order.GetLocationInfoLine', function (retVal) {
                locationInfoStore.loadData(retVal);
            }, CS.onError, orderid);
        });
    }
}

//修改定位信息
function EditLocationInfo(id, orderid) {
    var r = locationInfoStore.findRecord("locationid", id).data;
    var win = new addLocationInfoWin({ shippingnoteid: orderid });
    win.show(null, function () {
        var form = Ext.getCmp('addLocationInfoform');
        form.form.setValues(r);
    });
}

//删除定位信息
function DelLocationInfo(locationid, orderid) {
    Ext.MessageBox.confirm('删除提示', '是否要删除数据!', function (obj) {
        if (obj == "yes") {
            CS('CZCLZ.Order.DelLocationInfo', function (retVal) {
                if (retVal) {
                    locationInfoStore.loadData(retVal);
                }
            }, CS.onError, locationid, orderid);
        }
        return;
    });
}

//确认到达
function ConfirmBtn(orderid) {
    if (privilege("一键发包模块_订单管理-订单列表_确认到达")) {
        CS('CZCLZ.Order.OrderConfirm', function (retVal) {
            if (retVal) {
                GetOrder(storeOrder.currentPage);
            }
        }, CS.onError, orderid);
    }
}

//上传回执单
function UploadingHZD(orderid) {
    if (privilege("一键发包模块_订单管理-订单列表_上传回执单")) {
        var win = new receiptInfoWin({ shippingnoteid: orderid });
        win.show(null, function () {
            GetReceiptInfoLine(orderid);
        });
    }
}

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

//获取回执单附件列表
function GetReceiptInfoLine(orderid) {
    CS('CZCLZ.Order.GetReceiptInfoList', function (retVal) {
        if (retVal) {
            receiptInfoStore.loadData(retVal);
        }
    }, CS.onError, orderid);
}

//删除回执单附件
function DelReceiptInfo(id, orderid) {
    Ext.MessageBox.confirm('删除提示', '是否要删除数据!', function (obj) {
        if (obj == "yes") {
            CS('CZCLZ.Order.DelReceiptInfo', function (retVal) {
                if (retVal) {
                    GetReceiptInfoLine(orderid);
                }
            }, CS.onError, id, orderid);
        }
        return;
    });
}

//上传回执单附件
function UploadFJ(orderid) {
    var win = new phWin({ shippingnoteid: orderid });
    win.show();
}

//订单进度
function ShowOrderStatus(orderid) {
    var statusWin = new orderStatusWin();
    statusWin.show(null, function () {
        CS('CZCLZ.Order.GetOrderRecord', function (retVal) {
            planStore.loadData(retVal);
        }, CS.onError, orderid);
    });
}

var myWin;
//选择供应商
function SelSupplier(orderid, uid) {
    myWin = new Ext.Window({
        extend: 'Ext.window.Window',
        viewModel: {
            type: 'mywindow'
        },
        height: 500,
        width: 460,
        layout: 'fit',
        title: "选择供应商",
        modal: true,
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
                        store: OrderUsersStore,
                        columns: [Ext.create('Ext.grid.RowNumberer'),
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'username',
                            sortable: false,
                            menuDisabled: true,
                            text: "供应商名称",
                            flex: 1
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'carriertel',
                            sortable: false,
                            menuDisabled: true,
                            text: "供应商电话",
                            width: 90
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'usertype',
                            sortable: false,
                            menuDisabled: true,
                            text: "供应商类型",
                            width: 80,
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                var str = "";//用户类型（0：平台人员；1：三方app；2：专线-供应商；3：企业发货人账号；4：承运司机；）
                                switch (value) {
                                    case 2:
                                        str = "专线";
                                        break;
                                    case 4:
                                        str = "整车司机";
                                        break;
                                    default:
                                        str = "";
                                }

                                return str;
                            }
                        },
                        {
                            text: '操作',
                            dataIndex: 'userid',
                            width: 90,
                            sortable: false,
                            menuDisabled: true,
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                var str;
                                if (value != uid) {
                                    str = "<a onclick='UpdateOrderInfo(\"" + orderid + "\",\"" + value + "\",\"GYS\");'>选择</a>";
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
                                        id: 'cx_mc',
                                        width: 180,
                                        labelWidth: 80,
                                        fieldLabel: '供应商名称'
                                    },
                                    {
                                        xtype: 'button',
                                        iconCls: 'search',
                                        text: '查询',
                                        handler: function () {
                                            GetUserByPage(1, "GYS");
                                        }
                                    }
                                ]
                            },
                            {
                                xtype: 'pagingtoolbar',
                                displayInfo: true,
                                store: OrderUsersStore,
                                dock: 'bottom'
                            }
                        ]
                    }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '关闭',
                        handler: function () {
                            myWin.close();
                        }
                    }
                ]
            }
        ]
    });
    myWin.show(null, function () {
        GetUserByPage(1, "GYS");
    });
}

//选择承运司机
function SelCarriageUser(orderid, uid) {
    myWin = new Ext.Window({
        extend: 'Ext.window.Window',
        viewModel: {
            type: 'mywindow'
        },
        height: 500,
        width: 460,
        layout: 'fit',
        title: "选择承运司机",
        modal: true,
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
                        store: OrderUsersStore,
                        columns: [Ext.create('Ext.grid.RowNumberer'),
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'username',
                            sortable: false,
                            menuDisabled: true,
                            text: "司机名称",
                            flex: 1
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'usertel',
                            sortable: false,
                            menuDisabled: true,
                            text: "司机电话",
                            width: 90
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'usertype',
                            sortable: false,
                            menuDisabled: true,
                            text: "司机类型",
                            width: 90,
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                var str = "";//用户类型（0：平台人员；1：三方app；2：专线-供应商；3：企业发货人账号；4：承运司机；）
                                switch (value) {
                                    case 4:
                                        str = "承运司机";
                                        break;
                                }

                                return str;
                            }
                        },
                        {
                            text: '操作',
                            dataIndex: 'userid',
                            width: 90,
                            sortable: false,
                            menuDisabled: true,
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                var str;
                                if (value != uid) {
                                    str = "<a onclick='UpdateOrderInfo(\"" + orderid + "\",\"" + value + "\",\"CYSJ\");'>选择</a>";
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
                                        id: 'cx_mc',
                                        width: 180,
                                        labelWidth: 80,
                                        fieldLabel: '司机名称'
                                    },
                                    {
                                        xtype: 'button',
                                        iconCls: 'search',
                                        text: '查询',
                                        handler: function () {
                                            GetUserByPage(1, "CYSJ");
                                        }
                                    }
                                ]
                            },
                            {
                                xtype: 'pagingtoolbar',
                                displayInfo: true,
                                store: OrderUsersStore,
                                dock: 'bottom'
                            }
                        ]
                    }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '关闭',
                        handler: function () {
                            myWin.close();
                        }
                    }
                ]
            }
        ]
    });
    myWin.show(null, function () {
        GetUserByPage(1, "CYSJ");
    });
}

//选择装车司机
function SelTakeGoodsDriver(orderid, uid) {
    myWin = new Ext.Window({
        extend: 'Ext.window.Window',
        viewModel: {
            type: 'mywindow'
        },
        height: 500,
        width: 460,
        layout: 'fit',
        title: "选择装车司机",
        modal: true,
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
                        store: OrderUsersStore,
                        columns: [Ext.create('Ext.grid.RowNumberer'),
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'username',
                            sortable: false,
                            menuDisabled: true,
                            text: "司机名称",
                            flex: 1
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'usertel',
                            sortable: false,
                            menuDisabled: true,
                            text: "司机电话",
                            width: 90
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'usertype',
                            sortable: false,
                            menuDisabled: true,
                            text: "司机类型",
                            width: 90,
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                var str = "";//用户类型（0：平台人员；1：三方app；2：专线-供应商；3：企业发货人账号；4：承运司机；）
                                switch (value) {
                                    case 4:
                                        str = "装车司机";
                                        break;
                                }

                                return str;
                            }
                        },
                        {
                            text: '操作',
                            dataIndex: 'userid',
                            width: 90,
                            sortable: false,
                            menuDisabled: true,
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                var str;
                                if (value != uid) {
                                    str = "<a onclick='UpdateOrderInfo(\"" + orderid + "\",\"" + value + "\",\"ZCSJ\");'>选择</a>";
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
                                        id: 'cx_mc',
                                        width: 180,
                                        labelWidth: 80,
                                        fieldLabel: '司机名称'
                                    },
                                    {
                                        xtype: 'button',
                                        iconCls: 'search',
                                        text: '查询',
                                        handler: function () {
                                            GetUserByPage(1, "ZCSJ");
                                        }
                                    }
                                ]
                            },
                            {
                                xtype: 'pagingtoolbar',
                                displayInfo: true,
                                store: OrderUsersStore,
                                dock: 'bottom'
                            }
                        ]
                    }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '关闭',
                        handler: function () {
                            myWin.close();
                        }
                    }
                ]
            }
        ]
    });
    myWin.show(null, function () {
        GetUserByPage(1, "ZCSJ");
    });
}

//获取供应商/承运司机
function GetUserByPage(nPage, typeStr) {
    CS('CZCLZ.Order.GetUserByPage', function (retVal) {
        OrderUsersStore.setData({
            data: retVal.dt,
            pageSize: carrierPageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });

    }, CS.onError, nPage, carrierPageSize, Ext.getCmp("cx_mc").getValue(), typeStr);
}

//更新订单信息
function UpdateOrderInfo(orderid, userid, typeStr) {
    if (typeStr == "GYS") {
        CS('CZCLZ.Order.UpdateOrderCarrierid', function (retVal) {
            if (retVal) {
                GetOrder(purPage);
                GetUserByPage(OrderUsersStore.currentPage, typeStr);
            }
        }, CS.onError, orderid, userid);
    } else if (typeStr == "CYSJ") {
        CS('CZCLZ.Order.UpdateJoborder', function (retVal) {
            if (retVal) {
                GetOrder(purPage);
                GetUserByPage(OrderUsersStore.currentPage, typeStr);
            }
        }, CS.onError, orderid, userid);
    } else if (typeStr == "ZCSJ") {
        CS('CZCLZ.Order.UpdateOrderTakeGoodsDriver', function (retVal) {
            if (retVal) {
                GetOrder(purPage);
                GetUserByPage(OrderUsersStore.currentPage, typeStr);
            }
        }, CS.onError, orderid, userid);
    }
    myWin.close();
}

//补单价格变更
function UpdateDoublePrice(orderid, id) {
    let pWin = new priceWin({ shippingnoteid: orderid, offerid: id, title: "补单价格变更" });
    pWin.show(null, function () {
        CS('CZCLZ.Order.GetOrderOffer', function (retVal) {
            if (retVal) {
                Ext.getCmp('SaveDoublePriceBtn').show();
                Ext.getCmp('SavePriceBtn').hide();
                Ext.getCmp('ConfirmPriceBtn').hide();
                let form = Ext.getCmp('priceForm');
                form.form.setValues(retVal[0]);
            }
        }, CS.onError, id);
    });
}

//实际价格变更
function UpdatePrice(orderid, id) {
    let pWin = new priceWin({ shippingnoteid: orderid, offerid: id, title: "实际价格变更" });
    pWin.show(null, function () {
        CS('CZCLZ.Order.GetOrderOffer', function (retVal) {
            if (retVal) {
                Ext.getCmp('SaveDoublePriceBtn').hide();
                Ext.getCmp('SavePriceBtn').show();
                Ext.getCmp('ConfirmPriceBtn').hide();
                let form = Ext.getCmp('priceForm');
                form.form.setValues(retVal[0]);
            }
        }, CS.onError, id);
    });
}

//实际价格确认
function ConfirmPrice(orderid, id) {
    let pWin = new priceWin({ shippingnoteid: orderid, offerid: id, title: "实际价格确认" });
    pWin.show(null, function () {
        CS('CZCLZ.Order.GetOrderOffer', function (retVal) {
            if (retVal) {
                Ext.getCmp('SaveDoublePriceBtn').hide();
                Ext.getCmp('SavePriceBtn').hide();
                Ext.getCmp('ConfirmPriceBtn').show();
                let form = Ext.getCmp('priceForm');
                form.form.setValues(retVal[0]);
            }
        }, CS.onError, id);
    });
}

//-------------------------------------------------------价格变更弹出界面--------------------------------------------------
Ext.define('priceWin', {
    extend: 'Ext.window.Window',
    id: 'priceWinId',
    height: 420,
    width: 420,
    layout: {
        type: 'fit'
    },
    modal: true,
    initComponent: function () {
        var me = this;
        let orderid = me.shippingnoteid;
        let id = me.offerid;
        me.items = [
            {
                xtype: 'form',
                bodyPadding: 10,
                id: 'priceForm',
                items: [
                    {
                        xtype: 'combobox',
                        fieldLabel: '下游对象',
                        labelWidth: 90,
                        anchor: '100%',
                        editable: false,
                        name: 'actualcarriertype',
                        store: Ext.create('Ext.data.Store', {
                            fields: [
                                { name: 'value' },
                                { name: 'name' }
                            ],
                            data: [
                                { 'value': 0, 'name': '司机' },
                                { 'value': 1, 'name': '专线' }
                            ]
                        }),
                        queryMode: 'local',
                        displayField: 'name',
                        valueField: 'value',
                        readOnly: true
                    },
                    {
                        xtype: 'combobox',
                        fieldLabel: '是否有提货费',
                        labelWidth: 90,
                        anchor: '100%',
                        editable: false,
                        name: 'istakegoodsbyactual',
                        store: Ext.create('Ext.data.Store', {
                            fields: [
                                { name: 'value' },
                                { name: 'name' }
                            ],
                            data: [
                                { 'value': 0, 'name': '提货' },
                                { 'value': 1, 'name': '不提' }
                            ]
                        }),
                        queryMode: 'local',
                        displayField: 'name',
                        valueField: 'value',
                        readOnly: true
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '提货费',
                        name: 'actualtakegoodsmoney',
                        labelWidth: 90,
                        anchor: '100%',
                        readOnly: true
                    },
                    {
                        xtype: 'combobox',
                        fieldLabel: '是否含票',
                        labelWidth: 90,
                        anchor: '100%',
                        editable: false,
                        name: 'isvotebyactual',
                        store: Ext.create('Ext.data.Store', {
                            fields: [
                                { name: 'value' },
                                { name: 'name' }
                            ],
                            data: [
                                { 'value': 0, 'name': '含票' },
                                { 'value': 1, 'name': '不含' }
                            ]
                        }),
                        queryMode: 'local',
                        displayField: 'name',
                        valueField: 'value',
                        readOnly: true
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '含票金额',
                        name: 'actualvotemoney',
                        labelWidth: 90,
                        anchor: '100%',
                        readOnly: true
                    },
                    {
                        xtype: 'combobox',
                        fieldLabel: '是否含油',
                        labelWidth: 90,
                        anchor: '100%',
                        editable: false,
                        name: 'isoilbyactual',
                        store: Ext.create('Ext.data.Store', {
                            fields: [
                                { name: 'value' },
                                { name: 'name' }
                            ],
                            data: [
                                { 'value': 0, 'name': '含油' },
                                { 'value': 1, 'name': '不含油' }
                            ]
                        }),
                        queryMode: 'local',
                        displayField: 'name',
                        valueField: 'value',
                        readOnly: true
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '含油金额',
                        name: 'actualoilmoney',
                        labelWidth: 90,
                        anchor: '100%',
                        readOnly: true
                    },
                    {
                        xtype: 'numberfield',
                        fieldLabel: '实际企业报价',
                        labelWidth: 90,
                        name: 'actualcompanypay',
                        allowDecimals: true,
                        allowNegative: false,
                        minValue: 0,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: 'numberfield',
                        fieldLabel: '实际下游成本',
                        labelWidth: 90,
                        name: 'actualmoney',
                        allowDecimals: true,
                        allowNegative: false,
                        minValue: 0,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: 'numberfield',
                        fieldLabel: '实际综合成本',
                        labelWidth: 90,
                        name: 'actualcompletemoney',
                        allowDecimals: true,
                        allowNegative: false,
                        minValue: 0,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: 'numberfield',
                        fieldLabel: '实际税费成本',
                        labelWidth: 90,
                        name: 'actualtaxmoney',
                        allowDecimals: true,
                        allowNegative: false,
                        minValue: 0,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: 'numberfield',
                        fieldLabel: '实际资金成本',
                        labelWidth: 90,
                        name: 'actualcostmoney',
                        allowDecimals: true,
                        allowNegative: false,
                        minValue: 0,
                        allowBlank: false,
                        anchor: '100%'
                    }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '补单',
                        iconCls: 'dropyes',
                        id: 'SaveDoublePriceBtn',
                        handler: function () {
                            let form = Ext.getCmp('priceForm');
                            if (form.form.isValid()) {
                                var values = form.form.getValues(false);
                                CS('CZCLZ.Order.UpdateDoubleOrderOffer', function (retVal) {
                                    if (retVal) {
                                        GetOrder(purPage);
                                        Ext.MessageBox.alert('提示', "补单价格变更成功！");
                                        Ext.getCmp("priceWinId").close();
                                    }
                                }, CS.onError, orderid, id, values);
                            }
                        }
                    },
                    {
                        text: '保存',
                        iconCls: 'dropyes',
                        id: 'SavePriceBtn',
                        handler: function () {
                            let form = Ext.getCmp('priceForm');
                            if (form.form.isValid()) {
                                var values = form.form.getValues(false);
                                CS('CZCLZ.Order.UpdateOrderOffer', function (retVal) {
                                    if (retVal) {
                                        GetOrder(purPage);
                                        Ext.MessageBox.alert('提示', "实际价格变更成功！");
                                        Ext.getCmp("priceWinId").close();
                                    }
                                }, CS.onError, orderid, id, values);
                            }
                        }
                    },
                    {
                        text: '确认',
                        id: 'ConfirmPriceBtn',
                        handler: function () {
                            CS('CZCLZ.Order.ConfirmOrderOffer', function (retVal) {
                                if (retVal) {
                                    GetOrder(purPage);
                                    Ext.getCmp("priceWinId").close();
                                }
                            }, CS.onError, orderid);
                        }
                    },
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
//-------------------------------------------------------订单进度弹出界面--------------------------------------------------
Ext.define('orderStatusWin', {
    extend: 'Ext.window.Window',

    height: 500,
    width: 700,
    layout: {
        type: 'fit'
    },
    title: '订单进度明细',
    modal: true,

    initComponent: function () {
        var me = this;
        Ext.QuickTips.init();
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
                            store: planStore,//字段：订单状态，操作人，操作内容，操作备注
                            columns: [Ext.create('Ext.grid.RowNumberer'),
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'shippingnotestatuscode',
                                sortable: false,
                                menuDisabled: true,
                                text: "订单状态",
                                width: 90,
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    var str = "";//订单状态代码，（【未完成】0：已下单；10：提货中；20：待出发；30：在途；）（【已完成】40：待验收付款；90：已验收付款；）（【异常】21：差额待确认；）
                                    switch (value) {
                                        case "0":
                                            str = "已下单";
                                            break;
                                        case "10":
                                            str = "提货中";
                                            break;
                                        case "20":
                                            str = "待出发";
                                            break;
                                        case "30":
                                            str = "在途";
                                            break;
                                        case "40":
                                            str = "待验收付款";
                                            break;
                                        case "90":
                                            str = "已验收付款";
                                            break;
                                        default:
                                            str = "差额待确认";
                                    }

                                    return str;
                                }
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'username',
                                sortable: false,
                                menuDisabled: true,
                                text: "操作人",
                                width: 90
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'recordtype',
                                sortable: false,
                                menuDisabled: true,
                                text: "操作内容",
                                width: 100
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'recordmemo',
                                sortable: false,
                                menuDisabled: true,
                                text: "操作备注",
                                flex: 1,
                                renderer: function (value, metaData, record, rowIdx, colIdx, store) {
                                    metaData.tdAttr = 'data-qtip="' + Ext.String.htmlEncode(value) + '"';
                                    return value;
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
//-------------------------------------------------------回执单附件-------------------------------------------------------
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
//-------------------------------------------------------附件------------------------------------------------------------
Ext.define('phWin', {
    extend: 'Ext.window.Window',
    title: "上传",
    height: 160,
    width: 560,
    modal: true,
    layout: 'border',
    initComponent: function () {
        var me = this;
        var id = me.shippingnoteid;
        me.items = [{
            xtype: 'UploaderPanel',
            id: 'uploadpicC',
            region: 'center',
            layout: 'column',

            autoScroll: true,
            items: [
                {
                    xtype: 'filefield',
                    allowBlank: false,
                    labelWidth: 65,
                    fieldLabel: '图片上传',
                    buttonText: '浏览',
                    columnWidth: 0.8
                },
                {
                    xtype: 'button',
                    text: '上传',
                    iconCls: 'upload',
                    columnWidth: 0.2,
                    margin: '0 5',
                    handler: function () {
                        Ext.getCmp('uploadpicC').upload('CZCLZ.Order.UploadPicForReceipt', function (retVal) {
                            var isDefault = false;
                            if (retVal.isdefault == 1) {
                                isDefault = true;
                            }
                            GetReceiptInfoLine(id);
                            me.close();
                        }, CS.onError, id);
                    }
                }
            ]

        }];
        me.callParent(arguments);
    }
});
//-------------------------------------------------------回执单附件明细弹出界面--------------------------------------------
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
                                    str = "<a onclick='ShowReceiptInfoPhoto(\"" + record.data.photoaccessaddress + "\");'>查看</a> || <a onclick='DelReceiptInfo(\"" + value + "\",\"" + id + "\");'>删除</a>";
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
                                            xtype: 'buttongroup',
                                            title: '',
                                            items: [
                                                {
                                                    xtype: 'button',
                                                    iconCls: 'add',
                                                    text: '上传',
                                                    handler: function () {
                                                        UploadFJ(id);
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
//-------------------------------------------------------定位信息编辑弹出界面----------------------------------------------
Ext.define('addLocationInfoWin', {
    extend: 'Ext.window.Window',
    id: 'locationInfoWinId',
    height: 300,
    width: 400,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '定位信息编辑',
    initComponent: function () {
        var me = this;
        var shippingnoteid = me.shippingnoteid;
        me.items = [
            {
                xtype: 'form',
                id: 'addLocationInfoform',
                bodyPadding: 10,
                items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: 'locationid',
                        name: 'locationid',
                        labelWidth: 70,
                        hidden: true,
                        colspan: 2
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '司机名称',
                        name: 'drivername',
                        labelWidth: 70,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '司机电话',
                        name: 'drivertel',
                        labelWidth: 70,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '车牌号',
                        name: 'vehiclenumber',
                        labelWidth: 70,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: 'datefield',
                        fieldLabel: '时间',
                        name: 'getlocationtime',
                        format: 'Y-m-d H:i:s',
                        labelWidth: 70,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '地址',
                        name: 'address',
                        labelWidth: 70,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '备注',
                        name: 'memo',
                        labelWidth: 70,
                        allowBlank: false,
                        anchor: '100%'
                    }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '确定',
                        iconCls: 'dropyes',
                        handler: function () {
                            var form = Ext.getCmp('addLocationInfoform');
                            if (form.form.isValid()) {
                                var values = form.form.getValues(false);
                                var me = this;
                                CS('CZCLZ.Order.SaveLocationInfo', function (retVal) {
                                    locationInfoStore.loadData(retVal);
                                    Ext.getCmp("locationInfoWinId").close();
                                }, CS.onError, values, shippingnoteid);

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
//-------------------------------------------------------填写定位信息弹出界面----------------------------------------------
Ext.define('locationInfoWin', {
    extend: 'Ext.window.Window',

    height: document.documentElement.clientHeight,
    width: document.documentElement.clientWidth,
    layout: {
        type: 'fit'
    },
    title: '定位信息',
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
                            store: locationInfoStore,//字段：车牌号，时间，地址，备注，操作列
                            columns: [Ext.create('Ext.grid.RowNumberer'),


                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'drivername',
                                sortable: false,
                                menuDisabled: true,
                                text: "司机姓名",
                                flex: 1
                            }, {
                                xtype: 'gridcolumn',
                                dataIndex: 'drivertel',
                                sortable: false,
                                menuDisabled: true,
                                text: "司机电话",
                                flex: 1
                            },

                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'vehiclenumber',
                                sortable: false,
                                menuDisabled: true,
                                text: "车牌号",
                                flex: 1
                            },
                            {
                                xtype: 'datecolumn',
                                dataIndex: 'getlocationtime',
                                format: 'Y-m-d',
                                sortable: false,
                                menuDisabled: true,
                                width: 110,
                                text: '时间'
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'address',
                                sortable: false,
                                menuDisabled: true,
                                text: "地址"
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'memo',
                                sortable: false,
                                menuDisabled: true,
                                text: "备注"
                            },
                            {
                                text: '操作',
                                dataIndex: 'locationid',
                                width: 120,
                                sortable: false,
                                menuDisabled: true,
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    var str;
                                    str = "<a onclick='EditLocationInfo(\"" + value + "\",\"" + id + "\");'>编辑</a> || <a onclick='DelLocationInfo(\"" + value + "\",\"" + id + "\");'>删除</a>";
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
                                            xtype: 'buttongroup',
                                            title: '',
                                            items: [
                                                {
                                                    xtype: 'button',
                                                    iconCls: 'add',
                                                    text: '新增',
                                                    handler: function () {
                                                        var win = new addLocationInfoWin({ shippingnoteid: id });
                                                        win.show(null, function () {
                                                        });
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
//-------------------------------------------------------证据查看弹出界面--------------------------------------------------
Ext.define('payFjWin', {
    extend: 'Ext.window.Window',
    height: document.documentElement.clientHeight,
    width: document.documentElement.clientWidth,
    layout: {
        type: 'fit'
    },
    modal: true,
    title: '证据查看',
    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'panel',
                bodyPadding: 10,
                items: [
                    {
                        id: 'picorvideo',
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
//-------------------------------------------------------查看补单情况弹出界面----------------------------------------------
Ext.define('doublePayWin', {
    extend: 'Ext.window.Window',

    height: document.documentElement.clientHeight,
    width: document.documentElement.clientWidth,
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
                            store: doublePayStore,//补单时间，补单金额，补单状态，补单原因，证据查看
                            columns: [Ext.create('Ext.grid.RowNumberer'),
                            {
                                xtype: 'datecolumn',
                                dataIndex: 'addtime',
                                format: 'Y-m-d',
                                sortable: false,
                                menuDisabled: true,
                                width: 110,
                                text: '补单时间'
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'companydoublepay',
                                sortable: false,
                                menuDisabled: true,
                                text: "补单金额",
                                flex: 1
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'paystatus',
                                sortable: false,
                                menuDisabled: true,
                                text: "补单状态",//0:未支付;1：已支付未确认;2：已支付已确认；
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    var str = "";
                                    switch (value) {
                                        case 0:
                                            str = "未支付";
                                            break;
                                        case 1:
                                            str = "已支付未确认";
                                            break;
                                        default:
                                            str = "已支付已确认";
                                    }
                                    return str;
                                }
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'memo',
                                sortable: false,
                                menuDisabled: true,
                                text: "补单原因"
                            },
                            {
                                text: '证据查看',
                                dataIndex: 'doublepayid',
                                width: 80,
                                sortable: false,
                                menuDisabled: true,
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    var str;
                                    str = "<a onclick='ShowDoublePayFj(\"" + value + "\");'>查看</a>";
                                    return str;
                                }
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
//-------------------------------------------------------GPS弹出界面------------------------------------------------------
Ext.define('gbsWin', {
    extend: 'Ext.window.Window',
    height: 150,
    width: 400,
    layout: {
        type: 'fit'
    },
    modal: true,
    title: '填写gps信息',
    initComponent: function () {
        var me = this;
        var id = me.shippingnoteid;
        me.items = [
            {
                xtype: 'form',
                bodyPadding: 10,
                id: 'gbsForm',
                items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: 'gps企业名称',
                        name: 'gpscompany',
                        labelWidth: 70,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: 'gps运单号',
                        name: 'gpsdenno',
                        labelWidth: 70,
                        allowBlank: false,
                        anchor: '100%'
                    }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '保存',
                        iconCls: 'dropyes',
                        handler: function () {
                            var form = Ext.getCmp('gbsForm');
                            if (form.form.isValid()) {
                                var values = form.form.getValues(false);
                                CS('CZCLZ.Order.UpdateOrderGPS', function (retVal) {
                                    if (retVal) {
                                        GetOrder(purPage);
                                        Ext.MessageBox.alert('提示', "GPS信息填写成功！");
                                    }
                                }, CS.onError, id, values);
                            }
                        }
                    },
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
//-------------------------------------------------------异常解决弹出界面--------------------------------------------------
Ext.define('abnormalWin', {
    extend: 'Ext.window.Window',
    height: 150,
    width: 400,
    layout: {
        type: 'fit'
    },
    modal: true,
    initComponent: function () {
        var me = this;
        var id = me.shippingnoteid;
        me.items = [
            {
                xtype: 'form',
                id: 'abnormalForm',
                bodyPadding: 10,
                items: [
                    {
                        xtype: 'textareafield',
                        id: 'nr',
                        name: 'abnormalmemo',
                        labelWidth: 70,
                        fieldLabel: '内容',
                        allowBlank: false,
                        anchor: '100%'
                    }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '保存',
                        iconCls: 'dropyes',
                        id: 'errorSaveBtn',
                        handler: function () {
                            CS('CZCLZ.Order.UpdateOrderError', function (retVal) {
                                if (retVal) {
                                    GetOrder(purPage);
                                    Ext.MessageBox.alert('提示', "异常内容填写成功！");
                                }
                            }, CS.onError, 1, id, Ext.getCmp("nr").getValue());
                            this.up('window').close();
                        }
                    },
                    {
                        text: '异常解决',
                        iconCls: 'close',
                        id: 'errorBtn',
                        handler: function () {
                            CS('CZCLZ.Order.UpdateOrderError', function (retVal) {
                                if (retVal) {
                                    GetOrder(purPage);
                                    Ext.MessageBox.alert('提示', "异常解决成功！");
                                }
                            }, CS.onError, 2, id, Ext.getCmp("nr").getValue());
                            this.up('window').close();
                        }
                    }
                ]
            }
        ];
        me.callParent(arguments);
    }
});
//-------------------------------------------------------主界面------------------------------------------------------------
Ext.define('orderView', {
    extend: 'Ext.container.Viewport',

    layout: {
        type: 'fit'
    },

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'gridpanel',
                title: '',
                store: storeOrder,
                columnLines: true,
                viewConfig: {
                    enableTextSelection: true
                },
                columns: [Ext.create('Ext.grid.RowNumberer'),
               
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'username',
                    sortable: false,
                    menuDisabled: true,
                    locked: true,

                    text: "厂家"
                }, {
                    xtype: 'datecolumn',
                    dataIndex: 'shippingnoteadddatetime',
                    format: 'Y-m-d',
                    locked: true,

                    sortable: false,
                    menuDisabled: true,
                    width: 110,
                    text: '订单时间'
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'goodsfromroute',
                    sortable: false,
                    locked: true,

                    menuDisabled: true,
                    text: "起始地"
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'goodstoroute',
                    locked: true,

                    sortable: false,
                    menuDisabled: true,
                    text: "目的地"
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'descriptionofgoods',
                    locked: true,

                    sortable: false,
                    menuDisabled: true,
                    text: "货物"
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'itemgrossweight',
                    locked: true,

                    sortable: false,
                    menuDisabled: true,
                    text: "重量"
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'tuoyunorder',
                    sortable: false,
                    locked: true,
                    menuDisabled: true,
                    text: "托运单协议编号"
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'goodsreceiptplace',
                    sortable: false,
                    locked: true,
                    menuDisabled: true,
                    text: "收货地址"
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
                    dataIndex: 'userGYS_username',
                    sortable: false,
                    menuDisabled: true,
                    text: "供应商"
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'userGYS_vehiclenumber',
                    sortable: false,
                    menuDisabled: true,
                    text: "车牌"
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'userGYS_identitydocumentnumber',
                    sortable: false,
                    menuDisabled: true,
                    text: "身份证"
                },
 


                {
                    xtype: 'gridcolumn',
                    dataIndex: 'cysj',
                    sortable: false,
                    hidden: true,
                    menuDisabled: true,
                    text: "承运司机"
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'userZCSJ_username',
                    sortable: false,
                    menuDisabled: true,
                    text: "装车司机"
                },
                   {
                       xtype: 'gridcolumn',
                       dataIndex: 'userZCSJ_vehiclenumber',
                       sortable: false,
                       menuDisabled: true,
                       text: "车牌"
                   },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'userZCSJ_identitydocumentnumber',
                        sortable: false,
                        menuDisabled: true,
                        text: "身份证"
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'userZCSJ_usertel',
                        sortable: false,
                        menuDisabled: true,
                        text: "司机电话"
                    },




                
 


                {
                    xtype: 'gridcolumn',
                    dataIndex: 'shippingnotestatusname',
                    sortable: false,
                    menuDisabled: true,
                    text: "订单状态",
                    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {

                        return "<input value='" + value + "' style='border:0px;BACKGROUND-COLOR: transparent;'>";
                    }
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'shippingnotenumber',
                    sortable: false,
                    menuDisabled: true,
                    text: "订单号",
                    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {

                        return "<input value='" + value + "' style='border:0px;BACKGROUND-COLOR: transparent;'>";
                    }
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
                    dataIndex: 'totalnumberofpackages',
                    sortable: false,
                    menuDisabled: true,
                    text: "数量"
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
                    dataIndex: 'vehicletype',
                    sortable: false,
                    menuDisabled: true,
                    text: "车型"
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'vehiclelength',
                    sortable: false,
                    menuDisabled: true,
                    text: "车长"
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
                    dataIndex: 'totalmonetaryamount',
                    sortable: false,
                    menuDisabled: true,
                    text: "预估企业运费"
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'actualnooilmoney',
                    sortable: false,
                    menuDisabled: true,
                    text: "实际未用票"
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'actualmoney',
                    sortable: false,
                    menuDisabled: true,
                    text: "实际下游成本"
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'actualcompanypay',
                    sortable: false,
                    menuDisabled: true,
                    text: "企业实际运费"
                },

                {
                    xtype: 'gridcolumn',
                    dataIndex: 'userCZY',
                    sortable: false,
                    menuDisabled: true,
                    text: "操作员"
                },

                ],
                viewConfig: {
                    getRowClass: function (record, rowIndex, rowParams, store) {
                        if (record.data.isabnormal != null) {
                            return "x-grid-record-yellow";
                        }
                    }
                },
                dockedItems: [
                    {
                        xtype: 'toolbar',
                        dock: 'top',
                        items: [
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
                                xtype: 'textfield',
                                id: 'cx_changjia',
                                width: 140,
                                labelWidth: 50,
                                fieldLabel: '厂家'
                            },
                            {
                                xtype: 'textfield',
                                id: 'cx_shippingnotenumber',
                                width: 160,
                                labelWidth: 70,
                                fieldLabel: '订单号'
                            },
                            {
                                xtype: 'textfield',
                                id: 'cx_gys',
                                width: 160,
                                labelWidth: 70,
                                hidden: true,
                                fieldLabel: '供应商'
                            },
                            {
                                xtype: 'textfield',
                                id: 'cx_zcsj',
                                width: 160,
                                labelWidth: 70,
                                hidden: true,

                                fieldLabel: '装车司机'
                            },
                            {
                                xtype: 'textfield',
                                id: 'cx_cysj',
                                width: 160,
                                labelWidth: 70,
                                hidden: true,

                                fieldLabel: '承运司机'
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
                                            GetOrder(purPage);
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
        ];
        me.callParent(arguments);
    }
});


Ext.onReady(function () {
    new orderView();

    GetOrder(purPage);
})



function ShowAddLXR(offid, totalmonetaryamount, actualnooilmoney, actualmoney, shippingnoteid) {
    var title = "新增实际运费组成";



    var c_window = new Ext.Window({
        extend: 'Ext.window.Window',
        viewModel: {
            type: 'mywindow'
        },
        autoShow: true,
        height: 300,
        id: "lmwin44",
        width: 350,
        layout: 'fit',
        title: title,
        modal: true,
        items: [
            {
                xtype: 'form',
                id: 'addform44',
                bodyPadding: 10,
                items: [

                    {
                        xtype: 'textfield',
                        fieldLabel: '预估下游成本',
                        id: 'estimatemoney',
                        name: 'estimatemoney',
                        labelWidth: 120,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '实际下游成本',
                        id: 'actualmoney',
                        name: 'actualmoney',
                        labelWidth: 120,
                        anchor: '100%',
                        value: actualmoney

                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '实际未用油金额',
                        id: 'actualnooilmoney',
                        name: 'actualnooilmoney',
                        labelWidth: 120,
                        anchor: '100%',
                        value: actualnooilmoney
                    }, {
                        xtype: 'textfield',
                        fieldLabel: '企业报价',
                        id: 'actualcompanypay',
                        name: 'actualcompanypay',
                        labelWidth: 120,
                        anchor: '100%'
                    }, {
                        xtype: 'textfield',
                        fieldLabel: '实际综合成本',
                        id: 'actualcompletemoney',
                        name: 'actualcompletemoney',
                        labelWidth: 120,
                        anchor: '100%'
                    }, {
                        xtype: 'textfield',
                        fieldLabel: '实际税费成本',
                        id: 'actualtaxmoney',
                        name: 'actualtaxmoney',
                        labelWidth: 120,
                        anchor: '100%'
                    }, {
                        xtype: 'textfield',
                        fieldLabel: '实际资金成本',
                        id: 'actualcostmoney',
                        name: 'actualcostmoney',
                        labelWidth: 120,
                        anchor: '100%'
                    }

                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '保存',
                        iconCls: 'dropyes',
                        handler: function () {
                            var form = Ext.getCmp('addform44');
                            if (form.form.isValid()) {
                                var values = form.form.getValues(false);
                                var me = this;
                                CS('CZCLZ.Order.SaveOFFER', function (retVal) {
                                    if (retVal) {
                                        GetOrder(1);
                                        me.up('window').close();

                                    }
                                }, CS.onError, values, offid);

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
        ]
    });
    c_window.show();

    //if (type == 2) {

    var r = storeOrder.findRecord("shippingnoteid", shippingnoteid).data;

    var form = Ext.getCmp('addform44');
    form.form.setValues(r);
    //}


}


var Rel_store = Ext.create('Ext.data.Store', {
    fields: [
        { name: 'ID' },

        { name: 'money' },
        { name: 'vehiclenumber' },
        { name: 'username' },

        { name: 'usertel' }
    ]
});
function RelSel(shippingnoteid, paytype) {
    CS('CZCLZ.Order.GetBYcostList', function (retVal) {
        if (retVal) {
            Rel_store.loadData(retVal, false);

        }
    }, CS.onError, shippingnoteid, paytype);
}

function ShowCZY(shippingnoteid, paytype) {

    RelSel(shippingnoteid, paytype);



    var c_window = new Ext.Window({
        extend: 'Ext.window.Window',
        viewModel: {
            type: 'mywindow'
        },
        autoShow: true,
        height: 400,
        id: "lmwin3",
        width: 600,
        layout: 'fit',
        title: "明细",
        modal: true,
        items: [
            {
                xtype: 'panel',
                layout: 'column',
                bodyStyle: 'overflow-x:hidden;overflow-y:auto;',
                buttonAlign: 'center',
                layout: 'fit',
                buttons: [

                    {
                        text: '关闭',
                        handler: function () {
                            Ext.getCmp("lmwin3").close();
                        }
                    }
                ],
                items: [
                    {
                        xtype: 'gridpanel',
                        border: 1,
                        id: 'DeptsGrid3',

                        columnLines: true,
                        columnWidth: 1,
                        store: Rel_store,

                        columns: [
                            Ext.create('Ext.grid.RowNumberer', { width: 40 }),

                            {
                                dataIndex: 'vehiclenumber',
                                text: '车牌号',
                                flex: 1,
                                align: "center",
                                sortable: false,
                                menuDisabled: true
                            },
                            {
                                dataIndex: 'username',
                                flex: 1,
                                text: '账号名称',
                                align: "center",
                                sortable: false,
                                menuDisabled: true
                            },
                            {
                                dataIndex: 'usertel',
                                text: '联系电话',
                                align: "center",
                                sortable: false,
                                menuDisabled: true
                            },
                            {
                                dataIndex: 'money',
                                text: '费用',
                                align: "center",
                                sortable: false,
                                menuDisabled: true
                            },
                            {
                                dataIndex: 'id',
                                flex: 1,
                                text: '修改金额',
                                align: "center",
                                sortable: false,
                                menuDisabled: true,
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    return "<div style='color:green;cursor:pointer;' onclick='SetEndAdd(\"" + shippingnoteid + "\",\"" + value + "\",\"" + record.data.id + "\",\"" + record.data.money + "\")'>修改</div>";
                                }
                            }
                        ]

                    }



                ]
            }
        ]
    });
    c_window.show();


}


function SetEndAdd(shippingnoteid, payid, id, money) {






    var c_window = new Ext.Window({
        extend: 'Ext.window.Window',
        viewModel: {
            type: 'mywindow'
        },
        autoShow: true,
        height: 150,
        id: "EndDQ",
        width: 300,
        layout: 'fit',
        title: "修改费用",
        modal: true,
        items: [
            {
                xtype: 'form',
                id: 'EndDQfrom',
                bodyPadding: 10,
                items: [

                    {
                        xtype: 'numberfield',
                        fieldLabel: '费用',
                        id: 'money',
                        name: 'money',
                        labelWidth: 70,
                        allowBlank: false,
                        allowDecimals: true,    //是否允许小数
                        allowNegative: false,    //是否允许负数
                        minValue: 0,
                        decimalPrecision: 2,    // 精确的位数

                        anchor: '100%'
                    }

                ],
                buttonAlign: 'center',
                buttons: [

                    {
                        text: '保存',
                        iconCls: 'dropyes',
                        handler: function () {
                            var form = Ext.getCmp('EndDQfrom');
                            if (form.form.isValid()) {
                                var values = form.form.getValues(false);
                                var me = this;
                                CS('CZCLZ.Order.UpdateMoney', function (retVal) {
                                    if (retVal) {
                                        Ext.Msg.alert('提示', '修改成功！');

                                        RelSel(shippingnoteid, payid);


                                        Ext.getCmp("EndDQ").close();

                                    }
                                }, CS.onError, id, Ext.getCmp("money").getValue());

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
        ]
    });
    c_window.show();



}

