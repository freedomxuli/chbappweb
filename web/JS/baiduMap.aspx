<%@ Page Language="C#" AutoEventWireup="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <style type="text/css">
        html, body
        {
            padding: 0px;
            margin: 0px;
            height: 100%;
        }

        .clearfix:after
        {
            content: ".";
            display: block;
            height: 0;
            clear: both;
            visibility: hidden;
        }



        .clearfix
        {
            zoom: 1; /* triggers hasLayout */
        }

        .area
        {
            cursor: pointer;
            font-size: 12px;
            text-decoration: underline;
            font-weight: bold;
            color: red;
        }

        .area1
        {
            cursor: pointer;
            font-size: 14px;
            text-decoration: underline;
            font-weight: bold;
            color: red;
        }

        .area2
        {
            cursor: pointer;
            font-size: 13px;
            font-weight: bold;
            color: #677284;
        }

        .s1, .s2, .s3
        {
            float: left;
            font-size: 12px;
            height: 31px;
            line-height: 31px;
        }

        .s1
        {
            width: 80px;
            padding-left: 10px;
        }

        .s2
        {
            width: 75px;
            color: #0C0;
        }

        .s3
        {
            width: 75px;
            color: Red;
        }

        .s4
        {
            color: #0C0;
            font-size: 12px;
        }

        .s5
        {
            color: Red;
            font-size: 12px;
        }

        .s6
        {
            font-size: 12px;
            cursor: pointer;
        }

        .c1
        {
            height: 31px;
            line-height: 31px;
            border: 1px solid #E0E3ED;
            background-image: url('approot/r/images/11_r2_c2.jpg');
        }

        .c2
        {
            border: 1px solid #E0E3ED;
            margin-bottom: 5px;
            background-color: White;
        }

        .c3
        {
            height: 31px;
            line-height: 31px;
            border: 1px solid #FAFAFA;
            background-image: url('approot/r/images/di.jpg');
        }

        .c4
        {
            margin-left: 10px;
            border: 0px solid #E0E3ED;
            margin-bottom: 5px;
            background-color: White;
        }

        .c5
        {
            height: 31px;
            line-height: 31px;
            border: 1px solid #FAFAFA;
            background-image: url('approot/r/images/di.jpg');
            margin-left: 140px;
        }

        .lj
        {
            padding: 2px;
            text-decoration: underline;
            cursor: pointer;
        }
    </style>
    <script type="text/javascript" src="jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="http://api.map.baidu.com/api?v=3.0&ak=dhfkGEAo2ss0UgYRrFH0zD1G50NtOpOC"></script>
</head>
<body>
    <div style="border: 1px solid #C0C0C0; width: 150px; height: 400px; position: absolute; left: 0px; z-index: 9999; background-color: #FFFFFF;">
        <div>
            <table>
                <tr>
                    <td>
                        <input id="txtSearch" name="txtSearch" type="text" value="" style="width: 90px" />
                    </td>
                    <td>
                        <input type="button" value="查询" onclick="search()" />
                    </td>
                </tr>
            </table>

        </div>
        <div id="searchResultPanel" style="overflow: auto; width: 150px; height: 350px;"></div>
        <div id="pagetool" style="font-size: 12px; position: absolute; bottom: 0; height: 20px; width: 100%">
            <%--<span class="lj" style="" >上一页</span>
            <span class="lj" style="float:right;">下一页</span>--%>
        </div>
    </div>

    <div style="width: 100%; height: 100%;" id="dvMap">
    </div>

    <form id="form1" runat="server">
        <script type="text/javascript" defer="defer">
            function GetRequest() {
                var url = location.search; //获取url中"?"符后的字串 
                var theRequest = new Object();
                if (url.indexOf("?") != -1) {
                    var str = url.substr(1);
                    strs = str.split("&");
                    for (var i = 0; i < strs.length; i++) {
                        theRequest[strs[i].split("=")[0]] = unescape(strs[i].split("=")[1]);
                    }
                }
                return theRequest;
            }

            var map = new BMap.Map('dvMap');
            map.addControl(new BMap.NavigationControl({ type: BMAP_NAVIGATION_CONTROL_SMALL }));
            map.addControl(new BMap.ScaleControl());
            map.addControl(new BMap.OverviewMapControl());
            map.enableScrollWheelZoom();
            map.centerAndZoom(new BMap.Point(119.968638, 31.777045), 12);

            var Request = new Object();
            Request = GetRequest();
            var jd = Request['jd'];
            var wd = Request['wd'];
            console.log(jd, wd);
            var point = null;
            if (jd != "" && jd != null && jd != 'undefined' && jd != 'null') {
                point = new BMap.Point(jd, wd);
                map.centerAndZoom(point, 18);

                var marker = new BMap.Marker(point);        // 创建标注    
                map.addOverlay(marker);// 将标注添加到地图中 
            } else {
                point = new BMap.Point(119.968638, 31.777045);
                map.centerAndZoom(point, 12);
            }                     

            var ls;

            map.addEventListener("dblclick", function (e) {

                //parent.document.getElementById('XM_JD').value = e.point.lng;

                //parent.document.getElementById('XM_WD').value = e.point.lat;

                // alert(e.point.lng + ", " + e.point.lat); 
                if (parent.Ext.getCmp("jd") != null) {
                    parent.Ext.getCmp("jd").setValue(e.point.lng);
                    parent.Ext.getCmp("wd").setValue(e.point.lat);
                    parent.Ext.getCmp("glwin").close();
                }


            });


            function ClickMap(pjsd_id, PJSD_Latitude, PJSD_Longitude) {
                map.centerAndZoom(new BMap.Point(PJSD_Longitude, PJSD_Latitude), 17);
                var marker = markerRecord[pjsd_id];
                if (marker)
                    marker.showInfo();
            }

            function selectPoint(lng, lat) {
                parent.Ext.getCmp("jd").setValue(lng);
                parent.Ext.getCmp("wd").setValue(lat);
                parent.Ext.getCmp("glwin").close();

            }

            function gotoPage(num) {
                ls.gotoPage(num);
            }

            function search() {

                // document.getElementById("txtResult").value = ""//每次生成前清空文本域  
                map.clearOverlays(); //清除地图上所有标记  

                var c = "常州市";
                var s = document.getElementById("txtSearch").value;
                ls = new BMap.LocalSearch(
                    c,
                    {
                        renderOptions: {
                            map: map,
                            autoViewport: true
                        },
                        pageCapacity: 8,
                        onSearchComplete: function (results) {
                            if (ls.getStatus() == BMAP_STATUS_SUCCESS) {
                                // 判断状态是否正确   
                                var html = "";
                                for (var i = 0; i < results.getCurrentNumPois() ; i++) {
                                    var poi = results.getPoi(i);
                                    //s.push(results.getPoi(i).title + ", " + results.getPoi(i).address);  
                                    html += "<a href='javascript:void(0)' style='font-size:12px;' onclick='ClickMap(\"\",\"" + poi.point.lat + "\",\"" + poi.point.lng + "\")'>" + poi.title + "</a>";
                                    html += "<br/><a href='javascript:void(0)' style='font-size:12px;' onclick='selectPoint(\"" + poi.point.lng + "\",\"" + poi.point.lat + "\")'>选取</a>";
                                    html += "<BR/><span style='font-size:12px;'>" + poi.point.lng + "," + poi.point.lat + "</span><BR/>";
                                }
                                document.getElementById("searchResultPanel").innerHTML = html;

                                var pagehtml = "";
                                var pageindex = results.getPageIndex();
                                var pagenum = results.getNumPages();
                                if (pagenum > 0) {
                                    if (pageindex > 0) {
                                        pagehtml += '<span class="lj" onclick="gotoPage(' + (pageindex - 1) + ')">上一页</span>';
                                    }
                                    if (pageindex < (pagenum - 1)) {
                                        pagehtml += '<span class="lj" style="float:right;" onclick="gotoPage(' + (pageindex + 1) + ')">下一页</span>';
                                    }
                                    document.getElementById("pagetool").innerHTML = pagehtml;
                                }


                            }
                        }
                    }
                );
                ls.search(s);



                //var html = "";
                //var i = 1;
                //ls.setSearchCompleteCallback(function (rs) {
                //    if (ls.getStatus() == BMAP_STATUS_SUCCESS) {
                //        for (j = 0; j < rs.getCurrentNumPois() ; j++) {
                //            var poi = rs.getPoi(j);
                //            map.addOverlay(new BMap.Marker(poi.point)); //如果查询到，则添加红色marker  

                //            html += "<a href='javascript:void(0)' style='font-size:12px;' onclick='ClickMap(\"\",\"" + poi.point.lat + "\",\"" + poi.point.lng + "\")'>" + poi.title + "</a>";
                //            html += "<br/><a href='javascript:void(0)' style='font-size:12px;' onclick='selectPoint(\"" + poi.point.lng + "\",\"" + poi.point.lat + "\")'>选取</a>";
                //            html += "<BR/><span style='font-size:12px;'>" + poi.point.lng + "," + poi.point.lat + "</span><BR/>";

                //            if(j>=20)
                //            {
                //                break;
                //            }
                //        }
                //        // alert(html)
                //        document.getElementById("searchResultPanel").innerHTML = html;
                //        // document.getElementById("txtResult").innerHTML = html;
                //        if (rs.getPageIndex != rs.getNumPages()) {
                //            ls.gotoPage(i);
                //            i = i + 1;
                //        }
                //    }
                //});
            }
        </script>
    </form>
</body>
</html>
