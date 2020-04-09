
var pageSize = 15;
var orderstore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [



    { name: 'shippingnoteid' },
         { name: 'userid' },
           { name: 'username' },
            { name: 'verifymoney' },
            { name: 'offerid' },
            { name: 'descriptionofgoods' },

            { name: 'money' },
        { name: 'hxmoney' },
       { name: 'shippingnoteadddatetime' },
       { name: 'shippingnotenumber' },
       { name: 'statisticstype' },
       { name: 'totalvaloremtax' },
       { name: 'rate' },
       { name: 'billingtime' },
       { name: 'invoicecode' },
        { name: 'totalamount' },
          { name: 'actualmoney' },
                    { name: 'actualdrivermoney' },

       { name: 'invoicenumber' }



    ],
    onPageChange: function (sto, nPage, sorters) {
        getUser(nPage);
    }
});
var orderstore2 = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [


    { name: 'billingid' },

    { name: 'shippingnoteid' },
         { name: 'userid' },

       { name: 'shippingnoteadddatetime' },
       { name: 'shippingnotenumber' },
       { name: 'statisticstype' },
       { name: 'totalvaloremtax' },
       { name: 'rate' },
       { name: 'billingtime' },
       { name: 'invoicecode' },
        { name: 'totalamount' },
          { name: 'actualmoney' },

       { name: 'invoicenumber' }



    ],
    onPageChange: function (sto, nPage, sorters) {
        getUser2(nPage);
    }
});

var orderstore3 = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [

     { name: 'billingid' },
     { name: 'userid' },
    { name: 'shippingnoteid' },

       { name: 'shippingnoteadddatetime' },
       { name: 'shippingnotenumber' },
       { name: 'statisticstype' },
       { name: 'totalvaloremtax' },
       { name: 'rate' },
       { name: 'billingtime' },
       { name: 'invoicecode' },
        { name: 'totalamount' },
        { name: 'actualmoney' },
       




       { name: 'invoicenumber' }



    ],
    onPageChange: function (sto, nPage, sorters) {
        getUser3(nPage);
    }
});
function getUser(nPage) {
    CS('CZCLZ.SJHXByMySql.GYSHXList', function (retVal) {
        orderstore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });

    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), Ext.getCmp("cx_name").getValue(), Ext.getCmp("cx_ddbm").getValue(), Ext.getCmp("cx_ishx").getValue());
}

Ext.onReady(function () {
    Ext.define('ConsumeView', {
        extend: 'Ext.container.Viewport',

        layout: {
            type: 'fit'
        },

        initComponent: function () {
            var me = this;

            Ext.applyIf(me, {
                items: [
                    {
                        xtype: 'tabpanel',
                        activeTab: 0,

                        items: [
                            {
                                xtype: 'panel',
                                layout: {
                                    type: 'fit'
                                },
                                title: '司机核销',
                                items: [
                                    {
                                        xtype: 'gridpanel',
                                        id: 'usergrid',
                                        border: 1,
                                        columnLines: 1,
                                        store: orderstore,
                                        selModel: Ext.create('Ext.selection.CheckboxModel', {
                                        }),
                                        columns: [
                                            {
                                                xtype: 'datecolumn',
                                                format: 'Y-m-d H:i:s',
                                                dataIndex: 'shippingnoteadddatetime',
                                                sortable: false,
                                                menuDisabled: true,
                                                flex: 1,
                                                text: '订单时间'
                                            },
                                              {
                                                  xtype: 'gridcolumn',
                                                  dataIndex: 'shippingnotenumber',
                                                  sortable: false,
                                                  menuDisabled: true,
                                                  flex: 1,
                                                  text: '单号',
                                                  renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {

                                                      return "<input value='" + value + "' style='border:0px;BACKGROUND-COLOR: transparent;'>";
                                                  }
                                              },
                                                {
                                                    xtype: 'gridcolumn',
                                                    dataIndex: 'descriptionofgoods',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    flex: 1,
                                                    text: '货名'
                                                }, {
                                                    xtype: 'gridcolumn',
                                                    dataIndex: 'username',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    flex: 1,
                                                    text: '司机名称'
                                                }, {
                                                    xtype: 'gridcolumn',
                                                    dataIndex: 'actualdrivermoney',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    flex: 1,
                                                    text: '司机金额'
                                                }, {
                                                    xtype: 'gridcolumn',
                                                    dataIndex: 'money',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    flex: 1,
                                                    text: '司机总付款'
                                                }, {
                                                    xtype: 'gridcolumn',
                                                    dataIndex: 'verifymoney',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    flex: 1,
                                                    text: '已核销金额'
                                                }, {
                                                    xtype: 'gridcolumn',
                                                    dataIndex: 'shippingnoteid',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    width: 100,
                                                    text: '剩余可核销金额',
                                                    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                                        return record.data.actualdrivermoney - record.data.verifymoney;
                                                    }
                                                }, {
                                                    xtype: 'gridcolumn',
                                                    dataIndex: 'shippingnoteid',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    width: 100,
                                                    text: '开票信息',
                                                    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                                        return "<div style='color:green;cursor:pointer;' onclick='LookP(\"" + record.data.userid + "\",\"" + record.data.actualmoney + "\")'>查看</div>";
                                                    }
                                                }, {
                                                    xtype: 'gridcolumn',
                                                    dataIndex: 'shippingnoteid',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    width: 100,
                                                    text: '订单信息',
                                                    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                                        return "<div style='color:green;cursor:pointer;' onclick='LookDD(\"" + record.data.shippingnotenumber + "\",\"" + record.data.actualmoney + "\")'>查看</div>";
                                                    }
                                                },
                                                 {
                                                    xtype: 'gridcolumn',
                                                    dataIndex: 'shippingnoteid',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    width: 100,
                                                    text: '核销记录',
                                                    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                                        return "<div style='color:green;cursor:pointer;' onclick='CKHXJL(\"" + value + "\",\"" + record.data.username + "\",\"" + record.data.actualdrivermoney + "\",\"" + record.data.userid + "\")'>查看</div>"; 
                                                    }
                                                 },
                                                 {
                                                    xtype: 'gridcolumn',
                                                    dataIndex: 'shippingnoteid',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    width: 100,
                                                    text: '操作',
                                                    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                                        return "<div style='color:green;cursor:pointer;' onclick='AddPJ(\"" + value + "\",\"" + record.data.username + "\",\"" + record.data.actualdrivermoney + "\",\"" + record.data.userid + "\",\"" + record.data.verifymoney + "\",\"" + record.data.offerid + "\")'>单独核销</div>";
                                                    }
                                                }

                                        ],
                                        dockedItems: [
                                            {
                                                xtype: 'toolbar',
                                                dock: 'top',
                                                items: [
                                                    {
                                                        id: 'cx_beg',
                                                        xtype: 'datefield',
                                                        fieldLabel: '订单时间',
                                                        format: 'Y-m-d',
                                                        labelWidth: 70,
                                                        width: 210
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
                                                        xtype: 'textfield',
                                                        id: 'cx_name',
                                                        width: 160,
                                                        labelWidth: 70,
                                                        fieldLabel: '司机名称'
                                                    },
                                                      {
                                                          xtype: 'textfield',
                                                          id: 'cx_ddbm',
                                                          width: 160,
                                                          labelWidth: 70,
                                                          fieldLabel: '订单号'
                                                      },
                                                       {
                                                           xtype: 'combobox',
                                                           id: 'cx_ishx',
                                                           name: 'cx_ishx',
                                                           fieldLabel: '核销状态',
                                                           editable: false,
                                                           labelWidth: 90,
                                                           store: Ext.create('Ext.data.Store', {
                                                               fields: [
                                                                   { name: 'val' },
                                                                   { name: 'txt' }
                                                               ],
                                                               data: [
                                                                    { 'val': "", 'txt': '全部' },
                                                                   { 'val': "0", 'txt': '未核销' },
                                                                   { 'val': "1", 'txt': '部分核销' },
                                                                   { 'val': "2", 'txt': '完整核销' }
                                                               ]


                                                           }),
                                                           queryMode: 'local',
                                                           displayField: 'txt',
                                                           valueField: 'val',
                                                           anchor: '100%',

                                                           value: ''
                                                       },
                                                    {
                                                        xtype: 'button',
                                                        iconCls: 'search',
                                                        text: '查询',
                                                        handler: function () {
                                                            getUser(1);
                                                        }
                                                    },
                                                    {
                                                        xtype: 'button',
                                                        iconCls: 'search',
                                                        text: '一键核销',
                                                        handler: function () {
                                                            var idlist = [];
                                                            var grid = Ext.getCmp("usergrid");
                                                            var rds = grid.getSelectionModel().getSelection();
                                                            if (rds.length == 0) {
                                                                Ext.Msg.show({
                                                                    title: '提示',
                                                                    msg: '请选择至少一条要核销的记录!',
                                                                    buttons: Ext.MessageBox.OK,
                                                                    icon: Ext.MessageBox.INFO
                                                                });
                                                                return;
                                                            }

                                                            var gys = "";
                                                            var actualdrivermoney = 0;
                                                            for (var n = 0, len = rds.length; n < len; n++) {
                                                                var rd = rds[n];
                                                                gys += rd.get("username") + ",";
                                                                idlist.push(rd.get("shippingnoteid") + "," + rd.get("userid") + "," + rd.get("offerid") + "," + rd.get("actualdrivermoney") + "," + rd.get("verifymoney") + "," + rd.get("username"));
                                                               
                                                                actualdrivermoney += (rd.get("actualdrivermoney") - rd.get("verifymoney"))
                                                            }
                                                             
                                                            AddPJ2(idlist, gys, actualdrivermoney);
                                                        }
                                                    }
                                                ]
                                            },
                                            {
                                                xtype: 'pagingtoolbar',
                                                dock: 'bottom',
                                                width: 360,
                                                store: orderstore,
                                                displayInfo: true
                                            }
                                        ]
                                    }
                                ]
                            },
                            
                            
                        ]
                    }
                ]
            });

            me.callParent(arguments);
        }

    });

    new ConsumeView();
    getUser(1);
  


});


function AddPJ2(idlist, gys, actualdrivermoney) {
    if (actualdrivermoney != null && actualdrivermoney != "" && actualdrivermoney != "null") {

    } else {
        actualdrivermoney = 0;
    }
  

    var c_window = new Ext.Window({
        extend: 'Ext.window.Window',
        viewModel: {
            type: 'mywindow'
        },
        autoShow: true,
        height: 300,
        id: "lmwin44",
        width: 450,
        layout: 'fit',
        title: '一键核销',
        modal: true,
        items: [
           {
               xtype: 'form',
               id: 'addform44',
               bodyPadding: 10,
               items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: '企业名称',
                        id: 'username',
                        name: 'username',
                        labelWidth: 90,
                        allowBlank: false,
                        anchor: '100%',
                        readOnly: true,
                        value: gys
                    },
                     {
                         xtype: 'numberfield',
                         fieldLabel: '核销总金额',
                         id: 'verifymoney',
                         name: 'verifymoney',
                         decimalPrecision: 5,
                         minValue: 0,
                         maxValue: actualdrivermoney,
                         readOnly: true,
                         labelWidth: 90,
                         allowBlank: false,
                         value:actualdrivermoney,
                         anchor: '100%'
                     },
                    
                     {
                         xtype: 'combobox',
                         id: 'verifypaytype',
                         name: 'verifypaytype',
                         fieldLabel: '付款方式',
                         editable: false,
                         labelWidth: 90,
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
                               { 'val': "9", 'txt': '其他' }
                             ]


                         }),
                         queryMode: 'local',
                         displayField: 'txt',
                         valueField: 'val',
                         anchor: '100%',

                         value: ''
                     },

                    {
                        id: 'verifytime',
                        name: 'verifytime',
                        xtype: 'datefield',
                        format: 'Y-m-d H:i:s',
                        fieldLabel: '核销时间',
                        labelWidth: 90,
                        allowBlank: false,
                        anchor: '100%',
                        value: new Date()
                    }


               ],
               buttonAlign: 'center',
               buttons: [
                   {
                       text: '确认',
                       iconCls: 'dropyes',
                       handler: function () {



                           Ext.MessageBox.confirm("提示", "总共核销金额" + actualdrivermoney + "元，是否核销？", function (obj) {
                               if (obj == "yes") {
                                   var form = Ext.getCmp('addform44');
                                   if (form.form.isValid()) {
                                       var values = form.form.getValues(false);
                                       var me = this;
                                       CS('CZCLZ.SJHXByMySql.HXMoneyALL', function (retVal) {
                                           if (retVal) {
                                               getUser(1);
                                               Ext.getCmp("lmwin44").close();

                                           }
                                       }, CS.onError, idlist, values);

                                   }
                               }
                           });

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



function AddPJ(notepid, username, actualdrivermoney, userid, verifymoney, offerid) {

    if (verifymoney != null && verifymoney != "" && verifymoney != "null") {

    } else {
        verifymoney = 0;
    }


    if (actualdrivermoney != null && actualdrivermoney != "" && actualdrivermoney != "null") {

    } else {
        actualdrivermoney = 0;
    }

    var c_window = new Ext.Window({
        extend: 'Ext.window.Window',
        viewModel: {
            type: 'mywindow'
        },
        autoShow: true,
        height: 300,
        id: "lmwin44",
        width: 450,
        layout: 'fit',
        title: '单独核销',
        modal: true,
        items: [
           {
               xtype: 'form',
               id: 'addform44',
               bodyPadding: 10,
               items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: '企业名称',
                        id: 'username',
                        name: 'username',
                        labelWidth: 90,
                        allowBlank: false,
                        anchor: '100%',
                        readOnly:true,
                        value: username
                    },
                     {
                         xtype: 'numberfield',
                         fieldLabel: '核销总金额',
                         id: 'verifymoney',
                         name: 'verifymoney',
                         decimalPrecision: 5,
                         minValue: 0,
                         maxValue: actualdrivermoney - verifymoney,
                         labelWidth: 90,
                         allowBlank: false,
                         anchor: '100%'
                     },
                     {
                         xtype: 'label',
                         text: '核销总金额：' + actualdrivermoney + ",已核销总金额：" + verifymoney + ",剩余可核销金额：" + (actualdrivermoney - verifymoney),
                     
                     },
                     {
                         xtype: 'combobox',
                         id: 'verifypaytype',
                         name: 'verifypaytype',
                         fieldLabel: '付款方式',
                         editable: false,
                         labelWidth: 90,
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
                               { 'val': "9", 'txt': '其他' }
                             ]


                         }),
                         queryMode: 'local',
                         displayField: 'txt',
                         valueField: 'val',
                         anchor: '100%',

                         value: ''
                     },

                    {
                        id: 'verifytime',
                        name: 'verifytime',
                        xtype: 'datefield',
                        format: 'Y-m-d H:i:s',
                        fieldLabel: '核销时间',
                        labelWidth: 90,
                        allowBlank: false,
                        anchor: '100%',
                        value: new Date()
                    }
                    

               ],
               buttonAlign: 'center',
               buttons: [
                   {
                       text: '确认',
                       iconCls: 'dropyes',
                       handler: function () {

                           

                           Ext.MessageBox.confirm("提示", "是否确认核销？", function (obj) {
                               if (obj == "yes") {
                                   var form = Ext.getCmp('addform44');
                                   if (form.form.isValid()) {
                                       var values = form.form.getValues(false);
                                       var me = this;
                                       CS('CZCLZ.SJHXByMySql.HXMoney', function (retVal) {
                                           if (retVal) {
                                               getUser(1);
                                               Ext.getCmp("lmwin44").close();

                                            }
                                       }, CS.onError, values, notepid, userid, username,(actualdrivermoney - verifymoney), actualdrivermoney, offerid);

                                   }
                               }
                           });

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

function ZF(billingid, shippingnoteid) {
    CS('CZCLZ.SJHXByMySql.ChangeZF', function (retVal) {
        if (retVal) {
            getUser(1);
            me.up('window').close();

        }
    }, CS.onError, billingid, shippingnoteid);
}


function LookP(userid, actualmoney) {



    var c_window = new Ext.Window({
        extend: 'Ext.window.Window',
        viewModel: {
            type: 'mywindow'
        },
        autoShow: true,
        height: 300,
        id: "lmwin44",
        width: 450,
        layout: 'fit',
        title: '开票信息',
        modal: true,
        items: [
           {
               xtype: 'form',
               id: 'addform44',
               bodyPadding: 10,
               items: [


                   {
                       xtype: 'textfield',
                       fieldLabel: '企业名称',
                       id: 'username',
                       name: 'username',
                       labelWidth: 70,
                       allowBlank: false, readOnly: true,

                       anchor: '100%'
                   },
                   ,
                   {
                       xtype: 'textfield',
                       fieldLabel: '纳税人识别号',
                       id: 'invoicenumber',
                       name: 'invoicenumber',
                       labelWidth: 70,
                       allowBlank: false, readOnly: true,

                       anchor: '100%'
                   },
                   ,
                   {
                       xtype: 'textfield',
                       fieldLabel: '地址',
                       id: 'invoiceaddress',
                       name: 'invoiceaddress',
                       labelWidth: 70,
                       allowBlank: false, readOnly: true,

                       anchor: '100%',
                   },
                   {
                       xtype: 'textfield',
                       fieldLabel: '电话',
                       id: 'invoicetel',
                       name: 'invoicetel',
                       labelWidth: 70,
                       allowBlank: false, readOnly: true,

                       anchor: '100%'
                   },
                   {
                       xtype: 'textfield',
                       fieldLabel: '开户行名称',
                       id: 'invoicebank',
                       name: 'invoicebank',
                       labelWidth: 70,
                       allowBlank: false, readOnly: true,

                       anchor: '100%'
                   },
                   {
                       xtype: 'textfield',
                       fieldLabel: '账号',
                       id: 'invoicebanknumber',
                       name: 'invoicebanknumber',
                       labelWidth: 70,
                       allowBlank: false, readOnly: true,

                       anchor: '100%'
                   }

               ],
               buttonAlign: 'center',
               buttons: [

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

    CS('CZCLZ.SJHXByMySql.GetUserMX', function (retVal) {
        if (retVal.length > 0) {
            var item = retVal[0];
            Ext.getCmp("username").setValue(item["username"]);
            Ext.getCmp("invoicenumber").setValue(item["invoicenumber"]);

            Ext.getCmp("invoiceaddress").setValue(item["invoiceaddress"]);

            Ext.getCmp("invoicetel").setValue(item["invoicetel"]);
            Ext.getCmp("invoicebank").setValue(item["invoicebank"]);
            Ext.getCmp("invoicebanknumber").setValue(item["invoicebanknumber"]);


        }
    }, CS.onError, userid);


}



function LookDD(shippingnotenumber, actualmoney) {



    var c_window = new Ext.Window({
        extend: 'Ext.window.Window',
        viewModel: {
            type: 'mywindow'
        },
        autoShow: true,
        height: 300,
        id: "lmwin44",
        width: 450,
        layout: 'fit',
        title: '订单信息',
        modal: true,
        items: [
           {
               xtype: 'form',
               id: 'addform44',
               bodyPadding: 10,
               items: [


                   {
                       xtype: 'textfield',
                       fieldLabel: '起始地',
                       id: 'goodsfromroute',
                       name: 'goodsfromroute',
                       labelWidth: 70,
                       allowBlank: false, readOnly: true,

                       anchor: '100%'
                   },
                   ,
                   {
                       xtype: 'textfield',
                       fieldLabel: '目的地',
                       id: 'goodstoroute',
                       name: 'goodstoroute',
                       labelWidth: 70,
                       allowBlank: false, readOnly: true,

                       anchor: '100%'
                   },
                   ,
                   {
                       xtype: 'textfield',
                       fieldLabel: '车牌号',
                       id: 'vehiclenumber',
                       name: 'vehiclenumber',
                       labelWidth: 70,
                       allowBlank: false, readOnly: true,

                       anchor: '100%',
                   },
                   {
                       xtype: 'textfield',
                       fieldLabel: '车型',
                       id: 'vehicletyperequirement',
                       name: 'vehicletyperequirement',
                       labelWidth: 70,
                       allowBlank: false, readOnly: true,

                       anchor: '100%'
                   },
                   {
                       xtype: 'textfield',
                       fieldLabel: '货物信息',
                       id: 'descriptionofgoods',
                       name: 'descriptionofgoods',
                       labelWidth: 70,
                       allowBlank: false, readOnly: true,

                       anchor: '100%'
                   },
                   {
                       xtype: 'textfield',
                       fieldLabel: '吨数',
                       id: 'itemgrossweight',
                       name: 'itemgrossweight',
                       labelWidth: 70,
                       allowBlank: false, readOnly: true,

                       anchor: '100%'
                   },
                   {
                       xtype: 'textfield',
                       fieldLabel: '司机身份证',
                       id: 'identitydocumentnumber',
                       name: 'identitydocumentnumber',
                       labelWidth: 70,
                       allowBlank: false, readOnly: true,

                       anchor: '100%'
                   }

               ],
               buttonAlign: 'center',
               buttons: [

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

    CS('CZCLZ.SJHXByMySql.GetDDMX', function (retVal) {
        if (retVal.length > 0) {
            var item = retVal[0];
            Ext.getCmp("goodsfromroute").setValue(item["goodsfromroute"]);
            Ext.getCmp("goodstoroute").setValue(item["goodstoroute"]);

            Ext.getCmp("vehiclenumber").setValue(item["vehiclenumber"]);

            Ext.getCmp("vehicletyperequirement").setValue(item["vehicletyperequirement"]);
            Ext.getCmp("descriptionofgoods").setValue(item["descriptionofgoods"]);
            Ext.getCmp("itemgrossweight").setValue(item["itemgrossweight"]);
            Ext.getCmp("identitydocumentnumber").setValue(item["identitydocumentnumber"]);


        }
    }, CS.onError, shippingnotenumber);


}



var verifystore = Ext.create('Ext.data.Store', {
    fields: [
         { name: 'verifymoney' },
       { name: 'verifypaytype' },
         { name: 'verifytime' }
       

    ]
});
function getStartOREnd(shippingnoteid) {
    verifystore.removeAll();
    
    CS('CZCLZ.SJHXByMySql.GetHXLIST', function (retVals) {
            if (retVals.length > 0) {
                verifystore.loadData(retVals, false);

            }
    }, CS.onError, shippingnoteid);
   



}

///起始地或者目的地
function CKHXJL(shippingnoteid) {





    getStartOREnd(shippingnoteid);
    var c_window = new Ext.Window({
        extend: 'Ext.window.Window',
        viewModel: {
            type: 'mywindow'
        },
        autoShow: true,
        height: 450,
        id: "lmwin88",
        width: 500,
        layout: 'fit',
        title: "核销记录",
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
                        store: verifystore,

                        columns: [
                                Ext.create('Ext.grid.RowNumberer', { width: 40 }),

                                    {
                                        dataIndex: 'verifymoney',
                                        text: '核销金额',
                                        flex: 1,
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    },
                                    {
                                        dataIndex: 'verifypaytype',
                                        flex: 1,
                                        text: '付款方式',
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true,
                                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                            if (value == "0")
                                            {
                                                return "银行卡";
                                            } else if (value == "1")
                                            {
                                                return "支付宝";
                                            } else if (value == "2") {
                                                return "微信";
                                            } else if (value == "3") {
                                                return "现金";
                                            } else if (value == "9") {
                                                return "其他";
                                            }
                                         }
                                    },
                                    {
                                        xtype: 'datecolumn',
    
                                        dataIndex: 'verifytime',
                                        flex: 1,
                                        text: '核销时间',
                                        format: 'Y-m-d H:i:s',
                                        align: "center",
                                        sortable: false,
                                        menuDisabled: true
                                    }
                                    
                        ]

                    }



               ]
           }
        ]
    });
    c_window.show();


}

