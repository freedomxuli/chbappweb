//************************************页面方法***************************************
function BindData() {
    CS('CZCLZ.CWBBMag.getXSQK', function (retVal) {
        var html = "";
        html += '<table border="0" cellspacing="1" cellpadding="1" width="80%" bgcolor="#ffffff">';
        html += '<tr style="font-family:微软雅黑;font-size:30px;font-weight:bold;">';
        html += '<td width="20%" style="font-size: 25px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center"></td>';
        html += '<td width="40%" style="font-size: 25px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">今日</td>';
        html += '<td width="40%" style="font-size: 25px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">历史</td>';
        html += '</tr>';
        html += '<tr>';
        html += '<td colspan="3" style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">销售情况</td>';
        html += '</tr>';
        if (retVal) {
            var jxl = "";
            var jgmrc = "";
            var jgmrs = "";
            var jfg = "";
            if (retVal.dt.length > 0) {
                if (retVal.dt[0]["xl"] != null && retVal.dt[0]["xl"] != "") {
                    jxl = retVal.dt[0]["xl"];
                }
                if (retVal.dt[0]["gmcs"] != null && retVal.dt[0]["gmcs"] != "") {
                    jgmrc = retVal.dt[0]["gmcs"];
                }

                if (retVal.dt[0]["gmrs"] != null && retVal.dt[0]["gmrs"] != "") {
                    jgmrs = retVal.dt[0]["gmrs"];
                }
            }
            if (retVal.dt2.length > 0) {
                if (retVal.dt2[0]["fg"] != null && retVal.dt2[0]["fg"] != "") {
                    jfg = retVal.dt2[0]["fg"];
                }
            }

            
            var hxl = "";
            var hgmrc = "";
            var hgmrs = "";
            var hfg = "";
            if (retVal.dt1.length > 0) {
                if (retVal.dt1[0]["xl"] != null && retVal.dt1[0]["xl"] != "") {
                    hxl = retVal.dt1[0]["xl"];
                }
                if (retVal.dt1[0]["gmcs"] != null && retVal.dt1[0]["gmcs"] != "") {
                    hgmrc = retVal.dt1[0]["gmcs"];
                }

                if (retVal.dt1[0]["gmrs"] != null && retVal.dt1[0]["gmrs"] != "") {
                    hgmrs = retVal.dt1[0]["gmrs"];
                }
            }
            if (retVal.dt3.length > 0) {
                if (retVal.dt3[0]["fg"] != null && retVal.dt3[0]["fg"] != "") {
                    hfg = retVal.dt3[0]["fg"];
                }
            }

            html += '<tr>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center"> 销量</td>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;cursor:pointer;color:#00C" align="center" onclick="jxl()">'+jxl+'</td>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;cursor:pointer;color:#00C" align="center" onclick="hxl()">' + hxl + '</td>';
            html += '</tr>';

            html += '<tr>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">购买人数（人）</td>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + jgmrs + '</td>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + hgmrs + '</td>';
            html += '</tr>';

            html += '<tr>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">购买人次（次）</td>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;cursor:pointer;color:#00C" align="center" onclick="jgmrc()">' + jgmrc + '</td>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;cursor:pointer;color:#00C" align="center" onclick="hgmrc()">' + hgmrc + '</td>';
            html += '</tr>';

            html += '<tr>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">复购情况（人）</td>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;cursor:pointer;color:#00C" align="center" onclick="jfg()">' + jfg + '</td>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;cursor:pointer;color:#00C" align="center" onclick="hfg()">' + hfg + '</td>';
            html += '</tr>';

            html += '<tr>';
            html += '<td colspan="3" style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">省钱情况</td>';
            html += '</tr>';

            if (retVal.dt5.length > 0) {
                for (var i = 0; i < retVal.dt5.length; i++) {
                    html += '<tr>';
                    html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal.dt5[i]["flag"]+ '</td>';
                    html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal.dt5[i]["counts"] + '</td>';
                    if (retVal.dt6.length > 0) {
                        for (var j = 0; j < retVal.dt6.length; j++) {
                            if (retVal.dt5[i]["flag"] == retVal.dt6[j]["flag"]) {
                                html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal.dt6[j]["counts"] + '</td>';
                            }
                        }
                    }
                    html += '</tr>';
                }
            }

            var jsq = "";
            var hsq = "";
            if (retVal.dt7[0]["sq"] != null && retVal.dt7[0]["sq"] != "") {
                jsq = retVal.dt7[0]["sq"];
            }
            if (retVal.dt8[0]["sq"] != null && retVal.dt8[0]["sq"] != "") {
                hsq = retVal.dt8[0]["sq"];
            }
            html += '<tr>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">省钱总额（元）</td>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;cursor:pointer;color:#00C" align="center" onclick="jsq()">' + jsq + '</td>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;cursor:pointer;color:#00C" align="center" onclick="hsq()">' + hsq + '</td>';
            html += '</tr>';
            var jscgm = "";
            var hscgm = "";
            if (retVal.dt9[0]["scgm"] != null && retVal.dt9[0]["scgm"] != "") {
                jscgm = retVal.dt9[0]["scgm"];
            }
            if (retVal.dt10[0]["scgm"] != null && retVal.dt10[0]["scgm"] != "") {
                hscgm = retVal.dt10[0]["scgm"];
            }
            html += '<tr>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">首次购买（人）</td>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;cursor:pointer;color:#00C" align="center" onclick="jscgm()">' + jscgm + '</td>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;cursor:pointer;color:#00C" align="center" onclick="hscgm()">' + hscgm + '（当月）</td>';
            html += '</tr>';
            html += '<tr>';
            html += '<td colspan="3" style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">专线情况</td>';
            html += '</tr>';
            var ktzx = "";
            if (retVal.dt11[0]["ktzx"] != null && retVal.dt11[0]["ktzx"] != "") {
                ktzx = retVal.dt11[0]["ktzx"];
            }
            html += '<tr>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">开通专线</td>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center"></td>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;cursor:pointer;color:#00C" align="center" onclick="ktzx()">' + ktzx + '</td>';
            html += '</tr>';

            var yxl = "";
            if (retVal.dt12[0]["yxl"] != null && retVal.dt12[0]["yxl"] != "") {
                yxl = retVal.dt12[0]["yxl"];
            }
            html += '<tr>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">已有销量专线</td>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;cursor:pointer;color:#00C" align="center" onclick="yxl()">' + yxl + '</td>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;"></td>';
            html += '</tr>';



            html += '</table>';
            document.getElementById("main").innerHTML=html; 
        }
    }, CS.onError);
}

                                                              
                                                                

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
                    xtype: 'panel',
                    bodyStyle: 'overflow-x:hidden;overflow-y:auto;',
                    html: '<table width="100%" cellspacing="0" cellpadding="0" border="0" style="margin-top:50px;margin-bottom:50px;"><tr><td width="100%" align="center" id="main" ></td></tr></table>'
                }
            ];
            me.callParent(arguments);
        }
    });

    new YhView();
    BindData();
    setInterval(function () { BindData() }, 60000);

    

})
//************************************主界面*****************************************



function jxl() {
    var win = new jxlWin();
    win.show(null, function () {
        CS('CZCLZ.CWBBMag.getJXL', function (retVal) {
            var html = "";
            html += '<table border="0" cellspacing="1" cellpadding="1" width="90%" bgcolor="#ffffff">';
            html += '<tr style="font-family:微软雅黑;font-size:30px;font-weight:bold;">';
            html += '<td width="30%" style="font-size: 18px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">地区</td>';
            html += '<td width="70%" style="font-size: 18px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">销量</td>';
            html += '</tr>';
            if (retVal) {
                for (var i = 0; i < retVal.length; i++) {
                    if (retVal[i]["dq_mc"]) {
                        html += '<tr>';
                        html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal[i]["dq_mc"] + '</td>';
                        html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal[i]["xl"] + '</td>';
                        html += '</tr>';
                    }
                }
            }
            html += '</table>';
            document.getElementById("jxl").innerHTML = html;

        }, CS.onError);
    });

   

}

Ext.define('jxlWin', {
    extend: 'Ext.window.Window',

    height: 230,
    width: 600,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '今日销量',

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'panel',
                bodyPadding: 10,
                html: '<table width="100%" cellspacing="0" cellpadding="0" border="0" style="margin-top:5px;"><tr><td width="100%" align="center" id="jxl" ></td></tr></table>'
            }
        ];
        me.callParent(arguments);
    }
});


function hxl() {
    var win = new hxlWin();
    win.show(null, function () {
        CS('CZCLZ.CWBBMag.getHXL', function (retVal) {
            var html = "";
            html += '<table border="0" cellspacing="1" cellpadding="1" width="90%" bgcolor="#ffffff">';
            html += '<tr style="font-family:微软雅黑;font-size:30px;font-weight:bold;">';
            html += '<td width="30%" style="font-size: 18px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">地区</td>';
            html += '<td width="70%" style="font-size: 18px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">销量</td>';
            html += '</tr>';
            if (retVal) {
                for (var i = 0; i < retVal.length; i++) {
                    if (retVal[i]["dq_mc"]) {
                        html += '<tr>';
                        html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal[i]["dq_mc"] + '</td>';
                        html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal[i]["xl"] + '</td>';
                        html += '</tr>';
                    }
                }
            }
            html += '</table>';
            document.getElementById("hxl").innerHTML = html;

        }, CS.onError);
    });



}

Ext.define('hxlWin', {
    extend: 'Ext.window.Window',

    height: 230,
    width: 600,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '历史销量',

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'panel',
                bodyPadding: 10,
                html: '<table width="100%" cellspacing="0" cellpadding="0" border="0" style="margin-top:5px;"><tr><td width="100%" align="center" id="hxl" ></td></tr></table>'
            }
        ];
        me.callParent(arguments);
    }
});

function jgmrc() {
    var win = new jgmrcWin();
    win.show(null, function () {
        CS('CZCLZ.CWBBMag.getJXL', function (retVal) {
            var html = "";
            html += '<table border="0" cellspacing="1" cellpadding="1" width="90%" bgcolor="#ffffff">';
            html += '<tr style="font-family:微软雅黑;font-size:30px;font-weight:bold;">';
            html += '<td width="30%" style="font-size: 18px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">地区</td>';
            html += '<td width="70%" style="font-size: 18px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">购买人次（次）</td>';
            html += '</tr>';
            if (retVal) {
                for (var i = 0; i < retVal.length; i++) {
                    if (retVal[i]["dq_mc"]) {
                        html += '<tr>';
                        html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal[i]["dq_mc"] + '</td>';
                        html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal[i]["gmrc"] + '</td>';
                        html += '</tr>';
                    }
                }
            }
            html += '</table>';
            document.getElementById("jgmrc").innerHTML = html;

        }, CS.onError);
    });
}

Ext.define('jgmrcWin', {
    extend: 'Ext.window.Window',

    height: 230,
    width: 600,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '今日购买人次',

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'panel',
                bodyPadding: 10,
                html: '<table width="100%" cellspacing="0" cellpadding="0" border="0" style="margin-top:5px;"><tr><td width="100%" align="center" id="jgmrc" ></td></tr></table>'
            }
        ];
        me.callParent(arguments);
    }
});

function hgmrc() {
    var win = new hgmrcWin();
    win.show(null, function () {
        CS('CZCLZ.CWBBMag.getHXL', function (retVal) {
            var html = "";
            html += '<table border="0" cellspacing="1" cellpadding="1" width="90%" bgcolor="#ffffff">';
            html += '<tr style="font-family:微软雅黑;font-size:30px;font-weight:bold;">';
            html += '<td width="30%" style="font-size: 18px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">地区</td>';
            html += '<td width="70%" style="font-size: 18px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">销量</td>';
            html += '</tr>';
            if (retVal) {
                for (var i = 0; i < retVal.length; i++) {
                    if (retVal[i]["dq_mc"]) {
                        html += '<tr>';
                        html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal[i]["dq_mc"] + '</td>';
                        html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal[i]["gmrc"] + '</td>';
                        html += '</tr>';
                    }
                }
            }
            html += '</table>';
            document.getElementById("hgmrc").innerHTML = html;

        }, CS.onError);
    });



}

Ext.define('hgmrcWin', {
    extend: 'Ext.window.Window',

    height: 230,
    width: 600,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '历史购买人次',

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'panel',
                bodyPadding: 10,
                html: '<table width="100%" cellspacing="0" cellpadding="0" border="0" style="margin-top:5px;"><tr><td width="100%" align="center" id="hgmrc" ></td></tr></table>'
            }
        ];
        me.callParent(arguments);
    }
});

function jfg() {
    var win = new jfgWin();
    win.show(null, function () {
        CS('CZCLZ.CWBBMag.getJFG', function (retVal) {
            var html = "";
            html += '<table border="0" cellspacing="1" cellpadding="1" width="90%" bgcolor="#ffffff">';
            html += '<tr style="font-family:微软雅黑;font-size:30px;font-weight:bold;">';
            html += '<td width="30%" style="font-size: 18px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">复购情况</td>';
            html += '<td width="70%" style="font-size: 18px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">人数（人）</td>';
            html += '</tr>';
            if (retVal.count1) {
                        html += '<tr>';
                        html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">2次</td>';
                        html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal.count1 + '</td>';
                        html += '</tr>';
            }
            if (retVal.count2) {
                html += '<tr>';
                html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">2次以上</td>';
                html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal.count2 + '</td>';
                html += '</tr>';
            }
            html += '</table>';
            document.getElementById("jfg").innerHTML = html;

        }, CS.onError);
    });
}

Ext.define('jfgWin', {
    extend: 'Ext.window.Window',

    height: 230,
    width: 600,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '今日复购情况',

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'panel',
                bodyPadding: 10,
                html: '<table width="100%" cellspacing="0" cellpadding="0" border="0" style="margin-top:5px;"><tr><td width="100%" align="center" id="jfg" ></td></tr></table>'
            }
        ];
        me.callParent(arguments);
    }
});


function hfg() {
    var win = new hfgWin();
    win.show(null, function () {
        CS('CZCLZ.CWBBMag.getHFG', function (retVal) {
            var html = "";
            html += '<table border="0" cellspacing="1" cellpadding="1" width="90%" bgcolor="#ffffff">';
            html += '<tr style="font-family:微软雅黑;font-size:30px;font-weight:bold;">';
            html += '<td width="30%" style="font-size: 18px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">复购情况</td>';
            html += '<td width="70%" style="font-size: 18px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">人数（人）</td>';
            html += '</tr>';
            if (retVal.count1) {
                html += '<tr>';
                html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">2次</td>';
                html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal.count1 + '</td>';
                html += '</tr>';
            }
            if (retVal.count2) {
                html += '<tr>';
                html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">2次以上</td>';
                html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal.count2 + '</td>';
                html += '</tr>';
            }
            html += '</table>';
            document.getElementById("hfg").innerHTML = html;

        }, CS.onError);
    });
}

Ext.define('hfgWin', {
    extend: 'Ext.window.Window',

    height: 230,
    width: 600,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '今日复购情况',

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'panel',
                bodyPadding: 10,
                html: '<table width="100%" cellspacing="0" cellpadding="0" border="0" style="margin-top:5px;"><tr><td width="100%" align="center" id="hfg" ></td></tr></table>'
            }
        ];
        me.callParent(arguments);
    }
});

function jsq() {
    var win = new jsqWin();
    win.show(null, function () {
        CS('CZCLZ.CWBBMag.getJSQ', function (retVal) {
            var html = "";
            html += '<table border="0" cellspacing="1" cellpadding="1" width="90%" bgcolor="#ffffff">';
            html += '<tr style="font-family:微软雅黑;font-size:30px;font-weight:bold;">';
            html += '<td width="30%" style="font-size: 18px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">地区</td>';
            html += '<td width="70%" style="font-size: 18px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">省钱总额</td>';
            html += '</tr>';
            if (retVal) {
                for (var i = 0; i < retVal.length; i++) {
                    if (retVal[i]["dq_mc"]) {
                        html += '<tr>';
                        html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal[i]["dq_mc"] + '</td>';
                        html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal[i]["sq"] + '</td>';
                        html += '</tr>';
                    }
                }
            }
            html += '</table>';
            document.getElementById("jsq").innerHTML = html;

        }, CS.onError);
    });
}

Ext.define('jsqWin', {
    extend: 'Ext.window.Window',

    height: 470,
    width: 600,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '今日省钱总额',

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'panel',
                bodyStyle: 'overflow-x:hidden;overflow-y:auto;',
                bodyPadding: 10,
                html: '<table width="100%" cellspacing="0" cellpadding="0" border="0" style="margin-top:5px;"><tr><td width="100%" align="center" id="jsq" ></td></tr></table>'
            }
        ];
        me.callParent(arguments);
    }
});


function hsq() {
    var win = new hsqWin();
    win.show(null, function () {
        CS('CZCLZ.CWBBMag.getHSQ', function (retVal) {
            var html = "";
            html += '<table border="0" cellspacing="1" cellpadding="1" width="90%" bgcolor="#ffffff">';
            html += '<tr style="font-family:微软雅黑;font-size:30px;font-weight:bold;">';
            html += '<td width="30%" style="font-size: 18px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">地区</td>';
            html += '<td width="70%" style="font-size: 18px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">省钱总额</td>';
            html += '</tr>';
            if (retVal) {
                for (var i = 0; i < retVal.length; i++) {
                    if (retVal[i]["dq_mc"]) {
                        html += '<tr>';
                        html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal[i]["dq_mc"] + '</td>';
                        html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal[i]["sq"] + '</td>';
                        html += '</tr>';
                    }
                }
            }
            html += '</table>';
            document.getElementById("hsq").innerHTML = html;

        }, CS.onError);
    });
}

Ext.define('hsqWin', {
    extend: 'Ext.window.Window',

    height: 470,
    width: 600,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '历史省钱总额',

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'panel',
                bodyStyle:'overflow-x:hidden;overflow-y:auto;',
                bodyPadding: 10,
                html: '<table width="100%" cellspacing="0" cellpadding="0" border="0" style="margin-top:5px;"><tr><td width="100%" align="center" id="hsq" ></td></tr></table>'
            }
        ];
        me.callParent(arguments);
    }
});

function jscgm() {
    var win = new jscgmWin();
    win.show(null, function () {
        CS('CZCLZ.CWBBMag.getJSCGM', function (retVal) {
            var html = "";
            html += '<table border="0" cellspacing="1" cellpadding="1" width="90%" bgcolor="#ffffff">';
            html += '<tr style="font-family:微软雅黑;font-size:30px;font-weight:bold;">';
            html += '<td width="30%" style="font-size: 18px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">地区</td>';
            html += '<td width="70%" style="font-size: 18px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">首次购买</td>';
            html += '</tr>';
            if (retVal) {
                for (var i = 0; i < retVal.length; i++) {
                    if (retVal[i]["dq_mc"]) {
                        html += '<tr>';
                        html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal[i]["dq_mc"] + '</td>';
                        html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal[i]["scgm"] + '</td>';
                        html += '</tr>';
                    }
                }
            }
            html += '</table>';
            document.getElementById("jscgm").innerHTML = html;

        }, CS.onError);
    });
}

Ext.define('jscgmWin', {
    extend: 'Ext.window.Window',

    height: 230,
    width: 600,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '今日首次够买',

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'panel',
                bodyStyle: 'overflow-x:hidden;overflow-y:auto;',
                bodyPadding: 10,
                html: '<table width="100%" cellspacing="0" cellpadding="0" border="0" style="margin-top:5px;"><tr><td width="100%" align="center" id="jscgm" ></td></tr></table>'
            }
        ];
        me.callParent(arguments);
    }
});


function hscgm() {
    var win = new hscgmWin();
    win.show(null, function () {
        CS('CZCLZ.CWBBMag.getHSCGM', function (retVal) {
            var html = "";
            html += '<table border="0" cellspacing="1" cellpadding="1" width="90%" bgcolor="#ffffff">';
            html += '<tr style="font-family:微软雅黑;font-size:30px;font-weight:bold;">';
            html += '<td width="30%" style="font-size: 18px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">地区</td>';
            html += '<td width="70%" style="font-size: 18px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">首次购买</td>';
            html += '</tr>';
            if (retVal) {
                for (var i = 0; i < retVal.length; i++) {
                    if (retVal[i]["dq_mc"]) {
                        html += '<tr>';
                        html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal[i]["dq_mc"] + '</td>';
                        html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal[i]["scgm"] + '</td>';
                        html += '</tr>';
                    }
                }
            }
            html += '</table>';
            document.getElementById("hscgm").innerHTML = html;

        }, CS.onError);
    });
}

Ext.define('hscgmWin', {
    extend: 'Ext.window.Window',

    height: 230,
    width: 600,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '当月首次够买',

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'panel',
                bodyStyle: 'overflow-x:hidden;overflow-y:auto;',
                bodyPadding: 10,
                html: '<table width="100%" cellspacing="0" cellpadding="0" border="0" style="margin-top:5px;"><tr><td width="100%" align="center" id="hscgm" ></td></tr></table>'
            }
        ];
        me.callParent(arguments);
    }
});

function ktzx() {
    var win = new ktzxWin();
    win.show(null, function () {
        CS('CZCLZ.CWBBMag.getKTZX', function (retVal) {
            var html = "";
            html += '<table border="0" cellspacing="1" cellpadding="1" width="90%" bgcolor="#ffffff">';
            html += '<tr style="font-family:微软雅黑;font-size:30px;font-weight:bold;">';
            html += '<td width="30%" style="font-size: 18px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">地区</td>';
            html += '<td width="70%" style="font-size: 18px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">开通专线</td>';
            html += '</tr>';
            if (retVal) {
                for (var i = 0; i < retVal.length; i++) {
                    if (retVal[i]["dq_mc"]) {
                        html += '<tr>';
                        html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal[i]["dq_mc"] + '</td>';
                        html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal[i]["ktzx"] + '</td>';
                        html += '</tr>';
                    }
                }
            }
            html += '</table>';
            document.getElementById("ktzx").innerHTML = html;

        }, CS.onError);
    });
}

Ext.define('ktzxWin', {
    extend: 'Ext.window.Window',

    height: 230,
    width: 600,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '开通专线',

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'panel',
                bodyStyle: 'overflow-x:hidden;overflow-y:auto;',
                bodyPadding: 10,
                html: '<table width="100%" cellspacing="0" cellpadding="0" border="0" style="margin-top:5px;"><tr><td width="100%" align="center" id="ktzx" ></td></tr></table>'
            }
        ];
        me.callParent(arguments);
    }
});

function yxl() {
    var win = new yxlWin();
    win.show(null, function () {
        CS('CZCLZ.CWBBMag.getYXL', function (retVal) {
            var html = "";
            html += '<table border="0" cellspacing="1" cellpadding="1" width="90%" bgcolor="#ffffff">';
            html += '<tr style="font-family:微软雅黑;font-size:30px;font-weight:bold;">';
            html += '<td width="30%" style="font-size: 18px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">地区</td>';
            html += '<td width="70%" style="font-size: 18px;color: White;font-weight: bold;border: solid 1px #ffffff;background-color: #5B9BD5;line-height:250%" align="center">已有销量专线</td>';
            html += '</tr>';
            if (retVal) {
                for (var i = 0; i < retVal.length; i++) {
                    if (retVal[i]["dq_mc"]) {
                        html += '<tr>';
                        html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal[i]["dq_mc"] + '</td>';
                        html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + retVal[i]["yxl"] + '</td>';
                        html += '</tr>';
                    }
                }
            }
            html += '</table>';
            document.getElementById("yxl").innerHTML = html;

        }, CS.onError);
    });
}

Ext.define('yxlWin', {
    extend: 'Ext.window.Window',

    height: 230,
    width: 600,
    layout: {
        type: 'fit'
    },
    closeAction: 'destroy',
    modal: true,
    title: '已有销量专线',

    initComponent: function () {
        var me = this;
        me.items = [
            {
                xtype: 'panel',
                bodyStyle: 'overflow-x:hidden;overflow-y:auto;',
                bodyPadding: 10,
                html: '<table width="100%" cellspacing="0" cellpadding="0" border="0" style="margin-top:5px;"><tr><td width="100%" align="center" id="yxl" ></td></tr></table>'
            }
        ];
        me.callParent(arguments);
    }
});
