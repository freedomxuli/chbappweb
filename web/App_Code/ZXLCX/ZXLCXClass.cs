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

/// <summary>
/// CYMagSf 的摘要说明
/// </summary>
[CSClass("ZXLCXClass")]
public class ZXLCXClass
{
    public ZXLCXClass()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }
    string ServiceURL = System.Configuration.ConfigurationManager.AppSettings["ZYServiceURL"].ToString();
    [CSMethod("GetSelList")]
    public object GetSelList(int pagnum, int pagesize, string cx_userid, string cx_typee, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(cx_userid.Trim()))
                {
                    where += " and " + dbc.C_Like("b.UserName", cx_userid.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(cx_typee.Trim()))
                {
                    where += " and " + dbc.C_EQ("a.type", cx_typee.Trim());
                }

                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.addtime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.addtime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select b.UserName,case when a.type=1 then '行驶证查询类型' when  a.type=0 then 'gps查询类型' end as typeName 
,a.addtime,a.carnumber from tb_b_zjxl_search a left join tb_b_user b on a.userid=b.UserID where 1=1
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
                if (!string.IsNullOrEmpty(jsr["cx_userid"]))
                {
                    wArr.Add(dbc.C_Like("b.UserName", jsr["cx_userid"], LikeStyle.LeftAndRightLike));
                }
                if (!string.IsNullOrEmpty(jsr["cx_typee"]))
                {
                    wArr.Add(dbc.C_Like("a.type", jsr["cx_typee"], LikeStyle.LeftAndRightLike));
                } 
                string sqlW = "";
                if (wArr.Count > 0)
                {
                    sqlW = " and " + string.Join(" and ", wArr);
                }


                string str = @"select b.UserName,case when a.type=1 then '行驶证查询类型' when  a.type=0 then 'gps查询类型' end as typeName 
,a.addtime,a.carnumber from tb_b_zjxl_search a left join tb_b_user b on a.userid=b.UserID where 1=1
                                 ";
            

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
                style4.Font.Color = System.Drawing.Color.Red;//文字大小

                style4.IsTextWrapped = true;//单元格内容自动换行
                style4.Font.IsBold = false;//粗体




                //第一行标题列表
                //合并单元格cells.Merge(1, 0, 3, 1) 参数1代表当前行，参数0代表当前行当前列即第一行第一列，参数3合并的行数，参数4合并的列数

                cells.Merge(0, 0, 1, 4);

                cells[0, 0].PutValue("中交兴路查询统计数据");
                cells[0, 0].SetStyle(style3);
                cells.SetRowHeight(0, 30);




                cells.SetColumnWidth(0, 30);
                cells[2, 0].PutValue("账号");
                cells[2, 0].SetStyle(style1);
                cells.SetColumnWidth(1, 30);
                cells[2, 1].PutValue("查询类型");
                cells[2, 1].SetStyle(style1);
                cells.SetColumnWidth(2, 30);
                cells[2, 2].PutValue("查询时间");
                cells[2, 2].SetStyle(style1);
                cells.SetColumnWidth(3, 30);
                cells[2, 3].PutValue("查询内容");
                cells[2, 3].SetStyle(style1);
              




                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        cells[i + 3, 0].PutValue(dt.Rows[i]["UserName"].ToString());
                        cells[i + 3, 0].SetStyle(style2);
                        cells[i + 3, 1].PutValue(dt.Rows[i]["typeName"].ToString());
                        cells[i + 3, 1].SetStyle(style2);
                        cells[i + 3, 2].PutValue(Convert.ToDateTime(dt.Rows[i]["addtime"].ToString()).ToString("yyyy-MM-dd"));
                        cells[i + 3, 2].SetStyle(style2);
                        cells[i + 3, 3].PutValue(dt.Rows[i]["carnumber"].ToString());
                        cells[i + 3, 3].SetStyle(style2);
                      

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


    [CSMethod("GetSelPSList")]
    public object GetSelPSList(int pagnum, int pagesize, string cx_userid, string beg, string end)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(cx_userid.Trim()))
                {
                    where += " and " + dbc.C_Like("b.UserName", cx_userid.Trim(), LikeStyle.LeftAndRightLike);
                }

             

                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.addtime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.addtime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }

                string str = @"select b.UserName,a.addtime,(select gpsnumber from tb_b_zjxl_user where userid=a.userid) memo,a.adduser from  tb_b_zjxl_add a left join tb_b_user b on a.userid=b.UserID  where 1=1
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



    //列表EXCEL退款审核数据导出EXCEL
    [CSMethod("GetSelPSOutList", 2)]
    public byte[] GetSelPSOutList(JSReader jsr)
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
                if (!string.IsNullOrEmpty(jsr["cx_userid"]))
                {
                    wArr.Add(dbc.C_Like("b.UserName", jsr["cx_userid"], LikeStyle.LeftAndRightLike));
                }
                if (!string.IsNullOrEmpty(jsr["cx_typee"]))
                {
                    wArr.Add(dbc.C_Like("a.type", jsr["cx_typee"], LikeStyle.LeftAndRightLike));
                }
                string sqlW = "";
                if (wArr.Count > 0)
                {
                    sqlW = " and " + string.Join(" and ", wArr);
                }


                string str = @"select b.UserName,a.addtime,(select gpsnumber from tb_b_zjxl_user where userid=a.userid) memo,a.adduser from  tb_b_zjxl_add a left join tb_b_user b on a.userid=b.UserID  where 1=1
                                 ";


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
                style4.Font.Color = System.Drawing.Color.Red;//文字大小

                style4.IsTextWrapped = true;//单元格内容自动换行
                style4.Font.IsBold = false;//粗体




                //第一行标题列表
                //合并单元格cells.Merge(1, 0, 3, 1) 参数1代表当前行，参数0代表当前行当前列即第一行第一列，参数3合并的行数，参数4合并的列数

                cells.Merge(0, 0, 1, 4);

                cells[0, 0].PutValue("中交兴路派送查询统计数据");
                cells[0, 0].SetStyle(style3);
                cells.SetRowHeight(0, 30);




                cells.SetColumnWidth(0, 30);
                cells[2, 0].PutValue("账号");
                cells[2, 0].SetStyle(style1);
                cells.SetColumnWidth(1, 30);
                cells[2, 1].PutValue("派送时间");
                cells[2, 1].SetStyle(style1);
                cells.SetColumnWidth(2, 30);
                cells[2, 2].PutValue("派送次数");
                cells[2, 2].SetStyle(style1);
                cells.SetColumnWidth(3, 30);
                cells[2, 3].PutValue("操作人员");
                cells[2, 3].SetStyle(style1);





                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                        cells[i + 3, 0].PutValue(dt.Rows[i]["UserName"].ToString());
                        cells[i + 3, 0].SetStyle(style2);
                        cells[i + 3, 1].PutValue(Convert.ToDateTime(dt.Rows[i]["addtime"].ToString()).ToString("yyyy-MM-dd"));
                        cells[i + 3, 1].SetStyle(style2);
                        cells[i + 3, 2].PutValue(dt.Rows[i]["memo"].ToString());
                        cells[i + 3, 2].SetStyle(style2);
                        cells[i + 3, 3].PutValue(dt.Rows[i]["adduser"].ToString());
                        cells[i + 3, 3].SetStyle(style2);


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



     [CSMethod("UploadSF", 1)]
    public object UploadSF(FileData[] fds)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                DataTable newdt = new DataTable();
                newdt.Columns.Add("UserID");
                newdt.Columns.Add("UserName");
                string str = "";
                if (fds[0].FileBytes.Length == 0)
                {
                    throw new Exception("你上传的文件可能已被打开，请关闭该文件！");
                }
                System.IO.MemoryStream ms = new System.IO.MemoryStream(fds[0].FileBytes);
                Workbook workbook = new Workbook(ms);
                Worksheet sheet = workbook.Worksheets[0];
                Cells cells = sheet.Cells;
                foreach (Cell cell in cells)
                {
                    if (cell.IsMerged == true)
                    {
                        Range range = cell.GetMergedRange();
                        cell.Value = cells[range.FirstRow, range.FirstColumn].Value;
                    }
                    else
                    {
                        cell.Value = cell.Value;
                    }
                }
                DataTable mydt = cells.ExportDataTableAsString(1, 0, cells.MaxRow+1, cells.MaxColumn + 1);

                List<DataRow> removelist = new List<DataRow>();
                for (int i = 0; i < mydt.Rows.Count; i++)
                {
                    bool IsNull = true;
                    for (int j = 0; j < mydt.Columns.Count; j++)
                    {
                        if (!string.IsNullOrEmpty(mydt.Rows[i][j].ToString().Trim()))
                        {
                            IsNull = false;
                        }
                    }
                    if (IsNull)
                    {
                        removelist.Add(mydt.Rows[i]);
                    }
                }
                for (int i = 0; i < removelist.Count; i++)
                {
                    mydt.Rows.Remove(removelist[i]);
                }

                string sql = "select * from tb_b_user where IsSHPass=1 ";
                DataTable sfdt = dbc.ExecuteDataTable(sql);

                for (int i = 0; i < mydt.Rows.Count; i++)
                {
                    DataRow[] drs = sfdt.Select("UserName=" + dbc.ToSqlValue(mydt.Rows[i][0]));
                    if (drs.Length > 0)
                    {
                        DataRow newdr = newdt.NewRow();
                        newdr["UserID"] = drs[0]["UserID"];
                        newdr["UserName"] = drs[0]["UserName"];
                        newdt.Rows.Add(newdr);
                    }
                    else
                    {
                        str += mydt.Rows[i][0] + ",";
                    }
                }
                if (!string.IsNullOrEmpty(str))
                {
                    str += "这些用户不存在！";
                }
                return new {dt=newdt,str=str };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

     [CSMethod("SavePS")]
     public void SavePS(string[] delIds, int mailBox)
     {
         using (DBConnection dbc = new DBConnection())
         {
             try
             {

                 var cmd = dbc.CreateCommand();
                 var CurrentUserId = SystemUser.CurrentUser.UserID;

                 if (delIds.Length > 0)
                 {

                     var saledt = dbc.GetEmptyDataTable("tb_b_zjxl_add");
                     var tb_b_zjxl_recorddt = dbc.GetEmptyDataTable("tb_b_zjxl_record");




                     foreach (string id in delIds)
                     {

                         string sql = "select UserID from tb_b_user where IsSHPass=1 and  UserName=" + dbc.ToSqlValue(id);
                         string UserID = dbc.ExecuteScalar(sql).ToString();

                         var addid = Guid.NewGuid().ToString();
                         var saledr = saledt.NewRow();
                         saledr["id"] = addid;
                         saledr["userid"] = UserID;
                         saledr["type"] = 0;
                         saledr["adduser"] = CurrentUserId;
                         saledr["status"] = 0;
                         saledr["addtime"] = DateTime.Now;
                         saledr["addnumber"] = mailBox;

                         saledt.Rows.Add(saledr);


                         var tb_b_zjxl_recorddr = tb_b_zjxl_recorddt.NewRow();
                         tb_b_zjxl_recorddr["id"] = Guid.NewGuid().ToString();
                         tb_b_zjxl_recorddr["userid"] = UserID;
                         tb_b_zjxl_recorddr["type"] = 0;
                         tb_b_zjxl_recorddr["gettype"] = 4;
                         tb_b_zjxl_recorddr["paisongdetailid"] = addid;

                         tb_b_zjxl_recorddr["adduser"] = CurrentUserId;
                         tb_b_zjxl_recorddr["status"] = 0;
                         tb_b_zjxl_recorddr["addtime"] = DateTime.Now;
                        
                         tb_b_zjxl_recorddt.Rows.Add(tb_b_zjxl_recorddr);



                         dbc.ExecuteNonQuery("update tb_b_zjxl_user set gpsnumber=gpsnumber+" + mailBox + " where userid=" + dbc.ToSqlValue(UserID));

                     }
                     dbc.InsertTable(saledt);
                     dbc.InsertTable(tb_b_zjxl_recorddt);
                 }
             }
             catch (Exception ex){
                 throw ex;
             }


         }
     }

}

