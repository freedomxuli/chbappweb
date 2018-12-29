var pageSize = 15;
var cx_role;
var cx_yhm;
var cx_xm;
var cx_beg;
var cx_end;
var gmuserid = "";
var xfsuserid = "";
var wsysuserid = "";
//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'UserID' },
       { name: 'UserName' },
       { name: 'UserXM' },
       { name: 'Points' },
       { name: 'Addtime' },
       { name: 'sxyfq' },
       { name: 'sqcs' },
       { name: 'xfyfq' },
       { name: 'wsyyfq' }
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
       { name: 'UserName' },
       { name: 'OrderCode' },
       { name: 'Points' },
       { name: 'Money' },
       { name: 'AddTime' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getGMList(nPage, gmuserid);
    }
});

var xfstore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'UserID' },
       { name: 'UserName' },
       { name: 'Points' },
       { name: 'AddTime' },
    ],
    onPageChange: function (sto, nPage, sorters) {
        getXFList(nPage, xfuserid);
    }
});

var wsystore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'UserID' },
       { name: 'UserName' },
       { name: 'points' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getWSYList(nPage, xfuserid);
    }
});

//************************************数据源*****************************************

//************************************页面方法***************************************
function getUser(nPage) {
    CS('CZCLZ.CWBBMag.GetWXGZHList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_yhm").getValue(), Ext.getCmp("cx_xm").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
}


function SQE(userId) {
    gmuserid = userId;
    var win = new GMList({ userId: userId });
    win.show(null, function () {
        getGMList(1);
    })
}

function getGMList(nPage) {
    CS('CZCLZ.CWBBMag.getWXSQEList', function (retVal) {
        gmstore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, gmuserid);
}

function XF(userId) {
    xfuserid = userId;
    var win = new XFList({ userId: userId });
    win.show(null, function () {
        getXFList(1);
    })
}

function getXFList(nPage) {
    CS('CZCLZ.CWBBMag.getWXXFList', function (retVal) {
        xfstore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, xfuserid);
}

function WSY(userId) {
    wsyuserid = userId;
    var win = new WSYList({ userId: userId });
    win.show(null, function () {
        getWSYList(1);
    })
}

function getWSYList(nPage) {
    CS('CZCLZ.CWBBMag.getWXWSYList', function (retVal) {
        wsystore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, wsyuserid);
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
    title: '售券额列表详情',
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
                                             dataIndex: 'UserName',
                                             sortable: false,
                                             menuDisabled: true,
                                             width: 100,
                                             text: '三方'
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
                                                        DownloadFile("CZCLZ.CWBBMag.getWXSQEListToFile", "微信公众号售券明细表.xls", me.userId);
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


Ext.define('XFList', {
    extend: 'Ext.window.Window',

    height: 422,
    width: 620,
    layout: {
        type: 'fit'
    },
    title: '消费列表详情',
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
                                    store: xfstore,
                                    columns: [
                                         {
                                             xtype: 'gridcolumn',
                                             dataIndex: 'UserName',
                                             sortable: false,
                                             menuDisabled: true,
                                             flex:1,
                                             text: '三方'
                                         },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'Points',
                                            sortable: false,
                                            menuDisabled: true,
                                            flex:1,
                                            text: '运费券'
                                        },
                                        {
                                            xtype: 'datecolumn',
                                            dataIndex: 'AddTime',
                                            format: 'Y-m-d h:m:s',
                                            sortable: false,
                                            menuDisabled: true,
                                            flex: 1,
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
                                                        DownloadFile("CZCLZ.CWBBMag.getWXXFListToFile", "微信公众号消费明细表.xls", me.userId);
                                                    }
                                                }
                                            ]
                                        }
                                    ]
                                },
                                {
                                    xtype: 'pagingtoolbar',
                                    displayInfo: true,
                                    store: xfstore,
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


Ext.define('WSYList', {
    extend: 'Ext.window.Window',
    height: 422,
    width: 620,
    layout: {
        type: 'fit'
    },
    title: '未使用列表详情',
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
                                    store: wsystore,
                                    columns: [
                                         {
                                             xtype: 'gridcolumn',
                                             dataIndex: 'UserName',
                                             sortable: false,
                                             menuDisabled: true,
                                             flex: 1,
                                             text: '三方'
                                         },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'points',
                                            sortable: false,
                                            menuDisabled: true,
                                            flex: 1,
                                            text: '运费券'
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
                                                        DownloadFile("CZCLZ.CWBBMag.getWXWSYListToFile", "微信公众号未使用表.xls", me.userId);
                                                    }
                                                }
                                            ]
                                        }
                                    ]
                                },
                                {
                                    xtype: 'pagingtoolbar',
                                    displayInfo: true,
                                    store: wsystore,
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
                                dataIndex: 'UserXM',
                                sortable: false,
                                menuDisabled: true,
                                flex:1,
                                text: "专线"
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'UserName',
                                sortable: false,
                                menuDisabled: true,
                                width:120,
                                text: "账号"
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'sxyfq',
                                width: 140,
                                sortable: false,
                                menuDisabled: true,
                                text: "售券额",
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    return "<a onclick='SQE(\"" + record.data.UserID + "\");'>" + (value == null ? "" : value) + "</a>"
                                }

                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'sqcs',
                                width: 140,
                                sortable: false,
                                menuDisabled: true,
                                text: "售券次数",
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    return "<a onclick='SQE(\"" + record.data.UserID + "\");'>" + (value == null ? "" : value) + "</a>"
                                }

                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'xfyfq',
                                width: 140,
                                sortable: false,
                                menuDisabled: true,
                                text: "消费",
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    return "<a onclick='XF(\"" + record.data.UserID + "\");'>" + (value == null ? "" : value) + "</a>"
                                }

                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'wsyyfq',
                                width: 140,
                                sortable: false,
                                menuDisabled: true,
                                text: "未使用",
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    return "<a onclick='WSY(\"" + record.data.UserID + "\");'>" + (value == null ? "" : value) + "</a>"
                                }

                            },
                             {
                                 xtype: 'datecolumn',
                                 dataIndex: 'Addtime',
                                 format: 'Y-m-d',
                                 sortable: false,
                                 menuDisabled: true,
                                 width: 110,
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
                                                        DownloadFile("CZCLZ.CWBBMag.GetWXGZHListToFile", "微信公众号数据统计表.xls", Ext.getCmp("cx_yhm").getValue(), Ext.getCmp("cx_xm").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
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
    cx_beg = Ext.getCmp("cx_beg").getValue();
    cx_end = Ext.getCmp("cx_end").getValue();
    getUser(1);

})
//************************************主界面*****************************************