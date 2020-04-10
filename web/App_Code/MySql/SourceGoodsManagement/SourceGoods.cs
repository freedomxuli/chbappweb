using SmartFramework4v2.Data.MySql;
using SmartFramework4v2.Web.Common.JSON;
using SmartFramework4v2.Web.WebExecutor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// SourceGoods 的摘要说明
/// </summary>
[CSClass("SourceGoods")]
public class SourceGoods
{
    public SourceGoods()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    /// <summary>
    /// 获取询价分页
    /// </summary>
    /// <param name="pagnum"></param>
    /// <param name="pagesize"></param>
    /// <param name="beg"></param>
    /// <param name="end"></param>
    /// <param name="changjia"></param>
    /// <returns></returns>
    [CSMethod("getSourceGoodsListByPage")]
    public object getSourceGoodsListByPage(int pagnum, int pagesize, string beg, string end, string changjia, string offerstatus, string flowstatus)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.addtime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.addtime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(changjia.Trim()))
                {
                    where += " and " + dbc.C_Like("b.username", changjia.Trim(), SmartFramework4v2.Data.LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(offerstatus))
                {
                    where += " and " + dbc.C_EQ("a.offerstatus", offerstatus.Trim());
                }
                if (!string.IsNullOrEmpty(flowstatus))
                {
                    where += " and " + dbc.C_EQ("a.flowstatus", flowstatus.Trim());
                }
                string str = @"select a.*,b.username,b.carriername,c.name vehiclelengthrequirementname from tb_b_sourcegoodsinfo_offer a
                            left join tb_b_user b on a.shipperid=b.userid
                            left join tb_b_dictionary_detail c on a.vehiclelengthrequirement=c.bm
                            where (a.shipperid in(
                                select d.userid from tb_b_operator_association d
                                left join tb_b_user e on d.userid=e.userid
                                inner join tb_b_user f on d.operator=f.userid and f.correlationid=" + dbc.ToSqlValue(SystemUser.CurrentUser.UserID) + @"
                                where d.status = 0
                            ) or 'D4D659F2-C2AE-4D96-AA87-A5DF0EC3F57C'=" + dbc.ToSqlValue(SystemUser.CurrentUser.UserID.ToUpper())+ @")";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.offerstatus asc, a.flowstatus asc, a.goodsinsertdatetime desc", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// 修改询价
    /// </summary>
    /// <param name="offerid"></param>
    /// <param name="jsr"></param>
    /// <returns></returns>
    [CSMethod("UpdateSourceGoods")]
    public bool UpdateSourceGoods(string offerid, JSReader jsr)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                DateTime ti = DateTime.Now;
                String estimatemoney = jsr["estimatemoney"].ToString();
                String totalmonetaryamount = jsr["totalmonetaryamount"].ToString();

                var dt = dbc.GetEmptyDataTable("tb_b_sourcegoodsinfo_offer");
                var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                var dr = dt.NewRow();
                dr["offerid"] = offerid;
                if (!string.IsNullOrEmpty(estimatemoney))
                {
                    dr["estimatemoney"] = Convert.ToDecimal(estimatemoney);
                }
                if (!string.IsNullOrEmpty(totalmonetaryamount))
                {
                    dr["totalmonetaryamount"] = Convert.ToDecimal(totalmonetaryamount);
                }
                dr["updateuser"] = SystemUser.CurrentUser.UserID;
                dr["updatetime"] = ti;
                dt.Rows.Add(dr);
                dbc.UpdateTable(dt, dtt);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }

    /// <summary>
    /// 操作员填写运费
    /// </summary>
    /// <param name="id"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    [CSMethod("updateOffer")]
    public bool updateOffer(string offerid, JSReader jsr)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                dbc.BeginTransaction();

                String up = "";
                String estimatemoney = jsr["estimatemoney"].ToString();
                String estimatecarriertype = jsr["estimatecarriertype"].ToString();
                String istakegoodsbyestimate = jsr["istakegoodsbyestimate"].ToString();
                String estimatetakegoodsmoney = jsr["estimatetakegoodsmoney"].ToString();
                if (!string.IsNullOrEmpty(estimatetakegoodsmoney))
                {
                    up += "estimatetakegoodsmoney=" + dbc.ToSqlValue(estimatetakegoodsmoney) + ",";
                }
                String isvotebyestimate = jsr["isvotebyestimate"].ToString();
                String estimatevotemoney = jsr["estimatevotemoney"].ToString();
                if (!string.IsNullOrEmpty(estimatevotemoney))
                {
                    up += "estimatevotemoney=" + dbc.ToSqlValue(estimatevotemoney) + ",";
                }
                String isoilbyestimate = jsr["isoilbyestimate"].ToString();
                String estimateoilmoney = jsr["estimateoilmoney"].ToString();
                if (!string.IsNullOrEmpty(estimateoilmoney))
                {
                    up += "estimateoilmoney=" + dbc.ToSqlValue(estimateoilmoney) + ",";
                }

                if (!string.IsNullOrEmpty(up))
                {
                    up = ", " + up;
                }

                string sql = "update tb_b_sourcegoodsinfo_offer set operatorid=" + dbc.ToSqlValue(SystemUser.CurrentUser.MQUserID) + ",flowstatus = 20,estimatemoney=" + dbc.ToSqlValue(estimatemoney) + ",estimatecarriertype=" + dbc.ToSqlValue(estimatecarriertype) + ",istakegoodsbyestimate=" + dbc.ToSqlValue(istakegoodsbyestimate) + ",isvotebyestimate=" + dbc.ToSqlValue(isvotebyestimate) + ",isoilbyestimate=" + dbc.ToSqlValue(isoilbyestimate) + up.TrimEnd(',') + @"  
    where offerid=" + dbc.ToSqlValue(offerid);
                dbc.ExecuteNonQuery(sql);

                sql = "update tb_b_shippingnoteinfo set operatorid = " + dbc.ToSqlValue(SystemUser.CurrentUser.UserID);
                dbc.ExecuteNonQuery(sql);

                DateTime ti = DateTime.Now;
                var dt = dbc.GetEmptyDataTable("tb_b_sourcegoodsinfo_offer_flow");
                var newDr = dt.NewRow();
                newDr["flowid"] = Guid.NewGuid();
                newDr["offerid"] = offerid;
                newDr["flowstatus"] = 20;
                newDr["status"] = 0;
                newDr["adduser"] = SystemUser.CurrentUser.UserID;
                newDr["addtime"] = ti;
                newDr["updateuser"] = SystemUser.CurrentUser.UserID;
                newDr["updatetime"] = ti;
                dt.Rows.Add(newDr);
                dbc.InsertTable(dt);

                /*insert into tb_b_sourcegoodsinfo_offer_record;recrodtype = "操作部已填写询价提交给市场部"，recordmemo = "xxx 在xxx时间 操作部已填写询价提交给市场部"；*/
                LogBySourcegoodsinfoOffer(dbc, offerid, "操作部已填写询价提交给市场部", SystemUser.CurrentUser.UserName + "在" + ti + "操作部已填写询价提交给市场部", ti);
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

    [CSMethod("updateOfferNew")]
    public bool updateOfferNew(string offerid, JSReader jsr)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                String sql = @"select a.*,b.isneededit,b.accountrate,b.completerate,b.cashrate,b.nooilmoney,c.shippingnoteid,b.advancerate,b.operaterate,b.grossrate,b.oilmoneyrate,b.invoicerate from tb_b_sourcegoodsinfo_offer a
            left join tb_b_user b on a.shipperid=b.userid
left join tb_b_shippingnoteinfo c on a.offerid=c.offerid
where a.offerid=" + dbc.ToSqlValue(offerid);
                DataTable dt = dbc.ExecuteDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    decimal estimatemoney = string.IsNullOrEmpty(jsr["estimatemoney"].ToString()) ? 0 : Convert.ToDecimal(jsr["estimatemoney"].ToString());//预估下游成本
                    decimal estimateoilmoney = string.IsNullOrEmpty(jsr["estimateoilmoney"].ToString()) ? 0 : Convert.ToDecimal(jsr["estimateoilmoney"].ToString());//用油金额
                    decimal estimatevotemoney = string.IsNullOrEmpty(jsr["estimatevotemoney"].ToString()) ? 0 : Convert.ToDecimal(jsr["estimatevotemoney"].ToString());//开票金额
                    /*
                     * estimateautomoney = （
                     * estimatemoney
                     * +（estimatemoney*tb_b_user.advancerate*实际天数）
                     * + estimatemoney*tb_b_user.operaterate 
                     * + estimatemoney*tb_b_user.grossrate 
                     * - estimateoilmoney*tb_b_user.oilmoneyrate 
                     * - estimatevotemoney*tb_b_user.invoicerate
                     * ）
                    */

                    DateTime dateNow = DateTime.Now;
                    DateTime dateNext = DateTime.Now.AddMonths(1);
                    int currentDays = DateTime.DaysInMonth(dateNow.Year, dateNow.Month) - DateTime.Today.Day + 1;//这个月还剩几天
                    int nextDays = DateTime.DaysInMonth(dateNext.Year, dateNext.Month);//下个月天数
                    int actualDays = currentDays + nextDays;//实际天数

                    string shippingnoteid = dt.Rows[0]["shippingnoteid"].ToString();
                    decimal estimateautomoney = 0m;
                    int isneededit = string.IsNullOrEmpty(dt.Rows[0]["isneededit"].ToString()) ? 0 : Convert.ToInt32(dt.Rows[0]["isneededit"].ToString());//是否开启修正（0：开启；1：不开启；）默认为1
                    decimal accountrate = string.IsNullOrEmpty(dt.Rows[0]["accountrate"].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[0]["accountrate"].ToString());//帐期财务汇率（1-1.5）
                    decimal advancerate = string.IsNullOrEmpty(dt.Rows[0]["advancerate"].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[0]["advancerate"].ToString());//垫资比例 ⭐⭐
                    decimal operaterate = string.IsNullOrEmpty(dt.Rows[0]["operaterate"].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[0]["operaterate"].ToString());//运营比例 ⭐⭐
                    decimal grossrate = string.IsNullOrEmpty(dt.Rows[0]["grossrate"].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[0]["grossrate"].ToString());//毛利比例 ⭐⭐
                    decimal oilmoneyrate = string.IsNullOrEmpty(dt.Rows[0]["oilmoneyrate"].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[0]["oilmoneyrate"].ToString());//用油比例 ⭐⭐
                    decimal invoicerate = string.IsNullOrEmpty(dt.Rows[0]["invoicerate"].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[0]["invoicerate"].ToString());//开票比例 ⭐⭐
                    decimal completerate = string.IsNullOrEmpty(dt.Rows[0]["completerate"].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[0]["completerate"].ToString());//综合成本汇率（0-1）
                    int cashrate = string.IsNullOrEmpty(dt.Rows[0]["cashrate"].ToString()) ? 0 : Convert.ToInt32(dt.Rows[0]["cashrate"].ToString());//押金成本，整数
                    decimal nooilmoney = string.IsNullOrEmpty(dt.Rows[0]["nooilmoney"].ToString()) ? 0 : Convert.ToDecimal(dt.Rows[0]["nooilmoney"].ToString());//未用油汇率（0-1）
                    estimateautomoney = decimal.Round((estimatemoney
                        + (estimatemoney * advancerate * actualDays)
                        + estimatemoney * operaterate
                        + estimatemoney * grossrate
                        - estimateoilmoney * oilmoneyrate
                        - estimatevotemoney * invoicerate) / completerate, 2);

                    int money = estimateautomoney % 10 > 0 ? (int)(estimateautomoney + 10) / 10 * 10 : (int)(estimateautomoney / 10) * 10;
                    dbc.BeginTransaction();
                    if (isneededit == 0)
                    {
                        sql = @"update tb_b_sourcegoodsinfo_offer set operatorid=" + dbc.ToSqlValue(SystemUser.CurrentUser.MQUserID) + ",flowstatus = 20,estimateautomoney=" + dbc.ToSqlValue(money) + ",estimatemoney=" + dbc.ToSqlValue(estimatemoney) + @"
    where offerid=" + dbc.ToSqlValue(offerid);
                        dbc.ExecuteNonQuery(sql);

                        DateTime ti = DateTime.Now;
                        var dt_flow = dbc.GetEmptyDataTable("tb_b_sourcegoodsinfo_offer_flow");
                        var newDr = dt_flow.NewRow();
                        newDr["flowid"] = Guid.NewGuid();
                        newDr["offerid"] = offerid;
                        newDr["flowstatus"] = 20;
                        newDr["status"] = 0;
                        newDr["adduser"] = SystemUser.CurrentUser.UserID;
                        newDr["addtime"] = ti;
                        newDr["updateuser"] = SystemUser.CurrentUser.UserID;
                        newDr["updatetime"] = ti;
                        dt_flow.Rows.Add(newDr);
                        dbc.InsertTable(dt_flow);

                        LogBySourcegoodsinfoOffer(dbc, offerid, "操作部已填写询价提交给市场部", SystemUser.CurrentUser.UserName + "在" + ti + "操作部已填写询价提交给市场部", ti);
                    }
                    else if (isneededit == 1)
                    {
                        sql = @"update tb_b_sourcegoodsinfo_offer set operatorid=" + dbc.ToSqlValue(SystemUser.CurrentUser.MQUserID) + ",flowstatus = 90,totalmonetaryamount=" + dbc.ToSqlValue(money) + ",estimatemoney=" + dbc.ToSqlValue(estimatemoney) + @"
    where offerid=" + dbc.ToSqlValue(offerid);
                        dbc.ExecuteNonQuery(sql);

                        DateTime ti = DateTime.Now;
                        var dt_flow = dbc.GetEmptyDataTable("tb_b_sourcegoodsinfo_offer_flow");
                        var newDr = dt_flow.NewRow();
                        newDr["flowid"] = Guid.NewGuid();
                        newDr["offerid"] = offerid;
                        newDr["flowstatus"] = 90;
                        newDr["status"] = 0;
                        newDr["adduser"] = SystemUser.CurrentUser.UserID;
                        newDr["addtime"] = ti;
                        newDr["updateuser"] = SystemUser.CurrentUser.UserID;
                        newDr["updatetime"] = ti;
                        dt_flow.Rows.Add(newDr);
                        dbc.InsertTable(dt_flow);

                        /*var dt_oper = dbc.GetEmptyDataTable("tb_b_shippingnoteinfo_operator");
                        var newDr2 = dt_oper.NewRow();
                        newDr2["id"] = Guid.NewGuid();
                        newDr2["shippingnoteid"] = shippingnoteid;
                        newDr2["userid"] = SystemUser.CurrentUser.MQUserID;
                        newDr2["status"] = 0;
                        newDr2["adduser"] = SystemUser.CurrentUser.UserID;
                        newDr2["addtime"] = ti;
                        newDr2["updateuser"] = SystemUser.CurrentUser.UserID;
                        newDr2["updatetime"] = ti;
                        dt_oper.Rows.Add(newDr2);
                        dbc.InsertTable(dt_oper);*/

                        LogBySourcegoodsinfoOffer(dbc, offerid, "市场部询价单已提交企业", SystemUser.CurrentUser.UserName + "在" + ti + "市场部询价单已提交企业", ti);

                    }
                    dbc.CommitTransaction();
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }

        }
    }

    /// <summary>
    /// 转操作部填报价格
    /// </summary>
    /// <param name="offerid"></param>
    /// <returns></returns>
    [CSMethod("ToWritePrice")]
    public bool ToWritePrice(String offerid)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                dbc.BeginTransaction();

                string sql = "update tb_b_sourcegoodsinfo_offer set flowstatus=10,businessid=" + dbc.ToSqlValue(SystemUser.CurrentUser.MQUserID) + " where offerid=" + dbc.ToSqlValue(offerid);
                dbc.ExecuteNonQuery(sql);

                DateTime ti = DateTime.Now;
                var dt = dbc.GetEmptyDataTable("tb_b_sourcegoodsinfo_offer_flow");
                var newDr = dt.NewRow();
                newDr["flowid"] = Guid.NewGuid();
                newDr["offerid"] = offerid;
                newDr["flowstatus"] = 10;
                newDr["status"] = 0;
                newDr["adduser"] = SystemUser.CurrentUser.UserID;
                newDr["addtime"] = ti;
                newDr["updateuser"] = SystemUser.CurrentUser.UserID;
                newDr["updatetime"] = ti;
                dt.Rows.Add(newDr);
                dbc.InsertTable(dt);

                /*insert into tb_b_sourcegoodsinfo_offer_record;recrodtype = "询价单市场部转操作部"，recordmemo = "xxx 在xxx时间 询价单市场部转操作部*/
                LogBySourcegoodsinfoOffer(dbc, offerid, "询价单市场部转操作部", SystemUser.CurrentUser.UserName + "在" + ti + "询价单市场部转操作部", ti);
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
    /// 市场部企业报价
    /// </summary>
    /// <param name="offerid"></param>
    /// <param name="jsr"></param>
    /// <returns></returns>
    [CSMethod("UpdateMktOffer")]
    public bool UpdateMktOffer(string offerid, JSReader jsr)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                dbc.BeginTransaction();

                String totalmonetaryamount = jsr["totalmonetaryamount"].ToString();
                String estimatemoney = jsr["estimatemoney"].ToString();
                String estimatecompletemoney = jsr["estimatecompletemoney"].ToString();
                String estimatetaxmoney = jsr["estimatetaxmoney"].ToString();
                String estimatecostmoney = jsr["estimatecostmoney"].ToString();

                string sql = "update tb_b_sourcegoodsinfo_offer set flowstatus = 90,totalmonetaryamount=" + dbc.ToSqlValue(totalmonetaryamount) + ",estimatemoney=" + dbc.ToSqlValue(estimatemoney) + ",estimatecompletemoney=" + dbc.ToSqlValue(estimatecompletemoney) + ",estimatetaxmoney=" + dbc.ToSqlValue(estimatetaxmoney) + ",estimatecostmoney=" + dbc.ToSqlValue(estimatecostmoney) + "   where offerid=" + dbc.ToSqlValue(offerid);
                dbc.ExecuteNonQuery(sql);

                DateTime ti = DateTime.Now;
                var dt = dbc.GetEmptyDataTable("tb_b_sourcegoodsinfo_offer_flow");
                var newDr = dt.NewRow();
                newDr["flowid"] = Guid.NewGuid();
                newDr["offerid"] = offerid;
                newDr["flowstatus"] = 90;
                newDr["status"] = 0;
                newDr["adduser"] = SystemUser.CurrentUser.UserID;
                newDr["addtime"] = ti;
                newDr["updateuser"] = SystemUser.CurrentUser.UserID;
                newDr["updatetime"] = ti;
                dt.Rows.Add(newDr);
                dbc.InsertTable(dt);

                /*insert into tb_b_sourcegoodsinfo_offer_record;recrodtype = "市场部询价单已提交企业"，recordmemo = "xxx 在xxx时间 市场部询价单已提交企业"；*/
                LogBySourcegoodsinfoOffer(dbc, offerid, "市场部询价单已提交企业", SystemUser.CurrentUser.UserName + "在" + ti + "市场部询价单已提交企业", ti);
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


    [CSMethod("UpdateMktOfferNew")]
    public bool UpdateMktOfferNew(string offerid, JSReader jsr)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                String sql = @"select a.*,b.isneededit,b.accountrate,b.completerate,b.cashrate,b.nooilmoney,c.shippingnoteid from tb_b_sourcegoodsinfo_offer a
            left join tb_b_user b on a.shipperid=b.userid
left join tb_b_shippingnoteinfo c on a.offerid=c.offerid
where a.offerid=" + dbc.ToSqlValue(offerid);
                DataTable dt = dbc.ExecuteDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    dbc.BeginTransaction();
                    string shippingnoteid = dt.Rows[0]["shippingnoteid"].ToString();
                    decimal estimateautomoney = string.IsNullOrEmpty(jsr["estimateautomoney"].ToString()) ? 0 : Convert.ToDecimal(jsr["estimateautomoney"].ToString());

                    //int money = estimateautomoney % 10 > 0 ? (int)(estimateautomoney + 10) / 10 * 10 : (int)(estimateautomoney / 10) * 10;

                    sql = @"update tb_b_sourcegoodsinfo_offer set operatorid=" + dbc.ToSqlValue(SystemUser.CurrentUser.MQUserID) + ",flowstatus = 90,estimateautomoney=" + dbc.ToSqlValue(estimateautomoney) + ",totalmonetaryamount=" + dbc.ToSqlValue(estimateautomoney) + @"
    where offerid=" + dbc.ToSqlValue(offerid);
                    dbc.ExecuteNonQuery(sql);

                    DateTime ti = DateTime.Now;
                    var dt_flow = dbc.GetEmptyDataTable("tb_b_sourcegoodsinfo_offer_flow");
                    var newDr = dt_flow.NewRow();
                    newDr["flowid"] = Guid.NewGuid();
                    newDr["offerid"] = offerid;
                    newDr["flowstatus"] = 90;
                    newDr["status"] = 0;
                    newDr["adduser"] = SystemUser.CurrentUser.UserID;
                    newDr["addtime"] = ti;
                    newDr["updateuser"] = SystemUser.CurrentUser.UserID;
                    newDr["updatetime"] = ti;
                    dt_flow.Rows.Add(newDr);
                    dbc.InsertTable(dt_flow);

                    /*var dt_oper = dbc.GetEmptyDataTable("tb_b_shippingnoteinfo_operator");
                    var newDr2 = dt_oper.NewRow();
                    newDr2["id"] = Guid.NewGuid();
                    newDr2["shippingnoteid"] = shippingnoteid;
                    newDr2["userid"] = SystemUser.CurrentUser.MQUserID;
                    newDr2["status"] = 0;
                    newDr2["adduser"] = SystemUser.CurrentUser.UserID;
                    newDr2["addtime"] = ti;
                    newDr2["updateuser"] = SystemUser.CurrentUser.UserID;
                    newDr2["updatetime"] = ti;
                    dt_oper.Rows.Add(newDr2);
                    dbc.InsertTable(dt_oper);*/

                    LogBySourcegoodsinfoOffer(dbc, offerid, "市场部询价单已提交企业", SystemUser.CurrentUser.UserName + "在" + ti + "市场部询价单已提交企业", ti);
                    dbc.CommitTransaction();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }

    /// <summary>
    /// 组成成分
    /// </summary>
    /// <param name="offerid"></param>
    /// <param name="jsr"></param>
    /// <returns></returns>
    [CSMethod("UpdateConstituteOffer")]
    public bool UpdateConstituteOffer(string offerid, JSReader jsr)
    {
        using (MySqlDbConnection dbc = MySqlConnstr.GetDBConnection())
        {
            try
            {
                decimal estimateautomoney = string.IsNullOrEmpty(jsr["estimateautomoney"].ToString()) ? 0 : Convert.ToDecimal(jsr["estimateautomoney"].ToString());//预估自动计算金额
                decimal estimatecompletemoney = string.IsNullOrEmpty(jsr["estimatecompletemoney"].ToString()) ? 0 : Convert.ToDecimal(jsr["estimatecompletemoney"].ToString());//预估综合成本
                decimal estimatetaxmoney = string.IsNullOrEmpty(jsr["estimatetaxmoney"].ToString()) ? 0 : Convert.ToDecimal(jsr["estimatetaxmoney"].ToString());//预估税费成本
                decimal estimatecostmoney = string.IsNullOrEmpty(jsr["estimatecostmoney"].ToString()) ? 0 : Convert.ToDecimal(jsr["estimatecostmoney"].ToString());//预估资金成本

                if (estimateautomoney != 0 && (estimateautomoney == estimatecompletemoney + estimatetaxmoney + estimatecostmoney))
                {
                    string sql = @"update tb_b_sourcegoodsinfo_offer set estimatecompletemoney=" + dbc.ToSqlValue(estimatecompletemoney) + ",estimatetaxmoney=" + dbc.ToSqlValue(estimatetaxmoney) + ",estimatecostmoney=" + dbc.ToSqlValue(estimatecostmoney) + @"
    where offerid=" + dbc.ToSqlValue(offerid);
                    dbc.ExecuteNonQuery(sql);

                    return true;
                }
                else
                {
                    throw new Exception("组成成分填写错误！");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public void LogBySourcegoodsinfoOffer(MySqlDbConnection dbc, string offerid, string recordtype, string recordmemo, DateTime ti)
    {
        var dt = dbc.GetEmptyDataTable("tb_b_sourcegoodsinfo_offer_record");
        var dr = dt.NewRow();
        dr["id"] = Guid.NewGuid();
        dr["offerid"] = offerid;
        dr["recordtype"] = recordtype;
        dr["recordmemo"] = recordmemo;
        dr["status"] = 0;
        dr["adduser"] = SystemUser.CurrentUser.UserID;
        dr["addtime"] = ti;
        dr["updateuser"] = SystemUser.CurrentUser.UserID;
        dr["updatetime"] = ti;
        dt.Rows.Add(dr);
        dbc.InsertTable(dt);
    }
}