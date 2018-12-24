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

                string str = @"select a.UserID,a.UserXM,a.UserName,a.Addtime,b.sykkf,c.zsyfq,d.sxyfq,e.sqcs,f.gzs,a.Points,g.sxed from tb_b_user a left join 
                                (select sum(Points) as sykkf,UserID from tb_b_platpoints where status=0 group by UserID) b on a.UserID=b.UserID
                                left join
                                (select isNULL(a.points,0)+isnull(b.points,0) as zsyfq,a.UserID from 
                                (select sum(Points) as points,UserID from tb_b_plattosale where   status=0 and pointkind=0 and DATEDIFF(HOUR,addtime,getDate())<=validHour group by UserID ) a left join (
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
                                where 1=1 and a.ClientKind=1";
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

                string str = @"select a.UserID,a.UserXM,a.UserName,a.Addtime,b.sykkf,c.zsyfq,d.sxyfq,e.sqcs,f.gzs,a.Points,g.sxed from tb_b_user a left join 
                                (select sum(Points) as sykkf,UserID from tb_b_platpoints where status=0 group by UserID) b on a.UserID=b.UserID
                                left join
                                (select isNULL(a.points,0)+isnull(b.points,0) as zsyfq,a.UserID from 
                                (select sum(Points) as points,UserID from tb_b_plattosale where   status=0 and pointkind=0 and DATEDIFF(HOUR,addtime,getDate())<=validHour group by UserID ) a left join (
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
                                where 1=1 and a.ClientKind=1 "+where+" order by a.AddTime desc,a.UserName,a.UserXM";
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

                string str = @"select a.UserID,a.UserName,a.Addtime,b.gmyfq,c.ysyyfq,d.gmcs,e.gzs,f.syyfq from tb_b_user a left join 
                                (select sum(Points) as gmyfq,BuyUserID from tb_b_order where SaleRecordID in
                                (select SaleRecordID from  tb_b_salerecord where status=0 and SaleRecordLX=0 and SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9')
                                and status=0 and ZhiFuZT=1 group by BuyUserID) b on a.UserID=b.BuyUserID
                                left join 
                                (select sum(Points) as ysyyfq,PayUserID  from tb_b_pay where 
                                mycardId in(select a.mycardId from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                                where b.status=0 and b.SaleRecordLX=0 and b.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9')
                                group by PayUserID) c on a.UserID=c.PayUserID
                                left join 
                                (select count(OrderID) as gmcs,BuyUserID from tb_b_order where [SaleRecordID] in
                                (select SaleRecordID from  tb_b_salerecord where status=0 and SaleRecordLX=0 and SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9')
                                and status=0 and ZhiFuZT=1 group by BuyUserID) d on a.UserID=d.BuyUserID 
                                left join 
                                (select count(GZ_ID) as gzs,UserID from tb_b_user_gz group by UserID) e on a.UserID=e.UserID
                                left join(
                                select sum(a.points) as syyfq,a.UserID from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                                where b.status=0 and b.SaleRecordLX=0 and b.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9' and a.PointsEndTime>=getDate()
                                group by a.UserID) f on a.UserID=f.UserID
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
                cells[0, 2].PutValue("购买运费券");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("已使用运费券");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("购买次数");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("关注数");
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

                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.AddTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select a.UserID,a.UserName,a.Addtime,b.gmyfq,c.ysyyfq,d.gmcs,e.gzs,f.syyfq from tb_b_user a left join 
                                (select sum(Points) as gmyfq,BuyUserID from tb_b_order where SaleRecordID in
                                (select SaleRecordID from  tb_b_salerecord where status=0 and SaleRecordLX=0 and SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9')
                                and status=0 and ZhiFuZT=1 group by BuyUserID) b on a.UserID=b.BuyUserID
                                left join 
                                (select sum(Points) as ysyyfq,PayUserID  from tb_b_pay where 
                                mycardId in(select a.mycardId from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                                where b.status=0 and b.SaleRecordLX=0 and b.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9')
                                group by PayUserID) c on a.UserID=c.PayUserID
                                left join 
                                (select count(OrderID) as gmcs,BuyUserID from tb_b_order where [SaleRecordID] in
                                (select SaleRecordID from  tb_b_salerecord where status=0 and SaleRecordLX=0 and SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9')
                                and status=0 and ZhiFuZT=1 group by BuyUserID) d on a.UserID=d.BuyUserID 
                                left join 
                                (select count(GZ_ID) as gzs,UserID from tb_b_user_gz group by UserID) e on a.UserID=e.UserID
                                left join(
                                select sum(a.points) as syyfq,a.UserID from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                                where b.status=0 and b.SaleRecordLX=0 and b.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9' and a.PointsEndTime>=getDate()
                                group by a.UserID) f on a.UserID=f.UserID
                                where 1=1 and a.ClientKind=2 "+where+" order by a.AddTime desc,a.UserName,a.UserXM";
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
                    if (dt.Rows[i]["gmyfq"] != null && dt.Rows[i]["gmyfq"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(dt.Rows[i]["gmyfq"]);
                    }
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["ysyyfq"] != null && dt.Rows[i]["ysyyfq"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(dt.Rows[i]["ysyyfq"]);
                    }
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["gmcs"] != null && dt.Rows[i]["gmcs"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(dt.Rows[i]["gmcs"]);
                    }
                    cells[i + 1, 4].SetStyle(style4);
                    if (dt.Rows[i]["gzs"] != null && dt.Rows[i]["gzs"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(dt.Rows[i]["gzs"]);
                    }
                    cells[i + 1, 5].SetStyle(style4);
                    cells[i + 1, 6].PutValue(Convert.ToDateTime(dt.Rows[i]["AddTime"]).ToString("yyyy-MM-dd"));
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
                                  where a.[SaleRecordID] in
                                (select SaleRecordID from  tb_b_salerecord where status=0 and SaleRecordLX=0 and SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9')
                                and a.status=0 and a.ZhiFuZT=1 and a.BuyUserID=@UserID  order by a.AddTime desc
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
                        where b.status=0 and b.SaleRecordLX=0 and b.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9' and a.PointsEndTime>=getDate()
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
                        where b.status=0 and b.SaleRecordLX=0 and b.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9' and a.PointsEndTime>=getDate()
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
                                  where a.[SaleRecordID] in
                                (select SaleRecordID from  tb_b_salerecord where status=0 and SaleRecordLX=0 and SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9')
                                and a.status=0 and a.ZhiFuZT=1 and a.BuyUserID=@UserID  order by a.AddTime desc
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
                                   where 
                                mycardId in(select a.mycardId from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                                where b.status=0 and b.SaleRecordLX=0 and b.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9')
                                and a.PayUserID=@UserID order by AddTime  desc
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
                                mycardId in(select a.mycardId from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                                where b.status=0 and b.SaleRecordLX=0 and b.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9')
                                and a.PayUserID=@UserID order by AddTime  desc
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

    [CSMethod("GetZXZFList")]
    public object GetZXZFList(int pagnum, int pagesize, string yhm, string xm,string sc, string beg, string end)
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

                string sql = @"select max(SaleRecordTime) as maxtime,min(SaleRecordTime) as mintime from tb_b_salerecord where SaleRecordLX!=0 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
                    and status=0  ";
                DataTable sjdt = dbc.ExecuteDataTable(sql);

                string sjstr = "";

                //上架时间
                string sjwhere = "";    string sjwhere1 = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    sjwhere += " and  SaleRecordTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                    sjwhere1 += " and  b.SaleRecordTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                    sjstr += Convert.ToDateTime(beg).ToString("yyyy-MM-dd");
                }
                else
                {
                    if (sjdt.Rows.Count > 0)
                    {
                        if (sjdt.Rows[0]["mintime"] != null && sjdt.Rows[0]["mintime"].ToString() != "")
                        {
                            sjstr += Convert.ToDateTime(sjdt.Rows[0]["mintime"]).ToString("yyyy-MM-dd");
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
                    sjwhere += " and SaleRecordTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy年MM月dd日") + "'";
                    sjwhere1 += " and b.SaleRecordTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy年MM月dd日") + "'";
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

                string str = @"select '"+sjstr+@"' as rq,  a.UserID,a.FromRoute,a.UserName,a.UserXM,b.sjdzq,c.ysyq,d.gqwsy,e.qxnwsy,f.sxq from tb_b_user a left join 
                            (select sum(SaleRecordPoints) as sjdzq,SaleRecordUserID from tb_b_salerecord where SaleRecordLX!=0 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
                            and status=0 " + sjwhere + @"  group by SaleRecordUserID) b on a.UserID=b.SaleRecordUserID
                            left join
                            (select sum(Points) as ysyq,CardUserID from tb_b_pay where ReceiveUserID=CardUserID and  mycardId in (select a.mycardId from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                            where b.status=0 " + sjwhere1 + @" and b.SaleRecordLX!=0 and b.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1))
                             group by CardUserID) c on a.UserID=c.CardUserID
                             left join
                             (select sum(points) as gqwsy,CardUserID from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                            where b.status=0 " + sjwhere1 + @" and b.SaleRecordLX!=0 and b.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
                            and a.PointsEndTime<getDate() group by CardUserID) d on a.UserID=d.CardUserID
                            left join
                            (select sum(points) as qxnwsy,CardUserID from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                            where b.status=0 " + sjwhere1 + @" and b.SaleRecordLX!=0 and b.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
                            and a.PointsEndTime>=getDate() group by CardUserID) e on a.UserID=e.CardUserID
                            left join
                            (select isnull(a.sjdzq,0)-isnull(b.sxyfq,0) as sxq,a.SaleRecordUserID from (
                            select sum(SaleRecordPoints) as sjdzq,SaleRecordUserID from tb_b_salerecord where SaleRecordLX!=0 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
                            and status=0 " + sjwhere + @" and DATEDIFF(HOUR,SaleRecordTime,getDate())>validHour  group by SaleRecordUserID ) a 
                            left join (
                            select sum(Points) as sxyfq,SaleUserID from tb_b_order where [SaleRecordID] in
                            (select SaleRecordID from  tb_b_salerecord where status=0 " + sjwhere + @" and SaleRecordLX!=0 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1) 
                            and DATEDIFF(HOUR,SaleRecordTime,getDate())>validHour)
                            and status=0  and ZhiFuZT=1 group by SaleUserID) b on a.SaleRecordUserID=b.SaleUserID) f on a.UserID=f.SaleRecordUserID
                            where a.IsCanRelease=1 and a.ClientKind=1 ";
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

    [CSMethod("GetSFJYList")]
    public object GetSFJYList(int pagnum, int pagesize, string yhm, string xm,string beg, string end)
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
                    where += " and " + dbc.C_Like("b.UserName", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(xm.Trim()))
                {
                    where += " and " + dbc.C_Like("e.UserXM", xm.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  d.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and d.AddTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select b.UserName,d.AddTime as jysj,a.AddTime as xfsj,e.UserXM,c.OrderCode,d.Money,'消费' as flag  from tb_b_pay a left join tb_b_user b on a.PayUserID=b.UserID 
                                left join tb_b_mycard c on a.mycardId=c.mycardId 
                                left join tb_b_order d on c.OrderCode=d.OrderCode
                                left join tb_b_user e on a.CardUserID=e.UserID
                                where b.ClientKind=2 and d.status=0 and d.ZhiFuZT=1  "+where+@"
                                union all
                                select b.UserName,d.AddTime as jysj,null as xfsj,e.UserXM,a.OrderCode,d.Money,'购买' as flag from tb_b_mycard a  left join tb_b_user b on a.UserID=b.UserID 
                                left join tb_b_order d on a.OrderCode=d.OrderCode
                                left join tb_b_user e on a.CardUserID=e.UserID
                                 where b.ClientKind=2 and d.status=0 and d.ZhiFuZT=1 and a.PointsEndTime>=getDate()  " + where + @"
                                 union all
                                 select b.UserName,d.AddTime as jysj,null as xfsj,e.UserXM,a.OrderCode,d.Money,'过期' as flag from tb_b_mycard a  left join tb_b_user b on a.UserID=b.UserID 
                                left join tb_b_order d on a.OrderCode=d.OrderCode
                                left join tb_b_user e on a.CardUserID=e.UserID
                                 where b.ClientKind=2 and d.status=0 and d.ZhiFuZT=1 and a.status=0 and a.PointsEndTime<getDate() " + where + @"";

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by d.AddTime desc,b.UserName,e.UserXM", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
