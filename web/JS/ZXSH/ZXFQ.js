var pageSize = 15;
var cx_yhm;
var cx_xm;
var cx_iscanrelease;
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
       { name: 'IsCanRelease' }
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
                        Ext.Msg.alert(str+"成功");
                    }
                }, CS.onError, id,iscanrelease);
            }
        });
    }
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