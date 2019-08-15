//-----------------------------------------------------------全局变量-----------------------------------------------------------------
var pageSize = 15;

//-----------------------------------------------------------数据源-------------------------------------------------------------------
var gqqStore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
        { name: 'fhrzh' },
        { name: 'points' },
        { name: 'addTimd' },
        { name: 'zxmc' },
        { name: 'invalidId' },
        { name: 'points' },
        { name: 'CardUserID' },
        { name: 'shzt' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getList(nPage);
    }
});
//-----------------------------------------------------------页面方法-----------------------------------------------------------------
function getList(nPage) {
    CS('CZCLZ.XJMag.GetGqqListByPage', function (retVal) {
        gqqStore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, {
            'cx_zxmc': Ext.getCmp("cx_zxmc").getValue(),
            'cx_fhrzh': Ext.getCmp("cx_fhrzh").getValue(),
            'cx_beg': Ext.getCmp("cx_beg").getValue(),
            'cx_end': Ext.getCmp("cx_end").getValue()
        });
}

function sh(id, zxid, ps) {
    if (privilege("运费券下架_过期券回退_审核")) {
        Ext.MessageBox.confirm('提示', '是否审核？', function (obj) {
            if (obj == "yes") {
                CS('CZCLZ.XJMag.GqqSh', function (retVal) {
                    if (retVal) {
                        Ext.Msg.show({
                            title: '提示',
                            msg: '审核成功!',
                            buttons: Ext.MessageBox.OK,
                            icon: Ext.MessageBox.INFO,
                            fn: function () {
                                getList(1);
                            }
                        });
                    } else {
                        Ext.Msg.show({
                            title: '提示',
                            msg: '该券已经回退审核过!',
                            buttons: Ext.MessageBox.OK,
                            icon: Ext.MessageBox.INFO,
                        });
                    }
                }, CS.onError, id, zxid, ps);
            }
            else {
                return;
            }
        });
    }
}
//-----------------------------------------------------------界    面-----------------------------------------------------------------
Ext.define('iView', {
    extend: 'Ext.container.Viewport',

    layout: {
        type: 'fit'
    },

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'gridpanel',
                store: gqqStore,
                columnLines: 1,
                border: 1,
                columns: [
                    {
                        xtype: 'rownumberer',
                        width: 30
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'fhrzh',
                        sortable: false,
                        menuDisabled: true,
                        flex: 1,
                        text: '发货人账号'
                    },
                    {
                        xtype: 'numbercolumn',
                        dataIndex: 'points',
                        sortable: false,
                        menuDisabled: true,
                        width: 150,
                        text: '过期券额'
                    },
                    {
                        xtype: 'datecolumn',
                        dataIndex: 'addTimd',
                        sortable: false,
                        menuDisabled: true,
                        width: 90,
                        text: '过期时间',
                        format: 'Y-m-d',
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'zxmc',
                        sortable: false,
                        menuDisabled: true,
                        flex: 1,
                        text: '专线名称'
                    },
                    {
                        xtype: 'gridcolumn',
                        sortable: false,
                        menuDisabled: true,
                        dataIndex: 'invalidId',
                        text: '操作',
                        width: 60,
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            if (record.data.shzt == 0) {
                                str = "<a href='JavaScript:void(0)' onclick='sh(\"" + value + "\",\"" + record.data.CardUserID + "\",\"" + record.data.points + "\")'>审核</a>";
                                return str;
                            }
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
                                xtype: 'textfield',
                                id: 'cx_zxmc',
                                labelWidth: 60,
                                width: 160,
                                fieldLabel: '专线名称'
                            },
                            {
                                xtype: 'textfield',
                                id: 'cx_fhrzh',
                                labelWidth: 80,
                                width: 180,
                                fieldLabel: '发货人账号'
                            },
                            {
                                xtype: 'datefield',
                                id: 'cx_beg',
                                fieldLabel: '起始日期',
                                width: 180,
                                format: 'Y-m-d',
                                labelWidth: 60
                            },
                            {
                                xtype: 'datefield',
                                id: 'cx_end',
                                fieldLabel: '截止日期',
                                width: 180,
                                format: 'Y-m-d',
                                labelWidth: 60
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
                                            if (privilege("运费券下架_过期券回退_查看")) {
                                                getList(1);
                                            }
                                        }
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        xtype: 'pagingtoolbar',
                        displayInfo: true,
                        store: gqqStore,
                        dock: 'bottom'
                    }
                ]
            }
        ];
        me.callParent(arguments);
    }
});

Ext.onReady(function () {
    new iView();
    getList(1);
})