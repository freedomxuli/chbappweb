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
using System.Security.Cryptography;
using System.Net;
using System.Web.Script.Serialization;

/// <summary>
///KFGMMag 的摘要说明
/// </summary>
public class sanfang
{
     public string sanfanguserid { get; set; }
}
[CSClass("KFGMMag")]
public class KFGMMag
{
    public KFGMMag()
    {
        //
        //TODO: 在此处添加构造函数逻辑
        //
    }
    string ServiceURL = System.Configuration.ConfigurationManager.AppSettings["ServiceURL"].ToString();
    [CSMethod("getHisSale")]
    public DataTable getHisSale(string UserID)
    {
        using (DBConnection dbc = new DBConnection())
        {
            string sql = @"select isNULL(a.points,0)+isnull(b.points,0) as points,a.UserID,a.PlatToSaleId, c.UserXM from 
(select Points as points,UserID,PlatToSaleId from tb_b_plattosale where  UserID=@UserID and status=0 and pointkind=0 ) a left join (
select sum(Points) as points,SaleUserID from [tb_b_order] where  [SaleUserID]=@UserID and ZhiFuZT=0 and status=0
group by SaleUserID) b  on a.UserID=b.SaleUserID 
 left join tb_b_user c on a.UserID = c.UserID";
            SqlCommand cmd = dbc.CreateCommand(sql);
            cmd.Parameters.Add("@UserID", UserID);
            DataTable dt = dbc.ExecuteDataTable(cmd);
            return dt;
        }
    }

    [CSMethod("GetPointSaleXM")]
    public DataTable GetPointSaleXM(string PlatToSaleId)
    {
        using (var dbc = new DBConnection())
        {
            string sql = "select * from tb_b_plattosale where PlatToSaleId = @PlatToSaleId";
            SqlCommand cmd = dbc.CreateCommand(sql);
            cmd.Parameters.Add("@PlatToSaleId", PlatToSaleId);
            DataTable dt = dbc.ExecuteDataTable(cmd);
            return dt;
        }
    }

    [CSMethod("GetList")]
    public object GetList(int pagnum, int pagesize,string yhm)
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
                    where += " and " + dbc.C_Like("b.UserXM", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                string str = @"  select a.*,b.UserName,b.UserXM from tb_b_platpoints a left join tb_b_user b on a.UserID=b.UserID
                        where a.status=0 and b.UserXM is not null";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.Points ", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public string GetRandom()
    {
        string str = "";
        for (int i = 0; i < 6; i++)
        {
            Random rd = new Random(Guid.NewGuid().GetHashCode());
            str += rd.Next(0, 10).ToString();
        }
        str += DateTime.Now.ToString("yyyyMMddHHmmssfff");
        return str;
    }

    [CSMethod("SaveKFGM")]
    public bool SaveUser(JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                var time = DateTime.Now;

                var kfsj = DateTime.Now.ToString();

                if (jsr["kfsj_rq"] != null && jsr["kfsj_rq"].ToString() != "")
                {
                    kfsj = Convert.ToDateTime(jsr["kfsj_rq"].ToString()).ToString("yyyy-MM-dd");

                    if (jsr["kfsj_hour"] != null && jsr["kfsj_hour"].ToString() != "")
                    {
                        kfsj += " " + jsr["kfsj_hour"].ToString() + ":00:00";
                    }
                }
                var PlatPointId = jsr["PlatPointId"].ToString();
                string str = "select * from tb_b_platpoints where PlatPointId=" + dbc.ToSqlValue(PlatPointId);
                DataTable pdt = dbc.ExecuteDataTable(str);

                if (pdt.Rows.Count>0)
                {
                    if (Convert.ToDecimal(pdt.Rows[0]["Points"].ToString()) - Convert.ToDecimal(jsr["points"].ToString()) < 0)
                    {
                        throw new Exception("开放购买的运费券不得超过其所有运费券！");
                    }
                    else
                    {
                        var userid = pdt.Rows[0]["UserID"].ToString();
                        str = "select * from tb_b_user where UserID='" + userid + "'";
                        DataTable udt = dbc.ExecuteDataTable(str);

                        var wlmc = "";
                        if (udt.Rows.Count > 0)
                        {
                            wlmc = udt.Rows[0]["UserXM"].ToString();
                        }

                        var SaleRecordID = Guid.NewGuid().ToString();

                        var logdt = dbc.GetEmptyDataTable("tb_b_salerecord");
                        var logdr = logdt.NewRow();
                        logdr["SaleRecordID"] = SaleRecordID;
                        logdr["SaleRecordCode"] = GetRandom();
                        logdr["SaleRecordUserID"] = userid;
                        logdr["SaleRecordUserXM"] = wlmc;
                        logdr["SaleRecordTime"] = kfsj;
                        logdr["SaleRecordPoints"] = Convert.ToDecimal(jsr["points"].ToString());
                        logdr["SaleRecordLX"] = 0;
                        logdr["Status"] = 0;
                        logdr["adduser"] = SystemUser.CurrentUser.UserID;
                        logdr["addtime"] = time;
                        logdr["updateuser"] = SystemUser.CurrentUser.UserID;
                        logdr["updatetime"] = time;
                        logdr["SaleRecordBelongID"] = "6E72B59D-BEC6-4835-A66F-8BC70BD82FE9";
                        logdr["ValidHour"] = Convert.ToDecimal(jsr["validHour"].ToString());
                        logdr["SaleRecordDiscount"] = Convert.ToDecimal(jsr["discount"].ToString());
                        logdt.Rows.Add(logdr);
                        dbc.InsertTable(logdt);


                        var saledt = dbc.GetEmptyDataTable("tb_b_plattosale");
                        DataTableTracker saledtt = new DataTableTracker(saledt);
                        if (string.IsNullOrEmpty(jsr["PlatToSaleId"].ToString()))
                        {

                            var saledr = saledt.NewRow();
                            saledr["PlatToSaleId"] = Guid.NewGuid().ToString();
                            saledr["UserID"] = userid;
                            saledr["points"] = Convert.ToDecimal(jsr["points"].ToString());
                            saledr["addtime"] = DateTime.Now;
                            saledr["status"] = 0;
                            saledr["discount"] = Convert.ToDecimal(jsr["discount"].ToString());
                            saledr["discountmemo"] = jsr["discountmemo"].ToString();
                            saledr["pointkind"] = 0;
                            saledr["SaleRecordID"] = SaleRecordID;
                            saledr["belongID"] = "6E72B59D-BEC6-4835-A66F-8BC70BD82FE9";
                            saledr["validHour"] = Convert.ToDecimal(jsr["validHour"].ToString());
                            saledr["SaleRecordTime"] = kfsj;
                            saledt.Rows.Add(saledr);
                            dbc.InsertTable(saledt);
                        }
                        else
                        {
                            str = "select points from tb_b_plattosale where PlatToSaleId = " + dbc.ToSqlValue(jsr["PlatToSaleId"].ToString());
                            decimal sale_new_points = Convert.ToDecimal(dbc.ExecuteScalar(str));

                            if (sale_new_points > 0)
                            {
                                throw new Exception("线上还有剩余" + sale_new_points + "运费券没卖完！不能开放！");
                            }
                            else
                            {

                                var saledr = saledt.NewRow();
                                saledr["PlatToSaleId"] = jsr["PlatToSaleId"].ToString();
                                saledr["points"] = Convert.ToDecimal(jsr["points"].ToString());
                                saledr["addtime"] = DateTime.Now;
                                saledr["status"] = 0;
                                saledr["discount"] = Convert.ToDecimal(jsr["discount"].ToString());
                                saledr["discountmemo"] = jsr["discountmemo"].ToString();
                                saledr["pointkind"] = 0;
                                saledr["SaleRecordID"] = SaleRecordID;
                                saledr["belongID"] = "6E72B59D-BEC6-4835-A66F-8BC70BD82FE9";
                                saledr["validHour"] = Convert.ToDecimal(jsr["validHour"].ToString());
                                saledr["SaleRecordTime"] = kfsj;
                                saledt.Rows.Add(saledr);
                                dbc.UpdateTable(saledt, saledtt);
                            }
                        }

                        var dt = dbc.GetEmptyDataTable("tb_b_platpoints");
                        var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                        var dr = dt.NewRow();
                        dr["PlatPointId"] = PlatPointId;
                        var point = Convert.ToDecimal(pdt.Rows[0]["Points"].ToString()) - Convert.ToDecimal(jsr["points"].ToString());
                        dr["Points"] = point;
                        dt.Rows.Add(dr);
                        dbc.UpdateTable(dt, dtt);

                        string sql = @"select distinct b.UserID,b.OpenID from tb_b_user_gz a left join tb_b_user b on a.UserId=b.UserID
                        where a.GZUserID=" + dbc.ToSqlValue(userid) + " and b.OpenID is not null";
                        DataTable gzdt = dbc.ExecuteDataTable(sql);

                        if (gzdt.Rows.Count > 0)
                        {
                            for (int i = 0; i < gzdt.Rows.Count; i++)
                            {
                                try
                                {
                                    WebServiceApp(System.Configuration.ConfigurationManager.AppSettings["ServiceURL"].ToString(), "pcReleaseNotify", "userid=" + userid);
                                    //new Handler().SendWeText(gzdt.Rows[i]["OpenID"].ToString(), "您关注的" + wlmc + "已开放运费券，赶紧抢购哦，手慢无！");
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                            }
                        }
                    }

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

    public static string WebServiceApp(string url, string method, string param)
    {
        //转换输入参数的编码类型，获取bytep[]数组 
        byte[] byteArray = Encoding.UTF8.GetBytes(param);
        //初始化新的webRequst
        //1． 创建httpWebRequest对象
        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(url + method));
        //2． 初始化HttpWebRequest对象
        webRequest.Method = "POST";
        webRequest.ContentType = "application/x-www-form-urlencoded";
        webRequest.ContentLength = byteArray.Length;
        //3． 附加要POST给服务器的数据到HttpWebRequest对象(附加POST数据的过程比较特殊，它并没有提供一个属性给用户存取，需要写入HttpWebRequest对象提供的一个stream里面。)
        Stream newStream = webRequest.GetRequestStream();//创建一个Stream,赋值是写入HttpWebRequest对象提供的一个stream里面
        newStream.Write(byteArray, 0, byteArray.Length);
        newStream.Close();
        //4． 读取服务器的返回信息
        string result = string.Empty;
        HttpWebResponse response = webRequest.GetResponse() as HttpWebResponse;
        Stream responseStream = response.GetResponseStream();
        using (StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8")))
        {
            result = reader.ReadToEnd();
        }
        responseStream.Close();
        return result;
    }

    [CSMethod("GetKFGMToFile", 2)]
    public byte[] GetKFGMToFile(string yhm){
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                Workbook workbook = new Workbook(); //工作簿
                Worksheet sheet = workbook.Worksheets[0]; //工作表
                Cells cells = sheet.Cells;//单元格

                //为标题设置样式  
                Style styleTitle = workbook.Styles[workbook.Styles.Add()];
                styleTitle.HorizontalAlignment = TextAlignmentType.Center;//文字居中
                styleTitle.Font.Name = "宋体";//文字字体
                styleTitle.Font.Size = 18;//文字大小
                styleTitle.Font.IsBold = true;//粗体

                //样式1
                Style style1 = workbook.Styles[workbook.Styles.Add()];
                style1.HorizontalAlignment = TextAlignmentType.Center;//文字居中
                style1.Font.Name = "宋体";//文字字体
                style1.Font.Size = 12;//文字大小
                style1.Font.IsBold = true;//粗体

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
                cells[0, 0].PutValue("物流名称");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("运费券");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);

                string where="";
                if (!string.IsNullOrEmpty(yhm.Trim()))
                {
                    where += " and " + dbc.C_Like("b.UserXM", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                string str = @"  select a.*,b.UserName,b.UserXM from tb_b_platpoints a left join tb_b_user b on a.UserID=b.UserID
                        where a.status=0 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = dbc.ExecuteDataTable(str + " order by a.Points ");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["UserXM"]);
                    cells[i + 1, 0].SetStyle(style4);
                    cells[i + 1, 1].PutValue(dt.Rows[i]["Points"]);
                    cells[i + 1, 1].SetStyle(style4);
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


    [CSMethod("UploadSF", 1)]
    public object UploadSF(FileData[] fds)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                DataTable newdt = new DataTable();
                newdt.Columns.Add("UserID");
                newdt.Columns.Add("UserName");
                string str = "";
                if (fds[0].FileBytes.Length == 0)
                {
                    throw new Exception("你上传的文件可能已被打开，请关闭该文件！");
                }
                System.IO.MemoryStream ms = new System.IO.MemoryStream(fds[0].FileBytes);
                Workbook workbook = new Workbook(ms);
                Worksheet sheet = workbook.Worksheets[0];
                Cells cells = sheet.Cells;
                foreach (Cell cell in cells)
                {
                    if (cell.IsMerged == true)
                    {
                        Range range = cell.GetMergedRange();
                        cell.Value = cells[range.FirstRow, range.FirstColumn].Value;
                    }
                    else
                    {
                        cell.Value = cell.Value;
                    }
                }
                DataTable mydt = cells.ExportDataTableAsString(1, 0, cells.MaxRow+1, cells.MaxColumn + 1);

                List<DataRow> removelist = new List<DataRow>();
                for (int i = 0; i < mydt.Rows.Count; i++)
                {
                    bool IsNull = true;
                    for (int j = 0; j < mydt.Columns.Count; j++)
                    {
                        if (!string.IsNullOrEmpty(mydt.Rows[i][j].ToString().Trim()))
                        {
                            IsNull = false;
                        }
                    }
                    if (IsNull)
                    {
                        removelist.Add(mydt.Rows[i]);
                    }
                }
                for (int i = 0; i < removelist.Count; i++)
                {
                    mydt.Rows.Remove(removelist[i]);
                }

                string sql = "select * from tb_b_user where IsSHPass=1 and ClientKind=2";
                DataTable sfdt = dbc.ExecuteDataTable(sql);

                for (int i = 0; i < mydt.Rows.Count; i++)
                {
                    DataRow[] drs = sfdt.Select("UserName=" + dbc.ToSqlValue(mydt.Rows[i][0]));
                    if (drs.Length > 0)
                    {
                        DataRow newdr = newdt.NewRow();
                        newdr["UserID"] = drs[0]["UserID"];
                        newdr["UserName"] = drs[0]["UserName"];
                        newdt.Rows.Add(newdr);
                    }
                    else
                    {
                        str += mydt.Rows[i][0] + ",";
                    }
                }
                if (!string.IsNullOrEmpty(str))
                {
                    str += "这些三方不存在！";
                }
                return new {dt=newdt,str=str };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("SavePS")]
    public object SavePS(string platpointid,string carduserid,int points,int validhour,string sanfangList )
    {
        JavaScriptSerializer js = new JavaScriptSerializer();
        List<sanfang> list = js.Deserialize<List<sanfang>>(sanfangList);
        string _url = ServiceURL + "tbbpaisong.webpaisong";
        string jsonParam = new JavaScriptSerializer().Serialize(new
        {
            platpointid = platpointid,  
            carduserid = carduserid, 
            points = points, 
            validhour=validhour,
            sanfangList = list
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
}
