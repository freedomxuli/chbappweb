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
       { name: 'id' },
       { name: 'UserXM' },
       { name: 'points' },
       { name: 'addtime' },
       { name: 'paisongcount' },
        { name: 'ylqrs' },
       { name: 'dlqrs' },
       { name: 'jlqrs' },
       { name: 'jzsj' },
       { name: 'clqrs' }
       
    ],
    onPageChange: function (sto, nPage, sorters) {
        getPSList(nPage);
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
       { name: 'getpoints' },
       { name: 'sfsj' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getSFList(nPage, id);
    }
});


//************************************数据源*****************************************

//************************************页面方法***************************************
function getPSList(nPage) {
    CS('CZCLZ.CWBBMag.GetPSListZ', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_xm").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
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
    width: 720,
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
                                            format: 'Y-m-d H:i:s',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 150,
                                            text: '获取时间'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'sfsj',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '是否使用',
                                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                                var str = "";
                                                if (value!=null&&value!="") {
                                                    str = "是";
                                                } else {
                                                    str = "否";
                                                }
                                                return str;
                                            }
                                        },
                                        {
                                            xtype: 'datecolumn',
                                            dataIndex: 'sfsj',
                                            format: 'Y-m-d H:i:s',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 150,
                                            text: '使用时间'
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
                             dataIndex: 'id',
                             sortable: false,
                             menuDisabled: true,
                             hidden: true,
                             flex: 1
                         },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'UserXM',
                                sortable: false,
                                menuDisabled: true,
                                flex: 1,
                                text: "专线"
                            },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'points',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: "运费券"
                             },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'paisongcount',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: "派送人数",
                                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                     return "<a onclick='SF(\"" + record.data.id + "\");'>" + (value == null ? "" : value) + "</a>"
                                 }
                             },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'ylqrs',
                                flex: 1,
                                sortable: false,
                                menuDisabled: true,
                                text: "已领取人数"

                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'dlqrs',
                                flex: 1,
                                sortable: false,
                                menuDisabled: true,
                                text: "待领取人数"

                            },
                                {
                                    xtype: 'gridcolumn',
                                    dataIndex: 'jlqrs',
                                    flex: 1,
                                    sortable: false,
                                    menuDisabled: true,
                                    text: "拒绝领取人数"

                                },
                                {
                                    xtype: 'gridcolumn',
                                    dataIndex: 'clqrs',
                                    flex: 1,
                                    sortable: false,
                                    menuDisabled: true,
                                    text: "超期未领取人数"

                                },

                             {
                                 xtype: 'datecolumn',
                                 dataIndex: 'addtime',
                                 format: 'Y-m-d H:i:s',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: '派送时间'
                             },
                             {
                                 xtype: 'datecolumn',
                                 format: 'Y-m-d H:i:s',
                                 dataIndex: 'jzsj',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: '截止时间'
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
                                            id: 'cx_xm',
                                            width: 160,
                                            labelWidth: 70,
                                            fieldLabel: '专线名称'
                                        },
                                        {
                                            id: 'cx_beg',
                                            xtype: 'datefield',
                                            fieldLabel: '派送时间',
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
                                                        if (privilege("财务报表_派送记录_查看")) {
                                                            getPSList(1);
                                                        }
                                                    }
                                                },
                                                {
                                                    xtype: 'button',
                                                    iconCls: 'view',
                                                    text: '导出',
                                                    handler: function () {
                                                        if (privilege("财务报表_派送记录_导出")) {
                                                            DownloadFile("CZCLZ.CWBBMag.GetPSListZToFile", "派送记录.xls", Ext.getCmp("cx_xm").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
                                                        }
                                                    }
                                                },
                                                {
                                                    xtype: 'button',
                                                    iconCls: 'view',
                                                    text: '批量导出明细',
                                                    handler: function () {
                                                        if (privilege("财务报表_派送记录_批量导出")) {
                                                            DownloadFile("CZCLZ.CWBBMag.GetPSSFToFilePL", "批量派送明细记录.xls", Ext.getCmp("cx_xm").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
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


    cx_xm = Ext.getCmp("cx_xm").getValue();
    cx_beg = Ext.getCmp("cx_beg").getValue();
    cx_end = Ext.getCmp("cx_end").getValue();
    getPSList(1);

})
//************************************主界面*****************************************