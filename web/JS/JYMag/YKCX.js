var pageSize = 15;
var cx_yhm;
//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
        { name: 'oilorderid' },
        { name: 'cardNo' },
        { name: 'oilNum' },
        { name: 'Price' },
        { name: 'money' },
        { name: 'oilType' },
        { name: 'oilName' },
        { name: 'oilLevel' },
        { name: 'status' },
        { name: 'oilordercode' },
        { name: 'orderId' },
        { name: 'addtime' },
        { name: 'UserTel' },
        { name: 'UserName' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        DataBind(nPage);
    }
});
//************************************数据源*****************************************

//************************************页面方法***************************************


function DataBind(nPage) {
    CS('CZCLZ.YKMag.GetYKDDList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_cardNo").getValue(), Ext.getCmp("cx_orderId").getValue(), Ext.getCmp("cx_zt").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
}


//************************************页面方法***************************************

//************************************弹出界面***************************************



//************************************弹出界面***************************************

//************************************主界面*****************************************
Ext.onReady(function() {
    Ext.define('YKView', {
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
                //    selModel: Ext.create('Ext.selection.CheckboxModel', {

                //}),
                columns: [Ext.create('Ext.grid.RowNumberer'),
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'cardNo',
                            sortable: false,
                            menuDisabled: true,
                            text: "加油卡号",
                            flex:1
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'oilNum',
                            sortable: false,
                            menuDisabled: true,
                            text: "加油量"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'Price',
                            sortable: false,
                            menuDisabled: true,
                            text: "单价"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'money',
                            sortable: false,
                            menuDisabled: true,
                            text: "消费总金额"
                        },
                         {
                             xtype: 'gridcolumn',
                             dataIndex: 'oilType',
                             sortable: false,
                             menuDisabled: true,
                             text: "油品类型"
                         },
                         {
                             xtype: 'gridcolumn',
                             dataIndex: 'oilName',
                             sortable: false,
                             menuDisabled: true,
                             text: "油品名称"
                         },
                         {
                             xtype: 'gridcolumn',
                             dataIndex: 'oilLevel',
                             sortable: false,
                             menuDisabled: true,
                             text: "油品等级"
                         },
                          {
                              xtype: 'gridcolumn',
                              dataIndex: 'status',
                              sortable: false,
                              menuDisabled: true,
                              text: "订单状态",
                              renderer: function (v, m) {
                                  str = "";
                                  if (v == 0) {
                                      str = "待付款";
                                  } else if (v == 1) {
                                      str = "支付成功";
                                  } else if (v == 2) {
                                      str = "交易取消";
                                  } else if (v == 3) {
                                      str = "交易撤销";
                                  }
                                  return str;
                              }
                          },
                          {
                              xtype: 'gridcolumn',
                              dataIndex: 'oilordercode',
                              sortable: false,
                              menuDisabled: true,
                              text: "订单号",
                              flex: 1
                          },
                          {
                              xtype: 'gridcolumn',
                              dataIndex: 'orderId',
                              sortable: false,
                              menuDisabled: true,
                              text: "找油网的交易流水号",
                              flex: 1
                          },
                           {
                               xtype: 'gridcolumn',
                               dataIndex: 'UserName',
                               sortable: false,
                               menuDisabled: true,
                               text: "用户手机号"
                           },
                           {
                               xtype: 'datecolumn',
                               dataIndex: 'addtime',
                               sortable: false,
                               menuDisabled: true,
                               format: 'Y-m-d H:i:s',
                               width:180,
                               text: "时间"
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
                                            id: 'cx_cardNo',
                                            width: 160,
                                            labelWidth: 60,
                                            fieldLabel: '加油卡号'
                                        },
                                        {
                                            xtype: 'textfield',
                                            id: 'cx_orderId',
                                            width: 250,
                                            labelWidth: 110,
                                            fieldLabel: '找油网交易流水号'
                                        },
                                        {
                                            xtype: 'combobox',
                                            id: 'cx_zt',
                                            width: 120,
                                            fieldLabel: '状态',
                                            editable: false,
                                            labelWidth: 30,
                                            store: Ext.create('Ext.data.Store', {
                                                fields: [
                                                   { name: 'val' },
                                                   { name: 'txt' }
                                                ],
                                                data: [
                                                     { 'val': "", 'txt': '全部' },
                                                    { 'val': 0, 'txt': '待付款' },
                                                    { 'val': 1, 'txt': '支付成功' },
                                                    { 'val': 2, 'txt': '交易取消' },
                                                    { 'val': 3, 'txt': '交易撤销' }]

                                            }),
                                            queryMode: 'local',
                                            displayField: 'txt',
                                            valueField: 'val',
                                            value: ''
                                        },
                                        {
                                            id: 'cx_beg',
                                            xtype: 'datefield',
                                            fieldLabel: '时间',
                                            format: 'Y-m-d',
                                            labelWidth: 40,
                                            width: 170
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
                                            xtype: 'buttongroup',
                                            title: '',
                                            items: [
                                                {
                                                    xtype: 'button',
                                                    iconCls: 'search',
                                                    text: '查询',
                                                    handler: function () {
                                                        DataBind(1);
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

    new YKView();

   
    DataBind();
})
//************************************主界面*****************************************