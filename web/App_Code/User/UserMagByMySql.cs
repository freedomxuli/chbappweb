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
using System.Net;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using ThoughtWorks.QRCode.Codec;
using WxPayAPI;
using LitJson;
using SmartFramework4v2.Data.MySql;


[CSClass("UserMagByMySqlClass")]
public class UserMagByMySqlClass
{

    
    [CSMethod("SaveUser")]
    public static bool SaveUser(JSReader jsr)
    {
        if (jsr["UserName"].IsNull || jsr["UserName"].IsEmpty)
        {
            throw new Exception("用户名不能为空");
        }
        if (jsr["Password"].IsNull || jsr["Password"].IsEmpty)
        {
            throw new Exception("密码不能为空");
        }

        var companyId = SystemUser.CurrentUser.CompanyID;
        var YHID = Guid.NewGuid().ToString();
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                if (jsr["UserID"].ToString() == "")
                {
                    DataTable dt_user = dbc.ExecuteDataTable("select * from tb_b_user where UserName='" + jsr["UserName"].ToString() + "'");
                    if (dt_user.Rows.Count > 0)
                    {
                        throw new Exception("该用户名已存在！");
                    }
                    //用户表

                    var dt = dbc.GetEmptyDataTable("tb_b_user");
                    var dr = dt.NewRow();
                    dr["UserID"] = new Guid(YHID);
                    dr["UserName"] = jsr["UserName"].ToString();
                    dr["Password"] = jsr["Password"].ToString();
                    dr["AddTime"] = DateTime.Now;
                    dr["IsSHPass"] = 1;
                    dr["Points"] = 0;
                    dr["ClientKind"] = 0;
                    //dr["Discount"] = ;
                    dr["UserXM"] = jsr["UserXM"].ToString();
                    dr["UserTel"] = jsr["UserTel"].ToString();
                    //dr["FromRoute"] = ;
                    //dr["ToRoute"] = ;
                    dr["companyId"] = companyId;
                    //dr["PayPassword"] = ;
                    dr["Address"] = jsr["Address"].ToString();
                    dt.Rows.Add(dr);
                    dbc.InsertTable(dt);

                    //角色用户关联表
                    var rdt = dbc.GetEmptyDataTable("tb_b_user_role");
                    var rdr = rdt.NewRow();
                    rdr["userroleId"] = Guid.NewGuid().ToString();
                    rdr["userId"] = new Guid(YHID);
                    rdr["roleId"] = jsr["roleId"].ToString();
                    rdr["companyId"] = companyId;
                    rdt.Rows.Add(rdr);
                    dbc.InsertTable(rdt);

                }
                else
                {
                    YHID = jsr["UserID"].ToString();
                    var oldname = dbc.ExecuteScalar("select UserName from tb_b_user where UserID='" + YHID + "'");
                    if (!jsr["UserName"].ToString().Equals(oldname.ToString()))
                    {
                        DataTable dt_user = dbc.ExecuteDataTable("select * from tb_b_user where UserName='" + jsr["UserName"].ToString() + "'");
                        if (dt_user.Rows.Count > 0)
                        {
                            throw new Exception("该用户名已存在！");
                        }
                    }
                    var dt = dbc.GetEmptyDataTable("tb_b_user");
                    var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                    var dr = dt.NewRow();
                    dr["UserID"] = new Guid(YHID);
                    dr["UserName"] = jsr["UserName"].ToString();
                    dr["Password"] = jsr["Password"].ToString();
                    dr["UserXM"] = jsr["UserXM"].ToString();
                    dr["UserTel"] = jsr["UserTel"].ToString();
                    dr["Address"] = jsr["Address"].ToString();
                    dt.Rows.Add(dr);
                    dbc.UpdateTable(dt, dtt);

                    //删除用户的角色关联
                    string del_js = "delete from tb_b_user_role where userId=@userId";
                    SqlCommand cmd = new SqlCommand(del_js);
                    cmd.Parameters.AddWithValue("@userId", YHID);
                    dbc.ExecuteNonQuery(cmd);

                    //建立用户角色关联
                    var rdt = dbc.GetEmptyDataTable("tb_b_user_role");
                    var rdr = rdt.NewRow();
                    rdr["userroleId"] = Guid.NewGuid().ToString();
                    rdr["userId"] = new Guid(YHID);
                    rdr["roleId"] = jsr["roleId"].ToString();
                    rdr["companyId"] = companyId;
                    rdt.Rows.Add(rdr);
                    dbc.InsertTable(rdt);
                }
                dbc.CommitTransaction();
                 
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }


        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                if (jsr["UserID"].ToString() == "")
                {
                    DataTable dt_user = dbc.ExecuteDataTable("select * from tb_b_user where UserName='" + jsr["UserName"].ToString() + "'");
                    if (dt_user.Rows.Count > 0)
                    {
                        throw new Exception("该用户名已存在！");
                    }
                    //用户表
                    var dt = dbc.GetEmptyDataTable("tb_b_user");
                    var dr = dt.NewRow();
                    dr["userid"] = new Guid(YHID);
                    dr["correlationid"] = new Guid(YHID);
                    dr["account"] = jsr["UserName"].ToString();
                    dr["password"] = jsr["Password"].ToString();
                    dr["usertype"] = 0;
                    dr["status"] = 0;
                    dr["addtime"] = DateTime.Now;
                    dr["username"] = jsr["UserXM"].ToString();
                    dr["usertel"] = jsr["UserTel"].ToString();

                    dr["registerationdatetime"] = DateTime.Now;
                    dr["updatetimedatetime"] = DateTime.Now;
                    dr["isdeleteflag"] = 0;




                    dt.Rows.Add(dr);
                    dbc.InsertTable(dt);


                }
                else
                {
                    YHID = jsr["UserID"].ToString();
                    var oldname = dbc.ExecuteScalar("select account from tb_b_user where userid='" + YHID + "'");
                    if (!jsr["UserName"].ToString().Equals(oldname.ToString()))
                    {
                        DataTable dt_user = dbc.ExecuteDataTable("select * from tb_b_user where account='" + jsr["UserName"].ToString() + "'");
                        if (dt_user.Rows.Count > 0)
                        {
                            throw new Exception("该用户名已存在！");
                        }
                    }
                    var dt = dbc.GetEmptyDataTable("tb_b_user");
                    var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                    var dr = dt.NewRow();
                    dr["UserID"] = new Guid(YHID);
                    dr["correlationid"] = new Guid(YHID);
                    dr["account"] = jsr["UserName"].ToString();
                    dr["password"] = jsr["Password"].ToString();
                    dr["username"] = jsr["UserXM"].ToString();
                    dr["usertel"] = jsr["UserTel"].ToString();

                    dr["updatetimedatetime"] = DateTime.Now;

                    dt.Rows.Add(dr);
                    dbc.UpdateTable(dt, dtt);


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



    [CSMethod("DelUser")]
    public bool DelUser(JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                for (int i = 0; i < jsr.ToArray().Length; i++)
                {
                    string delstr = "delete from tb_b_user_role where userId=@userId";
                    SqlCommand cmd = new SqlCommand(delstr);
                    cmd.Parameters.AddWithValue("@userId", jsr.ToArray()[i].ToString());
                    dbc.ExecuteNonQuery(cmd);

                    string str = "delete from tb_b_user where UserID=@UserID and (ClientKind<>1 and ClientKind<>2)";
                    SqlCommand ucmd = new SqlCommand(str);
                    ucmd.Parameters.AddWithValue("@UserID", jsr.ToArray()[i].ToString());
                    dbc.ExecuteNonQuery(ucmd);
                }

                dbc.CommitTransaction();
               
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }


        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                for (int i = 0; i < jsr.ToArray().Length; i++)
                {
                    

                    string str = "delete from tb_b_user where UserID=@UserID ";
                    var ucmd = dbc.CreateCommand(str);
                    ucmd.Parameters.AddWithValue("@UserID", jsr.ToArray()[i].ToString());
                    dbc.ExecuteNonQuery(ucmd);
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

    
    [CSMethod("SelectFromUser")]
    public object SelectFromUser(string UserName, string UserXM)
    {
        using (DBConnection dbc = new DBConnection())
        {
             try
            {

                var cmd = dbc.CreateCommand();
                cmd.CommandText = "select UserID,UserName,UserXM,Address from tb_b_user where 1=1";
                if (UserName != null && UserName != "")
                {
                    cmd.CommandText += " and UserName like @UserName";
                    cmd.Parameters.AddWithValue("@UserName", "%" + UserName + "%");
                }
                if (UserXM != null && UserXM != "")
                {
                    cmd.CommandText += " and UserXM like @UserXM";
                    cmd.Parameters.AddWithValue("@UserXM", "%" + UserXM + "%");
                }

                cmd.CommandText += " order by UserXM desc";

                var dt = dbc.ExecuteDataTable(cmd);
                return dt;

            }
            catch (Exception ex)
            {
                 throw ex;
            }
        }
         
 
    }



    [CSMethod("SaveGYS")]
    public static bool SaveGYS(JSReader jsr)
    {
       
        var companyId = SystemUser.CurrentUser.CompanyID;
        var YHID = Guid.NewGuid().ToString();
        

        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                if (jsr["userid"].ToString() == "")
                {
                    DataTable dt_user = dbc.ExecuteDataTable("select * from tb_b_user where UserName='" + jsr["username"].ToString() + "'");
                    if (dt_user.Rows.Count > 0)
                    {
                        throw new Exception("该用户名已存在！");
                    }
                    //用户表
                    var dt = dbc.GetEmptyDataTable("tb_b_user");
                    var dr = dt.NewRow();
                    dr["userid"] = new Guid(YHID);
                    dr["username"] = jsr["username"].ToString();
                    dr["carrierregisteredaddress"] = jsr["carrierregisteredaddress"].ToString();
                    dr["businessscope"] = jsr["businessscope"].ToString();
                    dr["permitnumber"] = jsr["permitnumber"].ToString();

                    dr["correlationid"] = jsr["correlationid"].ToString();
                    dr["carriertel"] = jsr["carriertel"].ToString();
                    dr["usertel"] = jsr["usertel"].ToString();

                    dr["dqbm"] = jsr["dqbm"].ToString();
                    dr["countrysubdivisioncode"] = jsr["countrysubdivisioncode"].ToString();

                    dr["usertype"] = 2;
                    dr["status"] = 0;
                    dr["addtime"] = DateTime.Now;

                    dr["isadmin"] = 0;

                    dr["registerationdatetime"] = DateTime.Now;
                    dr["updatetimedatetime"] = DateTime.Now;
                    dr["isdeleteflag"] = 0;


                    dr["carrierroute"] = jsr["carrierroute"].ToString();
                    dr["carriercontact"] = jsr["carriercontact"].ToString();
                    dr["carriercontact"] = jsr["carriercontact"].ToString();
                    dr["invoicerise"] = jsr["invoicerise"].ToString();
                    dr["invoiceaddress"] = jsr["invoiceaddress"].ToString();
                    dr["invoicetel"] = jsr["invoicetel"].ToString();
                    dr["invoicenumber"] = jsr["invoicenumber"].ToString();
                    dr["invoicebank"] = jsr["invoicebank"].ToString();
                    dr["invoicebanknumber"] = jsr["invoicebanknumber"].ToString();






                    dt.Rows.Add(dr);
                    dbc.InsertTable(dt);


                }
                else
                {
                    YHID = jsr["userid"].ToString();
                    var oldname = dbc.ExecuteScalar("select account from tb_b_user where userid='" + YHID + "'");
                    if (!jsr["username"].ToString().Equals(oldname.ToString()))
                    {
                        DataTable dt_user = dbc.ExecuteDataTable("select * from tb_b_user where account='" + jsr["username"].ToString() + "'");
                        if (dt_user.Rows.Count > 0)
                        {
                            throw new Exception("该用户名已存在！");
                        }
                    }
                    var dt = dbc.GetEmptyDataTable("tb_b_user");
                    var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                    var dr = dt.NewRow();
                    dr["userid"] = new Guid(YHID);
                    dr["username"] = jsr["username"].ToString();
                    dr["carrierregisteredaddress"] = jsr["carrierregisteredaddress"].ToString();
                    dr["businessscope"] = jsr["businessscope"].ToString();
                    dr["permitnumber"] = jsr["permitnumber"].ToString();
                    dr["dqbm"] = jsr["dqbm"].ToString();
                    dr["countrysubdivisioncode"] = jsr["countrysubdivisioncode"].ToString();
                    dr["isadmin"] = 0;

                    dr["correlationid"] = jsr["correlationid"].ToString();
                    dr["updatetimedatetime"] = DateTime.Now;

                    dr["carrierroute"] = jsr["carrierroute"].ToString();
                    dr["carriercontact"] = jsr["carriercontact"].ToString();
                    dr["carriertel"] = jsr["carriertel"].ToString();
                    dr["usertel"] = jsr["usertel"].ToString(); 
                    dr["invoicerise"] = jsr["invoicerise"].ToString();
                    dr["invoiceaddress"] = jsr["invoiceaddress"].ToString();
                    dr["invoicetel"] = jsr["invoicetel"].ToString();
                    dr["invoicenumber"] = jsr["invoicenumber"].ToString();
                    dr["invoicebank"] = jsr["invoicebank"].ToString();
                    dr["invoicebanknumber"] = jsr["invoicebanknumber"].ToString();


                    dt.Rows.Add(dr);
                    dbc.UpdateTable(dt, dtt);


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



    [CSMethod("SaveSJ")]
    public static bool SaveSJ(JSReader jsr)
    {

        var companyId = SystemUser.CurrentUser.CompanyID;
        var YHID = Guid.NewGuid().ToString();


        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                if (jsr["userid"].ToString() == "")
                {
                    DataTable dt_user = dbc.ExecuteDataTable("select * from tb_b_user where UserName='" + jsr["username"].ToString() + "'");
                    if (dt_user.Rows.Count > 0)
                    {
                        throw new Exception("该用户名已存在！");
                    }
                    //用户表
                    var dt = dbc.GetEmptyDataTable("tb_b_user");
                    var dr = dt.NewRow();
                    dr["userid"] = new Guid(YHID);
                    dr["username"] = jsr["username"].ToString();
                    dr["GENDER"] = jsr["GENDER"].ToString();
                    dr["identitydocumentnumber"] = jsr["identitydocumentnumber"].ToString();
                    dr["mobiletelephonenumber"] = jsr["mobiletelephonenumber"].ToString();
                    dr["usertel"] = jsr["telephonenumber"].ToString();

                    dr["telephonenumber"] = jsr["telephonenumber"].ToString();

                    dr["validperiodfrom"] = jsr["validperiodfrom"].ToString();
                    dr["validperiodto"] = jsr["validperiodto"].ToString();

                    dr["usertype"] = 4;
                    dr["status"] = 0;
                    dr["addtime"] = DateTime.Now;

                    dr["isadmin"] = 0;

                    dr["registerationdatetime"] = DateTime.Now;
                    dr["updatetimedatetime"] = DateTime.Now;
                    dr["isdeleteflag"] = 0;

                    dr["vehicleclass"] = jsr["vehicleclass"].ToString();

                    dr["drivertype"] = jsr["drivertype"].ToString();
                    dr["driverbanktype"] = jsr["driverbanktype"].ToString();
                    dr["vehiclenumber"] = jsr["vehiclenumber"].ToString();
                    dr["driverbanknumber"] = jsr["driverbanknumber"].ToString();
                    dr["driverroutenumber"] = jsr["driverroutenumber"].ToString();


                    dr["vehicletype"] = jsr["vehicletype"].ToString();
                    dr["vehiclelength"] = jsr["vehiclelength"].ToString();
                    dr["vehicletime"] = jsr["vehicletime"].ToString();

      






                    dt.Rows.Add(dr);
                    dbc.InsertTable(dt);


                }
                else
                {
                    YHID = jsr["userid"].ToString();
                    var oldname = dbc.ExecuteScalar("select account from tb_b_user where userid='" + YHID + "'");
                    if (!jsr["username"].ToString().Equals(oldname.ToString()))
                    {
                        DataTable dt_user = dbc.ExecuteDataTable("select * from tb_b_user where account='" + jsr["username"].ToString() + "'");
                        if (dt_user.Rows.Count > 0)
                        {
                            throw new Exception("该用户名已存在！");
                        }
                    }
                    var dt = dbc.GetEmptyDataTable("tb_b_user");
                    var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                    var dr = dt.NewRow();
                    dr["userid"] = new Guid(YHID);
                    dr["username"] = jsr["username"].ToString();
                    dr["GENDER"] = jsr["GENDER"].ToString();
                    dr["identitydocumentnumber"] = jsr["identitydocumentnumber"].ToString();
                    dr["mobiletelephonenumber"] = jsr["mobiletelephonenumber"].ToString();
                    dr["usertel"] = jsr["telephonenumber"].ToString();

                    dr["telephonenumber"] = jsr["telephonenumber"].ToString();

                    dr["validperiodfrom"] = jsr["validperiodfrom"].ToString();
                    dr["validperiodto"] = jsr["validperiodto"].ToString();
 

                     dr["updatetimedatetime"] = DateTime.Now;


                     dr["vehicleclass"] = jsr["vehicleclass"].ToString();

                    dr["drivertype"] = jsr["drivertype"].ToString();
                    dr["driverbanktype"] = jsr["driverbanktype"].ToString();
                    dr["vehiclenumber"] = jsr["vehiclenumber"].ToString();
                    dr["driverbanknumber"] = jsr["driverbanknumber"].ToString();
                    dr["driverroutenumber"] = jsr["driverroutenumber"].ToString();

                    dr["vehicletype"] = jsr["vehicletype"].ToString();
                    dr["vehiclelength"] = jsr["vehiclelength"].ToString();
                    dr["vehicletime"] = jsr["vehicletime"].ToString();

                    dt.Rows.Add(dr);
                    dbc.UpdateTable(dt, dtt);


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


    [CSMethod("SaveQY")]
    public static bool SaveQY(JSReader jsr)
    {

        var companyId = SystemUser.CurrentUser.CompanyID;
        var YHID = Guid.NewGuid().ToString();


        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                if (jsr["userid"].ToString() == "")
                {
                    DataTable dt_user = dbc.ExecuteDataTable("select * from tb_b_user where UserName='" + jsr["username"].ToString() + "'");
                    if (dt_user.Rows.Count > 0)
                    {
                        throw new Exception("该用户名已存在！");
                    }
                    //用户表
                    var dt = dbc.GetEmptyDataTable("tb_b_user");
                    var dr = dt.NewRow();
                    dr["userid"] = new Guid(YHID);
                    dr["account"] = jsr["account"].ToString();
                    dr["password"] = jsr["password"].ToString();

                    dr["dqbm"] = jsr["dqbm"].ToString();
                    dr["correlationid"] = jsr["correlationid"].ToString();

                    dr["companycode"] = jsr["companycode"].ToString();

                    dr["username"] = jsr["username"].ToString();
                    dr["usertel"] = jsr["usertel"].ToString();

                    dr["registeredcapital"] = jsr["registeredcapital"].ToString();

                    dr["unifiedsocialcreditldentifier"] = jsr["unifiedsocialcreditldentifier"].ToString();
                    dr["unifiedsocialdatetime"] = jsr["unifiedsocialdatetime"].ToString();
                    dr["invoicerise"] = jsr["invoicerise"].ToString();

                    dr["invoiceaddress"] = jsr["invoiceaddress"].ToString();

                    dr["invoicetel"] = jsr["invoicetel"].ToString();
                    dr["invoicenumber"] = jsr["invoicenumber"].ToString();

                    dr["invoicebank"] = jsr["invoicebank"].ToString();
                    
                    dr["invoicebanknumber"] = jsr["invoicebanknumber"].ToString();

                    if (jsr["accountrate"] != null && jsr["accountrate"].ToString() != "")
                    {
                        dr["accountrate"] = Convert.ToDecimal(jsr["accountrate"].ToString());
                    }

                    if (jsr["completerate"] != null && jsr["completerate"].ToString() != "")
                    {
                        dr["completerate"] = Convert.ToDecimal(jsr["completerate"].ToString());
                    }
                    if (jsr["nooilmoney"] != null && jsr["nooilmoney"].ToString() != "")
                    {
                        dr["nooilmoney"] = Convert.ToDecimal(jsr["nooilmoney"].ToString());
                    }





                    if (jsr["advancerate"] != null && jsr["advancerate"].ToString() != "")
                    {
                        dr["advancerate"] = Convert.ToDecimal(jsr["advancerate"].ToString());
                    }
                    if (jsr["operaterate"] != null && jsr["operaterate"].ToString() != "")
                    {
                        dr["operaterate"] = Convert.ToDecimal(jsr["operaterate"].ToString());
                    }
                    if (jsr["grossrate"] != null && jsr["grossrate"].ToString() != "")
                    {
                        dr["grossrate"] = Convert.ToDecimal(jsr["grossrate"].ToString());
                    }
                    if (jsr["oilmoneyrate"] != null && jsr["oilmoneyrate"].ToString() != "")
                    {
                        dr["oilmoneyrate"] = Convert.ToDecimal(jsr["oilmoneyrate"].ToString());
                    }
                    if (jsr["oilmoneyrate"] != null && jsr["invoicerate"].ToString() != "")
                    {
                        dr["invoicerate"] = Convert.ToDecimal(jsr["invoicerate"].ToString());
                    }








                    if (jsr["cashrate"] != null && jsr["cashrate"].ToString() != "")
                    {
                        dr["cashrate"] = jsr["cashrate"].ToString();
                    }
                    if (jsr["isneededit"] != null && jsr["isneededit"].ToString() != "")
                    {
                        dr["isneededit"] = 0;
                    }
                    else
                    {
                        dr["isneededit"] = 1;
                    }

                    if (jsr["ismonthly"] != null && jsr["ismonthly"].ToString() != "")
                    {
                        dr["ismonthly"] = 0;
                    }
                    else {
                        dr["ismonthly"] = 1;
                    }



                    if (jsr["paychbrate"] != null && jsr["paychbrate"].ToString() != "")
                    {
                        dr["paychbrate"] = jsr["paychbrate"].ToString();
                    }
                    dr["roleid"] = 0;

                    dr["isadmin"] = 0;

                    dr["usertype"] = 3;
                    dr["status"] = 0;
                    dr["addtime"] = DateTime.Now;
                     dr["registerationdatetime"] = DateTime.Now;
                    dr["updatetimedatetime"] = DateTime.Now;
                    dr["isdeleteflag"] = 0;




                    dt.Rows.Add(dr);
                    dbc.InsertTable(dt);


                }
                else
                {
                    YHID = jsr["userid"].ToString();
                    var oldname = dbc.ExecuteScalar("select account from tb_b_user where userid='" + YHID + "'");
                    if (!jsr["username"].ToString().Equals(oldname.ToString()))
                    {
                        DataTable dt_user = dbc.ExecuteDataTable("select * from tb_b_user where account='" + jsr["username"].ToString() + "'");
                        if (dt_user.Rows.Count > 0)
                        {
                            throw new Exception("该用户名已存在！");
                        }
                    }
                    var dt = dbc.GetEmptyDataTable("tb_b_user");
                    var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                    var dr = dt.NewRow();
                    dr["userid"] = new Guid(YHID);
                    dr["account"] = jsr["account"].ToString();
                    dr["password"] = jsr["password"].ToString();

                    dr["dqbm"] = jsr["dqbm"].ToString();
                    dr["correlationid"] = jsr["correlationid"].ToString();

                    dr["companycode"] = jsr["companycode"].ToString();

                    dr["username"] = jsr["username"].ToString();
                    dr["usertel"] = jsr["usertel"].ToString();

                    dr["registeredcapital"] = jsr["registeredcapital"].ToString();

                    dr["unifiedsocialcreditldentifier"] = jsr["unifiedsocialcreditldentifier"].ToString();
                    dr["unifiedsocialdatetime"] = jsr["unifiedsocialdatetime"].ToString();
                    dr["invoicerise"] = jsr["invoicerise"].ToString();

                    dr["invoiceaddress"] = jsr["invoiceaddress"].ToString();

                    dr["invoicetel"] = jsr["invoicetel"].ToString();
                    dr["invoicenumber"] = jsr["invoicenumber"].ToString();

                    dr["invoicebank"] = jsr["invoicebank"].ToString();

                    dr["invoicebanknumber"] = jsr["invoicebanknumber"].ToString();

                    if (jsr["accountrate"] != null && jsr["accountrate"].ToString() != "")
                    {
                        dr["accountrate"] = Convert.ToDecimal(jsr["accountrate"].ToString());
                    }

                    if (jsr["completerate"] != null && jsr["completerate"].ToString() != "")
                    {
                        dr["completerate"] = Convert.ToDecimal(jsr["completerate"].ToString());
                    }
                    if (jsr["nooilmoney"] != null && jsr["nooilmoney"].ToString() != "")
                    {
                        dr["nooilmoney"] = Convert.ToDecimal(jsr["nooilmoney"].ToString());
                    }



                    if (jsr["advancerate"] != null && jsr["advancerate"].ToString() != "")
                    {
                        dr["advancerate"] = Convert.ToDecimal(jsr["advancerate"].ToString());
                    }
                    if (jsr["operaterate"] != null && jsr["operaterate"].ToString() != "")
                    {
                        dr["operaterate"] = Convert.ToDecimal(jsr["operaterate"].ToString());
                    }
                    if (jsr["grossrate"] != null && jsr["grossrate"].ToString() != "")
                    {
                        dr["grossrate"] = Convert.ToDecimal(jsr["grossrate"].ToString());
                    }
                    if (jsr["oilmoneyrate"] != null && jsr["oilmoneyrate"].ToString() != "")
                    {
                        dr["oilmoneyrate"] = Convert.ToDecimal(jsr["oilmoneyrate"].ToString());
                    }
                    if (jsr["oilmoneyrate"] != null && jsr["invoicerate"].ToString() != "")
                    {
                        dr["invoicerate"] = Convert.ToDecimal(jsr["invoicerate"].ToString());
                    }


                    if (jsr["cashrate"] != null && jsr["cashrate"].ToString() != "")
                    {
                        dr["cashrate"] = jsr["cashrate"].ToString();
                    }
                    if (jsr["isneededit"] != null && jsr["isneededit"].ToString() != "")
                    {
                        dr["isneededit"] = 1;
                    }
                    else
                    {
                        dr["isneededit"] = 0;
                    }

                    if (jsr["ismonthly"] != null && jsr["ismonthly"].ToString() != "")
                    {
                        dr["ismonthly"] = 0;
                    }
                    else
                    {
                        dr["ismonthly"] = 1;
                    }
                    dr["isadmin"] = 0;
                    if (jsr["paychbrate"] != null && jsr["paychbrate"].ToString() != "")
                    {
                        dr["paychbrate"] = jsr["paychbrate"].ToString();
                    }
 
                    dr["updatetimedatetime"] = DateTime.Now;

                    dt.Rows.Add(dr);
                    dbc.UpdateTable(dt, dtt);


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


    [CSMethod("GetUserList")]
    public object GetUserList(int pagnum, int pagesize, string roleId, string yhm, string areacode,int order)
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
                    where += " and " + dbc.C_Like("username", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(areacode.Trim()))
                {
                    where += " and " + dbc.C_EQ("dqbm", areacode.Trim());
                }

                string str = @"select * from tb_b_user where usertype=2 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by username", pagesize, ref cp, out ac);

                var privilege = new SystemUser().GetPrivilegeList(SystemUser.CurrentUser.RoleID);
                 
                 

                return new { dt = dtPage, cp = cp, ac = ac  };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }


    [CSMethod("GetUserListSJ")]
    public object GetUserListSJ(int pagnum, int pagesize, string roleId, string yhm, string drivertype, string tel)
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
                    where += " and " + dbc.C_Like("username", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(drivertype.Trim()))
                {
                    where += " and " + dbc.C_EQ("dqbm", drivertype.Trim());
                }

                if (!string.IsNullOrEmpty(tel.Trim()))
                {
                    where += " and  (" + dbc.C_Like("telephonenumber", tel.Trim(), LikeStyle.LeftAndRightLike);
                    where += " or " + dbc.C_Like("mobiletelephonenumber", tel.Trim(), LikeStyle.LeftAndRightLike) + ")";

                }

                string str = @"select * from tb_b_user where usertype=4 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by username", pagesize, ref cp, out ac);

                var privilege = new SystemUser().GetPrivilegeList(SystemUser.CurrentUser.RoleID);



                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }


    //企业用户登录
    [CSMethod("GetUserListByQY")]
    public object GetUserListByQY(int pagnum, int pagesize, string roleId, string yhm, string areacode)
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
                    where += " and " + dbc.C_Like("username", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                

                string str = @"select * from tb_b_user where usertype=3 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by username", pagesize, ref cp, out ac);

                var privilege = new SystemUser().GetPrivilegeList(SystemUser.CurrentUser.RoleID);



                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }


    [CSMethod("DelUserGYS")]
    public bool DelUserGYS(JSReader jsr)
    {
        

        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                for (int i = 0; i < jsr.ToArray().Length; i++)
                {


                    string str = "delete from tb_b_user where userid=@userid ";
                    var ucmd = dbc.CreateCommand(str);
                    ucmd.Parameters.AddWithValue("@userid", jsr.ToArray()[i].ToString());
                    dbc.ExecuteNonQuery(ucmd);
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



    [CSMethod("GetBBBCX")]
    public object GetBBBCX()
    {


        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                  var cmd = dbc.CreateCommand();
                  cmd.CommandText = "SELECT id,name FROM tb_b_dictionary_detail WHERE bm LIKE '018%'";
                var dt = dbc.ExecuteDataTable(cmd);
                 return dt;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }


    [CSMethod("GetBBBCC")]
    public object GetBBBCC(string ccbm)
    {


        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                var cmd = dbc.CreateCommand();
                cmd.CommandText = "SELECT id,name FROM tb_b_dictionary_detail WHERE bm LIKE " + dbc.ToSqlValue(ccbm + "%");
                var dt = dbc.ExecuteDataTable(cmd);
                return dt;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }


    [CSMethod("Getcorrelationid")]
    public object Getcorrelationid(string UserID)
    {


        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                var UserName = "";
                if (UserID != null && UserID != "")
                {

                    var cmd = dbc.CreateCommand();
                    cmd.CommandText = "select UserName from tb_b_user where UserID=@UserID ";
                    cmd.Parameters.Add("@UserID", UserID);
                    var dt = dbc.ExecuteDataTable(cmd);
                   
                    if (dt.Rows.Count > 0)
                    {
                        UserName = dt.Rows[0][0].ToString();
                    }
                }
                return UserName;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        return true;

    }



    [CSMethod("GetGLYWY")]
    public object GetGLYWY(string UserID, int operatortype)
    {
         using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                 var cmd = dbc.CreateCommand();
                 cmd.CommandText = "SELECT a.id,b.account as UserName,b.username as UserXM,b.userid as UserID FROM tb_b_operator_association a LEFT JOIN tb_b_user b ON a.operator=b.userid WHERE a.status=0 and a.operatortype=" + operatortype + " and  a.userid=" + dbc.ToSqlValue(UserID);
                 var dt = dbc.ExecuteDataTable(cmd);

                return dt;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
     }

    [CSMethod("GetGLCZY")]
    public object GetGLCZY(string UserID, int operatortype)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                var cmd = dbc.CreateCommand();
                cmd.CommandText = "SELECT a.id,b.account as UserName,b.username as UserXM,b.userid as UserID FROM tb_b_operator_association a LEFT JOIN tb_b_user b ON a.operator=b.userid WHERE a.status=0 and a.operatortype=" + operatortype + " and  a.userid=" + dbc.ToSqlValue(UserID);
                var dt = dbc.ExecuteDataTable(cmd);

                return dt;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetGLGOODS")]
    public object GetGLGOODS(string UserID)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                var cmd = dbc.CreateCommand();
                cmd.CommandText = "SELECT a.goodsid,b.goodsname,b.goodstype FROM tb_b_goods_association a LEFT JOIN tb_b_goods b ON a.goodsid=b.goodsid WHERE a.status=0  and  a.userid=" + dbc.ToSqlValue(UserID);
                var dt = dbc.ExecuteDataTable(cmd);

                return dt;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }


    [CSMethod("GetGLLXR")]
    public object GetGLLXR(string UserID)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                var cmd = dbc.CreateCommand();
                cmd.CommandText = "SELECT * FROM tb_b_contacts_person WHERE status=0 and userid=" + dbc.ToSqlValue(UserID);
                var dt = dbc.ExecuteDataTable(cmd);

                return dt;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetGLFLD")]
    public object GetGLFLD(string UserID)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                var cmd = dbc.CreateCommand();
                cmd.CommandText = "SELECT * FROM tb_b_carrier_points WHERE status=0 and userid=" + dbc.ToSqlValue(UserID);
                var dt = dbc.ExecuteDataTable(cmd);

                return dt;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

     



    [CSMethod("DelGLYWY")]
    public object DelGLYWY(string id)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                var cmd = dbc.CreateCommand();
                cmd.CommandText = "update  tb_b_operator_association set status=1 WHERE id=" + dbc.ToSqlValue(id);
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
    /// 添加业务员关联关系，如果当前关联表中有关联则不行进关联
    /// </summary>
    /// <param name="jsr"></param>
    /// <returns></returns>
    [CSMethod("SaveYWYGL")]
    public bool SaveYWYGL(JSReader jsr, string userid, int operatortype)
    {


        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                for (int i = 0; i < jsr.ToArray().Length; i++)
                {
                    //判断是否有关联关系存在
                    var sccount = dbc.ExecuteScalar("select count(*) from tb_b_operator_association where status=0 and operatortype=" + dbc.ToSqlValue(operatortype) + " and  userid=" + dbc.ToSqlValue(userid) + " and operator=" + dbc.ToSqlValue(jsr.ToArray()[i].ToString()));
                    if (Convert.ToInt32(sccount) == 0)
                    {
                        var dt = dbc.GetEmptyDataTable("tb_b_operator_association");
                        var dr = dt.NewRow();
                        dr["id"] = Guid.NewGuid();
                        dr["operator"] = jsr.ToArray()[i].ToString();
                        dr["userid"] = userid;
                        dr["status"] = 0;

                        dr["adduser"] = SystemUser.CurrentUser.UserID;
                        dr["updateuser"] = SystemUser.CurrentUser.UserID;
                        dr["addtime"] = DateTime.Now;
                        dr["updatetime"] = DateTime.Now;

                        dr["operatortype"] = operatortype;
                        
 
 



                        dt.Rows.Add(dr);
                        dbc.InsertTable(dt);
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



    [CSMethod("SaveLXR")]
    public static bool SaveLXR(JSReader jsr, string usid)
    {

         var YHID = Guid.NewGuid().ToString();
         using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                if (jsr["id"].ToString() == "")
                {


                    var dt = dbc.GetEmptyDataTable("tb_b_contacts_person");
                    var dr = dt.NewRow();
                    dr["id"] = new Guid(YHID);
                    dr["name"] = jsr["name"].ToString();
                    dr["tel"] = jsr["tel"].ToString();
                    dr["userid"] = usid;
                    dr["status"] = 0;

                    dr["adduser"] = SystemUser.CurrentUser.UserID;
                    dr["updateuser"] = SystemUser.CurrentUser.UserID;
                    dr["addtime"] = DateTime.Now;
                    dr["updatetime"] = DateTime.Now;



                    dt.Rows.Add(dr);
                    dbc.InsertTable(dt);


                }
                else
                {
                    YHID = jsr["id"].ToString();

                    var dt = dbc.GetEmptyDataTable("tb_b_contacts_person");
                    var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                    var dr = dt.NewRow();
                    dr["id"] = new Guid(YHID);
                    dr["name"] = jsr["name"].ToString();
                    dr["tel"] = jsr["tel"].ToString();
                    dr["userid"] = usid;
                    dr["updateuser"] = SystemUser.CurrentUser.UserID;
                    dr["updatetime"] = DateTime.Now;

                    dt.Rows.Add(dr);
                    dbc.UpdateTable(dt, dtt);


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


    [CSMethod("SaveFLD")]
    public static bool SaveFLD(JSReader jsr, string usid)
    {

        var YHID = Guid.NewGuid().ToString();
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                if (jsr["id"].ToString() == "")
                {


                    var dt = dbc.GetEmptyDataTable("tb_b_carrier_points");
                    var dr = dt.NewRow();
                    dr["id"] = new Guid(YHID);
                    dr["pointsname"] = jsr["pointsuser"].ToString();
                    dr["pointsuser"] = jsr["pointsuser"].ToString();
                    dr["pointstel"] = jsr["pointstel"].ToString();
                    dr["pointsaddress"] = jsr["pointsaddress"].ToString();
                    dr["userid"] = usid;
                    dr["status"] = 0;

                    dr["adduser"] = SystemUser.CurrentUser.UserID;
                    dr["updateuser"] = SystemUser.CurrentUser.UserID;
                    dr["addtime"] = DateTime.Now;
                    dr["updatetime"] = DateTime.Now;



                    dt.Rows.Add(dr);
                    dbc.InsertTable(dt);


                }
                else
                {
                    YHID = jsr["id"].ToString();

                    var dt = dbc.GetEmptyDataTable("tb_b_carrier_points");
                    var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                    var dr = dt.NewRow();
                    dr["id"] = new Guid(YHID);
                    dr["pointsname"] = jsr["pointsuser"].ToString();
                    dr["pointsuser"] = jsr["pointsuser"].ToString();
                    dr["pointstel"] = jsr["pointstel"].ToString();
                    dr["pointsaddress"] = jsr["pointsaddress"].ToString();
                    dr["userid"] = usid;
                    dr["updateuser"] = SystemUser.CurrentUser.UserID;
                    dr["updatetime"] = DateTime.Now;

                    dt.Rows.Add(dr);
                    dbc.UpdateTable(dt, dtt);


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

    [CSMethod("DelGLLXR")]
    public object DelGLLXR(string id)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                var cmd = dbc.CreateCommand();
                cmd.CommandText = "update  tb_b_contacts_person set status=1 WHERE id=" + dbc.ToSqlValue(id);
                dbc.ExecuteNonQuery(cmd);

                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }


    [CSMethod("DelGLFLD")]
    public object DelGLFLD(string id)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                var cmd = dbc.CreateCommand();
                cmd.CommandText = "update  tb_b_carrier_points set status=1 WHERE id=" + dbc.ToSqlValue(id);
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
    /// 根据省份code获取子市
    /// </summary>
    /// <returns></returns>
    [CSMethod("GetAreaList")]
    public DataTable GetAreaList(string pcode)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                if (pcode == "000000")
                {
                    

                    string sql = @"SELECT * FROM tb_b_area WHERE RIGHT(CODE,4)='0000' and pcode=" + dbc.ToSqlValue(pcode) + " order by code";
                    DataTable dt = dbc.ExecuteDataTable(sql);
                    return dt;


                }
                else if (pcode != null && pcode != "")
                {
                    string sql = @"SELECT * FROM tb_b_area WHERE pcode=" + dbc.ToSqlValue(pcode) + " order by code";
                    DataTable dt = dbc.ExecuteDataTable(sql);
                    return dt;
                }
                else {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }


    [CSMethod("GetStartDQ")]
    public DataTable GetStartDQ(string usid)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
               var dt= dbc.ExecuteDataTable(@"SELECT a.*,b.name AS n1,c.name AS n2,d.name AS n3 
FROM tb_b_fromroute_address a LEFT JOIN tb_b_area b ON a.province=b.CODE
LEFT JOIN tb_b_area c ON a.city=c.CODE
LEFT JOIN tb_b_area d ON a.area=d.CODE where a.userid=" + dbc.ToSqlValue(usid));

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetStartDQInfor")]
    public DataTable GetStartDQInfor(string id)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                var dt = dbc.ExecuteDataTable(@"SELECT a.*,b.name AS n1,c.name AS n2,d.name AS n3 
FROM tb_b_fromroute_address a LEFT JOIN tb_b_area b ON a.province=b.CODE
LEFT JOIN tb_b_area c ON a.city=c.CODE
LEFT JOIN tb_b_area d ON a.area=d.CODE where a.id=" + dbc.ToSqlValue(id));

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }


    [CSMethod("SaveStartDQ")]
    public static bool SaveStartDQ(JSReader jsr, string usid,string id)
    {

        var YHID = Guid.NewGuid().ToString();
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                if (id != null && id != "")
                {
                    YHID = id;
                }
                
                var dt = dbc.GetEmptyDataTable("tb_b_fromroute_address");
                var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);

                var dr = dt.NewRow();
                dr["id"] = new Guid(YHID);
                dr["province"] = jsr["province"].ToString();
                dr["city"] = jsr["city"].ToString();
                dr["area"] = jsr["area"].ToString();
                dr["bm"] = jsr["bm"].ToString();
                dr["address"] = jsr["address"].ToString();
                dr["userid"] = usid;
            
 
                if (id != null && id != "")
                {
                     dr["updateuser"] = SystemUser.CurrentUser.UserID;
                     dr["updatetime"] = DateTime.Now;
                     dt.Rows.Add(dr);

                    dbc.UpdateTable(dt,dtt);

                }
                else {

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

    [CSMethod("DelStartDQ")]
    public static bool DelStartDQ(string id)
    {

        var YHID = Guid.NewGuid().ToString();
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                //这个地方全删全插
                dbc.ExecuteNonQuery("delete from tb_b_fromroute_address where id=" + dbc.ToSqlValue(id));
               
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


    [CSMethod("GetEndDQ")]
    public DataTable GetEndDQ(string usid)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                var dt = dbc.ExecuteDataTable(@"SELECT a.*,b.name AS n1,c.name AS n2,d.name AS n3 
FROM tb_b_toroute_address a LEFT JOIN tb_b_area b ON a.province=b.CODE
LEFT JOIN tb_b_area c ON a.city=c.CODE
LEFT JOIN tb_b_area d ON a.area=d.CODE where a.userid=" + dbc.ToSqlValue(usid));

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetEndDQInfor")]
    public DataTable GetEndDQInfor(string id)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                var dt = dbc.ExecuteDataTable(@"SELECT a.*,b.name AS n1,c.name AS n2,d.name AS n3 
FROM tb_b_toroute_address a LEFT JOIN tb_b_area b ON a.province=b.CODE
LEFT JOIN tb_b_area c ON a.city=c.CODE
LEFT JOIN tb_b_area d ON a.area=d.CODE where a.id=" + dbc.ToSqlValue(id));

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("SaveEndDQ")]
    public static bool SaveEndDQ(JSReader jsr, string usid,string id)
    {

        var YHID = Guid.NewGuid().ToString();
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {

                if (id != null && id != "")
                {
                    YHID = id;
                }


                var dt = dbc.GetEmptyDataTable("tb_b_toroute_address");
                var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);

                var dr = dt.NewRow();
                dr["id"] = new Guid(YHID);
                dr["province"] = jsr["province"].ToString();
                dr["city"] = jsr["city"].ToString();
                dr["area"] = jsr["area"].ToString();
                dr["bm"] = jsr["bm"].ToString();
                dr["address"] = jsr["address"].ToString();
                dr["company"] = jsr["company"].ToString();
                dr["person"] = jsr["person"].ToString();
                dr["tel"] = jsr["tel"].ToString();

                dr["userid"] = usid;

                if (id != null && id != "")
                {
                    dr["updateuser"] = SystemUser.CurrentUser.UserID;
                    dr["updatetime"] = DateTime.Now;
                    dt.Rows.Add(dr);

                    dbc.UpdateTable(dt, dtt);

                }
                else
                {

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

    [CSMethod("DelEndDQ")]
    public static bool DelEndDQ(string id)
    {

        var YHID = Guid.NewGuid().ToString();
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                //这个地方全删全插
                dbc.ExecuteNonQuery("delete from tb_b_toroute_address where id=" + dbc.ToSqlValue(id));

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

    [CSMethod("GetGOODS")]
    public object GetGOODS(int pagnum, int pagesize, string goodsname)
    {


        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";


                if (!string.IsNullOrEmpty(goodsname.Trim()))
                {
                    where += " and " + dbc.C_Like("goodsname", goodsname.Trim(), LikeStyle.LeftAndRightLike);
                }


                string str = @"select * from tb_b_goods where status=0 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by goodsname", pagesize, ref cp, out ac);

 
                

                return new { dt = dtPage, cp = cp, ac = ac };
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }


    [CSMethod("SaveGOODSGL")]
    public bool SaveGOODSGL(JSReader jsr, string userid)
    {


        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                for (int i = 0; i < jsr.ToArray().Length; i++)
                {
                    //判断是否有关联关系存在
                    var sccount = dbc.ExecuteScalar("select count(*) from tb_b_goods_association where status=0 and  userid=" + dbc.ToSqlValue(userid) + " and goodsid=" + dbc.ToSqlValue(jsr.ToArray()[i].ToString()));
                    if (Convert.ToInt32(sccount) == 0)
                    {
                        var dt = dbc.GetEmptyDataTable("tb_b_goods_association");
                        var dr = dt.NewRow();
                        dr["id"] = Guid.NewGuid();
                        dr["goodsid"] = jsr.ToArray()[i].ToString();
                        dr["userid"] = userid;
                        dr["status"] = 0;

                        dr["adduser"] = SystemUser.CurrentUser.UserID;
                        dr["updateuser"] = SystemUser.CurrentUser.UserID;
                        dr["addtime"] = DateTime.Now;
                        dr["updatetime"] = DateTime.Now;

 





                        dt.Rows.Add(dr);
                        dbc.InsertTable(dt);
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

    [CSMethod("DelGLGOODS")]
    public object DelGLGOODS(string id)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                var cmd = dbc.CreateCommand();
                cmd.CommandText = "update  tb_b_goods_association set status=1 WHERE goodsid=" + dbc.ToSqlValue(id);
                dbc.ExecuteNonQuery(cmd);

                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }


    [CSMethod("GetProductImages")]
    public object GetProductImages(string pid)
    {

        string _url = System.Configuration.ConfigurationManager.AppSettings["ServiceURL"].ToString() + "tbbuserphoto.selectUserphoto";
        string jsonParam = new JavaScriptSerializer().Serialize(new
        {
            tradeCode = "tbbuserphoto.selectUserphoto",
            userid = pid,
            userphotogltype = 0
        });
        var request1 = (HttpWebRequest)WebRequest.Create(_url);
        request1.Method = "POST";
        request1.ContentType = "application/json;charset=UTF-8";
        var byteData = Encoding.UTF8.GetBytes(jsonParam);
        var length = byteData.Length;
        request1.ContentLength = length;
        var writer = request1.GetRequestStream();
        writer.Write(byteData, 0, length);
        writer.Close();
        var response = (HttpWebResponse)request1.GetResponse();
        var responseString = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();

        return responseString;

    }

    [CSMethod("UploadPicForProduct", 1)]
    public object UploadPicForProduct(FileData[] fds, string UserID,string goodsid)
    {
        string ServiceURL = System.Configuration.ConfigurationManager.AppSettings["ServiceURL"].ToString();
        WebRequest request = (HttpWebRequest)WebRequest.Create(ServiceURL + "uploadMultipleFiles");
        MsMultiPartFormData form = new MsMultiPartFormData();
        form.AddFormField("devilField", "中国人");
        form.AddStreamFile("fileUpload", fds[0].FileName, fds[0].FileBytes);
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
        List<FJ> list = js.Deserialize<List<FJ>>(responseContent);
        string _url = ServiceURL + "tbbuserphoto.update";
        string jsonParam = new JavaScriptSerializer().Serialize(new
        {
            userid = UserID,
            userphotogltype = 0,
            fileList = list
        });
        var request1 = (HttpWebRequest)WebRequest.Create(_url);
        request1.Method = "POST";
        request1.ContentType = "application/json;charset=UTF-8";
        var byteData = Encoding.UTF8.GetBytes(jsonParam);
        var length = byteData.Length;
        request1.ContentLength = length;
        var writer = request1.GetRequestStream();
        writer.Write(byteData, 0, length);
        writer.Close();
        var response = (HttpWebResponse)request1.GetResponse();
        var responseString = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();

        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {


                var dt = dbc.GetEmptyDataTable("tb_b_goods_pic");
                var dr = dt.NewRow();
                dr["id"] = Guid.NewGuid();
                dr["goodsid"] = goodsid;
                dr["userid"] = UserID;
                dr["fjid"] = list[0].fjId;
                dr["status"] = 0;

                dr["adduser"] = SystemUser.CurrentUser.UserID;
                dr["updateuser"] = SystemUser.CurrentUser.UserID;
                dr["addtime"] = DateTime.Now;
                dr["updatetime"] = DateTime.Now;
                dt.Rows.Add(dr);
                dbc.InsertTable(dt);



                dbc.CommitTransaction();

            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }


        return new { fileurl = list[0].fileFullUrl, isdefault = 0, fileid = list[0].fjId };

    }

    [CSMethod("DelProductImageByPicID")]
    public bool DelProductImageByPicID(string fj_id, string userid, string goodsid)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                dbc.BeginTransaction();

                var sqlStr = @"update tb_b_goods_pic set status = 1,updatetime = getdate(),updateuser = @XGYH_ID where fjid = @fj_id and userid=@userid and goodsid=@goodsid";
               var cmd = dbc.CreateCommand(sqlStr);
                cmd.Parameters.AddWithValue("@XGYH_ID", DBNull.Value);
                cmd.Parameters.AddWithValue("@fj_id", fj_id);
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@goodsid", goodsid);
                dbc.ExecuteNonQuery(cmd);

                dbc.CommitTransaction();
                return true;
            }
            catch
            {
                dbc.RoolbackTransaction();
                return false;
            }
        }
    }




}