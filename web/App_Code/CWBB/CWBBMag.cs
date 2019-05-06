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

                string sql = @"select max(SaleRecordTime) as maxtime,min(SaleRecordTime) as mintime from tb_b_salerecord where SaleRecordLX!=0 and SaleRecordVerifyType=1 and SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
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

                string str = @"select '"+sjstr+ @"' as rq,  a.UserID,a.FromRoute,a.UserName,a.UserXM,b.sjdzq,c.ysyq,d.gqwsy,e.qxnwsy,f.sxq,g.zsq from tb_b_user a left join 
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
    public object getXSZBList(int pagnum, int pagesize,string userId, string beg, string end)
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

                string str = @"select a.rq,b.FromRoute,b.UserName,b.UserXM,a.xsdzq,c.xfje,d.gqje,e.wsyje,a.zje,a.pjzk,a.yj from
		                    (select sum(c.Points) as xsdzq,sum(c.Money) as zje,sum(c.CHBMoney) as yj, CONVERT(varchar(100), c.AddTime, 23) as rq,c.SaleUserID,
		                    AVG(d.SaleRecordDiscount) as pjzk from tb_b_order c 
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
		                    and d.status=0 and d.SaleRecordLX!=0 and and d.SaleRecordVerifyType=1 d.SaleRecordBelongID in (select UserID from tb_b_user where ClientKind=1)
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


                string str = @"  select a.AddTime,a.Points, case when c.Points>0 then a.Money end as xfje,
		                        case when g.gqje>0 then a.Money end as gqje,case when f.wsyje>0 then a.Money end as wsyje,
                                d.SaleRecordDiscount,e.UserName,c.AddTime as xfrq from tb_b_order a 
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
    public object GetSFJYList(int pagnum, int pagesize, string yhm, string xm,string beg, string end,string ordercode)
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
                if (!string.IsNullOrEmpty(ordercode))
                {
                    where1 += " and " + dbc.C_Like("c.OrderCode", ordercode.Trim(), LikeStyle.LeftAndRightLike);
                    where += " and " + dbc.C_Like("d.OrderCode", ordercode.Trim(), LikeStyle.LeftAndRightLike);
                }

                string str = @"select b.UserName,d.AddTime as jysj,a.AddTime as xfsj,e.UserXM,c.OrderCode,d.Money,'消费' as flag,f.redenvelopeid,f.money as redmoney,
                                case when g.SaleRecordLX=0 then '耗材券' when  g.SaleRecordLX is null then '耗材券'
                                when  g.SaleRecordLX<>0 then  '自发券' end as KIND
                                from tb_b_pay a left join tb_b_user b on a.PayUserID=b.UserID 
                                left join tb_b_mycard c on a.mycardId=c.mycardId 
                                left join tb_b_order d on c.OrderCode=d.OrderCode
                                left join tb_b_redenvelope f on d.redenvelopeid=f.redenvelopeid
                                left join tb_b_user e on a.CardUserID=e.UserID
                                left join tb_b_salerecord g on d.SaleRecordID=g.SaleRecordID
                                where b.ClientKind=2 and d.status=0 and c.status=1 and d.ZhiFuZT=1  " + where + where1 + @"
                                union all
                                select b.UserName,d.AddTime as jysj,null as xfsj,e.UserXM,a.OrderCode,d.Money,'购买' as flag,f.redenvelopeid,f.money as redmoney,
                                case when g.SaleRecordLX=0 then '耗材券' when  g.SaleRecordLX is null then '耗材券'
                                when  g.SaleRecordLX<>0 then  '自发券' end as KIND
                                from tb_b_mycard a  left join tb_b_user b on a.UserID=b.UserID 
                                left join tb_b_order d on a.OrderCode=d.OrderCode
                                left join tb_b_redenvelope f on d.redenvelopeid=f.redenvelopeid
                                left join tb_b_user e on a.CardUserID=e.UserID
                                left join tb_b_salerecord g on d.SaleRecordID=g.SaleRecordID
                                 where b.ClientKind=2 and d.status=0 and a.status=0  and d.ZhiFuZT=1 and a.PointsEndTime>=getDate()  " + where + @"
                                 union all
                                 select b.UserName,d.AddTime as jysj,null as xfsj,e.UserXM,a.OrderCode,d.Money,'过期' as flag,f.redenvelopeid,f.money as redmoney,
                                case when g.SaleRecordLX=0 then '耗材券' when  g.SaleRecordLX is null then '耗材券'
                                when  g.SaleRecordLX<>0 then  '自发券' end as KIND 
                                from tb_b_mycard a  left join tb_b_user b on a.UserID=b.UserID 
                                left join tb_b_order d on a.OrderCode=d.OrderCode
                                left join tb_b_redenvelope f on d.redenvelopeid=f.redenvelopeid
                                left join tb_b_user e on a.CardUserID=e.UserID
                                left join tb_b_salerecord g on d.SaleRecordID=g.SaleRecordID
                                 where b.ClientKind=2 and d.status=0 and d.ZhiFuZT=1 and a.status=9 and a.PointsEndTime<getDate() " + where + @"";

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
    public byte[] GetSFJYListToFile(string yhm, string xm, string beg, string end, string ordercode)
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
                if (!string.IsNullOrEmpty(ordercode))
                {
                    where1 += " and " + dbc.C_Like("c.OrderCode", ordercode.Trim(), LikeStyle.LeftAndRightLike);
                    where += " and " + dbc.C_Like("d.OrderCode", ordercode.Trim(), LikeStyle.LeftAndRightLike);
                }

                string str = @"select b.UserName,d.AddTime as jysj,a.AddTime as xfsj,e.UserXM,c.OrderCode,d.Money,'消费' as flag,f.redenvelopeid,f.money as redmoney,
                                case when g.SaleRecordLX=0 then '耗材券' when  g.SaleRecordLX is null then '耗材券'
                                when  g.SaleRecordLX<>0 then  '自发券' end as KIND
                                from tb_b_pay a left join tb_b_user b on a.PayUserID=b.UserID 
                                left join tb_b_mycard c on a.mycardId=c.mycardId 
                                left join tb_b_order d on c.OrderCode=d.OrderCode
                                left join tb_b_redenvelope f on d.redenvelopeid=f.redenvelopeid
                                left join tb_b_user e on a.CardUserID=e.UserID
                                left join tb_b_salerecord g on d.SaleRecordID=g.SaleRecordID
                                where b.ClientKind=2 and d.status=0 and c.status=1 and d.ZhiFuZT=1  " + where+where1 + @"
                                union all
                                select b.UserName,d.AddTime as jysj,null as xfsj,e.UserXM,a.OrderCode,d.Money,'购买' as flag,f.redenvelopeid,f.money as redmoney,
                                case when g.SaleRecordLX=0 then '耗材券' when  g.SaleRecordLX is null then '耗材券'
                                when  g.SaleRecordLX<>0 then  '自发券' end as KIND
                                from tb_b_mycard a  left join tb_b_user b on a.UserID=b.UserID 
                                left join tb_b_order d on a.OrderCode=d.OrderCode
                                left join tb_b_redenvelope f on d.redenvelopeid=f.redenvelopeid
                                left join tb_b_user e on a.CardUserID=e.UserID
                                left join tb_b_salerecord g on d.SaleRecordID=g.SaleRecordID
                                 where b.ClientKind=2 and d.status=0 and a.status=0  and d.ZhiFuZT=1 and a.PointsEndTime>=getDate()  " + where + @"
                                 union all
                                 select b.UserName,d.AddTime as jysj,null as xfsj,e.UserXM,a.OrderCode,d.Money,'过期' as flag,f.redenvelopeid,f.money as redmoney,
                                case when g.SaleRecordLX=0 then '耗材券' when  g.SaleRecordLX is null then '耗材券'
                                when  g.SaleRecordLX<>0 then  '自发券' end as KIND 
                                from tb_b_mycard a  left join tb_b_user b on a.UserID=b.UserID 
                                left join tb_b_order d on a.OrderCode=d.OrderCode
                                left join tb_b_redenvelope f on d.redenvelopeid=f.redenvelopeid
                                left join tb_b_user e on a.CardUserID=e.UserID
                                left join tb_b_salerecord g on d.SaleRecordID=g.SaleRecordID
                                 where b.ClientKind=2 and d.status=0 and d.ZhiFuZT=1 and a.status=9 and a.PointsEndTime<getDate() " + where + @"";

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
                    if (dt.Rows[i]["redenvelopeid"] != null && dt.Rows[i]["redenvelopeid"].ToString() != "")
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
    [CSMethod("GetPSList")]
    public object GetPSList(int pagnum, int pagesize, string xm, string beg, string end)
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

    [CSMethod("GetPSListToFile", 2)]
    public byte[] GetPSListToFile(string xm, string beg, string end)
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
                                    where a.status=0 ";
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

    [CSMethod("GetPSSF")]
    public object GetPSSF(int pagnum, int pagesize, string id)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;
                string str = @"select a.*,b.UserName from tb_b_paisong_detail a left join  
                              tb_b_user b on a.sanfanguserid=b.UserID
                              where paisongid=@id and status=0
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


                string str = @"select a.*,b.UserName from tb_b_paisong_detail a left join  
                              tb_b_user b on a.sanfanguserid=b.UserID
                              where paisongid=@id and status=0
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
                    where += " and (a.AddTime>='"+Convert.ToDateTime(sj).ToString("yyyy-MM-dd")+"' and a.AddTime<'"+Convert.ToDateTime(sj).AddDays(1).ToString("yyyy-MM-dd")+"')";
                }

                if(!string.IsNullOrEmpty(lx)){
                    if(Convert.ToInt32(lx)==1){
                        where+="and (b.SaleRecordLX=0 or b.SaleRecordLX is null)";
                    }
                    else if (Convert.ToInt32(lx) == 2)
                    {
                        where+="and b.SaleRecordLX<>0";
                    }
                }

                string str = @" select sum(a.Points) as points,c.UserXM,a.SaleUserID from tb_b_order a left join tb_b_salerecord b on a.SaleRecordID=b.SaleRecordID
                            left join tb_b_user c on a.SaleUserID=c.UserID
                             where a.status=0 and a.ZhiFuZT=1 and b.Status=0   "+where+@" 
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
            try {
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
    public object getJXL(){
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

                int count1=0;
                int count2=0;
                DataRow[] drs1 = dt.Select("gmcs=2");
                count1 = drs1.Length;

                DataRow[] drs2 = dt.Select("gmcs>2");
                count2 = drs2.Length;

                return new {count1=count1,count2=count2};
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
}
