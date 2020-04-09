//-----------------------------------------------------------全局变量-----------------------------------------------------------------
var pageSize = 15;

//-----------------------------------------------------------数据源-------------------------------------------------------------------
var storeSourceGoods = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
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
        { name: 'estimatemoney' },
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
        { name: 'estimatecostmoney' }, //预估资金成本
        { name: 'estimateautomoney' } //预估自动计算金额
    ],
    onPageChange: function (sto, nPage, sorters) {
        getSourceGoodsListByPage(nPage);
    }
});

//-----------------------------------------------------------页面方法-----------------------------------------------------------------
//获取订单列表
function getSourceGoodsListByPage(nPage) {
    CS('CZCLZ.SourceGoods.getSourceGoodsListByPage', function (retVal) {
        storeSourceGoods.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });

    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), Ext.getCmp("cx_changjia").getValue(), Ext.getCmp("cx_offerstatus").getValue(), Ext.getCmp("cx_flowstatus").getValue());
}

//转操作部填报价格
function ToWritePrice(offerid) {
    if (privilege("一键发包模块_询价管理-询价列表_转操作部填报价格")) {
        Ext.MessageBox.confirm('添加提示', '是否要转操作部填报价格!', function (obj) {
            if (obj == "yes") {
                CS('CZCLZ.SourceGoods.ToWritePrice', function (retVal) {
                    getSourceGoodsListByPage(1);
                }, CS.onError, offerid);
            } else {
                return;
            }
        });

    }
}

//操作员填写运费
function EditTotalmonetaryamount(offerid) {
    if (privilege("一键发包模块_询价管理-询价列表_操作员填写运费")) {
        //var Win = new monetaryAmountWin({ offerid: offerid });
        //Win.show();
        var Win = new monetaryAmountWinNew({ offerid: offerid });
        Win.show();
    }
}

//市场部企业报价
function MktOffer(offerid) {
    if (privilege("一键发包模块_询价管理-询价列表_市场部企业报价")) {
        //var r = storeSourceGoods.findRecord("offerid", offerid).data;
        //let oWin = new mktOfferWin({ offerid: offerid });
        //oWin.show(null, function () {
        //    console.log(r);
        //    var form = Ext.getCmp('mktOfferForm');
        //    form.form.setValues(r);
        //});
        var r = storeSourceGoods.findRecord("offerid", offerid).data;
        let oWin = new mktOfferWinNew({ offerid: offerid });
        oWin.show(null, function () {
            var form = Ext.getCmp('mktOfferForm');
            form.form.setValues(r);
        });
    }
}

//组成成分
function OfferConstitute(offerid) {
    if (privilege("一键发包模块_询价管理-询价列表_组成成分")) {
        var r = storeSourceGoods.findRecord("offerid", offerid).data;
        let coWin = new offerConstituteWin({ offerid: offerid });
        coWin.show(null, function () {
            var form = Ext.getCmp('offerConstituteForm');
            form.form.setValues(r);
        });
    }
}

//定时刷新页面
function runSearch() {
    let task = {
        run: function () {
            getSourceGoodsListByPage(storeSourceGoods.currentPage);
            //runner.stop(task);//关闭定时器
        },
        interval: 20000//间隔时间1s
    }
    var runner = new Ext.util.TaskRunner();
    runner.start(task);
}
//------------------------------------------------------组成成分------------------------------------------------------------
Ext.define('offerConstituteWin', {
    extend: 'Ext.window.Window',
    height: 400,
    width: 440,
    layout: {
        type: 'fit'
    },
    modal: true,
    title: '组成成分',
    initComponent: function () {
        var me = this;
        var id = me.offerid;
        me.items = [
            {
                xtype: 'form',
                id: 'offerConstituteForm',
                bodyPadding: 10,
                items: [
                    {
                        xtype: 'numberfield',
                        name: 'estimateautomoney',
                        labelWidth: 90,
                        fieldLabel: '预估自动计算金额',
                        allowBlank: false,
                        minValue: 0,
                        anchor: '100%',
                        readOnly: true
                    },
                    {
                        xtype: 'numberfield',
                        name: 'estimatecompletemoney',
                        labelWidth: 90,
                        fieldLabel: '预估综合成本',
                        allowBlank: false,
                        minValue: 0,
                        anchor: '100%'
                    },
                    {
                        xtype: 'numberfield',
                        name: 'estimatetaxmoney',
                        labelWidth: 90,
                        fieldLabel: '预估税费成本',
                        allowBlank: false,
                        minValue: 0,
                        anchor: '100%'
                    },
                    {
                        xtype: 'numberfield',
                        name: 'estimatecostmoney',
                        labelWidth: 90,
                        fieldLabel: '预估资金成本',
                        allowBlank: false,
                        minValue: 0,
                        anchor: '100%'
                    }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '保存',
                        iconCls: 'dropyes',
                        id: 'errorSaveBtn',
                        handler: function () {
                            let form = Ext.getCmp('offerConstituteForm');
                            var values = form.form.getValues(false);
                            CS('CZCLZ.SourceGoods.UpdateConstituteOffer', function (retVal) {
                                if (retVal) {
                                    getSourceGoodsListByPage(1);
                                    Ext.MessageBox.alert('提示', "组成成分保存成功！");
                                }
                            }, CS.onError, id, values);
                            this.up('window').close();
                        }
                    }
                ]
            }];
        me.callParent(arguments);
    }
});

//------------------------------------------------------市场部企业报价弹出界面(new)------------------------------------------------------------
Ext.define('mktOfferWinNew', {
    extend: 'Ext.window.Window',
    height: 400,
    width: 440,
    layout: {
        type: 'fit'
    },
    modal: true,
    title: '市场部企业报价',
    initComponent: function () {
        var me = this;
        var id = me.offerid;
        me.items = [
            {
                xtype: 'form',
                id: 'mktOfferForm',
                bodyPadding: 10,
                items: [
                    {
                        xtype: 'numberfield',
                        name: 'estimateautomoney',
                        labelWidth: 90,
                        fieldLabel: '预估自动计算金额',
                        allowBlank: false,
                        minValue: 0,
                        anchor: '100%'
                    }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '保存',
                        iconCls: 'dropyes',
                        id: 'errorSaveBtn',
                        handler: function () {
                            let form = Ext.getCmp('mktOfferForm');
                            var values = form.form.getValues(false);
                            CS('CZCLZ.SourceGoods.UpdateMktOfferNew', function (retVal) {
                                if (retVal) {
                                    getSourceGoodsListByPage(1);
                                    Ext.MessageBox.alert('提示', "市场部企业报价成功！");
                                }
                            }, CS.onError, id, values);
                            this.up('window').close();
                        }
                    }
                ]
            }];
        me.callParent(arguments);
    }
});
//------------------------------------------------------市场部企业报价弹出界面------------------------------------------------------------
Ext.define('mktOfferWin', {
    extend: 'Ext.window.Window',
    height: 400,
    width: 440,
    layout: {
        type: 'fit'
    },
    modal: true,
    title: '市场部企业报价',
    initComponent: function () {
        var me = this;
        var id = me.offerid;
        me.items = [
            {
                xtype: 'form',
                id: 'mktOfferForm',
                bodyPadding: 10,
                items: [
                    {
                        xtype: 'numberfield',
                        name: 'totalmonetaryamount',
                        labelWidth: 90,
                        fieldLabel: '预估企业报价',
                        allowBlank: false,
                        minValue: 0,
                        anchor: '100%'
                    },
                    {
                        xtype: 'numberfield',
                        name: 'estimatemoney',
                        labelWidth: 90,
                        fieldLabel: '预估下游成本',
                        allowBlank: false,
                        minValue: 0,
                        anchor: '100%'
                    },
                    {
                        xtype: 'numberfield',
                        name: 'estimatecompletemoney',
                        labelWidth: 90,
                        fieldLabel: '预估综合成本',
                        allowBlank: false,
                        minValue: 0,
                        anchor: '100%'
                    },
                    {
                        xtype: 'numberfield',
                        name: 'estimatetaxmoney',
                        labelWidth: 90,
                        fieldLabel: '预估税费成本',
                        allowBlank: false,
                        minValue: 0,
                        anchor: '100%'
                    },
                    {
                        xtype: 'numberfield',
                        name: 'estimatecostmoney',
                        labelWidth: 90,
                        fieldLabel: '预估资金成本',
                        allowBlank: false,
                        minValue: 0,
                        anchor: '100%'
                    },
                    {
                        xtype: 'combobox',
                        fieldLabel: '下游对象',
                        labelWidth: 90,
                        anchor: '100%',
                        editable: false,
                        name: 'estimatecarriertype',
                        store: Ext.create('Ext.data.Store', {
                            fields: [
                                { name: 'value' },
                                { name: 'name' }
                            ],
                            data: [
                                { 'value': 0, 'name': '司机' },
                                { 'value': 1, 'name': '专线' }
                            ]
                        }),
                        queryMode: 'local',
                        displayField: 'name',
                        valueField: 'value',
                        readOnly: true
                    },
                    {
                        xtype: 'radiogroup',
                        fieldLabel: '是否有提货费',
                        items: [{
                            boxLabel: '提货',
                            name: 'istakegoodsbyestimate',
                            inputValue: 0
                        }, {
                            boxLabel: '不提货',
                            name: 'istakegoodsbyestimate',
                            inputValue: 1
                        }],
                        disabled: true
                    },
                    {
                        xtype: 'numberfield',
                        name: 'estimatetakegoodsmoney',
                        id: 'estimatetakegoodsmoney',
                        labelWidth: 90,
                        fieldLabel: '提货费金额',
                        minValue: 0,
                        anchor: '100%',
                        readOnly: true
                    },
                    {
                        xtype: 'radiogroup',
                        fieldLabel: '是否含票',
                        items: [{
                            boxLabel: '含票',
                            name: 'isvotebyestimate',
                            inputValue: 0
                        }, {
                            boxLabel: '不含',
                            name: 'isvotebyestimate',
                            inputValue: 1
                        }],
                        disabled: true
                    },
                    {
                        xtype: 'numberfield',
                        name: 'estimatevotemoney',
                        id: 'estimatevotemoney',
                        labelWidth: 90,
                        fieldLabel: '含票金额',
                        minValue: 0,
                        anchor: '100%',
                        readOnly: true
                    },
                    {
                        xtype: 'radiogroup',
                        fieldLabel: '是否含油',
                        items: [{
                            boxLabel: '含油',
                            name: 'isoilbyestimate',
                            inputValue: 0
                        }, {
                            boxLabel: '不含油',
                            name: 'isoilbyestimate',
                            inputValue: 1
                        }],
                        disabled: true
                    },
                    {
                        xtype: 'numberfield',
                        name: 'estimateoilmoney',
                        id: 'estimateoilmoney',
                        labelWidth: 90,
                        fieldLabel: '含油金额',
                        minValue: 0,
                        anchor: '100%',
                        readOnly: true
                    }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '保存',
                        iconCls: 'dropyes',
                        id: 'errorSaveBtn',
                        handler: function () {
                            let form = Ext.getCmp('mktOfferForm');
                            var values = form.form.getValues(false);
                            CS('CZCLZ.SourceGoods.UpdateMktOffer', function (retVal) {
                                if (retVal) {
                                    getSourceGoodsListByPage(1);
                                    Ext.MessageBox.alert('提示', "市场部企业报价成功！");
                                }
                            }, CS.onError, id, values);
                            this.up('window').close();
                        }
                    }
                ]
            }
        ];
        me.callParent(arguments);
    }
});

//------------------------------------------------------操作员填写运费弹出界面(new)------------------------------------------------------------
Ext.define('monetaryAmountWinNew', {
    extend: 'Ext.window.Window',
    height: 400,
    width: 400,
    layout: {
        type: 'fit'
    },
    modal: true,
    title: '操作员填写运费',
    initComponent: function () {
        var me = this;
        var id = me.offerid;
        me.items = [
            {
                xtype: 'form',
                id: 'monetaryAmountForm',
                bodyPadding: 10,
                items: [
                    {
                        xtype: 'numberfield',
                        name: 'estimatemoney',
                        labelWidth: 90,
                        fieldLabel: '下游成本',
                        allowBlank: false,
                        minValue: 0,
                        anchor: '100%'
                    },
                    {
                        xtype: 'numberfield',
                        name: 'estimatenooilmoney',
                        labelWidth: 90,
                        fieldLabel: '未用油金额',
                        allowBlank: false,
                        minValue: 0,
                        anchor: '100%'
                    }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '保存',
                        iconCls: 'dropyes',
                        id: 'errorSaveBtn',
                        handler: function () {
                            let form = Ext.getCmp('monetaryAmountForm');
                            var values = form.form.getValues(false);
                            CS('CZCLZ.SourceGoods.updateOfferNew', function (retVal) {
                                getSourceGoodsListByPage(storeSourceGoods.currentPage);
                                Ext.MessageBox.alert('提示', "运费填写成功！");
                            }, CS.onError, id, values);
                            this.up('window').close();
                        }
                    }
                ]
            }
        ];
        me.callParent(arguments);
    }
});

//------------------------------------------------------操作员填写运费弹出界面------------------------------------------------------------
Ext.define('monetaryAmountWin', {
    extend: 'Ext.window.Window',
    height: 400,
    width: 400,
    layout: {
        type: 'fit'
    },
    modal: true,
    title: '操作员填写运费',
    initComponent: function () {
        var me = this;
        var id = me.offerid;
        me.items = [
            {
                xtype: 'form',
                id: 'monetaryAmountForm',
                bodyPadding: 10,
                items: [
                    {
                        xtype: 'numberfield',
                        name: 'estimatemoney',
                        labelWidth: 90,
                        fieldLabel: '下游成本',
                        allowBlank: false,
                        minValue: 0,
                        anchor: '100%'
                    },
                    {
                        xtype: 'combobox',
                        fieldLabel: '下游对象',
                        labelWidth: 90,
                        anchor: '100%',
                        editable: false,
                        name: 'estimatecarriertype',
                        store: Ext.create('Ext.data.Store', {
                            fields: [
                                { name: 'value' },
                                { name: 'name' }
                            ],
                            data: [
                                { 'value': 0, 'name': '司机' },
                                { 'value': 1, 'name': '专线' }
                            ]
                        }),
                        queryMode: 'local',
                        displayField: 'name',
                        valueField: 'value'
                    },
                    {
                        xtype: 'radiogroup',
                        fieldLabel: '是否有提货费',
                        items: [{
                            boxLabel: '提货',
                            name: 'istakegoodsbyestimate',
                            inputValue: 0,
                            checked: true
                        }, {
                            boxLabel: '不提货',
                            name: 'istakegoodsbyestimate',
                            inputValue: 1
                        }],
                        listeners: {
                            'change': function (group, checked) {
                                var fun = Ext.getCmp('estimatetakegoodsmoney');
                                if (checked.istakegoodsbyestimate == 1) {
                                    fun.setValue(null);
                                    fun.setReadOnly(true);
                                } else {
                                    fun.setReadOnly(false);
                                }
                                //console.log(checked.istakegoodsbyactual);
                            }
                        }
                    },
                    {
                        xtype: 'numberfield',
                        name: 'estimatetakegoodsmoney',
                        id: 'estimatetakegoodsmoney',
                        labelWidth: 90,
                        fieldLabel: '提货费金额',
                        //allowBlank: false,
                        minValue: 0,
                        anchor: '100%'
                    },
                    {
                        xtype: 'radiogroup',
                        fieldLabel: '是否含票',
                        items: [{
                            boxLabel: '含票',
                            name: 'isvotebyestimate',
                            inputValue: 0,
                            checked: true
                        }, {
                            boxLabel: '不含',
                            name: 'isvotebyestimate',
                            inputValue: 1
                        }],
                        listeners: {
                            'change': function (group, checked) {
                                var fun = Ext.getCmp('estimatevotemoney');
                                if (checked.isvotebyestimate == 1) {
                                    fun.setValue(null);
                                    fun.setReadOnly(true);
                                } else {
                                    fun.setReadOnly(false);
                                }
                                //console.log(checked.istakegoodsbyactual);
                            }
                        }
                    },
                    {
                        xtype: 'numberfield',
                        name: 'estimatevotemoney',
                        id: 'estimatevotemoney',
                        labelWidth: 90,
                        fieldLabel: '含票金额',
                        //allowBlank: false,
                        minValue: 0,
                        anchor: '100%'
                    },
                    {
                        xtype: 'radiogroup',
                        fieldLabel: '是否含油',
                        items: [{
                            boxLabel: '含油',
                            name: 'isoilbyestimate',
                            inputValue: 0,
                            checked: true
                        }, {
                            boxLabel: '不含油',
                            name: 'isoilbyestimate',
                            inputValue: 1
                        }],
                        listeners: {
                            'change': function (group, checked) {
                                var fun = Ext.getCmp('estimateoilmoney');
                                if (checked.isoilbyestimate == 1) {
                                    fun.setValue(null);
                                    fun.setReadOnly(true);
                                } else {
                                    fun.setReadOnly(false);
                                }
                                //console.log(checked.istakegoodsbyactual);
                            }
                        }
                    },
                    {
                        xtype: 'numberfield',
                        name: 'estimateoilmoney',
                        id: 'estimateoilmoney',
                        labelWidth: 90,
                        fieldLabel: '含油金额',
                        //allowBlank: false,
                        minValue: 0,
                        anchor: '100%'
                    }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '保存',
                        iconCls: 'dropyes',
                        id: 'errorSaveBtn',
                        handler: function () {
                            let form = Ext.getCmp('monetaryAmountForm');
                            var values = form.form.getValues(false);
                            CS('CZCLZ.SourceGoods.updateOffer', function (retVal) {
                                if (retVal) {
                                    getSourceGoodsListByPage(1);
                                    Ext.MessageBox.alert('提示', "运费填写成功！");
                                }
                            }, CS.onError, id, values);
                            this.up('window').close();
                        }
                    }
                ]
            }
        ];
        me.callParent(arguments);
    }
});

//-----------------------------------------------------------界    面-----------------------------------------------------------------
Ext.define('SourceGoodsView', {
    extend: 'Ext.container.Viewport',

    layout: {
        type: 'fit'
    },

    initComponent: function () {
        var me = this;
        me.items = [
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
                        text: '操作',
                        dataIndex: 'offerid',
                        width: 130,
                        sortable: false,
                        menuDisabled: true,
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            var str = '';
                            if (record.data.flowstatus == 0) {
                                if (record.data.offerstatus == 0) {
                                    str += "<a onclick='ToWritePrice(\"" + value + "\");'>转操作部填报价格</a>";
                                }
                            } else if (record.data.flowstatus == 10) {
                                str += "<a onclick='EditTotalmonetaryamount(\"" + value + "\");'>操作员填写运费</a>";
                            } else if (record.data.flowstatus == 20) {
                                str += "<a onclick='MktOffer(\"" + value + "\");'>市场部企业报价</a>||";
                                str += "<a onclick='OfferConstitute(\"" + value + "\");'>组成成分</a>";
                            } else if (record.data.flowstatus == 90) {
                                str += "<a onclick='OfferConstitute(\"" + value + "\");'>组成成分</a>";
                            }
                            return str;
                        }
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

                            return "<input value='" + value + "' style='border:0px;BACKGROUND-COLOR: transparent;'>";
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
                        text: "备注"
                    }
                ],
                viewConfig: {
                    enableTextSelection: true,
                    getRowClass: function (record, rowIndex, rowParams, store) {
                        if (record.data.flowstatus == 10) {
                            return "x-grid-record-yellow";
                        } else if (record.data.flowstatus == 20) {
                            return "x-grid-record-pink";
                        } if (record.data.flowstatus == 90) {
                            return "x-grid-record-green";
                        }
                    }
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
                                            getSourceGoodsListByPage(1);
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
        ];
        me.callParent(arguments);
    }
});

Ext.onReady(function () {
    new SourceGoodsView();

    getSourceGoodsListByPage(1);

    //开启定时器 20s
    runSearch();
})