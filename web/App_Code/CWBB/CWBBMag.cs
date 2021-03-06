﻿using System;
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
/// <summary>
///CWBBMag 的摘要说明
/// </summary>

[CSClass("CWBBMag")]
public class CWBBMag
{
    #region 市场数据统计表--专线
    [CSMethod("GetZXSJList")]
    public object GetZXSJList(int pagnum, int pagesize, string yhm, string xm, string beg, string end)
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
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.AddTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select a.UserID,a.UserXM,a.UserName,a.Addtime,b.sykkf,c.zsyfq,d.sxyfq,e.sqcs,f.gzs,a.Points,g.sxed,h.ps from tb_b_user a left join 
                                (select sum(Points) as sykkf,UserID from tb_b_platpoints where status=0 group by UserID) b on a.UserID=b.UserID
                                left join
                                (select isNULL(a.points,0)+isnull(b.points,0) as zsyfq,a.UserID from 
                                (select sum(Points) as points,UserID from tb_b_plattosale where   status=0 and pointkind=0 and (DATEDIFF(HOUR,addtime,getDate())<=validHour or validHour=null) group by UserID ) a left join (
                                select sum(Points) as points,SaleUserID from [tb_b_order] where  ZhiFuZT=0 and status=0
                                group by SaleUserID) b  on a.UserID=b.SaleUserID) c on a.UserID=c.UserID
                                left join
                                (select sum(Points) as sxyfq,SaleUserID from tb_b_order where [SaleRecordID] in
                                (select SaleRecordID from  tb_b_salerecord where status=0 and SaleRecordLX=0 and SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9')
                                and status=0 and ZhiFuZT=1 group by SaleUserID) d on a.UserID=d.SaleUserID
                                left join
                                (select count(OrderID) as sqcs,SaleUserID from tb_b_order where [SaleRecordID] in
                                (select SaleRecordID from  tb_b_salerecord where status=0 and SaleRecordLX=0 and SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9')
                                and status=0 and ZhiFuZT=1 group by SaleUserID) e on a.UserID=e.SaleUserID
                                left join(select count(GZ_ID) as gzs,GZUserID from tb_b_user_gz group by GZUserID) f on a.UserID=f.GZUserID
                                left join(select sum(sqjf) as sxed,userId  from tb_b_jfsq where  issq=1 group by userId) g on a.UserID=g.userId
                                left join (select sum(getpoints) as ps,b.carduserid from tb_b_paisong_detail a left join tb_b_paisong b on a.paisongid=b.id 
                                where getstatus=1 and b.status=0 and a.status=0 group by b.carduserid) h on a.UserID=h.carduserid
                                where 1=1 and a.ClientKind=1 and g.sxed>0";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.AddTime desc,a.UserName,a.UserXM", pagesize, ref cp, out ac);

                //dtPage.Columns.Add("dqS");
                //for (int i = 0; i < dtPage.Rows.Count; i++)
                //{
                //    if (dtPage.Rows[i]["DqBm"] != null && dtPage.Rows[i]["DqBm"].ToString() != "")
                //    {
                //        string sql = "select dq_bm from tb_b_dq where dq_bm=(select dq_sj from tb_b_dq where dq_sj<>'000000' and dq_bm=" + dbc.ToSqlValue(dtPage.Rows[i]["DqBm"]) + ")";
                //        DataTable dt = dbc.ExecuteDataTable(sql);
                //        if (dt.Rows.Count > 0)
                //        {
                //            dtPage.Rows[i]["dqS"] = dt.Rows[0][0];
                //        }
                //    }
                //}
                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    [CSMethod("GetZXSJListToFile", 2)]
    public byte[] GetZXSJListToFile(string yhm, string xm, string beg, string end)
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
                cells[0, 0].PutValue("专线");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("账号");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("剩余运费券");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("在售运费券");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("售券额");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("售券次数");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);
                cells[0, 6].PutValue("关注数");
                cells[0, 6].SetStyle(style2);
                cells.SetColumnWidth(6, 20);
                cells[0, 7].PutValue("注册时间");
                cells[0, 7].SetStyle(style2);
                cells.SetColumnWidth(7, 20);
                cells[0, 8].PutValue("账户余额");
                cells[0, 8].SetStyle(style2);
                cells.SetColumnWidth(8, 20);
                cells[0, 9].PutValue("授信额度");
                cells[0, 9].SetStyle(style2);
                cells.SetColumnWidth(9, 20);
                cells[0, 10].PutValue("实际派送运费券");
                cells[0, 10].SetStyle(style2);
                cells.SetColumnWidth(10, 20);


                string where = "";

                if (!string.IsNullOrEmpty(yhm.Trim()))
                {
                    where += " and " + dbc.C_Like("a.UserName", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(xm.Trim()))
                {
                    where += " and " + dbc.C_Like("a.UserXM", xm.Trim(), LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.AddTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select a.UserID,a.UserXM,a.UserName,a.Addtime,b.sykkf,c.zsyfq,d.sxyfq,e.sqcs,f.gzs,a.Points,g.sxed,h.ps from tb_b_user a left join 
                                (select sum(Points) as sykkf,UserID from tb_b_platpoints where status=0 group by UserID) b on a.UserID=b.UserID
                                left join
                                (select isNULL(a.points,0)+isnull(b.points,0) as zsyfq,a.UserID from 
                                (select sum(Points) as points,UserID from tb_b_plattosale where   status=0 and pointkind=0 and (DATEDIFF(HOUR,addtime,getDate())<=validHour or validHour=null) group by UserID ) a left join (
                                select sum(Points) as points,SaleUserID from [tb_b_order] where  ZhiFuZT=0 and status=0
                                group by SaleUserID) b  on a.UserID=b.SaleUserID) c on a.UserID=c.UserID
                                left join
                                (select sum(Points) as sxyfq,SaleUserID from tb_b_order where [SaleRecordID] in
                                (select SaleRecordID from  tb_b_salerecord where status=0 and SaleRecordLX=0 and SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9')
                                and status=0 and ZhiFuZT=1 group by SaleUserID) d on a.UserID=d.SaleUserID
                                left join
                                (select count(OrderID) as sqcs,SaleUserID from tb_b_order where [SaleRecordID] in
                                (select SaleRecordID from  tb_b_salerecord where status=0 and SaleRecordLX=0 and SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9')
                                and status=0 and ZhiFuZT=1 group by SaleUserID) e on a.UserID=e.SaleUserID
                                left join(select count(GZ_ID) as gzs,GZUserID from tb_b_user_gz group by GZUserID) f on a.UserID=f.GZUserID
                                left join(select sum(sqjf) as sxed,userId  from tb_b_jfsq where  issq=1 group by userId) g on a.UserID=g.userId
                                left join (select sum(getpoints) as ps,b.carduserid from tb_b_paisong_detail a left join tb_b_paisong b on a.paisongid=b.id 
                                where getstatus=1 and b.status=0 and a.status=0 group by b.carduserid) h on a.UserID=h.carduserid
                                where 1=1 and a.ClientKind=1 and g.sxed>0 " + where + " order by a.AddTime desc,a.UserName,a.UserXM";
                //开始取分页数据
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = dbc.ExecuteDataTable(str);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["UserXM"]);
                    cells[i + 1, 0].SetStyle(style4);
                    cells[i + 1, 1].PutValue(dt.Rows[i]["UserName"]);
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["sykkf"] != null && dt.Rows[i]["sykkf"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(dt.Rows[i]["sykkf"]);
                    }
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["zsyfq"] != null && dt.Rows[i]["zsyfq"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(dt.Rows[i]["zsyfq"]);
                    }
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["sxyfq"] != null && dt.Rows[i]["sxyfq"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(dt.Rows[i]["sxyfq"]);
                    }
                    cells[i + 1, 4].SetStyle(style4);
                    if (dt.Rows[i]["sqcs"] != null && dt.Rows[i]["sqcs"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(dt.Rows[i]["sqcs"]);
                    }
                    cells[i + 1, 5].SetStyle(style4);
                    if (dt.Rows[i]["gzs"] != null && dt.Rows[i]["gzs"].ToString() != "")
                    {
                        cells[i + 1, 6].PutValue(dt.Rows[i]["gzs"]);
                    }
                    cells[i + 1, 6].SetStyle(style4);
                    cells[i + 1, 7].PutValue(Convert.ToDateTime(dt.Rows[i]["AddTime"]).ToString("yyyy-MM-dd"));
                    cells[i + 1, 7].SetStyle(style4);
                    if (dt.Rows[i]["Points"] != null && dt.Rows[i]["Points"].ToString() != "")
                    {
                        cells[i + 1, 8].PutValue(dt.Rows[i]["Points"]);
                    }
                    cells[i + 1, 8].SetStyle(style4);
                    if (dt.Rows[i]["sxed"] != null && dt.Rows[i]["sxed"].ToString() != "")
                    {
                        cells[i + 1, 9].PutValue(dt.Rows[i]["sxed"]);
                    }
                    cells[i + 1, 9].SetStyle(style4);
                    if (dt.Rows[i]["ps"] != null && dt.Rows[i]["ps"].ToString() != "")
                    {
                        cells[i + 1, 10].PutValue(dt.Rows[i]["ps"]);
                    }
                    cells[i + 1, 10].SetStyle(style4);

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

    [CSMethod("getSQEList")]
    public object getSQEList(int pagnum, int pagesize, string UserID)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string str = @"  select a.*,b.UserName from tb_b_order a 
                                 left join tb_b_user b on a.BuyUserID=b.UserID
                                  where a.[SaleRecordID] in
                                (select SaleRecordID from  tb_b_salerecord where status=0 and SaleRecordLX=0 and SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9')
                                and a.status=0 and a.ZhiFuZT=1 and a.SaleUserID=@UserID  order by a.AddTime desc
                                ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@UserID", UserID);

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

    [CSMethod("getSQEListToFile", 2)]
    public byte[] getSQEListToFile(string UserID)
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
                cells[0, 0].PutValue("订单号");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("三方");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("运费券");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("金额");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("时间");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);


                string str = @"  select a.*,b.UserName from tb_b_order a 
                                 left join tb_b_user b on a.BuyUserID=b.UserID
                                  where a.[SaleRecordID] in
                                (select SaleRecordID from  tb_b_salerecord where status=0 and SaleRecordLX=0 and SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9')
                                and a.status=0 and a.ZhiFuZT=1 and a.SaleUserID=@UserID  order by a.AddTime desc
                                ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                DataTable dt = dbc.ExecuteDataTable(cmd);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["OrderCode"]);
                    cells[i + 1, 0].SetStyle(style4);
                    cells[i + 1, 1].PutValue(dt.Rows[i]["UserName"]);
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["points"] != null && dt.Rows[i]["points"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(dt.Rows[i]["points"]);
                    }
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["Money"] != null && dt.Rows[i]["Money"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(dt.Rows[i]["Money"]);
                    }
                    cells[i + 1, 3].SetStyle(style4);
                    cells[i + 1, 4].PutValue(Convert.ToDateTime(dt.Rows[i]["AddTime"]).ToString("yyyy-MM-dd"));
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

    [CSMethod("getGZSList")]
    public object getGZSList(int pagnum, int pagesize, string UserID)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string str = @" select case when b.ClientKind=1 then b.UserXM when  b.ClientKind=2 then b.UserName else b.UserName END as gzr,
                                a.GZ_TIME from tb_b_user_gz a left join tb_b_user b 
								on a.UserID=b.UserID where a.GZUserID=@UserID order by a.GZ_TIME desc
                                ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@UserID", UserID);

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

    [CSMethod("getGZSListToFile", 2)]
    public byte[] getGZSListToFile(string UserID)
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
                cells[0, 0].PutValue("关注人");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("时间");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);

                string str = @" select case when b.ClientKind=1 then b.UserXM when  b.ClientKind=2 then b.UserName else b.UserName END as gzr,
                                a.GZ_TIME from tb_b_user_gz a left join tb_b_user b 
								on a.UserID=b.UserID where a.GZUserID=@UserID order by a.GZ_TIME desc
                                ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                DataTable dt = dbc.ExecuteDataTable(cmd);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["gzr"]);
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["GZ_TIME"] != null && dt.Rows[i]["GZ_TIME"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(Convert.ToDateTime(dt.Rows[i]["GZ_TIME"]).ToString("yyyy-MM-dd"));
                    }
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
    #endregion

    #region 市场数据统计--三方
    [CSMethod("GetSFSJList")]
    public object GetSFSJList(int pagnum, int pagesize, string yhm, string beg, string end)
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

                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.AddTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select a.UserID,a.UserName,a.Addtime,b.gmyfq,c.ysyyfq,d.gmcs,e.gzs,f.syyfq,g.gqwsy,h.ps from tb_b_user a left join 
                                (select sum(Points) as gmyfq,BuyUserID from tb_b_order where  status=0 and ZhiFuZT=1 group by BuyUserID) b on a.UserID=b.BuyUserID
                                left join 
                                (select sum(Points) as ysyyfq,PayUserID  from tb_b_pay group by PayUserID) c on a.UserID=c.PayUserID
                                left join 
                                (select count(OrderID) as gmcs,BuyUserID from tb_b_order where status=0 and ZhiFuZT=1 group by BuyUserID) d on a.UserID=d.BuyUserID 
                                left join 
                                (select count(GZ_ID) as gzs,UserID from tb_b_user_gz group by UserID) e on a.UserID=e.UserID
                                left join(
                                select sum(a.points) as syyfq,a.UserID from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                                where b.status=0  and a.PointsEndTime>=getDate() and a.status=0
                                group by a.UserID) f on a.UserID=f.UserID
                                left join(
                                select sum(a.points) as gqwsy,a.UserID from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                                where b.status=0  and a.PointsEndTime<getDate() and (a.status=0  or a.status=9)
                                group by a.UserID) g on a.UserID=g.UserID
                                left join (select sum(getpoints) as ps,a.sanfanguserid from tb_b_paisong_detail a left join tb_b_paisong b on a.paisongid=b.id 
                                where getstatus=1 and b.status=0 and a.status=0 group by a.sanfanguserid) h on a.UserID=h.sanfanguserid
                                where 1=1 and a.ClientKind=2";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.AddTime desc,a.UserName,a.UserXM", pagesize, ref cp, out ac);

                //dtPage.Columns.Add("dqS");
                //for (int i = 0; i < dtPage.Rows.Count; i++)
                //{
                //    if (dtPage.Rows[i]["DqBm"] != null && dtPage.Rows[i]["DqBm"].ToString() != "")
                //    {
                //        string sql = "select dq_bm from tb_b_dq where dq_bm=(select dq_sj from tb_b_dq where dq_sj<>'000000' and dq_bm=" + dbc.ToSqlValue(dtPage.Rows[i]["DqBm"]) + ")";
                //        DataTable dt = dbc.ExecuteDataTable(sql);
                //        if (dt.Rows.Count > 0)
                //        {
                //            dtPage.Rows[i]["dqS"] = dt.Rows[0][0];
                //        }
                //    }
                //}
                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    [CSMethod("GetSFSJListToFile", 2)]
    public byte[] GetSFSJListToFile(string yhm, string beg, string end)
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
                cells[0, 0].PutValue("账号");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("剩余运费券");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("过期未使用");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("购买运费券");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("已使用运费券");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("购买次数");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);
                cells[0, 6].PutValue("关注数");
                cells[0, 6].SetStyle(style2);
                cells.SetColumnWidth(6, 20);
                cells[0, 7].PutValue("注册时间");
                cells[0, 7].SetStyle(style2);
                cells.SetColumnWidth(7, 20);
                cells[0, 8].PutValue("派送运费券");
                cells[0, 8].SetStyle(style2);
                cells.SetColumnWidth(8, 20);


                string where = "";

                if (!string.IsNullOrEmpty(yhm.Trim()))
                {
                    where += " and " + dbc.C_Like("a.UserName", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.AddTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select a.UserID,a.UserName,a.Addtime,b.gmyfq,c.ysyyfq,d.gmcs,e.gzs,f.syyfq,g.gqwsy,h.ps from tb_b_user a left join 
                                (select sum(Points) as gmyfq,BuyUserID from tb_b_order where  status=0 and ZhiFuZT=1 group by BuyUserID) b on a.UserID=b.BuyUserID
                                left join 
                                (select sum(Points) as ysyyfq,PayUserID  from tb_b_pay group by PayUserID) c on a.UserID=c.PayUserID
                                left join 
                                (select count(OrderID) as gmcs,BuyUserID from tb_b_order where status=0 and ZhiFuZT=1 group by BuyUserID) d on a.UserID=d.BuyUserID 
                                left join 
                                (select count(GZ_ID) as gzs,UserID from tb_b_user_gz group by UserID) e on a.UserID=e.UserID
                                left join(
                                select sum(a.points) as syyfq,a.UserID from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                                where b.status=0  and a.PointsEndTime>=getDate() and a.status=0
                                group by a.UserID) f on a.UserID=f.UserID
                                left join(
                                select sum(a.points) as gqwsy,a.UserID from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                                where b.status=0  and a.PointsEndTime<getDate() and (a.status=0  or a.status=9)
                                group by a.UserID) g on a.UserID=g.UserID
                                left join (select sum(getpoints) as ps,a.sanfanguserid from tb_b_paisong_detail a left join tb_b_paisong b on a.paisongid=b.id 
                                where getstatus=1 and b.status=0 and a.status=0 group by a.sanfanguserid) h on a.UserID=h.sanfanguserid
                                where 1=1 and a.ClientKind=2" + where + " order by a.AddTime desc,a.UserName,a.UserXM";
                //开始取分页数据
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = dbc.ExecuteDataTable(str);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["UserName"]);
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["syyfq"] != null && dt.Rows[i]["syyfq"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["syyfq"]);
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["gqwsy"] != null && dt.Rows[i]["gqwsy"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(dt.Rows[i]["gqwsy"]);
                    }
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["gmyfq"] != null && dt.Rows[i]["gmyfq"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(dt.Rows[i]["gmyfq"]);
                    }
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["ysyyfq"] != null && dt.Rows[i]["ysyyfq"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(dt.Rows[i]["ysyyfq"]);
                    }
                    cells[i + 1, 4].SetStyle(style4);
                    if (dt.Rows[i]["gmcs"] != null && dt.Rows[i]["gmcs"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(dt.Rows[i]["gmcs"]);
                    }
                    cells[i + 1, 5].SetStyle(style4);
                    if (dt.Rows[i]["gzs"] != null && dt.Rows[i]["gzs"].ToString() != "")
                    {
                        cells[i + 1, 6].PutValue(dt.Rows[i]["gzs"]);
                    }
                    cells[i + 1, 6].SetStyle(style4);
                    cells[i + 1, 7].PutValue(Convert.ToDateTime(dt.Rows[i]["AddTime"]).ToString("yyyy-MM-dd"));
                    cells[i + 1, 7].SetStyle(style4);
                    if (dt.Rows[i]["ps"] != null && dt.Rows[i]["ps"].ToString() != "")
                    {
                        cells[i + 1, 8].PutValue(dt.Rows[i]["ps"]);
                    }
                    cells[i + 1, 8].SetStyle(style4);

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

    [CSMethod("getGMList")]
    public object getGMList(int pagnum, int pagesize, string UserID)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string str = @"  select a.*,b.UserXM from tb_b_order a 
                                 left join tb_b_user b on a.SaleUserID=b.UserID
                                  where  a.status=0 and a.ZhiFuZT=1 and a.BuyUserID=@UserID  order by a.AddTime desc
                                ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@UserID", UserID);

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

    [CSMethod("getPSList")]
    public object getPSList(int pagnum, int pagesize, string UserID)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string str = @" select a.getpoints,b.carduserid,c.UserXM,b.addtime from tb_b_paisong_detail a left join tb_b_paisong b on a.paisongid=b.id 
                                left join tb_b_user c on b.carduserid=c.userid
                                where getstatus=1 and b.status=0 and a.status=0 and a.sanfanguserid=@UserID order by b.addtime desc
                                ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@UserID", UserID);

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

    [CSMethod("getSYList")]
    public object getSYList(string UserID)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string str = @" select a.points,c.UserXM,a.PointsEndTime from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                        left join tb_b_user c on a.CardUserID=c.UserID
                        where b.status=0  and a.PointsEndTime>=getDate()
                        and a.UserID=@UserID order by a.PointsEndTime
                                ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                DataTable dt = dbc.ExecuteDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    [CSMethod("getSYListToFile", 2)]
    public byte[] getSYListToFile(string UserID)
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
                cells[0, 0].PutValue("专线");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("运费券");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("截止日期");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);


                string str = @" select a.points,c.UserXM,a.PointsEndTime from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                        left join tb_b_user c on a.CardUserID=c.UserID
                        where b.status=0  and a.PointsEndTime>=getDate()
                        and a.UserID=@UserID order by a.PointsEndTime
                                ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                DataTable dt = dbc.ExecuteDataTable(cmd);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["UserXM"]);
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["points"] != null && dt.Rows[i]["points"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["points"]);
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    cells[i + 1, 2].PutValue(Convert.ToDateTime(dt.Rows[i]["PointsEndTime"]).ToString("yyyy-MM-dd"));
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

    [CSMethod("getGMListToFile", 2)]
    public byte[] getGMListToFile(string UserID)
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
                cells[0, 0].PutValue("订单号");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("专线");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("运费券");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("金额");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("时间");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);


                string str = @"  select a.*,b.UserXM from tb_b_order a 
                                 left join tb_b_user b on a.SaleUserID=b.UserID
                                  where  a.status=0 and a.ZhiFuZT=1 and a.BuyUserID=@UserID  order by a.AddTime desc
                                ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                DataTable dt = dbc.ExecuteDataTable(cmd);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["OrderCode"]);
                    cells[i + 1, 0].SetStyle(style4);
                    cells[i + 1, 1].PutValue(dt.Rows[i]["UserXM"]);
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["points"] != null && dt.Rows[i]["points"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(dt.Rows[i]["points"]);
                    }
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["Money"] != null && dt.Rows[i]["Money"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(dt.Rows[i]["Money"]);
                    }
                    cells[i + 1, 3].SetStyle(style4);
                    cells[i + 1, 4].PutValue(Convert.ToDateTime(dt.Rows[i]["AddTime"]).ToString("yyyy-MM-dd"));
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

    [CSMethod("getPSListToFile", 2)]
    public byte[] getPSListToFile(string UserID)
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
                cells[0, 0].PutValue("专线");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("运费券");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("时间");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);


                string str = @"  select a.getpoints,b.carduserid,c.UserXM,b.addtime from tb_b_paisong_detail a left join tb_b_paisong b on a.paisongid=b.id 
                                left join tb_b_user c on b.carduserid=c.userid
                                where getstatus=1 and b.status=0 and a.status=0 and a.sanfanguserid=@UserID order by b.addtime desc
                                ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                DataTable dt = dbc.ExecuteDataTable(cmd);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["UserXM"]);
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["getpoints"] != null && dt.Rows[i]["getpoints"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["getpoints"]);
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    cells[i + 1, 2].PutValue(Convert.ToDateTime(dt.Rows[i]["addtime"]).ToString("yyyy-MM-dd"));
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

    [CSMethod("getYSYList")]
    public object getYSYList(int pagnum, int pagesize, string UserID)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;
                string str = @"  select a.Points,a.AddTime,
                                  case when c.ClientKind=1 then c.UserXM when  c.ClientKind=2 then c.UserName else c.UserName END as jydx,
                                  b.UserXM   from tb_b_pay a
                                  left join tb_b_user b on a.CardUserID=b.UserID
                                  left join tb_b_user c on a.ReceiveUserID=c.UserID
                                   where a.PayUserID=@UserID order by AddTime  desc
                                ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@UserID", UserID);
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

    [CSMethod("getYSYListToFile", 2)]
    public byte[] getYSYListToFile(string UserID)
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
                cells[0, 0].PutValue("交易对象");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("运费券");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("时间");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);

                string str = @"  select a.Points,a.AddTime,
                                  case when c.ClientKind=1 then c.UserXM when  c.ClientKind=2 then c.UserName else c.UserName END as jydx,
                                  b.UserXM   from tb_b_pay a
                                  left join tb_b_user b on a.CardUserID=b.UserID
                                  left join tb_b_user c on a.ReceiveUserID=c.UserID
                                   where 
                                a.PayUserID=@UserID order by AddTime  desc
                                ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                DataTable dt = dbc.ExecuteDataTable(cmd);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["jydx"]);
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["Points"] != null && dt.Rows[i]["Points"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["Points"]);
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    cells[i + 1, 2].PutValue(Convert.ToDateTime(dt.Rows[i]["AddTime"]).ToString("yyyy-MM-dd"));
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

    [CSMethod("getGQWSYList")]
    public object getGQWSYList(int pagnum, int pagesize, string UserID)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;
                string str = @" select a.points,c.UserXM,a.PointsEndTime from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                        left join tb_b_user c on a.CardUserID=c.UserID
                        where b.status=0  and a.PointsEndTime<getDate() and (a.status=0 or a.status=9)
                        and a.UserID=@UserID order by a.PointsEndTime
                                ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@UserID", UserID);
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

    [CSMethod("getGQWSYListToFile", 2)]
    public byte[] getGQWSYListToFile(string UserID)
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
                cells[0, 0].PutValue("专线");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("运费券");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("截止时间");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);

                string str = @" select a.points,c.UserXM,a.PointsEndTime from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                        left join tb_b_user c on a.CardUserID=c.UserID
                        where b.status=0  and a.PointsEndTime<getDate() and (a.status=0 or a.status=9)
                        and a.UserID=@UserID order by a.PointsEndTime
                                ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                DataTable dt = dbc.ExecuteDataTable(cmd);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["UserXM"]);
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["points"] != null && dt.Rows[i]["points"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["points"]);
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    cells[i + 1, 2].PutValue(Convert.ToDateTime(dt.Rows[i]["PointsEndTime"]).ToString("yyyy-MM-dd"));
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
    #endregion

    #region 自放券总表
    [CSMethod("GetZXZFList")]
    public object GetZXZFList(int pagnum, int pagesize, string yhm, string xm, string sc, string beg, string end)
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

                if (!string.IsNullOrEmpty(sc.Trim()))
                {
                    where += " and " + dbc.C_Like("a.FromRoute", sc.Trim(), LikeStyle.LeftAndRightLike);
                }

                string sql = @"select max(SaleRecordTime) as maxtime,min(SaleRecordTime) as mintime from tb_b_salerecord where SaleRecordLX!=0 and SaleRecordVerifyType=1 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
                    and status=0  ";
                DataTable sjdt = dbc.ExecuteDataTable(sql);

                string sjstr = "";

                //上架时间
                string sjwhere = ""; string sjwhere1 = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    sjwhere += " and  SaleRecordTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                    sjwhere1 += " and  b.SaleRecordTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                    sjstr += Convert.ToDateTime(beg).ToString("yyyy年MM月dd日");
                }
                else
                {
                    if (sjdt.Rows.Count > 0)
                    {
                        if (sjdt.Rows[0]["mintime"] != null && sjdt.Rows[0]["mintime"].ToString() != "")
                        {
                            sjstr += Convert.ToDateTime(sjdt.Rows[0]["mintime"]).ToString("yyyy年MM月dd日");
                        }
                        else
                        {
                            sjstr += DateTime.Now.ToString("yyyy年MM月dd日");
                        }
                    }
                    else
                    {
                        sjstr += DateTime.Now.ToString("yyyy年MM月dd日");
                    }
                }
                if (!string.IsNullOrEmpty(end))
                {
                    sjwhere += " and SaleRecordTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                    sjwhere1 += " and b.SaleRecordTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                    sjstr += "到" + Convert.ToDateTime(end).ToString("yyyy年MM月dd日");
                }
                else
                {
                    if (sjdt.Rows.Count > 0)
                    {
                        if (sjdt.Rows[0]["maxtime"] != null && sjdt.Rows[0]["maxtime"].ToString() != "")
                        {
                            sjstr += "到" + Convert.ToDateTime(sjdt.Rows[0]["maxtime"]).ToString("yyyy年MM月dd日");
                        }
                        else
                        {
                            sjstr += "到" + DateTime.Now.ToString("yyyy年MM月dd日");
                        }
                    }
                    else
                    {
                        sjstr += "到" + DateTime.Now.ToString("yyyy年MM月dd日");
                    }
                }

                string str = @"select '" + sjstr + @"' as rq,  a.UserID,a.FromRoute,a.UserName,a.UserXM,b.sjdzq,c.ysyq,d.gqwsy,e.qxnwsy,f.sxq,g.zsq from tb_b_user a left join 
                            (select sum(SaleRecordPoints) as sjdzq,SaleRecordUserID from tb_b_salerecord where SaleRecordLX!=0 and SaleRecordVerifyType=1 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
                            and status=0 " + sjwhere + @"  group by SaleRecordUserID) b on a.UserID=b.SaleRecordUserID
                            left join
                            (select sum(Points) as ysyq,CardUserID from tb_b_pay where ReceiveUserID=CardUserID and  mycardId in (select a.mycardId from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                            where b.status=0 " + sjwhere1 + @" and b.SaleRecordLX!=0 and b.SaleRecordVerifyType=1 and b.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1))
                             group by CardUserID) c on a.UserID=c.CardUserID
                             left join
                             (select sum(points) as gqwsy,CardUserID from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                            where b.status=0 " + sjwhere1 + @" and b.SaleRecordLX!=0 and b.SaleRecordVerifyType=1 and b.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
                            and a.PointsEndTime<getDate() and a.status=9 group by CardUserID) d on a.UserID=d.CardUserID
                            left join
                            (select sum(points) as qxnwsy,CardUserID from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                            where b.status=0 " + sjwhere1 + @" and b.SaleRecordLX!=0 and b.SaleRecordVerifyType=1 and b.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1) and a.status=0
                            and a.PointsEndTime>=getDate() group by CardUserID) e on a.UserID=e.CardUserID
                            left join
                            (select isnull(a.sjdzq,0)-isnull(b.sxyfq,0) as sxq,a.SaleRecordUserID from (
                            select sum(SaleRecordPoints) as sjdzq,SaleRecordUserID from tb_b_salerecord where SaleRecordLX!=0 and SaleRecordVerifyType=1 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
                            and status=0 " + sjwhere + @" and DATEDIFF(HOUR,SaleRecordTime,getDate())>validHour  group by SaleRecordUserID ) a 
                            left join (
                            select sum(Points) as sxyfq,SaleUserID from tb_b_order where [SaleRecordID] in
                            (select SaleRecordID from  tb_b_salerecord where status=0 " + sjwhere + @" and SaleRecordLX!=0 and SaleRecordVerifyType=1 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1) 
                            and DATEDIFF(HOUR,SaleRecordTime,getDate())>validHour)
                            and status=0  and ZhiFuZT=1 group by SaleUserID) b on a.SaleRecordUserID=b.SaleUserID) f on a.UserID=f.SaleRecordUserID
                            left join(
							select sum(Points) as zsq, UserID from  tb_b_plattosale where SaleRecordVerifyType=1 and status=0 and [SaleRecordID] in
                            (select SaleRecordID from  tb_b_salerecord where status=0 " + sjwhere + @"  and SaleRecordLX!=0 and SaleRecordVerifyType=1 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1) 
                            and DATEDIFF(HOUR,SaleRecordTime,getDate())<=validHour) group by UserID
							) g on a.UserID=g.UserID
                            where b.sjdzq>0 and a.ClientKind=1 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.AddTime desc,a.UserName,a.UserXM", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    [CSMethod("GetZXZFListToFile", 2)]
    public byte[] GetZXZFListToFile(string yhm, string xm, string sc, string beg, string end)
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
                cells[0, 0].PutValue("日期");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("市场");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("注册账号");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("专线名称");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("上架电子券");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("已使用券");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);
                cells[0, 6].PutValue("过期未使用券");
                cells[0, 6].SetStyle(style2);
                cells.SetColumnWidth(6, 20);
                cells[0, 7].PutValue("在期限内未使用券");
                cells[0, 7].SetStyle(style2);
                cells.SetColumnWidth(7, 20);
                cells[0, 8].PutValue("失效券（下架）");
                cells[0, 8].SetStyle(style2);
                cells.SetColumnWidth(8, 20);
                cells[0, 9].PutValue("在售券");
                cells[0, 9].SetStyle(style2);
                cells.SetColumnWidth(9, 20);


                string where = "";

                if (!string.IsNullOrEmpty(yhm.Trim()))
                {
                    where += " and " + dbc.C_Like("a.UserName", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(xm.Trim()))
                {
                    where += " and " + dbc.C_Like("a.UserXM", xm.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(sc.Trim()))
                {
                    where += " and " + dbc.C_Like("a.FromRoute", sc.Trim(), LikeStyle.LeftAndRightLike);
                }

                string sql = @"select max(SaleRecordTime) as maxtime,min(SaleRecordTime) as mintime from tb_b_salerecord where SaleRecordLX!=0 and SaleRecordVerifyType=1 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
                    and status=0  ";
                DataTable sjdt = dbc.ExecuteDataTable(sql);

                string sjstr = "";

                //上架时间
                string sjwhere = ""; string sjwhere1 = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    sjwhere += " and  SaleRecordTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                    sjwhere1 += " and  b.SaleRecordTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                    sjstr += Convert.ToDateTime(beg).ToString("yyyy年MM月dd日");
                }
                else
                {
                    if (sjdt.Rows.Count > 0)
                    {
                        if (sjdt.Rows[0]["mintime"] != null && sjdt.Rows[0]["mintime"].ToString() != "")
                        {
                            sjstr += Convert.ToDateTime(sjdt.Rows[0]["mintime"]).ToString("yyyy年MM月dd日");
                        }
                        else
                        {
                            sjstr += DateTime.Now.ToString("yyyy年MM月dd日");
                        }
                    }
                    else
                    {
                        sjstr += DateTime.Now.ToString("yyyy年MM月dd日");
                    }
                }
                if (!string.IsNullOrEmpty(end))
                {
                    sjwhere += " and SaleRecordTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                    sjwhere1 += " and b.SaleRecordTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                    sjstr += "到" + Convert.ToDateTime(end).ToString("yyyy年MM月dd日");
                }
                else
                {
                    if (sjdt.Rows.Count > 0)
                    {
                        if (sjdt.Rows[0]["maxtime"] != null && sjdt.Rows[0]["maxtime"].ToString() != "")
                        {
                            sjstr += "到" + Convert.ToDateTime(sjdt.Rows[0]["maxtime"]).ToString("yyyy年MM月dd日");
                        }
                        else
                        {
                            sjstr += "到" + DateTime.Now.ToString("yyyy年MM月dd日");
                        }
                    }
                    else
                    {
                        sjstr += "到" + DateTime.Now.ToString("yyyy年MM月dd日");
                    }
                }

                string str = @"select '" + sjstr + @"' as rq,  a.UserID,a.FromRoute,a.UserName,a.UserXM,b.sjdzq,c.ysyq,d.gqwsy,e.qxnwsy,f.sxq,g.zsq from tb_b_user a left join 
                            (select sum(SaleRecordPoints) as sjdzq,SaleRecordUserID from tb_b_salerecord where SaleRecordLX!=0 and SaleRecordVerifyType=1 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
                            and status=0 " + sjwhere + @"  group by SaleRecordUserID) b on a.UserID=b.SaleRecordUserID
                            left join
                            (select sum(Points) as ysyq,CardUserID from tb_b_pay where ReceiveUserID=CardUserID and  mycardId in (select a.mycardId from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                            where b.status=0 " + sjwhere1 + @" and b.SaleRecordLX!=0 and b.SaleRecordVerifyType=1 and b.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1))
                             group by CardUserID) c on a.UserID=c.CardUserID
                             left join
                             (select sum(points) as gqwsy,CardUserID from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                            where b.status=0 " + sjwhere1 + @" and b.SaleRecordLX!=0 and b.SaleRecordVerifyType=1 and b.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
                            and a.PointsEndTime<getDate() and a.status=9 group by CardUserID) d on a.UserID=d.CardUserID
                            left join
                            (select sum(points) as qxnwsy,CardUserID from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                            where b.status=0 " + sjwhere1 + @" and b.SaleRecordLX!=0 and b.SaleRecordVerifyType=1 and b.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1) and a.status=0
                            and a.PointsEndTime>=getDate() group by CardUserID) e on a.UserID=e.CardUserID
                            left join
                            (select isnull(a.sjdzq,0)-isnull(b.sxyfq,0) as sxq,a.SaleRecordUserID from (
                            select sum(SaleRecordPoints) as sjdzq,SaleRecordUserID from tb_b_salerecord where SaleRecordLX!=0 and SaleRecordVerifyType=1 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
                            and status=0 " + sjwhere + @" and DATEDIFF(HOUR,SaleRecordTime,getDate())>validHour  group by SaleRecordUserID ) a 
                            left join (
                            select sum(Points) as sxyfq,SaleUserID from tb_b_order where [SaleRecordID] in
                            (select SaleRecordID from  tb_b_salerecord where status=0 " + sjwhere + @" and SaleRecordLX!=0 and SaleRecordVerifyType=1 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1) 
                            and DATEDIFF(HOUR,SaleRecordTime,getDate())>validHour)
                            and status=0  and ZhiFuZT=1 group by SaleUserID) b on a.SaleRecordUserID=b.SaleUserID) f on a.UserID=f.SaleRecordUserID
                            left join(
							select sum(Points) as zsq, UserID from  tb_b_plattosale where SaleRecordVerifyType=1 and status=0 and [SaleRecordID] in
                            (select SaleRecordID from  tb_b_salerecord where status=0 " + sjwhere + @"  and SaleRecordLX!=0 and SaleRecordVerifyType=1 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1) 
                            and DATEDIFF(HOUR,SaleRecordTime,getDate())<=validHour) group by UserID
							) g on a.UserID=g.UserID
                            where b.sjdzq>0 and a.ClientKind=1  ";
                str += where;
                //开始取分页数据
                DataTable dt = dbc.ExecuteDataTable(str + " order by a.AddTime desc,a.UserName,a.UserXM");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["rq"]);
                    cells[i + 1, 0].SetStyle(style4);
                    cells[i + 1, 1].PutValue(dt.Rows[i]["FromRoute"]);
                    cells[i + 1, 1].SetStyle(style4);
                    cells[i + 1, 2].PutValue(dt.Rows[i]["UserName"]);
                    cells[i + 1, 2].SetStyle(style4);
                    cells[i + 1, 3].PutValue(dt.Rows[i]["UserXM"]);
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["sjdzq"] != null && dt.Rows[i]["sjdzq"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(dt.Rows[i]["sjdzq"]);
                    }
                    cells[i + 1, 4].SetStyle(style4);
                    if (dt.Rows[i]["ysyq"] != null && dt.Rows[i]["ysyq"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(dt.Rows[i]["ysyq"]);
                    }
                    cells[i + 1, 5].SetStyle(style4);
                    if (dt.Rows[i]["gqwsy"] != null && dt.Rows[i]["gqwsy"].ToString() != "")
                    {
                        cells[i + 1, 6].PutValue(dt.Rows[i]["gqwsy"]);
                    }
                    cells[i + 1, 6].SetStyle(style4);
                    if (dt.Rows[i]["qxnwsy"] != null && dt.Rows[i]["qxnwsy"].ToString() != "")
                    {
                        cells[i + 1, 7].PutValue(dt.Rows[i]["qxnwsy"]);
                    }
                    cells[i + 1, 7].SetStyle(style4);
                    if (dt.Rows[i]["sxq"] != null && dt.Rows[i]["sxq"].ToString() != "")
                    {
                        cells[i + 1, 8].PutValue(dt.Rows[i]["sxq"]);
                    }
                    cells[i + 1, 8].SetStyle(style4);
                    if (dt.Rows[i]["zsq"] != null && dt.Rows[i]["zsq"].ToString() != "")
                    {
                        cells[i + 1, 9].PutValue(dt.Rows[i]["zsq"]);
                    }
                    cells[i + 1, 9].SetStyle(style4);
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

    [CSMethod("getXSZBList")]
    public object getXSZBList(int pagnum, int pagesize, string userId, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;
                string sql = @"select max(SaleRecordTime) as maxtime,min(SaleRecordTime) as mintime from tb_b_salerecord where SaleRecordLX!=0 and SaleRecordVerifyType=1 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
                    and status=0  ";
                DataTable sjdt = dbc.ExecuteDataTable(sql);

                //上架时间
                string sjwhere = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    sjwhere += " and  c.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    sjwhere += " and c.AddTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select a.rq,b.FromRoute,b.UserName,b.UserXM,a.xsdzq,a.hbje,c.xfje,d.gqje,e.wsyje,a.zje,a.pjzk,a.yj from
		                    (select sum(c.Points) as xsdzq,sum(c.Money) as zje,sum(c.CHBMoney) as yj, CONVERT(varchar(100), c.AddTime, 23) as rq,c.SaleUserID,
		                    AVG(d.SaleRecordDiscount) as pjzk,sum(c.redenvelopemoney) as hbje from tb_b_order c 
		                    left join tb_b_salerecord d on c.SaleRecordID=d.SaleRecordID  where d.status=0  and d.SaleRecordLX!=0 and d.SaleRecordVerifyType=1 
		                    and d.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1) 
                            and c.status=0 " + sjwhere + @" and c.ZhiFuZT=1 and c.SaleUserID=@UserID  group by CONVERT(varchar(100), c.AddTime, 23),c.SaleUserID) a 
                            left join tb_b_user b on a.SaleUserID=b.UserID
                            left join 
                            (select sum(c.Money) as xfje,CONVERT(varchar(100), c.AddTime, 23) as rq from tb_b_pay a left join tb_b_mycard b on a.mycardId=b.mycardId
		                    left join tb_b_order c on b.OrderCode=c.OrderCode
		                    left join tb_b_salerecord d on b.SaleRecordID=d.SaleRecordID 
		                    where a.ReceiveUserID=@UserID and c.status=0  and c.ZhiFuZT=1 " + sjwhere + @" and 
		                    d.status=0 and d.SaleRecordLX!=0 and d.SaleRecordVerifyType=1 and d.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
		                    group by CONVERT(varchar(100), c.AddTime, 23)) c on a.rq=c.rq
		                    left join 
		                    (select sum(c.Money) as gqje,CONVERT(varchar(100), c.AddTime, 23) as rq from tb_b_mycard b left join tb_b_order c on b.OrderCode=c.OrderCode
		                    left join tb_b_salerecord d on b.SaleRecordID=d.SaleRecordID 
		                    where c.SaleUserID=@UserID and c.status=0  and c.ZhiFuZT=1 " + sjwhere + @" and b.PointsEndTime<getDate() and b.status=9
		                    and d.status=0 and d.SaleRecordLX!=0 and d.SaleRecordVerifyType=1 and d.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
		                    group by CONVERT(varchar(100), c.AddTime, 23)) d on a.rq=d.rq
		                    left join
		                    (select sum(c.Money) as wsyje,CONVERT(varchar(100), c.AddTime, 23) as rq from tb_b_mycard b left join tb_b_order c on b.OrderCode=c.OrderCode
		                    left join tb_b_salerecord d on b.SaleRecordID=d.SaleRecordID 
		                    where c.SaleUserID=@UserID and c.status=0  and c.ZhiFuZT=1 " + sjwhere + @" and b.PointsEndTime>=getDate() and b.status=0
		                    and d.status=0 and d.SaleRecordLX!=0 and d.SaleRecordVerifyType=1 and d.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
		                    group by CONVERT(varchar(100), c.AddTime, 23)) e on a.rq=e.rq order by rq desc
		                    ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.Add("@UserID", userId);
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

    [CSMethod("getXSZBListToFile", 2)]
    public byte[] getXSZBListToFile(string userId, string beg, string end)
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
                cells[0, 0].PutValue("日期");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("市场");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("注册账号");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("专线名称");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("销售电子券");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("销售（已使用）金额");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);
                cells[0, 6].PutValue("过期金额");
                cells[0, 6].SetStyle(style2);
                cells.SetColumnWidth(6, 20);
                cells[0, 7].PutValue("未使用金额");
                cells[0, 7].SetStyle(style2);
                cells.SetColumnWidth(7, 20);
                cells[0, 8].PutValue("总金额");
                cells[0, 8].SetStyle(style2);
                cells.SetColumnWidth(8, 20);
                cells[0, 9].PutValue("平均折扣");
                cells[0, 9].SetStyle(style2);
                cells.SetColumnWidth(9, 20);
                cells[0, 10].PutValue("佣金");
                cells[0, 10].SetStyle(style2);
                cells.SetColumnWidth(10, 20);
                cells[0, 11].PutValue("红包金额");
                cells[0, 11].SetStyle(style2);
                cells.SetColumnWidth(11, 20);


                string sql = @"select max(SaleRecordTime) as maxtime,min(SaleRecordTime) as mintime from tb_b_salerecord where SaleRecordLX!=0 and SaleRecordVerifyType=1 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
                    and status=0  ";
                DataTable sjdt = dbc.ExecuteDataTable(sql);

                //上架时间
                string sjwhere = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    sjwhere += " and  c.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    sjwhere += " and c.AddTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select a.rq,b.FromRoute,b.UserName,b.UserXM,a.xsdzq,a.hbje,c.xfje,d.gqje,e.wsyje,a.zje,a.pjzk,a.yj from
		                    (select sum(c.Points) as xsdzq,sum(c.Money) as zje,sum(c.CHBMoney) as yj, CONVERT(varchar(100), c.AddTime, 23) as rq,c.SaleUserID,
		                    AVG(d.SaleRecordDiscount) as pjzk,sum(c.redenvelopemoney) as hbje from tb_b_order c 
		                    left join tb_b_salerecord d on c.SaleRecordID=d.SaleRecordID  where d.status=0  and d.SaleRecordLX!=0 and d.SaleRecordVerifyType=1 
		                    and d.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1) 
                            and c.status=0 " + sjwhere + @" and c.ZhiFuZT=1 and c.SaleUserID=@UserID  group by CONVERT(varchar(100), c.AddTime, 23),c.SaleUserID) a 
                            left join tb_b_user b on a.SaleUserID=b.UserID
                            left join 
                            (select sum(c.Money) as xfje,CONVERT(varchar(100), c.AddTime, 23) as rq from tb_b_pay a left join tb_b_mycard b on a.mycardId=b.mycardId
		                    left join tb_b_order c on b.OrderCode=c.OrderCode
		                    left join tb_b_salerecord d on b.SaleRecordID=d.SaleRecordID 
		                    where a.ReceiveUserID=@UserID and c.status=0  and c.ZhiFuZT=1 " + sjwhere + @" and 
		                    d.status=0 and d.SaleRecordLX!=0 and d.SaleRecordVerifyType=1 and d.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
		                    group by CONVERT(varchar(100), c.AddTime, 23)) c on a.rq=c.rq
		                    left join 
		                    (select sum(c.Money) as gqje,CONVERT(varchar(100), c.AddTime, 23) as rq from tb_b_mycard b left join tb_b_order c on b.OrderCode=c.OrderCode
		                    left join tb_b_salerecord d on b.SaleRecordID=d.SaleRecordID 
		                    where c.SaleUserID=@UserID and c.status=0  and c.ZhiFuZT=1 " + sjwhere + @" and b.PointsEndTime<getDate() and b.status=9
		                    and d.status=0 and d.SaleRecordLX!=0 and d.SaleRecordVerifyType=1 and d.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
		                    group by CONVERT(varchar(100), c.AddTime, 23)) d on a.rq=d.rq
		                    left join
		                    (select sum(c.Money) as wsyje,CONVERT(varchar(100), c.AddTime, 23) as rq from tb_b_mycard b left join tb_b_order c on b.OrderCode=c.OrderCode
		                    left join tb_b_salerecord d on b.SaleRecordID=d.SaleRecordID 
		                    where c.SaleUserID=@UserID and c.status=0  and c.ZhiFuZT=1 " + sjwhere + @" and b.PointsEndTime>=getDate() and b.status=0
		                    and d.status=0 and d.SaleRecordLX!=0 and d.SaleRecordVerifyType=1 and d.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
		                    group by CONVERT(varchar(100), c.AddTime, 23)) e on a.rq=e.rq order by rq desc
		                    ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.Add("@UserID", userId);
                //开始取分页数据
                DataTable dt = dbc.ExecuteDataTable(cmd);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["rq"]);
                    cells[i + 1, 0].SetStyle(style4);
                    cells[i + 1, 1].PutValue(dt.Rows[i]["FromRoute"]);
                    cells[i + 1, 1].SetStyle(style4);
                    cells[i + 1, 2].PutValue(dt.Rows[i]["UserName"]);
                    cells[i + 1, 2].SetStyle(style4);
                    cells[i + 1, 3].PutValue(dt.Rows[i]["UserXM"]);
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["xsdzq"] != null && dt.Rows[i]["xsdzq"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(dt.Rows[i]["xsdzq"]);
                    }
                    cells[i + 1, 4].SetStyle(style4);
                    if (dt.Rows[i]["xfje"] != null && dt.Rows[i]["xfje"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(dt.Rows[i]["xfje"]);
                    }
                    cells[i + 1, 5].SetStyle(style4);
                    if (dt.Rows[i]["gqje"] != null && dt.Rows[i]["gqje"].ToString() != "")
                    {
                        cells[i + 1, 6].PutValue(dt.Rows[i]["gqje"]);
                    }
                    cells[i + 1, 6].SetStyle(style4);
                    if (dt.Rows[i]["wsyje"] != null && dt.Rows[i]["wsyje"].ToString() != "")
                    {
                        cells[i + 1, 7].PutValue(dt.Rows[i]["wsyje"]);
                    }
                    cells[i + 1, 7].SetStyle(style4);
                    if (dt.Rows[i]["zje"] != null && dt.Rows[i]["zje"].ToString() != "")
                    {
                        cells[i + 1, 8].PutValue(dt.Rows[i]["zje"]);
                    }
                    cells[i + 1, 8].SetStyle(style4);
                    if (dt.Rows[i]["pjzk"] != null && dt.Rows[i]["pjzk"].ToString() != "")
                    {
                        cells[i + 1, 9].PutValue(dt.Rows[i]["pjzk"]);
                    }
                    cells[i + 1, 9].SetStyle(style4);
                    if (dt.Rows[i]["yj"] != null && dt.Rows[i]["yj"].ToString() != "")
                    {
                        cells[i + 1, 10].PutValue(dt.Rows[i]["yj"]);
                    }
                    cells[i + 1, 10].SetStyle(style4);
                    if (dt.Rows[i]["hbje"] != null && dt.Rows[i]["hbje"].ToString() != "")
                    {
                        cells[i + 1, 11].PutValue(dt.Rows[i]["hbje"]);
                    }
                    cells[i + 1, 11].SetStyle(style4);
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

    [CSMethod("getXSMXList")]
    public object getXSMXList(string userId, string rq)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string str = @"  select a.AddTime,a.Points, case when c.Points>0 then a.Money end as xfje,
		                        case when g.gqje>0 then a.Money end as gqje,case when f.wsyje>0 then a.Money end as wsyje,
                                d.SaleRecordDiscount,e.UserName,c.AddTime as xfrq,a.redenvelopemoney as hbje from tb_b_order a 
		                        left join tb_b_pay c on a.OrderCode=c.OrderCode
		                        left join tb_b_salerecord d on a.SaleRecordID=d.SaleRecordID 
		                        left join tb_b_user e on a.BuyUserID=e.UserID
		                        left join (select b.OrderCode,b.points as wsyje from tb_b_mycard b where status=0 and b.PointsEndTime>=getDate()) f on a.OrderCode=f.OrderCode
		                        left join (select b.OrderCode,b.points as gqje from tb_b_mycard b where status=9 and b.PointsEndTime<getDate()) g on a.OrderCode=g.OrderCode
		                         where a.SaleUserID=@UserID and a.AddTime>='" + Convert.ToDateTime(rq).ToString("yyyy-MM-dd") + @"' and a.AddTime<'" + Convert.ToDateTime(rq).AddDays(1).ToString("yyyy-MM-dd") + @"'
		                         and d.status=0 and d.SaleRecordLX!=0 and d.SaleRecordVerifyType=1 and d.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1) and a.Status=0 and a.ZhiFuZT=1
		                    ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.Add("@UserID", userId);
                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.ExecuteDataTable(cmd);

                return dtPage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    [CSMethod("getXSMXListToFile", 2)]
    public byte[] getXSMXListToFile(string userId, string rq)
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
                cells[0, 0].PutValue("销售日期");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("销售电子券");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("消费（已使用）金额");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("过期金额");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("未使用金额");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("折扣");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);
                cells[0, 6].PutValue("购买三方账号");
                cells[0, 6].SetStyle(style2);
                cells.SetColumnWidth(6, 20);
                cells[0, 7].PutValue("消费日期");
                cells[0, 7].SetStyle(style2);
                cells.SetColumnWidth(7, 20);
                cells[0, 8].PutValue("红包金额");
                cells[0, 8].SetStyle(style2);
                cells.SetColumnWidth(8, 20);


                string str = @"  select a.AddTime,a.Points, case when c.Points>0 then a.Money end as xfje,
		                        case when g.gqje>0 then a.Money end as gqje,case when f.wsyje>0 then a.Money end as wsyje,
                                d.SaleRecordDiscount,e.UserName,c.AddTime as xfrq,a.redenvelopemoney as hbje from tb_b_order a 
		                        left join tb_b_pay c on a.OrderCode=c.OrderCode
		                        left join tb_b_salerecord d on a.SaleRecordID=d.SaleRecordID 
		                        left join tb_b_user e on a.BuyUserID=e.UserID
		                        left join (select b.OrderCode,b.points as wsyje from tb_b_mycard b where status=0 and b.PointsEndTime>=getDate()) f on a.OrderCode=f.OrderCode
		                        left join (select b.OrderCode,b.points as gqje from tb_b_mycard b where status=9 and b.PointsEndTime<getDate()) g on a.OrderCode=g.OrderCode
		                         where a.SaleUserID=@UserID and a.AddTime>='" + Convert.ToDateTime(rq).ToString("yyyy-MM-dd") + @"' and a.AddTime<'" + Convert.ToDateTime(rq).AddDays(1).ToString("yyyy-MM-dd") + @"'
		                         and d.status=0 and d.SaleRecordLX!=0 and d.SaleRecordVerifyType=1 and d.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1) and a.Status=0 and a.ZhiFuZT=1
		                    ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.Add("@UserID", userId);
                DataTable dt = dbc.ExecuteDataTable(cmd);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["AddTime"] != null && dt.Rows[i]["AddTime"].ToString() != "")
                    {
                        cells[i + 1, 0].PutValue(Convert.ToDateTime(dt.Rows[i]["AddTime"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["Points"] != null && dt.Rows[i]["Points"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["Points"]);
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["xfje"] != null && dt.Rows[i]["xfje"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(dt.Rows[i]["xfje"]);
                    }
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["gqje"] != null && dt.Rows[i]["gqje"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(dt.Rows[i]["gqje"]);
                    }
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["wsyje"] != null && dt.Rows[i]["wsyje"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(dt.Rows[i]["wsyje"]);
                    }
                    cells[i + 1, 4].SetStyle(style4);

                    if (dt.Rows[i]["SaleRecordDiscount"] != null && dt.Rows[i]["SaleRecordDiscount"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(dt.Rows[i]["SaleRecordDiscount"]);
                    }
                    cells[i + 1, 5].SetStyle(style4);
                    if (dt.Rows[i]["UserName"] != null && dt.Rows[i]["UserName"].ToString() != "")
                    {
                        cells[i + 1, 6].PutValue(dt.Rows[i]["UserName"]);
                    }
                    cells[i + 1, 6].SetStyle(style4);
                    if (dt.Rows[i]["xfrq"] != null && dt.Rows[i]["xfrq"].ToString() != "")
                    {
                        cells[i + 1, 7].PutValue(Convert.ToDateTime(dt.Rows[i]["xfrq"]).ToString("yyyy-MM-dd"));
                    }
                    cells[i + 1, 7].SetStyle(style4);
                    if (dt.Rows[i]["hbje"] != null && dt.Rows[i]["hbje"].ToString() != "")
                    {
                        cells[i + 1, 8].PutValue(dt.Rows[i]["hbje"]);
                    }
                    cells[i + 1, 8].SetStyle(style4);
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
    #endregion

    #region 耗材券总表
    [CSMethod("GetHCQList")]
    public object GetHCQList(int pagnum, int pagesize, string yhm, string xm, string sc, string beg, string end)
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

                if (!string.IsNullOrEmpty(sc.Trim()))
                {
                    where += " and " + dbc.C_Like("a.FromRoute", sc.Trim(), LikeStyle.LeftAndRightLike);
                }

                string sql = @"select max(SaleRecordTime) as maxtime,min(SaleRecordTime) as mintime from tb_b_salerecord where SaleRecordLX!=0 and SaleRecordVerifyType=1 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
                    and status=0  ";
                DataTable sjdt = dbc.ExecuteDataTable(sql);

                string sjstr = "";

                //上架时间
                string sjwhere = ""; string sjwhere1 = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    sjwhere += " and  SaleRecordTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                    sjwhere1 += " and  b.SaleRecordTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                    sjstr += Convert.ToDateTime(beg).ToString("yyyy年MM月dd日");
                }
                else
                {
                    if (sjdt.Rows.Count > 0)
                    {
                        if (sjdt.Rows[0]["mintime"] != null && sjdt.Rows[0]["mintime"].ToString() != "")
                        {
                            sjstr += Convert.ToDateTime(sjdt.Rows[0]["mintime"]).ToString("yyyy年MM月dd日");
                        }
                        else
                        {
                            sjstr += DateTime.Now.ToString("yyyy年MM月dd日");
                        }
                    }
                    else
                    {
                        sjstr += DateTime.Now.ToString("yyyy年MM月dd日");
                    }
                }
                if (!string.IsNullOrEmpty(end))
                {
                    sjwhere += " and SaleRecordTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                    sjwhere1 += " and b.SaleRecordTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                    sjstr += "到" + Convert.ToDateTime(end).ToString("yyyy年MM月dd日");
                }
                else
                {
                    if (sjdt.Rows.Count > 0)
                    {
                        if (sjdt.Rows[0]["maxtime"] != null && sjdt.Rows[0]["maxtime"].ToString() != "")
                        {
                            sjstr += "到" + Convert.ToDateTime(sjdt.Rows[0]["maxtime"]).ToString("yyyy年MM月dd日");
                        }
                        else
                        {
                            sjstr += "到" + DateTime.Now.ToString("yyyy年MM月dd日");
                        }
                    }
                    else
                    {
                        sjstr += "到" + DateTime.Now.ToString("yyyy年MM月dd日");
                    }
                }

                string str = @"select '" + sjstr + @"' as rq,  a.UserID,a.FromRoute,a.UserName,a.UserXM,b.sjdzq,c.ysyq,d.gqwsy,e.qxnwsy,f.sxq,g.zsq,h.wsj,i.xj from tb_b_user a left join 
                            (select sum(SaleRecordPoints) as sjdzq,SaleRecordUserID from tb_b_salerecord where SaleRecordLX=0 and SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9'
                            and status=0 " + sjwhere + @"  group by SaleRecordUserID) b on a.UserID=b.SaleRecordUserID
                            left join
                            (select sum(Points) as ysyq,CardUserID from tb_b_pay where ReceiveUserID=CardUserID and  mycardId in (select a.mycardId from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                            where b.status=0 " + sjwhere1 + @" and b.SaleRecordLX=0 and b.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9')
                             group by CardUserID) c on a.UserID=c.CardUserID
                             left join
                             (select sum(points) as gqwsy,CardUserID from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                            where b.status=0 " + sjwhere1 + @" and b.SaleRecordLX=0 and b.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9'
                            and a.PointsEndTime<getDate() and a.status=9 group by CardUserID) d on a.UserID=d.CardUserID
                            left join
                            (select sum(points) as qxnwsy,CardUserID from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                            where b.status=0 " + sjwhere1 + @" and b.SaleRecordLX=0 and b.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9' and a.status=0
                            and a.PointsEndTime>=getDate() group by CardUserID) e on a.UserID=e.CardUserID
                            left join
                            (select isnull(a.sjdzq,0)-isnull(b.sxyfq,0) as sxq,a.SaleRecordUserID from (
                            select sum(SaleRecordPoints) as sjdzq,SaleRecordUserID from tb_b_salerecord where SaleRecordLX=0 and SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9'
                            and status=0 " + sjwhere + @" and DATEDIFF(HOUR,SaleRecordTime,getDate())>validHour  group by SaleRecordUserID ) a 
                            left join (
                            select sum(Points) as sxyfq,SaleUserID from tb_b_order where [SaleRecordID] in
                            (select SaleRecordID from  tb_b_salerecord where status=0 " + sjwhere + @" and SaleRecordLX=0 and SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9'
                            and DATEDIFF(HOUR,SaleRecordTime,getDate())>validHour)
                            and status=0  and ZhiFuZT=1 group by SaleUserID) b on a.SaleRecordUserID=b.SaleUserID) f on a.UserID=f.SaleRecordUserID
                            left join(
							select sum(Points) as zsq, UserID from  tb_b_plattosale where [SaleRecordID] in
                            (select SaleRecordID from  tb_b_salerecord where status=0 " + sjwhere + @"  and SaleRecordLX=0 and SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9' 
                            and DATEDIFF(HOUR,SaleRecordTime,getDate())<=validHour) group by UserID
							) g on a.UserID=g.UserID
                            left join (select sum(Points) as wsj,UserID from tb_b_platpoints where status=0 group by UserID) h on a.UserID=h.UserID
                            left join (select sum(Points) as xj,UserID from tb_b_xj where status=0 and addtime>='2018-12-30 09:00:00'  group by UserID) i on a.UserID=i.UserID
                            where a.ClientKind=1 and b.sjdzq>0";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.AddTime desc,a.UserName,a.UserXM", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    [CSMethod("GetHCQListToFile", 2)]
    public byte[] GetHCQListToFile(string yhm, string xm, string sc, string beg, string end)
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
                cells[0, 0].PutValue("日期");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("市场");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("注册账号");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("专线名称");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("上架电子券");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("已使用券");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);
                cells[0, 6].PutValue("过期未使用券");
                cells[0, 6].SetStyle(style2);
                cells.SetColumnWidth(6, 20);
                cells[0, 7].PutValue("在期限内未使用券");
                cells[0, 7].SetStyle(style2);
                cells.SetColumnWidth(7, 20);
                cells[0, 8].PutValue("失效券（下架）");
                cells[0, 8].SetStyle(style2);
                cells.SetColumnWidth(8, 20);
                cells[0, 9].PutValue("在售券");
                cells[0, 9].SetStyle(style2);
                cells.SetColumnWidth(9, 20);
                cells[0, 10].PutValue("未上架");
                cells[0, 10].SetStyle(style2);
                cells.SetColumnWidth(10, 20);
                cells[0, 11].PutValue("下架券");
                cells[0, 11].SetStyle(style2);
                cells.SetColumnWidth(11, 20);


                string where = "";

                if (!string.IsNullOrEmpty(yhm.Trim()))
                {
                    where += " and " + dbc.C_Like("a.UserName", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(xm.Trim()))
                {
                    where += " and " + dbc.C_Like("a.UserXM", xm.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(sc.Trim()))
                {
                    where += " and " + dbc.C_Like("a.FromRoute", sc.Trim(), LikeStyle.LeftAndRightLike);
                }

                string sql = @"select max(SaleRecordTime) as maxtime,min(SaleRecordTime) as mintime from tb_b_salerecord where SaleRecordLX!=0 and SaleRecordVerifyType=1 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
                    and status=0  ";
                DataTable sjdt = dbc.ExecuteDataTable(sql);

                string sjstr = "";

                //上架时间
                string sjwhere = ""; string sjwhere1 = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    sjwhere += " and  SaleRecordTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                    sjwhere1 += " and  b.SaleRecordTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                    sjstr += Convert.ToDateTime(beg).ToString("yyyy年MM月dd日");
                }
                else
                {
                    if (sjdt.Rows.Count > 0)
                    {
                        if (sjdt.Rows[0]["mintime"] != null && sjdt.Rows[0]["mintime"].ToString() != "")
                        {
                            sjstr += Convert.ToDateTime(sjdt.Rows[0]["mintime"]).ToString("yyyy年MM月dd日");
                        }
                        else
                        {
                            sjstr += DateTime.Now.ToString("yyyy年MM月dd日");
                        }
                    }
                    else
                    {
                        sjstr += DateTime.Now.ToString("yyyy年MM月dd日");
                    }
                }
                if (!string.IsNullOrEmpty(end))
                {
                    sjwhere += " and SaleRecordTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                    sjwhere1 += " and b.SaleRecordTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                    sjstr += "到" + Convert.ToDateTime(end).ToString("yyyy年MM月dd日");
                }
                else
                {
                    if (sjdt.Rows.Count > 0)
                    {
                        if (sjdt.Rows[0]["maxtime"] != null && sjdt.Rows[0]["maxtime"].ToString() != "")
                        {
                            sjstr += "到" + Convert.ToDateTime(sjdt.Rows[0]["maxtime"]).ToString("yyyy年MM月dd日");
                        }
                        else
                        {
                            sjstr += "到" + DateTime.Now.ToString("yyyy年MM月dd日");
                        }
                    }
                    else
                    {
                        sjstr += "到" + DateTime.Now.ToString("yyyy年MM月dd日");
                    }
                }

                string str = @"select '" + sjstr + @"' as rq,  a.UserID,a.FromRoute,a.UserName,a.UserXM,b.sjdzq,c.ysyq,d.gqwsy,e.qxnwsy,f.sxq,g.zsq,h.wsj,i.xj from tb_b_user a left join 
                            (select sum(SaleRecordPoints) as sjdzq,SaleRecordUserID from tb_b_salerecord where SaleRecordLX=0 and SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9'
                            and status=0 " + sjwhere + @"  group by SaleRecordUserID) b on a.UserID=b.SaleRecordUserID
                            left join
                            (select sum(Points) as ysyq,CardUserID from tb_b_pay where ReceiveUserID=CardUserID and  mycardId in (select a.mycardId from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                            where b.status=0 " + sjwhere1 + @" and b.SaleRecordLX=0 and b.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9')
                             group by CardUserID) c on a.UserID=c.CardUserID
                             left join
                             (select sum(points) as gqwsy,CardUserID from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                            where b.status=0 " + sjwhere1 + @" and b.SaleRecordLX=0 and b.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9'
                            and a.PointsEndTime<getDate() and a.status=9 group by CardUserID) d on a.UserID=d.CardUserID
                            left join
                            (select sum(points) as qxnwsy,CardUserID from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                            where b.status=0 " + sjwhere1 + @" and b.SaleRecordLX=0 and b.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9' and a.status=0
                            and a.PointsEndTime>=getDate() group by CardUserID) e on a.UserID=e.CardUserID
                            left join
                            (select isnull(a.sjdzq,0)-isnull(b.sxyfq,0) as sxq,a.SaleRecordUserID from (
                            select sum(SaleRecordPoints) as sjdzq,SaleRecordUserID from tb_b_salerecord where SaleRecordLX=0 and SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9'
                            and status=0 " + sjwhere + @" and DATEDIFF(HOUR,SaleRecordTime,getDate())>validHour  group by SaleRecordUserID ) a 
                            left join (
                            select sum(Points) as sxyfq,SaleUserID from tb_b_order where [SaleRecordID] in
                            (select SaleRecordID from  tb_b_salerecord where status=0 " + sjwhere + @" and SaleRecordLX=0 and SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9'
                            and DATEDIFF(HOUR,SaleRecordTime,getDate())>validHour)
                            and status=0  and ZhiFuZT=1 group by SaleUserID) b on a.SaleRecordUserID=b.SaleUserID) f on a.UserID=f.SaleRecordUserID
                            left join(
							select sum(Points) as zsq, UserID from  tb_b_plattosale where [SaleRecordID] in
                            (select SaleRecordID from  tb_b_salerecord where status=0 " + sjwhere + @"  and SaleRecordLX=0 and SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9' 
                            and DATEDIFF(HOUR,SaleRecordTime,getDate())<=validHour) group by UserID
							) g on a.UserID=g.UserID
                            left join (select sum(Points) as wsj,UserID from tb_b_platpoints where status=0 group by UserID) h on a.UserID=h.UserID
                            left join (select sum(Points) as xj,UserID from tb_b_xj where status=0 and addtime>='2018-12-30 09:00:00' group by UserID) i on a.UserID=i.UserID
                            where a.ClientKind=1 and b.sjdzq>0";
                str += where;

                //开始取分页数据
                DataTable dt = dbc.ExecuteDataTable(str + " order by a.AddTime desc,a.UserName,a.UserXM");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["rq"]);
                    cells[i + 1, 0].SetStyle(style4);
                    cells[i + 1, 1].PutValue(dt.Rows[i]["FromRoute"]);
                    cells[i + 1, 1].SetStyle(style4);
                    cells[i + 1, 2].PutValue(dt.Rows[i]["UserName"]);
                    cells[i + 1, 2].SetStyle(style4);
                    cells[i + 1, 3].PutValue(dt.Rows[i]["UserXM"]);
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["sjdzq"] != null && dt.Rows[i]["sjdzq"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(dt.Rows[i]["sjdzq"]);
                    }
                    cells[i + 1, 4].SetStyle(style4);
                    if (dt.Rows[i]["ysyq"] != null && dt.Rows[i]["ysyq"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(dt.Rows[i]["ysyq"]);
                    }
                    cells[i + 1, 5].SetStyle(style4);
                    if (dt.Rows[i]["gqwsy"] != null && dt.Rows[i]["gqwsy"].ToString() != "")
                    {
                        cells[i + 1, 6].PutValue(dt.Rows[i]["gqwsy"]);
                    }
                    cells[i + 1, 6].SetStyle(style4);
                    if (dt.Rows[i]["qxnwsy"] != null && dt.Rows[i]["qxnwsy"].ToString() != "")
                    {
                        cells[i + 1, 7].PutValue(dt.Rows[i]["qxnwsy"]);
                    }
                    cells[i + 1, 7].SetStyle(style4);
                    if (dt.Rows[i]["sxq"] != null && dt.Rows[i]["sxq"].ToString() != "")
                    {
                        cells[i + 1, 8].PutValue(dt.Rows[i]["sxq"]);
                    }
                    cells[i + 1, 8].SetStyle(style4);
                    if (dt.Rows[i]["zsq"] != null && dt.Rows[i]["zsq"].ToString() != "")
                    {
                        cells[i + 1, 9].PutValue(dt.Rows[i]["zsq"]);
                    }
                    cells[i + 1, 9].SetStyle(style4);
                    if (dt.Rows[i]["wsj"] != null && dt.Rows[i]["wsj"].ToString() != "")
                    {
                        cells[i + 1, 10].PutValue(dt.Rows[i]["wsj"]);
                    }
                    cells[i + 1, 10].SetStyle(style4);
                    if (dt.Rows[i]["xj"] != null && dt.Rows[i]["xj"].ToString() != "")
                    {
                        cells[i + 1, 11].PutValue(dt.Rows[i]["xj"]);
                    }
                    cells[i + 1, 11].SetStyle(style4);
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

    [CSMethod("getHCQXSZBList")]
    public object getHCQXSZBList(int pagnum, int pagesize, string userId, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;
                string sql = @"select max(SaleRecordTime) as maxtime,min(SaleRecordTime) as mintime from tb_b_salerecord where SaleRecordLX!=0 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
                    and status=0  ";
                DataTable sjdt = dbc.ExecuteDataTable(sql);

                //上架时间
                string sjwhere = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    sjwhere += " and  c.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    sjwhere += " and c.AddTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select a.rq,b.FromRoute,b.UserName,b.UserXM,a.xsdzq,c.xfje,d.gqje,e.wsyje,a.zje,a.pjzk,a.yj from
		                    (select sum(c.Points) as xsdzq,sum(c.Money) as zje,sum(c.CHBMoney) as yj, CONVERT(varchar(100), c.AddTime, 23) as rq,c.SaleUserID,
		                    AVG(d.SaleRecordDiscount) as pjzk from tb_b_order c 
		                    left join tb_b_salerecord d on c.SaleRecordID=d.SaleRecordID  where d.status=0  and d.SaleRecordLX=0 
		                    and d.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9' 
                            and c.status=0 " + sjwhere + @" and c.ZhiFuZT=1 and c.SaleUserID=@UserID  group by CONVERT(varchar(100), c.AddTime, 23),c.SaleUserID) a 
                            left join tb_b_user b on a.SaleUserID=b.UserID
                            left join 
                            (select sum(c.Money) as xfje,CONVERT(varchar(100), c.AddTime, 23) as rq from tb_b_pay a left join tb_b_mycard b on a.mycardId=b.mycardId
		                    left join tb_b_order c on b.OrderCode=c.OrderCode
		                    left join tb_b_salerecord d on b.SaleRecordID=d.SaleRecordID 
		                    where a.ReceiveUserID=@UserID and c.status=0  and c.ZhiFuZT=1 " + sjwhere + @" and 
		                    d.status=0 and d.SaleRecordLX=0 and d.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9'
		                    group by CONVERT(varchar(100), c.AddTime, 23)) c on a.rq=c.rq
		                    left join 
		                    (select sum(c.Money) as gqje,CONVERT(varchar(100), c.AddTime, 23) as rq from tb_b_mycard b left join tb_b_order c on b.OrderCode=c.OrderCode
		                    left join tb_b_salerecord d on b.SaleRecordID=d.SaleRecordID 
		                    where b.status=9 and c.SaleUserID=@UserID and c.status=0  and c.ZhiFuZT=1 " + sjwhere + @" and b.PointsEndTime<getDate()
		                    and d.status=0 and d.SaleRecordLX=0 and d.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9'
		                    group by CONVERT(varchar(100), c.AddTime, 23)) d on a.rq=d.rq
		                    left join
		                    (select sum(c.Money) as wsyje,CONVERT(varchar(100), c.AddTime, 23) as rq from tb_b_mycard b left join tb_b_order c on b.OrderCode=c.OrderCode
		                    left join tb_b_salerecord d on b.SaleRecordID=d.SaleRecordID 
		                    where c.SaleUserID=@UserID and c.status=0  and c.ZhiFuZT=1 " + sjwhere + @" and b.PointsEndTime>=getDate() and b.status=0
		                    and d.status=0 and d.SaleRecordLX=0 and d.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9'
		                    group by CONVERT(varchar(100), c.AddTime, 23)) e on a.rq=e.rq order by rq
		                    ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.Add("@UserID", userId);
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

    [CSMethod("getHCQXSZBListToFile", 2)]
    public byte[] getHCQXSZBListToFile(string userId, string beg, string end)
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
                cells[0, 0].PutValue("日期");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("市场");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("注册账号");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("专线名称");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("销售电子券");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("销售（已使用）金额");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);
                cells[0, 6].PutValue("过期金额");
                cells[0, 6].SetStyle(style2);
                cells.SetColumnWidth(6, 20);
                cells[0, 7].PutValue("未使用金额");
                cells[0, 7].SetStyle(style2);
                cells.SetColumnWidth(7, 20);
                cells[0, 8].PutValue("总金额");
                cells[0, 8].SetStyle(style2);
                cells.SetColumnWidth(8, 20);
                cells[0, 9].PutValue("平均折扣");
                cells[0, 9].SetStyle(style2);
                cells.SetColumnWidth(9, 20);
                cells[0, 10].PutValue("佣金");
                cells[0, 10].SetStyle(style2);
                cells.SetColumnWidth(10, 20);


                string sql = @"select max(SaleRecordTime) as maxtime,min(SaleRecordTime) as mintime from tb_b_salerecord where SaleRecordLX!=0 and SaleRecordVerifyType=1 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
                    and status=0  ";
                DataTable sjdt = dbc.ExecuteDataTable(sql);

                //上架时间
                string sjwhere = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    sjwhere += " and  c.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    sjwhere += " and c.AddTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select a.rq,b.FromRoute,b.UserName,b.UserXM,a.xsdzq,c.xfje,d.gqje,e.wsyje,a.zje,a.pjzk,a.yj from
		                    (select sum(c.Points) as xsdzq,sum(c.Money) as zje,sum(c.CHBMoney) as yj, CONVERT(varchar(100), c.AddTime, 23) as rq,c.SaleUserID,
		                    AVG(d.SaleRecordDiscount) as pjzk from tb_b_order c 
		                    left join tb_b_salerecord d on c.SaleRecordID=d.SaleRecordID  where d.status=0  and d.SaleRecordLX=0 
		                    and d.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9' 
                            and c.status=0 " + sjwhere + @" and c.ZhiFuZT=1 and c.SaleUserID=@UserID  group by CONVERT(varchar(100), c.AddTime, 23),c.SaleUserID) a 
                            left join tb_b_user b on a.SaleUserID=b.UserID
                            left join 
                            (select sum(c.Money) as xfje,CONVERT(varchar(100), c.AddTime, 23) as rq from tb_b_pay a left join tb_b_mycard b on a.mycardId=b.mycardId
		                    left join tb_b_order c on b.OrderCode=c.OrderCode
		                    left join tb_b_salerecord d on b.SaleRecordID=d.SaleRecordID 
		                    where a.ReceiveUserID=@UserID and c.status=0  and c.ZhiFuZT=1 " + sjwhere + @" and 
		                    d.status=0 and d.SaleRecordLX=0 and d.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9'
		                    group by CONVERT(varchar(100), c.AddTime, 23)) c on a.rq=c.rq
		                    left join 
		                    (select sum(c.Money) as gqje,CONVERT(varchar(100), c.AddTime, 23) as rq from tb_b_mycard b left join tb_b_order c on b.OrderCode=c.OrderCode
		                    left join tb_b_salerecord d on b.SaleRecordID=d.SaleRecordID 
		                    where c.SaleUserID=@UserID and c.status=0  and c.ZhiFuZT=1 " + sjwhere + @" and b.PointsEndTime<getDate() and b.status=9
		                    and d.status=0 and d.SaleRecordLX=0 and d.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9'
		                    group by CONVERT(varchar(100), c.AddTime, 23)) d on a.rq=d.rq
		                    left join
		                    (select sum(c.Money) as wsyje,CONVERT(varchar(100), c.AddTime, 23) as rq from tb_b_mycard b left join tb_b_order c on b.OrderCode=c.OrderCode
		                    left join tb_b_salerecord d on b.SaleRecordID=d.SaleRecordID 
		                    where c.SaleUserID=@UserID and c.status=0  and c.ZhiFuZT=1 " + sjwhere + @" and b.PointsEndTime>=getDate() and b.status=0
		                    and d.status=0 and d.SaleRecordLX=0 and d.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9'
		                    group by CONVERT(varchar(100), c.AddTime, 23)) e on a.rq=e.rq order by rq
		                    ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.Add("@UserID", userId);
                //开始取分页数据
                DataTable dt = dbc.ExecuteDataTable(cmd);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["rq"]);
                    cells[i + 1, 0].SetStyle(style4);
                    cells[i + 1, 1].PutValue(dt.Rows[i]["FromRoute"]);
                    cells[i + 1, 1].SetStyle(style4);
                    cells[i + 1, 2].PutValue(dt.Rows[i]["UserName"]);
                    cells[i + 1, 2].SetStyle(style4);
                    cells[i + 1, 3].PutValue(dt.Rows[i]["UserXM"]);
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["xsdzq"] != null && dt.Rows[i]["xsdzq"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(dt.Rows[i]["xsdzq"]);
                    }
                    cells[i + 1, 4].SetStyle(style4);
                    if (dt.Rows[i]["xfje"] != null && dt.Rows[i]["xfje"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(dt.Rows[i]["xfje"]);
                    }
                    cells[i + 1, 5].SetStyle(style4);
                    if (dt.Rows[i]["gqje"] != null && dt.Rows[i]["gqje"].ToString() != "")
                    {
                        cells[i + 1, 6].PutValue(dt.Rows[i]["gqje"]);
                    }
                    cells[i + 1, 6].SetStyle(style4);
                    if (dt.Rows[i]["wsyje"] != null && dt.Rows[i]["wsyje"].ToString() != "")
                    {
                        cells[i + 1, 7].PutValue(dt.Rows[i]["wsyje"]);
                    }
                    cells[i + 1, 7].SetStyle(style4);
                    if (dt.Rows[i]["zje"] != null && dt.Rows[i]["zje"].ToString() != "")
                    {
                        cells[i + 1, 8].PutValue(dt.Rows[i]["zje"]);
                    }
                    cells[i + 1, 8].SetStyle(style4);
                    if (dt.Rows[i]["pjzk"] != null && dt.Rows[i]["pjzk"].ToString() != "")
                    {
                        cells[i + 1, 9].PutValue(dt.Rows[i]["pjzk"]);
                    }
                    cells[i + 1, 9].SetStyle(style4);
                    if (dt.Rows[i]["yj"] != null && dt.Rows[i]["yj"].ToString() != "")
                    {
                        cells[i + 1, 10].PutValue(dt.Rows[i]["yj"]);
                    }
                    cells[i + 1, 10].SetStyle(style4);
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

    [CSMethod("getHCQXSMXList")]
    public object getHCQXSMXList(string userId, string rq)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string str = @"  select a.AddTime,a.Points, case when c.Points>0 then a.Money end as xfje,
		                        case when g.gqje>0 then a.Money end as gqje,case when f.wsyje>0 then a.Money end as wsyje,
                                d.SaleRecordDiscount,e.UserName,c.AddTime as xfrq  from tb_b_order a 
		                        left join tb_b_pay c on a.OrderCode=c.OrderCode
		                        left join tb_b_salerecord d on a.SaleRecordID=d.SaleRecordID 
		                        left join tb_b_user e on a.BuyUserID=e.UserID
		                        left join (select b.OrderCode,b.points as wsyje from tb_b_mycard b where status=0 and b.PointsEndTime>=getDate()) f on a.OrderCode=f.OrderCode
		                        left join (select b.OrderCode,b.points as gqje from tb_b_mycard b where status=9 and b.PointsEndTime<getDate()) g on a.OrderCode=g.OrderCode
		                         where a.SaleUserID=@UserID and a.AddTime>='" + Convert.ToDateTime(rq).ToString("yyyy-MM-dd") + @"' and a.AddTime<'" + Convert.ToDateTime(rq).AddDays(1).ToString("yyyy-MM-dd") + @"'
		                         and d.status=0 and d.SaleRecordLX=0 and d.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9' and a.Status=0 and a.ZhiFuZT=1
		                    ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.Add("@UserID", userId);
                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.ExecuteDataTable(cmd);

                return dtPage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    [CSMethod("getHCQXSMXListToFile", 2)]
    public byte[] getHCQXSMXListToFile(string userId, string rq)
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
                cells[0, 0].PutValue("销售日期");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("销售电子券");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("消费（已使用）金额");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("过期金额");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("未使用金额");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("折扣");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);
                cells[0, 6].PutValue("购买三方账号");
                cells[0, 6].SetStyle(style2);
                cells.SetColumnWidth(6, 20);
                cells[0, 7].PutValue("消费日期");
                cells[0, 7].SetStyle(style2);
                cells.SetColumnWidth(7, 20);


                string str = @"  select a.AddTime,a.Points, case when c.Points>0 then a.Money end as xfje,
		                        case when g.gqje>0 then a.Money end as gqje,case when f.wsyje>0 then a.Money end as wsyje,
                                d.SaleRecordDiscount,e.UserName,c.AddTime as xfrq  from tb_b_order a 
		                        left join tb_b_pay c on a.OrderCode=c.OrderCode
		                        left join tb_b_salerecord d on a.SaleRecordID=d.SaleRecordID 
		                        left join tb_b_user e on a.BuyUserID=e.UserID
		                        left join (select b.OrderCode,b.points as wsyje from tb_b_mycard b where status=0 and b.PointsEndTime>=getDate()) f on a.OrderCode=f.OrderCode
		                        left join (select b.OrderCode,b.points as gqje from tb_b_mycard b where status=9 and b.PointsEndTime<getDate()) g on a.OrderCode=g.OrderCode
		                         where a.SaleUserID=@UserID and a.AddTime>='" + Convert.ToDateTime(rq).ToString("yyyy-MM-dd") + @"' and a.AddTime<'" + Convert.ToDateTime(rq).AddDays(1).ToString("yyyy-MM-dd") + @"'
		                         and d.status=0 and d.SaleRecordLX=0 and d.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9' and a.Status=0 and a.ZhiFuZT=1
		                    ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.Add("@UserID", userId);
                DataTable dt = dbc.ExecuteDataTable(cmd);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["AddTime"] != null && dt.Rows[i]["AddTime"].ToString() != "")
                    {
                        cells[i + 1, 0].PutValue(Convert.ToDateTime(dt.Rows[i]["AddTime"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["Points"] != null && dt.Rows[i]["Points"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["Points"]);
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["xfje"] != null && dt.Rows[i]["xfje"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(dt.Rows[i]["xfje"]);
                    }
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["gqje"] != null && dt.Rows[i]["gqje"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(dt.Rows[i]["gqje"]);
                    }
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["wsyje"] != null && dt.Rows[i]["wsyje"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(dt.Rows[i]["wsyje"]);
                    }
                    cells[i + 1, 4].SetStyle(style4);

                    if (dt.Rows[i]["SaleRecordDiscount"] != null && dt.Rows[i]["SaleRecordDiscount"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(dt.Rows[i]["SaleRecordDiscount"]);
                    }
                    cells[i + 1, 5].SetStyle(style4);
                    if (dt.Rows[i]["UserName"] != null && dt.Rows[i]["UserName"].ToString() != "")
                    {
                        cells[i + 1, 6].PutValue(dt.Rows[i]["UserName"]);
                    }
                    cells[i + 1, 6].SetStyle(style4);
                    if (dt.Rows[i]["xfrq"] != null && dt.Rows[i]["xfrq"].ToString() != "")
                    {
                        cells[i + 1, 7].PutValue(Convert.ToDateTime(dt.Rows[i]["xfrq"]).ToString("yyyy-MM-dd"));
                    }
                    cells[i + 1, 7].SetStyle(style4);
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

    [CSMethod("getHCQSJQ")]
    public object getHCQSJQ(string userId, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {

                string sql = @"select max(SaleRecordTime) as maxtime,min(SaleRecordTime) as mintime from tb_b_salerecord where SaleRecordLX!=0 and SaleRecordVerifyType=1 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
                    and status=0  ";
                DataTable sjdt = dbc.ExecuteDataTable(sql);

                string sjstr = "";

                //上架时间
                string sjwhere = ""; string sjwhere1 = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    sjwhere += " and  SaleRecordTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                    sjwhere1 += " and  b.SaleRecordTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                    sjstr += Convert.ToDateTime(beg).ToString("yyyy年MM月dd日");
                }
                else
                {
                    if (sjdt.Rows.Count > 0)
                    {
                        if (sjdt.Rows[0]["mintime"] != null && sjdt.Rows[0]["mintime"].ToString() != "")
                        {
                            sjstr += Convert.ToDateTime(sjdt.Rows[0]["mintime"]).ToString("yyyy年MM月dd日");
                        }
                        else
                        {
                            sjstr += DateTime.Now.ToString("yyyy年MM月dd日");
                        }
                    }
                    else
                    {
                        sjstr += DateTime.Now.ToString("yyyy年MM月dd日");
                    }
                }
                if (!string.IsNullOrEmpty(end))
                {
                    sjwhere += " and SaleRecordTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                    sjwhere1 += " and b.SaleRecordTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                    sjstr += "到" + Convert.ToDateTime(end).ToString("yyyy年MM月dd日");
                }
                else
                {
                    if (sjdt.Rows.Count > 0)
                    {
                        if (sjdt.Rows[0]["maxtime"] != null && sjdt.Rows[0]["maxtime"].ToString() != "")
                        {
                            sjstr += "到" + Convert.ToDateTime(sjdt.Rows[0]["maxtime"]).ToString("yyyy年MM月dd日");
                        }
                        else
                        {
                            sjstr += "到" + DateTime.Now.ToString("yyyy年MM月dd日");
                        }
                    }
                    else
                    {
                        sjstr += "到" + DateTime.Now.ToString("yyyy年MM月dd日");
                    }
                }
                string str = @" select * from tb_b_salerecord where SaleRecordLX=0 and SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9'
                            and status=0 and  SaleRecordUserID=@UserID" + sjwhere + @" order by SaleRecordTime desc";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.Add("@UserID", userId);
                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.ExecuteDataTable(cmd);

                return dtPage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    [CSMethod("getHCQSJQToFile", 2)]
    public byte[] getHCQSJQToFile(string userId, string beg, string end)
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
                cells[0, 0].PutValue("上架时间");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("上架电子券");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("折扣");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("有效时长");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);

                string sql = @"select max(SaleRecordTime) as maxtime,min(SaleRecordTime) as mintime from tb_b_salerecord where SaleRecordLX!=0 and SaleRecordVerifyType=1 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
                    and status=0  ";
                DataTable sjdt = dbc.ExecuteDataTable(sql);

                string sjstr = "";

                //上架时间
                string sjwhere = ""; string sjwhere1 = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    sjwhere += " and  SaleRecordTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                    sjwhere1 += " and  b.SaleRecordTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                    sjstr += Convert.ToDateTime(beg).ToString("yyyy年MM月dd日");
                }
                else
                {
                    if (sjdt.Rows.Count > 0)
                    {
                        if (sjdt.Rows[0]["mintime"] != null && sjdt.Rows[0]["mintime"].ToString() != "")
                        {
                            sjstr += Convert.ToDateTime(sjdt.Rows[0]["mintime"]).ToString("yyyy年MM月dd日");
                        }
                        else
                        {
                            sjstr += DateTime.Now.ToString("yyyy年MM月dd日");
                        }
                    }
                    else
                    {
                        sjstr += DateTime.Now.ToString("yyyy年MM月dd日");
                    }
                }
                if (!string.IsNullOrEmpty(end))
                {
                    sjwhere += " and SaleRecordTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                    sjwhere1 += " and b.SaleRecordTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                    sjstr += "到" + Convert.ToDateTime(end).ToString("yyyy年MM月dd日");
                }
                else
                {
                    if (sjdt.Rows.Count > 0)
                    {
                        if (sjdt.Rows[0]["maxtime"] != null && sjdt.Rows[0]["maxtime"].ToString() != "")
                        {
                            sjstr += "到" + Convert.ToDateTime(sjdt.Rows[0]["maxtime"]).ToString("yyyy年MM月dd日");
                        }
                        else
                        {
                            sjstr += "到" + DateTime.Now.ToString("yyyy年MM月dd日");
                        }
                    }
                    else
                    {
                        sjstr += "到" + DateTime.Now.ToString("yyyy年MM月dd日");
                    }
                }
                string str = @" select * from tb_b_salerecord where SaleRecordLX=0 and SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9'
                            and status=0 and  SaleRecordUserID=@UserID" + sjwhere + @" order by SaleRecordTime desc";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.Add("@UserID", userId);
                //开始取分页数据
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = dbc.ExecuteDataTable(cmd);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["SaleRecordTime"] != null && dt.Rows[i]["SaleRecordTime"].ToString() != "")
                    {
                        cells[i + 1, 0].PutValue(Convert.ToDateTime(dt.Rows[i]["SaleRecordTime"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["SaleRecordPoints"] != null && dt.Rows[i]["SaleRecordPoints"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["SaleRecordPoints"]);
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["SaleRecordDiscount"] != null && dt.Rows[i]["SaleRecordDiscount"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(dt.Rows[i]["SaleRecordDiscount"]);
                    }
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["ValidHour"] != null && dt.Rows[i]["ValidHour"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(dt.Rows[i]["ValidHour"]);
                    }
                    cells[i + 1, 3].SetStyle(style4);
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

    [CSMethod("getHCQXJQ")]
    public object getHCQXJQ(string userId)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string str = @" select * from tb_b_xj where status=0 and addtime>='2018-12-30 09:00:00' and UserID=@UserID order by addtime desc";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.Add("@UserID", userId);
                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.ExecuteDataTable(cmd);

                return dtPage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    [CSMethod("getHCQXJQToFile", 2)]
    public byte[] getHCQXJQToFile(string userId)
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
                cells[0, 0].PutValue("下架时间");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("下架电子券");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("原因");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);

                string str = @" select * from tb_b_xj where status=0 and addtime>='2018-12-30 09:00:00' and UserID=@UserID order by addtime desc";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.Add("@UserID", userId);
                //开始取分页数据
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = dbc.ExecuteDataTable(cmd);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["addtime"] != null && dt.Rows[i]["addtime"].ToString() != "")
                    {
                        cells[i + 1, 0].PutValue(Convert.ToDateTime(dt.Rows[i]["addtime"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["Points"] != null && dt.Rows[i]["Points"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["Points"]);
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["Memo"] != null && dt.Rows[i]["Memo"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(dt.Rows[i]["Memo"]);
                    }
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
    #endregion

    #region 三方交易明细
    [CSMethod("GetSFJYList")]
    public object GetSFJYList(int pagnum, int pagesize, string yhm, string xm, string beg, string end, string ordercode, string xfbeg, string xfend)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                string where1 = "";

                if (!string.IsNullOrEmpty(yhm.Trim()))
                {
                    where += " and " + dbc.C_Like("c.UserName", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(xm.Trim()))
                {
                    where += " and " + dbc.C_Like("d.UserXM", xm.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.AddTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(ordercode))
                {
                    //where1 += " and " + dbc.C_Like("c.OrderCode", ordercode.Trim(), LikeStyle.LeftAndRightLike);
                    where += " and " + dbc.C_Like("a.OrderCode", ordercode.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(xfbeg))
                {
                    where1 += " and  b.AddTime>='" + Convert.ToDateTime(xfbeg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(xfend))
                {
                    where1 += " and b.AddTime<='" + Convert.ToDateTime(xfend).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select c.UserName,a.AddTime as jysj,b.AddTime as xfsj,d.UserXM,a.OrderCode,a.Money,'消费' as flag,a.redenvelopeid,a.redenvelopemoney as redmoney,
                                case when e.pointkind=0 then '耗材券' when  e.pointkind is null then '耗材券'
                                when  e.pointkind = 4 then  '授权券'
                                when  e.pointkind = 1 then  '自发券'
                                when  e.pointkind = 2 then  '自发券'
                                when  e.pointkind = 3 then  '自发券' end as KIND
                                from tb_b_order a
                                inner join tb_b_pay b on a.OrderCode = b.OrderCode
                                left join tb_b_user c on a.BuyUserID =c.UserID
                                left join tb_b_user d on a.SaleUserID=d.UserID
                                left join tb_b_plattosale e on a.PlatToSaleId=e.PlatToSaleId
                                where a.status=0 and a.ZhiFuZT=1 " + where + where1 + @"
                                union all
                                select c.UserName,a.AddTime as jysj,null as xfsj,d.UserXM,a.OrderCode,a.Money,'购买' as flag,a.redenvelopeid,a.redenvelopemoney as redmoney,
                                case when e.pointkind=0 then '耗材券' when  e.pointkind is null then '耗材券'
                                when  e.pointkind = 4 then  '授权券'
                                when  e.pointkind = 1 then  '自发券'
                                when  e.pointkind = 2 then  '自发券'
                                when  e.pointkind = 3 then  '自发券' end as KIND
                                from tb_b_order a
                                left join tb_b_mycard b on a.OrderCode = b.OrderCode
                                left join tb_b_user c on a.BuyUserID =c.UserID
                                left join tb_b_user d on a.SaleUserID=d.UserID
                                left join tb_b_plattosale e on a.PlatToSaleId=e.PlatToSaleId
                                where a.status=0 and a.ZhiFuZT=1 and b.status = 0  " + where + @"
                                union all
                                select c.UserName,a.AddTime as jysj,null as xfsj,d.UserXM,a.OrderCode,a.Money,'过期' as flag,a.redenvelopeid,a.redenvelopemoney as redmoney,
                                case when e.pointkind=0 then '耗材券' when  e.pointkind is null then '耗材券'
                                when  e.pointkind = 4 then  '授权券'
                                when  e.pointkind = 1 then  '自发券'
                                when  e.pointkind = 2 then  '自发券'
                                when  e.pointkind = 3 then  '自发券' end as KIND
                                from tb_b_order a
                                left join tb_b_mycard b on a.OrderCode = b.OrderCode
                                left join tb_b_user c on a.BuyUserID =c.UserID
                                left join tb_b_user d on a.SaleUserID=d.UserID
                                left join tb_b_plattosale e on a.PlatToSaleId=e.PlatToSaleId
                                where a.status=0 and a.ZhiFuZT=1 and b.status = 9  " + where + @"";

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.AddTime desc,c.UserName,d.UserXM", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    [CSMethod("GetSFJYListToFile", 2)]
    public byte[] GetSFJYListToFile(string yhm, string xm, string beg, string end, string ordercode, string xfbeg, string xfend)
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
                cells[0, 0].PutValue("三方账号");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("交易时间");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("消费时间");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("专线名称");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("订单编号");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("交易金额");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);
                cells[0, 6].PutValue("是否使用红包");
                cells[0, 6].SetStyle(style2);
                cells.SetColumnWidth(6, 20);
                cells[0, 7].PutValue("红包金额");
                cells[0, 7].SetStyle(style2);
                cells.SetColumnWidth(7, 20);
                cells[0, 8].PutValue("交易类型");
                cells[0, 8].SetStyle(style2);
                cells.SetColumnWidth(8, 20);
                cells[0, 9].PutValue("券类型");
                cells[0, 9].SetStyle(style2);
                cells.SetColumnWidth(9, 20);

                string where = "";
                string where1 = "";

                if (!string.IsNullOrEmpty(yhm.Trim()))
                {
                    where += " and " + dbc.C_Like("c.UserName", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(xm.Trim()))
                {
                    where += " and " + dbc.C_Like("d.UserXM", xm.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.AddTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(ordercode))
                {
                    //where1 += " and " + dbc.C_Like("c.OrderCode", ordercode.Trim(), LikeStyle.LeftAndRightLike);
                    where += " and " + dbc.C_Like("a.OrderCode", ordercode.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(xfbeg))
                {
                    where1 += " and  b.AddTime>='" + Convert.ToDateTime(xfbeg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(xfend))
                {
                    where1 += " and b.AddTime<='" + Convert.ToDateTime(xfend).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select c.UserName,a.AddTime as jysj,b.AddTime as xfsj,d.UserXM,a.OrderCode,a.Money,'消费' as flag,a.redenvelopeid,a.redenvelopemoney as redmoney,
                                case when e.pointkind=0 then '耗材券' when  e.pointkind is null then '耗材券'
                                when  e.pointkind = 4 then  '授权券'
                                when  e.pointkind = 1 then  '自发券'
                                when  e.pointkind = 2 then  '自发券'
                                when  e.pointkind = 3 then  '自发券' end as KIND
                                from tb_b_order a
                                inner join tb_b_pay b on a.OrderCode = b.OrderCode
                                left join tb_b_user c on a.BuyUserID =c.UserID
                                left join tb_b_user d on a.SaleUserID=d.UserID
                                left join tb_b_plattosale e on a.PlatToSaleId=e.PlatToSaleId
                                where a.status=0 and a.ZhiFuZT=1  " + where + where1 + @"
                                union all
                                select c.UserName,a.AddTime as jysj,null as xfsj,d.UserXM,a.OrderCode,a.Money,'购买' as flag,a.redenvelopeid,a.redenvelopemoney as redmoney,
                                case when e.pointkind=0 then '耗材券' when  e.pointkind is null then '耗材券'
                                when  e.pointkind = 4 then  '授权券'
                                when  e.pointkind = 1 then  '自发券'
                                when  e.pointkind = 2 then  '自发券'
                                when  e.pointkind = 3 then  '自发券' end as KIND
                                from tb_b_order a
                                left join tb_b_mycard b on a.OrderCode = b.OrderCode
                                left join tb_b_user c on a.BuyUserID =c.UserID
                                left join tb_b_user d on a.SaleUserID=d.UserID
                                left join tb_b_plattosale e on a.PlatToSaleId=e.PlatToSaleId
                                where a.status=0 and a.ZhiFuZT=1 and b.status = 0  " + where + @"
                                union all
                                select c.UserName,a.AddTime as jysj,null as xfsj,d.UserXM,a.OrderCode,a.Money,'过期' as flag,a.redenvelopeid,a.redenvelopemoney as redmoney,
                                case when e.pointkind=0 then '耗材券' when  e.pointkind is null then '耗材券'
                                when  e.pointkind = 4 then  '授权券'
                                when  e.pointkind = 1 then  '自发券'
                                when  e.pointkind = 2 then  '自发券'
                                when  e.pointkind = 3 then  '自发券' end as KIND
                                from tb_b_order a
                                left join tb_b_mycard b on a.OrderCode = b.OrderCode
                                left join tb_b_user c on a.BuyUserID =c.UserID
                                left join tb_b_user d on a.SaleUserID=d.UserID
                                left join tb_b_plattosale e on a.PlatToSaleId=e.PlatToSaleId
                                where a.status=0 and a.ZhiFuZT=1 and b.status = 9  " + where + @"";

                //开始取分页数据
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = dbc.ExecuteDataTable(str + " order by a.AddTime desc,c.UserName,d.UserXM");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["UserName"]);
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["jysj"] != null && dt.Rows[i]["jysj"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(Convert.ToDateTime(dt.Rows[i]["jysj"]).ToString("yyyy-MM-dd HH:mm:SS"));
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["xfsj"] != null && dt.Rows[i]["xfsj"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(Convert.ToDateTime(dt.Rows[i]["xfsj"]).ToString("yyyy-MM-dd HH:mm:SS"));
                    }
                    cells[i + 1, 2].SetStyle(style4);
                    cells[i + 1, 3].PutValue(dt.Rows[i]["UserXM"]);
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["OrderCode"] != null && dt.Rows[i]["OrderCode"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(dt.Rows[i]["OrderCode"]);
                    }
                    cells[i + 1, 4].SetStyle(style4);
                    if (dt.Rows[i]["Money"] != null && dt.Rows[i]["Money"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(dt.Rows[i]["Money"]);
                    }
                    cells[i + 1, 5].SetStyle(style4);
                    if (dt.Rows[i]["redmoney"] != null && dt.Rows[i]["redmoney"].ToString() != "" && Convert.ToInt32(dt.Rows[i]["redmoney"].ToString()) > 0)
                    {
                        cells[i + 1, 6].PutValue("是");
                    }
                    else { cells[i + 1, 6].PutValue("否"); }
                    cells[i + 1, 6].SetStyle(style4);
                    if (dt.Rows[i]["redmoney"] != null && dt.Rows[i]["redmoney"].ToString() != "")
                    {
                        cells[i + 1, 7].PutValue(dt.Rows[i]["redmoney"]);
                    }
                    cells[i + 1, 7].SetStyle(style4);
                    if (dt.Rows[i]["flag"] != null && dt.Rows[i]["flag"].ToString() != "")
                    {
                        cells[i + 1, 8].PutValue(dt.Rows[i]["flag"]);
                    }
                    cells[i + 1, 8].SetStyle(style4);
                    if (dt.Rows[i]["KIND"] != null && dt.Rows[i]["KIND"].ToString() != "")
                    {
                        cells[i + 1, 9].PutValue(dt.Rows[i]["KIND"]);
                    }
                    cells[i + 1, 9].SetStyle(style4);
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
    #endregion

    #region 微信公众号

    [CSMethod("GetWXGZHList")]
    public object GetWXGZHList(int pagnum, int pagesize, string yhm, string xm, string beg, string end)
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
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.AddTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select a.UserID,a.UserXM,a.UserName,a.Addtime,d.sxyfq,e.sqcs,b.xfyfq,c.wsyyfq from tb_b_user a 
                                left join
                                (select sum(Points) as sxyfq,SaleUserID from tb_b_order where [SaleRecordID] is null 
                                and status=0 and ZhiFuZT=1 group by SaleUserID) d on a.UserID=d.SaleUserID
                                left join
                                (select count(OrderID) as sqcs,SaleUserID from tb_b_order where [SaleRecordID] is null 
                                and status=0 and ZhiFuZT=1 group by SaleUserID) e on a.UserID=e.SaleUserID
                                left join 
							    (select sum(Points) as xfyfq,CardUserID from tb_b_pay where OrderCode is null and ReceiveUserID=CardUserID group by CardUserID) b on a.UserID=b.CardUserID
                                left join 
                                (select sum(Points) as wsyyfq,CardUserID from tb_b_mycard where SaleRecordID is null and status=0 group by CardUserID) c on a.UserID=c.CardUserID
                                where 1=1 and a.ClientKind=1";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.AddTime desc,a.UserName,a.UserXM", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    [CSMethod("GetWXGZHListToFile", 2)]
    public byte[] GetWXGZHListToFile(string yhm, string xm, string beg, string end)
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
                cells[0, 0].PutValue("专线");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("账号");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("售券额");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("售券次数");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("消费");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("未使用");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);
                cells[0, 6].PutValue("注册时间");
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
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.AddTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select a.UserID,a.UserXM,a.UserName,a.Addtime,d.sxyfq,e.sqcs,b.xfyfq,c.wsyyfq from tb_b_user a 
                                left join
                                (select sum(Points) as sxyfq,SaleUserID from tb_b_order where [SaleRecordID] is null 
                                and status=0 and ZhiFuZT=1 group by SaleUserID) d on a.UserID=d.SaleUserID
                                left join
                                (select count(OrderID) as sqcs,SaleUserID from tb_b_order where [SaleRecordID] is null 
                                and status=0 and ZhiFuZT=1 group by SaleUserID) e on a.UserID=e.SaleUserID
                                left join 
							    (select sum(Points) as xfyfq,CardUserID from tb_b_pay where OrderCode is null and ReceiveUserID=CardUserID group by CardUserID) b on a.UserID=b.CardUserID
                                left join 
                                (select sum(Points) as wsyyfq,CardUserID from tb_b_mycard where SaleRecordID is null and status=0 group by CardUserID) c on a.UserID=c.CardUserID
                                where 1=1 and a.ClientKind=1";
                str += where;

                //开始取分页数据
                DataTable dt = dbc.ExecuteDataTable(str + " order by a.AddTime desc,a.UserName,a.UserXM");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["UserXM"]);
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["UserName"] != null && dt.Rows[i]["UserName"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["UserName"]);
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["sxyfq"] != null && dt.Rows[i]["sxyfq"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(dt.Rows[i]["sxyfq"]);
                    }
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["sqcs"] != null && dt.Rows[i]["sqcs"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(dt.Rows[i]["sqcs"]);
                    }
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["xfyfq"] != null && dt.Rows[i]["xfyfq"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(dt.Rows[i]["xfyfq"]);
                    }
                    cells[i + 1, 4].SetStyle(style4);
                    if (dt.Rows[i]["wsyyfq"] != null && dt.Rows[i]["wsyyfq"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(dt.Rows[i]["wsyyfq"]);
                    }
                    cells[i + 1, 5].SetStyle(style4);
                    if (dt.Rows[i]["Addtime"] != null && dt.Rows[i]["Addtime"].ToString() != "")
                    {
                        cells[i + 1, 6].PutValue(Convert.ToDateTime(dt.Rows[i]["Addtime"]).ToString("yyyy-MM-dd"));
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

    [CSMethod("getWXSQEList")]
    public object getWXSQEList(int pagnum, int pagesize, string UserID)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string str = @"  select a.*,b.UserName from tb_b_order a 
                                 left join tb_b_user b on a.BuyUserID=b.UserID
                                  where a.[SaleRecordID] is null
                                and a.status=0 and a.ZhiFuZT=1 and a.SaleUserID=@UserID  order by a.AddTime desc
                                ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@UserID", UserID);

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

    [CSMethod("getWXSQEListToFile", 2)]
    public byte[] getWXSQEListToFile(string UserID)
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
                cells[0, 0].PutValue("订单号");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("三方");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("运费券");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("金额");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("时间");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);


                string str = @"  select a.*,b.UserName from tb_b_order a 
                                 left join tb_b_user b on a.BuyUserID=b.UserID
                                  where a.[SaleRecordID] is null
                                and a.status=0 and a.ZhiFuZT=1 and a.SaleUserID=@UserID  order by a.AddTime desc
                                ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                DataTable dt = dbc.ExecuteDataTable(cmd);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["OrderCode"]);
                    cells[i + 1, 0].SetStyle(style4);
                    cells[i + 1, 1].PutValue(dt.Rows[i]["UserName"]);
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["points"] != null && dt.Rows[i]["points"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(dt.Rows[i]["points"]);
                    }
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["Money"] != null && dt.Rows[i]["Money"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(dt.Rows[i]["Money"]);
                    }
                    cells[i + 1, 3].SetStyle(style4);
                    cells[i + 1, 4].PutValue(Convert.ToDateTime(dt.Rows[i]["AddTime"]).ToString("yyyy-MM-dd"));
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

    [CSMethod("getWXXFList")]
    public object getWXXFList(int pagnum, int pagesize, string UserID)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string str = @"select a.*,b.UserName from tb_b_pay a 
                                left join tb_b_user b on a.PayUserID=b.UserID  
                                where a.OrderCode is null and a.ReceiveUserID=a.CardUserID and a.CardUserID=@UserID  order by a.AddTime desc
                                                                ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@UserID", UserID);

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

    [CSMethod("getWXXFListToFile", 2)]
    public byte[] getWXXFListToFile(string UserID)
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
                cells[0, 0].PutValue("三方");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("运费券");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("时间");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);


                string str = @"  select a.*,b.UserName from tb_b_pay a 
                                left join tb_b_user b on a.PayUserID=b.UserID  
                                where a.OrderCode is null and a.ReceiveUserID=a.CardUserID and a.CardUserID=@UserID  order by a.AddTime desc
                                ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                DataTable dt = dbc.ExecuteDataTable(cmd);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["UserName"]);
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["Points"] != null && dt.Rows[i]["Points"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["points"]);
                    }
                    cells[i + 1, 2].PutValue(Convert.ToDateTime(dt.Rows[i]["AddTime"]).ToString("yyyy-MM-dd"));
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

    [CSMethod("getWXWSYList")]
    public object getWXWSYList(int pagnum, int pagesize, string UserID)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string str = @"
                                select a.*,b.UserName  from tb_b_mycard a
                                left join tb_b_user b on a.UserID=b.UserID  
                                where a.SaleRecordID is null and  a.status=0 and a.CardUserID=@UserID  order by b.UserName
                                                                ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@UserID", UserID);

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

    [CSMethod("getWXWSYListToFile", 2)]
    public byte[] getWXWSYListToFile(string UserID)
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
                cells[0, 0].PutValue("三方");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("运费券");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);


                string str = @"  select a.*,b.UserName  from tb_b_mycard a
                                left join tb_b_user b on a.UserID=b.UserID  
                                where a.SaleRecordID is null and  a.status=0 and a.CardUserID=@UserID  order by b.UserName
                                ";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@UserID", UserID);
                DataTable dt = dbc.ExecuteDataTable(cmd);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["UserName"]);
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["points"] != null && dt.Rows[i]["points"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["points"]);
                    }
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
    #endregion

    #region  派送
    [CSMethod("GetPSListZ")]
    public object GetPSListZ(int pagnum, int pagesize, string xm, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";

                if (!string.IsNullOrEmpty(xm.Trim()))
                {
                    where += " and " + dbc.C_Like("e.UserXM", xm.Trim(), LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.addtime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.addtime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"  select a.*,e.UserXM,b.ylqrs,c.dlqrs,d.jlqrs,f.clqrs from tb_b_paisong a  
                                      left join tb_b_user e on a.carduserid=e.UserID 
                                      left join (select count(id) as ylqrs,paisongid from tb_b_paisong_detail where status=0 and getstatus=1 group by paisongid) b on a.id=b.paisongid
                                      left join (select count(id) as dlqrs,paisongid from tb_b_paisong_detail where status=0 and getstatus=0 group by paisongid)c on a.id=c.paisongid
                                    left join (select count(id) as jlqrs,paisongid from tb_b_paisong_detail where status=0 and getstatus=2 group by paisongid)d on a.id=d.paisongid
                                    left join (select count(id) as clqrs,paisongid from tb_b_paisong_detail where status=0 and getstatus=3 group by paisongid)f on a.id=f.paisongid
                                    where a.status=0 and a.points>0 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.addtime desc,e.UserName,e.UserXM", pagesize, ref cp, out ac);

                dtPage.Columns.Add("jzsj");
                foreach (DataRow dr in dtPage.Rows)
                {
                    if (dr["validhour"] != null && dr["validhour"].ToString() != "")
                    {
                        dr["jzsj"] = Convert.ToDateTime(dr["addtime"]).AddHours(Convert.ToInt32(dr["validhour"].ToString()));
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

    [CSMethod("GetPSListZToFile", 2)]
    public byte[] GetPSListZToFile(string xm, string beg, string end)
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
                cells[0, 0].PutValue("专线");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("运费券");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("派送人数");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("已领取人数");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("待领取人数");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("拒绝领取人数");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);
                cells[0, 6].PutValue("超期未领取人数");
                cells[0, 6].SetStyle(style2);
                cells.SetColumnWidth(6, 20);
                cells[0, 7].PutValue("派送时间");
                cells[0, 7].SetStyle(style2);
                cells.SetColumnWidth(7, 20);
                cells[0, 8].PutValue("截止时间");
                cells[0, 8].SetStyle(style2);
                cells.SetColumnWidth(8, 20);

                string where = "";

                if (!string.IsNullOrEmpty(xm.Trim()))
                {
                    where += " and " + dbc.C_Like("e.UserXM", xm.Trim(), LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.addtime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.addtime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"  select a.*,e.UserXM,b.ylqrs,c.dlqrs,d.jlqrs,f.clqrs from tb_b_paisong a  
                                      left join tb_b_user e on a.carduserid=e.UserID 
                                      left join (select count(id) as ylqrs,paisongid from tb_b_paisong_detail where status=0 and getstatus=1 group by paisongid) b on a.id=b.paisongid
                                      left join (select count(id) as dlqrs,paisongid from tb_b_paisong_detail where status=0 and getstatus=0 group by paisongid)c on a.id=c.paisongid
                                    left join (select count(id) as jlqrs,paisongid from tb_b_paisong_detail where status=0 and getstatus=2 group by paisongid)d on a.id=d.paisongid
                                    left join (select count(id) as clqrs,paisongid from tb_b_paisong_detail where status=0 and getstatus=3 group by paisongid)f on a.id=f.paisongid
                                    where a.status=0 and a.points>0 ";
                str += where;

                System.Data.DataTable dt = dbc.ExecuteDataTable(str + " order by a.addtime desc,e.UserName,e.UserXM");

                dt.Columns.Add("jzsj");
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["validhour"] != null && dr["validhour"].ToString() != "")
                    {
                        dr["jzsj"] = Convert.ToDateTime(dr["addtime"]).AddHours(Convert.ToInt32(dr["validhour"].ToString()));
                    }
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["UserXM"] != null && dt.Rows[i]["UserXM"].ToString() != "")
                    {
                        cells[i + 1, 0].PutValue(dt.Rows[i]["UserXM"]);
                    }
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["points"] != null && dt.Rows[i]["points"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["points"]);
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["paisongcount"] != null && dt.Rows[i]["paisongcount"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(dt.Rows[i]["paisongcount"]);
                    }
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["ylqrs"] != null && dt.Rows[i]["ylqrs"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(dt.Rows[i]["ylqrs"]);
                    }
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["dlqrs"] != null && dt.Rows[i]["dlqrs"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(dt.Rows[i]["dlqrs"]);
                    }
                    cells[i + 1, 4].SetStyle(style4);
                    if (dt.Rows[i]["jlqrs"] != null && dt.Rows[i]["jlqrs"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(dt.Rows[i]["jlqrs"]);
                    }
                    cells[i + 1, 5].SetStyle(style4);
                    if (dt.Rows[i]["clqrs"] != null && dt.Rows[i]["clqrs"].ToString() != "")
                    {
                        cells[i + 1, 6].PutValue(dt.Rows[i]["clqrs"]);
                    }
                    cells[i + 1, 6].SetStyle(style4);
                    if (dt.Rows[i]["addtime"] != null && dt.Rows[i]["addtime"].ToString() != "")
                    {
                        cells[i + 1, 7].PutValue(Convert.ToDateTime(dt.Rows[i]["addtime"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 7].SetStyle(style4);
                    if (dt.Rows[i]["validhour"] != null && dt.Rows[i]["validhour"].ToString() != "")
                    {
                        cells[i + 1, 8].PutValue(Convert.ToDateTime(dt.Rows[i]["addtime"]).AddHours(Convert.ToInt32(dt.Rows[i]["validhour"].ToString())).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 8].SetStyle(style4);
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

    [CSMethod("GetPSSFToFilePL", 2)]
    public byte[] GetPSSFToFilePL(string xm, string beg, string end)
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
                cells[0, 0].PutValue("专线");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("三方登录名");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("运费券");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("状态");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("获取时间");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("是否使用");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);
                cells[0, 6].PutValue("使用时间");
                cells[0, 6].SetStyle(style2);
                cells.SetColumnWidth(6, 20);
                cells[0, 7].PutValue("派送时间");
                cells[0, 7].SetStyle(style2);
                cells.SetColumnWidth(7, 20);


                string where = "";

                if (!string.IsNullOrEmpty(xm.Trim()))
                {
                    where += " and " + dbc.C_Like("f.UserXM", xm.Trim(), LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  e.addtime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and e.addtime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"  select a.*,b.UserName,d.AddTime as sfsj,f.UserXM as zxmc,e.addtime as pssj,e.points,e.validhour from tb_b_paisong_detail a left join  
                                tb_b_user b on a.sanfanguserid=b.UserID
                                left join tb_b_mycard c on a.id=c.PaisongDetailID
                                left join tb_b_pay d on c.mycardId=d.mycardId
                                left join tb_b_paisong e on a.paisongid=e.id
                                left join tb_b_user f on e.carduserid=f.UserID
                                where a.status=0 and e.status=0  and e.points>0
                                
                                ";
                str += where;

                System.Data.DataTable dt = dbc.ExecuteDataTable(str + " order by e.addtime desc,a.paisongid, a.getstatus,b.UserName");

                dt.Columns.Add("jzsj");
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["validhour"] != null && dr["validhour"].ToString() != "")
                    {
                        dr["jzsj"] = Convert.ToDateTime(dr["pssj"]).AddHours(Convert.ToInt32(dr["validhour"].ToString()));
                    }
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["zxmc"] != null && dt.Rows[i]["zxmc"].ToString() != "")
                    {
                        cells[i + 1, 0].PutValue(dt.Rows[i]["zxmc"]);
                    }
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["UserName"] != null && dt.Rows[i]["UserName"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["UserName"]);
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["getpoints"] != null && dt.Rows[i]["getpoints"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(dt.Rows[i]["getpoints"]);
                    }
                    cells[i + 1, 2].SetStyle(style4);

                    if (dt.Rows[i]["getstatus"] != null && dt.Rows[i]["getstatus"].ToString() != "")
                    {
                        if (Convert.ToInt32(dt.Rows[i]["getstatus"]) == 0)
                        {
                            cells[i + 1, 3].PutValue("待领取");
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["getstatus"]) == 1)
                        {
                            cells[i + 1, 3].PutValue("已领取");
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["getstatus"]) == 2)
                        {
                            cells[i + 1, 3].PutValue("拒绝领取");
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["getstatus"]) == 3)
                        {
                            cells[i + 1, 3].PutValue("超期未领取");
                        }

                    }
                    cells[i + 1, 3].SetStyle(style4);

                    if (dt.Rows[i]["gettime"] != null && dt.Rows[i]["gettime"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(Convert.ToDateTime(dt.Rows[i]["gettime"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 4].SetStyle(style4);

                    string sfsy = "";
                    if (dt.Rows[i]["sfsj"] != null && dt.Rows[i]["sfsj"].ToString() != "")
                    {
                        sfsy = "是";
                    }
                    else { sfsy = "否"; }
                    cells[i + 1, 5].PutValue(sfsy);
                    cells[i + 1, 5].SetStyle(style4);

                    if (dt.Rows[i]["sfsj"] != null && dt.Rows[i]["sfsj"].ToString() != "")
                    {
                        cells[i + 1, 6].PutValue(Convert.ToDateTime(dt.Rows[i]["sfsj"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 6].SetStyle(style4);

                    if (dt.Rows[i]["pssj"] != null && dt.Rows[i]["pssj"].ToString() != "")
                    {
                        cells[i + 1, 7].PutValue(Convert.ToDateTime(dt.Rows[i]["pssj"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 7].SetStyle(style4);

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

    [CSMethod("GetPSSF")]
    public object GetPSSF(int pagnum, int pagesize, string id)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;
                string str = @"select a.*,b.UserName,d.AddTime as sfsj from tb_b_paisong_detail a left join  
                              tb_b_user b on a.sanfanguserid=b.UserID
                                left join tb_b_mycard c on a.id=c.PaisongDetailID
                                left join tb_b_pay d on c.mycardId=d.mycardId
                              where paisongid=@id and a.status=0
                              order by a.getstatus,b.UserName";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@id", id);
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

    [CSMethod("GetPSSFToFile", 2)]
    public byte[] GetPSSFToFile(string id)
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
                cells[0, 0].PutValue("三方");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("运费券");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("状态");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("获取时间");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("消费时间");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);


                string str = @"select a.*,b.UserName,d.AddTime as sfsj from tb_b_paisong_detail a left join  
                              tb_b_user b on a.sanfanguserid=b.UserID
                                left join tb_b_mycard c on a.id=c.PaisongDetailID
                                left join tb_b_pay d on c.mycardId=d.mycardId
                              where paisongid=@id and a.status=0
                              order by a.getstatus,b.UserName";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@id", id);
                System.Data.DataTable dt = dbc.ExecuteDataTable(cmd);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["UserName"] != null && dt.Rows[i]["UserName"].ToString() != "")
                    {
                        cells[i + 1, 0].PutValue(dt.Rows[i]["UserName"]);
                    }
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["getpoints"] != null && dt.Rows[i]["getpoints"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["getpoints"]);
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["getstatus"] != null && dt.Rows[i]["getstatus"].ToString() != "")
                    {
                        if (Convert.ToInt32(dt.Rows[i]["getstatus"]) == 0)
                        {
                            cells[i + 1, 2].PutValue("待领取");
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["getstatus"]) == 1)
                        {
                            cells[i + 1, 2].PutValue("已领取");
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["getstatus"]) == 2)
                        {
                            cells[i + 1, 2].PutValue("拒绝领取");
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["getstatus"]) == 3)
                        {
                            cells[i + 1, 2].PutValue("超期未领取");
                        }

                    }
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["gettime"] != null && dt.Rows[i]["gettime"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(Convert.ToDateTime(dt.Rows[i]["gettime"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["sfsj"] != null && dt.Rows[i]["sfsj"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(Convert.ToDateTime(dt.Rows[i]["sfsj"]).ToString("yyyy-MM-dd hh:mm:ss"));
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
    #endregion

    #region 上下架明细
    [CSMethod("GetSXJList")]
    public object GetSXJList(int pagnum, int pagesize, string zt, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";

                if (!string.IsNullOrEmpty(zt.Trim()))
                {
                    where += " and " + dbc.C_EQ("a.FLAG", Convert.ToInt32(zt));
                }
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.SJ>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.SJ<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @" select  a.ZXMC,a.YFQ,a.SJ,a.ZK,a.YXQ,a.FLAG,c.UserXM as CZR from 
                            (select SaleRecordUserXM as ZXMC,SaleRecordPoints as YFQ,adduser as CZR,SaleRecordTime as SJ,SaleRecordDiscount as ZK,DATEADD(HOUR,ValidHour,SaleRecordTime) YXQ,'1' as FLAG
                            from tb_b_salerecord where SaleRecordLX=0 and status=0
                            union all
                            (select b.UserXM as ZXMC,a.Points as YFQ,a.adduser as CZR,a.addtime as SJ,null as ZK,null as YXQ,'2' as FLAG from tb_b_xj a left join tb_b_user b on a.UserID=b.UserID where a.status=0)
                            ) a 
                            left join tb_b_user c on a.CZR=c.UserID where 1=1 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.YXQ desc,a.SJ desc,a.ZXMC", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    [CSMethod("GetSXJListToFile", 2)]
    public byte[] GetSXJListToFile(string zt, string beg, string end)
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
                cells[0, 0].PutValue("状态");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("专线名称");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("上架/下架时间");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("上架/下架券额");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("折扣");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("操作人");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);
                cells[0, 6].PutValue("有效期");
                cells[0, 6].SetStyle(style2);
                cells.SetColumnWidth(6, 20);

                string where = "";

                if (!string.IsNullOrEmpty(zt))
                {
                    where += " and " + dbc.C_EQ("a.FLAG", Convert.ToInt32(zt));
                }
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.SJ>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.SJ<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @" select  a.ZXMC,a.YFQ,a.SJ,a.ZK,a.YXQ,a.FLAG,c.UserXM as CZR from 
                            (select SaleRecordUserXM as ZXMC,SaleRecordPoints as YFQ,adduser as CZR,SaleRecordTime as SJ,SaleRecordDiscount as ZK,DATEADD(HOUR,ValidHour,SaleRecordTime) YXQ,'1' as FLAG
                            from tb_b_salerecord where SaleRecordLX=0 and status=0
                            union all
                            (select b.UserXM as ZXMC,a.Points as YFQ,a.adduser as CZR,a.addtime as SJ,null as ZK,null as YXQ,'2' as FLAG from tb_b_xj a left join tb_b_user b on a.UserID=b.UserID where a.status=0)
                            ) a 
                            left join tb_b_user c on a.CZR=c.UserID where 1=1 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dt = dbc.ExecuteDataTable(str + " order by a.YXQ desc,a.SJ desc,a.ZXMC");
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    if (dt.Rows[i]["FLAG"] != null && dt.Rows[i]["FLAG"].ToString() != "")
                    {
                        if (Convert.ToInt32(dt.Rows[i]["FLAG"]) == 1)
                        {
                            cells[i + 1, 0].PutValue("上架");
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["FLAG"]) == 2)
                        {
                            cells[i + 1, 0].PutValue("下架");
                        }

                    }
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["ZXMC"] != null && dt.Rows[i]["ZXMC"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["ZXMC"]);
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["SJ"] != null && dt.Rows[i]["SJ"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(Convert.ToDateTime(dt.Rows[i]["SJ"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["YFQ"] != null && dt.Rows[i]["YFQ"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(dt.Rows[i]["YFQ"]);
                    }
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["ZK"] != null && dt.Rows[i]["ZK"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(dt.Rows[i]["ZK"]);
                    }
                    cells[i + 1, 4].SetStyle(style4);
                    if (dt.Rows[i]["CZR"] != null && dt.Rows[i]["CZR"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(dt.Rows[i]["CZR"]);
                    }
                    cells[i + 1, 5].SetStyle(style4);
                    if (dt.Rows[i]["YXQ"] != null && dt.Rows[i]["YXQ"].ToString() != "")
                    {
                        cells[i + 1, 6].PutValue(Convert.ToDateTime(dt.Rows[i]["YXQ"]).ToString("yyyy-MM-dd hh:mm:ss"));
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
    #endregion

    #region  每日明细
    [CSMethod("getMRMXList")]
    public object getMRMXList(int pagnum, int pagesize, string sj)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";

                if (!string.IsNullOrEmpty(sj))
                {
                    where += " and " + dbc.C_EQ("a.sj", Convert.ToDateTime(sj).ToString("yyyy-MM-dd"));
                }


                string str = @" select a.*,b.xfrs,b.yqje,b.zxfq,c.gqwsy from 
                                (select sum(Points) as xsq,convert(char(10),AddTime,120) as sj from tb_b_order  where status=0 and ZhiFuZT=1 group by convert(char(10),AddTime,120)) a 
                                left join 
                                (select count(distinct PayUserID) as xfrs,
                                sum(CHBMoney) as yqje,sum(Points) as zxfq,
                                convert(char(10),AddTime,120) as sj from  tb_b_pay group by convert(char(10),AddTime,120)) b 
                                on a.sj=b.sj
                                left join 
                                (select sum(points) as gqwsy, convert(char(10),PointsEndTime,120) as sj from  tb_b_mycard where PointsEndTime<getDate() and status=9    group by convert(char(10),PointsEndTime,120)) c
                                on a.sj=c.sj where 1=1";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.sj desc", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    [CSMethod("getMRMXListToFile", 2)]
    public byte[] getMRMXListToFile(string sj)
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
                cells[0, 0].PutValue("时间");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("用券人数");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("用券金额");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("失效金额");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("总消费券");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("销售券");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);

                string where = "";

                if (!string.IsNullOrEmpty(sj))
                {
                    where += " and " + dbc.C_EQ("a.sj", Convert.ToDateTime(sj).ToString("yyyy-MM-dd"));
                }


                string str = @" select a.*,b.xfrs,b.yqje,b.zxfq,c.gqwsy from 
                                (select sum(Points) as xsq,convert(char(10),AddTime,120) as sj from tb_b_order  where status=0 and ZhiFuZT=1 group by convert(char(10),AddTime,120)) a 
                                left join 
                                (select count(distinct PayUserID) as xfrs,
                                sum(CHBMoney) as yqje,sum(Points) as zxfq,
                                convert(char(10),AddTime,120) as sj from  tb_b_pay group by convert(char(10),AddTime,120)) b 
                                on a.sj=b.sj
                                left join 
                                (select sum(points) as gqwsy, convert(char(10),PointsEndTime,120) as sj from  tb_b_mycard where PointsEndTime<getDate() and status=9    group by convert(char(10),PointsEndTime,120)) c
                                on a.sj=c.sj where 1=1";
                str += where;

                //开始取分页数据
                System.Data.DataTable dt = dbc.ExecuteDataTable(str + " order by a.sj desc");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["sj"] != null && dt.Rows[i]["sj"].ToString() != "")
                    {
                        cells[i + 1, 0].PutValue(Convert.ToDateTime(dt.Rows[i]["sj"]).ToString("yyyy-MM-dd"));
                    }
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["xfrs"] != null && dt.Rows[i]["xfrs"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["xfrs"]);
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["yqje"] != null && dt.Rows[i]["yqje"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(dt.Rows[i]["yqje"]);
                    }
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["gqwsy"] != null && dt.Rows[i]["gqwsy"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(dt.Rows[i]["gqwsy"]);
                    }
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["zxfq"] != null && dt.Rows[i]["zxfq"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(dt.Rows[i]["zxfq"]);
                    }
                    cells[i + 1, 4].SetStyle(style4);
                    if (dt.Rows[i]["xsq"] != null && dt.Rows[i]["xsq"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(dt.Rows[i]["xsq"]);
                    }
                    cells[i + 1, 5].SetStyle(style4);
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

    [CSMethod("GetMRXSList")]
    public object GetMRXSList(int pagnum, int pagesize, string sj, string lx)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";

                if (!string.IsNullOrEmpty(sj))
                {
                    where += " and (a.AddTime>='" + Convert.ToDateTime(sj).ToString("yyyy-MM-dd") + "' and a.AddTime<'" + Convert.ToDateTime(sj).AddDays(1).ToString("yyyy-MM-dd") + "')";
                }

                if (!string.IsNullOrEmpty(lx))
                {
                    if (Convert.ToInt32(lx) == 1)
                    {
                        where += "and (b.SaleRecordLX=0 or b.SaleRecordLX is null)";
                    }
                    else if (Convert.ToInt32(lx) == 2)
                    {
                        where += "and b.SaleRecordLX<>0";
                    }
                }

                string str = @" select sum(a.Points) as points,c.UserXM,a.SaleUserID from tb_b_order a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID
                            left join tb_b_user c on a.SaleUserID=c.UserID
                             where a.status=0 and a.ZhiFuZT=1 and b.Status=0   " + where + @" 
                              group by a.SaleUserID,c.UserXM order by c.UserXM ";

                //开始取分页数据
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

    [CSMethod("GetMRXSListToFile", 2)]
    public byte[] GetMRXSListToFile(string sj, string lx)
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
                cells[0, 1].PutValue("销售券额");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);

                string where = "";

                if (!string.IsNullOrEmpty(sj))
                {
                    where += " and (a.AddTime>='" + Convert.ToDateTime(sj).ToString("yyyy-MM-dd") + "' and a.AddTime<'" + Convert.ToDateTime(sj).AddDays(1).ToString("yyyy-MM-dd") + "')";
                }

                if (!string.IsNullOrEmpty(lx))
                {
                    if (Convert.ToInt32(lx) == 1)
                    {
                        where += "and (b.SaleRecordLX=0 or b.SaleRecordLX is null)";
                    }
                    else if (Convert.ToInt32(lx) == 2)
                    {
                        where += "and b.SaleRecordLX<>0";
                    }
                }

                string str = @" select sum(a.Points) as points,c.UserXM,a.SaleUserID from tb_b_order a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID
                            left join tb_b_user c on a.SaleUserID=c.UserID
                             where a.status=0 and a.ZhiFuZT=1 and b.Status=0   " + where + @" 
                              group by a.SaleUserID,c.UserXM order by c.UserXM ";

                DataTable dt = dbc.ExecuteDataTable(str);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["points"] != null && dt.Rows[i]["points"].ToString() != "")
                    {
                        cells[i + 1, 0].PutValue(dt.Rows[i]["points"]);
                    }
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["UserXM"] != null && dt.Rows[i]["UserXM"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["UserXM"]);
                    }
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
    #endregion

    #region 销售情况
    [CSMethod("getXSQK")]
    public object getXSQK()
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                //今日销量 购买人数 购买人次
                string str = @"select sum(a.Points) as xl,count(a.BuyUserID) as gmcs,count(distinct a.BuyUserID) as gmrs  from tb_b_order a left join tb_b_user b on a.SaleUserID=b.UserID 
                where a.status=0 and a.ZhiFuZT=1 and DateDiff(dd,a.AddTime,getdate())=0";
                DataTable dt = dbc.ExecuteDataTable(str);

                string str_1 = @"select sum(a.Points) points from tb_b_pay a left join tb_b_paisong_detail b on a.PaisongDetailID=b.id
				 where PaisongDetailID is not null and carduserid in (select userid from tb_b_user where clientkind = 1)
			 and DateDiff(dd,b.gettime,getdate())=0";
                DataTable dt_1 = dbc.ExecuteDataTable(str_1);

                //历史销量 购买人数 购买人次
                string str1 = @"select sum(a.Points) as xl,count(a.BuyUserID) as gmcs,count(distinct a.BuyUserID) as gmrs  from tb_b_order a left join tb_b_user b on a.SaleUserID=b.UserID 
                where a.status=0 and a.ZhiFuZT=1 ";
                DataTable dt1 = dbc.ExecuteDataTable(str1);

                string str1_1 = @"select sum(a.Points) points from tb_b_pay a left join tb_b_paisong_detail b on a.PaisongDetailID=b.id
				 where PaisongDetailID is not null and carduserid in (select userid from tb_b_user where clientkind = 1)
			 and DateDiff(dd,b.gettime,getdate())<>0";
                DataTable dt1_1 = dbc.ExecuteDataTable(str1_1);

                //今日复购情况
                string str2 = @"select count(*) as fg from 
                            (select count(OrderID) as gmcs,BuyUserID from tb_b_order where status=0 and ZhiFuZT=1 and DateDiff(dd,AddTime,getdate())=0 group by BuyUserID) a
                            where gmcs>1";
                DataTable dt2 = dbc.ExecuteDataTable(str2);

                //历史复购情况
                string str3 = @"select count(*) as fg from 
                            (select count(OrderID) as gmcs,BuyUserID from tb_b_order where status=0 and ZhiFuZT=1  group by BuyUserID) a
                            where gmcs>1";
                DataTable dt3 = dbc.ExecuteDataTable(str3);

                //今日省钱人数
                string str5 = @"select case when m.userid is not null then m.userid else n.userid end as userid,isnull(m.sq,0)+isnull(n.sq,0) as sq into #jtemp from 
                                (
                                select sum((isnull(a.Points,0)-isnull(a.money,0)+isnull(b.money,0))) as sq ,a.BuyUserID as userid  from tb_b_order a 
                                left join tb_b_redenvelope  b on a.redenvelopeid=b.redenvelopeid 
                                where a.status=0 and a.ZhiFuZT=1 and DateDiff(dd,a.AddTime,getdate())=0
                                group by a.BuyUserID
                                 )m full join 
                                (select sanfanguserid as userid,sum(isnull(getpoints,0)) as sq from tb_b_paisong_detail  where status=0 and getstatus=1 and DateDiff(dd,gettime,getdate())=0
                                group by sanfanguserid
                                ) n 
                                on  m.userid=n.userid

                                select count(userid) as counts,'500以下（人）' as  flag from #jtemp where sq<500
                                union all
                                select count(userid) as counts,'500-1000（人）' as  flag  from #jtemp where sq>=500 and sq<1000
                                union all
                                select count(userid) as counts,'1000-2000（人）' as  flag  from #jtemp where sq>=1000 and sq<2000
                                union all
                                select count(userid) as counts,'2000以上（人）' as  flag  from #jtemp where sq>=2000 

                                DROP TABLE #jtemp;";
                DataTable dt5 = dbc.ExecuteDataTable(str5);

                //历史省钱人数
                string str6 = @"select case when m.userid is not null then m.userid else n.userid end as userid,isnull(m.sq,0)+isnull(n.sq,0) as sq into #jtemp from 
                                (
                                select sum((isnull(a.Points,0)-isnull(a.money,0)+isnull(b.money,0))) as sq ,a.BuyUserID as userid  from tb_b_order a 
                                left join tb_b_redenvelope  b on a.redenvelopeid=b.redenvelopeid 
                                where a.status=0 and a.ZhiFuZT=1 
                                group by a.BuyUserID
                                 )m full join 
                                (select sanfanguserid as userid,sum(isnull(getpoints,0)) as sq from tb_b_paisong_detail  where status=0 and getstatus=1 
                                group by sanfanguserid
                                ) n 
                                on  m.userid=n.userid

                                select count(userid) as counts,'500以下（人）' as  flag from #jtemp where sq<500
                                union all
                                select count(userid) as counts,'500-1000（人）' as  flag  from #jtemp where sq>=500 and sq<1000
                                union all
                                select count(userid) as counts,'1000-2000（人）' as  flag  from #jtemp where sq>=1000 and sq<2000
                                union all
                                select count(userid) as counts,'2000以上（人）' as  flag  from #jtemp where sq>=2000 

                                DROP TABLE #jtemp;";
                DataTable dt6 = dbc.ExecuteDataTable(str6);

                //今日省钱数
                string str7 = @"select sum(isnull(m.sq,0)+isnull(n.sq,0)) as sq from 
                                (
                                select sum((isnull(a.Points,0)-isnull(a.money,0)+isnull(b.money,0))) as sq ,a.BuyUserID as userid  from tb_b_order a 
                                left join tb_b_redenvelope  b on a.redenvelopeid=b.redenvelopeid 
                                where a.status=0 and a.ZhiFuZT=1 and DateDiff(dd,a.AddTime,getdate())=0
                                group by a.BuyUserID
                                    )m full join 
                                (select PayUserID as userid,sum(isnull(Points,0)) as sq from tb_b_pay where PaisongDetailID is not null and DateDiff(dd,AddTime,getdate())=0 group by PayUserID
                                ) n 
                                on  m.userid=n.userid";
                DataTable dt7 = dbc.ExecuteDataTable(str7);

                //历史省钱数
                string str8 = @"select sum(isnull(m.sq,0)+isnull(n.sq,0)) as sq from 
                                (
                                select sum((isnull(a.Points,0)-isnull(a.money,0)+isnull(b.money,0))) as sq ,a.BuyUserID as userid  from tb_b_order a 
                                left join tb_b_redenvelope  b on a.redenvelopeid=b.redenvelopeid 
                                where a.status=0 and a.ZhiFuZT=1 
                                group by a.BuyUserID
                                    )m full join 
                                (select PayUserID as userid,sum(isnull(Points,0)) as sq from tb_b_pay where PaisongDetailID is not null group by PayUserID
                                ) n 
                                on  m.userid=n.userid";
                DataTable dt8 = dbc.ExecuteDataTable(str8);

                //今日首次购买
                string str9 = @"select count(BuyUserID) as scgm from (
                                select BuyUserID,AddTime,status,ZhiFuZT,row_number() over(partition by BuyUserID order by AddTime asc) num 
                                from  tb_b_order where status=0 and ZhiFuZT=1) a where a.num=1 and  DateDiff(dd,a.AddTime,getdate())=0";
                DataTable dt9 = dbc.ExecuteDataTable(str9);

                //当月首次购买
                string str10 = @"select count(BuyUserID) as scgm from (
                                select BuyUserID,AddTime,status,ZhiFuZT,row_number() over(partition by BuyUserID order by AddTime asc) num 
                                from  tb_b_order where status=0 and ZhiFuZT=1) a where a.num=1 and  dateDiff(Month,a.AddTime,getdate())=0 ";
                DataTable dt10 = dbc.ExecuteDataTable(str10);

                //今日开通专线统计 
                string str13 = @"select count(*) as ktzx from tb_b_user where ClientKind=1 and IsCanRelease=1 and DqBm is not null
								and DateDiff(dd,canReleaseTime,getdate())=0";
                DataTable dt13 = dbc.ExecuteDataTable(str13);

                //历史开通专线统计
                string str11 = @"select count(*) as ktzx from tb_b_user where ClientKind=1 and IsCanRelease=1 and DqBm is not null";
                DataTable dt11 = dbc.ExecuteDataTable(str11);

                //今日已有销量专线统计
                string str12 = @"select count(distinct SaleUserID) as yxl  from tb_b_order where status=0 and ZhiFuZT=1 and DateDiff(dd,AddTime,getdate())=0";
                DataTable dt12 = dbc.ExecuteDataTable(str12);

                return new { dt = dt, dt_1 = dt_1, dt1 = dt1, dt1_1 = dt1_1, dt2 = dt2, dt3 = dt3, dt5 = dt5, dt6 = dt6, dt7 = dt7, dt8 = dt8, dt9 = dt9, dt10 = dt10, dt11 = dt11, dt12 = dt12, dt13 = dt13 };

            }
            catch (Exception ex) { throw ex; }
        }

    }


    [CSMethod("getJXL")]
    public object getJXL()
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string str = @"select ISNULL(m.xl,0) as xl,m.dq_mc,m.gmrc from 

(select sum(a.Points) as xl, b.FromRoute as dq_mc,count(BuyUserID) as gmrc from tb_b_order a left join tb_b_user b on a.SaleUserID=b.UserID
                                left join tb_b_dq c on b.DqBm=c.dq_bm
                                where a.status=0 and a.ZhiFuZT=1 and DateDiff(dd,a.AddTime,getdate())=0
                                group by b.FromRoute) m left join 

								(select sum(a.Points) points,c.FromRoute from tb_b_pay a left join tb_b_paisong_detail b on a.PaisongDetailID=b.id
								left join tb_b_user c on a.carduserid=c.UserID
				 where a.PaisongDetailID is not null and  c.clientkind = 1 and DateDiff(dd,b.gettime,getdate())=0 
				 group by c.FromRoute) n on m.dq_mc=n.FromRoute";
                DataTable dt = dbc.ExecuteDataTable(str);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("getHXL")]
    public object getHXL()
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string str = @"select ISNULL(m.xl,0) as xl,m.dq_mc,m.gmrc from 

(select sum(a.Points) as xl, b.FromRoute as dq_mc,count(BuyUserID) as gmrc from tb_b_order a left join tb_b_user b on a.SaleUserID=b.UserID
                                left join tb_b_dq c on b.DqBm=c.dq_bm
                                where a.status=0 and a.ZhiFuZT=1 
                                group by b.FromRoute) m left join 

								(select sum(a.Points) points,c.FromRoute from tb_b_pay a left join tb_b_paisong_detail b on a.PaisongDetailID=b.id
								left join tb_b_user c on a.carduserid=c.UserID
				 where a.PaisongDetailID is not null and  c.clientkind = 1 
				 group by c.FromRoute) n on m.dq_mc=n.FromRoute";
                DataTable dt = dbc.ExecuteDataTable(str);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("getJFG")]
    public object getJFG()
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string str = @"select count(OrderID) as gmcs,BuyUserID from tb_b_order where status=0 and ZhiFuZT=1 and DateDiff(dd,AddTime,getdate())=0 group by BuyUserID";
                DataTable dt = dbc.ExecuteDataTable(str);

                int count1 = 0;
                int count2 = 0;
                DataRow[] drs1 = dt.Select("gmcs=2");
                count1 = drs1.Length;

                DataRow[] drs2 = dt.Select("gmcs>2");
                count2 = drs2.Length;

                return new { count1 = count1, count2 = count2 };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("getHFG")]
    public object getHFG()
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string str = @"select count(OrderID) as gmcs,BuyUserID from tb_b_order where status=0 and ZhiFuZT=1  group by BuyUserID";
                DataTable dt = dbc.ExecuteDataTable(str);

                int count1 = 0;
                int count2 = 0;
                DataRow[] drs1 = dt.Select("gmcs=2");
                count1 = drs1.Length;

                DataRow[] drs2 = dt.Select("gmcs>2");
                count2 = drs2.Length;

                return new { count1 = count1, count2 = count2 };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("getJSQ")]
    public object getJSQ()
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string str = @"select sum(a.sq) as sq,b.FromRoute as dq_mc from 
                                (select case when m.userid is not null then m.userid else n.userid end as userid,isnull(m.sq,0)+isnull(n.sq,0) as sq  from 
                                (
                                select sum((isnull(a.Points,0)-isnull(a.money,0)+isnull(b.money,0))) as sq ,a.SaleUserID as userid  from tb_b_order a 
                                left join tb_b_redenvelope  b on a.redenvelopeid=b.redenvelopeid 
                                where a.status=0 and a.ZhiFuZT=1 and DateDiff(dd,a.AddTime,getdate())=0
                                group by a.SaleUserID
                                )m full join 
                                (select CardUserID as userid,sum(isnull(Points,0)) as sq from tb_b_pay where PaisongDetailID is not null and DateDiff(dd,AddTime,getdate())=0 group by CardUserID
                                ) n 
                                on  m.userid=n.userid) a left join tb_b_user b on a.userid=b.UserID
                                group by b.FromRoute";
                DataTable dt = dbc.ExecuteDataTable(str);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("getHSQ")]
    public object getHSQ()
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string str = @"select sum(a.sq) as sq,b.FromRoute as dq_mc from 
                                (select case when m.userid is not null then m.userid else n.userid end as userid,isnull(m.sq,0)+isnull(n.sq,0) as sq  from 
                                (
                                select sum((isnull(a.Points,0)-isnull(a.money,0)+isnull(b.money,0))) as sq ,a.SaleUserID as userid  from tb_b_order a 
                                left join tb_b_redenvelope  b on a.redenvelopeid=b.redenvelopeid 
                                where a.status=0 and a.ZhiFuZT=1 
                                group by a.SaleUserID
                                )m full join 
                                (select CardUserID as userid,sum(isnull(Points,0)) as sq from tb_b_pay where PaisongDetailID is not null group by CardUserID
                                ) n 
                                on  m.userid=n.userid) a left join tb_b_user b on a.userid=b.UserID
                                group by b.FromRoute";
                DataTable dt = dbc.ExecuteDataTable(str);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("getJSCGM")]
    public object getJSCGM()
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string str = @"select count(a.BuyUserID) as scgm,c.dq_mc from (
                                select BuyUserID,AddTime,status,ZhiFuZT,SaleUserID,row_number() over(partition by BuyUserID order by AddTime asc) num 
                                from  tb_b_order where status=0 and ZhiFuZT=1) a
                                left join tb_b_user b on a.SaleUserID=b.UserID
                                left join tb_b_dq c on b.DqBm=c.dq_bm
                                 where a.num=1 and  DateDiff(dd,a.AddTime,getdate())=0
                                 group by c.dq_mc
                                ";
                DataTable dt = dbc.ExecuteDataTable(str);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("getHSCGM")]
    public object getHSCGM()
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string str = @"select count(a.BuyUserID) as scgm,c.dq_mc from (
                                select BuyUserID,AddTime,status,ZhiFuZT,SaleUserID,row_number() over(partition by BuyUserID order by AddTime asc) num 
                                from  tb_b_order where status=0 and ZhiFuZT=1) a
                                left join tb_b_user b on a.SaleUserID=b.UserID
                                left join tb_b_dq c on b.DqBm=c.dq_bm
                                 where a.num=1 and  dateDiff(Month,a.AddTime,getdate())=0
                                 group by c.dq_mc
                                ";
                DataTable dt = dbc.ExecuteDataTable(str);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("getJKTZX")]
    public object getJKTZX()
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string str = @"select count(b.UserID) as ktzx,dq_mc from tb_b_user b
								 left join tb_b_dq c on b.DqBm=c.dq_bm
								 where b.ClientKind=1 and b.IsCanRelease=1 and b.DqBm is not null  and DateDiff(dd,canReleaseTime,getdate())=0 
								 group by c.dq_mc";
                DataTable dt = dbc.ExecuteDataTable(str);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("getHKTZX")]
    public object getHKTZX()
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string str = @"select count(b.UserID) as ktzx,dq_mc from tb_b_user b
								 left join tb_b_dq c on b.DqBm=c.dq_bm
								 where b.ClientKind=1 and b.IsCanRelease=1 and b.DqBm is not null 
								 group by c.dq_mc";
                DataTable dt = dbc.ExecuteDataTable(str);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("getYXL")]
    public object getYXL()
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string str = @"select count(distinct SaleUserID)as yxl,c.dq_mc  from tb_b_order a
								 left join tb_b_user b on a.SaleUserID=b.UserID
									left join tb_b_dq c on b.DqBm=c.dq_bm
								  where a.status=0 and a.ZhiFuZT=1 and DateDiff(dd,a.AddTime,getdate())=0
								  group by  c.dq_mc";
                DataTable dt = dbc.ExecuteDataTable(str);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    #endregion

    #region 当月专线
    [CSMethod("getDYZXList")]
    public object getDYZXList(int pagnum, int pagesize, string zxmc)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";

                if (!string.IsNullOrEmpty(zxmc))
                {
                    where += " and " + dbc.C_Like("a.UserXM", zxmc, LikeStyle.LeftAndRightLike);
                }


                string str = @" select a.UserID,a.UserName,a.UserXM,b.zsx,c.zfchb,d.gmlj,e.xflj from tb_b_user a
                                left join (
                                select sum(sqjf) as zsx,userid from  tb_b_jfsq where issq=1 group by userid) b on a.UserID=b.userId
                                left join 
                                (select sum(points) as zfchb,UserID from tb_b_givetoplat where status=0 and  DateDiff(month,AddTime,getdate())=0 group by UserID ) c on a.UserID=c.UserID
                                left join 
                                (select sum(points) as gmlj,SaleUserID from tb_b_order  where status=0 and ZhiFuZT=1  and  DateDiff(month,AddTime,getdate())=0 group by SaleUserID ) d on a.UserID=d.SaleUserID
                                left join 
                                (select sum(points) as xflj,CardUserID from tb_b_pay  where   DateDiff(month,AddTime,getdate())=0 group by CardUserID ) e on a.UserID=e.CardUserID
                                where clientkind=1 and IsSHPass=1";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.UserName", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    [CSMethod("getDYZXListToFile", 2)]
    public byte[] getDYZXListToFile(string zxmc)
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
                cells[0, 0].PutValue("专线账号");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("专线名称");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("总授信");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("当月支付查货宝");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("当月累计购买");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("当月累计消费");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);

                string where = "";

                if (!string.IsNullOrEmpty(zxmc))
                {
                    where += " and " + dbc.C_Like("a.UserXM", zxmc, LikeStyle.LeftAndRightLike);
                }


                string str = @" select a.UserID,a.UserName,a.UserXM,b.zsx,c.zfchb,d.gmlj,e.xflj from tb_b_user a
                                left join (
                                select sum(sqjf) as zsx,userid from  tb_b_jfsq where issq=1 group by userid) b on a.UserID=b.userId
                                left join 
                                (select sum(points) as zfchb,UserID from tb_b_givetoplat where status=0 and  DateDiff(month,AddTime,getdate())=0 group by UserID ) c on a.UserID=c.UserID
                                left join 
                                (select sum(points) as gmlj,SaleUserID from tb_b_order  where status=0 and ZhiFuZT=1  and  DateDiff(month,AddTime,getdate())=0 group by SaleUserID ) d on a.UserID=d.SaleUserID
                                left join 
                                (select sum(points) as xflj,CardUserID from tb_b_pay  where   DateDiff(month,AddTime,getdate())=0 group by CardUserID ) e on a.UserID=e.CardUserID
                                where clientkind=1 and IsSHPass=1";
                str += where;

                //开始取分页数据
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = dbc.ExecuteDataTable(str + " order by a.UserName");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["UserName"] != null && dt.Rows[i]["UserName"].ToString() != "")
                    {
                        cells[i + 1, 0].PutValue(dt.Rows[i]["UserName"]);
                    }
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["UserXM"] != null && dt.Rows[i]["UserXM"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["UserXM"]);
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["zsx"] != null && dt.Rows[i]["zsx"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(dt.Rows[i]["zsx"]);
                    }
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["zfchb"] != null && dt.Rows[i]["zfchb"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(dt.Rows[i]["zfchb"]);
                    }
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["gmlj"] != null && dt.Rows[i]["gmlj"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(dt.Rows[i]["gmlj"]);
                    }
                    cells[i + 1, 4].SetStyle(style4);
                    if (dt.Rows[i]["xflj"] != null && dt.Rows[i]["xflj"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(dt.Rows[i]["xflj"]);
                    }
                    cells[i + 1, 5].SetStyle(style4);
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
    #endregion

    #region  平台派送红包
    [CSMethod("GetPSListZ_HB")]
    public object GetPSListZ_HB(int pagnum, int pagesize, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";

                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.addtime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.addtime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"  select a.*,b.ylqrs,c.dlqrs,d.jlqrs,f.clqrs from tb_b_paisongredenvelope a  
                            left join (select count(id) as ylqrs,paisongredenvelopeid from tb_b_paisongredenvelope_detail where status=0 and getstatus=1 group by paisongredenvelopeid) b on a.id=b.paisongredenvelopeid
                            left join (select count(id) as dlqrs,paisongredenvelopeid from tb_b_paisongredenvelope_detail where status=0 and getstatus=0 group by paisongredenvelopeid)c on a.id=c.paisongredenvelopeid
                            left join (select count(id) as jlqrs,paisongredenvelopeid from tb_b_paisongredenvelope_detail where status=0 and getstatus=2 group by paisongredenvelopeid)d on a.id=d.paisongredenvelopeid
                            left join (select count(id) as clqrs,paisongredenvelopeid from tb_b_paisongredenvelope_detail where status=0 and getstatus=3 group by paisongredenvelopeid)f on a.id=f.paisongredenvelopeid
                            where a.status=0 and a.points>0 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.addtime desc", pagesize, ref cp, out ac);

                dtPage.Columns.Add("jzsj");
                foreach (DataRow dr in dtPage.Rows)
                {
                    if (dr["validhour"] != null && dr["validhour"].ToString() != "")
                    {
                        dr["jzsj"] = Convert.ToDateTime(dr["addtime"]).AddHours(Convert.ToInt32(dr["validhour"].ToString()));
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

    [CSMethod("GetPSListZToFile_HB", 2)]
    public byte[] GetPSListZToFile_HB(string beg, string end)
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
                cells[0, 0].PutValue("运费券");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("派送人数");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("已领取人数");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("待领取人数");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("拒绝领取人数");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("超期未领取人数");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);
                cells[0, 6].PutValue("派送时间");
                cells[0, 6].SetStyle(style2);
                cells.SetColumnWidth(6, 20);
                cells[0, 7].PutValue("截止时间");
                cells[0, 7].SetStyle(style2);
                cells.SetColumnWidth(7, 20);

                string where = "";

                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.addtime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.addtime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"   select a.*,b.ylqrs,c.dlqrs,d.jlqrs,f.clqrs from tb_b_paisongredenvelope a  
                            left join (select count(id) as ylqrs,paisongredenvelopeid from tb_b_paisongredenvelope_detail where status=0 and getstatus=1 group by paisongredenvelopeid) b on a.id=b.paisongredenvelopeid
                            left join (select count(id) as dlqrs,paisongredenvelopeid from tb_b_paisongredenvelope_detail where status=0 and getstatus=0 group by paisongredenvelopeid)c on a.id=c.paisongredenvelopeid
                            left join (select count(id) as jlqrs,paisongredenvelopeid from tb_b_paisongredenvelope_detail where status=0 and getstatus=2 group by paisongredenvelopeid)d on a.id=d.paisongredenvelopeid
                            left join (select count(id) as clqrs,paisongredenvelopeid from tb_b_paisongredenvelope_detail where status=0 and getstatus=3 group by paisongredenvelopeid)f on a.id=f.paisongredenvelopeid
                            where a.status=0 and a.points>0 ";
                str += where;

                System.Data.DataTable dt = dbc.ExecuteDataTable(str + " order by a.addtime desc");

                dt.Columns.Add("jzsj");
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["validhour"] != null && dr["validhour"].ToString() != "")
                    {
                        dr["jzsj"] = Convert.ToDateTime(dr["addtime"]).AddHours(Convert.ToInt32(dr["validhour"].ToString()));
                    }
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["points"] != null && dt.Rows[i]["points"].ToString() != "")
                    {
                        cells[i + 1, 0].PutValue(dt.Rows[i]["points"]);
                    }
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["paisongcount"] != null && dt.Rows[i]["paisongcount"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["paisongcount"]);
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["ylqrs"] != null && dt.Rows[i]["ylqrs"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(dt.Rows[i]["ylqrs"]);
                    }
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["dlqrs"] != null && dt.Rows[i]["dlqrs"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(dt.Rows[i]["dlqrs"]);
                    }
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["jlqrs"] != null && dt.Rows[i]["jlqrs"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(dt.Rows[i]["jlqrs"]);
                    }
                    cells[i + 1, 4].SetStyle(style4);
                    if (dt.Rows[i]["clqrs"] != null && dt.Rows[i]["clqrs"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(dt.Rows[i]["clqrs"]);
                    }
                    cells[i + 1, 5].SetStyle(style4);
                    if (dt.Rows[i]["addtime"] != null && dt.Rows[i]["addtime"].ToString() != "")
                    {
                        cells[i + 1, 6].PutValue(Convert.ToDateTime(dt.Rows[i]["addtime"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 6].SetStyle(style4);
                    if (dt.Rows[i]["validhour"] != null && dt.Rows[i]["validhour"].ToString() != "")
                    {
                        cells[i + 1, 7].PutValue(Convert.ToDateTime(dt.Rows[i]["addtime"]).AddHours(Convert.ToInt32(dt.Rows[i]["validhour"].ToString())).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 7].SetStyle(style4);
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

    [CSMethod("GetPSSFToFilePL_HB", 2)]
    public byte[] GetPSSFToFilePL_HB(string beg, string end)
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
                cells[0, 0].PutValue("三方登录名");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("运费券");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("状态");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("获取时间");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("是否使用");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("使用时间");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);
                cells[0, 6].PutValue("派送时间");
                cells[0, 6].SetStyle(style2);
                cells.SetColumnWidth(6, 20);


                string where = "";

                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  pssj>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and pssj<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"     select *  from (
                        select a.addtime as pssj,a.points,b.getpoints,b.getstatus, b.gettime,c.updatetime,c.validhour,c.isuse,d.UserName from (
 select id,addtime,points from tb_b_paisongredenvelope where addtime < '2019-11-04' and points > 0 and status = 0
) a
left join tb_b_paisongredenvelope_detail b on a.id = b.paisongredenvelopeid and status = 0
left join (select paisongdetailid,updatetime,validhour,isuse,userid from tb_b_redenvelope where AddTime<'2019-11-04' and type = 3) c on b.id = c.paisongdetailid
left join tb_b_user d on b.sanfanguserid = d.UserID
union
select a.addtime as pssj,a.points,b.getpoints,b.getstatus, b.gettime,b.updatetime,b.validhour,b.isuse,d.UserName from (
 select id,addtime,points from tb_b_paisongredenvelope where addtime >= '2019-11-04' and points > 0 and status = 0
) a
left join (
    select b.paisongredenvelopeid, b.getpoints,b.getstatus,b.sanfanguserid,b.gettime,c.updatetime,c.validhour,c.isuse from tb_b_paisongredenvelope_detail b
left join tb_b_redenvelope c on b.paisongredenvelopeid = c.paisongdetailid and b.sanfanguserid = c.userid and b.gettime >= '2019-11-04'
) b on a.id = b.paisongredenvelopeid
left join tb_b_user d on b.sanfanguserid = d.UserID  )  a where 1=1
                                
                                ";
                str += where;

                System.Data.DataTable dt = dbc.ExecuteDataTable(str + " order by pssj desc,getstatus,UserName");

                //dt.Columns.Add("sysj");
                //foreach (DataRow dr in dt.Rows)
                //{
                //    if (dr["validhour"] != null && dr["validhour"].ToString() != "")
                //    {
                //        dr["sysj"] = Convert.ToDateTime(dr["addtime"]).AddHours(Convert.ToInt32(dr["validhour"].ToString()));
                //    }
                //}

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["UserName"] != null && dt.Rows[i]["UserName"].ToString() != "")
                    {
                        cells[i + 1, 0].PutValue(dt.Rows[i]["UserName"]);
                    }
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["getpoints"] != null && dt.Rows[i]["getpoints"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["getpoints"]);
                    }
                    cells[i + 1, 1].SetStyle(style4);

                    if (dt.Rows[i]["getstatus"] != null && dt.Rows[i]["getstatus"].ToString() != "")
                    {
                        if (Convert.ToInt32(dt.Rows[i]["getstatus"]) == 0)
                        {
                            cells[i + 1, 2].PutValue("待领取");
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["getstatus"]) == 1)
                        {
                            cells[i + 1, 2].PutValue("已领取");
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["getstatus"]) == 2)
                        {
                            cells[i + 1, 2].PutValue("拒绝领取");
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["getstatus"]) == 3)
                        {
                            cells[i + 1, 2].PutValue("超期未领取");
                        }

                    }
                    cells[i + 1, 2].SetStyle(style4);

                    if (dt.Rows[i]["gettime"] != null && dt.Rows[i]["gettime"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(Convert.ToDateTime(dt.Rows[i]["gettime"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 3].SetStyle(style4);

                    string sfsy = "";
                    if (dt.Rows[i]["isuse"] != null && dt.Rows[i]["isuse"].ToString() != "")
                    {
                        if (Convert.ToInt32(dt.Rows[i]["isuse"]) == 1)
                        {
                            sfsy = "是";
                        }
                        else
                        {
                            sfsy = "否";
                        }
                    }
                    else { sfsy = "否"; }
                    cells[i + 1, 4].PutValue(sfsy);
                    cells[i + 1, 4].SetStyle(style4);

                    if (dt.Rows[i]["updatetime"] != null && dt.Rows[i]["updatetime"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(Convert.ToDateTime(dt.Rows[i]["updatetime"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 5].SetStyle(style4);

                    if (dt.Rows[i]["pssj"] != null && dt.Rows[i]["pssj"].ToString() != "")
                    {
                        cells[i + 1, 6].PutValue(Convert.ToDateTime(dt.Rows[i]["pssj"]).ToString("yyyy-MM-dd hh:mm:ss"));
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

    [CSMethod("GetPSSF_HB")]
    public object GetPSSF_HB(int pagnum, int pagesize, string id)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;
                string str = @"select a.getpoints,a.getstatus, a.gettime,b.UserName,c.updatetime as sfsj,c.validhour from tb_b_paisongredenvelope_detail a left join  
                            tb_b_user b on a.sanfanguserid=b.UserID
                            left join tb_b_redenvelope c on a.paisongredenvelopeid=c.paisongdetailid and a.sanfanguserid = c.userid
                            left join tb_b_paisongredenvelope d on a.paisongredenvelopeid=d.id 
                            where paisongredenvelopeid=@id and a.status=0 and d.addtime >= '2019-11-04'
                                       union
                                          select a.getpoints,a.getstatus, a.gettime,b.UserName,c.updatetime as sfsj,c.validhour from tb_b_paisongredenvelope_detail a left join  
                            tb_b_user b on a.sanfanguserid=b.UserID
                            left join tb_b_redenvelope c on a.paisongredenvelopeid=c.paisongdetailid 
                            left join tb_b_paisongredenvelope d on a.paisongredenvelopeid=d.id 
                            where paisongredenvelopeid=@id and a.status=0 and d.addtime < '2019-11-04' order by a.getstatus,b.UserName";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@id", id);
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

    [CSMethod("GetPSSFToFile_HB", 2)]
    public byte[] GetPSSFToFile_HB(string id)
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
                cells[0, 0].PutValue("三方");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("运费券");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("状态");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("获取时间");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("消费时间");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);


                string str = @"select a.getpoints,a.getstatus, a.gettime,b.UserName,c.updatetime as sfsj,c.validhour from tb_b_paisongredenvelope_detail a left join  
                            tb_b_user b on a.sanfanguserid=b.UserID
                            left join tb_b_redenvelope c on a.paisongredenvelopeid=c.paisongdetailid and a.sanfanguserid = c.userid
                            left join tb_b_paisongredenvelope d on a.paisongredenvelopeid=d.id 
                            where paisongredenvelopeid=@id and a.status=0 and d.addtime >= '2019-11-04'
                                       union
                                          select a.getpoints,a.getstatus, a.gettime,b.UserName,c.updatetime as sfsj,c.validhour from tb_b_paisongredenvelope_detail a left join  
                            tb_b_user b on a.sanfanguserid=b.UserID
                            left join tb_b_redenvelope c on a.paisongredenvelopeid=c.paisongdetailid 
                            left join tb_b_paisongredenvelope d on a.paisongredenvelopeid=d.id 
                            where paisongredenvelopeid=@id and a.status=0 and d.addtime < '2019-11-04' order by a.getstatus,b.UserName";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@id", id);
                System.Data.DataTable dt = dbc.ExecuteDataTable(cmd);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["UserName"] != null && dt.Rows[i]["UserName"].ToString() != "")
                    {
                        cells[i + 1, 0].PutValue(dt.Rows[i]["UserName"]);
                    }
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["getpoints"] != null && dt.Rows[i]["getpoints"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["getpoints"]);
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["getstatus"] != null && dt.Rows[i]["getstatus"].ToString() != "")
                    {
                        if (Convert.ToInt32(dt.Rows[i]["getstatus"]) == 0)
                        {
                            cells[i + 1, 2].PutValue("待领取");
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["getstatus"]) == 1)
                        {
                            cells[i + 1, 2].PutValue("已领取");
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["getstatus"]) == 2)
                        {
                            cells[i + 1, 2].PutValue("拒绝领取");
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["getstatus"]) == 3)
                        {
                            cells[i + 1, 2].PutValue("超期未领取");
                        }

                    }
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["gettime"] != null && dt.Rows[i]["gettime"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(Convert.ToDateTime(dt.Rows[i]["gettime"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["sfsj"] != null && dt.Rows[i]["sfsj"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(Convert.ToDateTime(dt.Rows[i]["sfsj"]).ToString("yyyy-MM-dd hh:mm:ss"));
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

    [CSMethod("GetPSHBJL")]
    public object GetPSHBJL(int pagnum, int pagesize, string yhm, string zt, string lx, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;
                string where = "";

                if (!string.IsNullOrEmpty(yhm))
                {
                    where += " and " + dbc.C_Like("b.UserName", yhm, LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(zt))
                {
                    where += " and " + dbc.C_EQ("a.isuse", Convert.ToInt32(zt));
                }

                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.updatetime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.updatetime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(lx))
                {
                    where += " and " + dbc.C_EQ("a.type", Convert.ToInt32(lx));
                }
                string str = @"select a.*,b.UserName,b.UserXM from tb_b_redenvelope a left join tb_b_user b on  a.userid=b.UserID
                                where 1=1 " + where + @"
                            order by a.addtime desc";
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str, pagesize, ref cp, out ac);
                dtPage.Columns.Add("jzsj");
                foreach (DataRow dr in dtPage.Rows)
                {
                    if (dr["validhour"] != null && dr["validhour"].ToString() != "")
                    {
                        dr["jzsj"] = Convert.ToDateTime(dr["addtime"]).AddHours(Convert.ToInt32(dr["validhour"].ToString()));
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
    [CSMethod("GetPSHBZJ")]
    public object GetPSHBZJ()
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string str = @"select  (select sum(money) from tb_b_redenvelope) as zj,(select sum(money) from tb_b_redenvelope where isuse=1) as dzf,
(select sum(money) from tb_b_redenvelope where isuse=9) as yzf,(select sum(money) from tb_b_redenvelope where isuse=3) as yfq,
(select sum(money) from tb_b_redenvelope  where isuse=2) as gq,(select sum(money) from tb_b_redenvelope where isuse=0) as wsy";
                DataTable dt = dbc.ExecuteDataTable(str);

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetPSHBJLToFile", 2)]
    public byte[] GetPSHBJLToFile(string yhm, string zt, string lx, string beg, string end)
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
                cells[0, 0].PutValue("领取人");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("金额");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("类型");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("截止时间");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("状态");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);

                string where = "";

                if (!string.IsNullOrEmpty(yhm))
                {
                    where += " and " + dbc.C_Like("b.UserName", yhm, LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(zt))
                {
                    where += " and " + dbc.C_EQ("a.isuse", Convert.ToInt32(zt));
                }

                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.updatetime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.updatetime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(lx))
                {
                    where += " and " + dbc.C_EQ("a.type", Convert.ToInt32(lx));
                }
                string str = @"select a.*,b.UserName,b.UserXM from tb_b_redenvelope a left join tb_b_user b on  a.userid=b.UserID
                                where 1=1 " + where + @"
                            order by a.addtime desc";
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = dbc.ExecuteDataTable(str);
                dt.Columns.Add("jzsj");
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["validhour"] != null && dr["validhour"].ToString() != "")
                    {
                        dr["jzsj"] = Convert.ToDateTime(dr["addtime"]).AddHours(Convert.ToInt32(dr["validhour"].ToString()));
                    }
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["UserName"] != null && dt.Rows[i]["UserName"].ToString() != "")
                    {
                        cells[i + 1, 0].PutValue(dt.Rows[i]["UserName"]);
                    }
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["money"] != null && dt.Rows[i]["money"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["money"]);
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    string type = "";
                    if (dt.Rows[i]["type"] != null && dt.Rows[i]["type"].ToString() != "")
                    {
                        if (Convert.ToInt32(dt.Rows[i]["type"]) == 0)
                        {
                            type = "新人红包";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["type"]) == 1)
                        {
                            type = "分享";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["type"]) == 2)
                        {
                            type = "订单红包";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["type"]) == 3)
                        {
                            type = "平台派送红包";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["type"]) == 4)
                        {
                            type = "抽奖红包";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["type"]) == 5)
                        {
                            type = "购买送红包";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["type"]) == 6)
                        {
                            type = "新人复购红包";
                        }
                        cells[i + 1, 2].PutValue(type);

                    }
                    cells[i + 1, 2].SetStyle(style4);

                    if (dt.Rows[i]["jzsj"] != null && dt.Rows[i]["jzsj"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(Convert.ToDateTime(dt.Rows[i]["jzsj"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 3].SetStyle(style4);

                    string isuse = "";
                    if (dt.Rows[i]["isuse"] != null && dt.Rows[i]["isuse"].ToString() != "")
                    {

                        if (Convert.ToInt32(dt.Rows[i]["isuse"]) == 2)
                        {
                            isuse = "过期";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["isuse"]) == 0)
                        {
                            isuse = "未使用";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["isuse"]) == 1)
                        {
                            isuse = "待支付";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["isuse"]) == 9)
                        {
                            isuse = "已使用";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["isuse"]) == 3)
                        {
                            isuse = "已废弃";
                        }

                        cells[i + 1, 4].PutValue(isuse);

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
    #endregion

    #region 用户剩余红包统计
    [CSMethod("GetRedenvelopeByPage")]
    public object GetRedenvelopeByPage(int pagnum, int pagesize, string num)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";

                if (!string.IsNullOrEmpty(num))
                {
                    where = " where a.ResidueNum =" + Convert.ToInt32(num);
                }

                string str = @"select a.*,b.UserName,b.UserXM from (
	                            select userid,COUNT(*) ResidueNum from tb_b_redenvelope a
	                            where isuse=0
	                            group by userid
                            )a
                            left join tb_b_user b on a.userid=b.userid " + where;

                //开始取分页数据
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

    [CSMethod("GetRedenvelopeToFile", 2)]
    public byte[] GetShareRecordToFile(string num)
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
                cells[0, 0].PutValue("账号");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("剩余红包数量");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);

                string where = "";

                if (!string.IsNullOrEmpty(num))
                {
                    where = " where a.ResidueNum =" + Convert.ToInt32(num);
                }

                string str = @"select a.*,b.UserName,b.UserXM from (
	                            select userid,COUNT(*) ResidueNum from tb_b_redenvelope a
	                            where isuse=0
	                            group by userid
                            )a
                            left join tb_b_user b on a.userid=b.userid " + where;

                DataTable dt = dbc.ExecuteDataTable(str);

                for (int i = 0; i < dt.Rows.Count; i++)
                {


                    if (!string.IsNullOrEmpty(dt.Rows[i]["UserName"].ToString()))
                    {
                        cells[i + 1, 0].PutValue(dt.Rows[i]["UserName"]);
                    }
                    else
                    {
                        cells[i + 1, 0].PutValue("");
                    }
                    cells[i + 1, 0].SetStyle(style4);

                    cells[i + 1, 1].PutValue(dt.Rows[i]["ResidueNum"]);
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
    #endregion

    #region 专线自发布统计
    [CSMethod("GetZxzfbTjByPage")]
    public object GetZxzfbTjByPage(int pagnum, int pagesize, JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                #region 参数
                JSReader[] jsr_fenda = jsr.ToArray();
                string cx_gzs = jsr_fenda[0];
                string gzsWhere = "";
                if (!string.IsNullOrEmpty(cx_gzs))
                {
                    gzsWhere += " and a.num >=" + cx_gzs;
                }
                string cx_gzs2 = jsr_fenda[1];
                if (!string.IsNullOrEmpty(cx_gzs2))
                {
                    gzsWhere += " and a.num <=" + cx_gzs2;
                }
                string cx_zhpf = jsr_fenda[2];
                string where = "";
                if (!string.IsNullOrEmpty(cx_zhpf))
                {
                    where += "and grade >= " + cx_zhpf;
                }
                string cx_zhpf2 = jsr_fenda[3];
                if (!string.IsNullOrEmpty(cx_zhpf2))
                {
                    where += "and grade <= " + cx_zhpf2;
                }
                string cx_pjs = jsr_fenda[4];
                string pjsWhere = "";
                if (!string.IsNullOrEmpty(cx_pjs))
                {
                    pjsWhere += " and a.num >= " + cx_pjs;
                }
                string cx_pjs2 = jsr_fenda[5];
                if (!string.IsNullOrEmpty(cx_pjs2))
                {
                    pjsWhere += " and a.num <= " + cx_pjs2;
                }
                string cx_dprs = jsr_fenda[6];
                string dprsWhere = "";
                if (!string.IsNullOrEmpty(cx_dprs))
                {
                    dprsWhere += " and a.num >= " + cx_dprs;
                }
                string cx_dprs2 = jsr_fenda[7];
                if (!string.IsNullOrEmpty(cx_dprs2))
                {
                    dprsWhere += " and a.num <= " + cx_dprs2;
                }
                string cx_gmcs = jsr_fenda[8];
                string gmcsWhere = "";
                if (!string.IsNullOrEmpty(cx_gmcs))
                {
                    gmcsWhere += " and a.num >= " + cx_gmcs;
                }
                string cx_gmcs2 = jsr_fenda[9];
                if (!string.IsNullOrEmpty(cx_gmcs2))
                {
                    gmcsWhere += " and a.num <= " + cx_gmcs2;
                }
                string cx_gmrs = jsr_fenda[10];
                string gmrsWhere = "";
                if (!string.IsNullOrEmpty(cx_gmrs))
                {
                    gmrsWhere += " and a.num >= " + cx_gmrs;
                }
                string cx_gmrs2 = jsr_fenda[11];
                if (!string.IsNullOrEmpty(cx_gmrs2))
                {
                    gmrsWhere += " and a.num <= " + cx_gmrs2;
                }
                string cx_zfbcs = jsr_fenda[12];
                string zfbcsWhere = "";
                if (!string.IsNullOrEmpty(cx_zfbcs))
                {
                    zfbcsWhere += " and a.num >= " + cx_zfbcs;
                }
                string cx_zfbcs2 = jsr_fenda[13];
                if (!string.IsNullOrEmpty(cx_zfbcs2))
                {
                    zfbcsWhere += " and a.num <= " + cx_zfbcs2;
                }
                string cx_city = jsr_fenda[14];
                if (!string.IsNullOrEmpty(cx_city))
                {
                    where += " and DqBm = " + dbc.ToSqlValue(cx_city);
                }
                string cx_isSq = jsr_fenda[15];
                if (!string.IsNullOrEmpty(cx_isSq))
                {
                    where += " and authorizestatus = " + cx_isSq;
                }
                string cx_isRz = jsr_fenda[16];
                if (!string.IsNullOrEmpty(cx_isRz))
                {
                    where += " and isidentification = " + cx_isRz;
                }
                string cx_tkl = jsr_fenda[17];
                string tklWhere = "";
                if (!string.IsNullOrEmpty(cx_tkl))
                {
                    tklWhere += " and Convert(decimal(18,4),CAST(num1 AS FLOAT)/num*100) > " + cx_tkl;
                }
                string cx_tkl2 = jsr_fenda[18];
                if (!string.IsNullOrEmpty(cx_tkl2))
                {
                    tklWhere += " and Convert(decimal(18,4),CAST(num1 AS FLOAT)/num*100) < " + cx_tkl2;
                }
                #endregion

                string str = @"select UserName,UserXM from tb_b_user 
                                where ClientKind = 1 " + where + @"
                                and UserID in (
                                    select userid from (
                                       select GZUserID userid from (
                                            select count(distinct userid) num,GZUserID from tb_b_user_gz group by GZUserID
                                       ) a where 1=1 " + gzsWhere + @" 
                                       intersect
                                       select assesseeuserid userid from (
                                            select count(1) num,assesseeuserid from tb_b_grade group by assesseeuserid
                                       ) a where 1=1 " + pjsWhere + @"
                                       intersect
                                       select assesseeuserid userid from (
                                            select count(distinct gradeuserid) num,assesseeuserid from tb_b_grade group by assesseeuserid
                                       ) a where 1=1 " + dprsWhere + @"
                                       intersect
                                       select SaleUserID userid from (
                                            select count(1) num,SaleUserID from tb_b_order where ZhiFuZT = 1 and Status = 0 group by SaleUserID
                                       ) a where 1=1 " + gmcsWhere + @"
                                       intersect
                                       select SaleUserID userid from (
                                            select count(distinct BuyUserID) num,SaleUserID from tb_b_order where ZhiFuZT = 1 and Status = 0 group by SaleUserID
                                       ) a where 1=1 " + gmrsWhere + @"
                                       intersect
                                       select SaleRecordUserID userid from (
                                            select count(1) num,SaleRecordUserID from tb_b_salerecord where SaleRecordLX >= 1 and SaleRecordLX <= 3 and Status = 0 and SaleRecordVerifyType = 1 group by SaleRecordUserID
                                       ) a where 1=1 " + zfbcsWhere + @"
                                       intersect
                                       select CardUserID userid from (
                                           select a.num,a.CardUserID,case when b.num1 is not null then b.num1 else 0 end num1 from (
                                                select count(1) num,CardUserID from tb_b_mycard group by CardUserID
                                           ) a left join (
                                                select count(1) num1,CardUserID from tb_b_mycard where status = 3 group by CardUserID
                                           ) b on a.CardUserID = b.CardUserID
                                       ) a where 1=1 " + tklWhere + @"
                                    ) a
                                )";

                //开始取分页数据
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

    [CSMethod("GetZxzfbTjToFile", 2)]
    public byte[] GetZxzfbTjToFile(JSReader jsr)
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
                cells[0, 0].PutValue("账号");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("名称");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);

                #region 参数
                JSReader[] jsr_fenda = jsr.ToArray();
                string cx_gzs = jsr_fenda[0];
                string gzsWhere = "";
                if (!string.IsNullOrEmpty(cx_gzs))
                {
                    gzsWhere += " and a.num >=" + cx_gzs;
                }
                string cx_gzs2 = jsr_fenda[1];
                if (!string.IsNullOrEmpty(cx_gzs2))
                {
                    gzsWhere += " and a.num <=" + cx_gzs2;
                }
                string cx_zhpf = jsr_fenda[2];
                string where = "";
                if (!string.IsNullOrEmpty(cx_zhpf))
                {
                    where += "and grade >= " + cx_zhpf;
                }
                string cx_zhpf2 = jsr_fenda[3];
                if (!string.IsNullOrEmpty(cx_zhpf2))
                {
                    where += "and grade <= " + cx_zhpf2;
                }
                string cx_pjs = jsr_fenda[4];
                string pjsWhere = "";
                if (!string.IsNullOrEmpty(cx_pjs))
                {
                    pjsWhere += " and a.num >= " + cx_pjs;
                }
                string cx_pjs2 = jsr_fenda[5];
                if (!string.IsNullOrEmpty(cx_pjs2))
                {
                    pjsWhere += " and a.num <= " + cx_pjs2;
                }
                string cx_dprs = jsr_fenda[6];
                string dprsWhere = "";
                if (!string.IsNullOrEmpty(cx_dprs))
                {
                    dprsWhere += " and a.num >= " + cx_dprs;
                }
                string cx_dprs2 = jsr_fenda[7];
                if (!string.IsNullOrEmpty(cx_dprs2))
                {
                    dprsWhere += " and a.num <= " + cx_dprs2;
                }
                string cx_gmcs = jsr_fenda[8];
                string gmcsWhere = "";
                if (!string.IsNullOrEmpty(cx_gmcs))
                {
                    gmcsWhere += " and a.num >= " + cx_gmcs;
                }
                string cx_gmcs2 = jsr_fenda[9];
                if (!string.IsNullOrEmpty(cx_gmcs2))
                {
                    gmcsWhere += " and a.num <= " + cx_gmcs2;
                }
                string cx_gmrs = jsr_fenda[10];
                string gmrsWhere = "";
                if (!string.IsNullOrEmpty(cx_gmrs))
                {
                    gmrsWhere += " and a.num >= " + cx_gmrs;
                }
                string cx_gmrs2 = jsr_fenda[11];
                if (!string.IsNullOrEmpty(cx_gmrs2))
                {
                    gmrsWhere += " and a.num <= " + cx_gmrs2;
                }
                string cx_zfbcs = jsr_fenda[12];
                string zfbcsWhere = "";
                if (!string.IsNullOrEmpty(cx_zfbcs))
                {
                    zfbcsWhere += " and a.num >= " + cx_zfbcs;
                }
                string cx_zfbcs2 = jsr_fenda[13];
                if (!string.IsNullOrEmpty(cx_zfbcs2))
                {
                    zfbcsWhere += " and a.num <= " + cx_zfbcs2;
                }
                string cx_city = jsr_fenda[14];
                if (!string.IsNullOrEmpty(cx_city))
                {
                    where += " and DqBm = " + dbc.ToSqlValue(cx_city);
                }
                string cx_isSq = jsr_fenda[15];
                if (!string.IsNullOrEmpty(cx_isSq))
                {
                    where += " and authorizestatus = " + cx_isSq;
                }
                string cx_isRz = jsr_fenda[16];
                if (!string.IsNullOrEmpty(cx_isRz))
                {
                    where += " and isidentification = " + cx_isRz;
                }
                string cx_tkl = jsr_fenda[17];
                string tklWhere = "";
                if (!string.IsNullOrEmpty(cx_tkl))
                {
                    tklWhere += " and Convert(decimal(18,4),CAST(num1 AS FLOAT)/num*100) > " + cx_tkl;
                }
                string cx_tkl2 = jsr_fenda[18];
                if (!string.IsNullOrEmpty(cx_tkl2))
                {
                    tklWhere += " and Convert(decimal(18,4),CAST(num1 AS FLOAT)/num*100) < " + cx_tkl2;
                }
                #endregion

                string str = @"select UserName,UserXM from tb_b_user 
                                where ClientKind = 1 " + where + @"
                                and UserID in (
                                    select userid from (
                                       select GZUserID userid from (
                                            select count(distinct userid) num,GZUserID from tb_b_user_gz group by GZUserID
                                       ) a where 1=1 " + gzsWhere + @" 
                                       intersect
                                       select assesseeuserid userid from (
                                            select count(1) num,assesseeuserid from tb_b_grade group by assesseeuserid
                                       ) a where 1=1 " + pjsWhere + @"
                                       intersect
                                       select assesseeuserid userid from (
                                            select count(distinct gradeuserid) num,assesseeuserid from tb_b_grade group by assesseeuserid
                                       ) a where 1=1 " + dprsWhere + @"
                                       intersect
                                       select SaleUserID userid from (
                                            select count(1) num,SaleUserID from tb_b_order where ZhiFuZT = 1 and Status = 0 group by SaleUserID
                                       ) a where 1=1 " + gmcsWhere + @"
                                       intersect
                                       select SaleUserID userid from (
                                            select count(distinct BuyUserID) num,SaleUserID from tb_b_order where ZhiFuZT = 1 and Status = 0 group by SaleUserID
                                       ) a where 1=1 " + gmrsWhere + @"
                                       intersect
                                       select SaleRecordUserID userid from (
                                            select count(1) num,SaleRecordUserID from tb_b_salerecord where SaleRecordLX >= 1 and SaleRecordLX <= 3 and Status = 0 and SaleRecordVerifyType = 1 group by SaleRecordUserID
                                       ) a where 1=1 " + zfbcsWhere + @"
                                       intersect
                                       select CardUserID userid from (
                                           select a.num,a.CardUserID,case when b.num1 is not null then b.num1 else 0 end num1 from (
                                                select count(1) num,CardUserID from tb_b_mycard group by CardUserID
                                           ) a left join (
                                                select count(1) num1,CardUserID from tb_b_mycard where status = 3 group by CardUserID
                                           ) b on a.CardUserID = b.CardUserID
                                       ) a where 1=1 " + tklWhere + @"
                                    ) a
                                )";

                DataTable dt = dbc.ExecuteDataTable(str);

                for (int i = 0; i < dt.Rows.Count; i++)
                {


                    if (!string.IsNullOrEmpty(dt.Rows[i]["UserName"].ToString()))
                    {
                        cells[i + 1, 0].PutValue(dt.Rows[i]["UserName"]);
                    }
                    else
                    {
                        cells[i + 1, 0].PutValue("");
                    }
                    cells[i + 1, 0].SetStyle(style4);

                    cells[i + 1, 1].PutValue(dt.Rows[i]["UserXM"]);
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
    #endregion

    #region 红包统计**需求20191111**
    #region 抽奖红包统计
    [CSMethod("GetCjhbTjLine")]
    public object GetCjhbTjLine(int pagnum, int pagesize, string beg, string end, string dqbm)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;
                string timeWhere = "";
                string dqBmWhere = "";

                if (!string.IsNullOrEmpty(beg))
                {
                    timeWhere += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    timeWhere += " and a.AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(dqbm))
                {
                    dqBmWhere += " and " + dbc.C_EQ("DqBm", dqbm);
                }

                string str = @"select * from (
                                    select d.UserName,a.addtime,a.money,
                                    case a.isuse when 0 then '未使用' when 1 then '待支付' when 2 then '已过期' when 3 then '已废弃' when 9 then '已支付' end isuse,
                                    a.updatetime,c.OrderCode
                                    from tb_b_redenvelope a
                                    left join tb_b_order_redenvelope b on a.redenvelopeid = b.redenvelopeid and b.status = 9
                                    left join tb_b_order c on b.orderid = c.OrderID
                                    left join tb_b_user d on a.userid = d.UserID
                                    where 1=1 " + timeWhere + @" and type = 4 and a.userid in (
                                        select distinct BuyUserID from tb_b_order where SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + @") and ZhiFuZT = 1 and status = 0
                                    )
                                    union
                                    select d.UserName,a.addtime,a.money,
                                    case a.isuse when 0 then '未使用' when 1 then '待支付' when 2 then '已过期' when 3 then '已废弃' when 9 then '已支付' end isuse,
                                    a.updatetime,c.OrderCode
                                    from tb_b_redenvelope a
                                    left join tb_b_order_redenvelope b on a.redenvelopeid = b.redenvelopeid and b.status = 9
                                    left join tb_b_order c on b.orderid = c.OrderID
                                    left join tb_b_user d on a.userid = d.UserID
                                    where 1=1 " + timeWhere + @" and type = 4 and a.userid in (
                                        select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                    )
                                ) a order by a.addtime desc";

                DataTable dtPage = new DataTable();
                dtPage = dbc.GetPagedDataTable(str, pagesize, ref cp, out ac);
                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetCjhbTj")]
    public object GetCjhbTj(string beg, string end, string dqbm)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string timeWhere = "";
                string timeWhere2 = "";
                string dqBmWhere = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    timeWhere += " and  AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                    timeWhere2 += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    timeWhere += " and AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                    timeWhere2 += " and a.AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(dqbm))
                {
                    dqBmWhere += " and " + dbc.C_EQ("DqBm", dqbm);
                }
                string str = @"select a.ordernum,b.gradenum,c.usernum,d.rafflenum,d.rafflemoney,e.zero, five, twenty, fifty, twohundred, fivehundred from (
                                select count(1) ordernum,0 type from tb_b_order
                                where ZhiFuZT = 1 and Status = 0 and SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + ") " + timeWhere + @"
                            ) a
                            left join (
                                select count(1) gradenum,0 type from tb_b_grade
                                where Status = 0 and assesseeuserid in (select userid from tb_b_user where 1=1 " + dqBmWhere + ") " + timeWhere + @"
                            ) b on a.type = b.type
                            left join (
                                select count(distinct BuyUserID) usernum,0 type from tb_b_order
                                where ZhiFuZT = 1 and Status = 0 and SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + ") " + timeWhere + @"
                            ) c on a.type = c.type
                            left join (
                                select count(a.id) rafflenum,sum(a.rafflemoney) rafflemoney,0 type from tb_b_raffle a
                                left join tb_b_pay b on a.payid = b.PayID
                                where Status = 0 and CardUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + ") " + timeWhere2 + @"
                            ) d on a.type = d.type
                            left join (
                                select type,
                                max(case rafflemoney when 0 then rafflenum else 0 end) zero,
                                max(case rafflemoney when 5 then rafflenum else 0 end) five,
                                max(case rafflemoney when 20 then rafflenum else 0 end) twenty,
                                max(case rafflemoney when 50 then rafflenum else 0 end) fifty,
                                max(case rafflemoney when 200 then rafflenum else 0 end) twohundred,
                                max(case rafflemoney when 200 then rafflenum else 0 end) fivehundred
                            from (
                                select count(a.id) rafflenum,CONVERT(decimal(18,0),rafflemoney) rafflemoney,0 type from tb_b_raffle a
                                left join tb_b_pay b on a.payid = b.PayID
                                where Status = 0 and CardUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + ") " + timeWhere2 + @"
                                group by rafflemoney
                            ) a group by a.type
                            ) e on a.type = e.type";
                DataTable dt = dbc.ExecuteDataTable(str);

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetCjhbTjLineToFile", 2)]
    public byte[] GetCjhbTjLineToFile(string beg, string end, string dqbm)
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
                cells[0, 0].PutValue("账号");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("获取时间");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 30);
                cells[0, 2].PutValue("红包金额");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("红包状态");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("更新时间");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 30);
                cells[0, 5].PutValue("对应使用订单号");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 30);

                string timeWhere = "";
                string dqBmWhere = "";

                if (!string.IsNullOrEmpty(beg))
                {
                    timeWhere += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    timeWhere += " and a.AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(dqbm))
                {
                    dqBmWhere += " and " + dbc.C_EQ("DqBm", dqbm);
                }

                string str = @"select * from (
                                    select d.UserName,a.addtime,a.money,
                                    case a.isuse when 0 then '未使用' when 1 then '待支付' when 2 then '已过期' when 3 then '已废弃' when 9 then '已支付' end isuse,
                                    a.updatetime,c.OrderCode
                                    from tb_b_redenvelope a
                                    left join tb_b_order_redenvelope b on a.redenvelopeid = b.redenvelopeid and b.status = 9
                                    left join tb_b_order c on b.orderid = c.OrderID
                                    left join tb_b_user d on a.userid = d.UserID
                                    where 1=1 " + timeWhere + @" and type = 4 and a.userid in (
                                        select distinct BuyUserID from tb_b_order where SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + @") and ZhiFuZT = 1 and status = 0
                                    )
                                    union
                                    select d.UserName,a.addtime,a.money,
                                    case a.isuse when 0 then '未使用' when 1 then '待支付' when 2 then '已过期' when 3 then '已废弃' when 9 then '已支付' end isuse,
                                    a.updatetime,c.OrderCode
                                    from tb_b_redenvelope a
                                    left join tb_b_order_redenvelope b on a.redenvelopeid = b.redenvelopeid and b.status = 9
                                    left join tb_b_order c on b.orderid = c.OrderID
                                    left join tb_b_user d on a.userid = d.UserID
                                    where 1=1 " + timeWhere + @" and type = 4 and a.userid in (
                                        select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                    )
                                ) a order by a.addtime desc";
                DataTable dt = dbc.ExecuteDataTable(str);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["UserName"] != null && dt.Rows[i]["UserName"].ToString() != "")
                    {
                        cells[i + 1, 0].PutValue(dt.Rows[i]["UserName"]);
                    }
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["addtime"] != null && dt.Rows[i]["addtime"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(Convert.ToDateTime(dt.Rows[i]["addtime"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["money"] != null && dt.Rows[i]["money"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(dt.Rows[i]["money"]);
                    }
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["isuse"] != null && dt.Rows[i]["isuse"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(dt.Rows[i]["isuse"]);
                    }
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["updatetime"] != null && dt.Rows[i]["updatetime"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(Convert.ToDateTime(dt.Rows[i]["updatetime"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 4].SetStyle(style4);
                    if (dt.Rows[i]["OrderCode"] != null && dt.Rows[i]["OrderCode"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(dt.Rows[i]["OrderCode"]);
                    }
                    cells[i + 1, 5].SetStyle(style4);
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
    #endregion

    #region 新人红包统计
    [CSMethod("GetXrhbTjLine")]
    public object GetXrhbTjLine(int pagnum, int pagesize, string beg, string end, string dqbm)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;
                string timeWhere = "";
                string dqBmWhere = "";

                if (!string.IsNullOrEmpty(beg))
                {
                    timeWhere += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    timeWhere += " and a.AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(dqbm))
                {
                    dqBmWhere += " and " + dbc.C_EQ("DqBm", dqbm);
                }

                string str = @"select * from (
                                select d.UserName,a.addtime,a.money,
                                case a.isuse when 0 then '未使用' when 1 then '待支付' when 2 then '已过期' when 3 then '已废弃' when 9 then '已支付' end isuse,
                                a.updatetime,c.OrderCode
                                from tb_b_redenvelope a
                                left join tb_b_order_redenvelope b on a.redenvelopeid = b.redenvelopeid and b.status = 9
                                left join tb_b_order c on b.orderid = c.OrderID
                                left join tb_b_user d on a.userid = d.UserID
                                where 1=1 " + timeWhere + @" and type = 0 and a.userid in (
                                    select distinct BuyUserID from tb_b_order where SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + @") and ZhiFuZT = 1 and status = 0
                                )
                                union
                                select d.UserName,a.addtime,a.money,
                                case a.isuse when 0 then '未使用' when 1 then '待支付' when 2 then '已过期' when 3 then '已废弃' when 9 then '已支付' end isuse,
                                a.updatetime,c.OrderCode
                                from tb_b_redenvelope a
                                left join tb_b_order_redenvelope b on a.redenvelopeid = b.redenvelopeid and b.status = 9
                                left join tb_b_order c on b.orderid = c.OrderID
                                left join tb_b_user d on a.userid = d.UserID
                                where  1=1 " + timeWhere + @" and type = 0 and a.userid in (
                                    select userid from tb_b_user where  1=1 " + dqBmWhere + @" and ClientKind = 2
                                )
                            ) a order by a.addtime desc";

                DataTable dtPage = new DataTable();
                dtPage = dbc.GetPagedDataTable(str, pagesize, ref cp, out ac);
                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetXrhbTjLineToFile", 2)]
    public byte[] GetXrhbTjLineToFile(string beg, string end, string dqbm)
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
                cells[0, 0].PutValue("账号");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("获取时间");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 30);
                cells[0, 2].PutValue("红包金额");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("红包状态");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("更新时间");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 30);
                cells[0, 5].PutValue("对应使用订单号");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 30);

                string timeWhere = "";
                string dqBmWhere = "";

                if (!string.IsNullOrEmpty(beg))
                {
                    timeWhere += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    timeWhere += " and a.AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(dqbm))
                {
                    dqBmWhere += " and " + dbc.C_EQ("DqBm", dqbm);
                }

                string str = @"select * from (
                                select d.UserName,a.addtime,a.money,
                                case a.isuse when 0 then '未使用' when 1 then '待支付' when 2 then '已过期' when 3 then '已废弃' when 9 then '已支付' end isuse,
                                a.updatetime,c.OrderCode
                                from tb_b_redenvelope a
                                left join tb_b_order_redenvelope b on a.redenvelopeid = b.redenvelopeid and b.status = 9
                                left join tb_b_order c on b.orderid = c.OrderID
                                left join tb_b_user d on a.userid = d.UserID
                                where 1=1 " + timeWhere + @" and type = 0 and a.userid in (
                                    select distinct BuyUserID from tb_b_order where SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + @") and ZhiFuZT = 1 and status = 0
                                )
                                union
                                select d.UserName,a.addtime,a.money,
                                case a.isuse when 0 then '未使用' when 1 then '待支付' when 2 then '已过期' when 3 then '已废弃' when 9 then '已支付' end isuse,
                                a.updatetime,c.OrderCode
                                from tb_b_redenvelope a
                                left join tb_b_order_redenvelope b on a.redenvelopeid = b.redenvelopeid and b.status = 9
                                left join tb_b_order c on b.orderid = c.OrderID
                                left join tb_b_user d on a.userid = d.UserID
                                where  1=1 " + timeWhere + @" and type = 0 and a.userid in (
                                    select userid from tb_b_user where  1=1 " + dqBmWhere + @" and ClientKind = 2
                                )
                            ) a order by a.addtime desc";
                DataTable dt = dbc.ExecuteDataTable(str);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["UserName"] != null && dt.Rows[i]["UserName"].ToString() != "")
                    {
                        cells[i + 1, 0].PutValue(dt.Rows[i]["UserName"]);
                    }
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["addtime"] != null && dt.Rows[i]["addtime"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(Convert.ToDateTime(dt.Rows[i]["addtime"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["money"] != null && dt.Rows[i]["money"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(dt.Rows[i]["money"]);
                    }
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["isuse"] != null && dt.Rows[i]["isuse"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(dt.Rows[i]["isuse"]);
                    }
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["updatetime"] != null && dt.Rows[i]["updatetime"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(Convert.ToDateTime(dt.Rows[i]["updatetime"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 4].SetStyle(style4);
                    if (dt.Rows[i]["OrderCode"] != null && dt.Rows[i]["OrderCode"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(dt.Rows[i]["OrderCode"]);
                    }
                    cells[i + 1, 5].SetStyle(style4);
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
    #endregion

    #region 订单分享红包统计
    [CSMethod("GetDdfxhbTjLine")]
    public object GetDdfxhbTjLine(int pagnum, int pagesize, string beg, string end, string dqbm)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;
                string timeWhere = "";
                string dqBmWhere = "";

                if (!string.IsNullOrEmpty(beg))
                {
                    timeWhere += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    timeWhere += " and a.AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(dqbm))
                {
                    dqBmWhere += " and " + dbc.C_EQ("DqBm", dqbm);
                }

                string str = @"select * from (
                                select d.UserName,a.addtime,a.money,
                                case a.isuse when 0 then '未使用' when 1 then '待支付' when 2 then '已过期' when 3 then '已废弃' when 9 then '已支付' end isuse,
                                a.updatetime,c.OrderCode
                                from tb_b_redenvelope a
                                left join tb_b_order_redenvelope b on a.redenvelopeid = b.redenvelopeid and b.status = 9
                                left join tb_b_order c on b.orderid = c.OrderID
                                left join tb_b_user d on a.userid = d.UserID
                                where 1=1 " + timeWhere + @" and type = 2 and a.userid in (
                                    select distinct BuyUserID from tb_b_order where SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + @") and ZhiFuZT = 1 and status = 0
                                )
                                union
                                select d.UserName,a.addtime,a.money,
                                case a.isuse when 0 then '未使用' when 1 then '待支付' when 2 then '已过期' when 3 then '已废弃' when 9 then '已支付' end isuse,
                                a.updatetime,c.OrderCode
                                from tb_b_redenvelope a
                                left join tb_b_order_redenvelope b on a.redenvelopeid = b.redenvelopeid and b.status = 9
                                left join tb_b_order c on b.orderid = c.OrderID
                                left join tb_b_user d on a.userid = d.UserID
                                where 1=1 " + timeWhere + @" and type = 2 and a.userid in (
                                    select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                )
                            ) a order by a.addtime desc";

                DataTable dtPage = new DataTable();
                dtPage = dbc.GetPagedDataTable(str, pagesize, ref cp, out ac);
                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetDdfxhbTj")]
    public object GetDdfxhbTj(string beg, string end, string dqbm)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string timeWhere = "";
                string dqBmWhere = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    timeWhere += " and  AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    timeWhere += " and AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(dqbm))
                {
                    dqBmWhere += " and " + dbc.C_EQ("DqBm", dqbm);
                }
                string str = @"select a.sharenum,b.orderredenvelopenum,b.orderredenvelopemoney,
                                c.firstordernum,c.orderordernum,d.useredenvelopenum,d.useredenvelopemoney,
                                e.ordernum,e.ordernum,e.orderpoints,e.avgpoints,
                                f.newordernum,f.newordernum,f.neworderpoints,f.newavgpoints,g.newsharenum
                                from (
                                    select count(1) sharenum,0 type from tb_b_share a where 1=1 " + timeWhere + @" and userid in (
                                        select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                        union
                                        select distinct BuyUserID from tb_b_order where SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 1) and ZhiFuZT = 1 and Status = 0
                                    )
                                ) a
                                left join
                                (
                                    select count(1) orderredenvelopenum,sum(money) orderredenvelopemoney,0 type from tb_b_redenvelope a where 1=1 " + timeWhere + @" and type = 2 and userid in (
                                        select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                        union
                                        select distinct BuyUserID from tb_b_order where SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 1) and ZhiFuZT = 1 and Status = 0
                                    )
                                ) b on a.type = b.type
                                left join
                                (
                                    select a.type,
                                           max(case a.money when 50 then a.num end) firstordernum,
                                           max(case a.money when 10 then a.num end) orderordernum
                                    from (
                                        select count(1) num, money,0 type from tb_b_redenvelope a where 1=1 " + timeWhere + @" and type = 2 and userid in (
                                            select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                            union
                                            select distinct BuyUserID from tb_b_order where SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 1) and ZhiFuZT = 1 and Status = 0
                                        ) group by money,type
                                    ) a group by a.type
                                ) c on a.type = c.type
                                left join
                                (
                                    select count(1) useredenvelopenum,sum(money) useredenvelopemoney,0 type from tb_b_redenvelope a where 1=1 " + timeWhere + @" and type = 2 and isuse = 9 and userid in (
                                        select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                        union
                                        select distinct BuyUserID from tb_b_order where SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 1) and ZhiFuZT = 1 and Status = 0
                                    )
                                ) d on a.type = d.type
                                left join
                                (
                                    select count(1) ordernum,count(distinct SaleUserID) zxordernum,sum(a.Points) orderpoints,CONVERT(decimal(18,2),sum(a.Points)/count(1)) avgpoints,0 type from tb_b_order a
                                    left join tb_b_order_redenvelope b on a.OrderID = b.orderid
                                    where a.ZhiFuZT = 1 and a.Status = 0 and b.redenvelopeid in (
                                       select redenvelopeid from tb_b_redenvelope a where 1=1 " + timeWhere + @" and type = 2 and isuse = 9 and userid in (
                                            select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                            union
                                            select distinct BuyUserID from tb_b_order where SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 1) and ZhiFuZT = 1 and Status = 0
                                       )
                                    )
                                ) e on a.type = e.type
                                left join (
                                    select count(1) newordernum,count(distinct SaleUserID) newzxordernum,sum(Points) neworderpoints,CONVERT(decimal(18,2),sum(Points)/count(1)) newavgpoints,0 type from tb_b_order
                                    where ZhiFuZT = 1 and Status = 0 and BuyUserID in (
                                        select b.UserID from tb_b_share a
                                        inner join tb_b_user b on a.tel = b.username
                                        where a.userid in (
                                           select userid from tb_b_redenvelope a where 1=1 " + timeWhere + @" and type = 2 and isuse = 9 and userid in (
                                                select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                                union
                                                select distinct BuyUserID from tb_b_order where SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 1) and ZhiFuZT = 1 and Status = 0
                                            )
                                        )
                                    )
                                ) f on a.type = f.type
                                left join (
                                    select count(1) newsharenum,0 type from tb_b_share
                                    where userid in (
                                        select b.UserID from tb_b_share a
                                        inner join tb_b_user b on a.tel = b.username
                                        where a.userid in (
                                           select userid from tb_b_redenvelope a where 1=1 " + timeWhere + @" and type = 2 and isuse = 9 and userid in (
                                                select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                                union
                                                select distinct BuyUserID from tb_b_order where SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 1) and ZhiFuZT = 1 and Status = 0
                                           )
                                        )
                                    )
                                ) g on a.type = g.type
                                ";
                DataTable dt = dbc.ExecuteDataTable(str);

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetDdfxhbTjLineToFile", 2)]
    public byte[] GetDdfxhbTjLineToFile(string beg, string end, string dqbm)
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
                cells[0, 0].PutValue("账号");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("获取时间");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 30);
                cells[0, 2].PutValue("红包金额");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("红包状态");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("更新时间");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 30);
                cells[0, 5].PutValue("对应使用订单号");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 30);

                string timeWhere = "";
                string dqBmWhere = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    timeWhere += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    timeWhere += " and a.AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(dqbm))
                {
                    dqBmWhere += " and " + dbc.C_EQ("DqBm", dqbm);
                }

                string str = @"select * from (
                                select d.UserName,a.addtime,a.money,
                                case a.isuse when 0 then '未使用' when 1 then '待支付' when 2 then '已过期' when 3 then '已废弃' when 9 then '已支付' end isuse,
                                a.updatetime,c.OrderCode
                                from tb_b_redenvelope a
                                left join tb_b_order_redenvelope b on a.redenvelopeid = b.redenvelopeid and b.status = 9
                                left join tb_b_order c on b.orderid = c.OrderID
                                left join tb_b_user d on a.userid = d.UserID
                                where 1=1 " + timeWhere + @" and type = 2 and a.userid in (
                                    select distinct BuyUserID from tb_b_order where SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + @") and ZhiFuZT = 1 and status = 0
                                )
                                union
                                select d.UserName,a.addtime,a.money,
                                case a.isuse when 0 then '未使用' when 1 then '待支付' when 2 then '已过期' when 3 then '已废弃' when 9 then '已支付' end isuse,
                                a.updatetime,c.OrderCode
                                from tb_b_redenvelope a
                                left join tb_b_order_redenvelope b on a.redenvelopeid = b.redenvelopeid and b.status = 9
                                left join tb_b_order c on b.orderid = c.OrderID
                                left join tb_b_user d on a.userid = d.UserID
                                where 1=1 " + timeWhere + @" and type = 2 and a.userid in (
                                    select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                )
                            ) a order by a.addtime desc";
                DataTable dt = dbc.ExecuteDataTable(str);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["UserName"] != null && dt.Rows[i]["UserName"].ToString() != "")
                    {
                        cells[i + 1, 0].PutValue(dt.Rows[i]["UserName"]);
                    }
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["addtime"] != null && dt.Rows[i]["addtime"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(Convert.ToDateTime(dt.Rows[i]["addtime"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["money"] != null && dt.Rows[i]["money"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(dt.Rows[i]["money"]);
                    }
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["isuse"] != null && dt.Rows[i]["isuse"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(dt.Rows[i]["isuse"]);
                    }
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["updatetime"] != null && dt.Rows[i]["updatetime"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(Convert.ToDateTime(dt.Rows[i]["updatetime"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 4].SetStyle(style4);
                    if (dt.Rows[i]["OrderCode"] != null && dt.Rows[i]["OrderCode"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(dt.Rows[i]["OrderCode"]);
                    }
                    cells[i + 1, 5].SetStyle(style4);
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
    #endregion

    #region 派送红包统计
    [CSMethod("GetPshbTjLine")]
    public object GetPshbTjLine(int pagnum, int pagesize, string beg, string end, string dqbm)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;
                string timeWhere = "";
                string dqBmWhere = "";

                if (!string.IsNullOrEmpty(beg))
                {
                    timeWhere += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    timeWhere += " and a.AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(dqbm))
                {
                    dqBmWhere += " and " + dbc.C_EQ("DqBm", dqbm);
                }

                string str = @"select * from (
                                    select d.UserName,a.addtime,a.money,
                                    case a.isuse when 0 then '未使用' when 1 then '待支付' when 2 then '已过期' when 3 then '已废弃' when 9 then '已支付' end isuse,
                                    a.updatetime,c.OrderCode
                                    from tb_b_redenvelope a
                                    left join tb_b_order_redenvelope b on a.redenvelopeid = b.redenvelopeid and b.status = 9
                                    left join tb_b_order c on b.orderid = c.OrderID
                                    left join tb_b_user d on a.userid = d.UserID
                                    where 1=1 " + timeWhere + @" and type = 3 and a.userid in (
                                        select distinct BuyUserID from tb_b_order where SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + @") and ZhiFuZT = 1 and status = 0
                                    )
                                    union
                                    select d.UserName,a.addtime,a.money,
                                    case a.isuse when 0 then '未使用' when 1 then '待支付' when 2 then '已过期' when 3 then '已废弃' when 9 then '已支付' end isuse,
                                    a.updatetime,c.OrderCode
                                    from tb_b_redenvelope a
                                    left join tb_b_order_redenvelope b on a.redenvelopeid = b.redenvelopeid and b.status = 9
                                    left join tb_b_order c on b.orderid = c.OrderID
                                    left join tb_b_user d on a.userid = d.UserID
                                    where  1=1 " + timeWhere + @" and type = 3 and a.userid in (
                                        select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                    )
                                ) a order by a.addtime desc";

                DataTable dtPage = new DataTable();
                dtPage = dbc.GetPagedDataTable(str, pagesize, ref cp, out ac);
                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetPshbTjToFile", 2)]
    public byte[] GetPshbTjToFile(string beg, string end, string dqbm)
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
                cells[0, 0].PutValue("账号");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("获取时间");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 30);
                cells[0, 2].PutValue("红包金额");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("红包状态");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("更新时间");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 30);
                cells[0, 5].PutValue("对应使用订单号");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 30);

                string timeWhere = "";
                string dqBmWhere = "";

                if (!string.IsNullOrEmpty(beg))
                {
                    timeWhere += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    timeWhere += " and a.AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(dqbm))
                {
                    dqBmWhere += " and " + dbc.C_EQ("DqBm", dqbm);
                }

                string str = @"select * from (
                                    select d.UserName,a.addtime,a.money,
                                    case a.isuse when 0 then '未使用' when 1 then '待支付' when 2 then '已过期' when 3 then '已废弃' when 9 then '已支付' end isuse,
                                    a.updatetime,c.OrderCode
                                    from tb_b_redenvelope a
                                    left join tb_b_order_redenvelope b on a.redenvelopeid = b.redenvelopeid and b.status = 9
                                    left join tb_b_order c on b.orderid = c.OrderID
                                    left join tb_b_user d on a.userid = d.UserID
                                    where 1=1 " + timeWhere + @" and type = 3 and a.userid in (
                                        select distinct BuyUserID from tb_b_order where SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + @") and ZhiFuZT = 1 and status = 0
                                    )
                                    union
                                    select d.UserName,a.addtime,a.money,
                                    case a.isuse when 0 then '未使用' when 1 then '待支付' when 2 then '已过期' when 3 then '已废弃' when 9 then '已支付' end isuse,
                                    a.updatetime,c.OrderCode
                                    from tb_b_redenvelope a
                                    left join tb_b_order_redenvelope b on a.redenvelopeid = b.redenvelopeid and b.status = 9
                                    left join tb_b_order c on b.orderid = c.OrderID
                                    left join tb_b_user d on a.userid = d.UserID
                                    where  1=1 " + timeWhere + @" and type = 3 and a.userid in (
                                        select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                    )
                                ) a order by a.addtime desc";
                DataTable dt = dbc.ExecuteDataTable(str);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["UserName"] != null && dt.Rows[i]["UserName"].ToString() != "")
                    {
                        cells[i + 1, 0].PutValue(dt.Rows[i]["UserName"]);
                    }
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["addtime"] != null && dt.Rows[i]["addtime"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(Convert.ToDateTime(dt.Rows[i]["addtime"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["money"] != null && dt.Rows[i]["money"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(dt.Rows[i]["money"]);
                    }
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["isuse"] != null && dt.Rows[i]["isuse"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(dt.Rows[i]["isuse"]);
                    }
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["updatetime"] != null && dt.Rows[i]["updatetime"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(Convert.ToDateTime(dt.Rows[i]["updatetime"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 4].SetStyle(style4);
                    if (dt.Rows[i]["OrderCode"] != null && dt.Rows[i]["OrderCode"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(dt.Rows[i]["OrderCode"]);
                    }
                    cells[i + 1, 5].SetStyle(style4);
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
    #endregion

    #region 新人复购红包统计
    [CSMethod("GetXrfghbTjLine")]
    public object GetXrfghbTjLine(int pagnum, int pagesize, string beg, string end, string dqbm)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;
                string timeWhere = "";
                string dqBmWhere = "";

                if (!string.IsNullOrEmpty(beg))
                {
                    timeWhere += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    timeWhere += " and a.AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(dqbm))
                {
                    dqBmWhere += " and " + dbc.C_EQ("DqBm", dqbm);
                }

                string str = @"select * from (
                                select d.UserName,a.addtime,a.money,
                                case a.isuse when 0 then '未使用' when 1 then '待支付' when 2 then '已过期' when 3 then '已废弃' when 9 then '已支付' end isuse,
                                a.updatetime,c.OrderCode
                                from tb_b_redenvelope a
                                left join tb_b_order_redenvelope b on a.redenvelopeid = b.redenvelopeid and b.status = 9
                                left join tb_b_order c on b.orderid = c.OrderID
                                left join tb_b_user d on a.userid = d.UserID
                                where 1=1 " + timeWhere + @" and type = 6 and a.userid in (
                                    select distinct BuyUserID from tb_b_order where SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + @") and ZhiFuZT = 1 and status = 0
                                )
                                union
                                select d.UserName,a.addtime,a.money,
                                case a.isuse when 0 then '未使用' when 1 then '待支付' when 2 then '已过期' when 3 then '已废弃' when 9 then '已支付' end isuse,
                                a.updatetime,c.OrderCode
                                from tb_b_redenvelope a
                                left join tb_b_order_redenvelope b on a.redenvelopeid = b.redenvelopeid and b.status = 9
                                left join tb_b_order c on b.orderid = c.OrderID
                                left join tb_b_user d on a.userid = d.UserID
                                where 1=1 " + timeWhere + @" and type = 6 and a.userid in (
                                    select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                )
                            ) a order by a.addtime desc";

                DataTable dtPage = new DataTable();
                dtPage = dbc.GetPagedDataTable(str, pagesize, ref cp, out ac);
                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetXrfghbTj")]
    public object GetXrfghbTj(string beg, string end, string dqbm)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string timeWhere = "";
                string dqBmWhere = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    timeWhere += " and  AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    timeWhere += " and AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(dqbm))
                {
                    dqBmWhere += " and " + dbc.C_EQ("DqBm", dqbm);
                }
                string str = @"select a.getnum,b.ordernum,b.zxnum,b.orderpoints,c.onebuynum,c.twobuynum,c.threebuynum,c.fourbuynum,c.morebuynum from (
                                    select count(distinct userid) getnum,0 type from tb_b_redenvelope a where 1=1 " + timeWhere + @" and userid in (
                                        select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                        union
                                        select distinct BuyUserID from tb_b_order where SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 1) and ZhiFuZT = 1 and Status = 0
                                    ) and type = 6
                                ) a
                                left join (
                                    select count(1) ordernum,count(distinct SaleUserID) zxnum,sum(Points) orderpoints,0 type from tb_b_order where ZhiFuZT = 1 and Status = 0 and BuyUserID in (
                                        select userid from tb_b_redenvelope a where 1=1 " + timeWhere + @" and userid in (
                                            select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                            union
                                            select distinct BuyUserID from tb_b_order where SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 1) and ZhiFuZT = 1 and Status = 0
                                        ) and type = 6
                                    )
                                ) b on a.type = b.type
                                left join (
                                    select type,
                                           max(case when ordernum = 1 then buynum end) onebuynum,
                                           max(case when ordernum = 2 then buynum end) twobuynum,
                                           max(case when ordernum = 3 then buynum end) threebuynum,
                                           max(case when ordernum = 4 then buynum end) fourbuynum,
                                           sum(case when ordernum > 4 then buynum end) morebuynum
                                    from (
                                        select count(BuyUserID) buynum,ordernum,0 type from (
                                            select count(1) ordernum,BuyUserID from tb_b_order where ZhiFuZT = 1 and Status = 0 and BuyUserID in (
                                                select userid from tb_b_redenvelope a where 1=1 " + timeWhere + @" and userid in (
                                                    select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                                    union
                                                    select distinct BuyUserID from tb_b_order where SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 1) and ZhiFuZT = 1 and Status = 0
                                                ) and type = 6
                                            ) group by BuyUserID
                                        ) a group by ordernum
                                    ) a group by type
                                ) c on a.type = c.type";
                DataTable dt = dbc.ExecuteDataTable(str);

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetXrfghbTjLineToFile", 2)]
    public byte[] GetXrfghbTjLineToFile(string beg, string end, string dqbm)
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
                cells[0, 0].PutValue("账号");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("获取时间");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 30);
                cells[0, 2].PutValue("红包金额");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("红包状态");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("更新时间");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 30);
                cells[0, 5].PutValue("对应使用订单号");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 30);

                string timeWhere = "";
                string dqBmWhere = "";

                if (!string.IsNullOrEmpty(beg))
                {
                    timeWhere += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    timeWhere += " and a.AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(dqbm))
                {
                    dqBmWhere += " and " + dbc.C_EQ("DqBm", dqbm);
                }

                string str = @"select * from (
                                select d.UserName,a.addtime,a.money,
                                case a.isuse when 0 then '未使用' when 1 then '待支付' when 2 then '已过期' when 3 then '已废弃' when 9 then '已支付' end isuse,
                                a.updatetime,c.OrderCode
                                from tb_b_redenvelope a
                                left join tb_b_order_redenvelope b on a.redenvelopeid = b.redenvelopeid and b.status = 9
                                left join tb_b_order c on b.orderid = c.OrderID
                                left join tb_b_user d on a.userid = d.UserID
                                where 1=1 " + timeWhere + @" and type = 6 and a.userid in (
                                    select distinct BuyUserID from tb_b_order where SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + @") and ZhiFuZT = 1 and status = 0
                                )
                                union
                                select d.UserName,a.addtime,a.money,
                                case a.isuse when 0 then '未使用' when 1 then '待支付' when 2 then '已过期' when 3 then '已废弃' when 9 then '已支付' end isuse,
                                a.updatetime,c.OrderCode
                                from tb_b_redenvelope a
                                left join tb_b_order_redenvelope b on a.redenvelopeid = b.redenvelopeid and b.status = 9
                                left join tb_b_order c on b.orderid = c.OrderID
                                left join tb_b_user d on a.userid = d.UserID
                                where 1=1 " + timeWhere + @" and type = 6 and a.userid in (
                                    select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                )
                            ) a order by a.addtime desc";
                DataTable dt = dbc.ExecuteDataTable(str);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["UserName"] != null && dt.Rows[i]["UserName"].ToString() != "")
                    {
                        cells[i + 1, 0].PutValue(dt.Rows[i]["UserName"]);
                    }
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["addtime"] != null && dt.Rows[i]["addtime"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(Convert.ToDateTime(dt.Rows[i]["addtime"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["money"] != null && dt.Rows[i]["money"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(dt.Rows[i]["money"]);
                    }
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["isuse"] != null && dt.Rows[i]["isuse"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(dt.Rows[i]["isuse"]);
                    }
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["updatetime"] != null && dt.Rows[i]["updatetime"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(Convert.ToDateTime(dt.Rows[i]["updatetime"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 4].SetStyle(style4);
                    if (dt.Rows[i]["OrderCode"] != null && dt.Rows[i]["OrderCode"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(dt.Rows[i]["OrderCode"]);
                    }
                    cells[i + 1, 5].SetStyle(style4);
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
    #endregion

    #region 赠送红包统计
    [CSMethod("GetZshbTjLine")]
    public object GetZshbTjLine(int pagnum, int pagesize, string beg, string end, string dqbm)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;
                string timeWhere = "";
                string dqBmWhere = "";

                if (!string.IsNullOrEmpty(beg))
                {
                    timeWhere += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    timeWhere += " and a.AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(dqbm))
                {
                    dqBmWhere += " and " + dbc.C_EQ("DqBm", dqbm);
                }

                string str = @"select * from (
                                select d.UserName,a.addtime,a.money,case a.isuse when 0 then '未使用' when 1 then '待支付' when 2 then '已过期' when 3 then '已废弃' when 9 then '已支付' end isuse,
                                a.updatetime,c.OrderCode
                                from tb_b_redenvelope a
                                left join tb_b_order_redenvelope b on a.redenvelopeid = b.redenvelopeid and b.status = 9
                                left join tb_b_order c on b.orderid = c.OrderID
                                left join tb_b_user d on a.userid = d.UserID
                                where 1=1 " + timeWhere + @" and type = 5 and a.userid in (
                                    select distinct BuyUserID from tb_b_order where SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + @") and ZhiFuZT = 1 and status = 0
                                )
                                union
                                select d.UserName,a.addtime,a.money,case a.isuse when 0 then '未使用' when 1 then '待支付' when 2 then '已过期' when 3 then '已废弃' when 9 then '已支付' end isuse,
                                a.updatetime,c.OrderCode
                                from tb_b_redenvelope a
                                left join tb_b_order_redenvelope b on a.redenvelopeid = b.redenvelopeid and b.status = 9
                                left join tb_b_order c on b.orderid = c.OrderID
                                left join tb_b_user d on a.userid = d.UserID
                                where 1=1 " + timeWhere + @" and type = 5 and a.userid in (
                                    select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                )
                            ) a order by a.addtime desc";

                DataTable dtPage = new DataTable();
                dtPage = dbc.GetPagedDataTable(str, pagesize, ref cp, out ac);
                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetZshbTj")]
    public object GetZshbTj(string beg, string end, string dqbm)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string timeWhere = "";
                string dqBmWhere = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    timeWhere += " and  AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    timeWhere += " and AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(dqbm))
                {
                    dqBmWhere += " and " + dbc.C_EQ("DqBm", dqbm);
                }
                string str = @"select a.getnum,b.ordernum,b.zxnum,b.orderpoints,b.avgorderpoints from (
                                    select count(distinct userid) getnum,0 type from tb_b_redenvelope a where 1=1 " + timeWhere + @" and userid in (
                                        select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                        union
                                        select distinct BuyUserID from tb_b_order where SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 1) and ZhiFuZT = 1 and Status = 0
                                    ) and type = 5
                                ) a
                                left join (
                                    select count(1) ordernum,count(distinct SaleUserID) zxnum,sum(Points) orderpoints,CONVERT(decimal(18,2),sum(Points)/count(1)) avgorderpoints,0 type from tb_b_order where ZhiFuZT = 1 and Status = 0 and ordercode in (
                                        select ordercode from tb_b_redenvelope a where 1=1 " + timeWhere + @" and userid in (
                                            select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                            union
                                            select distinct BuyUserID from tb_b_order where SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 1) and ZhiFuZT = 1 and Status = 0
                                        ) and type = 5
                                    )
                                ) b on a.type = b.type";
                DataTable dt = dbc.ExecuteDataTable(str);

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetZshbTjLineToFile", 2)]
    public byte[] GetZshbTjLineToFile(string beg, string end, string dqbm)
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
                cells[0, 0].PutValue("账号");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("获取时间");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 30);
                cells[0, 2].PutValue("红包金额");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("红包状态");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("更新时间");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 30);
                cells[0, 5].PutValue("对应使用订单号");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 30);

                string timeWhere = "";
                string dqBmWhere = "";

                if (!string.IsNullOrEmpty(beg))
                {
                    timeWhere += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    timeWhere += " and a.AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(dqbm))
                {
                    dqBmWhere += " and " + dbc.C_EQ("DqBm", dqbm);
                }

                string str = @"select * from (
                                select d.UserName,a.addtime,a.money,case a.isuse when 0 then '未使用' when 1 then '待支付' when 2 then '已过期' when 3 then '已废弃' when 9 then '已支付' end isuse,
                                a.updatetime,c.OrderCode
                                from tb_b_redenvelope a
                                left join tb_b_order_redenvelope b on a.redenvelopeid = b.redenvelopeid and b.status = 9
                                left join tb_b_order c on b.orderid = c.OrderID
                                left join tb_b_user d on a.userid = d.UserID
                                where 1=1 " + timeWhere + @" and type = 5 and a.userid in (
                                    select distinct BuyUserID from tb_b_order where SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + @") and ZhiFuZT = 1 and status = 0
                                )
                                union
                                select d.UserName,a.addtime,a.money,case a.isuse when 0 then '未使用' when 1 then '待支付' when 2 then '已过期' when 3 then '已废弃' when 9 then '已支付' end isuse,
                                a.updatetime,c.OrderCode
                                from tb_b_redenvelope a
                                left join tb_b_order_redenvelope b on a.redenvelopeid = b.redenvelopeid and b.status = 9
                                left join tb_b_order c on b.orderid = c.OrderID
                                left join tb_b_user d on a.userid = d.UserID
                                where 1=1 " + timeWhere + @" and type = 5 and a.userid in (
                                    select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                )
                            ) a order by a.addtime desc";
                DataTable dt = dbc.ExecuteDataTable(str);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["UserName"] != null && dt.Rows[i]["UserName"].ToString() != "")
                    {
                        cells[i + 1, 0].PutValue(dt.Rows[i]["UserName"]);
                    }
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["addtime"] != null && dt.Rows[i]["addtime"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(Convert.ToDateTime(dt.Rows[i]["addtime"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["money"] != null && dt.Rows[i]["money"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(dt.Rows[i]["money"]);
                    }
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["isuse"] != null && dt.Rows[i]["isuse"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(dt.Rows[i]["isuse"]);
                    }
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["updatetime"] != null && dt.Rows[i]["updatetime"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(Convert.ToDateTime(dt.Rows[i]["updatetime"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 4].SetStyle(style4);
                    if (dt.Rows[i]["OrderCode"] != null && dt.Rows[i]["OrderCode"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(dt.Rows[i]["OrderCode"]);
                    }
                    cells[i + 1, 5].SetStyle(style4);
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
    #endregion

    #region 订单使用红包情况
    [CSMethod("GetDdsyhbTjLine")]
    public object GetDdsyhbTjLine(int pagnum, int pagesize, string beg, string end, string dqbm)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;
                string timeWhere = "";
                string dqBmWhere = "";

                if (!string.IsNullOrEmpty(beg))
                {
                    timeWhere += " and a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    timeWhere += " and a.AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(dqbm))
                {
                    dqBmWhere += " and " + dbc.C_EQ("DqBm", dqbm);
                }

                string str = @"select a.*,b.UserName,c.SaleRecordPoints,c.SaleRecordUserXM from tb_b_order a
                                left join tb_b_user b on a.BuyUserID = b.UserID
                                left join tb_b_salerecord c on a.SaleRecordID=c.SaleRecordID
                                where a.ZhiFuZT = 1 and a.Status = 0 " + timeWhere + @" and a.BuyUserID in (
                                    select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                    union
                                    select distinct BuyUserID from tb_b_order where SaleUserID in (
                                        select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 1
                                    ) and ZhiFuZT = 1 and Status = 0
                                ) and a.redenvelopemoney > 0 and a.redenvelopemoney <= 70";

                DataTable dtPage = new DataTable();
                dtPage = dbc.GetPagedDataTable(str, pagesize, ref cp, out ac);
                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetDdsyhbTj")]
    public object GetDdsyhbTj(string beg, string end, string dqbm)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string timeWhere = "";
                string dqBmWhere = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    timeWhere += " and  AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    timeWhere += " and AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(dqbm))
                {
                    dqBmWhere += " and " + dbc.C_EQ("DqBm", dqbm);
                }
                string str = @"select sum(redenvelopemoney) usehbje,count(1) ordernum,sum(Points) orderpoints,count(distinct BuyUserID) fhrnum, count(distinct SaleUserID) zxnum from tb_b_order 
                                where ZhiFuZT = 1 and Status = 0 " + timeWhere + @" and BuyUserID in (
                                    select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                    union
                                    select distinct BuyUserID from tb_b_order where SaleUserID in (select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 1) and ZhiFuZT = 1 and Status = 0
                                ) and redenvelopemoney > 0 and redenvelopemoney <= 70";
                DataTable dt = dbc.ExecuteDataTable(str);

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetDdsyhbTjLineToFile", 2)]
    public byte[] GetDdsyhbTjLineToFile(string beg, string end, string dqbm)
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
                cells[0, 0].PutValue("账号");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("时间");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 30);
                cells[0, 2].PutValue("券额");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("专线名");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("红包金额");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 30);
                

                string timeWhere = "";
                string dqBmWhere = "";

                if (!string.IsNullOrEmpty(beg))
                {
                    timeWhere += " and a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    timeWhere += " and a.AddTime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(dqbm))
                {
                    dqBmWhere += " and " + dbc.C_EQ("DqBm", dqbm);
                }

                string str = @"select a.*,b.UserName,c.SaleRecordPoints,c.SaleRecordUserXM from tb_b_order a
                                left join tb_b_user b on a.BuyUserID = b.UserID
                                left join tb_b_salerecord c on a.SaleRecordID=c.SaleRecordID
                                where a.ZhiFuZT = 1 and a.Status = 0 " + timeWhere + @" and a.BuyUserID in (
                                    select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                    union
                                    select distinct BuyUserID from tb_b_order where SaleUserID in (
                                        select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 1
                                    ) and ZhiFuZT = 1 and Status = 0
                                ) and a.redenvelopemoney > 0 and a.redenvelopemoney <= 70";

                DataTable dt = dbc.ExecuteDataTable(str);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["UserName"] != null && dt.Rows[i]["UserName"].ToString() != "")
                    {
                        cells[i + 1, 0].PutValue(dt.Rows[i]["UserName"]);
                    }
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["AddTime"] != null && dt.Rows[i]["AddTime"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(Convert.ToDateTime(dt.Rows[i]["AddTime"]).ToString("yyyy-MM-dd hh:mm:ss"));
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["SaleRecordPoints"] != null && dt.Rows[i]["SaleRecordPoints"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(dt.Rows[i]["SaleRecordPoints"]);
                    }
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["SaleRecordUserXM"] != null && dt.Rows[i]["SaleRecordUserXM"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(dt.Rows[i]["SaleRecordUserXM"]);
                    }
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["redenvelopemoney"] != null && dt.Rows[i]["redenvelopemoney"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(dt.Rows[i]["redenvelopemoney"]);
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
    #endregion

    #region 省钱统计
    [CSMethod("GetSqTjLine")]
    public object GetSqTjLine(int pagnum, int pagesize, string money, string dqbm)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;
                string moneyWhere = "";
                string dqBmWhere = "";

                if (!string.IsNullOrEmpty(money))
                {
                    moneyWhere += " and a.savepoints>=" + money;
                }
                if (!string.IsNullOrEmpty(dqbm))
                {
                    dqBmWhere += " and " + dbc.C_EQ("DqBm", dqbm);
                }

                string str = @"select a.*,b.UserName from (
                                    select sum(Points - Money) savepoints,BuyUserID from tb_b_order where ZhiFuZT = 1 and Status = 0 and BuyUserID in (
                                        select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                        union
                                        select distinct BuyUserID from tb_b_order where SaleUserID in (
                                            select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 1
                                        ) and ZhiFuZT = 1 and Status = 0
                                    ) group by BuyUserID
                                ) a 
                                left join tb_b_user b on a.BuyUserID = b.UserID
                                where 1=1 " + moneyWhere;

                DataTable dtPage = new DataTable();
                dtPage = dbc.GetPagedDataTable(str, pagesize, ref cp, out ac);
                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetSqTjLineToFile", 2)]
    public byte[] GetSqTjLineToFile(string money, string dqbm)
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
                cells[0, 0].PutValue("账号");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("省钱金额");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 30);


                string moneyWhere = "";
                string dqBmWhere = "";

                if (!string.IsNullOrEmpty(money))
                {
                    moneyWhere += " and a.savepoints>=" + money;
                }
                if (!string.IsNullOrEmpty(dqbm))
                {
                    dqBmWhere += " and " + dbc.C_EQ("DqBm", dqbm);
                }

                string str = @"select a.*,b.UserName from (
                                    select sum(Points - Money) savepoints,BuyUserID from tb_b_order where ZhiFuZT = 1 and Status = 0 and BuyUserID in (
                                        select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 2
                                        union
                                        select distinct BuyUserID from tb_b_order where SaleUserID in (
                                            select userid from tb_b_user where 1=1 " + dqBmWhere + @" and ClientKind = 1
                                        ) and ZhiFuZT = 1 and Status = 0
                                    ) group by BuyUserID
                                ) a 
                                left join tb_b_user b on a.BuyUserID = b.UserID
                                where 1=1 " + moneyWhere;

                DataTable dt = dbc.ExecuteDataTable(str);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["UserName"] != null && dt.Rows[i]["UserName"].ToString() != "")
                    {
                        cells[i + 1, 0].PutValue(dt.Rows[i]["UserName"]);
                    }
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["savepoints"] != null && dt.Rows[i]["savepoints"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["savepoints"]);
                    }
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

    #endregion
    #endregion
}
