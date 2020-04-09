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
using System.Web.Script.Serialization;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

/// <summary>
/// CYMagSf 的摘要说明
/// </summary>
[CSClass("ThreeClass")]
public class ThreeClass
{
    public ThreeClass()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }
    string ServiceURL = System.Configuration.ConfigurationManager.AppSettings["ZYServiceURL"].ToString();
    [CSMethod("GetThreeczgmmxLIST")]
    public object GetThreeczgmmxLIST(int pagnum, int pagesize, string threezh, string zxname, string ddh, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(threezh.Trim()))
                {
                    where += " and " + dbc.C_Like("c.UserName", threezh.Trim(), LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(zxname.Trim()))
                {
                    where += " and " + dbc.C_Like("c.UserXM", zxname.Trim(), LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(ddh.Trim()))
                {
                    where += " and " + dbc.C_EQ("a.OrderCode", ddh.Trim());
                }

                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select c.UserName,c.UserXM,a.AddTime,a.OrderCode,a.Money,a.redenvelopemoney,a.Points as ishb,d.points from tb_b_order a left join tb_b_plattosale b on a.PlatToSaleId=b.PlatToSaleId
left join tb_b_user c on b.UserID=c.UserID
left join tb_b_mydonate d on a.OrderCode=d.ordercode

where b.pointkind=5 and  a.status = 0 and zhifuzt = 1   ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.AddTime desc", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }


    //列表EXCEL退款审核数据导出EXCEL
    [CSMethod("GetThreeczgmmxOutLIST", 2)]
    public byte[] GetThreeczgmmxOutLIST(string threezh, string zxname, string ddh, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {

                string where = "";
                if (!string.IsNullOrEmpty(threezh.Trim()))
                {
                    where += " and " + dbc.C_Like("c.UserName", threezh.Trim(), LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(zxname.Trim()))
                {
                    where += " and " + dbc.C_Like("c.UserXM", zxname.Trim(), LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(ddh.Trim()))
                {
                    where += " and " + dbc.C_EQ("a.OrderCode", ddh.Trim());
                }

                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select c.UserName,c.UserXM,a.AddTime,a.OrderCode,a.Money,a.redenvelopemoney,a.Points as ishb,d.points from tb_b_order a left join tb_b_plattosale b on a.PlatToSaleId=b.PlatToSaleId
left join tb_b_user c on b.UserID=c.UserID
left join tb_b_mydonate d on a.OrderCode=d.ordercode

where b.pointkind=5    and  a.status = 0 and zhifuzt = 1  ";
                str += where;

                //开始取分页数据

                System.Data.DataTable dt = dbc.ExecuteDataTable(str + " order by a.AddTime desc");

                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(); //工作簿
                Aspose.Cells.Worksheet sheet = workbook.Worksheets[0]; //工作表
                Aspose.Cells.Cells cells = sheet.Cells;//单元格

                //样式1
                Aspose.Cells.Style style1 = workbook.Styles[workbook.Styles.Add()];//新增样式    
                style1.HorizontalAlignment = TextAlignmentType.Center;//文字居中
                style1.Font.Name = "宋体";//文字字体
                style1.Font.Size = 12;//文字大小
                style1.IsTextWrapped = true;//单元格内容自动换行
                style1.Font.IsBold = true;//粗体
                style1.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                style1.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style1.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style1.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                //样式2
                Aspose.Cells.Style style2 = workbook.Styles[workbook.Styles.Add()];//新增样式    
                style2.HorizontalAlignment = TextAlignmentType.Left;//文字居左
                style2.Font.Name = "宋体";//文字字体
                style2.Font.Size = 12;//文字大小
                style2.IsTextWrapped = true;//单元格内容自动换行
                style2.Font.IsBold = false;//粗体
                style2.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                //样式3
                Aspose.Cells.Style style3 = workbook.Styles[workbook.Styles.Add()];//新增样式    
                style3.HorizontalAlignment = TextAlignmentType.Center;//文字居左
                style3.Font.Name = "宋体";//文字字体
                style3.Font.Size = 20;//文字大小
                style3.IsTextWrapped = true;//单元格内容自动换行
                style3.Font.IsBold = true;//粗体


                //样式4
                Aspose.Cells.Style style4 = workbook.Styles[workbook.Styles.Add()];//新增样式    
                style4.HorizontalAlignment = TextAlignmentType.Left;//文字居左
                style4.Font.Name = "宋体";//文字字体
                style4.Font.Size = 10;//文字大小
                style4.Font.Color = System.Drawing.Color.Red;//文字大小

                style4.IsTextWrapped = true;//单元格内容自动换行
                style4.Font.IsBold = false;//粗体




                //第一行标题列表
                //合并单元格cells.Merge(1, 0, 3, 1) 参数1代表当前行，参数0代表当前行当前列即第一行第一列，参数3合并的行数，参数4合并的列数

                cells.Merge(0, 0, 1, 9);

                cells[0, 0].PutValue("三方充值购买明细");
                cells[0, 0].SetStyle(style3);
                cells.SetRowHeight(0, 30);




                cells.SetColumnWidth(0, 30);
                cells[2, 0].PutValue("三方账号");
                cells[2, 0].SetStyle(style1);
                cells.SetColumnWidth(1, 30);
                cells[2, 1].PutValue("专线名称");
                cells[2, 1].SetStyle(style1);
                cells.SetColumnWidth(2, 30);
                cells[2, 2].PutValue("交易时间");
                cells[2, 2].SetStyle(style1);
                cells.SetColumnWidth(3, 30);
                cells[2, 3].PutValue("订单编号");
                cells[2, 3].SetStyle(style1);

                cells.SetColumnWidth(4, 30);
                cells[2, 4].PutValue("充值金额");
                cells[2, 4].SetStyle(style1);

                cells.SetColumnWidth(5, 30);
                cells[2, 5].PutValue("实际支付金额");
                cells[2, 5].SetStyle(style1);

                cells.SetColumnWidth(6, 30);
                cells[2, 6].PutValue("是否使用红包");
                cells[2, 6].SetStyle(style1);

                cells.SetColumnWidth(7, 30);
                cells[2, 7].PutValue("红包金额");
                cells[2, 7].SetStyle(style1);

                cells.SetColumnWidth(8, 30);
                cells[2, 8].PutValue("赠送券额");
                cells[2, 8].SetStyle(style1);
              




                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        cells[i + 3, 0].PutValue(dt.Rows[i]["UserName"].ToString());
                        cells[i + 3, 0].SetStyle(style2);
                        cells[i + 3, 1].PutValue(dt.Rows[i]["UserXM"].ToString());
                        cells[i + 3, 1].SetStyle(style2);
                        cells[i + 3, 2].PutValue(Convert.ToDateTime(dt.Rows[i]["AddTime"].ToString()).ToString("yyyy-MM-dd"));
                        cells[i + 3, 2].SetStyle(style2);
                        cells[i + 3, 3].PutValue(dt.Rows[i]["OrderCode"].ToString());
                        cells[i + 3, 3].SetStyle(style2);
                        cells[i + 3, 4].PutValue(dt.Rows[i]["ishb"].ToString());
                        cells[i + 3, 4].SetStyle(style2);
                        cells[i + 3, 5].PutValue(dt.Rows[i]["Money"].ToString());
                        cells[i + 3, 5].SetStyle(style2);
                        cells[i + 3, 6].PutValue(dt.Rows[i]["redenvelopemoney"].ToString());
                        cells[i + 3, 6].SetStyle(style2);
                        cells[i + 3, 7].PutValue(dt.Rows[i]["points"].ToString());
                        cells[i + 3, 7].SetStyle(style2);
                      

                    }
                }


                System.IO.MemoryStream ms = workbook.SaveToStream();
                byte[] bt = ms.ToArray();
                return bt;






            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }


    [CSMethod("GetThreeXFgmmxLIST")]
    public object GetThreeXFgmmxLIST(int pagnum, int pagesize, string threezh, string zxname, string ddh, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(threezh.Trim()))
                {
                    where += " and " + dbc.C_Like("b.UserName", threezh.Trim(), LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(zxname.Trim()))
                {
                    where += " and " + dbc.C_Like("b.UserXM", zxname.Trim(), LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(ddh.Trim()))
                {
                    where += " and " + dbc.C_EQ("a.OrderCode", ddh.Trim());
                }

                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select b.UserName,b.UserXM,a.addtime,a.ordercode,a.points,a.myvoucherpayid from tb_b_myvoucher_pay a left join tb_b_user b on a.carduserid=b.UserID
 ";
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

    //列表EXCEL退款审核数据导出EXCEL
    [CSMethod("GetThreeXFgmmxOutLIST", 2)]
    public byte[] GetThreeXFgmmxOutLIST(string threezh, string zxname, string ddh, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {

                string where = "";
                if (!string.IsNullOrEmpty(threezh.Trim()))
                {
                    where += " and " + dbc.C_Like("b.UserName", threezh.Trim(), LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(zxname.Trim()))
                {
                    where += " and " + dbc.C_Like("b.UserXM", zxname.Trim(), LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(ddh.Trim()))
                {
                    where += " and " + dbc.C_EQ("a.OrderCode", ddh.Trim());
                }

                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select b.UserName,b.UserXM,a.addtime,a.ordercode,a.points,a.myvoucherpayid from tb_b_myvoucher_pay a left join tb_b_user b on a.carduserid=b.UserID
 ";
                str += where;

                //开始取分页数据

                System.Data.DataTable dt = dbc.ExecuteDataTable(str + " order by a.AddTime desc");

                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(); //工作簿
                Aspose.Cells.Worksheet sheet = workbook.Worksheets[0]; //工作表
                Aspose.Cells.Cells cells = sheet.Cells;//单元格

                //样式1
                Aspose.Cells.Style style1 = workbook.Styles[workbook.Styles.Add()];//新增样式    
                style1.HorizontalAlignment = TextAlignmentType.Center;//文字居中
                style1.Font.Name = "宋体";//文字字体
                style1.Font.Size = 12;//文字大小
                style1.IsTextWrapped = true;//单元格内容自动换行
                style1.Font.IsBold = true;//粗体
                style1.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                style1.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style1.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style1.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                //样式2
                Aspose.Cells.Style style2 = workbook.Styles[workbook.Styles.Add()];//新增样式    
                style2.HorizontalAlignment = TextAlignmentType.Left;//文字居左
                style2.Font.Name = "宋体";//文字字体
                style2.Font.Size = 12;//文字大小
                style2.IsTextWrapped = true;//单元格内容自动换行
                style2.Font.IsBold = false;//粗体
                style2.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                //样式3
                Aspose.Cells.Style style3 = workbook.Styles[workbook.Styles.Add()];//新增样式    
                style3.HorizontalAlignment = TextAlignmentType.Center;//文字居左
                style3.Font.Name = "宋体";//文字字体
                style3.Font.Size = 20;//文字大小
                style3.IsTextWrapped = true;//单元格内容自动换行
                style3.Font.IsBold = true;//粗体


                //样式4
                Aspose.Cells.Style style4 = workbook.Styles[workbook.Styles.Add()];//新增样式    
                style4.HorizontalAlignment = TextAlignmentType.Left;//文字居左
                style4.Font.Name = "宋体";//文字字体
                style4.Font.Size = 10;//文字大小
                style4.Font.Color = System.Drawing.Color.Red;//文字大小

                style4.IsTextWrapped = true;//单元格内容自动换行
                style4.Font.IsBold = false;//粗体




                //第一行标题列表
                //合并单元格cells.Merge(1, 0, 3, 1) 参数1代表当前行，参数0代表当前行当前列即第一行第一列，参数3合并的行数，参数4合并的列数

                cells.Merge(0, 0, 1, 9);

                cells[0, 0].PutValue("三方充值消费明细表");
                cells[0, 0].SetStyle(style3);
                cells.SetRowHeight(0, 30);




                cells.SetColumnWidth(0, 30);
                cells[2, 0].PutValue("三方账号");
                cells[2, 0].SetStyle(style1);
                cells.SetColumnWidth(1, 30);
                cells[2, 1].PutValue("专线名称");
                cells[2, 1].SetStyle(style1);
                cells.SetColumnWidth(2, 30);
                cells[2, 2].PutValue("交易时间");
                cells[2, 2].SetStyle(style1);
                cells.SetColumnWidth(3, 30);
                cells[2, 3].PutValue("订单编号");
                cells[2, 3].SetStyle(style1);

                cells.SetColumnWidth(4, 30);
                cells[2, 4].PutValue("充值金额");
                cells[2, 4].SetStyle(style1);

                





                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        cells[i + 3, 0].PutValue(dt.Rows[i]["UserName"].ToString());
                        cells[i + 3, 0].SetStyle(style2);
                        cells[i + 3, 1].PutValue(dt.Rows[i]["UserXM"].ToString());
                        cells[i + 3, 1].SetStyle(style2);
                        cells[i + 3, 2].PutValue(Convert.ToDateTime(dt.Rows[i]["AddTime"].ToString()).ToString("yyyy-MM-dd"));
                        cells[i + 3, 2].SetStyle(style2);
                        cells[i + 3, 3].PutValue(dt.Rows[i]["OrderCode"].ToString());
                        cells[i + 3, 3].SetStyle(style2);
                        cells[i + 3, 4].PutValue(dt.Rows[i]["points"].ToString());
                        cells[i + 3, 4].SetStyle(style2);
                       

                    }
                }


                System.IO.MemoryStream ms = workbook.SaveToStream();
                byte[] bt = ms.ToArray();
                return bt;






            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }




    [CSMethod("GetThreeZSgmmxLIST")]
    public object GetThreeZSgmmxLIST(int pagnum, int pagesize, string threezh, string zxname, string ddh, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(threezh.Trim()))
                {
                    where += " and " + dbc.C_Like("b.UserName", threezh.Trim(), LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(zxname.Trim()))
                {
                    where += " and " + dbc.C_Like("b.UserXM", zxname.Trim(), LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(ddh.Trim()))
                {
                    where += " and " + dbc.C_EQ("a.OrderCode", ddh.Trim());
                }

                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select b.UserName,b.UserXM,a.addtime,a.ordercode,a.points,a.mydonatepayid from tb_b_mydonate_pay a left join tb_b_user b on a.carduserid=b.UserID
 ";
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

    //列表EXCEL退款审核数据导出EXCEL
    [CSMethod("GetThreeZSgmmxOutLIST", 2)]
    public byte[] GetThreeZSgmmxOutLIST(string threezh, string zxname, string ddh, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {

                string where = "";
                if (!string.IsNullOrEmpty(threezh.Trim()))
                {
                    where += " and " + dbc.C_Like("b.UserName", threezh.Trim(), LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(zxname.Trim()))
                {
                    where += " and " + dbc.C_Like("b.UserXM", zxname.Trim(), LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(ddh.Trim()))
                {
                    where += " and " + dbc.C_EQ("a.OrderCode", ddh.Trim());
                }

                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select b.UserName,b.UserXM,a.addtime,a.ordercode,a.points from tb_b_mydonate_pay a left join tb_b_user b on a.carduserid=b.UserID
 ";
                str += where;

                //开始取分页数据

                System.Data.DataTable dt = dbc.ExecuteDataTable(str + " order by a.AddTime desc");

                Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(); //工作簿
                Aspose.Cells.Worksheet sheet = workbook.Worksheets[0]; //工作表
                Aspose.Cells.Cells cells = sheet.Cells;//单元格

                //样式1
                Aspose.Cells.Style style1 = workbook.Styles[workbook.Styles.Add()];//新增样式    
                style1.HorizontalAlignment = TextAlignmentType.Center;//文字居中
                style1.Font.Name = "宋体";//文字字体
                style1.Font.Size = 12;//文字大小
                style1.IsTextWrapped = true;//单元格内容自动换行
                style1.Font.IsBold = true;//粗体
                style1.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                style1.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style1.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style1.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                //样式2
                Aspose.Cells.Style style2 = workbook.Styles[workbook.Styles.Add()];//新增样式    
                style2.HorizontalAlignment = TextAlignmentType.Left;//文字居左
                style2.Font.Name = "宋体";//文字字体
                style2.Font.Size = 12;//文字大小
                style2.IsTextWrapped = true;//单元格内容自动换行
                style2.Font.IsBold = false;//粗体
                style2.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                style2.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

                //样式3
                Aspose.Cells.Style style3 = workbook.Styles[workbook.Styles.Add()];//新增样式    
                style3.HorizontalAlignment = TextAlignmentType.Center;//文字居左
                style3.Font.Name = "宋体";//文字字体
                style3.Font.Size = 20;//文字大小
                style3.IsTextWrapped = true;//单元格内容自动换行
                style3.Font.IsBold = true;//粗体


                //样式4
                Aspose.Cells.Style style4 = workbook.Styles[workbook.Styles.Add()];//新增样式    
                style4.HorizontalAlignment = TextAlignmentType.Left;//文字居左
                style4.Font.Name = "宋体";//文字字体
                style4.Font.Size = 10;//文字大小
                style4.Font.Color = System.Drawing.Color.Red;//文字大小

                style4.IsTextWrapped = true;//单元格内容自动换行
                style4.Font.IsBold = false;//粗体




                //第一行标题列表
                //合并单元格cells.Merge(1, 0, 3, 1) 参数1代表当前行，参数0代表当前行当前列即第一行第一列，参数3合并的行数，参数4合并的列数

                cells.Merge(0, 0, 1, 9);

                cells[0, 0].PutValue("三方充值购买明细");
                cells[0, 0].SetStyle(style3);
                cells.SetRowHeight(0, 30);




                cells.SetColumnWidth(0, 30);
                cells[2, 0].PutValue("三方账号");
                cells[2, 0].SetStyle(style1);
                cells.SetColumnWidth(1, 30);
                cells[2, 1].PutValue("专线名称");
                cells[2, 1].SetStyle(style1);
                cells.SetColumnWidth(2, 30);
                cells[2, 2].PutValue("交易时间");
                cells[2, 2].SetStyle(style1);
                cells.SetColumnWidth(3, 30);
                cells[2, 3].PutValue("订单编号");
                cells[2, 3].SetStyle(style1);

                cells.SetColumnWidth(4, 30);
                cells[2, 4].PutValue("充值金额");
                cells[2, 4].SetStyle(style1);

                cells.SetColumnWidth(5, 30);
                cells[2, 5].PutValue("实际支付金额");
                cells[2, 5].SetStyle(style1);

                cells.SetColumnWidth(6, 30);
                cells[2, 6].PutValue("是否使用红包");
                cells[2, 6].SetStyle(style1);

                cells.SetColumnWidth(7, 30);
                cells[2, 7].PutValue("红包金额");
                cells[2, 7].SetStyle(style1);

                cells.SetColumnWidth(8, 30);
                cells[2, 8].PutValue("赠送券额");
                cells[2, 8].SetStyle(style1);





                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        cells[i + 3, 0].PutValue(dt.Rows[i]["UserName"].ToString());
                        cells[i + 3, 0].SetStyle(style2);
                        cells[i + 3, 1].PutValue(dt.Rows[i]["UserXM"].ToString());
                        cells[i + 3, 1].SetStyle(style2);
                        cells[i + 3, 2].PutValue(Convert.ToDateTime(dt.Rows[i]["AddTime"].ToString()).ToString("yyyy-MM-dd"));
                        cells[i + 3, 2].SetStyle(style2);
                        cells[i + 3, 3].PutValue(dt.Rows[i]["OrderCode"].ToString());
                        cells[i + 3, 3].SetStyle(style2);
                        cells[i + 3, 4].PutValue(dt.Rows[i]["Money"].ToString());
                        cells[i + 3, 4].SetStyle(style2);
                        cells[i + 3, 5].PutValue(dt.Rows[i]["redenvelopemoney"].ToString());
                        cells[i + 3, 5].SetStyle(style2);
                        cells[i + 3, 6].PutValue(dt.Rows[i]["ishb"].ToString());
                        cells[i + 3, 6].SetStyle(style2);
                        cells[i + 3, 7].PutValue(dt.Rows[i]["points"].ToString());
                        cells[i + 3, 7].SetStyle(style2);


                    }
                }


                System.IO.MemoryStream ms = workbook.SaveToStream();
                byte[] bt = ms.ToArray();
                return bt;






            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }



    [CSMethod("GetPSHBZJ")]
    public object GetPSHBZJ(string nid,int lx)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string str = @"select * from tb_b_payaccount where status=0";

                if (lx == 1)
                {
                    str += " and myvoucherpayid="+dbc.ToSqlValue(nid);
                }
                else if (lx == 2)
                {
                    str += " and mydonatepayid=" + dbc.ToSqlValue(nid);

                }
                str += " order by payaccounttime desc";
                DataTable dt = dbc.ExecuteDataTable(str);

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }



}

