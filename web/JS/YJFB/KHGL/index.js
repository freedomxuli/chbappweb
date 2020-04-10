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
       { name: 'userid' },
       { name: 'correlationid' },
       { name: 'account' },
       { name: 'password' },
       { name: 'usertype' },
       { name: 'dqbm' },
       { name: 'companycode' },
       { name: 'usertel' },
       { name: 'username' },
       { name: 'registeredcapital' },
       { name: 'unifiedsocialcreditldentifier' },
       { name: 'unifiedsocialdatetime' },
       { name: 'permitnumber' },
       { name: 'invoicerise' },
       { name: 'invoiceaddress' },
       { name: 'invoicetel' },
       { name: 'invoicenumber' },
       { name: 'invoicebank' },
              { name: 'ismonthly' },
                            { name: 'paychbrate' },

                             { name: 'accountrate' },
                              { name: 'completerate' },
                               { name: 'cashrate' },
                                 { name: 'nooilmoney' },
                                   { name: 'isneededit' },

                            { name: 'advancerate' },
                            { name: 'operaterate' },
                            { name: 'grossrate' },
                            { name: 'oilmoneyrate' },
                    { name: 'invoicerate' },

                            
              
       { name: 'invoicebanknumber' }

       
       
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

var sel_storeyewu = createSFW4Store({
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
              { name: 'UserXM' },

                     { name: 'UserTel' },

       { name: 'Password' },
       { name: 'roleId' },
       { name: 'roleName' },
       { name: 'UserXM' },
       { name: 'ClientKind' },
       { name: 'Address' },
       { name: 'UserTel' }
    ]
});

var Goel_store = createSFW4Store({
    data: [],
    pageSize: pageSize2,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'goodsid' },
         { name: 'goodsname' },
          { name: 'goodstype' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        GOel_getUser(nPage);
    }
});

var Gel_store = Ext.create('Ext.data.Store', {
    fields: [
                { name: 'goodsid' },
         { name: 'goodsname' },
          { name: 'goodstype' }
       
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
var fromAreaStore = Ext.create('Ext.data.Store', {
    fields: [
       { name: 'code', type: 'string' },
       { name: 'name', type: 'string' }
    ]
});
var fromAreaStore2 = Ext.create('Ext.data.Store', {
    fields: [
       { name: 'code', type: 'string' },
       { name: 'name', type: 'string' }
    ]
});
var fromAreaStore3 = Ext.create('Ext.data.Store', {
    fields: [
       { name: 'code', type: 'string' },
       { name: 'name', type: 'string' }
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

var Sdqstore = Ext.create('Ext.data.Store', {
    fields: [
         { name: 'id' },
       { name: 'province' },
         { name: 'city' },
       { name: 'area' },
       { name: 'bm' },
        { name: 'n1' },
         { name: 'n2' },
          { name: 'n3' },
    { name: 'address' }

    ]
});
//************************************数据源*****************************************

//************************************页面方法***************************************


function sel_getUser(nPage, roleId) {
    CS('CZCLZ.YHGLClass.GetUserListOnlyCZY', function (retVal) {
        sel_store.setData({
            data: retVal.dt,
            pageSize: pageSize2,
            total: retVal.ac,
            currentPage: retVal.cp
        });

    }, CS.onError, nPage, pageSize,roleId, Ext.getCmp("sel_yhm").getValue(), Ext.getCmp("sel_xm").getValue());
}
function selyewu_getUser(nPage, roleId) {
    CS('CZCLZ.YHGLClass.GetUserListYJFB', function (retVal) {
        sel_storeyewu.setData({
            data: retVal.dt,
            pageSize: pageSize2,
            total: retVal.ac,
            currentPage: retVal.cp
        });

    }, CS.onError, nPage, pageSize, roleId, Ext.getCmp("sel_yhm").getValue(), Ext.getCmp("sel_xm").getValue());
}





function Rel_getUser(usid, operatortype)
{
    CS('CZCLZ.UserMagByMySqlClass.GetGLYWY', function (retVal) {
        if (retVal) {
            Rel_store.loadData(retVal, false);
             
        }
    }, CS.onError, usid, operatortype);
}


function GOel_getUser(nPage, roleId) {
    CS('CZCLZ.UserMagByMySqlClass.GetGOODS', function (retVal) {
        Goel_store.setData({
            data: retVal.dt,
            pageSize: pageSize2,
            total: retVal.ac,
            currentPage: retVal.cp
        });

    }, CS.onError, nPage, pageSize,   Ext.getCmp("goodsname").getValue() );
}
function Gel_getUser(usid, operatortype) {
    CS('CZCLZ.UserMagByMySqlClass.GetGLGOODS', function (retVal) {
        if (retVal) {
            Gel_store.loadData(retVal, false);

        }
    }, CS.onError, usid, operatortype);
}

function Lel_getUser(usid) {
    CS('CZCLZ.UserMagByMySqlClass.GetGLLXR', function (retVal) {
        if (retVal) {
            Lel_store.loadData(retVal, false);

        }
    }, CS.onError, usid);
}


function getStartOREnd(usid, type) {
    Sdqstore.removeAll();
    if (type == 0) {
        CS('CZCLZ.UserMagByMySqlClass.GetStartDQ', function (retVals) {
            if (retVals.length > 0) {
                Sdqstore.loadData(retVals, false);

            }
        }, CS.onError, usid);
    } else {

        CS('CZCLZ.UserMagByMySqlClass.GetEndDQ', function (retVals) {
            if (retVals.length > 0) {
                Sdqstore.loadData(retVals, false);

            }
        }, CS.onError, usid);
    }





}


function getUser(nPage) {
    CS('CZCLZ.UserMagByMySqlClass.GetUserListByQY', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
        
    }, CS.onError, nPage, pageSize, "", Ext.getCmp("cx_yhm").getValue(), Ext.getCmp("areacode").getValue());
}

function EditUser(id) {
    var r = store.findRecord("userid", id).data;
    var win = new addWin();
    win.show(null, function () {
        win.setTitle("用户修改");
        var form = Ext.getCmp('addform');
        form.form.setValues(r);

        CS('CZCLZ.UserMagByMySqlClass.Getcorrelationid', function (retVal) {

            Ext.getCmp("FSHUserName").setValue(retVal);

        }, CS.onError, r.correlationid);
      

    });
     if (r.ismonthly == "0") {
        Ext.getCmp("ismonthly").setValue(true);
    } else {
        Ext.getCmp("ismonthly").setValue(false);
     }

     if (r.isneededit == "1") {
         Ext.getCmp("isneededit").setValue(true);
     } else {
         Ext.getCmp("isneededit").setValue(false);
     }

     CS('CZCLZ.UserMagByMySqlClass.GetAreaList', function (retVal) {
         if (retVal) {
             AreaStore2.removeAll();
              AreaStore2.loadData(retVal, true);
             Ext.getCmp("dqbm").setValue(r.dqbm);

         }
     }, CS.onError, r.dqbm.substr(0, 2) + "0000");
     Ext.getCmp("Sdqbm").setValue(r.dqbm.substr(0,2)+"0000");



}

function AddPhoto(v,f)
{
    var picItem = [];
    CS('CZCLZ.UserMagByMySqlClass.GetProductImages', function (retVal) {
         var result = retVal.evalJSON();
         var isDefault = false;
        
        if (result.data.length > 0) {
            Ext.getCmp('uploadproductpic').add(new SelectImg({
                isSelected: isDefault,
                src: result.data[0].photoUrl,
                fileid: result.data[0].fjId
            }));
        }
        
    }, CS.onError, v);

    var win = new phWin({ UserID: f, goodsid: v });
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
        var goodsid = me.goodsid;


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
                        Ext.getCmp('uploadproductpic').upload('CZCLZ.UserMagByMySqlClass.UploadPicForProduct', function (retVal) {
                            var isDefault = false;
                            if (retVal.isdefault == 1)
                                isDefault = true;
                            Ext.getCmp('uploadproductpic').add(new SelectImg({
                                isSelected: isDefault,
                                src: retVal.fileurl,
                                fileid: retVal.fileid
                            }));
                        }, CS.onError, UserID, goodsid);
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
                                    CS('CZCLZ.UserMagByMySqlClass.DelProductImageByPicID', function (retVal) {
                                        if (retVal) {
                                            Ext.getCmp('uploadproductpic').remove(selPics[0]);
                                        }
                                    }, CS.onError, selPics[0].fileid, UserID, goodsid);
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

    height: 400,
    width: 400,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '客户管理',

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'form',
                id: 'addform',
                autoScroll: true,
                bodyPadding: 10,
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
                        fieldLabel: '厂家名称',
                        id: 'username',
                        name: 'username',
                        labelWidth: 70,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '厂家标识',
                        id: 'companycode',
                        name: 'companycode',
                        labelWidth: 70,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '管理员账号',
                        id: 'account',
                        name: 'account',
                        labelWidth: 70,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '管理员密码',
                        id: 'password',
                        name: 'password',
                        labelWidth: 70,
                        allowBlank: false,
                        anchor: '100%'
                    },

                    {
                        xtype: 'textfield',
                        fieldLabel: '发票抬头',
                        id: 'invoicerise',
                        name: 'invoicerise',
                        labelWidth: 70,
                        allowBlank: false,
                        anchor: '100%'
                    },
                       {
                           xtype: 'textfield',
                           fieldLabel: '联系电话',
                           id: 'usertel',
                           name: 'usertel',
                           labelWidth: 70,
                           allowBlank: false,

                           anchor: '100%'
                       },
                    {
                        xtype: 'textfield',
                        fieldLabel: '地址',
                        id: 'invoiceaddress',
                        name: 'invoiceaddress',
                        labelWidth: 70,
                        allowBlank: false,

                        anchor: '100%'
                    },
                    {
                        xtype: 'numberfield',
                        fieldLabel: '帐期财务汇率（1-1.5）',
                        id: 'accountrate',
                        hidden:true,
                        name: 'accountrate',
                        labelWidth: 70,
                      //  allowBlank: false,
                        allowDecimals: true,    //是否允许小数
                        allowNegative: false,    //是否允许负数
                        maxValue: 1.5,
                        minValue: 1,
                        decimalPrecision: 2,    // 精确的位数

                        anchor: '100%'
                    },
                    {
                        xtype: 'numberfield',
                        fieldLabel: '综合成本汇率（0-1）',
                        id: 'completerate',
                        name: 'completerate',
                        labelWidth: 70,
                        allowBlank: false,
                        allowDecimals: true,    //是否允许小数
                        allowNegative: false,    //是否允许负数
                        maxValue: 1,
                        minValue: 0,
                        decimalPrecision: 2,    // 精确的位数

                        anchor: '100%'
                    },
                    {
                        xtype: 'numberfield',
                        fieldLabel: '押金成本',
                        id: 'cashrate',
                        name: 'cashrate',
                        labelWidth: 70,
                        hidden: true,

                       // allowBlank: false,
                        allowDecimals: true,    //是否允许小数
                        allowNegative: false,    //是否允许负数
                
                        decimalPrecision: 0,    // 精确的位数

                        anchor: '100%'
                    },
                 
                 {
                     xtype: 'numberfield',
                     fieldLabel: '垫资比例',
                     id: 'advancerate',
                     name: 'advancerate',
                     labelWidth: 70,
                     allowBlank: false,
                     allowDecimals: true,    //是否允许小数
                     allowNegative: false,    //是否允许负数
                     maxValue: 1,
                     minValue: 0,
                     decimalPrecision: 4,    // 精确的位数
                     value:0.0007,
                     anchor: '100%'
                 },
                 {
                     xtype: 'numberfield',
                     fieldLabel: '运营费比例',
                     id: 'operaterate',
                     name: 'operaterate',
                     labelWidth: 70,
                     allowBlank: false,
                     allowDecimals: true,    //是否允许小数
                     allowNegative: false,    //是否允许负数
                     maxValue: 1,
                     minValue: 0,
                     decimalPrecision: 2,    // 精确的位数
                     value: 0.05,
                     anchor: '100%'
                 },
                 {
                     xtype: 'numberfield',
                     fieldLabel: '开票比例',
                     id: 'invoicerate',
                     name: 'invoicerate',
                     labelWidth: 70,
                     allowBlank: false,
                     allowDecimals: true,    //是否允许小数
                     allowNegative: false,    //是否允许负数
                     maxValue: 1,
                     minValue: 0,
                     decimalPrecision: 2,    // 精确的位数
                     value: 0.05,
                     anchor: '100%'
                 },
                    {
                        xtype: 'numberfield',
                        fieldLabel: '毛利比例',
                        id: 'grossrate',
                        name: 'grossrate',
                        labelWidth: 70,
                        allowBlank: false,
                        allowDecimals: true,    //是否允许小数
                        allowNegative: false,    //是否允许负数
                        maxValue: 1,
                        minValue: 0,
                        decimalPrecision: 2,    // 精确的位数
                        value: 0.08,
                        anchor: '100%'
                    },
                    {
                        xtype: 'numberfield',
                        fieldLabel: '用油比例',
                        id: 'oilmoneyrate',
                        name: 'oilmoneyrate',
                        labelWidth: 70,
                        allowBlank: false,
                        allowDecimals: true,    //是否允许小数
                        allowNegative: false,    //是否允许负数
                        maxValue: 1,
                        minValue: 0,
                        decimalPrecision: 2,    // 精确的位数
                        value: 0.08,
                        anchor: '100%'
                    },


                    {
                        xtype: 'numberfield',
                        fieldLabel: '未用油汇率（0-1）',
                        id: 'nooilmoney',
                        name: 'nooilmoney',
                        labelWidth: 70,
                        hidden: true,

                       // allowBlank: false,
                        allowDecimals: true,    //是否允许小数
                        allowNegative: false,    //是否允许负数
                        maxValue: 1,
                        minValue: 0,
                        decimalPrecision: 2,    // 精确的位数

                        anchor: '100%'
                    },
                     {
                         xtype: 'textfield',
                         fieldLabel: '发票联系电话',
                         id: 'invoicetel',
                         name: 'invoicetel',
                         labelWidth: 70,
                         allowBlank: false,

                         anchor: '100%'
                     },
                      {
                          xtype: 'textfield',
                          fieldLabel: '纳税识别号',
                          id: 'invoicenumber',
                          name: 'invoicenumber',
                          labelWidth: 70,
                          allowBlank: false,

                          anchor: '100%'
                      },
                    {
                        xtype: 'textfield',
                        fieldLabel: '开户银行',
                        id: 'invoicebank',
                        name: 'invoicebank',
                        labelWidth: 70,
                        allowBlank: false,

                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '银行账号',
                        id: 'invoicebanknumber',
                        name: 'invoicebanknumber',
                        labelWidth: 70,
                        allowBlank: false,

                        anchor: '100%'
                    },
                     {
                         xtype: 'combobox',
                         id: 'Sdqbm',
                         name: 'Sdqbm',

                         fieldLabel: '省',
                         editable: false,
                         allowBlank: false,

                         labelWidth: 70,
                         store: AreaStore,
                         queryMode: 'local',
                         displayField: 'name',
                         valueField: 'code',
                         listeners: {
                             select: function (combo, record, opts) {
                                  CS('CZCLZ.UserMagByMySqlClass.GetAreaList', function (retVal) {
                                     if (retVal) {
                                         AreaStore2.removeAll();
                                           AreaStore2.loadData(retVal, true);
                                          Ext.getCmp("dqbm").setValue("");

                                      }
                                 }, CS.onError, record[0].get("code"));
                                  
                             }
                         }

                     }, {
                         xtype: 'combobox',
                         id: 'dqbm',
                         name: 'dqbm',

                         fieldLabel: '市区',
                         editable: false,
                         allowBlank: false,

                         labelWidth: 70,
                         store: AreaStore2,
                         queryMode: 'local',
                         displayField: 'name',
                         valueField: 'code'

                     },
                {
                    xtype: 'textfield',
                    fieldLabel: '注册资金(万)',
                    id: 'registeredcapital',
                    name: 'registeredcapital',
                    labelWidth: 70,
                    allowBlank: false,

                    anchor: '100%'
                },
                {
                    xtype: 'textfield',
                    fieldLabel: '统一社会信用代码',
                    id: 'unifiedsocialcreditldentifier',
                    name: 'unifiedsocialcreditldentifier',
                    labelWidth: 70,
                    allowBlank: false,

                    anchor: '100%'
                },
                {
                    xtype: 'datefield',
                    fieldLabel: '统一社会信用代码注册时间',
                    id: 'unifiedsocialdatetime',
                    name: 'unifiedsocialdatetime',
                    format: 'Y-m-d',

                    labelWidth: 70,
                    allowBlank: false,

                    anchor: '100%'
                },
                 {

                    xtype: "checkbox",

                    labelWidth: 70,

                    boxLabel: "月结客户",

                    name: "ismonthly",

                    id: "ismonthly",
 
                    allowBlank: false,
                     anchor: '100%'


                 },
                  {

                      xtype: "checkbox",

                      labelWidth: 70,

                      boxLabel: "是否无需修正",

                      name: "isneededit",

                      id: "isneededit",

                      allowBlank: false,
                      anchor: '100%',
                      value:1



                  },
                 {
                     xtype: 'textfield',
                     id: 'FSHUserName',
                     name: 'FSHUserName',
                     labelWidth: 70,
                      readOnly: true,
                     fieldLabel: '关联发啥货账户',
                     anchor: '100%'
                 },
                    {
                        xtype: 'displayfield',
                        fieldLabel: '',
                        value: '<a href=\'javascript:GetFSHZH()\' >抓取发啥货账户</a>',
                        columnWidth: 0.2,
                        editable: false
                    },
                    {
                        xtype: 'textfield',
                        id: 'correlationid',
                        name: 'correlationid',
                        labelWidth: 70,
                        hidden: true,
                         fieldLabel: '关联发啥货账户ID',
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
                                CS('CZCLZ.UserMagByMySqlClass.SaveQY', function (retVal) {
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
                            width: 150,
                            sortable: false,
                            menuDisabled: true,
                            text: "厂家名称"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'companycode',
                            sortable: false,
                            width: 80,
                            menuDisabled: true,
                            text: "厂家标识"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'account',
                            sortable: false,
                            width: 80,

                            menuDisabled: true,
                            text: "管理员账号"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'password',
                            sortable: false,
                            width: 80,
                            menuDisabled: true,
                            text: "管理员密码"
                        },
                           {
                               xtype: 'gridcolumn',
                               dataIndex: 'usertel',
                               sortable: false,
                               width: 80,

                               menuDisabled: true,
                               text: "联系电话"
                           },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'invoicerise',
                            sortable: false,
                            width: 150,

                            menuDisabled: true,
                            text: "发票抬头"
                        },
                         {
                             xtype: 'gridcolumn',
                             dataIndex: 'invoiceaddress',
                             sortable: false,
                             width: 150,

                             menuDisabled: true,
                             text: "地址"
                         },
                          {
                              xtype: 'gridcolumn',
                              dataIndex: 'invoicetel',
                              sortable: false,
                              width: 80,

                              menuDisabled: true,
                              text: "发票联系电话"
                          },
                           {
                               xtype: 'gridcolumn',
                               dataIndex: 'invoicenumber',
                               sortable: false,
                               width: 80,

                               menuDisabled: true,
                               text: "纳税识别号"
                           },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'invoicebank',
                                sortable: false,
                                width: 80,

                                menuDisabled: true,
                                text: "开户银行"
                            },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'invoicebanknumber',
                                 sortable: false,
                                 width: 150,

                                 menuDisabled: true,
                                 text: "银行账号"
                             },
                              {
                                  xtype: 'gridcolumn',
                                  dataIndex: 'dqbm',
                                  sortable: false,
                                  width: 80,

                                  menuDisabled: true,
                                  text: "所属地区"
                              },
                               {
                                   xtype: 'gridcolumn',
                                   dataIndex: 'registeredcapital',
                                   sortable: false,
                                   width: 100,

                                   menuDisabled: true,
                                   text: "注册资金（万）"
                               },
                                {
                                    xtype: 'gridcolumn',
                                    dataIndex: 'unifiedsocialcreditldentifier',
                                    sortable: false,
                                    width: 150,

                                    menuDisabled: true,
                                    text: "统一社会信用代码"
                                },
                                 {
                                     xtype: 'datecolumn',
                                     dataIndex: 'unifiedsocialdatetime',
                                     sortable: false,
                                     width: 150,
                                     format: 'Y-m-d H:i:s',

                                     menuDisabled: true,
                                     text: "统一社会信用代码注册时间"
                                 },
                                  {
                                      xtype: 'gridcolumn',
                                      dataIndex: 'ismonthly',
                                      sortable: false,
                                      width: 80,

                                      menuDisabled: true,
                                      text: "月结客户",
                                      renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                          if (value == "0")
                                          {
                                              return "是";
                                          } else {
                                              return "否";
                                          }
                                      }
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
                            text: '业务员',
                            dataIndex: 'userid',
                            width: 50,
                            sortable: false,
                            menuDisabled: true,
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                var str;
                                str = "<a onclick='ShowYWY(\"" + value + "\");'>查看</a>";//　<a onclick='AddPhoto(\"" + value + "\");'>添加照片</a>
                                return str;
                            }
                        },
                         {
                             text: '操作员',
                             dataIndex: 'userid',
                             width: 50,
                             sortable: false,
                             menuDisabled: true,
                             renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                 var str;
                                 str = "<a onclick='ShowCZY(\"" + value + "\");'>查看</a>";//　<a onclick='AddPhoto(\"" + value + "\");'>添加照片</a>
                                 return str;
                             }
                         },
                         {
                            text: '联系人',
                            dataIndex: 'userid',
                            width: 50,
                            sortable: false,
                            menuDisabled: true,
                            renderer: function(value, cellmeta, record, rowIndex, columnIndex, store) {
                                var str;
                                str = "<a onclick='ShowLXR(\"" + value + "\");'>查看</a>";//　<a onclick='AddPhoto(\"" + value + "\");'>添加照片</a>
                                return str;
                            }
                        },
                         {
                             text: '起始地',
                             dataIndex: 'userid',
                             width: 50,
                             sortable: false,
                             menuDisabled: true,
                             renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                 var str;
                                 str = "<a onclick='SetStart(\"" + value + "\",0);'>设置</a>";//　<a onclick='AddPhoto(\"" + value + "\");'>添加照片</a>
                                 return str;
                             }
                         },
                         {
                             text: '目的地',
                             dataIndex: 'userid',
                             width: 50,
                             sortable: false,
                             menuDisabled: true,
                             renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                 var str;
                                 str = "<a onclick='SetEnd(\"" + value + "\",1);'>设置</a>";//　<a onclick='AddPhoto(\"" + value + "\");'>添加照片</a>
                                 return str;
                             }
                         },
                          {
                              text: '常用货物',
                              dataIndex: 'userid',
                              width: 50,
                              sortable: false,
                              menuDisabled: true,
                              renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                  var str;
                                  str = "<a onclick='ShowHW(\"" + value + "\");'>查看</a>";//　<a onclick='AddPhoto(\"" + value + "\");'>添加照片</a>
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
                                    width: 160,
                                    labelWidth: 80,
                                    fieldLabel: '客户名称'
                                },
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
    Rel_getUser(usid,0);
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
                                            return "<div style='color:green;cursor:pointer;' onclick='DelYWY(\"" + value + "\",\"" + usid + "\",0)'>删除</div>";
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
                                               Rel_getUser(usid,0);
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
                        store: sel_storeyewu,
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
                                                    selyewu_getUser("", "4b43e3b3-6033-469d-941e-d0c5be48f19c");
                                                 }
                                            } 

                                        ]
                                    },
                                    {
                                        xtype: 'pagingtoolbar',
                                        displayInfo: true,
                                        store: sel_storeyewu,
                                        dock: 'bottom'
                                    }

                        ]

                    }



               ]
           }
        ]
    });
    c_window.show();
    selyewu_getUser("", "4b43e3b3-6033-469d-941e-d0c5be48f19c");

}

function DelYWY(id, usid, operatortype)
{
    Ext.MessageBox.confirm('删除提示', '是否要删除数据!', function (obj) {
        if (obj == "yes") {
            
            CS('CZCLZ.UserMagByMySqlClass.DelGLYWY', function (retVal) {
                if (retVal) {
                     Rel_getUser(usid, operatortype);
                }
            }, CS.onError, id);
        }
        else {
            return;
        }
    });
}

///操作员
function ShowCZY(usid) {
    Rel_getUser(usid,1);
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
        title: "操作员",
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
                                            return "<div style='color:green;cursor:pointer;' onclick='DelYWY(\"" + value + "\",\"" + usid + "\",1)'>删除</div>";
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
                                                     ShowAddCZ(usid);
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

function ShowAddCZ(usid) {
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
        title: "新增操作员",
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
                                               Rel_getUser(usid,1);
                                               Ext.getCmp("lmwin4").close();

                                           }
                                       }, CS.onError, idlist, usid,1);
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
                                        text: '操作员账号',
                                        flex: 1,
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    },
                                    {
                                        dataIndex: 'UserXM',
                                        flex: 1,
                                        text: '操作员名称',
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
                                                    sel_getUser("", "0c11bf4b-d1ca-4e91-b384-504c29991076");
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
    sel_getUser("", "0c11bf4b-d1ca-4e91-b384-504c29991076");

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

 


///起始地或者目的地
function SetStart(usid, type) {

   



    getStartOREnd(usid, type);
    var c_window = new Ext.Window({
        extend: 'Ext.window.Window',
        viewModel: {
            type: 'mywindow'
        },
        autoShow: true,
        height: 500,
        id: "lmwin88",
        width: 900,
        layout: 'fit',
        title: "起始地",
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
                              Ext.getCmp("lmwin88").close();
                          }
                      }
               ],
               items: [
                    {
                        xtype: 'gridpanel',
                        border: 1,
                        id: 'DeptsGrid88',

                        columnLines: true,
                        columnWidth: 1,
                        store: Sdqstore,

                        columns: [
                                Ext.create('Ext.grid.RowNumberer', { width: 40 }),

                                    {
                                        dataIndex: 'n1',
                                        text: '省',
                                        flex: 1,
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    },
                                    {
                                        dataIndex: 'n2',
                                        flex: 1,
                                        text: '市',
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    },
                                    {
                                        dataIndex: 'n3',
                                        flex: 1,
                                        text: '区',
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    },
                                    {
                                        dataIndex: 'bm',
                                        flex: 1,
                                        text: '编码',
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    },
                                    {
                                        dataIndex: 'address',
                                        flex: 1,
                                        text: '地址',
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
                                            return "<a style='color:green;cursor:pointer;' onclick='Delsdq(\"" + value + "\",\"" + usid + "\")'>删除</a>&nbsp;&nbsp;<a style='color:green;cursor:pointer;' onclick='SetStartAdd(\"" + usid + "\",\"" + value + "\")'>修改</a>";
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
                                                     SetStartAdd(usid, "");
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





function SetStartAdd(usid,id) {
    CS('CZCLZ.UserMagByMySqlClass.GetAreaList', function (retVal) {
        if (retVal) {
            fromAreaStore.removeAll();

             fromAreaStore.loadData(retVal, true);
            Ext.getCmp("province").setValue('');

            if (id != null && id != "") {

                CS('CZCLZ.UserMagByMySqlClass.GetStartDQInfor', function (retVals) {
                    if (retVals.length > 0) {

                        Ext.getCmp("province").setValue(retVals[0]["province"]);

                        CS('CZCLZ.UserMagByMySqlClass.GetAreaList', function (retVal) {
                            if (retVal) {
                                fromAreaStore2.removeAll();

                                 fromAreaStore2.loadData(retVal, true);
                                Ext.getCmp("city").setValue(retVals[0]["city"]);



                                CS('CZCLZ.UserMagByMySqlClass.GetAreaList', function (retVal) {
                                    if (retVal) {
                                        fromAreaStore3.removeAll();
                                        fromAreaStore3.loadData(retVal, true);
                                        Ext.getCmp("area").setValue(retVals[0]["area"]);

                                    }
                                }, CS.onError, retVals[0]["city"]);



                            }
                        }, CS.onError, retVals[0]["province"]);




                        Ext.getCmp("bm").setValue(retVals[0]["bm"]);
                        Ext.getCmp("address").setValue(retVals[0]["address"]);

                    }
                }, CS.onError, id);
            }



        }
    }, CS.onError, '000000');

 



    var f_window = new Ext.Window({
        extend: 'Ext.window.Window',
        viewModel: {
            type: 'mywindow'
        },
        autoShow: true,
        height: 250,
        id: "StartDQ",
        width: 500,
        layout: 'fit',
        title: "起始地点",
        modal: true,
        items: [
           {
               xtype: 'form',
               id: 'StartDQfrom',
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
                       xtype: 'combobox',
                       id: 'province',
                       name: 'province',
                        fieldLabel: '省',
                        editable: false,
                        allowBlank: false,

                       labelWidth: 70,
                       store: fromAreaStore,
                       queryMode: 'local',
                       displayField: 'name',
                       valueField: 'code',
                       anchor: '100%',
                       listeners: {
                           select: function (combo, record, opts) {

                               CS('CZCLZ.UserMagByMySqlClass.GetAreaList', function (retVal) {
                                   if (retVal) {
                                       fromAreaStore2.removeAll();
                                       fromAreaStore3.removeAll();

                                        fromAreaStore2.loadData(retVal, true);
                                       Ext.getCmp("city").setValue('');

                                   }
                               }, CS.onError, record[0].get("code"));

                             
                               
                           }
                       }

                   },
                   {

                       xtype: 'combobox',
                       id: 'city',
                       name: 'city',
                       fieldLabel: '市',
                       editable: false,
                       allowBlank: false,

                       labelWidth: 70,
                       store: fromAreaStore2,
                       queryMode: 'local',
                       displayField: 'name',
                       valueField: 'code',
                       anchor: '100%',
                       listeners: {
                           select: function (combo, record, opts) {
                               Ext.getCmp("bm").setValue(record[0].get("code"));

                               CS('CZCLZ.UserMagByMySqlClass.GetAreaList', function (retVal) {
                                   if (retVal) {
                                       fromAreaStore3.removeAll();
                                        fromAreaStore3.loadData(retVal, true);
                                       Ext.getCmp("area").setValue('');

                                   }
                               }, CS.onError, record[0].get("code"));



                           }
                       }

                   },
                   {
                       xtype: 'combobox',
                       id: 'area',
                       name: 'area',
                       fieldLabel: '区',
                       editable: false,
 
                       labelWidth: 70,
                       store: fromAreaStore3,
                       queryMode: 'local',
                       displayField: 'name',
                       valueField: 'code',
                       anchor: '100%',
                       listeners: {
                           select: function (combo, record, opts) {
                               Ext.getCmp("bm").setValue(record[0].get("code"));
                           }
                       }
                   },
                   {
                       xtype: 'textfield',
                       fieldLabel: '编码',
                       id: 'bm',
                       name: 'bm',
                       labelWidth: 70,
                       allowBlank: false,
                       anchor: '100%'
                   },
                   {
                       xtype: 'textfield',
                       fieldLabel: '地址',
                       id: 'address',
                       name: 'address',
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
                           var form = Ext.getCmp('StartDQfrom');
                           if (form.form.isValid()) {
                               var values = form.form.getValues(false);
                               var me = this;
                               CS('CZCLZ.UserMagByMySqlClass.SaveStartDQ', function (retVal) {
                                   if (retVal) {
                                       Ext.Msg.alert('提示', '添加成功！');

                                       getStartOREnd(usid,0);

                                        me.up('window').close();

                                   }
                               }, CS.onError, values, usid,id);

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
    f_window.show();

   


}

function Delsdq(id, usid)
{
    CS('CZCLZ.UserMagByMySqlClass.DelStartDQ', function (retVal) {
        if (retVal) {
            Ext.Msg.alert('提示', '删除成功！');
            getStartOREnd(usid,0);

        }
    }, CS.onError, id);
}


function Deledq(id, usid) {
    CS('CZCLZ.UserMagByMySqlClass.DelEndDQ', function (retVal) {
        if (retVal) {
            Ext.Msg.alert('提示', '删除成功！');
            getStartOREnd(usid, 1);

        }
    }, CS.onError, id);
}



///起始地或者目的地
function SetEnd(usid, type) {





    getStartOREnd(usid, type);
    var c_window = new Ext.Window({
        extend: 'Ext.window.Window',
        viewModel: {
            type: 'mywindow'
        },
        autoShow: true,
        height: 500,
        id: "lmwin88",
        width: 900,
        layout: 'fit',
        title: "目的地",
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
                              Ext.getCmp("lmwin88").close();
                          }
                      }
               ],
               items: [
                    {
                        xtype: 'gridpanel',
                        border: 1,
                        id: 'DeptsGrid88',

                        columnLines: true,
                        columnWidth: 1,
                        store: Sdqstore,

                        columns: [
                                Ext.create('Ext.grid.RowNumberer', { width: 40 }),

                                    {
                                        dataIndex: 'n1',
                                        text: '省',
                                        flex: 1,
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    },
                                    {
                                        dataIndex: 'n2',
                                        flex: 1,
                                        text: '市',
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    },
                                    {
                                        dataIndex: 'n3',
                                        flex: 1,
                                        text: '区',
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    },
                                    {
                                        dataIndex: 'bm',
                                        flex: 1,
                                        text: '编码',
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    },
                                    {
                                        dataIndex: 'address',
                                        flex: 1,
                                        text: '地址',
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
                                            return "<a style='color:green;cursor:pointer;' onclick='Deledq(\"" + value + "\",\"" + usid + "\")'>删除</a>&nbsp;&nbsp;<a style='color:green;cursor:pointer;' onclick='SetEndAdd(\"" + usid + "\",\"" + value + "\")'>修改</a>";
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
                                                     SetEndAdd(usid, "");
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


function SetEndAdd(usid,id) {
    CS('CZCLZ.UserMagByMySqlClass.GetAreaList', function (retVal) {
        if (retVal) {
            fromAreaStore.removeAll();

             fromAreaStore.loadData(retVal, true);
            Ext.getCmp("province").setValue('');

            if (id != null && id != "") {
                CS('CZCLZ.UserMagByMySqlClass.GetEndDQInfor', function (retVals) {
                    if (retVals.length > 0) {

                        Ext.getCmp("province").setValue(retVals[0]["province"]);

                        CS('CZCLZ.UserMagByMySqlClass.GetAreaList', function (retVal) {
                            if (retVal) {
                                fromAreaStore2.removeAll();

                                fromAreaStore2.loadData(retVal, true);
                                Ext.getCmp("city").setValue(retVals[0]["city"]);



                                CS('CZCLZ.UserMagByMySqlClass.GetAreaList', function (retVal) {
                                    if (retVal) {
                                        fromAreaStore3.removeAll();

                                        fromAreaStore3.loadData(retVal, true);
                                        Ext.getCmp("area").setValue(retVals[0]["area"]);

                                    }
                                }, CS.onError, retVals[0]["city"]);



                            }
                        }, CS.onError, retVals[0]["province"]);




                        Ext.getCmp("bm").setValue(retVals[0]["bm"]);
                        Ext.getCmp("address").setValue(retVals[0]["address"]);
                        Ext.getCmp("company").setValue(retVals[0]["company"]);
                        Ext.getCmp("person").setValue(retVals[0]["person"]);
                        Ext.getCmp("tel").setValue(retVals[0]["tel"]);

                    }
                }, CS.onError, id);
            }




        }
    }, CS.onError, '000000');





    var c_window = new Ext.Window({
        extend: 'Ext.window.Window',
        viewModel: {
            type: 'mywindow'
        },
        autoShow: true,
        height: 300,
        id: "EndDQ",
        width: 500,
        layout: 'fit',
        title: "目的地",
        modal: true,
        items: [
           {
               xtype: 'form',
               id: 'EndDQfrom',
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
                       xtype: 'combobox',
                       id: 'province',
                       name: 'province',
                       fieldLabel: '省',
                       editable: false,
                       allowBlank: false,

                       labelWidth: 70,
                       store: fromAreaStore,
                       queryMode: 'local',
                       displayField: 'name',
                       valueField: 'code',
                       anchor: '100%',
                       listeners: {
                           select: function (combo, record, opts) {

                               CS('CZCLZ.UserMagByMySqlClass.GetAreaList', function (retVal) {
                                   if (retVal) {
                                       fromAreaStore2.removeAll();
                                       fromAreaStore3.removeAll();

                                        fromAreaStore2.loadData(retVal, true);
                                       Ext.getCmp("city").setValue('');

                                   }
                               }, CS.onError, record[0].get("code"));



                           }
                       }

                   },
                   {

                       xtype: 'combobox',
                       id: 'city',
                       name: 'city',
                       fieldLabel: '市',
                       editable: false,
                       allowBlank: false,

                       labelWidth: 70,
                       store: fromAreaStore2,
                       queryMode: 'local',
                       displayField: 'name',
                       valueField: 'code',
                       anchor: '100%',
                       listeners: {
                           select: function (combo, record, opts) {
                               Ext.getCmp("bm").setValue(record[0].get("code"));

                               CS('CZCLZ.UserMagByMySqlClass.GetAreaList', function (retVal) {
                                   if (retVal) {
                                       fromAreaStore3.removeAll();
                                       fromAreaStore3.loadData(retVal, true);
                                       Ext.getCmp("area").setValue('');

                                   }
                               }, CS.onError, record[0].get("code"));



                           }
                       }

                   },
                   {
                       xtype: 'combobox',
                       id: 'area',
                       name: 'area',
                       fieldLabel: '区',
                       editable: false,

                       labelWidth: 70,
                       store: fromAreaStore3,
                       queryMode: 'local',
                       displayField: 'name',
                       valueField: 'code',
                       anchor: '100%',
                       listeners: {
                           select: function (combo, record, opts) {
                               Ext.getCmp("bm").setValue(record[0].get("code"));
                           }
                       }
                   },
                   {
                       xtype: 'textfield',
                       fieldLabel: '编码',
                       id: 'bm',
                       name: 'bm',
                       labelWidth: 70,
                       allowBlank: false,
                       anchor: '100%'
                   },
                   {
                       xtype: 'textfield',
                       fieldLabel: '地址',
                       id: 'address',
                       name: 'address',
                       labelWidth: 70,
                       allowBlank: false,
                       anchor: '100%'
                   },
                   {
                       xtype: 'textfield',
                       fieldLabel: '收货公司',
                       id: 'company',
                       name: 'company',
                       labelWidth: 70,
                       allowBlank: false,
                       anchor: '100%'
                   },
                   {
                       xtype: 'textfield',
                       fieldLabel: '收货人',
                       id: 'person',
                       name: 'person',
                       labelWidth: 70,
                       allowBlank: false,
                       anchor: '100%'
                   },
                   {
                       xtype: 'textfield',
                       fieldLabel: '联系方式',
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
                           var form = Ext.getCmp('EndDQfrom');
                           if (form.form.isValid()) {
                               var values = form.form.getValues(false);
                               var me = this;
                               CS('CZCLZ.UserMagByMySqlClass.SaveEndDQ', function (retVal) {
                                   if (retVal) {
                                       Ext.Msg.alert('提示', '添加成功！');

                                       getStartOREnd(usid, 1);

                                       me.up('window').close();

                                   }
                               }, CS.onError, values, usid,id);

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
 


}




///操作员
function ShowHW(usid) {
    Gel_getUser(usid);
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
        title: "常用货物",
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
                        store: Gel_store,

                        columns: [
                                Ext.create('Ext.grid.RowNumberer', { width: 40 }),

                                    {
                                        dataIndex: 'goodsname',
                                        text: '货物名称',
                                        flex: 1,
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    },
                                     
                                    {
                                        dataIndex: 'goodsid',
                                        hidden: true,
                                        text: '货物ID',
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    },
                                    {
                                        dataIndex: 'goodsid',
                                        flex: 1,
                                        text: '操作',
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true,
                                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                             return "<a style='color:green;' onclick='DelWH(\"" + value + "\",\"" + usid + "\",1)'>删除</a>&nbsp;&nbsp;<a onclick='AddPhoto(\"" + value + "\",\"" + usid + "\");'>添加图片</a>";
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
                                                     ShowAddWH(usid);
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

function ShowAddWH(usid) {
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
        title: "新增货物",
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

                                           idlist.push(rd.get("goodsid"));
                                       }

                                       CS('CZCLZ.UserMagByMySqlClass.SaveGOODSGL', function (retVal) {
                                           if (retVal) {
                                               Gel_getUser(usid);
                                               Ext.getCmp("lmwin4").close();

                                           }
                                       }, CS.onError, idlist, usid);
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
                        store: Goel_store,
                        selModel: Ext.create('Ext.selection.CheckboxModel', {

                        }),
                        columns: [
                                Ext.create('Ext.grid.RowNumberer', { width: 40 }),

                                    {
                                        dataIndex: 'goodsname',
                                        text: '货物名称',
                                        flex: 1,
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
                                                id: 'goodsname',
                                                width: 140,
                                                labelWidth: 50,
                                                fieldLabel: '名称'
                                            },
                                             
                                            {
                                                xtype: 'button',
                                                text: '查询',
                                                handler: function () {
                                                    GOel_getUser(1);
                                                }
                                            }

                                        ]
                                    },
                                    {
                                        xtype: 'pagingtoolbar',
                                        displayInfo: true,
                                        store: Goel_store,
                                        dock: 'bottom'
                                    }

                        ]

                    }



               ]
           }
        ]
    });
    c_window.show();
    GOel_getUser(1);

}


function DelWH(id, usid, operatortype) {
    Ext.MessageBox.confirm('删除提示', '是否要删除数据!', function (obj) {
        if (obj == "yes") {

            CS('CZCLZ.UserMagByMySqlClass.DelGLGOODS', function (retVal) {
                if (retVal) {
                    Gel_getUser(usid);
 
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
             AreaStore.loadData(retVal, true);
  
        }
    }, CS.onError, '000000');
 
}
