﻿var pageSize=15;
var cx_role;
var cx_yhm;
var cx_xm;
//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'UserID' },
       { name: 'UserName' },
       { name: 'Password' },
       { name: 'roleId' },
       { name: 'roleName' },
       { name: 'UserXM' },
       { name: 'ClientKind' },
       { name: 'Address' },
       { name: 'UserTel' }
    ],
    onPageChange: function(sto, nPage, sorters) {
        getUser(nPage);
    }
});


var roleStore = Ext.create('Ext.data.Store', {
    fields: [
       { name: 'roleId' },
       { name: 'roleName' }
    ]
});

var roleStore1 = Ext.create('Ext.data.Store', {
    fields: [
       { name: 'roleId' },
       { name: 'roleName' }
    ]
});



//************************************数据源*****************************************

//************************************页面方法***************************************
function getUser(nPage) {
    CS('CZCLZ.YHGLClass.GetUserList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_role").getValue(), Ext.getCmp("cx_yhm").getValue(), Ext.getCmp("cx_xm").getValue());
}

function EditUser(id) {
    var r = store.findRecord("UserID", id).data;
    CS('CZCLZ.YHGLClass.GetRole', function (retVal) {
        if (retVal) {
            roleStore1.loadData(retVal, false);
            var win = new addWin();
            win.show(null, function () {
                win.setTitle("用户修改");
                var form = Ext.getCmp('addform');
                form.form.setValues(r);
                if (r.ClientKind == 2) {
                    Ext.getCmp("roleId").allowBlank = true;
                    Ext.getCmp("roleId").hide();
                    Ext.getCmp("UserXM").allowBlank = true;
                    Ext.getCmp("UserXM").hide();
                } else if (r.ClientKind == 1) {
                    Ext.getCmp("roleId").allowBlank = true;
                    Ext.getCmp("roleId").hide();
                } else {
                    Ext.getCmp("roleId").getEl().allowBlank = false;
                    Ext.getCmp("roleId").show();
                    Ext.getCmp("UserXM").getEl().allowBlank = false;
                    Ext.getCmp("UserXM").show();
                }
                
            });
            
        }
    }, CS.onError);
    
}

function AddPhoto(v)
{
    var picItem = [];
    CS('CZCLZ.YHGLClass.GetProductImages', function (retVal) {
        for (var i = 0; i < retVal.length; i++) {
            var isDefault = false;
            if (retVal[i].ISDEFAULT == 1)
                isDefault = true;
            Ext.getCmp('uploadproductpic').add(new SelectImg({

                isSelected: isDefault,
                src: retVal[i].FILEURL,
                fileid: retVal[i].fj_id
            }));
        }
    }, CS.onError, v);

    var win = new phWin({ UserID: v });
    win.show();
}
//************************************页面方法***************************************

//************************************弹出界面***************************************
Ext.define('SelectImg', {
    extend: 'Ext.Img',

    height: 80,
    width: 120,
    margin: 5,
    padding: 2,
    constructor: function (config) {
        var me = this;
        config = config || {};
        config.cls = config.isSelected ? "clsSelected" : "clsUnselected";
        me.callParent([config]);
        me.on('render', function () {
            Ext.fly(me.el).on('click', function () {
                var oldSelectImg = Ext.getCmp('uploadproductpic').query('image[isSelected=true]');
                if (oldSelectImg.length < 0 || oldSelectImg[0] != me) {
                    me.removeCls('clsUnselected');
                    me.addCls('clsSelected');
                    me.isSelected = true;
                    if (oldSelectImg.length > 0) {
                        oldSelectImg[0].removeCls('clsSelected');
                        oldSelectImg[0].addCls('clsUnselected');
                        oldSelectImg[0].isSelected = false;
                    }
                }
            });
        });

    },

    initComponent: function () {
        var me = this;
        me.callParent(arguments);
    }
});

Ext.define('phWin', {
    extend: 'Ext.window.Window',
    height: 275,
    width: 653,
    modal: true,
    layout: 'border',
    initComponent: function () {
        var me = this;
        var UserID = me.UserID;


        me.items = [{
            xtype: 'UploaderPanel',
            id: 'uploadproductpic',
            region: 'center',
            autoScroll: true,
            dockedItems: [{
                xtype: 'toolbar',
                dock: 'top',
                items: [{
                    xtype: 'filefield',
                    fieldLabel: '上传图片',
                    width: 300,
                    buttonText: '浏览'
                }, {
                    xtype: 'button',
                    text: '上传',
                    iconCls: 'upload',
                    handler: function () {
                        Ext.getCmp('uploadproductpic').upload('CZCLZ.YHGLClass.UploadPicForProduct', function (retVal) {
                            var isDefault = false;
                            if (retVal.isdefault == 1)
                                isDefault = true;
                            Ext.getCmp('uploadproductpic').add(new SelectImg({
                                isSelected: isDefault,
                                src: retVal.fileurl,
                                fileid: retVal.fileid
                            }));
                        }, CS.onError, UserID);
                    }
                }]
            }],
            buttonAlign: 'center',
            buttons: [
                //{
                //    text: '设为默认',
                //    handler: function () {
                //        Ext.MessageBox.confirm('确认', '是否设置该图片为默认图片？', function (btn) {
                //            if (btn == 'yes') {
                //                var selPics = Ext.getCmp('uploadproductpic').query('image[isSelected=true]');
                //                if (selPics.length > 0) {
                //                    CS('CZCLZ.YHGLClass.SetDefaultPicForProduct', function (retVal) {
                //                        if (retVal)
                //                            Ext.Msg.alert('提示', '设置成功！');
                //                        else
                //                            Ext.Msg.alert('提示', '设置失败！');
                //                    }, CS.onError, selPics[0].fileid, UserID);
                //                }
                //            }
                //        });
                //    }
                //},
                {
                    text: '删除',
                    handler: function () {
                        Ext.MessageBox.confirm('确认', '是否删除该图片？', function (btn) {
                            if (btn == 'yes') {
                                var selPics = Ext.getCmp('uploadproductpic').query('image[isSelected=true]');
                                if (selPics.length > 0) {
                                    CS('CZCLZ.YHGLClass.DelProductImageByPicID', function (retVal) {
                                        if (retVal) {
                                            Ext.getCmp('uploadproductpic').remove(selPics[0]);
                                        }
                                    }, CS.onError, selPics[0].fileid);
                                }
                            }
                        });
                    }
                }
            ]
        }];
        me.callParent(arguments);
    }
});

Ext.define('addWin', {
    extend: 'Ext.window.Window',

    height: 300,
    width: 400,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '用户管理',

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
                        id: 'UserID',
                        name: 'UserID',
                        labelWidth: 70,
                        hidden: true,
                        colspan: 2
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '用户名',
                        id: 'UserName',
                        name: 'UserName',
                        labelWidth: 70,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '密码',
                        id: 'Password',
                        name: 'Password',
                        labelWidth: 70,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '真实姓名',
                        id: 'UserXM',
                        name: 'UserXM',
                        labelWidth: 70,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '电话',
                        id: 'UserTel',
                        name: 'UserTel',
                        labelWidth: 70,
                        anchor: '100%'
                    },
                     {
                         xtype: 'combobox',
                         id: 'roleId',
                         name: 'roleId',
                         anchor: '100%',
                         fieldLabel: '角色',
                         allowBlank: false,
                         editable: false,
                         labelWidth: 70,
                         store: roleStore1,
                         queryMode: 'local',
                         displayField: 'roleName',
                         valueField: 'roleId',
                         value: ''
                     },
                     {
                         xtype: 'textareafield',
                         id: 'Address',
                         name: 'Address',
                         labelWidth: 70,
                         fieldLabel: '地址',
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
                                var me = this;
                                CS('CZCLZ.YHGLClass.SaveUser', function (retVal) {
                                    if (retVal) {
                                        me.up('window').close();
                                        getUser(1);
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
                            dataIndex: 'UserName',
                            sortable: false,
                            menuDisabled: true,
                            text: "登录名"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'roleName',
                            sortable: false,
                            menuDisabled: true,
                            text: "角色"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'UserXM',
                            sortable: false,
                            menuDisabled: true,
                            text: "姓名"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'UserTel',
                            sortable: false,
                            menuDisabled: true,
                            text: "电话"
                        },
                        {
                            text: '操作',
                            dataIndex: 'UserID',
                            width:120,
                            sortable: false,
                            menuDisabled: true,
                            renderer: function(value, cellmeta, record, rowIndex, columnIndex, store) {
                                var str;
                                str = "<a onclick='EditUser(\"" + value + "\");'>修改</a>　<a onclick='AddPhoto(\"" + value + "\");'>添加照片</a>";
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
                                    xtype: 'combobox',
                                    id: 'cx_role',
                                    width: 160,
                                    fieldLabel: '角色',
                                    editable: false,
                                    labelWidth: 40,
                                    store: roleStore,
                                    queryMode: 'local',
                                    displayField: 'roleName',
                                    valueField: 'roleId',
                                    value: ''
                                },
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
                                    fieldLabel: '真实姓名'
                                },
                                {
                                    xtype: 'buttongroup',
                                    title: '',
                                    items: [
                                        {
                                            xtype: 'button',
                                            iconCls: 'search',
                                            text: '查询',
                                            handler: function() {
                                                getUser(1);
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
                                            iconCls: 'add',
                                            text: '新增',
                                            handler: function () {
                                                CS('CZCLZ.YHGLClass.GetRole', function (retVal) {
                                                    if (retVal) {
                                                        roleStore1.loadData(retVal, false);
                                                        var win = new addWin();
                                                        win.show(null, function () {
                                                        });
                                                    }
                                                }, CS.onError);
                                               
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
                                            iconCls: 'delete',
                                            text: '删除',
                                            handler: function() {
                                                var idlist = [];
                                                var grid = Ext.getCmp("usergrid");
                                                var rds = grid.getSelectionModel().getSelection();
                                                if (rds.length == 0) {
                                                    Ext.Msg.show({
                                                        title: '提示',
                                                        msg: '请选择至少一条要删除的记录!',
                                                        buttons: Ext.MessageBox.OK,
                                                        icon: Ext.MessageBox.INFO
                                                    });
                                                    return;
                                                }

                                                Ext.MessageBox.confirm('删除提示', '是否要删除数据!', function(obj) {
                                                    if (obj == "yes") {
                                                        for (var n = 0, len = rds.length; n < len; n++) {
                                                            var rd = rds[n];

                                                            idlist.push(rd.get("UserID"));
                                                        }

                                                        CS('CZCLZ.YHGLClass.DelUser', function (retVal) {
                                                        if (retVal) {
                                                                getUser(1);
                                                            }
                                                        }, CS.onError, idlist);
                                                    }
                                                    else {
                                                        return;
                                                    }
                                                });



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

    CS('CZCLZ.YHGLClass.GetRole', function (retVal) {
        if (retVal) {
            roleStore.add([{ 'roleId': '', 'roleName': '全部角色' }]);
            roleStore.loadData(retVal, false);
            Ext.getCmp("cx_role").setValue('');
        }
    }, CS.onError, "");

    cx_role = Ext.getCmp("cx_role").getValue();
    cx_yhm = Ext.getCmp("cx_yhm").getValue();
    cx_xm = Ext.getCmp("cx_xm").getValue();

    getUser(1);

})
//************************************主界面*****************************************