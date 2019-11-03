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
        { name: 'caruser' },
        { name: 'ispushwr' },
        { name: 'idcard' },
        { name: 'roadnumber' }
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
var fjlist = [];
function EditPic(id) {
    var win = new PicList({ carid: id });
    win.show(null, function () {
        for (var i = 1; i < 7; i++) {
            var type = "";
            if (i == 1) { type = 2; }
            else if (i == 2) { type = 3; }
            else if (i == 3) { type = 7; }
            else if (i == 4) { type = 8; }
            else if (i == 5) { type = 9; }
            else if (i == 6) { type = 10; }
            GetPic(id, type, i);
        }
    })
    
}

function GetPic(id,type,index) {

    CS('CZCLZ.CarMag.GetPicList', function (retVal) {
        if (retVal) {
            var height = document.getElementById("uploadproductpic" + index).style.height;
            var html = "";
            if (retVal.fjId) {
                html += '<table width="100%"  border="0" cellspacing="0" cellpadding="0" >';
                html += '  <tr>';
                html += '    <td align="center"><img src="' + retVal.fileFullUrl + '"  height="' + height + '" /></td>';
                html += '  </tr>';
                html += '</table>';
            }
            Ext.getCmp('uploadproductpic' + index).setText(html, false);
        }
    }, CS.onError, id, type);
}

var zxd = 0;


function tp2(type, userId) {
    var win2 = new phWin2({ type: type, carid: userId });
    win2.show();
}

function TS(id) {
    CS('CZCLZ.CarMag.TS', function (retVal) {
        if (retVal) {
            getCar(1);
            Ext.MessageBox.alert('确认', '推送成功！');
        }
    }, CS.onError, id);
}


//************************************页面方法***************************************

//************************************弹出界面***************************************
Ext.define('phWin2', {
    extend: 'Ext.window.Window',
    title: "上传",
    height: 80,
    width: 400,
    modal: true,
    layout: 'border',
    id: 'sbBdwin',
    initComponent: function () {
        var me = this;
        var type = me.type;
        var carid = me.carid;
        me.items = [{
            xtype: 'UploaderPanel',
            id: 'sbBdform',
            region: 'center',
            layout: 'column',

            autoScroll: true,
            items: [
                {
                    xtype: 'filefield',
                    allowBlank: false,
                    labelWidth: 65,
                    fieldLabel: '图片上传',
                    buttonText: '浏览',
                    columnWidth: 0.8
                }, {
                    xtype: 'button',
                    text: '上传',
                    iconCls: 'upload',
                    columnWidth: 0.2,
                    margin: '0 5',
                    handler: function () {
                            var screen = "";
                            if (type == 1) { screen = 2; }
                            else if (type == 2) { screen = 3; }
                            else if (type == 3) { screen = 7; }
                            else if (type == 4) { screen = 8; }
                            else if (type == 5) { screen = 9; }
                            else if (type == 6) { screen = 10; }

                            Ext.getCmp('sbBdform').upload('CZCLZ.CarMag.UploadPic', function (retVal) {
                            var height = document.getElementById("uploadproductpic" + type).style.height;
                            var html = "";
                            html += '<table width="100%"  border="0" cellspacing="0" cellpadding="0" >';
                            html += '  <tr>';
                            console.log(retVal[0].fileFullUrl);
                            html += '    <td align="center"><img src="' + retVal[0].fileFullUrl + '"  height="' + height + '" /></td>';
                            html += '  </tr>';
                            html += '</table>';
                            Ext.getCmp('uploadproductpic' + type).setText(html, false);
                            var temp = [];
                            var flag = true;
                            for (var i = 0; i < fjlist.length; i++) {
                                if (fjlist[i].screen == screen) {
                                    flag = false;
                                    temp.push({ "screen": screen, "list": retVal })
                                } else {
                                    temp.push(fjlist[i]);
                                }
                            }
                            fjlist = temp;
                            if (flag) {
                                fjlist.push({ "screen": screen, "list": retVal })
                            }

                            me.close();
                            }, CS.onError, screen, carid);
                    }
                }
            ]

        }];
        me.callParent(arguments);
    }
});

Ext.define('addWin', {
    extend: 'Ext.window.Window',

    height: 380,
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
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '身份证',
                        id: 'idcard',
                        name: 'idcard',
                        labelWidth: 80,
                        anchor: '100%'
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '道路运输证',
                        id: 'roadnumber',
                        name: 'roadnumber',
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


Ext.define('PicList', {
    extend: 'Ext.window.Window',

    height: document.documentElement.clientHeight,
    width: document.documentElement.clientWidth,
    layout: {
        type: 'fit'
    },
    title: '图片修改',
    modal: true,

    initComponent: function () {
        var me = this;
        var carid = me.carid
        Ext.applyIf(me, {
            items: [

        {
            xtype: 'panel',
            region: 'center',
            itemId: 'centerPanel',
            layout: 'border',
            items: [
                {
                    xtype: 'panel',
                    flex: 1,
                    region: 'center',
                    split: true,
                    layout: {
                        type: 'hbox',
                        align: 'stretch'
                    },
                    items: [
                        {
                            xtype: 'panel',
                            flex: 1,
                            title: '身份证正面上传',
                            layout: 'fit',
                            buttonAlign: 'center',
                            buttons: [
                                {
                                    text: '上传',
                                    handler: function () {
                                        tp2(1, carid);
                                    }
                                }
                            ],
                            items: [
                                {
                                    id: 'uploadproductpic1',
                                    html: '',
                                    xtype: 'label',
                                }
                            ]

                        },
                        {
                            xtype: 'panel',
                            flex: 1,
                            title: '身份证反面上传',
                            layout: 'fit',
                            buttonAlign: 'center',
                            buttons: [
                                {
                                    text: '上传',
                                    handler: function () {
                                        tp2(2, carid);
                                    }
                                }
                            ],
                            items: [
                                {
                                    id: 'uploadproductpic2',
                                    html: '',
                                    xtype: 'label',
                                }
                            ]
                        },
                        {
                            xtype: 'panel',
                            flex: 1,
                            title: '行驶证车头上传',
                            layout: 'fit',
                            buttonAlign: 'center',
                            buttons: [
                                {
                                    text: '上传',
                                    handler: function () {
                                        tp2(3, carid);
                                    }
                                }
                            ],
                            items: [
                                {
                                    id: 'uploadproductpic3',
                                    html: '',
                                    xtype: 'label',
                                }
                            ]
                        }
                        
                    ]
                },
                {
                    xtype: 'panel',
                    flex: 1,
                    region: 'south',
                    split: true,
                    layout: {
                        type: 'hbox',
                        align: 'stretch'
                    },
                    items: [
                        {
                            xtype: 'panel',
                            flex: 1,
                            title: '行驶证车尾上传',
                            layout: 'fit',
                            buttonAlign: 'center',
                            buttons: [
                                {
                                    text: '上传',
                                    handler: function () {
                                        tp2(4, carid);
                                    }
                                }
                            ],
                            items: [
                                {
                                    id: 'uploadproductpic4',
                                    html: '',
                                    xtype: 'label',
                                }
                            ]
                        },
                        {
                            xtype: 'panel',
                            flex: 1,
                            title: '驾驶证上传',
                            layout: 'fit',
                            buttonAlign: 'center',
                            buttons: [
                                {
                                    text: '上传',
                                    handler: function () {
                                        tp2(5, carid);
                                    }
                                }
                            ],
                            items: [
                                {
                                    id: 'uploadproductpic5',
                                    html: '',
                                    xtype: 'label',
                                }
                            ]
                        },
                        {
                            xtype: 'panel',
                            flex: 1,
                            title: '营运证上传',
                            layout: 'fit',
                            buttonAlign: 'center',
                            buttons: [
                                {
                                    text: '上传',
                                    handler: function () {
                                        tp2(6, carid);
                                    }
                                }
                            ],
                            items: [
                                {
                                    id: 'uploadproductpic6',
                                    html: '',
                                    xtype: 'label',
                                }
                            ]
                        }

                    ]
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
                        xtype: 'gridcolumn',
                        dataIndex: 'idcard',
                        sortable: false,
                        menuDisabled: true,
                        flex: 1,
                        text: "身份证"
                    },
                    {
                        xtype: 'gridcolumn',
                        dataIndex: 'roadnumber',
                        sortable: false,
                        menuDisabled: true,
                        flex: 1,
                        text: "道路运输证"
                    },
                    {
                        text: '操作',
                        dataIndex: 'id',
                        width: 180,
                        sortable: false,
                        menuDisabled: true,
                        renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                            var str;
                            str = "<a onclick='EditCar(\"" + value + "\");'>修改</a>  <a onclick='EditPic(\"" + value + "\");'>图片修改</a>";
                            if (record.data.ispushwr == 0) {
                                str += "  <a onclick='TS(\"" + value + "\");'>修改推送</a>"
                            } else {
                                str += "  <a onclick='TS(\"" + value + "\");'>新增推送</a>"
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