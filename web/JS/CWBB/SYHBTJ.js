//-----------------------------------------------------------全局变量-----------------------------------------------------------------
var pageSize = 15;

//-----------------------------------------------------------数据源-------------------------------------------------------------------
var shareStore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'ResidueNum' },
       { name: 'UserName' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getShareRecord(nPage);
    }
});

//-----------------------------------------------------------页面方法-----------------------------------------------------------------
function getShareRecord(nPage) {
    CS('CZCLZ.CWBBMag.GetRedenvelopeByPage', function (retVal) {
        shareStore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_residuenum").getValue());
}

//-----------------------------------------------------------界    面-----------------------------------------------------------------
Ext.onReady(function () {
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
                    store: shareStore,
                    columnLines: true,
                    columns: [Ext.create('Ext.grid.RowNumberer'),
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'UserName',
                            sortable: false,
                            menuDisabled: true,
                            width: 130,
                            text: "账号"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'ResidueNum',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: "数量"
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
                                    id: 'cx_residuenum',
                                    xtype: 'numberfield',
                                    fieldLabel: '剩余红包总数',
                                    labelWidth: 100,
                                    width: 190
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
                                                if (privilege("财务报表_用户红包剩余统计_查看")) {
                                                    getShareRecord(1);
                                                }
                                            }
                                        },
                                        {
                                            xtype: 'button',
                                            iconCls: 'view',
                                            text: '导出',
                                            handler: function () {
                                                if (privilege("财务报表_用户红包剩余统计_导出")) {
                                                    DownloadFile("CZCLZ.CWBBMag.GetRedenvelopeToFile", "红包剩余数量统计表.xls", Ext.getCmp("cx_residuenum").getValue());
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
                            store: shareStore,
                            dock: 'bottom'
                        }
                    ]
                }
            ];
            me.callParent(arguments);
        }
    });

    new iView();
    getShareRecord(1);
})
