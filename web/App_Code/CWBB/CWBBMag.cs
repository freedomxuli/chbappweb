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

                string str = @"select a.UserID,a.UserXM,a.UserName,a.Addtime,b.sykkf,c.zsyfq,d.sxyfq,e.sqcs,f.gzs,a.Points,g.sxed from tb_b_user a left join 
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

                string str = @"select a.UserID,a.UserName,a.Addtime,b.gmyfq,c.ysyyfq,d.gmcs,e.gzs,f.syyfq,g.gqwsy from tb_b_user a left join 
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
                                where b.status=0 and b.SaleRecordLX=0 and b.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9' and a.PointsEndTime>=getDate() and a.status=0
                                group by a.UserID) f on a.UserID=f.UserID
                                left join(
                                select sum(a.points) as gqwsy,a.UserID from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                                where b.status=0 and b.SaleRecordLX=0 and b.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9' and a.PointsEndTime<getDate() and a.status=0
                                group by a.UserID) g on a.UserID=g.UserID
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

                string str = @"select a.UserID,a.UserName,a.Addtime,b.gmyfq,c.ysyyfq,d.gmcs,e.gzs,f.syyfq,g.gqwsy from tb_b_user a left join 
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
                                where b.status=0 and b.SaleRecordLX=0 and b.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9' and a.PointsEndTime>=getDate() and a.status=0
                                group by a.UserID) f on a.UserID=f.UserID
                                left join(
                                select sum(a.points) as gqwsy,a.UserID from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                                where b.status=0 and b.SaleRecordLX=0 and b.SaleRecordBelongID='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9' and a.PointsEndTime<getDate() and a.status=0
                                group by a.UserID) g on a.UserID=g.UserID
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
    #endregion

    #region 自放券总表
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

                string str = @"select '"+sjstr+@"' as rq,  a.UserID,a.FromRoute,a.UserName,a.UserXM,b.sjdzq,c.ysyq,d.gqwsy,e.qxnwsy,f.sxq,g.zsq from tb_b_user a left join 
                            (select sum(SaleRecordPoints) as sjdzq,SaleRecordUserID from tb_b_salerecord where SaleRecordLX!=0 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
                            and status=0 " + sjwhere + @"  group by SaleRecordUserID) b on a.UserID=b.SaleRecordUserID
                            left join
                            (select sum(Points) as ysyq,CardUserID from tb_b_pay where ReceiveUserID=CardUserID and  mycardId in (select a.mycardId from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                            where b.status=0 " + sjwhere1 + @" and b.SaleRecordLX!=0 and b.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1))
                             group by CardUserID) c on a.UserID=c.CardUserID
                             left join
                             (select sum(points) as gqwsy,CardUserID from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                            where b.status=0 " + sjwhere1 + @" and b.SaleRecordLX!=0 and b.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
                            and a.PointsEndTime<getDate() and a.status=0 group by CardUserID) d on a.UserID=d.CardUserID
                            left join
                            (select sum(points) as qxnwsy,CardUserID from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                            where b.status=0 " + sjwhere1 + @" and b.SaleRecordLX!=0 and b.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1) and a.status=0
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
                            left join(
							select sum(Points) as zsq, UserID from  tb_b_plattosale where [SaleRecordID] in
                            (select SaleRecordID from  tb_b_salerecord where status=0 " + sjwhere + @"  and SaleRecordLX!=0 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1) 
                            and DATEDIFF(HOUR,SaleRecordTime,getDate())<=validHour) group by UserID
							) g on a.UserID=g.UserID
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

                string sql = @"select max(SaleRecordTime) as maxtime,min(SaleRecordTime) as mintime from tb_b_salerecord where SaleRecordLX!=0 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
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
                            (select sum(SaleRecordPoints) as sjdzq,SaleRecordUserID from tb_b_salerecord where SaleRecordLX!=0 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
                            and status=0 " + sjwhere + @"  group by SaleRecordUserID) b on a.UserID=b.SaleRecordUserID
                            left join
                            (select sum(Points) as ysyq,CardUserID from tb_b_pay where ReceiveUserID=CardUserID and  mycardId in (select a.mycardId from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                            where b.status=0 " + sjwhere1 + @" and b.SaleRecordLX!=0 and b.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1))
                             group by CardUserID) c on a.UserID=c.CardUserID
                             left join
                             (select sum(points) as gqwsy,CardUserID from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                            where b.status=0 " + sjwhere1 + @" and b.SaleRecordLX!=0 and b.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
                            and a.PointsEndTime<getDate() and a.status=0 group by CardUserID) d on a.UserID=d.CardUserID
                            left join
                            (select sum(points) as qxnwsy,CardUserID from tb_b_mycard a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID 
                            where b.status=0 " + sjwhere1 + @" and b.SaleRecordLX!=0 and b.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1) and a.status=0
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
                            left join(
							select sum(Points) as zsq, UserID from  tb_b_plattosale where [SaleRecordID] in
                            (select SaleRecordID from  tb_b_salerecord where status=0 " + sjwhere + @"  and SaleRecordLX!=0 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1) 
                            and DATEDIFF(HOUR,SaleRecordTime,getDate())<=validHour) group by UserID
							) g on a.UserID=g.UserID
                            where a.IsCanRelease=1 and a.ClientKind=1  ";
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
    public object getXSZBList(int pagnum, int pagesize,string userId, string beg, string end)
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
		                    left join tb_b_salerecord d on c.SaleRecordID=d.SaleRecordID  where d.status=0  and d.SaleRecordLX!=0 
		                    and d.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1) 
                            and c.status=0 " + sjwhere + @" and c.ZhiFuZT=1 and c.SaleUserID=@UserID  group by CONVERT(varchar(100), c.AddTime, 23),c.SaleUserID) a 
                            left join tb_b_user b on a.SaleUserID=b.UserID
                            left join 
                            (select sum(c.Money) as xfje,CONVERT(varchar(100), c.AddTime, 23) as rq from tb_b_pay a left join tb_b_mycard b on a.mycardId=b.mycardId
		                    left join tb_b_order c on b.OrderCode=c.OrderCode
		                    left join tb_b_salerecord d on b.SaleRecordID=d.SaleRecordID 
		                    where a.ReceiveUserID=@UserID and c.status=0  and c.ZhiFuZT=1 " + sjwhere + @" and 
		                    d.status=0 and d.SaleRecordLX!=0 and d.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
		                    group by CONVERT(varchar(100), c.AddTime, 23)) c on a.rq=c.rq
		                    left join 
		                    (select sum(c.Money) as gqje,CONVERT(varchar(100), c.AddTime, 23) as rq from tb_b_mycard b left join tb_b_order c on b.OrderCode=c.OrderCode
		                    left join tb_b_salerecord d on b.SaleRecordID=d.SaleRecordID 
		                    where c.SaleUserID=@UserID and c.status=0  and c.ZhiFuZT=1 " + sjwhere + @" and b.PointsEndTime<getDate()
		                    and d.status=0 and d.SaleRecordLX!=0 and d.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
		                    group by CONVERT(varchar(100), c.AddTime, 23)) d on a.rq=d.rq
		                    left join
		                    (select sum(c.Money) as wsyje,CONVERT(varchar(100), c.AddTime, 23) as rq from tb_b_mycard b left join tb_b_order c on b.OrderCode=c.OrderCode
		                    left join tb_b_salerecord d on b.SaleRecordID=d.SaleRecordID 
		                    where c.SaleUserID=@UserID and c.status=0  and c.ZhiFuZT=1 " + sjwhere + @" and b.PointsEndTime>=getDate() and b.status=0
		                    and d.status=0 and d.SaleRecordLX!=0 and d.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
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
		                    left join tb_b_salerecord d on c.SaleRecordID=d.SaleRecordID  where d.status=0  and d.SaleRecordLX!=0 
		                    and d.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1) 
                            and c.status=0 " + sjwhere + @" and c.ZhiFuZT=1 and c.SaleUserID=@UserID  group by CONVERT(varchar(100), c.AddTime, 23),c.SaleUserID) a 
                            left join tb_b_user b on a.SaleUserID=b.UserID
                            left join 
                            (select sum(c.Money) as xfje,CONVERT(varchar(100), c.AddTime, 23) as rq from tb_b_pay a left join tb_b_mycard b on a.mycardId=b.mycardId
		                    left join tb_b_order c on b.OrderCode=c.OrderCode
		                    left join tb_b_salerecord d on b.SaleRecordID=d.SaleRecordID 
		                    where a.ReceiveUserID=@UserID and c.status=0  and c.ZhiFuZT=1 " + sjwhere + @" and 
		                    d.status=0 and d.SaleRecordLX!=0 and d.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
		                    group by CONVERT(varchar(100), c.AddTime, 23)) c on a.rq=c.rq
		                    left join 
		                    (select sum(c.Money) as gqje,CONVERT(varchar(100), c.AddTime, 23) as rq from tb_b_mycard b left join tb_b_order c on b.OrderCode=c.OrderCode
		                    left join tb_b_salerecord d on b.SaleRecordID=d.SaleRecordID 
		                    where c.SaleUserID=@UserID and c.status=0  and c.ZhiFuZT=1 " + sjwhere + @" and b.PointsEndTime<getDate()
		                    and d.status=0 and d.SaleRecordLX!=0 and d.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
		                    group by CONVERT(varchar(100), c.AddTime, 23)) d on a.rq=d.rq
		                    left join
		                    (select sum(c.Money) as wsyje,CONVERT(varchar(100), c.AddTime, 23) as rq from tb_b_mycard b left join tb_b_order c on b.OrderCode=c.OrderCode
		                    left join tb_b_salerecord d on b.SaleRecordID=d.SaleRecordID 
		                    where c.SaleUserID=@UserID and c.status=0  and c.ZhiFuZT=1 " + sjwhere + @" and b.PointsEndTime>=getDate() and b.status=0
		                    and d.status=0 and d.SaleRecordLX!=0 and d.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
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

    [CSMethod("getXSMXList")]
    public object getXSMXList(string userId, string rq)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string str = @"  select a.AddTime,a.Points, case when c.Points>0 then a.Money end as xfje,
		                        case when g.gqje>0 then a.Money end as gqje,case when f.wsyje>0 then a.Money end as wsyje,
                                d.SaleRecordDiscount,e.UserName,c.AddTime as xfrq from tb_b_order a 
		                        left join tb_b_pay c on a.OrderCode=c.OrderCode
		                        left join tb_b_salerecord d on a.SaleRecordID=d.SaleRecordID 
		                        left join tb_b_user e on a.BuyUserID=e.UserID
		                        left join (select b.OrderCode,b.points as wsyje from tb_b_mycard b where status=0 and b.PointsEndTime>=getDate()) f on a.OrderCode=f.OrderCode
		                        left join (select b.OrderCode,b.points as gqje from tb_b_mycard b where status=0 and b.PointsEndTime<getDate()) g on a.OrderCode=g.OrderCode
		                         where a.SaleUserID=@UserID and a.AddTime>='" + Convert.ToDateTime(rq).ToString("yyyy-MM-dd") + @"' and a.AddTime<'" + Convert.ToDateTime(rq).AddDays(1).ToString("yyyy-MM-dd") + @"'
		                         and d.status=0 and d.SaleRecordLX!=0 and d.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1) and a.Status=0 and a.ZhiFuZT=1
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


                string str = @"  select a.AddTime,a.Points, case when c.Points>0 then a.Money end as xfje,
		                        case when g.gqje>0 then a.Money end as gqje,case when f.wsyje>0 then a.Money end as wsyje,
                                d.SaleRecordDiscount,e.UserName,c.AddTime as xfrq from tb_b_order a 
		                        left join tb_b_pay c on a.OrderCode=c.OrderCode
		                        left join tb_b_salerecord d on a.SaleRecordID=d.SaleRecordID 
		                        left join tb_b_user e on a.BuyUserID=e.UserID
		                        left join (select b.OrderCode,b.points as wsyje from tb_b_mycard b where status=0 and b.PointsEndTime>=getDate()) f on a.OrderCode=f.OrderCode
		                        left join (select b.OrderCode,b.points as gqje from tb_b_mycard b where status=0 and b.PointsEndTime<getDate()) g on a.OrderCode=g.OrderCode
		                         where a.SaleUserID=@UserID and a.AddTime>='" + Convert.ToDateTime(rq).ToString("yyyy-MM-dd") + @"' and a.AddTime<'" + Convert.ToDateTime(rq).AddDays(1).ToString("yyyy-MM-dd") + @"'
		                         and d.status=0 and d.SaleRecordLX!=0 and d.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1) and a.Status=0 and a.ZhiFuZT=1
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

                string sql = @"select max(SaleRecordTime) as maxtime,min(SaleRecordTime) as mintime from tb_b_salerecord where SaleRecordLX!=0 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
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
                            and a.PointsEndTime<getDate() and a.status=0 group by CardUserID) d on a.UserID=d.CardUserID
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
                            left join (select sum(Points) as xj,UserID from tb_b_xj where status=0 group by UserID) i on a.UserID=i.UserID
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

                string sql = @"select max(SaleRecordTime) as maxtime,min(SaleRecordTime) as mintime from tb_b_salerecord where SaleRecordLX!=0 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
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
                            and a.PointsEndTime<getDate() and a.status=0 group by CardUserID) d on a.UserID=d.CardUserID
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
                            left join (select sum(Points) as xj,UserID from tb_b_xj where status=0 group by UserID) i on a.UserID=i.UserID
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
		                    where c.SaleUserID=@UserID and c.status=0  and c.ZhiFuZT=1 " + sjwhere + @" and b.PointsEndTime<getDate()
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
		                    where c.SaleUserID=@UserID and c.status=0  and c.ZhiFuZT=1 " + sjwhere + @" and b.PointsEndTime<getDate()
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
		                        left join (select b.OrderCode,b.points as gqje from tb_b_mycard b where status=0 and b.PointsEndTime<getDate()) g on a.OrderCode=g.OrderCode
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
		                        left join (select b.OrderCode,b.points as gqje from tb_b_mycard b where status=0 and b.PointsEndTime<getDate()) g on a.OrderCode=g.OrderCode
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

                string sql = @"select max(SaleRecordTime) as maxtime,min(SaleRecordTime) as mintime from tb_b_salerecord where SaleRecordLX!=0 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
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

                string sql = @"select max(SaleRecordTime) as maxtime,min(SaleRecordTime) as mintime from tb_b_salerecord where SaleRecordLX!=0 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
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
                string str = @" select * from tb_b_xj where status=0 and UserID=@UserID";
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

                string str = @" select * from tb_b_xj where status=0 and UserID=@UserID";
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
                                where b.ClientKind=2 and d.status=0 and c.status=1 and d.ZhiFuZT=1  "+where+@"
                                union all
                                select b.UserName,d.AddTime as jysj,null as xfsj,e.UserXM,a.OrderCode,d.Money,'购买' as flag from tb_b_mycard a  left join tb_b_user b on a.UserID=b.UserID 
                                left join tb_b_order d on a.OrderCode=d.OrderCode
                                left join tb_b_user e on a.CardUserID=e.UserID
                                 where b.ClientKind=2 and d.status=0 and a.status=0  and d.ZhiFuZT=1 and a.PointsEndTime>=getDate()  " + where + @"
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

    [CSMethod("GetSFJYListToFile", 2)]
    public byte[] GetSFJYListToFile(string yhm, string xm, string beg, string end)
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
                cells[0, 6].PutValue("交易类型");
                cells[0, 6].SetStyle(style2);
                cells.SetColumnWidth(6, 20);


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
                                where b.ClientKind=2 and d.status=0 and c.status=1 and d.ZhiFuZT=1  " + where + @"
                                union all
                                select b.UserName,d.AddTime as jysj,null as xfsj,e.UserXM,a.OrderCode,d.Money,'购买' as flag from tb_b_mycard a  left join tb_b_user b on a.UserID=b.UserID 
                                left join tb_b_order d on a.OrderCode=d.OrderCode
                                left join tb_b_user e on a.CardUserID=e.UserID
                                 where b.ClientKind=2 and d.status=0 and a.status=0  and d.ZhiFuZT=1 and a.PointsEndTime>=getDate()  " + where + @"
                                 union all
                                 select b.UserName,d.AddTime as jysj,null as xfsj,e.UserXM,a.OrderCode,d.Money,'过期' as flag from tb_b_mycard a  left join tb_b_user b on a.UserID=b.UserID 
                                left join tb_b_order d on a.OrderCode=d.OrderCode
                                left join tb_b_user e on a.CardUserID=e.UserID
                                 where b.ClientKind=2 and d.status=0 and d.ZhiFuZT=1 and a.status=0 and a.PointsEndTime<getDate() " + where + @"";

                //开始取分页数据
                DataTable dt = dbc.ExecuteDataTable(str + " order by d.AddTime desc,b.UserName,e.UserXM");

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
                    if (dt.Rows[i]["flag"] != null && dt.Rows[i]["flag"].ToString() != "")
                    {
                        cells[i + 1, 6].PutValue(dt.Rows[i]["flag"]);
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
							    (select sum(Points) as xfyfq,CardUserID from tb_b_pay where mycardId is null and ReceiveUserID=CardUserID group by CardUserID) b on a.UserID=b.CardUserID
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
							    (select sum(Points) as xfyfq,CardUserID from tb_b_pay where mycardId is null and ReceiveUserID=CardUserID group by CardUserID) b on a.UserID=b.CardUserID
                                left join 
                                (select sum(Points) as wsyyfq,CardUserID from tb_b_mycard where SaleRecordID is null and status=0 group by CardUserID) c on a.UserID=c.CardUserID
                                where 1=1 and a.ClientKind=1";
                str += where;

                //开始取分页数据
                DataTable dt= dbc.ExecuteDataTable(str + " order by a.AddTime desc,a.UserName,a.UserXM");

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
                                where a.mycardId is null and a.ReceiveUserID=a.CardUserID and a.CardUserID=@UserID  order by a.AddTime desc
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
                                where a.mycardId is null and a.ReceiveUserID=a.CardUserID and a.CardUserID=@UserID  order by a.AddTime desc
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
}
