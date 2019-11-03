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
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;

/// <summary>
///JsMag 的摘要说明
/// </summary>
[CSClass("CarMag")]
public class CarMag
{
    string ServiceURL = System.Configuration.ConfigurationManager.AppSettings["ServiceURL"].ToString();
    string PICServiceURL = System.Configuration.ConfigurationManager.AppSettings["PICServiceURL"].ToString();
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
                      dr["idcard"] = jsr["idcard"].ToString();
                      dr["roadnumber"] = jsr["roadnumber"].ToString();
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

      [CSMethod("GetPicList")]
      public object GetPicList(string id,int type)
      {
          using (DBConnection dbc = new DBConnection())
          {
              try
              {
                  string fjId = "";
                  string fileName = "";
                  string fileFullUrl = "";

                  string str = @"select a.carid,a.carphotoglid,a.carphotogltype,b.FJ_ID,b.FJ_MC  from tb_b_carphoto a left join tb_b_FJ b on a.FJ_ID=b.FJ_ID where a.status=0 and a.carid=@carid and a.carphotogltype=@carphotogltype
                                order by a.addtime desc";
                  SqlCommand cmd = new SqlCommand(str);
                  cmd.Parameters.Add("@carid", id);
                  cmd.Parameters.Add("@carphotogltype", type);
                  DataTable dt = dbc.ExecuteDataTable(cmd);
                  if (dt.Rows.Count > 0)
                  {
                      fjId = dt.Rows[0]["FJ_ID"].ToString();
                      fileName = dt.Rows[0]["FJ_MC"].ToString();
                      fileFullUrl = PICServiceURL + fjId + "." + fileName;
                  }
                  return new { fjId = fjId, fileName = fileName, fileFullUrl = fileFullUrl, type = type };
              }
              catch (Exception ex)
              {
                  throw ex;
              }
          }
      }

      [CSMethod("UploadPic", 1)]
      public object UploadPic(FileData[] fds, string carphotogltype,string carid)
      {
          try
          {
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
              using (DBConnection dbc = new DBConnection())
              {
                  if (list.Count > 0)
                  {
                      dbc.ExecuteDataTable("update tb_b_carphoto  set status=1 where carphotogltype=" + dbc.ToSqlValue(carphotogltype) + " and carid=" + dbc.ToSqlValue(carid));

                      DataTable dt = dbc.GetEmptyDataTable("tb_b_carphoto");
                      for (int i = 0; i < list.Count; i++)
                      {
                          DataRow dr = dt.NewRow();
                          dr["carphotoglid"] = Guid.NewGuid();
                          dr["FJ_ID"]=list[i].fjId;
                          dr["status"]=0;
                          dr["adduser"]=SystemUser.CurrentUser.UserID;
                          dr["addtime"]=DateTime.Now;
                          dr["carphotogltype"]=Convert.ToInt32(carphotogltype);
                          dr["carid"]=carid;
                          dt.Rows.Add(dr);
                      }
                      dbc.InsertTable(dt);
                  }
              }

              return list;


          }
          catch (Exception ex)
          {
              throw ex;
          }
      }

      [CSMethod("TS")]
      public object TS(string carid)
      {
          using (DBConnection dbc = new DBConnection())
          {
              try
              {
                  string str = "select * from tb_b_car where id=" + dbc.ToSqlValue(carid);
                  DataTable dt = dbc.ExecuteDataTable(str);

                  string picstr = @"select a.carid,a.carphotoglid,a.carphotogltype,b.FJ_ID,b.FJ_MC,a.addtime  from tb_b_carphoto a left join tb_b_FJ b on a.FJ_ID=b.FJ_ID where a.status=0 and a.carid="
                                + dbc.ToSqlValue(carid);
                  DataTable picdt = dbc.ExecuteDataTable(picstr);

                  if (dt.Rows.Count > 0)
                  {

                      if (dt.Rows[0]["ispushwr"].ToString() == "0")
                      {
                          string driving_license_pic_url = "";
                          DataRow[] drs7 = picdt.Select("carphotogltype=7", "addtime desc");
                          if (drs7.Length > 0)
                          {
                              driving_license_pic_url = PICServiceURL + drs7[0]["FJ_ID"].ToString() + "." + drs7[0]["FJ_MC"].ToString();
                          }
                          else
                          {
                              throw new Exception("请先上传行驶证图片再推送！");
                          }

                          string road_trans_cert_pic_url = "";
                          DataRow[] drs10 = picdt.Select("carphotogltype=10", "addtime desc");
                          if (drs10.Length > 0)
                          {
                              road_trans_cert_pic_url = PICServiceURL + drs10[0]["FJ_ID"].ToString() + "." + drs10[0]["FJ_MC"].ToString();
                          }
                          else
                          {
                              throw new Exception("请先上传运营证再推送！");
                          }

                          string _url = System.Configuration.ConfigurationManager.AppSettings["CARServiceURL"].ToString() + "ApiVehicle/edit";
                          string jsonParam = new JavaScriptSerializer().Serialize(new
                          {
                              vehicle_number = dt.Rows[0]["carnumber"],//                 车辆牌照号
                              road_trans_cert_number = dt.Rows[0]["roadnumber"],//         道路运输证字号
                              carrier_name = dt.Rows[0]["linkedunit"],//                  业户名称
                              driving_license_pic_url = driving_license_pic_url,
                              road_trans_cert_pic_url = road_trans_cert_pic_url
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

                          string id_pic1_pic_url = "";
                          DataRow[] drs2 = picdt.Select("carphotogltype=2", "addtime desc");
                          if (drs2.Length > 0)
                          {
                              id_pic1_pic_url = PICServiceURL + drs2[0]["FJ_ID"].ToString() + "." + drs2[0]["FJ_MC"].ToString();
                          }
                          else
                          {
                              throw new Exception("请先上传身份证头像面再推送！");
                          }

                          string id_pic2_pic_url = "";
                          DataRow[] drs3 = picdt.Select("carphotogltype=3", "addtime desc");
                          if (drs3.Length > 0)
                          {
                              id_pic2_pic_url = PICServiceURL + drs3[0]["FJ_ID"].ToString() + "." + drs3[0]["FJ_MC"].ToString();
                          }
                          else
                          {
                              throw new Exception("请先上传身份证头像面再推送！");
                          }

                          string driver_lic_pic_url = "";
                          DataRow[] drs9 = picdt.Select("carphotogltype=9", "addtime desc");
                          if (drs9.Length > 0)
                          {
                              driver_lic_pic_url = PICServiceURL + drs9[0]["FJ_ID"].ToString() + "." + drs9[0]["FJ_MC"].ToString();
                          }
                          else
                          {
                              throw new Exception("请先上传身份证头像面再推送！");
                          }

                          string _url_sj = System.Configuration.ConfigurationManager.AppSettings["CARServiceURL"].ToString() + "ApiDriver/edit";
                          string jsonParam_sj = new JavaScriptSerializer().Serialize(new
                          {
                              id_card_no = dt.Rows[0]["idcard"],//                  身份证号码 (已身份证号码为准)
                              real_name = dt.Rows[0]["driverxm"],//                  姓名
                              mobile = dt.Rows[0]["caruser"],//                    联系手机号
                              id_pic1_pic_url = id_pic1_pic_url,//             身份证头像面
                              id_pic2_pic_url = id_pic2_pic_url,//             身份证国徽面
                              driver_lic_pic_url = driver_lic_pic_url//          驾驶证图片
                          });
                          var request_sj = (HttpWebRequest)WebRequest.Create(_url_sj);
                          request_sj.Method = "POST";
                          request_sj.ContentType = "application/json;charset=UTF-8";
                          var byteData_sj = Encoding.UTF8.GetBytes(jsonParam_sj);
                          var length_sj = byteData.Length;
                          request_sj.ContentLength = length_sj;
                          var writer_sj = request_sj.GetRequestStream();
                          writer_sj.Write(byteData, 0, length_sj);
                          writer_sj.Close();
                          var response_sj = (HttpWebResponse)request_sj.GetResponse();
                          var responseString_sj = new StreamReader(response_sj.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();


                          JObject jo = (JObject)JsonConvert.DeserializeObject(responseString);
                          JObject jo_sj = (JObject)JsonConvert.DeserializeObject(responseString_sj);
                          try
                          {
                              if (Convert.ToBoolean(jo_sj["success"].ToString()) && Convert.ToBoolean(jo["success"].ToString()))
                              {
                                  DataTable odt = dbc.GetEmptyDataTable("tb_b_car");
                                  DataTableTracker odtt = new DataTableTracker(odt);
                                  DataRow odr = odt.NewRow();
                                  odr["id"] = carid;
                                  odr["ispushwr"] = 0;
                                  odt.Rows.Add(odr);
                                  dbc.UpdateTable(odt, odtt);

                                  dbc.CommitTransaction();
                                  return true;
                              }
                              else
                              {
                                  throw new Exception("推送失败");
                              }
                          }
                          catch (Exception ex)
                          {
                              throw new Exception(jo["details"].ToString() + jo_sj["details"].ToString());
                          }
                      }
                      else
                      {

                          string driving_license_pic_url = "";
                          DataRow[] drs7 = picdt.Select("carphotogltype=7", "addtime desc");
                          if (drs7.Length > 0)
                          {
                              driving_license_pic_url = PICServiceURL + drs7[0]["FJ_ID"].ToString() + "." + drs7[0]["FJ_MC"].ToString();
                          }
                          else
                          {
                              throw new Exception("请先上传行驶证图片再推送！");
                          }

                          string road_trans_cert_pic_url = "";
                          DataRow[] drs10 = picdt.Select("carphotogltype=10", "addtime desc");
                          if (drs10.Length > 0)
                          {
                              road_trans_cert_pic_url = PICServiceURL + drs10[0]["FJ_ID"].ToString() + "." + drs10[0]["FJ_MC"].ToString();
                          }
                          else
                          {
                              throw new Exception("请先上传运营证再推送！");
                          }

                          string _url = System.Configuration.ConfigurationManager.AppSettings["CARServiceURL"].ToString() + "ApiVehicle/add";
                          string jsonParam = new JavaScriptSerializer().Serialize(new
                          {
                              vehicle_number = dt.Rows[0]["carnumber"],//                 车辆牌照号
                              road_trans_cert_number = dt.Rows[0]["roadnumber"],//         道路运输证字号
                              carrier_name = dt.Rows[0]["linkedunit"],//                  业户名称
                              driving_license_pic_url = driving_license_pic_url,
                              road_trans_cert_pic_url = road_trans_cert_pic_url
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

                          string id_pic1_pic_url = "";
                          DataRow[] drs2 = picdt.Select("carphotogltype=2", "addtime desc");
                          if (drs2.Length > 0)
                          {
                              id_pic1_pic_url = PICServiceURL + drs2[0]["FJ_ID"].ToString() + "." + drs2[0]["FJ_MC"].ToString();
                          }
                          else
                          {
                              throw new Exception("请先上传身份证头像面再推送！");
                          }

                          string id_pic2_pic_url = "";
                          DataRow[] drs3 = picdt.Select("carphotogltype=3", "addtime desc");
                          if (drs3.Length > 0)
                          {
                              id_pic2_pic_url = PICServiceURL + drs3[0]["FJ_ID"].ToString() + "." + drs3[0]["FJ_MC"].ToString();
                          }
                          else
                          {
                              throw new Exception("请先上传身份证头像面再推送！");
                          }

                          string driver_lic_pic_url = "";
                          DataRow[] drs9 = picdt.Select("carphotogltype=9", "addtime desc");
                          if (drs9.Length > 0)
                          {
                              driver_lic_pic_url = PICServiceURL + drs9[0]["FJ_ID"].ToString() + "." + drs9[0]["FJ_MC"].ToString();
                          }
                          else
                          {
                              throw new Exception("请先上传身份证头像面再推送！");
                          }

                          string _url_sj = System.Configuration.ConfigurationManager.AppSettings["CARServiceURL"].ToString() + "ApiDriver/add";
                          string jsonParam_sj = new JavaScriptSerializer().Serialize(new
                          {
                              id_card_no = dt.Rows[0]["idcard"],//                  身份证号码 (已身份证号码为准)
                              real_name = dt.Rows[0]["driverxm"],//                  姓名
                              mobile = dt.Rows[0]["caruser"],//                    联系手机号
                              id_pic1_pic_url = id_pic1_pic_url,//             身份证头像面
                              id_pic2_pic_url = id_pic2_pic_url,//             身份证国徽面
                              driver_lic_pic_url = driver_lic_pic_url//          驾驶证图片
                          });
                          var request_sj = (HttpWebRequest)WebRequest.Create(_url_sj);
                          request_sj.Method = "POST";
                          request_sj.ContentType = "application/json;charset=UTF-8";
                          var byteData_sj = Encoding.UTF8.GetBytes(jsonParam_sj);
                          var length_sj = byteData.Length;
                          request_sj.ContentLength = length_sj;
                          var writer_sj = request_sj.GetRequestStream();
                          writer_sj.Write(byteData, 0, length_sj);
                          writer_sj.Close();
                          var response_sj = (HttpWebResponse)request_sj.GetResponse();
                          var responseString_sj = new StreamReader(response_sj.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();


                          JObject jo = (JObject)JsonConvert.DeserializeObject(responseString);
                          JObject jo_sj = (JObject)JsonConvert.DeserializeObject(responseString_sj);
                          try
                          {
                              if (Convert.ToBoolean(jo_sj["success"].ToString()) && Convert.ToBoolean(jo["success"].ToString()))
                              {
                                  DataTable odt = dbc.GetEmptyDataTable("tb_b_car");
                                  DataTableTracker odtt = new DataTableTracker(odt);
                                  DataRow odr = odt.NewRow();
                                  odr["id"] = carid;
                                  odr["ispushwr"] = 0;
                                  odt.Rows.Add(odr);
                                  dbc.UpdateTable(odt, odtt);

                                  dbc.CommitTransaction();
                                  return true;
                              }
                              else
                              {
                                  throw new Exception("推送失败");
                              }
                          }
                          catch (Exception ex)
                          {
                              //if (jo["msg"].ToString() == "车辆牌照号已存在!")
                              //{
                              //    dbc.ExecuteDataTable("update tb_b_car  set ispushwr=0 where  id=" + dbc.ToSqlValue(carid));
                              //}
                              throw new Exception(jo["msg"].ToString() + jo_sj["msg"].ToString());
                          }
                      }

                  }

                  return true;
              }
              catch (Exception ex)
              {
                  throw ex;
              }
          }
      }
}
