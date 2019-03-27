var pageSize = 15;
var cx_yhm;
var cx_xm;
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
       { name: 'UserName' },
       { name: 'jysj' },
       { name: 'xfsj' },
       { name: 'UserXM' },
       { name: 'OrderCode' },
       { name: 'Money' },
       { name: 'flag' },
       { name: 'redenvelopeid' },
       { name: 'redmoney' },
       { name: 'KIND' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getUser(nPage);
    }
});

//************************************数据源*****************************************

//************************************页面方法***************************************
function getUser(nPage) {
    CS('CZCLZ.CWBBMag.GetSFJYList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_yhm").getValue(), Ext.getCmp("cx_xm").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
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
                            dataIndex: 'UserName',
                            sortable: false,
                            menuDisabled: true,
                            width: 200,
                            text: '三方账号'
                        },
                         {
                             xtype: 'datecolumn',
                             dataIndex: 'jysj',
                             format: 'Y-m-d H:i:s',
                             sortable: false,
                             menuDisabled: true,
                             width: 150,
                             text: "交易时间"
                         },
                          {
                              xtype: 'datecolumn',
                              dataIndex: 'xfsj',
                              sortable: false,
                              menuDisabled: true,
                              format: 'Y-m-d H:i:s',
                              width: 150,
                              text: "消费时间"
                          },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'UserXM',
                                sortable: false,
                                menuDisabled: true,
                                flex:1,
                                text: "专线名称"
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'OrderCode',
                                width: 180,
                                sortable: false,
                                menuDisabled: true,
                                text: "订单编号"

                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'Money',
                                width: 140,
                                sortable: false,
                                menuDisabled: true,
                                text: "交易金额"

                            },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'redenvelopeid',
                                 width: 140,
                                 sortable: false,
                                 menuDisabled: true,
                                 text: "是否使用红包",
                                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                     var str = "否";
                                     if (value) {
                                         str = "是";
                                     }
                                     return str;
                                 }
                             },
                              {
                                  xtype: 'gridcolumn',
                                  dataIndex: 'redmoney',
                                  width: 140,
                                  sortable: false,
                                  menuDisabled: true,
                                  text: "红包金额"

                              },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'flag',
                                width: 140,
                                sortable: false,
                                menuDisabled: true,
                                text: "交易类型"

                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'KIND',
                                width: 140,
                                sortable: false,
                                menuDisabled: true,
                                text: "券类型"

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
                                                        DownloadFile("CZCLZ.CWBBMag.GetSFJYListToFile", "三方交易表.xls", Ext.getCmp("cx_yhm").getValue(), Ext.getCmp("cx_xm").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
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