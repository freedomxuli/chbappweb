
//-----------------------------------------------------------全局变量-----------------------------------------------------------------
var zxd = 3;
var fjlist = [];

//-----------------------------------------------------------数据源-------------------------------------------------------------------
//-----------------------------------------------------------页面方法-----------------------------------------------------------------
function windowonload() {
    var screen = "安卓(720*1280)"; 

    CS('CZCLZ.GGWMag.GetGGWList', function (retVal) {
        var result = retVal.evalJSON();
        if (result.list.length > 0) {
            var height = document.getElementById("uploadproductpic1").style.height;
            var html = "";
            html += '<table width="100%"  border="0" cellspacing="0" cellpadding="0" >';
            html += '  <tr>';
            html += '    <td align="center"><img src="' + result.list[0].advertUrl + '"  height="' + height + '" /></td>';
            html += '  </tr>';
            html += '</table>';
            Ext.getCmp('uploadproductpic1').setText(html, false);
            var list = [];
            list.push({
                "fjId": result.list[0].fjId,
                "fileName": result.list[0].FJ_MC,
                "fileFullUrl": result.list[0].advertUrl
            })
            fjlist.push({ "screen": result.list[0].screen, "list": list })
        }
    }, CS.onError, zxd, screen);
}

function tp() {
    var win = new phWin();
    win.show();
}
//-----------------------------------------------------------附    件-----------------------------------------------------------------
Ext.define('phWin', {
    extend: 'Ext.window.Window',
    title: "上传",
    height: 80,
    width: 400,
    modal: true,
    layout: 'border',
    id: 'sbBdwin',
    initComponent: function () {
        var me = this;
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
                        var screen = "安卓(720*1280)";
                        
                        Ext.getCmp('sbBdform').upload('CZCLZ.GGWMag.UploadPic', function (retVal) {
                            console.log(retVal);
                            var height = document.getElementById("uploadproductpic1").style.height;
                            var html = "";
                            html += '<table width="100%"  border="0" cellspacing="0" cellpadding="0" >';
                            html += '  <tr>';
                            html += '    <td align="center"><img src="' + retVal[0].fileFullUrl + '"  height="' + height + '" /></td>';
                            html += '  </tr>';
                            html += '</table>';
                            Ext.getCmp('uploadproductpic1').setText(html, false);
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
                        }, CS.onError);
                    }
                }
            ]

        }];
        me.callParent(arguments);
    }
});

//-----------------------------------------------------------界    面-----------------------------------------------------------------
Ext.define('knjtjbxxcx', {
    extend: 'Ext.container.Viewport',
    listeners: {
        destroy: function () {
            FJ_ID = [];
            upfiles = [];
        }
    },
    layout: {
        type: 'border'
    },
    initComponent: function () {
        var me = this;
        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'panel',
                    region: 'center',
                    itemId: 'centerPanel',
                    layout: 'border',
                    buttonAlign: 'center',
                    buttons: [
                        {
                            text: '上传',
                            handler: function () {
                                tp();
                            }
                        },
                        {
                            text: '保存',
                            handler: function () {
                                if (fjlist.length < 0) {
                                    Ext.Msg.show({
                                        title: '提示',
                                        msg: '请上传图片！',
                                        buttons: Ext.MessageBox.OK,
                                        icon: Ext.MessageBox.ERROR
                                    });
                                    return;
                                } else {
                                    for (var i = 0; i < fjlist.length; i++) {
                                        if (fjlist[i].list.length <= 0) {
                                            Ext.Msg.show({
                                                title: '提示',
                                                msg: '请上传分辨率为' + fjlist[i].screen + '的图片！',
                                                buttons: Ext.MessageBox.OK,
                                                icon: Ext.MessageBox.ERROR
                                            });
                                            return;
                                        } else
                                            CS('CZCLZ.GGWMag.SaveGGW', function (retVal) {
                                                if (retVal) {
                                                }
                                            }, CS.onError, zxd, fjlist[i].screen, JSON.stringify(fjlist[i].list));
                                    }
                                    Ext.Msg.show({
                                        title: '提示',
                                        msg: '保存成功！',
                                        buttons: Ext.MessageBox.OK,
                                        icon: Ext.MessageBox.INFO
                                    });
                                }
                            }
                        }
                    ],
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
                                    title: '720*1280',
                                    layout: 'fit',
                                    buttonAlign: 'center',
                                    items: [
                                        {
                                            id: 'uploadproductpic1',
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

Ext.onReady(function () {
    new knjtjbxxcx();
    windowonload();
});