//-----------------------------------------------------------全局变量-----------------------------------------------------------------
var pageSize = 15;

//-----------------------------------------------------------数据源-------------------------------------------------------------------
var gxysStore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
        { name: 'addtime' },
        { name: 'oiltransfercode' },
        { name: 'oilcardcode' },
        { name: 'outuserid' },
        { name: 'money' },
        { name: 'zcxm' },
        { name: 'inuserid' },
        { name: 'zrxm' },
        { name: 'zrzh' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        DataBind(nPage);
    }
});

//-----------------------------------------------------------页面方法-----------------------------------------------------------------
function DataBind(nPage) {
    CS('CZCLZ.YKMag.GetHBList', function (retVal) {
        gxysStore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_oilcardcode").getValue(), Ext.getCmp("cx_oiltransfercode").getValue(), Ext.getCmp("cx_yhzh").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), 0);
}

//-----------------------------------------------------------界    面-----------------------------------------------------------------
Ext.define('GXYSView', {
    extend: 'Ext.container.Viewport',

    layout: {
        type: 'fit'
    },

    initComponent: function () {
        var me = this;
        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'gridpanel',
                    store: gxysStore,
                    columnLines: true,
                    columns: [
                        Ext.create('Ext.grid.RowNumberer'),
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'zxmc',
                            sortable: false,
                            menuDisabled: true,
                            text: "专线名称",
                            flex: 1
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'balance',
                            sortable: false,
                            menuDisabled: true,
                            text: "油卡剩余金额",
                            flex: 1
                        },
                        {
                            text: '操作',
                            dataIndex: 'ID',
                            sortable: false,
                            menuDisabled: true,
                            align: 'center',
                            width: 100,
                            renderer: function (v, s, r) {
                                var wh = '<a class="EditItem" href="javascript:void(0);" onclick="delGl(\'' + v + '\')">删除</a>';
                                return wh;
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
                                    id: 'cx_oilcardcode',
                                    width: 160,
                                    labelWidth: 60,
                                    fieldLabel: '专线名称'
                                },
                                {
                                    xtype: 'textfield',
                                    id: 'cx_oiltransfercode',
                                    width: 160,
                                    labelWidth: 60,
                                    fieldLabel: '归属地'
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
                                },
                                //{
                                //    xtype: 'buttongroup',
                                //    items: [
                                //        {
                                //            xtype: 'button',
                                //            iconCls: 'add',
                                //            text: '新增',
                                //            handler: function () {
                                //                CS('CZCLZ.YKMag.GetVisionList', function (retVal) {
                                //                    if (retVal) {
                                //                        var win = new addWin();
                                //                        win.show(null, function () {
                                //                            Ext.getCmp("carriageoil").setValue(retVal[0]["carriageoil"]);
                                //                        })
                                //                    }
                                //                }, CS.onError);

                                //            }
                                //        }
                                //    ]
                                //},
                                //{
                                //    xtype: 'buttongroup',
                                //    title: '',
                                //    items: [
                                //        {
                                //            xtype: 'button',
                                //            iconCls: 'view',
                                //            text: '导出',
                                //            handler: function () {
                                //                DownloadFile("CZCLZ.YKMag.GetGXYSListToFile", "干线运输划拨.xls", Ext.getCmp("cx_oilcardcode").getValue(), Ext.getCmp("cx_oiltransfercode").getValue(), Ext.getCmp("cx_yhzh").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), 1);
                                //            }
                                //        }
                                //    ]
                                //}
                            ]
                        }
                    ]
                }
            ]
        });
        me.callParent(arguments);
    }
});

//Ext.onReady(function () {
//    new GXYSView();
//    DataBind();
//})