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
    public object GetCYDList(int pagnum, int pagesize, string carriagecode, string UserXM, string beg, string end, string isinvoice, string carriagestatus, string ismoneypay)
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
                if (!string.IsNullOrEmpty(carriagestatus))
                {
                    where += " and " + dbc.C_EQ("a.carriagestatus", carriagestatus); ;
                }
                if (!string.IsNullOrEmpty(ismoneypay))
                {
                    where += " and " + dbc.C_EQ("a.ismoneypay", ismoneypay); ;
                }
                string str = @"select a.*,b.UserName as sjzh,b.UserTel as sjdh,c.UserXM as zx,d.driverxm as sjxm,d.carnumber as sjcarnumber,d.caruser,e.name as kp,
                                case a.carriagestatus
                                when 10 then 1
                                when 30 then 2
                                when 50 then 3
                                when 40 then 4
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
    public byte[] GetCYDListToFile(string carriagecode, string UserXM, string beg, string end, string iskp, string status, string moneypay)
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
                if (!string.IsNullOrEmpty(status))
                {
                    where += " and " + dbc.C_EQ("a.carriagestatus", status); ;
                }
                if (!string.IsNullOrEmpty(moneypay))
                {
                    where += " and " + dbc.C_EQ("a.ismoneypay", moneypay); ;
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
    public void Cykp(JSReader jsr, int zt)
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
                string str = "select a.*,c.modetype,c.modecoefficient,c.carriagegetmode from tb_b_carriage a left join tb_b_user c on a.userid=c.UserID where a.status=0 and a.carriageid=" + dbc.ToSqlValue(carriageid);
                DataTable dt = dbc.ExecuteDataTable(str);
                if (dt.Rows.Count > 0)
                {
                    bool bo = false;
                    if (dt.Rows[0]["carriagegetmode"].ToString() == "1")
                    {
                        bo = true;//现金，无需判断
                    }
                    if ((Convert.ToInt32(dt.Rows[0]["carriagestatus"]) == 50 &&
                        Convert.ToInt32(dt.Rows[0]["isoilpay"]) == 1 &&
                        Convert.ToInt32(dt.Rows[0]["ismoneypay"]) == 1 &&
                        Convert.ToInt32(dt.Rows[0]["ismoneynewpay"]) == 1 &&
                        (bo || Convert.ToInt32(dt.Rows[0]["modetype"]) == 1)) ||
                        (Convert.ToInt32(dt.Rows[0]["carriagestatus"]) == 50 &&
                        Convert.ToInt32(dt.Rows[0]["ismoneypay"]) == 1 &&
                        (bo || Convert.ToInt32(dt.Rows[0]["modetype"]) == 2)))
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

                //var request = (HttpWebRequest)WebRequest.Create(ServiceURL + "sendSms/arrive/todriver");
                //request.Method = "POST";
                //request.ContentType = "application/json;charset=UTF-8";
                //var byteData = Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(new
                //{
                //    carriageid = carriageid
                //}));
                //var length = byteData.Length;
                //request.ContentLength = length;
                //var writer = request.GetRequestStream();
                //writer.Write(byteData, 0, length);
                //writer.Close();


                //var request1 = (HttpWebRequest)WebRequest.Create(ServiceURL + "sendSms/finish/tocaruser");
                //request1.Method = "POST";
                //request1.ContentType = "application/json;charset=UTF-8";
                //var byteData1 = Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(new
                //{
                //    carriageid = carriageid
                //}));
                //var length1 = byteData1.Length;
                //request1.ContentLength = length1;
                //var writer1 = request1.GetRequestStream();
                //writer1.Write(byteData1, 0, length1);
                //writer1.Close();

                //var request3 = (HttpWebRequest)WebRequest.Create(ServiceURL + "sendSms/finish/tocaruser");
                //request3.Method = "POST";
                //request3.ContentType = "application/json;charset=UTF-8";
                //var byteData3 = Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(new
                //{
                //    carriageid = carriageid
                //}));
                //var length3 = byteData3.Length;
                //request3.ContentLength = length3;
                //var writer3 = request3.GetRequestStream();
                //writer3.Write(byteData3, 0, length3);
                //writer3.Close();

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
                string str = @"select a.*,b.UserName,d.caruser,c.modetype,c.modecoefficient from tb_b_carriage a 
                left join tb_b_user b on a.driverid=b.UserID 
                left join tb_b_user c on a.userid=c.UserID 
                left join  tb_b_car d on a.carid=d.id
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

    #region 物润对接
    [CSMethod("GetWrList")]
    public object GetCYDList(int pagnum, int pagesize, string carriagecode, string UserXM, string beg, string end, string isinvoice, string ismoneypay)
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
                where += " and " + dbc.C_EQ("a.carriagestatus", 90);
                if (!string.IsNullOrEmpty(ismoneypay))
                {
                    where += " and " + dbc.C_EQ("a.ismoneypay", ismoneypay); ;
                }
                string str = @"select a.*,b.UserName as sjzh,b.UserTel as sjdh,c.UserXM as zx,d.driverxm as sjxm,d.carnumber as sjcarnumber,d.caruser,e.name as kp,
                                case a.carriagestatus
                                when 10 then 1
                                when 30 then 2
                                when 50 then 3
                                when 40 then 4
                                when 0 then 5
                                when 11 then 6
                                when 20 then 7
                                when 21 then 8
                                else 9 end as px,c.modetype,c.modecoefficient,c.carriagegetmode,
                                f.FJ_ID fjid0,f.Full_Url fjurl0,
                                g.FJ_ID fjid1,g.Full_Url fjurl1,
                                h.FJ_ID fjid2,h.Full_Url fjurl2 
                              from tb_b_carriage a 
                            left join tb_b_user b on a.driverid=b.UserID 
                            inner join tb_b_user c on a.userid=c.UserID and c.ClientKind=1
                            left join  tb_b_car d on a.carid=d.id
                            left join tb_b_carriagechb e on c.carriagechbid=e.id
                            left join tb_b_carriage_photo f on a.carriageid=f.carriageid and f.type=0 and f.status=0
                            left join tb_b_carriage_photo g on a.carriageid=g.carriageid and g.type=1 and g.status=0
                            left join tb_b_carriage_photo h on a.carriageid=h.carriageid and h.type=2 and h.status=0
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

    [CSMethod("GetWrListToFile", 2)]
    public byte[] GetCYDListToFile(string carriagecode, string UserXM, string beg, string end, string iskp, string moneypay)
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
                where += " and " + dbc.C_EQ("a.carriagestatus", 90);
                if (!string.IsNullOrEmpty(moneypay))
                {
                    where += " and " + dbc.C_EQ("a.ismoneypay", moneypay); ;
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

    [CSMethod("UploadPicForcarriage", 1)]
    public object UploadPicForcarriage(FileData[] fds, string carriageid, int type)
    {
        try
        {
            string url = System.Configuration.ConfigurationManager.AppSettings["ServiceURL"].ToString();

            WebRequest request = (HttpWebRequest)WebRequest.Create(url + "uploadAdver");
            MsMultiPartFormData form = new MsMultiPartFormData();
            form.AddFormField("devilField", "中国人");
            form.AddStreamFile("fileUpload", fds[0].FileName, fds[0].FileBytes);
            form.PrepareFormData();
            request.ContentType = "multipart/form-data; boundary=" + form.Boundary;
            request.Method = "POST";
            Stream stream = request.GetRequestStream();
            foreach (var b in form.GetFormData())
            {
                stream.WriteByte(b);
            }
            stream.Close();
            string responseContent = "";
            using (HttpWebResponse res = (HttpWebResponse)request.GetResponse())
            {
                using (Stream resStream = res.GetResponseStream())
                {
                    byte[] buffer = new byte[1024];
                    int read;
                    while ((read = resStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        responseContent += Encoding.UTF8.GetString(buffer, 0, read);
                    }
                }
                res.Close();
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<FJ> list = js.Deserialize<List<FJ>>(responseContent);
            #region 插入数据
            using (DBConnection dbc = new DBConnection())
            {
                try
                {
                    dbc.BeginTransaction();
                    string sqlStr = @"update tb_b_carriage_photo set status=1 
                where carriageid=" + dbc.ToSqlValue(carriageid) + " and type=" + type;
                    dbc.ExecuteNonQuery(sqlStr);

                    DataTable photoDt = dbc.GetEmptyDataTable("tb_b_carriage_photo");
                    DataRow phdr = photoDt.NewRow();
                    phdr["carriagephotoid"] = Guid.NewGuid();
                    phdr["carriageid"] = carriageid;
                    phdr["type"] = type;
                    phdr["FJ_ID"] = list[0].fjId;
                    phdr["status"] = 0;
                    phdr["adduser"] = SystemUser.CurrentUser.UserID;
                    phdr["addtime"] = DateTime.Now;
                    phdr["updateuser"] = SystemUser.CurrentUser.UserID;
                    phdr["updatetime"] = DateTime.Now;
                    phdr["Full_Url"] = list[0].fileFullUrl;
                    photoDt.Rows.Add(phdr);
                    dbc.InsertTable(photoDt);

                    dbc.CommitTransaction();
                }
                catch (Exception ex)
                {
                    dbc.RoolbackTransaction();
                    throw ex;
                }

            }
            #endregion

            return new { fileurl = list[0].fileFullUrl, isdefault = 0, fileid = list[0].fjId };
        }
        catch (Exception ex)
        {
            throw ex;
        }





    }

    public int ConvertDateTimeInt(DateTime time)
    {
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
        return (int)(time - startTime).TotalSeconds;
    }

    [CSMethod("PushWr")]
    public string PushWr(string carriageid)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                #region 准备数据
                DateTime _dtStart = new DateTime(1970, 1, 1, 8, 0, 0);
                string sqlstr = @"select a.*,b.dq_bm f_dq_bm,c.dq_bm t_dq_bm,d.UserXM,d.UserName,e.carnumber,f.addtime flowaddtime,
g.addtime flowaddtime2,h.Full_Url Full_Url1,i.Full_Url Full_Url2,j.Full_Url Full_Url3 from tb_b_carriage a
                                left join tb_b_dq b on a.carriagefromcity=b.dq_mc
                                left join tb_b_dq c on a.carriagetocity=c.dq_mc
                                left join tb_b_user d on a.driverid=d.UserID
                                left join tb_b_car e on a.carid=e.id
                                left join tb_b_carriage_flow f on a.carriageid=f.carriageid and f.carriagestatus=90
                                left join tb_b_carriage_flow g on a.carriageid=g.carriageid and g.carriagestatus=50
                                left join tb_b_carriage_photo h on a.carriageid=h.carriageid and h.type=0 and h.status=0
                                left join tb_b_carriage_photo i on a.carriageid=i.carriageid and i.type=1 and i.status=0
                                left join tb_b_carriage_photo j on a.carriageid=j.carriageid and j.type=2 and j.status=0
                                where a.carriageid=" + dbc.ToSqlValue(carriageid);
                DataTable dt = dbc.ExecuteDataTable(sqlstr);
                if (dt.Rows.Count == 0)
                {
                    return "称运单不存在。";
                }
                #endregion

                carriageid = carriageid.Replace("-", "");
                string url = System.Configuration.ConfigurationManager.AppSettings["ServiceBaseURL"].ToString();
                #region 1.发布货源接口
                DataRow dr1 = dt.Rows[0];
                DateTime addtime = Convert.ToDateTime(dr1["addtime"].ToString());

                var request1 = (HttpWebRequest)WebRequest.Create(url + "car/PlanGoods/add");
                request1.Method = "POST";
                request1.ContentType = "application/json;charset=UTF-8";
                var byteData1 = Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(new
                {
                    goods_no = carriageid,//(合作方货源编号)
                    goods_price = string.IsNullOrEmpty(dr1["carriagepoints"].ToString()) ? 0m : Convert.ToDecimal(dr1["carriagepoints"]) * 100 / 30000, //(运输单价(单位：分))
                    goods_type = "999",// (货物类型)
                    goods_name = "百货",//(货物名称)
                    goods_num = 30000,// (货量(单位：KG))
                    load_time = ConvertDateTimeInt(addtime),//(起运时间(时间戳))
                    load_place_id = dr1["f_dq_bm"].ToString(),// 找tb_b_dq表找编码 (装货地id(省市区编码或港口编码))
                    unload_place_id = dr1["t_dq_bm"].ToString(),// 找tb_b_dq表找编码（卸货地id(省市区编码或港口编码)）
                    load_place_detail = "物流园区",//(装货地详细地址(不包含省市区))
                    unload_place_detail = dr1["carriageaddress"].ToString(),//(卸货地详细地址(不包含省市区))
                    shipper_contact_name = dr1["UserXM"].ToString(),//(托运方联系人名称)
                    shipper_contact_mobile = dr1["UserName"].ToString(),//(托运方手机号)
                    consi_name = "查货宝",//(收货方名称)
                    consi_contact_name = "刘洁宏",//      (收货联系人姓名)
                    consi_contact_mobile = "15051939262"//   (收货人手机号)
                }));
                var length1 = byteData1.Length;
                request1.ContentLength = length1;
                var writer1 = request1.GetRequestStream();
                writer1.Write(byteData1, 0, length1);
                writer1.Close();
                HttpWebResponse response;
                try
                {
                    //获得响应流
                    response = (HttpWebResponse)request1.GetResponse();
                }
                catch (WebException ex)
                {
                    response = ex.Response as HttpWebResponse;
                }
                Stream s = response.GetResponseStream();
                StreamReader sRead = new StreamReader(s);
                string postContent = sRead.ReadToEnd();
                sRead.Close();
                JavaScriptSerializer js = new JavaScriptSerializer();
                resultWr res1 = js.Deserialize<resultWr>(postContent);
                if (res1.code != 0 && res1.code != 110)
                {
                    return res1.msg;
                }
                #endregion

                #region 2.预约货源
                DataRow dr2 = dt.Rows[0];
                decimal carriageoilmoney = 0m;
                if (!string.IsNullOrEmpty(dr2["carriageoilmoney"].ToString()))
                {
                    carriageoilmoney = Convert.ToDecimal(dr2["carriageoilmoney"].ToString());
                }
                decimal carriagemoneynew = 0m;
                if (!string.IsNullOrEmpty(dr2["carriagemoneynew"].ToString()))
                {
                    carriagemoneynew = Convert.ToDecimal(dr2["carriagemoneynew"].ToString());
                }
                decimal carriagemoney = 0m;
                if (!string.IsNullOrEmpty(dr2["carriagemoney"].ToString()))
                {
                    carriagemoney = Convert.ToDecimal(dr2["carriagemoney"].ToString());
                }
                if (string.IsNullOrEmpty(dr2["flowaddtime"].ToString()))
                {
                    return "卸货时间不存在。";
                }
                if (string.IsNullOrEmpty(dr2["flowaddtime2"].ToString()))
                {
                    return "交易完成时间不存在。";
                }
                var request2 = (HttpWebRequest)WebRequest.Create(url + "car/PlanGoods/createOrder");
                request2.Method = "POST";
                request2.ContentType = "application/json;charset=UTF-8";
                var byteData2 = Encoding.UTF8.GetBytes(new JavaScriptSerializer().Serialize(new
                {
                    goods_no = carriageid,//（合作方货源编号）
                    order_no = dr2["carriagecode"].ToString(),//tb_b_carriage.carriagecode（合作方唯一业务单号）
                    waybill_amount = (carriageoilmoney + carriagemoney + carriagemoneynew) * 100,//(tb_b_carriage.carriageoilmoney+carriagemoney+carriagemoneynew) (合同金额(司机的劳务费+油费+过路费、单位：分)）
                    invoice_amount = (carriageoilmoney + carriagemoney + carriagemoneynew) * 100 * 1.03m,//(tb_b_carriage.carriagemoney+carriagemoneynew) *1.03（开票金额(含税、司机的劳务费*(1+0.03)、单位：分、开票金额计算公式以商务合同约定为准)）
                    labour_amount = (carriageoilmoney + carriagemoney + carriagemoneynew) * 100,//(tb_b_carriage.carriagemoney+carriagemoneynew) （运输劳务费用(不含税、单位：分)）
                    contract_time = ConvertDateTimeInt(addtime),//（合同签订日期(时间戳)）
                    carrier_mobile = dr2["UserName"].ToString(),//（承运人手机号(个体承运人为司机手机号)）
                    transport_mobile = dr2["UserName"].ToString(),//（司机手机号）
                    transport_type = 1,//（运输工具类型(1车辆 2船舶)(目前传1)）
                    trans_vehicle_name = dr2["carnumber"].ToString(),//tb_b_carriage.carid.tb_b_car.carnumber（运输工具名称(车牌号)）
                    unload_time = ConvertDateTimeInt(Convert.ToDateTime(dr2["flowaddtime"].ToString())),//tb_b_carriage_flow.addtime(carriage status = 90)（卸货时间(时间戳)）
                    finish_time = ConvertDateTimeInt(Convert.ToDateTime(dr2["flowaddtime2"].ToString())),//(carriage status = 50)（交易完成时间(时间戳)）
                    pay_style = "第三方支付",//（支付方式）
                    pay_channel = "宝付",//（支付渠道）
                    pay_pic = dr2["Full_Url1"],//tb_b_carriage.photo(type = 0)（支付凭证图片URL地址(支持多张，逗号拼接)）
                    contract_pic = dr2["Full_Url2"],//tb_b_carriage.photo(type = 1)（合同图片URL地址(支持多张，逗号拼接)）
                    reply_pic = dr2["Full_Url3"]//tb_b_carriage.photo(type = 2)（回单图片URL地址(支持多张，逗号拼接)）
                }));
                var length2 = byteData2.Length;
                request2.ContentLength = length2;
                var writer2 = request2.GetRequestStream();
                writer2.Write(byteData2, 0, length2);
                writer2.Close();
                HttpWebResponse response2;
                try
                {
                    //获得响应流
                    response2 = (HttpWebResponse)request2.GetResponse();
                }
                catch (WebException ex)
                {
                    response2 = ex.Response as HttpWebResponse;
                }
                Stream s2 = response2.GetResponseStream();
                StreamReader sRead2 = new StreamReader(s2);
                string postContent2 = sRead2.ReadToEnd();
                sRead2.Close();

                JavaScriptSerializer js2 = new JavaScriptSerializer();
                resultWr res2 = js.Deserialize<resultWr>(postContent2);
                if (res2.code != 0)
                {
                    return res2.msg;
                }
                #endregion

                //3.更新tb_b_carriage
                sqlstr = "update tb_b_carriage set ispushwr=0 where carriageid=" + dbc.ToSqlValue(carriageid);
                dbc.ExecuteNonQuery(sqlstr);
                return "推送成功";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// 生成并下载合同
    /// </summary>
    /// <param name="carriageid"></param>
    /// <returns></returns>
    [CSMethod("DealHt", 2)]
    public byte[] DealHt(string carriageid)
    {
        using (DBConnection dbc = new DBConnection())
        {
            Aspose.Words.Document doc = new Aspose.Words.Document(System.Web.HttpContext.Current.Server.MapPath("~/Mb/司机协议（干线运输服务）20191108.docx"));
            //获取书签
            Aspose.Words.Bookmark sjxm = doc.Range.Bookmarks["司机姓名"];
            Aspose.Words.Bookmark sjxm2 = doc.Range.Bookmarks["司机姓名2"];
            Aspose.Words.Bookmark cysj = doc.Range.Bookmarks["承运时间"];
            Aspose.Words.Bookmark skr = doc.Range.Bookmarks["收款人"];

            string sqlstr = @"select b.driverxm,c.UserXM,a.addtime from tb_b_carriage a
                            left join tb_b_car b on a.carid=b.id
                            left join tb_b_user c on b.caruser=c.UserName
                            where a.carriageid=" + dbc.ToSqlValue(carriageid);
            DataTable dt = dbc.ExecuteDataTable(sqlstr);
            if (dt.Rows.Count > 0)
            {
                sjxm.Text = dt.Rows[0]["driverxm"].ToString();
                sjxm2.Text = dt.Rows[0]["driverxm"].ToString();
                cysj.Text = Convert.ToDateTime(dt.Rows[0]["addtime"].ToString()).ToString("yyyy年MM月dd日");
                skr.Text = dt.Rows[0]["UserXM"].ToString();
            }
            doc.MailMerge.DeleteFields();
            var docStream = new MemoryStream();
            doc.Save(docStream, Aspose.Words.Saving.SaveOptions.CreateSaveOptions(Aspose.Words.SaveFormat.Doc));
            byte[] filedata = docStream.ToArray();
            return filedata;
        }
    }

    /// <summary>
    /// 下载支付明细模板
    /// </summary>
    /// <returns></returns>
    [CSMethod("DealPayLine", 2)]
    public byte[] DealPayLine()
    {
        Aspose.Words.Document doc = new Aspose.Words.Document(System.Web.HttpContext.Current.Server.MapPath("~/Mb/支付明细.docx"));
        var docStream = new MemoryStream();
        doc.Save(docStream, Aspose.Words.Saving.SaveOptions.CreateSaveOptions(Aspose.Words.SaveFormat.Doc));
        byte[] filedata = docStream.ToArray();
        return filedata;
    }

    [CSMethod("DealHd", 2)]
    public byte[] DealHd(string carriageid)
    {
        using (DBConnection dbc = new DBConnection())
        {
            Workbook workbook = new Workbook(System.Web.HttpContext.Current.Server.MapPath("~/Mb/回单模板.xlsx"));//工作簿
            Worksheet sheet = workbook.Worksheets[0]; //工作表
            Cells cells = sheet.Cells;//单元格

            string sqlstr = @"select c.UserXM,d.addtime,a.carriagefromprovince,a.carriagefromcity,a.carriagetoprovince,a.carriagetocity,a.carriageaddress,
                              b.driverxm,b.caruser,a.carriagemoney,a.carriagemoneynew,a.carriageoilmoney from tb_b_carriage a
                            left join tb_b_car b on a.carid=b.id
                            left join tb_b_user c on a.userid=c.userid
                            left join tb_b_carriage_flow d on a.carriageid=d.carriageid and d.carriagestatus=90
                            where a.carriageid=" + dbc.ToSqlValue(carriageid);
            DataTable dt = dbc.ExecuteDataTable(sqlstr);
            if (dt.Rows.Count > 0)
            {
                cells[1, 1].PutValue(dt.Rows[0]["UserXM"].ToString());
                cells[1, 3].PutValue(string.IsNullOrEmpty(dt.Rows[0]["addtime"].ToString()) ? "" : Convert.ToDateTime(dt.Rows[0]["addtime"].ToString()).ToString("yyyy/MM/dd hh:mm:ss"));
                cells[2, 1].PutValue(dt.Rows[0]["carriagefromprovince"].ToString() + dt.Rows[0]["carriagefromcity"].ToString() + "物流园区");
                cells[2, 3].PutValue(dt.Rows[0]["carriagetoprovince"].ToString() + dt.Rows[0]["carriagetocity"].ToString() + dt.Rows[0]["carriageaddress"].ToString());
                cells[4, 1].PutValue(dt.Rows[0]["driverxm"].ToString());
                cells[4, 3].PutValue(dt.Rows[0]["caruser"].ToString());
                decimal money1 = string.IsNullOrEmpty(dt.Rows[0]["carriagemoney"].ToString()) ? 0m : Convert.ToDecimal(dt.Rows[0]["carriagemoney"].ToString());
                decimal money2 = string.IsNullOrEmpty(dt.Rows[0]["carriagemoneynew"].ToString()) ? 0m : Convert.ToDecimal(dt.Rows[0]["carriagemoneynew"].ToString());
                decimal money3 = string.IsNullOrEmpty(dt.Rows[0]["carriageoilmoney"].ToString()) ? 0m : Convert.ToDecimal(dt.Rows[0]["carriageoilmoney"].ToString());
                cells[5, 1].PutValue(money1 + money2 + money3);
            }

            MemoryStream ms = workbook.SaveToStream();
            byte[] bt = ms.ToArray();
            return bt;
        }
    }
    #endregion

    #region 在线查单专线设置
    [CSMethod("getZxcdList")]
    public object getZxcdList(int pagnum, int pagesize, string beg, string end, string cx_zxmc, string cx_fhrmc)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and a.AddTime>='" + Convert.ToDateTime(beg).ToString() + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.AddTime<='" + Convert.ToDateTime(end).AddDays(1).ToString() + "'";
                }
                if (!string.IsNullOrEmpty(cx_zxmc))
                {
                    where += " and " + dbc.C_Like("d.UserXM", cx_zxmc, LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(cx_fhrmc))
                {
                    where += " and " + dbc.C_Like("c.fhrmc", cx_fhrmc, LikeStyle.LeftAndRightLike);
                }

                string str = @"select a.OrderCode,c.UserName fhrmc,d.UserXM,b.carnumber,b.servicecode,b.networkName,b.memo,b.servicestatus,a.AddTime,b.person,b.id serviceid,a.PayID from tb_b_pay a
                                inner join(
	                                select t1.*,t2.networkName from tb_b_pay_service t1
	                                left join tb_b_network t2 on t1.networkid=t2.networkId
                                )b on a.PayID=b.payid
                                inner join tb_b_user c on a.PayUserID=c.UserID
                                left join tb_b_user d on a.CardUserID=d.UserID" + where + " order by AddTime desc";

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

    [CSMethod("Qrdd")]
    public void Qrdd(string id, string payid)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                dbc.BeginTransaction();
                string sqlstr = "update tb_b_pay_service set servicestatus=20 where id=" + dbc.ToSqlValue(id);
                dbc.ExecuteNonQuery(sqlstr);

                string userid = SystemUser.CurrentUser.UserID;
                DateTime ti = DateTime.Now;
                var dt = dbc.GetEmptyDataTable("tb_b_pay_flow");
                var dr = dt.NewRow();
                dr["id"] = Guid.NewGuid().ToString();
                dr["payid"] = payid;
                dr["servicestatus"] = 20;
                dr["status"] = 0;
                dr["adduser"] = userid;
                dr["addtime"] = ti;
                dr["updateuser"] = userid;
                dr["updatetime"] = ti;
                dt.Rows.Add(dr);
                dbc.InsertTable(dt);
                dbc.CommitTransaction();
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }
    #endregion
}

public class resultWr
{
    private string _msg;
    private int _code;
    private string _data;

    public string msg { get; set; }
    public int code { get; set; }
    public string data { get; set; }
}

