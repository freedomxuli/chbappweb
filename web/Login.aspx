<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=8" />
    <meta name="renderer" content="webkit" />
    <title></title>
    <script src="JS/jquery-1.7.1.min.js"></script>
    <script src="JS/jquery.blockUI.js"></script>
    <script src="JS/jquery.cb.js"></script>
    <script type="text/javascript" src="js/extjs/ext-all.js"></script>
    <link rel="Stylesheet" type="text/css" href="js/extjs/resources/css/ext-all.css" />
    <script type="text/javascript" src="js/extjs/ext-lang-zh_CN.js"></script>

    <script type="text/javascript" src="js/json.js"></script>

    <script type="text/javascript" src="js/cb.js"></script>

    <script type="text/javascript" src="js/fun.js"></script>
    <style type="text/css">
        html, body {
            height: 100%;
            margin: 0px;
            padding: 0px;
        }

        body {
            background: url(Images/BG1.PNG) no-repeat no-repeat center center;
        }

        #username {
            border-style: none;
            border-color: inherit;
            border-width: 0px;
            background-color: transparent;
            height: 60px;
            line-height: 60px;
            width: 100%;
            font-size: 28px;
        }

        #captcha {
            border-style: none;
            border-color: inherit;
            border-width: 0px;
            background-color: transparent;
            height: 60px;
            line-height: 60px;
            width: 100%;
            font-size: 28px;
        }

        #password {
            border-style: none;
            border-color: inherit;
            border-width: 0px;
            background-color: transparent;
            height: 60px;
            line-height: 60px;
            width: 100%;
            font-size: 28px;
        }

        #container {
            width: 950px;
            height: 802px;
            background: url(Images/login/login_form_bg2.png) no-repeat no-repeat center center;
            position: absolute;
            top: 10px;
            left: 50%;
            margin-left: -480px;
            /*margin-top: -280px;*/
        }

        #btnSubmit {
            width: 510px;
            height: 64px;
            position: relative;
            cursor: pointer;
        }

        #imgcode {
            height: 60px;
            position: relative;
            cursor: pointer;
        }

        input {
            outline: none;
            background: transparent;
            border: none;
            outline: medium;
        }

        *:focus {
            outline: none;
            background-color: transparent;
        }

        ::selection {
            background: transparent;
        }

        ::-moz-selection {
            background: transparent;
        }
    </style>
</head>
<body onkeydown="Send()">

    <div class="wrapmiddle">
        <div class="wrapsub">
            <div class="wrapbox" style="width: 90%; height: 500px; margin: 0 auto;">
                <div style="width: 59%; height: 100%; float: left; text-align: center;">

                    <img src="images/flogo.png" style="margin: 0 auto; margin-top: 30%; width: 80%">
                </div>
                <div style="width: 40%; height: 100%; float: left;">

                    <table border="0" width="100%" align="center" style="margin-top: 250px; background-color: #fff; height: 450px;">
                        <tr>
                            <td align="center" valign="top" height="30" style="border: 0px solid #ff0000;"></td>
                        </tr>
                        <tr>
                            <td align="center" valign="top" height="60">

                                <div style="height: 65px; border: 1px solid #808080; border-radius: 10px; width: 80%; float: left; margin-left: 30px;">

                                    <table border="0" width="100%" align="center" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td align="center" valign="top" height="35" class="auto-style2" width="200" style="background-color: #e2e2e2; line-height: 65px; font-size: 28px;">账&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;号</td>
                                            <td align="left" valign="middle" height="35">
                                                <input id="username" type="text" />
                                            </td>
                                        </tr>

                                    </table>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" valign="top" height="10" style="border: 0px solid #ff0000;"></td>
                        </tr>
                        <tr>
                            <td align="center" valign="top" height="60">
                                <div style="height: 65px; border: 1px solid #808080; border-radius: 10px; width: 80%; float: left; margin-left: 30px;">

                                    <table border="0" width="100%" align="center" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td align="center" valign="top" height="60" class="auto-style2" width="200" style="background-color: #e2e2e2; line-height: 65px; font-size: 28px;">密&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;码</td>
                                            <td align="left" valign="middle" height="60">
                                                <input id="password" type="password" />
                                            </td>
                                        </tr>

                                    </table>
                                </div>


                            </td>
                        </tr>
                        <tr>
                            <td align="center" valign="top" height="10" style="border: 0px solid #ff0000;"></td>
                        </tr>
                        <tr>
                            <td align="center" valign="top" height="35">

                                <div style="height: 65px; border: 1px solid #808080; border-radius: 10px; width: 60%; float: left; margin-left: 30px;">

                                    <table border="0" width="100%" align="center" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td align="center" valign="top" height="65" width="200" style="background-color: #e2e2e2; line-height: 65px; font-size: 28px;">验&nbsp;&nbsp;证&nbsp;&nbsp;码</td>
                                            <td align="left" valign="middle" height="65">
                                                <input id="captcha" type="text" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <div style="height: 35px; border-radius: 10px; width: 20%; float: left">
                                    <img id="imgcode" src="captcha.aspx?vctype=log" style="cursor: pointer; vertical-align: top; margin-top: 3px;" onclick='code(this);' />

                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" valign="top" height="22" style="border: 0px solid #ff0000;"></td>
                        </tr>
                        <tr>
                            <td align="center" valign="top" height="35" style="text-align: left;">
                                <img src="images/dlbtn.png" onclick="Login()" style="margin-left: 30px;">
                            </td>
                        </tr>
                    </table>
                     <table border="0" width="100%" align="center" cellpadding="0" cellspacing="0">
                           <tr>
                            <td align="center" valign="top" height="30" style="border: 0px solid #ff0000;"></td>
                        </tr>
                        <tr>
                            <td align="left" valign="top" height="35" class="auto-style2"  >
                            <img src="images/logtel.png"   style="margin-left: 30px;">
  
                            </td>
                            
                        </tr>

                    </table>

                </div>
            </div>
        </div>
    </div>

    <%--  <div id="container">
        <img src="Images/login/login_btn.png" id="btnSubmit" />
    </div>--%>
    <script type="text/javascript">
        function Send() {
            if (window.event.keyCode == 13) {
                Login();
            }
        }
        function code(v) {
            setTimeout(function () { v.src = "captcha.aspx?vctype=log&r=" + Math.random().toString() }, 1);
        }
        function Login() {
            CS("CZCLZ.UserClass.Login", function (retVal) {
                if (!retVal) {
                    Ext.MessageBox.show({
                        title: "错误",
                        msg: "登陆失败",
                        buttons: Ext.MessageBox.OK,
                        icon: Ext.MessageBox.WARNING
                    });
                    code(document.getElementById('imgcode'));
                }
                else {
                    window.location.href = 'Main/Index.aspx';
                }
            }, function (retValue) {
                Ext.MessageBox.show({
                    title: "错误",
                    msg: retValue,
                    buttons: Ext.MessageBox.OK,
                    icon: Ext.MessageBox.WARNING
                });
                code(document.getElementById('imgcode'));
            },
            document.getElementById('username').value,
            document.getElementById('password').value,
            document.getElementById('captcha').value);
        }


        $('#btnSubmit').click(function () {
            Login();
        });
    </script>
</body>
</html>


