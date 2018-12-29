var pageSize = 15;
var cx_role;
var cx_yhm;
var cx_beg;
var cx_end;
var gmuserid = "";
var ysyuserid = "";
//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'UserID' },
       { name: 'UserName' },
       { name: 'Points' },
       { name: 'gmyfq' },
       { name: 'ysyyfq' },
       { name: 'gmcs' },
       { name: 'gzs' },
       { name: 'syyfq' },
       { name: 'gqwsy' },
       { name: 'Addtime' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getUser(nPage);
    }
});

var gmstore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'UserID' },
       { name: 'UserXM' },
       { name: 'OrderCode' },
       { name: 'Points' },
       { name: 'Money' },
       { name: 'AddTime' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getGMList(nPage, gmuserid);
    }
});

var ysystore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'jydx' },
       { name: 'Points' },
       { name: 'AddTime' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getYSYList(nPage, ysyuserid);
    }
});

var systore = Ext.create('Ext.data.Store', {
    fields: [
        { name: 'UserXM' },
        { name: 'points' },
        { name: 'PointsEndTime' }
    ]
});



//************************************数据源*****************************************

//************************************页面方法***************************************
function getUser(nPage) {
    CS('CZCLZ.CWBBMag.GetSFSJList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_yhm").getValue(),Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
}

function GM(userId) {
    gmuserid = userId;
    var win = new GMList({ userId: userId });
    win.show(null, function () {
        getGMList(1);
    })
}

function getGMList(nPage) {
    CS('CZCLZ.CWBBMag.getGMList', function (retVal) {
        gmstore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, gmuserid);
}

function SY(userId) {
    CS('CZCLZ.CWBBMag.getSYList', function (retVal) {
        var win = new SYList({userId:userId});
        win.show(null, function () {
            systore.loadData(retVal);
        })
    }, CS.onError, userId);

}

function YSY(userId) {
    ysyuserid = userId;
    var win = new YSYList({ userId: userId });
    win.show(null, function () {
        getYSYList(1);
    })
}

function getYSYList(nPage) {
    CS('CZCLZ.CWBBMag.getYSYList', function (retVal) {
        ysystore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, ysyuserid);
}
//************************************页面方法***************************************

//************************************弹出界面***************************************
Ext.define('GMList', {
    extend: 'Ext.window.Window',

    height: 422,
    width: 620,
    layout: {
        type: 'fit'
    },
    title: '购买列表详情',
    modal: true,

    initComponent: function () {
        var me = this;
        var userId = me.userId;
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
                                    store: gmstore,
                                    columns: [
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'OrderCode',
                                            sortable: false,
                                            menuDisabled: true,
                                            flex: 1,
                                            text: '订单号'
                                        },
                                         {
                                             xtype: 'gridcolumn',
                                             dataIndex: 'UserXM',
                                             sortable: false,
                                             menuDisabled: true,
                                             width: 100,
                                             text: '专线'
                                         },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'Points',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '运费券'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'Money',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '金额'
                                        },
                                        {
                                            xtype: 'datecolumn',
                                            dataIndex: 'AddTime',
                                            format: 'Y-m-d',
                                            sortable: false,
                                            menuDisabled: true,
                                            width:100,
                                            text: '时间'
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
                                                        DownloadFile("CZCLZ.CWBBMag.getGMListToFile", "三方用户购买明细表.xls", userId);
                                                    }
                                                },
                                            ]
                                        }
                                    ]
                                },
                                {
                                    xtype: 'pagingtoolbar',
                                    displayInfo: true,
                                    store: gmstore,
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

Ext.define('SYList', {
    extend: 'Ext.window.Window',

    height: 422,
    width: 620,
    layout: {
        type: 'fit'
    },
    title: '剩余运费券详情',
    modal: true,

    initComponent: function () {
        var me = this;
        var userId=me.userId;
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
                                    store: systore,
                                    columns: [
                                         {
                                             xtype: 'gridcolumn',
                                             dataIndex: 'UserXM',
                                             sortable: false,
                                             menuDisabled: true,
                                             flex:1,
                                             text: '专线'
                                         },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'points',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '运费券'
                                        },
                                        {
                                            xtype: 'datecolumn',
                                            dataIndex: 'PointsEndTime',
                                            format: 'Y-m-d',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '截止日期'
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
                                                        DownloadFile("CZCLZ.CWBBMag.getSYListToFile", "三方剩余运费券明细表.xls", userId);
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

Ext.define('YSYList', {
    extend: 'Ext.window.Window',

    height: 422,
    width: 620,
    layout: {
        type: 'fit'
    },
    title: '已使用运费券列表详情',
    modal: true,

    initComponent: function () {
        var me = this;
        var userId = me.userId;
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
                                    store: ysystore,
                                    columns: [
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'jydx',
                                            sortable: false,
                                            menuDisabled: true,
                                            flex: 1,
                                            text: '交易对象'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'Points',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '运费券'
                                        },
                                        {
                                            xtype: 'datecolumn',
                                            dataIndex: 'AddTime',
                                            format: 'Y-m-d',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '时间'
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
                                                        DownloadFile("CZCLZ.CWBBMag.getYSYListToFile", "三方已使用运费券明细表.xls", userId);
                                                    }
                                                },
                                            ]
                                        }
                                    ]
                                },
                                {
                                    xtype: 'pagingtoolbar',
                                    displayInfo: true,
                                    store: ysystore,
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
                                dataIndex: 'UserName',
                                sortable: false,
                                menuDisabled: true,
                                flex:1,
                                text: "三方账号"
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'syyfq',
                                flex: 1,
                                sortable: false,
                                menuDisabled: true,
                                text: "剩余运费券",
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    return "<a onclick='SY(\"" + record.data.UserID + "\");'>" + (value == null ? "" : value) + "</a>"
                                }

                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'gqwsy',
                                flex: 1,
                                sortable: false,
                                menuDisabled: true,
                                text: "过期未使用",
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    return "<a onclick='SY(\"" + record.data.UserID + "\");'>" + (value == null ? "" : value) + "</a>"
                                }

                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'gmyfq',
                                sortable: false,
                                menuDisabled: true,
                                flex: 1,
                                text: "购买运费券",
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    return "<a onclick='GM(\"" + record.data.UserID + "\");'>" + (value == null ? "" : value) + "</a>"
                                }
                            },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'ysyyfq',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: "已使用运费券",
                                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                     return "<a onclick='YSY(\"" + record.data.UserID + "\");'>" + (value == null ? "" : value) + "</a>"
                                 }
                             },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'gmcs',
                                sortable: false,
                                menuDisabled: true,
                                flex: 1,
                                text: "购买次数"
                            },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'gzs',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: "关注数"
                             },
                             {
                                 xtype: 'datecolumn',
                                 dataIndex: 'Addtime',
                                 format: 'Y-m-d',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: '注册时间'
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
                                            fieldLabel: '用户名'
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
                                                {
                                                    xtype: 'button',
                                                    iconCls: 'view',
                                                    text: '导出',
                                                    handler: function () {
                                                        DownloadFile("CZCLZ.CWBBMag.GetSFSJListToFile", "三方市场数据统计表.xls", Ext.getCmp("cx_yhm").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
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
    cx_beg = Ext.getCmp("cx_beg").getValue();
    cx_end = Ext.getCmp("cx_end").getValue();
    getUser(1);

})
//************************************主界面*****************************************