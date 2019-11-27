var pageSize = 15;
//************************************数据源*****************************************
var zxcdStore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'OrderCode' },
       { name: 'fhrmc' },
       { name: 'UserXM' },
       { name: 'carnumber' },
       { name: 'servicecode' },
       { name: 'networkName' },
       { name: 'memo' },
       { name: 'servicestatus' },
       { name: 'AddTime' },
       { name: 'person' },
       { name: 'serviceid' },
       { name: 'PayID' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getList(nPage);
    }
});
//************************************数据源*****************************************

//************************************页面方法***************************************
function getList(nPage) {
    CS('CZCLZ.CYMag.getZxcdList', function (retVal) {
        zxcdStore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), Ext.getCmp("cx_zxmc").getValue(), Ext.getCmp("cx_fhrmc").getValue());
}


function Edit(id, payid) {
    CS('CZCLZ.CYMag.Qrdd', function (retVal) {
        getList(zxcdStore.currentPage);
    }, CS.onError, id, payid);
}
//************************************页面方法***************************************

//************************************主界面*****************************************
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
                    title: '',
                    store: zxcdStore,
                    columnLines: true,
                    columns: [Ext.create('Ext.grid.RowNumberer'),
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'OrderCode',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: "订单号"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'fhrmc',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: "发货人账号"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'UserXM',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: "专线名"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'carnumber',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: "车牌"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'servicecode',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: "运单号"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'networkName',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: "分流点"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'person',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: "中转/配送信息"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'servicestatus',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: "当前进度",
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                var str = "";//配送类型：0：未装车；10：已装车，在途；20：已到分流点，待分流；30：中转/配送中；90：完成；
                                if (value == 0) {
                                    str = "未装车";
                                } else if (value == 10) {
                                    str = "已装车，在途";
                                } else if (value == 20) {
                                    str = "已到分流点，待分流";
                                } else if (value == 30) {
                                    str = "中转/配送中";
                                } else if (value == 90) {
                                    str = "完成";
                                }
                                return str;
                            }
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'servicestatus',
                            sortable: false,
                            menuDisabled: true,
                            width: 100,
                            text: '操作',
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                if (value == 10) {
                                    return "<a onclick='Edit(\"" + record.data.serviceid + "\",\"" + record.data.PayID + "\");'>确认到达</a>";
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
                                            id: 'cx_beg',
                                            xtype: 'datefield',
                                            fieldLabel: '时间',
                                            format: 'Y-m-d',
                                            labelWidth: 50,
                                            width: 180
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
                                            id: 'cx_zxmc',
                                            xtype: 'textfield',
                                            fieldLabel: '专线名称',
                                            labelWidth: 60,
                                            width: 150
                                        },
                                        {
                                            id: 'cx_fhrmc',
                                            xtype: 'textfield',
                                            fieldLabel: '发货人账号',
                                            labelWidth: 90,
                                            width: 180
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
                                        }
                                    ]
                                },
                                {
                                    xtype: 'pagingtoolbar',
                                    displayInfo: true,
                                    store: zxcdStore,
                                    dock: 'bottom'
                                }
                    ]
                }
            ];
            me.callParent(arguments);
        }
    });

    new iView();

    getList(1);

})
//************************************主界面*****************************************