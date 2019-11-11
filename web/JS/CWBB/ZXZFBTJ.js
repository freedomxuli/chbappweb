//-----------------------------------------------------------全局变量-----------------------------------------------------------------
var pageSize = 15;

//-----------------------------------------------------------数据源-------------------------------------------------------------------
var shareStore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'UserName' },
       { name: 'UserXM' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getTj(nPage);
    }
});

//-----------------------------------------------------------页面方法-----------------------------------------------------------------
function getTj(nPage) {
    var arr = [];
    var cx_gzs = Ext.getCmp("cx_gzs").getValue();
    arr.push(cx_gzs);
    var cx_gzs2 = Ext.getCmp("cx_gzs2").getValue();
    arr.push(cx_gzs2);
    var cx_zhpf = Ext.getCmp("cx_zhpf").getValue();
    arr.push(cx_zhpf);
    var cx_zhpf2 = Ext.getCmp("cx_zhpf2").getValue();
    arr.push(cx_zhpf2);
    var cx_pjs = Ext.getCmp("cx_pjs").getValue();
    arr.push(cx_pjs);
    var cx_pjs2 = Ext.getCmp("cx_pjs2").getValue();
    arr.push(cx_pjs2);
    var cx_dprs = Ext.getCmp("cx_dprs").getValue();
    arr.push(cx_dprs);
    var cx_dprs2 = Ext.getCmp("cx_dprs2").getValue();
    arr.push(cx_dprs2);
    var cx_gmcs = Ext.getCmp("cx_gmcs").getValue();
    arr.push(cx_gmcs);
    var cx_gmcs2 = Ext.getCmp("cx_gmcs2").getValue();
    arr.push(cx_gmcs2);
    var cx_gmrs = Ext.getCmp("cx_gmrs").getValue();
    arr.push(cx_gmrs);
    var cx_gmrs2 = Ext.getCmp("cx_gmrs2").getValue();
    arr.push(cx_gmrs2);
    var cx_zfbcs = Ext.getCmp("cx_zfbcs").getValue();
    arr.push(cx_zfbcs);
    var cx_zfbcs2 = Ext.getCmp("cx_zfbcs2").getValue();
    arr.push(cx_zfbcs2);
    var cx_city = Ext.getCmp("cx_city").getValue();
    arr.push(cx_city);
    var cx_isSq = Ext.getCmp("cx_isSq").getValue();
    arr.push(cx_isSq);
    var cx_isRz = Ext.getCmp("cx_isRz").getValue();
    arr.push(cx_isRz);
    var cx_tkl = Ext.getCmp("cx_tkl").getValue();
    arr.push(cx_tkl);
    var cx_tkl2 = Ext.getCmp("cx_tkl2").getValue();
    arr.push(cx_tkl2);

    CS('CZCLZ.CWBBMag.GetZxzfbTjByPage', function (retVal) {
        shareStore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, arr);
}

//-----------------------------------------------------------界    面-----------------------------------------------------------------
Ext.onReady(function () {
    Ext.define('iView', {
        extend: 'Ext.container.Viewport',

        layout: {
            type: 'fit'
        },

        initComponent: function () {
            var me = this;
            me.items = [
                {
                    xtype: 'gridpanel',
                    store: shareStore,
                    columnLines: true,
                    columns: [Ext.create('Ext.grid.RowNumberer'),
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'UserName',
                            sortable: false,
                            menuDisabled: true,
                            width: 130,
                            text: "账号"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'UserXM',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: "名称"
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
                                    id: 'cx_gzs',
                                    xtype: 'numberfield',
                                    fieldLabel: '关注数',
                                    labelWidth: 100,
                                    width: 190
                                },
                                {
                                    id: 'cx_gzs2',
                                    xtype: 'numberfield',
                                    fieldLabel: '~',
                                    labelWidth: 10,
                                    width: 90,
                                    labelSeparator: ''
                                },
                                {
                                    id: 'cx_zhpf',
                                    xtype: 'numberfield',
                                    fieldLabel: '综合评分',
                                    labelWidth: 100,
                                    width: 190
                                },
                                {
                                    id: 'cx_zhpf2',
                                    xtype: 'numberfield',
                                    fieldLabel: '~',
                                    labelWidth: 10,
                                    width: 90,
                                    labelSeparator: ''
                                },
                                {
                                    id: 'cx_pjs',
                                    xtype: 'numberfield',
                                    fieldLabel: '评价数',
                                    labelWidth: 100,
                                    width: 190
                                },
                                {
                                    id: 'cx_pjs2',
                                    xtype: 'numberfield',
                                    fieldLabel: '~',
                                    labelWidth: 10,
                                    width: 90,
                                    labelSeparator: ''
                                },
                                {
                                    id: 'cx_dprs',
                                    xtype: 'numberfield',
                                    fieldLabel: '点评人数',
                                    labelWidth: 100,
                                    width: 190
                                },
                                {
                                    id: 'cx_dprs2',
                                    xtype: 'numberfield',
                                    fieldLabel: '~',
                                    labelWidth: 10,
                                    width: 90,
                                    labelSeparator: ''
                                },
                                {
                                    id: 'cx_gmcs',
                                    xtype: 'numberfield',
                                    fieldLabel: '购买次数',
                                    labelWidth: 100,
                                    width: 190
                                },
                                {
                                    id: 'cx_gmcs2',
                                    xtype: 'numberfield',
                                    fieldLabel: '~',
                                    labelWidth: 10,
                                    width: 90,
                                    labelSeparator: ''
                                }
                            ]
                        },
                        {
                            xtype: 'toolbar',
                            dock: 'top',
                            items: [
                                {
                                    id: 'cx_gmrs',
                                    xtype: 'numberfield',
                                    fieldLabel: '购买人数',
                                    labelWidth: 100,
                                    width: 190
                                },
                                {
                                    id: 'cx_gmrs2',
                                    xtype: 'numberfield',
                                    fieldLabel: '~',
                                    labelWidth: 10,
                                    width: 90,
                                    labelSeparator: ''
                                },
                                {
                                    id: 'cx_zfbcs',
                                    xtype: 'numberfield',
                                    fieldLabel: '专线自发布次数',
                                    labelWidth: 100,
                                    width: 190
                                },
                                {
                                    id: 'cx_zfbcs2',
                                    xtype: 'numberfield',
                                    fieldLabel: '~',
                                    labelWidth: 10,
                                    width: 90,
                                    labelSeparator: ''
                                },
                                {
                                    xtype: 'combobox',
                                    id: 'cx_city',
                                    width: 190,
                                    fieldLabel: '城市',
                                    editable: false,
                                    labelWidth: 70,
                                    store: Ext.create('Ext.data.Store', {
                                        fields: [
                                            { name: 'val' },
                                            { name: 'txt' }
                                        ],
                                        data: [
                                            { 'val': "", 'txt': '全部' },
                                            { 'val': "320400", 'txt': '常州' },
                                            { 'val': "320500", 'txt': '苏州' },
                                            { 'val': "320200", 'txt': '无锡' }]

                                    }),
                                    queryMode: 'local',
                                    displayField: 'txt',
                                    valueField: 'val',
                                    value: ''
                                },
                                {
                                    xtype: 'combobox',
                                    id: 'cx_isSq',
                                    width: 190,
                                    fieldLabel: '是否开通授权平台',
                                    editable: false,
                                    labelWidth: 110,
                                    store: Ext.create('Ext.data.Store', {
                                        fields: [
                                            { name: 'val' },
                                            { name: 'txt' }
                                        ],
                                        data: [
                                            { 'val': "", 'txt': '全部' },
                                            { 'val': 0, 'txt': '已授权' },
                                            { 'val': 1, 'txt': '待授权' }]

                                    }),
                                    queryMode: 'local',
                                    displayField: 'txt',
                                    valueField: 'val',
                                    value: ''
                                },
                                {
                                    xtype: 'combobox',
                                    id: 'cx_isRz',
                                    width: 180,
                                    fieldLabel: '是否为平台认证',
                                    editable: false,
                                    labelWidth: 110,
                                    store: Ext.create('Ext.data.Store', {
                                        fields: [
                                            { name: 'val' },
                                            { name: 'txt' }
                                        ],
                                        data: [
                                            { 'val': "", 'txt': '全部' },
                                            { 'val': 0, 'txt': '是' },
                                            { 'val': 1, 'txt': '否' }]

                                    }),
                                    queryMode: 'local',
                                    displayField: 'txt',
                                    valueField: 'val',
                                    value: ''
                                },
                                {
                                    id: 'cx_tkl',
                                    xtype: 'numberfield',
                                    fieldLabel: '退款率',
                                    labelWidth: 100,
                                    width: 190
                                },
                                {
                                    id: 'cx_tkl2',
                                    xtype: 'numberfield',
                                    fieldLabel: '~',
                                    labelWidth: 10,
                                    width: 90,
                                    labelSeparator: ''
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
                                                if (privilege("财务报表_专线自发布统计_查看")) {
                                                    getTj(1);
                                                }
                                            }
                                        },
                                        {
                                            xtype: 'button',
                                            iconCls: 'view',
                                            text: '导出',
                                            handler: function () {
                                                if (privilege("财务报表_专线自发布统计_导出")) {
                                                    var arr = [];
                                                    var cx_gzs = Ext.getCmp("cx_gzs").getValue();
                                                    arr.push(cx_gzs);
                                                    var cx_gzs2 = Ext.getCmp("cx_gzs2").getValue();
                                                    arr.push(cx_gzs2);
                                                    var cx_zhpf = Ext.getCmp("cx_zhpf").getValue();
                                                    arr.push(cx_zhpf);
                                                    var cx_zhpf2 = Ext.getCmp("cx_zhpf2").getValue();
                                                    arr.push(cx_zhpf2);
                                                    var cx_pjs = Ext.getCmp("cx_pjs").getValue();
                                                    arr.push(cx_pjs);
                                                    var cx_pjs2 = Ext.getCmp("cx_pjs2").getValue();
                                                    arr.push(cx_pjs2);
                                                    var cx_dprs = Ext.getCmp("cx_dprs").getValue();
                                                    arr.push(cx_dprs);
                                                    var cx_dprs2 = Ext.getCmp("cx_dprs2").getValue();
                                                    arr.push(cx_dprs2);
                                                    var cx_gmcs = Ext.getCmp("cx_gmcs").getValue();
                                                    arr.push(cx_gmcs);
                                                    var cx_gmcs2 = Ext.getCmp("cx_gmcs2").getValue();
                                                    arr.push(cx_gmcs2);
                                                    var cx_gmrs = Ext.getCmp("cx_gmrs").getValue();
                                                    arr.push(cx_gmrs);
                                                    var cx_gmrs2 = Ext.getCmp("cx_gmrs2").getValue();
                                                    arr.push(cx_gmrs2);
                                                    var cx_zfbcs = Ext.getCmp("cx_zfbcs").getValue();
                                                    arr.push(cx_zfbcs);
                                                    var cx_zfbcs2 = Ext.getCmp("cx_zfbcs2").getValue();
                                                    arr.push(cx_zfbcs2);
                                                    var cx_city = Ext.getCmp("cx_city").getValue();
                                                    arr.push(cx_city);
                                                    var cx_isSq = Ext.getCmp("cx_isSq").getValue();
                                                    arr.push(cx_isSq);
                                                    var cx_isRz = Ext.getCmp("cx_isRz").getValue();
                                                    arr.push(cx_isRz);
                                                    var cx_tkl = Ext.getCmp("cx_tkl").getValue();
                                                    arr.push(cx_tkl);
                                                    var cx_tkl2 = Ext.getCmp("cx_tkl2").getValue();
                                                    arr.push(cx_tkl2);
                                                    DownloadFile("CZCLZ.CWBBMag.GetZxzfbTjToFile", "专线自发布统计表.xls", arr);
                                                }
                                            }
                                        },
                                    ]
                                }
                            ]
                        },
                        {
                            xtype: 'pagingtoolbar',
                            displayInfo: true,
                            store: shareStore,
                            dock: 'bottom'
                        }
                    ]
                }
            ];
            me.callParent(arguments);
        }
    });

    new iView();
    getTj(1);
})
