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

/// <summary>
/// YKMag 的摘要说明
/// </summary>
[CSClass("YKMag")]
public class YKMag
{
    public YKMag()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}

    [CSMethod("GetVisionList")]
    public object GetVisionList()
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string str = "select id,carriagepayratio,carriageoil from tb_b_version";
                DataTable dt = dbc.ExecuteDataTable(str);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetVisionByID")]
    public object GetVisionByID(string id)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string str = "select carriagepayratio,carriageoil from tb_b_version where id=@id";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.Add("@id", id);
                DataTable dt = dbc.ExecuteDataTable(cmd);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("SaveVision")]
    public bool SaveVision(JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                var id = jsr["id"].ToString();
                var dt = dbc.GetEmptyDataTable("tb_b_version");
                var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                var dr = dt.NewRow();
                dr["id"] = id;
                dr["carriagepayratio"] = Convert.ToDecimal(jsr["carriagepayratio"].ToString());
                if (jsr["carriageoil"] != null && jsr["carriageoil"].ToString() != "")
                {
                    int carriageoil = Convert.ToInt32(jsr["XZCarriageoil"].ToString()) + Convert.ToInt32(jsr["carriageoil"].ToString());
                    dr["carriageoil"] = carriageoil;
                }
                else
                {
                    dr["carriageoil"] = Convert.ToInt32(jsr["XZCarriageoil"].ToString());
                }
                dt.Rows.Add(dr);
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


    [CSMethod("GetYKDDList")]
    public object GetYKDDList(int pagnum, int pagesize, string cardNo,string orderId,string zt, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(cardNo.Trim()))
                {
                    where += " and " + dbc.C_Like("a.cardNo", cardNo.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(orderId.Trim()))
                {
                    where += " and " + dbc.C_Like("a.orderId", orderId.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(zt.Trim()))
                {
                    where += " and " + dbc.C_EQ("a.status", zt.Trim());
                }
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.addtime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.addtime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @" select * from tb_b_oil_order a left join tb_b_user b on a.userid=b.userid
                                 ";
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

    [CSMethod("ADDHB")]
    public object ADDHB(JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                var inuserzh = jsr["inuserzh"].ToString();
                int money= Convert.ToInt32(jsr["money"].ToString());

                string str = "select * from tb_b_user where UserName=" + dbc.ToSqlValue(inuserzh);
                DataTable udt = dbc.ExecuteDataTable(str);

                if (udt.Rows.Count > 0)
                {
                    string str1 = "select id,carriagepayratio,carriageoil from tb_b_version";
                    DataTable vdt = dbc.ExecuteDataTable(str1);

                    int carriageoil = 0;
                    if (vdt.Rows.Count > 0)
                    {
                        if (vdt.Rows[0]["carriageoil"] != null && vdt.Rows[0]["carriageoil"].ToString() != "")
                        {
                            carriageoil = Convert.ToInt32(vdt.Rows[0]["carriageoil"]);
                        }
                    }

                    if (carriageoil <= Convert.ToInt32(money))
                    {
                        throw new Exception("划拨总油量不得超过其所有运费券！");
                    }
                    else
                    {
                        var oilcardcode = "CHB_" + GetStrAscii(6) + DateTime.Now.ToString("yyyyMMddHHmmssffff");
                        var oiltransfercode = GetStrAscii(6) + DateTime.Now.ToString("yyyyMMddHHmmssffff");

                        DataTable otdt = dbc.GetEmptyDataTable("tb_b_oil_transfer");
                        DataRow otdr = otdt.NewRow();
                        otdr["oiltransferid"]=Guid.NewGuid();
                        otdr["outuserid"]="6E72B59D-BEC6-4835-A66F-8BC70BD82FE9";
                        otdr["inuserid"]=udt.Rows[0]["UserID"];
                        otdr["money"]=money;
                        otdr["oilcardcode"]=oilcardcode;
                        otdr["oiltransfercode"]=oiltransfercode;
                        otdr["status"]=0;
                        otdr["addtime"]=DateTime.Now;
                        otdr["updatetime"]= DateTime.Now;
                        otdt.Rows.Add(otdr);
                        dbc.InsertTable(otdt);

                        DataTable modt = dbc.GetEmptyDataTable("tb_b_myoilcard");
                        DataRow modr = modt.NewRow();
                        modr["myoilcardId"]=Guid.NewGuid();
                        modr["oilmoney"]=money;
                        modr["UserID"]=udt.Rows[0]["UserID"];
                        modr["oilcardcode"] = oilcardcode;
                        modr["oiltransfercode"]=oiltransfercode;
                        modr["status"]=0;
                        modr["addtime"]=DateTime.Now;
                        modr["updatetime"]= DateTime.Now;
                        modt.Rows.Add(modr);
                        dbc.InsertTable(modt);

                        DataTable vndt = dbc.GetEmptyDataTable("tb_b_version");
                        DataTableTracker vndtt=new DataTableTracker(vndt); 
                        DataRow vndr = vndt.NewRow();
                        vndr["id"] = vdt.Rows[0]["id"];
                        vndr["carriageoil"] = carriageoil - money;
                        vndt.Rows.Add(vndr);
                        dbc.UpdateTable(vndt, vndtt);

                    }
                }
                else
                {
                    throw new Exception("转入人员不存在，请重新填写手机号！");
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

    private string GetStrAscii(int nLength)
    {
        int nStrLength = nLength;
        string strString = "1234567890";
        //string  strString   = "1234567890qwertyuioplkjhgfdsazxcvbnmASDFGHJKLMNBVCXZQWERTYUIOP";
        StringBuilder strtemp = new StringBuilder();
        Random random = new Random((int)DateTime.Now.Ticks);
        for (int i = 0; i < nStrLength; i++)
        {
            random = new Random(unchecked(random.Next() * 1000));
            strtemp.Append(strString[random.Next(10)]);
        }
        return strtemp.ToString();
    }

    [CSMethod("GetYKHBList")]
    public object GetYKHBList(int pagnum, int pagesize, string oilcardcode, string oiltransfercode, string yhzh, string zt, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(oilcardcode.Trim()))
                {
                    where += " and " + dbc.C_Like("a.oilcardcode", oilcardcode.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(oiltransfercode.Trim()))
                {
                    where += " and " + dbc.C_Like("a.oiltransfercode", oiltransfercode.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(yhzh.Trim()))
                {
                    where += " and " + dbc.C_EQ("a.inuserid", zt.Trim());
                }

                if (!string.IsNullOrEmpty(zt.Trim()))
                {
                    if (Convert.ToInt32(zt) == 0)
                    {//是
                        where += " and " + dbc.C_EQ("a.outuserid", "6E72B59D-BEC6-4835-A66F-8BC70BD82FE9");
                    }else if(Convert.ToInt32(zt) ==1){
                        where += " and " + dbc.C_NEQ("a.outuserid", "6E72B59D-BEC6-4835-A66F-8BC70BD82FE9");
                    }
                }
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.addtime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.addtime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @" select * from (select a.addtime,a.oiltransfercode, a.oilcardcode,a.outuserid,a.money,'查货宝' as zcxm,'' as zczh,a.inuserid, b.UserXM as zrxm,b.UserName as zrzh 
  from tb_b_oil_transfer a left join tb_b_user b on a.inuserid=b.userid 
  where outuserid='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9' and status=0
  union all
  select a.addtime,a.oiltransfercode,a.oilcardcode,a.outuserid,a.money,c.UserXM as zcxm,c.UserName as zczh,a.inuserid, b.UserXM as zrxm,b.UserName as zrzh 
  from tb_b_oil_transfer a left join tb_b_user b on a.inuserid=b.userid 
  left join tb_b_user c on a.outuserid=c.UserID
  where outuserid<>'6E72B59D-BEC6-4835-A66F-8BC70BD82FE9' and status=0) a where 1=1
                                 ";
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

}