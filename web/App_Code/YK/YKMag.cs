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

/// <summary>
/// YKMag 的摘要说明
/// </summary>
[CSClass("YKMag")]
public class YKMag
{
    public YKMag()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    [CSMethod("GetVisionList")]
    public object GetVisionList()
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string str = "select id,carriagepayratio,carriageoil from tb_b_version";
                DataTable dt = dbc.ExecuteDataTable(str);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetVisionByID")]
    public object GetVisionByID(string id)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string str = "select carriagepayratio,carriageoil from tb_b_version where id=@id";
                SqlCommand cmd = new SqlCommand(str);
                cmd.Parameters.Add("@id", id);
                DataTable dt = dbc.ExecuteDataTable(cmd);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("SaveVision")]
    public bool SaveVision(JSReader jsr, string carriageoil)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                var id = jsr["id"].ToString();
                var dt = dbc.GetEmptyDataTable("tb_b_version");
                var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                var dr = dt.NewRow();
                dr["id"] = id;
                dr["carriagepayratio"] = Convert.ToDecimal(jsr["carriagepayratio"].ToString());
                if (!string.IsNullOrEmpty(carriageoil))
                {
                    int carriageoilx = Convert.ToInt32(jsr["XZCarriageoil"].ToString()) + Convert.ToInt32(carriageoil);
                    dr["carriageoil"] = carriageoilx;
                }
                else
                {
                    dr["carriageoil"] = Convert.ToInt32(jsr["XZCarriageoil"].ToString());
                }
                dt.Rows.Add(dr);
                dbc.UpdateTable(dt, dtt);

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


    [CSMethod("GetYKDDList")]
    public object GetYKDDList(int pagnum, int pagesize, string cardNo, string orderId, string zt, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(cardNo.Trim()))
                {
                    where += " and " + dbc.C_Like("a.cardNo", cardNo.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(orderId.Trim()))
                {
                    where += " and " + dbc.C_Like("a.orderId", orderId.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(zt.Trim()))
                {
                    where += " and " + dbc.C_EQ("a.status", zt.Trim());
                }
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.addtime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.addtime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @" select * from tb_b_oil_order a left join tb_b_user b on a.userid=b.userid  where 1=1  
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

    [CSMethod("GetYKDDListToFile", 2)]
    public byte[] GetYKDDListToFile(string cardNo, string orderId, string zt, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {

                Workbook workbook = new Workbook(); //工作簿
                Worksheet sheet = workbook.Worksheets[0]; //工作表
                Cells cells = sheet.Cells;//单元格

                //为标题设置样式  
                Style styleTitle = workbook.Styles[workbook.Styles.Add()];
                styleTitle.HorizontalAlignment = TextAlignmentType.Center;//文字居中
                styleTitle.Font.Name = "宋体";//文字字体
                styleTitle.Font.Size = 18;//文字大小
                styleTitle.Font.IsBold = true;//粗体

                //样式1
                Style style1 = workbook.Styles[workbook.Styles.Add()];
                style1.HorizontalAlignment = TextAlignmentType.Center;//文字居中
                style1.Font.Name = "宋体";//文字字体
                style1.Font.Size = 12;//文字大小
                style1.Font.IsBold = true;//粗体

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
                cells[0, 0].PutValue("加油卡号");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("加油量");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("单价");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("消费总金额");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("油品类型");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("油品名称");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);
                cells[0, 6].PutValue("油品等级");
                cells[0, 6].SetStyle(style2);
                cells.SetColumnWidth(6, 20);
                cells[0, 7].PutValue("订单状态");
                cells[0, 7].SetStyle(style2);
                cells.SetColumnWidth(7, 20);
                cells[0, 8].PutValue("订单号");
                cells[0, 8].SetStyle(style2);
                cells.SetColumnWidth(8, 20);
                cells[0, 9].PutValue("找有网的交易流水号");
                cells[0, 9].SetStyle(style2);
                cells.SetColumnWidth(9, 20);
                cells[0, 10].PutValue("用户手机号");
                cells[0, 10].SetStyle(style2);
                cells.SetColumnWidth(10, 20);
                cells[0, 11].PutValue("时间");
                cells[0, 11].SetStyle(style2);
                cells.SetColumnWidth(11, 20);

                string where = "";
                if (!string.IsNullOrEmpty(cardNo.Trim()))
                {
                    where += " and " + dbc.C_Like("a.cardNo", cardNo.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(orderId.Trim()))
                {
                    where += " and " + dbc.C_Like("a.orderId", orderId.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(zt.Trim()))
                {
                    where += " and " + dbc.C_EQ("a.status", zt.Trim());
                }
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.addtime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.addtime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @" select * from tb_b_oil_order a left join tb_b_user b on a.userid=b.userid where 1=1  
                                 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = dbc.ExecuteDataTable(str + " order by a.addtime desc");


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["cardNo"]);
                    cells[i + 1, 0].SetStyle(style4);
                    cells[i + 1, 1].PutValue(dt.Rows[i]["oilNum"]);
                    cells[i + 1, 1].SetStyle(style4);
                    cells[i + 1, 2].PutValue(dt.Rows[i]["Price"]);
                    cells[i + 1, 2].SetStyle(style4);
                    cells[i + 1, 3].PutValue(dt.Rows[i]["money"]);
                    cells[i + 1, 3].SetStyle(style4);
                    cells[i + 1, 4].PutValue(dt.Rows[i]["oilType"]);
                    cells[i + 1, 4].SetStyle(style4);
                    cells[i + 1, 5].PutValue(dt.Rows[i]["oilName"]);
                    cells[i + 1, 5].SetStyle(style4);
                    cells[i + 1, 6].PutValue(dt.Rows[i]["oilLevel"]);
                    cells[i + 1, 6].SetStyle(style4);
                    string status = "";
                    if (dt.Rows[i]["status"] != null && dt.Rows[i]["status"].ToString() != "")
                    {
                        if (Convert.ToInt32(dt.Rows[i]["status"].ToString()) == 0)
                        {
                            status = "待付款";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["status"].ToString()) == 1)
                        {
                            status = "支付成功";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["status"].ToString()) == 2)
                        {
                            status = "交易取消";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["status"].ToString()) == 3)
                        {
                            status = "交易撤销";
                        }
                    }

                    cells[i + 1, 7].PutValue(status);
                    cells[i + 1, 7].SetStyle(style4);
                    cells[i + 1, 8].PutValue(dt.Rows[i]["oilordercode"]);
                    cells[i + 1, 8].SetStyle(style4);
                    cells[i + 1, 9].PutValue(dt.Rows[i]["orderId"]);
                    cells[i + 1, 9].SetStyle(style4);
                    cells[i + 1, 10].PutValue(dt.Rows[i]["UserName"]);
                    cells[i + 1, 10].SetStyle(style4);
                    if (dt.Rows[i]["addtime"] != null && dt.Rows[i]["addtime"].ToString() != "")
                    {
                        cells[i + 1, 11].PutValue(Convert.ToDateTime(dt.Rows[i]["addtime"]).ToString("yyyy-MM-dd HH:mm:ss"));
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

    [CSMethod("ADDHB")]
    public object ADDHB(JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                var inuserzh = jsr["inuserzh"].ToString();
                int money = Convert.ToInt32(jsr["money"].ToString());

                string str = "select * from tb_b_user where UserName=" + dbc.ToSqlValue(inuserzh);
                DataTable udt = dbc.ExecuteDataTable(str);

                if (udt.Rows.Count > 0)
                {
                    string str1 = "select id,carriagepayratio,carriageoil from tb_b_version";
                    DataTable vdt = dbc.ExecuteDataTable(str1);

                    int carriageoil = 0;
                    if (vdt.Rows.Count > 0)
                    {
                        if (vdt.Rows[0]["carriageoil"] != null && vdt.Rows[0]["carriageoil"].ToString() != "")
                        {
                            carriageoil = Convert.ToInt32(vdt.Rows[0]["carriageoil"]);
                        }
                    }

                    if (carriageoil <= Convert.ToInt32(money))
                    {
                        throw new Exception("划拨总油量不得超过其所有运费券！");
                    }
                    else
                    {
                        var oilcardcode = "CHB_" + GetStrAscii(6) + DateTime.Now.ToString("yyyyMMddHHmmssffff");
                        var oiltransfercode = GetStrAscii(6) + DateTime.Now.ToString("yyyyMMddHHmmssffff");

                        DataTable otdt = dbc.GetEmptyDataTable("tb_b_oil_transfer");
                        DataRow otdr = otdt.NewRow();
                        otdr["oiltransferid"] = Guid.NewGuid();
                        otdr["outuserid"] = "6E72B59D-BEC6-4835-A66F-8BC70BD82FE9";
                        otdr["inuserid"] = udt.Rows[0]["UserID"];
                        otdr["money"] = money;
                        otdr["oilcardcode"] = oilcardcode;
                        otdr["oiltransfercode"] = oiltransfercode;
                        otdr["status"] = 0;
                        otdr["addtime"] = DateTime.Now;
                        otdr["updatetime"] = DateTime.Now;
                        otdr["transfertype"] = Convert.ToInt32(jsr["transfertype"].ToString());
                        otdt.Rows.Add(otdr);
                        dbc.InsertTable(otdt);

                        DataTable modt = dbc.GetEmptyDataTable("tb_b_myoilcard");
                        DataRow modr = modt.NewRow();
                        modr["myoilcardId"] = Guid.NewGuid();
                        modr["oilmoney"] = money;
                        modr["UserID"] = udt.Rows[0]["UserID"];
                        modr["oilcardcode"] = oilcardcode;
                        modr["oiltransfercode"] = oiltransfercode;
                        modr["status"] = 0;
                        modr["addtime"] = DateTime.Now;
                        modr["updatetime"] = DateTime.Now;
                        modt.Rows.Add(modr);
                        dbc.InsertTable(modt);

                        DataTable vndt = dbc.GetEmptyDataTable("tb_b_version");
                        DataTableTracker vndtt = new DataTableTracker(vndt);
                        DataRow vndr = vndt.NewRow();
                        vndr["id"] = vdt.Rows[0]["id"];
                        vndr["carriageoil"] = carriageoil - money;
                        vndt.Rows.Add(vndr);
                        dbc.UpdateTable(vndt, vndtt);

                    }
                }
                else
                {
                    throw new Exception("转入人员不存在，请重新填写手机号！");
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

    private string GetStrAscii(int nLength)
    {
        int nStrLength = nLength;
        string strString = "1234567890";
        //string  strString   = "1234567890qwertyuioplkjhgfdsazxcvbnmASDFGHJKLMNBVCXZQWERTYUIOP";
        StringBuilder strtemp = new StringBuilder();
        Random random = new Random((int)DateTime.Now.Ticks);
        for (int i = 0; i < nStrLength; i++)
        {
            random = new Random(unchecked(random.Next() * 1000));
            strtemp.Append(strString[random.Next(10)]);
        }
        return strtemp.ToString();
    }

    [CSMethod("GetYKHBList")]
    public object GetYKHBList(int pagnum, int pagesize, string oilcardcode, string oiltransfercode, string yhzh, string zt, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(oilcardcode.Trim()))
                {
                    where += " and " + dbc.C_Like("a.oilcardcode", oilcardcode.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(oiltransfercode.Trim()))
                {
                    where += " and " + dbc.C_Like("a.oiltransfercode", oiltransfercode.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(yhzh.Trim()))
                {
                    where += " and " + dbc.C_Like("a.zrzh", yhzh.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(zt.Trim()))
                {
                    if (Convert.ToInt32(zt) == 0)
                    {//是
                        where += " and " + dbc.C_EQ("a.outuserid", "6E72B59D-BEC6-4835-A66F-8BC70BD82FE9");
                    }
                    else if (Convert.ToInt32(zt) == 1)
                    {
                        where += " and " + dbc.C_NEQ("a.outuserid", "6E72B59D-BEC6-4835-A66F-8BC70BD82FE9");
                    }
                }
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.addtime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.addtime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @" select * from (select a.addtime,a.oiltransfercode, a.oilcardcode,a.outuserid,a.money,'查货宝' as zcxm,'' as zczh,a.inuserid, b.UserXM as zrxm,b.UserName as zrzh 
  from tb_b_oil_transfer a left join tb_b_user b on a.inuserid=b.userid 
  where outuserid='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9' and status=0
  union all
  select a.addtime,a.oiltransfercode,a.oilcardcode,a.outuserid,a.money,c.UserXM as zcxm,c.UserName as zczh,a.inuserid, b.UserXM as zrxm,b.UserName as zrzh 
  from tb_b_oil_transfer a left join tb_b_user b on a.inuserid=b.userid 
  left join tb_b_user c on a.outuserid=c.UserID
  where outuserid<>'6E72B59D-BEC6-4835-A66F-8BC70BD82FE9' and status=0) a where 1=1
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


    [CSMethod("GetYKHBListToFile", 2)]
    public byte[] GetYKHBListToFile(string oilcardcode, string oiltransfercode, string yhzh, string zt, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                Workbook workbook = new Workbook(); //工作簿
                Worksheet sheet = workbook.Worksheets[0]; //工作表
                Cells cells = sheet.Cells;//单元格

                //为标题设置样式  
                Style styleTitle = workbook.Styles[workbook.Styles.Add()];
                styleTitle.HorizontalAlignment = TextAlignmentType.Center;//文字居中
                styleTitle.Font.Name = "宋体";//文字字体
                styleTitle.Font.Size = 18;//文字大小
                styleTitle.Font.IsBold = true;//粗体

                //样式1
                Style style1 = workbook.Styles[workbook.Styles.Add()];
                style1.HorizontalAlignment = TextAlignmentType.Center;//文字居中
                style1.Font.Name = "宋体";//文字字体
                style1.Font.Size = 12;//文字大小
                style1.Font.IsBold = true;//粗体

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
                cells[0, 0].PutValue("装出人员");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("转入人员账号");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("转入人员名称");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("转出油卡金额");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("油卡编号");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("油卡划拨编号");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);
                cells[0, 6].PutValue("时间");
                cells[0, 6].SetStyle(style2);
                cells.SetColumnWidth(6, 20);

                string where = "";
                if (!string.IsNullOrEmpty(oilcardcode.Trim()))
                {
                    where += " and " + dbc.C_Like("a.oilcardcode", oilcardcode.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(oiltransfercode.Trim()))
                {
                    where += " and " + dbc.C_Like("a.oiltransfercode", oiltransfercode.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(yhzh.Trim()))
                {
                    where += " and " + dbc.C_Like("a.zrzh", yhzh.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(zt.Trim()))
                {
                    if (Convert.ToInt32(zt) == 0)
                    {//是
                        where += " and " + dbc.C_EQ("a.outuserid", "6E72B59D-BEC6-4835-A66F-8BC70BD82FE9");
                    }
                    else if (Convert.ToInt32(zt) == 1)
                    {
                        where += " and " + dbc.C_NEQ("a.outuserid", "6E72B59D-BEC6-4835-A66F-8BC70BD82FE9");
                    }
                }
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.addtime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.addtime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @" select * from (select a.addtime,a.oiltransfercode, a.oilcardcode,a.outuserid,a.money,'查货宝' as zcxm,'' as zczh,a.inuserid, b.UserXM as zrxm,b.UserName as zrzh 
  from tb_b_oil_transfer a left join tb_b_user b on a.inuserid=b.userid 
  where outuserid='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9' and status=0
  union all
  select a.addtime,a.oiltransfercode,a.oilcardcode,a.outuserid,a.money,c.UserXM as zcxm,c.UserName as zczh,a.inuserid, b.UserXM as zrxm,b.UserName as zrzh 
  from tb_b_oil_transfer a left join tb_b_user b on a.inuserid=b.userid 
  left join tb_b_user c on a.outuserid=c.UserID
  where outuserid<>'6E72B59D-BEC6-4835-A66F-8BC70BD82FE9' and status=0) a where 1=1
                                 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = dbc.ExecuteDataTable(str + " order by a.addtime desc");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["zcxm"]);
                    cells[i + 1, 0].SetStyle(style4);
                    cells[i + 1, 1].PutValue(dt.Rows[i]["zrzh"]);
                    cells[i + 1, 1].SetStyle(style4);
                    cells[i + 1, 2].PutValue(dt.Rows[i]["zrxm"]);
                    cells[i + 1, 2].SetStyle(style4);
                    cells[i + 1, 3].PutValue(dt.Rows[i]["money"]);
                    cells[i + 1, 3].SetStyle(style4);
                    cells[i + 1, 4].PutValue(dt.Rows[i]["oilcardcode"]);
                    cells[i + 1, 4].SetStyle(style4);
                    cells[i + 1, 5].PutValue(dt.Rows[i]["oiltransfercode"]);
                    cells[i + 1, 5].SetStyle(style4);
                    if (dt.Rows[i]["addtime"] != null && dt.Rows[i]["addtime"].ToString() != "")
                    {
                        cells[i + 1, 6].PutValue(Convert.ToDateTime(dt.Rows[i]["addtime"]).ToString("yyyy-MM-dd HH:mm:ss"));
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

    #region 干线运输\油品销售
    [CSMethod("GetHBList")]
    public object GetHBList(int pagnum, int pagesize, string oilcardcode, string oiltransfercode, string yhzh, string beg, string end, int transfertype)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(oilcardcode.Trim()))
                {
                    where += " and " + dbc.C_Like("a.oilcardcode", oilcardcode.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(oiltransfercode.Trim()))
                {
                    where += " and " + dbc.C_Like("a.oiltransfercode", oiltransfercode.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(yhzh.Trim()))
                {
                    where += " and " + dbc.C_Like("a.zrzh", yhzh.Trim(), LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.addtime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.addtime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select a.addtime,a.oiltransfercode, a.oilcardcode,a.outuserid,a.money,'查货宝' as zcxm,a.inuserid, b.UserXM as zrxm,b.UserName as zrzh from tb_b_oil_transfer a 
                            left join tb_b_user b on a.inuserid=b.userid 
                            where outuserid='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9' and " + dbc.C_EQ("transfertype", transfertype) + " and status=0";
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

    [CSMethod("GetHBListToFile", 2)]
    public byte[] GetHBListToFile(string oilcardcode, string oiltransfercode, string yhzh, string beg, string end, int transfertype)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                Workbook workbook = new Workbook(); //工作簿
                Worksheet sheet = workbook.Worksheets[0]; //工作表
                Cells cells = sheet.Cells;//单元格

                //为标题设置样式  
                Style styleTitle = workbook.Styles[workbook.Styles.Add()];
                styleTitle.HorizontalAlignment = TextAlignmentType.Center;//文字居中
                styleTitle.Font.Name = "宋体";//文字字体
                styleTitle.Font.Size = 18;//文字大小
                styleTitle.Font.IsBold = true;//粗体

                //样式1
                Style style1 = workbook.Styles[workbook.Styles.Add()];
                style1.HorizontalAlignment = TextAlignmentType.Center;//文字居中
                style1.Font.Name = "宋体";//文字字体
                style1.Font.Size = 12;//文字大小
                style1.Font.IsBold = true;//粗体

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
                cells[0, 0].PutValue("转出人员");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("转入人员账号");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("转入人员名称");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("转出油卡金额");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("油卡编号");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("油卡划拨编号");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);
                cells[0, 6].PutValue("时间");
                cells[0, 6].SetStyle(style2);
                cells.SetColumnWidth(6, 20);

                string where = "";
                if (!string.IsNullOrEmpty(oilcardcode.Trim()))
                {
                    where += " and " + dbc.C_Like("a.oilcardcode", oilcardcode.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(oiltransfercode.Trim()))
                {
                    where += " and " + dbc.C_Like("a.oiltransfercode", oiltransfercode.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(yhzh.Trim()))
                {
                    where += " and " + dbc.C_Like("a.zrzh", yhzh.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.addtime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.addtime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select a.addtime,a.oiltransfercode, a.oilcardcode,a.outuserid,a.money,'查货宝' as zcxm,a.inuserid, b.UserXM as zrxm,b.UserName as zrzh from tb_b_oil_transfer a 
                            left join tb_b_user b on a.inuserid=b.userid 
                            where outuserid='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9' and " + dbc.C_EQ("transfertype", transfertype) + " and status=0";
                str += where;

                //开始取分页数据
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = dbc.ExecuteDataTable(str + " order by a.addtime desc");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["zcxm"]);
                    cells[i + 1, 0].SetStyle(style4);
                    cells[i + 1, 1].PutValue(dt.Rows[i]["zrzh"]);
                    cells[i + 1, 1].SetStyle(style4);
                    cells[i + 1, 2].PutValue(dt.Rows[i]["zrxm"]);
                    cells[i + 1, 2].SetStyle(style4);
                    cells[i + 1, 3].PutValue(dt.Rows[i]["money"]);
                    cells[i + 1, 3].SetStyle(style4);
                    cells[i + 1, 4].PutValue(dt.Rows[i]["oilcardcode"]);
                    cells[i + 1, 4].SetStyle(style4);
                    cells[i + 1, 5].PutValue(dt.Rows[i]["oiltransfercode"]);
                    cells[i + 1, 5].SetStyle(style4);
                    if (dt.Rows[i]["addtime"] != null && dt.Rows[i]["addtime"].ToString() != "")
                    {
                        cells[i + 1, 6].PutValue(Convert.ToDateTime(dt.Rows[i]["addtime"]).ToString("yyyy-MM-dd HH:mm:ss"));
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

    #region 干线运输统计
    [CSMethod("GetGxysTj")]
    public object GetGxysTj(string userxm, string dqmc, int transfertype)
    {
        using (DBConnection dbc = new DBConnection())
        {
            string sqlw = " where 1=1 ";
            List<string> wArr = new List<string>();
            if (!string.IsNullOrEmpty(userxm))
            {
                sqlw += " and " + dbc.C_Like("c.UserXM", userxm, LikeStyle.LeftAndRightLike);
            }
            if (!string.IsNullOrEmpty(dqmc))
            {
                sqlw += " and " + dbc.C_Like("c.dq_mc", dqmc, LikeStyle.LeftAndRightLike);
            }
            //专线
            var cmd = dbc.CreateCommand();
            cmd.CommandText = @"SELECT a.*,ISNULL(b.YK_XH,0)YK_XH,(ISNULL(a.YK_AMOUNT,0)-ISNULL(b.YK_XH,0))YK_SY,c.DqBm,c.UserXM FROM(
	                                SELECT UserID,ISNULL(SUM(oilmoney),0) YK_AMOUNT FROM tb_b_myoilcard where oiltransfercode in(
		                                select oiltransfercode from tb_b_oil_transfer where status=0 and transfertype=@transfertype and outuserid='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9'
	                                )
	                                GROUP BY UserID
                                )a
                                left join (
	                                select UserID,ISNULL(SUM(money),0) YK_XH from tb_b_oil_order where status=1 and cardNo in(
		                                SELECT oilcardcode FROM tb_b_myoilcard where oiltransfercode in(
			                                select oiltransfercode from tb_b_oil_transfer where status=0 and transfertype=@transfertype and outuserid='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9'
		                                )
	                                )
	                                GROUP BY UserID
                                )b on a.UserID=b.userid
                                inner join (
                                    select t1.*,t2.dq_mc from tb_b_user t1
                                    left join tb_b_dq t2 on t1.DqBm=t2.dq_bm 
                                    where t1.ClientKind=1 
                                ) c on a.UserID=c.UserID " + sqlw + @"
                                order by c.DqBm";
            cmd.Parameters.AddWithValue("@transfertype", transfertype);
            DataTable dt = dbc.ExecuteDataTable(cmd);

            //干线
            cmd.Parameters.Clear();
            cmd.CommandText = @"select ISNULL(SUM(money),0) from tb_b_oil_transfer where outuserid='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9' and transfertype=@transfertype";
            cmd.Parameters.AddWithValue("@transfertype", transfertype);
            decimal AmountYk = Convert.ToDecimal(dbc.ExecuteScalar(cmd));

            cmd.Parameters.Clear();
            cmd.CommandText = @"select ISNULL(SUM(money),0) YK_XH from tb_b_oil_order where status=1 and cardNo in(
	                                SELECT oilcardcode FROM tb_b_myoilcard where oiltransfercode in(
		                                select oiltransfercode from tb_b_oil_transfer where status=0 and transfertype=@transfertype and outuserid='6E72B59D-BEC6-4835-A66F-8BC70BD82FE9'
	                                )
                                )";
            cmd.Parameters.AddWithValue("@transfertype", transfertype);
            decimal XhYk = Convert.ToDecimal(dbc.ExecuteScalar(cmd));

            return new { dt = dt, gxye = AmountYk - XhYk };
        }
    }
    #endregion
}