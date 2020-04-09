<%@ Page Language="C#" AutoEventWireup="true"  %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <script type="text/javascript" src="approot/r/js/extjs/ext-all-debug-w-comments.js"></script>

    <script type="text/javascript" src="approot/r/js/extjs/ext-lang-zh_CN.js"></script>
    <script type="text/javascript" src="approot/r/js/BoxSelect/BoxSelect.js"></script>
    <script type="text/javascript" src="approot/r/js/json.js"></script>
    <script type="text/javascript" src="approot/r/js/fun.js"></script>
    <script type="text/javascript" src="approot/r/js/cb.js"></script>
    <script type='text/javascript' src="approot/r/js/jquery-1.7.1.min.js"></script>

    <!-- 原始地址：//webapi.amap.com/ui/1.0/ui/misc/PathSimplifier/examples/simple-data.html -->
    <script type="text/javascript" src="http://webapi.amap.com/ui/1.0/ui/misc/PathSimplifier.js?v=1.0.11&mt=ui&key=8d416796f26df06c6ba6d98d1d8198e9"></script>
    <script type="text/javascript" src="http://webapi.amap.com/ui/1.0/ui/overlay/SimpleInfoWindow.js?v=1.0.11&mt=ui&key=8d416796f26df06c6ba6d98d1d8198e9"></script>
    <%--<base href="//webapi.amap.com/ui/1.0/ui/overlay/SimpleInfoWindow/examples/" />
    <base href="//webapi.amap.com/ui/1.0/ui/misc/PathSimplifier/examples/" />--%>
    <meta charset="utf-8" />
    <meta name="viewport" content="initial-scale=1.0, user-scalable=no, width=device-width" />
    <title>查看轨迹</title>
    <style>
        html,
        body,
        #container {
            width: 100%;
            height: 100%;
            margin: 0px;
        }
        p.my-title {
            font-weight:bold;
            background-color:#0094ff;
            color:white;
            margin:auto;
        }
        p.my-desc {
            margin: 5px 0;
            line-height: 150%;
            font-size:10px;
            font-family:Consolas;
        }
    </style>
</head>
<body>
    <div id="container"></div>
    <script type="text/javascript" src='//webapi.amap.com/maps?v=1.4.15&key=8d416796f26df06c6ba6d98d1d8198e9'></script>
    <!-- UI组件库 1.0 -->
    <script src="//webapi.amap.com/ui/1.0/main.js?v=1.0.11"></script>
    <script type="text/javascript">
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

        var Request = new Object();
        Request = GetRequest();
        var orderId = Request['shippingnoteid'];
        console.log(orderId);
        CS("CZCLZ.Order.GetOrderLocus", function (res) {
            var locusInfo = JSON.parse(res);//轨迹JSON
            if (locusInfo.success == "True") {
                //创建地图
                var map = new AMap.Map('container', {
                    zoom: 4
                });
                AMapUI.load(['ui/misc/PathSimplifier', 'lib/$', 'ui/overlay/SimpleInfoWindow'], function (PathSimplifier, $, SimpleInfoWindow) {
                    
                    var pointsArr = [];
                    for (var i = 0; i < locusInfo.data.length; i++) {
                        let lonlatRes = [locusInfo.data[i]["longitudedegree"], locusInfo.data[i]["latitudedegree"]];
                        let chepai = locusInfo.data[i]['vehiclenumber'];
                        let address = locusInfo.data[i]['address'];
                        let ti = locusInfo.data[i]['getlocationtime'];
                        let drivername = locusInfo.data[i]['drivername'];
                        let fromCity = locusInfo.data[i]['goodsfromroute'];
                        let toCity = locusInfo.data[i]['goodstoroute'];
                        pointsArr.push({ lnglat: lonlatRes });

                        //标签页
                        var infoWindow = new SimpleInfoWindow({
                            infoTitle: '<p class="my-title">' + chepai + '</p>',
                            infoBody: '<p class="my-desc">司机姓名：'+drivername+'</p>'+
                                '<p class="my-desc">出发地：'+fromCity+'</p>'+
                                '<p class="my-desc">目的地：'+toCity+'</p>'+
                                '<p class="my-desc">地址：'+address+'</p>'+
                                '<p class="my-desc">定位时间：'+ti+'</p>',

                            //基点指向marker的头部位置
                            offset: new AMap.Pixel(0, -31)
                        });
                        //点标记
                        let marker = new AMap.Marker({
                            map: map,
                            zIndex: 9999999,
                            position: lonlatRes
                        });
                        AMap.event.addListener(marker, 'click', function () {
                            infoWindow.open(map, marker.getPosition());
                        });
                    }
                    //轨迹
                    if (!PathSimplifier.supportCanvas) {
                        alert('当前环境不支持 Canvas！');
                        return;
                    }

                    var pathSimplifierIns = new PathSimplifier({
                        zIndex: 100,
                        //autoSetFitView:false,
                        map: map, //所属的地图实例

                        getPath: function (pathData, pathIndex) {
                            //return pathData.path;
                            var points = pathData.points,
                                lnglatList = [];

                            for (var i = 0, len = points.length; i < len; i++) {
                                lnglatList.push(points[i].lnglat);
                            }

                            return lnglatList;
                        },
                        getHoverTitle: function (pathData, pathIndex, pointIndex) {

                            if (pointIndex >= 0) {
                                //point 
                                return pathData.points[pointIndex].name;
                            }

                            return '点数量' + pathData.points.length;
                        },
                        renderOptions: {

                            renderAllPointsIfNumberBelow: 100 //绘制路线节点，如不需要可设置为-1
                        }
                    });

                    window.pathSimplifierIns = pathSimplifierIns;


                    //设置数据
                    pathSimplifierIns.setData([{
                        //name: '路线0',
                        points: pointsArr
                    }]);

                    //选中路线0
                    pathSimplifierIns.setSelectedPathIndex(0);


                    pathSimplifierIns.on('pointClick', function (e, info) {
                        
                    });

                    //对第一条线路（即索引 0）创建一个巡航器
                    var navg1 = pathSimplifierIns.createPathNavigator(0, {
                        loop: true, //循环播放
                        speed: 1000000 //巡航速度，单位千米/小时
                    });

                    navg1.start();


                });
            }
        }, CS.onError, orderId);
        
    </script>
</body>
</html>
