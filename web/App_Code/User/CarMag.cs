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
using WxPayAPI;
using Newtonsoft.Json;

/// <summary>
///JsMag 的摘要说明
/// </summary>
[CSClass("CarMag")]
public class CarMag
{
    public CarMag()
    {
        //
        //TODO: 在此处添加构造函数逻辑
        //


    }
      [CSMethod("GetCarList")]
    public object GetCarList(int pagnum, int pagesize, string cz, string sj, string sbh)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(cz.Trim()))
                {
                    where += " and " + dbc.C_Like("a.caruser", cz.Trim(), LikeStyle.LeftAndRightLike); 
                }

                if (!string.IsNullOrEmpty(sj.Trim()))
                {
                    where += " and " + dbc.C_Like("a.driverxm", sj.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(sbh.Trim()))
                {
                    where += " and " + dbc.C_Like("a.mirrornumber", sbh.Trim(), LikeStyle.LeftAndRightLike);
                }

                string str = @" select a.*,b.UserXM,b.UserName from  tb_b_car a left join  tb_b_user b on a.userid=b.UserID where  a.status=0 ";
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
      [CSMethod("IsBdBf")]
      public ToJsonMy2 IsBdBf(string username)
      {
          //构造获取openid及access_token的url
          WxPayData data = new WxPayData();
          data.SetValue("loginmobile", username);
          string url = "http://47.110.134.105:8011/BAOHUTONG_H5/findCustInfo.action?" + data.ToUrl();

          //请求url以获取数据
          string json = HttpService.Get(url);
          ToJsonMy2 ToJsonMy2 = JsonConvert.DeserializeObject<ToJsonMy2>(json);
          return ToJsonMy2;
      }

      [CSMethod("SaveCar")]
      public bool SaveCar(JSReader jsr)
      {
          if (jsr["caruser"].IsNull || jsr["caruser"].IsEmpty)
          {
              throw new Exception("车主不能为空");
          }

          using (DBConnection dbc = new DBConnection())
          {
              dbc.BeginTransaction();
              try
              {
                  if (jsr["id"].ToString() != "")
                  {
                      var userName = jsr["caruser"].ToString();
                      var cz = "";
                      DataTable czdt = dbc.ExecuteDataTable("select * from tb_b_user where userName=" + dbc.ToSqlValue(userName));
                      if (czdt.Rows.Count > 0)
                      {

                          if (!IsBdBf(userName).success)
                          {
                              throw new Exception("车主未绑定宝付账号!");
                          }
                          else
                          {
                              cz = czdt.Rows[0]["UserID"].ToString();
                          }
                      }
                      else
                      {
                          throw new Exception("车主账号不存在!");
                      }
                      var sj = "";
                      if (!jsr["drivername"].IsNull && !jsr["drivername"].IsEmpty)
                      {
                          DataTable sjdt = dbc.ExecuteDataTable("select * from tb_b_user where isdriver = 1 and clientkind = 1 and userName=" + dbc.ToSqlValue(jsr["drivername"].ToString()));
                          if (sjdt.Rows.Count > 0)
                          {
                              sj = sjdt.Rows[0]["UserID"].ToString();
                          }
                          else
                          {
                              throw new Exception("司机账号不存在!");
                          }
                      }

                      var dt = dbc.GetEmptyDataTable("tb_b_car");
                      var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                      var dr = dt.NewRow();

                      dr["id"] = new Guid(jsr["id"].ToString());
                      dr["mirrornumber"] = jsr["mirrornumber"].ToString();
                      dr["linkedunit"] = jsr["linkedunit"].ToString();
                      if (!string.IsNullOrEmpty(cz))
                      {
                          dr["userid"] = cz;
                          dr["caruser"] = userName;
                      }
                      dr["driverxm"] = jsr["driverxm"].ToString();
                      dr["drivername"] = jsr["drivername"].ToString();
                      dr["drivermemo"] = jsr["drivermemo"].ToString();
                      if (!string.IsNullOrEmpty(sj))
                      {
                          dr["driverid"] = sj;
                      }
                      dr["updateuser"] = SystemUser.CurrentUser.UserID;
                      dr["updatetime"] = DateTime.Now;
                      dt.Rows.Add(dr);
                      dbc.UpdateTable(dt, dtt);

                  }
                  else
                  {
                      throw new Exception("该车辆不存在！");
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
}
