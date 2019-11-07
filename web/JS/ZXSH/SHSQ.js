var pageSize = 15;
var userId = "";
//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
        { name: 'id' },
        { name: 'reviewtime' },
        { name: 'shr' },
        { name: 'empowertype' },
        { name: 'adduser' },
        { name: 'addtime' },
        { name: 'zxmc' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getList(nPage);
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
            'MC': '未审核'
        },
        {
            'ID': '1',
            'MC': '已审核'
        }
    ]
});
//************************************数据源*****************************************

//************************************页面方法***************************************
function getList(nPage) {
    CS('CZCLZ.SHSQMag.GetSHSQList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_isVerifyType").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), Ext.getCmp("cx_xm").getValue());
}

function sh(id) {
    if (privilege("专线审核_审核授权_编辑") && privilege("专线审核_审核授权_审核")) {
        Ext.MessageBox.confirm('提示', '是否要通过!', function (obj) {
            if (obj == "yes") {
                CS('CZCLZ.SHSQMag.SHSQ', function (retVal) {
                    if (retVal) {
                        getList(1);
                        Ext.MessageBox.alert('提示', '审核通过成功！', "");
                    }
                }, CS.onError, id);
            }
            else {

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
                                dataIndex: 'zxmc',
                                sortable: false,
                                menuDisabled: true,
                                flex: 1,
                                text: "授权专线"
                            },
                            {
                                xtype: 'gridcolumn',
                                sortable: false,
                                menuDisabled: true,
                                dataIndex: 'empowertype',
                                text: "授权状态",
                                flex: 1,
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    if (value == 0) {
                                        return "授权";
                                    } else if (value == 1) {
                                        return "取消授权";
                                    } else { return ""}
                                }

                            },
                            
                            {
                                xtype: 'datecolumn',
                                dataIndex: 'addtime',
                                sortable: false,
                                menuDisabled: true,
                                width: 150,
                                format: 'Y-m-d H:i:s',
                                text: "新增时间"
                            },
                             {
                                 xtype: 'gridcolumn',
                                 dataIndex: 'shr',
                                 sortable: false,
                                 menuDisabled: true,
                                 width: 100,
                                 text: "审核人"
                             },
                             {
                                 xtype: 'datecolumn',
                                 dataIndex: 'reviewtime',
                                 sortable: false,
                                 menuDisabled: true,
                                 width: 150,
                                 format: 'Y-m-d H:i:s',
                                 text: "审核时间"
                             },
                            {
                                xtype: 'gridcolumn',
                                sortable: false,
                                menuDisabled: true,
                                dataIndex: 'id',
                                text: '操作',
                                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                    if (!record.data.shr) {
                                        var str = "<a href='JavaScript:void(0)' onclick='sh(\"" + value + "\")'>审核</a>";
                                        return str;
                                    }
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
                                            id: 'cx_xm',
                                            width: 160,
                                            labelWidth: 70,
                                            fieldLabel: '物流名称'
                                        },
                                        {
                                            xtype: 'combobox',
                                            id: 'cx_isVerifyType',
                                            width: 260,
                                            fieldLabel: '是否审核',
                                            editable: false,
                                            labelWidth: 70,
                                            store: KindStore,
                                            queryMode: 'local',
                                            displayField: 'MC',
                                            valueField: 'ID',
                                            value: ''
                                        },
                                        {
                                            xtype: 'datefield',
                                            id: 'cx_beg',
                                            fieldLabel: '新增时间',
                                            width: 180,
                                            format: 'Y-m-d',
                                            labelWidth: 60
                                        },
                                        {
                                            xtype: 'datefield',
                                            id: 'cx_end',
                                            fieldLabel: '~',//SaleRecordTime
                                            width: 130,
                                            format: 'Y-m-d',
                                            labelWidth: 10,
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
                                                        if (privilege("专线审核_审核授权_查看")) {
                                                            getList(1);
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
                                                    iconCls: 'view',
                                                    text: '导出',
                                                    handler: function () {
                                                        if (privilege("专线审核_审核授权_导出")) {
                                                            DownloadFile("CZCLZ.SHSQMag.GetSHSQListToFile", "审核授权.xls", Ext.getCmp("cx_isVerifyType").getValue(), Ext.getCmp("cx_beg").getValue(), Ext.getCmp("cx_end").getValue(), Ext.getCmp("cx_xm").getValue());
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


    getList(1);

})
//************************************主界面*****************************************