var pageSize = 15;
var cx_yhm;
var cx_xm;
var cx_iscanrelease;
var userId = "";
//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'UserID' },
       { name: 'UserName' },
       { name: 'Password' },
       { name: 'roleId' },
       { name: 'roleName' },
       { name: 'UserXM' },
       { name: 'ClientKind' },
       { name: 'Address' },
       { name: 'UserTel' },
       { name: 'FromRoute' },
       { name: 'ToRoute' },
       { name: 'Points' },
       { name: 'SalePoints' },
       { name: 'KFGMPoints' },
       { name: 'dqS' },
       { name: 'DqBm' },
       { name: 'AddTime' },
       { name: 'IsCanRelease' },
       { name: 'canReleaseTime' },
       { name: 'fqcs' }
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
            'MC': '不可以'
        },
        {
            'ID': '1',
            'MC': '可以'
        }
    ]
});

var kfcsstore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'SaleRecordUserID' },
       { name: 'SaleRecordUserXM' },
       { name: 'SaleRecordPoints' },
       { name: 'SaleRecordTime' },
       { name: 'ValidHour' },
       { name: 'SaleRecordDiscount' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getKFCSList(nPage);
    }
});
//************************************数据源*****************************************

//************************************页面方法***************************************
function getUser(nPage) {
    CS('CZCLZ.ZXSHMag.GetZXList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_yhm").getValue(), Ext.getCmp("cx_xm").getValue(), Ext.getCmp("cx_iscanrelease").getValue());
}

function sq(id,iscanrelease,str) {
    if (privilege("专线审核_自行发布运费券_编辑")) {
        Ext.MessageBox.confirm('提示', '是否'+str+'专线自行发布运费券!', function (obj) {
            if (obj == "yes") {
                CS('CZCLZ.ZXSHMag.ZXFQ', function (retVal) {
                    if (retVal) {
                        getUser(1);
                        Ext.MessageBox.alert('提示', str + "成功");
                    }
                }, CS.onError, id,iscanrelease);
            }
        });
    }
}

function KFCS(id) {
    userId = id;
    var win = new KFCSList({ userId: userId });
    win.show(null, function () {
        getKFCSList(1);
    })
}

function getKFCSList(nPage) {
    CS('CZCLZ.ZXSHMag.getKFCSList', function (retVal) {
        kfcsstore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, userId);
}
//************************************页面方法***************************************
//************************************弹出界面***************************************
Ext.define('KFCSList', {
    extend: 'Ext.window.Window',

    height: 500,
    width: 800,
    layout: {
        type: 'fit'
    },
    title: '开放次数列表',
    modal: true,

    initComponent: function () {
        var me = this;
        var userId = me.userId
        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'tabpanel',
                    activeTab: 1,
                    items: [
                        {
                            xtype: 'panel',
                            layout: {
                                type: 'fit'
                            },
                            hidden: true,
                            items: [
                                {
                                    xtype: 'gridpanel',
                                    columnLines: 1,
                                    border: 1,
                                    store: kfcsstore,
                                    columns: [
                                         {
                                             xtype: 'gridcolumn',
                                             dataIndex: 'SaleRecordUserXM',
                                             sortable: false,
                                             menuDisabled: true,
                                             flex:1,
                                             text: '专线名称'
                                         },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'SaleRecordPoints',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '电子券'
                                        },
                                        {
                                            xtype: 'datecolumn',
                                            dataIndex: 'SaleRecordTime',
                                            sortable: false,
                                            menuDisabled: true,
                                            format: 'Y-m-d',
                                            flex: 1,
                                            text: '时间'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'SaleRecordDiscount',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 100,
                                            text: '折扣'
                                        },
                                        {
                                            xtype: 'gridcolumn',
                                            dataIndex: 'ValidHour',
                                            sortable: false,
                                            menuDisabled: true,
                                            width: 150,
                                            text: '有效时间（小时）'
                                        }
                                    ],
                                    dockedItems: [
                                {
                                    xtype: 'toolbar',
                                    dock: 'top',
                                    items: [
                                        {
                                            xtype: 'buttongroup',
                                            title: '',
                                            items: [
                                                {
                                                    xtype: 'button',
                                                    iconCls: 'view',
                                                    text: '导出',
                                                    handler: function () {
                                                        DownloadFile("CZCLZ.ZXSHMag.getKFCSListToFile", "自发券明细                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           .xls", userId, Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue());
                                                    }
                                                },
                                            ]
                                        }
                                    ]
                                },
                                {
                                    xtype: 'pagingtoolbar',
                                    displayInfo: true,
                                    store: kfcsstore,
                                    dock: 'bottom'
                                }
                                    ]
                                }
                            ]
                        }

                    ],
                    buttonAlign: 'center',
                    buttons: [
                        {
                            text: '关闭',
                            handler: function () {
                                me.close();
                            }
                        }
                    ]
                }
            ]
        });

        me.callParent(arguments);
    }

});
//************************************弹出界面***************************************
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
                                dataIndex: 'UserXM',
                                sortable: false,
                                menuDisabled: true,
                                flex:1,
                                text: "专线名称"
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'UserName',
                                sortable: false,
                                menuDisabled: true,
                                width: 120,
                                text: "登录名"
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'UserTel',
                                sortable: false,
                                menuDisabled: true,
                                width: 120,
                                text: "电话"
                            },
                            {
                                xtype: 'gridcolumn',
                                sortable: false,
                                menuDisabled: true,
                                text: "线路",
                                flex: 1,
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    var str="";
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
                                dataIndex: 'IsCanRelease',
                                sortable: false,
                                menuDisabled: true,
                                width: 150,
                                text: "是否可以自行发布运费券",
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    var str="";
                                    if (value == 1) {
                                        str += "可以"
                                    } else {
                                        str += "不可以"
                                    }
                                    return str;
                                }
                            },
                            {
                                xtype: 'datecolumn',
                                dataIndex: 'canReleaseTime',
                                sortable: false,
                                menuDisabled: true,
                                width: 150,
                                text: "审核时间",
                                format:'Y-m-d'
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'fqcs',
                                sortable: false,
                                menuDisabled: true,
                                width: 120,
                                text: "开放次数",
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    return "<a onclick='KFCS(\"" + record.data.UserID + "\");'>" + (value == null ? "" : value) + "</a>"
                                }
                                
                            },
                            {
                                xtype: 'gridcolumn',
                                sortable: false,
                                menuDisabled: true,
                                dataIndex: 'UserID',
                                text: '操作',
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    str = "";
                                    if (record.data.IsCanRelease == 1) {
                                        str = "<a href='JavaScript:void(0)' onclick='sq(\"" + value + "\",\"" + 0+'\",\"' + "关闭" + "\")'>关闭</a>";
                                    }else{
                                        str = "<a href='JavaScript:void(0)' onclick='sq(\"" + value + "\",\"" + 1+ '\",\"' + "开放" + "\")'>开放</a>";
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
                                            id: 'cx_yhm',
                                            width: 140,
                                            labelWidth: 50,
                                            fieldLabel: '用户名'
                                        },
                                        {
                                            xtype: 'textfield',
                                            id: 'cx_xm',
                                            width: 160,
                                            labelWidth: 70,
                                            fieldLabel: '物流名称'
                                        },
                                        {
                                            xtype: 'combobox',
                                            id: 'cx_iscanrelease',
                                            width: 260,
                                            fieldLabel: '是否可以自行发布运费券',
                                            editable: false,
                                            labelWidth: 140,
                                            store: KindStore,
                                            queryMode: 'local',
                                            displayField: 'MC',
                                            valueField: 'ID',
                                            value: ''
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
                                                        getUser(1);
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

    new YhView();


    cx_yhm = Ext.getCmp("cx_yhm").getValue();
    cx_xm = Ext.getCmp("cx_xm").getValue();
    cx_iscanrelease = Ext.getCmp("cx_iscanrelease").getValue();
    getUser(1);

})
//************************************主界面*****************************************