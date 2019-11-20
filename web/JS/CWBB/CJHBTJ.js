﻿//-----------------------------------------------------------全局变量-----------------------------------------------------------------
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
function getTj() {
    CS('CZCLZ.CWBBMag.GetCjhbTj', function (retVal) {
        if (retVal) {
            Ext.getCmp("ordernum").setValue(retVal[0]["ordernum"]);
            Ext.getCmp("gradenum").setValue(retVal[0]["gradenum"]);
            Ext.getCmp("usernum").setValue(retVal[0]["usernum"]);
            Ext.getCmp("rafflenum").setValue(retVal[0]["rafflenum"]);
            Ext.getCmp("rafflemoney").setValue(retVal[0]["rafflemoney"]);
            Ext.getCmp("five").setValue(retVal[0]["five"]);
            Ext.getCmp("twenty").setValue(retVal[0]["twenty"]);
            Ext.getCmp("fifty").setValue(retVal[0]["fifty"]);
            Ext.getCmp("twohundred").setValue(retVal[0]["twohundred"]);
            Ext.getCmp("fivehundred").setValue(retVal[0]["fivehundred"]);
        }
    }, CS.onError, Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), Ext.getCmp("cx_city").getValue());
}
function getList(nPage) {
    CS('CZCLZ.CWBBMag.GetCjhbTjLine', function (retVal) {
        tjStore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
        getTj();
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
                            xtype: 'displayfield',
                            width: 160,
                            labelWidth: 50,
                            id: 'ordernum',
                            fieldLabel: '订单数'
                        },
                        {
                            xtype: 'displayfield',
                            width: 160,
                            labelWidth: 50,
                            id: 'gradenum',
                            fieldLabel: '评价数'
                        },
                        {
                            xtype: 'displayfield',
                            width: 160,
                            labelWidth: 50,
                            id: 'usernum',
                            fieldLabel: '货主数'
                        },
                        {
                            xtype: 'displayfield',
                            width: 160,
                            labelWidth: 50,
                            id: 'rafflenum',
                            fieldLabel: '红包数'
                        },
                        {
                            xtype: 'displayfield',
                            width: 160,
                            labelWidth: 70,
                            id: 'rafflemoney',
                            fieldLabel: '红包总金额'
                        },
                        {
                            xtype: 'displayfield',
                            width: 160,
                            labelWidth: 110,
                            id: 'five',
                            fieldLabel: '其中5元红包的个数'
                        },
                        {
                            xtype: 'displayfield',
                            width: 160,
                            labelWidth: 100,
                            id: 'twenty',
                            fieldLabel: '20元红包个数'
                        },
                        {
                            xtype: 'displayfield',
                            width: 160,
                            labelWidth: 100,
                            id: 'fifty',
                            fieldLabel: '50元红包个数'
                        },
                        {
                            xtype: 'displayfield',
                            width: 160,
                            labelWidth: 100,
                            id: 'twohundred',
                            fieldLabel: '200元红包个数'
                        },
                        {
                            xtype: 'displayfield',
                            width: 160,
                            labelWidth: 100,
                            id: 'fivehundred',
                            fieldLabel: '500元红包个数'
                        }
                    ]
                },
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
                                        if (privilege("财务报表_抽奖红包统计_导出")) {
                                            DownloadFile("CZCLZ.CWBBMag.GetCjhbTjLineToFile", "抽奖红包统计.xls", Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), Ext.getCmp("cx_city").getValue());
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