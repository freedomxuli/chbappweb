var pageSize = 15;
var cx_role;
var cx_yhm;
var cx_xm;
var cx_beg;
var cx_end;
var gmuserid = "";
var gzsuserid = "";
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
       { name: 'sykkf' },
       { name: 'zsyfq' },
       { name: 'sxyfq' },
       { name: 'sqcs' },
       { name: 'gzs' },
       { name: 'sxed' }
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

var gzsstore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'UserID' },
       { name: 'gzr' },
       { name: 'GZ_TIME' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getGZSList(nPage, gzsuserid);
    }
});





//************************************数据源*****************************************

//************************************页面方法***************************************
function getUser(nPage) {
    CS('CZCLZ.CWBBMag.GetZXSJList', function (retVal) {
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
    CS('CZCLZ.CWBBMag.getSQEList', function (retVal) {
        gmstore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, gmuserid);
}

function GZS(userId) {
    gzsuserid = userId;
    var win = new GZSList({ userId: userId });
    win.show(null, function () {
        getGZSList(1);
    })
}

function getGZSList(nPage) {
    CS('CZCLZ.CWBBMag.getGZSList', function (retVal) {
        gzsstore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, gzsuserid);
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
                                                        DownloadFile("CZCLZ.CWBBMag.getSQEListToFile", "专线售券明细表.xls", me.userId);
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

Ext.define('GZSList', {
    extend: 'Ext.window.Window',

    height: 422,
    width: 620,
    layout: {
        type: 'fit'
    },
    title: '关注数列表详情',
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
                                    store: gzsstore,
                                    columns: [
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'gzr',
                                            sortable: false,
                                            menuDisabled: true,
                                            flex: 1,
                                            text: '关注人'
                                        },
                                        {
                                            xtype: 'datecolumn',
                                            dataIndex: 'GZ_TIME',
                                            format: 'Y-m-d',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '关注时间'
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
                                                        DownloadFile("CZCLZ.CWBBMag.getGZSListToFile", "专线关注数明细表.xls", userId);
                                                    }
                                                },
                                            ]
                                        }
                                    ]
                                },
                                {
                                    xtype: 'pagingtoolbar',
                                    displayInfo: true,
                                    store: gzsstore,
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
                                dataIndex: 'sykkf',
                                width: 140,
                                sortable: false,
                                menuDisabled: true,
                                text: "剩余可开放运费券"

                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'zsyfq',
                                width: 140,
                                sortable: false,
                                menuDisabled: true,
                                text: "在售运费券"

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
                                 dataIndex: 'gzs',
                                 sortable: false,
                                 menuDisabled: true,
                                 width: 140,
                                 text: "关注数",
                                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                     return "<a onclick='GZS(\"" + record.data.UserID + "\");'>" + (value == null ? "" : value) + "</a>"
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
                             },
                              {
                                  xtype: 'gridcolumn',
                                  dataIndex: 'Points',
                                  sortable: false,
                                  menuDisabled: true,
                                  width: 140,
                                  text: "账户余额"
                              },
                              {
                                  xtype: 'gridcolumn',
                                  dataIndex: 'sxed',
                                  sortable: false,
                                  menuDisabled: true,
                                  width: 140,
                                  text: "授信额度"
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
                                                        DownloadFile("CZCLZ.CWBBMag.GetZXSJListToFile", "专线市场数据统计表.xls", Ext.getCmp("cx_yhm").getValue(), Ext.getCmp("cx_xm").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
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