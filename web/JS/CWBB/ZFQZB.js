﻿var pageSize = 15;
var cx_yhm;
var cx_xm;
var cx_sc;
var cx_beg;
var cx_end;
var userId = "";
//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'UserID' },
       { name: 'FromRoute' },
       { name: 'UserName' },
       { name: 'UserXM' },
       { name: 'sjdzq' },
       { name: 'ysyq' },
       { name: 'gqwsy' },
       { name: 'qxnwsy' },
       { name: 'sqcs' },
       { name: 'sxq' },
       { name: 'sxed' },
       { name: 'zsq' },
       { name: 'rq' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getUser(nPage);
    }
});

var xszbstore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'rq' },
       { name: 'FromRoute' },
       { name: 'UserName' },
       { name: 'UserXM' },
       { name: 'xsdzq' },
       { name: 'xfje' },
       { name: 'gqje' },
       { name: 'wsyje' },
       { name: 'zje' },
       { name: 'pjzk' },
       { name: 'yj' },
       { name: 'hbje' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getXSZBList(nPage);
    }
});

var mxstore = Ext.create('Ext.data.Store', {
    fields: [
        { name: 'AddTime' },
        { name: 'Points' },
        { name: 'xfje' },
        { name: 'gqje' },
        { name: 'wsyje' },
        { name: 'SaleRecordDiscount' },
        { name: 'UserName' },
        { name: 'xfrq' },
        { name: 'hbje' }
    ]
});

//************************************数据源*****************************************

//************************************页面方法***************************************
function getUser(nPage) {
    CS('CZCLZ.CWBBMag.GetZXZFList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_yhm").getValue(), Ext.getCmp("cx_xm").getValue(), Ext.getCmp("cx_sc").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
}

function CKXSZB(id) {
    if (privilege("财务报表_自发券总表_查看")) {
        userId = id;
        var win = new XSZBList({ userId: id });
        win.show(null, function () {
            getXSZBList(1);
        })
    }
}

function getXSZBList(nPage) {
    CS('CZCLZ.CWBBMag.getXSZBList', function (retVal) {
        xszbstore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, userId, Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
}

function CKXSMX(userId,rq) {
    CS('CZCLZ.CWBBMag.getXSMXList', function (retVal) {
        var win = new MXList({ userId: userId ,rq:rq});
        win.show(null, function () {
            mxstore.loadData(retVal);
        })
    }, CS.onError, userId,rq);

}
//************************************页面方法***************************************

//************************************弹出界面***************************************
Ext.define('XSZBList', {
    extend: 'Ext.window.Window',

    height: document.documentElement.clientHeight,
    width: document.documentElement.clientWidth,
    layout: {
        type: 'fit'
    },
    title: '自发券销售总表',
    modal: true,

    initComponent: function () {
        var me = this;
        var userId = me.userId
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
                                    store: xszbstore,
                                    columns: [
                                        {
                                            xtype: 'datecolumn',
                                            dataIndex: 'rq',
                                            format: 'Y年m月d日',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 150,
                                            text: '日期'
                                        },
                                         {
                                             xtype: 'gridcolumn',
                                             dataIndex: 'FromRoute',
                                             sortable: false,
                                             menuDisabled: true,
                                             width: 100,
                                             text: '市场'
                                         },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'UserName',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '注册账号'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'UserXM',
                                            sortable: false,
                                            menuDisabled: true,
                                            flex:1,
                                            text: '专线名称'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'xsdzq',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '销售电子券'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'xfje',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 150,
                                            text: '消费（已使用）金额'
                                        },
                                         {
                                             xtype: 'gridcolumn',
                                             dataIndex: 'hbje',
                                             sortable: false,
                                             menuDisabled: true,
                                             width: 150,
                                             text: '红包金额'
                                         },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'gqje',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '过期金额'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'wsyje',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '未使用金额'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'zje',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '总金额'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'pjzk',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '平均折扣'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'yj',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '佣金'
                                        },
                                         {
                                             text: '操作',
                                             width: 250,
                                             sortable: false,
                                             menuDisabled: true,
                                             renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                                 var str;
                                                 str = "<a onclick='CKXSMX(\"" + userId +"\",\""+record.data.rq+ "\");'>查看</a>";
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
                                            xtype: 'buttongroup',
                                            title: '',
                                            items: [
                                                {
                                                    xtype: 'button',
                                                    iconCls: 'view',
                                                    text: '导出',
                                                    handler: function () {
                                                        DownloadFile("CZCLZ.CWBBMag.getXSZBListToFile", "自放券销售总表.xls", userId, Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
                                                    }
                                                },
                                            ]
                                        }
                                    ]
                                },
                                {
                                    xtype: 'pagingtoolbar',
                                    displayInfo: true,
                                    store: xszbstore,
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

Ext.define('MXList', {
    extend: 'Ext.window.Window',

    height: document.documentElement.clientHeight/2,
    width: document.documentElement.clientWidth/2,
    layout: {
        type: 'fit'
    },
    title: '自发券明细',
    modal: true,

    initComponent: function () {
        var me = this;
        var userId = me.userId;
        var rq = me.rq;
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
                                    store: mxstore,
                                    columns: [
                                    {
                                        xtype: 'datecolumn',
                                        dataIndex: 'AddTime',
                                        format: 'Y-m-d H:i:s',
                                        sortable: false,
                                        menuDisabled: true,
                                        width: 100,
                                        text: '销售日期'
                                    },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'Points',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '销售电子券'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'xfje',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '消费（已使用）金额'
                                        },
                                         {
                                             xtype: 'gridcolumn',
                                             dataIndex: 'hbje',
                                             sortable: false,
                                             menuDisabled: true,
                                             width: 100,
                                             text: '红包金额'
                                         },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'gqje',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '过期金额'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'wsyje',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '未使用金额'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'SaleRecordDiscount',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '折扣'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'UserName',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '购买三方账号'
                                        },
                                        {
                                            xtype: 'datecolumn',
                                            dataIndex: 'xfrq',
                                            format: 'Y-m-d H:i:s',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '消费日期'
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
                                                    iconCls: 'view',
                                                    text: '导出',
                                                    handler: function () {
                                                        DownloadFile("CZCLZ.CWBBMag.getXSMXListToFile", "自发券销售明细表.xls", userId, rq);
                                                    }
                                                },
                                            ]
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
                    columns: [Ext.create('Ext.grid.RowNumberer'),
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'rq',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: '日期'
                        },
                         {
                             xtype: 'gridcolumn',
                             dataIndex: 'FromRoute',
                             sortable: false,
                             menuDisabled: true,
                             width: 60,
                             text: "市场"
                         },
                          {
                              xtype: 'gridcolumn',
                              dataIndex: 'UserName',
                              sortable: false,
                              menuDisabled: true,
                              width: 120,
                              text: "注册账号"
                          },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'UserXM',
                                sortable: false,
                                menuDisabled: true,
                                flex:1,
                                text: "专线"
                            },
                           
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'sjdzq',
                                width: 100,
                                sortable: false,
                                menuDisabled: true,
                                text: "上架电子券"

                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'ysyq',
                                width: 100,
                                sortable: false,
                                menuDisabled: true,
                                text: "已使用券"

                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'gqwsy',
                                width: 100,
                                sortable: false,
                                menuDisabled: true,
                                text: "过期未使用"

                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'qxnwsy',
                                width: 110,
                                sortable: false,
                                menuDisabled: true,
                                text: "在期限内未使用券"

                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'sxq',
                                width: 100,
                                sortable: false,
                                menuDisabled: true,
                                text: "失效券（下架）"

                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'zsq',
                                width: 100,
                                sortable: false,
                                menuDisabled: true,
                                text: "在售券"

                            },
                            {
                                text: '操作',
                                dataIndex: 'UserID',
                                width: 50,
                                sortable: false,
                                menuDisabled: true,
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    var str;
                                    str = "<a onclick='CKXSZB(\"" + value + "\");'>查看</a>";
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
                                            width: 140,
                                            labelWidth: 50,
                                            fieldLabel: '账号'
                                        },
                                        {
                                            xtype: 'textfield',
                                            id: 'cx_xm',
                                            width: 160,
                                            labelWidth: 70,
                                            fieldLabel: '专线'
                                        },
                                         {
                                             xtype: 'textfield',
                                             id: 'cx_sc',

                                             width: 160,
                                             labelWidth: 70,
                                             fieldLabel: '市场'
                                         },
                                        {
                                            id: 'cx_beg',
                                            xtype: 'datefield',
                                            fieldLabel: '时间',
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
                                                        if (privilege("财务报表_自发券总表_查看")) {
                                                            getUser(1);
                                                        }
                                                    }
                                                },
                                                {
                                                    xtype: 'button',
                                                    iconCls: 'view',
                                                    text: '导出',
                                                    handler: function () {
                                                        if (privilege("财务报表_自发券总表_导出")) {
                                                            DownloadFile("CZCLZ.CWBBMag.GetZXZFListToFile", "自放券总表.xls", Ext.getCmp("cx_yhm").getValue(), Ext.getCmp("cx_xm").getValue(), Ext.getCmp("cx_sc").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
                                                        }
                                                    }
                                                },
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


    cx_yhm = Ext.getCmp("cx_yhm").getValue();
    cx_xm = Ext.getCmp("cx_xm").getValue();
    cx_sc = Ext.getCmp("cx_sc").getValue();
    cx_beg = Ext.getCmp("cx_beg").getValue();
    cx_end = Ext.getCmp("cx_end").getValue();
    getUser(1);

})
//************************************主界面*****************************************