var pageSize = 15;
var cx_yhm;
var cx_zt;
var cx_lx;
var id = "";
//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'redenvelopeid' },
       { name: 'userid' },
        { name: 'type' },
        { name: 'beuserid' },
        { name: 'money' },
        { name: 'addtime' },
        { name: 'isuse' },
        { name: 'validhour' },
        { name: 'ordercode' },
        { name: 'paisongdetailid' },
        { name: 'updatetime' },
       { name: 'UserXM' },
       { name: 'UserName' },
       { name: 'jzsj' }
       
    ],
    onPageChange: function (sto, nPage, sorters) {
        getList(nPage);
    }
});
//************************************数据源*****************************************

//************************************页面方法***************************************
function getList(nPage) {
    CS('CZCLZ.CWBBMag.GetPSHBJL', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_yhm").getValue(), Ext.getCmp("cx_zt").getValue(), Ext.getCmp("cx_lx").getValue());
}



//************************************页面方法***************************************

//************************************主界面*****************************************
Ext.onReady(function () {
    Ext.define('YhView', {
        extend: 'Ext.container.Viewport',

        layout: {
            type: 'fit'
        },

        initComponent: function () {
            var me = this;
            me.items = [
                {
                    xtype: 'gridpanel',
                    id: 'usergrid',
                    title: '',
                    store: store,
                    columnLines: true,
                    columns: [Ext.create('Ext.grid.RowNumberer'),
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'UserName',
                                sortable: false,
                                menuDisabled: true,
                                flex: 1,
                                text: "领取人"
                            },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'money',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: "金额"
                             },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'type',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: "类型",
                                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                     //0：新人红包；1：分享（暂不使用）；2、订单红包；3、平台派送红包；4、抽奖红包
                                     if (value == 0) {
                                         return "新人红包";
                                     } else if (value == 1) {
                                         return "分享";
                                     } else if (value == 2) {
                                         return "订单红包";
                                     } else if (value == 3) {
                                         return "平台派送红包";
                                     } else if (value == 4) {
                                         return "抽奖红包";
                                     }
                                 }
                             },
                             {
                                 xtype: 'datecolumn',
                                 dataIndex: 'jzsj',
                                 format: 'Y-m-d H:i:s',
                                 sortable: false,
                                 menuDisabled: true,
                                 width: 150,
                                 text: '截止时间'
                             },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'isuse',
                                flex: 1,
                                sortable: false,
                                menuDisabled: true,
                                text: "状态",
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    //过期：isuse=2；未使用：isuse = 0；已使用：isuse in (1，3，9)
                                    if (value == 2) {
                                        return "过期";
                                    } else if (value == 0) {
                                        return "未使用";
                                    } else if (value == 1 || value == 3 || value == 9) {
                                        return "已使用";
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
                                    xtype: 'displayfield',
                                    width: 160,
                                    labelWidth: 50,
                                    id:'zj',
                                    fieldLabel: '总计'
                                },
                                 {
                                     xtype: 'displayfield',
                                     width: 160,
                                     labelWidth: 50,
                                     id: 'ysy',
                                     fieldLabel: '已使用'
                                 },
                                  {
                                      xtype: 'displayfield',
                                      width: 160,
                                      labelWidth: 50,
                                      id: 'gq',
                                      fieldLabel: '过期'
                                  },
                                   {
                                       xtype: 'displayfield',
                                       width: 160,
                                       labelWidth: 50,
                                       id: 'wsy',
                                       fieldLabel: '未使用'
                                   }
                                
                            ]
                        },
                                {
                                    xtype: 'toolbar',
                                    dock: 'top',
                                    items: [
                                         {
                                             xtype: 'textfield',
                                             id: 'cx_yhm',
                                             width: 140,
                                             labelWidth: 50,
                                             fieldLabel: '领取人'
                                         },
                                          {
                                              xtype: 'combobox',
                                              id: 'cx_zt',
                                              width: 160,
                                              fieldLabel: '状态',
                                              editable: false,
                                              labelWidth: 40,
                                              store: Ext.create('Ext.data.Store', {
                                                  fields: [
                                                     { name: 'val' },
                                                     { name: 'txt' }
                                                  ],//过期：isuse=2；未使用：isuse = 0；已使用：isuse in (1，3，9)
                                                  data: [{ 'val': '', 'txt': '全部' },
                                                          { 'val': 0, 'txt': '未使用' },
                                                          { 'val': 1, 'txt': '已使用' },
                                                          { 'val': 2, 'txt': '过期' }
                                                          ]
                                              }),
                                              queryMode: 'local',
                                              displayField: 'txt',
                                              valueField: 'val',
                                              value: ''
                                          }, {
                                              xtype: 'combobox',
                                              id: 'cx_lx',
                                              width: 160,
                                              fieldLabel: '类型',
                                              editable: false,
                                              labelWidth: 40,
                                              store: Ext.create('Ext.data.Store', {
                                                  fields: [
                                                     { name: 'val' },
                                                     { name: 'txt' }
                                                  ], //0：新人红包；1：分享（暂不使用）；2、订单红包；3、平台派送红包；4、抽奖红包
                                                  data: [{ 'val': '', 'txt': '全部' },
                                                          { 'val': 0, 'txt': '新人红包' },
                                                          { 'val': 1, 'txt': '分享' },
                                                          { 'val': 2, 'txt': '订单红包' },
                                                          { 'val': 3, 'txt': '平台派送红包' },
                                                          { 'val': 4, 'txt': '抽奖红包' }]
                                              }),
                                              queryMode: 'local',
                                              displayField: 'txt',
                                              valueField: 'val',
                                              value: ''
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
                                        },
                                        {
                                            xtype: 'button',
                                            iconCls: 'view',
                                            text: '导出',
                                            handler: function () {
                                                if (privilege("财务报表_红包汇总_导出")) {
                                                    DownloadFile("CZCLZ.CWBBMag.GetPSHBJLToFile", "红包汇总.xls", Ext.getCmp("cx_yhm").getValue(), Ext.getCmp("cx_zt").getValue(), Ext.getCmp("cx_lx").getValue());
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

    new YhView();


    cx_yhm = Ext.getCmp("cx_yhm").getValue();
    cx_zt = Ext.getCmp("cx_zt").getValue();
    cx_lx = Ext.getCmp("cx_lx").getValue();
    getList(1);

    CS('CZCLZ.CWBBMag.GetPSHBZJ', function (retVal) {
        if (retVal) {
            Ext.getCmp("zj").setValue(retVal[0]["zj"]);
            Ext.getCmp("ysy").setValue(retVal[0]["ysy"]);
            Ext.getCmp("gq").setValue(retVal[0]["gq"]);
            Ext.getCmp("wsy").setValue(retVal[0]["wsy"]);

        }
    }, CS.onError);
})
//************************************主界面*****************************************