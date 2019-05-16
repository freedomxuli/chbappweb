using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartFramework4v2.Web.WebExecutor;
using System.Data;
using SmartFramework4v2.Data.SqlServer;
using System.Text;
using System.Data.SqlClient;
using SmartFramework4v2.Web.Common.JSON;
using SmartFramework4v2.Data;
using Aspose.Cells;
using System.IO;
using System.Web.Script.Serialization;
using System.Net;

/// <summary>
/// YKMag 的摘要说明
/// </summary>
[CSClass("CYMag")]
public class CYMag
{
    public CYMag()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}
    string ServiceURL = System.Configuration.ConfigurationManager.AppSettings["ZYServiceURL"].ToString();
    [CSMethod("GetCYDList")]
    public object GetCYDList(int pagnum, int pagesize, string carriagecode, string UserXM, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(carriagecode.Trim()))
                {
                    where += " and " + dbc.C_Like("a.carriagecode", carriagecode.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(UserXM.Trim()))
                {
                    where += " and " + dbc.C_Like("c.UserXM", UserXM.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.carriagetime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.carriagetime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select a.*,b.UserName as sjzh,b.carnumber as sjcarnumber,b.UserXM as sjxm,b.UserTel as sjdh,c.UserXM as zx
                              from tb_b_carriage a left join tb_b_user b on a.driverid=b.UserID
                            left join tb_b_user c on a.userid=c.UserID where 1=1 
                                 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.carriagetime desc", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("QR")]
    public object QR(string carriageid, int carriagestatus)
    {
       using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                if (carriagestatus == 10)
                {
                    DataTable odt = dbc.GetEmptyDataTable("tb_b_carriage");
                    DataTableTracker odtt = new DataTableTracker(odt);
                    DataRow odr = odt.NewRow();
                    odr["carriageid"] = carriageid;
                    odr["carriagestatus"] = 20;
                    odt.Rows.Add(odr);
                    dbc.UpdateTable(odt, odtt);

                    DataTable ofdt = dbc.GetEmptyDataTable("tb_b_carriage_flow");
                    DataRow ofdr = ofdt.NewRow();
                    ofdr["carriageflowid"] = Guid.NewGuid().ToString();
                    ofdr["carriageid"] = carriageid;
                    ofdr["carriagestatus"] = 20;
                    ofdr["status"] = 0;
                    ofdr["adduser"] = SystemUser.CurrentUser.UserID;
                    ofdr["addtime"] = DateTime.Now;
                    ofdt.Rows.Add(ofdr);
                    dbc.InsertTable(ofdt);
                }

                dbc.CommitTransaction();

                string str = "select * from tb_b_carriage where status=0 and carriageid=" + dbc.ToSqlValue(carriageid);
                DataTable dt = dbc.ExecuteDataTable(str);

                if (dt.Rows.Count > 0)
                {
                    var request = (HttpWebRequest)WebRequest.Create(ServiceURL + "sendSms/pass/tozx");
                    request.Method = "POST";
                    request.ContentType = "application/json;charset=UTF-8";
                    var byteData = Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(new
                    {
                        id = dt.Rows[0]["userid"]
                    }));
                    var length = byteData.Length;
                    request.ContentLength = length;
                    var writer = request.GetRequestStream();
                    writer.Write(byteData, 0, length);
                    writer.Close();

                    var request1 = (HttpWebRequest)WebRequest.Create(ServiceURL + "sendSms/pass/todriver");
                    request1.Method = "POST";
                    request1.ContentType = "application/json;charset=UTF-8";
                    var byteData1 = Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(new
                    {
                        carriageid = carriageid
                    }));
                    var length1 = byteData1.Length;
                    request1.ContentLength = length;
                    var writer1 = request1.GetRequestStream();
                    writer1.Write(byteData1, 0, length);
                    writer1.Close();
                }

                

                return true;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }

    [CSMethod("JJ")]
    public object JJ(string carriageid, int carriagestatus,string thyj)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                if (carriagestatus == 10 || carriagestatus == 11)
                {
                    DataTable odt = dbc.GetEmptyDataTable("tb_b_carriage");
                    DataTableTracker odtt = new DataTableTracker(odt);
                    DataRow odr = odt.NewRow();
                    odr["carriageid"] = carriageid;
                    odr["carriagestatus"] = 21;
                    odt.Rows.Add(odr);
                    dbc.UpdateTable(odt, odtt);

                    DataTable ofdt = dbc.GetEmptyDataTable("tb_b_carriage_flow");
                    DataRow ofdr = ofdt.NewRow();
                    ofdr["carriageflowid"] = Guid.NewGuid().ToString();
                    ofdr["carriageid"] = carriageid;
                    ofdr["carriagestatus"] = 21;
                    ofdr["status"] = 0;
                    ofdr["adduser"] = SystemUser.CurrentUser.UserID;
                    ofdr["addtime"] = DateTime.Now;
                    ofdr["memo"] = thyj;
                    ofdt.Rows.Add(ofdr);
                    dbc.InsertTable(ofdt);
                }

                dbc.CommitTransaction();

                string str = "select * from tb_b_carriage where status=0 and carriageid=" + dbc.ToSqlValue(carriageid);
                DataTable dt = dbc.ExecuteDataTable(str);

                if (dt.Rows.Count > 0)
                {
                    var request = (HttpWebRequest)WebRequest.Create(ServiceURL + "sendSms/reject/tozx");
                    request.Method = "POST";
                    request.ContentType = "application/json;charset=UTF-8";
                    var byteData = Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(new
                    {
                        id = dt.Rows[0]["userid"]
                    }));
                    var length = byteData.Length;
                    request.ContentLength = length;
                    var writer = request.GetRequestStream();
                    writer.Write(byteData, 0, length);
                    writer.Close();

                    var request1 = (HttpWebRequest)WebRequest.Create(ServiceURL + "sendSms/reject/todriver");
                    request1.Method = "POST";
                    request1.ContentType = "application/json;charset=UTF-8";
                    var byteData1 = Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(new
                    {
                        carriageid = carriageid
                    }));
                    var length1 = byteData1.Length;
                    request1.ContentLength = length;
                    var writer1 = request1.GetRequestStream();
                    writer1.Write(byteData1, 0, length);
                    writer1.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }
     
    [CSMethod("TH")]
    public object TH(string carriageid, int carriagestatus)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                if (carriagestatus == 10 || carriagestatus == 11)
                {
                    DataTable odt = dbc.GetEmptyDataTable("tb_b_carriage");
                    DataTableTracker odtt = new DataTableTracker(odt);
                    DataRow odr = odt.NewRow();
                    odr["carriageid"] = carriageid;
                    odr["carriagestatus"] = 0;
                    odt.Rows.Add(odr);
                    dbc.UpdateTable(odt, odtt);

                    DataTable ofdt = dbc.GetEmptyDataTable("tb_b_carriage_flow");
                    DataRow ofdr = ofdt.NewRow();
                    ofdr["carriageflowid"] = Guid.NewGuid().ToString();
                    ofdr["carriageid"] = carriageid;
                    ofdr["carriagestatus"] = 0;
                    ofdr["status"] = 0;
                    ofdr["adduser"] = SystemUser.CurrentUser.UserID;
                    ofdr["addtime"] = DateTime.Now;
                    ofdt.Rows.Add(ofdr);
                    dbc.InsertTable(ofdt);
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

    [CSMethod("WC")]
    public object WC(string carriageid, int carriagestatus)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                string str = "select * from tb_b_carriage where status=0 and carriageid=" + dbc.ToSqlValue(carriageid);
                DataTable dt = dbc.ExecuteDataTable(str);
                if (dt.Rows.Count > 0)
                {
                    if (Convert.ToInt32(dt.Rows[0]["carriagestatus"]) == 50 && Convert.ToInt32(dt.Rows[0]["isoilpay"]) == 1 && Convert.ToInt32(dt.Rows[0]["ismoneypay"]) == 1)
                    {
                        DataTable odt = dbc.GetEmptyDataTable("tb_b_carriage");
                        DataTableTracker odtt = new DataTableTracker(odt);
                        DataRow odr = odt.NewRow();
                        odr["carriageid"] = carriageid;
                        odr["carriagestatus"] = 90;
                        odt.Rows.Add(odr);
                        dbc.UpdateTable(odt, odtt);

                        DataTable ofdt = dbc.GetEmptyDataTable("tb_b_carriage_flow");
                        DataRow ofdr = ofdt.NewRow();
                        ofdr["carriageflowid"] = Guid.NewGuid().ToString();
                        ofdr["carriageid"] = carriageid;
                        ofdr["carriagestatus"] = 90;
                        ofdr["status"] = 0;
                        ofdr["adduser"] = SystemUser.CurrentUser.UserID;
                        ofdr["addtime"] = DateTime.Now;
                        ofdt.Rows.Add(ofdr);
                        dbc.InsertTable(ofdt);
                    }
                }
                dbc.CommitTransaction();

                var request = (HttpWebRequest)WebRequest.Create(ServiceURL + "sendSms/arrive/todriver");
                request.Method = "POST";
                request.ContentType = "application/json;charset=UTF-8";
                var byteData = Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(new
                {
                    carriageid = carriageid
                }));
                var length = byteData.Length;
                request.ContentLength = length;
                var writer = request.GetRequestStream();
                writer.Write(byteData, 0, length);
                writer.Close();

                return true;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }

    [CSMethod("YKDK")]
    public object YKDK(string carriageid, int carriagestatus)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                string str = "select * from tb_b_carriage where status=0 and carriageid=" + dbc.ToSqlValue(carriageid);
                DataTable dt = dbc.ExecuteDataTable(str);
                if (dt.Rows.Count > 0)
                {
                    if ((Convert.ToInt32(dt.Rows[0]["isoilpay"]) == 0) && ( Convert.ToInt32(dt.Rows[0]["carriagestatus"]) == 30 || Convert.ToInt32(dt.Rows[0]["carriagestatus"]) == 40 || Convert.ToInt32(dt.Rows[0]["carriagestatus"]) == 50))
                    {
                        DataTable odt = dbc.GetEmptyDataTable("tb_b_carriage");
                        DataTableTracker odtt = new DataTableTracker(odt);
                        DataRow odr = odt.NewRow();
                        odr["carriageid"] = carriageid;
                        odr["isoilpay"] = 1;
                        odt.Rows.Add(odr);
                        dbc.UpdateTable(odt, odtt);

                        DataTable pfdt = dbc.GetEmptyDataTable("tb_b_carriage_pay");
                        DataRow pfdr = pfdt.NewRow();
                        pfdr["carriagepayid"] = Guid.NewGuid().ToString();
                        pfdr["carriagepaytype"] = 0;
                        pfdr["adduser"] = SystemUser.CurrentUser.UserID;
                        pfdr["addtime"] = DateTime.Now;
                        pfdr["carriageid"] = carriageid;
                        pfdt.Rows.Add(pfdr);
                        dbc.InsertTable(pfdt);
                    }
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

    [CSMethod("XJDK")]
    public object XJDK(string carriageid, int carriagestatus)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                string str = "select * from tb_b_carriage where status=0 and carriageid=" + dbc.ToSqlValue(carriageid);
                DataTable dt = dbc.ExecuteDataTable(str);
                if (dt.Rows.Count > 0)
                {
                    if ((Convert.ToInt32(dt.Rows[0]["ismoneypay"]) == 0) && ( Convert.ToInt32(dt.Rows[0]["carriagestatus"]) == 30 || Convert.ToInt32(dt.Rows[0]["carriagestatus"]) == 40 || Convert.ToInt32(dt.Rows[0]["carriagestatus"]) == 50))
                    {
                        DataTable odt = dbc.GetEmptyDataTable("tb_b_carriage");
                        DataTableTracker odtt = new DataTableTracker(odt);
                        DataRow odr = odt.NewRow();
                        odr["carriageid"] = carriageid;
                        odr["ismoneypay"] = 1;
                        odt.Rows.Add(odr);
                        dbc.UpdateTable(odt, odtt);

                        DataTable pfdt = dbc.GetEmptyDataTable("tb_b_carriage_pay");
                        DataRow pfdr = pfdt.NewRow();
                        pfdr["carriagepayid"] = Guid.NewGuid().ToString();
                        pfdr["carriagepaytype"] = 1;
                        pfdr["adduser"] = SystemUser.CurrentUser.UserID;
                        pfdr["addtime"] = DateTime.Now;
                        pfdr["carriageid"] = carriageid;
                        pfdt.Rows.Add(pfdr);
                        dbc.InsertTable(pfdt);
                    }
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

    [CSMethod("YSFDK")]
    public object YSFDK(string carriageid, int carriagestatus)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                string str = "select * from tb_b_carriage where status=0 and carriageid=" + dbc.ToSqlValue(carriageid);
                DataTable dt = dbc.ExecuteDataTable(str);
                if (dt.Rows.Count > 0)
                {
                    if ((Convert.ToInt32(dt.Rows[0]["ismoneynewpay"]) == 0) && (Convert.ToInt32(dt.Rows[0]["carriagestatus"]) == 50))
                    {
                        DataTable odt = dbc.GetEmptyDataTable("tb_b_carriage");
                        DataTableTracker odtt = new DataTableTracker(odt);
                        DataRow odr = odt.NewRow();
                        odr["carriageid"] = carriageid;
                        odr["ismoneynewpay"] = 1;
                        odt.Rows.Add(odr);
                        dbc.UpdateTable(odt, odtt);

                        DataTable pfdt = dbc.GetEmptyDataTable("tb_b_carriage_pay");
                        DataRow pfdr = pfdt.NewRow();
                        pfdr["carriagepayid"] = Guid.NewGuid().ToString();
                        pfdr["carriagepaytype"] = 1;
                        pfdr["adduser"] = SystemUser.CurrentUser.UserID;
                        pfdr["addtime"] = DateTime.Now;
                        pfdr["carriageid"] = carriageid;
                        pfdt.Rows.Add(pfdr);
                        dbc.InsertTable(pfdt);
                    }
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

   

    [CSMethod("getInsure")]
    public object getInsure(string carriageid)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {

                string _url = ServiceURL + "getInsure";
                string jsonParam = new JavaScriptSerializer().Serialize(new
                {
                    carriageid = carriageid
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

                return responseString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}

