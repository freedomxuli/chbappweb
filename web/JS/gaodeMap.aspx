<%@ Page Language="C#" AutoEventWireup="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link rel="stylesheet" href="https://a.amap.com/jsapi_demos/static/demo-center/css/demo-center.css" />
    <title></title>
    <style type="text/css">
        html, body {
            padding: 0px;
            margin: 0px;
            height: 100%;
        }

        .lj {
            padding: 2px;
            text-decoration: underline;
            cursor: pointer;
        }

        .button.small {
            padding: 4px 12px;
        }

        .button:hover {
            background-color: #eee;
            color: #555;
        }
    </style>
    <script type="text/javascript" src="jquery-1.7.1.min.js"></script>
    <script src="https://a.amap.com/jsapi_demos/static/demo-center/js/demoutils.js"></script>
    <script type="text/javascript" src="https://webapi.amap.com/maps?v=1.4.15&key=8d416796f26df06c6ba6d98d1d8198e9&plugin=AMap.Autocomplete,AMap.PlaceSearch,AMap.AdvancedInfoWindow"></script>
    <script type="text/javascript" src="https://cache.amap.com/lbs/static/addToolbar.js"></script>
    <script type="text/javascript" src="https://a.amap.com/jsapi_demos/static/demo-center/js/jquery-1.11.1.min.js"></script>
    <script type="text/javascript" src="https://a.amap.com/jsapi_demos/static/demo-center/js/underscore-min.js"></script>
    <script type="text/javascript" src="https://a.amap.com/jsapi_demos/static/demo-center/js/backbone-min.js"></script>
    <script type="text/javascript" src='https://a.amap.com/jsapi_demos/static/demo-center/js/prety-json.js'></script>
</head>
<body>
    <div style="width: 100%; height: 100%;" id="container">
    </div>
    <div class="info">
        <div class="input-item">
            <div class="input-item-prepend">
                <span class="input-item-text" style="width: 8rem;">请输入关键字</span>
            </div>
            <input id='tipinput' type="text" />
            <input class="small button" style="margin-left:15px;" type="button" value="查询" onclick="search()" />
        </div>
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
            var Request = new Object();
            Request = GetRequest();
            var jd = Request['jd'];
            var wd = Request['wd'];
            var title = "";//Request['title'];


            //1.加载地图
            var marker = [];
            var map = new AMap.Map('container', {
                center: [119.968638, 31.777045],//中心经纬度
                zoom: 14,//缩放级别,
                isHotspot: true,//地图热点
                resizeEnable: true //是否监控地图容器尺寸变化
            });
            //加载已选取得点
            if (jd != "" && jd != null && jd != 'undefined' && jd != 'null') {
                var position = new AMap.LngLat(jd, wd);  // 标准写法
                map.setZoomAndCenter(18, position);// 同时传入缩放级别和中心点经纬度

                marker[0] = new AMap.Marker({
                    position: position,   // 经纬度对象，也可以是经纬度构成的一维数组[116.39, 39.9]
                    title: title
                });
                // 将创建的点标记添加到已有的地图实例：
                map.add(marker[0]);
            }
            //6.为地图注册click事件获取鼠标点击出的经纬度坐标
            map.on('click', function (e) {
                parent.Ext.getCmp("jd").setValue(e.lnglat.getLng());
                parent.Ext.getCmp("wd").setValue(e.lnglat.getLat());
                parent.Ext.getCmp("glwin").close();
                console.log(e.lnglat.getLng(),e.lnglat.getLat());
            });
            map.on("complete", function () {
                log.success("地图加载完成！");
            });
            //2.输入提示
            var auto = new AMap.Autocomplete({
                input: "tipinput",
                city: '常州'
            });
            //3.主动搜索点
            var placeSearch = new AMap.PlaceSearch({
                city: '全国'
            })
            function search() {
                //var markerClusterer = new AMap.MarkerClusterer(map, { markers: marker });
                //markerClusterer.clearMarkers();
                var searchP = $('#tipinput').val();
                placeSearch.search(searchP, function (status, result) {
                    // 查询成功时，result即对应匹配的POI信息
                    var pois = result.poiList.pois;
                    for (var i = 0; i < pois.length; i++) {
                        var poi = pois[i];
                        marker[i] = new AMap.Marker({
                            position: poi.location,   // 经纬度对象，也可以是经纬度构成的一维数组[116.39, 39.9]
                            title: poi.name
                        });
                        // 将创建的点标记添加到已有的地图实例：
                        map.add(marker[i]);
                    }
                    map.setFitView();
                })
            }
            //4.获取输入提示信息内容列表结合result（暂时不用）
            function autoInput() {
                var keywords = $('#tipinput').val();
                AMap.plugin('AMap.Autocomplete', function () {
                    // 实例化Autocomplete
                    var autoOptions = {
                        city: '全国'
                    }
                    var autoComplete = new AMap.Autocomplete(autoOptions);
                    autoComplete.search(keywords, function (status, result) {
                        // 搜索成功时，result即是对应的匹配数据
                        //var node = new PrettyJSON.view.Node({
                        //    el: document.querySelector("#input-info"),
                        //    data: result
                        //});
                        //console.log(result);
                    })
                })
            }
            autoInput();
            document.getElementById("tipinput").oninput = autoInput;
            //5.热点（暂时不用）
            //var placeSearch = new AMap.PlaceSearch();  //构造地点查询类
            //var infoWindow = new AMap.AdvancedInfoWindow({});
            //map.on('hotspotover', function (result) {
            //    placeSearch.getDetails(result.id, function (status, result) {
            //        if (status === 'complete' && result.info === 'OK') {
            //            placeSearch_CallBack(result);
            //        }
            //    });
            //});
            ////回调函数
            //function placeSearch_CallBack(data) { //infoWindow.open(map, result.lnglat);
            //    var poiArr = data.poiList.pois;
            //    var location = poiArr[0].location;
            //    infoWindow.setContent(createContent(poiArr[0]));
            //    infoWindow.open(map, location);
            //}
            //function createContent(poi) {  //信息窗体内容
            //    var s = [];
            //    s.push('<div class="info-title">' + poi.name + '</div><div class="info-content">' + "地址：" + poi.address);
            //    s.push("电话：" + poi.tel);
            //    s.push("类型：" + poi.type);
            //    s.push('<div>');
            //    return s.join("<br>");
            //}
            




        </script>
    </form>
</body>
</html>
