var pageSize = 15;
var cx_zxmc;

var id = "";
//************************************数据源*****************************************
var fldStore = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
       { name: 'userid' },
       { name: 'zxmc' },
       { name: 'networkName' },
       { name: 'networkPerson' },
       { name: 'networkTel' },
       { name: 'jd' },
       { name: 'wd' },
       { name: 'networkAddress' },
       { name: 'networkId' },
       { name: 'networkType' },
       { name: 'networkAddtime' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getList(nPage);
    }
});
//************************************数据源*****************************************

//************************************页面方法***************************************
function getList(nPage) {
    CS('CZCLZ.YHGLClass.getNetWorkList', function (retVal) {
        fldStore.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_zxmc").getValue());
}


function Edit(id, type) {
    var r = fldStore.findRecord("networkId", id).data;
    var win = new addWin();
    win.show(null, function () {
        win.setTitle("分流点修改");
        var form = Ext.getCmp('addform');
        form.form.setValues(r);
        if (type == 2) {
            Ext.getCmp("jd").allowBlank = true;
            Ext.getCmp("jd").setReadOnly(true);
            Ext.getCmp("wd").allowBlank = true;
            Ext.getCmp("wd").setReadOnly(true);
            Ext.getCmp("networkAddress").allowBlank = true;
            Ext.getCmp("networkAddress").setReadOnly(true);
            Ext.getCmp("btnSave").hide();
        }
    });
}
//************************************页面方法***************************************

Ext.define('addWin', {
    extend: 'Ext.window.Window',

    height: 300,
    width: 400,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '分流点管理',

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'form',
                id: 'addform',
                bodyPadding: 10,
                items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: 'networkId',
                        id: 'networkId',
                        name: 'networkId',
                        labelWidth: 70,
                        hidden: true,
                        colspan: 2
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: 'userid',
                        id: 'userid',
                        name: 'userid',
                        labelWidth: 70,
                        hidden: true,
                        colspan: 2
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: 'networkType',
                        id: 'networkType',
                        name: 'networkType',
                        labelWidth: 70,
                        hidden: true,
                        colspan: 2
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '专线名称',
                        id: 'zxmc',
                        name: 'zxmc',
                        labelWidth: 70,
                        readOnly: true,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '分流点/网点名称',
                        id: 'networkName',
                        name: 'networkName',
                        labelWidth: 70,
                        readOnly: true,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '联系人',
                        id: 'networkPerson',
                        name: 'networkPerson',
                        labelWidth: 70,
                        readOnly: true,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '电话',
                        id: 'networkTel',
                        name: 'networkTel',
                        labelWidth: 70,
                        readOnly: true,
                        anchor: '100%'
                    },
                    {
                        xtype: 'numberfield',
                        fieldLabel: '经度',
                        id: 'jd',
                        name: 'jd',
                        labelWidth: 70,
                        anchor: '100%',
                        allowBlank: false,
                    },
                    {
                        xtype: 'numberfield',
                        fieldLabel: '纬度',
                        id: 'wd',
                        name: 'wd',
                        labelWidth: 70,
                        anchor: '100%',
                        allowBlank: false,
                    },
                    {
                        xtype: 'textareafield',
                        id: 'networkAddress',
                        name: 'networkAddress',
                        labelWidth: 70,
                        fieldLabel: '地址',
                        anchor: '100%',
                        allowBlank: false,
                    }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '保存',
                        iconCls: 'dropyes',
                        id: 'btnSave',
                        handler: function () {
                            var form = Ext.getCmp('addform');
                            if (form.form.isValid()) {
                                var values = form.form.getValues(false);
                                var me = this;
                                CS('CZCLZ.YHGLClass.SaveNetWork', function (retVal) {
                                    if (retVal) {
                                        me.up('window').close();
                                        getList(1);
                                    }
                                }, CS.onError, values);

                            }
                        }
                    },
                     {
                         text: '取消',
                         iconCls: 'back',
                         handler: function () {
                             this.up('window').close();
                         }
                     }
                ]
            }
        ];
        me.callParent(arguments);
    }
});
//************************************主界面*****************************************
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
                    title: '',
                    store: fldStore,
                    columnLines: true,
                    columns: [Ext.create('Ext.grid.RowNumberer'),
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'zxmc',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: "专线名称"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'networkType',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: "网点类型",
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                switch (value) {
                                    case 0:
                                        return "网点";
                                        break;
                                    case 1:
                                        return "分流点";
                                        break;
                                }
                            }
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'networkName',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: "分流点/网点名称"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'networkPerson',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: "联系人"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'networkTel',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: "联系电话"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'jd',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: "分流点经度"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'wd',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: "分流点纬度"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'networkAddress',
                            sortable: false,
                            menuDisabled: true,
                            flex: 1,
                            text: "地址"
                        },
                        {
                            xtype: 'gridcolumn',
                            dataIndex: 'networkId',
                            sortable: false,
                            menuDisabled: true,
                            width: 100,
                            text: '操作',
                            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                                return "<a onclick='Edit(\"" + value + "\",1);'>修改</a> | <a onclick='Edit(\"" + value + "\",2);'>查看</a>";
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
                                             id: 'cx_zxmc',
                                             xtype: 'textfield',
                                             fieldLabel: '专线名称',
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
                                                        getList(1);
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
                                                        if (privilege("系统维护中心_分流点管理_导出")) {
                                                            DownloadFile("CZCLZ.YHGLClass.getNetWorkListToFile", "分流点明细.xls", Ext.getCmp("cx_zxmc").getValue());
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
                                    store: fldStore,
                                    dock: 'bottom'
                                }
                    ]
                }
            ];
            me.callParent(arguments);
        }
    });

    new iView();

    cx_zxmc = Ext.getCmp("cx_zxmc").getValue();
    getList(1);

})
//************************************主界面*****************************************