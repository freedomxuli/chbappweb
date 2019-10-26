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
///KFSQMag 的摘要说明
/// </summary>

[CSClass("KFSQMag")]
public class KFSQMag
{
    [CSMethod("GetList")]
    public object GetList(int pagnum, int pagesize, string yhm,string zt)
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
                    where += " and " + dbc.C_Like("userxm", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(zt))
                {
                    where += " and " + dbc.C_EQ("zt", Convert.ToInt32(zt));
                }

                string str = @"select * from (select a.userID,a.userxm,
                            case when b.pointkind is null then 0 
                            when b.pointkind=4 and b.points>0 and b.salerecordverifytype = 1 and dateadd(hour,b.validhour,b.addtime)>GETDATE() then 1 
                            when b.pointkind=4 and b.points<=0 and b.salerecordverifytype = 1 and dateadd(hour,b.validhour,b.addtime)<GETDATE() then 2 
                            when b.pointkind=4 and b.salerecordverifytype = 2 then 3
                            else 9 end zt, 
                            b.pointkind,b.salerecordverifytype,b.validhour,b.addtime,b.discount from tb_b_user  a 
                            left join (select * from tb_b_plattosale where status=0 and pointkind=4) b on a.UserID=b.UserID  where a.clientkind = 1 and a.IsSHPass=1) c 
                            where 1=1 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by zt ", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetListToFile", 2)]
    public byte[] GetListToFile(string yhm,string zt)
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
                cells[0, 1].PutValue("上架状态");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("上架折扣");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);

                string where = "";
                if (!string.IsNullOrEmpty(yhm.Trim()))
                {
                    where += " and " + dbc.C_Like("userxm", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(zt))
                {
                    where += " and " + dbc.C_EQ("zt", Convert.ToInt32(zt));
                }

                string str = @"select * from (select a.userID,a.userxm,
                            case when b.pointkind is null then 0 
                            when b.pointkind=4 and b.points>0 and b.salerecordverifytype = 1 and dateadd(hour,b.validhour,b.addtime)>GETDATE() then 1 
                            when b.pointkind=4 and b.points<=0 and b.salerecordverifytype = 1 and dateadd(hour,b.validhour,b.addtime)<GETDATE() then 2 
                            when b.pointkind=4 and b.salerecordverifytype = 2 then 3
                            else 9 end zt, 
                            b.pointkind,b.salerecordverifytype,b.validhour,b.addtime,b.discount,b.discountmemo from tb_b_user  a 
                            left join (select * from tb_b_plattosale where status=0 and pointkind=4) b on a.UserID=b.UserID  where a.clientkind = 1 and a.IsSHPass=1) c 
                            where 1=1 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = dbc.ExecuteDataTable(str + " order by zt ");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["userxm"]);
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["zt"] != null && dt.Rows[i]["zt"].ToString() != "")
                    {
                        if (Convert.ToInt32(dt.Rows[i]["zt"]) == 0)
                        {
                            cells[i + 1, 1].PutValue("未上架");
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["zt"]) == 1)
                        {
                            cells[i + 1, 1].PutValue("上架在售");
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["zt"]) == 2)
                        {
                            cells[i + 1, 1].PutValue("上架售罄");
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["zt"]) == 3)
                        {
                            cells[i + 1, 1].PutValue("已下架");
                        }
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    cells[i + 1, 2].PutValue(dt.Rows[i]["discountmemo"]);
                    cells[i + 1, 2].SetStyle(style4);
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

    [CSMethod("SaveKFSQ")]
    public bool SaveKFSQ(JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {

                var UserId = jsr["UserId"].ToString();
                string str = "select * from tb_b_plattosale where pointkind = 4 and status=0 and UserID=" + dbc.ToSqlValue(UserId);
                DataTable pdt = dbc.ExecuteDataTable(str);

                str = "select * from tb_b_user where UserID='" + UserId + "'";
                DataTable udt = dbc.ExecuteDataTable(str);

                var zxmc = "";
                if (udt.Rows.Count > 0)
                {
                    zxmc = udt.Rows[0]["UserXM"].ToString();
                }

                if (pdt.Rows.Count > 0)
                { //上过架
                    var SaleRecordID = Guid.NewGuid().ToString();

                    var logdt = dbc.GetEmptyDataTable("tb_b_salerecord");
                    var logdr = logdt.NewRow();
                    logdr["SaleRecordID"] = SaleRecordID;
                    logdr["SaleRecordCode"] = GetRandom();
                    logdr["SaleRecordUserID"] = UserId;
                    logdr["SaleRecordUserXM"] = zxmc;
                    logdr["SaleRecordTime"] = DateTime.Now;
                    logdr["SaleRecordPoints"] = 10000000;
                    logdr["SaleRecordLX"] = 4;
                    logdr["Status"] = 0;
                    logdr["adduser"] = SystemUser.CurrentUser.UserID;
                    logdr["addtime"] = DateTime.Now;
                    logdr["updateuser"] = DBNull.Value;
                    logdr["updatetime"] = DBNull.Value;
                    logdr["SaleRecordBelongID"] = UserId;
                    logdr["ValidHour"] = 10000;
                    logdr["SaleRecordDiscount"] = Convert.ToDecimal(jsr["discount"].ToString());
                    logdr["SaleRecordVerifyType"] = 1;
                    logdr["SaleRecordVerifyTime"] = DateTime.Now;
                    logdt.Rows.Add(logdr);
                    dbc.InsertTable(logdt);

                    var zxdt = dbc.GetEmptyDataTable("tb_b_zxsalelist");
                    var zxdr = zxdt.NewRow();
                    zxdr["ZXSaleListID"] = Guid.NewGuid().ToString();
                    zxdr["SaleRecordID"] = SaleRecordID;
                    zxdr["ZXSaleListUserID"] = UserId;
                    zxdr["ZXSaleListProduceLx"] = "A1E09B41-C13C-4C92-95D1-EC0C02928508";
                    zxdr["ZXSaleListPackLx"] = "75ECC903-1D1E-4397-A6AD-7F7D2ACDAA73";
                    zxdr["ZXSaleListHours"] = 10000;
                    zxdr["ZXSaleListCitys"] = "本专线所有城市";
                    zxdr["ZXSaleListZK"] = "1CF548FB-CB94-4ED4-87DE-7A9601CBB669";
                    zxdr["ZXSaleListMoney"] = 10000000;
                    zxdr["ZXSaleListFc"] = "5C2936FF-BFC6-454E-97CF-BA9DCDC10662";
                    zxdr["ZXSaleListZhLx"] = DBNull.Value;
                    zxdr["ZXSaleListPhLx"] = DBNull.Value;
                    zxdr["ZXSaleListPackMxLx"] = DBNull.Value;
                    zxdr["ZXSaleListTime"] = DateTime.Now;
                    zxdr["Status"] = 0;
                    zxdr["adduser"] = SystemUser.CurrentUser.UserID;
                    zxdr["addtime"] = DateTime.Now;
                    zxdr["updateuser"] = DBNull.Value;
                    zxdr["updatetime"] = DBNull.Value;
                    zxdr["ZXSaleListSyfw"] = "1F3F15FC-8D84-4F53-A1F6-9890AA8FA1EA";
                    zxdt.Rows.Add(zxdr);
                    dbc.InsertTable(zxdt);


                    var saledt = dbc.GetEmptyDataTable("tb_b_plattosale");
                    DataTableTracker saledtt = new DataTableTracker(saledt);
                    for (int i = 0; i < pdt.Rows.Count; i++)
                    {
                        var saledr = saledt.NewRow();
                        saledr["PlatToSaleId"] = pdt.Rows[i]["PlatToSaleId"];
                        saledr["UserID"] = UserId;
                        saledr["points"] = 10000000;
                        saledr["addtime"] = DateTime.Now;
                        saledr["status"] = 0;
                        saledr["discount"] = Convert.ToDecimal(jsr["discount"].ToString());
                        saledr["discountmemo"] = jsr["discountmemo"].ToString();
                        saledr["memo"] = DBNull.Value;
                        saledr["pointkind"] = 4;
                        saledr["SaleRecordID"] = SaleRecordID;
                        saledr["belongID"] = UserId;
                        saledr["validHour"] = 10000;
                        saledr["SaleRecordTime"] = DateTime.Now;
                        saledr["SaleRecordVerifyType"] = 1;
                        saledr["SaleRecordVerifyTime"] = DateTime.Now;
                        saledt.Rows.Add(saledr);
                        dbc.UpdateTable(saledt, saledtt);
                    }

                }
                else
                {
                    var SaleRecordID = Guid.NewGuid().ToString();

                    var logdt = dbc.GetEmptyDataTable("tb_b_salerecord");
                    var logdr = logdt.NewRow();
                    logdr["SaleRecordID"] = SaleRecordID;
                    logdr["SaleRecordCode"] = GetRandom();
                    logdr["SaleRecordUserID"] = UserId;
                    logdr["SaleRecordUserXM"] = zxmc;
                    logdr["SaleRecordTime"] = DateTime.Now;
                    logdr["SaleRecordPoints"] = 10000000;
                    logdr["SaleRecordLX"] = 4;
                    logdr["Status"] = 0;
                    logdr["adduser"] = SystemUser.CurrentUser.UserID;
                    logdr["addtime"] = DateTime.Now;
                    logdr["updateuser"] = DBNull.Value;
                    logdr["updatetime"] = DBNull.Value;
                    logdr["SaleRecordBelongID"] = UserId;
                    logdr["ValidHour"] = 10000;
                    logdr["SaleRecordDiscount"] = Convert.ToDecimal(jsr["discount"].ToString());
                    logdr["SaleRecordVerifyType"] = 1;
                    logdr["SaleRecordVerifyTime"] = DateTime.Now;
                    logdt.Rows.Add(logdr);
                    dbc.InsertTable(logdt);

                    var zxdt = dbc.GetEmptyDataTable("tb_b_zxsalelist");
                    var zxdr = zxdt.NewRow();
                    zxdr["ZXSaleListID"] = Guid.NewGuid().ToString();
                    zxdr["SaleRecordID"] = SaleRecordID;
                    zxdr["ZXSaleListUserID"] = UserId;
                    zxdr["ZXSaleListProduceLx"] = "A1E09B41-C13C-4C92-95D1-EC0C02928508";
                    zxdr["ZXSaleListPackLx"] = "75ECC903-1D1E-4397-A6AD-7F7D2ACDAA73";
                    zxdr["ZXSaleListHours"] = 10000;
                    zxdr["ZXSaleListCitys"] = "本专线所有城市";
                    zxdr["ZXSaleListZK"] = "1CF548FB-CB94-4ED4-87DE-7A9601CBB669";
                    zxdr["ZXSaleListMoney"] = 10000000;
                    zxdr["ZXSaleListFc"] = "5C2936FF-BFC6-454E-97CF-BA9DCDC10662";
                    zxdr["ZXSaleListZhLx"] = DBNull.Value;
                    zxdr["ZXSaleListPhLx"] = DBNull.Value;
                    zxdr["ZXSaleListPackMxLx"] = DBNull.Value;
                    zxdr["ZXSaleListTime"] = DateTime.Now;
                    zxdr["Status"] = 0;
                    zxdr["adduser"] = SystemUser.CurrentUser.UserID;
                    zxdr["addtime"] = DateTime.Now;
                    zxdr["updateuser"] = DBNull.Value;
                    zxdr["updatetime"] = DBNull.Value;
                    zxdr["ZXSaleListSyfw"] = "1F3F15FC-8D84-4F53-A1F6-9890AA8FA1EA";
                    zxdt.Rows.Add(zxdr);
                    dbc.InsertTable(zxdt);

                    var saledt = dbc.GetEmptyDataTable("tb_b_plattosale");
                    var saledr = saledt.NewRow();
                    saledr["PlatToSaleId"] = Guid.NewGuid().ToString();
                    saledr["UserID"] = UserId;
                    saledr["points"] = 10000000;
                    saledr["addtime"] = DateTime.Now;
                    saledr["status"] = 0;
                    saledr["discount"] = Convert.ToDecimal(jsr["discount"].ToString());
                    saledr["discountmemo"] = jsr["discountmemo"].ToString();
                    saledr["memo"] = DBNull.Value;
                    saledr["pointkind"] = 4;
                    saledr["SaleRecordID"] = SaleRecordID;
                    saledr["belongID"] = UserId;
                    saledr["validHour"] = 10000;
                    saledr["SaleRecordTime"] = DateTime.Now;
                    saledr["SaleRecordVerifyType"] = 1;
                    saledr["SaleRecordVerifyTime"] = DateTime.Now;
                    saledt.Rows.Add(saledr);
                    dbc.InsertTable(saledt);

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

    [CSMethod("KFSQXJ")]
    public bool KFSQXJ(string id)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                dbc.BeginTransaction();

                string sql = "select * from tb_b_plattosale where UserID = @UserID and status=0 and pointkind =4 and salerecordverifytype=1";
                SqlCommand cmd = dbc.CreateCommand(sql);
                cmd.Parameters.Add("@UserID", id);
                DataTable dt = dbc.ExecuteDataTable(cmd);

                if (dt.Rows.Count > 0)
                {
                    var saledt = dbc.GetEmptyDataTable("tb_b_plattosale");
                    DataTableTracker saledtt = new DataTableTracker(saledt);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var saledr = saledt.NewRow();
                        saledr["PlatToSaleId"] = dt.Rows[i]["PlatToSaleId"];
                        saledr["SaleRecordVerifyType"] = 2;
                        saledr["SaleRecordVerifyTime"] = DateTime.Now;
                        saledt.Rows.Add(saledr);
                        dbc.UpdateTable(saledt, saledtt);

                        var redt = dbc.GetEmptyDataTable("tb_b_record");
                        var redr = redt.NewRow();
                        redr["CaoZuoJiLuID"] = Guid.NewGuid();
                        redr["UserID"] =SystemUser.CurrentUser.UserID;
                        redr["CaoZuoLeiXing"] = "下架授权运费券";
                        redr["CaoZuoNeiRong"] = SystemUser.CurrentUser.UserName+"(操作员)在"+DateTime.Now+"时间下架了授权运费券";
                        redr["CaoZuoTime"] = DateTime.Now;
                        redt.Rows.Add(redr);
                        dbc.InsertTable(redt);
                    }
                }
                else
                {
                    throw new Exception("暂无需下架的券！");
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
