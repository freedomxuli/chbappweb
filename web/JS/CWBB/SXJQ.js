var pageSize = 15;
var cx_xm;
var cx_beg;
var cx_end;
var id = "";
//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'ZXMC' },
       { name: 'YFQ' },
       { name: 'SJ' },
       { name: 'ZK' },
       { name: 'YXQ' },
       { name: 'FLAG' },
       { name: 'CZR' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        GetSXJList(nPage);
    }
});

var sfstore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'UserName' },
        { name: 'getstatus' },
       { name: 'gettime' },
       { name: 'getpoints' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getSFList(nPage, id);
    }
});


//************************************数据源*****************************************

//************************************页面方法***************************************
function GetSXJList(nPage) {
    CS('CZCLZ.CWBBMag.GetSXJList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_sxj").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
}


function SF(userId) {
    id = userId;
    var win = new SFList({ userId: userId });
    win.show(null, function () {
        getSFList(1);
    })
}

function getSFList(nPage) {
    CS('CZCLZ.CWBBMag.GetPSSF', function (retVal) {
        sfstore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, id);
}

//************************************页面方法***************************************

//************************************弹出界面***************************************
Ext.define('SFList', {
    extend: 'Ext.window.Window',

    height: 422,
    width: 620,
    layout: {
        type: 'fit'
    },
    title: '派送三方列表',
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
                                    store: sfstore,
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
                                            dataIndex: 'getpoints',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '运费券'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'getstatus',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '状态',
                                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                                var str = "";
                                                if (value == 0) {
                                                    str = "待领取";
                                                } else if (value == 1) {
                                                    str = "已领取";
                                                } else if (value == 2) {
                                                    str = "拒绝领取";
                                                } else if (value == 3) {
                                                    str = "超期未领取";
                                                }
                                                return str;
                                            }
                                        },
                                        {
                                            xtype: 'datecolumn',
                                            dataIndex: 'gettime',
                                            format: 'Y-m-d h:m:s',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '获取时间'
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
                                                        DownloadFile("CZCLZ.CWBBMag.GetPSSFToFile", "派送三方明细表.xls", me.userId);
                                                    }
                                                },
                                            ]
                                        }
                                    ]
                                },
                                {
                                    xtype: 'pagingtoolbar',
                                    displayInfo: true,
                                    store: sfstore,
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
                            dataIndex: 'FLAG',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: "状态",
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                var str="";
                                if (value == "1") {
                                    str = "上架";
                                } else if (value == "2") {
                                    str = "下架";
                                }
                                return str;
                            }
                        },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'ZXMC',
                                sortable: false,
                                menuDisabled: true,
                                flex: 1,
                                text: "专线名称"
                            },
                            {
                                xtype: 'datecolumn',
                                dataIndex: 'SJ',
                                sortable: false,
                                menuDisabled: true,
                                format: 'Y-m-d H:i:s',
                                flex: 1,
                                text: "上架时间/下架时间"
                            },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'YFQ',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: "上架/下架券额"
                             },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'ZK',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: "折扣"
                             },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'CZR',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: "操作人"
                             },
                             {
                                 xtype: 'datecolumn',
                                 dataIndex: 'YXQ',
                                 sortable: false,
                                 menuDisabled: true,
                                 format: 'Y-m-d H:i:s',
                                 flex: 1,
                                 text: "有效期"
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
                                            id: 'cx_sxj',
                                            width: 160,
                                            fieldLabel: '状态',
                                            editable: false,
                                            labelWidth: 40,
                                            store: Ext.create('Ext.data.Store', {
                                                fields: [
                                                   { name: 'val' },
                                                   { name: 'txt' }
                                                ],
                                                data: [
                                                     { 'val': "", 'txt': '全部' },
                                                    { 'val': 1, 'txt': '上架' },
                                                    { 'val': 2, 'txt': '下架' }]

                                            }),
                                            queryMode: 'local',
                                            displayField: 'txt',
                                            valueField: 'val',
                                            value: ''
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
                                                        getPSList(1);
                                                    }
                                                },
                                                {
                                                    xtype: 'button',
                                                    iconCls: 'view',
                                                    text: '导出',
                                                    handler: function () {
                                                        DownloadFile("CZCLZ.CWBBMag.GetPSListToFile", "派送记录.xls",  Ext.getCmp("cx_xm").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
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


    cx_xm = Ext.getCmp("cx_sxj").getValue();
    cx_beg = Ext.getCmp("cx_beg").getValue();
    cx_end = Ext.getCmp("cx_end").getValue();
    GetSXJList(1);

})
//************************************主界面*****************************************