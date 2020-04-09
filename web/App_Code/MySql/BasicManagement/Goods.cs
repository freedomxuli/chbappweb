using Aspose.Cells;
using SmartFramework4v2.Data;
using SmartFramework4v2.Data.MySql;
using SmartFramework4v2.Web.Common.JSON;
using SmartFramework4v2.Web.WebExecutor;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

/// <summary>
/// Goods 的摘要说明
/// </summary>
[CSClass("Goods")]
public class Goods
{
    public Goods()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    #region 货品管理
    /// <summary>
    /// 获取货品类别
    /// </summary>
    /// <returns></returns>
    [CSMethod("GetGoodsType")]
    public DataTable GetGoodsType()
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            string sqlStr = "select id,name from tb_b_dictionary_detail where bm like '008%' order by code";
            DataTable dt = dbc.ExecuteDataTable(sqlStr);
            return dt;
        }

    }

    /// <summary>
    /// 分页获取货品数据集
    /// </summary>
    /// <param name="pagnum"></param>
    /// <param name="pagesize"></param>
    /// <param name="goodsTypeBm"></param>
    /// <param name="goodsName"></param>
    /// <returns></returns>
    [CSMethod("GetGoodsList")]
    public object GetGoodsList(int pagnum, int pagesize, string goodsTypeId, string goodsName)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(goodsTypeId.Trim()))
                {
                    where += " and " + dbc.C_EQ("a.goodstype", goodsTypeId.Trim());
                }

                if (!string.IsNullOrEmpty(goodsName.Trim()))
                {
                    where += " and " + dbc.C_Like("a.goodsname", goodsName.Trim(), LikeStyle.LeftAndRightLike);
                }

                string str = @"select a.*,b.id goodstypeid,b.name goodstypename from tb_b_goods a
                                left join tb_b_dictionary_detail b on a.goodstype=b.id where a.status=0 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by b.code asc", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// 保存货品
    /// </summary>
    /// <param name="jsr"></param>
    [CSMethod("SaveGoods")]
    public void SaveGoods(JSReader jsr)
    {
        if (string.IsNullOrEmpty(jsr["goodstypeid"].ToString()))
        {
            throw new Exception("货品类别不能为空");
        }
        if (jsr["goodsname"].IsNull || jsr["goodsname"].IsEmpty)
        {
            throw new Exception("货品名称不能为空");
        }

        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                if (string.IsNullOrEmpty(jsr["goodsid"].ToString()))
                {
                    var goodsid = Guid.NewGuid().ToString();
                    var dt = dbc.GetEmptyDataTable("tb_b_goods");
                    var dr = dt.NewRow();
                    dr["goodsid"] = goodsid;
                    dr["goodsname"] = jsr["goodsname"].ToString();
                    dr["goodstype"] = jsr["goodstypeid"].ToString();
                    dr["status"] = 0;
                    dr["adduser"] = SystemUser.CurrentUser.UserID;
                    dr["addtime"] = DateTime.Now;
                    dr["updateuser"] = SystemUser.CurrentUser.UserID;
                    dr["updatetime"] = DateTime.Now;
                    dt.Rows.Add(dr);
                    dbc.InsertTable(dt);
                }
                else
                {
                    var dt = dbc.GetEmptyDataTable("tb_b_goods");
                    var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                    var dr = dt.NewRow();
                    dr["goodsid"] = jsr["goodsid"].ToString();
                    dr["goodsname"] = jsr["goodsname"].ToString();
                    dr["goodstype"] = jsr["goodstypeid"].ToString();
                    dr["updateuser"] = SystemUser.CurrentUser.UserID;
                    dr["updatetime"] = DateTime.Now;
                    dt.Rows.Add(dr);
                    dbc.UpdateTable(dt, dtt);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// 获取货品详情
    /// </summary>
    /// <param name="goodsId"></param>
    /// <returns></returns>
    [CSMethod("GetGoodsById")]
    public DataTable GetGoodsById(string goodsId)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            string sqlStr = "select * from tb_b_goods where goodsid='" + goodsId + "'";
            return dbc.ExecuteDataTable(sqlStr);
        }
    }

    /// <summary>
    /// 删除货品
    /// </summary>
    /// <param name="jsr"></param>
    /// <returns></returns>
    [CSMethod("DelGoods")]
    public object DelGoods(string goodsId)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                string sqlStr = "update tb_b_goods set status=1 where goodsid=" + dbc.ToSqlValue(goodsId);
                dbc.ExecuteDataTable(sqlStr);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    #endregion

    #region 价格模型
    /// <summary>
    /// 货物分类列表
    /// </summary>
    /// <param name="pagnum"></param>
    /// <param name="pagesize"></param>
    /// <param name="goodstypename"></param>
    /// <returns></returns>
    [CSMethod("GetGoodsTypeList")]
    public object GetGoodsTypeList(int pagnum, int pagesize, string goodstypename)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(goodstypename.Trim()))
                {
                    where += " and " + dbc.C_Like("a.name", goodstypename.Trim(), LikeStyle.LeftAndRightLike);
                }

                string str = @"select a.id goodsTypeId,a.name goodsTypeName,a.code goodsTypeCode,(select count(*) from tb_b_goods b where a.id=b.goodstype) goodsNum from tb_b_dictionary_detail a 
where a.bm like '008%'";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.bm asc", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// 货物列表
    /// </summary>
    /// <param name="goodstype"></param>
    /// <returns></returns>
    [CSMethod("GetGoodsLine")]
    public DataTable GetGoodsLine(string goodstype, string goodsName)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                string where = "";
                if (!string.IsNullOrEmpty(goodsName))
                {
                    where += " and " + dbc.C_Like("a.goodsname", goodsName.Trim(), LikeStyle.LeftAndRightLike);
                }
                string sql = @"select a.*,(select count(1) from tb_b_pricemodel where goodsid = a.goodsid and status=0) priceNum,b.name goodsTypeName from tb_b_goods a 
left join tb_b_dictionary_detail b on a.goodstype=b.id
where a.status=0 and a.goodstype = " + dbc.ToSqlValue(goodstype) + where;
                DataTable dt = dbc.ExecuteDataTable(sql);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// 货物列表导出
    /// </summary>
    /// <param name="goodstype"></param>
    /// <param name="goodsName"></param>
    /// <returns></returns>
    [CSMethod("GetGoodsLineToFile", 2)]
    public byte[] GetGoodsLineToFile(string goodstype, string goodsName)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
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
                cells[0, 0].PutValue("货物分类");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("货物名称");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("价格种类数量");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);

                DataTable dt = GetGoodsLine(goodstype, goodsName);


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["goodsTypeName"]);
                    cells[i + 1, 0].SetStyle(style4);
                    cells[i + 1, 1].PutValue(dt.Rows[i]["goodsname"]);
                    cells[i + 1, 1].SetStyle(style4);
                    cells[i + 1, 2].PutValue(dt.Rows[i]["priceNum"]);
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

    /// <summary>
    /// 价格模型列表
    /// </summary>
    /// <param name="goodsId"></param>
    /// <param name="fromroute"></param>
    /// <param name="toroute"></param>
    /// <param name="statisticstype"></param>
    /// <returns></returns>
    [CSMethod("GetGoodsPriceLine")]
    public DataTable GetGoodsPriceLine(string goodsId, string fromroute, string toroute, string statisticstype)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                string where = "";
                if (!string.IsNullOrEmpty(fromroute))
                {
                    where += " and " + dbc.C_EQ("fromroutecode", fromroute);
                }
                if (!string.IsNullOrEmpty(toroute))
                {
                    where += " and " + dbc.C_EQ("toroutecode", toroute);
                }
                if (!string.IsNullOrEmpty(statisticstype))
                {
                    where += " and " + dbc.C_EQ("statisticstype", statisticstype);
                }
                string sql = @"select id,CASE statisticstype WHEN 1 THEN '零担' WHEN 2 THEN '整车' ELSE '' END transporttype,
                                fromroutecode,fromroutename,toroutecode,toroutename,price,pickprice,deliverprice,frompart,topart
                                from tb_b_pricemodel where goodsid=" + dbc.ToSqlValue(goodsId) + " and status=0 " + where + " order by updatetime desc";
                DataTable dt = dbc.ExecuteDataTable(sql);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetGoodsPriceLineToFile", 2)]
    public byte[] GetGoodsPriceLineToFile(string goodsId, string fromroute, string toroute, string statisticstype)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
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

                /*goodstypeid，goodsid，运输类型（只能是零担/整车），起始地（省市区三个列），目的地（省市区三个列），
                 * 范围起始值，范围结束值，车型（栏板车/厢车），车长（栏板车（4.2m,6.8m,9.6m,13m,17.5m）厢车（4.2m,6.8m,9.6m,17.5m）），单价，提货价，送货价*/
                cells.SetRowHeight(0, 20);
                cells[0, 0].PutValue("货物分类");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("货物名称");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("价格种类数量");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);

                DataTable dt = GetGoodsPriceLine(goodsId, fromroute, toroute, statisticstype);


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["goodsTypeName"]);
                    cells[i + 1, 0].SetStyle(style4);
                    cells[i + 1, 1].PutValue(dt.Rows[i]["goodsname"]);
                    cells[i + 1, 1].SetStyle(style4);
                    cells[i + 1, 2].PutValue(dt.Rows[i]["priceNum"]);
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

    /// <summary>
    /// 删除价格模型
    /// </summary>
    /// <param name="id"></param>
    [CSMethod("DelGoodsPrice")]
    public bool DelGoodsPrice(JSReader jsr)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                for (int i = 0; i < jsr.ToArray().Length; i++)
                {
                    string sqlStr = "update tb_b_pricemodel set status=1 where id=" + dbc.ToSqlValue(jsr.ToArray()[i].ToString());
                    dbc.ExecuteDataTable(sqlStr);
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

    /// <summary>
    /// 价格模型模板导出
    /// </summary>
    /// <param name="goodsid">货品ID</param>
    /// <returns></returns>
    [CSMethod("ExportPriceModelTemplate", 2)]
    public byte[] ExportPriceModelTemplate(string goodsid)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                Workbook workbook = new Workbook(System.Web.HttpContext.Current.Server.MapPath("~/Mb/价格模板.xls"));//工作簿
                Worksheet sheet = workbook.Worksheets[0]; //工作表
                Cells cells = sheet.Cells;//单元格
                string sqlstr = @"select a.goodstypeid,a.goodsid,a.statisticstype,b.sheng1,b.shi1,b.qu1,c.sheng2,c.shi2,c.qu2,a.frompart,a.topart,a.vehicletyperequirement,a.vehiclelengthrequirement,a.price,a.pickprice,a.deliverprice 
                                    from tb_b_pricemodel a
                                    left join(
	                                    select t1.`code`,t3.`NAME` sheng1,t2.`NAME` shi1,t1.`NAME` qu1 from tb_b_area t1,tb_b_area t2 ,tb_b_area t3
	                                    where t1.pcode=t2.`code` and t2.pcode=t3.`code`
                                    )b on a.fromroutecode=b.`code`
                                    left join(
	                                    select t1.`code`,t3.`NAME` sheng2,t2.`NAME` shi2,t1.`NAME` qu2 from tb_b_area t1,tb_b_area t2 ,tb_b_area t3
	                                    where t1.pcode=t2.`code` and t2.pcode=t3.`code`
                                    )c on a.toroutecode=c.`code` where goodsid=" + dbc.ToSqlValue(goodsid);
                DataTable dt = dbc.ExecuteDataTable(sqlstr);
                int excelEditRow = 2;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[excelEditRow + i, 0].PutValue(dt.Rows[i]["goodstypeid"].ToString());
                    cells[excelEditRow + i, 1].PutValue(dt.Rows[i]["goodsid"].ToString());
                    cells[excelEditRow + i, 2].PutValue((string.IsNullOrEmpty(dt.Rows[i]["statisticstype"].ToString())) ? "" : (dt.Rows[i]["statisticstype"].ToString() == "1" ? "零担" : "整车"));//运输类型（只能是零担/整车）
                    cells[excelEditRow + i, 3].PutValue(dt.Rows[i]["sheng1"].ToString());//起始地（省市区三个列）
                    cells[excelEditRow + i, 4].PutValue(dt.Rows[i]["shi1"].ToString());
                    cells[excelEditRow + i, 5].PutValue(dt.Rows[i]["qu1"].ToString());
                    cells[excelEditRow + i, 6].PutValue(dt.Rows[i]["sheng2"].ToString());//目的地（省市区三个列）
                    cells[excelEditRow + i, 7].PutValue(dt.Rows[i]["shi2"].ToString());
                    cells[excelEditRow + i, 8].PutValue(dt.Rows[i]["qu2"].ToString());
                    cells[excelEditRow + i, 9].PutValue(dt.Rows[i]["frompart"].ToString());//范围起始值，
                    cells[excelEditRow + i, 10].PutValue(dt.Rows[i]["topart"].ToString());//范围结束值
                    cells[excelEditRow + i, 11].PutValue((string.IsNullOrEmpty(dt.Rows[i]["vehicletyperequirement"].ToString())) ? "" : (dt.Rows[i]["vehicletyperequirement"].ToString() == "1" ? "栏板车" : "厢车"));//车型（栏板车/厢车）
                    cells[excelEditRow + i, 12].PutValue(dt.Rows[i]["vehiclelengthrequirement"].ToString());//车长（栏板车（4.2m,6.8m,9.6m,13m,17.5m）厢车（4.2m,6.8m,9.6m,17.5m））
                    cells[excelEditRow + i, 13].PutValue(dt.Rows[i]["price"].ToString());//单价
                    cells[excelEditRow + i, 14].PutValue(dt.Rows[i]["pickprice"].ToString());//提货价
                    cells[excelEditRow + i, 15].PutValue(dt.Rows[i]["deliverprice"].ToString());//送货价
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

    /// <summary>
    /// 价格模型模板导入
    /// </summary>
    /// <param name="fds"></param>
    /// <param name="goodsid">货品ID</param>
    /// <returns></returns>
    [CSMethod("UploadPriceModel", 1)]
    public object UploadPriceModel(FileData[] fds, string goodsid, string goodsname, string goodstypeid)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                dbc.BeginTransaction();
                string str = "";
                if (fds[0].FileBytes.Length == 0)
                {
                    throw new Exception("你上传的文件可能已被打开，请关闭该文件！");
                }
                System.IO.MemoryStream ms = new System.IO.MemoryStream(fds[0].FileBytes);
                Workbook workbook = new Workbook(ms);
                Worksheet sheet = workbook.Worksheets[0];
                Cells cells = sheet.Cells;
                //foreach (Cell cell in cells)
                //{
                //    if (cell.IsMerged == true)
                //    {
                //        Range range = cell.GetMergedRange();
                //        cell.Value = cells[range.FirstRow, range.FirstColumn].Value;
                //    }
                //    else
                //    {
                //        cell.Value = cell.Value;
                //    }
                //}
                DataTable mydt = cells.ExportDataTableAsString(2, 0, cells.MaxRow + 1, cells.MaxColumn + 1);
                //数据准备
                string sql = "select * from tb_b_area ";
                DataTable arasDt = dbc.ExecuteDataTable(sql);

                sql = "select * from tb_b_dictionary_detail";
                DataTable dicDt = dbc.ExecuteDataTable(sql);
                //遍历验证
                for (int i = 0; i < mydt.Rows.Count; i++)
                {
                    DataRow dr = mydt.Rows[i];
                    if (!string.IsNullOrEmpty(dr[2].ToString()))
                    {
                        string _goodstypeid = dr[0].ToString().Trim();
                        string _goodsid = dr[1].ToString().Trim();
                        string _statisticstype = (string.IsNullOrEmpty(dr[2].ToString().Trim())) ? "" : (dr[2].ToString().Trim() == "零担" ? "1" : "2");//运输类型（只能是零担/整车）
                        //= dr[3].ToString();//起始地（省市区三个列）
                        //= dr[4].ToString();
                        string _fromroutecode = "";
                        string _fromroutename = dr[5].ToString().Trim();
                        if (!string.IsNullOrEmpty(_fromroutename))
                        {
                            _fromroutecode = arasDt.Select("name='" + _fromroutename + "'")[0]["code"].ToString();
                        }
                        //= dr[6].ToString();//目的地（省市区三个列）
                        //= dr[7].ToString();
                        string _toroutecode = "";
                        string _toroutename = dr[8].ToString().Trim();
                        if (!string.IsNullOrEmpty(_toroutename))
                        {
                            _toroutecode = arasDt.Select("name='" + _toroutename + "'")[0]["code"].ToString();
                        }
                        string _frompart = dr[9].ToString().Trim();//范围起始值
                        string _topart = dr[10].ToString().Trim();//范围结束值
                        string _vehicletyperequirement = (string.IsNullOrEmpty(dr[11].ToString().Trim())) ? "" : (dr[11].ToString().Trim() == "栏板车" ? "1" : "2");//车型（栏板车/厢车）
                        string _vehiclelengthrequirement = "";//车长
                        DataRow[] ccDrs = dicDt.Select("name=" + dr[12].ToString().Trim());
                        if (ccDrs.Length > 0)
                        {
                            _vehiclelengthrequirement = ccDrs[0]["bm"].ToString();
                        }
                        string _price = dr[13].ToString().Trim();//单价
                        string _pickprice = dr[14].ToString().Trim();//提货价
                        string _deliverprice = dr[15].ToString().Trim();//送货价

                        #region 验证
                        switch (_statisticstype)
                        {
                            case "1"://如果运输类型为零担，起始地，目的地，范围起始值，范围结束值，单价，提货价，送货价为必填
                                if (string.IsNullOrEmpty(_fromroutename))
                                {
                                    str = "请填写起始地！";
                                }
                                if (string.IsNullOrEmpty(_toroutename))
                                {
                                    str = "请填写目的地！";
                                }
                                if (string.IsNullOrEmpty(_frompart))
                                {
                                    str = "请填写范围起始值！";
                                }
                                if (string.IsNullOrEmpty(_topart))
                                {
                                    str = "请填写范围结束值！";
                                }
                                if (string.IsNullOrEmpty(_price))
                                {
                                    str = "请填写单价！";
                                }
                                if (string.IsNullOrEmpty(_pickprice))
                                {
                                    str = "请填写提货价！";
                                }
                                if (string.IsNullOrEmpty(_deliverprice))
                                {
                                    str = "请填写送货价！";
                                }
                                break;
                            case "2"://如果运输类型为整车，起始地（省市区三个列），目的地（省市区三个列），车型，车长，单价，提货价，送货价为必填
                                if (string.IsNullOrEmpty(_fromroutename))
                                {
                                    str = "请填写起始地！";
                                }
                                if (string.IsNullOrEmpty(_toroutename))
                                {
                                    str = "请填写目的地！";
                                }
                                if (string.IsNullOrEmpty(_vehicletyperequirement))
                                {
                                    str = "请填写车型！";
                                }
                                if (string.IsNullOrEmpty(_vehiclelengthrequirement))
                                {
                                    str = "请填写车长！";
                                }
                                if (string.IsNullOrEmpty(_price))
                                {
                                    str = "请填写单价！";
                                }
                                if (string.IsNullOrEmpty(_pickprice))
                                {
                                    str = "请填写提货价！";
                                }
                                if (string.IsNullOrEmpty(_deliverprice))
                                {
                                    str = "请填写送货价！";
                                }
                                break;

                        }
                        #endregion

                        if (!string.IsNullOrEmpty(str))
                        {
                            throw new Exception("你上传的文件填写数据有误【" + (i + 3) + "行" + str + "】");
                        }

                        #region 数据添加或修改
                        /**导入时先根据goodstypeid，goodsid，运输类型，起始地，目的地，车型，车长搜索一下，
                         * 如果存在，则更新单价，提货价，送货价，
                         * 如果不存在，则插入新的数据，省市区要查询地区表，查询转换成code插入对应地区编号
                         */
                        string _id = Verify(_goodstypeid, _goodsid, _statisticstype, _fromroutecode, _toroutecode, _vehicletyperequirement, _vehiclelengthrequirement);
                        if (!string.IsNullOrEmpty(_id))
                        {
                            //这里还需判断导入表货物与表货物一致
                            if (_goodstypeid != goodstypeid || _goodsid != goodsid)
                            {
                                throw new Exception("你上传的文件填写数据有误【当前表数据与货物不一致】");
                            }
                            var dt = dbc.GetEmptyDataTable("tb_b_pricemodel");
                            var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                            var upDr = dt.NewRow();
                            upDr["id"] = _id;
                            upDr["goodsname"] = goodsname;
                            upDr["goodstypeid"] = _goodstypeid;
                            upDr["goodsid"] = _goodsid;
                            upDr["fromroutecode"] = _fromroutecode;
                            upDr["fromroutename"] = _fromroutename;
                            upDr["toroutecode"] = _toroutecode;
                            upDr["toroutename"] = _toroutename;
                            upDr["price"] = !string.IsNullOrEmpty(_price) ? Convert.ToDecimal(_price) : 0m;
                            upDr["pickprice"] = !string.IsNullOrEmpty(_pickprice) ? Convert.ToDecimal(_pickprice) : 0m;
                            upDr["deliverprice"] = !string.IsNullOrEmpty(_deliverprice) ? Convert.ToDecimal(_deliverprice) : 0m;
                            if (!string.IsNullOrEmpty(_frompart))
                            {
                                upDr["frompart"] = _frompart;
                            }
                            if (!string.IsNullOrEmpty(_topart))
                            {
                                upDr["topart"] = _topart;
                            }
                            upDr["status"] = 0;
                            upDr["updateuser"] = SystemUser.CurrentUser.UserID;
                            upDr["updatetime"] = DateTime.Now;
                            if (!string.IsNullOrEmpty(_statisticstype))
                            {
                                upDr["statisticstype"] = Convert.ToInt32(_statisticstype);
                            }
                            upDr["vehicletyperequirement"] = _vehicletyperequirement;
                            upDr["vehiclelengthrequirement"] = _vehiclelengthrequirement;
                            dt.Rows.Add(upDr);
                            dbc.UpdateTable(dt, dtt);
                        }
                        else
                        {
                            var id = Guid.NewGuid().ToString();
                            var dt = dbc.GetEmptyDataTable("tb_b_pricemodel");
                            var inDr = dt.NewRow();
                            inDr["id"] = id;
                            inDr["goodsname"] = goodsname;
                            inDr["goodstypeid"] = goodstypeid;
                            inDr["goodsid"] = goodsid;
                            inDr["fromroutecode"] = _fromroutecode;
                            inDr["fromroutename"] = _fromroutename;
                            inDr["toroutecode"] = _toroutecode;
                            inDr["toroutename"] = _toroutename;
                            inDr["price"] = !string.IsNullOrEmpty(_price) ? Convert.ToDecimal(_price) : 0m;
                            inDr["pickprice"] = !string.IsNullOrEmpty(_pickprice) ? Convert.ToDecimal(_pickprice) : 0m;
                            inDr["deliverprice"] = !string.IsNullOrEmpty(_deliverprice) ? Convert.ToDecimal(_deliverprice) : 0m;
                            if (!string.IsNullOrEmpty(_frompart))
                            {
                                inDr["frompart"] = _frompart;
                            }
                            if (!string.IsNullOrEmpty(_topart))
                            {
                                inDr["topart"] = _topart;
                            }
                            inDr["status"] = 0;
                            inDr["adduser"] = SystemUser.CurrentUser.UserID;
                            inDr["addtime"] = DateTime.Now;
                            inDr["updateuser"] = SystemUser.CurrentUser.UserID;
                            inDr["updatetime"] = DateTime.Now;
                            if (!string.IsNullOrEmpty(_statisticstype))
                            {
                                inDr["statisticstype"] = Convert.ToInt32(_statisticstype);
                            }
                            inDr["vehicletyperequirement"] = _vehicletyperequirement;
                            inDr["vehiclelengthrequirement"] = _vehiclelengthrequirement;

                            dt.Rows.Add(inDr);
                            dbc.InsertTable(dt);
                        }
                        #endregion
                    }
                }

                sql = @"select id,CASE statisticstype WHEN 1 THEN '零担' WHEN 2 THEN '整车' ELSE '' END transporttype,
                        fromroutecode,fromroutename,toroutecode,toroutename,price,pickprice,deliverprice,frompart,topart
                        from tb_b_pricemodel 
                        where goodsid=" + dbc.ToSqlValue(goodsid) + " and status=0 order by updatetime desc";
                DataTable retDt = dbc.ExecuteDataTable(sql);

                dbc.CommitTransaction();
                return new { dt = retDt, str = str };
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }

    private string Verify(string _goodstypeid, string _goodsid, string _statisticstype, string _fromroutecode, string _toroutecode, string _vehicletyperequirement, string _vehiclelengthrequirement)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            string id = "";
            string sql = @"select * from tb_b_pricemodel where status=0 and goodstypeid = " + dbc.ToSqlValue(_goodstypeid)
                      + " and goodsid=" + dbc.ToSqlValue(_goodsid)
                      + " and statisticstype=" + dbc.ToSqlValue(_statisticstype)
                      + " and fromroutecode=" + dbc.ToSqlValue(_fromroutecode)
                      + " and toroutecode=" + dbc.ToSqlValue(_toroutecode)
                      + " and vehicletyperequirement=" + dbc.ToSqlValue(_vehicletyperequirement)
                      + " and vehiclelengthrequirement= " + dbc.ToSqlValue(_vehiclelengthrequirement);
            DataTable dt = dbc.ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                id = dt.Rows[0]["id"].ToString();
            }
            return id;
        }
    }
    #endregion

    /// <summary>
    /// 根据省份code获取子市
    /// </summary>
    /// <returns></returns>
    [CSMethod("GetAreaList")]
    public DataTable GetAreaList(string pcode)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                string sql = @"SELECT * FROM tb_b_area WHERE pcode=" + dbc.ToSqlValue(pcode) + " order by `code`";
                DataTable dt = dbc.ExecuteDataTable(sql);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}