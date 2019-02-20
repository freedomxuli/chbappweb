var pageSize = 15;
var cx_beg;
var cx_end;
var cx_isregister;
var cx_isbuy;
var cx_tjr;

var id = "";
//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'sharetype' },
       { name: 'addtime' },
       { name: 'isregister' },
       { name: 'isbuy' },
       { name: 'tjname' },
       { name: 'tjxm' },
       { name: 'btjname' },
       { name: 'btjxm' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getShareList(nPage);
    }
});
//************************************数据源*****************************************

//************************************页面方法***************************************
function getShareList(nPage) {
    CS('CZCLZ.YHGLClass.getShareList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_isregister").getValue(), Ext.getCmp("cx_isbuy").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), Ext.getCmp("cx_tjr").getValue());
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
                            dataIndex: 'tjname',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: "推荐人登录名"
                        },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'tjxm',
                                sortable: false,
                                menuDisabled: true,
                                flex: 1,
                                text: "推荐人姓名"
                            },
                            {
                                xtype: 'gridcolumn',
                                dataIndex: 'btjname',
                                sortable: false,
                                menuDisabled: true,
                                flex: 1,
                                text: "被推荐人登录名"
                            },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'btjxm',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: "被推荐人姓名"
                             },
                             {
                                 xtype: 'datecolumn',
                                 dataIndex: 'addtime',
                                 sortable: false,
                                 menuDisabled: true,
                                 format: 'Y-m-d H:i:s',
                                 flex: 1,
                                 text: "时间"
                             },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'isregister',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: "是否注册",
                                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                     switch (value) {
                                         case 0:
                                             return "否";
                                             break;
                                         case 1:
                                             return "是";
                                             break;
                                     }
                                 }
                             },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'isbuy',
                                 sortable: false,
                                 menuDisabled: true,
                                 flex: 1,
                                 text: "是否购买",
                                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                     switch (value) {
                                         case 0:
                                             return "否";
                                             break;
                                         case 1:
                                             return "是";
                                             break;
                                     }
                                 }
                             }
                             //,
                             //{
                             //    xtype: 'gridcolumn',
                             //    dataIndex: 'sharetype',
                             //    sortable: false,
                             //    menuDisabled: true,
                             //    flex: 1,
                             //    text: "类型",
                             //    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                             //        switch (value) {
                             //            case 0:
                             //                return "后台用户";
                             //                break;
                             //            case 1:
                             //                return "三方用户";
                             //                break;
                             //        }
                             //    }
                             //}

                    ],
                    viewConfig: {

                    },
                    dockedItems: [
                                {
                                    xtype: 'toolbar',
                                    dock: 'top',
                                    items: [
                                        
                                        {
                                            xtype: 'combobox',
                                            id: 'cx_isregister',
                                            width: 160,
                                            fieldLabel: '是否注册',
                                            editable: false,
                                            labelWidth: 60,
                                            store: Ext.create('Ext.data.Store', {
                                                fields: [
                                                   { name: 'val' },
                                                   { name: 'txt' }
                                                ],
                                                data: [
                                                     { 'val': "", 'txt': '全部' },
                                                    { 'val': 0, 'txt': '否' },
                                                    { 'val': 1, 'txt': '是' }]

                                            }),
                                            queryMode: 'local',
                                            displayField: 'txt',
                                            valueField: 'val',
                                            value: ''
                                        },
                                        {
                                            xtype: 'combobox',
                                            id: 'cx_isbuy',
                                            width: 160,
                                            fieldLabel: '是否购买',
                                            editable: false,
                                            labelWidth: 60,
                                            store: Ext.create('Ext.data.Store', {
                                                fields: [
                                                   { name: 'val' },
                                                   { name: 'txt' }
                                                ],
                                                data: [
                                                     { 'val': "", 'txt': '全部' },
                                                    { 'val': 0, 'txt': '否' },
                                                    { 'val': 1, 'txt': '是' }]

                                            }),
                                            queryMode: 'local',
                                            displayField: 'txt',
                                            valueField: 'val',
                                            value: ''
                                        },
                                        {
                                            id: 'cx_beg',
                                            xtype: 'datefield',
                                            fieldLabel: '时间',
                                            format: 'Y-m-d',
                                            labelWidth: 40,
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
                                             id: 'cx_tjr',
                                             xtype: 'textfield',
                                             fieldLabel: '推荐人',
                                             labelWidth: 60,
                                             width: 150
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
                                                        getShareList(1);
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


    cx_tjr = Ext.getCmp("cx_tjr").getValue();
    cx_beg = Ext.getCmp("cx_beg").getValue();
    cx_end = Ext.getCmp("cx_end").getValue();
    cx_isregister = Ext.getCmp("cx_isregister").getValue();
    cx_isbuy = Ext.getCmp("cx_isbuy").getValue();
    getShareList(1);

})
//************************************主界面*****************************************