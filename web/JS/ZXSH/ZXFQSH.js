﻿var pageSize = 15;
var cx_xm;
var cx_isVerifyType;
var userId = "";
//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
        { name: 'SaleRecordID' },
        { name: 'SaleRecordCode' },
        { name: 'SaleRecordUserID' },
        { name: 'SaleRecordUserXM' },
        { name: 'SaleRecordPoints' },
        { name: 'SaleRecordLX' },
        { name: 'Status' },
        { name: 'adduser' },
        { name: 'updateuser' },
        { name: 'updatetime' },
        { name: 'SaleRecordBelongID' },
        { name: 'ValidHour' },
        { name: 'SaleRecordTime' },
        { name: 'addtime' },
        { name: 'SaleRecordDiscount' },
        { name: 'SaleRecordVerifyType' },
        { name: 'SaleRecordVerifyTime' },
        { name: 'ZXSaleListCitys' },
        { name: 'ProduceLx' },
        { name: 'PackLx' },
        { name: 'Fc' },
        { name: 'ZhLx' },
        { name: 'PhLx' },
        { name: 'FromRoute' },
        { name: 'ToRoute' }
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
            'ID': '1',
            'MC': '拒绝'
        }

    ]
});
//************************************数据源*****************************************

//************************************页面方法***************************************
function getUser(nPage) {
    CS('CZCLZ.ZXSHMag.getZFQList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_xm").getValue(), Ext.getCmp("cx_isVerifyType").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
}

function sq(id) {
    if (privilege("专线审核_审核专线自发券_编辑") && privilege("专线审核_审核专线自发券_审核")) {
        Ext.MessageBox.confirm('提示', '是否要通过!', function (obj) {
            if (obj == "yes") {
                CS('CZCLZ.ZXSHMag.ZFQSH', function (retVal) {
                    if (retVal) {
                        getUser(1);
                        Ext.MessageBox.alert('提示', '申请通过成功！',"");
                    }
                }, CS.onError, id, 1,"");
            }
            else {

                var yjwin = new yjWin({ id: id });
                yjwin.show();
               
            }
        });
    }
}

//************************************页面方法***************************************
//************************************弹出页面***************************************
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
                            CS('CZCLZ.ZXSHMag.ZFQSH', function (retVal) {
                                if (retVal) {
                                    getUser(1);
                                    Ext.MessageBox.alert('提示', '申请拒绝成功！');
                                }
                            }, CS.onError, id, 2,Ext.getCmp("yj").getValue());

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
//************************************弹出页面***************************************
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
                                dataIndex: 'SaleRecordUserXM',
                                sortable: false,
                                menuDisabled: true,
                                flex:1,
                                text: "专线名称"
                            },
                            {
                                xtype: 'gridcolumn',
                                sortable: false,
                                menuDisabled: true,
                                text: "线路",
                                flex: 1,
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
                                xtype: 'gridcolumn',
                                dataIndex: 'ZXSaleListCitys',
                                sortable: false,
                                menuDisabled: true,
                                flex: 1,
                                text: "目的地"
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'SaleRecordPoints',
                                sortable: false,
                                menuDisabled: true,
                                width: 100,
                                text: "运费券"

                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'SaleRecordDiscount',
                                sortable: false,
                                menuDisabled: true,
                                width: 100,
                                text: "折扣"
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'ValidHour',
                                sortable: false,
                                menuDisabled: true,
                                width: 100,
                                text: "有效时间"
                            },
                            {
                                xtype: 'datecolumn',
                                dataIndex: 'SaleRecordTime',
                                sortable: false,
                                menuDisabled: true,
                                width: 150,
                                format: 'Y-m-d H:i:s',
                                text: "开放时间"
                            },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'ProduceLx',
                                 sortable: false,
                                 menuDisabled: true,
                                 width: 100,
                                 text: "货物类型"
                             },
                             {
                                 xtype: 'gridcolumn',
                                 sortable: false,
                                 menuDisabled: true,
                                 width: 100,
                                 text: "重货/泡货类型",
                                 renderer: function (value, metaData, record, rowIndex, colIndex, store, view) {
                                     var str = "";
                                     if (record.data.ProduceLx == "重货") {
                                         str += record.data.ZhLx;
                                     } else if (record.data.ProduceLx == "泡货") {
                                         str += record.data.PhLx;
                                     } 
                                     return str;
                                 }
                             },
                              {
                                  xtype: 'gridcolumn',
                                  dataIndex: 'PackLx',
                                  sortable: false,
                                  menuDisabled: true,
                                  width: 100,
                                  text: "包装要求"
                              },
                               {
                                   xtype: 'gridcolumn',
                                   dataIndex: 'Fc',
                                   sortable: false,
                                   menuDisabled: true,
                                   width: 100,
                                   text: "发车时间"
                               },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'SaleRecordVerifyType',
                                 sortable: false,
                                 menuDisabled: true,
                                 width: 100,
                                 text: "审核状态",
                                 renderer: function (value, metaData, record, rowIndex, colIndex, store, view) {
                                     var str = "";
                                     if (value == 1) {
                                         str += "通过"
                                     } else if (value == 2) {
                                         str += "拒绝"
                                     } else if (value == 0 || value == null) {
                                         str += "待审核"
                                     }
                                     return str;
                                 }

                             },
                             {
                                 xtype: 'datecolumn',
                                 dataIndex: 'SaleRecordVerifyTime',
                                 sortable: false,
                                 menuDisabled: true,
                                 width: 150,
                                 format: 'Y-m-d H:i:s',
                                 text: "审核时间"
                             },
                            {
                                xtype: 'gridcolumn',
                                sortable: false,
                                menuDisabled: true,
                                dataIndex: 'SaleRecordID',
                                text: '操作',
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    str = "";
                                    console.log(record.data.SaleRecordVerifyType);
                                    if (record.data.SaleRecordVerifyType == 0 || record.data.SaleRecordVerifyType == null) {
                                        str = "<a href='JavaScript:void(0)' onclick='sq(\"" + value + "\")'>审核</a>";
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
                                            width: 160,
                                            labelWidth: 70,
                                            fieldLabel: '物流名称'
                                        },
                                        {
                                            xtype: 'combobox',
                                            id: 'cx_isVerifyType',
                                            width: 260,
                                            fieldLabel: '是否审核',
                                            editable: false,
                                            labelWidth: 70,
                                            store: KindStore,
                                            queryMode: 'local',
                                            displayField: 'MC',
                                            valueField: 'ID',
                                            value: ''
                                        },
                                        {
                                            xtype: 'datefield',
                                            id: 'cx_beg',
                                            fieldLabel: '开放时间',//SaleRecordTime
                                            width: 180,
                                            format: 'Y-m-d',
                                            labelWidth: 60
                                        },
                                        {
                                            xtype: 'datefield',
                                            id: 'cx_end',
                                            fieldLabel: '~',//SaleRecordTime
                                            width: 130,
                                            format: 'Y-m-d',
                                            labelWidth: 10,
                                            labelSeparator:''
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
                                                        if (privilege("专线审核_审核专线自发券_查看")) {
                                                            getUser(1);
                                                        }
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
                                                    iconCls: 'view',
                                                    text: '导出',
                                                    handler: function () {
                                                        if (privilege("专线审核_审核专线自发券_导出")) {
                                                            DownloadFile("CZCLZ.ZXSHMag.getZFQListToFile", "审核专线自发券.xls", Ext.getCmp("cx_xm").getValue(), Ext.getCmp("cx_isVerifyType").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
                                                        }
                                                    }
                                                }]
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


    cx_xm = Ext.getCmp("cx_xm").getValue();
    cx_isVerifyType = Ext.getCmp("cx_isVerifyType").getValue();
    getUser(1);

})
//************************************主界面*****************************************