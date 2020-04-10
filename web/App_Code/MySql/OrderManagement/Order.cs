using Aspose.Cells;
using SmartFramework4v2.Data.MySql;
using SmartFramework4v2.Web.Common.JSON;
using SmartFramework4v2.Web.WebExecutor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

public class GJRes
{
    public DateTime timestamp { get; set; }
    public string message { get; set; }
    public string details { get; set; }
    public int code { get; set; }
    public string msg { get; set; }
}
/// <summary>
/// Order 的摘要说明
/// </summary>
[CSClass("Order")]
public class Order
{
    public Order()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    #region 订单管理-订单列表
    /// <summary>
    /// 获取操作员/业务员分页
    /// </summary>
    /// <param name="pagnum"></param>
    /// <param name="pagesize"></param>
    /// <param name="yhm"></param>
    /// <param name="xm"></param>
    /// <param name="operatortype">操作员类型（0：业务员；1：操作员；）</param>
    /// <returns></returns>
    [CSMethod("GetOperaUserByPage")]
    public object GetOperaUserByPage(int pagnum, int pagesize, string yhm, string xm, int operatortype)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(yhm.Trim()))
                {
                    where += " and " + dbc.C_Like("b.account", yhm.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(xm.Trim()))
                {
                    where += " and " + dbc.C_Like("b.username", xm.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
                }

                var cmd = dbc.CreateCommand();
                cmd.CommandText = @"SELECT a.id,b.account as UserName,b.username as UserXM,b.userid as UserID FROM tb_b_operator_association a 
                                    LEFT JOIN tb_b_user b ON a.operator=b.userid 
                                    WHERE a.status=0 and a.operatortype=" + operatortype + where + " order by a.addtime desc";
                var dt = dbc.ExecuteDataTable(cmd);

                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(cmd, pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    /// <summary>
    /// 获取订单列表分页
    /// </summary>
    /// <param name="pagnum"></param>
    /// <param name="pagesize"></param>
    /// <param name="beg"></param>
    /// <param name="end"></param>
    /// <param name="changjia"></param>
    /// <returns></returns>
    [CSMethod("GetOrderList")]
    public object GetOrderList(int pagnum, int pagesize, string beg, string end, string changjia, string ordernum, string gys, string zcsj, string cysj)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.shippingnoteadddatetime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.shippingnoteadddatetime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(changjia.Trim()))
                {
                    where += " and " + dbc.C_Like("d.username", changjia.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(ordernum.Trim()))
                {
                    where += " and " + dbc.C_Like("a.shippingnotenumber", ordernum.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(gys.Trim()))
                {
                    where += " and " + dbc.C_Like("f.username", gys.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(zcsj.Trim()))
                {
                    where += " and " + dbc.C_Like("g.username", zcsj.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(cysj.Trim()))
                {
                    where += " and " + dbc.C_Like("h.username", cysj.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
                }

                string str = @"select (SELECT COUNT(*) FROM tb_b_receiptinfo WHERE isdeleteflag=0 AND shippingnoteid=a.shippingnoteid) AS hzcount,e.username as usernamea ,a.operatorid,a.shippingnoteid,d.username,a.shippingnoteadddatetime,a.tuoyunorder,
                                a.shippingnotestatuscode,
                                CASE a.shippingnotestatuscode
	                                WHEN 0 THEN '已下单'
	                                WHEN 10 THEN '提货中'
	                                WHEN 20 THEN '待出发'
	                                WHEN 30 THEN '在途'
	                                WHEN 40 THEN '待验收付款'
	                                WHEN 90 THEN '订单完成'
	                                ELSE '差额待确认'
                                END AS shippingnotestatusname,
                                a.shippingnotenumber,d.goodsfromroute,d.goodstoroute,d.descriptionofgoods,d.totalnumberofpackages,d.itemgrossweight,d.cube,
                                d.vehicletyperequirement,d.vehiclelengthrequirement,d.istakegoods,d.estimatemoney,d.vehicletype,d.vehiclelength,d.actualoilmoney,d.actualvotemoney,
 d.actualnooilmoney,
d.actualcompletemoney,d.actualtaxmoney,d.actualcostmoney,
                                CASE d.istakegoods
	                                WHEN 0 THEN
		                                '提货'
	                                ELSE
		                                '不提'
                                END AS istakegoodsname,
                                d.isdelivergoods,
                                CASE d.isdelivergoods
	                                WHEN 0 THEN
		                                '送货'
	                                ELSE
		                                '不送'
                                END AS isdelivergoodsname,
                                d.actualcompanypay,d.actualmoney,d.totalmonetaryamount,d.memo,a.isabnormal,a.abnormalmemo,a.gpscompany,a.gpsdenno,
								(select count(*) from tb_b_shippingnoteinfo_doublepay where tb_b_shippingnoteinfo_doublepay.shippingnoteid=a.shippingnoteid) doublepaynum,d.vehiclelengthrequirementname,d.vehicleamount,
a.carrierid,f.username AS carriername,h.driverid,h.username cysj,a.takegoodsdriver,g.username AS takegoodsdrivername,a.offerid,a.actualmoneystatus,a.consignmentdatetime,d.goodsfromroutecode,d.goodstoroutecode,d.consignee,d.consicontactname,d.consitelephonenumber,d.placeofloading,d.goodsreceiptplace
                                from tb_b_shippingnoteinfo a
                                left join (
	                                select c.username,c.carriername,b.*,e.name vehiclelengthrequirementname from tb_b_sourcegoodsinfo_offer b
	                                left join tb_b_user c on b.shipperid=c.userid
                                    left join tb_b_dictionary_detail e on b.vehiclelengthrequirement=e.bm
                                ) d on a.offerid=d.offerid
                                left join tb_b_user e on a.operatorid=e.userid
left join tb_b_user f on a.carrierid=f.userid
left join (
    select h1.shippingnotenumber,h1.driverid,h2.username from tb_b_joborderinfo h1
    left join tb_b_user h2 on h1.driverid=h2.userid
) h on a.shippingnotenumber=h.shippingnotenumber
left join tb_b_user g on a.takegoodsdriver=g.userid
                                where a.isdeleteflag=0 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.shippingnoteadddatetime desc", pagesize, ref cp, out ac);
                dtPage.Columns.Add("userCZY");
                dtPage.Columns.Add("userGYS");
                dtPage.Columns.Add("userGYS_username");
                dtPage.Columns.Add("userGYS_vehiclenumber");
                dtPage.Columns.Add("userGYS_identitydocumentnumber");
             
                dtPage.Columns.Add("userZCSJ");

                dtPage.Columns.Add("userZCSJ_username");
                dtPage.Columns.Add("userZCSJ_vehiclenumber");
                dtPage.Columns.Add("userZCSJ_identitydocumentnumber");
                dtPage.Columns.Add("userZCSJ_usertel");
                foreach (DataRow dr in dtPage.Rows)
                {
                    var userCZY = "";
                    var dt = dbc.ExecuteDataTable("SELECT username FROM tb_b_shippingnoteinfo_operator a LEFT JOIN tb_b_user b ON a.userid=b.userid where a.status=0 and shippingnoteid=" + dbc.ToSqlValue(dr["shippingnoteid"].ToString()));
                    foreach (DataRow dr2 in dt.Rows)
                    {
                        userCZY += dr2[0].ToString() + ",";
                    }
                    dr["userCZY"] = userCZY;


                    var userGYS = "";
                    dt = dbc.ExecuteDataTable(@"    SELECT b.vehiclenumber,b.username,b.usertel,b.identitydocumentnumber FROM tb_b_shippingnoteinfo_cost a LEFT JOIN tb_b_user b ON a.userid=b.userid
 where a.paytype=0 and a.shippingnoteid=" + dbc.ToSqlValue(dr["shippingnoteid"].ToString()));
                    foreach (DataRow dr2 in dt.Rows)
                    {
                        userGYS += dr2["username"].ToString() + "(" + dr2["vehiclenumber"].ToString() + "," + dr2["identitydocumentnumber"].ToString() + ")" + ",";

                        dr["userGYS_username"] = dr2["username"].ToString();
                        dr["userGYS_vehiclenumber"] = dr2["vehiclenumber"].ToString();
                        dr["userGYS_identitydocumentnumber"] = dr2["identitydocumentnumber"].ToString();
                    
                    }
                    dr["userGYS"] = userGYS;


                    var userZCSJ = "";
                    dt = dbc.ExecuteDataTable(@"    SELECT b.vehiclenumber,b.username,b.usertel,b.identitydocumentnumber FROM tb_b_shippingnoteinfo_cost a LEFT JOIN tb_b_user b ON a.userid=b.userid
 where a.paytype=1 and a.shippingnoteid=" + dbc.ToSqlValue(dr["shippingnoteid"].ToString())); 
                    foreach (DataRow dr2 in dt.Rows)
                    {
                        userZCSJ += dr2["username"].ToString() + "(" + dr2["vehiclenumber"].ToString() + "," + dr2["identitydocumentnumber"].ToString() + "," + dr2["usertel"].ToString() + ")" + ",";
                        dr["userZCSJ_username"] = dr2["username"].ToString();
                        dr["userZCSJ_vehiclenumber"] = dr2["vehiclenumber"].ToString();
                        dr["userZCSJ_identitydocumentnumber"] = dr2["identitydocumentnumber"].ToString();
                        dr["userZCSJ_usertel"] = dr2["usertel"].ToString();

                    
                    }
                    dr["userZCSJ"] = userZCSJ;



                }

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("UpdateOrder")]
    public bool UpdateOrder(string orderId, JSReader jsr)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                dbc.BeginTransaction();
                DateTime ti = DateTime.Now;

                //consignmentdatetime//订单时间；tb_b_shippingnoteinfo.consignmentdatetime

                var dt = dbc.GetEmptyDataTable("tb_b_sourcegoodsinfo_offer");
                var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                var dr = dt.NewRow();

                dr["offerid"] = jsr["offerid"].ToString();
                dr["placeofloading"] = jsr["placeofloading"];//起始地地址；placeofloading
                dr["goodsfromroute"] = jsr["goodsfromroute"].ToString();//起始地省市区；tb_b_sourcegoodsinfo_offer.goodsfromroute
                dr["goodsreceiptplace"] = jsr["goodsreceiptplace"].ToString();//收货地址；tb_b_sourcegoodsinfo_offer.goodsreceiptplace
                dr["goodstoroute"] = jsr["goodstoroute"].ToString();//收货地省市区；tb_b_sourcegoodsinfo_offer.goodstoroute
                dr["descriptionofgoods"] = jsr["descriptionofgoods"].ToString();//货物名称；tb_b_sourcegoodsinfo_offer.descriptionofgoods
                dr["totalnumberofpackages"] = jsr["totalnumberofpackages"].ToString();//货物数量；tb_b_sourcegoodsinfo_offer.totalnumberofpackages
                dr["itemgrossweight"] = jsr["itemgrossweight"].ToString();//货物重量；tb_b_sourcegoodsinfo_offer.itemgrossweight
                dr["cube"] = jsr["cube"].ToString();//货物体积；tb_b_sourcegoodsinfo_offer.cube
                dr["istakegoods"] = jsr["istakegoods"].ToString();//是否提货；tb_b_sourcegoodsinfo_offer.istakegoods
                dr["isdelivergoods"] = jsr["isdelivergoods"].ToString();//是否送货；tb_b_sourcegoodsinfo_offer.isdelivergoods
                dr["consignee"] = jsr["consignee"].ToString();//收货人；tb_b_sourcegoodsinfo_offer.consignee
                dr["consicontactname"] = jsr["consicontactname"].ToString();//收货联系人；tb_b_sourcegoodsinfo_offer.consicontactname
                dr["consitelephonenumber"] = jsr["consitelephonenumber"].ToString();//收货联系电话；tb_b_sourcegoodsinfo_offer.consitelephonenumber
                dr["memo"] = jsr["memo"].ToString();//备注；tb_b_sourcegoodsinfo_offer.memo
                //dr["tuoyunorder"] = jsr["tuoyunorder"].ToString();//托运单编号；tb_b_sourcegoodsinfo_offer.tuoyunorder
                dr["updateuser"] = SystemUser.CurrentUser.UserID;
                dr["updatetime"] = DateTime.Now;

                dt.Rows.Add(dr);
                dbc.UpdateTable(dt, dtt);

				var dt1 = dbc.GetEmptyDataTable("tb_b_shippingnoteinfo");
                var dtt1 = new SmartFramework4v2.Data.DataTableTracker(dt1);
				var dr1 = dt1.NewRow();
				dr1["shippingnoteid"] = orderId;
				dr1["tuoyunorder"] = jsr["tuoyunorder"].ToString();
				dt1.Rows.Add(dr1);
                dbc.UpdateTable(dt1, dtt1);

                dbc.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }

        }
    }

    /// <summary>
    /// 选择操作员
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="userid"></param>
    /// <returns></returns>
    [CSMethod("UpdateOrderOperator")]
    public bool UpdateOrderOperator(string orderId, string userId, string userName)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                DateTime ti = DateTime.Now;

                string sql = "update tb_b_shippingnoteinfo set operatorid=" + dbc.ToSqlValue(userId) + " where shippingnoteid=" + dbc.ToSqlValue(orderId);
                dbc.ExecuteNonQuery(sql);

                LogByShippingnote(dbc, orderId, "更新操作员", ti + "更新操作员:" + userName, ti);

                dbc.CommitTransaction();
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
        return true;

    }


    /// <summary>
    /// 选择操作员
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="userid"></param>
    /// <returns></returns>
    [CSMethod("UpdateOrderOperatorList")]
    public bool UpdateOrderOperatorList(string orderId, JSReader jsr)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                DateTime ti = DateTime.Now;
                if (jsr.ToArray().Length > 0)
                {

                    dbc.ExecuteNonQuery("delete from tb_b_shippingnoteinfo_operator where shippingnoteid=" + dbc.ToSqlValue(orderId));
                    for (int i = 0; i < jsr.ToArray().Length; i++)
                    {
                        string[] arr = jsr.ToArray()[i].ToString().Split(',');
                        var userId = "";
                        var userName = "";

                        if (arr.Length > 0)
                        {
                            userId = arr[0].ToString();
                            userName = arr[1].ToString();

                        }


                        var dt = dbc.GetEmptyDataTable("tb_b_shippingnoteinfo_operator");
                        var dr = dt.NewRow();
                        dr["id"] = Guid.NewGuid();
                        dr["shippingnoteid"] = orderId;
                        dr["userid"] = userId;
                        dr["status"] = 0;

                        dr["adduser"] = SystemUser.CurrentUser.UserID;
                        dr["updateuser"] = SystemUser.CurrentUser.UserID;
                        dr["addtime"] = DateTime.Now;
                        dr["updatetime"] = DateTime.Now;


                        dt.Rows.Add(dr);
                        dbc.InsertTable(dt);


                        LogByShippingnote(dbc, orderId, "新增操作员", ti + "更新人:" + userName, ti);
                    }
                }
                dbc.CommitTransaction();
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
        return true;

    }


    /// <summary>
    /// 填写异常功能
    /// </summary>
    /// <param name="type">1:保存功能 2:解决异常功能</param>
    /// <param name="orderId"></param>
    /// <param name="abnormalmemo"></param>
    [CSMethod("UpdateOrderError")]
    public bool UpdateOrderError(int type, string orderId, string abnormalmemo)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                dbc.BeginTransaction();
                /*  保存功能：更新tb_b_shippingnoteinfo.isabnormal = 1，abnormalmemo = 填写内容
                    插入tb_b_shippingnoteinfo_record，recordtype = ‘填写异常’，recordmemo = ‘xxx 时间 填写异常xxx’
                    解决异常功能：更新tb_b_shippingnoteinfo.isabnormal = 0
                    插入tb_b_shippingnoteinfo_record，recordtype = ‘解决异常’，recordmemo = ‘xxx 时间解决异常’*/

                DateTime ti = DateTime.Now;
                if (type == 1)
                {
                    string sql = "update tb_b_shippingnoteinfo set isabnormal=1,abnormalmemo=" + dbc.ToSqlValue(abnormalmemo) + " where shippingnoteid=" + dbc.ToSqlValue(orderId);
                    dbc.ExecuteNonQuery(sql);

                    LogByShippingnote(dbc, orderId, "填写异常", ti + " 填写异常:" + abnormalmemo, ti);
                }
                else if (type == 2)
                {
                    string sql = "update tb_b_shippingnoteinfo set isabnormal=0 where shippingnoteid=" + dbc.ToSqlValue(orderId);
                    dbc.ExecuteNonQuery(sql);

                    LogByShippingnote(dbc, orderId, "解决异常", ti + " 解决异常", ti);
                }
                dbc.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }

    /// <summary>
    /// 填写gps信息功能
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="jsr"></param>
    /// <returns></returns>
    [CSMethod("UpdateOrderGPS")]
    public bool UpdateOrderGPS(string orderId, JSReader jsr)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                dbc.BeginTransaction();
                DateTime ti = DateTime.Now;
                string gpscompany = jsr["gpscompany"].ToString();
                string gpsdenno = jsr["gpsdenno"].ToString();

                string sql = "update tb_b_shippingnoteinfo set gpscompany=" + dbc.ToSqlValue(gpscompany) + ",gpsdenno=" + dbc.ToSqlValue(gpsdenno) + " where shippingnoteid=" + dbc.ToSqlValue(orderId);
                dbc.ExecuteNonQuery(sql);

                LogByShippingnote(dbc, orderId, "更新gps信息", ti + " 更新gps信息，gps企业名称：" + gpscompany + "，gps运单号：" + gpsdenno, ti);
                dbc.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }

        }
    }

    /// <summary>
    /// 获取轨迹数据JSON
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [CSMethod("GetOrderLocus")]
    public string GetOrderLocus(string orderId)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                string sql = @"select a.vehiclenumber,a.address,a.getlocationtime,c.drivername,b.goodsfromroute,b.goodstoroute,a.longitudedegree,a.latitudedegree from tb_b_locationinfo a 
                                left join (
	                                select t1.shippingnoteid,t2.goodsfromroute,t2.goodstoroute from tb_b_shippingnoteinfo t1 
	                                left join tb_b_sourcegoodsinfo_offer t2 on t1.offerid=t2.offerid
                                ) b on a.shippingnoteid=b.shippingnoteid
                                left join tb_b_joborderinfo c on a.joborderid=c.joborderid
                                where a.status=0 and a.shippingnoteid=" + dbc.ToSqlValue(orderId);

                DataTable dt = dbc.ExecuteDataTable(sql);

                if (dt.Rows.Count == 0)
                {
                    Dictionary<double, double> dic = new Dictionary<double, double>();
                    dic.Add(116.405289, 39.904987);
                    dic.Add(113.964458, 40.54664);
                    dic.Add(111.47836, 41.135964);
                    dic.Add(108.949297, 41.670904);
                    dic.Add(106.380111, 42.149509);
                    dic.Add(103.774185, 42.56996);
                    dic.Add(101.135432, 42.930601);
                    dic.Add(98.46826, 43.229964);
                    dic.Add(95.777529, 43.466798);
                    dic.Add(93.068486, 43.64009);
                    dic.Add(90.34669, 43.749086);
                    dic.Add(87.61792, 43.793308);

                    foreach (var v in dic)
                    {
                        DataRow newDr = dt.NewRow();
                        newDr["vehiclenumber"] = "苏A·88888";//车牌
                        newDr["address"] = "测试地址";//地址
                        newDr["getlocationtime"] = DateTime.Now;//时间
                        newDr["drivername"] = "测试司机";//司机姓名
                        newDr["goodsfromroute"] = "北京";//出发地
                        newDr["goodstoroute"] = "乌鲁木齐";//目的地
                        newDr["longitudedegree"] = v.Key;
                        newDr["latitudedegree"] = v.Value;
                        dt.Rows.Add(newDr);
                    }
                }

                return CreateReturnJson(true, 0, ToJson(dt));
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }

    /// <summary>
    /// 获取轨迹URL
    /// </summary>
    string ServiceURL = System.Configuration.ConfigurationManager.AppSettings["ZYServiceURL"].ToString();
    [CSMethod("GetOrderGJ")]
    public object GetOrderGJ(string orderId)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                string sql = "select * from tb_b_shippingnoteinfo where shippingnoteid=" + dbc.ToSqlValue(orderId);
                DataTable dt = dbc.ExecuteDataTable(sql);
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("订单不存在。");
                }

                string _url = ServiceURL + "getGpsLocationNew2";
                string jsonParam = new JavaScriptSerializer().Serialize(new
                {
                    userdenno = dt.Rows[0]["gpsdenno"].ToString(),
                    suoshugongsi = dt.Rows[0]["gpscompany"].ToString()

                });
                var request = (HttpWebRequest)WebRequest.Create(_url);
                request.Method = "POST";
                request.ContentType = "application/json;charset=UTF-8";
                var byteData = Encoding.UTF8.GetBytes(jsonParam);
                var length = byteData.Length;
                request.ContentLength = length;
                var writer = request.GetRequestStream();
                writer.Write(byteData, 0, length);
                writer.Close();
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();
                if (responseString.IndexOf("code") != -1)
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    GJRes gj = js.Deserialize<GJRes>(responseString);
                    throw new Exception(gj.msg);
                }


                return responseString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    /// <summary>
    /// 获取订单补单情况
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [CSMethod("GetDoublePayLine")]
    public DataTable GetDoublePayLine(string orderId)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            string sql = "select * from tb_b_shippingnoteinfo_doublepay where status=0 and shippingnoteid=" + dbc.ToSqlValue(orderId);
            DataTable dt = dbc.ExecuteDataTable(sql);

            return dt;
        }
    }

    /// <summary>
    /// 证据查看
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [CSMethod("GetDoublePayFj")]
    public DataTable GetDoublePayFj(string doublepayid)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            string sql = @"select b.mc,b.url,b.type from tb_b_shippingnoteinfo_doublepay_pic a
left join tb_b_fj b on a.fjid=b.id
where a.status=0 and a.doublepayid=" + dbc.ToSqlValue(doublepayid);
            DataTable dt = dbc.ExecuteDataTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["type"].ToString().StartsWith("image"))
                {
                    dr["type"] = 1;
                }
                else if (dr["type"].ToString().StartsWith("video"))
                {
                    dr["type"] = 2;
                }
            }
            return dt;
        }
    }

    /// <summary>
    /// 获取订单定位信息集合
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [CSMethod("GetLocationInfoLine")]
    public DataTable GetLocationInfoLine(string orderId)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            string sql = "select * from tb_b_locationinfo where status=0 and shippingnoteid=" + dbc.ToSqlValue(orderId);
            DataTable dt = dbc.ExecuteDataTable(sql);

            return dt;
        }
    }

    /// <summary>
    /// 保存订单定位信息
    /// </summary>
    /// <param name="jsr"></param>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [CSMethod("SaveLocationInfo")]
    public DataTable SaveLocationInfo(JSReader jsr, string orderId)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                DateTime ti = DateTime.Now;

                string locationid = jsr["locationid"].ToString();
                DateTime locationTime = Convert.ToDateTime(jsr["getlocationtime"].ToString());
                string address = jsr["address"].ToString();
                string memo = jsr["memo"].ToString();

                if (!string.IsNullOrEmpty(locationid))
                {
                    //更新
                    string sql = "update tb_b_locationinfo set vehiclenumber=" + dbc.ToSqlValue(jsr["vehiclenumber"].ToString()) + ",drivertel=" + dbc.ToSqlValue(jsr["drivertel"].ToString()) + ",drivername=" + dbc.ToSqlValue(jsr["drivername"].ToString()) + ",getlocationtime=" + dbc.ToSqlValue(locationTime) + ",address=" + dbc.ToSqlValue(address) + ",memo=" + dbc.ToSqlValue(memo) + " where locationid=" + dbc.ToSqlValue(locationid);
                    dbc.ExecuteNonQuery(sql);

                    LogByShippingnote(dbc, orderId, "修改定位信息", ti + " 修改定位信息，时间" + locationTime + "，地址" + address + "，备注" + memo, ti);
                }
                else
                {
                    //新增
                    string sql = @"select a.*,b.vehiclelicenseplatecolor from tb_b_joborderinfo a 
left join tb_b_vehicle b on a.vehicleid=b.vehicleid
where shippingnoteid=" + dbc.ToSqlValue(orderId);//
                    DataTable jobDt = dbc.ExecuteDataTable(sql);

                    sql = "select * from tb_b_shippingnoteinfo where shippingnoteid=" + dbc.ToSqlValue(orderId);
                    DataTable shippingDt = dbc.ExecuteDataTable(sql);

                    string joborderid = "";
                    string jobordernumber = "";
                    string vehicleid = "";
                    string vehiclenumber = "";
                    string vehiclelicenseplatecolor = "";
                    string shippingnotenumber = "";
                    if (jobDt.Rows.Count > 0)
                    {
                        joborderid = jobDt.Rows[0]["joborderid"].ToString();
                        jobordernumber = jobDt.Rows[0]["jobordernumber"].ToString();
                        jobordernumber = jobDt.Rows[0]["jobordernumber"].ToString();
                        vehicleid = jobDt.Rows[0]["vehicleid"].ToString();
                        vehiclenumber = jobDt.Rows[0]["vehiclenumber"].ToString();
                        vehiclelicenseplatecolor = jobDt.Rows[0]["vehiclelicenseplatecolor"].ToString();
                    }

                    if (shippingDt.Rows.Count > 0)
                    {
                        shippingnotenumber = shippingDt.Rows[0]["shippingnotenumber"].ToString();
                    }

                    var id = Guid.NewGuid().ToString();
                    var dt = dbc.GetEmptyDataTable("tb_b_locationinfo");
                    var dr = dt.NewRow();
                    dr["locationid"] = id;
                    dr["jobordernumber"] = jobordernumber;
                    dr["joborderid"] = joborderid;
                    dr["shippingnotenumber"] = shippingnotenumber;
                    dr["shippingnoteid"] = orderId;
                    dr["vehicleid"] = vehicleid;
                    // dr["vehiclenumber"] = vehiclenumber;

                    dr["vehiclenumber"] = jsr["vehiclenumber"].ToString();
                    dr["drivername"] = jsr["drivername"].ToString();
                    dr["drivertel"] = jsr["drivertel"].ToString();



                    dr["vehiclelicenseplatecolor"] = vehiclelicenseplatecolor;
                    //dr["vehiclespeed"] = "";
                    //dr["longitudedegree"] = "";
                    //dr["latitudedegree"] = "";
                    //dr["direction"] = "";
                    dr["address"] = address;
                    dr["memo"] = memo;
                    dr["getlocationtime"] = locationTime;
                    dr["isdeleteflag"] = 0;
                    //dr["synchronizationtime"] = "";
                    //dr["isexception"] = "";
                    dr["status"] = 0;
                    dr["adduser"] = SystemUser.CurrentUser.UserID;
                    dr["addtime"] = DateTime.Now;
                    dr["updateuser"] = SystemUser.CurrentUser.UserID;
                    dr["updatetime"] = DateTime.Now;
                    dt.Rows.Add(dr);
                    dbc.InsertTable(dt);

                    LogByShippingnote(dbc, orderId, "新增定位信息", ti + " 新增定位信息，时间" + locationTime + "，地址" + address + "，备注" + memo, ti);
                }

                dbc.CommitTransaction();


                string sqlRet = "select * from tb_b_locationinfo where status=0 and shippingnoteid=" + dbc.ToSqlValue(orderId);
                DataTable retDt = dbc.ExecuteDataTable(sqlRet);

                return retDt;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }

    }




    [CSMethod("SaveOFFER")]
    public static bool SaveOFFER(JSReader jsr, string offid)
    {

        var YHID = Guid.NewGuid().ToString();
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                if (offid != null && offid != "")
                {

                    YHID = offid;

                    var dt = dbc.GetEmptyDataTable("tb_b_sourcegoodsinfo_offer");
                    var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                    var dr = dt.NewRow();

                    dr["offerid"] = new Guid(YHID);

                    dr["estimatemoney"] = jsr["estimatemoney"].ToString();
                    dr["actualmoney"] = jsr["actualmoney"].ToString();
                    //dr["actualnooilmoney"] = jsr["actualnooilmoney"].ToString();

                    dr["actualcompanypay"] = jsr["actualcompanypay"].ToString();
                    dr["actualoilmoney"] = jsr["actualoilmoney"].ToString();
                    dr["actualvotemoney"] = jsr["actualvotemoney"].ToString();

                    //dr["actualcompletemoney"] = jsr["actualcompletemoney"].ToString();

                    //dr["actualtaxmoney"] = jsr["actualtaxmoney"].ToString();

                    //dr["actualcostmoney"] = jsr["actualcostmoney"].ToString();

                    dr["updateuser"] = SystemUser.CurrentUser.UserID;
                    dr["updatetime"] = DateTime.Now;

                    dt.Rows.Add(dr);
                    dbc.UpdateTable(dt, dtt);




                }
                else
                {

                    var dt = dbc.GetEmptyDataTable("tb_b_sourcegoodsinfo_offer");
                    var dr = dt.NewRow();
                    dr["offerid"] = new Guid(YHID);
                    dr["estimatemoney"] = jsr["estimatemoney"].ToString();
                    dr["actualmoney"] = jsr["actualmoney"].ToString();
                    dr["actualnooilmoney"] = jsr["actualnooilmoney"].ToString();

                    dr["actualcompanypay"] = jsr["actualcompanypay"].ToString();

                    dr["actualcompletemoney"] = jsr["actualcompletemoney"].ToString();

                    dr["actualtaxmoney"] = jsr["actualtaxmoney"].ToString();

                    dr["actualcostmoney"] = jsr["actualcostmoney"].ToString();






                    dr["status"] = 0;

                    dr["adduser"] = SystemUser.CurrentUser.UserID;
                    dr["updateuser"] = SystemUser.CurrentUser.UserID;
                    dr["addtime"] = DateTime.Now;
                    dr["updatetime"] = DateTime.Now;



                    dt.Rows.Add(dr);
                    dbc.InsertTable(dt);


                }
                dbc.CommitTransaction();

            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
        return true;



    }





    [CSMethod("GetBYcostList")]
    public object GetBYcostList(string shippingnoteid, int paytype)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                var cmd = dbc.CreateCommand();
                cmd.CommandText = @"SELECT a.id,a.money,b.vehiclenumber,b.username,b.usertel,b.identitydocumentnumber FROM tb_b_shippingnoteinfo_cost a LEFT JOIN tb_b_user b ON a.userid=b.userid
 where a.paytype=" + dbc.ToSqlValue(paytype) + " and a.shippingnoteid=" + dbc.ToSqlValue(shippingnoteid);
                var dt = dbc.ExecuteDataTable(cmd);

                return dt;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }

    [CSMethod("UpdateMoney")]
    public object UpdateMoney(string id, string money)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                var cmd = dbc.CreateCommand();
                cmd.CommandText = @"update tb_b_shippingnoteinfo_cost set money=" + dbc.ToSqlValue(money) + @" where id=" + dbc.ToSqlValue(id);
                dbc.ExecuteNonQuery(cmd);

                return true;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }






    /// <summary>
    /// 删除定位信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [CSMethod("DelLocationInfo")]
    public DataTable DelLocationInfo(string id, string orderId)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                dbc.BeginTransaction();
                string sql = "update tb_b_locationinfo set status=1 where locationid=" + dbc.ToSqlValue(id);
                dbc.ExecuteNonQuery(sql);

                sql = "select * from tb_b_locationinfo where locationid=" + dbc.ToSqlValue(id);
                DataTable infoDt = dbc.ExecuteDataTable(sql);

                DateTime ti = DateTime.Now;
                LogByShippingnote(dbc, orderId, "删除定位信息", ti + " 删除定位信息，时间" + infoDt.Rows[0]["getlocationtime"].ToString() + "，地址" + infoDt.Rows[0]["address"].ToString() + "，备注" + infoDt.Rows[0]["memo"].ToString(), ti);
                dbc.CommitTransaction();

                sql = "select * from tb_b_locationinfo where status=0 and shippingnoteid=" + dbc.ToSqlValue(orderId);
                DataTable dt = dbc.ExecuteDataTable(sql);

                return dt;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }

    /// <summary>
    /// 确认到达
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [CSMethod("OrderConfirm")]
    public bool OrderConfirm(string orderId)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                dbc.BeginTransaction();
                DateTime ti = DateTime.Now;

                string sql = "update tb_b_shippingnoteinfo set shippingnotestatuscode=40 where shippingnoteid=" + dbc.ToSqlValue(orderId);
                dbc.ExecuteNonQuery(sql);

                var id = Guid.NewGuid().ToString();
                var flowDt = dbc.GetEmptyDataTable("tb_b_shippingnoteinfo_flow");
                var dr = flowDt.NewRow();
                dr["flowid"] = id;
                dr["shippingnoteid"] = orderId;
                dr["shippingnotestatuscode"] = 40;
                dr["status"] = 0;
                dr["adduser"] = SystemUser.CurrentUser.UserID;
                dr["addtime"] = DateTime.Now;
                dr["updateuser"] = SystemUser.CurrentUser.UserID;
                dr["updatetime"] = DateTime.Now;
                flowDt.Rows.Add(dr);
                dbc.InsertTable(flowDt);

                sql = "select * from tb_b_joborderinfo where shippingnoteid=" + dbc.ToSqlValue(orderId);
                DataTable jobDt = dbc.ExecuteDataTable(sql);

                if (jobDt.Rows.Count > 0)
                {
                    var id2 = Guid.NewGuid().ToString();
                    var joborderChangeInfoDt = dbc.GetEmptyDataTable("tb_b_joborderchangeinfo");
                    var dr2 = joborderChangeInfoDt.NewRow();
                    dr2["joborderchangeid"] = id2;// char(36) NOT NULL COMMENT '派车状态变化 id',
                    dr2["jobordernumber"] = jobDt.Rows[0]["jobordernumber"].ToString();// varchar(50) DEFAULT NULL COMMENT '派车单号',
                    dr2["shippingnotenumber"] = jobDt.Rows[0]["shippingnotenumber"].ToString();// varchar(50) DEFAULT NULL COMMENT '订单号',
                    dr2["joborderstatuscode"] = "72";//` char(3) DEFAULT NULL COMMENT '派车单状态代码 (1:完全到达 24: 已发车 72:已确认 收货 999:其他 0: 异常终止)',
                    dr2["createdatetime"] = ti;//` datetime DEFAULT NULL COMMENT '生成时间',
                    dr2["statuschangedatetime"] = ti;//` datetime DEFAULT NULL COMMENT '变更时间',
                    //dr2["longitudedegree"] = "";//` decimal(20,8) DEFAULT NULL COMMENT '经度',
                    //dr2["latitudedegree"] = "";//` decimal(20,8) DEFAULT NULL COMMENT '纬度',
                    //dr2["isdeleteflag"] = "";//` int(11) DEFAULT NULL COMMENT '是否删除(0:否;1: 是)',
                    //dr2["synchronizationtime"] = "";//` datetime DEFAULT NULL COMMENT '同步时间',
                    //dr2["isexception"] = "";//` char(36) DEFAULT NULL COMMENT '同步标识(0:成 功，-1:失败)',
                    dr2["joborderid"] = jobDt.Rows[0]["joborderid"].ToString();//` char(36) DEFAULT NULL COMMENT '派单ID',
                    dr2["shippingnoteid"] = orderId;//` char(36) DEFAULT NULL COMMENT '订单ID',
                    joborderChangeInfoDt.Rows.Add(dr2);
                    dbc.InsertTable(joborderChangeInfoDt);
                }
                LogByShippingnote(dbc, orderId, "确认送达", ti + " 点击确认送达", ti);

                dbc.CommitTransaction();

                return true;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }

    /// <summary>
    /// 获取回执单附件集
    /// </summary>
    /// <param name="orderId"></param>
    [CSMethod("GetReceiptInfoList")]
    public DataTable GetReceiptInfoList(string orderId)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            string sql = "select * from tb_b_receiptinfo where isdeleteflag=0 and shippingnoteid=" + dbc.ToSqlValue(orderId);
            DataTable dt = dbc.ExecuteDataTable(sql);

            return dt;
        }
    }

    /// <summary>
    /// 删除回执单附件
    /// </summary>
    /// <param name="receiptinfo"></param>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [CSMethod("DelReceiptInfo")]
    public bool DelReceiptInfo(string receiptinfo, string orderId)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                dbc.BeginTransaction();
                string sql = "update tb_b_receiptinfo set isdeleteflag=1 where receiptinfo=" + dbc.ToSqlValue(receiptinfo);
                dbc.ExecuteNonQuery(sql);

                sql = "select * from tb_b_receiptinfo where receiptinfo=" + dbc.ToSqlValue(receiptinfo);
                DataTable infoDt = dbc.ExecuteDataTable(sql);

                DateTime ti = DateTime.Now;
                LogByShippingnote(dbc, orderId, "删除回执单", ti + " 删除回执单，附件url:" + infoDt.Rows[0]["photoaccessaddress"].ToString(), ti);
                dbc.CommitTransaction();


                return true;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }

    /// <summary>
    /// 新增回执单
    /// </summary>
    /// <param name="fds"></param>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [CSMethod("UploadPicForReceipt", 1)]
    public object UploadPicForReceipt(FileData[] fds, string orderId)
    {
        try
        {
            string url = System.Configuration.ConfigurationManager.AppSettings["UploadPicServerURL"].ToString() + "api/factory/uploadFile";
            WebRequest request = (HttpWebRequest)WebRequest.Create(url);
            MsMultiPartFormData form = new MsMultiPartFormData();
            form.AddStreamFile("file", fds[0].FileName, fds[0].FileBytes);
            form.PrepareFormData();
            request.ContentType = "multipart/form-data; boundary=" + form.Boundary;
            request.Method = "POST";
            Stream stream = request.GetRequestStream();
            foreach (var b in form.GetFormData())
            {
                stream.WriteByte(b);
            }
            stream.Close();
            string responseContent = "";
            using (HttpWebResponse res = (HttpWebResponse)request.GetResponse())
            {
                using (Stream resStream = res.GetResponseStream())
                {
                    byte[] buffer = new byte[1024];
                    int read;
                    while ((read = resStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        responseContent += Encoding.UTF8.GetString(buffer, 0, read);
                    }
                }
                res.Close();
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            ReceiptFJ list = js.Deserialize<ReceiptFJ>(responseContent);
            //List<ReceiptFJ> list = js.Deserialize(responseContent, typeof(List<ReceiptFJ>)) as List<ReceiptFJ>;
            #region 插入数据
            using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
            {
                try
                {
                    dbc.BeginTransaction();
                    string sqlStr = @"select * from tb_b_joborderinfo where shippingnoteid=" + dbc.ToSqlValue(orderId);
                    DataTable jobDt = dbc.ExecuteDataTable(sqlStr);

                    if (jobDt.Rows.Count == 0)
                    {
                        throw new Exception("缺少派车单");
                    }

                    DateTime ti = DateTime.Now;
                    DataTable receiptInfoDt = dbc.GetEmptyDataTable("tb_b_receiptinfo");
                    DataRow phdr = receiptInfoDt.NewRow();
                    phdr["receiptinfo"] = Guid.NewGuid();
                    phdr["joborderid"] = jobDt.Rows[0]["joborderid"].ToString();
                    phdr["jobordernumber"] = jobDt.Rows[0]["jobordernumber"].ToString();
                    phdr["shippingnoteid"] = orderId;
                    phdr["shippingnotenumber"] = jobDt.Rows[0]["shippingnotenumber"].ToString();
                    phdr["fjid"] = list.id;
                    phdr["photoaccessaddress"] = list.url;
                    phdr["receiptadditionalinformation"] = list.mc;
                    phdr["isdeleteflag"] = 0;
                    phdr["adduser"] = SystemUser.CurrentUser.UserID;
                    phdr["addtime"] = ti;
                    phdr["updateuser"] = SystemUser.CurrentUser.UserID;
                    phdr["updatetime"] = ti;
                    receiptInfoDt.Rows.Add(phdr);
                    dbc.InsertTable(receiptInfoDt);

                    LogByShippingnote(dbc, orderId, "新增回执单", ti + " 新增回执单，附件url:" + list.url, ti);
                    dbc.CommitTransaction();
                }
                catch (Exception ex)
                {
                    dbc.RoolbackTransaction();
                    throw ex;
                }

            }
            #endregion

            return new { fileurl = list.url, isdefault = 0, fileid = list.id };
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 获取订单日志
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [CSMethod("GetOrderRecord")]
    public object GetOrderRecord(string orderId)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            string sql = @"select a.shippingnotestatuscode,b.*,c.username from tb_b_shippingnoteinfo a 
                            left join tb_b_shippingnoteinfo_record b on a.shippingnoteid=b.shippingnoteid
                            left join tb_b_user c on b.adduser=c.correlationid
                            where a.shippingnoteid=" + dbc.ToSqlValue(orderId) + " order by addtime desc";
            DataTable dt = dbc.ExecuteDataTable(sql);

            return dt;
        }
    }

    /// <summary>
    /// 获取供应商分页
    /// </summary>
    /// <param name="pagnum"></param>
    /// <param name="pagesize"></param>
    /// <param name="mc"></param>
    /// <param name="typeStr"></param>
    /// <returns></returns>
    [CSMethod("GetUserByPage")]
    public object GetUserByPage(int pagnum, int pagesize, string mc, string typeStr)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if ("GYS".Equals(typeStr))
                {
                    where += " and (usertype=2 or (usertype=4 and drivertype=1))";
                }
                else if ("CYSJ".Equals(typeStr) || "ZCSJ".Equals(typeStr))
                {
                    where += " and usertype=4";
                }

                if (!string.IsNullOrEmpty(mc.Trim()))
                {
                    where += " and " + dbc.C_Like("username", mc.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
                }


                string str = @"select * from tb_b_user where status=0";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by addtime desc", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// 更新供应商
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="userId"></param>
    [CSMethod("UpdateOrderCarrierid")]
    public bool UpdateOrderCarrierid(string orderId, string userId)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                dbc.BeginTransaction();

                string sql = "select * from tb_b_user where userid=" + dbc.ToSqlValue(userId);
                DataTable userDt = dbc.ExecuteDataTable(sql);
                string userName = "";
                if (userDt.Rows.Count > 0)
                {
                    userName = userDt.Rows[0]["username"].ToString();
                }

                sql = "update tb_b_shippingnoteinfo set carrierid=" + dbc.ToSqlValue(userId) + " where shippingnoteid=" + dbc.ToSqlValue(orderId);
                dbc.ExecuteNonQuery(sql);
                //插入tb_b_shippingnoteinfo_record；recordtype = "选择供应商"，recordmemo = "xxx 在xxx时间 选择了供应商（专线/整车司机）xxx"
                DateTime ti = DateTime.Now;
                LogByShippingnote(dbc, orderId, "选择供应商", SystemUser.CurrentUser.UserName + " 在" + ti + "选择了供应商" + userName, ti);

                dbc.CommitTransaction();

                return true;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }

    /// <summary>
    /// 更新承运司机
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [CSMethod("UpdateJoborder")]
    public bool UpdateJoborder(string orderId, string userId)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                dbc.BeginTransaction();

                //订单信息
                string sql = "select * from tb_b_shippingnoteinfo where shippingnoteid=" + dbc.ToSqlValue(orderId);
                DataTable orderDt = dbc.ExecuteDataTable(sql);
                if (orderDt.Rows.Count == 0)
                {
                    throw new Exception("订单缺失，请联系管理员！");
                }
                //用户信息
                sql = "select * from tb_b_user where userid=" + dbc.ToSqlValue(userId);
                DataTable userDt = dbc.ExecuteDataTable(sql);
                string userName = "";
                string userTel = "";
                string vehiclenumber = "";
                if (userDt.Rows.Count > 0)
                {
                    userName = userDt.Rows[0]["username"].ToString();
                    userTel = userDt.Rows[0]["usertel"].ToString();
                    vehiclenumber = userDt.Rows[0]["vehiclenumber"].ToString();
                }

                sql = "update tb_b_joborderinfo set driverid=" + dbc.ToSqlValue(userId) + ",drivername=" + dbc.ToSqlValue(userName) + ",drivertel=" + dbc.ToSqlValue(userTel) + ",vehiclenumber=" + dbc.ToSqlValue(vehiclenumber) + @" 
                    where shippingnotenumber=" + dbc.ToSqlValue(orderDt.Rows[0]["shippingnotenumber"].ToString());
                dbc.ExecuteNonQuery(sql);

                //插入tb_b_shippingnoteinfo_record；recordtype = "选择承运司机"，recordmemo = "xxx 在xxx时间 选择了承运司机xxx"
                DateTime ti = DateTime.Now;
                LogByShippingnote(dbc, orderId, "选择承运司机", SystemUser.CurrentUser.UserName + " 在" + ti + "选择了承运司机" + userName, ti);

                dbc.CommitTransaction();

                return true;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }

    /// <summary>
    /// 更新装车司机
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [CSMethod("UpdateOrderTakeGoodsDriver")]
    public bool UpdateOrderTakeGoodsDriver(string orderId, string userId)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                dbc.BeginTransaction();

                string sql = "select * from tb_b_user where userid=" + dbc.ToSqlValue(userId);
                DataTable userDt = dbc.ExecuteDataTable(sql);
                string userName = "";
                if (userDt.Rows.Count > 0)
                {
                    userName = userDt.Rows[0]["username"].ToString();
                }

                sql = "update tb_b_shippingnoteinfo set takegoodsdriver=" + dbc.ToSqlValue(userId) + " where shippingnoteid=" + dbc.ToSqlValue(orderId);
                dbc.ExecuteNonQuery(sql);
                //插入tb_b_shippingnoteinfo_record；recordtype = "选择装车司机"，recordmemo = "xxx 在xxx时间 选择了装车司机xxx"
                DateTime ti = DateTime.Now;
                LogByShippingnote(dbc, orderId, "选择装车司机", SystemUser.CurrentUser.UserName + " 在" + ti + "选择了装车司机" + userName, ti);

                dbc.CommitTransaction();

                return true;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }

    /// <summary>
    /// 获取价格
    /// </summary>
    /// <param name="offerId"></param>
    /// <returns></returns>
    [CSMethod("GetOrderOffer")]
    public object GetOrderOffer(string offerId)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            string sql = "select * from tb_b_sourcegoodsinfo_offer where offerid=" + dbc.ToSqlValue(offerId);
            DataTable dt = dbc.ExecuteDataTable(sql);

            return dt;
        }
    }

    /// <summary>
    /// 实际价格变更
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="offerId"></param>
    /// <param name="jsr"></param>
    [CSMethod("UpdateOrderOffer")]
    public bool UpdateOrderOffer(string orderId, string offerId, JSReader jsr)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                dbc.BeginTransaction();

                string actualcompanypay = jsr["actualcompanypay"].ToString();//实际企业报价
                string actualmoney = jsr["actualmoney"].ToString();//实际下游成本
                string actualcompletemoney = jsr["actualcompletemoney"].ToString();//实际综合成本
                string actualtaxmoney = jsr["actualtaxmoney"].ToString();//实际税费成本
                string actualcostmoney = jsr["actualcostmoney"].ToString();//实际资金成本

                string sql = @"update tb_b_sourcegoodsinfo_offer set companyrefund=totalmonetaryamount-" + Convert.ToDecimal(actualcompanypay) + ",actualcompanypay=" + dbc.ToSqlValue(actualcompanypay) + ",actualmoney=" + dbc.ToSqlValue(actualmoney) + ",actualcompletemoney=" + dbc.ToSqlValue(actualcompletemoney) + ",actualtaxmoney=" + dbc.ToSqlValue(actualtaxmoney) + ",actualcostmoney=" + dbc.ToSqlValue(actualcostmoney) + @" 
                                where offerid = " + dbc.ToSqlValue(offerId);
                dbc.ExecuteNonQuery(sql);

                sql = "update tb_b_shippingnoteinfo set actualmoneystatus=2 where shippingnoteid=" + dbc.ToSqlValue(orderId);
                dbc.ExecuteNonQuery(sql);

                /*插入tb_b_shippingnoteinfo_record；recordtype = "市场部提交实际价格"，recordmemo = "xxx 在xxx时间 市场部提交"*/
                DateTime ti = DateTime.Now;
                LogByShippingnote(dbc, orderId, "市场部提交实际价格", SystemUser.CurrentUser.UserName + " 在" + ti + "市场部提交", ti);

                dbc.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }

    /// <summary>
    /// 企业确认实际价格
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [CSMethod("ConfirmOrderOffer")]
    public bool ConfirmOrderOffer(string orderId)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                dbc.BeginTransaction();
                string sql = "update tb_b_shippingnoteinfo set actualmoneystatus=3 where shippingnoteid=" + dbc.ToSqlValue(orderId);
                dbc.ExecuteNonQuery(sql);

                /*插入tb_b_shippingnoteinfo_record；recordtype = "企业确认实际价格"，recordmemo = "xxx 在xxx时间 企业确认实际价格"*/
                DateTime ti = DateTime.Now;
                LogByShippingnote(dbc, orderId, "企业确认实际价格", SystemUser.CurrentUser.UserName + " 在" + ti + "企业确认实际价格", ti);
                dbc.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }

    /// <summary>
    /// 补单价格变更
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="offerId"></param>
    /// <param name="jsr"></param>
    /// <returns></returns>
    [CSMethod("UpdateDoubleOrderOffer")]
    public bool UpdateDoubleOrderOffer(string orderId, string offerId, JSReader jsr)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                dbc.BeginTransaction();

                string actualcompanypay = jsr["actualcompanypay"].ToString();//实际企业报价
                string actualmoney = jsr["actualmoney"].ToString();//实际下游成本
                string actualcompletemoney = jsr["actualcompletemoney"].ToString();//实际综合成本
                string actualtaxmoney = jsr["actualtaxmoney"].ToString();//实际税费成本
                string actualcostmoney = jsr["actualcostmoney"].ToString();//实际资金成本

                string sql = @"update tb_b_sourcegoodsinfo_offer set companyrefund=" + Convert.ToDecimal(actualcompanypay) + "-totalmonetaryamount,actualcompanypay=" + dbc.ToSqlValue(actualcompanypay) + ",actualmoney=" + dbc.ToSqlValue(actualmoney) + ",actualcompletemoney=" + dbc.ToSqlValue(actualcompletemoney) + ",actualtaxmoney=" + dbc.ToSqlValue(actualtaxmoney) + ",actualcostmoney=" + dbc.ToSqlValue(actualcostmoney) + @" 
                                where offerid = " + dbc.ToSqlValue(offerId);
                dbc.ExecuteNonQuery(sql);

                sql = "update tb_b_shippingnoteinfo set actualmoneystatus=2 where shippingnoteid=" + dbc.ToSqlValue(orderId);
                dbc.ExecuteNonQuery(sql);

                /*插入tb_b_shippingnoteinfo_record；recordtype = "市场部提交补单价格"，recordmemo = "xxx 在xxx时间 市场部提交"*/
                DateTime ti = DateTime.Now;
                LogByShippingnote(dbc, orderId, "市场部提交补单价格", SystemUser.CurrentUser.UserName + " 在" + ti + "市场部提交", ti);

                dbc.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }
    #endregion

    #region 订单管理-回执列表
    /// <summary>
    /// 获取订单
    /// </summary>
    /// <param name="pagnum"></param>
    /// <param name="pagesize"></param>
    /// <param name="beg"></param>
    /// <param name="end"></param>
    /// <param name="changjia"></param>
    /// <param name="type">1 未寄送 2 已寄送</param>
    /// <returns></returns>
    [CSMethod("GetReceiptList")]
    public object GetReceiptList(int pagnum, int pagesize, string beg, string end, string changjia, int type)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.shippingnoteadddatetime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.shippingnoteadddatetime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(changjia.Trim()))
                {
                    where += " and " + dbc.C_EQ("d.username", changjia.Trim());
                }
                if (type == 1)
                {
                    where += " and issend=1 ";
                }
                else if (type == 2)
                {
                    where += " and issend=0 ";
                }
                string str = @"select a.shippingnoteid,d.username,a.shippingnoteadddatetime,
                                a.shippingnotestatuscode,
                                CASE a.shippingnotestatuscode
	                                WHEN 0 THEN '已下单'
	                                WHEN 10 THEN '提货中'
	                                WHEN 20 THEN '待出发'
	                                WHEN 30 THEN '在途'
	                                WHEN 40 THEN '待验收付款'
	                                WHEN 90 THEN '订单完成'
	                                ELSE '差额待确认'
                                END AS shippingnotestatusname,
                                a.shippingnotenumber,d.goodsfromroute,d.goodstoroute,d.goodsreceiptplace,d.consignee,d.consicontactname,d.consitelephonenumber,d.descriptionofgoods,d.totalnumberofpackages,d.itemgrossweight,d.cube,
                                d.vehicletyperequirement,d.vehiclelengthrequirement,d.carriername,d.istakegoods,
                                CASE d.istakegoods
	                                WHEN 0 THEN
		                                '提货'
	                                ELSE
		                                '不提'
                                END AS istakegoodsname,
                                d.isdelivergoods,
                                CASE d.isdelivergoods
	                                WHEN 0 THEN
		                                '送货'
	                                ELSE
		                                '不送'
                                END AS isdelivergoodsname,
                                d.actualcompanypay,d.actualmoney,d.memo,a.isabnormal,a.abnormalmemo,a.gpscompany,a.gpsdenno,
								(select count(*) from tb_b_shippingnoteinfo_doublepay where tb_b_shippingnoteinfo_doublepay.shippingnoteid=a.shippingnoteid) doublepaynum,d.vehiclelengthrequirementname
                                from tb_b_shippingnoteinfo a
                                left join (
	                                select c.username,c.carriername,b.*,e.name vehiclelengthrequirementname from tb_b_sourcegoodsinfo_offer b
	                                left join tb_b_user c on b.shipperid=c.userid
                                    left join tb_b_dictionary_detail e on b.vehiclelengthrequirement=e.bm
                                ) d on a.offerid=d.offerid
                                where a.isdeleteflag=0 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.shippingnoteadddatetime desc", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// 寄送
    /// </summary>
    /// <param name="receiptinfo"></param>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [CSMethod("UpIsSend")]
    public bool UpIsSend(string orderId)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                dbc.BeginTransaction();
                string sql = "update tb_b_shippingnoteinfo set issend=0 where shippingnoteid=" + dbc.ToSqlValue(orderId);
                dbc.ExecuteNonQuery(sql);

                DateTime ti = DateTime.Now;
                LogByShippingnote(dbc, orderId, "寄送", ti + " 点击寄送", ti);
                dbc.CommitTransaction();


                return true;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }

    /// <summary>
    /// 查看订单寄送操作历史
    /// </summary>
    /// <returns></returns>
    [CSMethod("GetShippingRecordLine")]
    public DataTable GetShippingRecordLine(string orderId)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            string sql = @"select a.*,b.username as addusername from tb_b_shippingnoteinfo_record a
            left join tb_b_user b on a.adduser=b.userid
            where a.status=0 and a.recordtype='寄送' and a.shippingnoteid=" + dbc.ToSqlValue(orderId);
            DataTable dt = dbc.ExecuteDataTable(sql);

            return dt;
        }
    }
    #endregion

    #region 订单管理-司机付款列表
    /// <summary>
    /// 司机付款列表分页
    /// </summary>
    /// <param name="pagnum"></param>
    /// <param name="pagesize"></param>
    /// <param name="changjia"></param>
    /// <param name="beg"></param>
    /// <param name="end"></param>
    /// <param name="shippingnotenumber"></param>
    /// <param name="paystatus"></param>
    /// <returns></returns>
    [CSMethod("GetDriverPayByPage")]
    public object GetDriverPayByPage(int pagnum, int pagesize, string changjia, string beg, string end, string shippingnotenumber, string paystatus)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(changjia.Trim()))
                {
                    where += " and " + dbc.C_Like("g.changjia", changjia.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and a.addtime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.addtime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(shippingnotenumber.Trim()))
                {
                    where += " and " + dbc.C_Like("g.shippingnotenumber", shippingnotenumber.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(paystatus))
                {
                    where += " and " + dbc.C_EQ("a.paystatus", paystatus);
                }
                string str = @"select a.*,g.shippingnotenumber,g.changjia,c.username sjname,g.tuoyunorder from tb_b_shippingnoteinfo_pay a
left join (
    select b.shippingnoteid,b.shippingnotenumber,b.tuoyunorder,e.username as changjia from tb_b_shippingnoteinfo b
    left join (
        select c.offerid,d.username from tb_b_sourcegoodsinfo_offer c
        left join tb_b_user d on c.shipperid=d.userid
    )e on b.offerid=e.offerid
)g on a.shippingnoteid=g.shippingnoteid
inner join tb_b_shippingnoteinfo_cost b on a.costid = b.id
left join tb_b_user c on b.userid = c.userid
where a.status=0 and a.paytype in (1,4)";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.addtime desc", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// 获取订单司机付款新增列（承运）
    /// </summary>
    /// <param name="shippingnotenumber"></param>
    /// <param name="changjia"></param>
    /// <returns></returns>
    [CSMethod("GetAddOrderList")]
    public object GetAddOrderList(String shippingnotenumber, String changjia, String addsearch_beg, String addsearch_end, String addsearch_are)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            /*搜索订单号，或者厂家信息得到列表，
             * 列表字段，“厂家”，“订单号”，“承运商”，“承运商金额”，“司机”，“司机金额”，操作列：选择，弹出框里面是“申请付款金额”，“申请付款备注”*/
            string where = "";
            if (!string.IsNullOrEmpty(shippingnotenumber.Trim()))
            {
                where += " and " + dbc.C_Like("b.shippingnotenumber", shippingnotenumber.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
            }
            if (!string.IsNullOrEmpty(changjia.Trim()))
            {
                where += " and " + dbc.C_Like("d.username", changjia.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
            }
            if (!string.IsNullOrEmpty(addsearch_beg))
            {
                where += " and b.consignmentdatetime>='" + Convert.ToDateTime(addsearch_beg).ToString("yyyy-MM-dd") + "'";
            }
            if (!string.IsNullOrEmpty(addsearch_end))
            {
                where += " and b.consignmentdatetime<='" + Convert.ToDateTime(addsearch_end).AddDays(1).ToString("yyyy-MM-dd") + "'";
            }
            if (!string.IsNullOrEmpty(addsearch_are))
            {
                where += " and " + dbc.C_Like("f.name", addsearch_are.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
            }
            String sql = @"select (IFNULL(a.money,0)-IFNULL(a.verifymoney,0)) as price,a.shippingnoteid,d.username changjia,b.shippingnotenumber,
                            e.username carriername,a.money actualwaymoney,b.consignmentdatetime,f.name as goodstoroutename,IFNULL(g.paysummoney,0) paysummoney,a.id
from tb_b_shippingnoteinfo_cost a
                            left join tb_b_shippingnoteinfo b on a.shippingnoteid = b.shippingnoteid
                            left join tb_b_sourcegoodsinfo_offer c on b.offerid = c.offerid
left join tb_b_area f on c.goodstoroutecode=f.code
                            left join tb_b_user d on c.shipperid = d.userid
                            left join tb_b_user e on a.userid = e.userid
left join (
	select shippingnoteid,sum(paymoney) paysummoney from tb_b_shippingnoteinfo_pay 
	where  (paytype=0 or paytype=3) and paystatus>=10
	GROUP BY shippingnoteid
)g on a.shippingnoteid=g.shippingnoteid
                            where a.status = 0 and paytype = 0 " + where + @"
                            order by b.shippingnoteadddatetime desc";
            DataTable dt = dbc.ExecuteDataTable(sql);
            return dt;
        }
    }

    /// <summary>
    /// 获取订单司机付款新增列（司机）
    /// </summary>
    /// <param name="shippingnotenumber"></param>
    /// <param name="changjia"></param>
    /// <returns></returns>
    [CSMethod("GetAddOrderList2")]
    public object GetAddOrderList2(String shippingnotenumber, String changjia, String addsearch_beg, String addsearch_end, String addsearch_are)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            /*搜索订单号，或者厂家信息得到列表，
             * 列表字段，“厂家”，“订单号”，“承运商”，“承运商金额”，“司机”，“司机金额”，操作列：选择，弹出框里面是“申请付款金额”，“申请付款备注”*/
            string where = "";
            if (!string.IsNullOrEmpty(shippingnotenumber.Trim()))
            {
                where += " and " + dbc.C_Like("b.shippingnotenumber", shippingnotenumber.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
            }
            if (!string.IsNullOrEmpty(changjia.Trim()))
            {
                where += " and " + dbc.C_Like("d.username", changjia.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
            }
            if (!string.IsNullOrEmpty(addsearch_beg))
            {
                where += " and b.consignmentdatetime>='" + Convert.ToDateTime(addsearch_beg).ToString("yyyy-MM-dd") + "'";
            }
            if (!string.IsNullOrEmpty(addsearch_end))
            {
                where += " and b.consignmentdatetime<='" + Convert.ToDateTime(addsearch_end).AddDays(1).ToString("yyyy-MM-dd") + "'";
            }
            if (!string.IsNullOrEmpty(addsearch_are))
            {
                where += " and " + dbc.C_Like("f.name", addsearch_are.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
            }
            String sql = @"select (IFNULL(a.money,0)-IFNULL(a.verifymoney,0)) as price,a.shippingnoteid,d.username changjia,b.shippingnotenumber,
                            e.username drivername,a.money actualdrivermoney,b.consignmentdatetime,f.name as goodstoroutename,IFNULL(g.paysummoney,0) paysummoney,a.id
                            from tb_b_shippingnoteinfo_cost a
                            left join tb_b_shippingnoteinfo b on a.shippingnoteid = b.shippingnoteid
                            left join tb_b_sourcegoodsinfo_offer c on b.offerid = c.offerid
left join tb_b_area f on c.goodstoroutecode=f.code
                            left join tb_b_user d on c.shipperid = d.userid
                            left join tb_b_user e on a.userid = e.userid
left join (
	select shippingnoteid,sum(paymoney) paysummoney from tb_b_shippingnoteinfo_pay 
	where  (paytype=1 or paytype=2) and paystatus>=10
	GROUP BY shippingnoteid
)g on a.shippingnoteid=g.shippingnoteid
                            where a.status = 0 and paytype = 1 and a.money > 0 " + where + @"
                            order by b.shippingnoteadddatetime desc";
            DataTable dt = dbc.ExecuteDataTable(sql);
            return dt;
        }
    }

    /// <summary>
    /// 新增司机付款申请
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="jsr"></param>
    /// <returns></returns>
    [CSMethod("SaveOrderPay")]
    public bool SaveOrderPay(string orderId, JSReader jsr, string yfje,string costid)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                //验证是否能继续申请
                decimal sqjeSum = 0m;
                decimal yfjeSum = !string.IsNullOrEmpty(yfje) ? Convert.ToDecimal(yfje) : 0m;
                String sql = @"select shippingnoteid,sum(paymoney) summoney from tb_b_shippingnoteinfo_pay where status=0 and shippingnoteid=" + dbc.ToSqlValue(orderId) + " group by shippingnoteid";
                DataTable sumDt = dbc.ExecuteDataTable(sql);
                if (sumDt.Rows.Count > 0)
                {
                    sqjeSum = Convert.ToDecimal(sumDt.Rows[0]["summoney"].ToString());
                }
                else
                {
                    sqjeSum = Convert.ToDecimal(jsr["paymoney"].ToString());
                }
                if (sqjeSum > yfjeSum)
                {
                    return false;
                }


                DateTime ti = DateTime.Now;

                //新增
                string paymoney = jsr["paymoney"].ToString();
                string paymemo = jsr["paymemo"].ToString();

                var id = Guid.NewGuid().ToString();
                var dt = dbc.GetEmptyDataTable("tb_b_shippingnoteinfo_pay");
                var dr = dt.NewRow();
                dr["id"] = id;
                dr["shippingnoteid"] = orderId;
                dr["paytype"] = 1;
                dr["paystatus"] = 10;
                dr["paymoney"] = paymoney;
                dr["paymemo"] = paymemo;
                dr["status"] = 0;
                dr["adduser"] = SystemUser.CurrentUser.UserID;
                dr["addtime"] = ti;
                dr["updateuser"] = SystemUser.CurrentUser.UserID;
                dr["updatetime"] = ti;
                dr["costid"] = costid;
                dt.Rows.Add(dr);
                dbc.InsertTable(dt);

                var childDt = dbc.GetEmptyDataTable("tb_b_shippingnoteinfo_pay_flow");
                var childDr = childDt.NewRow();
                childDr["id"] = Guid.NewGuid().ToString();
                childDr["shippingnoteid"] = orderId;
                childDr["shippingnotepayid"] = id;
                childDr["paystatus"] = 10;
                /*childDr["paytype"] = 1;
                childDr["paymoney"] = paymoney;
                childDr["paymemo"] = paymemo;*/
                childDr["status"] = 0;
                childDr["adduser"] = SystemUser.CurrentUser.UserID;
                childDr["addtime"] = ti;
                childDr["updateuser"] = SystemUser.CurrentUser.UserID;
                childDr["updatetime"] = ti;
                childDt.Rows.Add(childDr);
                dbc.InsertTable(childDt);

                /*插入tb_b_shippingnoteinfo_record ，recordtype = "客服申请司机付款"，recordmemo = "xxx在xxx时间 客服申请司机付款xx元"*/
                LogByShippingnote(dbc, orderId, "客服申请司机付款", SystemUser.CurrentUser.UserName + "在" + ti + "客服申请司机付款" + paymoney + "元", ti);

                dbc.CommitTransaction();

                return true;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }

    }

    /// <summary>
    /// 操作主管申请司机付款
    /// </summary>
    /// <param name="jsr"></param>
    /// <returns></returns>
    [CSMethod("UpdateOrderPaySubmit")]
    public bool UpdateOrderPaySubmit(JSReader jsr)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                dbc.BeginTransaction();

                DateTime ti = DateTime.Now;

                for (int i = 0; i < jsr.ToArray().Length; i++)
                {
                    String id = jsr[i].ToString();

                    String sql = "select * from tb_b_shippingnoteinfo_pay where id=" + dbc.ToSqlValue(id);
                    DataTable dt = dbc.ExecuteDataTable(sql);
                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("申请不存在，请联系管理员！");
                    }
                    String orderId = dt.Rows[0]["shippingnoteid"].ToString();
                    string paymoney = dt.Rows[0]["paymoney"].ToString();

                    sql = "update tb_b_shippingnoteinfo_pay set paystatus=20 where id=" + dbc.ToSqlValue(id);
                    dbc.ExecuteNonQuery(sql);

                    var childDt = dbc.GetEmptyDataTable("tb_b_shippingnoteinfo_pay_flow");
                    var childDr = childDt.NewRow();
                    childDr["id"] = Guid.NewGuid().ToString();
                    childDr["shippingnoteid"] = orderId;
                    childDr["shippingnotepayid"] = id;
                    childDr["paystatus"] = 20;
                    childDr["status"] = 0;
                    childDr["adduser"] = SystemUser.CurrentUser.UserID;
                    childDr["addtime"] = ti;
                    childDr["updateuser"] = SystemUser.CurrentUser.UserID;
                    childDr["updatetime"] = ti;
                    childDt.Rows.Add(childDr);
                    dbc.InsertTable(childDt);

                    /*插入tb_b_shippingnoteinfo_record ，recordtype = "操作主管申请司机付款"，recordmemo = "xxx在xxx时间 操作主管申请司机付款xx元"*/
                    LogByShippingnote(dbc, orderId, "操作主管申请司机付款", SystemUser.CurrentUser.UserName + "在" + ti + "操作主管申请司机付款" + paymoney + "元", ti);
                }

                dbc.CommitTransaction();
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
        return true;

    }

    /// <summary>
    /// 财务确认司机付款
    /// </summary>
    /// <param name="jsr"></param>
    /// <returns></returns>
    [CSMethod("UpdateOrderPayConfirm")]
    public bool UpdateOrderPayConfirm(JSReader jsr)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                dbc.BeginTransaction();

                DateTime ti = DateTime.Now;

                for (int i = 0; i < jsr.ToArray().Length; i++)
                {
                    String id = jsr[i].ToString();

                    String sql = "select * from tb_b_shippingnoteinfo_pay where id=" + dbc.ToSqlValue(id);
                    DataTable dt = dbc.ExecuteDataTable(sql);
                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("申请不存在，请联系管理员！");
                    }
                    String orderId = dt.Rows[0]["shippingnoteid"].ToString();
                    string paymoney = dt.Rows[0]["paymoney"].ToString();

                    sql = "update tb_b_shippingnoteinfo_pay set paystatus=90 where id=" + dbc.ToSqlValue(id);
                    dbc.ExecuteNonQuery(sql);

                    var childDt = dbc.GetEmptyDataTable("tb_b_shippingnoteinfo_pay_flow");
                    var childDr = childDt.NewRow();
                    childDr["id"] = Guid.NewGuid().ToString();
                    childDr["shippingnoteid"] = orderId;
                    childDr["shippingnotepayid"] = id;
                    childDr["paystatus"] = 90;
                    childDr["status"] = 0;
                    childDr["adduser"] = SystemUser.CurrentUser.UserID;
                    childDr["addtime"] = ti;
                    childDr["updateuser"] = SystemUser.CurrentUser.UserID;
                    childDr["updatetime"] = ti;
                    childDt.Rows.Add(childDr);
                    dbc.InsertTable(childDt);

                    /*插入tb_b_shippingnoteinfo_record ，recordtype = "财务确认司机付款"，recordmemo = "xxx在xxx时间 财务确认司机付款xx元*/
                    LogByShippingnote(dbc, orderId, "财务确认司机付款", SystemUser.CurrentUser.UserName + "在" + ti + "财务确认司机付款" + paymoney + "元", ti);
                }

                dbc.CommitTransaction();
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
        return true;

    }
    #endregion

    #region 订单管理-承运商付款申请
    /// <summary>
    /// 承运商付款申请分页
    /// </summary>
    /// <param name="pagnum"></param>
    /// <param name="pagesize"></param>
    /// <param name="changjia"></param>
    /// <param name="beg"></param>
    /// <param name="end"></param>
    /// <param name="shippingnotenumber"></param>
    /// <param name="paystatus"></param>
    /// <returns></returns>
    [CSMethod("GetCarrierPayByPage")]
    public object GetCarrierPayByPage(int pagnum, int pagesize, string changjia, string beg, string end, string shippingnotenumber, string paystatus)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(changjia.Trim()))
                {
                    where += " and " + dbc.C_Like("g.changjia", changjia.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and a.addtime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.addtime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(shippingnotenumber.Trim()))
                {
                    where += " and " + dbc.C_Like("g.shippingnotenumber", shippingnotenumber.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(paystatus))
                {
                    where += " and " + dbc.C_EQ("a.paystatus", paystatus);
                }
                string str = @"select a.*,g.shippingnotenumber,g.changjia,c.username carriername,g.tuoyunorder from tb_b_shippingnoteinfo_pay a
left join (
    select b.shippingnoteid,b.shippingnotenumber,b.tuoyunorder,e.username as changjia from tb_b_shippingnoteinfo b
    left join (
        select c.offerid,d.username from tb_b_sourcegoodsinfo_offer c
        left join tb_b_user d on c.shipperid=d.userid
    )e on b.offerid=e.offerid
)g on a.shippingnoteid=g.shippingnoteid
inner join tb_b_shippingnoteinfo_cost b on a.costid = b.id
left join tb_b_user c on b.userid = c.userid
where a.status=0 and a.paytype in (0,3)";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.addtime desc", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// 新增承运商申请
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="jsr"></param>
    /// <param name="yfje"></param>
    /// <returns></returns>
    [CSMethod("SaveOrderCarrierPay")]
    public bool SaveOrderCarrierPay(string orderId, JSReader jsr, string yfje, string costid)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                //验证是否能继续申请
                decimal sqjeSum = 0m;
                decimal yfjeSum = !string.IsNullOrEmpty(yfje) ? Convert.ToDecimal(yfje) : 0m;
                String sql = @"select shippingnoteid,sum(paymoney) summoney from tb_b_shippingnoteinfo_pay where status=0 and shippingnoteid=" + dbc.ToSqlValue(orderId) + " group by shippingnoteid";
                DataTable sumDt = dbc.ExecuteDataTable(sql);
                if (sumDt.Rows.Count > 0)
                {
                    sqjeSum = Convert.ToDecimal(sumDt.Rows[0]["summoney"].ToString()) + Convert.ToDecimal(jsr["paymoney"].ToString());
                }
                else
                {
                    sqjeSum = Convert.ToDecimal(jsr["paymoney"].ToString());
                }
                if (sqjeSum > yfjeSum)
                {
                    return false;
                }

                DateTime ti = DateTime.Now;

                //新增
                string paymoney = jsr["paymoney"].ToString();
                string paymemo = jsr["paymemo"].ToString();

                var id = Guid.NewGuid().ToString();
                var dt = dbc.GetEmptyDataTable("tb_b_shippingnoteinfo_pay");
                var dr = dt.NewRow();
                dr["id"] = id;
                dr["shippingnoteid"] = orderId;
                dr["paytype"] = 0;
                dr["paystatus"] = 10;
                dr["paymoney"] = paymoney;
                dr["paymemo"] = paymemo;
                dr["status"] = 0;
                dr["adduser"] = SystemUser.CurrentUser.UserID;
                dr["addtime"] = ti;
                dr["updateuser"] = SystemUser.CurrentUser.UserID;
                dr["updatetime"] = ti;
                dr["costid"] = costid;
                dt.Rows.Add(dr);
                dbc.InsertTable(dt);

                var childDt = dbc.GetEmptyDataTable("tb_b_shippingnoteinfo_pay_flow");
                var childDr = childDt.NewRow();
                childDr["id"] = Guid.NewGuid().ToString();
                childDr["shippingnoteid"] = orderId;
                childDr["shippingnotepayid"] = id;
                childDr["paystatus"] = 10;
                /*childDr["paytype"] = 1;
                childDr["paymoney"] = paymoney;
                childDr["paymemo"] = paymemo;*/
                childDr["status"] = 0;
                childDr["adduser"] = SystemUser.CurrentUser.UserID;
                childDr["addtime"] = ti;
                childDr["updateuser"] = SystemUser.CurrentUser.UserID;
                childDr["updatetime"] = ti;
                childDt.Rows.Add(childDr);
                dbc.InsertTable(childDt);

                /*插入tb_b_shippingnoteinfo_record ，recordtype = "客服申请司机付款"，recordmemo = "xxx在xxx时间 客服申请承运商付款xx元*/
                LogByShippingnote(dbc, orderId, "客服申请承运商付款", SystemUser.CurrentUser.UserName + "在" + ti + "客服申请承运商付款" + paymoney + "元", ti);

                dbc.CommitTransaction();

                return true;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }

    }

    /// <summary>
    /// 操作主管申请司机付款
    /// </summary>
    /// <param name="jsr"></param>
    /// <returns></returns>
    [CSMethod("UpdateOrderCarrierPaySubmit")]
    public bool UpdateOrderCarrierPaySubmit(JSReader jsr)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                dbc.BeginTransaction();

                DateTime ti = DateTime.Now;

                for (int i = 0; i < jsr.ToArray().Length; i++)
                {
                    String id = jsr[i].ToString();

                    String sql = "select * from tb_b_shippingnoteinfo_pay where id=" + dbc.ToSqlValue(id);
                    DataTable dt = dbc.ExecuteDataTable(sql);
                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("申请不存在，请联系管理员！");
                    }
                    String orderId = dt.Rows[0]["shippingnoteid"].ToString();
                    string paymoney = dt.Rows[0]["paymoney"].ToString();

                    sql = "update tb_b_shippingnoteinfo_pay set paystatus=20 where id=" + dbc.ToSqlValue(id);
                    dbc.ExecuteNonQuery(sql);

                    var childDt = dbc.GetEmptyDataTable("tb_b_shippingnoteinfo_pay_flow");
                    var childDr = childDt.NewRow();
                    childDr["id"] = Guid.NewGuid().ToString();
                    childDr["shippingnoteid"] = orderId;
                    childDr["shippingnotepayid"] = id;
                    childDr["paystatus"] = 20;
                    childDr["status"] = 0;
                    childDr["adduser"] = SystemUser.CurrentUser.UserID;
                    childDr["addtime"] = ti;
                    childDr["updateuser"] = SystemUser.CurrentUser.UserID;
                    childDr["updatetime"] = ti;
                    childDt.Rows.Add(childDr);
                    dbc.InsertTable(childDt);

                    /*插入tb_b_shippingnoteinfo_record ，recordtype = "操作主管申请承运商付款"，recordmemo = "xxx在xxx时间 操作主管申请承运商付款xx元*/
                    LogByShippingnote(dbc, orderId, "操作主管申请承运商付款", SystemUser.CurrentUser.UserName + "在" + ti + "操作主管申请承运商付款" + paymoney + "元", ti);
                }

                dbc.CommitTransaction();
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
        return true;

    }

    /// <summary>
    /// 财务确认司机付款
    /// </summary>
    /// <param name="jsr"></param>
    /// <returns></returns>
    [CSMethod("UpdateOrderCarrierPayConfirm")]
    public bool UpdateOrderCarrierPayConfirm(JSReader jsr)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                dbc.BeginTransaction();

                DateTime ti = DateTime.Now;

                for (int i = 0; i < jsr.ToArray().Length; i++)
                {
                    String id = jsr[i].ToString();

                    String sql = "select * from tb_b_shippingnoteinfo_pay where id=" + dbc.ToSqlValue(id);
                    DataTable dt = dbc.ExecuteDataTable(sql);
                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("申请不存在，请联系管理员！");
                    }
                    String orderId = dt.Rows[0]["shippingnoteid"].ToString();
                    string paymoney = dt.Rows[0]["paymoney"].ToString();

                    sql = "update tb_b_shippingnoteinfo_pay set paystatus=90 where id=" + dbc.ToSqlValue(id);
                    dbc.ExecuteNonQuery(sql);

                    var childDt = dbc.GetEmptyDataTable("tb_b_shippingnoteinfo_pay_flow");
                    var childDr = childDt.NewRow();
                    childDr["id"] = Guid.NewGuid().ToString();
                    childDr["shippingnoteid"] = orderId;
                    childDr["shippingnotepayid"] = id;
                    childDr["paystatus"] = 90;
                    childDr["status"] = 0;
                    childDr["adduser"] = SystemUser.CurrentUser.UserID;
                    childDr["addtime"] = ti;
                    childDr["updateuser"] = SystemUser.CurrentUser.UserID;
                    childDr["updatetime"] = ti;
                    childDt.Rows.Add(childDr);
                    dbc.InsertTable(childDt);

                    /*插入tb_b_shippingnoteinfo_record ，recordtype = "财务确认承运商付款"，recordmemo = "xxx在xxx时间 财务确认承运商付款xx元*/
                    LogByShippingnote(dbc, orderId, "财务确认承运商付款", SystemUser.CurrentUser.UserName + "在" + ti + "财务确认承运商付款" + paymoney + "元", ti);
                }

                dbc.CommitTransaction();
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
        return true;

    }
    #endregion

    #region 订单管理-工作日志
    /// <summary>
    /// 获取日志分页
    /// </summary>
    /// <param name="pagnum"></param>
    /// <param name="pagesize"></param>
    /// <param name="changjia"></param>
    /// <param name="czy"></param>
    /// <param name="ywy"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    [CSMethod("GetLogByPage")]
    public object GetLogByPage(int pagnum, int pagesize, string changjia, string czy, string ywy, int type)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(changjia.Trim()))
                {
                    where += " and " + dbc.C_Like("t1.changjia", changjia.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(czy.Trim()))
                {
                    where += " and " + dbc.C_Like("t1.operatorname", czy.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(ywy.Trim()))
                {
                    where += " and " + dbc.C_Like("t1.businessname", ywy.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
                }
                string str = "";
                if (type == 1)//询价
                {
                    str = @"select t1.changjia,t1.operatorname,t1.businessname,t1.status,t.addtime,t.recordmemo from tb_b_sourcegoodsinfo_offer_record t
                            left join (
                                select a.offerid,b.username changjia,c.username operatorname,d.username businessname,a.offerstatus status from tb_b_sourcegoodsinfo_offer a
                                left join tb_b_user b on a.shipperid=b.userid
                                left join tb_b_user c on a.operatorid=c.userid
                                left join tb_b_user d on a.businessid=d.userid
                            )t1 on t.offerid=t1.offerid
                            where t.status=0 ";
                    str += where + " order by t.addtime desc";
                }
                else if (type == 2)//订单
                {
                    str = @"select t1.changjia,t1.operatorname,t1.businessname,t1.status,t.addtime,t.recordmemo from tb_b_shippingnoteinfo_record t
                            left join(
                                select a.shippingnoteid,d.changjia,e.username operatorname,f.username businessname,a.shippingnotestatuscode status from tb_b_shippingnoteinfo a
                                left join (
	                                select b.offerid,c.username changjia from tb_b_sourcegoodsinfo_offer b
	                                left join tb_b_user c on b.shipperid=c.userid
                                ) d on a.offerid=d.offerid
                                left join tb_b_user e on a.operatorid=e.userid
                                left join tb_b_user f on a.businessid=f.userid
                            )t1 on t.shippingnoteid=t1.shippingnoteid
                            where t.status=0 ";
                    str += where + " order by t.addtime desc";
                }

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str, pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// 日志导出
    /// </summary>
    /// <param name="changjia"></param>
    /// <param name="czy"></param>
    /// <param name="ywy"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    [CSMethod("ExportLog", 2)]
    public byte[] ExportLog(string changjia, string czy, string ywy, int type)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                #region 获取数据
                string where = "";
                if (!string.IsNullOrEmpty(changjia.Trim()))
                {
                    where += " and " + dbc.C_Like("t1.changjia", changjia.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(czy.Trim()))
                {
                    where += " and " + dbc.C_Like("t1.operatorname", czy.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(ywy.Trim()))
                {
                    where += " and " + dbc.C_Like("t1.businessname", ywy.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
                }
                string str = "";
                if (type == 1)//询价
                {
                    str = @"select t1.changjia,t1.operatorname,t1.businessname,t1.status,t.addtime,t.recordmemo from tb_b_sourcegoodsinfo_offer_record t
                            left join (
                                select a.offerid,b.username changjia,c.username operatorname,d.username businessname,a.offerstatus status from tb_b_sourcegoodsinfo_offer a
                                left join tb_b_user b on a.shipperid=b.userid
                                left join tb_b_user c on a.operatorid=c.userid
                                left join tb_b_user d on a.businessid=d.userid
                            )t1 on t.offerid=t1.offerid
                            where t.status=0 ";
                    str += where + " order by t.addtime desc";
                }
                else if (type == 2)//订单
                {
                    str = @"select t1.changjia,t1.operatorname,t1.businessname,t1.status,t.addtime,t.recordmemo from tb_b_shippingnoteinfo_record t
                            left join(
                                select a.shippingnoteid,d.changjia,e.username operatorname,f.username businessname,a.shippingnotestatuscode status from tb_b_shippingnoteinfo a
                                left join (
	                                select b.offerid,c.username changjia from tb_b_sourcegoodsinfo_offer b
	                                left join tb_b_user c on b.shipperid=c.userid
                                ) d on a.offerid=d.offerid
                                left join tb_b_user e on a.operatorid=e.userid
                                left join tb_b_user f on a.businessid=f.userid
                            )t1 on t.shippingnoteid=t1.shippingnoteid
                            where t.status=0 ";
                    str += where + " order by t.addtime desc";
                }
                #endregion
                DataTable dt = dbc.ExecuteDataTable(str);

                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(); //工作簿
                Aspose.Cells.Worksheet sheet = workbook.Worksheets[0]; //工作表
                Aspose.Cells.Cells cells = sheet.Cells;//单元格

                #region 样式
                //样式1
                Aspose.Cells.Style style1 = workbook.Styles[workbook.Styles.Add()];//新增样式    
                style1.HorizontalAlignment = TextAlignmentType.Center;//文字居中
                style1.Font.Name = "宋体";//文字字体
                style1.Font.Size = 12;//文字大小
                style1.IsTextWrapped = true;//单元格内容自动换行
                style1.Font.IsBold = true;//粗体
                style1.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                style1.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style1.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style1.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                //样式2
                Aspose.Cells.Style style2 = workbook.Styles[workbook.Styles.Add()];//新增样式    
                style2.HorizontalAlignment = TextAlignmentType.Left;//文字居左
                style2.Font.Name = "宋体";//文字字体
                style2.Font.Size = 12;//文字大小
                style2.IsTextWrapped = true;//单元格内容自动换行
                style2.Font.IsBold = false;//粗体
                style2.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                //样式3
                Aspose.Cells.Style style3 = workbook.Styles[workbook.Styles.Add()];//新增样式    
                style3.HorizontalAlignment = TextAlignmentType.Center;//文字居左
                style3.Font.Name = "宋体";//文字字体
                style3.Font.Size = 20;//文字大小
                style3.IsTextWrapped = true;//单元格内容自动换行
                style3.Font.IsBold = true;//粗体


                //样式4
                Aspose.Cells.Style style4 = workbook.Styles[workbook.Styles.Add()];//新增样式    
                style4.HorizontalAlignment = TextAlignmentType.Left;//文字居左
                style4.Font.Name = "宋体";//文字字体
                style4.Font.Size = 10;//文字大小
                style4.Font.Color = Color.Red;//文字大小

                style4.IsTextWrapped = true;//单元格内容自动换行
                style4.Font.IsBold = false;//粗体
                #endregion

                //第一行标题列表
                //合并单元格cells.Merge(1, 0, 3, 1) 参数1代表当前行，参数0代表当前行当前列即第一行第一列，参数3合并的行数，参数4合并的列数
                /*
                 * 列表字段，询价：厂家，操作员，业务员，询价状态，操作时间，操作内容
                 * 列表字段，订单：厂家，操作员，业务员，订单状态，操作时间，操作内容
                 */
                cells.SetColumnWidth(0, 30);
                cells[0, 0].PutValue("厂家");
                cells[0, 0].SetStyle(style1);
                cells.SetColumnWidth(1, 15);
                cells[0, 1].PutValue("操作员");
                cells[0, 1].SetStyle(style1);
                cells.SetColumnWidth(2, 15);
                cells[0, 2].PutValue("业务员");
                cells[0, 2].SetStyle(style1);
                cells.SetColumnWidth(3, 15);
                cells[0, 3].PutValue("询价状态");
                cells[0, 3].SetStyle(style1);
                cells.SetColumnWidth(4, 30);
                cells[0, 4].PutValue("操作时间");
                cells[0, 4].SetStyle(style1);
                cells.SetColumnWidth(5, 100);
                cells[0, 5].PutValue("操作内容");
                cells[0, 5].SetStyle(style1);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var status = "";
                        if (dt.Rows[i]["status"] != null && dt.Rows[i]["status"].ToString() != "")
                        {
                            if (type == 1)
                            {
                                //报价单状态，（0：已咨询；1：已下单；2：已过期；）
                                switch (dt.Rows[i]["status"].ToString())
                                {
                                    case "0":
                                        status = "已咨询";
                                        break;
                                    case "1":
                                        status = "已下单";
                                        break;
                                    case "2":
                                        status = "已过期";
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else if (type == 2)
                            {
                                //订单状态代码，（【未完成】0：已下单；10：提货中；20：待出发；30：在途；）（【已完成】40：待验收付款；90：已验收付款；）（【异常】21：差额待确认；）
                                switch (dt.Rows[i]["status"].ToString())
                                {
                                    case "0":
                                        status = "已下单";
                                        break;
                                    case "10":
                                        status = "提货中";
                                        break;
                                    case "20":
                                        status = "待出发";
                                        break;
                                    case "30":
                                        status = "在途";
                                        break;
                                    case "40":
                                        status = "待验收付款";
                                        break;
                                    case "90":
                                        status = "已验收付款";
                                        break;
                                    case "21":
                                        status = "差额待确认";
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }

                        cells[i + 1, 0].PutValue(dt.Rows[i]["changjia"].ToString());
                        cells[i + 1, 0].SetStyle(style2);
                        cells[i + 1, 1].PutValue(dt.Rows[i]["operatorname"].ToString());
                        cells[i + 1, 1].SetStyle(style2);
                        cells[i + 1, 2].PutValue(dt.Rows[i]["businessname"].ToString());
                        cells[i + 1, 2].SetStyle(style2);
                        cells[i + 1, 3].PutValue(status);
                        cells[i + 1, 3].SetStyle(style2);
                        if (dt.Rows[i]["addtime"].ToString() != "")
                        {
                            cells[i + 1, 4].PutValue(Convert.ToDateTime(dt.Rows[i]["addtime"].ToString()).ToString("yyyy-MM-dd HH:mm:dd"));
                        }
                        else
                        {
                            cells[i + 1, 4].PutValue("");

                        }
                        cells[i + 1, 4].SetStyle(style2);
                        cells[i + 1, 5].PutValue(dt.Rows[i]["recordmemo"].ToString());
                        cells[i + 1, 5].SetStyle(style2);
                    }
                }


                System.IO.MemoryStream ms = workbook.SaveToStream();
                byte[] bt = ms.ToArray();
                return bt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    #endregion

    #region 订单管理-订单汇总
    /// <summary>
    /// 汇总
    /// </summary>
    /// <returns></returns>
    [CSMethod("Summary")]
    public DataTable Summary()
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            /*未完结任务：x单；已询价未下单：x单；已询价已下单：x单；询价失效：x单；*/
            DataTable dt = new DataTable();
            dt.Columns.Add("wwj");
            dt.Columns.Add("yzx");
            dt.Columns.Add("yxd");
            dt.Columns.Add("ysx");

            DataRow newDr = dt.NewRow();
            String sql = @"select count(shippingnoteid) from tb_b_shippingnoteinfo where isdeleteflag=0 and shippingnotestatuscode<90";
            DataTable sumWwjDt = dbc.ExecuteDataTable(sql);
            newDr["wwj"] = Convert.ToInt32(sumWwjDt.Rows[0][0]);

            /*offerstatus 报价单状态，（0：已咨询；1：已下单；2：已过期；）*/
            sql = @"select count(shippingnotenumber) from tb_b_sourcegoodsinfo_offer where status=0 and offerstatus=0";
            DataTable sumYzxDt = dbc.ExecuteDataTable(sql);
            newDr["yzx"] = Convert.ToInt32(sumYzxDt.Rows[0][0]);

            sql = @"select count(shippingnotenumber) from tb_b_sourcegoodsinfo_offer where status=0 and offerstatus=1";
            DataTable sumYxdDt = dbc.ExecuteDataTable(sql);
            newDr["yxd"] = Convert.ToInt32(sumYxdDt.Rows[0][0]);

            sql = @"select count(shippingnotenumber) from tb_b_sourcegoodsinfo_offer where status=0 and offerstatus=2";
            DataTable sumYsxDt = dbc.ExecuteDataTable(sql);
            newDr["ysx"] = Convert.ToInt32(sumYsxDt.Rows[0][0]);
            dt.Rows.Add(newDr);

            return dt;
        }
    }

    /// <summary>
    /// 询单列表导出
    /// </summary>
    /// <param name="beg"></param>
    /// <param name="end"></param>
    /// <param name="changjia"></param>
    /// <returns></returns>
    [CSMethod("ExportSourceGoods", 2)]
    public byte[] ExportSourceGoods(string beg, string end, string changjia, string offerstatus, string flowstatus)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                #region 获取数据
                string where = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.addtime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.addtime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(changjia.Trim()))
                {
                    where += " and " + dbc.C_EQ("b.username", changjia.Trim());
                }
                if (!string.IsNullOrEmpty(offerstatus))
                {
                    where += " and " + dbc.C_EQ("a.offerstatus", offerstatus.Trim());
                }
                if (!string.IsNullOrEmpty(flowstatus))
                {
                    where += " and " + dbc.C_EQ("a.flowstatus", flowstatus.Trim());
                }
                string str = @"select a.*,b.username,b.carriername,c.name vehiclelengthrequirementname from tb_b_sourcegoodsinfo_offer a
                            left join tb_b_user b on a.shipperid=b.userid
                            left join tb_b_dictionary_detail c on a.vehiclelengthrequirement=c.bm
                            where a.shipperid in(
                                select d.userid from tb_b_operator_association d
                                left join tb_b_user e on d.userid=e.userid
                                inner join tb_b_user f on d.operator=f.userid and f.correlationid=" + dbc.ToSqlValue(SystemUser.CurrentUser.UserID) + @"
                                where d.status = 0
                            )";
                str += where;
                str += " order by addtime desc";
                #endregion
                DataTable dt = dbc.ExecuteDataTable(str);

                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(); //工作簿
                Aspose.Cells.Worksheet sheet = workbook.Worksheets[0]; //工作表
                Aspose.Cells.Cells cells = sheet.Cells;//单元格

                #region 样式
                //样式1
                Aspose.Cells.Style style1 = workbook.Styles[workbook.Styles.Add()];//新增样式    
                style1.HorizontalAlignment = TextAlignmentType.Center;//文字居中
                style1.Font.Name = "宋体";//文字字体
                style1.Font.Size = 12;//文字大小
                style1.IsTextWrapped = true;//单元格内容自动换行
                style1.Font.IsBold = true;//粗体
                style1.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                style1.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style1.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style1.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                //样式2
                Aspose.Cells.Style style2 = workbook.Styles[workbook.Styles.Add()];//新增样式    
                style2.HorizontalAlignment = TextAlignmentType.Left;//文字居左
                style2.Font.Name = "宋体";//文字字体
                style2.Font.Size = 12;//文字大小
                style2.IsTextWrapped = true;//单元格内容自动换行
                style2.Font.IsBold = false;//粗体
                style2.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                //样式3
                Aspose.Cells.Style style3 = workbook.Styles[workbook.Styles.Add()];//新增样式    
                style3.HorizontalAlignment = TextAlignmentType.Center;//文字居左
                style3.Font.Name = "宋体";//文字字体
                style3.Font.Size = 20;//文字大小
                style3.IsTextWrapped = true;//单元格内容自动换行
                style3.Font.IsBold = true;//粗体


                //样式4
                Aspose.Cells.Style style4 = workbook.Styles[workbook.Styles.Add()];//新增样式    
                style4.HorizontalAlignment = TextAlignmentType.Left;//文字居左
                style4.Font.Name = "宋体";//文字字体
                style4.Font.Size = 10;//文字大小
                style4.Font.Color = Color.Red;//文字大小

                style4.IsTextWrapped = true;//单元格内容自动换行
                style4.Font.IsBold = false;//粗体
                #endregion

                String[] ColumnName = { "厂家", "询价时间", "询价状态", "询价流程状态", "单号", "起始地", "目的地", "收货地址", "收货方", "收货联系人", "收货联系方式", "货物", "数量", "重量", "体积", "车型", "车长", "承运方", "是否提货", "是否送货", "预付运费", "预估企业报价", "预估下游成本", "预估税费成本", "预估资金成本", "备注" };
                String[] ColumnV = { "username", "addtime", "offerstatus", "flowstatus", "shippingnotenumber", "goodsfromroute", "goodstoroute", "goodsreceiptplace", "consignee", "consicontactname", "consitelephonenumber", "descriptionofgoods", "totalnumberofpackages", "itemgrossweight", "cube", "vehicletyperequirement", "vehiclelengthrequirementname", "carriername", "istakegoods", "isdelivergoods", "actualcompanypay", "totalmonetaryamount", "estimatemoney", "estimatetaxmoney", "estimatecostmoney", "memo" };
                for (int i = 0; i < ColumnName.Length; i++)
                {
                    cells.SetColumnWidth(i, 30);
                    cells[0, i].PutValue(ColumnName[i]);
                    cells[0, i].SetStyle(style1);
                }

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int a = 0; a < ColumnV.Length; a++)
                        {
                            cells[i + 1, a].PutValue(dt.Rows[i][ColumnV[a]].ToString());
                            cells[i + 1, a].SetStyle(style2);
                        }
                    }
                }

                System.IO.MemoryStream ms = workbook.SaveToStream();
                byte[] bt = ms.ToArray();
                return bt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// 订单列表导出
    /// </summary>
    /// <param name="beg"></param>
    /// <param name="end"></param>
    /// <param name="changjia"></param>
    /// <returns></returns>
    [CSMethod("ExportOrder", 2)]
    public byte[] ExportOrder(string beg, string end, string changjia)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                #region 获取数据
                string where = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.shippingnoteadddatetime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.shippingnoteadddatetime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(changjia.Trim()))
                {
                    where += " and " + dbc.C_Like("d.username", changjia.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
                }

                string str = @"select e.username as usernamea ,a.operatorid,a.shippingnoteid,d.username,a.shippingnoteadddatetime,
                                a.shippingnotestatuscode,
                                CASE a.shippingnotestatuscode
	                                WHEN 0 THEN '已下单'
	                                WHEN 10 THEN '提货中'
	                                WHEN 20 THEN '待出发'
	                                WHEN 30 THEN '在途'
	                                WHEN 40 THEN '待验收付款'
	                                WHEN 90 THEN '订单完成'
	                                ELSE '差额待确认'
                                END AS shippingnotestatusname,
                                a.shippingnotenumber,d.goodsfromroute,d.goodstoroute,d.goodsreceiptplace,d.consignee,d.consicontactname,d.consitelephonenumber,d.descriptionofgoods,d.totalnumberofpackages,d.itemgrossweight,d.cube,
                                d.vehicletyperequirement,d.vehiclelengthrequirement,d.istakegoods,
                                CASE d.istakegoods
	                                WHEN 0 THEN
		                                '提货'
	                                ELSE
		                                '不提'
                                END AS istakegoodsname,
                                d.isdelivergoods,
                                CASE d.isdelivergoods
	                                WHEN 0 THEN
		                                '送货'
	                                ELSE
		                                '不送'
                                END AS isdelivergoodsname,
                                d.actualcompanypay,d.actualmoney,d.totalmonetaryamount,d.memo,a.isabnormal,a.abnormalmemo,a.gpscompany,a.gpsdenno,
								(select count(*) from tb_b_shippingnoteinfo_doublepay where tb_b_shippingnoteinfo_doublepay.shippingnoteid=a.shippingnoteid) doublepaynum,d.vehiclelengthrequirementname,
a.carrierid,f.username AS carriername,h.driverid,a.takegoodsdriver,g.username AS takegoodsdrivername,a.offerid,a.actualmoneystatus
                                from tb_b_shippingnoteinfo a
                                left join (
	                                select c.username,c.carriername,b.*,e.name vehiclelengthrequirementname from tb_b_sourcegoodsinfo_offer b
	                                left join tb_b_user c on b.shipperid=c.userid
                                    left join tb_b_dictionary_detail e on b.vehiclelengthrequirement=e.bm
                                ) d on a.offerid=d.offerid
                                left join tb_b_user e on a.operatorid=e.userid
left join tb_b_user f on a.carrierid=f.userid
left join (
    select h1.shippingnotenumber,h1.driverid,h2.username from tb_b_joborderinfo h1
    left join tb_b_user h2 on h1.driverid=h2.userid
) h on a.shippingnotenumber=h.shippingnotenumber
left join tb_b_user g on a.takegoodsdriver=g.userid
                                where a.isdeleteflag=0 ";
                str += where;
                str += " order by a.shippingnoteadddatetime desc";
                #endregion
                DataTable dt = dbc.ExecuteDataTable(str);

                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(); //工作簿
                Aspose.Cells.Worksheet sheet = workbook.Worksheets[0]; //工作表
                Aspose.Cells.Cells cells = sheet.Cells;//单元格

                #region 样式
                //样式1
                Aspose.Cells.Style style1 = workbook.Styles[workbook.Styles.Add()];//新增样式    
                style1.HorizontalAlignment = TextAlignmentType.Center;//文字居中
                style1.Font.Name = "宋体";//文字字体
                style1.Font.Size = 12;//文字大小
                style1.IsTextWrapped = true;//单元格内容自动换行
                style1.Font.IsBold = true;//粗体
                style1.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                style1.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style1.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style1.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                //样式2
                Aspose.Cells.Style style2 = workbook.Styles[workbook.Styles.Add()];//新增样式    
                style2.HorizontalAlignment = TextAlignmentType.Left;//文字居左
                style2.Font.Name = "宋体";//文字字体
                style2.Font.Size = 12;//文字大小
                style2.IsTextWrapped = true;//单元格内容自动换行
                style2.Font.IsBold = false;//粗体
                style2.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                //样式3
                Aspose.Cells.Style style3 = workbook.Styles[workbook.Styles.Add()];//新增样式    
                style3.HorizontalAlignment = TextAlignmentType.Center;//文字居左
                style3.Font.Name = "宋体";//文字字体
                style3.Font.Size = 20;//文字大小
                style3.IsTextWrapped = true;//单元格内容自动换行
                style3.Font.IsBold = true;//粗体


                //样式4
                Aspose.Cells.Style style4 = workbook.Styles[workbook.Styles.Add()];//新增样式    
                style4.HorizontalAlignment = TextAlignmentType.Left;//文字居左
                style4.Font.Name = "宋体";//文字字体
                style4.Font.Size = 10;//文字大小
                style4.Font.Color = Color.Red;//文字大小

                style4.IsTextWrapped = true;//单元格内容自动换行
                style4.Font.IsBold = false;//粗体
                #endregion
                /*"厂家","供应商","装车司机","订单时间","订单状态","订单号","起始地","目的地","收货地址","收货方",
                 * "收货联系人","收货联系方式","货物","数量","重量","体积","车型","车长","是否提货","是否送货",
                 * "预付运费","实际运费","企业实际运费","备注"*/
                /*username,'carriername','takegoodsdrivername',shippingnoteadddatetime,shippingnotestatusname,shippingnotenumber,goodsfromroute,goodstoroute,goodsreceiptplace,consignee,
                 * consicontactname,consitelephonenumber,descriptionofgoods,totalnumberofpackages,itemgrossweight,cube,vehicletyperequirement,vehiclelengthrequirementname,istakegoodsname,isdelivergoodsname,
                 * totalmonetaryamount,actualmoney,"actualcompanypay",memo*/
                String[] ColumnName = { "厂家", "供应商", "装车司机", "订单时间", "订单状态", "订单号", "起始地", "目的地", "收货地址", "收货方", "收货联系人", "收货联系方式", "货物", "数量", "重量", "体积", "车型", "车长", "是否提货", "是否送货", "预付运费", "实际运费", "企业实际运费", "备注" };
                String[] ColumnV = { "username", "carriername", "takegoodsdrivername", "shippingnoteadddatetime", "shippingnotestatusname", "shippingnotenumber", "goodsfromroute", "goodstoroute", "goodsreceiptplace", "consignee", "consicontactname", "consitelephonenumber", "descriptionofgoods", "totalnumberofpackages", "itemgrossweight", "cube", "vehicletyperequirement", "vehiclelengthrequirementname", "istakegoodsname", "isdelivergoodsname", "totalmonetaryamount", "actualmoney", "actualcompanypay", "memo" };
                for (int i = 0; i < ColumnName.Length; i++)
                {
                    cells.SetColumnWidth(i, 30);
                    cells[0, i].PutValue(ColumnName[i]);
                    cells[0, i].SetStyle(style1);
                }

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int a = 0; a < ColumnV.Length; a++)
                        {
                            cells[i + 1, a].PutValue(dt.Rows[i][ColumnV[a]].ToString());
                            cells[i + 1, a].SetStyle(style2);
                        }
                    }
                }

                System.IO.MemoryStream ms = workbook.SaveToStream();
                byte[] bt = ms.ToArray();
                return bt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    #endregion

    /// <summary>
    /// 更新订单日志
    /// </summary>
    /// <param name="dbc"></param>
    /// <param name="shippingnoteid"></param>
    /// <param name="recordtype"></param>
    /// <param name="recordmemo"></param>
    /// <param name="ti"></param>
    public void LogByShippingnote(MySqlDbConnection dbc, string shippingnoteid, string recordtype, string recordmemo, DateTime ti)
    {
        var dt = dbc.GetEmptyDataTable("tb_b_shippingnoteinfo_record");
        var dr = dt.NewRow();
        dr["id"] = Guid.NewGuid();
        dr["shippingnoteid"] = shippingnoteid;
        dr["recordtype"] = recordtype;
        dr["recordmemo"] = recordmemo;
        dr["status"] = 0;
        dr["adduser"] = SystemUser.CurrentUser.UserID;
        dr["addtime"] = ti;
        dr["updateuser"] = SystemUser.CurrentUser.UserID;
        dr["updatetime"] = ti;
        dt.Rows.Add(dr);
        dbc.InsertTable(dt);
    }

    public static string ToJson(DataTable dt)
    {
        if (dt.Rows.Count == 0)
        {
            return "[]";
        }

        StringBuilder jsonBuilder = new StringBuilder();
        // jsonBuilder.Append("{"); 
        //jsonBuilder.Append(dt.TableName.ToString());  
        jsonBuilder.Append("[");//转换成多个model的形式
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            jsonBuilder.Append("{");
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                jsonBuilder.Append("\"");
                jsonBuilder.Append(dt.Columns[j].ColumnName);
                jsonBuilder.Append("\":\"");
                jsonBuilder.Append(dt.Rows[i][j].ToString().Replace("\\", "\\\\").Replace("\t", " ").Replace("\r", " ").Replace("\n", "<br/>").Replace("\"", "'"));
                jsonBuilder.Append("\",");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("},");
        }
        jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
        jsonBuilder.Append("]");
        //  jsonBuilder.Append("}");
        return jsonBuilder.ToString();
    }

    public static string CreateReturnJson(bool success, int code, string data)
    {
        string str = "{\"success\":\"" + success + "\"";
        str += ",\"code\":\"" + code + "\"";
        str += ",\"data\":" + data;
        str += "}";
        return str;
    }


}

public class ReceiptFJ
{
    public string size;
    public string mc;
    public string id;
    public string type;
    public string url;
}