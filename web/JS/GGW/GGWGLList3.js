var zxd = 2;
var fjlist=[];
function windowonload() {
    for (var i = 1; i < 8; i++) {
        var screen = "";
        if (i == 1) { screen = "750*1334"; }
        else if (i == 2) { screen = "1242*2208"; }
        else if (i == 3) { screen = "1125*2436"; }
        else if (i == 4) { screen = "1242*2688"; }
        else if (i == 5) { screen = "1080*1920"; }
        else if (i == 6) { screen = "640*1136"; }
        else if (i == 7) { screen = "安卓(720*1280)"; }
       
        CS('CZCLZ.GGWMag.GetGGWList', function (retVal) {
            var result = retVal.evalJSON();
            if (result.list.length > 0) {
                var type = 0;
                if (result.list[0].screen == "750*1334") {type=1; }
                else if (result.list[0].screen == "1242*2208") { type = 2; }
                else if (result.list[0].screen == "1125*2436") { type = 3; }
                else if (result.list[0].screen == "1242*2688") { type = 4; }
                else if (result.list[0].screen == "1080*1920") { type = 5; }
                else if (result.list[0].screen == "640*1136") { type = 6; }
                else if (result.list[0].screen == "安卓(720*1280)") { type = 7; }
                var height = document.getElementById("uploadproductpic" + type).style.height;
                var html = "";
                html += '<table width="100%"  border="0" cellspacing="0" cellpadding="0" >';
                html += '  <tr>';
                html += '    <td align="center"><img src="' + result.list[0].advertUrl + '"  height="' + height + '" /></td>';
                html += '  </tr>';
                html += '</table>';
                Ext.getCmp('uploadproductpic' + type).setText(html, false);
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
}

//***********************************数据源******************************************


//*******************************附件******************************************
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
                        if (type == 1) { screen = "750*1334"; }
                        else if (type == 2) { screen = "1242*2208"; }
                        else if (type == 3) { screen = "1125*2436"; }
                        else if (type == 4) { screen = "1242*2688"; }
                        else if (type == 5) { screen = "1080*1920"; }
                        else if (type == 6) { screen = "640*1136"; }
                        else if (type == 7) { screen = "安卓(720*1280)"; }

                        Ext.getCmp('sbBdform').upload('CZCLZ.GGWMag.UploadPic', function (retVal) {
                            var height = document.getElementById("uploadproductpic" + type).style.height;
                            var html = "";
                            html += '<table width="100%"  border="0" cellspacing="0" cellpadding="0" >';
                            html += '  <tr>';
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
                        }, CS.onError);
                    }
                }
            ]

        }];
        me.callParent(arguments);
    }
});
function tp2(type) {
    var win2 = new phWin2({ type: type });
    win2.show();
}
//***********************************************************************
Ext.onReady(function () {
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
                    text: '保存',
                    handler: function () {
                        if (fjlist.length < 7) {
                            console.log(fjlist);
                            Ext.Msg.show({
                                title: '提示',
                                msg: '请上传所有图片！',
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
                                }else
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
                            title: '750*1334',
                            layout: 'fit',
                            buttonAlign: 'center',
                            buttons: [
                                {
                                    text: '上传',
                                    handler: function () {
                                        tp2(1);
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
                            title: '1242*2208',
                            layout: 'fit',
                            buttonAlign: 'center',
                            buttons: [
                                {
                                    text: '上传',
                                    handler: function () {
                                        tp2(2);
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
                            title: '1125*2436',
                            layout: 'fit',
                            buttonAlign: 'center',
                            buttons: [
                                {
                                    text: '上传',
                                    handler: function () {
                                        tp2(3);
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
                        },
                        {
                            xtype: 'panel',
                            flex: 1,
                            title: '1242*2688',
                            layout: 'fit',
                            buttonAlign: 'center',
                            buttons: [
                                {
                                    text: '上传',
                                    handler: function () {
                                        tp2(4);
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
                            title: '1080*1920',
                            layout: 'fit',
                            buttonAlign: 'center',
                            buttons: [
                                {
                                    text: '上传',
                                    handler: function () {
                                        tp2(5);
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
                            title: '640*1136',
                            layout: 'fit',
                            buttonAlign: 'center',
                            buttons: [
                                {
                                    text: '上传',
                                    handler: function () {
                                        tp2(6);
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
                        },
                        {
                            xtype: 'panel',
                            flex: 1,
                            title: '安卓(720*1280)',
                            layout: 'fit',
                            buttonAlign: 'center',
                            buttons: [
                                {
                                    text: '上传',
                                    handler: function () {
                                        tp2(7);
                                    }
                                }
                            ],
                            items: [
                                {
                                    id: 'uploadproductpic7',
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
    new knjtjbxxcx();
    windowonload();
});


