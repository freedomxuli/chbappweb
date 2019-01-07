﻿var pageSize = 15;
var cx_yhm;
var cx_xm;
var cx_iscanrelease;
//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'id' },
       { name: 'UserName' },
       { name: 'Password' },
       { name: 'roleId' },
       { name: 'roleName' },
       { name: 'UserXM' },
       { name: 'ClientKind' },
       { name: 'Address' },
       { name: 'UserTel' },
       { name: 'FromRoute' },
       { name: 'ToRoute' },
       { name: 'Points' },
       { name: 'SalePoints' },
       { name: 'KFGMPoints' },
       { name: 'UserContent' },
       { name: 'DqBm' },
       { name: 'AddTime' },
       { name: 'status' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getUser(nPage);
    }
});

var KindStore = Ext.create('Ext.data.Store', {
    fields: [
       { name: 'ID' },
       { name: 'MC' }
    ],
    data: [
        {
            'ID': '',
            'MC': '全部'
        },
        {
            'ID': '0',
            'MC': '待审核'
        },
        {
            'ID': '1',
            'MC': '通过'
        },
        {
            'ID': '2',
            'MC': '拒绝'
        }
    ]
});
//************************************数据源*****************************************

//************************************页面方法***************************************
function getUser(nPage) {
    CS('CZCLZ.ZXSHMag.GetZXList2', function (retVal) {
        if (retVal){
            var result = retVal.evalJSON();
            console.log(result);
            store.setData({
                data: result.list,
                pageSize: result.pagination.pageSize,
                total: result.pagination.total,
                currentPage: result.pagination.current
            });
        }
        }, CS.onError, nPage, pageSize, Ext.getCmp("cx_yhm").getValue(), Ext.getCmp("cx_xm").getValue(), Ext.getCmp("cx_ispass").getValue());
}


function ck(id) {
    var r = store.findRecord("id", id).data;
    var win = new addWin({id:id});
    win.show(null, function () { })
    Ext.getCmp("UserContent").setValue(r.UserContent);
    Ext.getCmp("btn1").hide();
    Ext.getCmp("btn2").hide();
    Ext.getCmp("btn3").show();
}

function sh(id) {
    var r = store.findRecord("id", id).data;
    var win = new addWin({ id: id });
    win.show(null, function () { })
    Ext.getCmp("UserContent").setValue(r.UserContent);
    Ext.getCmp("btn1").show();
    Ext.getCmp("btn2").show();
    Ext.getCmp("btn3").hide();
}

//************************************页面方法***************************************

//************************************弹出界面***************************************
Ext.define('addWin', {
    extend: 'Ext.window.Window',
    id:"addWin",
    height: 150,
    width: 400,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '用户简介',

    initComponent: function () {
        var me = this;
        var id = me.id;
        me.items = [
            {
                xtype: 'form',
                id: 'addform',
                bodyPadding: 10,
                items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: 'ID',
                        id: 'UserID',
                        name: 'UserID',
                        labelWidth: 70,
                        hidden: true,
                        colspan: 2
                    },
                     {
                         xtype: 'textareafield',
                         id: 'UserContent',
                         name: 'UserContent',
                         labelWidth: 70,
                         fieldLabel: '简介',
                         anchor: '100%'
                     }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '通过',
                        iconCls: 'dropyes',
                        id:"btn1",
                        handler: function () {
                            if (privilege("专线审核_简介审核_编辑")) {
                                Ext.MessageBox.confirm('提示', '是否通过!', function (obj) {
                                    if (obj == "yes") {
                                        CS('CZCLZ.ZXSHMag.SHJJCG', function (retVal) {
                                            if (retVal) {
                                                var result = retVal.evalJSON();
                                                getUser(1);
                                                if (result.success) {
                                                    Ext.MessageBox.alert('提示', "审核成功！");
                                                } else {
                                                    Ext.MessageBox.alert('提示', result.details);
                                                }
                                            }
                                        }, CS.onError, id);
                                    }
                                });
                            }
                            this.up('window').close();
                        }
                    },
                     {
                         text: '不通过',
                         id: "btn2",
                         iconCls: 'delete',
                         handler: function () {
                             if (privilege("专线审核_简介审核_编辑")) {
                                 this.up('window').close();
                                var yjwin = new yjWin({ id: id });
                                yjwin.show();
                               
                             }
                         }
                     },
                     {
                         text: '关闭',
                         id: "btn3",
                         iconCls: 'close',
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

Ext.define('yjWin', {
    extend: 'Ext.window.Window',

    height: 150,
    width: 400,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '退回意见',
    initComponent: function () {
        var me = this;
        var id = me.id;
        me.items = [
            {
                xtype: 'form',
                id: 'yjform',
                bodyPadding: 10,
                items: [
                     {
                         xtype: 'textareafield',
                         id: 'yj',
                         name: 'yj',
                         labelWidth: 70,
                         fieldLabel: '意见',
                         anchor: '100%'
                     }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '确认',
                        iconCls: 'dropyes',
                        handler: function () {
                            if (privilege("专线审核_简介审核_编辑")) {
                                CS('CZCLZ.ZXSHMag.SHJJSB', function (retVal) {
                                    if (retVal) {
                                        var result = retVal.evalJSON();
                                        console.log(result);
                                        getUser(1);
                                        if (result.success) {
                                            Ext.MessageBox.alert('提示', "审核成功！");
                                        }
                                        
                                    }
                                }, CS.onError, id, Ext.getCmp("yj").getValue());
                            }
                            this.up('window').close();
                        }
                    },
                     {
                         text: '取消',
                         iconCls: 'close',
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
                                dataIndex: 'UserXM',
                                sortable: false,
                                menuDisabled: true,
                                flex:1,
                                text: "专线名称"
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'UserName',
                                sortable: false,
                                menuDisabled: true,
                                width: 120,
                                text: "登录名"
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'UserTel',
                                sortable: false,
                                menuDisabled: true,
                                width: 120,
                                text: "电话"
                            },
                            {
                                xtype: 'gridcolumn',
                                sortable: false,
                                menuDisabled: true,
                                text: "线路",
                                flex: 1,
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    var str="";
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
                                xtype: 'gridcolumn',
                                dataIndex: 'UserContent',
                                sortable: false,
                                menuDisabled: true,
                                flex: 1,
                                text: "简介"
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'status',
                                sortable: false,
                                menuDisabled: true,
                                width: 150,
                                text: "是否通过",
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    var str="";
                                    if (value == 0) {
                                        str += "待审核"
                                    }else if (value == 1) {
                                        str += "通过"
                                    }
                                    else if (value == 2) {
                                        str += "拒绝"
                                    }
                                    return str;
                                }
                            },
                            {
                                xtype: 'gridcolumn',
                                sortable: false,
                                menuDisabled: true,
                                dataIndex: 'id',
                                text: '操作',
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {

                                    var str = "";
                                    if (record.data.status == 0) {
                                        str = "<a href='JavaScript:void(0)' onclick='sh(\"" + value + "\")'>审核</a>";
                                    } else if (record.data.status == 1) {
                                        str = "<a href='JavaScript:void(0)' onclick='ck(\"" + value + "\")'>查看</a>";
                                    }
                                    else if (record.data.status == 2) {
                                        str = "<a href='JavaScript:void(0)' onclick='ck(\"" + value + "\")'>查看</a>";
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
                                            id: 'cx_yhm',
                                            width: 140,
                                            labelWidth: 50,
                                            fieldLabel: '用户名'
                                        },
                                        {
                                            xtype: 'textfield',
                                            id: 'cx_xm',
                                            width: 160,
                                            labelWidth: 70,
                                            fieldLabel: '物流名称'
                                        },
                                        {
                                            xtype: 'combobox',
                                            id: 'cx_ispass',
                                            width: 260,
                                            fieldLabel: '是否通过',
                                            editable: false,
                                            labelWidth: 140,
                                            store: KindStore,
                                            queryMode: 'local',
                                            displayField: 'MC',
                                            valueField: 'ID',
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


    cx_yhm = Ext.getCmp("cx_yhm").getValue();
    cx_xm = Ext.getCmp("cx_xm").getValue();
    cx_iscanrelease = Ext.getCmp("cx_ispass").getValue();
    getUser(1);

})
//************************************主界面*****************************************