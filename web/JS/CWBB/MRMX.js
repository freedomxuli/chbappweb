var pageSize = 15;
var cx_sj;
var id = "";
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
                                 text: "销售券"
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