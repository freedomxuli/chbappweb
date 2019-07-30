//-----------------------------------------------------------全局变量-----------------------------------------------------------------


//-----------------------------------------------------------数据源-------------------------------------------------------------------
var ChbStore = Ext.create('Ext.data.Store', {
    fields: [
       { name: 'id' },
       { name: 'name' }
    ]
});

//-----------------------------------------------------------页面方法-----------------------------------------------------------------
function GetInfo() {
    CS('CZCLZ.ChbKp.GetInfo', function (retVal) {
        if (retVal) {
            ChbStore.loadData(retVal);
        }
    }, CS.onError);
}

function xg(id) {
    var r = ChbStore.findRecord("id", id).data;
    var win = new addWin();
    win.show(null, function () {
        win.setTitle("修改");
        var form = Ext.getCmp('addform');
        form.form.setValues(r);
    });
}
//-----------------------------------------------------------编辑界面-----------------------------------------------------------------
Ext.define('addWin', {
    extend: 'Ext.window.Window',

    height: 250,
    width: 400,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '新增',

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'form',
                id: 'addform',
                frame: true,
                bodyPadding: 10,
                title: '',
                items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: 'ID',
                        id: 'id',
                        name: 'id',
                        labelWidth: 70,
                        hidden: true,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textareafield',
                        id: 'name',
                        name: 'name',
                        fieldLabel: '名称',
                        labelWidth: 70,
                        allowBlank: false,
                        anchor: '100%'
                    }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '确定',
                        handler: function () {
                            var form = Ext.getCmp('addform');
                            if (form.form.isValid()) {
                                //取得表单中的内容
                                var values = form.form.getValues(false);
                                var me = this;
                                CS('CZCLZ.ChbKp.SaveInfo', function (retVal) {
                                    if (retVal) {
                                        ChbStore.loadData(retVal);
                                    }
                                    me.up('window').close()
                                }, CS.onError, values);
                            }
                        }
                    },
                    {
                        text: '取消',
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

//-----------------------------------------------------------界    面-----------------------------------------------------------------
Ext.define('ChbView', {
    extend: 'Ext.container.Viewport',

    layout: {
        type: 'fit'
    },

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'gridpanel',
                id: 'ChbGrid',
                store: ChbStore,
                columnLines: 1,
                border: 1,
                columns: [Ext.create('Ext.grid.RowNumberer'),
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'name',
                        sortable: false,
                        menuDisabled: true,
                        width: 400,
                        text: '名称'
                    },
                    {
                        xtype: 'gridcolumn',
                        sortable: false,
                        menuDisabled: true,
                        text: '操作',
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            var r = record.data;
                            var id = r["id"];
                            return "<a href='JavaScript:void(0)' onclick='xg(\"" + id + "\")'>修改</a>";
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
                                xtype: 'buttongroup',
                                title: '',
                                items: [
                                    {
                                        xtype: 'button',
                                        iconCls: 'add',
                                        text: '新增',
                                        handler: function () {
                                            var win = new addWin();
                                            win.show();
                                        }
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
    new ChbView();

    GetInfo();
})