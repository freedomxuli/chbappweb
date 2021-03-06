﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="menu.aspx.cs" Inherits="weixin_html_menu" %>

<!DOCTYPE html>
<html>

	<head>
		<meta charset="utf-8">
		<title>查货宝运费券系统</title>
		<meta name="viewport" content="width=device-width, initial-scale=1,maximum-scale=1,user-scalable=no">
		<meta name="apple-mobile-web-app-capable" content="yes">
		<meta name="apple-mobile-web-app-status-bar-style" content="black">

		<!--标准mui.css-->
		<link rel="stylesheet" href="../css/mui.min.css">
        <link rel="stylesheet" type="text/css" href="../css/icons-extra.css" />
		<!--App自定义的css-->
		<!--<link rel="stylesheet" type="text/css" href="../css/app.css"/>-->
	</head>

	<body>

		<!--<header class="mui-bar mui-bar-nav">
		    <a class="mui-action-back mui-icon mui-icon-left-nav mui-pull-left"></a>
		    <h1 class="mui-title">9宫格默认样式</h1>
		</header>-->
		<div class="mui-content">
		        <ul class="mui-table-view mui-grid-view mui-grid-9">
		            <li class="mui-table-view-cell mui-media mui-col-xs-4 mui-col-sm-3" onclick="BuyPoints();" style="padding-left:0px;padding-right:0px;"><a href="#" id="BuyPoints">
		                    <span class="mui-icon-extra mui-icon-extra-cart"></span>
		                    <div class="mui-media-body">购买运费券</div></a></li>
		            <li class="mui-table-view-cell mui-media mui-col-xs-4 mui-col-sm-3" onclick="PayPoints();"style="padding-left:0px;padding-right:0px;"><a href="#" id="PayPoints">
		                    <span class="mui-icon-extra mui-icon-extra-sweep"></span>
		                    <div class="mui-media-body">支付运费券</div></a></li>
		            <li class="mui-table-view-cell mui-media mui-col-xs-4 mui-col-sm-3" onclick="ReceivePoints();"style="padding-left:0px;padding-right:0px;"><a href="#" id="ReceivePoints">
		                    <span class="mui-icon-extra mui-icon-extra-gold"></span>
		                    <div class="mui-media-body">接收运费券</div></a></li>
		            <li class="mui-table-view-cell mui-media mui-col-xs-4 mui-col-sm-3" onclick="MyCards();"style="padding-left:0px;padding-right:0px;"><a href="#" id="MyCards">
		                    <span class="mui-icon-extra mui-icon-extra-card"></span>
		                    <div class="mui-media-body">运费券余额</div></a></li>
		            <li class="mui-table-view-cell mui-media mui-col-xs-4 mui-col-sm-3" onclick="MyOrders();"style="padding-left:0px;padding-right:0px;"><a href="#" id="MyOrders">
		                    <span class="mui-icon-extra mui-icon-extra-order"></span>
		                    <div class="mui-media-body">我的订单</div></a></li>
		            <li class="mui-table-view-cell mui-media mui-col-xs-4 mui-col-sm-3" onclick="TransactionRecords();"style="padding-left:0px;padding-right:0px;"><a href="#" id="TransactionRecords">
		                    <span class="mui-icon-extra mui-icon-extra-topic"></span>
		                    <div class="mui-media-body">交易记录</div></a></li>
		            <li class="mui-table-view-cell mui-media mui-col-xs-4 mui-col-sm-3" onclick="MySettings();"style="padding-left:0px;padding-right:0px;"><a href="#" id="MySettings">
		                    <span class="mui-icon mui-icon-gear"></span>
		                    <div class="mui-media-body">我的设置</div></a></li>
		            <li class="mui-table-view-cell mui-media mui-col-xs-4 mui-col-sm-3" onclick="ApplyPoints();"style="padding-left:0px;padding-right:0px;"><a href="#" id="ApplyPoints">
		                    <span class="mui-icon-extra mui-icon-extra-prech"></span>
		                    <div class="mui-media-body">申请运费券</div></a></li>
		           <li class="mui-table-view-cell mui-media mui-col-xs-4 mui-col-sm-3" onclick="CZSM();"style="padding-left:0px;padding-right:0px;"><a href="#" id="CZSM">
		                    <span class="mui-icon-extra mui-icon-extra-dictionary"></span>
		                    <div class="mui-media-body">操作说明</div></a></li>
		        </ul> 
		</div>
	</body>
	<script src="../js/mui.min.js"></script>
    <script src="../js/mui.showLoading.js"></script>
    <script src="../js/app.js"></script>
    <script src="../js/MyOwnJS/tijiaoshenqing.js"></script>
	<script type="text/javascript">
	    function BuyPoints() {
	        document.location.href = "BuyPointsList.html";
	    }
	    function ReceivePoints() {
	        document.location.href = "ReceivePoints.html";
	    }
	    function MyCards() {
	        document.location.href = "MyCards.html";
	    }
	    function MyOrders() {
	        document.location.href = "MyOrders.html";
	    }
	    function TransactionRecords() {
	        document.location.href = "OrderList.html";
	    }
	    //var BuyPointsButton = doc.getElementById('BuyPoints');
	    //BuyPointsButton.addEventListener('tap', function (event) {
	    //    document.location.href = "BuyPointsList.html";
	    //}, false);

	    //var MyOrdersButton = doc.getElementById('MyOrders');
	    //MyOrdersButton.addEventListener('tap', function (event) {
	    //    document.location.href = "MyOrders.html";
	    //}, false);

	    //var TransactionRecordsButton = doc.getElementById('TransactionRecords');
	    //TransactionRecordsButton.addEventListener('tap', function (event) {
	    //    document.location.href = "OrderList.html";
	    //}, false);

	    //var MyCardsButton = doc.getElementById('MyCards');
	    //MyCardsButton.addEventListener('tap', function (event) {
	    //    document.location.href = "MyCards.html";
	    //}, false);

	    function PayPoints() {
	        document.location.href = "PayPoints.aspx";
	    }

	    //var PayPointsButton = doc.getElementById('PayPoints');
	    //PayPointsButton.addEventListener('tap', function (event) {
	    //    document.location.href = "PayPoints.aspx";
	    //}, false);
	    function MySettings() {
	        document.location.href = "MySettings.html";
	    }
	    function ApplyPoints() {
	        panduanyh(localStorage.getItem("mgps_UserName"));
	        // document.location.href = "ApplyPoints.aspx";
	    }
	    function CZSM() {
	        document.location.href = "SYSH.html";
	        // document.location.href = "ApplyPoints.aspx";
	    }
	    //var MySettingsButton = doc.getElementById('MySettings');
	    //MySettingsButton.addEventListener('tap', function (event) {
	    //    document.location.href = "MySettings.html";
	    //}, false);

	    //var ApplyPointsButton = doc.getElementById('ApplyPoints');
	    //ApplyPointsButton.addEventListener('tap', function (event) {
	    //    panduanyh(localStorage.getItem("mgps_UserName"));
	    //}, false);
	</script>
</html>
