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
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using Aspose.Cells;
using System.Web.Script.Serialization;
using System.Net;
/// <summary>
///ZXSHMag 的摘要说明
/// </summary>

[CSClass("ZXSHMag")]
public class ZXSHMag
{
    string ServiceURL=System.Configuration.ConfigurationManager.AppSettings["ServiceURL"].ToString();

    [CSMethod("GetZXList")]
    public object GetZXList(int pagnum, int pagesize, string yhm, string xm,string isrelease)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";

                if (!string.IsNullOrEmpty(yhm.Trim()))
                {
                    where += " and " + dbc.C_Like("a.UserName", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(xm.Trim()))
                {
                    where += " and " + dbc.C_Like("a.UserXM", xm.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(isrelease.Trim()))
                {
                    where += " and " + dbc.C_EQ("a.IsCanRelease", Convert.ToInt32(isrelease));
                }

                string str = @"select a.*,b.fqcs from [tb_b_user] a 
  left join (select count(SaleRecordID) as fqcs,SaleRecordUserID from tb_b_salerecord where status=0 and SaleRecordLX!=0 group by SaleRecordUserID) b
  on a.UserID=b.SaleRecordUserID
  where a.IsSHPass=1 and a.ClientKind=1 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.AddTime desc,a.UserName,a.UserXM", pagesize, ref cp, out ac);

                dtPage.Columns.Add("dqS");
                for (int i = 0; i < dtPage.Rows.Count; i++)
                {
                    if (dtPage.Rows[i]["DqBm"] != null && dtPage.Rows[i]["DqBm"].ToString() != "")
                    {
                        string sql = "select dq_bm from tb_b_dq where dq_bm=(select dq_sj from tb_b_dq where dq_sj<>'000000' and dq_bm=" + dbc.ToSqlValue(dtPage.Rows[i]["DqBm"]) + ")";
                        DataTable dt = dbc.ExecuteDataTable(sql);
                        if (dt.Rows.Count > 0)
                        {
                            dtPage.Rows[i]["dqS"] = dt.Rows[0][0];
                        }
                    }
                }
                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    [CSMethod("ZXFQ")]
    public object ZXFQ(string userId, int iscanrealese)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                var dt = dbc.GetEmptyDataTable("tb_b_user");
                var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                var sr = dt.NewRow();
                sr["UserID"] = new Guid(userId);
                sr["IsCanRelease"] = iscanrealese;
                sr["canReleaseTime"] = DateTime.Now;
                dt.Rows.Add(sr);
                dbc.UpdateTable(dt, dtt);
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


    [CSMethod("GetZXList2")]
    public object GetZXList2(int pagnum, int pagesize, string yhm, string xm, string ispass)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {

                string _url = ServiceURL+"tbbuserapply.selectApply";
                string jsonParam = new JavaScriptSerializer().Serialize(new
                {
                    tradeCode="tbbuserapply.selectApply",
                    status = ispass,
                    userid="",
                    username=yhm,
                    userxm=xm,
                    currentPage=pagnum,
                    pageSize = 10
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


    [CSMethod("SHJJCG")]
    public object SHJJCG(string userId)
    {
        string _url = ServiceURL + "tbbuserapply.pass";
            string jsonParam = new JavaScriptSerializer().Serialize(new
            {
                tradeCode = "tbbuserapply.pass",
                id = userId
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
    [CSMethod("SHJJSB")]
    public object SHJJSB(string userId,string yj)
    {
        string _url = ServiceURL + "tbbuserapply.reject";
        string jsonParam = new JavaScriptSerializer().Serialize(new
        {
            tradeCode="tbbuserapply.pass",
	        id=userId,
            reviewreason = yj
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

    [CSMethod("getKFCSList")]
    public object getKFCSList(int pagnum, int pagesize, string userid)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;
                string str = @"select * from  tb_b_salerecord where SaleRecordLX!=0 and status=0 and SaleRecordUserID=@SaleRecordUserID order by AddTime desc";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.Add("@SaleRecordUserID", userid);
                //开始取分页数据
                System.Data.DataTable dtPage = dbc.GetPagedDataTable(cmd, pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    [CSMethod("getKFCSListToFile")]
    public object getKFCSListToFile(int pagnum, int pagesize, string userid)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;
                string str = @"select * from  tb_b_salerecord where SaleRecordLX!=0 and status=0 and SaleRecordUserID=@SaleRecordUserID order by AddTime desc";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.Add("@SaleRecordUserID", userid);
                //开始取分页数据
                System.Data.DataTable dtPage = dbc.GetPagedDataTable(cmd, pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    #region  专线发布运费券审核功能
    [CSMethod("getZFQList")]
    public object getZFQList(int pagnum, int pagesize, string userxm, string isVerifyType)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;
                string where="";
                if(!string.IsNullOrEmpty(userxm)){
                    where+=" and "+dbc.C_Like("SaleRecordUserXM",userxm,LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(isVerifyType))
                {
                    where += " and " + dbc.C_EQ("SaleRecordVerifyType", Convert.ToInt32(isVerifyType));
                }
                string str = "select * from  tb_b_salerecord where status=0 and SaleRecordLX!=0  and SaleRecordVerifyType!=3 " + where + " order by addtime desc";
                System.Data.DataTable dtPage = dbc.GetPagedDataTable(str, pagesize, ref cp, out ac);
                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("ZFQSH")]
    public object ZFQSH(string SaleRecordID, int issh)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                var time = DateTime.Now;
                var userid = "";
                string str = "select * from tb_b_salerecord where (SaleRecordVerifyType=0 or SaleRecordVerifyType is null) and SaleRecordID=" + dbc.ToSqlValue(SaleRecordID);
                DataTable sdt = dbc.ExecuteDataTable(str);

                if (sdt.Rows.Count > 0)
                {
                    var dt = dbc.GetEmptyDataTable("tb_b_salerecord");
                    var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                    var sr = dt.NewRow();
                    sr["SaleRecordID"] = new Guid(SaleRecordID);
                    sr["SaleRecordVerifyType"] = issh;
                    sr["SaleRecordVerifyTime"] = time;
                    dt.Rows.Add(sr);
                    dbc.UpdateTable(dt, dtt);


                    string str1 = "select * from tb_b_plattosale where (SaleRecordVerifyType=0 or SaleRecordVerifyType is null) and SaleRecordID=" + dbc.ToSqlValue(SaleRecordID);
                    DataTable sdt1 = dbc.ExecuteDataTable(str1);

                    if (sdt1.Rows.Count > 0)
                    {
                        var dt1 = dbc.GetEmptyDataTable("tb_b_plattosale");
                        var dtt1 = new SmartFramework4v2.Data.DataTableTracker(dt1);
                        var sr1 = dt1.NewRow();
                        sr1["PlatToSaleId"] = sdt1.Rows[0]["PlatToSaleId"];
                        sr1["SaleRecordVerifyType"] = issh;
                        sr1["SaleRecordVerifyTime"] = time;
                        dt1.Rows.Add(sr1);
                        dbc.UpdateTable(dt1, dtt1);

                        userid=sdt1.Rows[0]["UserID"].ToString();
                       
                    }

                    
                }
                dbc.CommitTransaction();
                try
                {
                    KFGMMag.WebServiceApp(System.Configuration.ConfigurationManager.AppSettings["ServiceURL"].ToString(), "pcReleaseNotify", "userid=" + userid);
                }
                catch (Exception ex)
                {
                    return true;
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
    #endregion
}
