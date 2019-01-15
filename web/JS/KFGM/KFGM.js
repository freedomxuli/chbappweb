//

var pageSize = 15;
var cx_yhm;
//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'PlatPointId' },
       { name: 'companyId' },
       { name: 'UserID' },
       { name: 'UserName' },
       { name: 'UserXM' },
       { name: 'Points' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getList(nPage);
    }
});

var hisStore = Ext.create('Ext.data.Store', {
    fields: [
        'PlatToSaleId', 'UserID', 'UserXM', 'points', 'discount', 'discountmemo'
    ]
})

var sfstore = Ext.create('Ext.data.Store', {
    fields: [
        'UserID', 'UserName'
    ]
})

//************************************数据源*****************************************

//************************************页面方法***************************************

function getList(nPage) {
    CS('CZCLZ.KFGMMag.GetList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_yhm").getValue());
}

function getHisSale(UserID) {
    CS('CZCLZ.KFGMMag.getHisSale', function (retVal) {
        if (retVal) {
            hisStore.loadData(retVal);
        }
    }, CS.onError, UserID);
}


function kfgm(id) {
    var r = store.findRecord("PlatPointId", id).data;
    //var win = new KFList({ PlatPointId: id, MaxPoint: r.Points, UserID: r.UserID });
    //win.show(null,function(){
    //    getHisSale(r.UserID);
    //});
    CS('CZCLZ.KFGMMag.getHisSale', function (retVal) {
        if (retVal.length > 0) {
            if (retVal[0]["points"] > 0) {
                Ext.Msg.show({
                    title: '提示',
                    msg: '线上还有剩余' + retVal[0]["points"] + '运费券没卖完！不能开放！',
                    buttons: Ext.MessageBox.OK,
                    icon: Ext.MessageBox.INFO
                });
                return;
            } else {
                var win = new addWin();
                win.show(null, function () {
                    Ext.getCmp("PlatPointId").setValue(id);
                    Ext.getCmp("MaxPoint").setValue(r.Points);

                    if (retVal) {
                        var str = "线上剩余运费券:" + retVal[0]["points"] + " 线上折扣:" + retVal[0]["discountmemo"];
                        Ext.getCmp("zspoint").setText(str);
                        Ext.getCmp("PlatToSaleId").setValue(retVal[0]["PlatToSaleId"]);
                    }
                });
            }
        } else {
            var win = new addWin();
            win.show(null, function () {
                Ext.getCmp("PlatPointId").setValue(id);
                Ext.getCmp("MaxPoint").setValue(r.Points);
            });
        }
    }, CS.onError, r.UserID);
}

function CXKF(PlatToSaleId, UserID, PlatPointId, MaxPoint, discount, discountmemo) {
    var win = new addWin();
    win.show(null, function () {
        Ext.getCmp("UserId").setValue(UserID);
        Ext.getCmp("PlatToSaleId").setValue(PlatToSaleId);
        Ext.getCmp("PlatPointId").setValue(PlatPointId);
        Ext.getCmp("MaxPoint").setValue(MaxPoint);
        Ext.getCmp("discount").setValue(discount);
        Ext.getCmp("discount").setDisabled(true);
        Ext.getCmp("discountmemo").setValue(discountmemo);
        Ext.getCmp("discountmemo").setDisabled(true);
    });
}

function ps(PlatToSaleId,UserID, points) {
    if (points > 0) {
        var win = new PSWin({ PlatToSaleId: PlatToSaleId, UserID: UserID, points: points });
        win.show(null, function () {
        });
    } else {
        Ext.Msg.show({
            title: '错误',
            msg: '暂无运费券可派送',
            buttons: Ext.MessageBox.OK,
            icon: Ext.MessageBox.ERROR
        });
    }
}
//************************************页面方法***************************************

//************************************弹出界面***************************************
Ext.define('KFList', {
    extend: 'Ext.window.Window',

    height: 407,
    width: 634,
    layout: {
        type: 'fit'
    },
    title: '历史开放',
    modal: true,

    initComponent: function () {
        var me = this;

        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'gridpanel',
                    border: 1,
                    columnLines: 1,
                    store: hisStore,

                    columns: [
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'UserXM',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: '专线'
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'points',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: '线上剩余运费券'
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'discountmemo',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: '线上折扣'
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'PlatToSaleId',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: '操作',
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                return "<a href='javascsript:void(0);' onclick='CXKF(\"" + value + "\",\"" + record.data.UserID + "\",\"" + me.PlatPointId + "\",\"" + me.MaxPoint + "\",\"" + record.data.discount + "\",\"" + record.data.discountmemo + "\");'>重新开放</a>";
                            }
                        }
                    ]
                }
            ],
            buttonAlign: 'center',
            buttons: [
                {
                    text: '新增开放折扣',
                    iconCls: 'add',
                    handler: function () {
                        var win = new addWin();
                        win.show(null, function () {
                            Ext.getCmp("PlatPointId").setValue(me.PlatPointId);
                            Ext.getCmp("MaxPoint").setValue(me.MaxPoint);
                            Ext.getCmp("UserId").setValue(me.UserID);
                            Ext.getCmp("PlatToSaleId").setValue("");
                        })
                    }
                },
                {
                    text: '关闭',
                    iconCls: 'close',
                    handler: function () {
                        me.close();
                    }
                }
            ]
        });

        me.callParent(arguments);
    }

});

Ext.define('addWin', {
    extend: 'Ext.window.Window',

    height: 250,
    width: 400,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '开放购买运费券',

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
                        id: 'UserId',
                        name: 'UserId',
                        hidden: true
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: 'ID',
                        id: 'PlatToSaleId',
                        name: 'PlatToSaleId',
                        hidden: true
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: 'ID',
                        id: 'PlatPointId',
                        name: 'PlatPointId',
                        hidden: true
                    },
                    {
                        xtype: 'numberfield',
                        id: 'MaxPoint',
                        name: 'MaxPoint',
                        hidden: true
                    },
                    {
                        xtype: 'numberfield',
                        fieldLabel: '分数',
                        id: 'points',
                        name: 'points',
                        allowBlank: false,
                        allowDecimals: false,
                        allowNegative: false,
                        minValue: 1,
                        anchor: '100%'
                    },

                    {
                        xtype: 'numberfield',
                        fieldLabel: '优惠折扣',
                        id: 'discount',
                        name: 'discount',
                        allowDecimals: true,    //是否允许小数
                        decimalPrecision: 2,    // 精确的位数
                        allowNegative: false,
                        minValue: 0,
                        maxValue: 1,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: "label",
                        text: "*优惠折扣必须为介于0-1的两位小数！",
                        style: "color:red;padding-left:100px;"
                    },
                     {
                         xtype: 'numberfield',
                         fieldLabel: '有效期限',
                         id: 'validHour',
                         name: 'validHour',
                         allowDecimals: false,    //是否允许小数
                         //decimalPrecision: 2,    // 精确的位数
                         allowNegative: false,
                         minValue: 72,
                         maxValue: 360,
                         allowBlank: false,
                         anchor: '100%'
                     },
                    {
                        xtype: "label",
                        text: "*有效期限必须为介于72-360小时！",
                        style: "color:red;padding-left:100px;"
                    },
                    {
                        xtype: 'textareafield',
                        id: 'discountmemo',
                        name: 'discountmemo',
                        fieldLabel: '优惠折扣备注',
                        allowBlank: false,
                        anchor: '100%'
                    },
                     {
                         xtype: "label",
                         id: "zspoint",
                         text: "",
                         style: "padding-left:100px;"
                     }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '确定',
                        iconCls: 'dropyes',
                        handler: function () {
                            var point = Ext.getCmp("points").getValue();
                            if (point < 1) {
                                Ext.Msg.show({
                                    title: '提示',
                                    msg: '开放购买的运费券必须大于1',
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.INFO
                                });
                                return;
                            }

                            var maxpoint = Ext.getCmp("MaxPoint").getValue();
                            if (point > maxpoint) {
                                Ext.Msg.show({
                                    title: '提示',
                                    msg: '开放购买的运费券不得超过其所有运费券',
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.INFO
                                });
                                return;
                            }
                            Ext.getCmp("discount").setDisabled(false);
                            Ext.getCmp("discountmemo").setDisabled(false);
                            var form = Ext.getCmp('addform');
                            if (form.form.isValid()) {
                                var values = form.form.getValues(false);
                                console.log(values);
                                var me = this;
                                CS('CZCLZ.KFGMMag.SaveKFGM', function (retVal) {
                                    if (retVal) {
                                        getHisSale(Ext.getCmp("UserId").getValue());
                                        me.up('window').close();
                                        getList(1);
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

Ext.define('PSWin', {
    extend: 'Ext.window.Window',

    height: 407,
    width: 634,
    layout: {
        type: 'fit'
    },
    title: '派送管理',
    modal: true,

    initComponent: function () {
        var me = this;
        var PlatToSaleId = me.PlatToSaleId;
        var UserID = me.UserID;
        var points = me.points;
        Ext.applyIf(me, {
            items: [

                {
                    xtype: 'gridpanel',
                    border: 1,
                    columnLines: 1,
                    store: sfstore,
                    dockedItems: [
                        
                        {
                            xtype: 'toolbar',
                            dock: 'top',
                            items: [
                                {
                                    xtype: 'numberfield',
                                    fieldLabel: '有效期限',
                                    id: 'psvalidHour',
                                    allowDecimals: false,    //是否允许小数
                                    //decimalPrecision: 2,    // 精确的位数
                                    allowNegative: false,
                                    minValue: 72,
                                    maxValue: 360,
                                    allowBlank: false,
                                },
                                {
                                    xtype: "label",
                                    text: "*有效期限必须为介于72-360小时！",
                                    style: "color:red;"
                                }
                            ]
                        },
                        {
                            xtype: 'toolbar',
                            dock: 'top',
                            items: [
                                {
                                    xtype: 'numberfield',
                                    fieldLabel: '派送运费券',
                                    allowBlank: false,
                                    allowDecimals: false,
                                    allowNegative: false,
                                    minValue: 1,
                                    id: 'psyfq'
                                },
                                {
                                    xtype: 'buttongroup',
                                    title: '',
                                    items: [
                                        {
                                            xtype: 'button',
                                            iconCls: 'view',
                                            text: '下载模板',
                                            handler: function () {
                                                window.location.href = "approot/r/JS/KFGM/派送三方模板.xls";
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
                                            iconCls: 'upload',
                                            text: '上传派送三方',
                                            handler: function () {
                                                var win = new drWin();
                                                win.show();
                                            }
                                        }
                                    ]
                                }
                            ]
                        }
                    ],
                    columns: [Ext.create('Ext.grid.RowNumberer'),
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'UserID',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            hidden: true
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'UserName',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: '三方'
                        }
                    ]
                }
            ],
            buttonAlign: 'center',
            buttons: [
                {
                    text: '确定',
                    iconCls: 'dropyes',
                    handler: function () {
                        if (!Ext.getCmp("psyfq").getValue()) {
                            Ext.Msg.show({
                                title: '错误',
                                msg: '请填写派送运费券！',
                                buttons: Ext.MessageBox.OK,
                                icon: Ext.MessageBox.ERROR
                            });
                            return;
                        } else {
                            if (Ext.getCmp("psyfq").getValue() > points) {
                                Ext.Msg.show({
                                    title: '错误',
                                    msg: '派送运费券不得大于可开放的数额！',
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.ERROR
                                });
                                return;
                            }
                        }
                        if (sfstore.data.items.length <= 0) {
                            Ext.Msg.show({
                                title: '错误',
                                msg: '请上传派送三方列表',
                                buttons: Ext.MessageBox.OK,
                                icon: Ext.MessageBox.ERROR
                            });
                            return;
                        }
                        if (!Ext.getCmp("psvalidHour").getValue()) {
                            Ext.Msg.show({
                                title: '错误',
                                msg: '请填写派送有效期限！',
                                buttons: Ext.MessageBox.OK,
                                icon: Ext.MessageBox.ERROR
                            });
                            return;
                        } else {
                            if (Ext.getCmp("psvalidHour").getValue() > 360 || Ext.getCmp("psvalidHour").getValue()<72) {
                                Ext.Msg.show({
                                    title: '错误',
                                    msg: '有效期限必须为介于72-360小时！',
                                    buttons: Ext.MessageBox.OK,
                                    icon: Ext.MessageBox.ERROR
                                });
                                return;
                            }
                        }
                      
                        var list = [];
                        for (var i = 0; i < sfstore.data.length; i++) {
                            list.push({ sanfanguserid: sfstore.data.items[i].data.UserID })
                        }
                        CS('CZCLZ.KFGMMag.SavePS', function (retVal) {
                            if (retVal) {
                                var result = retVal.evalJSON();
                                if (result.success) {
                                    Ext.MessageBox.alert('提示', "派送成功！");
                                    getList(1);
                                } else {
                                    Ext.MessageBox.alert('提示', result.details);
                                }
                            }
                        }, CS.onError, PlatToSaleId, UserID, Ext.getCmp("psyfq").getValue(), Ext.getCmp("psvalidHour").getValue(), JSON.stringify(list));
                        sfstore.removeAll();
                        this.up('window').close();
                        
                    }
                },
                {
                    text: '关闭',
                    iconCls: 'close',
                    handler: function () {
                        sfstore.removeAll();
                        me.close();
                    }
                }
            ]
        });

        me.callParent(arguments);
    }

});

//***************************上传附件层*********************************************************************************************************************
Ext.define('drWin', {
    extend: 'Ext.window.Window',

    height: 120,
    width: 350,
    id: "drWin",
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    title: '导入',

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'UploaderPanel',
                id: 'uploadproductpic',
                frame: true,
                bodyPadding: 10,
                title: '',
                items: [
            {
                xtype: 'filefield',
                fieldLabel: '上传文件',
                labelWidth: 70,
                buttonText: '浏览',
                allowBlank: false,
                anchor: '100%'
            }
                ],
                buttonAlign: 'center',
                buttons: [
            {
                text: '确定',
                handler: function () {
                    var form = Ext.getCmp('uploadproductpic');
                    if (form.form.isValid()) {
                        //取得表单中的内容
                        var values = form.form.getValues(false);
                        var me = this;
                        Ext.getCmp('uploadproductpic').upload('CZCLZ.KFGMMag.UploadSF', function (retVal) {
                            if (retVal) {
                                if (retVal.dt) {
                                    sfstore.loadData(retVal.dt);
                                    me.up('window').close();
                                }
                                if (retVal.str) {
                                    Ext.Msg.show({
                                        title: '提示',
                                        msg: retVal.str,
                                        buttons: Ext.MessageBox.OK,
                                        icon: Ext.MessageBox.INFO,
                                        fn: function () {
                                            me.up('window').close();
                                        }
                                    });
                                }
                            }
                        }, function (err) {
                            Ext.Msg.show({
                                title: '错误',
                                msg: err,
                                buttons: Ext.MessageBox.OK,
                                icon: Ext.MessageBox.ERROR,
                                fn: function () {
                                    me.up('window').close();
                                }
                            });
                        });
                    }
                }
            },
            {
                text: '关闭',
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

//************************************************************************************************************************************************************
//************************************弹出界面***************************************

//************************************主界面*****************************************
Ext.onReady(function () {
    Ext.define('KFGMView', {
        extend: 'Ext.container.Viewport',

        layout: {
            type: 'fit'
        },

        initComponent: function () {
            var me = this;
            me.items = [
                {
                    xtype: 'gridpanel',
                    id: 'KFGMGrid',
                    store: store,
                    columnLines: 1,
                    border: 1,
                    columns: [Ext.create('Ext.grid.RowNumberer'),
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'UserXM',
                            sortable: false,
                            menuDisabled: true,
                            width: 400,
                            text: '物流名称'
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'Points',
                            sortable: false,
                            menuDisabled: true,
                            width: 200,
                            text: '运费券'
                        },
                        {
                            xtype: 'gridcolumn',
                            sortable: false,
                            menuDisabled: true,
                            dataIndex: 'PlatPointId',
                            text: '操作',
                            width: 150,
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                str = "<a href='JavaScript:void(0)' onclick='kfgm(\"" + value + "\")'>开放购买</a> || <a href='JavaScript:void(0)' onclick='ps(\"" + value + '\",\"' + record.data.UserID + '\",\"' + record.data.Points + "\")'>派送</a>";
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
                                    id: 'cx_yhm',
                                    labelWidth: 60,
                                    fieldLabel: '物流名称'
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
                                                getList(1);
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
                                            text: '导出',
                                            iconCls: 'download',
                                            handler: function () {
                                                DownloadFile("CZCLZ.KFGMMag.GetKFGMToFile", "开放购买运费券.xls", Ext.getCmp("cx_yhm").getValue());
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

    new KFGMView();

    getList();
})
//************************************主界面*****************************************