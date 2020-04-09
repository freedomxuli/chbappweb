//-----------------------------------------------------------全局变量-----------------------------------------------------------------
var pageSize = 15;
//-----------------------------------------------------------数据源-------------------------------------------------------------------
var goodTypeStore = Ext.create('Ext.data.Store', {
    fields: [
       { name: 'id', type: 'string' },
       { name: 'name', type: 'string' }
    ]
});

var goodTypeEditStore = Ext.create('Ext.data.Store', {
    fields: [
       { name: 'id', type: 'string' },
       { name: 'name', type: 'string' }
    ]
});

var storeGoods = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'goodsid' },
       { name: 'goodsname' },
       { name: 'goodstypeid' },
       { name: 'goodstypename' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getGoods(nPage);
    }
});

//-----------------------------------------------------------页面方法-----------------------------------------------------------------
//获取货品集
function getGoods(nPage) {
    CS('CZCLZ.Goods.GetGoodsList', function (retVal) {
        storeGoods.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });

    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_goodstype").getValue(), Ext.getCmp("cx_goodsname").getValue());
}

//修改
function Edit(v) {
    var r = storeGoods.findRecord("goodsid", v).data;
    CS('CZCLZ.Goods.GetGoodsType', function (ret) {
        if (ret) {
            goodTypeEditStore.loadData(ret, false);
            var win = new addWin();
            win.show(null, function () {
                var form = Ext.getCmp('addform');
                form.form.setValues(r);
            });

        }
    }, CS.onError);
}

//删除
function Del(v) {
    Ext.MessageBox.confirm('删除提示', '是否要删除数据!', function (obj) {
        if (obj == "yes") {
            CS('CZCLZ.Goods.DelGoods', function (retVal) {
                if (retVal) {
                   
                    getGoods(storeGoods.currentPage);
                }
            }, CS.onError, v);
        }
        return;
    });
}
//-----------------------------------------------------------弹出界面-----------------------------------------------------------------
Ext.define('addWin', {
    extend: 'Ext.window.Window',
    id: 'winId',
    height: 300,
    width: 400,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '货品管理',
    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'form',
                id: 'addform',
                bodyPadding: 10,
                items: [
                    {
                        xtype: 'combobox',
                        fieldLabel: '货品类别',
                        editable: false,
                        labelWidth: 70,
                        store: goodTypeEditStore,
                        queryMode: 'local',
                        displayField: 'name',
                        valueField: 'id',
                        name: 'goodstypeid',
                        anchor: '100%',
                        id: 'goodstypeid'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: 'goodsid',
                        name: 'goodsid',
                        labelWidth: 70,
                        hidden: true,
                        colspan: 2
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '货品名称',
                        name: 'goodsname',
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
                            var form = Ext.getCmp('addform');
                            if (form.form.isValid()) {
                                var values = form.form.getValues(false);
                                var me = this;
                                CS('CZCLZ.Goods.SaveGoods', function (retVal) {
                                    getGoods(1);
                                    Ext.getCmp("winId").close();
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
//-----------------------------------------------------------界    面-----------------------------------------------------------------
Ext.define('goodsView', {
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
                store: storeGoods,
                columnLines: true,
                columns: [Ext.create('Ext.grid.RowNumberer'),
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'goodstypename',
                        sortable: false,
                        menuDisabled: true,
                        text: "货品类别"
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'goodsname',
                        sortable: false,
                        menuDisabled: true,
                        text: "货名"
                    },
                    {
                        text: '操作',
                        dataIndex: 'goodsid',
                        width: 150,
                        sortable: false,
                        menuDisabled: true,
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            var str;
                            str = "<a onclick='Edit(\"" + value + "\");'>修改</a> <a onclick='Del(\"" + value + "\");'>删除</a>";
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
                                id: 'cx_goodstype',
                                width: 230,
                                fieldLabel: '货品类别',
                                editable: false,
                                labelWidth: 80,
                                store: goodTypeStore,
                                queryMode: 'local',
                                displayField: 'name',
                                valueField: 'id',
                                value: ''
                            },
                            {
                                xtype: 'textfield',
                                id: 'cx_goodsname',
                                width: 140,
                                labelWidth: 50,
                                fieldLabel: '货名'
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
                                            getGoods(1);
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
                                            CS('CZCLZ.Goods.GetGoodsType', function (retVal) {
                                                if (retVal) {
                                                    goodTypeEditStore.loadData(retVal, false);
                                                    var win = new addWin();
                                                    win.show(null, function () {
                                                    });
                                                }
                                            }, CS.onError);
                                        }
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        xtype: 'pagingtoolbar',
                        displayInfo: true,
                        store: storeGoods,
                        dock: 'bottom'
                    }
                ]
            }
        ];
        me.callParent(arguments);
    }
});

Ext.onReady(function () {
    new goodsView();
    CS('CZCLZ.Goods.GetGoodsType', function (retVal) {
        if (retVal) {
            goodTypeStore.add([{ 'id': '', 'name': '全部' }]);
            goodTypeStore.loadData(retVal, true);
            Ext.getCmp("cx_goodstype").setValue('');
        }
    }, CS.onError);

    getGoods(1);

})