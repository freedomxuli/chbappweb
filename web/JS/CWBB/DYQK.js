var pageSize = 15;
var cx_zxmc;
var id = "";
var sj = "";
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
       { name: 'zsx' },
       { name: 'gmlj' },
        { name: 'xflj' }
       
    ],
    onPageChange: function (sto, nPage, sorters) {
        getDYZXList(nPage);
    }
});


//************************************数据源*****************************************

//************************************页面方法***************************************
function getDYZXList(nPage) {
    CS('CZCLZ.CWBBMag.getDYZXList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_zxmc").getValue());
}

//************************************页面方法***************************************

//************************************弹出界面***************************************

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
                                 flex: 1,
                                 text: "专线账号"
                             },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'UserXM',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: "专线名称"
                             },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'zsx',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: "总授信"
                             },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'zfchb',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: "当月支付查货宝"
                             },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'gmlj',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: "当月累计购买"
                             },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'xflj',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: "当月累计消费"
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
                                            id: 'cx_zxmc',
                                            xtype: 'textfield',
                                            fieldLabel: '专线名称',
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
                                                        getDYZXList(1);
                                                    }
                                                },
                                                {
                                                    xtype: 'button',
                                                    iconCls: 'view',
                                                    text: '导出',
                                                    handler: function () {
                                                        DownloadFile("CZCLZ.CWBBMag.getDYZXListToFile", "当月情况.xls", Ext.getCmp("cx_zxmc").getValue());
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


    cx_zxmc = Ext.getCmp("cx_zxmc").getValue();
    getDYZXList(1);

})
//************************************主界面*****************************************