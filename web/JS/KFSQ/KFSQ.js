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
       { name: 'userID' },
       { name: 'userxm' },
       { name: 'zt' },
       { name: 'pointkind' },
       { name: 'discount' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getList(nPage);
    }
});



//************************************数据源*****************************************

//************************************页面方法***************************************

function getList(nPage) {
    CS('CZCLZ.KFSQMag.GetList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_yhm").getValue(), Ext.getCmp("cx_zt").getValue());
}



function SJ(id) {
    if (privilege("开放专线授权运费券_开放专线授权运费券_编辑")) {
        var win = new addWin();
        win.show(null, function () {
            Ext.getCmp("UserId").setValue(id);
        });
    }
}

function XJ(id) {
    if (privilege("开放专线授权运费券_开放专线授权运费券_编辑")) {
        CS('CZCLZ.KFSQMag.KFSQXJ', function (retVal) {
            if (retVal) {
                getList(1);
                Ext.MessageBox.alert('确认', '下架成功！');
            }
        }, CS.onError, id);
    }
}



//************************************页面方法***************************************

//************************************弹出界面***************************************


Ext.define('addWin', {
    extend: 'Ext.window.Window',

    height: 150,
    width: 400,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '上架',

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'form',
                id: 'addform',
                bodyPadding: 10,
                items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: 'ID',
                        id: 'UserId',
                        name: 'UserId',
                        hidden: true
                    },
                    {
                        xtype: 'numberfield',
                        fieldLabel: '优惠折扣',
                        id: 'discount',
                        name: 'discount',
                        allowDecimals: true,    //是否允许小数
                        decimalPrecision: 2,    // 精确的位数
                        allowNegative: false,
                        minValue: 0,
                        maxValue: 1,
                        allowBlank: false,
                        anchor: '100%',
                        listeners: {
                            afterrender: function () {
                                Ext.get(this.el).select('input').on('keyup', function (evt, t, option) {
                                    var zk = Number(Ext.getCmp("discount").getValue() * 100).toFixed(0) + "";
                                    if (zk == 1) {
                                        Ext.getCmp("discountmemo").setValue("原价");
                                    } else {
                                        var lastNum = zk.substr(zk.length - 1, 1);
                                        if (lastNum == '0') {
                                            zk = zk.substr(0, zk.length - 1);
                                        }
                                        Ext.getCmp("discountmemo").setValue(zk + "折");
                                    }
                                    return false;
                                });
                            }
                        },
                        value:1
                    },
                    {
                        xtype: "label",
                        text: "*优惠折扣必须为介于0-1的两位小数！",
                        style: "color:red;padding-left:100px;"
                    },
                    {
                        xtype: 'textfield',
                        id: 'discountmemo',
                        name: 'discountmemo',
                        fieldLabel: '优惠折扣备注',
                        allowBlank: false,
                        anchor: '100%',
                        readOnly: true,
                        value: "原价",
                        hidden: true

                    }
                
                    
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '确定',
                        iconCls: 'dropyes',
                        handler: function () {
                            Ext.getCmp("discount").setDisabled(false);
                            Ext.getCmp("discountmemo").setDisabled(false);
                            var form = Ext.getCmp('addform');
                            if (form.form.isValid()) {
                                var values = form.form.getValues(false);
                                console.log(values);
                                var me = this;
                                CS('CZCLZ.KFSQMag.SaveKFSQ', function (retVal) {
                                    if (retVal) {
                                        me.up('window').close();
                                        getList(1);
                                        Ext.MessageBox.alert('确认', '上架成功！');
                                    }
                                }, CS.onError, values);

                            }
                        }
                    },
                     {
                         text: '取消',
                         iconCls: 'back',
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


//***************************上传附件层*********************************************************************************************************************
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
                        Ext.getCmp('uploadproductpic').upload('CZCLZ.KFSQMag.UploadSF', function (retVal) {
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

//************************************************************************************************************************************************************
//************************************弹出界面***************************************

//************************************主界面*****************************************
Ext.onReady(function () {
    Ext.define('KFSQView', {
        extend: 'Ext.container.Viewport',

        layout: {
            type: 'fit'
        },

        initComponent: function () {
            var me = this;
            me.items = [
                {
                    xtype: 'gridpanel',
                    id: 'KFSQGrid',
                    store: store,
                    columnLines: 1,
                    border: 1,
                    columns: [Ext.create('Ext.grid.RowNumberer'),
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'userxm',
                            sortable: false,
                            menuDisabled: true,
                            width: 400,
                            text: '专线名称'
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'zt',
                            sortable: false,
                            menuDisabled: true,
                            width: 200,
                            text: '上架状态',
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                if (value == 0) {
                                    return "未上架";
                                } else if (value == 1) {
                                    return "上架在售";
                                } else if (value == 2) {
                                    return "上架售罄";
                                } else if (value == 3) {
                                    return "已下架";
                                } else {
                                    return "";
                                }
                            }
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'discount',
                            sortable: false,
                            menuDisabled: true,
                            width: 200,
                            text: '上架折扣',
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                if (value) {
                                    if (value == 1) {
                                        return "原价";
                                    } else {
                                        var zk = Number(value * 100).toFixed(0) + "";
                                        var lastNum = zk.substr(zk.length - 1, 1);
                                        if (lastNum == '0') {
                                            zk = zk.substr(0, zk.length - 1);
                                        }
                                        return zk + "折";
                                    }
                                }
                            }
                        },
                        {
                            xtype: 'gridcolumn',
                            sortable: false,
                            menuDisabled: true,
                            dataIndex: 'userID',
                            text: '操作',
                            width: 150,
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                str = "<a href='JavaScript:void(0)' onclick='SJ(\"" + value + "\")'>上架</a> || <a href='JavaScript:void(0)' onclick='XJ(\"" + value + '\",\"' + record.data.UserID + '\",\"' + record.data.Points + "\")'>下架</a>";
                                return str;
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
                                    fieldLabel: '专线名称'
                                },
                                {
                                    xtype: 'combobox',
                                    id: 'cx_zt',
                                    fieldLabel: '上架状态',
                                    editable: false,
                                    labelWidth: 60,
                                    store: Ext.create('Ext.data.Store', {
                                        fields: [
                                           { name: 'val' },
                                           { name: 'txt' }
                                        ], 
                                        data: [{ 'val': '', 'txt': '全部' },
                                                { 'val': 0, 'txt': '未上架' },
                                                { 'val': 1, 'txt': '上架在售' },
                                                { 'val': 2, 'txt': '上架售罄' },
                                                { 'val': 3, 'txt': '已下架' }]
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
                                                if (privilege("开放专线授权运费券_开放专线授权运费券_查看")) {
                                                    getList(1);
                                                }
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
                                            text: '导出',
                                            iconCls: 'download',
                                            handler: function () {
                                                if (privilege("开放专线授权运费券_开放专线授权运费券_导出")) {
                                                    DownloadFile("CZCLZ.KFSQMag.GetListToFile", "开放专线授权运费券.xls", Ext.getCmp("cx_yhm").getValue(), Ext.getCmp("cx_zt").getValue());
                                                }
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

    new KFSQView();

    getList();
})
//************************************主界面*****************************************