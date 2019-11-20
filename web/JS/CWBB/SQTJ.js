//-----------------------------------------------------------全局变量-----------------------------------------------------------------
var pageSize = 15;
var cx_city;
var cx_money;

//-----------------------------------------------------------数据源-------------------------------------------------------------------
var tjStore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
        { name: 'UserName' },
        { name: 'savepoints' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getList(nPage);
    }
});
//-----------------------------------------------------------页面方法-----------------------------------------------------------------
function getList(nPage) {
    CS('CZCLZ.CWBBMag.GetSqTjLine', function (retVal) {
        tjStore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_money").getValue(), Ext.getCmp("cx_city").getValue());
}

//-----------------------------------------------------------界    面-----------------------------------------------------------------
Ext.define('TjView', {
    extend: 'Ext.container.Viewport',
    layout: {
        type: 'fit'
    },
    initComponent: function () {
        var me = this;
        me.items = [
        {
            xtype: 'gridpanel',
            store: tjStore,
            columnLines: true,
            columns: [Ext.create('Ext.grid.RowNumberer'),
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'UserName',
                    sortable: false,
                    menuDisabled: true,
                    flex: 1,
                    text: "账号"
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'savepoints',
                    sortable: false,
                    menuDisabled: true,
                    flex: 1,
                    text: "省钱金额"
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
                            id: 'cx_money',
                            xtype: 'numberfield',
                            fieldLabel: '金额大于',
                            labelWidth: 100,
                            width: 190
                        },
                        {
                            xtype: 'combobox',
                            id: 'cx_city',
                            width: 160,
                            fieldLabel: '城市',
                            editable: false,
                            labelWidth: 40,
                            store: Ext.create('Ext.data.Store', {
                                fields: [
                                    { name: 'dq_bm' },
                                    { name: 'dq_mc' }
                                ],
                                data: [
                                    { 'dq_bm': '', 'dq_mc': '全部' },
                                    { 'dq_bm': '320400', 'dq_mc': '常州' },
                                    { 'dq_bm': '320500', 'dq_mc': '苏州' },
                                    { 'dq_bm': '320200', 'dq_mc': '无锡' }
                                ]
                            }),
                            queryMode: 'local',
                            displayField: 'dq_mc',
                            valueField: 'dq_bm',
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
                                        getList(1);
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
                                        if (privilege("财务报表_省钱统计_导出")) {
                                            DownloadFile("CZCLZ.CWBBMag.GetSqTjLineToFile", "省钱统计.xls", Ext.getCmp("cx_money").getValue(), Ext.getCmp("cx_city").getValue());
                                        }
                                    }
                                }
                            ]
                        },
                    ]
                },
                {
                    xtype: 'pagingtoolbar',
                    displayInfo: true,
                    store: tjStore,
                    dock: 'bottom'
                }
            ]
        }
        ];
        me.callParent(arguments);
    }
});

Ext.onReady(function () {
    new TjView();

    cx_money = Ext.getCmp("cx_money").getValue();
    cx_city = Ext.getCmp("cx_city").getValue();
    getList(1);
})