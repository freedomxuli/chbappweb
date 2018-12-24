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
/// <summary>
///ZXSHMag 的摘要说明
/// </summary>

[CSClass("ZXSHMag")]
public class ZXSHMag
{
   

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
                    where += " and " + dbc.C_Like("UserName", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(xm.Trim()))
                {
                    where += " and " + dbc.C_Like("UserXM", xm.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(isrelease.Trim()))
                {
                    where += " and " + dbc.C_EQ("IsCanRelease", Convert.ToInt32(isrelease));
                }

                string str = @"select * from [tb_b_user] where IsSHPass=1 and ClientKind=1 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by AddTime desc,UserName,UserXM", pagesize, ref cp, out ac);

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


}
