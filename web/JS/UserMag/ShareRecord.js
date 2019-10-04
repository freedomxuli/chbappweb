//-----------------------------------------------------------全局变量-----------------------------------------------------------------
var pageSize = 15;

//-----------------------------------------------------------数据源-------------------------------------------------------------------
var shareStore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'y' },
       { name: 'm' },
       { name: 'd' },
       { name: 'recordNum' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getShareRecord(nPage);
    }
});

//-----------------------------------------------------------页面方法-----------------------------------------------------------------
function getShareRecord(nPage) {
    CS('CZCLZ.YHGLClass.GetShareRecordByPage', function (retVal) {
        shareStore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
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
                            sortable: false,
                            menuDisabled: true,
                            width: 100,
                            text: '日期',
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                var str = record.data.y + "-" + record.data.m + "-" + record.data.d;
                                return str;
                            }
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'recordNum',
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
                                                if (privilege("系统维护中心_分享记录_查看")) {
                                                    getShareRecord(1);
                                                }
                                            }
                                        },
                                        {
                                            xtype: 'button',
                                            iconCls: 'view',
                                            text: '导出',
                                            handler: function () {
                                                if (privilege("系统维护中心_分享记录_导出")) {
                                                    DownloadFile("CZCLZ.YHGLClass.GetShareRecordToFile", "分享记录.xls", Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
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
