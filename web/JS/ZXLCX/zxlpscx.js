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
            { name: 'memo' }
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
function DataBind(nPage) {
    CS('CZCLZ.ZXLCXClass.GetSelPSList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_userid").getValue(),   Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
}
  

function getListEXCELOUT(nPage) {


    DownloadFile("CZCLZ.ZXLCXClass.GetSelPSOutList", "中交兴路查询次数统计数据下载.xls", {
        'cx_userid': Ext.getCmp("cx_userid").getValue(),
      
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
                title: '中交兴路派送查询统计',
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
                        xtype: 'datecolumn',
                        dataIndex: 'addtime',
                        sortable: false,
                        menuDisabled: true,
                        text: "派送时间",
                        format: 'Y-m-d H:i:s',
                        width: 200
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'memo',
                        sortable: false,
                        menuDisabled: true,
                        text: "派送次数",
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
                                         DataBind(1);
                                      
                                }
                            }, {
                                xtype: 'button',
                                iconCls: 'search',
                                text: '导出EXCEL',
                                handler: function () {
                                         getListEXCELOUT(1);
                                     

                                }
                            }, {
                                xtype: 'button',
                                iconCls: 'search',
                                text: '新增派送',
                                handler: function () {
                                         var win = new PSWin();
                                        win.show(null, function () {
                                        });
                                     

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
  

    DataBind(1);
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
                        var idlist = [];

                        var grid = Ext.getCmp("gridMain");
                        var rds = grid.getStore();
                        
                        //for (var n = 0, len = rds.getCount(); n < len; n++) {
                        //    var rd = rds[n];
                        //    alert(rd.get("UserName"))
                        //    idlist.push(rd.get("UserName"));
                        //}

                        rds.each(function (record) {
                            idlist.push(record.get("UserName"));
                             
                        });
                        CS('CZCLZ.ZXLCXClass.SavePS', function (retVal) {
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
                        Ext.getCmp('uploadproductpic').upload('CZCLZ.ZXLCXClass.UploadSF', function (retVal) {
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
