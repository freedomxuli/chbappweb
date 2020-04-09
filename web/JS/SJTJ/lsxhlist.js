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
          { name: 'num' },
           { name: 'UserName' },
            { name: 'ordercode' },
              { name: 'addtime' },
    { name: 'UserXM' }
 
    ],
    onPageChange: function (sto, nPage, sorters) {
        DataBind(nPage);
    }
});

var store2 = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
         { name: 'dq_mc' },
          { name: 'OrderCode' },
           { name: 'UserName' },
            { name: 'Points' },
               { name: 'AddTime' },
    { name: 'UserXM' }

    ],
    onPageChange: function (sto, nPage, sorters) {
        DataBind(nPage);
    }
});

var store3 = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
         { name: 'dq_mc' },
          { name: 'OrderCode' },
           { name: 'UserName' },
            { name: 'Points' },
               { name: 'AddTime' },
    { name: 'UserXM' }

    ],
    onPageChange: function (sto, nPage, sorters) {
        DataBind(nPage);
    }
});

var sfstore = Ext.create('Ext.data.Store', {
    fields: [
        'UserID', 'UserName'
    ]
})
//-----------------------------------------------------------页面方法-----------------------------------------------------------------
function DataBindV1(nPage) {
    CS('CZCLZ.SJPBClass.lsxhlistV1', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("areacode").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
}
  

function getListEXCELOUTV1(nPage) {


    DownloadFile("CZCLZ.SJPBClass.GetTkshListOutListV1", "历史消耗红包额度明细.xls", {
        'areacode': Ext.getCmp("areacode").getValue(),
         'cx_beg': Ext.getCmp("cx_beg").getValue(),
        'cx_end': Ext.getCmp("cx_end").getValue()
        
    });


}



function DataBindV2(nPage) {
    CS('CZCLZ.SJPBClass.lsxhlistV2', function (retVal) {
        store2.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("areacode2").getValue(), Ext.getCmp("cx_beg2").getValue(), Ext.getCmp("cx_end2").getValue());
}

function getListEXCELOUTV2(nPage) {


    DownloadFile("CZCLZ.SJPBClass.GetTkshListOutListV2", "历史配比红包专线订单数明细.xls", {
        'areacode': Ext.getCmp("areacode2").getValue(),
        'cx_beg': Ext.getCmp("cx_beg2").getValue(),
        'cx_end': Ext.getCmp("cx_end2").getValue()

    });


}


function DataBindV3(nPage) {
    CS('CZCLZ.SJPBClass.lsxhlistV3', function (retVal) {
        store3.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("areacode3").getValue(), Ext.getCmp("cx_beg3").getValue(), Ext.getCmp("cx_end3").getValue());
}

function getListEXCELOUTV3(nPage) {


    DownloadFile("CZCLZ.SJPBClass.GetTkshListOutListV3", "历史原价购买订单数.xls", {
        'areacode': Ext.getCmp("areacode3").getValue(),
        'cx_beg': Ext.getCmp("cx_beg3").getValue(),
        'cx_end': Ext.getCmp("cx_end3").getValue()

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
                xtype: 'tabpanel',
                activeTab: 0,
                items: [
                    {
                        xtype: 'panel',
                        layout: {
                            type: 'fit'
                        },
                        title: '历史消耗红包额度明细',

                        items: [
                           {
                               xtype: 'gridpanel',
                               columnLines: 1,
                               border: 1,
                               store: store,

                               columns: [
                                   {
                                       xtype: 'gridcolumn',
                                       dataIndex: 'dq_mc',
                                       sortable: false,
                                       menuDisabled: true,
                                       width: 100,
                                       text: '地区'
                                   },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'num',
                                        sortable: false,
                                        menuDisabled: true,
                                        width: 100,
                                        text: '额度明细'
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'UserName',
                                        sortable: false,
                                        menuDisabled: true,
                                        width: 200,
                                        text: '用户'
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'ordercode',
                                        sortable: false,
                                        menuDisabled: true,
                                        flex: 1,
                                        text: '订单号'
                                    },
                                      {
                                          xtype: 'datecolumn',
                                          dataIndex: 'addtime',
                                          sortable: false,
                                          menuDisabled: true,
                                          text: "消耗时间",
                                          format: 'Y-m-d H:i:s',
                                          width: 200
                                      },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'UserXM',
                                        sortable: false,
                                        menuDisabled: true,
                                        flex: 1,
                                        text: '专线名称'
                                    }
                               ],
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
                                                    DataBindV1(1);

                                                }
                                            }
                                       ]
                                   },
                                   {
                                       xtype: 'buttongroup',
                                       title: '',
                                       items: [
                                           {
                                               xtype: 'button',
                                               iconCls: 'view',
                                               text: '导出',
                                               handler: function () {
                                                   getListEXCELOUTV1();

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
                        ]
                    },
                    {
                        xtype: 'panel',
                        layout: {
                            type: 'fit'
                        },
                        title: '历史配比红包专线订单数明细',

                        items: [
                           {
                               xtype: 'gridpanel',
                               columnLines: 1,
                               border: 1,
                               store: store2,

                               columns: [
                                   {
                                       xtype: 'gridcolumn',
                                       dataIndex: 'dq_mc',
                                       sortable: false,
                                       menuDisabled: true,
                                       width: 100,
                                       text: '地区'
                                   },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'OrderCode',
                                        sortable: false,
                                        menuDisabled: true,
                                        flex: 1,
                                        text: '订单号'
                                    }, {
                                        xtype: 'datecolumn',
                                        dataIndex: 'AddTime',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "时间",
                                        format: 'Y-m-d H:i:s',
                                        width: 200
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'Points',
                                        sortable: false,
                                        menuDisabled: true,
                                        width: 200,
                                        text: '消费券额'
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'UserName',
                                        sortable: false,
                                        menuDisabled: true,
                                        width: 200,
                                        text: '用户'
                                    },
                                      
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'UserXM',
                                        sortable: false,
                                        menuDisabled: true,
                                        flex: 1,
                                        text: '专线名称'
                                    }
                               ],
                               dockedItems: [
                               {
                                   xtype: 'toolbar',
                                   dock: 'top',
                                   items: [
                                       {
                                           xtype: 'combobox',
                                           id: 'areacode2',
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
                                           id: 'cx_beg2',
                                           xtype: 'datefield',
                                           fieldLabel: '时间',
                                           format: 'Y-m-d',
                                           labelWidth: 40,
                                           width: 170
                                       },
                                       {
                                           id: 'cx_end2',
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
                                                        DataBindV2(1);

                                                    }
                                                }
                                           ]
                                       },
                                       {
                                           xtype: 'buttongroup',
                                           title: '',
                                           items: [
                                               {
                                                   xtype: 'button',
                                                   iconCls: 'view',
                                                   text: '导出',
                                                   handler: function () {
                                                       getListEXCELOUTV2();

                                                   }
                                               }
                                           ]
                                       }
                                   ]
                               },
                               {
                                   xtype: 'pagingtoolbar',
                                   displayInfo: true,
                                   store: store2,
                                   dock: 'bottom'
                               }
                               ]
                           }
                        ]
                    },
                    {
                        xtype: 'panel',
                        layout: {
                            type: 'fit'
                        },
                        title: '历史原价购买订单数',

                        items: [
                           {
                               xtype: 'gridpanel',
                               columnLines: 1,
                               border: 1,
                               store: store3,

                               columns: [
                                   {
                                       xtype: 'gridcolumn',
                                       dataIndex: 'dq_mc',
                                       sortable: false,
                                       menuDisabled: true,
                                       width: 100,
                                       text: '地区'
                                   },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'OrderCode',
                                        sortable: false,
                                        menuDisabled: true,
                                        flex: 1,
                                        text: '订单号'
                                    }, {
                                        xtype: 'datecolumn',
                                        dataIndex: 'AddTime',
                                        sortable: false,
                                        menuDisabled: true,
                                        text: "时间",
                                        format: 'Y-m-d H:i:s',
                                        width: 200
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'Points',
                                        sortable: false,
                                        menuDisabled: true,
                                        width: 200,
                                        text: '消费券额'
                                    },
                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'UserName',
                                        sortable: false,
                                        menuDisabled: true,
                                        width: 200,
                                        text: '用户'
                                    },

                                    {
                                        xtype: 'gridcolumn',
                                        dataIndex: 'UserXM',
                                        sortable: false,
                                        menuDisabled: true,
                                        flex: 1,
                                        text: '专线名称'
                                    }
                               ],
                               dockedItems: [
                               {
                                   xtype: 'toolbar',
                                   dock: 'top',
                                   items: [
                                       {
                                           xtype: 'combobox',
                                           id: 'areacode3',
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
                                           id: 'cx_beg3',
                                           xtype: 'datefield',
                                           fieldLabel: '时间',
                                           format: 'Y-m-d',
                                           labelWidth: 40,
                                           width: 170
                                       },
                                       {
                                           id: 'cx_end3',
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
                                                        DataBindV3(1);

                                                    }
                                                }
                                           ]
                                       },
                                       {
                                           xtype: 'buttongroup',
                                           title: '',
                                           items: [
                                               {
                                                   xtype: 'button',
                                                   iconCls: 'view',
                                                   text: '导出',
                                                   handler: function () {
                                                       getListEXCELOUTV3();

                                                   }
                                               }
                                           ]
                                       }
                                   ]
                               },
                               {
                                   xtype: 'pagingtoolbar',
                                   displayInfo: true,
                                   store: store3,
                                   dock: 'bottom'
                               }
                               ]
                           }
                        ]
                    }

                ]
            }
        ];
        me.callParent(arguments);
    }
});

Ext.onReady(function () {
    new CYDView();
 

    DataBindV1();

    DataBindV2();
    DataBindV3();
})



Ext.define('PSWin', {
    extend: 'Ext.window.Window',

    height: 407,
    width: 634,
    layout: {
        type: 'fit'
    },
    title: '派送管理',
    modal: true,

    initComponent: function () {
        var me = this;
        
        Ext.applyIf(me, {
            items: [

                {
                    xtype: 'gridpanel',
                    border: 1,
                    id: 'gridMain',
                    columnLines: 1,
                    store: sfstore,
                    dockedItems: [
 
                        {
                            xtype: 'toolbar',
                            dock: 'top',
                            items: [
                                {
                                    xtype: 'numberfield',
                                    fieldLabel: '派送次数',
                                    allowBlank: false,
                                    allowDecimals: false,
                                    allowNegative: false,
                                    minValue: 1,
                                    id: 'psyfq'
                                },
                                {
                                    xtype: 'buttongroup',
                                    title: '',
                                    items: [
                                        {
                                            xtype: 'button',
                                            iconCls: 'view',
                                            text: '下载模板',
                                            handler: function () {
                                                window.location.href = "approot/r/JS/ZXLCX/派送模板.xls";
                                            }
                                        }
                                    ]
                                },
                                {
                                    xtype: 'buttongroup',
                                    title: '',
                                    items: [
                                        {
                                            xtype: 'button',
                                            iconCls: 'upload',
                                            text: '上传派送用户',
                                            handler: function () {
                                                var win = new drWin();
                                                win.show();
                                            }
                                        }
                                    ]
                                }
                            ]
                        }
                    ],
                    columns: [Ext.create('Ext.grid.RowNumberer'),
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'UserID',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            hidden: true
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'UserName',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: '派送用户'
                        }
                    ]
                }
            ],
            buttonAlign: 'center',
            buttons: [
                {
                    text: '确定',
                    iconCls: 'dropyes',
                    handler: function () {

                        if (!Ext.getCmp("psyfq").getValue()) {
                            Ext.Msg.show({
                                title: '错误',
                                msg: '请填写派送次数！',
                                buttons: Ext.MessageBox.OK,
                                icon: Ext.MessageBox.ERROR
                            });
                            return;
                        }
 

                        var grid = Ext.getCmp("gridMain");
                        var rds = grid.grid.getStore();
                        
                        for (var n = 0, len = rds.length; n < len; n++) {
                            var rd = rds[n];
                            idlist.push(rd.get("UserID"));
                        }
                        CS('CZCLZ.SJPBClass.SavePS', function (retVal) {
                            Ext.MessageBox.alert('提醒', '派送成功！');
                            
                        }, CS.onError, idlist, Ext.getCmp("psyfq").getValue());


                    
                        sfstore.removeAll();
                        this.up('window').close();

                    }
                },
                {
                    text: '关闭',
                    iconCls: 'close',
                    handler: function () {
                        sfstore.removeAll();
                        me.close();
                    }
                }
            ]
        });

        me.callParent(arguments);
    }

});



Ext.define('drWin', {
    extend: 'Ext.window.Window',

    height: 120,
    width: 350,
    id: "drWin",
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    title: '导入',

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'UploaderPanel',
                id: 'uploadproductpic',
                frame: true,
                bodyPadding: 10,
                title: '',
                items: [
            {
                xtype: 'filefield',
                fieldLabel: '上传文件',
                labelWidth: 70,
                buttonText: '浏览',
                allowBlank: false,
                anchor: '100%'
            }
                ],
                buttonAlign: 'center',
                buttons: [
            {
                text: '确定',
                handler: function () {
                    var form = Ext.getCmp('uploadproductpic');
                    if (form.form.isValid()) {
                        //取得表单中的内容
                        var values = form.form.getValues(false);
                        var me = this;
                        Ext.getCmp('uploadproductpic').upload('CZCLZ.SJPBClass.UploadSF', function (retVal) {
                            if (retVal) {
                                if (retVal.dt) {
                                    sfstore.loadData(retVal.dt);
                                    me.up('window').close();
                                }
                                if (retVal.str) {
                                    Ext.Msg.show({
                                        title: '提示',
                                        msg: retVal.str,
                                        buttons: Ext.MessageBox.OK,
                                        icon: Ext.MessageBox.INFO,
                                        fn: function () {
                                            me.up('window').close();
                                        }
                                    });
                                }
                            }
                        }, function (err) {
                            Ext.Msg.show({
                                title: '错误',
                                msg: err,
                                buttons: Ext.MessageBox.OK,
                                icon: Ext.MessageBox.ERROR,
                                fn: function () {
                                    me.up('window').close();
                                }
                            });
                        });
                    }
                }
            },
            {
                text: '关闭',
                handler: function () {
                    this.up('window').close();
                }
            }
                ]
            }
        ];
        me.callParent(arguments);
    }
});
