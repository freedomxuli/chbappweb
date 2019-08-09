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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

/// <summary>
/// XJMag 的摘要说明
/// </summary>
[CSClass("XJMag")]
public class XJMag
{
    public XJMag()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    [CSMethod("GetList")]
    public object GetList(int pagnum, int pagesize, string yhm)
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
                    where += " and " + dbc.C_Like("b.UserXM", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                string str = @"  select a.*,b.UserName,b.UserXM,case when a.points>0 then a.validHour-(DATEDIFF(HOUR,a.SaleRecordTime,getDate())) else null end as xssy
                    from tb_b_plattosale a left join tb_b_user b on a.UserID=b.UserID
                        where a.status=0 and a.pointkind=0  ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by case when a.points>0 then a.validHour-(DATEDIFF(HOUR,a.SaleRecordTime,getDate())) else 999 end asc,a.points desc", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("SaveXJ")]
    public bool SaveXJ(JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                dbc.BeginTransaction();

                var PlatToSaleId = jsr["PlatToSaleId"].ToString();
                var points = jsr["points"].ToString();
                var memo = jsr["memo"].ToString();

                string sql = "select points,UserID from tb_b_plattosale where PlatToSaleId = @PlatToSaleId";
                SqlCommand cmd = dbc.CreateCommand(sql);
                cmd.Parameters.Add("@PlatToSaleId", PlatToSaleId);
                DataTable dt = dbc.ExecuteDataTable(cmd);

                if (Convert.ToDecimal(dt.Rows[0]["points"]) != Convert.ToDecimal(points))
                {
                    throw new Exception("需下架所有券！");
                }
                else
                {
                    sql = "update tb_b_plattosale set points = @points where PlatToSaleId = @PlatToSaleId";
                    cmd = dbc.CreateCommand(sql);
                    cmd.Parameters.Add("@points", Convert.ToDecimal(Convert.ToDecimal(dt.Rows[0]["points"]) - Convert.ToDecimal(points)));
                    cmd.Parameters.Add("@PlatToSaleId", PlatToSaleId);
                    dbc.ExecuteNonQuery(cmd);

                    sql = "select Points,PlatPointId from tb_b_platpoints where UserID = @UserID and status = 0";
                    cmd = dbc.CreateCommand(sql);
                    cmd.Parameters.Add("@UserID", dt.Rows[0]["UserID"].ToString());
                    DataTable dt_plat = dbc.ExecuteDataTable(cmd);

                    sql = "update tb_b_platpoints set points = @points where PlatPointId = @PlatPointId";
                    cmd = dbc.CreateCommand(sql);
                    cmd.Parameters.Add("@points", Convert.ToDecimal(Convert.ToDecimal(dt_plat.Rows[0]["Points"]) + Convert.ToDecimal(points)));
                    cmd.Parameters.Add("@PlatPointId", dt_plat.Rows[0]["PlatPointId"].ToString());
                    dbc.ExecuteNonQuery(cmd);

                    DataTable dt_xj = dbc.GetEmptyDataTable("tb_b_xj");
                    DataRow dr = dt_xj.NewRow();
                    dr["XJ_ID"] = Guid.NewGuid();
                    dr["UserID"] = dt.Rows[0]["UserID"].ToString();
                    dr["Points"] = points;
                    dr["status"] = 0;
                    dr["adduser"] = SystemUser.CurrentUser.UserID;
                    dr["addtime"] = DateTime.Now;
                    dr["updateuser"] = SystemUser.CurrentUser.UserID;
                    dr["updatetime"] = DateTime.Now;
                    dr["Memo"] = memo;
                    dt_xj.Rows.Add(dr);
                    dbc.InsertTable(dt_xj);

                    dbc.CommitTransaction();
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

    [CSMethod("getHisSale")]
    public DataTable getHisSale(string UserID)
    {
        using (DBConnection dbc = new DBConnection())
        {
            string sql = @"select isNULL(a.points,0)+isnull(b.points,0) as points,a.UserID,a.PlatToSaleId,a.discountmemo, c.UserXM from 
(select Points as points,UserID,PlatToSaleId,discountmemo from tb_b_plattosale where  UserID=@UserID and status=0 and pointkind=0 ) a left join (
select sum(Points) as points,SaleUserID from [tb_b_order] where  [SaleUserID]=@UserID and ZhiFuZT=0 and status=0
group by SaleUserID) b  on a.UserID=b.SaleUserID 
 left join tb_b_user c on a.UserID = c.UserID";
            SqlCommand cmd = dbc.CreateCommand(sql);
            cmd.Parameters.Add("@UserID", UserID);
            DataTable dt = dbc.ExecuteDataTable(cmd);
            return dt;
        }
    }


    #region 运费券清理
    [CSMethod("GetQLList")]
    public object GetQLList(int pagnum, int pagesize, string yhm)
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
                    where += " and " + dbc.C_Like("b.UserXM", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                string str = @" select a.*,b.UserXM from tb_b_plattosale a left join tb_b_user b on a.userid=b.userid
                                where a.status=0 and a.pointkind!=0 and a.points>0 and a.points<500 
                                and a.validHour-(DATEDIFF(HOUR,a.addtime,getDate()))>=0
                                and a.SaleRecordID in (select distinct a.SaleRecordID from (select SaleRecordPoints,SaleRecordID from tb_b_salerecord where status=0) a 
                                left join (select sum(points) as points,SaleRecordID from tb_b_order where status=0 and zhifuZT=1 group by SaleRecordID) b  on a.SaleRecordID=b.SaleRecordID
                                left join (select points,SaleRecordID from tb_b_plattosale where status=0 ) c on a.SaleRecordID=c.SaleRecordID
                                where isnull(a.SaleRecordPoints,0)-(isnull(b.points,0)+isnull(c.points,0))=0)";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.addtime ", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("QLYFQ")]
    public object QLYFQ(string id)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                dbc.BeginTransaction();
                string str = "select * from tb_b_plattosale where status=0 and PlatToSaleId=@PlatToSaleId";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@PlatToSaleId", id);
                DataTable dt = dbc.ExecuteDataTable(cmd);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["points"] != null && dt.Rows[0]["points"].ToString() != "")
                    {
                        if (Convert.ToDecimal(dt.Rows[0]["points"]) == 0 || Convert.ToDecimal(dt.Rows[0]["points"]) >= 500)
                        {
                            throw new Exception("该数据不需要清理！");
                        }
                        else
                        {
                            cmd.Parameters.Clear();
                            str = "select * from tb_b_salerecord where SaleRecordID=@SaleRecordID";
                            cmd = dbc.CreateCommand(str);
                            cmd.Parameters.AddWithValue("@salerecordid", dt.Rows[0]["SaleRecordID"]);
                            DataTable dt1 = dbc.ExecuteDataTable(str);

                            cmd.Parameters.Clear();
                            string sql = "update tb_b_salerecord set SaleRecordPoints=@SaleRecordPoints where SaleRecordID=@SaleRecordID";
                            cmd = dbc.CreateCommand(sql);
                            cmd.Parameters.AddWithValue("@SaleRecordPoints", Convert.ToDecimal(Convert.ToDecimal(dt1.Rows[0]["SaleRecordPoints"]) - Convert.ToDecimal(dt.Rows[0]["points"])));
                            cmd.Parameters.AddWithValue("@SaleRecordID", dt.Rows[0]["SaleRecordID"]);
                            dbc.ExecuteNonQuery(cmd);

                            cmd.Parameters.Clear();
                            sql = "update tb_b_plattosale set points = 0 where PlatToSaleId = @PlatToSaleId";
                            cmd = dbc.CreateCommand(sql);
                            cmd.Parameters.AddWithValue("@PlatToSaleId", id);
                            dbc.ExecuteNonQuery(cmd);
                        }
                    }
                }
                else
                {
                    throw new Exception("该数据不存在！");
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

    #endregion

    #region 过期券回退
    [CSMethod("GetGqqListByPage")]
    public object GetGqqListByPage(int pagnum, int pagesize, JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;
                List<string> wArr = new List<string>();
                if (!string.IsNullOrEmpty(jsr["cx_zxmc"]))
                {
                    wArr.Add(dbc.C_Like("c.UserXM", jsr["cx_zxmc"], LikeStyle.LeftAndRightLike));
                }
                if (!string.IsNullOrEmpty(jsr["cx_fhrzh"]))
                {
                    wArr.Add(dbc.C_Like("b.UserName", jsr["cx_fhrzh"], LikeStyle.LeftAndRightLike));
                }
                string cx_beg = jsr["cx_beg"].ToString();
                if (!string.IsNullOrEmpty(cx_beg))
                {
                    wArr.Add("a.addTimd >= " + dbc.ToSqlValue(Convert.ToDateTime(cx_beg)));
                }

                string cx_endjsr = jsr["cx_end"].ToString();
                if (!string.IsNullOrEmpty(cx_endjsr))
                {
                    wArr.Add("a.addTimd < " + dbc.ToSqlValue(Convert.ToDateTime(cx_endjsr).AddDays(1)));
                }
                string sqlW = "";
                if (wArr.Count > 0)
                {
                    sqlW = " and " + string.Join(" and ", wArr);
                }
                string str = @"select a.*,b.UserName fhrzh,c.UserXM zxmc,
                                case 
                                when d.id IS NULL then 0
                                else 1 end as shzt from(
	                                select t1.*,t2.UserID,t2.CardUserID from tb_b_invalid_points t1
	                                left join tb_b_mycard t2 on t1.mycardId=t2.mycardId
                                )a
                                left join tb_b_user b on a.UserID=b.UserID
                                left join tb_b_user c on a.CardUserID=c.UserID
                                left join tb_b_return_points d on a.invalidId=d.invalidId and d.status=0

                            where a.status=0 and a.addTimd>'2019/6/24'" + sqlW + " order by a.addTimd desc";

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

    [CSMethod("GqqSh")]
    public bool GqqSh(string invalidId, string zxid, decimal points)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                dbc.BeginTransaction();
                //验证过期券是否回退过
                string sql = @"select * from tb_b_return_points where invalidId=" + dbc.ToSqlValue(invalidId);
                DataTable checkDt = dbc.ExecuteDataTable(sql);
                if (checkDt.Rows.Count > 0)
                {
                    return false;
                }
                //插入过期券回退记录
                DataTable returnDt = dbc.GetEmptyDataTable("tb_b_return_points");
                DataRow dr = returnDt.NewRow();
                dr["id"] = Guid.NewGuid().ToString();
                dr["invalidId"] = invalidId;
                dr["addtime"] = DateTime.Now;
                dr["adduser"] = SystemUser.CurrentUser.UserID;
                dr["updatetime"] = DateTime.Now;
                dr["updateuser"] = SystemUser.CurrentUser.UserID;
                dr["status"] = 0;
                returnDt.Rows.Add(dr);
                dbc.InsertTable(returnDt);
                //专线Points回退
                sql = @"update tb_b_platpoints set Points=Points+" + points + " where UserID=" + dbc.ToSqlValue(zxid);
                dbc.ExecuteNonQuery(sql);
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

    #region 退款审核界面
    [CSMethod("GetTkshList")]
    public object GetTkshList(int pagnum, int pagesize, JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                List<string> wArr = new List<string>();
                string cx_beg = jsr["cx_beg"].ToString();
                if (!string.IsNullOrEmpty(cx_beg))
                {
                    wArr.Add("a.addtime >= " + dbc.ToSqlValue(Convert.ToDateTime(cx_beg)));
                }
                string cx_endjsr = jsr["cx_end"].ToString();
                if (!string.IsNullOrEmpty(cx_endjsr))
                {
                    wArr.Add("a.addtime < " + dbc.ToSqlValue(Convert.ToDateTime(cx_endjsr).AddDays(1)));
                }
                if (!string.IsNullOrEmpty(jsr["cx_fhrzh"]))
                {
                    wArr.Add(dbc.C_Like("b.UserName", jsr["cx_fhrzh"], LikeStyle.LeftAndRightLike));
                }
                if (!string.IsNullOrEmpty(jsr["cx_zxmc"]))
                {
                    wArr.Add(dbc.C_Like("c.UserXM", jsr["cx_zxmc"], LikeStyle.LeftAndRightLike));
                }
                if (!string.IsNullOrEmpty(jsr["cx_istk"]))
                {
                    wArr.Add(dbc.C_EQ("a.status", Convert.ToInt32(jsr["cx_istk"].ToString())));
                }
                string sqlW = "";
                if (wArr.Count > 0)
                {
                    sqlW = " and " + string.Join(" and ", wArr);
                }

                string str = @"select a.*,b.UserName fhrmc,c.UserXM zxmc from tb_b_mycard a 
                                left join tb_b_user b on a.UserID=b.UserID
                                left join tb_b_user c on a.CardUserID=c.UserID
                                where a.status in(2,3) and a.OrderCode is not null ";
                str += sqlW;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.status asc,a.addtime desc", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("VerifyPassWorld")]
    public bool VerifyPassWorld(string id, string password)
    {
        using (DBConnection dbc = new DBConnection())
        {
            string ServiceURL = System.Configuration.ConfigurationManager.AppSettings["ServiceURL"].ToString();

            string sqlStr = @"select a.UserID YH_ID, a.UserName YH_DLM,a.UserXM YH_XM,a.Password,b.roleId,b.companyId from tb_b_user a 
                            left join tb_b_user_role b on a.UserID = b.userId 
                            where a.UserID=@UserID and a.Password=@Password ";
            SqlCommand cmd = new SqlCommand(sqlStr);
            cmd.Parameters.AddWithValue("@UserID", SystemUser.CurrentUser.UserID);
            cmd.Parameters.AddWithValue("@Password", password);
            var dtUser = dbc.ExecuteDataTable(cmd);
            if (dtUser.Rows.Count > 0)
            {
                string _url = ServiceURL + "baofooReturn";
                string jsonParam = new JavaScriptSerializer().Serialize(new
                {
                    mycardid = id,
                    userid = SystemUser.CurrentUser.UserID,
                    userxm = SystemUser.CurrentUser.UserName
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



                JObject jo = (JObject)JsonConvert.DeserializeObject(responseString);
                try
                {
                    if (Convert.ToBoolean(jo["success"].ToString()))
                    {
                        //sqlStr = "update tb_b_mycard set status=3 where mycardId=" + dbc.ToSqlValue(id);
                        //dbc.ExecuteNonQuery(sqlStr);
                        return true;
                    }
                    else
                    {
                        throw new Exception("退款失败");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(jo["details"].ToString());
                }

            }
            return false;
        }
    }
    #endregion
}