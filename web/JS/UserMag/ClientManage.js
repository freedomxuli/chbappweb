inline_include("approot/r/weixin/js/qrcode/jquery-1.8.3.min.js");
inline_include("approot/r/weixin/js/qrcode/jquery.qrcode.js");
inline_include("approot/r/weixin/js/qrcode/qrcode.js");
inline_include("approot/r/weixin/js/qrcode/utf.js");

var pageSize = 15;
var cx_role;
var cx_yhm;
var cx_xm;
var cx_beg;
var cx_end;

var yuserid;

//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
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
        { name: 'UserTel' },
        { name: 'FromRoute' },
        { name: 'ToRoute' },
        { name: 'Points' },
        { name: 'SalePoints' },
        { name: 'KFGMPoints' },
        { name: 'dqS' },
        { name: 'DqBm' },
        { name: 'AddTime' },
        { name: 'searchAddress' },
        { name: 'ewmbs' },
        { name: 'IDCard' },
        { name: 'caruser' },
        { name: 'modetype' },
        { name: 'modecoefficient' },
        { name: 'carriagemaxmoney' },
        { name: 'mirrornumber' },
        { name: 'carriagegetmode' },
        { name: 'carriagechbid' },
        { name: 'carriageoilrate' },
        { name: 'carriagemoneyrate' },
        { name: 'isidentification' },
        { name: 'IsSHPass' },
        { name: 'isdonate' },
        { name: 'donateratio' },
        { name: 'isshowsource' },
        { name: 'redenvelopequota' },
        { name: 'iscost' },
        { name: 'ischbmember' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getUser(nPage);
    }
});

var yglsjstore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
        { name: 'id' },
        { name: 'drivername' },
        { name: 'driverxm' },
        { name: 'linkedunit' },
        { name: 'carnumber' },
        { name: 'drivermemo' },
        { name: 'mirrornumber' },
        { name: 'caruser' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getYGLSJList(nPage);
    }
});

var glsjstore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
        { name: 'id' },
        { name: 'drivername' },
        { name: 'driverxm' },
        { name: 'linkedunit' },
        { name: 'carnumber' },
        { name: 'drivermemo' },
        { name: 'mirrornumber' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getGLSJList(nPage);
    }
});



var roleStore = Ext.create('Ext.data.Store', {
    fields: [
        { name: 'ClientKind' },
        { name: 'ClientName' }
    ],
    data: [
        {
            'ClientKind': '',
            'ClientName': '全部',
        },
        {
            'ClientKind': '1',
            'ClientName': '专线',
        },
        {
            'ClientKind': '2',
            'ClientName': '三方',
        }
    ]
});

var isdonateStore = Ext.create('Ext.data.Store', {
    fields: [
        { name: 'val' },
        { name: 'text' }
    ],
    data: [
        {
            'val': '',
            'text': '全部',
        },
        {
            'val': '0',
            'text': '是',
        },
        {
            'val': '1',
            'text': '否',
        }
    ]
});
var roleStore1 = Ext.create('Ext.data.Store', {
    fields: [
        { name: 'ClientKind' },
        { name: 'ClientName' }
    ],
    data: [
        {
            'ClientKind': '1',
            'ClientName': '专线',
        },
        {
            'ClientKind': '2',
            'ClientName': '三方',
        }
    ]
});

var orderstore = Ext.create('Ext.data.Store', {
    fields: [
        { name: 'KIND' },
        { name: 'UserName' },
        { name: 'DATE' },
        { name: 'MONEY' }
    ]
});

var moneystore = Ext.create('Ext.data.Store', {
    fields: [
        { name: 'UserName' },
        { name: 'MONEY' }
    ]
});

var dqstore = Ext.create('Ext.data.Store', {
    fields: [
        { name: 'dq_mc' },
        { name: 'dq_bm' }
    ]
});

var sdqstore = Ext.create('Ext.data.Store', {
    fields: [
        { name: 'dq_mc' },
        { name: 'dq_bm' }
    ]
});

var kpstore = Ext.create('Ext.data.Store', {
    fields: [
        { name: 'id' },
        { name: 'name' }
    ]
});


//************************************数据源*****************************************

//************************************页面方法***************************************
function getUser(nPage) {
    CS('CZCLZ.YHGLClass.GetClientList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_role").getValue(), Ext.getCmp("cx_yhm").getValue(), Ext.getCmp("cx_xm").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), Ext.getCmp("cx_isdonate").getValue());
}


function GetKP() {
    CS('CZCLZ.YHGLClass.GetKP', function (retVal) {
        kpstore.loadData(retVal);
    }, CS.onError);

}

function EditUser(id) {
    GetKP();
    var r = store.findRecord("UserID", id).data;
    var win = new addWin({ oldv: r });
    win.show(null, function () {
        if (r.ClientKind == 1) {
            //Ext.getCmp('modetype').show();
            Ext.getCmp('modecoefficient').show();
            Ext.getCmp('carriagemaxmoney').show();
            Ext.getCmp('mirrornumber').show();

            Ext.getCmp('isdonate').show();//是否下单送红包 显示
            Ext.getCmp('donateratio').show();//下单送红包比例 显示
            Ext.getCmp('isshowsource').show();//是否显示发布来源 显示
            Ext.getCmp('iscost').show();//是否收取佣金； 显示
            Ext.getCmp('ischbmember').show();//是否查货宝会员 显示


        } else if (r.ClientKind == 2) {
            Ext.getCmp('modetype').hide();
            Ext.getCmp('modetype').setValue(2);
            Ext.getCmp('modecoefficient').show();
            Ext.getCmp('carriagemaxmoney').show();
            Ext.getCmp('mirrornumber').show();
        } else {
            Ext.getCmp('modetype').hide();
            Ext.getCmp('modecoefficient').hide();
            Ext.getCmp('carriagemaxmoney').hide();
            Ext.getCmp('mirrornumber').hide();
        }
        if (r.carriagegetmode == 1) {
            Ext.getCmp('modecoefficient').decimalPrecision = 3;
        } else if (r.carriagegetmode == 0) {
            Ext.getCmp('modecoefficient').decimalPrecision = 2;
        }
        CS('CZCLZ.YHGLClass.GetDQS', function (retVal) {
            if (retVal) {
                sdqstore.loadData(retVal);
                CS('CZCLZ.YHGLClass.GetDQ', function (ret) {
                    dqstore.loadData(ret);
                    win.setTitle("用户修改");
                    var form = Ext.getCmp('addform');
                    form.form.setValues(r);
                    if (r.ClientKind == 2) {
                        Ext.getCmp("roleId").allowBlank = true;
                        Ext.getCmp("roleId").hide();
                        Ext.getCmp("UserXM").allowBlank = true;
                        Ext.getCmp("isidentification").allowBlank = true;
                        Ext.getCmp("isidentification").hide();
                        Ext.getCmp("carriagechbid").allowBlank = false;
                        Ext.getCmp("carriagechbid").show();
                        if (!r.carriagegetmode) {
                            Ext.getCmp("carriagegetmode").setValue(1);
                            Ext.getCmp("carriageoilrate").hide();
                            Ext.getCmp("carriagemoneyrate").hide();
                            Ext.getCmp("carriageoilrate").setValue();
                            Ext.getCmp("carriagemoneyrate").setValue();
                            Ext.getCmp("carriageoilrate").allowBlank = true;
                            Ext.getCmp("carriagemoneyrate").allowBlank = true;

                            Ext.getCmp('modecoefficient').decimalPrecision = 3;
                        }
                        Ext.getCmp("carriagegetmode").setReadOnly(true);
                        CS('CZCLZ.YHGLClass.JudgeUser', function (ret1) {
                            if (ret1) {
                                Ext.getCmp("IDCard").allowBlank = false;
                                Ext.getCmp("IDCard").show();
                                Ext.getCmp("Password").allowBlank = false;
                                Ext.getCmp("Password").show();
                            } else {
                                Ext.getCmp("IDCard").allowBlank = true;
                                Ext.getCmp("IDCard").hide();
                                Ext.getCmp("Password").allowBlank = true;
                                Ext.getCmp("Password").hide();
                            }
                        }, CS.onError);
                        //Ext.getCmp("UserXM").hide();
                    } else if (r.ClientKind == 1) {
                        Ext.getCmp("roleId").allowBlank = true;
                        Ext.getCmp("roleId").hide();
                        Ext.getCmp("IDCard").allowBlank = true;
                        Ext.getCmp("IDCard").hide();
                        Ext.getCmp("isidentification").allowBlank = false;
                        Ext.getCmp("isidentification").show();
                        Ext.getCmp("carriagechbid").allowBlank = false;
                        Ext.getCmp("carriagechbid").show();
                        if (!r.carriagegetmode) {
                            Ext.getCmp("carriagegetmode").setValue(0);
                        }
                        if (Ext.getCmp("carriagegetmode").getValue() == "0" && Ext.getCmp("modetype").getValue() == "1") {
                            Ext.getCmp("modetype").show();

                            Ext.getCmp("carriageoilrate").show();
                            Ext.getCmp("carriagemoneyrate").show();
                            Ext.getCmp("carriageoilrate").allowBlank = false;
                            Ext.getCmp("carriagemoneyrate").allowBlank = false;
                            Ext.getCmp('modecoefficient').decimalPrecision = 2;
                        } else {
                            Ext.getCmp("modetype").hide();
                            Ext.getCmp("modetype").setValue();

                            Ext.getCmp("carriageoilrate").hide();
                            Ext.getCmp("carriagemoneyrate").hide();
                            Ext.getCmp("carriageoilrate").setValue();
                            Ext.getCmp("carriagemoneyrate").setValue();
                            Ext.getCmp("carriageoilrate").allowBlank = true;
                            Ext.getCmp("carriagemoneyrate").allowBlank = true;
                            Ext.getCmp('modecoefficient').decimalPrecision = 3;
                        }

                        CS('CZCLZ.YHGLClass.JudgeUser', function (ret1) {
                            if (ret1) {
                                Ext.getCmp("Password").allowBlank = false;
                                Ext.getCmp("Password").show();
                            } else {
                                Ext.getCmp("Password").allowBlank = true;
                                Ext.getCmp("Password").hide();
                            }
                        }, CS.onError);
                    } else {
                        Ext.getCmp("roleId").getEl().allowBlank = false;
                        Ext.getCmp("roleId").show();
                        Ext.getCmp("UserXM").getEl().allowBlank = false;
                        Ext.getCmp("UserXM").show();
                        CS('CZCLZ.YHGLClass.JudgeUser', function (ret1) {
                            if (ret1) {
                                Ext.getCmp("IDCard").allowBlank = false;
                                Ext.getCmp("IDCard").show();
                                Ext.getCmp("Password").allowBlank = false;
                                Ext.getCmp("Password").show();
                            } else {
                                Ext.getCmp("IDCard").allowBlank = true;
                                Ext.getCmp("IDCard").hide();
                                Ext.getCmp("Password").allowBlank = true;
                                Ext.getCmp("Password").hide();
                            }
                        }, CS.onError);
                    }
                }, CS.onError, r.dqS);
            }
        }, CS.onError);
    });
}

function AddPhoto(v) {
    var picItem = [];
    CS('CZCLZ.YHGLClass.GetProductImages', function (retVal) {

        var result = retVal.evalJSON();
        //for (var i = 0; i < result.length; i++) {
        var isDefault = false;
        //    //if (result[i].ISDEFAULT == 1)
        //    //    isDefault = true;
        if (result.data.length > 0) {
            Ext.getCmp('uploadproductpic').add(new SelectImg({
                isSelected: isDefault,
                src: result.data[0].photoUrl,
                fileid: result.data[0].fjId
            }));
        }
        //}
    }, CS.onError, v);

    var win = new phWin({ UserID: v });
    win.show();
}
function AddPhoto1(v) {
    var picItem = [];
    CS('CZCLZ.YHGLClass.GetProductImages1', function (retVal) {

        var result = retVal.evalJSON();
        //for (var i = 0; i < result.length; i++) {
        var isDefault = false;
        //    //if (result[i].ISDEFAULT == 1)
        //    //    isDefault = true;
        if (result.data.length > 0) {
            Ext.getCmp('uploadproductpic1').add(new SelectImg({
                isSelected: isDefault,
                src: result.data[0].photoUrl,
                fileid: result.data[0].fjId
            }));
        }
        //}
    }, CS.onError, v);

    var win = new phWin1({ UserID: v });
    win.show();
}

function LookLists(id) {
    var win = new OrderList();
    win.show(null, function () {
        CS('CZCLZ.YHGLClass.GetOrderList', function (retVal) {
            if (retVal) {
                orderstore.loadData(retVal.dt);
                moneystore.loadData(retVal.dt2);
            }
        }, CS.onError, id);
    });
}


function LookEWM(username) {
    var win = new EWMWin();
    win.show(null, function () {
        jQuery('#qrcodeTable').qrcode({
            render: "table",
            text: username,
            width: "150",               //二维码的宽度
            height: "150",
        });
    });
}

function LookEWM1(userid) {
    var win = new EWMWin1({ id: userid });
    win.show(null, function () {
        jQuery('#qrcodeTable1').qrcode({
            render: "table",
            text: "http://share.chahuobao.net/freight/html/sfbd.html?userid=" + userid,
            width: "570",               //二维码的宽度
            height: "570",
        });
    });
}

function GLSJ(userid) {
    yuserid = userid;
    var win = new YGLSJList({ userid: userid });
    win.show(null, function () {
        getYGLSJList(1);
    })
}

function getYGLSJList(nPage) {
    CS('CZCLZ.DriverMag.getYGLSJList', function (retVal) {
        yglsjstore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, yuserid);
}

function getGLSJList(nPage) {
    CS('CZCLZ.DriverMag.getGLSJList', function (retVal) {
        glsjstore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, yuserid, Ext.getCmp("cx_cp").getValue());
}


function DeleteGL(duserid, userid) {
    Ext.MessageBox.confirm('确认', '是否删除该关联？', function (btn) {
        if (btn == 'yes') {
            CS('CZCLZ.DriverMag.DelGLByDUID', function (retVal) {
                if (retVal) {
                    getYGLSJList(1);
                }
            }, CS.onError, duserid, userid);
        }
    });
}


function GLSJMX(duserid, userid) {
    CS('CZCLZ.DriverMag.GLSJMX', function (retVal) {
        if (retVal) {
            getGLSJList(1);
            getYGLSJList(1);
        }
    }, CS.onError, duserid, userid);

}

function IsBdBf(username) {
    CS('CZCLZ.YHGLClass.IsBdBf', function (retVal) {
        if (retVal.success) {
            Ext.Msg.show({
                title: '提示',
                msg: '已绑定宝付账号',
                buttons: Ext.MessageBox.OK,
                icon: Ext.MessageBox.INFO
            });
        } else {
            Ext.Msg.show({
                title: '提示',
                msg: '未绑定宝付账号',
                buttons: Ext.MessageBox.OK,
                icon: Ext.MessageBox.INFO
            });
        }
    }, CS.onError, username);
}

function shpass(id, isshpass) {
    Ext.MessageBox.confirm('确认', isshpass == 0 ? '确认下架？' : '确认重新上架？', function (btn) {
        if (btn == 'yes') {
            CS('CZCLZ.YHGLClass.Shpass', function (retVal) {
                getUser(store.currentPage);
            }, CS.onError, id, isshpass);
        }
    });
}

//设置下单送红包上限额度
function buyRedNum(uid, num) {
    var win = new upRedNumWin({ UserID: uid, redenvelopequota: num });
    win.show(null, function () {
        if (num == null || num == 'null' || num == "") {
            Ext.getCmp('redenvelopequota').setValue(0)
        } else {
            Ext.getCmp('redenvelopequota').setValue(num)
        }
    })
}
//************************************页面方法***************************************

//************************************弹出界面***************************************
Ext.define('YGLSJList', {
    extend: 'Ext.window.Window',

    height: 500,
    width: 650,
    layout: {
        type: 'fit'
    },
    title: '关联司机',
    modal: true,

    initComponent: function () {
        var me = this;
        var userid = me.userid
        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'tabpanel',
                    activeTab: 1,
                    items: [
                        {
                            xtype: 'panel',
                            layout: {
                                type: 'fit'
                            },
                            hidden: true,
                            items: [
                                {
                                    xtype: 'gridpanel',
                                    columnLines: 1,
                                    border: 1,
                                    store: yglsjstore,
                                    columns: [
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'drivername',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '司机账号'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'driverxm',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '司机姓名'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'linkedunit',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '挂靠单位'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'carnumber',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '车牌号'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'drivermemo',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '司机备注'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'mirrornumber',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '后视镜设备编号'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'caruser',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '车主账号'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'id',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '操作',
                                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                                return "<a onclick='DeleteGL(\"" + value + "\",\"" + userid + "\");'>删除</a>";
                                            }
                                        }
                                    ],
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
                                                                var win = new GLSJList({ userid: userid });
                                                                win.show(null, function () {
                                                                    getGLSJList(1);
                                                                })
                                                            }
                                                        }
                                                    ]
                                                },

                                            ]
                                        },
                                        {
                                            xtype: 'pagingtoolbar',
                                            displayInfo: true,
                                            store: yglsjstore,
                                            dock: 'bottom'
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


Ext.define('GLSJList', {
    extend: 'Ext.window.Window',

    height: 422,
    width: 620,
    layout: {
        type: 'fit'
    },
    title: '可关联司机',
    modal: true,

    initComponent: function () {
        var me = this;
        var userid = me.userid
        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'tabpanel',
                    activeTab: 1,
                    items: [
                        {
                            xtype: 'panel',
                            layout: {
                                type: 'fit'
                            },
                            hidden: true,
                            items: [
                                {
                                    xtype: 'gridpanel',
                                    columnLines: 1,
                                    border: 1,
                                    store: glsjstore,
                                    columns: [
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'driverxm',
                                            sortable: false,
                                            menuDisabled: true,
                                            flex: 1,
                                            text: '司机姓名'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'drivername',
                                            sortable: false,
                                            menuDisabled: true,
                                            flex: 1,
                                            text: '司机账号'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'linkedunit',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '挂靠单位'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'carnumber',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '车牌号'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'drivermemo',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '司机备注'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'mirrornumber',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '后视镜设备编号'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'id',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '操作',
                                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                                return "<a onclick='GLSJMX(\"" + value + "\",\"" + userid + "\");'>关联</a>";
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
                                                    id: 'cx_cp',
                                                    width: 160,
                                                    fieldLabel: '车牌号',
                                                    labelWidth: 60
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
                                                                getGLSJList(1);
                                                            }
                                                        }
                                                    ]
                                                }

                                            ]
                                        },
                                        {
                                            xtype: 'pagingtoolbar',
                                            displayInfo: true,
                                            store: glsjstore,
                                            dock: 'bottom'
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



Ext.define('SelectImg', {
    extend: 'Ext.Img',

    height: 80,
    width: 120,
    margin: 5,
    padding: 2,
    constructor: function (config) {
        var me = this;
        config = config || {};
        config.cls = config.isSelected ? "clsSelected" : "clsUnselected";
        me.callParent([config]);
        me.on('render', function () {
            Ext.fly(me.el).on('click', function () {
                var oldSelectImg = Ext.getCmp('uploadproductpic').query('image[isSelected=true]');
                if (oldSelectImg.length < 0 || oldSelectImg[0] != me) {
                    me.removeCls('clsUnselected');
                    me.addCls('clsSelected');
                    me.isSelected = true;
                    if (oldSelectImg.length > 0) {
                        oldSelectImg[0].removeCls('clsSelected');
                        oldSelectImg[0].addCls('clsUnselected');
                        oldSelectImg[0].isSelected = false;
                    }
                }
            });

            Ext.fly(me.el).on('click', function () {
                var oldSelectImg = Ext.getCmp('uploadproductpic1').query('image[isSelected=true]');
                if (oldSelectImg.length < 0 || oldSelectImg[0] != me) {
                    me.removeCls('clsUnselected');
                    me.addCls('clsSelected');
                    me.isSelected = true;
                    if (oldSelectImg.length > 0) {
                        oldSelectImg[0].removeCls('clsSelected');
                        oldSelectImg[0].addCls('clsUnselected');
                        oldSelectImg[0].isSelected = false;
                    }
                }
            });
        });

    },

    initComponent: function () {
        var me = this;
        me.callParent(arguments);
    }
});

Ext.define('phWin', {
    extend: 'Ext.window.Window',
    height: 275,
    width: 653,
    modal: true,
    layout: 'border',
    initComponent: function () {
        var me = this;
        var UserID = me.UserID;


        me.items = [{
            xtype: 'UploaderPanel',
            id: 'uploadproductpic',
            region: 'center',
            autoScroll: true,
            dockedItems: [{
                xtype: 'toolbar',
                dock: 'top',
                items: [{
                    xtype: 'filefield',
                    fieldLabel: '上传图片',
                    width: 300,
                    buttonText: '浏览'
                }, {
                    xtype: 'button',
                    text: '上传',
                    iconCls: 'upload',
                    handler: function () {
                        Ext.getCmp('uploadproductpic').upload('CZCLZ.YHGLClass.UploadPicForProduct', function (retVal) {
                            var isDefault = false;
                            if (retVal.isdefault == 1)
                                isDefault = true;
                            Ext.getCmp('uploadproductpic').add(new SelectImg({
                                isSelected: isDefault,
                                src: retVal.fileurl,
                                fileid: retVal.fileid
                            }));
                        }, CS.onError, UserID);
                    }
                }]
            }],
            buttonAlign: 'center',
            buttons: [
                //{
                //    text: '设为默认',
                //    handler: function () {
                //        Ext.MessageBox.confirm('确认', '是否设置该图片为默认图片？', function (btn) {
                //            if (btn == 'yes') {
                //                var selPics = Ext.getCmp('uploadproductpic').query('image[isSelected=true]');
                //                if (selPics.length > 0) {
                //                    CS('CZCLZ.YHGLClass.SetDefaultPicForProduct', function (retVal) {
                //                        if (retVal)
                //                            Ext.Msg.alert('提示', '设置成功！');
                //                        else
                //                            Ext.Msg.alert('提示', '设置失败！');
                //                    }, CS.onError, selPics[0].fileid, UserID);
                //                }
                //            }
                //        });
                //    }
                //},
                {
                    text: '删除',
                    handler: function () {
                        Ext.MessageBox.confirm('确认', '是否删除该图片？', function (btn) {
                            if (btn == 'yes') {
                                var selPics = Ext.getCmp('uploadproductpic').query('image[isSelected=true]');
                                if (selPics.length > 0) {
                                    CS('CZCLZ.YHGLClass.DelProductImageByPicID', function (retVal) {
                                        if (retVal) {
                                            Ext.getCmp('uploadproductpic').remove(selPics[0]);
                                        }
                                    }, CS.onError, selPics[0].fileid, UserID, 0);
                                }
                            }
                        });
                    }
                }
            ]
        }];
        me.callParent(arguments);
    }
});

Ext.define('phWin1', {
    extend: 'Ext.window.Window',
    height: 275,
    width: 653,
    modal: true,
    layout: 'border',
    initComponent: function () {
        var me = this;
        var UserID = me.UserID;


        me.items = [{
            xtype: 'UploaderPanel',
            id: 'uploadproductpic1',
            region: 'center',
            autoScroll: true,
            dockedItems: [{
                xtype: 'toolbar',
                dock: 'top',
                items: [{
                    xtype: 'filefield',
                    fieldLabel: '上传图片',
                    width: 300,
                    buttonText: '浏览'
                }, {
                    xtype: 'button',
                    text: '上传',
                    iconCls: 'upload',
                    handler: function () {
                        Ext.getCmp('uploadproductpic1').upload('CZCLZ.YHGLClass.UploadPicForProduct1', function (retVal) {
                            var isDefault = false;
                            if (retVal.isdefault == 1)
                                isDefault = true;
                            Ext.getCmp('uploadproductpic1').add(new SelectImg({
                                isSelected: isDefault,
                                src: retVal.fileurl,
                                fileid: retVal.fileid
                            }));
                        }, CS.onError, UserID);
                    }
                }]
            }],
            buttonAlign: 'center',
            buttons: [
                //{
                //    text: '设为默认',
                //    handler: function () {
                //        Ext.MessageBox.confirm('确认', '是否设置该图片为默认图片？', function (btn) {
                //            if (btn == 'yes') {
                //                var selPics = Ext.getCmp('uploadproductpic').query('image[isSelected=true]');
                //                if (selPics.length > 0) {
                //                    CS('CZCLZ.YHGLClass.SetDefaultPicForProduct', function (retVal) {
                //                        if (retVal)
                //                            Ext.Msg.alert('提示', '设置成功！');
                //                        else
                //                            Ext.Msg.alert('提示', '设置失败！');
                //                    }, CS.onError, selPics[0].fileid, UserID);
                //                }
                //            }
                //        });
                //    }
                //},
                {
                    text: '删除',
                    handler: function () {
                        Ext.MessageBox.confirm('确认', '是否删除该图片？', function (btn) {
                            if (btn == 'yes') {
                                var selPics = Ext.getCmp('uploadproductpic1').query('image[isSelected=true]');
                                if (selPics.length > 0) {
                                    CS('CZCLZ.YHGLClass.DelProductImageByPicID', function (retVal) {
                                        if (retVal) {
                                            Ext.getCmp('uploadproductpic1').remove(selPics[0]);
                                        }
                                    }, CS.onError, selPics[0].fileid, UserID, 99);
                                }
                            }
                        });
                    }
                }
            ]
        }];
        me.callParent(arguments);
    }
});

Ext.define('addWin', {
    extend: 'Ext.window.Window',

    height: 500,
    width: 450,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '用户管理',

    initComponent: function () {
        var me = this;
        var oldv = me.oldv;
        me.items = [
            {
                xtype: 'form',
                id: 'addform',
                bodyPadding: 10,
                bodyStyle: 'overflow-x:hidden;overflow-y:auto',
                items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: 'ID',
                        id: 'UserID',
                        name: 'UserID',
                        labelWidth: 80,
                        hidden: true,
                        colspan: 2
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '用户名',
                        id: 'UserName',
                        name: 'UserName',
                        labelWidth: 80,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '密码',
                        id: 'Password',
                        name: 'Password',
                        labelWidth: 80,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '真实姓名',
                        id: 'UserXM',
                        name: 'UserXM',
                        labelWidth: 80,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '电话',
                        id: 'UserTel',
                        name: 'UserTel',
                        labelWidth: 80,
                        anchor: '100%'
                    },
                    {
                        xtype: 'combobox',
                        id: 'dqS',
                        name: 'dqS',
                        anchor: '100%',
                        fieldLabel: '所属地(省)',
                        allowBlank: false,
                        editable: false,
                        labelWidth: 80,
                        store: sdqstore,
                        queryMode: 'local',
                        displayField: 'dq_mc',
                        valueField: 'dq_bm',
                        value: '',
                        listeners: {
                            'select': function (o) {
                                Ext.getCmp("DqBm").setValue();
                                CS('CZCLZ.YHGLClass.GetDQ', function (retVal) {
                                    dqstore.loadData(retVal);
                                }, CS.onError, Ext.getCmp("dqS").getValue());
                            }
                        }
                    },
                    {
                        xtype: 'combobox',
                        id: 'DqBm',
                        name: 'DqBm',
                        anchor: '100%',
                        fieldLabel: '所属地(市)',
                        allowBlank: false,
                        editable: false,
                        labelWidth: 80,
                        store: dqstore,
                        queryMode: 'local',
                        displayField: 'dq_mc',
                        valueField: 'dq_bm',
                        value: ''
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '线路起点',
                        id: 'FromRoute',
                        name: 'FromRoute',
                        labelWidth: 80,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '线路终点',
                        id: 'ToRoute',
                        name: 'ToRoute',
                        labelWidth: 80,
                        anchor: '100%'
                    },
                    {
                        xtype: 'combobox',
                        id: 'roleId',
                        name: 'roleId',
                        anchor: '100%',
                        fieldLabel: '角色',
                        allowBlank: false,
                        editable: false,
                        labelWidth: 80,
                        store: roleStore1,
                        queryMode: 'local',
                        displayField: 'ClientName',
                        valueField: 'ClientKind',
                        value: ''
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '身份证号',
                        id: 'IDCard',
                        name: 'IDCard',
                        labelWidth: 80,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textareafield',
                        id: 'Address',
                        name: 'Address',
                        labelWidth: 80,
                        fieldLabel: '地址',
                        anchor: '100%'
                    },
                    {
                        xtype: 'textareafield',
                        id: 'searchAddress',
                        name: 'searchAddress',
                        labelWidth: 80,
                        fieldLabel: '搜索地址',
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '车主账号',
                        id: 'caruser',
                        name: 'caruser',
                        labelWidth: 80,
                        anchor: '100%'
                    },
                    {
                        xtype: 'combobox',
                        id: 'carriagegetmode',
                        name: 'carriagegetmode',
                        anchor: '100%',
                        fieldLabel: '干线运输类型',
                        editable: false,
                        allowBlank: false,
                        labelWidth: 80,
                        queryMode: 'local',
                        displayField: 'TEXT',
                        valueField: 'VALUE',
                        store: Ext.create('Ext.data.Store', {
                            fields: ['VALUE', 'TEXT'],
                            data: [
                                { 'VALUE': 0, 'TEXT': '授信' }, { 'VALUE': 1, 'TEXT': '现金' }
                            ]
                        }),
                        value: 1,
                        listeners: {
                            select: function (combo, record, opts) {
                                if (record[0].data.VALUE == 1) {
                                    Ext.getCmp("modetype").hide();
                                    Ext.getCmp("modetype").setValue();

                                    Ext.getCmp("carriageoilrate").hide()
                                    Ext.getCmp("carriagemoneyrate").hide();
                                    Ext.getCmp("carriageoilrate").setValue();
                                    Ext.getCmp("carriagemoneyrate").setValue();
                                    Ext.getCmp("carriageoilrate").allowBlank = true;
                                    Ext.getCmp("carriagemoneyrate").allowBlank = true;

                                    Ext.getCmp('modecoefficient').decimalPrecision = 3;

                                } else if (record[0].data.VALUE == 0) {
                                    Ext.getCmp("modetype").show();

                                    Ext.getCmp("carriageoilrate").show();
                                    Ext.getCmp("carriagemoneyrate").show();
                                    Ext.getCmp("carriageoilrate").allowBlank = false;
                                    Ext.getCmp("carriagemoneyrate").allowBlank = false;

                                    Ext.getCmp('modecoefficient').decimalPrecision = 2;

                                }
                            }
                        }

                    },
                    {
                        xtype: 'combobox',
                        id: 'modetype',
                        name: 'modetype',
                        anchor: '100%',
                        fieldLabel: '干线运输模式',
                        editable: false,
                        labelWidth: 80,
                        queryMode: 'local',
                        displayField: 'TEXT',
                        valueField: 'VALUE',
                        store: Ext.create('Ext.data.Store', {
                            fields: ['VALUE', 'TEXT'],
                            data: [
                                { 'VALUE': 1, 'TEXT': '模式一' }, { 'VALUE': 2, 'TEXT': '模式二' }
                            ]
                        }),
                        value: 2,
                        listeners: {
                            select: function (combo, record, opts) {
                                if (Ext.getCmp("modetype").getValue() == 1 && record[0].data.VALUE == 1) {
                                    Ext.getCmp("carriageoilrate").show();
                                    Ext.getCmp("carriagemoneyrate").show();
                                    Ext.getCmp("carriageoilrate").allowBlank = false;
                                    Ext.getCmp("carriagemoneyrate").allowBlank = false;
                                } else {
                                    Ext.getCmp("carriageoilrate").hide()
                                    Ext.getCmp("carriagemoneyrate").hide();
                                    Ext.getCmp("carriageoilrate").setValue();
                                    Ext.getCmp("carriagemoneyrate").setValue();
                                    Ext.getCmp("carriageoilrate").allowBlank = true;
                                    Ext.getCmp("carriagemoneyrate").allowBlank = true;
                                }
                            }
                        }
                    },
                    {
                        xtype: 'numberfield',
                        id: 'carriageoilrate',
                        name: 'carriageoilrate',
                        fieldLabel: '油卡比例',
                        labelWidth: 80,
                        anchor: '100%',
                        minValue: 10,
                        maxValue: 90,
                        hidden: true,
                        listeners: {
                            'blur': function () {
                                if (Ext.getCmp("carriageoilrate").getValue() % 10 == 0) {
                                    Ext.getCmp("carriagemoneyrate").setValue(100 - Ext.getCmp("carriageoilrate").getValue());
                                    return true;
                                } else {
                                    Ext.getCmp("carriageoilrate").setValue();
                                    alert("输入的值必须为整十数！");
                                }
                            }
                        }

                    },
                    {
                        xtype: 'numberfield',
                        id: 'carriagemoneyrate',
                        name: 'carriagemoneyrate',
                        fieldLabel: '现金比例',
                        labelWidth: 80,
                        anchor: '100%',
                        minValue: 10,
                        maxValue: 90,
                        hidden: true,
                        listeners: {
                            'blur': function () {
                                if (Ext.getCmp("carriagemoneyrate").getValue() % 10 == 0) {
                                    Ext.getCmp("carriageoilrate").setValue(100 - Ext.getCmp("carriagemoneyrate").getValue());
                                    return true;
                                } else {
                                    Ext.getCmp("carriagemoneyrate").setValue();
                                    alert("输入的值必须为整十数！");
                                }
                            }
                        }
                    },
                    {
                        xtype: 'numberfield',
                        id: 'modecoefficient',
                        name: 'modecoefficient',
                        fieldLabel: '模式系数',
                        labelWidth: 80,
                        anchor: '100%',
                        minValue: 0.01,
                        maxValue: 0.99,
                        value: 0.94
                    },
                    {
                        xtype: 'numberfield',
                        id: 'carriagemaxmoney',
                        name: 'carriagemaxmoney',
                        fieldLabel: '承运最大限额',
                        labelWidth: 80,
                        anchor: '100%',
                        minValue: 0,
                        decimalPrecision: 2,
                        value: 0
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '设备号',
                        id: 'mirrornumber',
                        name: 'mirrornumber',
                        labelWidth: 80,
                        anchor: '100%'
                    },
                    {
                        xtype: 'combobox',
                        id: 'carriagechbid',
                        name: 'carriagechbid',
                        anchor: '100%',
                        fieldLabel: '开票抬头',
                        allowBlank: false,
                        editable: false,
                        labelWidth: 80,
                        store: kpstore,
                        queryMode: 'local',
                        displayField: 'name',
                        valueField: 'id',
                        value: ''
                    },
                    {
                        xtype: 'combobox',
                        id: 'isidentification',
                        name: 'isidentification',
                        anchor: '100%',
                        fieldLabel: '查货宝认证',
                        allowBlank: false,
                        editable: false,
                        labelWidth: 80,
                        store: Ext.create('Ext.data.Store', {
                            fields: ['VALUE', 'TEXT'],
                            data: [
                                { 'VALUE': 0, 'TEXT': '否' }, { 'VALUE': 1, 'TEXT': '是' }
                            ]
                        }),
                        queryMode: 'local',
                        displayField: 'TEXT',
                        valueField: 'VALUE'
                    },
                    {
                        xtype: 'combobox',
                        id: 'isdonate',
                        name: 'isdonate',
                        anchor: '100%',
                        fieldLabel: '是否下单送红包',
                        allowBlank: false,
                        editable: false,
                        labelWidth: 80,
                        store: Ext.create('Ext.data.Store', {
                            fields: ['VALUE', 'TEXT'],
                            data: [
                                { 'VALUE': 0, 'TEXT': '赠送' }, { 'VALUE': 1, 'TEXT': '不赠送' }
                            ]
                        }),
                        queryMode: 'local',
                        displayField: 'TEXT',
                        valueField: 'VALUE',
                        value: 1,
                        hidden: true
                    },
                    {
                        xtype: 'numberfield',
                        id: 'donateratio',
                        name: 'donateratio',
                        fieldLabel: '下单送红包比例',
                        labelWidth: 80,
                        anchor: '100%',
                        minValue: 1,
                        maxValue: 99,
                        value: 5,
                        allowDecimals: false,
                        hidden: true
                    },
                    {
                        xtype: 'combobox',
                        id: 'isshowsource',
                        name: 'isshowsource',
                        anchor: '100%',
                        fieldLabel: '是否显示发布来源',
                        allowBlank: false,
                        editable: false,
                        labelWidth: 80,
                        store: Ext.create('Ext.data.Store', {
                            fields: ['VALUE', 'TEXT'],
                            data: [
                                { 'VALUE': 0, 'TEXT': '显示' }, { 'VALUE': 1, 'TEXT': '不显示' }
                            ]
                        }),
                        queryMode: 'local',
                        displayField: 'TEXT',
                        valueField: 'VALUE',
                        value: 0,
                        hidden: true
                    },
                    {
                        xtype: 'combobox',
                        id: 'iscost',
                        name: 'iscost',
                        anchor: '100%',
                        fieldLabel: '是否收取佣金',
                        allowBlank: false,
                        editable: false,
                        labelWidth: 80,
                        store: Ext.create('Ext.data.Store', {
                            fields: ['VALUE', 'TEXT'],
                            data: [
                                { 'VALUE': 0, 'TEXT': '是' }, { 'VALUE': 1, 'TEXT': '否' }
                            ]
                        }),
                        queryMode: 'local',
                        displayField: 'TEXT',
                        valueField: 'VALUE',
                        value: 1,
                        hidden: true
                    },
                    {
                        xtype: 'combobox',
                        id: 'ischbmember',
                        name: 'ischbmember',
                        anchor: '100%',
                        fieldLabel: '是否查货宝会员',
                        allowBlank: false,
                        editable: false,
                        labelWidth: 80,
                        store: Ext.create('Ext.data.Store', {
                            fields: ['VALUE', 'TEXT'],
                            data: [
                                { 'VALUE': 0, 'TEXT': '是' }, { 'VALUE': 1, 'TEXT': '否' }
                            ]
                        }),
                        queryMode: 'local',
                        displayField: 'TEXT',
                        valueField: 'VALUE',
                        value: 1,
                        hidden: true
                    }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '确定',
                        iconCls: 'dropyes',
                        handler: function () {
                            var form = Ext.getCmp('addform');
                            if (form.form.isValid()) {
                                var values = form.form.getValues(false);
                                var nr = "";
                                if (values.carriagegetmode == 1) {
                                    values.modetype = 1;
                                }
                                for (var key in values) {
                                    if (oldv.hasOwnProperty(key)) {

                                        if (values[key] != oldv[key]) {
                                            if (!(oldv[key] == null && values[key] == "")) {
                                                nr += key + "：" + oldv[key] + " 改为 " + values[key] + ",";
                                            }
                                        }
                                    }
                                }
                                var me = this;
                                CS('CZCLZ.YHGLClass.SaveClient', function (retVal) {
                                    if (retVal) {
                                        me.up('window').close();
                                        getUser(1);
                                    }
                                }, CS.onError, values, nr.substr(0, nr.length - 1));

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


Ext.define('EWMWin', {
    extend: 'Ext.window.Window',

    height: 300,
    width: 400,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '查看二维码',

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'panel',
                region: 'center',
                width: 150,
                html: '<table border="0" cellspacing="0" cellpadding="0" width="100%" style="margin-top:30px;">'
                    + ' <tr>'
                    + '   <td align="center"> <div id="qrcodeTable"></div></td>'
                    + ' </tr>'
                    + '</table>',
                buttonAlign: 'center',
                buttons: [
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

Ext.define('EWMWin1', {
    extend: 'Ext.window.Window',

    height: 670,
    width: 600,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '查看绑定二维码',

    initComponent: function () {
        var me = this;
        var id = me.id;
        me.items = [
            {
                xtype: 'panel',
                region: 'center',
                width: 150,
                html: '<table border="0" cellspacing="0" cellpadding="0" width="100%" style="margin-top:10px;">'
                    + ' <tr>'
                    + '   <td align="center"> <div id="qrcodeTable1"></div></td>'
                    + ' </tr>'
                    + '</table>',
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '下载',
                        handler: function () {
                            DownloadFile("CZCLZ.YHGLClass.GetEWMToFile1", "二维码.jpg", id);
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


Ext.define('OrderList', {
    extend: 'Ext.window.Window',

    height: 422,
    width: 516,
    layout: {
        type: 'fit'
    },
    title: '记录列表',
    modal: true,

    initComponent: function () {
        var me = this;

        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'tabpanel',
                    activeTab: 1,
                    items: [
                        {
                            xtype: 'panel',
                            layout: {
                                type: 'fit'
                            },
                            title: '交易记录',
                            hidden: true,
                            items: [
                                {
                                    xtype: 'gridpanel',
                                    columnLines: 1,
                                    border: 1,
                                    store: orderstore,
                                    columns: [
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'KIND',
                                            sortable: false,
                                            menuDisabled: true,
                                            flex: 1,
                                            text: '类别'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'UserName',
                                            sortable: false,
                                            menuDisabled: true,
                                            flex: 1,
                                            text: '交易对象'
                                        },
                                        {
                                            xtype: 'datecolumn',
                                            dataIndex: 'DATE',
                                            format: 'Y-m-d',
                                            sortable: false,
                                            menuDisabled: true,
                                            flex: 1,
                                            text: '交易时间'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'MONEY',
                                            sortable: false,
                                            menuDisabled: true,
                                            flex: 1,
                                            text: '交易金额'
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
                            title: '运费券余额',
                            items: [
                                {
                                    xtype: 'gridpanel',
                                    columnLines: 1,
                                    border: 1,
                                    store: moneystore,
                                    columns: [
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'UserName',
                                            sortable: false,
                                            menuDisabled: true,
                                            flex: 1,
                                            text: '专线'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'MONEY',
                                            sortable: false,
                                            menuDisabled: true,
                                            flex: 1,
                                            text: '运费券金额'
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

Ext.define('upRedNumWin', {
    extend: 'Ext.window.Window',
    height: 275,
    width: 350,
    modal: true,
    layout: {
        type: 'fit'
    },
    title: '设置下单送红包上限额度',
    initComponent: function () {
        var me = this;
        var userid = me.UserID
        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'form',
                    id: 'yjform',
                    bodyPadding: 10,
                    items: [
                        {
                            xtype: 'numberfield',
                            id: 'redenvelopequota',
                            name: 'redenvelopequota',
                            fieldLabel: '现有额度',
                            labelWidth: 80,
                            value: 0,
                            allowDecimals: false,
                            anchor: '100%',
                            readOnly: true
                        },
                        {
                            xtype: 'numberfield',
                            id: 'addnum',
                            name: 'addnum',
                            fieldLabel: '增加额度',
                            labelWidth: 80,
                            value: 0,
                            allowDecimals: false,
                            anchor: '100%',
                            minValue: 0
                        }
                    ],
                    buttonAlign: 'center',
                    buttons: [
                        {
                            text: '确认',
                            iconCls: 'dropyes',
                            handler: function () {
                                CS('CZCLZ.YHGLClass.AddRedNum', function (retVal) {
                                    if (retVal) {
                                        getUser(1);
                                        Ext.MessageBox.alert('提示', "修改成功！");
                                    }
                                }, CS.onError, userid, Ext.getCmp("redenvelopequota").getValue(), Ext.getCmp("addnum").getValue());
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
            ]
        });

        me.callParent(arguments);
    }
});
//************************************弹出界面***************************************

//************************************主界面*****************************************
Ext.onReady(function () {
    Ext.define('YhView', {
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
                    title: '',
                    store: store,
                    columnLines: true,
                    selModel: Ext.create('Ext.selection.CheckboxModel', {

                    }),
                    columns: [Ext.create('Ext.grid.RowNumberer'),
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'UserName',
                        sortable: false,
                        menuDisabled: true,
                        text: "登录名"
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'ClientKind',
                        sortable: false,
                        menuDisabled: true,
                        text: "角色",
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            switch (value) {
                                case 1:
                                    return "专线";
                                    break;
                                case 2:
                                    return "三方";
                                    break;
                            }
                        }
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'UserXM',
                        sortable: false,
                        menuDisabled: true,
                        text: "姓名"
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'UserTel',
                        sortable: false,
                        menuDisabled: true,
                        text: "电话"
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'FromRoute',
                        sortable: false,
                        menuDisabled: true,
                        hidden: true,
                        text: "起点"
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'ToRoute',
                        sortable: false,
                        menuDisabled: true,
                        hidden: true,
                        text: "终点"
                    },
                    {
                        xtype: 'gridcolumn',
                        sortable: false,
                        menuDisabled: true,
                        text: "线路",
                        width: 240,
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            var str = "";
                            if (record.data.FromRoute) {
                                str += record.data.FromRoute
                            }
                            if (record.data.ToRoute) {
                                str += "─" + record.data.ToRoute;
                            }
                            return str;
                        }

                    },
                    {
                        xtype: 'datecolumn',
                        dataIndex: 'AddTime',
                        format: 'Y-m-d',
                        sortable: false,
                        menuDisabled: true,
                        width: 110,
                        text: '注册时间'
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'redenvelopequota',
                        sortable: false,
                        menuDisabled: true,
                        text: "赠送红包额度"
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'KFGMPoints',
                        width: 140,
                        sortable: false,
                        menuDisabled: true,
                        text: "可开放购买运费券"
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'Points',
                        sortable: false,
                        menuDisabled: true,
                        width: 140,
                        text: "专线用户剩余运费券"
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'SalePoints',
                        sortable: false,
                        menuDisabled: true,
                        width: 140,
                        text: "专线用户在售运费券"
                    },
                    {
                        xtype: 'gridcolumn',
                        sortable: false,
                        menuDisabled: true,
                        text: "模式类型",
                        dataIndex: 'modetype',
                        width: 100,
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            var str = "";
                            if (value == 1) {
                                str = "模式一";
                            } else if (value == 2) {
                                str = "模式二";
                            }
                            return str;
                        }

                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'modecoefficient',
                        sortable: false,
                        menuDisabled: true,
                        width: 90,
                        text: "模式系数"
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'carriagemaxmoney',
                        sortable: false,
                        menuDisabled: true,
                        width: 90,
                        text: "承运最大限额"
                    },
                    {
                        text: '操作',
                        dataIndex: 'UserID',
                        width: 700,
                        sortable: false,
                        menuDisabled: true,
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            var str;
                            if (record.data.ClientKind == 1) {
                                str = "<a onclick='EditUser(\"" + value + "\");'>修改</a> <a onclick='IsBdBf(\"" + record.data.UserName + "\");'>查看是否绑定宝付账号</a> <a onclick='LookLists(\"" + value + "\");'>查看记录</a> <a onclick='LookEWM(\"" + record.data.ewmbs + "\");'>查看二维码</a> <a onclick='AddPhoto(\"" + value + "\");'>添加照片</a> <a onclick='GLSJ(\"" + value + "\");'>关联司机</a>  <a onclick='AddPhoto1(\"" + value + "\");'>添加认证照片</a>  <a onclick='buyRedNum(\"" + value + "\",\"" + record.data.redenvelopequota + "\");'>下单购买红包上限</a>";
                            } else if (record.data.ClientKind == 2) {
                                str = "<a onclick='EditUser(\"" + value + "\");'>修改</a> <a onclick='IsBdBf(\"" + record.data.UserName + "\");'>查看是否绑定宝付账号</a> <a onclick='LookLists(\"" + value + "\");'>查看记录</a> <a onclick='LookEWM(\"" + record.data.ewmbs + "\");'>查看二维码</a> <a onclick='AddPhoto(\"" + value + "\");'>添加照片</a> <a onclick='LookEWM1(\"" + record.data.UserID + "\");'>查看绑定二维码</a> <a onclick='GLSJ(\"" + value + "\");'>关联司机</a>";
                            }
                            if (record.data.IsSHPass == 1) {
                                str += " <a onclick='shpass(\"" + value + "\",\"0\");'>下架</a>";
                            } else if (record.data.IsSHPass == 0) {
                                str += " <a onclick='shpass(\"" + value + "\",\"1\");'>重新上架</a>";
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
                                    xtype: 'combobox',
                                    id: 'cx_role',
                                    width: 160,
                                    fieldLabel: '角色',
                                    editable: false,
                                    labelWidth: 40,
                                    store: roleStore,
                                    queryMode: 'local',
                                    displayField: 'ClientName',
                                    valueField: 'ClientKind',
                                    value: ''
                                },
                                {
                                    xtype: 'textfield',
                                    id: 'cx_yhm',
                                    width: 140,
                                    labelWidth: 50,
                                    fieldLabel: '用户名'
                                },
                                {
                                    xtype: 'textfield',
                                    id: 'cx_xm',
                                    width: 160,
                                    labelWidth: 70,
                                    fieldLabel: '真实姓名'
                                },
                                {
                                    id: 'cx_beg',
                                    xtype: 'datefield',
                                    fieldLabel: '注册时间',
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
                                    xtype: 'combobox',
                                    id: 'cx_isdonate',
                                    width: 220,
                                    fieldLabel: '是否配置赠送红包',
                                    editable: false,
                                    labelWidth: 110,
                                    store: isdonateStore,
                                    queryMode: 'local',
                                    displayField: 'text',
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
                                                getUser(1);
                                            }
                                        },
                                        //{
                                        //    xtype: 'button',
                                        //    iconCls: 'delete',
                                        //    text: '删除',
                                        //    handler: function () {
                                        //        var idlist = [];
                                        //        var grid = Ext.getCmp("usergrid");
                                        //        var rds = grid.getSelectionModel().getSelection();
                                        //        if (rds.length == 0) {
                                        //            Ext.Msg.show({
                                        //                title: '提示',
                                        //                msg: '请选择至少一条要删除的记录!',
                                        //                buttons: Ext.MessageBox.OK,
                                        //                icon: Ext.MessageBox.INFO
                                        //            });
                                        //            return;
                                        //        }

                                        //        Ext.MessageBox.confirm('删除提示', '是否要删除数据!', function (obj) {
                                        //            if (obj == "yes") {
                                        //                for (var n = 0, len = rds.length; n < len; n++) {
                                        //                    var rd = rds[n];

                                        //                    idlist.push(rd.get("UserID"));
                                        //                }

                                        //                CS('CZCLZ.YHGLClass.DelUser', function (retVal) {
                                        //                    if (retVal) {
                                        //                        getUser(1);
                                        //                    }
                                        //                }, CS.onError, idlist);
                                        //            }
                                        //            else {
                                        //                return;
                                        //            }
                                        //        });
                                        //    }
                                        //},
                                        {
                                            xtype: 'button',
                                            iconCls: 'view',
                                            text: '导出专线用户统计表',
                                            handler: function () {
                                                DownloadFile("CZCLZ.YHGLClass.GetZXUSERToFile", "专线用户统计表.xls", Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), Ext.getCmp("cx_isdonate").getValue());
                                            }
                                        },
                                        {
                                            xtype: 'button',
                                            iconCls: 'view',
                                            text: '导出三方用户统计表',
                                            handler: function () {
                                                DownloadFile("CZCLZ.YHGLClass.GetSFUSERToFile", "三方用户统计表.xls", Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
                                            }
                                        },
                                        {
                                            xtype: 'button',
                                            iconCls: 'view',
                                            text: '导出三方用户剩余运费券',
                                            handler: function () {
                                                DownloadFile("CZCLZ.YHGLClass.GetSFYFQToFile", "三方用户剩余运费券.xls", Ext.getCmp("cx_role").getValue(), Ext.getCmp("cx_yhm").getValue(), Ext.getCmp("cx_xm").getValue());
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

    new YhView();

    Ext.getCmp("cx_role").setValue('');

    cx_role = Ext.getCmp("cx_role").getValue();
    cx_yhm = Ext.getCmp("cx_yhm").getValue();
    cx_xm = Ext.getCmp("cx_xm").getValue();
    cx_beg = Ext.getCmp("cx_beg").getValue();
    cx_end = Ext.getCmp("cx_end").getValue();

    getUser(1);

})
//************************************主界面*****************************************