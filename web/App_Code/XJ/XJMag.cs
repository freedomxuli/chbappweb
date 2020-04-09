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
using System.Drawing;

/// <summary>
/// XJMag 的摘要说明
/// </summary>
[CSClass("XJMag")]
public class XJMag
{
    public XJMag()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    [CSMethod("GetList")]
    public object GetList(int pagnum, int pagesize, string yhm)
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

                string str = @"  select a.*,b.UserName,b.UserXM,case when a.points>0 then a.validHour-(DATEDIFF(HOUR,a.SaleRecordTime,getDate())) else null end as xssy
                    from tb_b_plattosale a left join tb_b_user b on a.UserID=b.UserID
                        where a.status=0 and a.pointkind=0  ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by case when a.points>0 then a.validHour-(DATEDIFF(HOUR,a.SaleRecordTime,getDate())) else 999 end asc,a.points desc", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("SaveXJ")]
    public bool SaveXJ(JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                dbc.BeginTransaction();

                var PlatToSaleId = jsr["PlatToSaleId"].ToString();
                var points = jsr["points"].ToString();
                var memo = jsr["memo"].ToString();

                string sql = "select points,UserID from tb_b_plattosale where PlatToSaleId = @PlatToSaleId";
                SqlCommand cmd = dbc.CreateCommand(sql);
                cmd.Parameters.Add("@PlatToSaleId", PlatToSaleId);
                DataTable dt = dbc.ExecuteDataTable(cmd);

                if (Convert.ToDecimal(dt.Rows[0]["points"]) != Convert.ToDecimal(points))
                {
                    throw new Exception("需下架所有券！");
                }
                else
                {
                    sql = "update tb_b_plattosale set points = @points where PlatToSaleId = @PlatToSaleId";
                    cmd = dbc.CreateCommand(sql);
                    cmd.Parameters.Add("@points", Convert.ToDecimal(Convert.ToDecimal(dt.Rows[0]["points"]) - Convert.ToDecimal(points)));
                    cmd.Parameters.Add("@PlatToSaleId", PlatToSaleId);
                    dbc.ExecuteNonQuery(cmd);

                    sql = "select Points,PlatPointId from tb_b_platpoints where UserID = @UserID and status = 0";
                    cmd = dbc.CreateCommand(sql);
                    cmd.Parameters.Add("@UserID", dt.Rows[0]["UserID"].ToString());
                    DataTable dt_plat = dbc.ExecuteDataTable(cmd);

                    sql = "update tb_b_platpoints set points = @points where PlatPointId = @PlatPointId";
                    cmd = dbc.CreateCommand(sql);
                    cmd.Parameters.Add("@points", Convert.ToDecimal(Convert.ToDecimal(dt_plat.Rows[0]["Points"]) + Convert.ToDecimal(points)));
                    cmd.Parameters.Add("@PlatPointId", dt_plat.Rows[0]["PlatPointId"].ToString());
                    dbc.ExecuteNonQuery(cmd);

                    DataTable dt_xj = dbc.GetEmptyDataTable("tb_b_xj");
                    DataRow dr = dt_xj.NewRow();
                    dr["XJ_ID"] = Guid.NewGuid();
                    dr["UserID"] = dt.Rows[0]["UserID"].ToString();
                    dr["Points"] = points;
                    dr["status"] = 0;
                    dr["adduser"] = SystemUser.CurrentUser.UserID;
                    dr["addtime"] = DateTime.Now;
                    dr["updateuser"] = SystemUser.CurrentUser.UserID;
                    dr["updatetime"] = DateTime.Now;
                    dr["Memo"] = memo;
                    dt_xj.Rows.Add(dr);
                    dbc.InsertTable(dt_xj);

                    dbc.CommitTransaction();
                }

                return true;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }

    [CSMethod("getHisSale")]
    public DataTable getHisSale(string UserID)
    {
        using (DBConnection dbc = new DBConnection())
        {
            string sql = @"select isNULL(a.points,0)+isnull(b.points,0) as points,a.UserID,a.PlatToSaleId,a.discountmemo, c.UserXM from 
(select Points as points,UserID,PlatToSaleId,discountmemo from tb_b_plattosale where  UserID=@UserID and status=0 and pointkind=0 ) a left join (
select sum(Points) as points,SaleUserID from [tb_b_order] where  [SaleUserID]=@UserID and ZhiFuZT=0 and status=0
group by SaleUserID) b  on a.UserID=b.SaleUserID 
 left join tb_b_user c on a.UserID = c.UserID";
            SqlCommand cmd = dbc.CreateCommand(sql);
            cmd.Parameters.Add("@UserID", UserID);
            DataTable dt = dbc.ExecuteDataTable(cmd);
            return dt;
        }
    }


    #region 运费券清理
    [CSMethod("GetQLList")]
    public object GetQLList(int pagnum, int pagesize, string yhm)
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

                string str = @" select a.*,b.UserXM from tb_b_plattosale a left join tb_b_user b on a.userid=b.userid
                                where a.status=0 and a.pointkind!=0 and a.points>0 and a.points<500 
                                and a.validHour-(DATEDIFF(HOUR,a.addtime,getDate()))>=0
                                and a.SaleRecordID in (select distinct a.SaleRecordID from (select SaleRecordPoints,SaleRecordID from tb_b_salerecord where status=0) a 
                                left join (select sum(points) as points,SaleRecordID from tb_b_order where status=0 and zhifuZT=1 group by SaleRecordID) b  on a.SaleRecordID=b.SaleRecordID
                                left join (select points,SaleRecordID from tb_b_plattosale where status=0 ) c on a.SaleRecordID=c.SaleRecordID
                                where isnull(a.SaleRecordPoints,0)-(isnull(b.points,0)+isnull(c.points,0))=0)";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.addtime ", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("QLYFQ")]
    public object QLYFQ(string id)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                dbc.BeginTransaction();
                string str = "select * from tb_b_plattosale where status=0 and PlatToSaleId=@PlatToSaleId";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.AddWithValue("@PlatToSaleId", id);
                DataTable dt = dbc.ExecuteDataTable(cmd);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["points"] != null && dt.Rows[0]["points"].ToString() != "")
                    {
                        if (Convert.ToDecimal(dt.Rows[0]["points"]) == 0 || Convert.ToDecimal(dt.Rows[0]["points"]) >= 500)
                        {
                            throw new Exception("该数据不需要清理！");
                        }
                        else
                        {
                            cmd.Parameters.Clear();
                            str = "select * from tb_b_salerecord where SaleRecordID=@SaleRecordID";
                            cmd = dbc.CreateCommand(str);
                            cmd.Parameters.AddWithValue("@salerecordid", dt.Rows[0]["SaleRecordID"]);
                            DataTable dt1 = dbc.ExecuteDataTable(str);

                            cmd.Parameters.Clear();
                            string sql = "update tb_b_salerecord set SaleRecordPoints=@SaleRecordPoints where SaleRecordID=@SaleRecordID";
                            cmd = dbc.CreateCommand(sql);
                            cmd.Parameters.AddWithValue("@SaleRecordPoints", Convert.ToDecimal(Convert.ToDecimal(dt1.Rows[0]["SaleRecordPoints"]) - Convert.ToDecimal(dt.Rows[0]["points"])));
                            cmd.Parameters.AddWithValue("@SaleRecordID", dt.Rows[0]["SaleRecordID"]);
                            dbc.ExecuteNonQuery(cmd);

                            cmd.Parameters.Clear();
                            sql = "update tb_b_plattosale set points = 0 where PlatToSaleId = @PlatToSaleId";
                            cmd = dbc.CreateCommand(sql);
                            cmd.Parameters.AddWithValue("@PlatToSaleId", id);
                            dbc.ExecuteNonQuery(cmd);
                        }
                    }
                }
                else
                {
                    throw new Exception("该数据不存在！");
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

    #endregion

    #region 过期券回退
    [CSMethod("GetGqqListByPage")]
    public object GetGqqListByPage(int pagnum, int pagesize, JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;
                List<string> wArr = new List<string>();
                if (!string.IsNullOrEmpty(jsr["cx_zxmc"]))
                {
                    wArr.Add(dbc.C_Like("c.UserXM", jsr["cx_zxmc"], LikeStyle.LeftAndRightLike));
                }
                if (!string.IsNullOrEmpty(jsr["cx_fhrzh"]))
                {
                    wArr.Add(dbc.C_Like("b.UserName", jsr["cx_fhrzh"], LikeStyle.LeftAndRightLike));
                }
                string cx_beg = jsr["cx_beg"].ToString();
                if (!string.IsNullOrEmpty(cx_beg))
                {
                    wArr.Add("a.addTimd >= " + dbc.ToSqlValue(Convert.ToDateTime(cx_beg)));
                }

                string cx_endjsr = jsr["cx_end"].ToString();
                if (!string.IsNullOrEmpty(cx_endjsr))
                {
                    wArr.Add("a.addTimd < " + dbc.ToSqlValue(Convert.ToDateTime(cx_endjsr).AddDays(1)));
                }
                string sqlW = "";
                if (wArr.Count > 0)
                {
                    sqlW = " and " + string.Join(" and ", wArr);
                }
                string str = @"select a.*,b.UserName fhrzh,c.UserXM zxmc,
                                case 
                                when d.id IS NULL then 0
                                else 1 end as shzt from(
	                                select t1.*,t2.UserID,t2.CardUserID from tb_b_invalid_points t1
	                                left join tb_b_mycard t2 on t1.mycardId=t2.mycardId
                                )a
                                left join tb_b_user b on a.UserID=b.UserID
                                left join tb_b_user c on a.CardUserID=c.UserID
                                left join tb_b_return_points d on a.invalidId=d.invalidId and d.status=0

                            where a.status=0 and a.addTimd>'2019/6/24'" + sqlW + " order by a.addTimd desc";

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

    [CSMethod("GqqSh")]
    public bool GqqSh(string invalidId, string zxid, decimal points)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                dbc.BeginTransaction();
                //验证过期券是否回退过
                string sql = @"select * from tb_b_return_points where invalidId=" + dbc.ToSqlValue(invalidId);
                DataTable checkDt = dbc.ExecuteDataTable(sql);
                if (checkDt.Rows.Count > 0)
                {
                    return false;
                }
                //插入过期券回退记录
                DataTable returnDt = dbc.GetEmptyDataTable("tb_b_return_points");
                DataRow dr = returnDt.NewRow();
                dr["id"] = Guid.NewGuid().ToString();
                dr["invalidId"] = invalidId;
                dr["addtime"] = DateTime.Now;
                dr["adduser"] = SystemUser.CurrentUser.UserID;
                dr["updatetime"] = DateTime.Now;
                dr["updateuser"] = SystemUser.CurrentUser.UserID;
                dr["status"] = 0;
                returnDt.Rows.Add(dr);
                dbc.InsertTable(returnDt);
                //专线Points回退
                sql = @"update tb_b_platpoints set Points=Points+" + points + " where UserID=" + dbc.ToSqlValue(zxid);
                dbc.ExecuteNonQuery(sql);
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
    #endregion

    #region 退款审核界面
    [CSMethod("GetTkshList")]
    public object GetTkshList(int pagnum, int pagesize, JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                List<string> wArr = new List<string>();
                string cx_beg = jsr["cx_beg"].ToString();
                if (!string.IsNullOrEmpty(cx_beg))
                {
                    wArr.Add("a.addtime >= " + dbc.ToSqlValue(Convert.ToDateTime(cx_beg)));
                }
                string cx_endjsr = jsr["cx_end"].ToString();
                if (!string.IsNullOrEmpty(cx_endjsr))
                {
                    wArr.Add("a.addtime < " + dbc.ToSqlValue(Convert.ToDateTime(cx_endjsr).AddDays(1)));
                }
                if (!string.IsNullOrEmpty(jsr["cx_fhrzh"]))
                {
                    wArr.Add(dbc.C_Like("b.UserName", jsr["cx_fhrzh"], LikeStyle.LeftAndRightLike));
                }
                if (!string.IsNullOrEmpty(jsr["cx_zxmc"]))
                {
                    wArr.Add(dbc.C_Like("c.UserXM", jsr["cx_zxmc"], LikeStyle.LeftAndRightLike));
                }
                if (!string.IsNullOrEmpty(jsr["cx_istk"]))
                {
                    wArr.Add(dbc.C_EQ("a.status", Convert.ToInt32(jsr["cx_istk"].ToString())));
                }
                string sqlW = "";
                if (wArr.Count > 0)
                {
                    sqlW = " and " + string.Join(" and ", wArr);
                }

                string str = @"select a.*,b.UserName fhrmc,c.UserXM zxmc
                                ,d.addtime tktime,e.Money tkmoney,
                                case
                                when d.salerecordlx=0 then '耗材券' when d.salerecordlx=4 then '授权券'
                                else '自发布券' end as qlx
                                from tb_b_mycard a 
                                left join tb_b_user b on a.UserID=b.UserID
                                left join tb_b_user c on a.CardUserID=c.UserID
                                left join tb_b_refund d on a.mycardid=d.mycardid
                                left join tb_b_order e on a.OrderCode=e.OrderCode
                                where a.status in(2,3) and a.OrderCode is not null ";
                str += sqlW;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.status asc,a.addtime desc", pagesize, ref cp, out ac);

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
    public byte[] GetTkshListOutList(JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                
                List<string> wArr = new List<string>();
                string cx_beg = jsr["cx_beg"].ToString();
                if (!string.IsNullOrEmpty(cx_beg))
                {
                    wArr.Add("a.addtime >= " + dbc.ToSqlValue(Convert.ToDateTime(cx_beg)));
                }
                string cx_endjsr = jsr["cx_end"].ToString();
                if (!string.IsNullOrEmpty(cx_endjsr))
                {
                    wArr.Add("a.addtime < " + dbc.ToSqlValue(Convert.ToDateTime(cx_endjsr).AddDays(1)));
                }
                if (!string.IsNullOrEmpty(jsr["cx_fhrzh"]))
                {
                    wArr.Add(dbc.C_Like("b.UserName", jsr["cx_fhrzh"], LikeStyle.LeftAndRightLike));
                }
                if (!string.IsNullOrEmpty(jsr["cx_zxmc"]))
                {
                    wArr.Add(dbc.C_Like("c.UserXM", jsr["cx_zxmc"], LikeStyle.LeftAndRightLike));
                }
                if (!string.IsNullOrEmpty(jsr["cx_istk"]))
                {
                    wArr.Add(dbc.C_EQ("a.status", Convert.ToInt32(jsr["cx_istk"].ToString())));
                }
                string sqlW = "";
                if (wArr.Count > 0)
                {
                    sqlW = " and " + string.Join(" and ", wArr);
                }

                string str = @"select a.*,b.UserName fhrmc,c.UserXM zxmc
                                ,d.addtime tktime,e.Money tkmoney,
                                case
                                when d.salerecordlx=0 then '耗材券'
                                else '自发布券' end as qlx
                                from tb_b_mycard a 
                                left join tb_b_user b on a.UserID=b.UserID
                                left join tb_b_user c on a.CardUserID=c.UserID
                                left join tb_b_refund d on a.mycardid=d.mycardid
                                left join tb_b_order e on a.OrderCode=e.OrderCode
                                where a.status in(2,3) and a.OrderCode is not null ";
                str += sqlW;

                //开始取分页数据
               
               System.Data.DataTable dt = dbc.ExecuteDataTable(str + " order by a.status asc,a.addtime desc");

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
                style4.Font.Color = Color.Red;//文字大小

                style4.IsTextWrapped = true;//单元格内容自动换行
                style4.Font.IsBold = false;//粗体




                //第一行标题列表
                //合并单元格cells.Merge(1, 0, 3, 1) 参数1代表当前行，参数0代表当前行当前列即第一行第一列，参数3合并的行数，参数4合并的列数

                cells.Merge(0, 0, 1, 9);

                cells[0, 0].PutValue("退款审核数据导出");
                cells[0, 0].SetStyle(style3);
                cells.SetRowHeight(0, 30);

               


                cells.SetColumnWidth(0, 30);
                cells[2, 0].PutValue("订单号");
                cells[2, 0].SetStyle(style1);
                cells.SetColumnWidth(1, 15);
                cells[2, 1].PutValue("申请退款券额");
                cells[2, 1].SetStyle(style1);
                cells.SetColumnWidth(2, 30);
                cells[2, 2].PutValue("发货人账号");
                cells[2, 2].SetStyle(style1);
                cells.SetColumnWidth(3, 30);
                cells[2, 3].PutValue("专线名称");
                cells[2, 3].SetStyle(style1);
                cells.SetColumnWidth(4, 15);
                cells[2, 4].PutValue("购买时间");
                cells[2, 4].SetStyle(style1);
                cells.SetColumnWidth(5, 20);
                cells[2, 5].PutValue("退款状态");
                cells[2, 5].SetStyle(style1);
                cells.SetColumnWidth(6, 20);
                cells[2, 6].PutValue("退款时间");
                cells[2, 6].SetStyle(style1);
                cells.SetColumnWidth(7, 20);
                cells[2, 7].PutValue("退款金额");
                cells[2, 7].SetStyle(style1);
                cells.SetColumnWidth(8, 20);
                cells[2,8].PutValue("券类型");
                cells[2,8].SetStyle(style1);




                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var status = "";
                        if (dt.Rows[i]["status"] != null && dt.Rows[i]["status"].ToString() != "")
                        {
                            if (dt.Rows[i]["status"].ToString() == "2")
                            {
                                status= "退款审核中";
                            }else if (dt.Rows[i]["status"].ToString() == "3")
                            {
                                status ="已退款";
                            }
                        }
                        

                        cells[i + 3, 0].PutValue(dt.Rows[i]["OrderCode"].ToString());
                        cells[i + 3, 0].SetStyle(style2);
                        cells[i + 3, 1].PutValue(dt.Rows[i]["points"].ToString());
                        cells[i + 3, 1].SetStyle(style2);
                        cells[i + 3, 2].PutValue(dt.Rows[i]["fhrmc"].ToString());
                        cells[i + 3, 2].SetStyle(style2);
                        cells[i + 3, 3].PutValue(dt.Rows[i]["zxmc"].ToString());
                        cells[i + 3, 3].SetStyle(style2);
                        if (dt.Rows[i]["addtime"].ToString() != "")
                        {
                            cells[i + 3, 4].PutValue(Convert.ToDateTime(dt.Rows[i]["addtime"].ToString()).ToString("yyyy-MM-dd"));
                        }
                        else
                        {
                            cells[i + 3, 4].PutValue("");

                        }
                        cells[i + 3, 4].SetStyle(style2);
                        cells[i + 3, 5].PutValue(status);
                        cells[i + 3, 5].SetStyle(style2);
                        if (dt.Rows[i]["tktime"].ToString() != "")
                        {
                            cells[i + 3, 6].PutValue(Convert.ToDateTime(dt.Rows[i]["tktime"].ToString()).ToString("yyyy-MM-dd"));
                        }
                        else {
                            cells[i + 3, 6].PutValue("");

                        }
                        cells[i + 3, 6].SetStyle(style2);
                        cells[i + 3, 7].PutValue(dt.Rows[i]["tkmoney"].ToString());
                        cells[i + 3, 7].SetStyle(style2);
                        cells[i + 3, 8].PutValue(dt.Rows[i]["qlx"].ToString());
                        cells[i + 3, 8].SetStyle(style2);

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

    [CSMethod("VerifyPassWorld")]
    public bool VerifyPassWorld(string id, string password)
    {
        using (DBConnection dbc = new DBConnection())
        {
            string ServiceURL = System.Configuration.ConfigurationManager.AppSettings["ServiceURL"].ToString();

            string sqlStr = @"select a.UserID YH_ID, a.UserName YH_DLM,a.UserXM YH_XM,a.Password,b.roleId,b.companyId from tb_b_user a 
                            left join tb_b_user_role b on a.UserID = b.userId 
                            where a.UserID=@UserID and a.Password=@Password ";
            SqlCommand cmd = new SqlCommand(sqlStr);
            cmd.Parameters.AddWithValue("@UserID", SystemUser.CurrentUser.UserID);
            cmd.Parameters.AddWithValue("@Password", password);
            var dtUser = dbc.ExecuteDataTable(cmd);
            if (dtUser.Rows.Count > 0)
            {
                string _url = ServiceURL + "baofooReturn";
                string jsonParam = new JavaScriptSerializer().Serialize(new
                {
                    mycardid = id,
                    userid = SystemUser.CurrentUser.UserID,
                    userxm = SystemUser.CurrentUser.UserName
                });
                var request = (HttpWebRequest)WebRequest.Create(_url);
                request.Method = "POST";
                request.ContentType = "application/json;charset=UTF-8";
                var byteData = Encoding.UTF8.GetBytes(jsonParam);
                var length = byteData.Length;
                request.ContentLength = length;
                var writer = request.GetRequestStream();
                writer.Write(byteData, 0, length);
                writer.Close();
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();



                JObject jo = (JObject)JsonConvert.DeserializeObject(responseString);
                try
                {
                    if (Convert.ToBoolean(jo["success"].ToString()))
                    {
                        //sqlStr = "update tb_b_mycard set status=3 where mycardId=" + dbc.ToSqlValue(id);
                        //dbc.ExecuteNonQuery(sqlStr);
                        return true;
                    }
                    else
                    {
                        throw new Exception("退款失败");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(jo["details"].ToString());
                }

            }
            return false;
        }
    }
    #endregion


    #region 退款审核界面
    [CSMethod("GetCZTkshList")]
    public object GetCZTkshList(int pagnum, int pagesize, JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                List<string> wArr = new List<string>();
                string cx_beg = jsr["cx_beg"].ToString();
                if (!string.IsNullOrEmpty(cx_beg))
                {
                    wArr.Add("a.addtime >= " + dbc.ToSqlValue(Convert.ToDateTime(cx_beg)));
                }
                string cx_endjsr = jsr["cx_end"].ToString();
                if (!string.IsNullOrEmpty(cx_endjsr))
                {
                    wArr.Add("a.addtime < " + dbc.ToSqlValue(Convert.ToDateTime(cx_endjsr).AddDays(1)));
                }
                if (!string.IsNullOrEmpty(jsr["cx_fhrzh"]))
                {
                    wArr.Add(dbc.C_Like("b.UserName", jsr["cx_fhrzh"], LikeStyle.LeftAndRightLike));
                }
                if (!string.IsNullOrEmpty(jsr["cx_zxmc"]))
                {
                    wArr.Add(dbc.C_Like("c.UserXM", jsr["cx_zxmc"], LikeStyle.LeftAndRightLike));
                }
                if (!string.IsNullOrEmpty(jsr["cx_istk"]))
                {
                    wArr.Add(dbc.C_EQ("a.status", Convert.ToInt32(jsr["cx_istk"].ToString())));
                }
                string sqlW = "";
                if (wArr.Count > 0)
                {
                    sqlW = " and " + string.Join(" and ", wArr);
                }

                string str = @"select a.*,b.UserName fhrmc,c.UserXM zxmc
                                ,d.addtime tktime,e.Money tkmoney,
                                case
                                when d.salerecordlx=0 then '耗材券' when d.salerecordlx=4 then '授权券' when d.salerecordlx=5 then '充值券'
                                else '自发布券' end as qlx
                                from  tb_b_myvoucher a 
                                left join tb_b_user b on a.UserID=b.UserID
                                left join tb_b_user c on a.CardUserID=c.UserID
                                left join tb_b_refund d on a.myvoucherid=d.mycardid
                                left join tb_b_order e on a.OrderCode=e.OrderCode
                                where a.status in(2,3) and a.OrderCode is not null ";
                str += sqlW;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.status asc,a.addtime desc", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    //列表EXCEL退款审核数据导出EXCEL
    [CSMethod("GetCZTkshListOutList", 2)]
    public byte[] GetCZTkshListOutList(JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {

                List<string> wArr = new List<string>();
                string cx_beg = jsr["cx_beg"].ToString();
                if (!string.IsNullOrEmpty(cx_beg))
                {
                    wArr.Add("a.addtime >= " + dbc.ToSqlValue(Convert.ToDateTime(cx_beg)));
                }
                string cx_endjsr = jsr["cx_end"].ToString();
                if (!string.IsNullOrEmpty(cx_endjsr))
                {
                    wArr.Add("a.addtime < " + dbc.ToSqlValue(Convert.ToDateTime(cx_endjsr).AddDays(1)));
                }
                if (!string.IsNullOrEmpty(jsr["cx_fhrzh"]))
                {
                    wArr.Add(dbc.C_Like("b.UserName", jsr["cx_fhrzh"], LikeStyle.LeftAndRightLike));
                }
                if (!string.IsNullOrEmpty(jsr["cx_zxmc"]))
                {
                    wArr.Add(dbc.C_Like("c.UserXM", jsr["cx_zxmc"], LikeStyle.LeftAndRightLike));
                }
                if (!string.IsNullOrEmpty(jsr["cx_istk"]))
                {
                    wArr.Add(dbc.C_EQ("a.status", Convert.ToInt32(jsr["cx_istk"].ToString())));
                }
                string sqlW = "";
                if (wArr.Count > 0)
                {
                    sqlW = " and " + string.Join(" and ", wArr);
                }

                string str = @"select a.*,b.UserName fhrmc,c.UserXM zxmc
                                ,d.addtime tktime,e.Money tkmoney,
                                case
                                 when d.salerecordlx=0 then '耗材券' when d.salerecordlx=4 then '授权券' when d.salerecordlx=5 then '充值券'
                                else '自发布券' end as qlx
                                from tb_b_myvoucher a 
                                left join tb_b_user b on a.UserID=b.UserID
                                left join tb_b_user c on a.CardUserID=c.UserID
                                left join tb_b_refund d on a.myvoucherid=d.mycardid
                                left join tb_b_order e on a.OrderCode=e.OrderCode
                                where a.status in(2,3) and a.OrderCode is not null ";
                str += sqlW;

                //开始取分页数据

                System.Data.DataTable dt = dbc.ExecuteDataTable(str + " order by a.status asc,a.addtime desc");

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
                style4.Font.Color = Color.Red;//文字大小

                style4.IsTextWrapped = true;//单元格内容自动换行
                style4.Font.IsBold = false;//粗体




                //第一行标题列表
                //合并单元格cells.Merge(1, 0, 3, 1) 参数1代表当前行，参数0代表当前行当前列即第一行第一列，参数3合并的行数，参数4合并的列数

                cells.Merge(0, 0, 1, 9);

                cells[0, 0].PutValue("充值退款数据导出");
                cells[0, 0].SetStyle(style3);
                cells.SetRowHeight(0, 30);




                cells.SetColumnWidth(0, 30);
                cells[2, 0].PutValue("订单号");
                cells[2, 0].SetStyle(style1);
                cells.SetColumnWidth(1, 15);
                cells[2, 1].PutValue("申请退款券额");
                cells[2, 1].SetStyle(style1);
                cells.SetColumnWidth(2, 30);
                cells[2, 2].PutValue("发货人账号");
                cells[2, 2].SetStyle(style1);
                cells.SetColumnWidth(3, 30);
                cells[2, 3].PutValue("专线名称");
                cells[2, 3].SetStyle(style1);
                cells.SetColumnWidth(4, 15);
                cells[2, 4].PutValue("购买时间");
                cells[2, 4].SetStyle(style1);
                cells.SetColumnWidth(5, 20);
                cells[2, 5].PutValue("退款状态");
                cells[2, 5].SetStyle(style1);
                cells.SetColumnWidth(6, 20);
                cells[2, 6].PutValue("退款时间");
                cells[2, 6].SetStyle(style1);
                cells.SetColumnWidth(7, 20);
                cells[2, 7].PutValue("退款金额");
                cells[2, 7].SetStyle(style1);
                cells.SetColumnWidth(8, 20);
                cells[2, 8].PutValue("券类型");
                cells[2, 8].SetStyle(style1);




                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var status = "";
                        if (dt.Rows[i]["status"] != null && dt.Rows[i]["status"].ToString() != "")
                        {
                            if (dt.Rows[i]["status"].ToString() == "2")
                            {
                                status = "退款审核中";
                            }
                            else if (dt.Rows[i]["status"].ToString() == "3")
                            {
                                status = "已退款";
                            }
                        }


                        cells[i + 3, 0].PutValue(dt.Rows[i]["OrderCode"].ToString());
                        cells[i + 3, 0].SetStyle(style2);
                        cells[i + 3, 1].PutValue(dt.Rows[i]["points"].ToString());
                        cells[i + 3, 1].SetStyle(style2);
                        cells[i + 3, 2].PutValue(dt.Rows[i]["fhrmc"].ToString());
                        cells[i + 3, 2].SetStyle(style2);
                        cells[i + 3, 3].PutValue(dt.Rows[i]["zxmc"].ToString());
                        cells[i + 3, 3].SetStyle(style2);
                        if (dt.Rows[i]["addtime"].ToString() != "")
                        {
                            cells[i + 3, 4].PutValue(Convert.ToDateTime(dt.Rows[i]["addtime"].ToString()).ToString("yyyy-MM-dd"));
                        }
                        else
                        {
                            cells[i + 3, 4].PutValue("");

                        }
                        cells[i + 3, 4].SetStyle(style2);
                        cells[i + 3, 5].PutValue(status);
                        cells[i + 3, 5].SetStyle(style2);
                        if (dt.Rows[i]["tktime"].ToString() != "")
                        {
                            cells[i + 3, 6].PutValue(Convert.ToDateTime(dt.Rows[i]["tktime"].ToString()).ToString("yyyy-MM-dd"));
                        }
                        else
                        {
                            cells[i + 3, 6].PutValue("");

                        }
                        cells[i + 3, 6].SetStyle(style2);
                        cells[i + 3, 7].PutValue(dt.Rows[i]["tkmoney"].ToString());
                        cells[i + 3, 7].SetStyle(style2);
                        cells[i + 3, 8].PutValue(dt.Rows[i]["qlx"].ToString());
                        cells[i + 3, 8].SetStyle(style2);

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

    [CSMethod("CZVerifyPassWorld")]
    public bool CZVerifyPassWorld(string id, string password)
    {
        using (DBConnection dbc = new DBConnection())
        {
            string ServiceURL = System.Configuration.ConfigurationManager.AppSettings["ServiceURL"].ToString();

            string sqlStr = @"select a.UserID YH_ID, a.UserName YH_DLM,a.UserXM YH_XM,a.Password,b.roleId,b.companyId from tb_b_user a 
                            left join tb_b_user_role b on a.UserID = b.userId 
                            where a.UserID=@UserID and a.Password=@Password ";
            SqlCommand cmd = new SqlCommand(sqlStr);
            cmd.Parameters.AddWithValue("@UserID", SystemUser.CurrentUser.UserID);
            cmd.Parameters.AddWithValue("@Password", password);
            var dtUser = dbc.ExecuteDataTable(cmd);
            if (dtUser.Rows.Count > 0)
            {
                string _url = ServiceURL + "baofooReturnMyvoucher";
                string jsonParam = new JavaScriptSerializer().Serialize(new
                {
                    myvoucherid = id,
                    userid = SystemUser.CurrentUser.UserID,
                    userxm = SystemUser.CurrentUser.UserName
                });
                var request = (HttpWebRequest)WebRequest.Create(_url);
                request.Method = "POST";
                request.ContentType = "application/json;charset=UTF-8";
                var byteData = Encoding.UTF8.GetBytes(jsonParam);
                var length = byteData.Length;
                request.ContentLength = length;
                var writer = request.GetRequestStream();
                writer.Write(byteData, 0, length);
                writer.Close();
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();



                JObject jo = (JObject)JsonConvert.DeserializeObject(responseString);
                try
                {
                    if (Convert.ToBoolean(jo["success"].ToString()))
                    {
                        //sqlStr = "update tb_b_mycard set status=3 where mycardId=" + dbc.ToSqlValue(id);
                        //dbc.ExecuteNonQuery(sqlStr);
                        return true;
                    }
                    else
                    {
                        throw new Exception("退款失败");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(jo["details"].ToString());
                }

            }
            return false;
        }
    }
    #endregion


}