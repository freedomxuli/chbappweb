using SmartFramework4v2.Data.SqlServer;
using SmartFramework4v2.Web.Common.JSON;
using SmartFramework4v2.Web.WebExecutor;
using System;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// ChbKp 的摘要说明
/// </summary>
[CSClass("ChbKp")]
public class ChbKp
{
    [CSMethod("GetInfo")]
    public object GetInfo()
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                var companyId = SystemUser.CurrentUser.CompanyID;
                string sql = "select * from tb_b_carriagechb ";
                SqlCommand cmd = new SqlCommand(sql);
                DataTable dt = dbc.ExecuteDataTable(cmd);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("SaveInfo")]
    public object SaveInfo(JSReader jsr)
    {
        var user = SystemUser.CurrentUser;
        string companyId = user.CompanyID;
        using (DBConnection dbc = new DBConnection())
        {
            string mc = jsr["name"];
            if (mc == "")
            {
                throw new Exception("名称不能为空！");
            }

            if (jsr["id"].ToString() == "")
            {
                //新增
                string id = Guid.NewGuid().ToString();

                var dt = dbc.GetEmptyDataTable("tb_b_carriagechb");
                var sr = dt.NewRow();
                sr["id"] = new Guid(id);
                sr["name"] = mc;
                dt.Rows.Add(sr);
                dbc.InsertTable(dt);
            }
            else
            {
                //修改
                string id = jsr["id"].ToString();
                var dt = dbc.GetEmptyDataTable("tb_b_carriagechb");
                var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                var sr = dt.NewRow();
                sr["id"] = new Guid(id);
                sr["name"] = mc;
                dt.Rows.Add(sr);
                dbc.UpdateTable(dt, dtt);
            }

            return GetInfo();
        }
    }
}