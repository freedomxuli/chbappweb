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
       { name: 'UserName' },
       { name: 'flag' },
       { name: 'gmzx' },
       { name: 'gmzxs' }
       
    ],
    onPageChange: function (sto, nPage, sorters) {
        getUserList(nPage);
    }
});

var djStore = Ext.create('Ext.data.Store', {
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
            'val': 'A',
            'txt': 'A',
        },
        {
            'val': 'B',
            'txt': 'B',
        },
        {
            'val': 'C',
            'txt': 'C',
        },
    ]
});

//************************************数据源*****************************************

//************************************页面方法***************************************
function getUserList(nPage) {
    CS('CZCLZ.FHRMag.GetUserList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_name").getValue(), Ext.getCmp("cx_dj").getValue());
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
                                dataIndex: 'UserName',
                                sortable: false,
                                menuDisabled: true,
                                width:120,
                                text: "手机号"
                            },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'flag',
                                 sortable: false,
                                 menuDisabled: true,
                                 width: 80,
                                 text: "等级"
                             },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'gmzx',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: "购买过的专线",
                                 renderer: function (v) {
                                     if (v) {
                                         return "<span title='" + v + "'>" + v + "</span>";
                                     }
                                 }
                             },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'gmzxs',
                                 sortable: false,
                                 menuDisabled: true,
                                 width: 120,
                                 text: "购买过的专线数"
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
                                            xtype: 'textfield',
                                            id: 'cx_name',
                                            width: 160,
                                            labelWidth: 70,
                                            fieldLabel: '手机号'
                                        },
                                         {
                                             xtype: 'combobox',
                                             id: 'cx_dj',
                                             width: 160,
                                             fieldLabel: '等级',
                                             editable: false,
                                             labelWidth: 40,
                                             store: djStore,
                                             queryMode: 'local',
                                             displayField: 'txt',
                                             valueField: 'val',
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
                                                        getUserList(1);
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
                                                            DownloadFile("CZCLZ.FHRMag.GetUserListToFile", "发货人等级.xls", Ext.getCmp("cx_name").getValue(), Ext.getCmp("cx_dj").getValue());
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

    cx_dj = Ext.getCmp("cx_dj").getValue();
    cx_name = Ext.getCmp("cx_name").getValue();
    getUserList(1);

})
//************************************主界面*****************************************