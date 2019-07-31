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

/// <summary>
///JsMag 的摘要说明
/// </summary>
[CSClass("DriverMag")]
public class DriverMag
{
    public DriverMag()
    {
        //
        //TODO: 在此处添加构造函数逻辑
        //
    }

    [CSMethod("getYGLSJList")]
    public object getYGLSJList(int pagnum, int pagesize, string userid)
    {
        using (var db = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string str = @"select  b.id,b.driverid,b.driverxm,b.linkedunit,b.carnumber,b.drivermemo,b.mirrornumber  from tb_b_zx_driver a left join tb_b_car b on a.driverid=b.id
                    where a.userid=@userid order by a.addtime desc";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.Add("@userid", userid);
                DataTable dt = db.GetPagedDataTable(cmd, pagesize, ref cp, out ac);

                return new { dt = dt, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("DelGLByDUID")]
    public object DelGLByDUID(string duserid, string userid)
    {
        using (var db = new DBConnection())
        {
            db.BeginTransaction();
            try
            {
                string str = "delete from tb_b_zx_driver where userid=@userid and driverid=@driverid";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.Add("@userid", userid);
                cmd.Parameters.Add("@driverid", duserid);
                db.ExecuteNonQuery(cmd);

                db.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                db.RoolbackTransaction();
                throw ex;
            }
        }
    }

    [CSMethod("getGLSJList")]
    public object getGLSJList(int pagnum, int pagesize, string userid,string cph)
    {
        using (var db = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(cph))
                {
                    where += " and " + db.C_Like("carnumber", cph, SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
                }
                string str = @"select id,driverxm,drivername,linkedunit,carnumber,drivermemo,mirrornumber from
                tb_b_car where status=0  and id not in (select driverid from tb_b_zx_driver where userid=@userid) " + where + @"
                              order by drivername";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.Add("@userid", userid);
                DataTable dt = db.GetPagedDataTable(cmd, pagesize, ref cp, out ac);

                return new { dt = dt, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }


    [CSMethod("GLSJMX")]
    public object GLSJMX(string duserid, string userid)
    {
        using (var db = new DBConnection())
        {
            db.BeginTransaction();
            try
            {
                string str = "select * from tb_b_zx_driver where userid=@userid and driverid=@driverid";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.Add("@userid", userid);
                cmd.Parameters.Add("@driverid", duserid);
                DataTable pdt = db.ExecuteDataTable(cmd);

                if (pdt.Rows.Count > 0)
                {
                    throw new Exception("关联已存在！");
                }
                else
                {
                    DataTable dt = db.GetEmptyDataTable("tb_b_zx_driver");
                    DataRow dr = dt.NewRow();
                    dr["id"]=Guid.NewGuid();
                    dr["userid"] =userid;
                    dr["driverid"]= duserid;
                    dr["addtime"]=DateTime.Now;
                    dr["adduser"] = SystemUser.CurrentUser.UserID;
                    dt.Rows.Add(dr);
                    db.InsertTable(dt);
                }
                db.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                db.RoolbackTransaction();
                throw ex;
            }
        }
    }
}
