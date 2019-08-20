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
/// YKMag 的摘要说明
/// </summary>
[CSClass("CYMag")]
public class CYMag
{
    public CYMag()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }
    string ServiceURL = System.Configuration.ConfigurationManager.AppSettings["ZYServiceURL"].ToString();
    [CSMethod("GetCYDList")]
    public object GetCYDList(int pagnum, int pagesize, string carriagecode, string UserXM, string beg, string end, string isinvoice)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(carriagecode.Trim()))
                {
                    where += " and " + dbc.C_Like("a.carriagecode", carriagecode.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(UserXM.Trim()))
                {
                    where += " and " + dbc.C_Like("c.UserXM", UserXM.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.carriagetime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.carriagetime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(isinvoice))
                {
                    where += " and " + dbc.C_EQ("a.isinvoice", isinvoice);
                }
                string str = @"select a.*,b.UserName as sjzh,b.UserTel as sjdh,c.UserXM as zx,d.driverxm as sjxm,d.carnumber as sjcarnumber,d.caruser,e.name as kp,
                                case a.carriagestatus
                                when 10 then 1
                                when 50 then 2
                                when 40 then 3
                                when 30 then 4
                                when 0 then 5
                                when 11 then 6
                                when 20 then 7
                                when 21 then 8
                                else 9 end as px,c.modetype,c.modecoefficient,c.carriagegetmode 
                              from tb_b_carriage a 
                            left join tb_b_user b on a.driverid=b.UserID 
                            inner join tb_b_user c on a.userid=c.UserID and c.ClientKind=1
                            left join  tb_b_car d on a.carid=d.id
                            left join tb_b_carriagechb e on c.carriagechbid=e.id
                            where a.status=0 and 1=1 
                                 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by px asc,a.carriagetime desc", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetCYDListToFile", 2)]
    public byte[] GetCYDListToFile(string carriagecode, string UserXM, string beg, string end, string iskp)
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
                cells[0, 0].PutValue("上游客户（专线名称）");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("运输日期");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("起运地");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("目的地");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("支付券额/运费");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("下游司机");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);
                cells[0, 6].PutValue("司机姓名");
                cells[0, 6].SetStyle(style2);
                cells.SetColumnWidth(6, 20);
                cells[0, 7].PutValue("电话");
                cells[0, 7].SetStyle(style2);
                cells.SetColumnWidth(7, 20);
                cells[0, 8].PutValue("车牌");
                cells[0, 8].SetStyle(style2);
                cells.SetColumnWidth(8, 20);
                cells[0, 9].PutValue("油卡");
                cells[0, 9].SetStyle(style2);
                cells.SetColumnWidth(9, 20);

                cells[0, 10].PutValue("现付现金");
                cells[0, 10].SetStyle(style2);
                cells.SetColumnWidth(10, 20);

                cells[0, 11].PutValue("验收付现金");
                cells[0, 11].SetStyle(style2);
                cells.SetColumnWidth(11, 20);

                cells[0, 12].PutValue("保费");
                cells[0, 12].SetStyle(style2);
                cells.SetColumnWidth(12, 20);
                cells[0, 13].PutValue("是否油卡付款");
                cells[0, 13].SetStyle(style2);
                cells.SetColumnWidth(13, 20);
                cells[0, 14].PutValue("是否现金付款");
                cells[0, 14].SetStyle(style2);
                cells.SetColumnWidth(14, 20);
                cells[0, 15].PutValue("订单状态");
                cells[0, 15].SetStyle(style2);
                cells.SetColumnWidth(15, 20);
                cells[0, 16].PutValue("是否开票");
                cells[0, 16].SetStyle(style2);
                cells.SetColumnWidth(16, 20);

                cells[0, 17].PutValue("车主账号");
                cells[0, 17].SetStyle(style2);
                cells.SetColumnWidth(17, 20);

                cells[0, 18].PutValue("承运单号");
                cells[0, 18].SetStyle(style2);
                cells.SetColumnWidth(18, 20);
                cells[0, 19].PutValue("开票抬头");
                cells[0, 19].SetStyle(style2);
                cells.SetColumnWidth(19, 20);



                string where = "";
                if (!string.IsNullOrEmpty(carriagecode.Trim()))
                {
                    where += " and " + dbc.C_Like("a.carriagecode", carriagecode.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(UserXM.Trim()))
                {
                    where += " and " + dbc.C_Like("c.UserXM", UserXM.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.carriagetime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.carriagetime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(iskp))
                {
                    where += " and " + dbc.C_EQ("a.isinvoice", iskp);
                }
                string str = @"select a.*,b.UserName as sjzh,b.carnumber as sjcarnumber,b.UserXM as sjxm,b.UserTel as sjdh,c.UserXM as zx,b.caruser,e.name as kp
                              from tb_b_carriage a left join tb_b_user b on a.driverid=b.UserID
                            left join tb_b_user c on a.userid=c.UserID 
                            left join tb_b_carriagechb e on c.carriagechbid=e.id
                            where a.status=0 and  1=1 
                                 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = dbc.ExecuteDataTable(str + " order by a.carriagetime desc");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["zx"] != null && dt.Rows[i]["zx"].ToString() != "")
                    {
                        cells[i + 1, 0].PutValue(dt.Rows[i]["zx"]);
                    }
                    cells[i + 1, 0].SetStyle(style4);
                    if (dt.Rows[i]["carriagetime"] != null && dt.Rows[i]["carriagetime"].ToString() != "")
                    {
                        cells[i + 1, 1].PutValue(Convert.ToDateTime(dt.Rows[i]["carriagetime"]).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    cells[i + 1, 1].SetStyle(style4);
                    var qyd = "";
                    if (dt.Rows[i]["carriagefromprovince"] != null && dt.Rows[i]["carriagefromprovince"].ToString() != "")
                    {
                        qyd += dt.Rows[i]["carriagefromprovince"].ToString();
                        if (dt.Rows[i]["carriagefromcity"] != null && dt.Rows[i]["carriagefromcity"].ToString() != "")
                        {
                            qyd += dt.Rows[i]["carriagefromcity"].ToString();
                        }
                    }
                    cells[i + 1, 2].PutValue(qyd);
                    cells[i + 1, 2].SetStyle(style4);
                    var mdd = "";
                    if (dt.Rows[i]["carriagetoprovince"] != null && dt.Rows[i]["carriagetoprovince"].ToString() != "")
                    {
                        mdd += dt.Rows[i]["carriagetoprovince"].ToString();
                        if (dt.Rows[i]["carriagetocity"] != null && dt.Rows[i]["carriagetocity"].ToString() != "")
                        {
                            mdd += dt.Rows[i]["carriagetocity"].ToString();
                        }
                    }
                    cells[i + 1, 3].PutValue(mdd);
                    cells[i + 1, 3].SetStyle(style4);

                    if (dt.Rows[i]["carriagepoints"] != null && dt.Rows[i]["carriagepoints"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(dt.Rows[i]["carriagepoints"]);
                    }
                    cells[i + 1, 4].SetStyle(style4);
                    if (dt.Rows[i]["sjzh"] != null && dt.Rows[i]["sjzh"].ToString() != "")
                    {
                        cells[i + 1, 5].PutValue(dt.Rows[i]["sjzh"]);
                    }
                    cells[i + 1, 5].SetStyle(style4);
                    if (dt.Rows[i]["sjxm"] != null && dt.Rows[i]["sjxm"].ToString() != "")
                    {
                        cells[i + 1, 6].PutValue(dt.Rows[i]["sjxm"]);
                    }
                    cells[i + 1, 6].SetStyle(style4);
                    if (dt.Rows[i]["sjdh"] != null && dt.Rows[i]["sjdh"].ToString() != "")
                    {
                        cells[i + 1, 7].PutValue(dt.Rows[i]["sjdh"]);
                    }
                    cells[i + 1, 7].SetStyle(style4);
                    if (dt.Rows[i]["sjcarnumber"] != null && dt.Rows[i]["sjcarnumber"].ToString() != "")
                    {
                        cells[i + 1, 8].PutValue(dt.Rows[i]["sjcarnumber"]);
                    }
                    cells[i + 1, 8].SetStyle(style4);
                    if (dt.Rows[i]["carriageoilmoney"] != null && dt.Rows[i]["carriageoilmoney"].ToString() != "")
                    {
                        cells[i + 1, 9].PutValue(dt.Rows[i]["carriageoilmoney"]);
                    }
                    cells[i + 1, 9].SetStyle(style4);
                    if (dt.Rows[i]["carriagemoney"] != null && dt.Rows[i]["carriagemoney"].ToString() != "")
                    {
                        cells[i + 1, 10].PutValue(dt.Rows[i]["carriagemoney"]);
                    }
                    cells[i + 1, 10].SetStyle(style4);

                    if (dt.Rows[i]["carriagemoneynew"] != null && dt.Rows[i]["carriagemoneynew"].ToString() != "")
                    {
                        cells[i + 1, 11].PutValue(dt.Rows[i]["carriagemoneynew"]);
                    }
                    cells[i + 1, 11].SetStyle(style4);

                    if (dt.Rows[i]["insurancemoney"] != null && dt.Rows[i]["insurancemoney"].ToString() != "")
                    {
                        cells[i + 1, 12].PutValue(Math.Round(Convert.ToDecimal(dt.Rows[i]["insurancemoney"]) / 100, 2));
                    }
                    cells[i + 1, 12].SetStyle(style4);

                    var isoilpay = "";
                    if (dt.Rows[i]["isoilpay"] != null && dt.Rows[i]["isoilpay"].ToString() != "")
                    {
                        if (Convert.ToInt32(dt.Rows[i]["isoilpay"]) == 0)
                        {
                            isoilpay = "否";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["isoilpay"]) == 1)
                        {
                            isoilpay = "是";
                        }
                    }
                    cells[i + 1, 13].PutValue(isoilpay);
                    cells[i + 1, 13].SetStyle(style4);
                    var ismoneypay = "";
                    if (dt.Rows[i]["ismoneypay"] != null && dt.Rows[i]["ismoneypay"].ToString() != "")
                    {
                        if (Convert.ToInt32(dt.Rows[i]["ismoneypay"]) == 0)
                        {
                            ismoneypay = "否";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["ismoneypay"]) == 1)
                        {
                            ismoneypay = "是";
                        }
                    }
                    cells[i + 1, 14].PutValue(ismoneypay);
                    cells[i + 1, 14].SetStyle(style4);

                    string carriagestatus = "";
                    if (dt.Rows[i]["carriagestatus"] != null && dt.Rows[i]["carriagestatus"].ToString() != "")
                    {
                        if (Convert.ToInt32(dt.Rows[i]["carriagestatus"]) == 0)
                        {
                            carriagestatus = "已申请待后台审核";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["carriagestatus"]) == 10)
                        {
                            carriagestatus = "司机确认待后台审核";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["carriagestatus"]) == 11)
                        {
                            carriagestatus = "司机拒绝待后台审核";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["carriagestatus"]) == 20)
                        {
                            carriagestatus = "后台已审核待专线付款";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["carriagestatus"]) == 21)
                        {
                            carriagestatus = "后台拒绝申请";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["carriagestatus"]) == 30)
                        {
                            carriagestatus = "专线支付券额运输开始";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["carriagestatus"]) == 40)
                        {
                            carriagestatus = "司机确认到货待专线确认";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["carriagestatus"]) == 50)
                        {
                            carriagestatus = "专线确认到货待后台结款";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["carriagestatus"]) == 90)
                        {
                            carriagestatus = "后台确认结款，承运完成";
                        }

                        cells[i + 1, 15].PutValue(carriagestatus);
                        cells[i + 1, 15].SetStyle(style4);
                    }

                    var isinvoice = "";
                    if (dt.Rows[i]["isinvoice"] != null && dt.Rows[i]["isinvoice"].ToString() != "")
                    {
                        if (Convert.ToInt32(dt.Rows[i]["isinvoice"]) == 0)
                        {
                            isinvoice = "未开";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["isinvoice"]) == 1)
                        {
                            isinvoice = "已开";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["isinvoice"]) == 2)
                        {
                            isinvoice = "已收票";
                        }
                    }
                    else
                    {
                        isinvoice = "未开";
                    }
                    cells[i + 1, 16].PutValue(isinvoice);
                    cells[i + 1, 16].SetStyle(style4);


                    if (dt.Rows[i]["caruser"] != null && dt.Rows[i]["caruser"].ToString() != "")
                    {
                        cells[i + 1, 17].PutValue(dt.Rows[i]["caruser"].ToString());
                    }

                    if (dt.Rows[i]["carriagecode"] != null && dt.Rows[i]["carriagecode"].ToString() != "")
                    {
                        cells[i + 1, 18].PutValue(dt.Rows[i]["carriagecode"]);

                    }
                    cells[i + 1, 18].SetStyle(style4);
                    if (dt.Rows[i]["kp"] != null && dt.Rows[i]["kp"].ToString() != "")
                    {
                        cells[i + 1, 19].PutValue(dt.Rows[i]["kp"]);

                    }
                    cells[i + 1, 19].SetStyle(style4);
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
    /// 开票\收票
    /// </summary>
    /// <param name="jsr"></param>
    [CSMethod("Cykp")]
    public void Cykp(JSReader jsr,int zt)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                DataTable dt = dbc.GetEmptyDataTable("tb_b_carriage");
                DataTableTracker dtt = new DataTableTracker(dt);
                for (int i = 0; i < jsr.ToArray().Length; i++)
                {
                    string id = jsr.ToArray()[i]["carriageid"].ToString();

                    DataRow dr = dt.NewRow();
                    dr["carriageid"] = id;
                    dr["isinvoice"] = zt;
                    dt.Rows.Add(dr);
                }
                dbc.UpdateTable(dt, dtt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("QR")]
    public object QR(string carriageid, int carriagestatus)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                string str = "select * from tb_b_carriage where status=0 and carriageid=" + dbc.ToSqlValue(carriageid);
                DataTable dt = dbc.ExecuteDataTable(str);

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["carriagestatus"] != null && dt.Rows[0]["carriagestatus"].ToString() != "")
                    {
                        if (carriagestatus == 10 && Convert.ToInt32(dt.Rows[0]["carriagestatus"].ToString()) == 10)
                        {
                            DataTable odt = dbc.GetEmptyDataTable("tb_b_carriage");
                            DataTableTracker odtt = new DataTableTracker(odt);
                            DataRow odr = odt.NewRow();
                            odr["carriageid"] = carriageid;
                            odr["carriagestatus"] = 20;
                            odt.Rows.Add(odr);
                            dbc.UpdateTable(odt, odtt);

                            DataTable ofdt = dbc.GetEmptyDataTable("tb_b_carriage_flow");
                            DataRow ofdr = ofdt.NewRow();
                            ofdr["carriageflowid"] = Guid.NewGuid().ToString();
                            ofdr["carriageid"] = carriageid;
                            ofdr["carriagestatus"] = 20;
                            ofdr["status"] = 0;
                            ofdr["adduser"] = SystemUser.CurrentUser.UserID;
                            ofdr["addtime"] = DateTime.Now;
                            ofdt.Rows.Add(ofdr);
                            dbc.InsertTable(ofdt);

                            dbc.CommitTransaction();

                            var request2 = (HttpWebRequest)WebRequest.Create(ServiceURL + "sendSms/pass/tozx");
                            request2.Method = "POST";
                            request2.ContentType = "application/json;charset=UTF-8";
                            var byteData2 = Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(new
                            {
                                userid = dt.Rows[0]["userid"]
                            }));
                            var length2 = byteData2.Length;
                            request2.ContentLength = length2;
                            var writer2 = request2.GetRequestStream();
                            writer2.Write(byteData2, 0, length2);
                            writer2.Close();

                            var request1 = (HttpWebRequest)WebRequest.Create(ServiceURL + "sendSms/pass/todriver");
                            request1.Method = "POST";
                            request1.ContentType = "application/json;charset=UTF-8";
                            var byteData1 = Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(new
                            {
                                carriageid = carriageid
                            }));
                            var length1 = byteData1.Length;
                            request1.ContentLength = length1;
                            var writer1 = request1.GetRequestStream();
                            writer1.Write(byteData1, 0, length1);
                            writer1.Close();

                            var request3 = (HttpWebRequest)WebRequest.Create(ServiceURL + "sendSms/verify/tocaruser");
                            request3.Method = "POST";
                            request3.ContentType = "application/json;charset=UTF-8";
                            var byteData3 = Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(new
                            {
                                carriageid = carriageid
                            }));
                            var length3 = byteData3.Length;
                            request3.ContentLength = length3;
                            var writer3 = request3.GetRequestStream();
                            writer3.Write(byteData3, 0, length3);
                            writer3.Close();
                            return true;
                        }
                        else
                        {
                            throw new Exception("无法操作该订单！");
                        }
                    }
                    else
                    {
                        throw new Exception("无法操作该订单！");
                    }

                }
                else
                {
                    throw new Exception("该订单不存在！");
                }
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }

    [CSMethod("QRTH")]
    public object QRTH(string carriageid, int carriagestatus)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                string str = "select * from tb_b_carriage where status=0 and carriageid=" + dbc.ToSqlValue(carriageid);
                DataTable dt = dbc.ExecuteDataTable(str);

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["carriagestatus"] != null && dt.Rows[0]["carriagestatus"].ToString() != "")
                    {
                        if (carriagestatus == 10 && Convert.ToInt32(dt.Rows[0]["carriagestatus"].ToString()) == 20)
                        {
                            DataTable odt = dbc.GetEmptyDataTable("tb_b_carriage");
                            DataTableTracker odtt = new DataTableTracker(odt);
                            DataRow odr = odt.NewRow();
                            odr["carriageid"] = carriageid;
                            odr["carriagestatus"] = 10;
                            odt.Rows.Add(odr);
                            dbc.UpdateTable(odt, odtt);

                            DataTable ofdt = dbc.GetEmptyDataTable("tb_b_carriage_flow");
                            DataRow ofdr = ofdt.NewRow();
                            ofdr["carriageflowid"] = Guid.NewGuid().ToString();
                            ofdr["carriageid"] = carriageid;
                            ofdr["carriagestatus"] = 10;
                            ofdr["status"] = 0;
                            ofdr["adduser"] = SystemUser.CurrentUser.UserID;
                            ofdr["addtime"] = DateTime.Now;
                            ofdt.Rows.Add(ofdr);
                            dbc.InsertTable(ofdt);

                            dbc.CommitTransaction();

                            return true;
                        }
                        else
                        {
                            throw new Exception("无法操作该订单！");
                        }
                    }
                    else
                    {
                        throw new Exception("无法操作该订单！");
                    }

                }
                else
                {
                    throw new Exception("该订单不存在！");
                }
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }

    [CSMethod("QRDD")]
    public object QRDD(string carriageid, int carriagestatus)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                string str = "select * from tb_b_carriage where status=0 and carriageid=" + dbc.ToSqlValue(carriageid);
                DataTable dt = dbc.ExecuteDataTable(str);

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["carriagestatus"] != null && dt.Rows[0]["carriagestatus"].ToString() != "")
                    {
                        if ((Convert.ToInt32(dt.Rows[0]["isarrive"]) == 0) && (Convert.ToInt32(dt.Rows[0]["carriagestatus"]) >= 30))
                        {
                            DataTable odt = dbc.GetEmptyDataTable("tb_b_carriage");
                            DataTableTracker odtt = new DataTableTracker(odt);
                            DataRow odr = odt.NewRow();
                            odr["carriageid"] = carriageid;
                            odr["isarrive"] = 1;
                            odt.Rows.Add(odr);
                            dbc.UpdateTable(odt, odtt);

                            //DataTable ofdt = dbc.GetEmptyDataTable("tb_b_carriage_flow");
                            //DataRow ofdr = ofdt.NewRow();
                            //ofdr["carriageflowid"] = Guid.NewGuid().ToString();
                            //ofdr["carriageid"] = carriageid;
                            //ofdr["carriagestatus"] = 20;
                            //ofdr["status"] = 0;
                            //ofdr["adduser"] = SystemUser.CurrentUser.UserID;
                            //ofdr["addtime"] = DateTime.Now;
                            //ofdt.Rows.Add(ofdr);
                            //dbc.InsertTable(ofdt);

                            dbc.CommitTransaction();

                            return true;
                        }
                        else
                        {
                            throw new Exception("无法操作该订单！");
                        }
                    }
                    else
                    {
                        throw new Exception("无法操作该订单！");
                    }

                }
                else
                {
                    throw new Exception("该订单不存在！");
                }
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }

    [CSMethod("JJ")]
    public object JJ(string carriageid, int carriagestatus, string thyj)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                if (carriagestatus == 0 || carriagestatus == 10 || carriagestatus == 11)
                {
                    DataTable odt = dbc.GetEmptyDataTable("tb_b_carriage");
                    DataTableTracker odtt = new DataTableTracker(odt);
                    DataRow odr = odt.NewRow();
                    odr["carriageid"] = carriageid;
                    odr["carriagestatus"] = 21;
                    odt.Rows.Add(odr);
                    dbc.UpdateTable(odt, odtt);

                    DataTable ofdt = dbc.GetEmptyDataTable("tb_b_carriage_flow");
                    DataRow ofdr = ofdt.NewRow();
                    ofdr["carriageflowid"] = Guid.NewGuid().ToString();
                    ofdr["carriageid"] = carriageid;
                    ofdr["carriagestatus"] = 21;
                    ofdr["status"] = 0;
                    ofdr["adduser"] = SystemUser.CurrentUser.UserID;
                    ofdr["addtime"] = DateTime.Now;
                    ofdr["memo"] = thyj;
                    ofdt.Rows.Add(ofdr);
                    dbc.InsertTable(ofdt);
                }

                dbc.CommitTransaction();

                string str = "select * from tb_b_carriage where status=0 and carriageid=" + dbc.ToSqlValue(carriageid);
                DataTable dt = dbc.ExecuteDataTable(str);

                if (dt.Rows.Count > 0)
                {
                    var request = (HttpWebRequest)WebRequest.Create(ServiceURL + "sendSms/reject/tozx");
                    request.Method = "POST";
                    request.ContentType = "application/json;charset=UTF-8";
                    var byteData = Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(new
                    {
                        id = dt.Rows[0]["userid"]
                    }));
                    var length = byteData.Length;
                    request.ContentLength = length;
                    var writer = request.GetRequestStream();
                    writer.Write(byteData, 0, length);
                    writer.Close();

                    var request1 = (HttpWebRequest)WebRequest.Create(ServiceURL + "sendSms/reject/todriver");
                    request1.Method = "POST";
                    request1.ContentType = "application/json;charset=UTF-8";
                    var byteData1 = Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(new
                    {
                        carriageid = carriageid
                    }));
                    var length1 = byteData1.Length;
                    request1.ContentLength = length1;
                    var writer1 = request1.GetRequestStream();
                    writer1.Write(byteData1, 0, length1);
                    writer1.Close();
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

    [CSMethod("TH")]
    public object TH(string carriageid, int carriagestatus)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                if (carriagestatus == 10 || carriagestatus == 11)
                {
                    DataTable odt = dbc.GetEmptyDataTable("tb_b_carriage");
                    DataTableTracker odtt = new DataTableTracker(odt);
                    DataRow odr = odt.NewRow();
                    odr["carriageid"] = carriageid;
                    odr["carriagestatus"] = 0;
                    odt.Rows.Add(odr);
                    dbc.UpdateTable(odt, odtt);

                    DataTable ofdt = dbc.GetEmptyDataTable("tb_b_carriage_flow");
                    DataRow ofdr = ofdt.NewRow();
                    ofdr["carriageflowid"] = Guid.NewGuid().ToString();
                    ofdr["carriageid"] = carriageid;
                    ofdr["carriagestatus"] = 0;
                    ofdr["status"] = 0;
                    ofdr["adduser"] = SystemUser.CurrentUser.UserID;
                    ofdr["addtime"] = DateTime.Now;
                    ofdt.Rows.Add(ofdr);
                    dbc.InsertTable(ofdt);
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

    [CSMethod("WC")]
    public object WC(string carriageid, int carriagestatus)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                string str = "select a.*,c.modetype,c.modecoefficient from tb_b_carriage a left join tb_b_user c on a.userid=c.UserID where a.status=0 and a.carriageid=" + dbc.ToSqlValue(carriageid);
                DataTable dt = dbc.ExecuteDataTable(str);
                if (dt.Rows.Count > 0)
                {
                    if ((Convert.ToInt32(dt.Rows[0]["carriagestatus"]) == 50 && Convert.ToInt32(dt.Rows[0]["isoilpay"]) == 1 && Convert.ToInt32(dt.Rows[0]["ismoneypay"]) == 1
                        && Convert.ToInt32(dt.Rows[0]["ismoneynewpay"]) == 1 && Convert.ToInt32(dt.Rows[0]["modetype"]) == 1) || (Convert.ToInt32(dt.Rows[0]["carriagestatus"]) == 50
                        && Convert.ToInt32(dt.Rows[0]["ismoneypay"]) == 1 && Convert.ToInt32(dt.Rows[0]["modetype"]) == 2))
                    {
                        DataTable odt = dbc.GetEmptyDataTable("tb_b_carriage");
                        DataTableTracker odtt = new DataTableTracker(odt);
                        DataRow odr = odt.NewRow();
                        odr["carriageid"] = carriageid;
                        odr["carriagestatus"] = 90;
                        odt.Rows.Add(odr);
                        dbc.UpdateTable(odt, odtt);

                        DataTable ofdt = dbc.GetEmptyDataTable("tb_b_carriage_flow");
                        DataRow ofdr = ofdt.NewRow();
                        ofdr["carriageflowid"] = Guid.NewGuid().ToString();
                        ofdr["carriageid"] = carriageid;
                        ofdr["carriagestatus"] = 90;
                        ofdr["status"] = 0;
                        ofdr["adduser"] = SystemUser.CurrentUser.UserID;
                        ofdr["addtime"] = DateTime.Now;
                        ofdt.Rows.Add(ofdr);
                        dbc.InsertTable(ofdt);
                    }
                }
                dbc.CommitTransaction();

                var request = (HttpWebRequest)WebRequest.Create(ServiceURL + "sendSms/arrive/todriver");
                request.Method = "POST";
                request.ContentType = "application/json;charset=UTF-8";
                var byteData = Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(new
                {
                    carriageid = carriageid
                }));
                var length = byteData.Length;
                request.ContentLength = length;
                var writer = request.GetRequestStream();
                writer.Write(byteData, 0, length);
                writer.Close();


                var request1 = (HttpWebRequest)WebRequest.Create(ServiceURL + "sendSms/finish/tocaruser");
                request1.Method = "POST";
                request1.ContentType = "application/json;charset=UTF-8";
                var byteData1 = Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(new
                {
                    carriageid = carriageid
                }));
                var length1 = byteData1.Length;
                request1.ContentLength = length1;
                var writer1 = request1.GetRequestStream();
                writer1.Write(byteData1, 0, length1);
                writer1.Close();

                var request3 = (HttpWebRequest)WebRequest.Create(ServiceURL + "sendSms/finish/tocaruser");
                request3.Method = "POST";
                request3.ContentType = "application/json;charset=UTF-8";
                var byteData3 = Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(new
                {
                    carriageid = carriageid
                }));
                var length3 = byteData3.Length;
                request3.ContentLength = length3;
                var writer3 = request3.GetRequestStream();
                writer3.Write(byteData3, 0, length3);
                writer3.Close();

                return true;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }

    [CSMethod("YKDK")]
    public object YKDK(string carriageid, int carriagestatus)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                string str = "select a.*,c.modetype,c.modecoefficient from tb_b_carriage a left join tb_b_user c on a.userid=c.UserID  where a.status=0 and a.carriageid=" + dbc.ToSqlValue(carriageid);
                DataTable dt = dbc.ExecuteDataTable(str);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["modetype"] != null && dt.Rows[0]["modetype"].ToString() != "")
                    {
                        if (dt.Rows[0]["modetype"].ToString() == "1")
                        {
                            if ((Convert.ToInt32(dt.Rows[0]["isoilpay"]) == 0) && (Convert.ToInt32(dt.Rows[0]["carriagestatus"]) == 30 || Convert.ToInt32(dt.Rows[0]["carriagestatus"]) == 40 || Convert.ToInt32(dt.Rows[0]["carriagestatus"]) == 50))
                            {
                                DataTable odt = dbc.GetEmptyDataTable("tb_b_carriage");
                                DataTableTracker odtt = new DataTableTracker(odt);
                                DataRow odr = odt.NewRow();
                                odr["carriageid"] = carriageid;
                                odr["isoilpay"] = 1;
                                odt.Rows.Add(odr);
                                dbc.UpdateTable(odt, odtt);

                                DataTable pfdt = dbc.GetEmptyDataTable("tb_b_carriage_pay");
                                DataRow pfdr = pfdt.NewRow();
                                pfdr["carriagepayid"] = Guid.NewGuid().ToString();
                                pfdr["carriagepaytype"] = 0;
                                pfdr["adduser"] = SystemUser.CurrentUser.UserID;
                                pfdr["addtime"] = DateTime.Now;
                                pfdr["carriageid"] = carriageid;
                                pfdt.Rows.Add(pfdr);
                                dbc.InsertTable(pfdt);
                            }
                        }
                        else
                        {
                            throw new Exception("没有可验收付打款的承运单！");
                        }
                    }
                    else
                    {
                        throw new Exception("没有可验收付打款的承运单！");
                    }
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

    [CSMethod("XJDK")]
    public object XJDK(string carriageid, int carriagestatus)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                string str = @"select a.*,b.UserName,d.caruser,c.modetype,c.modecoefficient from tb_b_carriage a 
                left join tb_b_user b on a.driverid=b.UserID 
                left join tb_b_user c on a.userid=c.UserID 
                left join  tb_b_car d on a.carid=d.id
                where a.status=0 and a.carriageid=" + dbc.ToSqlValue(carriageid);
                DataTable dt = dbc.ExecuteDataTable(str);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["caruser"] != null && dt.Rows[0]["caruser"].ToString() != "")
                    {
                        string sql = "select * from tb_b_user where (ClientKind=1 or ClientKind=2) and IsSHPass=1 and UserName=" + dbc.ToSqlValue(dt.Rows[0]["caruser"].ToString());
                        DataTable udt = dbc.ExecuteDataTable(sql);
                        if (udt.Rows.Count > 0)
                        {
                            if ((Convert.ToInt32(dt.Rows[0]["ismoneypay"]) == 0) && (Convert.ToInt32(dt.Rows[0]["carriagestatus"]) == 30 || Convert.ToInt32(dt.Rows[0]["carriagestatus"]) == 40 || Convert.ToInt32(dt.Rows[0]["carriagestatus"]) == 50))
                            {
                                decimal money = 0;
                                if (dt.Rows[0]["carriagemoney"] != null && dt.Rows[0]["carriagemoney"].ToString() != "")
                                {
                                    //if (dt.Rows[0]["insurancemoney"] != null && dt.Rows[0]["insurancemoney"].ToString() != "")
                                    //{
                                    //    money = Convert.ToDecimal(dt.Rows[0]["carriagemoney"]) - Convert.ToDecimal(dt.Rows[0]["insurancemoney"]) / 100;
                                    //}
                                    //else
                                    //{
                                        money = Convert.ToDecimal(dt.Rows[0]["carriagemoney"]);
                                    //}
                                }

                                string _url = ServiceURL + "huabozijin";
                                string jsonParam = new JavaScriptSerializer().Serialize(new
                                {
                                    username = dt.Rows[0]["caruser"],
                                    carriagecode = dt.Rows[0]["carriagecode"],
                                    money = money.ToString(),
                                    type = "1"
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
                                        DataTable odt = dbc.GetEmptyDataTable("tb_b_carriage");
                                        DataTableTracker odtt = new DataTableTracker(odt);
                                        DataRow odr = odt.NewRow();
                                        odr["carriageid"] = carriageid;
                                        odr["ismoneypay"] = 1;
                                        odt.Rows.Add(odr);
                                        dbc.UpdateTable(odt, odtt);

                                        DataTable pfdt = dbc.GetEmptyDataTable("tb_b_carriage_pay");
                                        DataRow pfdr = pfdt.NewRow();
                                        pfdr["carriagepayid"] = Guid.NewGuid().ToString();
                                        pfdr["carriagepaytype"] = 1;
                                        pfdr["adduser"] = SystemUser.CurrentUser.UserID;
                                        pfdr["addtime"] = DateTime.Now;
                                        pfdr["carriageid"] = carriageid;
                                        pfdt.Rows.Add(pfdr);
                                        dbc.InsertTable(pfdt);

                                        dbc.CommitTransaction();

                                        if (dt.Rows.Count > 0)
                                        {
                                            var request1 = (HttpWebRequest)WebRequest.Create(ServiceURL + "sendSms/pay/tocaruser");
                                            request1.Method = "POST";
                                            request1.ContentType = "application/json;charset=UTF-8";
                                            var byteData1 = Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(new
                                            {
                                                carriageid = carriageid
                                            }));
                                            var length1 = byteData1.Length;
                                            request1.ContentLength = length1;
                                            var writer1 = request1.GetRequestStream();
                                            writer1.Write(byteData1, 0, length1);
                                            writer1.Close();
                                        }
                                        return true;
                                    }
                                    else
                                    {
                                        throw new Exception("现金打款失败");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception(jo["details"].ToString());
                                }
                            }
                            else
                            {
                                throw new Exception("没有可现金打款的承运单！");
                            }
                        }
                        else
                        {
                            throw new Exception("划拨对象不是有效用户，请核实!");
                        }
                    }
                    else
                    {
                        throw new Exception("对不起，车主用户不存在!");
                    }
                }
                else
                {
                    throw new Exception("承运单不存在！");
                }

            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }

    [CSMethod("XJDK1")]
    public object XJDK1(string carriageid, int carriagestatus)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                string str = @"select a.*,b.UserName,b.caruser from tb_b_carriage a left join tb_b_user b on a.driverid=b.UserID 
                where a.status=0 and a.carriageid=" + dbc.ToSqlValue(carriageid);
                DataTable dt = dbc.ExecuteDataTable(str);
                if (dt.Rows.Count > 0)
                {
                    if ((Convert.ToInt32(dt.Rows[0]["ismoneypay"]) == 0) && (Convert.ToInt32(dt.Rows[0]["carriagestatus"]) == 30 || Convert.ToInt32(dt.Rows[0]["carriagestatus"]) == 40 || Convert.ToInt32(dt.Rows[0]["carriagestatus"]) == 50))
                    {
                               

                        DataTable odt = dbc.GetEmptyDataTable("tb_b_carriage");
                        DataTableTracker odtt = new DataTableTracker(odt);
                        DataRow odr = odt.NewRow();
                        odr["carriageid"] = carriageid;
                        odr["ismoneypay"] = 1;
                        odt.Rows.Add(odr);
                        dbc.UpdateTable(odt, odtt);

                        DataTable pfdt = dbc.GetEmptyDataTable("tb_b_carriage_pay");
                        DataRow pfdr = pfdt.NewRow();
                        pfdr["carriagepayid"] = Guid.NewGuid().ToString();
                        pfdr["carriagepaytype"] = 1;
                        pfdr["adduser"] = SystemUser.CurrentUser.UserID;
                        pfdr["addtime"] = DateTime.Now;
                        pfdr["carriageid"] = carriageid;
                        pfdt.Rows.Add(pfdr);
                        dbc.InsertTable(pfdt);

                        dbc.CommitTransaction();

                        if (dt.Rows.Count > 0)
                        {
                            var request1 = (HttpWebRequest)WebRequest.Create(ServiceURL + "sendSms/pay/tocaruser");
                            request1.Method = "POST";
                            request1.ContentType = "application/json;charset=UTF-8";
                            var byteData1 = Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(new
                            {
                                carriageid = carriageid
                            }));
                            var length1 = byteData1.Length;
                            request1.ContentLength = length1;
                            var writer1 = request1.GetRequestStream();
                            writer1.Write(byteData1, 0, length1);
                            writer1.Close();
                        }
                        return true;
                    }
                    else
                    {
                        throw new Exception("没有可现金打款的承运单！");
                    }
                    
                }
                else
                {
                    throw new Exception("承运单不存在！");
                }

            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }

    [CSMethod("YSFDK")]
    public object YSFDK(string carriageid, int carriagestatus)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                string str = @"select a.*,b.UserName,b.caruser,c.modetype,c.modecoefficient from tb_b_carriage a left join tb_b_user b on a.driverid=b.UserID 
                left join tb_b_user c on a.userid=c.UserID 
                where a.status=0 and a.carriageid=" + dbc.ToSqlValue(carriageid);
                DataTable dt = dbc.ExecuteDataTable(str);

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["modetype"] != null && dt.Rows[0]["modetype"].ToString() != "")
                    {
                        if (dt.Rows[0]["modetype"].ToString() == "1")
                        {
                            if (dt.Rows[0]["caruser"] != null && dt.Rows[0]["caruser"].ToString() != "")
                            {
                                string sql = "select * from tb_b_user where (ClientKind=1 or ClientKind=2) and IsSHPass=1 and UserName=" + dbc.ToSqlValue(dt.Rows[0]["caruser"].ToString());
                                DataTable udt = dbc.ExecuteDataTable(sql);
                                if (udt.Rows.Count > 0)
                                {

                                    if ((Convert.ToInt32(dt.Rows[0]["ismoneynewpay"]) == 0) && (Convert.ToInt32(dt.Rows[0]["carriagestatus"]) >= 50))
                                    {
                                        decimal money = 0;
                                        if (dt.Rows[0]["carriagemoneynew"] != null && dt.Rows[0]["carriagemoneynew"].ToString() != "")
                                        {
                                            money = Convert.ToDecimal(dt.Rows[0]["carriagemoneynew"]);
                                        }

                                        string _url = ServiceURL + "huabozijin";
                                        string jsonParam = new JavaScriptSerializer().Serialize(new
                                        {
                                            username = dt.Rows[0]["caruser"],
                                            carriagecode = dt.Rows[0]["carriagecode"],
                                            money = money.ToString(),
                                            type = "2"
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
                                                DataTable odt = dbc.GetEmptyDataTable("tb_b_carriage");
                                                DataTableTracker odtt = new DataTableTracker(odt);
                                                DataRow odr = odt.NewRow();
                                                odr["carriageid"] = carriageid;
                                                odr["ismoneynewpay"] = 1;
                                                odt.Rows.Add(odr);
                                                dbc.UpdateTable(odt, odtt);

                                                DataTable pfdt = dbc.GetEmptyDataTable("tb_b_carriage_pay");
                                                DataRow pfdr = pfdt.NewRow();
                                                pfdr["carriagepayid"] = Guid.NewGuid().ToString();
                                                pfdr["carriagepaytype"] = 2;
                                                pfdr["adduser"] = SystemUser.CurrentUser.UserID;
                                                pfdr["addtime"] = DateTime.Now;
                                                pfdr["carriageid"] = carriageid;
                                                pfdt.Rows.Add(pfdr);
                                                dbc.InsertTable(pfdt);

                                                dbc.CommitTransaction();
                                                return true;
                                            }
                                            else
                                            {
                                                throw new Exception("验收付打款失败！");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new Exception(jo["details"].ToString());
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("没有可验收付打款的承运单！");
                                    }
                                }
                                else
                                {
                                    throw new Exception("划拨对象不是有效用户，请核实!");
                                }
                            }
                            else
                            {
                                throw new Exception("对不起，车主用户不存在!");
                            }
                        }
                        else
                        {
                            throw new Exception("没有可验收付打款的承运单！");
                        }
                    }
                    else
                    {
                        throw new Exception("没有可验收付打款的承运单！");
                    }
                    
                }
                else
                {
                    throw new Exception("承运单不存在！");
                }

            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }


    [CSMethod("YSFDK1")]
    public object YSFDK1(string carriageid, int carriagestatus)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                string str = @"select a.*,b.UserName,b.caruser,c.modetype,c.modecoefficient from tb_b_carriage a left join tb_b_user b on a.driverid=b.UserID 
                left join tb_b_user c on a.userid=c.UserID 
                where a.status=0 and a.carriageid=" + dbc.ToSqlValue(carriageid);
                DataTable dt = dbc.ExecuteDataTable(str);

                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["modetype"] != null && dt.Rows[0]["modetype"].ToString() != "")
                    {
                        if (dt.Rows[0]["modetype"].ToString() == "1")
                        {
                            if ((Convert.ToInt32(dt.Rows[0]["ismoneynewpay"]) == 0) && (Convert.ToInt32(dt.Rows[0]["carriagestatus"]) >= 50))
                            {
                                DataTable odt = dbc.GetEmptyDataTable("tb_b_carriage");
                                DataTableTracker odtt = new DataTableTracker(odt);
                                DataRow odr = odt.NewRow();
                                odr["carriageid"] = carriageid;
                                odr["ismoneynewpay"] = 1;
                                odt.Rows.Add(odr);
                                dbc.UpdateTable(odt, odtt);

                                DataTable pfdt = dbc.GetEmptyDataTable("tb_b_carriage_pay");
                                DataRow pfdr = pfdt.NewRow();
                                pfdr["carriagepayid"] = Guid.NewGuid().ToString();
                                pfdr["carriagepaytype"] = 2;
                                pfdr["adduser"] = SystemUser.CurrentUser.UserID;
                                pfdr["addtime"] = DateTime.Now;
                                pfdr["carriageid"] = carriageid;
                                pfdt.Rows.Add(pfdr);
                                dbc.InsertTable(pfdt);

                                dbc.CommitTransaction();
                                return true;
                            }
                            else
                            {
                                throw new Exception("没有可验收付打款的承运单！");
                            }
                        }
                        else
                        {
                            throw new Exception("没有可验收付打款的承运单！");
                        }
                    }
                    else
                    {
                        throw new Exception("没有可验收付打款的承运单！");
                    }
                }
                else
                {
                    throw new Exception("承运单不存在！");
                }

            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }


    [CSMethod("getInsure")]
    public object getInsure(string carriageid)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {

                string _url = ServiceURL + "getInsure";
                string jsonParam = new JavaScriptSerializer().Serialize(new
                {
                    carriageid = carriageid
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

                return responseString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    [CSMethod("QRMM")]
    public object QRMM(string password)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                if (SystemUser.CurrentUser.Password == password.Trim())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}

