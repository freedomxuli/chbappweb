var pageSize = 15;
var cx_yhm;
//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'PlatToSaleId' },
       { name: 'UserID' },
       { name: 'UserXM' },
       { name: 'points' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getList(nPage);
    }
});

//************************************数据源*****************************************

//************************************页面方法***************************************
function getList(nPage) {
    CS('CZCLZ.XJMag.GetQLList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_yhm").getValue());
}

function ql(id) {
        Ext.MessageBox.confirm('删除提示', '是否要删除数据!', function (obj) {
            if (obj == "yes") {
                CS('CZCLZ.XJMag.QLYFQ', function (retVal) {
                    if (retVal) {
                        getUser(1);
                    }
                }, CS.onError, id);
            }
            else {
                return;
            }
        });
}
//************************************页面方法***************************************


//************************************主界面*****************************************
Ext.onReady(function () {
    Ext.define('KFGMView', {
        extend: 'Ext.container.Viewport',

        layout: {
            type: 'fit'
        },

        initComponent: function () {
            var me = this;
            me.items = [
                {
                    xtype: 'gridpanel',
                    id: 'KFGMGrid',
                    store: store,
                    columnLines: 1,
                    border: 1,
                    columns: [
                        {
                            xtype: 'rownumberer',
                            width:30
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'UserXM',
                            sortable: false,
                            menuDisabled: true,
                            width: 400,
                            text: '物流名称'
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'points',
                            sortable: false,
                            menuDisabled: true,
                            width: 200,
                            text: '线上运费券'
                        },
                        {
                            xtype: 'gridcolumn',
                            sortable: false,
                            menuDisabled: true,
                            dataIndex: 'PlatToSaleId',
                            text: '操作',
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                if (record.data.points > 0)
                                {
                                    str = "<a href='JavaScript:void(0)' onclick='ql(\"" + value + "\")'>清理</a>";
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
                                    id: 'cx_yhm',
                                    labelWidth: 60,
                                    fieldLabel: '物流名称'
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
                            store: store,
                            dock: 'bottom'
                        }
                    ]
                }
            ];
            me.callParent(arguments);
        }
    });

    new KFGMView();

    getList();
})
//************************************主界面*****************************************