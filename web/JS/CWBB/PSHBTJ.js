//-----------------------------------------------------------全局变量-----------------------------------------------------------------
var pageSize = 15;
var cx_city;
var cx_beg;
var cx_end;

//-----------------------------------------------------------数据源-------------------------------------------------------------------
var tjStore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
        { name: 'UserName' },
        { name: 'addtime' },
        { name: 'money' },
        { name: 'isuse' },
        { name: 'updatetime' },
        { name: 'OrderCode' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getList(nPage);
    }
});
//-----------------------------------------------------------页面方法-----------------------------------------------------------------
function getList(nPage) {
    CS('CZCLZ.CWBBMag.GetPshbTjLine', function (retVal) {
        tjStore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), Ext.getCmp("cx_city").getValue());
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
                    xtype: 'datecolumn',
                    dataIndex: 'addtime',
                    format: 'Y-m-d H:i:s',
                    sortable: false,
                    menuDisabled: true,
                    width: 150,
                    text: '获取时间'
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'money',
                    sortable: false,
                    menuDisabled: true,
                    flex: 1,
                    text: "红包金额"
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'isuse',
                    sortable: false,
                    menuDisabled: true,
                    flex: 1,
                    text: "红包状态"
                },
                {
                    xtype: 'datecolumn',
                    dataIndex: 'updatetime',
                    format: 'Y-m-d H:i:s',
                    sortable: false,
                    menuDisabled: true,
                    width: 150,
                    text: '更新时间'
                },
                {
                    xtype: 'gridcolumn',
                    dataIndex: 'OrderCode',
                    sortable: false,
                    menuDisabled: true,
                    flex: 1,
                    text: "对应使用订单号"
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
                            id: 'cx_beg',
                            xtype: 'datefield',
                            fieldLabel: '时间',
                            format: 'Y-m-d',
                            labelWidth: 40,
                            width: 150

                        },
                        {
                            id: 'cx_end',
                            xtype: 'datefield',
                            format: 'Y-m-d',
                            fieldLabel: '至',
                            labelWidth: 20,
                            width: 140
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
                                        if (privilege("财务报表_派送红包统计_导出")) {
                                            DownloadFile("CZCLZ.CWBBMag.GetPshbTjToFile", "派送红包统计.xls", Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), Ext.getCmp("cx_city").getValue());
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

    cx_beg = Ext.getCmp("cx_beg").getValue();
    cx_end = Ext.getCmp("cx_end").getValue();
    cx_city = Ext.getCmp("cx_city").getValue();
    getList(1);
})