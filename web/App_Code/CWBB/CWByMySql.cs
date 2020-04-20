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
using SmartFramework4v2.Data.MySql;
/// <summary>
///CWBBMag 的摘要说明
/// </summary>

[CSClass("CWByMySql")]
public class CWByMySql
{
    
    [CSMethod("ExportGYSHX", 2)]
    public byte[] ExportGYSHX(string stime, string etime, string username,string ddbm)
    {
		using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                string where = "";


                if (!string.IsNullOrEmpty(username))
                {
                    where += " and " + dbc.C_Like("f.username", username.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (stime != null && stime != "")
                {
                    where += " and c.shippingnoteadddatetim>="+dbc.ToSqlValue(stime);
                }
                if (etime != null && etime != "")
                {
                    where += " and c.shippingnoteadddatetim<=" + dbc.ToSqlValue(etime);
                }
                if (ddbm != null && ddbm != "")
                {
                    where += " and c.shippingnotenumber like '%" + ddbm + "%'";
                }

                string str = @"
SELECT e.actualcompanypay,e.goodsfromroute,e.goodstoroute,e.goodsreceiptplace,e.descriptionofgoods,d.invoicestatus,d.billingid,e.actualmoney,f.username,f.userid,c.shippingnoteid,c.shippingnoteadddatetime,c.shippingnotenumber,c.statisticstype,d.totalamount,d.totalvaloremtax,d.rate,d.billingtime,d.invoicecode,d.invoicenumber FROM
 tb_b_shippingnoteinfo c LEFT JOIN  
(SELECT a.shippingnoteid,b.totalvaloremtax,b.rate,b.billingtime,b.invoicecode,b.invoicenumber,b.totalamount,b.invoicestatus,b.billingid  
	FROM tb_b_invoicedetail a LEFT JOIN tb_b_invoice b ON a.billingid=b.billingid
) d ON c.shippingnoteid=d.shippingnoteid 
LEFT JOIN tb_b_sourcegoodsinfo_offer e ON c.offerid=e.offerid
LEFT JOIN tb_b_user f ON e.shipperid=f.userid 
WHERE 
(d.invoicestatus=0
OR c.shippingnoteid NOT IN (SELECT shippingnoteid FROM tb_b_invoicedetail  )
)

 AND c.shippingnotestatuscode = 90

 ";
                str += where + " ORDER BY c.consignmentdatetime";


                //开始取分页数据
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = dbc.ExecuteDataTable(str);

				Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(); //工作簿
                Aspose.Cells.Worksheet sheet = workbook.Worksheets[0]; //工作表
                Aspose.Cells.Cells cells = sheet.Cells;//单元格

                #region 样式
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
                #endregion

                String[] ColumnName = { "订单时间", "厂家", "单号", "起始地", "目的地", "收货地址", "货物", "应收金额", "合计金额","开票时间","发票代码","发票号码" };
                String[] ColumnV = { "shippingnoteadddatetime", "username", "shippingnotenumber", "goodsfromroute", "goodstoroute", "goodsreceiptplace", "descriptionofgoods", "actualcompanypay","totalamount","billingtime","invoicecode" ,"invoicenumber"  };
                for (int i = 0; i < ColumnName.Length; i++)
                {
                    cells.SetColumnWidth(i, 30);
                    cells[0, i].PutValue(ColumnName[i]);
                    cells[0, i].SetStyle(style1);
                }

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int a = 0; a < ColumnV.Length; a++)
                        {
                            var ColumnVs = dt.Rows[i][ColumnV[a]].ToString();
                            cells[i + 1, a].PutValue(ColumnVs);
                            cells[i + 1, a].SetStyle(style2);
                        }
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
    
    [CSMethod("GetUserList")]
    public object GetUserList(int pagnum, int pagesize, string stime, string etime, string username, string ddbm)
    {


        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";


                if (!string.IsNullOrEmpty(username))
                {
                    where += " and " + dbc.C_Like("f.username", username.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (stime != null && stime != "")
                {
                    where += " and c.shippingnoteadddatetim>="+dbc.ToSqlValue(stime);
                }
                if (etime != null && etime != "")
                {
                    where += " and c.shippingnoteadddatetim<=" + dbc.ToSqlValue(etime);
                }
                if (ddbm != null && ddbm != "")
                {
                    where += " and c.shippingnotenumber like '%" + ddbm + "%'";
                }

                string str = @"
SELECT e.actualcompanypay,e.goodsfromroute,e.goodstoroute,e.goodsreceiptplace,e.descriptionofgoods,d.invoicestatus,d.billingid,e.actualmoney,f.username,f.userid,c.shippingnoteid,c.shippingnoteadddatetime,c.shippingnotenumber,c.statisticstype,d.totalamount,d.totalvaloremtax,d.rate,d.billingtime,d.invoicecode,d.invoicenumber FROM
 tb_b_shippingnoteinfo c LEFT JOIN  
(SELECT a.shippingnoteid,b.totalvaloremtax,b.rate,b.billingtime,b.invoicecode,b.invoicenumber,b.totalamount,b.invoicestatus,b.billingid  
	FROM tb_b_invoicedetail a LEFT JOIN tb_b_invoice b ON a.billingid=b.billingid
) d ON c.shippingnoteid=d.shippingnoteid 
LEFT JOIN tb_b_sourcegoodsinfo_offer e ON c.offerid=e.offerid
LEFT JOIN tb_b_user f ON e.shipperid=f.userid 
WHERE 
(d.invoicestatus=0
OR c.shippingnoteid NOT IN (SELECT shippingnoteid FROM tb_b_invoicedetail  )
)

 AND c.shippingnotestatuscode = 90

 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " ORDER BY c.consignmentdatetime", pagesize, ref cp, out ac);

 


                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }


    [CSMethod("GetUserList2")]
    public object GetUserList2(int pagnum, int pagesize, string stime, string etime, string username)
    {


        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";


                if (!string.IsNullOrEmpty(username))
                {
                    where += " and " + dbc.C_Like("f.username", username.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (stime != null && stime != "")
                {
                    where += " and c.shippingnoteadddatetim>=" + dbc.ToSqlValue(stime);
                }
                if (etime != null && etime != "")
                {
                    where += " and c.shippingnoteadddatetim<=" + dbc.ToSqlValue(etime);
                }


                string str = @"
SELECT d.billingid,e.actualmoney,f.username,f.userid,c.shippingnoteid,c.shippingnoteadddatetime,c.shippingnotenumber,c.statisticstype,d.totalamount,d.totalvaloremtax,d.rate,d.billingtime,d.invoicecode,d.invoicenumber FROM
 tb_b_shippingnoteinfo c LEFT JOIN  
(SELECT a.shippingnoteid,b.totalvaloremtax,b.rate,b.billingtime,b.invoicecode,b.invoicenumber,b.totalamount,b.invoicestatus,b.billingid  
	FROM tb_b_invoicedetail a LEFT JOIN tb_b_invoice b ON a.billingid=b.billingid
) d ON c.shippingnoteid=d.shippingnoteid 
LEFT JOIN tb_b_sourcegoodsinfo_offer e ON c.offerid=e.offerid
LEFT JOIN tb_b_user f ON e.shipperid=f.userid 
WHERE 
d.invoicestatus=0 AND c.shippingnotestatuscode = 90

 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " ORDER BY c.consignmentdatetime", pagesize, ref cp, out ac);




                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    [CSMethod("GetUserList3")]
    public object GetUserList3(int pagnum, int pagesize, string stime, string etime, string username)
    {


        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";


                if (!string.IsNullOrEmpty(username))
                {
                    where += " and " + dbc.C_Like("f.username", username.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (stime != null && stime != "")
                {
                    where += " and c.shippingnoteadddatetim>=" + dbc.ToSqlValue(stime);
                }
                if (etime != null && etime != "")
                {
                    where += " and c.shippingnoteadddatetim<=" + dbc.ToSqlValue(etime);
                }


                string str = @"
SELECT d.billingid,e.actualmoney,f.username,f.userid,c.shippingnoteid,c.shippingnoteadddatetime,c.shippingnotenumber,c.statisticstype,d.totalamount,d.totalvaloremtax,d.rate,d.billingtime,d.invoicecode,d.invoicenumber FROM
 tb_b_shippingnoteinfo c LEFT JOIN  
(SELECT a.shippingnoteid,b.totalvaloremtax,b.rate,b.billingtime,b.invoicecode,b.invoicenumber,b.totalamount,b.invoicestatus,b.billingid  
	FROM tb_b_invoicedetail a LEFT JOIN tb_b_invoice b ON a.billingid=b.billingid
) d ON c.shippingnoteid=d.shippingnoteid 
LEFT JOIN tb_b_sourcegoodsinfo_offer e ON c.offerid=e.offerid
LEFT JOIN tb_b_user f ON e.shipperid=f.userid 
WHERE 
d.invoicestatus=1 AND c.shippingnotestatuscode = 90

 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " ORDER BY c.consignmentdatetime", pagesize, ref cp, out ac);




                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }


    [CSMethod("SaveKP")]
    public static bool SaveKP(JSReader jsr, string shippingnoteid)
    {

        var companyId = SystemUser.CurrentUser.CompanyID;


        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {


                var billingid = Guid.NewGuid().ToString();
                var dt = dbc.GetEmptyDataTable("tb_b_invoice");
                var dr = dt.NewRow();
                dr["billingid"] = new Guid(billingid);
                dr["totalamount"] = jsr["totalamount"].ToString();
              //  dr["totalvaloremtax"] = jsr["totalvaloremtax"].ToString();
                dr["rate"] = jsr["rate"].ToString();
                dr["billingtime"] = jsr["billingtime"].ToString();

                dr["invoicecode"] = jsr["invoicecode"].ToString();

                dr["invoicenumber"] = jsr["invoicenumber"].ToString();
                dr["isdeleteflag"] = 0;
                dr["invoicestatus"] = 0;
                dt.Rows.Add(dr);
                dbc.InsertTable(dt);

                var dtde = dbc.GetEmptyDataTable("tb_b_invoicedetail");
                var drde = dtde.NewRow();

                drde["invoicedetailid"] = Guid.NewGuid().ToString();

                drde["billingid"] = new Guid(billingid);
                drde["billingitem"] = jsr["billingitem"].ToString();
                drde["amount"] = jsr["totalamount"].ToString();
                drde["shippingnoteid"] = shippingnoteid;
                drde["isdeleteflag"] = 0;
                dtde.Rows.Add(drde);
                dbc.InsertTable(dtde);



                var dtre = dbc.GetEmptyDataTable("tb_b_shippingnoteinfo_record");
                var drre = dtre.NewRow();

                drre["id"] = Guid.NewGuid().ToString();
                drre["shippingnoteid"] = shippingnoteid;
                 drre["recordtype"] = "开票";
                 drre["recordmemo"] = "开票时间：" + jsr["billingtime"].ToString() + ",票号:" + jsr["invoicenumber"].ToString() + ",金额:" + jsr["totalamount"].ToString() + ",代码:" + jsr["invoicecode"].ToString();
                 drre["status"] = 0;

                 drre["adduser"] = SystemUser.CurrentUser.UserID;
                 drre["updateuser"] = SystemUser.CurrentUser.UserID;
                 drre["addtime"] = DateTime.Now;
                 drre["updatetime"] = DateTime.Now;

                dtre.Rows.Add(drre);
                dbc.InsertTable(dtre);






                dbc.CommitTransaction();

            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
        return true;



    }


    [CSMethod("ChangeZF")]
    public static bool ChangeZF(string billingid, string shippingnoteid)
    {

        var companyId = SystemUser.CurrentUser.CompanyID;


        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {

                dbc.ExecuteNonQuery("update tb_b_invoice set invoicestatus=1 where billingid=" + dbc.ToSqlValue(billingid));

                var dt = dbc.ExecuteDataTable("select * from tb_b_invoice  where billingid=" + dbc.ToSqlValue(billingid));

                var jsr = dt.Rows[0];
                var dtre = dbc.GetEmptyDataTable("tb_b_shippingnoteinfo_record");
                var drre = dtre.NewRow();

                drre["id"] = Guid.NewGuid().ToString();
                drre["shippingnoteid"] = shippingnoteid;
                drre["recordtype"] = "作废发票";
                drre["recordmemo"] = "作废发票时间：" + DateTime.Now.ToString("yyyy-MM-dd") + ",票号:" + jsr["invoicenumber"].ToString() + ",金额:" + jsr["totalamount"].ToString() + ",代码:" + jsr["invoicecode"].ToString();
                drre["status"] = 0;

                drre["adduser"] = SystemUser.CurrentUser.UserID;
                drre["updateuser"] = SystemUser.CurrentUser.UserID;
                drre["addtime"] = DateTime.Now;
                drre["updatetime"] = DateTime.Now;

                dtre.Rows.Add(drre);
                dbc.InsertTable(dtre);



                dbc.CommitTransaction();

            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
        return true;



    }



    [CSMethod("GetUserMX")]
    public static object GetUserMX(string userid)
    {

 

        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
             try
            {

                return dbc.ExecuteDataTable("select * from  tb_b_user  where userid=" + dbc.ToSqlValue(userid));
             }
            catch (Exception ex)
            {
                 throw ex;
            }
        }
 


    } 


    [CSMethod("GetDDMX")]
    public static object GetDDMX(string shippingnotenumber)
    {

 

        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
             try
            {

                var sql = @" SELECT c.*,b.*,e.name vehiclelengthrequirementname,f.vehiclenumber FROM tb_b_sourcegoodsinfo_offer b
	                                LEFT JOIN tb_b_user c ON b.shipperid=c.userid
                                    LEFT JOIN tb_b_dictionary_detail e ON b.vehiclelengthrequirement=e.bm
                                    LEFT JOIN tb_b_locationinfo f ON b.shippingnotenumber=f.shippingnotenumber

where b.shippingnotenumber=" + dbc.ToSqlValue(shippingnotenumber);
                var dt = dbc.ExecuteDataTable(sql);
                return dt;
             }
            catch (Exception ex)
            {
                 throw ex;
            }
        }
 


    }






    [CSMethod("GYSHXList")]
    public object GYSHXList(int pagnum, int pagesize, string stime, string etime, string username,string ddbm,string ishx)
    {


        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";


                if (!string.IsNullOrEmpty(username))
                {
                    where += " and " + dbc.C_Like("f.username", username.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (stime != null && stime != "")
                {
                    where += " and c.shippingnoteadddatetim>=" + dbc.ToSqlValue(stime);
                }
                if (etime != null && etime != "")
                {
                    where += " and c.shippingnoteadddatetim<=" + dbc.ToSqlValue(etime);
                }
                if (ddbm != null && ddbm != "")
                {
                    where += " and c.shippingnotenumber like '%" + dbc.ToSqlValue(ddbm) + "%'";
                }


                string str = @"
select * from (
SELECT c.offerid,(SELECT SUM(verifymoney) FROM tb_b_shippingnoteinfo_verify WHERE shippingnoteid=c.shippingnoteid) AS verifymoney,e.actualmoney,e.actualwaymoney,f.username,f.userid,c.shippingnoteid,c.shippingnoteadddatetime,c.shippingnotenumber,c.statisticstype,d.totalamount,d.totalvaloremtax,d.rate,d.billingtime,d.invoicecode,d.invoicenumber,e.goodsfromroute,e.goodstoroute,e.goodsreceiptplace FROM
 tb_b_shippingnoteinfo c LEFT JOIN  
(SELECT a.shippingnoteid,b.totalvaloremtax,b.rate,b.billingtime,b.invoicecode,b.invoicenumber,b.totalamount 
	FROM tb_b_invoicedetail a LEFT JOIN tb_b_invoice b ON a.billingid=b.billingid
) d ON c.shippingnoteid=d.shippingnoteid 
LEFT JOIN tb_b_sourcegoodsinfo_offer e ON c.offerid=e.offerid
LEFT JOIN tb_b_user f ON e.shipperid=f.userid 
WHERE 
c.shippingnoteid NOT IN (SELECT shippingnoteid FROM tb_b_invoicedetail  ) AND c.shippingnotestatuscode >= 30

 ";
                str += where + " ORDER BY c.consignmentdatetime)  AS tab ";


                if (ishx != null && ishx != "")
                {
                    if (ishx == "0")
                    {
                        str += " where verifymoney=0 ";
                    }
                    else if (ishx == "1")
                    {
                        str += " where verifymoney<actualwaymoney ";
                    }
                    else if (ishx == "2")
                    {
                        str += " where verifymoney=actualwaymoney ";
                    }


                 }


                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " ", pagesize, ref cp, out ac);




                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }


    [CSMethod("HXMoney")]
    public static bool HXMoney(JSReader jsr, string shippingnoteid, string userid, string username, string ye, string actualwaymoney, string offerid)
    {

        var companyId = SystemUser.CurrentUser.CompanyID;


        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                //
                if (ye == "0")
                {
                    dbc.ExecuteNonQuery("update tb_b_sourcegoodsinfo_offer   set carrierverifystatus = 2,carrierverifymoney =" + dbc.ToSqlValue(actualwaymoney) + " where  offerid=" + dbc.ToSqlValue(offerid));
                }
                else {
                    var dts = dbc.ExecuteDataTable("SELECT carrierverifymoney,carrierverifystatus FROM tb_b_sourcegoodsinfo_offer WHERE offerid=" + dbc.ToSqlValue(offerid));
                    if (dts.Rows.Count > 0)
                    {
                        if (dts.Rows[0][0] != null && dts.Rows[0][0].ToString() != "")
                        {
                            dbc.ExecuteNonQuery("update tb_b_sourcegoodsinfo_offer   set carrierverifystatus = 1,carrierverifymoney =carrierverifymoney+" + dbc.ToSqlValue(jsr["verifymoney"].ToString()) + " where  offerid=" + dbc.ToSqlValue(offerid));

                        }
                        else {
                            dbc.ExecuteNonQuery("update tb_b_sourcegoodsinfo_offer   set carrierverifystatus = 1,carrierverifymoney =" + dbc.ToSqlValue(jsr["verifymoney"].ToString()) + " where  offerid=" + dbc.ToSqlValue(offerid));

                        }
                    }


                }


                 var dt = dbc.GetEmptyDataTable("tb_b_shippingnoteinfo_verify");
                var dr = dt.NewRow();
                dr["id"] = Guid.NewGuid().ToString();
                dr["shippingnoteid"] = shippingnoteid;
                dr["verifytype"] = 1;//供应商核销
                dr["verifymoney"] = jsr["verifymoney"].ToString();
                dr["verifypaytype"] = jsr["verifypaytype"].ToString();
                dr["verifytime"] = jsr["verifytime"].ToString();

                dr["userid"] = userid;
                dr["status"] = 0;
                dr["adduser"] = SystemUser.CurrentUser.UserID; 
                dr["addtime"] = DateTime.Now;
                dr["updateuser"] = SystemUser.CurrentUser.UserID;
                dr["updatetime"] = DateTime.Now;
                dt.Rows.Add(dr);
                dbc.InsertTable(dt);



                var dt2 = dbc.GetEmptyDataTable("tb_b_shippingnoteinfo_record");
                var dr2 = dt2.NewRow();
                dr2["id"] = Guid.NewGuid().ToString();
                dr2["shippingnoteid"] = shippingnoteid;
                dr2["recordtype"] = "财务核销";
                dr2["recordmemo"] = SystemUser.CurrentUser.UserName + "在" + Convert.ToDateTime(jsr["verifytime"].ToString()).ToString("yyyy-MM-dd") + "，财务核销"+ username +jsr["verifymoney"].ToString()+"元";

                dr2["status"] = 0;
                dr2["adduser"] = SystemUser.CurrentUser.UserID;
                dr2["addtime"] = DateTime.Now;
                dr2["updateuser"] = SystemUser.CurrentUser.UserID;
                dr2["updatetime"] = DateTime.Now;
                dt2.Rows.Add(dr2);
                dbc.InsertTable(dt2);




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

    [CSMethod("HXMoneyALL")]
    public static bool HXMoneyALL(JSReader jsr,JSReader jsr2)
    {

        var companyId = SystemUser.CurrentUser.CompanyID;


        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                for (int i = 0; i < jsr.ToArray().Length; i++)
                {

                    string shippingnoteid = "";
                    string userid = "";
                    string offerid = "";
                    string actualwaymoney = "";
                    string username = "";
                    string verifymoney = "";
                    string[] arr = jsr.ToArray()[i].ToString().Split(',');
                    if (arr.Length > 0)
                    {
                        shippingnoteid = arr[0].ToString();
                        userid = arr[1].ToString();
                        offerid = arr[2].ToString();
                        actualwaymoney = arr[3].ToString();
                        if (arr[4].ToString() != null && arr[4].ToString() != "" && arr[4].ToString() != "null")
                        {
                            verifymoney = arr[4].ToString();
                        }
                        else {
                            verifymoney = "0";
                        }
                        username = arr[5].ToString();
                    }
                    string ye = (Convert.ToDouble(actualwaymoney) - Convert.ToDouble(verifymoney)).ToString();


                    if (ye == "0")
                    {
                        dbc.ExecuteNonQuery("update tb_b_sourcegoodsinfo_offer   set carrierverifystatus = 2,carrierverifymoney =" + dbc.ToSqlValue(actualwaymoney) + " where  offerid=" + dbc.ToSqlValue(offerid));
                    }
                    else
                    {
                        var dts = dbc.ExecuteDataTable("SELECT carrierverifymoney,carrierverifystatus FROM tb_b_sourcegoodsinfo_offer WHERE offerid=" + dbc.ToSqlValue(offerid));
                        if (dts.Rows.Count > 0)
                        {
                            if (dts.Rows[0][0] != null && dts.Rows[0][0].ToString() != "")
                            {
                                dbc.ExecuteNonQuery("update tb_b_sourcegoodsinfo_offer   set carrierverifystatus = 1,carrierverifymoney =carrierverifymoney+" + dbc.ToSqlValue(ye) + " where  offerid=" + dbc.ToSqlValue(offerid));

                            }
                            else
                            {
                                dbc.ExecuteNonQuery("update tb_b_sourcegoodsinfo_offer   set carrierverifystatus = 1,carrierverifymoney =" + dbc.ToSqlValue(ye) + " where  offerid=" + dbc.ToSqlValue(offerid));

                            }
                        }


                    }


                    var dt = dbc.GetEmptyDataTable("tb_b_shippingnoteinfo_verify");
                    var dr = dt.NewRow();
                    dr["id"] = Guid.NewGuid().ToString();
                    dr["shippingnoteid"] = shippingnoteid;
                    dr["verifytype"] = 1;//供应商核销
                    dr["verifymoney"] = ye;
                    dr["verifypaytype"] = jsr2["verifypaytype"].ToString();
                    dr["verifytime"] = jsr2["verifytime"].ToString();

                    dr["userid"] = userid;
                    dr["status"] = 0;
                    dr["adduser"] = SystemUser.CurrentUser.UserID;
                    dr["addtime"] = DateTime.Now;
                    dr["updateuser"] = SystemUser.CurrentUser.UserID;
                    dr["updatetime"] = DateTime.Now;
                    dt.Rows.Add(dr);
                    dbc.InsertTable(dt);



                    var dt2 = dbc.GetEmptyDataTable("tb_b_shippingnoteinfo_record");
                    var dr2 = dt2.NewRow();
                    dr2["id"] = Guid.NewGuid().ToString();
                    dr2["shippingnoteid"] = shippingnoteid;
                    dr2["recordtype"] = "财务核销";
                    dr2["recordmemo"] = SystemUser.CurrentUser.UserName + "在" + Convert.ToDateTime(jsr2["verifytime"].ToString()).ToString("yyyy-MM-dd") + "，财务核销" + username + ye + "元";

                    dr2["status"] = 0;
                    dr2["adduser"] = SystemUser.CurrentUser.UserID;
                    dr2["addtime"] = DateTime.Now;
                    dr2["updateuser"] = SystemUser.CurrentUser.UserID;
                    dr2["updatetime"] = DateTime.Now;
                    dt2.Rows.Add(dr2);
                    dbc.InsertTable(dt2);

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


    [CSMethod("GetHXLIST")]
    public DataTable GetHXLIST(string shippingnoteid)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                var dt = dbc.ExecuteDataTable(@"SELECT a.*,b.`username` FROM tb_b_shippingnoteinfo_verify a LEFT JOIN tb_b_user b ON a.`userid`=b.`userid`
WHERE shippingnoteid=" + dbc.ToSqlValue(shippingnoteid) + @"
ORDER BY verifytime DESC");

                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }



    [CSMethod("SaveKPALL")]
    public static bool SaveKPALL(JSReader jsr, JSReader jsr2)
    {

        var companyId = SystemUser.CurrentUser.CompanyID;


        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                for (int i = 0; i < jsr.ToArray().Length; i++)
                {
                     string shippingnoteid = "";
                    string offerid = "";
                     string actualcompanypay = "";
                     string[] arr = jsr.ToArray()[i].ToString().Split(',');
                     if (arr.Length > 0)
                     {
                         shippingnoteid = arr[0].ToString();
                         offerid = arr[1].ToString();
                         actualcompanypay = arr[2].ToString();


                     }


                    var billingid = Guid.NewGuid().ToString();
                    var dt = dbc.GetEmptyDataTable("tb_b_invoice");
                    var dr = dt.NewRow();
                    dr["billingid"] = new Guid(billingid);
                    dr["totalamount"] = actualcompanypay;
                    dr["totalvaloremtax"] = actualcompanypay;
                    dr["rate"] = jsr["rate"].ToString();
                    dr["billingtime"] = jsr["billingtime"].ToString();

                    dr["invoicecode"] = jsr["invoicecode"].ToString();

                    dr["invoicenumber"] = jsr["invoicenumber"].ToString();
                    dr["isdeleteflag"] = 0;
                    dr["invoicestatus"] = 0;
                    dt.Rows.Add(dr);
                    dbc.InsertTable(dt);

                    var dtde = dbc.GetEmptyDataTable("tb_b_invoicedetail");
                    var drde = dtde.NewRow();

                    drde["invoicedetailid"] = Guid.NewGuid().ToString();

                    drde["billingid"] = new Guid(billingid);
                    drde["billingitem"] = jsr["billingitem"].ToString();
                    drde["amount"] = actualcompanypay;
                    drde["shippingnoteid"] = shippingnoteid;
                    drde["isdeleteflag"] = 0;
                    dtde.Rows.Add(drde);
                    dbc.InsertTable(dtde);



                    var dtre = dbc.GetEmptyDataTable("tb_b_shippingnoteinfo_record");
                    var drre = dtre.NewRow();

                    drre["id"] = Guid.NewGuid().ToString();
                    drre["shippingnoteid"] = shippingnoteid;
                    drre["recordtype"] = "开票";
                    drre["recordmemo"] = "开票时间：" + jsr["billingtime"].ToString() + ",票号:" + jsr["invoicenumber"].ToString() + ",金额:" + actualcompanypay + ",代码:" + jsr["invoicecode"].ToString();
                    drre["status"] = 0;

                    drre["adduser"] = SystemUser.CurrentUser.UserID;
                    drre["updateuser"] = SystemUser.CurrentUser.UserID;
                    drre["addtime"] = DateTime.Now;
                    drre["updatetime"] = DateTime.Now;

                    dtre.Rows.Add(drre);
                    dbc.InsertTable(dtre);


                }





                dbc.CommitTransaction();

            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
        return true;



    }



 



}
