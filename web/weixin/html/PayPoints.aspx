﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PayPoints.aspx.cs" Inherits="weixin_html_PayPoints" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>支付运费券</title>
    <meta name="viewport" content="width=device-width, initial-scale=1,maximum-scale=1,user-scalable=no" />
	<meta name="apple-mobile-web-app-capable" content="yes" />
	<meta name="apple-mobile-web-app-status-bar-style" content="black" />
	<link rel="stylesheet" href="../css/mui.min.css" />
</head>

<body>
    <form id="form1" runat="server">
    <!--下拉刷新容器-->
	<div id="pullrefresh" class="mui-content mui-scroll-wrapper">
		<div class="mui-scroll">
			<!--数据列表-->
			<ul class="mui-table-view">
					
			</ul>
		</div>
	</div>
	<script src="../js/mui.min.js"></script>
    <script src="../js/mui.showLoading.js"></script>
    <script src="../js/app.js"></script>
    <script>
        var list;
        var pagnum = 1;
        var pagesize = 10;
        var totalcount = 0;
        var flag = false;
    	mui.init({
    		pullRefresh: {
    		    container: '#pullrefresh',
    		    up: {
    		        contentrefresh: '正在加载...',
    		        callback: pullupRefresh
    		    }
    		}
    	});

    	/**
            * 上拉加载具体业务实现
            */
    	function pullupRefresh() {
    	    mui.showLoading("正在加载..", "div");
    	    var ajax_sign;
    	    var ajax_msg;
    	    mui.ajax(grobal_url, {
    	        dataType: "json",
    	        type: "post",
    	        data: {
    	            "action": "GetMyCardsList",
    	            "UserName":'15251979955', //localStorage.getItem("mgps_UserName"),
    	            "pagnum": pagnum,
    	            "pagesize": pagesize
    	        },
    	        success: function (data, status, xhr) {
    	            ajax_sign = data.sign;
    	            ajax_msg = data.msg;
    	            if (ajax_sign == '1') {
    	                list = data.value.dt;
    	                pagnum = (data.value.cp + 1);
    	                totalcount = data.value.ac;
    	                if (totalcount <= (pagnum - 1) * pagesize) {
    	                    flag = true;
    	                }
    	                setTimeout(function () {
    	                    mui('#pullrefresh').pullRefresh().endPullupToRefresh(flag); //参数为true代表没有更多数据了。
    	                    var table = document.body.querySelector('.mui-table-view');
    	                    var cells = document.body.querySelectorAll('.mui-table-view-cell');
    	                    for (var i = cells.length, len = i + 20; i < len; i++) {
    	                        var li = document.createElement('li');
    	                        li.className = 'mui-table-view-cell';
    	                        //li.innerHTML = '<a class="mui-navigate-right">Item ' + (i + 1) + '</a>';
    	                        li.innerHTML = '<div class="mui-table">' +
                                                    '<div class="mui-table-cell">' +
                                                        '<h4 style="line-height:150%">' + list[i]["zxmc"] + '-运费券-<span style="color:red;">' + list[i]["points"] + '券</span></h4>' +
                                                        '<h5>所有人：' + list[i]["syr"] + '</h5>' +
                                                        '<h5>电话：' + list[i]["UserTel"] + '</h5>' +
                                                        '<h5>地址：' + (list[i]["Address"] == null ? "" : list[i]["Address"]) + '</h5>' +
                                                        //'<p class="mui-h6 mui-ellipsis">备注：在对应专线使用该运费券可抵扣运费，且一次最多抵扣500</p>' +
                                                    '</div>' +
                                                    '<div class="mui-table-cell mui-col-xs-2 mui-text-center">' +
                                                        '<br /><br /><button type="button" class="mui-btn mui-btn-primary mui-btn-outlined" data-index="wl" onClick="PayPoints(\'' + list[i]["CardUserID"] + '\',\'' + list[i]["zxmc"] + '\',\'' + list[i]["points"] + '\');">' +
                                                            '扫码' +
                                                        '</button>' +
                                                    '</div>' +
                                                '</div>';
    	                        table.appendChild(li);
    	                    }
    	                }, 1500);
    	            } else {
    	                mui.alert(ajax_msg)
    	            }
    	        }
    	    });
    	}

    	function PayPoints(CardUserID, zxmc, points) {
    	    localStorage.setItem("PPCardUserID", CardUserID);
    	    localStorage.setItem("PPzxmc", zxmc);
    	    localStorage.setItem("PPpoints", points);
    	    document.location.href = "PayPointsMX.aspx";
    	    ////点击按钮扫描二维码
    	    //wx.scanQRCode({
    	    //    needResult: 1, // 默认为0，扫描结果由微信处理，1则直接返回扫描结果，
    	    //    scanType: ["qrCode"], // 可以指定扫二维码还是一维码，默认二者都有
    	    //    success: function (res) {
    	    //        var result = res.resultStr; // 当needResult 为 1 时，扫码返回的结果
    	    //        alert(result);
    	    //        //window.location.href = result;//因为我这边是扫描后有个链接，然后跳转到该页面
    	    //    }
    	    //});
    	}

    	if (mui.os.plus) {
    		mui.plusReady(function () {
    		    setTimeout(function () {
    		        mui('#pullrefresh').pullRefresh().pullupLoading();
    		    }, 1000);

    		});
    	} else {
    		mui.ready(function () {
    		    mui('#pullrefresh').pullRefresh().pullupLoading();
    		});
    	}
    </script>
    </form>
</body>
</html>
