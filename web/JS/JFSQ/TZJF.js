//

var pageSize = 15;
var cx_yhm;
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
       { name: 'zed' },
       { name: 'kyed' },
       { name: 'kkfed' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getList(nPage);
    }
});
//************************************数据源*****************************************
//专线名、总额度、可用额度、可开放额度、操作列
//************************************页面方法***************************************
function getList(nPage) {
    CS('CZCLZ.JFSQMag.GetTZList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_yhm").getValue(), Ext.getCmp("cx_xm").getValue());
}


function sq(id) {
    if (privilege("申请运费券_调整申请额度_编辑")) {
        Ext.MessageBox.confirm('提示', '是否要授权!', function (obj) {
            if (obj == "yes") {
                //CS('CZCLZ.JFSQMag.JFSQ', function (retVal) {
                //    if (retVal) {
                //        getList(1);
                //        Ext.MessageBox.alert('提示', '授权成功！');
                //    }
                //}, CS.onError, id,1);
            }
        });
    }
}
//************************************页面方法***************************************

//************************************弹出界面***************************************

//************************************弹出界面***************************************

//************************************主界面*****************************************
Ext.onReady(function () {
    Ext.define('JFSQView', {
        extend: 'Ext.container.Viewport',

        layout: {
            type: 'fit'
        },

        initComponent: function () {
            var me = this;
            me.items = [
                {
                    xtype: 'gridpanel',
                    id: 'JFSQGrid',
                    store: store,
                    
                    columnLines: 1,
                    border: 1,
                    columns: [Ext.create('Ext.grid.RowNumberer'),
                         {
                             xtype: 'gridcolumn',
                             dataIndex: 'UserXM',
                             sortable: false,
                             menuDisabled: true,
                             flex:1,
                             text: '物流名称'
                         },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'UserName',
                            sortable: false,
                            menuDisabled: true,
                            width: 100,
                            text: '用户名'
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'zed',
                            sortable: false,
                            menuDisabled: true,
                            width: 100,
                            text: '总额度'
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'kyed',
                            sortable: false,
                            menuDisabled: true,
                            width: 100,
                            text: '可用额度'
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'kkfed',
                            sortable: false,
                            menuDisabled: true,
                            width: 100,
                            text: '可开放额度'
                        },
                        {
                            xtype: 'gridcolumn',
                            sortable: false,
                            menuDisabled: true,
                            dataIndex: 'UserID',
                            text: '操作',
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                return  "<a href='JavaScript:void(0)' onclick='tz(\"" + value + "\")'>扣除</a>";
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
                                    id: 'cx_xm',
                                    width: 140,
                                    labelWidth: 60,
                                    fieldLabel: '物流名称'
                                },
                                {
                                    xtype: 'textfield',
                                    id: 'cx_yhm',
                                    width: 140,
                                    labelWidth: 60,
                                    fieldLabel: '用户名'
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

    new JFSQView();

    getList();
})
//************************************主界面*****************************************