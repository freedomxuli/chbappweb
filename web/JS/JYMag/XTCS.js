//************************************数据源*****************************************
var store = Ext.create('Ext.data.Store', {
    fields: [
        {name:'id'},
       { name: 'carriagepayratio' },
       { name: 'carriageoil' }
    ]
});
//************************************数据源*****************************************

//************************************页面方法***************************************

function Edit(id) {
    var r = store.findRecord("id", id).data;
    var win = new addWin();
    win.show(null, function () {
        win.setTitle("系统参数设置");
        var form = Ext.getCmp('addform');
        form.form.setValues(r);
                
    });
            
    
}

function DataBind() {
    CS('CZCLZ.YKMag.GetVisionList', function (retVal) {
        if (retVal) {
            store.loadData(retVal)
        }
    }, CS.onError);
}


//************************************页面方法***************************************

//************************************弹出界面***************************************

Ext.define('addWin', {
    extend: 'Ext.window.Window',

    height: 160,
    width: 400,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '系统参数设置',

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
                        fieldLabel: 'id',
                        id: 'id',
                        name: 'id',
                        hidden: true
                    },
                    {
                        xtype: 'numberfield',
                        fieldLabel: '支付参数系数',
                        id: 'carriagepayratio',
                        name: 'carriagepayratio',
                        decimalPrecision:2,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: 'displayfield',
                        fieldLabel: '可划拨油量',
                        name: 'carriageoil',
                        id: 'carriageoil',
                        anchor: '100%'
                    },
                    {
                        xtype: 'numberfield',
                        fieldLabel: '新增可划拨油量',
                        id: 'XZCarriageoil',
                        name: 'XZCarriageoil',
                        allowDecimals: false,
                        allowNegative: false,
                        allowBlank: false,
                        anchor: '100%'
                    }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '确定',
                        iconCls: 'dropyes',
                        handler: function () {
                            var form = Ext.getCmp('addform');
                            if (form.form.isValid()) {
                                var values = form.form.getValues(false);
                                if (Ext.getCmp("XZCarriageoil").getValue() < 0) {
                                    Ext.Msg.show({
                                        title: '提示',
                                        msg: '新增可划拨油量不能小于0!',
                                        buttons: Ext.MessageBox.OK,
                                        icon: Ext.MessageBox.INFO
                                    });
                                    return;
                                }

                                if (Ext.getCmp("carriagepayratio").getValue() <= 0 || Ext.getCmp("carriagepayratio").getValue()>1) {
                                    Ext.Msg.show({
                                        title: '提示',
                                        msg: '支付参数系数必须大于0小于等于1!',
                                        buttons: Ext.MessageBox.OK,
                                        icon: Ext.MessageBox.INFO
                                    });
                                    return;
                                }
                                var me = this;
                                CS('CZCLZ.YKMag.SaveVision', function (retVal) {
                                    if (retVal) {
                                        me.up('window').close();
                                        DataBind();
                                    }
                                }, CS.onError, values, Ext.getCmp("carriageoil").getValue());

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

//************************************弹出界面***************************************

//************************************主界面*****************************************
Ext.onReady(function() {
    Ext.define('YhView', {
        extend: 'Ext.container.Viewport',

        layout: {
            type: 'fit'
        },

        initComponent: function() {
            var me = this;
            me.items = [
                {
                    xtype: 'gridpanel',
                    id: 'usergrid',
                    title: '',
                    store: store,
                    columnLines:true,
                    selModel: Ext.create('Ext.selection.CheckboxModel', {

                }),
                columns: [Ext.create('Ext.grid.RowNumberer'),
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'carriagepayratio',
                            sortable: false,
                            menuDisabled: true,
                            text: "支付参数系数"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'carriageoil',
                            sortable: false,
                            menuDisabled: true,
                            text: "可划拨油量"
                        },
                        {
                            text: '操作',
                            dataIndex: 'id',
                            width:150,
                            sortable: false,
                            menuDisabled: true,
                            renderer: function(value, cellmeta, record, rowIndex, columnIndex, store) {
                                var str;
                                str = "<a onclick='Edit(\"" + value + "\");'>修改</a>";
                                return str;
                            }
                        }
                    ],
                    viewConfig: {

                    }
        }
            ];
            me.callParent(arguments);
        }
    });

    new YhView();

   
    DataBind();
})
//************************************主界面*****************************************