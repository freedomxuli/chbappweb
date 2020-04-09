//-----------------------------------------------------------全局变量-----------------------------------------------------------------
var pageSize = 15;

//-----------------------------------------------------------数据源-------------------------------------------------------------------
var storeGoodsType = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'goodsTypeId' },
       { name: 'goodsTypeName' },
       { name: 'goodsNum' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getGoodsType(nPage);
    }
});

var goodsStore = Ext.create('Ext.data.Store', {
    fields: [
        { name: 'goodstype' },
        { name: 'goodsTypeName' },
        { name: 'goodsid' },
        { name: 'goodsname' },
        { name: 'priceNum' }
    ]
});

var goodsPriceStore = Ext.create('Ext.data.Store', {
    fields: [
        { name: 'id' },
        { name: 'transporttype' },
        { name: 'fromroutecode' },
        { name: 'fromroutename' },
        { name: 'toroutecode' },
        { name: 'toroutename' },
        { name: 'price' },
        { name: 'pickprice' },
        { name: 'deliverprice' },
        { name: 'frompart' },
        { name: 'topart' }
    ]
});

//运输类型
var statisticsTypeStore = Ext.create('Ext.data.Store', {
    fields: [
        { name: 'val' },
        { name: 'name' }
    ],
    data: [
        {
            'val': '',
            'name': '全部',
        },
        {
            'val': '1',
            'name': '零担',
        },
        {
            'val': '2',
            'name': '整车',
        }
    ]
});

//起始地
var fromAreaStore = Ext.create('Ext.data.Store', {
    fields: [
       { name: 'code', type: 'string' },
       { name: 'name', type: 'string' }
    ]
});

//目的地
var toAreaStore = Ext.create('Ext.data.Store', {
    fields: [
       { name: 'code', type: 'string' },
       { name: 'name', type: 'string' }
    ]
});
//-----------------------------------------------------------页面方法-----------------------------------------------------------------
//获取货品类别集
function getGoodsType(nPage) {
    CS('CZCLZ.Goods.GetGoodsTypeList', function (retVal) {
        storeGoodsType.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_goodstypename").getValue());
}

//查询货物列表
function getGoods(id) {
    var goodsName = Ext.getCmp("cx_goodsname").getValue();
    CS('CZCLZ.Goods.GetGoodsLine', function (retVal) {
        goodsStore.loadData(retVal);
    }, CS.onError, id, goodsName);
}

//货物列表显示
function ShowGoodsLine(v) {
    var win = new goodsWin({ goodstype: v });
    win.show(null, function () {
        getGoods(v);
    });
}

//获取价格模型列表
function getGoodsPrice(id, fromroute, toroute, statisticstype) {
    CS('CZCLZ.Goods.GetGoodsPriceLine', function (retVal) {
        goodsPriceStore.loadData(retVal);
    }, CS.onError, id, fromroute, toroute, statisticstype);
}

//价格模型列表显示
function ShowGoodsPriceLine(v, n, goodstypeid) {
    var win = new priceWin({ goodsid: v, goodsname: n, goodstypeid: goodstypeid });
    win.show(null, function () {
        CS('CZCLZ.Goods.GetAreaList', function (retVal) {
            if (retVal) {
                fromAreaStore.add([{ 'code': '', 'name': '全部' }]);
                fromAreaStore.loadData(retVal, true);
                Ext.getCmp("cx_fromroute").setValue('');

                toAreaStore.add([{ 'code': '', 'name': '全部' }]);
                toAreaStore.loadData(retVal, true);
                Ext.getCmp("cx_toroute").setValue('');

                getGoodsPrice(v, '', '', '');
            }
        }, CS.onError, '320000');
    });
}

//删除价格模型
function DelGoodsPriceLine(id) {
    Ext.MessageBox.confirm('删除提示', '是否要删除数据!', function (obj) {
        if (obj == "yes") {
            CS('CZCLZ.Goods.DelGoodsPrice', function (retVal) {
                if (retVal) {
                    getGoodsPrice(goodsid, '', '', '');
                }
            }, CS.onError, [id]);
        }
        return;
    });
}

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
        var id = me.id;
        var na = me.name;
        var typeid = me.typeid;
        me.items = [
            {
                xtype: 'UploaderPanel',
                id: 'uploadpricemodel',
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
                            var form = Ext.getCmp('uploadpricemodel');
                            if (form.form.isValid()) {
                                //取得表单中的内容
                                var values = form.form.getValues(false);
                                var me = this;
                                Ext.getCmp('uploadpricemodel').upload('CZCLZ.Goods.UploadPriceModel', function (retVal) {
                                    if (retVal) {
                                        if (retVal.dt) {
                                            goodsPriceStore.loadData(retVal.dt);
                                            me.up('window').close();
                                        }
                                        if (retVal.str != "") {
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
                                }, id, na, typeid);
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
//-------------------------------------------------------价格模型列表界面--------------------------------------------------------------
Ext.define('priceWin', {
    extend: 'Ext.window.Window',

    height: document.documentElement.clientHeight,
    width: document.documentElement.clientWidth,
    layout: {
        type: 'fit'
    },
    title: '价格模型列表',
    modal: true,

    initComponent: function () {
        var me = this;
        var goodsid = me.goodsid;
        var goodsname = me.goodsname;
        var goodstypeid = me.goodstypeid;
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
                            store: goodsPriceStore,
                            id: 'goodspricegrid',
                            selModel: Ext.create('Ext.selection.CheckboxModel', {

                            }),
                            columns: [
                                {
                                    xtype: 'gridcolumn',
                                    dataIndex: 'transporttype',
                                    sortable: false,
                                    menuDisabled: true,
                                    text: "运输类型",
                                    flex: 1
                                },
                                {
                                    xtype: 'gridcolumn',
                                    dataIndex: 'fromroutename',
                                    sortable: false,
                                    menuDisabled: true,
                                    text: "起始地"
                                },
                                {
                                    xtype: 'gridcolumn',
                                    dataIndex: 'toroutename',
                                    sortable: false,
                                    menuDisabled: true,
                                    text: "目的地"
                                },
                                {
                                    xtype: 'gridcolumn',
                                    dataIndex: 'price',
                                    sortable: false,
                                    menuDisabled: true,
                                    text: "单价"
                                },
                                {
                                    xtype: 'gridcolumn',
                                    dataIndex: 'pickprice',
                                    sortable: false,
                                    menuDisabled: true,
                                    text: "提货价"
                                },
                                {
                                    xtype: 'gridcolumn',
                                    dataIndex: 'deliverprice',
                                    sortable: false,
                                    menuDisabled: true,
                                    text: "送货价"
                                },
                                {
                                    xtype: 'gridcolumn',
                                    dataIndex: 'frompart',
                                    sortable: false,
                                    menuDisabled: true,
                                    text: "范围起始值"
                                },
                                {
                                    xtype: 'gridcolumn',
                                    dataIndex: 'topart',
                                    sortable: false,
                                    menuDisabled: true,
                                    text: "范围结束值"
                                },
                                {
                                    text: '操作',
                                    dataIndex: 'id',
                                    width: 80,
                                    sortable: false,
                                    menuDisabled: true,
                                    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                        var str;
                                        str = "<a onclick='DelGoodsPriceLine(\"" + value + "\");'>删除</a>";
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
                                            xtype: 'combobox',
                                            id: 'cx_fromroute',
                                            width: 230,
                                            fieldLabel: '起始地',
                                            editable: false,
                                            labelWidth: 80,
                                            store: fromAreaStore,
                                            queryMode: 'local',
                                            displayField: 'name',
                                            valueField: 'code',
                                            value: ''
                                        },
                                        {
                                            xtype: 'combobox',
                                            id: 'cx_toroute',
                                            width: 230,
                                            fieldLabel: '目的地',
                                            editable: false,
                                            labelWidth: 80,
                                            store: toAreaStore,
                                            queryMode: 'local',
                                            displayField: 'name',
                                            valueField: 'code',
                                            value: ''
                                        },
                                        {
                                            xtype: 'combobox',
                                            id: 'cx_statisticstype',
                                            width: 230,
                                            fieldLabel: '运输类型',
                                            editable: false,
                                            labelWidth: 80,
                                            store: statisticsTypeStore,
                                            queryMode: 'local',
                                            displayField: 'name',
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
                                                        var cx_fromroute = Ext.getCmp("cx_fromroute").getValue();
                                                        var cx_toroute = Ext.getCmp("cx_toroute").getValue();
                                                        var cx_statisticstype = Ext.getCmp("cx_statisticstype").getValue();
                                                        getGoodsPrice(goodsid, cx_fromroute, cx_toroute, cx_statisticstype);
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
                                                    iconCls: 'delete',
                                                    text: '删除',
                                                    handler: function () {
                                                        var idlist = [];
                                                        var grid = Ext.getCmp("goodspricegrid");
                                                        var rds = grid.getSelectionModel().getSelection();
                                                        if (rds.length == 0) {
                                                            Ext.Msg.show({
                                                                title: '提示',
                                                                msg: '请选择至少一条要删除的记录!',
                                                                buttons: Ext.MessageBox.OK,
                                                                icon: Ext.MessageBox.INFO
                                                            });
                                                            return;
                                                        }

                                                        Ext.MessageBox.confirm('删除提示', '是否要删除数据!', function (obj) {
                                                            if (obj == "yes") {
                                                                for (var n = 0, len = rds.length; n < len; n++) {
                                                                    var rd = rds[n];

                                                                    idlist.push(rd.get("id"));
                                                                }

                                                                CS('CZCLZ.Goods.DelGoodsPrice', function (retVal) {
                                                                    if (retVal) {
                                                                        getGoodsPrice(goodsid, '', '', '');
                                                                    }
                                                                }, CS.onError, idlist);
                                                            }
                                                            else {
                                                                return;
                                                            }
                                                        });
                                                    }
                                                }
                                            ]
                                        },
                                        {
                                            xtype: 'button',
                                            iconCls: 'view',
                                            text: '模板导出',
                                            handler: function () {
                                                DownloadFile("CZCLZ.Goods.ExportPriceModelTemplate", "价格模板.xls", goodsid);
                                            }
                                        },
                                        {
                                            xtype: 'button',
                                            iconCls: 'view',
                                            text: '导入新增',
                                            handler: function () {
                                                var win = new drWin({ id: goodsid, name: goodsname, typeid: goodstypeid });
                                                win.show();
                                            }
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
                                getGoods(goodstypeid)
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
//---------------------------------------------------------货物列表界面----------------------------------------------------------------
Ext.define('goodsWin', {
    extend: 'Ext.window.Window',

    height: document.documentElement.clientHeight,
    width: document.documentElement.clientWidth,
    layout: {
        type: 'fit'
    },
    title: '货物列表',
    modal: true,

    initComponent: function () {
        var me = this;
        var goodstype = me.goodstype;
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
                            store: goodsStore,
                            columns: [Ext.create('Ext.grid.RowNumberer'),
                                {
                                    xtype: 'gridcolumn',
                                    dataIndex: 'goodsTypeName',
                                    sortable: false,
                                    menuDisabled: true,
                                    text: "货物分类",
                                    flex: 1
                                },
                                {
                                    xtype: 'gridcolumn',
                                    dataIndex: 'goodsname',
                                    sortable: false,
                                    menuDisabled: true,
                                    text: "货物名称"
                                },
                                {
                                    xtype: 'gridcolumn',
                                    dataIndex: 'priceNum',
                                    sortable: false,
                                    menuDisabled: true,
                                    text: "价格种类数量"
                                },
                                {
                                    text: '操作',
                                    dataIndex: 'goodsid',
                                    width: 80,
                                    sortable: false,
                                    menuDisabled: true,
                                    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                        var str;
                                        str = "<a onclick='ShowGoodsPriceLine(\"" + value + "\",\"" + record.data.goodsname + "\",\"" + record.data.goodstype + "\");'>明细</a>";
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
                                            id: 'cx_goodsname',
                                            width: 280,
                                            labelWidth: 90,
                                            fieldLabel: '货物名称'
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
                                                        getGoods(goodstype);
                                                    }
                                                }
                                            ]
                                        },
                                        {
                                            xtype: 'button',
                                            iconCls: 'view',
                                            text: '导出',
                                            handler: function () {
                                                DownloadFile("CZCLZ.Goods.GetGoodsLineToFile", "货品列表.xls", goodstype, Ext.getCmp("cx_goodsname").getValue());
                                            }
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

//-----------------------------------------------------------界    面-----------------------------------------------------------------
Ext.define('goodsTypeView', {
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
                store: storeGoodsType,
                columnLines: true,
                columns: [Ext.create('Ext.grid.RowNumberer'),
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'goodsTypeName',
                        sortable: false,
                        menuDisabled: true,
                        text: "货物分类名称",
                        flex: 1
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'goodsNum',
                        sortable: false,
                        menuDisabled: true,
                        text: "货物数量",
                        flex: 1
                    },
                    {
                        text: '操作',
                        dataIndex: 'goodsTypeId',
                        width: 80,
                        sortable: false,
                        menuDisabled: true,
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            var str;
                            str = "<a onclick='ShowGoodsLine(\"" + value + "\");'>货品明细</a>";
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
                                id: 'cx_goodstypename',
                                width: 280,
                                labelWidth: 90,
                                fieldLabel: '货物分类名称'
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
                                            getGoodsType(1);
                                        }
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        xtype: 'pagingtoolbar',
                        displayInfo: true,
                        store: storeGoodsType,
                        dock: 'bottom'
                    }
                ]
            }
        ];
        me.callParent(arguments);
    }
});

Ext.onReady(function () {
    new goodsTypeView();

    getGoodsType(1);

})



