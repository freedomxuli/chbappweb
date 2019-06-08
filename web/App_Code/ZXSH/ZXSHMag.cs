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
using Newtonsoft.Json;
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

    [CSMethod("GetZXListToFile", 2)]
    public byte[] GetZXListToFile(string yhm, string xm,string isrelease)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                Workbook workbook = new Workbook(); //工作簿
                Worksheet sheet = workbook.Worksheets[0]; //工作表
                Cells cells = sheet.Cells;//单元格

                //样式2
                Style style2 = workbook.Styles[workbook.Styles.Add()];
                style2.HorizontalAlignment = TextAlignmentType.Left;//文字居中
                style2.Font.Name = "宋体";//文字字体
                style2.Font.Size = 14;//文字大小
                style2.Font.IsBold = true;//粗体
                style2.IsTextWrapped = true;//单元格内容自动换行
                style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; //应用边界线 左边界线
                style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin; //应用边界线 右边界线
                style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin; //应用边界线 上边界线
                style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; //应用边界线 下边界线
                style2.IsLocked = true;

                //样式3
                Style style4 = workbook.Styles[workbook.Styles.Add()];
                style4.HorizontalAlignment = TextAlignmentType.Left;//文字居中
                style4.Font.Name = "宋体";//文字字体
                style4.Font.Size = 11;//文字大小
                style4.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                style4.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style4.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style4.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;


                cells.SetRowHeight(0, 20);
                cells[0, 0].PutValue("专线名称");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("登录名");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("电话");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("线路");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("是否可以自行发布运费券");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("审核时间");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);
                cells[0, 6].PutValue("开放次数");
                cells[0, 6].SetStyle(style2);
                cells.SetColumnWidth(6, 20);

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
  where a.IsSHPass=1 and a.ClientKind=1 "+ where +@" order by a.AddTime desc,a.UserName,a.UserXM";

                //开始取分页数据
                System.Data.DataTable dt = dbc.ExecuteDataTable(str);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["UserXM"]);
                    cells[i + 1, 0].SetStyle(style4);
                    cells[i + 1, 1].PutValue(dt.Rows[i]["UserName"]);
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["UserTel"] != null && dt.Rows[i]["UserTel"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(dt.Rows[i]["UserTel"]);
                    }
                    cells[i + 1, 2].SetStyle(style4);

                    var xl="";
                    if (dt.Rows[i]["FromRoute"] != null && dt.Rows[i]["FromRoute"].ToString() != ""){
                        xl += dt.Rows[i]["FromRoute"].ToString();
                    }
                    if (dt.Rows[i]["ToRoute"] != null && dt.Rows[i]["ToRoute"].ToString() != ""){
                        xl += "─" + dt.Rows[i]["ToRoute"].ToString();
                    }
                    cells[i + 1, 3].PutValue(xl);
                    cells[i + 1, 3].SetStyle(style4);
                     var can="";
                     if (dt.Rows[i]["IsCanRelease"] != null && dt.Rows[i]["IsCanRelease"].ToString() != "")
                     {
                         if (Convert.ToInt32(dt.Rows[i]["IsCanRelease"].ToString()) == 1)
                         {
                             can += "可以";
                         }
                         else
                         {
                             can += "不可以";
                         }
                     }
                     cells[i + 1, 4].PutValue(can);
                    cells[i + 1, 4].SetStyle(style4);
                    if (dt.Rows[i]["canReleaseTime"] != null && dt.Rows[i]["canReleaseTime"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(Convert.ToDateTime(dt.Rows[i]["canReleaseTime"]).ToString("yyyy-MM-dd"));
                    }
                    cells[i + 1, 5].SetStyle(style4);
                    if (dt.Rows[i]["fqcs"] != null && dt.Rows[i]["fqcs"].ToString() != "")
                    {
                        cells[i + 1, 6].PutValue(dt.Rows[i]["fqcs"]);
                    }
                    cells[i + 1, 6].SetStyle(style4);
                }

                MemoryStream ms = workbook.SaveToStream();
                byte[] bt = ms.ToArray();
                return bt;
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

    public struct ToJsonMy2
    {
        public pagination pagination { get; set; }
        public zxlist[] list;
    }

    public struct pagination
    {
        public string current{ get; set; }
        public string pageSize{ get; set; }
        public string total{ get; set; }
        public string totalPage{ get; set; }
    }


    public struct zxlist
    {
        public string Address { get; set; }
        public string ClientKind { get; set; }
        public string FromRoute { get; set; }
        public string IsCanRelease { get; set; }
        public string Points { get; set; }
        public string ToRoute { get; set; }
        public string UserContent { get; set; }
        public string UserName { get; set; }
        public string UserTel { get; set; }
        public string UserXM { get; set; }
        public string addtime { get; set; }
        public string ewmbs { get; set; }
        public string id { get; set; }
        public string status { get; set; }
        public param param;
    }
    public struct param
    {
        public string address { get; set; }
        public string fromroute { get; set; }
        public string returnSelect { get; set; }
        public string toroute { get; set; }
        public string tradeCode { get; set; }
        public string usercontent { get; set; }
        public string userid { get; set; }
        public string usertel { get; set; }
    }
    [CSMethod("GetZXList2ToFile", 2)]
    public byte[] GetZXList2ToFile(string yhm, string xm, string ispass)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                Workbook workbook = new Workbook(); //工作簿
                Worksheet sheet = workbook.Worksheets[0]; //工作表
                Cells cells = sheet.Cells;//单元格

                //样式2
                Style style2 = workbook.Styles[workbook.Styles.Add()];
                style2.HorizontalAlignment = TextAlignmentType.Left;//文字居中
                style2.Font.Name = "宋体";//文字字体
                style2.Font.Size = 14;//文字大小
                style2.Font.IsBold = true;//粗体
                style2.IsTextWrapped = true;//单元格内容自动换行
                style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; //应用边界线 左边界线
                style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin; //应用边界线 右边界线
                style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin; //应用边界线 上边界线
                style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; //应用边界线 下边界线
                style2.IsLocked = true;

                //样式3
                Style style4 = workbook.Styles[workbook.Styles.Add()];
                style4.HorizontalAlignment = TextAlignmentType.Left;//文字居中
                style4.Font.Name = "宋体";//文字字体
                style4.Font.Size = 11;//文字大小
                style4.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                style4.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style4.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style4.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;


                cells.SetRowHeight(0, 20);
                cells[0, 0].PutValue("专线名称");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("登录名");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("电话");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("线路");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("简介");           
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("是否通过");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);

                string _url = ServiceURL + "tbbuserapply.selectApply";
                string jsonParam = new JavaScriptSerializer().Serialize(new
                {
                    tradeCode = "tbbuserapply.selectApply",
                    status = ispass,
                    userid = "",
                    username = yhm,
                    userxm = xm,
                    //currentPage = 1,
                    //pageSize = 10,
                    closePagination = true
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

                ToJsonMy2 ToJsonMy2 = JsonConvert.DeserializeObject<ToJsonMy2>(responseString);    //将json数据转化为对象类型并赋值给list
                zxlist[] list = ToJsonMy2.list;
                if (list.Length > 0)
                {
                    for (int i = 0; i < list.Length; i++)
                    {
                        cells[i + 1, 0].PutValue(list[i].UserXM);
                        cells[i + 1, 0].SetStyle(style4);
                        cells[i + 1, 1].PutValue(list[i].UserName);
                        cells[i + 1, 1].SetStyle(style4);
                        if (list[i].param.usertel != null && list[i].param.usertel != "")
                        {
                            cells[i + 1, 2].PutValue(list[i].param.usertel);
                        }
                        cells[i + 1, 2].SetStyle(style4);

                        var xl = "";
                        if (list[i].param.fromroute != null && list[i].param.fromroute != "")
                        {
                            xl += list[i].param.fromroute;
                        }
                        if (list[i].param.toroute != null && list[i].param.toroute != "")
                        {
                            xl += "─" + list[i].param.toroute;
                        }
                        cells[i + 1, 3].PutValue(xl);
                        cells[i + 1, 3].SetStyle(style4);

                        cells[i + 1, 4].PutValue(list[i].param.usercontent);
                        cells[i + 1, 4].SetStyle(style4);

                        var shzt = "";


                        if (list[i].status != null && list[i].status != "")
                        {
                            if (Convert.ToInt32(list[i].status) == 1)
                            {
                                shzt = "通过";
                            }
                            else if (Convert.ToInt32(list[i].status) == 2)
                            {
                                shzt = "拒绝";
                            }
                            else if (Convert.ToInt32(list[i].status) == 0)
                            {
                                shzt = "待审核";
                            }
                        }
                        else
                        {
                            shzt = "待审核";
                        }
                        cells[i + 1, 5].PutValue(shzt);
                        cells[i + 1, 5].SetStyle(style4);
                    }
                }

                MemoryStream ms = workbook.SaveToStream();
                byte[] bt = ms.ToArray();
                return bt;
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
                    where+=" and "+dbc.C_Like("a.SaleRecordUserXM",userxm,LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(isVerifyType))
                {
                    where += " and " + dbc.C_EQ("a.SaleRecordVerifyType", Convert.ToInt32(isVerifyType));
                }
                string str = @"select a.*,b.ZXSaleListCitys,c.ZdMxMc as ProduceLx, d.ZdMxMc as PackLx,e.ZdMxMc as Fc,f.ZdMxMc as ZhLx,g.ZdMxMc as PhLx,h.FromRoute,h.ToRoute
 from  tb_b_salerecord a left join tb_b_zxsalelist b on a.SaleRecordID=b.SaleRecordID
 left join tb_b_zdmx c on b.ZXSaleListProduceLx=c.ZdMxID
  left join tb_b_zdmx d on b.ZXSaleListPackLx=d.ZdMxID
    left join tb_b_zdmx e on b.ZXSaleListFc=e.ZdMxID
	    left join tb_b_zdmx f on b.ZXSaleListZhLx=f.ZdMxID
		    left join tb_b_zdmx g on b.ZXSaleListPhLx=g.ZdMxID
            left join tb_b_user h on a.SaleRecordUserID=h.UserID
   where a.status=0 and a.SaleRecordLX!=0  and a.SaleRecordVerifyType!=3 
   and b.status=0 " + where +@"  order by a.addtime desc";
                System.Data.DataTable dtPage = dbc.GetPagedDataTable(str, pagesize, ref cp, out ac);
                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }


    [CSMethod("getZFQListToFile", 2)]
    public byte[] getZFQListToFile(string userxm, string isVerifyType)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                Workbook workbook = new Workbook(); //工作簿
                Worksheet sheet = workbook.Worksheets[0]; //工作表
                Cells cells = sheet.Cells;//单元格

                //样式2
                Style style2 = workbook.Styles[workbook.Styles.Add()];
                style2.HorizontalAlignment = TextAlignmentType.Left;//文字居中
                style2.Font.Name = "宋体";//文字字体
                style2.Font.Size = 14;//文字大小
                style2.Font.IsBold = true;//粗体
                style2.IsTextWrapped = true;//单元格内容自动换行
                style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin; //应用边界线 左边界线
                style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin; //应用边界线 右边界线
                style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin; //应用边界线 上边界线
                style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin; //应用边界线 下边界线
                style2.IsLocked = true;

                //样式3
                Style style4 = workbook.Styles[workbook.Styles.Add()];
                style4.HorizontalAlignment = TextAlignmentType.Left;//文字居中
                style4.Font.Name = "宋体";//文字字体
                style4.Font.Size = 11;//文字大小
                style4.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                style4.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style4.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style4.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;


                cells.SetRowHeight(0, 20);
                cells[0, 0].PutValue("专线名称");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("线路");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("目的地");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("运费券");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("折扣");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("有效时间");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);
                cells[0, 6].PutValue("开放时间");
                cells[0, 6].SetStyle(style2);
                cells.SetColumnWidth(6, 20);
                cells[0, 7].PutValue("货物类型");
                cells[0, 7].SetStyle(style2);
                cells.SetColumnWidth(7, 20);
                cells[0, 8].PutValue("重物/泡货类型");
                cells[0, 8].SetStyle(style2);
                cells.SetColumnWidth(8, 20);
                cells[0, 9].PutValue("包装要求");
                cells[0, 9].SetStyle(style2);
                cells.SetColumnWidth(9, 20);
                cells[0, 10].PutValue("发车时间");
                cells[0, 10].SetStyle(style2);
                cells.SetColumnWidth(10, 20);
                cells[0, 11].PutValue("审核装态");
                cells[0, 11].SetStyle(style2);
                cells.SetColumnWidth(11, 20);
                cells[0, 12].PutValue("审核时间");
                cells[0, 12].SetStyle(style2);
                cells.SetColumnWidth(12, 20);

                string where = "";
                if (!string.IsNullOrEmpty(userxm))
                {
                    where += " and " + dbc.C_Like("a.SaleRecordUserXM", userxm, LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(isVerifyType))
                {
                    where += " and " + dbc.C_EQ("a.SaleRecordVerifyType", Convert.ToInt32(isVerifyType));
                }
                string str = @"select a.*,b.ZXSaleListCitys,c.ZdMxMc as ProduceLx, d.ZdMxMc as PackLx,e.ZdMxMc as Fc,f.ZdMxMc as ZhLx,g.ZdMxMc as PhLx,h.FromRoute,h.ToRoute
 from  tb_b_salerecord a left join tb_b_zxsalelist b on a.SaleRecordID=b.SaleRecordID
 left join tb_b_zdmx c on b.ZXSaleListProduceLx=c.ZdMxID
  left join tb_b_zdmx d on b.ZXSaleListPackLx=d.ZdMxID
    left join tb_b_zdmx e on b.ZXSaleListFc=e.ZdMxID
	    left join tb_b_zdmx f on b.ZXSaleListZhLx=f.ZdMxID
		    left join tb_b_zdmx g on b.ZXSaleListPhLx=g.ZdMxID
            left join tb_b_user h on a.SaleRecordUserID=h.UserID
   where a.status=0 and a.SaleRecordLX!=0  and a.SaleRecordVerifyType!=3 
   and b.status=0 " + where + @"  order by a.addtime desc";
                System.Data.DataTable dt = dbc.ExecuteDataTable(str);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["SaleRecordUserXM"]);
                    cells[i + 1, 0].SetStyle(style4);

                    var xl = "";
                    if (dt.Rows[i]["FromRoute"] != null && dt.Rows[i]["FromRoute"].ToString() != "")
                    {
                        xl += dt.Rows[i]["FromRoute"].ToString();
                    }
                    if (dt.Rows[i]["ToRoute"] != null && dt.Rows[i]["ToRoute"].ToString() != "")
                    {
                        xl += "─" + dt.Rows[i]["ToRoute"].ToString();
                    }
                    cells[i + 1, 1].PutValue(xl);
                    cells[i + 1, 1].SetStyle(style4);
                    cells[i + 1, 2].PutValue(dt.Rows[i]["ZXSaleListCitys"]);
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["SaleRecordPoints"] != null && dt.Rows[i]["SaleRecordPoints"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(dt.Rows[i]["SaleRecordPoints"]);
                    }
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["SaleRecordDiscount"] != null && dt.Rows[i]["SaleRecordDiscount"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(dt.Rows[i]["SaleRecordDiscount"]);
                    }
                    cells[i + 1, 4].SetStyle(style4);
                    if (dt.Rows[i]["ValidHour"] != null && dt.Rows[i]["ValidHour"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(dt.Rows[i]["ValidHour"]);
                    }
                    cells[i + 1, 5].SetStyle(style4);
                    if (dt.Rows[i]["SaleRecordTime"] != null && dt.Rows[i]["SaleRecordTime"].ToString() != "")
                    {
                        cells[i + 1, 6].PutValue(Convert.ToDateTime(dt.Rows[i]["SaleRecordTime"]).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    cells[i + 1, 6].SetStyle(style4);
                    if (dt.Rows[i]["ProduceLx"] != null && dt.Rows[i]["ProduceLx"].ToString() != "")
                    {
                        cells[i + 1, 7].PutValue(dt.Rows[i]["ProduceLx"]);
                    }
                    cells[i + 1, 7].SetStyle(style4);
                    var lx = "";
                    if (dt.Rows[i]["ProduceLx"] != null && dt.Rows[i]["ProduceLx"].ToString() != "")
                    {
                        if (dt.Rows[i]["ProduceLx"].ToString() == "重货")
                        {
                            if (dt.Rows[i]["ZhLx"] != null && dt.Rows[i]["ZhLx"].ToString() != "")
                            {
                                lx = dt.Rows[i]["ZhLx"].ToString();
                            }
                        }
                        else if (dt.Rows[i]["ProduceLx"].ToString() == "泡货")
                        {
                            if (dt.Rows[i]["PhLx"] != null && dt.Rows[i]["PhLx"].ToString() != "")
                            {
                                lx = dt.Rows[i]["PhLx"].ToString();
                            }
                        }
                    }
                    cells[i + 1, 8].PutValue(lx);
                    cells[i + 1, 8].SetStyle(style4);
                    if (dt.Rows[i]["PackLx"] != null && dt.Rows[i]["PackLx"].ToString() != "")
                    {
                        cells[i + 1, 9].PutValue(dt.Rows[i]["PackLx"]);
                    }
                    cells[i + 1, 9].SetStyle(style4);

                    if (dt.Rows[i]["Fc"] != null && dt.Rows[i]["Fc"].ToString() != "")
                    {
                        cells[i + 1, 10].PutValue(dt.Rows[i]["Fc"]);
                    }
                    cells[i + 1, 10].SetStyle(style4);

                    var shzt = "";


                    if (dt.Rows[i]["SaleRecordVerifyType"] != null && dt.Rows[i]["SaleRecordVerifyType"].ToString() != "")
                    {
                        if (Convert.ToInt32(dt.Rows[i]["SaleRecordVerifyType"].ToString()) == 1)
                        {
                            shzt = "通过";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["SaleRecordVerifyType"].ToString()) == 2)
                        {
                            shzt = "拒绝";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["SaleRecordVerifyType"].ToString()) == 0)
                        {
                            shzt = "待审核";
                        }
                    }
                    else
                    {
                        shzt = "待审核";
                    }
                    cells[i + 1, 11].PutValue(shzt);
                    cells[i + 1, 11].SetStyle(style4);
                    if (dt.Rows[i]["SaleRecordVerifyTime"] != null && dt.Rows[i]["SaleRecordVerifyTime"].ToString() != "")
                    {
                        cells[i + 1, 12].PutValue(Convert.ToDateTime(dt.Rows[i]["SaleRecordVerifyTime"]).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    cells[i + 1, 12].SetStyle(style4);
                }

                MemoryStream ms = workbook.SaveToStream();
                byte[] bt = ms.ToArray();
                return bt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }

    [CSMethod("ZFQSH")]
    public object ZFQSH(string SaleRecordID, int issh,string thyj)
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
                        sr1["SaleRecordTime"] = sdt.Rows[0]["SaleRecordTime"];
                        dt1.Rows.Add(sr1);
                        dbc.UpdateTable(dt1, dtt1);

                        userid=sdt1.Rows[0]["UserID"].ToString();


                        string str2 = "select * from tb_b_user where userid="+dbc.ToSqlValue(userid);
                        DataTable udt = dbc.ExecuteDataTable(str2);
                        if (udt.Rows.Count > 0)
                        {
                            var username = udt.Rows[0]["UserName"];

                            if (issh == 1)
                            {
                                string _url = ServiceURL + "sendSms/releasePass";
                                string jsonParam = new JavaScriptSerializer().Serialize(new
                                {
                                    username = udt.Rows[0]["UserName"],
                                    userxm = udt.Rows[0]["UserXM"]
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
                            }
                            else
                            {
                                string _url = ServiceURL + "sendSms/releaseReject";
                                string jsonParam = new JavaScriptSerializer().Serialize(new
                                {
                                    username = udt.Rows[0]["UserName"],
                                    userxm = udt.Rows[0]["UserXM"],
                                    memo = thyj
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
                            }
                        }
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
