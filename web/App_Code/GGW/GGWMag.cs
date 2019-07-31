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
/// <summary>
///GGWMag 的摘要说明
/// </summary>

[CSClass("GGWMag")]
public class GGWMag
{
    string ServiceURL=System.Configuration.ConfigurationManager.AppSettings["ServiceURL"].ToString();

    [CSMethod("UploadPic", 1)]
    public object UploadPic(FileData[] fds)
    {
        try
        {
            WebRequest request = (HttpWebRequest)WebRequest.Create(ServiceURL + "uploadAdver");
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
            return list;

           
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    
    [CSMethod("GetGGWList")]
    public object GetGGWList(int zxd, string screen)
    {
        string _url = ServiceURL + "tbbadvert.selectAdvert";
        string jsonParam = new JavaScriptSerializer().Serialize(new
        {
            tradeCode = "tbbadvert.selectAdvert",
            zxd = zxd,
            screen = screen,
            status=0
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

    [CSMethod("SaveGGW")]
    public void SaveGGW(int zxd, string screen, string liststr)
    {
        JavaScriptSerializer js = new JavaScriptSerializer();
        List<FJ> list = js.Deserialize<List<FJ>>(liststr);
        string _url = ServiceURL + "tbbadvert.update";
        string jsonParam = new JavaScriptSerializer().Serialize(new
        {
            zxd = zxd,  // 1-专线，0-三方，2-司机
            screen = screen, //分辨率
            status = 0,  // 默认状态0
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
    }
}
