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
///SHSQMag 的摘要说明
/// </summary>

[CSClass("SHSQMag")]
public class SHSQMag
{

    [CSMethod("GetSHSQList")]
    public object GetSHSQList(int pagnum, int pagesize, string zt, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;
                string where = "";
                if (!string.IsNullOrEmpty(zt))
                {
                    if (Convert.ToInt32(zt) == 0)
                    {
                        where += " and a.reviewuser is null";
                    }
                    else if (Convert.ToInt32(zt) == 1)
                    {
                        where += " and a.reviewuser is not null";
                    }
                }
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.addtime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.addtime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str= @"select a.*,b.UserXM as shr,c.UserXM as zxmc from tb_b_empower_release a left join tb_b_user b on a.reviewuser=b.UserID
                    left join tb_b_user c on a.adduser=c.UserID
                where 1=1 "+where+" order by addtime desc";
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str, pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    [CSMethod("GetSHSQListToFile", 2)]
    public byte[] GetSHSQListToFile(string zt, string beg, string end)
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
                cells[0, 0].PutValue("授权专线");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("授权状态");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("新增时间");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("审核人");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("审核时间");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);

                string where = "";
                if (!string.IsNullOrEmpty(zt))
                {
                    if (Convert.ToInt32(zt) == 0)
                    {
                        where += " and a.reviewuser is null";
                    }
                    else if (Convert.ToInt32(zt) == 1)
                    {
                        where += " and a.reviewuser is not null";
                    }
                }
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.addtime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.addtime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select a.*,b.UserXM as shr,c.UserXM as zxmc from tb_b_empower_release a left join tb_b_user b on a.reviewuser=b.UserID
                    left join tb_b_user c on a.adduser=c.UserID
                where 1=1 " + where + " order by addtime desc";
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = dbc.ExecuteDataTable(str);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["zxmc"]);
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["empowertype"] != null && dt.Rows[i]["empowertype"].ToString() != "")
                    {
                        if (Convert.ToInt32(dt.Rows[i]["empowertype"]) == 0)
                        {
                            cells[i + 1, 1].PutValue("授权");
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["empowertype"]) == 1)
                        {
                            cells[i + 1, 1].PutValue("取消授权");
                        }
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["addtime"] != null && dt.Rows[i]["addtime"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(Convert.ToDateTime(dt.Rows[i]["addtime"]).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    cells[i + 1, 2].SetStyle(style4);
                    cells[i + 1, 3].PutValue(dt.Rows[i]["shr"]);
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["reviewtime"] != null && dt.Rows[i]["reviewtime"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(Convert.ToDateTime(dt.Rows[i]["reviewtime"]).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    cells[i + 1, 4].SetStyle(style4);
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


    [CSMethod("SHSQ")]
    public bool SHSQ(string id)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                DataTable dt = dbc.GetEmptyDataTable("tb_b_empower_release");
                DataTableTracker dtt = new DataTableTracker(dt);
                var dr = dt.NewRow();
                dr["id"] = id;
                dr["reviewtime"] = DateTime.Now;
                dr["reviewuser"] = SystemUser.CurrentUser.UserID;
                dr["updateuser"] = SystemUser.CurrentUser.UserID;
                dr["updatetime"] = DateTime.Now;
                dt.Rows.Add(dr);
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
}
