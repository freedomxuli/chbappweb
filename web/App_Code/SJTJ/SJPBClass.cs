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
[CSClass("SJPBClass")]
public class SJPBClass
{
    public SJPBClass()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }
    string ServiceURL = System.Configuration.ConfigurationManager.AppSettings["ZYServiceURL"].ToString();
    [CSMethod("GetPBHBZEDList")]
    public object GetPBHBZEDList(int pagnum, int pagesize, string areacode)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                var cmd = dbc.CreateCommand();
                cmd.CommandText = @"select c.dq_mc,b.userxm,a.addtime,a.quota
                                    from tb_b_user_quota a left join tb_b_user b on a.userid = b.UserID
                                    left join tb_b_dq c on b.DqBm=c.dq_bm  
                                    where a.status = 0and b.isdonate = 0";
                if (areacode != null && areacode != "")
                {
                    cmd.CommandText += " and b.DqBm =" + dbc.ToSqlValue(areacode);
                }
                else {

                    cmd.CommandText += " and ( b.DqBm='320500' or  b.DqBm='320400')";
                }
                 cmd.CommandText += "order by a.addtime desc";





                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(cmd, pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    //列表EXCEL退款审核数据导出EXCEL
    [CSMethod("GetTkshListOutList", 2)]
    public byte[] GetTkshListOutList(string areacode)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {

                var cmd = dbc.CreateCommand();
                cmd.CommandText = @"select c.dq_mc,b.userxm,a.addtime,a.quota
                                    from tb_b_user_quota a left join tb_b_user b on a.userid = b.UserID
                                    left join tb_b_dq c on b.DqBm=c.dq_bm  
                                    where a.status = 0and b.isdonate = 0";
                if (areacode != null && areacode != "")
                {
                    cmd.CommandText += " and b.DqBm =" + dbc.ToSqlValue(areacode);
                }
                else
                {

                    cmd.CommandText += " and ( b.DqBm='320500' or  b.DqBm='320400')";
                }
                cmd.CommandText += "order by a.addtime desc";

                System.Data.DataTable dt = dbc.ExecuteDataTable(cmd);

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

                cells.Merge(0, 0, 1, 4);

                cells[0, 0].PutValue("后台配比红包总额度明细");
                cells[0, 0].SetStyle(style3);
                cells.SetRowHeight(0, 30);




                cells.SetColumnWidth(0, 30);
                cells[2, 0].PutValue("地区");
                cells[2, 0].SetStyle(style1);
                cells.SetColumnWidth(1, 30);
                cells[2, 1].PutValue("专线名称");
                cells[2, 1].SetStyle(style1);
                cells.SetColumnWidth(2, 30);
                cells[2, 2].PutValue("查询时间");
                cells[2, 2].SetStyle(style1);
                cells.SetColumnWidth(3, 30);
                cells[2, 3].PutValue("总额度");
                cells[2, 3].SetStyle(style1);





                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        cells[i + 3, 0].PutValue(dt.Rows[i]["dq_mc"].ToString());
                        cells[i + 3, 0].SetStyle(style2);
                        cells[i + 3, 1].PutValue(dt.Rows[i]["userxm"].ToString());
                        cells[i + 3, 1].SetStyle(style2);
                        cells[i + 3, 2].PutValue(Convert.ToDateTime(dt.Rows[i]["addtime"].ToString()).ToString("yyyy-MM-dd"));
                        cells[i + 3, 2].SetStyle(style2);
                        cells[i + 3, 3].PutValue(dt.Rows[i]["quota"].ToString());
                        cells[i + 3, 3].SetStyle(style2);


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


    /// <summary>
    /// 历史消耗红包额度明细
    /// </summary>
    /// <param name="pagnum"></param>
    /// <param name="pagesize"></param>
    /// <param name="areacode"></param>
    /// <returns></returns>
    [CSMethod("lsxhlistV1")]
    public object lsxhlistV1(int pagnum, int pagesize, string areacode, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                var cmd = dbc.CreateCommand();
                cmd.CommandText = @" select e.dq_mc,Convert(decimal(18,0),a.money/0.05) num,c.UserName,a.ordercode,a.addtime,d.UserXM from tb_b_redenvelope a
left join tb_b_order b on a.ordercode = b.OrderCode
left join tb_b_user c on a.userid = c.UserID
left join tb_b_user d on b.SaleUserID = d.UserID
left join tb_b_dq e on d.DqBm=e.dq_bm  
where a.type = 5 
 and d.isdonate = 0
 
";
                if (areacode != null && areacode != "")
                {
                    cmd.CommandText += " and d.DqBm =" + dbc.ToSqlValue(areacode);
                }
                else
                {

                    cmd.CommandText += " and ( d.DqBm='320500' or  d.DqBm='320400')";
                }




                if (beg != null && beg != "")
                {
                    cmd.CommandText += " and a.addtime >=" + dbc.ToSqlValue(beg);
                }

                if (end != null && end != "")
                {
                    cmd.CommandText += " and a.addtime <=" + dbc.ToSqlValue(end);
                }


                cmd.CommandText += "order by a.addtime desc";





                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(cmd, pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    
    
    //历史消耗红包额度明细EXCEL
    [CSMethod("GetTkshListOutListV1", 2)]
    public byte[] GetTkshListOutListV1(JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {

                var cmd = dbc.CreateCommand();
                 cmd.CommandText = @" select e.dq_mc,Convert(decimal(18,0),a.money/0.05) num,c.UserName,a.ordercode,a.addtime,d.UserXM from tb_b_redenvelope a
left join tb_b_order b on a.ordercode = b.OrderCode
left join tb_b_user c on a.userid = c.UserID
left join tb_b_user d on b.SaleUserID = d.UserID
left join tb_b_dq e on d.DqBm=e.dq_bm  
where a.type = 5 
 and d.isdonate = 0
 
";
                if (jsr["areacode"] != null && jsr["areacode"].ToString() != "")
                {
                    cmd.CommandText += " and d.DqBm =" + dbc.ToSqlValue(jsr["areacode"].ToString());
                }
                else
                {

                    cmd.CommandText += " and ( d.DqBm='320500' or  d.DqBm='320400')";
                }




                if (jsr["beg"] != null && jsr["beg"].ToString() != "")
                {
                    cmd.CommandText += " and a.addtime >=" + dbc.ToSqlValue(Convert.ToDateTime(jsr["beg"].ToString()));
                }

                if (jsr["end"] != null && jsr["end"].ToString() != "")
                {
                    cmd.CommandText += " and a.addtime <=" + dbc.ToSqlValue(Convert.ToDateTime(jsr["end"].ToString()));
                }



                cmd.CommandText += "order by a.addtime desc";

                System.Data.DataTable dt = dbc.ExecuteDataTable(cmd);

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

                cells.Merge(0, 0, 1, 6);

                cells[0, 0].PutValue("历史消耗红包额度明细");
                cells[0, 0].SetStyle(style3);
                cells.SetRowHeight(0, 30);




                cells.SetColumnWidth(0, 30);
                cells[2, 0].PutValue("地区");
                cells[2, 0].SetStyle(style1);
                cells.SetColumnWidth(1, 30);
                cells[2, 1].PutValue("额度明细");
                cells[2, 1].SetStyle(style1);
                cells.SetColumnWidth(2, 30);
                cells[2, 2].PutValue("用户");
                cells[2, 2].SetStyle(style1);
                cells.SetColumnWidth(3, 30);
                cells[2, 3].PutValue("订单号");
                cells[2, 3].SetStyle(style1);
                cells.SetColumnWidth(4, 30);
                cells[2, 4].PutValue("消耗时间");
                cells[2, 4].SetStyle(style1);
                cells.SetColumnWidth(5, 50);
                cells[2, 5].PutValue("专线名称");
                cells[2, 5].SetStyle(style1);





                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        cells[i + 3, 0].PutValue(dt.Rows[i]["dq_mc"].ToString());
                        cells[i + 3, 0].SetStyle(style2);
                        cells[i + 3, 1].PutValue(dt.Rows[i]["num"].ToString());
                        cells[i + 3, 1].SetStyle(style2);
                        cells[i + 3, 2].PutValue(dt.Rows[i]["UserName"].ToString());
                        cells[i + 3, 2].SetStyle(style2);
                        cells[i + 3, 3].PutValue(dt.Rows[i]["ordercode"].ToString());
                        cells[i + 3, 3].SetStyle(style2);
                        cells[i + 3, 4].PutValue(Convert.ToDateTime(dt.Rows[i]["addtime"].ToString()));
                        cells[i + 3, 4].SetStyle(style2);
                        cells[i + 3, 5].PutValue(dt.Rows[i]["UserXM"].ToString());
                        cells[i + 3, 5].SetStyle(style2);


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


    /// <summary>
    /// 历史消耗红包额度明细
    /// </summary>
    /// <param name="pagnum"></param>
    /// <param name="pagesize"></param>
    /// <param name="areacode"></param>
    /// <returns></returns>
    [CSMethod("lsxhlistV2")]
    public object lsxhlistV2(int pagnum, int pagesize, string areacode, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                var cmd = dbc.CreateCommand();
                cmd.CommandText = @" select e.dq_mc,a.OrderCode,a.AddTime,a.Points,b.UserName,c.UserXM from tb_b_order a
left join tb_b_user b on a.BuyUserID = b.UserID
left join tb_b_user c on a.SaleUserID = c.UserID
left join tb_b_dq e on c.DqBm=e.dq_bm  
where a.status = 0 and a.zhifuzt = 1  
 and c.isdonate = 0

 
 
";
                if (areacode != null && areacode != "")
                {
                    cmd.CommandText += " and c.DqBm =" + dbc.ToSqlValue(areacode);
                }
                else
                {

                    cmd.CommandText += " and ( c.DqBm='320500' or  c.DqBm='320400')";
                }




                if (beg != null && beg != "")
                {
                    cmd.CommandText += " and a.addtime >=" + dbc.ToSqlValue(beg);
                }

                if (end != null && end != "")
                {
                    cmd.CommandText += " and a.addtime <=" + dbc.ToSqlValue(end);
                }

                cmd.CommandText += "  and ordercode in (select ordercode from tb_b_redenvelope where type = 5 ";

                if (beg != null && beg != "")
                {
                    cmd.CommandText += " and addtime >=" + dbc.ToSqlValue(beg);
                }

                if (end != null && end != "")
                {
                    cmd.CommandText += " and addtime <=" + dbc.ToSqlValue(end);
                }

                cmd.CommandText += ") order by a.addtime desc";





                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(cmd, pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }


    //历史消耗红包额度明细EXCEL
    [CSMethod("GetTkshListOutListV2", 2)]
    public byte[] GetTkshListOutListV2(JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {

                var cmd = dbc.CreateCommand();
                cmd.CommandText = @" select e.dq_mc,a.OrderCode,a.AddTime,a.Points,b.UserName,c.UserXM from tb_b_order a
left join tb_b_user b on a.BuyUserID = b.UserID
left join tb_b_user c on a.SaleUserID = c.UserID
left join tb_b_dq e on c.DqBm=e.dq_bm  
where a.status = 0 and a.zhifuzt = 1  
 and c.isdonate = 0

 
 
";
                if (jsr["areacode"] != null && jsr["areacode"].ToString() != "")
                {
                    cmd.CommandText += " and c.DqBm =" + dbc.ToSqlValue(jsr["areacode"].ToString());
                }
                else
                {

                    cmd.CommandText += " and ( c.DqBm='320500' or  c.DqBm='320400')";
                }




                if (jsr["beg"] != null && jsr["beg"].ToString() != "")
                {
                    cmd.CommandText += " and a.addtime >=" + dbc.ToSqlValue(Convert.ToDateTime(jsr["beg"].ToString()));
                }

                if (jsr["end"] != null && jsr["end"].ToString() != "")
                {
                    cmd.CommandText += " and a.addtime <=" + dbc.ToSqlValue(Convert.ToDateTime(jsr["end"].ToString()));
                }



                cmd.CommandText += "  and ordercode in (select ordercode from tb_b_redenvelope where type = 5 ";

                if (jsr["beg"] != null && jsr["beg"].ToString() != "")
                {
                    cmd.CommandText += " and addtime >=" + dbc.ToSqlValue(Convert.ToDateTime(jsr["beg"].ToString()));
                }

                if (jsr["end"] != null && jsr["end"].ToString() != "")
                {
                    cmd.CommandText += " and addtime <=" + dbc.ToSqlValue(Convert.ToDateTime(jsr["end"].ToString()));
                }

                cmd.CommandText += ") order by a.addtime desc";
                System.Data.DataTable dt = dbc.ExecuteDataTable(cmd);

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

                cells.Merge(0, 0, 1, 6);

                cells[0, 0].PutValue("历史配比红包专线订单数明细");
                cells[0, 0].SetStyle(style3);
                cells.SetRowHeight(0, 30);




                cells.SetColumnWidth(0, 30);
                cells[2, 0].PutValue("地区");
                cells[2, 0].SetStyle(style1);
                cells.SetColumnWidth(1, 30);
                cells[2, 1].PutValue("订单号");
                cells[2, 1].SetStyle(style1);
                cells.SetColumnWidth(2, 30);
                cells[2, 2].PutValue("时间");
                cells[2, 2].SetStyle(style1);
                cells.SetColumnWidth(3, 30);
                cells[2, 3].PutValue("消费券额");
                cells[2, 3].SetStyle(style1);
                cells.SetColumnWidth(4, 30);
                cells[2, 4].PutValue("用户");
                cells[2, 4].SetStyle(style1);
                cells.SetColumnWidth(5, 50);
                cells[2, 5].PutValue("专线名称");
                cells[2, 5].SetStyle(style1);





                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        cells[i + 3, 0].PutValue(dt.Rows[i]["dq_mc"].ToString());
                        cells[i + 3, 0].SetStyle(style2);
                        cells[i + 3, 1].PutValue(dt.Rows[i]["OrderCode"].ToString());
                        cells[i + 3, 1].SetStyle(style2);
                        cells[i + 3, 2].PutValue(Convert.ToDateTime(dt.Rows[i]["AddTime"].ToString()));
                        cells[i + 3, 2].SetStyle(style2);
                        cells[i + 3, 3].PutValue(dt.Rows[i]["Points"].ToString());
                        cells[i + 3, 3].SetStyle(style2);
                        cells[i + 3, 4].PutValue(dt.Rows[i]["UserName"].ToString());
                        cells[i + 3, 4].SetStyle(style2);
                        cells[i + 3, 5].PutValue(dt.Rows[i]["UserXM"].ToString());
                        cells[i + 3, 5].SetStyle(style2);


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


    /// <summary>
    /// 历史消耗红包额度明细
    /// </summary>
    /// <param name="pagnum"></param>
    /// <param name="pagesize"></param>
    /// <param name="areacode"></param>
    /// <returns></returns>
    [CSMethod("lsxhlistV3")]
    public object lsxhlistV3(int pagnum, int pagesize, string areacode, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                var cmd = dbc.CreateCommand();
                cmd.CommandText = @" select e.dq_mc,a.OrderCode,a.AddTime,a.Points,b.UserName,c.UserXM from tb_b_order a
left join tb_b_user b on a.BuyUserID = b.UserID
left join tb_b_user c on a.SaleUserID = c.UserID
left join tb_b_dq e on c.DqBm=e.dq_bm  
left join tb_b_plattosale f on f.PlatToSaleId=a.PlatToSaleId 
where a.status = 0 and a.zhifuzt = 1 

 
 
 
";

                if (beg != null && beg != "")
                {
                    cmd.CommandText += " and a.addtime >=" + dbc.ToSqlValue(beg);
                }

                if (end != null && end != "")
                {
                    cmd.CommandText += " and a.addtime <=" + dbc.ToSqlValue(end);
                }


                if (areacode != null && areacode != "")
                {
                    cmd.CommandText += " and c.DqBm =" + dbc.ToSqlValue(areacode);
                }
                else
                {

                    cmd.CommandText += " and ( c.DqBm='320500' or  c.DqBm='320400')";
                }


                  cmd.CommandText += @" and c.isdonate = 0
 and f.pointkind = 4 and f.SaleRecordVerifyType = 1 and f.status = 0";



                  cmd.CommandText += "  and ordercode not in (select ordercode from tb_b_redenvelope where type = 5 ";

                if (beg != null && beg != "")
                {
                    cmd.CommandText += " and addtime >=" + dbc.ToSqlValue(beg);
                }

                if (end != null && end != "")
                {
                    cmd.CommandText += " and addtime <=" + dbc.ToSqlValue(end);
                }

                cmd.CommandText += ") order by a.addtime desc";





                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(cmd, pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }


    //历史消耗红包额度明细EXCEL
    [CSMethod("GetTkshListOutListV3", 2)]
    public byte[] GetTkshListOutListV3(JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {

                var cmd = dbc.CreateCommand();
                cmd.CommandText = @" select e.dq_mc,a.OrderCode,a.AddTime,a.Points,b.UserName,c.UserXM from tb_b_order a
left join tb_b_user b on a.BuyUserID = b.UserID
left join tb_b_user c on a.SaleUserID = c.UserID
left join tb_b_dq e on c.DqBm=e.dq_bm  
left join tb_b_plattosale f on f.PlatToSaleId=a.PlatToSaleId 
where a.status = 0 and a.zhifuzt = 1 

 
 
 
";
                if (jsr["beg"] != null && jsr["beg"].ToString() != "")
                {
                    cmd.CommandText += " and a.addtime >=" + dbc.ToSqlValue(Convert.ToDateTime(jsr["beg"].ToString()));
                }

                if (jsr["end"] != null && jsr["end"].ToString() != "")
                {
                    cmd.CommandText += " and a.addtime <=" + dbc.ToSqlValue(Convert.ToDateTime(jsr["end"].ToString()));
                }
                if (jsr["areacode"] != null && jsr["areacode"].ToString() != "")
                {
                    cmd.CommandText += " and c.DqBm =" + dbc.ToSqlValue(jsr["areacode"].ToString());
                }
                else
                {

                    cmd.CommandText += " and ( c.DqBm='320500' or  c.DqBm='320400')";
                }

                cmd.CommandText += @" and c.isdonate = 0
 and f.pointkind = 4 and f.SaleRecordVerifyType = 1 and f.status = 0";






                cmd.CommandText += "  and ordercode not in (select ordercode from tb_b_redenvelope where type = 5 ";

                if (jsr["beg"] != null && jsr["beg"].ToString() != "")
                {
                    cmd.CommandText += " and addtime >=" + dbc.ToSqlValue(Convert.ToDateTime(jsr["beg"].ToString()));
                }

                if (jsr["end"] != null && jsr["end"].ToString() != "")
                {
                    cmd.CommandText += " and addtime <=" + dbc.ToSqlValue(Convert.ToDateTime(jsr["end"].ToString()));
                }

                cmd.CommandText += ") order by a.addtime desc";
                System.Data.DataTable dt = dbc.ExecuteDataTable(cmd);

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

                cells.Merge(0, 0, 1, 6);

                cells[0, 0].PutValue("历史原价购买订单数");
                cells[0, 0].SetStyle(style3);
                cells.SetRowHeight(0, 30);




                cells.SetColumnWidth(0, 30);
                cells[2, 0].PutValue("地区");
                cells[2, 0].SetStyle(style1);
                cells.SetColumnWidth(1, 30);
                cells[2, 1].PutValue("订单号");
                cells[2, 1].SetStyle(style1);
                cells.SetColumnWidth(2, 30);
                cells[2, 2].PutValue("时间");
                cells[2, 2].SetStyle(style1);
                cells.SetColumnWidth(3, 30);
                cells[2, 3].PutValue("消费券额");
                cells[2, 3].SetStyle(style1);
                cells.SetColumnWidth(4, 30);
                cells[2, 4].PutValue("用户");
                cells[2, 4].SetStyle(style1);
                cells.SetColumnWidth(5, 50);
                cells[2, 5].PutValue("专线名称");
                cells[2, 5].SetStyle(style1);





                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        cells[i + 3, 0].PutValue(dt.Rows[i]["dq_mc"].ToString());
                        cells[i + 3, 0].SetStyle(style2);
                        cells[i + 3, 1].PutValue(dt.Rows[i]["OrderCode"].ToString());
                        cells[i + 3, 1].SetStyle(style2);
                        cells[i + 3, 2].PutValue(Convert.ToDateTime(dt.Rows[i]["AddTime"].ToString()));
                        cells[i + 3, 2].SetStyle(style2);
                        cells[i + 3, 3].PutValue(dt.Rows[i]["Points"].ToString());
                        cells[i + 3, 3].SetStyle(style2);
                        cells[i + 3, 4].PutValue(dt.Rows[i]["UserName"].ToString());
                        cells[i + 3, 4].SetStyle(style2);
                        cells[i + 3, 5].PutValue(dt.Rows[i]["UserXM"].ToString());
                        cells[i + 3, 5].SetStyle(style2);


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




}

