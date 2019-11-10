var pageSize = 15;
var cx_name;
var cx_dj;
var id = "";
//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'UserID' },
       { name: 'username' },
       { name: 'dqbm'}
       
    ],
    onPageChange: function (sto, nPage, sorters) {
        GetFHRTJList(nPage);
    }
});
//************************************数据源*****************************************

//************************************页面方法***************************************
function GetFHRTJList(nPage) {
    if (Ext.getCmp("cx_fgcs").getValue() < 0) {
        Ext.MessageBox.alert('提示', '复购次数不能小于0！');
        return;
    }
    if (Ext.getCmp("cx_hb").getValue() < 0) {
        Ext.MessageBox.alert('提示', '红包剩余金额不能小于0！');
        return;
    }
    if (Ext.getCmp("cx_lxgs").getValue() != "" && Ext.getCmp("cx_lxgs").getValue() != null
        && Ext.getCmp("cx_lxts").getValue() != "" && Ext.getCmp("cx_lxts").getValue() != null
        && Ext.getCmp("cx_lxcs").getValue() != "" && Ext.getCmp("cx_lxcs").getValue() != null) {
       
    } else if (Ext.getCmp("cx_lxgs").getValue() == null&& Ext.getCmp("cx_lxts").getValue() == null&& Ext.getCmp("cx_lxcs").getValue() == null) { } else {
        Ext.MessageBox.alert('提示', '请填写完整连续h个n天的订单数在m次以内！');
        return;
    }


    CS('CZCLZ.FHRMag.GetFHRTJList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), Ext.getCmp("cx_fgcs").getValue(), Ext.getCmp("cx_hb").getValue()
    , Ext.getCmp("cx_city").getValue(), Ext.getCmp("cx_lxgs").getValue(), Ext.getCmp("cx_lxts").getValue(), Ext.getCmp("cx_lxcs").getValue());
}

//************************************页面方法***************************************


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
                    columns: [Ext.create('Ext.grid.RowNumberer'),
                         {
                             xtype: 'gridcolumn',
                             dataIndex: 'id',
                             sortable: false,
                             menuDisabled: true,
                             hidden: true,
                             flex: 1
                         },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'username',
                                sortable: false,
                                menuDisabled: true,
                                flex: 1,
                                text: "手机号"
                            },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'dqbm',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: "城市",
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    var str = "";
                                    if (value == "320500") {
                                        str = "苏州";
                                    } else if (value == "320200") {
                                        str = "无锡";
                                    } else {
                                        str = "常州";
                                    }
                                    return str;
                                }
                             },

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
                                            fieldLabel: '开始时间',
                                            format: 'Y-m-d',
                                            labelWidth: 60,
                                            width: 190
                                        },
                                        {
                                            id: 'cx_end',
                                            xtype: 'datefield',
                                            format: 'Y-m-d',
                                            fieldLabel: '结束时间',
                                            labelWidth: 60,
                                            width: 190
                                        },
                                        {
                                            id: 'cx_fgcs',
                                            xtype: 'numberfield',
                                            fieldLabel: '复购次数',
                                            allowDecimals: false,
                                            allowNegative: false,
                                            minValue:0,
                                            labelWidth: 60,
                                            width: 190
                                        },
                                        {
                                            xtype: 'displayfield',
                                            value: "次以内"
                                        },
                                        {
                                            id: 'cx_hb',
                                            xtype: 'numberfield',
                                            fieldLabel: '红包剩余金额',
                                            allowNegative: false,
                                            minValue: 0,
                                            labelWidth: 80,
                                            width: 190
                                        },
                                         {
                                             xtype: 'combobox',
                                             id: 'cx_city',
                                             width: 160,
                                             fieldLabel: '城市',
                                             editable: false,
                                             labelWidth: 40,
                                             store: Ext.create('Ext.data.Store', {
                                                 fields: [
                                                    { name: 'val' },
                                                    { name: 'txt' }
                                                 ],
                                                 data: [
                                                     {
                                                         'val': '',
                                                         'txt': '全部',
                                                     },
                                                     {
                                                         'val': '320400',
                                                         'txt': '常州',
                                                     },
                                                     {
                                                         'val': '320500',
                                                         'txt': '苏州',
                                                     },
                                                     {
                                                         'val': '320200',
                                                         'txt': '无锡',
                                                     },
                                                 ]
                                             }),
                                             queryMode: 'local',
                                             displayField: 'txt',
                                             valueField: 'val',
                                             value: ''
                                         }
                                    ]
                                },
                                {
                                    xtype: 'toolbar',
                                    dock: 'top',
                                    items: [
                                        {
                                            xtype: 'displayfield',
                                            value: "连续"
                                        },
                                        {
                                            id: 'cx_lxgs',
                                            xtype: 'numberfield',
                                            allowDecimals: false,
                                            allowNegative: false,
                                            minValue: 0,
                                            width: 100
                                        },
                                        {
                                            xtype: 'displayfield',
                                            value: "个"
                                        },
                                        {
                                            id: 'cx_lxts',
                                            xtype: 'numberfield',
                                            allowDecimals: false,
                                            allowNegative: false,
                                            minValue: 0,
                                            width: 100
                                        },
                                        {
                                            xtype: 'displayfield',
                                            value: "天的订单数在"
                                        },
                                         {
                                             id: 'cx_lxcs',
                                             xtype: 'numberfield',
                                             allowDecimals: false,
                                             allowNegative: false,
                                             minValue: 0,
                                             width: 100
                                         },
                                         {
                                             xtype: 'displayfield',
                                             allowDecimals: false,
                                             allowNegative: false,
                                             minValue: 0,
                                             value: "次以内"
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
                                                        if (privilege("财务报表_发货人统计_查看")) {
                                                            GetFHRTJList(1);
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
                                                        text: '导出',
                                                        handler: function () {
                                                            if (privilege("财务报表_发货人统计_导出")) {
                                                                DownloadFile("CZCLZ.FHRMag.GetFHRTJListToFile", "发货人统计.xls", Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), Ext.getCmp("cx_fgcs").getValue(), Ext.getCmp("cx_hb").getValue()
                                                                , Ext.getCmp("cx_city").getValue(), Ext.getCmp("cx_lxgs").getValue(), Ext.getCmp("cx_lxts").getValue(), Ext.getCmp("cx_lxcs").getValue());
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

    GetFHRTJList(1);

})
//************************************主界面*****************************************