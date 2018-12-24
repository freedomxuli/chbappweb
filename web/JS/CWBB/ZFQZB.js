var pageSize = 15;
var cx_yhm;
var cx_xm;
var cx_sc;
var cx_beg;
var cx_end;
var gmuserid = "";
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
       { name: 'rq' }
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

//************************************页面方法***************************************

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
                            width: 200,
                            text: '日期'
                        },
                         {
                             xtype: 'gridcolumn',
                             dataIndex: 'FromRoute',
                             sortable: false,
                             menuDisabled: true,
                             width: 150,
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
                                width: 140,
                                sortable: false,
                                menuDisabled: true,
                                text: "上架电子券"

                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'ysyq',
                                width: 140,
                                sortable: false,
                                menuDisabled: true,
                                text: "已使用券"

                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'gqwsy',
                                width: 140,
                                sortable: false,
                                menuDisabled: true,
                                text: "过期未使用"

                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'qxnwsy',
                                width: 140,
                                sortable: false,
                                menuDisabled: true,
                                text: "在期限内未使用券"

                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'sxq',
                                width: 140,
                                sortable: false,
                                menuDisabled: true,
                                text: "失效券（下架）"

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
                                                        getUser(1);
                                                    }
                                                },
                                                {
                                                    xtype: 'button',
                                                    iconCls: 'view',
                                                    text: '导出',
                                                    handler: function () {
                                                        DownloadFile("CZCLZ.CWBBMag.GetZXSJListToFile", "专线用户统计表.xls", Ext.getCmp("cx_yhm").getValue(), Ext.getCmp("cx_xm").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
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