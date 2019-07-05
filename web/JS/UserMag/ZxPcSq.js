/*
专线、三方客户端授权
*/
var pageSize = 15;

//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'UserID' },
       { name: 'UserName' },
       { name: 'UserXM' },
       { name: 'FromRoute' },
       { name: 'ToRoute' },
       { name: 'opentype' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getUser(nPage);
    }
});

var openStore = Ext.create('Ext.data.Store', {
    fields: [
       { name: 'id' },
       { name: 'mc' }
    ],
    data: [
        {
            'id': '',
            'mc': '全部',
        },
        {
            'id': '1',
            'mc': '已开通',
        },
        {
            'id': '2',
            'mc': '未开通'
        }
    ]
});

var modulestore = Ext.create('Ext.data.Store', {
    fields: [
       { name: 'moduleId' },
       { name: 'moduleName' },
       { name: 'moduleType' },
       { name: 'id' }
    ]
})

//************************************页面方法***************************************
function getUser(nPage) {
    CS('CZCLZ.YHGLClass.GetClientByOpen', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_xm").getValue(), Ext.getCmp("cx_open").getValue());
}

function OpenClientPC(userid)
{
    var win = new ModuleList({ userid: userid });
    win.show(null, function () {
        CS('CZCLZ.YHGLClass.GetModuleByPC', function (retVal) {
            modulestore.loadData(retVal);
            for (var i = 0; i < retVal.length; i++) {
                if (retVal[i].moduleType == 1)
                {
                    var record = modulestore.findRecord("moduleId", retVal[i].moduleId);
                    Ext.getCmp('moduleselect').getSelectionModel().select(record, true, true);
                }
            }
        },CS.onError,userid)
    });
}

function CloseClientPC(userid)
{
    Ext.MessageBox.confirm("提示", "是否关闭该专线PC端？", function (obj) {
        if (obj == "yes") {
            CS('CZCLZ.YHGLClass.ClosePc', function (retVal) {
                getUser(1);
                Ext.Msg.alert('提醒', '关闭成功！');
            }, CS.onError, userid);
        }
    });
}

function SelectModule(userid)
{
    var win = new ModuleSelectList({ userid: userid });
    win.show(null, function () {
        CS('CZCLZ.YHGLClass.GetModuleByPC', function (retVal) {
            modulestore.loadData(retVal);
            for (var i = 0; i < retVal.length; i++) {
                if (retVal[i].moduleType == 1) {
                    var record = modulestore.findRecord("moduleId", retVal[i].moduleId);
                    Ext.getCmp('moduleselect').getSelectionModel().select(record, true, true);
                }
            }
        }, CS.onError, userid)
    });
}

//************************************页面方法***************************************

//************************************弹出界面***************************************

Ext.define('ModuleList', {
    extend: 'Ext.window.Window',

    height: 422,
    width: 316,
    layout: {
        type: 'fit'
    },
    title: '模块列表',
    modal: true,

    initComponent: function () {
        var me = this;

        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'panel',
                    layout: {
                        type: 'fit'
                    },
                    items: [
                        {
                            xtype: 'gridpanel',
                            border: 1,
                            store: modulestore,
                            id: 'moduleselect',
                            columnLines: true,
                            selModel: Ext.create('Ext.selection.CheckboxModel', {

                            }),
                            columns: [
                                {
                                    xtype: 'gridcolumn',
                                    dataIndex: 'moduleId',
                                    sortable: false,
                                    menuDisabled: true,
                                    hidden: true,
                                    flex: 1,
                                    text: 'id'
                                },
                                {
                                    xtype: 'gridcolumn',
                                    dataIndex: 'moduleName',
                                    sortable: false,
                                    menuDisabled: true,
                                    flex: 1,
                                    text: '模块名称'
                                }
                            ]
                        }
                    ],
                    buttonAlign: 'center',
                    buttons: [
                        {
                            text: '选择并开通',
                            handler: function () {
                                var idlist = [];
                                var grid = Ext.getCmp("moduleselect");
                                var rds = grid.getSelectionModel().getSelection();
                                if (rds.length == 0) {
                                    Ext.Msg.show({
                                        title: '提示',
                                        msg: '请选择至少一条要开通的模块!',
                                        buttons: Ext.MessageBox.OK,
                                        icon: Ext.MessageBox.INFO
                                    });
                                    return;
                                } else {
                                    for (var n = 0, len = rds.length; n < len; n++) {
                                        var rd = rds[n];
                                        idlist.push(rd.get("moduleId"));
                                    }
                                    CS('CZCLZ.YHGLClass.OpenPc', function (retVal) {
                                        if (retVal) {
                                            Ext.Msg.alert('提醒', '开通成功！');
                                            getUser(1);
                                            me.close();
                                        } else {
                                            Ext.Msg.alert('提醒', '开通失败！');
                                        }
                                    }, CS.onError, idlist, me.userid);
                                }
                            }
                        },
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

Ext.define('ModuleSelectList', {
    extend: 'Ext.window.Window',

    height: 422,
    width: 316,
    layout: {
        type: 'fit'
    },
    title: '模块列表',
    modal: true,

    initComponent: function () {
        var me = this;

        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'panel',
                    layout: {
                        type: 'fit'
                    },
                    items: [
                        {
                            xtype: 'gridpanel',
                            border: 1,
                            store: modulestore,
                            id: 'moduleselect',
                            columnLines: true,
                            selModel: Ext.create('Ext.selection.CheckboxModel', {

                            }),
                            columns: [
                                {
                                    xtype: 'gridcolumn',
                                    dataIndex: 'moduleId',
                                    sortable: false,
                                    menuDisabled: true,
                                    hidden: true,
                                    flex: 1,
                                    text: 'id'
                                },
                                {
                                    xtype: 'gridcolumn',
                                    dataIndex: 'moduleName',
                                    sortable: false,
                                    menuDisabled: true,
                                    flex: 1,
                                    text: '模块名称'
                                }
                            ]
                        }
                    ],
                    buttonAlign: 'center',
                    buttons: [
                        {
                            text: '确认',
                            handler: function () {
                                var idlist = [];
                                var grid = Ext.getCmp("moduleselect");
                                var rds = grid.getSelectionModel().getSelection();
                                if (rds.length == 0) {
                                    Ext.Msg.show({
                                        title: '提示',
                                        msg: '请选择至少一条要开通的模块!',
                                        buttons: Ext.MessageBox.OK,
                                        icon: Ext.MessageBox.INFO
                                    });
                                    return;
                                } else {
                                    for (var n = 0, len = rds.length; n < len; n++) {
                                        var rd = rds[n];
                                        idlist.push(rd.get("moduleId"));
                                    }
                                    CS('CZCLZ.YHGLClass.SelectModule', function (retVal) {
                                        if (retVal) {
                                            Ext.Msg.alert('提醒', '开通成功！');
                                            getUser(1);
                                            me.close();
                                        } else {
                                            Ext.Msg.alert('提醒', '开通失败！');
                                        }
                                    }, CS.onError, idlist, me.userid);
                                }
                            }
                        },
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

//************************************弹出界面***************************************

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
                    //selModel: Ext.create('Ext.selection.CheckboxModel', {

                    //}),
                    columns: [Ext.create('Ext.grid.RowNumberer'),
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'UserName',
                                sortable: false,
                                menuDisabled: true,
                                text: "登录名"
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'UserXM',
                                sortable: false,
                                menuDisabled: true,
                                width: 360,
                                text: "账户名称"
                            },
                            {
                                xtype: 'gridcolumn',
                                sortable: false,
                                menuDisabled: true,
                                text: "线路",
                                width: 240,
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    var str = "";
                                    if (record.data.FromRoute) {
                                        str += record.data.FromRoute
                                    }
                                    if (record.data.ToRoute) {
                                        str += "─" + record.data.ToRoute;
                                    }
                                    return str;
                                }

                            },
                            {
                                text: '操作',
                                dataIndex: 'UserID',
                                width: 350,
                                sortable: false,
                                menuDisabled: true,
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    var str;
                                    if (record.data.opentype == 0) {
                                        str = "<a onclick='OpenClientPC(\"" + value + "\");'>开通PC</a>";
                                    } else {
                                        str = "<a onclick='CloseClientPC(\"" + value + "\");'>关闭专线PC</a> <a onclick='SelectModule(\"" + value + "\");'>选择模块</a>";
                                    }
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
                                            id: 'cx_xm',
                                            width: 200,
                                            labelWidth: 70,
                                            fieldLabel: '账户名称'
                                        },
                                        {
                                            xtype: 'combobox',
                                            id: 'cx_open',
                                            width: 200,
                                            fieldLabel: '是否开通',
                                            editable: false,
                                            labelWidth: 80,
                                            store: openStore,
                                            queryMode: 'local',
                                            displayField: 'mc',
                                            valueField: 'id',
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
                                                        getUser(1);
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

    new YhView();

    Ext.getCmp("cx_open").setValue('');

    getUser(1);

})
//************************************主界面*****************************************