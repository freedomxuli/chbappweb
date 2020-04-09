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
         { name: 'dq_mc' },
          { name: 'userxm' },
           { name: 'addtime' },
            { name: 'quota' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        DataBind(nPage);
    }
});

//-----------------------------------------------------------页面方法-----------------------------------------------------------------
function DataBind(nPage) {
    CS('CZCLZ.SJPBClass.GetPBHBZEDList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("areacode").getValue());
}
  

function getListEXCELOUT(nPage) {


    DownloadFile("CZCLZ.SJPBClass.GetTkshListOutList", "后台配比红包总额度明细下载.xls", Ext.getCmp("areacode").getValue());


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
                title: '后台配比红包总额度明细',
                store: store,
                columnLines: true,
                selModel: Ext.create('Ext.selection.CheckboxModel', {
                    checkOnly: true
                }),
                columns: [
                    Ext.create('Ext.grid.RowNumberer'), 
                      {
                          xtype: 'gridcolumn',
                          dataIndex: 'dq_mc',
                          sortable: false,
                          menuDisabled: true,
                          text: "地区",
                          width: 150
                      },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'userxm',
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
                        text: "查询时间",
                        format: 'Y-m-d H:i:s',
                        width: 200
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'quota',
                        sortable: false,
                        menuDisabled: true,
                        text: "总额度",
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
                                xtype: 'combobox',
                                id: 'areacode',
                                width: 180,
                                fieldLabel: '地区',
                                editable: false,
                                labelWidth: 60,
                                store: Ext.create('Ext.data.Store', {
                                    fields: [
                                        { name: 'val' },
                                        { name: 'txt' }
                                    ],
                                    data: [
                                        { 'val': "", 'txt': '全部' },
                                        { 'val': "320400", 'txt': '常州市' },
                                        { 'val': "320500", 'txt': '苏州市' }
                                    ]


                                }),
                                queryMode: 'local',
                                displayField: 'txt',
                                valueField: 'val',
                                value: ''
                            },
                            
                            
                            {
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