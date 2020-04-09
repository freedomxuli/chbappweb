inline_include("approot/r/weixin/js/qrcode/jquery-1.8.3.min.js");
inline_include("approot/r/weixin/js/qrcode/jquery.qrcode.js");
inline_include("approot/r/weixin/js/qrcode/qrcode.js");
inline_include("approot/r/weixin/js/qrcode/utf.js");

 
var pageSize = 15;
var pageSize2 = 15;

var cx_role;
var cx_yhm;
var areacode;

var mmck = false;
//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'username' },
        { name: 'correlationid' },
        { name: 'carrierregisteredaddress' },
        { name: 'businessscope' },
        { name: 'userid' },
        { name: 'dqbm' },
        { name: 'countrysubdivisioncode' },
        { name: 'carrierroute' },  
        { name: 'carriercontact' },  
        { name: 'carriercontact' },  
        { name: 'invoicerise' },  
        { name: 'invoiceaddress' },  
        { name: 'invoicetel' },  
        { name: 'invoicenumber' },  
        { name: 'invoicebank' },   
        { name: 'invoicebanknumber' },

           { name: 'vehicletype' },
              { name: 'vehiclelength' },
                 { name: 'vehicletime' },


        
    { name: 'vehicleclass' },

   { name: 'GENDER' },
   { name: 'identitydocumentnumber' },
    { name: 'mobiletelephonenumber' },
    { name: 'telephonenumber' },
    { name: 'validperiodfrom' },
    { name: 'validperiodto' },
    { name: 'updatetimedatetime' },
    { name: 'drivertype' },
    { name: 'driverbanktype' },
    { name: 'vehiclenumber' },
    { name: 'driverbanknumber' },
    { name: 'driverroutenumber' },
      
       { name: 'permitnumber' }
    ],
    onPageChange: function(sto, nPage, sorters) {
        getUser(nPage);
    }
});


var sel_store = createSFW4Store({
    data: [],
    pageSize: pageSize2,
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
    onPageChange: function (sto, nPage, sorters) {
        sel_getUser(nPage);
    }
});

var Rel_store = Ext.create('Ext.data.Store', {
    fields: [
                { name: 'ID' },

        { name: 'UserID' },
       { name: 'UserName' },
       { name: 'Password' },
       { name: 'roleId' },
       { name: 'roleName' },
       { name: 'UserXM' },
       { name: 'ClientKind' },
       { name: 'Address' },
       { name: 'UserTel' }
    ]
});

var Lel_store = Ext.create('Ext.data.Store', {
    fields: [
         { name: 'id' },
       { name: 'name' },
         { name: 'userid' },
       { name: 'tel' }
      
    ]
});


 


var FLD_store = Ext.create('Ext.data.Store', {
    fields: [
         { name: 'id' },
       { name: 'userid' },
         { name: 'pointsname' },
       { name: 'pointsuser' },
        { name: 'pointstel' },
         { name: 'pointsaddress' } 


    ]
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

var winstore = Ext.create('Ext.data.Store', {
    fields: [
        { name: 'UserID' },
           { name: 'UserName' },
       { name: 'UserXM' }
    ]
});

var AreaStore = Ext.create('Ext.data.Store', {
    fields: [
       { name: 'code', type: 'string' },
       { name: 'name', type: 'string' }
    ]
});
var AreaStore2 = Ext.create('Ext.data.Store', {
    fields: [
       { name: 'code', type: 'string' },
       { name: 'name', type: 'string' }
    ]
});


var CXSTORE = Ext.create('Ext.data.Store', {
    fields: [
       { name: 'id', type: 'string' },
       { name: 'name', type: 'string' }
    ]
});

var CCSTORE = Ext.create('Ext.data.Store', {
    fields: [
       { name: 'id', type: 'string' },
       { name: 'name', type: 'string' }
    ]
});

//************************************数据源*****************************************

//************************************页面方法***************************************


function sel_getUser(nPage) {
    CS('CZCLZ.YHGLClass.GetUserList', function (retVal) {
        sel_store.setData({
            data: retVal.dt,
            pageSize: pageSize2,
            total: retVal.ac,
            currentPage: retVal.cp
        });

    }, CS.onError, nPage, pageSize, "4b43e3b3-6033-469d-941e-d0c5be48f19c", Ext.getCmp("sel_yhm").getValue(), Ext.getCmp("sel_xm").getValue());
}



function GetBBBCX() {
    CS('CZCLZ.UserMagByMySqlClass.GetBBBCX', function (retVal) {
        if (retVal) {
            CXSTORE.loadData(retVal, false);

        }
    }, CS.onError);
}


function Rel_getUser(usid)
{
    CS('CZCLZ.UserMagByMySqlClass.GetGLYWY', function (retVal) {
        if (retVal) {
            Rel_store.loadData(retVal, false);
             
        }
    }, CS.onError, usid,0);
}

function Lel_getUser(usid) {
    CS('CZCLZ.UserMagByMySqlClass.GetGLLXR', function (retVal) {
        if (retVal) {
            Lel_store.loadData(retVal, false);

        }
    }, CS.onError, usid);
}

function FLD_getUser(usid) {
    CS('CZCLZ.UserMagByMySqlClass.GetGLFLD', function (retVal) {
        if (retVal) {
            FLD_store.loadData(retVal, false);

        }
    }, CS.onError, usid);
}

function getUser(nPage) {
    CS('CZCLZ.UserMagByMySqlClass.GetUserListSJ', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
        
    }, CS.onError, nPage, pageSize, "", Ext.getCmp("cx_yhm").getValue(), Ext.getCmp("cz_sjlx").getValue(), Ext.getCmp("cx_tel").getValue());
}

function EditUser(id) {
    var r = store.findRecord("userid", id).data;
    var win = new addWin();
    win.show(null, function () {
        win.setTitle("用户修改");
        var form = Ext.getCmp('addform');
       
         
        var bm = "";
        if (r.vehicletype == "3A9F0B1F-FAC3-4C00-908E-1F372E62A292") {
            bm = "015";
        } else {
            bm = "016";
        }


        CS('CZCLZ.UserMagByMySqlClass.GetBBBCC', function (retVal) {
            if (retVal) {
                CCSTORE.removeAll();
                CCSTORE.loadData(retVal, true);
                form.form.setValues(r);
            }
        }, CS.onError, bm);


    });


    
    
}

function AddPhoto(v)
{
    var picItem = [];
    CS('CZCLZ.YHGLClass.GetProductImages', function (retVal) {
        var result = retVal.evalJSON();
        //for (var i = 0; i < result.length; i++) {
        var isDefault = false;
        //    //if (result[i].ISDEFAULT == 1)
        //    //    isDefault = true;
        if (result.data.length > 0) {
            Ext.getCmp('uploadproductpic').add(new SelectImg({
                isSelected: isDefault,
                src: result.data[0].photoUrl,
                fileid: result.data[0].fjId
            }));
        }
    }, CS.onError, v);

    var win = new phWin({ UserID: v });
    win.show();
}

function LookEWM1(userid) {
    var win = new EWMWin1({ id: userid });
    win.show(null, function () {
        jQuery('#qrcodeTable1').qrcode({
            render: "table",
            text: "http://share.chahuobao.net/freight/html/htbd.html?userid=" + userid,
            width: "570",               //二维码的宽度
            height: "570",
        });
    });
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

    height: 500,
    width: 400,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '司机管理',

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'form',
                id: 'addform',
                bodyPadding: 10,
                autoScroll:true,
                items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: 'ID',
                        id: 'userid',
                        name: 'userid',
                        labelWidth: 70,
                        hidden: true,
                        colspan: 2
                    },
                    
                    {
                        xtype: 'textfield',
                        fieldLabel: '司机名称',
                        id: 'username',
                        name: 'username',
                        labelWidth: 70,
                        allowBlank: false,
                        anchor: '100%'
                    },

                     {

                         layout: 'column',
                         defaults: { anthor: '95%' },
                         border: 0,
                         items: [

                             {
                                 xtype: 'combobox',
                                 id: 'GENDER',
                                 name: 'GENDER',
                                 fieldLabel: '性别',
                                 editable: false,
                                 labelWidth: 70,
                                 store: Ext.create('Ext.data.Store', {
                                     fields: [
                                         { name: 'val' },
                                         { name: 'txt' }
                                     ],
                                     data: [

                                          { 'val': "1", 'txt': '男' },
                                         { 'val': "2", 'txt': '女' }
                                     ]


                                 }),
                                 queryMode: 'local',
                                 displayField: 'txt',
                                 valueField: 'val',
                                 anchor: '100%',
                                 columnWidth: .5,
                                 value: ''
                             }, {
                                 xtype: 'combobox',
                                 id: 'drivertype',
                                 name: 'drivertype',
                                 fieldLabel: '司机类型',
                                 editable: false,
                                 labelWidth: 70,
                                 store: Ext.create('Ext.data.Store', {
                                     fields: [
                                         { name: 'val' },
                                         { name: 'txt' }
                                     ],
                                     data: [

                                          { 'val': "0", 'txt': '零担' },
                                         { 'val': "1", 'txt': '整车' }
                                     ]


                                 }),
                                 queryMode: 'local',
                                 displayField: 'txt',
                                 valueField: 'val',
                                 anchor: '100%',
                                 columnWidth: .5,
                                 value: ''
                             }

                         ]

                     },


                    {
                        margin: '5 0 0 0',

                        xtype: 'textfield',
                        fieldLabel: '司机身份证',
                        id: 'identitydocumentnumber',
                        name: 'identitydocumentnumber',
                        labelWidth: 70,
                         anchor: '100%'
                    },
              
                    {
                        xtype: 'textfield',
                        fieldLabel: '移动电话号码',
                        id: 'mobiletelephonenumber',
                        name: 'mobiletelephonenumber',
                        hidden:true,
                        labelWidth: 70,
                       
                        anchor: '100%'

                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '电话号码',
                        id: 'telephonenumber',
                        name: 'telephonenumber',
                        labelWidth: 70,
                        allowBlank: false,
                        anchor: '100%'

                    },

                     {
                         xtype: 'combobox',
                         id: 'driverbanktype',
                         name: 'driverbanktype',
                         fieldLabel: '收款类型',
                         editable: false,
                         labelWidth: 70,
                         store: Ext.create('Ext.data.Store', {
                             fields: [
                                 { name: 'val' },
                                 { name: 'txt' }
                             ],
                             data: [
                                    { 'val': "0", 'txt': '银行卡' },
                                    { 'val': "1", 'txt': '支付宝' },
                                    { 'val': "2", 'txt': '微信' },
                                    { 'val': "3", 'txt': '现金' },
                                    { 'val': "4", 'txt': '其他' }
                             ]


                         }),
                         queryMode: 'local',
                         displayField: 'txt',
                         valueField: 'val',
                         anchor: '100%',
                         columnWidth: .5,
                         value: ''
                     },
                    
                    {
                        xtype: 'textfield',
                        fieldLabel: '收款账号',
                        margin:'5 0 0 0',
                        id: 'driverbanknumber',
                        name: 'driverbanknumber',
                        labelWidth: 70,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '道路运输许可证',
                        id: 'driverroutenumber',
                        name: 'driverroutenumber',
                        labelWidth: 70,
                        allowBlank: false,

                        anchor: '100%'
                    }, {
                        xtype: 'textfield',
                        fieldLabel: '车牌',
                        id: 'vehiclenumber',
                        name: 'vehiclenumber',
                        labelWidth: 70,
                        allowBlank: false,

                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '驾驶证',
                        id: 'vehicleclass',
                        name: 'vehicleclass',
                        labelWidth: 70,
 
                        anchor: '100%'
                    },
                      {
                          id: 'validperiodfrom',
                          name: 'validperiodfrom',
                          xtype: 'datefield',
                          fieldLabel: '驾驶证开始时间',
                          format: 'Y-m-d',
                          labelWidth: 70,
                          width: 210
                      }, {
                          id: 'validperiodto',
                          name: 'validperiodto',
                          xtype: 'datefield',
                          fieldLabel: '驾驶证结束时间',
                          format: 'Y-m-d',
                          labelWidth: 70,
                          width: 210
                      }, {
                          id: 'vehicletime',
                          name: 'vehicletime',
                          xtype: 'datefield',
                          fieldLabel: '报废期限',
                          format: 'Y-m-d',
                          labelWidth: 70,
                          width: 210
                      },
                      {
                          xtype: 'combobox',
                          id: 'vehicletype',
                          name: 'vehicletype',

                          fieldLabel: '车型',
                          editable: false,
                          allowBlank: false,

                          labelWidth: 70,
                          store: CXSTORE,
                          queryMode: 'local',
                          displayField: 'name',
                          valueField: 'id',
                          listeners: {
                              select: function (combo, record, opts) {

                                  var bm="";
                                  if( record[0].get("id")=="3A9F0B1F-FAC3-4C00-908E-1F372E62A292")
                                  {
                                      bm="015";
                                  }else{
                                      bm="016";
                                  }


                                  CS('CZCLZ.UserMagByMySqlClass.GetBBBCC', function (retVal) {
                                      if (retVal) {
                                          CCSTORE.removeAll();
                                          CCSTORE.loadData(retVal, true);
 
                                      }
                                  }, CS.onError, bm);

                              }
                          }
 
                      },
                      {
                          xtype: 'combobox',
                          id: 'vehiclelength',
                          name: 'vehiclelength',

                          fieldLabel: '车长',
                          editable: false,
                          allowBlank: false,

                          labelWidth: 70,
                          store: CCSTORE,
                          queryMode: 'local',
                          displayField: 'name',
                          valueField: 'id'
                          
 
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
                                CS('CZCLZ.UserMagByMySqlClass.SaveSJ', function (retVal) {
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

Ext.define('EWMWin1', {
    extend: 'Ext.window.Window',

    height: 670,
    width: 600,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '查看绑定二维码',

    initComponent: function () {
        var me = this;
        var id = me.id;
        me.items = [
            {
                xtype: 'panel',
                region: 'center',
                width: 150,
                html: '<table border="0" cellspacing="0" cellpadding="0" width="100%" style="margin-top:10px;">'
                   + ' <tr>'
                   + '   <td align="center"> <div id="qrcodeTable1"></div></td>'
                  + ' </tr>'
                  + '</table>',
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '下载',
                        handler: function () {
                            DownloadFile("CZCLZ.YHGLClass.GetEWMToFile", "二维码.jpg", id);
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
                            dataIndex: 'username',
                            flex:1,
                            sortable: false,
                            menuDisabled: true,
                            text: "司机名称"
                        },
                        
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'GENDER',
                            sortable: false,
                            flex: 1,

                            menuDisabled: true,
                            text: "司机性别",
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                if (value == "1") {
                                    return "男";
                                } else {
                                    return "女";
                                }
                            }
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'telephonenumber',
                            sortable: false,
                            flex: 1,

                            menuDisabled: true,
                            text: "司机电话",
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                return record.data.mobiletelephonenumber + "/" + record.data.telephonenumber;
                            }
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'driverbanktype',
                            sortable: false,
                            flex: 1,

                            menuDisabled: true,
                            text: "司机收款类型",
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                if (value == "0") {
                                    return "银行卡";
                                } else if (value == "1") {
                                    return "支付宝";
                                } if (value == "2") {
                                    return "微信";
                                } if (value == "3") {
                                    return "现金";
                                } if (value == "4") {
                                    return "其他";
                                }
                            }
                        },
                         {
                             xtype: 'gridcolumn',
                             dataIndex: 'driverbanknumber',
                             sortable: false,
                             flex: 1,

                             menuDisabled: true,
                             text: "司机收款账号"
                         },
                          {
                              xtype: 'gridcolumn',
                              dataIndex: 'vehicleclass',
                              sortable: false,
                              flex: 1,

                              menuDisabled: true,
                              text: "驾驶证号"
                          },
                           {
                               xtype: 'datecolumn',
                               format: 'Y-m-d H:i:s',
                               dataIndex: 'validperiodfrom',
                               sortable: false,
                               flex: 1,

                               menuDisabled: true,
                               text: "驾驶证开始时间"
                           },
                        {
                            xtype: 'datecolumn',
                            format: 'Y-m-d H:i:s',
                            dataIndex: 'validperiodto',
                            sortable: false,
                            flex: 1,

                            menuDisabled: true,
                            text: "驾驶证结束时间"
                        },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'driverroutenumber',
                                sortable: false,
                                flex: 1,

                                menuDisabled: true,
                                text: "道路许可证号"
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'vehiclenumber',
                                sortable: false,
                                flex: 1,

                                menuDisabled: true,
                                text: "车牌"
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'identitydocumentnumber',
                                sortable: false,
                                flex: 1,

                                menuDisabled: true,
                                text: "身份证"
                            },
                        {
                            text: '操作',
                            dataIndex: 'userid',
                            width: 50,
                            sortable: false,
                            menuDisabled: true,
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                var str;
                                str = "<a onclick='EditUser(\"" + value + "\");'>修改</a> ";
                                return str;
                            }
                        },
                         
                            {
                                dataIndex: 'userid',
                                flex: 1,
                                text: '道路许可证',
                                align: "center",
                                sortable: false,
                                menuDisabled: true,
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    return "<a onclick='AddPhoto(\"" + value + "\");'>添加图片</a>";
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
                                    width: 160,
                                    labelWidth: 80,
                                    fieldLabel: '司机名称'
                                },
                                {
                                    xtype: 'combobox',
                                    id: 'cz_sjlx',
                                    width: 180,
                                    fieldLabel: '司机类型',
                                    editable: false,
                                    labelWidth: 60,
                                    store: Ext.create('Ext.data.Store', {
                                        fields: [
                                            { name: 'val' },
                                            { name: 'txt' }
                                        ],
                                        data: [
                                            { 'val': "", 'txt': '全部' },
                                            { 'val': "0", 'txt': '零担' },
                                            { 'val': "1", 'txt': '整车' }
                                        ]


                                    }),
                                    queryMode: 'local',
                                    displayField: 'txt',
                                    valueField: 'val',
                                    value: ''
                                },
                                 {
                                     xtype: 'textfield',
                                     id: 'cx_tel',
                                     width: 160,
                                     labelWidth: 80,
                                     fieldLabel: '司机电话'
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

                                                            

                                                            if(suertype==1)
                                                            {
                                                                 Ext.getCmp("roleId").setValue('0c11bf4b-d1ca-4e91-b384-504c29991076');
                                                            }else{
                                                                Ext.getCmp("roleId").setValue('4b43e3b3-6033-469d-941e-d0c5be48f19c');

                                                            }
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

                                                            idlist.push(rd.get("userid"));
                                                        }

                                                        CS('CZCLZ.UserMagByMySqlClass.DelUserGYS', function (retVal) {
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

    getUser(1);
    GetArea();
    GetBBBCX();

})
//************************************主界面*****************************************


 

function GetFSHZH() {
     var c_window = new Ext.Window({
        extend: 'Ext.window.Window',
        viewModel: {
            type: 'mywindow'
        },
        autoShow: true,
        height: 500,
        id: "lmwin2",
        width: 500,
        layout: 'fit',
        title: "筛选发啥货账户",
        modal: true,
        items: [
           {
               xtype: 'panel',
               layout: 'column',
               bodyStyle: 'overflow-x:hidden;overflow-y:auto;',
               buttonAlign: 'center',
               layout: 'fit',
               buttons: [
                    
                      {
                          text: '关闭',
                          handler: function () {
                              Ext.getCmp("lmwin2").close();
                          }
                      }
               ],
               items: [
                    {
                        xtype: 'gridpanel',
                        border: 1,
                        id: 'DeptsGrid',

                        columnLines: true,
                        columnWidth: 1,
                        store: winstore,
                      
                        columns: [
                                Ext.create('Ext.grid.RowNumberer', { width: 40 }),

                                    {
                                        dataIndex: 'UserName',
                                        text: '账号',
                                        flex: 1,
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    },
                                    {
                                        dataIndex: 'UserXM',
                                        flex: 1,
                                        text: '账号名称',
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    },
                                    {
                                        dataIndex: 'UserID',
                                        hidden:true,
                                        text: '小区名称',
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    },
                                    {
                                        dataIndex: 'UserID',
                                        flex: 1,
                                        text: '操作',
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true,
                                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                            return "<div style='color:green;cursor:pointer;' onclick='CHK(\"" + record.data.UserName + "\",\"" + value + "\")'>选中</div>";
                                        }
                                    } 
                        ],
                        dockedItems: [
                                    {
                                        xtype: 'toolbar',
                                        dock: 'top',
                                        items: [
                                              {
                                                  xtype: 'textfield',
                                                  id: 'f_zh',
                                                  width: 160,
                                                  labelWidth: 80,
                                                  fieldLabel: '发啥货账号'
                                              },
                                              {
                                                  xtype: 'textfield',
                                                  id: 'f_xm',
                                                  width: 160,
                                                  labelWidth: 80,
                                                  fieldLabel: '账号名称'
                                              },

                                            {
                                                xtype: 'button',
                                                text: '查看',
                                                handler: function () {
                                                    SelInfor();

                                                }
                                            }

                                        ]
                                    }

                        ]

                    }



               ]
           }
        ]
    });
    c_window.show();


}

function SelInfor() {
     CS('CZCLZ.UserMagByMySqlClass.SelectFromUser', function (retVal) {
        if (retVal) {

            winstore.loadData(retVal);

        }
    }, CS.onError, Ext.getCmp("f_zh").getValue(), Ext.getCmp("f_xm").getValue())
}

function CHK(username, id)
{
    Ext.getCmp("FSHUserName").setValue(username);
    Ext.getCmp("correlationid").setValue(id);
    Ext.getCmp("lmwin2").close();
}

///业务员
function ShowYWY(usid) {
    Rel_getUser(usid);
    var c_window = new Ext.Window({
        extend: 'Ext.window.Window',
        viewModel: {
            type: 'mywindow'
        },
        autoShow: true,
        height: 500,
        id: "lmwin3",
        width: 500,
        layout: 'fit',
        title: "业务员",
        modal: true,
        items: [
           {
               xtype: 'panel',
               layout: 'column',
               bodyStyle: 'overflow-x:hidden;overflow-y:auto;',
               buttonAlign: 'center',
               layout: 'fit',
               buttons: [

                      {
                          text: '关闭',
                          handler: function () {
                              Ext.getCmp("lmwin3").close();
                          }
                      }
               ],
               items: [
                    {
                        xtype: 'gridpanel',
                        border: 1,
                        id: 'DeptsGrid3',

                        columnLines: true,
                        columnWidth: 1,
                        store: Rel_store,

                        columns: [
                                Ext.create('Ext.grid.RowNumberer', { width: 40 }),

                                    {
                                        dataIndex: 'UserName',
                                        text: '账号',
                                        flex: 1,
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    },
                                    {
                                        dataIndex: 'UserXM',
                                        flex: 1,
                                        text: '账号名称',
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    },
                                    {
                                        dataIndex: 'UserID',
                                        hidden: true,
                                        text: '小区名称',
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    },
                                    {
                                        dataIndex: 'id',
                                        flex: 1,
                                        text: '操作',
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true,
                                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                            return "<div style='color:green;cursor:pointer;' onclick='DelYWY(\"" + value + "\",\"" + record.data.UserID + "\")'>删除</div>";
                                        }
                                    }
                        ],
                        dockedItems: [
                                    {
                                        xtype: 'toolbar',
                                        dock: 'top',
                                        items: [
                                             {
                                                xtype: 'button',
                                                text: '新增',
                                                handler: function () {
                                                    ShowAddYW(usid);
                                                }
                                            }

                                        ]
                                    }

                        ]

                    }



               ]
           }
        ]
    });
    c_window.show();


}

function ShowAddYW(usid) {
    var c_window = new Ext.Window({
        extend: 'Ext.window.Window',
        viewModel: {
            type: 'mywindow'
        },
        autoShow: true,
        height: 500,
        id: "lmwin4",
        width: 500,
        layout: 'fit',
        title: "新增业务员",
        modal: true,
        items: [
           {
               xtype: 'panel',
               layout: 'column',
               bodyStyle: 'overflow-x:hidden;overflow-y:auto;',
               buttonAlign: 'center',
               layout: 'fit',
               buttons: [
                       {
                           text: '添加',
                           handler: function () {
                               var idlist = [];
                               var grid = Ext.getCmp("DeptsGrid4");
                               var rds = grid.getSelectionModel().getSelection();
                               if (rds.length == 0) {
                                   Ext.Msg.show({
                                       title: '提示',
                                       msg: '请选择至少一条要添加的记录!',
                                       buttons: Ext.MessageBox.OK,
                                       icon: Ext.MessageBox.INFO
                                   });
                                   return;
                               }

                               Ext.MessageBox.confirm('添加提示', '是否要添加数据!', function (obj) {
                                   if (obj == "yes") {
                                       for (var n = 0, len = rds.length; n < len; n++) {
                                           var rd = rds[n];

                                           idlist.push(rd.get("UserID"));
                                       }

                                       CS('CZCLZ.UserMagByMySqlClass.SaveYWYGL', function (retVal) {
                                           if (retVal) {
                                               Rel_getUser(usid);
                                               Ext.getCmp("lmwin4").close();

                                           }
                                       }, CS.onError, idlist, usid,0);
                                   }
                                   else {
                                       return;
                                   }
                               });
                           }
                       },
                      {
                          text: '关闭',
                          handler: function () {
                              Ext.getCmp("lmwin4").close();
                          }
                      }
               ],
               items: [
                    {
                        xtype: 'gridpanel',
                        border: 1,
                        id: 'DeptsGrid4',

                        columnLines: true,
                        columnWidth: 1,
                        store: sel_store,
                        selModel: Ext.create('Ext.selection.CheckboxModel', {

                        }),
                        columns: [
                                Ext.create('Ext.grid.RowNumberer', { width: 40 }),

                                    {
                                        dataIndex: 'UserName',
                                        text: '业务员账号',
                                        flex: 1,
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    },
                                    {
                                        dataIndex: 'UserXM',
                                        flex: 1,
                                        text: '业务员名称',
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    },
                                    {
                                        dataIndex: 'UserID',
                                        hidden: true,
                                        text: '',
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    } 
                        ],
                        dockedItems: [
                                    {
                                        xtype: 'toolbar',
                                        dock: 'top',
                                        items: [
                                              
                                            {
                                                xtype: 'textfield',
                                                id: 'sel_yhm',
                                                width: 140,
                                                labelWidth: 50,
                                                fieldLabel: '用户名'
                                            },
                                            {
                                                xtype: 'textfield',
                                                id: 'sel_xm',
                                                width: 160,
                                                labelWidth: 70,
                                                fieldLabel: '真实姓名'
                                            },
                                            {
                                                xtype: 'button',
                                                text: '查询',
                                                handler: function () {
                                                    sel_getUser();
                                                 }
                                            } 

                                        ]
                                    },
                                    {
                                        xtype: 'pagingtoolbar',
                                        displayInfo: true,
                                        store: sel_store,
                                        dock: 'bottom'
                                    }

                        ]

                    }



               ]
           }
        ]
    });
    c_window.show();
    sel_getUser();

}

function DelYWY(id,usid)
{
    Ext.MessageBox.confirm('删除提示', '是否要删除数据!', function (obj) {
        if (obj == "yes") {
            
            CS('CZCLZ.UserMagByMySqlClass.DelGLYWY', function (retVal) {
                if (retVal) {
                    Rel_getUser(usid);
                }
            }, CS.onError, id);
        }
        else {
            return;
        }
    });
}

///联系人
function ShowLXR(usid) {
    Lel_getUser(usid);
    var c_window = new Ext.Window({
        extend: 'Ext.window.Window',
        viewModel: {
            type: 'mywindow'
        },
        autoShow: true,
        height: 500,
        id: "lmwin33",
        width: 500,
        layout: 'fit',
        title: "联系人",
        modal: true,
        items: [
           {
               xtype: 'panel',
               layout: 'column',
               bodyStyle: 'overflow-x:hidden;overflow-y:auto;',
               buttonAlign: 'center',
               layout: 'fit',
               buttons: [

                      {
                          text: '关闭',
                          handler: function () {
                              Ext.getCmp("lmwin33").close();
                          }
                      }
               ],
               items: [
                    {
                        xtype: 'gridpanel',
                        border: 1,
                        id: 'DeptsGrid33',

                        columnLines: true,
                        columnWidth: 1,
                        store: Lel_store,

                        columns: [
                                Ext.create('Ext.grid.RowNumberer', { width: 40 }),

                                    {
                                        dataIndex: 'name',
                                        text: '名称',
                                        flex: 1,
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    },
                                    {
                                        dataIndex: 'tel',
                                        flex: 1,
                                        text: '电话',
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    },
                                   
                                    {
                                        dataIndex: 'id',
                                        flex: 1,
                                        text: '操作',
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true,
                                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                            return "<a style='color:green;cursor:pointer;' onclick='DelLXR(\"" + value + "\",\"" + record.data.userid + "\")'>删除</a>&nbsp;&nbsp;<a style='color:green;cursor:pointer;' onclick='ShowAddLXR(\"" + record.data.userid + "\",2,\"" + value + "\")'>修改</a>";
                                        }
                                    }
                        ],
                        dockedItems: [
                                    {
                                        xtype: 'toolbar',
                                        dock: 'top',
                                        items: [
                                             {
                                                 xtype: 'button',
                                                 text: '新增',
                                                 handler: function () {
                                                     ShowAddLXR(usid,1,"");
                                                 }
                                             }

                                        ]
                                    }

                        ]

                    }



               ]
           }
        ]
    });
    c_window.show();


}


 


function ShowAddLXR(usid,type,id) {
    var title = "新增联系人";
    if (type == 2)
    {
        title = "修改联系人";
      
    }


    var c_window = new Ext.Window({
        extend: 'Ext.window.Window',
        viewModel: {
            type: 'mywindow'
        },
        autoShow: true,
        height: 150,
        id: "lmwin44",
        width: 500,
        layout: 'fit',
        title: title,
        modal: true,
        items: [
           {
               xtype: 'form',
               id: 'addform44',
               bodyPadding: 10,
               items: [
                   {
                       xtype: 'textfield',
                       fieldLabel: 'ID',
                       id: 'id',
                       name: 'id',
                       labelWidth: 70,
                       hidden: true,
                       colspan: 2
                   },
                   {
                       xtype: 'textfield',
                       fieldLabel: '名称',
                       id: 'name',
                       name: 'name',
                       labelWidth: 70,
                       allowBlank: false,
                       anchor: '100%'
                   },
                   {
                       xtype: 'textfield',
                       fieldLabel: '电话',
                       id: 'tel',
                       name: 'tel',
                       labelWidth: 70,
                       allowBlank: false,
                       anchor: '100%'
                   }
                   
               ],
               buttonAlign: 'center',
               buttons: [
                   {
                       text: '保存',
                       iconCls: 'dropyes',
                       handler: function () {
                           var form = Ext.getCmp('addform44');
                           if (form.form.isValid()) {
                               var values = form.form.getValues(false);
                               var me = this;
                               CS('CZCLZ.UserMagByMySqlClass.SaveLXR', function (retVal) {
                                   if (retVal) {
                                       Lel_getUser(usid);
                                       me.up('window').close();

                                   }
                               }, CS.onError, values, usid);

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
        ]
    });
    c_window.show();

    if (type == 2) {
      
        var r = Lel_store.findRecord("id", id).data;

        var form = Ext.getCmp('addform44');
        form.form.setValues(r);
    }


}

function DelLXR(id, usid) {
    Ext.MessageBox.confirm('删除提示', '是否要删除数据!', function (obj) {
        if (obj == "yes") {

            CS('CZCLZ.UserMagByMySqlClass.DelGLLXR', function (retVal) {
                if (retVal) {
                    Lel_getUser(usid);
                }
            }, CS.onError, id);
        }
        else {
            return;
        }
    });
}


//生成地区方法
function GetArea() {
    CS('CZCLZ.UserMagByMySqlClass.GetAreaList', function (retVal) {
        if (retVal) {
            AreaStore.add([{ 'code': '', 'name': '全部' }]);
            AreaStore.loadData(retVal, true);

        }
    }, CS.onError, '000000');

}




///分流点
function ShowFLD(usid) {
    FLD_getUser(usid);
    var c_window = new Ext.Window({
        extend: 'Ext.window.Window',
        viewModel: {
            type: 'mywindow'
        },
        autoShow: true,
        height: 500,
        id: "lmwin33",
        width: 700,
        layout: 'fit',
        title: "分流点",
        modal: true,
        items: [
           {
               xtype: 'panel',
               layout: 'column',
               bodyStyle: 'overflow-x:hidden;overflow-y:auto;',
               buttonAlign: 'center',
               layout: 'fit',
               buttons: [

                      {
                          text: '关闭',
                          handler: function () {
                              Ext.getCmp("lmwin33").close();
                          }
                      }
               ],
               items: [
                    {
                        xtype: 'gridpanel',
                        border: 1,
                        id: 'DeptsGrid33',

                        columnLines: true,
                        columnWidth: 1,
                        store: FLD_store,

                        columns: [
                                Ext.create('Ext.grid.RowNumberer', { width: 40 }),

                                    {
                                        dataIndex: 'pointsname',
                                        text: '分流点名称',
                                        flex: 1,
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    },
                                    {
                                        dataIndex: 'pointsuser',
                                        flex: 1,
                                        text: '分流点负责人姓名',
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    },
                                      {
                                          dataIndex: 'pointstel',
                                          flex: 1,
                                          text: '分流点负责人电话',
                                          align: "center",
                                          sortable: false,
                                          menuDisabled: true
                                      },
                                        {
                                            dataIndex: 'pointsaddress',
                                            flex: 1,
                                            text: '分流点地址',
                                            align: "center",
                                            sortable: false,
                                            menuDisabled: true
                                        },

                                    {
                                        dataIndex: 'id',
                                        flex: 1,
                                        text: '操作',
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true,
                                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                            return "<a style='color:green;cursor:pointer;' onclick='DelFLD(\"" + value + "\",\"" + record.data.userid + "\")'>删除</a>&nbsp;&nbsp;<a style='color:green;cursor:pointer;' onclick='ShowAddFLD(\"" + record.data.userid + "\",2,\"" + value + "\")'>修改</a>";
                                        }
                                    }
                        ],
                        dockedItems: [
                                    {
                                        xtype: 'toolbar',
                                        dock: 'top',
                                        items: [
                                             {
                                                 xtype: 'button',
                                                 text: '新增',
                                                 handler: function () {
                                                     ShowAddFLD(usid, 1, "");
                                                 }
                                             }

                                        ]
                                    }

                        ]

                    }



               ]
           }
        ]
    });
    c_window.show();


}





function ShowAddFLD(usid, type, id) {
    var title = "新增分流点";
    if (type == 2) {
        title = "修改分流点";

    }


    var c_window = new Ext.Window({
        extend: 'Ext.window.Window',
        viewModel: {
            type: 'mywindow'
        },
        autoShow: true,
        height: 200,
        id: "lmwin44",
        width: 500,
        layout: 'fit',
        title: title,
        modal: true,
        items: [
           {
               xtype: 'form',
               id: 'addform44',
               bodyPadding: 10,
               items: [
                   {
                       xtype: 'textfield',
                       fieldLabel: 'ID',
                       id: 'id',
                       name: 'id',
                       labelWidth: 70,
                       hidden: true,
                       colspan: 2
                   },
                   {
                       xtype: 'textfield',
                       fieldLabel: '分流点名称',
                       id: 'pointsname',
                       name: 'pointsname',
                       labelWidth: 70,
                       allowBlank: false,
                       anchor: '100%'
                   },
                   {
                       xtype: 'textfield',
                       fieldLabel: '分流点负责人姓名',
                       id: 'pointsuser',
                       name: 'pointsuser',
                       labelWidth: 70,
                       allowBlank: false,
                       anchor: '100%'
                   },
                   {
                       xtype: 'textfield',
                       fieldLabel: '分流点负责人电话',
                       id: 'pointstel',
                       name: 'pointstel',
                       labelWidth: 70,
                       allowBlank: false,
                       anchor: '100%'
                   },
                   {
                       xtype: 'textfield',
                       fieldLabel: '分流点地址',
                       id: 'pointsaddress',
                       name: 'pointsaddress',
                       labelWidth: 70,
                       allowBlank: false,
                       anchor: '100%'
                   }

               ],
               buttonAlign: 'center',
               buttons: [
                   {
                       text: '保存',
                       iconCls: 'dropyes',
                       handler: function () {
                           var form = Ext.getCmp('addform44');
                           if (form.form.isValid()) {
                               var values = form.form.getValues(false);
                               var me = this;
                               CS('CZCLZ.UserMagByMySqlClass.SaveFLD', function (retVal) {
                                   if (retVal) {
                                       FLD_getUser(usid);
                                       me.up('window').close();

                                   }
                               }, CS.onError, values, usid);

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
        ]
    });
    c_window.show();

    if (type == 2) {

        var r = FLD_store.findRecord("id", id).data;

        var form = Ext.getCmp('addform44');
        form.form.setValues(r);
    }


}

function DelFLD(id, usid) {
    Ext.MessageBox.confirm('删除提示', '是否要删除数据!', function (obj) {
        if (obj == "yes") {

            CS('CZCLZ.UserMagByMySqlClass.DelGLFLD', function (retVal) {
                if (retVal) {
                    FLD_getUser(usid);
                }
            }, CS.onError, id);
        }
        else {
            return;
        }
    });
}

