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
///FHRSHMag 的摘要说明
/// </summary>

[CSClass("FHRSHMag")]
public class FHRSHMag
{
    string ServiceURL = System.Configuration.ConfigurationManager.AppSettings["ZYServiceURL"].ToString();

    [CSMethod("GetFHRList")]
    public object GetFHRList(int pagnum, int pagesize, string yhm, string xm, string ispass)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {

                string _url = ServiceURL + "tbbfhrapply.selectApply";
                string jsonParam = new JavaScriptSerializer().Serialize(new
                {
                    tradeCode = "tbbfhrapply.selectApply",
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
        public string mirrornumber { get; set; }
        public string caruser { get; set; }
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
        public string userxm { get; set; }
        public string carnumber { get; set; }
        public string linkedunit { get; set; }
        public string drivermemo { get; set; }
    }
    [CSMethod("GetFHRList2ToFile", 2)]
    public byte[] GetFHRList2ToFile(string yhm, string xm, string ispass)
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
                cells[0, 0].PutValue("司机名称");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("登录名");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);

                string _url = ServiceURL + "tbbfhrapply.selectApply";
                string jsonParam = new JavaScriptSerializer().Serialize(new
                {
                    tradeCode = "tbbfhrapply.selectApply",
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
                        if (list[i].param.userxm != null && list[i].param.userxm != "")
                        {
                            cells[i + 1, 0].PutValue(list[i].param.userxm);
                            cells[i + 1, 0].SetStyle(style4);
                        }
                        cells[i + 1, 1].PutValue(list[i].UserName);
                        cells[i + 1, 1].SetStyle(style4);
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

    [CSMethod("SHFHRCG")]
    public object SHFHRCG(string userId)
    {
        string _url = ServiceURL + "tbbfhrapply.pass";
            string jsonParam = new JavaScriptSerializer().Serialize(new
            {
                tradeCode = "tbbfhrapply.pass",
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
    [CSMethod("SHFHRSB")]
    public object SHFHRSB(string userId, string yj)
    {
        string _url = ServiceURL + "tbbfhrapply.reject";
        string jsonParam = new JavaScriptSerializer().Serialize(new
        {
            tradeCode = "tbbfhrapply.reject",
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


}
