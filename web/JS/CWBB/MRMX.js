var pageSize = 15;
var cx_sj;
var id = "";
var sj = "";
//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'sj' },
       { name: 'xsq' },
        { name: 'xfrs' },
       { name: 'yqje' },
       { name: 'zxfq' },
        { name: 'gqwsy' }
       
    ],
    onPageChange: function (sto, nPage, sorters) {
        getMRMXList(nPage);
    }
});

var xsstore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'UserXM' },
       { name: 'points' },
    ],
    onPageChange: function (sto, nPage, sorters) {
        getXSList(nPage, sj);
    }
});

//************************************数据源*****************************************

//************************************页面方法***************************************
function getMRMXList(nPage) {
    CS('CZCLZ.CWBBMag.getMRMXList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_sj").getValue());
}

function GetXSList(nPage) {
    CS('CZCLZ.CWBBMag.GetMRXSList', function (retVal) {
        xsstore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize,sj, Ext.getCmp("cx_lx").getValue());
}


function XS(xssj) {
    sj = xssj;
    var win = new XSList({ xssj: xssj });
    win.show(null, function () {
        GetXSList(1);
    })
}
//************************************页面方法***************************************

//************************************弹出界面***************************************
Ext.define('XSList', {
    extend: 'Ext.window.Window',

    height: 422,
    width: 620,
    layout: {
        type: 'fit'
    },
    title: '销售明细',
    modal: true,

    initComponent: function () {
        var me = this;
        var xssj = me.xssj
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
                                    store: xsstore,
                                    columns: [
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'UserXM',
                                            sortable: false,
                                            menuDisabled: true,
                                            flex: 1,
                                            text: '专线名称'
                                        },
                                         {
                                             xtype: 'gridcolumn',
                                             dataIndex: 'points',
                                             sortable: false,
                                             menuDisabled: true,
                                             width: 100,
                                             text: '销售总额'
                                         }
                                    ],
                                    dockedItems: [
                                {
                                    xtype: 'toolbar',
                                    dock: 'top',
                                    items: [
                                        {
                                            xtype: 'combobox',
                                            id: 'cx_lx',
                                            width: 160,
                                            fieldLabel: '券类型',
                                            editable: false,
                                            labelWidth: 60,
                                            store: Ext.create('Ext.data.Store', {
                                                fields: [
                                                   { name: 'val' },
                                                   { name: 'txt' }
                                                ],
                                                data: [{ 'val': '', 'txt': '全部' },
                                                        { 'val': 1, 'txt': '耗材券' },
                                                        { 'val': 2, 'txt': '自发券' }]
                                            }),
                                            queryMode: 'local',
                                            displayField: 'txt',
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
                                                         GetXSList(1);
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
                                                    iconCls: 'view',
                                                    text: '导出',
                                                    handler: function () {
                                                        DownloadFile("CZCLZ.CWBBMag.GetMRXSListToFile", "专线销售表.xls", xssj, Ext.getCmp("cx_lx").getValue());
                                                    }
                                                }
                                            ]
                                        }
                                    ]
                                },
                                {
                                    xtype: 'pagingtoolbar',
                                    displayInfo: true,
                                    store: xsstore,
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
                                dataIndex: 'sj',
                                sortable: false,
                                menuDisabled: true,
                                flex: 1,
                                text: "时间"
                            },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'xfrs',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: "用券人数"
                             },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'yqje',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: "用券金额"
                             },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'gqwsy',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: "失效金额"
                             },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'zxfq',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: "总消费券"
                             },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'xsq',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: "销售券",
                                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                     return "<a onclick='XS(\"" + record.data.sj + "\");'>" + (value == null ? "" : value) + "</a>"
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
                                            id: 'cx_sj',
                                            xtype: 'datefield',
                                            fieldLabel: '时间',
                                            format: 'Y-m-d',
                                            labelWidth: 80,
                                            width: 210
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
                                                        getMRMXList(1);
                                                    }
                                                },
                                                {
                                                    xtype: 'button',
                                                    iconCls: 'view',
                                                    text: '导出',
                                                    handler: function () {
                                                        DownloadFile("CZCLZ.CWBBMag.getMRMXListToFile", "每日明细.xls",  Ext.getCmp("cx_sj").getValue());
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


    cx_sj = Ext.getCmp("cx_sj").getValue();
    getMRMXList(1);

})
//************************************主界面*****************************************