//-----------------------------------------------------------全局变量-----------------------------------------------------------------
var pageSize = 15;
//-----------------------------------------------------------数据源-------------------------------------------------------------------
//询单列表
var storeSourceGoods = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
    /*厂家，询价时间，询价状态，询价流程状态，单号，起始地，目的地，收货地址，收货方，收货联系人，收货联系方式，货物，
             * 数量，重量，体积，车型，车长，承运方，是否提货，是否送货，预付运费，预估企业报价，预估下游成本，预估税费成本，
             * 预估资金成本，备注*/
/*username,addtime,offerstatus,flowstatus,shippingnotenumber,goodsfromroute,goodstoroute,goodsreceiptplace,consignee,consicontactname,consitelephonenumber,descriptionofgoods,
 totalnumberofpackages,itemgrossweight,cube,vehicletyperequirement,vehiclelengthrequirementname,carriername,istakegoods,isdelivergoods,actualcompanypay,totalmonetaryamount,estimatemoney,estimatetaxmoney
 estimatecostmoney,memo*/
        { name: 'offerid' },//订单ID
        { name: 'username' },//厂家
        { name: 'addtime' },//询价时间
        { name: 'offerstatus' },//询价状态
        { name: 'flowstatus' },//询价流程状态
        { name: 'shippingnotenumber' },//单号
        { name: 'goodsfromroute' },//起始地
        { name: 'goodstoroute' },//目的地
        { name: 'goodsreceiptplace' },//收货地址
        { name: 'consignee' },//收货方
        { name: 'consicontactname' },//收货联系人
        { name: 'consitelephonenumber' },//收货联系方式
        { name: 'descriptionofgoods' },//货物
        { name: 'totalnumberofpackages' },//数量
        { name: 'itemgrossweight' },//重量
        { name: 'cube' },//体积
        { name: 'vehicletyperequirement' },//车型
        { name: 'vehiclelengthrequirement' },//车长
        { name: 'vehiclelengthrequirementname' },//车长
        { name: 'carriername' },//承运方
        { name: 'istakegoods' },//是否提货
        { name: 'isdelivergoods' },//是否送货
        { name: 'actualcompanypay' },//预付运费
        { name: 'memo' }, //备注
        { name: 'estimatecarriertype' },
        { name: 'istakegoodsbyestimate' },
        { name: 'estimatetakegoodsmoney' },
        { name: 'isvotebyestimate' },
        { name: 'estimatevotemoney' },
        { name: 'isoilbyestimate' },
        { name: 'estimateoilmoney' },
        { name: 'totalmonetaryamount' },//预估企业报价
        { name: 'estimatemoney' },//预估下游成本
        { name: 'estimatecompletemoney' },//预估综合成本
        { name: 'estimatetaxmoney' },//预估税费成本
        { name: 'estimatecostmoney' } //预估资金成本
    ],
    onPageChange: function (sto, nPage, sorters) {
        getSourceGoodsByPage(nPage);
    }
});

//订单列表
var storeOrder = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
    /*"厂家","供应商","装车司机","订单时间","订单状态","订单号","起始地","目的地","收货地址","收货方","收货联系人","收货联系方式",
     * "货物","数量","重量","体积","车型","车长","是否提货","是否送货","预付运费","实际运费","企业实际运费",
     * "备注" */
/*username,'carriername','takegoodsdrivername',shippingnoteadddatetime,shippingnotestatusname,shippingnotenumber,goodsfromroute,goodstoroute,goodsreceiptplace,consignee,consicontactname,consitelephonenumber,
 descriptionofgoods,totalnumberofpackages,itemgrossweight,cube,vehicletyperequirement,vehiclelengthrequirementname,istakegoodsname,isdelivergoodsname,totalmonetaryamount,actualmoney,"actualcompanypay",
 memo*/
        { name: 'usernamea' },//操作员
        { name: 'operatorid' },//订单ID
        { name: 'shippingnoteid' },//订单ID
        { name: 'username' },//厂家
        { name: 'shippingnoteadddatetime' },//订单时间
        { name: 'shippingnotestatuscode' },//订单状态
        { name: 'shippingnotestatusname' },//订单状态
        { name: 'shippingnotenumber' },//单号
        { name: 'goodsfromroute' },//起始地
        { name: 'goodstoroute' },//目的地
        { name: 'goodsreceiptplace' },//收货地址
        { name: 'consignee' },//收货方
        { name: 'consicontactname' },//收货联系人
        { name: 'consitelephonenumber' },//收货联系方式
        { name: 'descriptionofgoods' },//货物
        { name: 'totalnumberofpackages' },//数量
        { name: 'itemgrossweight' },//重量
        { name: 'cube' },//体积
        { name: 'vehicletyperequirement' },//车型
        { name: 'vehiclelengthrequirement' },//车长
        { name: 'vehiclelengthrequirementname' },//车长
        //{ name: 'carriername' },//承运方
        { name: 'istakegoods' },//是否提货
        { name: 'istakegoodsname' },//是否提货
        { name: 'isdelivergoods' },//是否送货
        { name: 'isdelivergoodsname' },//是否送货
        { name: 'totalmonetaryamount' },//预付运费
        { name: 'actualcompanypay' },//企业实际运费
        { name: 'actualmoney' },//实际运费
        { name: 'memo' },//备注
        //{ name: 'isabnormal' },
        { name: 'abnormalmemo' },
        { name: 'gpscompany' },
        { name: 'gpsdenno' },
        { name: 'doublepaynum' },
        { name: 'carrierid' },//供应商
        { name: 'carriername' },//供应商
        { name: 'driverid' },//承运司机
        { name: 'takegoodsdriver' }, //装车司机
        { name: 'takegoodsdrivername' }, //装车司机
        { name: 'offerid' },
        { name: 'isabnormal' },
        { name: 'actualmoneystatus' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        GetOrderByPage(nPage);
    }
});
//-----------------------------------------------------------页面方法-----------------------------------------------------------------
//获取询价列表
function getSourceGoodsByPage(nPage) {
    CS('CZCLZ.SourceGoods.getSourceGoodsListByPage', function (retVal) {
        storeSourceGoods.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });

    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), Ext.getCmp("cx_changjia").getValue(), Ext.getCmp("cx_offerstatus").getValue(), Ext.getCmp("cx_flowstatus").getValue());
}

//获取订单列表
function GetOrderByPage(nPage) {
    CS('CZCLZ.Order.GetOrderList', function (retVal) {
        storeOrder.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });

    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_beg2").getValue(), Ext.getCmp("cx_end2").getValue(), Ext.getCmp("cx_changjia2").getValue(),'','','','','');
}

//获取列表
function getLine(type, nPage) {
    Summary();
    if (type == 1) {
        getSourceGoodsByPage(nPage);
    } else {
        GetOrderByPage(nPage);
    }
}

//head汇总
function Summary() {
    CS('CZCLZ.Order.Summary', function (retVal) {
        let ret = retVal[0];
        Ext.getCmp('myFieldId').setText('未完结任务：' + ret.wwj + '单；已询价未下单：' + ret.yzx + '单；已询价已下单：' + ret.yxd + '单；询价失效：' + ret.ysx +'单；');
    }, CS.onError);
}
//-----------------------------------------------------------界    面-----------------------------------------------------------------
Ext.define('orderSumView', {
    extend: 'Ext.container.Viewport',

    layout: {
        type: 'fit'
    },

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'panel',
                layout: {
                    type: 'fit'
                },
                items: [
                    {
                        xtype: 'tabpanel',
                        activeTab: 0,
                        listeners: {
                            'tabchange': function (t, n) {
                                console.log(n);
                                if (n.title === "询单列表") {
                                    getLine(1, 1);
                                } else if (n.title === "订单列表") {
                                    getLine(2, 1);
                                }
                            }
                        },
                        items: [
                            {
                                xtype: 'panel',
                                layout: {
                                    type: 'fit'
                                },
                                title: '询单列表',
                                items: [
                                    {
                                        xtype: 'gridpanel',
                                        title: '',
                                        store: storeSourceGoods,
                                        columnLines: true,
                                        columns: [Ext.create('Ext.grid.RowNumberer'),
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'username',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "厂家"
                                        },
                                        {
                                            xtype: 'datecolumn',
                                            dataIndex: 'addtime',
                                            format: 'Y-m-d',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 110,
                                            text: '询价时间'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'offerstatus',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "询价状态",
                                            width: 110,
                                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                                var str = "";//报价单状态，（0：已咨询；1：已下单；2：已过期；）
                                                switch (value) {
                                                    case 0:
                                                        str = "已咨询";
                                                        break;
                                                    case 1:
                                                        str = "已下单";
                                                        break;
                                                    default:
                                                        str = "已过期";
                                                }

                                                return str;
                                            }
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'flowstatus',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "询价流程状态",
                                            width: 170,
                                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                                var str = "";//询价流程状态（0：询价单等待市场部转发操作；10：询价单已发送操作员，等待报价；20：操作员已报价，等待市场部提交客户报价；90：市场部已提交客户报价，等待客户下单；）
                                                switch (value) {
                                                    case 0:
                                                        str = "询价单等待市场部转发操作";
                                                        break;
                                                    case 10:
                                                        str = "询价单已发送操作员，等待报价";
                                                        break;
                                                    case 20:
                                                        str = "操作员已报价，等待市场部提交客户报价";
                                                        break;
                                                    case 90:
                                                        str = "市场部已提交客户报价，等待客户下单";
                                                        break;
                                                    default:
                                                        str = "";
                                                }

                                                return str;
                                            }
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'shippingnotenumber',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 220,
                                            text: "单号",
                                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {

                                                return "<input value='"+ value +"' style='border:0px;BACKGROUND-COLOR: transparent;'>";
                                            }
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'goodsfromroute',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "起始地"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'goodstoroute',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "目的地"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'goodsreceiptplace',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "收货地址"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'consignee',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "收货方"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'consicontactname',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "收货联系人"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'consitelephonenumber',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "收货联系方式"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'descriptionofgoods',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "货物"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'totalnumberofpackages',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "数量"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'itemgrossweight',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "重量"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'cube',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "体积"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'vehicletyperequirement',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "车型",
                                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                                var str = "";//车型（1：栏板车；2：厢车；）
                                                switch (value) {
                                                    case "1":
                                                        str = "栏板车";
                                                        break;
                                                    case "2":
                                                        str = "厢车";
                                                        break;
                                                    default:
                                                        str = "";
                                                }

                                                return str;
                                            }
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'vehiclelengthrequirementname',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "车长"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'carriername',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "承运方"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'istakegoods',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "是否提货",
                                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                                var str = "";//是否提货（0：提货；1：不提；）
                                                switch (value) {
                                                    case 0:
                                                        str = "提货";
                                                        break;
                                                    default:
                                                        str = "不提";
                                                }

                                                return str;
                                            }
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'isdelivergoods',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "是否送货",
                                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                                var str = "";//是否送货（0：送货；1：不送；）
                                                switch (value) {
                                                    case 0:
                                                        str = "送货";
                                                        break;
                                                    default:
                                                        str = "不送";
                                                }

                                                return str;
                                            }
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'actualcompanypay',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "预付运费"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'totalmonetaryamount',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "预估企业报价"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'estimatemoney',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "预估下游成本"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'estimatecompletemoney',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "预估综合成本"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'estimatetaxmoney',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "预估税费成本"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'estimatecostmoney',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "预估资金成本"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'memo',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "备注",
                                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                                 return "<input value='" + value + "' style='border:0px;BACKGROUND-COLOR: transparent;'>";
                                            }
                                        },
                                        {
                                            text: '操作',
                                            dataIndex: 'offerid',
                                            width: 130,
                                            sortable: false,
                                            menuDisabled: true,
                                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                                var str = '';
                                                
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
                                                        id: 'cx_beg',
                                                        xtype: 'datefield',
                                                        fieldLabel: '询价时间',
                                                        format: 'Y-m-d',
                                                        labelWidth: 80,
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
                                                        id: 'cx_changjia',
                                                        width: 140,
                                                        labelWidth: 50,
                                                        fieldLabel: '厂家'
                                                    },
                                                    {
                                                        xtype: 'combobox',//报价单状态，（0：已咨询；1：已下单；2：已过期；）
                                                        fieldLabel: '类型',
                                                        labelWidth: 60,
                                                        width: 180,
                                                        editable: false,
                                                        id: 'cx_offerstatus',
                                                        store: Ext.create('Ext.data.Store', {
                                                            fields: [
                                                                { name: 'value' },
                                                                { name: 'name' }
                                                            ],
                                                            data: [
                                                                { 'value': "", 'name': '全部' },
                                                                { 'value': "0", 'name': '已咨询' },
                                                                { 'value': "1", 'name': '已下单' },
                                                                { 'value': "2", 'name': '已过期' }
                                                            ]
                                                        }),
                                                        queryMode: 'local',
                                                        displayField: 'name',
                                                        valueField: 'value',
                                                        value: ""
                                                    },
                                                    {
                                                        xtype: 'combobox',//询价流程状态（0：询价单等待市场部转发操作；10：询价单已发送操作员，等待报价；20：操作员已报价，等待市场部提交客户报价；90：市场部已提交客户报价，等待客户下单；）
                                                        fieldLabel: '流程状态',
                                                        labelWidth: 60,
                                                        width: 280,
                                                        editable: false,
                                                        id: 'cx_flowstatus',
                                                        store: Ext.create('Ext.data.Store', {
                                                            fields: [
                                                                { name: 'value' },
                                                                { name: 'name' }
                                                            ],
                                                            data: [
                                                                { 'value': '', 'name': '全部' },
                                                                { 'value': '0', 'name': '询价单等待市场部转发操作' },
                                                                { 'value': '10', 'name': '询价单已发送操作员，等待报价' },
                                                                { 'value': '20', 'name': '操作员已报价，等待市场部提交客户报价' },
                                                                { 'value': '90', 'name': '市场部已提交客户报价，等待客户下单' }
                                                            ]
                                                        }),
                                                        queryMode: 'local',
                                                        displayField: 'name',
                                                        valueField: 'value'
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
                                                                    getSourceGoodsByPage(1);
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
                                                                text: '导出',
                                                                handler: function () {
                                                                    DownloadFile("CZCLZ.Order.ExportSourceGoods", "询单列表.xls", Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), Ext.getCmp("cx_changjia").getValue(), Ext.getCmp("cx_offerstatus").getValue(), Ext.getCmp("cx_flowstatus").getValue());
                                                                }
                                                            }
                                                        ]
                                                    }
                                                ]
                                            },
                                            {
                                                xtype: 'pagingtoolbar',
                                                displayInfo: true,
                                                store: storeSourceGoods,
                                                dock: 'bottom'
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
                                title: '订单列表',
                                items: [
                                    {
                                        xtype: 'gridpanel',
                                        title: '',
                                        store: storeOrder,
                                        columnLines: true,
                                        columns: [Ext.create('Ext.grid.RowNumberer'),
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'username',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "厂家"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'carriername',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "供应商"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'takegoodsdrivername',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "装车司机"
                                        },
                                        {
                                            xtype: 'datecolumn',
                                            dataIndex: 'shippingnoteadddatetime',
                                            format: 'Y-m-d',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 110,
                                            text: '订单时间'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'shippingnotestatusname',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "订单状态"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'shippingnotenumber',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "订单号",
                                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {

                                                return "<input value='"+ value +"' style='border:0px;BACKGROUND-COLOR: transparent;'>";
                                            }
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'goodsfromroute',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "起始地"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'goodstoroute',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "目的地"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'goodsreceiptplace',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "收货地址"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'consignee',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "收货方"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'consicontactname',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "收货联系人"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'consitelephonenumber',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "收货联系方式"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'descriptionofgoods',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "货物"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'totalnumberofpackages',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "数量"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'itemgrossweight',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "重量"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'cube',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "体积"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'vehicletyperequirement',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "车型",
                                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                                var str = "";//车型（1：栏板车；2：厢车；）
                                                switch (value) {
                                                    case "1":
                                                        str = "栏板车";
                                                        break;
                                                    case "2":
                                                        str = "厢车";
                                                        break;
                                                    default:
                                                        str = "";
                                                }

                                                return str;
                                            }
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'vehiclelengthrequirementname',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "车长"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'istakegoodsname',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "是否提货"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'isdelivergoodsname',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "是否送货"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'totalmonetaryamount',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "预付运费"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'actualmoney',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "实际运费"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'actualcompanypay',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "企业实际运费"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'memo',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "备注"
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'usernamea',
                                            sortable: false,
                                            menuDisabled: true,
                                            text: "操作员"
                                        },
                                        {
                                            text: '操作',
                                            dataIndex: 'shippingnoteid',
                                            width: 80,
                                            sortable: false,
                                            menuDisabled: true,
                                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                                var str = "";
                                                
                                                return str;
                                            }
                                        }
                                        ],
                                        viewConfig: {
                                            getRowClass: function (record, rowIndex, rowParams, store) {
                                                if (record.data.isabnormal != null) {
                                                    return "x-grid-record-yellow";
                                                }
                                            }
                                        },
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
                                                        labelWidth: 80,
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
                                                        id: 'cx_changjia2',
                                                        width: 140,
                                                        labelWidth: 50,
                                                        fieldLabel: '厂家'
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
                                                                    GetOrderByPage(1);
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
                                                                text: '导出',
                                                                handler: function () {
                                                                    DownloadFile("CZCLZ.Order.ExportOrder", "订单列表.xls", Ext.getCmp("cx_beg2").getValue(), Ext.getCmp("cx_end2").getValue(), Ext.getCmp("cx_changjia2").getValue());
                                                                }
                                                            }
                                                        ]
                                                    }
                                                ]
                                            },
                                            {
                                                xtype: 'pagingtoolbar',
                                                displayInfo: true,
                                                store: storeOrder,
                                                dock: 'bottom'
                                            }
                                        ]
                                    }
                                ]
                            }
                        ]

                    }
                ],
                dockedItems: [
                    {
                        xtype: 'toolbar',
                        dock: 'top',
                        items: [
                            {
                                xtype: 'label',
                                id: 'myFieldId',
                                margin: '10 10 10 10'
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
    new orderSumView();
    getLine(1, 1);
})