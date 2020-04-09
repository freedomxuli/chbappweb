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
            { name: 'AddTime' },
            { name: 'OrderCode' },
            { name: 'Money' },
            { name: 'redenvelopemoney' },
            { name: 'ishb' },
            { name: 'points' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        DataBind(nPage);
    }
});

//-----------------------------------------------------------页面方法-----------------------------------------------------------------
function DataBind(nPage) {
    CS('CZCLZ.ThreeClass.GetThreeczgmmxLIST', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("a").getValue(), Ext.getCmp("b").getValue(), Ext.getCmp("c").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
}
  

function getListEXCELOUT(nPage) {


    DownloadFile("CZCLZ.ThreeClass.GetThreeczgmmxOutLIST", "三方充值购买明细表.xls", Ext.getCmp("a").getValue(), Ext.getCmp("b").getValue(), Ext.getCmp("c").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());


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
                title: '三方充值购买明细表',
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
                        dataIndex: 'AddTime',
                        sortable: false,
                        menuDisabled: true,
                        text: "交易时间",
                        format: 'Y-m-d H:i:s',
                        width: 200
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'OrderCode',
                        sortable: false,
                        menuDisabled: true,
                        text: "订单编号",
                        width: 150
                      
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'ishb',
                        sortable: false,
                        menuDisabled: true,
                        text: "充值金额",
                        width: 150

                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'Money',
                        sortable: false,
                        menuDisabled: true,
                        text: "实际支付金额",
                        width: 150

                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'redenvelopemoney',
                        sortable: false,
                        menuDisabled: true,
                        text: "是否使用红包",
                        width: 150,
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            if (value != null && value != "" && value != "0") {
                                return "是";
                            } else {
                                return "否";
                            }
                        }

                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'redenvelopemoney',
                        sortable: false,
                        menuDisabled: true,
                        text: "红包金额",
                        width: 150
                        

                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'points',
                        sortable: false,
                        menuDisabled: true,
                        text: "赠送券额",
                        width: 150


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
    if (!privilegeBo("承运模块_三方承运单查询_开票")) {
        Ext.getCmp('kaipiao').disable(true);
    }

    DataBind();
})