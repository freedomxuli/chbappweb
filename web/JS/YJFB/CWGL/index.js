
var pageSize = 15;
var orderstore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [

             { name: 'descriptionofgoods' },

    { name: 'shippingnoteid' },
         { name: 'userid' },
                           { name: 'actualcompanypay' },

                  { name: 'invoicestatus' },
                                   { name: 'actualcompanypay' },

         
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
    CS('CZCLZ.CWByMySql.GetUserList', function (retVal) {
        orderstore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });

    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), Ext.getCmp("cx_name").getValue());
}
function getUser2(nPage) {
    CS('CZCLZ.CWByMySql.GetUserList2', function (retVal) {
        orderstore2.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });

    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_beg2").getValue(), Ext.getCmp("cx_end2").getValue(), Ext.getCmp("cx_name2").getValue());
}

function getUser3(nPage) {
    CS('CZCLZ.CWByMySql.GetUserList3', function (retVal) {
        orderstore3.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });

    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_beg3").getValue(), Ext.getCmp("cx_end3").getValue(), Ext.getCmp("cx_name3").getValue());
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
                                title: '开票管理',
                                items: [
                                    {
                                        xtype: 'gridpanel',
                                        border: 1,
                                        columnLines: 1,
                                        store: orderstore,
                                        id:'usergrid',
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
                                                  flex: 2,
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
                                                },

                                                
                                                {
                                                xtype: 'gridcolumn',
                                                dataIndex: 'actualcompanypay',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    flex: 1,
                                                    text: '应付金额'
                                                },  {
                                                xtype: 'gridcolumn',
                                                dataIndex: 'username',
                                                sortable: false,
                                                menuDisabled: true,
                                                flex: 1,
                                                text: '企业名称'
                                            }, 
                                                {
                                                    xtype: 'gridcolumn',
                                                    dataIndex: 'totalamount',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    flex: 1,
                                                    text: '合计金额'
                                                },   {
                                                    xtype: 'datecolumn',
                                                    format: 'Y-m-d H:i:s',
                                                    dataIndex: 'billingtime',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    flex: 1,
                                                    text: '开票时间'
                                                }, {
                                                    xtype: 'gridcolumn',
                                                    dataIndex: 'invoicecode',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    flex: 1,
                                                    text: '发票代码'
                                                }, {
                                                    xtype: 'gridcolumn',
                                                    dataIndex: 'invoicenumber',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    flex: 1,
                                                    text: '发票号码'
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
                                                }, {
                                                    xtype: 'gridcolumn',
                                                    dataIndex: 'shippingnoteid',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    width: 100,
                                                    text: '操作',
                                                    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                                        if (record.data.invoicestatus == 0) {
                                                            return "<div style='color:green;cursor:pointer;' onclick='ZF(\"" + value + "\",\"" + record.data.actualmoney + "\")'>作废</div>";

                                                        } else {
                                                            return "<div style='color:green;cursor:pointer;' onclick='AddPJ(\"" + value + "\",\"" + record.data.actualmoney + "\")'>开票</div>";
                                                        }
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
                                                        fieldLabel: '厂家'
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
                                                        text: '一键开票',
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
                                                            var actualwaymoney = 0;
                                                            for (var n = 0, len = rds.length; n < len; n++) {
                                                                var rd = rds[n];
                                                                idlist.push(rd.get("shippingnoteid") + "," + rd.get("offerid") + "," + rd.get("actualcompanypay"));

                                                                actualwaymoney += rd.get("actualcompanypay")
                                                            }

                                                            AddPJ2(idlist,  actualwaymoney);
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
                            {
                                xtype: 'panel',
                                layout: {
                                    type: 'fit'
                                },
                                title: '已开票',
                            hidden:true,
                                items: [
                                    {
                                        xtype: 'gridpanel',
                                        border: 1,
                                        columnLines: 1,
                                        store: orderstore2,
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
                                                    dataIndex: 'statisticstype',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    flex: 1,
                                                    text: '货名'
                                                }, {
                                                    xtype: 'gridcolumn',
                                                    dataIndex: 'totalamount',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    flex: 1,
                                                    text: '合计金额'
                                                }, {
                                                    xtype: 'gridcolumn',
                                                    dataIndex: 'totalvaloremtax',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    flex: 1,
                                                    text: '价税合计'
                                                }, {
                                                    xtype: 'gridcolumn',
                                                    dataIndex: 'rate',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    flex: 1,
                                                    text: '税率'
                                                }, {
                                                    xtype: 'datecolumn',
                                                    format: 'Y-m-d H:i:s',
                                                    dataIndex: 'billingtime',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    flex: 1,
                                                    text: '开票时间'
                                                }, {
                                                    xtype: 'gridcolumn',
                                                    dataIndex: 'invoicecode',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    flex: 1,
                                                    text: '发票代码'
                                                }, {
                                                    xtype: 'gridcolumn',
                                                    dataIndex: 'invoicenumber',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    flex: 1,
                                                    text: '发票号码'
                                                }, {
                                                    xtype: 'gridcolumn',
                                                    dataIndex: 'billingid',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    width: 100,
                                                    text: '操作',
                                                    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                                        return "<div style='color:green;cursor:pointer;' onclick='ZF(\"" + value + "\",\"" + record.data.actualmoney + "\")'>作废</div>";
                                                    }
                                                }

                                        ],
                                        dockedItems: [
                                            {
                                                xtype: 'toolbar',
                                                dock: 'top',
                                                items: [
                                                    {
                                                        id: 'cx_beg2',
                                                        xtype: 'datefield',
                                                        fieldLabel: '订单时间',
                                                        format: 'Y-m-d',
                                                        labelWidth: 70,
                                                        width: 210
                                                    },
                                                    {
                                                        id: 'cx_end2',
                                                        xtype: 'datefield',
                                                        format: 'Y-m-d',
                                                        fieldLabel: '至',
                                                        labelWidth: 20,
                                                        width: 150
                                                    },
                                                    {
                                                        xtype: 'textfield',
                                                        id: 'cx_name2',
                                                        width: 160,
                                                        labelWidth: 70,
                                                        fieldLabel: '厂家'
                                                    },
                                                    {
                                                        xtype: 'button',
                                                        iconCls: 'search',
                                                        text: '查询',
                                                        handler: function () {
                                                            getUser2(1);
                                                        }
                                                    }
                                                ]
                                            },
                                            {
                                                xtype: 'pagingtoolbar',
                                                dock: 'bottom',
                                                width: 360,
                                                store: orderstore2,
                                                displayInfo: true
                                            }
                                        ]
                                    }
                                ]
                            },
                            {
                                xtype: 'panel',
                                layout: {
                                    type: 'fit'
                                },
                                title: '已作废',
                                hidden:true,
                                items: [
                                    {
                                        xtype: 'gridpanel',
                                        border: 1,
                                        columnLines: 1,
                                        store: orderstore3,
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
                                                    dataIndex: 'statisticstype',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    flex: 1,
                                                    text: '货名'
                                                }, {
                                                    xtype: 'gridcolumn',
                                                    dataIndex: 'totalamount',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    flex: 1,
                                                    text: '合计金额'
                                                }, {
                                                    xtype: 'gridcolumn',
                                                    dataIndex: 'totalvaloremtax',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    flex: 1,
                                                    text: '价税合计'
                                                }, {
                                                    xtype: 'gridcolumn',
                                                    dataIndex: 'rate',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    flex: 1,
                                                    text: '税率'
                                                }, {
                                                    xtype: 'datecolumn',
                                                    format: 'Y-m-d H:i:s',
                                                    dataIndex: 'billingtime',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    flex: 1,
                                                    text: '开票时间'
                                                }, {
                                                    xtype: 'gridcolumn',
                                                    dataIndex: 'invoicecode',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    flex: 1,
                                                    text: '发票代码'
                                                }, {
                                                    xtype: 'gridcolumn',
                                                    dataIndex: 'invoicenumber',
                                                    sortable: false,
                                                    menuDisabled: true,
                                                    flex: 1,
                                                    text: '发票号码'
                                                }

                                        ],
                                        dockedItems: [
                                            {
                                                xtype: 'toolbar',
                                                dock: 'top',
                                                items: [
                                                    {
                                                        id: 'cx_beg3',
                                                        xtype: 'datefield',
                                                        fieldLabel: '订单时间',
                                                        format: 'Y-m-d',
                                                        labelWidth: 70,
                                                        width: 210
                                                    },
                                                    {
                                                        id: 'cx_end3',
                                                        xtype: 'datefield',
                                                        format: 'Y-m-d',
                                                        fieldLabel: '至',
                                                        labelWidth: 20,
                                                        width: 150
                                                    },
                                                    {
                                                        xtype: 'textfield',
                                                        id: 'cx_name3',
                                                        width: 160,
                                                        labelWidth: 70,
                                                        fieldLabel: '厂家'
                                                    },
                                                    {
                                                        xtype: 'button',
                                                        iconCls: 'search',
                                                        text: '查询',
                                                        handler: function () {
                                                            getUser3(1);
                                                        }
                                                    }
                                                ]
                                            },
                                            {
                                                xtype: 'pagingtoolbar',
                                                dock: 'bottom',
                                                width: 360,
                                                store: orderstore3,
                                                displayInfo: true
                                            }
                                        ]
                                    }
                                ]
                            }
                        ]
                    }
                ]
            });

            me.callParent(arguments);
        }

    });

    new ConsumeView();
    getUser(1);
    //getUser2(1);
    //getUser3(1);
 

});





function AddPJ(notepid, actualmoney) {
 


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
        title: '开票',
        modal: true,
        items: [
           {
               xtype: 'form',
               id: 'addform44',
               bodyPadding: 10,
               items: [
                    {
                        id: 'billingtime',
                        name: 'billingtime',
                        xtype: 'datefield',
                        format: 'Y-m-d',
                        fieldLabel: '开票时间',
                        labelWidth: 70,
                        allowBlank: false,
                        anchor: '100%',
                        value: new Date()
                    },
                   {
                       xtype: 'numberfield',
                       fieldLabel: '税率（%）',
                       id: 'rate',
                       name: 'rate',
                       labelWidth: 70,
                       allowBlank: false,
                       anchor: '100%',
                       value:'9'
                   },
                   {
                       xtype: 'textfield',
                       fieldLabel: '发票代码',
                       id: 'invoicecode',
                       name: 'invoicecode',
                       labelWidth: 70,
                       allowBlank: false,
                       anchor: '100%'
                   },
                   ,
                   {
                       xtype: 'textfield',
                       fieldLabel: '发票号码',
                       id: 'invoicenumber',
                       name: 'invoicenumber',
                       labelWidth: 70,
                       allowBlank: false,
                       anchor: '100%'
                   },
                   ,
                   {
                       xtype: 'textfield',
                       fieldLabel: '价税合计',
                       id: 'totalvaloremtax',
                       name: 'totalvaloremtax',
                       labelWidth: 70,
                       allowBlank: false,
                       anchor: '100%',
                       value: actualmoney
                   },
                   {
                       xtype: 'textfield',
                       fieldLabel: '合计金额',
                       id: 'totalamount',
                       name: 'totalamount',
                       labelWidth: 70,
                       allowBlank: false,
                       anchor: '100%',
                       value: actualmoney*0.91
                   },
                   {
                       xtype: 'textfield',
                       fieldLabel: '开票项目',
                       id: 'billingitem',
                       name: 'billingitem',
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
                               CS('CZCLZ.CWByMySql.SaveKP', function (retVal) {
                                   if (retVal) {
                                       getUser(1);
                                       me.up('window').close();

                                   }
                               }, CS.onError, values, notepid);

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

function ZF(billingid, shippingnoteid)
{
    CS('CZCLZ.CWByMySql.ChangeZF', function (retVal) {
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

    CS('CZCLZ.CWByMySql.GetUserMX', function (retVal) {
        if (retVal.length>0) {
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

    CS('CZCLZ.CWByMySql.GetDDMX', function (retVal) {
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



function AddPJ2(idlist, actualcompanypay) {



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
                       id: 'billingtime',
                       name: 'billingtime',
                       xtype: 'datefield',
                       format: 'Y-m-d',
                       fieldLabel: '开票时间',
                       labelWidth: 70,
                       allowBlank: false,
                       anchor: '100%',
                       value: new Date()
                   },
                   {
                       xtype: 'numberfield',
                       fieldLabel: '税率（%）',
                       id: 'rate',
                       name: 'rate',
                       labelWidth: 70,
                       allowBlank: false,
                       anchor: '100%',
                       value: '9'
                   },
                   {
                       xtype: 'textfield',
                       fieldLabel: '发票代码',
                       id: 'invoicecode',
                       name: 'invoicecode',
                       labelWidth: 70,
                       allowBlank: false,
                       anchor: '100%'
                   },
                   ,
                   {
                       xtype: 'textfield',
                       fieldLabel: '发票号码',
                       id: 'invoicenumber',
                       name: 'invoicenumber',
                       labelWidth: 70,
                       allowBlank: false,
                       anchor: '100%'
                   },
                   ,
                   {
                       xtype: 'textfield',
                       fieldLabel: '企业付款',
                       id: 'actualcompanypay',
                       name: 'actualcompanypay',
                       labelWidth: 70,
                       allowBlank: false,
                       anchor: '100%',
                       value: actualcompanypay
                   },
                   
                   {
                       xtype: 'textfield',
                       fieldLabel: '开票项目',
                       id: 'billingitem',
                       name: 'billingitem',
                       labelWidth: 70,
                       allowBlank: false,
                       anchor: '100%'
                   }


               ],
               buttonAlign: 'center',
               buttons: [
                   {
                       text: '确认',
                       iconCls: 'dropyes',
                       handler: function () {



                           Ext.MessageBox.confirm("提示", "总共开票金额" + actualcompanypay + "元，是否开票？", function (obj) {
                               if (obj == "yes") {
                                   var form = Ext.getCmp('addform44');
                                   if (form.form.isValid()) {
                                       var values = form.form.getValues(false);
                                       var me = this;
                                  

                                       CS('CZCLZ.CWByMySql.SaveKPALL', function (retVal) {
                                           if (retVal) {
                                               getUser(1);
                                               me.up('window').close();

                                           }
                                       }, CS.onError, values, idlist);


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
