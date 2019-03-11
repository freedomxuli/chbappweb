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
using System.Net;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using ThoughtWorks.QRCode.Codec;
/// <summary>
///UserMag 的摘要说明
/// </summary>

[CSClass("FHRMag")]
public class FHRMag
{
    [CSMethod("GetUserList")]
    public object GetUserList(int pagnum, int pagesize, string yhm,string dj)
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

                string where1 = "";
                if (!string.IsNullOrEmpty(dj))
                {
                    where1 += " and " + dbc.C_EQ("flag", dj);
                }


                string str = @"select * into #temp from  tb_b_order where status=0 and ZhiFuZT=1 and DateDiff(dd,AddTime,getdate())<4

                            select * into #temp1 fROM (
                            select orderid,BuyUserID,AddTime,SaleRecordID,ROW_NUMBER() over(partition by BuyUserID order by addtime asc) as num from #temp) a 
                            where a.num=1

                            select * into #temp2 fROM (
                            select orderid,BuyUserID,AddTime,SaleRecordID,ROW_NUMBER() over(partition by BuyUserID order by addtime desc) as num from #temp) a 
                            where a.num=1

                            select *,'A' as flag into #adt fROM (
                            select a.BuyUserID fROM #temp1 a  left join #temp2 b on a.BuyUserID=b.BuyUserID  
                            where  DateDiff(dd,a.AddTime,b.AddTime)>=1
                            union
                            select BuyUserID  from(
                            select count(distinct SaleUserID) as xls,BuyUserID from #temp a 
                            group by BuyUserID) b where xls>=5) a 


                            select distinct BuyUserID,'B' as flag into #bdt  from  tb_b_order where status=0 and ZhiFuZT=1 and DateDiff(dd,AddTime,getdate())<10
                            and BuyUserID not in (select BuyUserID from #adt)

                            SELECT t.BuyUserID
	                            ,STUFF((SELECT ','+ltrim(UserXM)
                                                FROM (select distinct BuyUserID,SaleUserID,b.UserXM from tb_b_order a left join tb_b_user b on a.SaleUserID=b.UserID  where status=0 and ZhiFuZT=1)  a
                                                WHERE BuyUserID=t.BuyUserID FOR XML PATH('')), 1, 1, '') 
	                            AS gmzx,count(t.SaleUserID) as gmzxs into #gmdt
                            FROM (select distinct BuyUserID,SaleUserID,b.UserXM from tb_b_order a left join tb_b_user b on a.SaleUserID=b.UserID  where status=0 and ZhiFuZT=1) t
                            GROUP BY BuyUserID

                            select * from (
                            select a.*,case when b.flag is not null then b.flag else 'C' END as flag,c.gmzx,c.gmzxs from tb_b_user a 
                            left join (select * from #adt union all select * from #bdt) b on a.UserID=b.BuyUserID 
                            left join #gmdt c on a.userid=c.BuyUserID
                            where ClientKind=2  " + where + @")  b where 1=1 "+where1+@" order by flag 

                            drop table #temp
                            drop table #temp1
                            drop table #temp2
                            drop table #adt
                            drop table #bdt
                            drop table #gmdt
                            ";

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

    [CSMethod("GetUserListToFile", 2)]
    public byte[] GetUserListToFile(string yhm, string dj)
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
                cells[0, 0].PutValue("手机号");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("等级");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("购买过的专线");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 70);
                cells[0, 3].PutValue("购买过的专线数");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);

                string where = "";
                if (!string.IsNullOrEmpty(yhm.Trim()))
                {
                    where += " and " + dbc.C_Like("a.UserName", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                string where1 = "";
                if (!string.IsNullOrEmpty(dj))
                {
                    where1 += " and " + dbc.C_EQ("flag", dj);
                }


                string str = @"select * into #temp from  tb_b_order where status=0 and ZhiFuZT=1 and DateDiff(dd,AddTime,getdate())<4

                            select * into #temp1 fROM (
                            select orderid,BuyUserID,AddTime,SaleRecordID,ROW_NUMBER() over(partition by BuyUserID order by addtime asc) as num from #temp) a 
                            where a.num=1

                            select * into #temp2 fROM (
                            select orderid,BuyUserID,AddTime,SaleRecordID,ROW_NUMBER() over(partition by BuyUserID order by addtime desc) as num from #temp) a 
                            where a.num=1

                            select *,'A' as flag into #adt fROM (
                            select a.BuyUserID fROM #temp1 a  left join #temp2 b on a.BuyUserID=b.BuyUserID  
                            where  DateDiff(dd,a.AddTime,b.AddTime)>=1
                            union
                            select BuyUserID  from(
                            select count(distinct SaleUserID) as xls,BuyUserID from #temp a 
                            group by BuyUserID) b where xls>=5) a 


                            select distinct BuyUserID,'B' as flag into #bdt  from  tb_b_order where status=0 and ZhiFuZT=1 and DateDiff(dd,AddTime,getdate())<10
                            and BuyUserID not in (select BuyUserID from #adt)

                            SELECT t.BuyUserID
	                            ,STUFF((SELECT ','+ltrim(UserXM)
                                                FROM (select distinct BuyUserID,SaleUserID,b.UserXM from tb_b_order a left join tb_b_user b on a.SaleUserID=b.UserID  where status=0 and ZhiFuZT=1)  a
                                                WHERE BuyUserID=t.BuyUserID FOR XML PATH('')), 1, 1, '') 
	                            AS gmzx,count(t.SaleUserID) as gmzxs into #gmdt
                            FROM (select distinct BuyUserID,SaleUserID,b.UserXM from tb_b_order a left join tb_b_user b on a.SaleUserID=b.UserID  where status=0 and ZhiFuZT=1) t
                            GROUP BY BuyUserID

                            select * from (
                            select a.*,case when b.flag is not null then b.flag else 'C' END as flag,c.gmzx,c.gmzxs from tb_b_user a 
                            left join (select * from #adt union all select * from #bdt) b on a.UserID=b.BuyUserID 
                            left join #gmdt c on a.userid=c.BuyUserID
                            where ClientKind=2  " + where + @")  b where 1=1 " + where1 + @" order by flag 

                            drop table #temp
                            drop table #temp1
                            drop table #temp2
                            drop table #adt
                            drop table #bdt
                            drop table #gmdt
                            ";

                //开始取分页数据
                System.Data.DataTable dt = dbc.ExecuteDataTable(str);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["UserName"] != null && dt.Rows[i]["UserName"].ToString() != "")
                    {
                        cells[i + 1, 0].PutValue(dt.Rows[i]["UserName"]);
                    }
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["flag"] != null && dt.Rows[i]["flag"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["flag"]);
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    if (dt.Rows[i]["gmzx"] != null && dt.Rows[i]["gmzx"].ToString() != "")
                    {
                        cells[i + 1, 2].PutValue(dt.Rows[i]["gmzx"]);
                    }
                    cells[i + 1, 2].SetStyle(style4);
                    if (dt.Rows[i]["gmzxs"] != null && dt.Rows[i]["gmzxs"].ToString() != "")
                    {
                        cells[i + 1, 3].PutValue(dt.Rows[i]["gmzxs"]);
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
}
