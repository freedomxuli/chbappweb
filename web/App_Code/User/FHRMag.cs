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
    public object GetUserList(int pagnum, int pagesize, string yhm)
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


                            select a.*,case when b.flag is not null then b.flag else 'C' END as flag,c.gmzx,c.gmzxs from tb_b_user a 
                            left join (select * from #adt union all select * from #bdt) b on a.UserID=b.BuyUserID 
                            left join #gmdt c on a.userid=c.BuyUserID
                            where ClientKind=2 " + where+@" order by flag 

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
               

}
