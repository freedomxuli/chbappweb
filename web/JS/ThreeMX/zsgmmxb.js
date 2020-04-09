inline_include("approot/r/js/jquery-1.6.4.js");
//-----------------------------------------------------------全局变量-----------------------------------------------------------------
var pageSize = 15;
var cx_yhm;
//-----------------------------------------------------------数据源-------------------------------------------------------------------
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
            { name: 'UserName' },
            { name: 'UserXM' },
            { name: 'addtime' },
            { name: 'ordercode' },
                       { name: 'mydonatepayid' },

            { name: 'points' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        DataBind(nPage);
    }
});

//-----------------------------------------------------------页面方法-----------------------------------------------------------------
function DataBind(nPage) {
    CS('CZCLZ.ThreeClass.GetThreeZSgmmxLIST', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("a").getValue(), Ext.getCmp("b").getValue(), Ext.getCmp("c").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
}
  

function getListEXCELOUT(nPage) {


    DownloadFile("CZCLZ.ThreeClass.GetThreeZSgmmxOutLIST", "三方消费明细表.xls", Ext.getCmp("a").getValue(), Ext.getCmp("b").getValue(), Ext.getCmp("c").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());


}


//-----------------------------------------------------------界    面-----------------------------------------------------------------
Ext.define('CYDView', {
    extend: 'Ext.container.Viewport',

    layout: {
        type: 'fit'
    },

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'gridpanel',
                id: 'cyGrid',
                title: '赠送券额消费明细表',
                store: store,
                columnLines: true,
                selModel: Ext.create('Ext.selection.CheckboxModel', {
                    checkOnly: true
                }),
                columns: [
                    Ext.create('Ext.grid.RowNumberer'), 
                     
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'UserName',
                        sortable: false,
                        menuDisabled: true,
                        text: "三方账号",
                        width: 150
                        
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'UserXM',
                        sortable: false,
                        menuDisabled: true,
                        text: "专线名称",
                        width: 150
                    },
            
                    {
                        xtype: 'datecolumn',
                        dataIndex: 'addtime',
                        sortable: false,
                        menuDisabled: true,
                        text: "交易时间",
                        format: 'Y-m-d H:i:s',
                        width: 200
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'ordercode',
                        sortable: false,
                        menuDisabled: true,
                        text: "订单编号",
                        width: 150
                      
                    } ,
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'points',
                        sortable: false,
                        menuDisabled: true,
                        text: "消费金额",
                        width: 150


                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'mydonatepayid',
                        sortable: false,
                        menuDisabled: true,
                        text: "操作",
                        width: 150,
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            return "<a href='javascript:void(0)' onclick='Jzshow(\""+ value +"\")'>记账</a>";
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
                                id: 'a',
                                width: 160,
                                labelWidth: 60,
                                fieldLabel: '三方账号'
                            },
                             {
                                 xtype: 'textfield',
                                 id: 'b',
                                 width: 160,
                                 labelWidth: 60,
                                 fieldLabel: '专线名称'
                             },
                            
                            {
                                id: 'cx_beg',
                                xtype: 'datefield',
                                fieldLabel: '购买时间',
                                format: 'Y-m-d',
                                labelWidth: 60,
                                width: 200
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
                                xtype: 'textfield',
                                id: 'c',
                                width: 160,
                                labelWidth: 60,
                                fieldLabel: '订单号'
                            }, {
                                xtype: 'button',
                                iconCls: 'search',
                                text: '查询',
                                handler: function () {
                                         DataBind(1);
                                      
                                }
                            }, {
                                xtype: 'button',
                                iconCls: 'search',
                                text: '导出EXCEL',
                                handler: function () {
                                         getListEXCELOUT(1);
 
                                }
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

Ext.onReady(function () {
    new CYDView();
  
    DataBind();
})


function Jzshow(nid)
{
   // gzsuserid = userId;
    var win = new GZSList();
    win.show(null, function () {
        getGZSList(nid);
    })
}
 

var gzsstore = Ext.create('Ext.data.Store', {
    fields: [
        { name: 'tradecode' },
        { name: 'payaccounttime' },
        { name: 'memo' } 
    ]
});

function getGZSList(nid) {
    CS('CZCLZ.ThreeClass.GetPSHBZJ', function (retVal) {
        gzsstore.loadData(retVal);
 
    }, CS.onError, nid,2);
}

Ext.define('GZSList', {
    extend: 'Ext.window.Window',

    height: 422,
    width: 620,
    layout: {
        type: 'fit'
    },
    title: '关注数列表详情',
    modal: true,

    initComponent: function () {
        var me = this;
        var userId = me.userId
        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'tabpanel',
                    activeTab: 1,
                    items: [
                        {
                            xtype: 'panel',
                            layout: {
                                type: 'fit'
                            },
                            hidden: true,
                            items: [
                                {
                                    xtype: 'gridpanel',
                                    columnLines: 1,
                                    border: 1,
                                    store: gzsstore,
                                    columns: [
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'tradecode',
                                            sortable: false,
                                            menuDisabled: true,
                                            flex: 1,
                                            text: '运单号'
                                        },
                                        {
                                            xtype: 'datecolumn',
                                            dataIndex: 'payaccounttime',
                                            format: 'Y-m-d',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '记账日期'
                                        }, {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'memo',
                                            sortable: false,
                                            menuDisabled: true,
                                            flex: 1,
                                            text: '备注'
                                        }
                                    ]
                                }
                            ]
                        }

                    ],
                    buttonAlign: 'center',
                    buttons: [
                        {
                            text: '关闭',
                            handler: function () {
                                me.close();
                            }
                        }
                    ]
                }
            ]
        });

        me.callParent(arguments);
    }

});