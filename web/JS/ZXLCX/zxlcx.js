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
          { name: 'typeName' },
           { name: 'addtime' },
            { name: 'carnumber' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        DataBind(nPage);
    }
});

//-----------------------------------------------------------页面方法-----------------------------------------------------------------
function DataBind(nPage) {
    CS('CZCLZ.ZXLCXClass.GetSelList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_userid").getValue(), Ext.getCmp("cx_typee").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
}
  

function getListEXCELOUT(nPage) {


    DownloadFile("CZCLZ.ZXLCXClass.GetTkshListOutList", "中交兴路查询统计数据下载.xls", {
        'cx_userid': Ext.getCmp("cx_userid").getValue(),
        'cx_typee': Ext.getCmp("cx_typee").getValue(),
        'cx_beg': Ext.getCmp("cx_beg").getValue(),
        'cx_end': Ext.getCmp("cx_end").getValue()
        
    });


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
                title: '中交兴路查询统计',
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
                        text: "账号",
                        width: 150
                        
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'typeName',
                        sortable: false,
                        menuDisabled: true,
                        text: "查询类型",
                        width: 150
                    },
            
                    {
                        xtype: 'datecolumn',
                        dataIndex: 'addtime',
                        sortable: false,
                        menuDisabled: true,
                        text: "查询时间",
                        format: 'Y-m-d H:i:s',
                        width: 200
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'carnumber',
                        sortable: false,
                        menuDisabled: true,
                        text: "查询内容",
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
                                id: 'cx_userid',
                                width: 160,
                                labelWidth: 60,
                                fieldLabel: '用户账号'
                            },
                            {
                                xtype: 'combobox',
                                id: 'cx_typee',
                                width: 180,
                                fieldLabel: '类型',
                                editable: false,
                                labelWidth: 60,
                                store: Ext.create('Ext.data.Store', {
                                    fields: [
                                        { name: 'val' },
                                        { name: 'txt' }
                                    ],
                                    data: [
                                        { 'val': "", 'txt': '全部' },
                                        { 'val': 0, 'txt': 'gps查询类型' },
                                        { 'val': 1, 'txt': '行驶证查询类型' }
                                    ]


                                }),
                                queryMode: 'local',
                                displayField: 'txt',
                                valueField: 'val',
                                value: ''
                            },
                            
                            {
                                id: 'cx_beg',
                                xtype: 'datefield',
                                fieldLabel: '时间',
                                format: 'Y-m-d',
                                labelWidth: 40,
                                width: 170
                            },
                            {
                                id: 'cx_end',
                                xtype: 'datefield',
                                format: 'Y-m-d',
                                fieldLabel: '至',
                                labelWidth: 20,
                                width: 150
                            }, {
                                xtype: 'button',
                                iconCls: 'search',
                                text: '查询',
                                handler: function () {
                                    if (privilege("财务报表_中交兴路查询统计_查看")) {
                                        DataBind(1);
                                    }
                                     
                                }
                            }, {
                                xtype: 'button',
                                iconCls: 'search',
                                text: '导出EXCEL',
                                handler: function () {
                                    if (privilege("财务报表_中交兴路查询统计_查看")) {
                                        getListEXCELOUT(1);
                                    }

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