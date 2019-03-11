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
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">'+jxl+'</td>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">'+hxl+'</td>';
            html += '</tr>';

            html += '<tr>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">购买人数（人）</td>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + jgmrs + '</td>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + hgmrs + '</td>';
            html += '</tr>';

            html += '<tr>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">购买人次（人）</td>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + jgmrc + '</td>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + hgmrc + '</td>';
            html += '</tr>';

            html += '<tr>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">复购情况（人）</td>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + jfg + '</td>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + hfg + '</td>';
            html += '</tr>';

            html += '<tr>';
            html += '<td colspan="3" style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">省钱人数（人）</td>';
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
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + jsq + '</td>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + hsq + '</td>';
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
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + jscgm + '</td>';
            html += '<td style="font-size: 18px;line-height: 250%;border: solid 1px #ffffff;background-color: #D2DEEF;" align="center">' + hscgm + '（当月）</td>';
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
                    html: '<table width="100%" cellspacing="0" cellpadding="0" border="0" style="margin-top:50px;"><tr><td width="100%" align="center" id="main" ></td></tr></table>'
                }
            ];
            me.callParent(arguments);
        }
    });

    new YhView();

    BindData();

})
//************************************主界面*****************************************