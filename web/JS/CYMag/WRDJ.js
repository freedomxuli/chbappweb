//-----------------------------------------------------------全局变量-----------------------------------------------------------------
var pageSize = 15;
var pages = 1;

//-----------------------------------------------------------数据源-------------------------------------------------------------------
var carriageStore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
        { name: 'carriageid' },
        { name: 'carriagecode' },
        { name: 'carriagetime' },
        { name: 'carriagefromprovince' },
        { name: 'carriagefromcity' },
        { name: 'carriagetoprovince' },
        { name: 'carriagetocity' },
        { name: 'driverid' },
        { name: 'carriagepoints' },
        { name: 'carriageoilmoney' },
        { name: 'carriagemoney' },
        { name: 'status' },
        { name: 'carriagestatus' },
        { name: 'memo' },
        { name: 'ispayinsurance' },
        { name: 'insuranceid' },
        { name: 'insurancemoney' },
        { name: 'carriagemoneynew' },
        { name: 'addtime' },
        { name: 'adduser' },
        { name: 'userid' },
        { name: 'isoilpay' },
        { name: 'ismoneypay' },
        { name: 'isarrive' },
        { name: 'ismoneynewpay' },
        { name: 'sjzh' },
        { name: 'sjxm' },
        { name: 'sjdh' },
        { name: 'sjcarnumber' },
        { name: 'zx' },
        { name: 'isinvoice' },
        { name: 'modetype' },
        { name: 'modecoefficient' },
        { name: 'caruser' },
        { name: 'carriagegetmode' },
        { name: 'kp' },
        { name: 'ispushwr' },
        { name: 'fjid0' },
        { name: 'fjurl0' },
        { name: 'fjid1' },
        { name: 'fjurl1' },
        { name: 'fjid2' },
        { name: 'fjurl2' }
    ],
    onPageChange: function (sto, nPage, sorters) {
		pages = nPage;
        DataBind(nPage);
    }
});

//-----------------------------------------------------------页面方法-----------------------------------------------------------------
function DataBind(nPage) {
    CS('CZCLZ.CYMag.GetWrList', function (retVal) {
        carriageStore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_carriagecode").getValue(), Ext.getCmp("cx_UserXM").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), Ext.getCmp("cx_isinvoice").getValue(), Ext.getCmp("cx_ismoneypay").getValue());
}

function TSWR(carriageid) {
    var r = carriageStore.findRecord("carriageid", carriageid).data;
    console.log(r);
    var win = new tswrWin({ carriageid: carriageid });
    win.show(null, function () {
        var isDefault = false;
        Ext.getCmp('uploadpic0').add(new SelectImg({
            isSelected: isDefault,
            src: r.fjurl0,
            fileid: r.fjid0
        }));
        var isDefault = false;
        Ext.getCmp('uploadpic1').add(new SelectImg({
            isSelected: isDefault,
            src: r.fjurl1,
            fileid: r.fjid1
        }));
        var isDefault = false;
        Ext.getCmp('uploadpic2').add(new SelectImg({
            isSelected: isDefault,
            src: r.fjurl2,
            fileid: r.fjid2
        }));
    })
}
//上传图片
function tp(id, type) {
    var win = new phWin({ carriageid: id, type: type });
    win.show();
}

//-----------------------------------------------------------------附件展示-----------------------------------------------------------------
Ext.define('SelectImg', {
    extend: 'Ext.Img',
    height: 390,
    width: 360,
    constructor: function (config) {
        var me = this;
        config = config || {};
        config.cls = config.isSelected ? "clsSelected" : "clsUnselected";
        me.callParent([config]);
        me.on('render', function () {
            Ext.fly(me.el).on('click', function () {
                window.open(me.src);
                var oldSelectImg = Ext.getCmp('uploadpicC').query('image[isSelected=true]');
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

//-----------------------------------------------------------------附件-----------------------------------------------------------------
Ext.define('phWin', {
    extend: 'Ext.window.Window',
    title: "上传",
    height: 160,
    width: 560,
    modal: true,
    layout: 'border',
    id: 'sbBdwin',
    initComponent: function () {
        var me = this;
        var carriageid = me.carriageid;
        var type = me.type;
        me.items = [{
            xtype: 'UploaderPanel',
            id: 'uploadpicC',
            region: 'center',
            layout: 'column',

            autoScroll: true,
            items: [
                {
                    xtype: 'filefield',
                    allowBlank: false,
                    labelWidth: 65,
                    fieldLabel: '图片上传',
                    buttonText: '浏览',
                    columnWidth: 0.8
                },
                {
                    xtype: 'button',
                    text: '上传',
                    iconCls: 'upload',
                    columnWidth: 0.2,
                    margin: '0 5',
                    handler: function () {
                        Ext.getCmp('uploadpicC').upload('CZCLZ.CYMag.UploadPicForcarriage', function (retVal) {
                            var isDefault = false;
                            if (retVal.isdefault == 1) {
                                isDefault = true;
                            }
                            Ext.getCmp('uploadpic' + type).removeAll();
                            Ext.getCmp('uploadpic' + type).add(new SelectImg({
                                isSelected: isDefault,
                                src: retVal.fileurl,
                                fileid: retVal.fileid
                            }));
                            me.close();
                        }, CS.onError, carriageid, type);
                    }
                }
            ]

        }];
        me.callParent(arguments);
    }
});
//-----------------------------------------------------------推送物润界面-----------------------------------------------------------------
Ext.define('tswrWin', {
    extend: 'Ext.window.Window',
    id: "tswrWin",
    height: 500,
    width: 1115,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '物润推送',
    initComponent: function () {
        var me = this;
        var id = me.carriageid;
        me.items = [
            {
                xtype: 'panel',
                bodyPadding: 10,
                split: true,
                layout: {
                    type: 'hbox',
                    align: 'stretch'
                },
                items: [
                    {
                        xtype: 'panel',
                        flex: 1,
                        layout: 'fit',
                        buttonAlign: 'center',
                        buttons: [
                            {
                                text: '银行支付凭证上传',
                                handler: function () {
                                    tp(id, 0);
                                }
                            }
                        ],
                        items: [
                            {
                                xtype: 'UploaderPanel',
                                id: 'uploadpic0',
                                region: 'center',
                                autoScroll: true
                            }
                        ]
                    },
                    {
                        xtype: 'panel',
                        flex: 1,
                        layout: 'fit',
                        buttonAlign: 'center',
                        buttons: [
                            {
                                text: '回单上传',
                                handler: function () {
                                    tp(id, 2);
                                }
                            }
                        ],
                        items: [
                            {
                                xtype: 'UploaderPanel',
                                id: 'uploadpic2',
                                region: 'center',
                                autoScroll: true
                            }
                        ]
                    },
                    {
                        xtype: 'panel',
                        flex: 1,
                        layout: 'fit',
                        buttonAlign: 'center',
                        buttons: [
                            {
                                text: '合同上传',
                                handler: function () {
                                    tp(id, 1);
                                }
                            }
                        ],
                        items: [
                            {
                                xtype: 'UploaderPanel',
                                id: 'uploadpic1',
                                region: 'center',
                                autoScroll: true
                            }
                        ]
                    }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '合同模板下载',
                        handler: function () {
                            DownloadFile("CZCLZ.CYMag.DealHt", "合同模板.doc", id);
                        }
                    },
                    {
                        text: '支付明细模板下载',
                        handler: function () {
                            DownloadFile("CZCLZ.CYMag.DealPayLine", "支付明细模板.doc","");
                        }
                    },
                    {
                        text: '回单模板下载',
                        handler: function () {
                            DownloadFile("CZCLZ.CYMag.DealHd", "回单模板.xls", id);
                        }
                    },
                    {
                        text: '推送物润',
                        handler: function () {
                            Ext.MessageBox.confirm("提示", "是否推送？", function (obj) {
                                if (obj == "yes") {
                                    CS('CZCLZ.CYMag.PushWr', function (retVal) {
                                        Ext.Msg.alert('提醒', retVal);
                                        DataBind(pages);
                                        Ext.getCmp("tswrWin").close();
                                    }, CS.onError, id);
                                }
                            });
                        }
                    },
                    {
                        text: '修改地址',
                        handler: function () {
                           
                            SetStartAdd(id);
                        }
                    },
                    {
                        text: '修改推送',
                        handler: function () {
							Ext.MessageBox.confirm("提示", "是否推送？", function (obj) {
							    if (obj == "yes") {
							        CS('CZCLZ.CYMag.ChangeTS', function (retVal) {
							            Ext.Msg.alert('提醒', retVal);
							            DataBind(pages);
							            Ext.getCmp("tswrWin").close();
							        }, CS.onError, id);
							    }
							});
                        }
                    }
                ]
            }
        ];
        me.callParent(arguments);
    }
});

//-----------------------------------------------------------界    面-----------------------------------------------------------------
Ext.define('CYDView', {
    extend: 'Ext.container.Viewport',

    layout: {
        type: 'fit'
    },

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'gridpanel',
                id: 'cyGrid',
                title: '',
                store: carriageStore,
                columnLines: true,
                viewConfig: {

                },
                columns: [
                    Ext.create('Ext.grid.RowNumberer'),//专线名称，司机手机、车牌、是否保险、保费、是否油卡付款、是否现金付款
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'carriageid',
                        sortable: false,
                        menuDisabled: true,
                        text: "操作",
                        width: 100,
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            //if (record.data.ispushwr == 1) {
                            var str = "<a onclick='TSWR(\"" + value + "\");'>推送物润</a>";
                            return str;
                            //}
                        }
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'ispushwr',
                        sortable: false,
                        menuDisabled: true,
                        text: "推送状态",
                        width: 100,
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            if (value == 1) {
                                return "未推送";
                            } else if (value == 0) {
                                return "已推送";
                            } 
                        }
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'isinvoice',
                        sortable: false,
                        menuDisabled: true,
                        text: "票据状态",
                        width: 100,
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            if (value == 1) {
                                return "已开";
                            } else if (value == 0) {
                                return "未开";
                            } else if (value == 2) {
                                return "已收票";
                            }
                        }
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'zx',
                        sortable: false,
                        menuDisabled: true,
                        text: "上游客户（专线名称）",
                        width: 200
                    },
                    {
                        xtype: 'datecolumn',
                        dataIndex: 'carriagetime',
                        sortable: false,
                        menuDisabled: true,
                        text: "运输日期",
                        format: 'Y-m-d H:i:s',
                        width: 200
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'carriagefromprovince',
                        sortable: false,
                        menuDisabled: true,
                        text: "起运地",
                        width: 150,
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            return record.data.carriagefromprovince + record.data.carriagefromcity;

                        }
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'carriagefromprovince',
                        sortable: false,
                        menuDisabled: true,
                        text: "目的地",
                        width: 150,
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            return record.data.carriagetoprovince + record.data.carriagetocity;

                        }
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'carriagepoints',
                        sortable: false,
                        menuDisabled: true,
                        text: "支付券额/运费",
                        width: 100
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'sjzh',
                        sortable: false,
                        menuDisabled: true,
                        text: "下游司机",
                        width: 100
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'sjxm',
                        sortable: false,
                        menuDisabled: true,
                        text: "司机姓名",
                        width: 100
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'sjdh',
                        sortable: false,
                        menuDisabled: true,
                        text: "电话",
                        width: 100
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'sjcarnumber',
                        sortable: false,
                        menuDisabled: true,
                        text: "车牌",
                        width: 100
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'carriageoilmoney',
                        sortable: false,
                        menuDisabled: true,
                        text: "油卡",
                        width: 100
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'carriagemoney',
                        sortable: false,
                        menuDisabled: true,
                        text: "现付现金",
                        width: 100
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'carriagemoneynew',
                        sortable: false,
                        menuDisabled: true,
                        text: "验收付现金",
                        width: 100
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'insurancemoney',
                        sortable: false,
                        menuDisabled: true,
                        text: "保费",
                        width: 100,
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            var str = "";
                            if (value != null && value != "") {
                                str = value / 100
                            }
                            return str;
                        }
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'isoilpay',
                        sortable: false,
                        menuDisabled: true,
                        text: "是否油卡付款",
                        width: 100,
                        renderer: function (v, m) {
                            str = "";
                            if (v == 0) {
                                str = "否";
                            } else if (v == 1) {
                                str = "是";
                            }
                            return str;
                        }
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'ismoneypay',
                        sortable: false,
                        menuDisabled: true,
                        text: "是否现金付款",
                        width: 100,
                        renderer: function (v, m) {
                            str = "";
                            if (v == 0) {
                                str = "否";
                            } else if (v == 1) {
                                str = "是";
                            }
                            return str;
                        }
                    },
                     {
                         xtype: 'gridcolumn',
                         dataIndex: 'carriagecode',
                         sortable: false,
                         menuDisabled: true,
                         text: "承运单号",
                         width: 200
                     },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'carriagestatus',
                        sortable: false,
                        menuDisabled: true,
                        text: "订单状态",
                        width: 200,
                        renderer: function (v, m) {
                            str = "";
                            if (v == 0) {
                                str = "已申请待后台审核";
                            } else if (v == 10) {
                                str = "司机确认待后台审核";
                            } else if (v == 11) {
                                str = "司机拒绝待后台审核";
                            } else if (v == 20) {
                                str = "后台已审核待专线付款";
                            } else if (v == 21) {
                                str = "后台拒绝申请";
                            } else if (v == 30) {
                                str = "专线支付券额运输开始";
                            } else if (v == 40) {
                                str = "司机确认到货待专线确认";
                            } else if (v == 50) {
                                str = "专线确认到货待后台结款";
                            } else if (v == 90) {
                                str = "后台确认结款，承运完成";
                            }
                            return str;
                        }
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'caruser',
                        sortable: false,
                        menuDisabled: true,
                        text: "车主账号",
                        width: 100
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'kp',
                        sortable: false,
                        menuDisabled: true,
                        text: "开票抬头",
                        width: 100
                    }
                ],
                dockedItems: [
                    {
                        xtype: 'toolbar',
                        dock: 'top',
                        items: [
                            {
                                xtype: 'textfield',
                                id: 'cx_carriagecode',
                                width: 160,
                                labelWidth: 60,
                                fieldLabel: '承运单号'
                            },
                            {
                                xtype: 'textfield',
                                id: 'cx_UserXM',
                                width: 160,
                                labelWidth: 60,
                                fieldLabel: '承运专线'
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
                                xtype: 'combobox',
                                id: 'cx_isinvoice',
                                width: 180,
                                fieldLabel: '是否开票',
                                editable: false,
                                labelWidth: 60,
                                store: Ext.create('Ext.data.Store', {
                                    fields: [
                                        { name: 'val' },
                                        { name: 'txt' }
                                    ],
                                    data: [
                                        { 'val': "", 'txt': '全部' },
                                        { 'val': 0, 'txt': '未开' },
                                        { 'val': 1, 'txt': '已开' },
                                        { 'val': 2, 'txt': '已收票' }]

                                }),
                                queryMode: 'local',
                                displayField: 'txt',
                                valueField: 'val',
                                value: ''
                            },
                            {
                                xtype: 'combobox',
                                id: 'cx_carriagestatus',
                                width: 250,
                                fieldLabel: '订单状态',
                                editable: false,
                                labelWidth: 60,
                                store: Ext.create('Ext.data.Store', {
                                    fields: [
                                        { name: 'val' },
                                        { name: 'txt' }
                                    ],
                                    data: [
                                        { 'val': 90, 'txt': '后台确认结款，承运完成' }]

                                }),
                                queryMode: 'local',
                                displayField: 'txt',
                                valueField: 'val',
                                value: 90,
                                hidden: true
                            },
                            {
                                xtype: 'combobox',
                                id: 'cx_ismoneypay',
                                width: 180,
                                fieldLabel: '是否支付',
                                editable: false,
                                labelWidth: 60,
                                store: Ext.create('Ext.data.Store', {
                                    fields: [
                                        { name: 'val' },
                                        { name: 'txt' }
                                    ],
                                    data: [
                                        { 'val': "", 'txt': '全部' },
                                        { 'val': "1", 'txt': '已支付现付' },
                                        { 'val': "0", 'txt': '未支付现付' }]

                                }),
                                queryMode: 'local',
                                displayField: 'txt',
                                valueField: 'val',
                                value: ''
                            },
                            {
                                xtype: 'buttongroup',
                                items: [
                                    {
                                        xtype: 'button',
                                        iconCls: 'search',
                                        text: '查询',
                                        handler: function () {
                                            if (privilege("承运模块_专线承运单查询_查看")) {
                                                DataBind(1);
                                            }
                                        }
                                    }
                                ]
                            },
                            {
                                xtype: 'buttongroup',
                                items: [
                                    {
                                        xtype: 'button',
                                        text: '导出',
                                        iconCls: 'view',
                                        handler: function () {
                                            if (privilege("承运模块_专线承运单查询_导出")) {
                                                DownloadFile("CZCLZ.CYMag.GetCYDListToFile", "承运单.xls", Ext.getCmp("cx_carriagecode").getValue(), Ext.getCmp("cx_UserXM").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), Ext.getCmp("cx_isinvoice").getValue(), Ext.getCmp("cx_ismoneypay").getValue());
                                            }
                                        }
                                    }
                                ]
                            }
                        ]
                    },
                    {
                        xtype: 'pagingtoolbar',
                        displayInfo: true,
                        store: carriageStore,
                        dock: 'bottom'
                    }
                ]
            }
        ];
        me.callParent(arguments);
    }
});

Ext.onReady(function () {
    new CYDView();
    DataBind(1);
});





function SetStartAdd(carriageid) {


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
        title: "修改地址",
        modal: true,
        items: [
           {
               xtype: 'form',
               id: 'StartDQfrom',
               bodyPadding: 10,
               items: [
                    
                   {
                       xtype: 'textfield',
                       fieldLabel: '收货地址',
                       id: 'wrshd',
                       name: 'wrshd',
                       labelWidth: 70,
                        anchor: '100%'
                   },
                   {
                       xtype: 'textfield',
                       fieldLabel: '起始地址',
                       id: 'wrqsd',
                       name: 'wrqsd',
                       labelWidth: 70,
                        anchor: '100%'
                   }

               ],
               buttonAlign: 'center',
               buttons: [

                   {
                       text: '保存',
                       iconCls: 'dropyes',
                       handler: function () {
 
                           CS('CZCLZ.CYMag.Savecarriage', function (retVal) {
                               if (retVal) {
                                   
                                   Ext.getCmp("StartDQ").close();

                               }
                           }, CS.onError, Ext.getCmp("wrqsd").getValue(), Ext.getCmp("wrshd").getValue(), carriageid);
                       }
                   },
                    {
                        text: '取消',
                        iconCls: 'back',
                        handler: function () {
                            Ext.getCmp("StartDQ").close();

                        }
                    }
               ]
           }
        ]
    });
     f_window.show();

 
     CS('CZCLZ.CYMag.Getcarriage', function (retVal) {
         if (retVal) {
             Ext.getCmp("wrqsd").setValue(retVal.wrqsd);
             Ext.getCmp("wrshd").setValue(retVal.wrshd);
         }
     }, CS.onError, carriageid);




}
