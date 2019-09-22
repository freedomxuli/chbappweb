var pageSize = 15;
var cx_cz;
var cx_sj;
var cx_sbh;
//************************************数据源*****************************************
var store = createSFW4Store({
    data: [],
    pageSize: pageSize,
    total: 1,
    currentPage: 1,
    fields: [
        { name: 'id' },
        { name: 'mirrornumber' },
        { name: 'driverxm' },
        { name: 'userid' },
        { name: 'UserXM' },
        { name: 'UserName' },
        { name: 'carnumber' },
        { name: 'linkedunit' },
        { name: 'drivermemo' },
        { name: 'caruser' }
    ],
    onPageChange: function (sto, nPage, sorters) {
        getCar(nPage);
    }
});


//************************************数据源*****************************************

//************************************页面方法***************************************
function getCar(nPage) {
    CS('CZCLZ.CarMag.GetCarList', function (retVal) {
        store.setData({
            data: retVal.dt,
            pageSize: pageSize,
            total: retVal.ac,
            currentPage: retVal.cp
        });
    }, CS.onError, nPage, pageSize, Ext.getCmp("cx_cz").getValue(), Ext.getCmp("cx_sj").getValue(), Ext.getCmp("cx_sbh").getValue());
}


function EditCar(id) {
    var r = store.findRecord("id", id).data;
    var win = new addWin();
    win.show(null, function () {
        win.setTitle("用户修改");
        var form = Ext.getCmp('addform');
        form.form.setValues(r);
    });
}
//************************************页面方法***************************************

//************************************弹出界面***************************************


Ext.define('addWin', {
    extend: 'Ext.window.Window',

    height: 320,
    width: 450,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '车辆管理',

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'form',
                id: 'addform',
                bodyPadding: 10,
                bodyStyle: 'overflow-x:hidden;overflow-y:auto',
                items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: 'ID',
                        id: 'id',
                        name: 'id',
                        labelWidth: 80,
                        hidden: true,
                        colspan: 2
                    },
                     {
                         xtype: 'textfield',
                         fieldLabel: '车牌',
                         id: 'carnumber',
                         name: 'carnumber',
                         labelWidth: 80,
                         allowBlank: false,
                        readOnly:true,
                         anchor: '100%'
                     },
                    {
                        xtype: 'textfield',
                        fieldLabel: '设备号',
                        id: 'mirrornumber',
                        name: 'mirrornumber',
                        labelWidth: 80,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '挂靠单位',
                        id: 'linkedunit',
                        name: 'linkedunit',
                        labelWidth: 80,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '车主账号',
                        id: 'caruser',
                        name: 'caruser',
                        labelWidth: 80,
                        allowBlank: false,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '司机姓名',
                        id: 'driverxm',
                        name: 'driverxm',
                        labelWidth: 80,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '司机账号',
                        id: 'drivername',
                        name: 'drivername',
                        labelWidth: 80,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textarea',
                        fieldLabel: '备注',
                        id: 'drivermemo',
                        name: 'drivermemo',
                        labelWidth: 80,
                        anchor: '100%'
                    }
                ],
                buttonAlign: 'center',
                buttons: [
                    {
                        text: '确定',
                        iconCls: 'dropyes',
                        handler: function () {
                            var form = Ext.getCmp('addform');
                            if (form.form.isValid()) {
                                var values = form.form.getValues(false);
                                var me = this;
                                CS('CZCLZ.CarMag.SaveCar', function (retVal) {
                                    if (retVal) {
                                        me.up('window').close();
                                        getCar(1);
                                        Ext.MessageBox.alert('确认', '保存成功！');
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
                    columns: [Ext.create('Ext.grid.RowNumberer'),
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'mirrornumber',
                        sortable: false,
                        menuDisabled: true,
                        flex:1,
                        text: "设备号"
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'driverxm',
                        sortable: false,
                        menuDisabled: true,
                        flex: 1,
                        text: "司机姓名"
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'caruser',
                        sortable: false,
                        menuDisabled: true,
                        flex: 1,
                        text: "车主账号"
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'carnumber',
                        sortable: false,
                        menuDisabled: true,
                        flex: 1,
                        text: "车牌"
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'linkedunit',
                        sortable: false,
                        menuDisabled: true,
                        flex: 1,
                        text: "挂靠单位"
                    },
                    {
                        text: '操作',
                        dataIndex: 'id',
                        width: 60,
                        sortable: false,
                        menuDisabled: true,
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            var str;
                            str = "<a onclick='EditCar(\"" + value + "\");'>修改</a>";
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
                                    id: 'cx_cz',
                                    width: 160,
                                    labelWidth: 70,
                                    fieldLabel: '车主账号'
                                },
                                {
                                    xtype: 'textfield',
                                    id: 'cx_sj',
                                    width: 160,
                                    labelWidth: 70,
                                    fieldLabel: '司机姓名'
                                },
                                {
                                    xtype: 'textfield',
                                    id: 'cx_sbh',
                                    width: 160,
                                    labelWidth: 70,
                                    fieldLabel: '设备号'
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
                                                  cx_cz = Ext.getCmp("cx_cz").getValue();
                                                  cx_sj = Ext.getCmp("cx_sj").getValue();
                                                  cx_sbh = Ext.getCmp("cx_sbh").getValue();
                                                  getCar(1);
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


    cx_cz = Ext.getCmp("cx_cz").getValue();
    cx_sj = Ext.getCmp("cx_sj").getValue();
    cx_sbh = Ext.getCmp("cx_sbh").getValue();
    getCar(1);

})
//************************************主界面*****************************************