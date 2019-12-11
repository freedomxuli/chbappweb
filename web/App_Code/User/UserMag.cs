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
using WxPayAPI;
using LitJson;
/// <summary>
///UserMag 的摘要说明
/// </summary>
public class FJ
{
    public string fjId { get; set; }
    public string fileName { get; set; }
    public string fileFullUrl { get; set; }
}
[CSClass("YHGLClass")]
public class UserMag
{
    [CSMethod("GetUserListTotal")]
    public object GetUserListTotal(string jsid, string xm)
    {
        if (!string.IsNullOrEmpty(jsid))
        {
            try
            {
                Guid guid = new Guid(jsid);
            }
            catch
            {
                throw new Exception("角色ID出错！");
            }
        }

        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string where = "";
                if (!string.IsNullOrEmpty(jsid))
                {
                    where += " and User_ID in (SELECT User_ID FROM tb_b_User_JS_Gl where JS_ID='" + jsid + "' and delflag=0 )";
                }

                if (!string.IsNullOrEmpty(xm.Trim()))
                {
                    where += " and " + dbc.C_Like("User_XM", xm.Trim(), LikeStyle.LeftAndRightLike);
                }

                string str = "select * from tb_b_Users where User_DelFlag=0  ";
                str += where;
                str += " order by LoginName,User_XM";

                DataTable dt = dbc.ExecuteDataTable(str);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }

    [CSMethod("UploadPicForProduct", 1)]
    public object UploadPicForProduct(FileData[] fds, string UserID)
    {
        string ServiceURL = System.Configuration.ConfigurationManager.AppSettings["ServiceURL"].ToString();
        WebRequest request = (HttpWebRequest)WebRequest.Create(ServiceURL + "uploadMultipleFiles");
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
        string _url = ServiceURL + "tbbuserphoto.update";
        string jsonParam = new JavaScriptSerializer().Serialize(new
        {
            userid = UserID,
            userphotogltype = 0,
            fileList = list
        });
        var request1 = (HttpWebRequest)WebRequest.Create(_url);
        request1.Method = "POST";
        request1.ContentType = "application/json;charset=UTF-8";
        var byteData = Encoding.UTF8.GetBytes(jsonParam);
        var length = byteData.Length;
        request1.ContentLength = length;
        var writer = request1.GetRequestStream();
        writer.Write(byteData, 0, length);
        writer.Close();
        var response = (HttpWebResponse)request1.GetResponse();
        var responseString = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();
        return new { fileurl = list[0].fileFullUrl, isdefault = 0, fileid = list[0].fjId };

    }


    [CSMethod("DelProductImageByPicID")]
    public bool DelProductImageByPicID(string fj_id, string userid, int userphotogltype)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                dbc.BeginTransaction();
                string sqlStr = "update tb_b_FJ set STATUS = 1,UPDATETIME = getdate(),XGYH_ID = @XGYH_ID where fj_id = @fj_id ";
                SqlCommand cmd = new SqlCommand(sqlStr);
                cmd.Parameters.AddWithValue("@XGYH_ID", DBNull.Value);
                cmd.Parameters.AddWithValue("@fj_id", fj_id);
                dbc.ExecuteNonQuery(cmd);

                sqlStr = @"update tb_b_userphoto set Status = 1,updatetime = getdate(),updateuser = @XGYH_ID where FJ_ID = @fj_id and UserId=@UserId and userphotogltype=@userphotogltype";
                cmd = new SqlCommand(sqlStr);
                cmd.Parameters.AddWithValue("@XGYH_ID", DBNull.Value);
                cmd.Parameters.AddWithValue("@fj_id", fj_id);
                cmd.Parameters.AddWithValue("@UserId", userid);
                cmd.Parameters.AddWithValue("@userphotogltype", userphotogltype);
                dbc.ExecuteNonQuery(cmd);

                dbc.CommitTransaction();
                return true;
            }
            catch
            {
                dbc.RoolbackTransaction();
                return false;
            }
        }
    }

    [CSMethod("GetProductImages")]
    public object GetProductImages(string pid)
    {

        string _url = System.Configuration.ConfigurationManager.AppSettings["ServiceURL"].ToString() + "tbbuserphoto.selectUserphoto";
        string jsonParam = new JavaScriptSerializer().Serialize(new
        {
            tradeCode = "tbbuserphoto.selectUserphoto",
            userid = pid,
            userphotogltype = 0
        });
        var request1 = (HttpWebRequest)WebRequest.Create(_url);
        request1.Method = "POST";
        request1.ContentType = "application/json;charset=UTF-8";
        var byteData = Encoding.UTF8.GetBytes(jsonParam);
        var length = byteData.Length;
        request1.ContentLength = length;
        var writer = request1.GetRequestStream();
        writer.Write(byteData, 0, length);
        writer.Close();
        var response = (HttpWebResponse)request1.GetResponse();
        var responseString = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();

        return responseString;

    }

    /// <summary>
    /// Resize图片
    /// </summary>
    /// <param name="bmp">原始Bitmap</param>
    /// <param name="newW">新的宽度</param>
    /// <param name="newH">新的高度</param>
    /// <param name="Mode">保留着，暂时未用</param>
    /// <returns>处理以后的图片</returns>
    public static byte[] KiResizeImage(byte[] stream)
    {
        try
        {

            Stream bmp = new MemoryStream(stream);

            Bitmap bitMap = new Bitmap(bmp);//创建bitmap
            bmp.Dispose();

            int x = 0;
            int y = 0;
            int width = 500;//缩放后的图的高，1280缩小成这个特定的高度
            int height = 375;//缩放后的图的宽，768缩小成这个特定的宽度
            Bitmap bitMap2 = new Bitmap(width, height, PixelFormat.Format32bppArgb);//缩放后的新图

            Graphics gImg = Graphics.FromImage(bitMap2);
            gImg.CompositingQuality = CompositingQuality.HighQuality;
            gImg.InterpolationMode = InterpolationMode.High;
            // 指定高质量插值法。 指定这个算法缩放图片后，得到的这张特定图片是黑图，而指定高质量的双三次插值法InterpolationMode.HighQualityBicubic也会生成黑图，而指定其他算法就没有问题。另外，缩放成其他大小也没有问题，比如width=547, height=328。
            gImg.SmoothingMode = SmoothingMode.HighQuality;
            gImg.Clear(Color.Transparent);
            gImg.DrawImage(bitMap, x, y, width, height);

            //Bitmap 转化为 Byte[]       
            byte[] bReturn = null;
            MemoryStream ms = new MemoryStream();
            bitMap2.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            bReturn = ms.GetBuffer();

            bitMap.Dispose();
            gImg.Dispose();
            bitMap2.Dispose();
            return bReturn;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static bool getJs_cj()
    {
        var user = SystemUser.CurrentUser;
        string userid = user.UserID;

        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                string str = "SELECT * FROM tb_b_User_JS_Gl where delflag=0 AND JS_ID='F6613AFB-06E2-454A-881F-8C51483976F3' and USER_ID='" + userid + "'";
                DataTable dt = dbc.ExecuteDataTable(str);
                if (dt.Rows.Count == 1)
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

    [CSMethod("GetOrderList")]
    public object GetOrderList(string userid)
    {
        using (var db = new DBConnection())
        {
            try
            {
                string str = @"select b.UserXM as wuliu,c.UserName as UserName,a.AddTime,a.Points as MONEY,'交易运费券' as KIND  from tb_b_pay a left join tb_b_user b on a.CardUserID=b.UserID
                            left join tb_b_user c on a.PayUserID=c.UserID where PayUserID=" + db.ToSqlValue(userid) + @"
                            union all 
                            select b.UserXM as wuliu,c.UserName as UserName,a.AddTime,a.Points as MONEY,'收到运费券' as KIND  from tb_b_pay a left join tb_b_user b on a.CardUserID=b.UserID
                            left join tb_b_user c on a.PayUserID=c.UserID where ReceiveUserID=" + db.ToSqlValue(userid) + @"
                            union all 
	                        select b.UserXM as wuliu,b.UserXM as UserName,a.AddTime,a.Points as MONEY,'购买运费券' as KIND  from tb_b_order a left join tb_b_user b on a.SaleUserID=b.UserID
                            where a.BuyUserID=" + db.ToSqlValue(userid) + @" and a.Status=0 and a.ZhiFuZT=1
                            union all
                            select b.UserXM as wuliu,'查货宝' as UserName,a.AddTime,a.points as MONEY,'支付平台运费券' as KIND  from tb_b_givetoplat a left join tb_b_user b on a.UserID=b.UserID
                            where a.UserID=" + db.ToSqlValue(userid) + @" and a.Status=0 and a.IsSH=1
                            union all
                             select '查货宝' as wuliu, b.UserXM as UserName,a.sqrq as AddTime,a.sqjf as MONEY,'申请运费券' as KIND  from tb_b_jfsq a left join tb_b_user b on a.UserID=b.UserID
                            where a.UserID=" + db.ToSqlValue(userid) + @"  and a.issq=1
                            order by AddTime desc";
                DataTable dt = db.ExecuteDataTable(str);

                dt.Columns.Add("DATE");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["addtime"] != null && dt.Rows[i]["addtime"].ToString() != "")
                    {
                        dt.Rows[i]["DATE"] = Convert.ToDateTime(dt.Rows[i]["addtime"]).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }

                str = @"select a.points MONEY,c.UserXM as UserName from tb_b_mycard a left join tb_b_user b on a.UserID=b.UserID
                        left join tb_b_user c on a.CardUserID=c.UserID
                        where a.UserID=" + db.ToSqlValue(userid) + " and a.status=0 order by a.points";
                DataTable dt2 = db.ExecuteDataTable(str);

                return new { dt = dt, dt2 = dt2 };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetOrderListByZX")]
    public object GetOrderListByZX(int pagnum, int pagesize, string kind, string btime, string etime, string mc)
    {
        using (var db = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where1 = "";
                string where2 = "";
                string where3 = "";
                if (!string.IsNullOrEmpty(kind))
                {
                    switch (kind)
                    {
                        case "1":
                            where1 = " and a.CardUserID = '0'";
                            where2 = " and a.UserID = '0'";
                            break;
                        case "2":
                            where1 = " and a.CardUserID = '0'";
                            where3 = " and a.UserID = '0'";
                            break;
                        case "3":
                            where2 = " and a.UserID = '0'";
                            where3 = " and a.UserID = '0'";
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(btime) && !string.IsNullOrEmpty(etime))
                {
                    where1 += " and a.AddTime >= " + db.ToSqlValue(Convert.ToDateTime(btime)) + " and a.AddTime < " + db.ToSqlValue(Convert.ToDateTime(etime).AddDays(1));
                    where2 += " and a.AddTime >= " + db.ToSqlValue(Convert.ToDateTime(btime)) + " and a.AddTime < " + db.ToSqlValue(Convert.ToDateTime(etime).AddDays(1));
                    where3 += " and a.sqrq >= " + db.ToSqlValue(Convert.ToDateTime(btime)) + " and a.sqrq < " + db.ToSqlValue(Convert.ToDateTime(etime).AddDays(1));
                }

                if (!string.IsNullOrEmpty(mc))
                {
                    where1 += " and " + db.C_Like("b.UserXM", mc, LikeStyle.LeftAndRightLike);
                    where2 += " and " + db.C_Like("b.UserXM", mc, LikeStyle.LeftAndRightLike);
                    where3 += " and " + db.C_Like("b.UserXM", mc, LikeStyle.LeftAndRightLike);
                }

                string str = @"
                            select b.UserXM,c.UserName as UserName,a.AddTime,a.Points as MONEY,'收券' as KIND  from tb_b_pay a left join tb_b_user b on a.CardUserID=b.UserID
                            left join tb_b_user c on a.PayUserID=c.UserID where a.ReceiveUserID = a.CardUserID " + where1 + @"
                            union all 
                            select b.UserXM,'查货宝' as UserName,a.AddTime,a.points as MONEY,'支付' as KIND  from tb_b_givetoplat a left join tb_b_user b on a.UserID=b.UserID
                            where a.Status=0 and a.IsSH=1 " + where2 + @"
                            union all
                            select b.UserXM, '查货宝' as UserName,a.sqrq as AddTime,a.sqjf as MONEY,'申请' as KIND  from tb_b_jfsq a left join tb_b_user b on a.UserID=b.UserID
                            where a.issq=1 " + where3 + @"
                            order by AddTime desc";
                DataTable dt = db.GetPagedDataTable(str, pagesize, ref cp, out ac);

                dt.Columns.Add("DATE");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["addtime"] != null && dt.Rows[i]["addtime"].ToString() != "")
                    {
                        dt.Rows[i]["DATE"] = Convert.ToDateTime(dt.Rows[i]["addtime"]).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }

                return new { dt = dt, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetOrderListBySF")]
    public object GetOrderListBySF(int pagnum, int pagesize, string kind, string btime, string etime, string mc, string zxmc)
    {
        using (var db = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where1 = "";
                string where2 = "";
                string where3 = "";
                if (!string.IsNullOrEmpty(kind))
                {
                    switch (kind)
                    {
                        case "1":
                            where1 = " and a.CardUserID = '0'";
                            where3 = " and a.CardUserID = '0'";
                            break;
                        case "2":
                            where2 = " and a.BuyUserID = '0'";
                            where1 = " and a.CardUserID = '0'";
                            break;
                        case "3":
                            where2 = " and a.BuyUserID = '0'";
                            where3 = " and a.CardUserID = '0'";
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(btime) && !string.IsNullOrEmpty(etime))
                {
                    where1 += " and a.AddTime >= " + db.ToSqlValue(Convert.ToDateTime(btime)) + " and a.AddTime < " + db.ToSqlValue(Convert.ToDateTime(etime).AddDays(1));
                    where2 += " and a.AddTime >= " + db.ToSqlValue(Convert.ToDateTime(btime)) + " and a.AddTime < " + db.ToSqlValue(Convert.ToDateTime(etime).AddDays(1));
                    where3 += " and a.AddTime >= " + db.ToSqlValue(Convert.ToDateTime(btime)) + " and a.AddTime < " + db.ToSqlValue(Convert.ToDateTime(etime).AddDays(1));
                }

                if (!string.IsNullOrEmpty(mc))
                {
                    where1 += " and " + db.C_Like("c.UserName", mc, LikeStyle.LeftAndRightLike);
                    where2 += " and " + db.C_Like("c.UserName", mc, LikeStyle.LeftAndRightLike);
                    where3 += " and " + db.C_Like("c.UserName", mc, LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(zxmc))
                {
                    where1 += " and " + db.C_Like("b.UserXM", zxmc, LikeStyle.LeftAndRightLike);
                    where2 += " and " + db.C_Like("b.UserXM", zxmc, LikeStyle.LeftAndRightLike);
                    where3 += " and " + db.C_Like("b.UserXM", zxmc, LikeStyle.LeftAndRightLike);
                }
                string str = @"select b.UserXM,c.UserName,a.AddTime,a.Points as MONEY,'转让' as KIND,'耗材券' as FLAG  from tb_b_pay a left join tb_b_user b on a.CardUserID=b.UserID
                                left join tb_b_user c on a.PayUserID=c.UserID 
                                left join tb_b_user d on a.ReceiveUserID=d.UserID 
                                where 1=1 and  c.ClientKind=2 and  d.ClientKind=2   " + where1 + @"
                                 union all 
                                select b.UserXM,c.UserName,a.AddTime,a.Points as MONEY,'消费' as KIND,
                               case when f.pointkind=0 then '耗材券' 
                                when  f.pointkind is null then '耗材券'
                                when  f.pointkind=4 then '授权券'
                                when  f.pointkind=1 then '授权券'
                                when  f.pointkind=2 then '授权券'
                                when  f.pointkind=3 then  '自发券' end as FLAG  
                                from tb_b_pay a left join tb_b_user b on a.CardUserID=b.UserID
                                left join tb_b_user c on a.PayUserID=c.UserID 
                                left join tb_b_user d on a.ReceiveUserID=d.UserID
                                left join tb_b_order e on a.OrderCode=e.OrderCode
                                left join tb_b_plattosale f on e.PlatToSaleId=f.PlatToSaleId
                                where 1=1 and  c.ClientKind=2 and  d.ClientKind=1   " + where3 + @"
                            union all 
	                        select b.UserXM,c.UserName,a.AddTime,a.Points as MONEY,'购买' as KIND,
                                case when f.pointkind=0 then '耗材券' 
                                when  f.pointkind is null then '耗材券'
                                when  f.pointkind=4 then '授权券'
                                when  f.pointkind=1 then '授权券'
                                when  f.pointkind=2 then '授权券'
                                when  f.pointkind=3 then  '自发券' end as FLAG  
                            from tb_b_order a left join tb_b_user b on a.SaleUserID=b.UserID
                            left join tb_b_user c on a.BuyUserID=c.UserID
                            left join tb_b_plattosale f on a.PlatToSaleId=f.PlatToSaleId
                            where a.Status=0 and a.ZhiFuZT=1 " + where2 + @"
                            order by AddTime desc";
                DataTable dt = db.GetPagedDataTable(str, pagesize, ref cp, out ac);

                dt.Columns.Add("DATE");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["addtime"] != null && dt.Rows[i]["addtime"].ToString() != "")
                    {
                        dt.Rows[i]["DATE"] = Convert.ToDateTime(dt.Rows[i]["addtime"]).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }

                return new { dt = dt, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetUserList")]
    public object GetUserList(int pagnum, int pagesize, string roleId, string yhm, string xm)
    {
        if (!string.IsNullOrEmpty(roleId))
        {
            try
            {
                Guid guid = new Guid(roleId);
            }
            catch
            {
                throw new Exception("角色ID出错！");
            }
        }

        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(roleId))
                {
                    where += " and a.UserID in (SELECT userId FROM tb_b_user_role where roleId='" + roleId + "')";
                }

                if (!string.IsNullOrEmpty(yhm.Trim()))
                {
                    where += " and " + dbc.C_Like("a.UserName", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(xm.Trim()))
                {
                    where += " and " + dbc.C_Like("a.UserXM", xm.Trim(), LikeStyle.LeftAndRightLike);
                }

                string str = @"select a.*,c.roleName,b.roleId from tb_b_user a left join tb_b_user_role b on a.UserID=b.UserID
                                left join tb_b_roledb c on b.roleId=c.roleId where 1=1 and ClientKind = 0 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.UserName,a.UserXM", pagesize, ref cp, out ac);

                var privilege = new SystemUser().GetPrivilegeList(SystemUser.CurrentUser.RoleID);

                var mmck = false;
                for (int i = 0; i < privilege.Rows.Count; i++)
                {
                    if (privilege.Rows[i]["privilegeName"] != null && privilege.Rows[i]["privilegeName"].ToString() != "")
                    {
                        if (privilege.Rows[i]["privilegeName"].ToString() == "系统维护中心_人员管理_密码查看")
                        {
                            mmck = true;
                        }
                    }
                }

                return new { dt = dtPage, cp = cp, ac = ac, mmck = mmck };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    [CSMethod("GetClientByOpen")]
    public object GetClientByOpen(int pagnum, int pagesize, string UserXM, string opentype)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";

                if (!string.IsNullOrEmpty(UserXM.Trim()))
                {
                    where += " and " + dbc.C_Like("UserXM", UserXM.Trim(), LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(opentype))
                {
                    if (opentype == "1")
                    {
                        where += " and opentype>0 ";
                    }
                    else if (opentype == "2")
                    {
                        where += " and opentype=0 ";
                    }
                }
                string str = @"select * from (
                                select a.UserID,a.UserName,a.UserXM,a.FromRoute,a.ToRoute,a.AddTime,case when b.num is null then 0 else 1 end opentype from tb_b_user a
                                left join (select count(1) num,userid from tb_b_user_pc where status = 0 group by userid) b on a.UserID = b.userid
                                where a.isdriver = 0 and (a.ClientKind = 1 or a.ClientKind = 2)
                            )t where 1=1 ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by AddTime desc,UserName,UserXM", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    [CSMethod("GetModuleByPC")]
    public DataTable GetModuleByPC(string userid)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string sql = @"select a.moduleId,a.moduleName,b.id,case when b.id is null then 0 else 1 end moduleType from tb_b_module_pc a
left join tb_b_module_pc_zx b on a.moduleId = b.moduleId
and b.userpcid in (select userpcid from tb_b_user_pc where userid = " + dbc.ToSqlValue(userid) + @" and status = 0)";
                DataTable dt = dbc.ExecuteDataTable(sql);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("OpenPc")]
    public bool OpenPc(string[] ids, string userid)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                var CurrentUserId = SystemUser.CurrentUser.UserID;
                string sql = "select count(1) num from tb_b_user_pc where status = 0 and userid = " + dbc.ToSqlValue(userid);
                int num = Convert.ToInt32(dbc.ExecuteScalar(sql).ToString());

                string userpcid = Guid.NewGuid().ToString();

                if (num != 0)
                {
                    return false;
                }
                else
                {
                    #region 插入专线pc用户关联表
                    DataTable dt = dbc.GetEmptyDataTable("tb_b_user_pc");
                    DataRow dr = dt.NewRow();
                    dr["userpcid"] = userpcid;
                    dr["userid"] = userid;
                    dr["status"] = 0;
                    dr["adduser"] = CurrentUserId;
                    dr["addtime"] = DateTime.Now;
                    dt.Rows.Add(dr);
                    dbc.InsertTable(dt);
                    #endregion

                    #region 插入模块pc关联表
                    DataTable dt_1 = dbc.GetEmptyDataTable("tb_b_module_pc_zx");
                    for (var i = 0; i < ids.Length; i++)
                    {
                        DataRow dr1 = dt_1.NewRow();
                        dr1["id"] = Guid.NewGuid();
                        dr1["moduleId"] = ids[i];
                        dr1["userpcid"] = userpcid;
                        dr1["status"] = 0;
                        dr1["adduser"] = CurrentUserId;
                        dr1["addtime"] = DateTime.Now;
                        dt_1.Rows.Add(dr1);
                    }
                    dbc.InsertTable(dt_1);
                    #endregion

                    #region 插入管理员权限表
                    DataTable dt_2 = dbc.GetEmptyDataTable("tb_b_roledb_pc");
                    DataRow dr2 = dt_2.NewRow();
                    dr2["roleId"] = Guid.NewGuid();
                    dr2["roleName"] = "管理员";
                    dr2["userpcid"] = userpcid;
                    dr2["rolePx"] = 0;
                    dt_2.Rows.Add(dr2);
                    dbc.InsertTable(dt_2);
                    #endregion

                    #region 插入用户角色关联表
                    DataTable dt_3 = dbc.GetEmptyDataTable("tb_b_user_pc_role");
                    DataRow dr3 = dt_3.NewRow();
                    dr3["userroleId"] = Guid.NewGuid();
                    dr3["userId"] = userid;
                    dr3["roleId"] = dr2["roleId"];
                    dr3["userpcid"] = userpcid;
                    dt_3.Rows.Add(dr3);
                    dbc.InsertTable(dt_3);
                    #endregion

                    #region 插入菜单模块表
                    sql = "select menuId from tb_b_menu_pc where " + dbc.C_In("moduleId", ids);
                    DataTable dt_4 = dbc.ExecuteDataTable(sql);

                    DataTable dt_5 = dbc.GetEmptyDataTable("tb_b_menu_pc_role");
                    for (var i = 0; i < dt_4.Rows.Count; i++)
                    {
                        DataRow dr5 = dt_5.NewRow();
                        dr5["id"] = Guid.NewGuid();
                        dr5["menuId"] = dt_4.Rows[i]["menuId"].ToString();
                        dr5["roleId"] = dr2["roleId"];
                        dr5["userpcid"] = userpcid;
                        dt_5.Rows.Add(dr5);
                    }
                    dbc.InsertTable(dt_5);
                    #endregion

                    #region 插入角色权限关联
                    sql = "select privilegeId from tb_b_privilege_pc where " + dbc.C_In("moduleId", ids);
                    DataTable dt_6 = dbc.ExecuteDataTable(sql);

                    DataTable dt_7 = dbc.GetEmptyDataTable("tb_b_privilege_pc_role");
                    for (var i = 0; i < dt_6.Rows.Count; i++)
                    {
                        DataRow dr7 = dt_7.NewRow();
                        dr7["id"] = Guid.NewGuid();
                        dr7["privilegeId"] = dt_6.Rows[i]["privilegeId"].ToString();
                        dr7["roleId"] = dr2["roleId"];
                        dr7["userpcid"] = userpcid;
                        dt_7.Rows.Add(dr7);
                    }
                    dbc.InsertTable(dt_7);
                    #endregion
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

    [CSMethod("ClosePc")]
    public bool ClosePc(string userid)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                dbc.BeginTransaction();

                var CurrentUserId = SystemUser.CurrentUser.UserID;
                string sql = "select count(1) num from tb_b_user_pc where status = 0 and userid = " + dbc.ToSqlValue(userid);
                int num = Convert.ToInt32(dbc.ExecuteScalar(sql).ToString());

                if (num != 0)
                {
                    sql = "select userpcid from tb_b_user_pc where status = 0 and userid = " + dbc.ToSqlValue(userid);
                    string userpcid = dbc.ExecuteScalar(sql).ToString();

                    #region 将关联表status置为1
                    sql = "update tb_b_user_pc set status = 1,updateuser = " + dbc.ToSqlValue(CurrentUserId) + ",updatetime = " + dbc.ToSqlValue(DateTime.Now) + " where userpcid = " + dbc.ToSqlValue(userpcid);
                    dbc.ExecuteNonQuery(sql);
                    #endregion

                    #region 删除模块pc关联表
                    sql = "delete from tb_b_module_pc_zx where userpcid = " + dbc.ToSqlValue(userpcid);
                    dbc.ExecuteNonQuery(sql);
                    #endregion

                    #region 删除管理员权限表
                    sql = "delete from tb_b_roledb_pc where userpcid = " + dbc.ToSqlValue(userpcid);
                    dbc.ExecuteNonQuery(sql);
                    #endregion

                    #region 删除用户角色关联表
                    sql = "delete from tb_b_user_pc_role where userpcid = " + dbc.ToSqlValue(userpcid);
                    dbc.ExecuteNonQuery(sql);
                    #endregion

                    #region 删除菜单模块表
                    sql = "delete from tb_b_menu_pc_role where userpcid = " + dbc.ToSqlValue(userpcid);
                    dbc.ExecuteNonQuery(sql);
                    #endregion

                    #region 删除角色权限关联
                    sql = "delete from tb_b_privilege_pc_role where userpcid = " + dbc.ToSqlValue(userpcid);
                    dbc.ExecuteNonQuery(sql);
                    #endregion

                }
                else
                {
                    return false;
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

    [CSMethod("SelectModule")]
    public bool SelectModule(string[] ids, string userid)
    {
        using (var dbc = new DBConnection())
        {
            try
            {
                dbc.BeginTransaction();

                string CurrentUserId = SystemUser.CurrentUser.UserID;

                string sql = "select userpcid from tb_b_user_pc where status = 0 and userid = " + dbc.ToSqlValue(userid);
                string userpcid = dbc.ExecuteScalar(sql).ToString();

                sql = "select roleId from tb_b_roledb_pc where userpcid = " + dbc.ToSqlValue(userpcid) + @" and roleName = '管理员'";
                string roleId = dbc.ExecuteScalar(sql).ToString();

                sql = @"select moduleId from (
                        select moduleId from tb_b_module_pc where moduleId not in (select moduleId from tb_b_module_pc_zx where userpcid = " + dbc.ToSqlValue(userpcid) + @")
                        ) a where " + dbc.C_In("a.moduleId", ids);
                DataTable dt_module = dbc.ExecuteDataTable(sql);

                #region 删除去掉的模块
                sql = "delete from tb_b_menu_pc_role where menuId in (select menuId from tb_b_menu_pc where " + dbc.C_NotIn("moduleId", ids) + ") and userpcid = " + dbc.ToSqlValue(userpcid);
                dbc.ExecuteNonQuery(sql);

                sql = "delete from tb_b_privilege_pc_role where privilegeId in (select privilegeId from tb_b_privilege_pc where " + dbc.C_NotIn("moduleId", ids) + ") and userpcid = " + dbc.ToSqlValue(userpcid);
                dbc.ExecuteNonQuery(sql);

                sql = "delete from tb_b_module_pc_zx where " + dbc.C_NotIn("moduleId", ids) + " and userpcid = " + dbc.ToSqlValue(userpcid);
                dbc.ExecuteNonQuery(sql);
                #endregion

                #region 插入新增的模块
                if (dt_module.Rows.Count > 0)
                {
                    List<string> ids_new = new List<string>();
                    #region 插入模块pc关联表
                    DataTable dt_1 = dbc.GetEmptyDataTable("tb_b_module_pc_zx");
                    for (var i = 0; i < dt_module.Rows.Count; i++)
                    {
                        DataRow dr1 = dt_1.NewRow();
                        dr1["id"] = Guid.NewGuid();
                        dr1["moduleId"] = dt_module.Rows[i]["moduleId"].ToString();
                        dr1["userpcid"] = userpcid;
                        dr1["status"] = 0;
                        dr1["adduser"] = CurrentUserId;
                        dr1["addtime"] = DateTime.Now;
                        dt_1.Rows.Add(dr1);
                        ids_new.Add(dt_module.Rows[i]["moduleId"].ToString());
                    }
                    dbc.InsertTable(dt_1);
                    #endregion

                    #region 插入菜单模块表
                    sql = "select menuId from tb_b_menu_pc where " + dbc.C_In("moduleId", ids_new.ToArray());
                    DataTable dt_4 = dbc.ExecuteDataTable(sql);

                    DataTable dt_5 = dbc.GetEmptyDataTable("tb_b_menu_pc_role");
                    for (var i = 0; i < dt_4.Rows.Count; i++)
                    {
                        DataRow dr5 = dt_5.NewRow();
                        dr5["id"] = Guid.NewGuid();
                        dr5["menuId"] = dt_4.Rows[i]["menuId"].ToString();
                        dr5["roleId"] = roleId;
                        dr5["userpcid"] = userpcid;
                        dt_5.Rows.Add(dr5);
                    }
                    dbc.InsertTable(dt_5);
                    #endregion

                    #region 插入角色权限关联
                    sql = "select privilegeId from tb_b_privilege_pc where " + dbc.C_In("moduleId", ids_new.ToArray());
                    DataTable dt_6 = dbc.ExecuteDataTable(sql);

                    DataTable dt_7 = dbc.GetEmptyDataTable("tb_b_privilege_pc_role");
                    for (var i = 0; i < dt_6.Rows.Count; i++)
                    {
                        DataRow dr7 = dt_7.NewRow();
                        dr7["id"] = Guid.NewGuid();
                        dr7["privilegeId"] = dt_6.Rows[i]["privilegeId"].ToString();
                        dr7["roleId"] = roleId;
                        dr7["userpcid"] = userpcid;
                        dt_7.Rows.Add(dr7);
                    }
                    dbc.InsertTable(dt_7);
                    #endregion
                }
                #endregion

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

    [CSMethod("GetClientList")]
    public object GetClientList(int pagnum, int pagesize, string roleId, string yhm, string xm, string beg, string end, string isdonate)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(roleId))
                {
                    where += " and a.ClientKind = " + roleId;
                }
                else
                {
                    where += " and (a.ClientKind = 1 or a.ClientKind = 2)";
                }

                if (!string.IsNullOrEmpty(yhm.Trim()))
                {
                    where += " and " + dbc.C_Like("a.UserName", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(xm.Trim()))
                {
                    where += " and " + dbc.C_Like("a.UserXM", xm.Trim(), LikeStyle.LeftAndRightLike);
                }
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.AddTime<='" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(isdonate))
                {
                    where += " and a.isdonate = " + isdonate;
                }

                string str = @"select a.*,c.roleName,b.roleId,d.SalePoints,e.Points as KFGMPoints from tb_b_user a 
                               left join tb_b_user_role b on a.UserID=b.UserID
                               left join tb_b_roledb c on b.roleId=c.roleId
                               left join (select sum(points) SalePoints,UserID from tb_b_plattosale where status = 0 and pointkind=0 and points > 0  group by UserID) d on a.UserID = d.UserID 
                               left join tb_b_platpoints e on a.UserID = e.UserID 
                               where 1=1";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.AddTime desc,a.UserName,a.UserXM", pagesize, ref cp, out ac);

                dtPage.Columns.Add("dqS");
                for (int i = 0; i < dtPage.Rows.Count; i++)
                {
                    if (dtPage.Rows[i]["DqBm"] != null && dtPage.Rows[i]["DqBm"].ToString() != "")
                    {
                        string sql = "select dq_bm from tb_b_dq where dq_bm=(select dq_sj from tb_b_dq where dq_sj<>'000000' and dq_bm=" + dbc.ToSqlValue(dtPage.Rows[i]["DqBm"]) + ")";
                        DataTable dt = dbc.ExecuteDataTable(sql);
                        if (dt.Rows.Count > 0)
                        {
                            dtPage.Rows[i]["dqS"] = dt.Rows[0][0];
                        }
                    }
                }
                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    [CSMethod("GetZXXFToFile", 2)]
    public byte[] GetZXXFToFile(string kind, string btime, string etime, string mc)
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
                cells[0, 0].PutValue("类别");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("专线");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("交易对象");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("交易时间");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 30);
                cells[0, 4].PutValue("交易金额");
                cells[0, 4].SetStyle(style2);

                string where1 = "";
                string where2 = "";
                string where3 = "";
                if (!string.IsNullOrEmpty(kind))
                {
                    switch (kind)
                    {
                        case "1":
                            where1 = " and a.CardUserID = '0'";
                            where2 = " and a.UserID = '0'";
                            break;
                        case "2":
                            where1 = " and a.CardUserID = '0'";
                            where3 = " and a.UserID = '0'";
                            break;
                        case "3":
                            where2 = " and a.UserID = '0'";
                            where3 = " and a.UserID = '0'";
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(btime) && !string.IsNullOrEmpty(etime))
                {
                    where1 += " and a.AddTime >= " + dbc.ToSqlValue(Convert.ToDateTime(btime)) + " and a.AddTime < " + dbc.ToSqlValue(Convert.ToDateTime(etime).AddDays(1));
                    where2 += " and a.AddTime >= " + dbc.ToSqlValue(Convert.ToDateTime(btime)) + " and a.AddTime < " + dbc.ToSqlValue(Convert.ToDateTime(etime).AddDays(1));
                    where3 += " and a.sqrq >= " + dbc.ToSqlValue(Convert.ToDateTime(btime)) + " and a.sqrq < " + dbc.ToSqlValue(Convert.ToDateTime(etime).AddDays(1));
                }

                if (!string.IsNullOrEmpty(mc))
                {
                    where1 += " and " + dbc.C_Like("b.UserXM", mc, LikeStyle.LeftAndRightLike);
                    where2 += " and " + dbc.C_Like("b.UserXM", mc, LikeStyle.LeftAndRightLike);
                    where3 += " and " + dbc.C_Like("b.UserXM", mc, LikeStyle.LeftAndRightLike);
                }

                string str = @"
                            select b.UserXM,c.UserName as UserName,a.AddTime,a.Points as MONEY,'收券' as KIND  from tb_b_pay a left join tb_b_user b on a.CardUserID=b.UserID
                            left join tb_b_user c on a.PayUserID=c.UserID where a.ReceiveUserID = a.CardUserID " + where1 + @"
                            union all 
                            select b.UserXM,'查货宝' as UserName,a.AddTime,a.points as MONEY,'支付' as KIND  from tb_b_givetoplat a left join tb_b_user b on a.UserID=b.UserID
                            where a.Status=0 and a.IsSH=1 " + where2 + @"
                            union all
                            select b.UserXM, '查货宝' as UserName,a.sqrq as AddTime,a.sqjf as MONEY,'申请' as KIND  from tb_b_jfsq a left join tb_b_user b on a.UserID=b.UserID
                            where a.issq=1 " + where3 + @"
                            order by AddTime desc";
                DataTable dt = dbc.ExecuteDataTable(str);

                dt.Columns.Add("DATE");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["addtime"] != null && dt.Rows[i]["addtime"].ToString() != "")
                    {
                        dt.Rows[i]["DATE"] = Convert.ToDateTime(dt.Rows[i]["addtime"]).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["KIND"]);
                    cells[i + 1, 0].SetStyle(style4);
                    cells[i + 1, 1].PutValue(dt.Rows[i]["UserXM"]);
                    cells[i + 1, 1].SetStyle(style4);
                    cells[i + 1, 2].PutValue(dt.Rows[i]["UserName"]);
                    cells[i + 1, 2].SetStyle(style4);
                    cells[i + 1, 3].PutValue(dt.Rows[i]["DATE"]);
                    cells[i + 1, 3].SetStyle(style4);
                    cells[i + 1, 4].PutValue(dt.Rows[i]["MONEY"]);
                    cells[i + 1, 4].SetStyle(style4);
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

    [CSMethod("GetSFXFToFile", 2)]
    public byte[] GetSFXFToFile(string kind, string btime, string etime, string mc, string zxmc)
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
                cells[0, 0].PutValue("类别");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("专线");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("三方");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("交易时间");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 30);
                cells[0, 4].PutValue("交易金额");
                cells[0, 4].SetStyle(style2);
                cells[0, 5].PutValue("券类型");
                cells[0, 5].SetStyle(style2);

                string where1 = "";
                string where2 = "";
                string where3 = "";
                if (!string.IsNullOrEmpty(kind))
                {
                    switch (kind)
                    {
                        case "1":
                            where1 = " and a.CardUserID = '0'";
                            where3 = " and a.CardUserID = '0'";
                            break;
                        case "2":
                            where2 = " and a.BuyUserID = '0'";
                            where1 = " and a.CardUserID = '0'";
                            break;
                        case "3":
                            where2 = " and a.BuyUserID = '0'";
                            where3 = " and a.CardUserID = '0'";
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(btime) && !string.IsNullOrEmpty(etime))
                {
                    where1 += " and a.AddTime >= " + dbc.ToSqlValue(Convert.ToDateTime(btime)) + " and a.AddTime < " + dbc.ToSqlValue(Convert.ToDateTime(etime).AddDays(1));
                    where2 += " and a.AddTime >= " + dbc.ToSqlValue(Convert.ToDateTime(btime)) + " and a.AddTime < " + dbc.ToSqlValue(Convert.ToDateTime(etime).AddDays(1));
                    where3 += " and a.AddTime >= " + dbc.ToSqlValue(Convert.ToDateTime(btime)) + " and a.AddTime < " + dbc.ToSqlValue(Convert.ToDateTime(etime).AddDays(1));
                }

                if (!string.IsNullOrEmpty(mc))
                {
                    where1 += " and " + dbc.C_Like("c.UserName", mc, LikeStyle.LeftAndRightLike);
                    where2 += " and " + dbc.C_Like("c.UserName", mc, LikeStyle.LeftAndRightLike);
                    where3 += " and " + dbc.C_Like("c.UserName", mc, LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(zxmc))
                {
                    where1 += " and " + dbc.C_Like("b.UserXM", zxmc, LikeStyle.LeftAndRightLike);
                    where2 += " and " + dbc.C_Like("b.UserXM", zxmc, LikeStyle.LeftAndRightLike);
                    where3 += " and " + dbc.C_Like("b.UserXM", zxmc, LikeStyle.LeftAndRightLike);
                }
                string str = @"select b.UserXM,c.UserName,a.AddTime,a.Points as MONEY,'转让' as KIND,'耗材券' as FLAG  from tb_b_pay a left join tb_b_user b on a.CardUserID=b.UserID
                                left join tb_b_user c on a.PayUserID=c.UserID 
                                left join tb_b_user d on a.ReceiveUserID=d.UserID 
                                where 1=1 and  c.ClientKind=2 and  d.ClientKind=2   " + where1 + @"
                                 union all 
                                select b.UserXM,c.UserName,a.AddTime,a.Points as MONEY,'消费' as KIND,
                               case when f.pointkind=0 then '耗材券' 
                                when  f.pointkind is null then '耗材券'
                                when  f.pointkind=4 then '授权券'
                                when  f.pointkind=1 then '授权券'
                                when  f.pointkind=2 then '授权券'
                                when  f.pointkind=3 then  '自发券' end as FLAG  
                                from tb_b_pay a left join tb_b_user b on a.CardUserID=b.UserID
                                left join tb_b_user c on a.PayUserID=c.UserID 
                                left join tb_b_user d on a.ReceiveUserID=d.UserID
                                left join tb_b_order e on a.OrderCode=e.OrderCode
                                left join tb_b_plattosale f on e.PlatToSaleId=f.PlatToSaleId
                                where 1=1 and  c.ClientKind=2 and  d.ClientKind=1   " + where3 + @"
                            union all 
	                        select b.UserXM,c.UserName,a.AddTime,a.Points as MONEY,'购买' as KIND,
                                case when f.pointkind=0 then '耗材券' 
                                when  f.pointkind is null then '耗材券'
                                when  f.pointkind=4 then '授权券'
                                when  f.pointkind=1 then '授权券'
                                when  f.pointkind=2 then '授权券'
                                when  f.pointkind=3 then  '自发券' end as FLAG  
                            from tb_b_order a left join tb_b_user b on a.SaleUserID=b.UserID
                            left join tb_b_user c on a.BuyUserID=c.UserID
                            left join tb_b_plattosale f on a.PlatToSaleId=f.PlatToSaleId
                            where a.Status=0 and a.ZhiFuZT=1 " + where2 + @"
                            order by AddTime desc";
                DataTable dt = dbc.ExecuteDataTable(str);

                dt.Columns.Add("DATE");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["addtime"] != null && dt.Rows[i]["addtime"].ToString() != "")
                    {
                        dt.Rows[i]["DATE"] = Convert.ToDateTime(dt.Rows[i]["addtime"]).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["KIND"]);
                    cells[i + 1, 0].SetStyle(style4);
                    cells[i + 1, 1].PutValue(dt.Rows[i]["UserXM"]);
                    cells[i + 1, 1].SetStyle(style4);
                    cells[i + 1, 2].PutValue(dt.Rows[i]["UserName"]);
                    cells[i + 1, 2].SetStyle(style4);
                    cells[i + 1, 3].PutValue(dt.Rows[i]["DATE"]);
                    cells[i + 1, 3].SetStyle(style4);
                    cells[i + 1, 4].PutValue(dt.Rows[i]["MONEY"]);
                    cells[i + 1, 4].SetStyle(style4);
                    cells[i + 1, 5].PutValue(dt.Rows[i]["FLAG"]);
                    cells[i + 1, 5].SetStyle(style4);
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

    [CSMethod("GetZXUSERToFile", 2)]
    public byte[] GetZXUSERToFile(string beg, string end, string isdonate)
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
                cells[0, 0].PutValue("登陆名");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("角色");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("姓名");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("电话");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("线路");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("可开放购买运费券");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);
                cells[0, 6].PutValue("剩余运费券");
                cells[0, 6].SetStyle(style2);
                cells.SetColumnWidth(6, 20);
                cells[0, 7].PutValue("在售运费券");
                cells[0, 7].SetStyle(style2);
                cells.SetColumnWidth(7, 20);

                cells[0, 8].PutValue("注册时间");
                cells[0, 8].SetStyle(style2);
                cells.SetColumnWidth(8, 20);

                cells[0, 9].PutValue("地址");
                cells[0, 9].SetStyle(style2);
                cells.SetColumnWidth(9, 20);

                cells[0, 10].PutValue("模式类型");
                cells[0, 10].SetStyle(style2);
                cells.SetColumnWidth(10, 20);

                cells[0, 11].PutValue("模式系数");
                cells[0, 11].SetStyle(style2);
                cells.SetColumnWidth(11, 20);

                cells[0, 12].PutValue("承运最大限额");
                cells[0, 12].SetStyle(style2);
                cells.SetColumnWidth(12, 20);

                cells[0, 13].PutValue("赠送红包额度");
                cells[0, 13].SetStyle(style2);
                cells.SetColumnWidth(13, 20);
                string where = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString() + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.AddTime<='" + Convert.ToDateTime(end).AddDays(1).ToString() + "'";
                }
                if (!string.IsNullOrEmpty(isdonate))
                {
                    where += " and a.isdonate='" + isdonate + "'";
                }

                string str = @"select a.*,c.roleName,b.roleId,d.SalePoints,e.Points as KFGMPoints from tb_b_user a left join tb_b_user_role b on a.UserID=b.UserID
                               left join tb_b_roledb c on b.roleId=c.roleId
                               left join (select sum(points) SalePoints,UserID from tb_b_plattosale where status = 0 and pointkind=0 group by UserID) d on a.UserID = d.UserID 
                               left join tb_b_platpoints e on a.UserID = e.UserID 
                               where 1=1 and a.ClientKind = 1
                                                                ";
                str += where;
                //开始取分页数据
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = dbc.ExecuteDataTable(str + " order by a.AddTime desc,a.UserName,a.UserXM");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["UserName"]);
                    cells[i + 1, 0].SetStyle(style4);
                    cells[i + 1, 1].PutValue("专线");
                    cells[i + 1, 1].SetStyle(style4);
                    cells[i + 1, 2].PutValue(dt.Rows[i]["UserXM"]);
                    cells[i + 1, 2].SetStyle(style4);
                    cells[i + 1, 3].PutValue(dt.Rows[i]["UserTel"]);
                    cells[i + 1, 3].SetStyle(style4);

                    var xl = "";
                    if (dt.Rows[i]["FromRoute"] != null && dt.Rows[i]["FromRoute"].ToString() != "")
                    {
                        xl += dt.Rows[i]["FromRoute"].ToString();
                    }
                    if (dt.Rows[i]["ToRoute"] != null && dt.Rows[i]["ToRoute"].ToString() != "")
                    {
                        xl += "─" + dt.Rows[i]["ToRoute"].ToString();
                    }
                    cells[i + 1, 4].PutValue(xl);
                    cells[i + 1, 4].SetStyle(style4);
                    cells[i + 1, 5].PutValue(dt.Rows[i]["KFGMPoints"]);
                    cells[i + 1, 5].SetStyle(style4);
                    cells[i + 1, 6].PutValue(dt.Rows[i]["Points"]);
                    cells[i + 1, 6].SetStyle(style4);
                    cells[i + 1, 7].PutValue(dt.Rows[i]["SalePoints"]);
                    cells[i + 1, 7].SetStyle(style4);
                    cells[i + 1, 8].PutValue(Convert.ToDateTime(dt.Rows[i]["AddTime"]).ToString("yyyy-MM-dd"));
                    cells[i + 1, 8].SetStyle(style4);
                    cells[i + 1, 9].PutValue(dt.Rows[i]["Address"]);
                    cells[i + 1, 9].SetStyle(style4);
                    if (dt.Rows[i]["modetype"] != DBNull.Value)
                    {
                        if (Convert.ToInt32(dt.Rows[i]["modetype"]) == 1)
                        {
                            cells[i + 1, 10].PutValue("模式一");
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["modetype"]) == 2)
                        {
                            cells[i + 1, 10].PutValue("模式二");
                        }
                    }
                    cells[i + 1, 10].SetStyle(style4);

                    if (dt.Rows[i]["modecoefficient"] != DBNull.Value)
                    {
                        cells[i + 1, 11].PutValue(Convert.ToDecimal(dt.Rows[i]["modecoefficient"]));
                    }
                    cells[i + 1, 11].SetStyle(style4);
                    if (dt.Rows[i]["carriagemaxmoney"] != DBNull.Value)
                    {
                        cells[i + 1, 12].PutValue(Convert.ToDecimal(dt.Rows[i]["carriagemaxmoney"]));
                    }
                    cells[i + 1, 12].SetStyle(style4);

                    if (dt.Rows[i]["redenvelopequota"] != DBNull.Value)
                    {
                        cells[i + 1, 13].PutValue(Convert.ToDecimal(dt.Rows[i]["redenvelopequota"]));
                    }
                    cells[i + 1, 13].SetStyle(style4);
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

    [CSMethod("GetSFUSERToFile", 2)]
    public byte[] GetSFUSERToFile(string beg, string end, string cx_isclose)
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
                cells[0, 0].PutValue("登陆名");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("角色");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("姓名");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("电话");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("注册时间");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                if (SystemUser.CurrentUser.RoleID == "80A7DD89-A0A3-445D-9CD9-552AE71AD69F")
                {
                    cells[0, 5].PutValue("身份证号");
                    cells[0, 5].SetStyle(style2);
                    cells.SetColumnWidth(5, 20);
                }

                string where = "";
                //if (!string.IsNullOrEmpty(yhm.Trim()))
                //{
                //    where += " and " + dbc.C_Like("b.UserXM", yhm.Trim(), LikeStyle.LeftAndRightLike);
                //}

                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  a.AddTime>='" + Convert.ToDateTime(beg).ToString() + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.AddTime<='" + Convert.ToDateTime(end).AddDays(1).ToString() + "'";
                }
                if (!string.IsNullOrEmpty(cx_isclose))
                {
                    where += " and a.isclose = " + dbc.ToSqlValue(cx_isclose);
                }
                string str = @"select a.*,c.roleName,b.roleId from tb_b_user a left join tb_b_user_role b on a.UserID=b.UserID
                               left join tb_b_roledb c on b.roleId=c.roleId
                               where 1=1 and a.ClientKind = 2";
                str += where;
                //开始取分页数据
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = dbc.ExecuteDataTable(str + " order by a.AddTime desc,a.UserName,a.UserXM");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["UserName"]);
                    cells[i + 1, 0].SetStyle(style4);
                    cells[i + 1, 1].PutValue("三方");
                    cells[i + 1, 1].SetStyle(style4);
                    cells[i + 1, 2].PutValue(dt.Rows[i]["UserXM"]);
                    cells[i + 1, 2].SetStyle(style4);
                    cells[i + 1, 3].PutValue(dt.Rows[i]["UserTel"]);
                    cells[i + 1, 3].SetStyle(style4);
                    cells[i + 1, 4].PutValue(Convert.ToDateTime(dt.Rows[i]["AddTime"]).ToString("yyyy-MM-dd"));
                    cells[i + 1, 4].SetStyle(style4);
                    if (SystemUser.CurrentUser.RoleID == "80A7DD89-A0A3-445D-9CD9-552AE71AD69F")
                    {
                        cells[i + 1, 5].PutValue(dt.Rows[i]["IDCard"]);
                        cells[i + 1, 5].SetStyle(style4);
                    }
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

    [CSMethod("GetSFYFQToFile", 2)]
    public byte[] GetSFYFQToFile(string roleId, string yhm, string xm, string cx_isclose)
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
                cells[0, 0].PutValue("三方");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("物流");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("剩余运费券");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);

                string where = "";
                string str = @" select a.points MONEY,c.UserXM as wuliu,b.UserName as sf from tb_b_mycard a left join tb_b_user b on a.UserID=b.UserID
                        left join tb_b_user c on a.CardUserID=c.UserID
                        where a.status=0 and b.ClientKind=2 order by b.UserName,c.UserXM,a.points";
                str += where;
                //开始取分页数据
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = dbc.ExecuteDataTable(str);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["sf"]);
                    cells[i + 1, 0].SetStyle(style4);
                    cells[i + 1, 1].PutValue(dt.Rows[i]["wuliu"]);
                    cells[i + 1, 1].SetStyle(style4);
                    cells[i + 1, 2].PutValue(dt.Rows[i]["MONEY"]);
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

    //[CSMethod("GetUserList")]
    //public object GetUserList(int cp, string userName, string deptType, string dept)
    //{
    //    string condition = "";
    //    if (deptType.Trim() != "")
    //    {
    //        condition += " and t2.dw_lx = '" + deptType.Replace("'", "''") + "'";
    //    }
    //    if (userName.Trim() != "")
    //    {
    //        condition += " and t1.yh_dlm like '%" + userName.Replace("'", "''") + "%'";
    //    }
    //    if (dept.Trim() != "")
    //    {
    //        condition += " and t1.dw_id = '" + dept.Replace("'", "''") + "'";
    //    }
    //    StringBuilder sb = new StringBuilder();
    //    sb.Append("select t1.yh_id, ");
    //    sb.Append("t1.yh_dlm, ");
    //    sb.Append("t1.addtime, ");
    //    sb.Append("t2.dw_mc, ");
    //    sb.Append("case t2.dw_lx ");
    //    sb.Append("when 0 then ");
    //    sb.Append("'物价局' ");
    //    sb.Append("when 1 then ");
    //    sb.Append("'配供中心' ");
    //    sb.Append("when 2 then ");
    //    sb.Append("'平价商店' ");
    //    sb.Append("when 3 then ");
    //    sb.Append("'蔬菜基地' ");
    //    sb.Append("end as dw_lx ,case t1.yh_enable when 0 then '启用' else '禁用' end as YH_ENABLE ");
    //    sb.Append("from TZCLZ_T_YH t1, TZCLZ_T_DW t2 ");
    //    sb.AppendFormat("where t1.dw_id = t2.dw_id and t1.status = 0 {0} order by t1.addtime", condition);
    //    int allCount = 0;
    //    int currentPage = cp;
    //    currentPage = cp;
    //    using (DBConnection dbc = new DBConnection())
    //    {
    //        DataTable dt = dbc.ExecuteDataTable(sb.ToString());
    //        return new { dtUser = dt, currentPage = cp, allCount = allCount };
    //    }
    //}

    [CSMethod("GetUserAndJs")]
    public object GetUserAndJs(string UserId)
    {

        using (DBConnection dbc = new DBConnection())
        {
            string sqlStrUser = "select * from tb_b_Users where User_ID='" + UserId + "'";
            DataTable dtuser = dbc.ExecuteDataTable(sqlStrUser);
            string sqlStrJs = "select distinct JS_ID from tb_b_User_JS_Gl where delflag=0 and User_ID='" + UserId + "'";
            DataTable dtjs = dbc.ExecuteDataTable(sqlStrJs);

            return new { dtuser = dtuser, dtjs = dtjs };
        }
    }


    [CSMethod("GetUser")]
    public DataTable GetUser(string UserId)
    {
        string sqlStr = "select * from tb_b_Users where User_ID='" + UserId + "'";
        using (DBConnection dbc = new DBConnection())
        {
            return dbc.ExecuteDataTable(sqlStr);
        }
    }

    [CSMethod("GetUserJs")]
    public DataTable GetUserJs(string UserId)
    {
        string sqlStr = "select distinct JS_ID from tb_b_User_JS_Gl where delflag=0 and User_ID='" + UserId + "'";
        using (DBConnection dbc = new DBConnection())
        {
            return dbc.ExecuteDataTable(sqlStr);
        }
    }

    [CSMethod("GetRole")]
    public DataTable GetRole()
    {
        string sqlStr = "select roleId,roleName from tb_b_roledb order by rolePx";
        using (DBConnection dbc = new DBConnection())
        {
            return dbc.ExecuteDataTable(sqlStr);
        }
    }

    [CSMethod("GetDWByJsid")]
    public DataTable GetDWByJsid(string UserId, string jsid)
    {
        using (DBConnection dbc = new DBConnection())
        {
            DataTable dt = new DataTable();
            string sqlStr = "select * from tb_b_roledb where status=0 and roleId='" + jsid + "'";
            DataTable dtjs = dbc.ExecuteDataTable(sqlStr);
            if (dtjs != null && dtjs.Rows.Count > 0)
            {
                int jstype = Convert.ToInt32(dtjs.Rows[0]["JS_Type"]);
                if (jstype == 0)
                {
                    string str = "select distinct DW_ID from tb_b_User_Dw_Gl where delflag=0 and DW_ID in(SELECT DW_ID  FROM tb_b_Department where STATUS=0) and User_ID='" + UserId + "'";
                    dt = dbc.ExecuteDataTable(str);
                }
                else
                {
                    switch (jsid.ToUpper())
                    {
                        case "F6613AFB-06E2-454A-881F-8C51483976F3":
                            dt = dbc.ExecuteDataTable("select distinct DW_ID from tb_b_User_Dw_Gl where delflag=0 and DW_ID in( select DW_ID from tb_b_DW where DW_LX=4 and STATUS=0) and User_ID='" + UserId + "'");
                            break;
                        case "7E53492E-CF66-411F-83C4-7923467F59B4":
                            dt = dbc.ExecuteDataTable("select distinct DW_ID from tb_b_User_Dw_Gl where delflag=0 and DW_ID in( select PJSD_ID from tb_b_PJSD where delflag=0) and User_ID='" + UserId + "'");
                            break;
                    }
                }
            }
            return dt;
        }
    }


    [CSMethod("GetDW")]
    public DataTable GetDW(string jsid)
    {
        using (DBConnection dbc = new DBConnection())
        {
            DataTable dt = new DataTable();
            string sqlStr = "select * from tb_b_JS where status=0 and JS_ID='" + jsid + "'";
            DataTable dtjs = dbc.ExecuteDataTable(sqlStr);
            if (dtjs != null && dtjs.Rows.Count > 0)
            {
                int jstype = Convert.ToInt32(dtjs.Rows[0]["JS_Type"]);
                if (jstype == 0)
                {
                    string str1 = "SELECT DW_ID ID,DW_MC MC FROM tb_b_Department where STATUS=0";
                    dt = dbc.ExecuteDataTable(str1);
                }
                else
                {
                    switch (jsid.ToUpper())
                    {
                        case "F6613AFB-06E2-454A-881F-8C51483976F3":
                            dt = dbc.ExecuteDataTable("select DW_ID ID,DW_MC MC from tb_b_DW where DW_LX=4 and STATUS=0");
                            break;
                        case "7E53492E-CF66-411F-83C4-7923467F59B4":
                            dt = dbc.ExecuteDataTable("select PJSD_ID ID,PJSD_MC MC from dbo.tb_b_PJSD where delflag=0");
                            break;
                    }
                }
            }
            return dt;
        }
    }

    [CSMethod("GetDWAndGl")]
    public object GetDWAndGl(string jsid, string UserId)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                //单位
                DataTable dt = new DataTable();
                //权限
                DataTable dtqx = new DataTable();
                //权限关联
                DataTable dtqxgl = new DataTable();
                //用户关联
                DataTable dtusergl = new DataTable();

                string sqlStr = "select * from tb_b_JS where status=0 and JS_ID='" + jsid + "'";
                DataTable dtjs = dbc.ExecuteDataTable(sqlStr);

                if (dtjs != null && dtjs.Rows.Count > 0)
                {
                    int jstype = Convert.ToInt32(dtjs.Rows[0]["JS_Type"]);
                    if (jstype == 0)
                    {
                        string str1 = "SELECT DW_ID ID,DW_MC MC FROM tb_b_Department where STATUS=0";
                        dt = dbc.ExecuteDataTable(str1);
                        string str2 = "SELECT PRIVILEGECODE ID,MODULENAME MC FROM tb_b_YH_QX where MODULENAME not like '平价商店权限_%' order by substring(MODULENAME,1,charindex('-',MODULENAME)),ORDERNO";
                        dtqx = dbc.ExecuteDataTable(str2);
                        if (UserId != null)
                        {
                            string str3 = "select a.PRIVILEGECODE,b.MODULENAME from tb_b_YH_YHQX a left join tb_b_YH_QX b on a.PRIVILEGECODE=b.PRIVILEGECODE where USERID='" + UserId + "'";
                            dtqxgl = dbc.ExecuteDataTable(str3);
                        }
                    }
                    else
                    {
                        switch (jsid.ToUpper())
                        {
                            case "F6613AFB-06E2-454A-881F-8C51483976F3":
                                dt = dbc.ExecuteDataTable("select DW_ID ID,DW_MC MC from tb_b_DW where DW_LX=4 and STATUS=0");
                                break;
                            case "7E53492E-CF66-411F-83C4-7923467F59B4":
                                dt = dbc.ExecuteDataTable("select PJSD_ID ID,PJSD_MC MC,QY_NAME from tb_b_PJSD a left join tb_b_Eare b on a.qy_id=b.qy_id where delflag=0 order by QY_PX,PJSD_NO");
                                string str2 = "SELECT PRIVILEGECODE ID,MODULENAME MC FROM tb_b_YH_QX where MODULENAME like '平价商店权限_%' order by substring(MODULENAME,1,charindex('-',MODULENAME)),ORDERNO";
                                dtqx = dbc.ExecuteDataTable(str2);
                                if (UserId != null)
                                {
                                    string str3 = "select a.PRIVILEGECODE,b.MODULENAME from tb_b_YH_YHQX a left join tb_b_YH_QX b on a.PRIVILEGECODE=b.PRIVILEGECODE where USERID='" + UserId + "'";
                                    dtqxgl = dbc.ExecuteDataTable(str3);
                                }
                                break;
                        }
                    }
                }


                if (UserId != null)
                {
                    dtusergl = GetDWByJsid(UserId, jsid);
                }

                return new { dtdw = dt, usergl = dtusergl, dtqx = dtqx, dtqxgl = dtqxgl };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetDeptsByType")]
    public DataTable GetDeptsByType(string DeptType)
    {
        string sqlStr = "select * from tb_b_Department where DW_LX = @DW_LX and STATUS = 0";
        using (DBConnection dbc = new DBConnection())
        {
            SqlCommand cmd = new SqlCommand(sqlStr);
            cmd.Parameters.AddWithValue("@DW_LX", DeptType);
            return dbc.ExecuteDataTable(cmd);
        }
    }
    [CSMethod("GetDepts")]
    public DataTable GetDepts()
    {
        string sqlStr = "select * from tb_b_Department where  STATUS = 0";
        using (DBConnection dbc = new DBConnection())
        {
            SqlCommand cmd = new SqlCommand(sqlStr);
            return dbc.ExecuteDataTable(cmd);
        }
    }

    [CSMethod("SaveUser")]
    public bool SaveUser(JSReader jsr)
    {
        if (jsr["UserName"].IsNull || jsr["UserName"].IsEmpty)
        {
            throw new Exception("用户名不能为空");
        }
        if (jsr["Password"].IsNull || jsr["Password"].IsEmpty)
        {
            throw new Exception("密码不能为空");
        }

        var companyId = SystemUser.CurrentUser.CompanyID;
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                if (jsr["UserID"].ToString() == "")
                {
                    DataTable dt_user = dbc.ExecuteDataTable("select * from tb_b_user where UserName='" + jsr["UserName"].ToString() + "'");
                    if (dt_user.Rows.Count > 0)
                    {
                        throw new Exception("该用户名已存在！");
                    }
                    //用户表
                    var YHID = Guid.NewGuid().ToString();
                    var dt = dbc.GetEmptyDataTable("tb_b_user");
                    var dr = dt.NewRow();
                    dr["UserID"] = new Guid(YHID);
                    dr["UserName"] = jsr["UserName"].ToString();
                    dr["Password"] = jsr["Password"].ToString();
                    dr["AddTime"] = DateTime.Now;
                    dr["IsSHPass"] = 1;
                    dr["Points"] = 0;
                    dr["ClientKind"] = 0;
                    //dr["Discount"] = ;
                    dr["UserXM"] = jsr["UserXM"].ToString();
                    dr["UserTel"] = jsr["UserTel"].ToString();
                    //dr["FromRoute"] = ;
                    //dr["ToRoute"] = ;
                    dr["companyId"] = companyId;
                    //dr["PayPassword"] = ;
                    dr["Address"] = jsr["Address"].ToString();
                    dt.Rows.Add(dr);
                    dbc.InsertTable(dt);

                    //角色用户关联表
                    var rdt = dbc.GetEmptyDataTable("tb_b_user_role");
                    var rdr = rdt.NewRow();
                    rdr["userroleId"] = Guid.NewGuid().ToString();
                    rdr["userId"] = new Guid(YHID);
                    rdr["roleId"] = jsr["roleId"].ToString();
                    rdr["companyId"] = companyId;
                    rdt.Rows.Add(rdr);
                    dbc.InsertTable(rdt);

                }
                else
                {
                    var YHID = jsr["UserID"].ToString();
                    var oldname = dbc.ExecuteScalar("select UserName from tb_b_user where UserID='" + YHID + "'");
                    if (!jsr["UserName"].ToString().Equals(oldname.ToString()))
                    {
                        DataTable dt_user = dbc.ExecuteDataTable("select * from tb_b_user where UserName='" + jsr["UserName"].ToString() + "'");
                        if (dt_user.Rows.Count > 0)
                        {
                            throw new Exception("该用户名已存在！");
                        }
                    }
                    var dt = dbc.GetEmptyDataTable("tb_b_user");
                    var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                    var dr = dt.NewRow();
                    dr["UserID"] = new Guid(YHID);
                    dr["UserName"] = jsr["UserName"].ToString();
                    dr["Password"] = jsr["Password"].ToString();
                    dr["UserXM"] = jsr["UserXM"].ToString();
                    dr["UserTel"] = jsr["UserTel"].ToString();
                    dr["Address"] = jsr["Address"].ToString();
                    dt.Rows.Add(dr);
                    dbc.UpdateTable(dt, dtt);

                    //删除用户的角色关联
                    string del_js = "delete from tb_b_user_role where userId=@userId";
                    SqlCommand cmd = new SqlCommand(del_js);
                    cmd.Parameters.AddWithValue("@userId", YHID);
                    dbc.ExecuteNonQuery(cmd);

                    //建立用户角色关联
                    var rdt = dbc.GetEmptyDataTable("tb_b_user_role");
                    var rdr = rdt.NewRow();
                    rdr["userroleId"] = Guid.NewGuid().ToString();
                    rdr["userId"] = new Guid(YHID);
                    rdr["roleId"] = jsr["roleId"].ToString();
                    rdr["companyId"] = companyId;
                    rdt.Rows.Add(rdr);
                    dbc.InsertTable(rdt);
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

    [CSMethod("SaveClient")]
    public bool SaveClient(JSReader jsr, string nr)
    {
        if (jsr["UserName"].IsNull || jsr["UserName"].IsEmpty)
        {
            throw new Exception("用户名不能为空");
        }
        if (jsr["Password"].IsNull || jsr["Password"].IsEmpty)
        {
            throw new Exception("密码不能为空");
        }

        var companyId = SystemUser.CurrentUser.CompanyID;
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                if (jsr["UserID"].ToString() == "")
                {
                    DataTable dt_user = dbc.ExecuteDataTable("select * from tb_b_user where UserName='" + jsr["UserName"].ToString() + "'");
                    if (dt_user.Rows.Count > 0)
                    {
                        throw new Exception("该用户名已存在！");
                    }
                    //用户表
                    var YHID = Guid.NewGuid().ToString();
                    var dt = dbc.GetEmptyDataTable("tb_b_user");
                    var dr = dt.NewRow();
                    dr["UserID"] = new Guid(YHID);
                    dr["UserName"] = jsr["UserName"].ToString();
                    dr["Password"] = jsr["Password"].ToString();
                    dr["AddTime"] = DateTime.Now;
                    dr["IsSHPass"] = 1;
                    dr["Points"] = 0;
                    dr["ClientKind"] = 0;
                    //dr["Discount"] = ;
                    dr["UserXM"] = jsr["UserXM"].ToString();
                    dr["UserTel"] = jsr["UserTel"].ToString();
                    //dr["FromRoute"] = ;
                    //dr["ToRoute"] = ;
                    dr["companyId"] = companyId;
                    //dr["PayPassword"] = ;
                    dr["Address"] = jsr["Address"].ToString();
                    dr["searchAddress"] = jsr["searchAddress"].ToString();
                    dr["DqBm"] = jsr["DqBm"].ToString();
                    dr["IDCard"] = jsr["IDCard"].ToString();
                    dr["caruser"] = jsr["caruser"].ToString();
                    if (!string.IsNullOrEmpty(jsr["modetype"].ToString()))
                    {
                        dr["modetype"] = Convert.ToInt32(jsr["modetype"].ToString());
                    }
                    else
                    {
                        dr["modetype"] = DBNull.Value;
                    }
                    if (!string.IsNullOrEmpty(jsr["modecoefficient"].ToString()))
                    {
                        dr["modecoefficient"] = Convert.ToDecimal(jsr["modecoefficient"].ToString());
                    }
                    else
                    {
                        dr["modecoefficient"] = DBNull.Value;
                    }
                    if (!string.IsNullOrEmpty(jsr["carriagemaxmoney"].ToString()))
                    {
                        dr["carriagemaxmoney"] = Convert.ToDecimal(jsr["carriagemaxmoney"].ToString());
                    }
                    else
                    {
                        dr["carriagemaxmoney"] = DBNull.Value;
                    }
                    if (!string.IsNullOrEmpty(jsr["mirrornumber"].ToString()))
                    {
                        dr["mirrornumber"] = jsr["mirrornumber"].ToString();
                    }
                    else
                    {
                        dr["mirrornumber"] = DBNull.Value;
                    }
                    if (!string.IsNullOrEmpty(jsr["carriagegetmode"].ToString()))
                    {
                        dr["carriagegetmode"] = jsr["carriagegetmode"].ToString();
                        if (jsr["carriagegetmode"].ToString() == "0" && jsr["modetype"].ToString() == "1")
                        {
                            if (!string.IsNullOrEmpty(jsr["carriageoilrate"].ToString()))
                            {
                                dr["carriageoilrate"] = jsr["carriageoilrate"].ToString();
                            }
                            else
                            {
                                dr["carriageoilrate"] = DBNull.Value;
                            }

                            if (!string.IsNullOrEmpty(jsr["carriagemoneyrate"].ToString()))
                            {
                                dr["carriagemoneyrate"] = jsr["carriagemoneyrate"].ToString();
                            }
                            else
                            {
                                dr["carriagemoneyrate"] = DBNull.Value;
                            }
                        }
                        else
                        {
                            dr["carriageoilrate"] = DBNull.Value; ;
                            dr["carriagemoneyrate"] = DBNull.Value; ;
                        }
                    }
                    else
                    {
                        dr["carriageoilrate"] = DBNull.Value;
                        dr["carriagegetmode"] = DBNull.Value;
                    }

                    if (!string.IsNullOrEmpty(jsr["carriagechbid"].ToString()))
                    {
                        dr["carriagechbid"] = jsr["carriagechbid"].ToString();
                    }
                    else
                    {
                        dr["carriagechbid"] = DBNull.Value;
                    }
                    if (!string.IsNullOrEmpty(jsr["isidentification"].ToString()))
                    {
                        dr["isidentification"] = jsr["isidentification"].ToString();
                    }
                    else
                    {
                        dr["isidentification"] = DBNull.Value;
                    }
                    //下单是否赠送红包，0:赠送；1:不赠送；默认为1
                    if (!string.IsNullOrEmpty(jsr["isdonate"].ToString()))
                    {
                        dr["isdonate"] = Convert.ToInt32(jsr["isdonate"].ToString());
                    }
                    else
                    {
                        dr["isdonate"] = DBNull.Value;
                    }
                    //下单赠送红包比例,1-99之间
                    if (!string.IsNullOrEmpty(jsr["donateratio"].ToString()))
                    {
                        dr["donateratio"] = Convert.ToInt32(jsr["donateratio"].ToString());
                    }
                    else
                    {
                        dr["donateratio"] = DBNull.Value;
                    }
                    //是否显示发布来源：0:显示；1:不显示；默认为0
                    if (!string.IsNullOrEmpty(jsr["isshowsource"].ToString()))
                    {
                        dr["isshowsource"] = Convert.ToInt32(jsr["isshowsource"].ToString());
                    }
                    else
                    {
                        dr["isshowsource"] = DBNull.Value;
                    }
                    //是否收取佣金；0:是；1:否；默认为1
                    if (!string.IsNullOrEmpty(jsr["iscost"].ToString()))
                    {
                        dr["iscost"] = Convert.ToInt32(jsr["iscost"].ToString());
                    }
                    else
                    {
                        dr["iscost"] = DBNull.Value;
                    }
                    //是否查货宝会员；0:是；1:否；默认为1
                    if (!string.IsNullOrEmpty(jsr["ischbmember"].ToString()))
                    {
                        dr["ischbmember"] = Convert.ToInt32(jsr["ischbmember"].ToString());
                    }
                    else
                    {
                        dr["ischbmember"] = DBNull.Value;
                    }

                    if (!string.IsNullOrEmpty(jsr["jd"].ToString()))
                    {
                        dr["jd"] = jsr["jd"].ToString();
                    }
                    else
                    {
                        dr["jd"] = DBNull.Value;
                    }
                    if (!string.IsNullOrEmpty(jsr["wd"].ToString()))
                    {
                        dr["wd"] = jsr["wd"].ToString();
                    }
                    else
                    {
                        dr["wd"] = DBNull.Value;
                    }
                    dt.Rows.Add(dr);
                    dbc.InsertTable(dt);

                    //角色用户关联表
                    var rdt = dbc.GetEmptyDataTable("tb_b_user_role");
                    var rdr = rdt.NewRow();
                    rdr["userroleId"] = Guid.NewGuid().ToString();
                    rdr["userId"] = new Guid(YHID);
                    rdr["roleId"] = jsr["roleId"].ToString();
                    rdr["companyId"] = companyId;
                    rdt.Rows.Add(rdr);
                    dbc.InsertTable(rdt);

                }
                else
                {
                    var YHID = jsr["UserID"].ToString();
                    var oldname = dbc.ExecuteScalar("select UserName from tb_b_user where UserID='" + YHID + "'");
                    if (!jsr["UserName"].ToString().Equals(oldname.ToString()))
                    {
                        DataTable dt_user = dbc.ExecuteDataTable("select * from tb_b_user where UserName='" + jsr["UserName"].ToString() + "'");
                        if (dt_user.Rows.Count > 0)
                        {
                            throw new Exception("该用户名已存在！");
                        }
                    }
                    var dt = dbc.GetEmptyDataTable("tb_b_user");
                    var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                    var dr = dt.NewRow();
                    dr["UserID"] = new Guid(YHID);
                    dr["UserName"] = jsr["UserName"].ToString();
                    dr["Password"] = jsr["Password"].ToString();
                    dr["UserXM"] = jsr["UserXM"].ToString();
                    dr["UserTel"] = jsr["UserTel"].ToString();
                    dr["FromRoute"] = jsr["FromRoute"].ToString();
                    dr["ToRoute"] = jsr["ToRoute"].ToString();
                    dr["Address"] = jsr["Address"].ToString();
                    dr["searchAddress"] = jsr["searchAddress"].ToString();
                    dr["DqBm"] = jsr["DqBm"].ToString();
                    dr["IDCard"] = jsr["IDCard"].ToString();
                    dr["caruser"] = jsr["caruser"].ToString();

                    if (!string.IsNullOrEmpty(jsr["modetype"].ToString()))
                    {
                        dr["modetype"] = Convert.ToInt32(jsr["modetype"].ToString());
                    }
                    else
                    {
                        dr["modetype"] = DBNull.Value;
                    }
                    if (!string.IsNullOrEmpty(jsr["modecoefficient"].ToString()))
                    {
                        dr["modecoefficient"] = Convert.ToDecimal(jsr["modecoefficient"].ToString());
                    }
                    else
                    {
                        dr["modecoefficient"] = DBNull.Value;
                    }
                    if (!string.IsNullOrEmpty(jsr["carriagemaxmoney"].ToString()))
                    {
                        dr["carriagemaxmoney"] = Convert.ToDecimal(jsr["carriagemaxmoney"].ToString());
                    }
                    else
                    {
                        dr["carriagemaxmoney"] = DBNull.Value;
                    }
                    if (!string.IsNullOrEmpty(jsr["mirrornumber"].ToString()))
                    {
                        dr["mirrornumber"] = jsr["mirrornumber"].ToString();
                    }
                    else
                    {
                        dr["mirrornumber"] = DBNull.Value;
                    }

                    if (!string.IsNullOrEmpty(jsr["carriagegetmode"].ToString()))
                    {
                        dr["carriagegetmode"] = jsr["carriagegetmode"].ToString();
                        if (jsr["carriagegetmode"].ToString() == "0" && jsr["modetype"].ToString() == "1")
                        {
                            if (!string.IsNullOrEmpty(jsr["carriageoilrate"].ToString()))
                            {
                                dr["carriageoilrate"] = jsr["carriageoilrate"].ToString();
                            }
                            else
                            {
                                dr["carriageoilrate"] = DBNull.Value;
                            }

                            if (!string.IsNullOrEmpty(jsr["carriagemoneyrate"].ToString()))
                            {
                                dr["carriagemoneyrate"] = jsr["carriagemoneyrate"].ToString();
                            }
                            else
                            {
                                dr["carriagemoneyrate"] = DBNull.Value;
                            }
                        }
                        else
                        {
                            dr["carriageoilrate"] = DBNull.Value;
                            dr["carriagemoneyrate"] = DBNull.Value;
                        }
                    }
                    else
                    {
                        dr["carriageoilrate"] = DBNull.Value;
                        dr["carriagegetmode"] = DBNull.Value;
                    }

                    if (!string.IsNullOrEmpty(jsr["carriagechbid"].ToString()))
                    {
                        dr["carriagechbid"] = jsr["carriagechbid"].ToString();
                    }
                    else
                    {
                        dr["carriagechbid"] = DBNull.Value;
                    }

                    if (!string.IsNullOrEmpty(jsr["isidentification"].ToString()))
                    {
                        dr["isidentification"] = jsr["isidentification"].ToString();
                    }
                    else
                    {
                        dr["isidentification"] = DBNull.Value;
                    }
                    //下单是否赠送红包，0:赠送；1:不赠送；默认为1
                    if (!string.IsNullOrEmpty(jsr["isdonate"].ToString()))
                    {
                        dr["isdonate"] = Convert.ToInt32(jsr["isdonate"].ToString());
                    }
                    else
                    {
                        dr["isdonate"] = DBNull.Value;
                    }
                    //下单赠送红包比例,1-99之间
                    if (!string.IsNullOrEmpty(jsr["donateratio"].ToString()))
                    {
                        dr["donateratio"] = Convert.ToInt32(jsr["donateratio"].ToString());
                    }
                    else
                    {
                        dr["donateratio"] = DBNull.Value;
                    }
                    //是否显示发布来源：0:显示；1:不显示；默认为0
                    if (!string.IsNullOrEmpty(jsr["isshowsource"].ToString()))
                    {
                        dr["isshowsource"] = Convert.ToInt32(jsr["isshowsource"].ToString());
                    }
                    else
                    {
                        dr["isshowsource"] = DBNull.Value;
                    }
                    //是否收取佣金；0:是；1:否；默认为1
                    if (!string.IsNullOrEmpty(jsr["iscost"].ToString()))
                    {
                        dr["iscost"] = Convert.ToInt32(jsr["iscost"].ToString());
                    }
                    else
                    {
                        dr["iscost"] = DBNull.Value;
                    }
                    //是否查货宝会员；0:是；1:否；默认为1
                    if (!string.IsNullOrEmpty(jsr["ischbmember"].ToString()))
                    {
                        dr["ischbmember"] = Convert.ToInt32(jsr["ischbmember"].ToString());
                    }
                    else
                    {
                        dr["ischbmember"] = DBNull.Value;
                    }

                    if (!string.IsNullOrEmpty(jsr["jd"].ToString()))
                    {
                        dr["jd"] = jsr["jd"].ToString();
                    }
                    else
                    {
                        dr["jd"] = DBNull.Value;
                    }
                    if (!string.IsNullOrEmpty(jsr["wd"].ToString()))
                    {
                        dr["wd"] = jsr["wd"].ToString();
                    }
                    else
                    {
                        dr["wd"] = DBNull.Value;
                    }

                    dt.Rows.Add(dr);
                    dbc.UpdateTable(dt, dtt);

                    //删除用户的角色关联
                    string del_js = "delete from tb_b_user_role where userId=@userId";
                    SqlCommand cmd = new SqlCommand(del_js);
                    cmd.Parameters.AddWithValue("@userId", YHID);
                    dbc.ExecuteNonQuery(cmd);

                    //建立用户角色关联
                    var rdt = dbc.GetEmptyDataTable("tb_b_user_role");
                    var rdr = rdt.NewRow();
                    rdr["userroleId"] = Guid.NewGuid().ToString();
                    rdr["userId"] = new Guid(YHID);
                    rdr["roleId"] = jsr["roleId"].ToString();
                    rdr["companyId"] = companyId;
                    rdt.Rows.Add(rdr);
                    dbc.InsertTable(rdt);

                    recordlog(dbc, YHID, DateTime.Now, 3, nr);
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
    /// 红包下单额度增加
    /// </summary>
    /// <param name="userid"></param>
    /// <param name="oldnum"></param>
    /// <param name="addnum"></param>
    /// <returns></returns>
    [CSMethod("AddRedNum")]
    public bool AddRedNum(string userid, string OldNum, string AddNum)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                #region 参数处理
                decimal oldnum = 0m;
                decimal addnum = 0m;
                if (!string.IsNullOrEmpty(OldNum))
                {
                    oldnum = Convert.ToDecimal(OldNum);
                }
                if (!string.IsNullOrEmpty(AddNum))
                {
                    addnum = Convert.ToDecimal(AddNum);
                }
                #endregion

                dbc.BeginTransaction();
                DateTime nowti = DateTime.Now;
                //更新用户表
                string sqlstr = "update tb_b_user set redenvelopequota=isnull(redenvelopequota,0)+" + addnum + " where UserID=" + dbc.ToSqlValue(userid);
                dbc.ExecuteNonQuery(sqlstr);

                //操作日志
                string nr = SystemUser.CurrentUser.UserName + "在" + nowti.ToString() + "时间增加了下单红包额度：" + addnum + "元，原额度为：" + oldnum + "元";
                recordlog(dbc, userid, DateTime.Now, 4, nr);

                //红包下单记录表
                var dt = dbc.GetEmptyDataTable("tb_b_user_quota");
                var dr = dt.NewRow();
                dr["id"] = Guid.NewGuid();
                dr["userid"] = Guid.Parse(userid);
                dr["quota"] = addnum;
                dr["status"] = 0;
                dr["adduser"] = SystemUser.CurrentUser.UserID;
                dr["addtime"] = nowti;
                dr["updateuser"] = SystemUser.CurrentUser.UserID;
                dr["updatetime"] = nowti;
                dt.Rows.Add(dr);
                dbc.InsertTable(dt);

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
    /// 黑名单设置
    /// </summary>
    /// <param name="userid"></param>
    /// <param name="isclose"></param>
    /// <param name="closeday"></param>
    /// <returns></returns>
    [CSMethod("EditHmd")]
    public bool EditHmd(string userid, int isclose, int closeday)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                dbc.BeginTransaction();
                string sqlstr = "select * from tb_b_user where UserID=" + dbc.ToSqlValue(userid);
                DataTable dt = dbc.ExecuteDataTable(sqlstr);
                string userName = "";
                if (dt.Rows.Count > 0)
                {
                    userName = dt.Rows[0]["UserName"].ToString();
                }

                DateTime nowti = DateTime.Now;
                //更新用户表
                if (isclose == 0)
                {
                    sqlstr = "update tb_b_user set isclose=" + isclose + ",closeday=" + closeday + " where UserID=" + dbc.ToSqlValue(userid);
                    dbc.ExecuteNonQuery(sqlstr);
                }
                else
                {
                    sqlstr = "update tb_b_user set isclose=" + isclose + ",closeday='' where UserID=" + dbc.ToSqlValue(userid);
                    dbc.ExecuteNonQuery(sqlstr);
                }

                //操作日志
                string cz = isclose == 0 ? "设置了【" + userName + "】黑名单（" + (closeday == 10000 ? "永久" : closeday + "天") + "）" : "关闭了【" + userName + "】黑名单";
                string nr = SystemUser.CurrentUser.UserName + "在" + nowti.ToString() + "时间" + cz;
                recordlog(dbc, userid, DateTime.Now, 5, nr);

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
    //[CSMethod("SaveUser")]
    //public bool SaveUser(JSReader jsr, JSReader yhjs,JSReader yhjsdw,JSReader qxids)
    //{
    //    if (jsr["LoginName"].IsNull || jsr["LoginName"].IsEmpty)
    //    {
    //        throw new Exception("用户名不能为空");
    //    }
    //    if (jsr["Password"].IsNull || jsr["Password"].IsEmpty)
    //    {
    //        throw new Exception("密码不能为空");
    //    }

    //    if (yhjs.ToArray().Length == 0)
    //    {
    //        throw new Exception("没有用户角色！");
    //    }

    //    var EditUser = SystemUser.CurrentUser;

    //    using (DBConnection dbc = new DBConnection())
    //    {
    //        dbc.BeginTransaction();
    //        try
    //        {
    //            if (jsr["User_ID"].ToString() == "")
    //            {
    //                DataTable dt_user = dbc.ExecuteDataTable("select * from tb_b_Users where LoginName='" + jsr["LoginName"].ToString() + "' and User_DelFlag=0");
    //                if (dt_user.Rows.Count > 0)
    //                {
    //                    throw new Exception("该用户名已存在！");
    //                }

    //                var YHID = Guid.NewGuid().ToString();
    //                //建立用户
    //                string sqlStr = "";
    //                if (jsr["QY_ID"].ToString() != "")
    //                {
    //                    sqlStr = "insert into tb_b_Users (User_ID,LoginName,Password,User_DM,User_XM,User_ZW,User_DH,User_SJ,User_Email,User_DZ,User_Enable,User_DelFlag,addtime,updatetime,updateuser,QY_ID) " +
    //                        "values (@User_ID,@LoginName,@Password,@User_DM,@User_XM,@User_ZW,@User_DH,@User_SJ,@User_Email,@User_DZ,@User_Enable,@User_DelFlag,@addtime,@updatetime,@updateuser,@qyid)";
    //                }
    //                else
    //                {
    //                    sqlStr = "insert into tb_b_Users (User_ID,LoginName,Password,User_DM,User_XM,User_ZW,User_DH,User_SJ,User_Email,User_DZ,User_Enable,User_DelFlag,addtime,updatetime,updateuser) " +
    //                        "values (@User_ID,@LoginName,@Password,@User_DM,@User_XM,@User_ZW,@User_DH,@User_SJ,@User_Email,@User_DZ,@User_Enable,@User_DelFlag,@addtime,@updatetime,@updateuser)";
    //                }
    //                SqlCommand cmd = new SqlCommand(sqlStr);
    //                cmd.Parameters.AddWithValue("@User_ID", YHID);
    //                cmd.Parameters.AddWithValue("@LoginName", jsr["LoginName"].ToString());
    //                cmd.Parameters.AddWithValue("@Password", jsr["Password"].ToString());
    //                cmd.Parameters.AddWithValue("@User_DM", jsr["User_DM"].ToString());
    //                cmd.Parameters.AddWithValue("@User_XM", jsr["User_XM"].ToString());
    //                cmd.Parameters.AddWithValue("@User_ZW", jsr["User_ZW"].ToString());
    //                cmd.Parameters.AddWithValue("@User_DH", jsr["User_DH"].ToString());
    //                cmd.Parameters.AddWithValue("@User_SJ", jsr["User_SJ"].ToString());
    //                cmd.Parameters.AddWithValue("@User_Email", jsr["User_Email"].ToString());
    //                cmd.Parameters.AddWithValue("@User_DZ", jsr["User_DZ"].ToString());
    //                if (jsr["QY_ID"].ToString() != "")
    //                {
    //                    cmd.Parameters.AddWithValue("@qyid", jsr["QY_ID"].ToString());
    //                }
    //                cmd.Parameters.AddWithValue("@User_Enable", Convert.ToInt32(jsr["User_Enable"].ToString()));
    //                cmd.Parameters.AddWithValue("@User_DelFlag", 0);
    //                cmd.Parameters.AddWithValue("@addtime", DateTime.Now);
    //                cmd.Parameters.AddWithValue("@updatetime", DateTime.Now);
    //                cmd.Parameters.AddWithValue("@updateuser", EditUser.UserID);

    //                dbc.ExecuteNonQuery(cmd);

    //                //建立用户角色关联
    //                for (int i = 0; i < yhjs.ToArray().Length; i++)
    //                {
    //                    string sqlstr_js = "insert into tb_b_User_JS_Gl (UserGl_id,User_ID,JS_ID,delflag,addtime,updatetime,updateuser) values(@UserGl_id,@User_ID,@JS_ID,@delflag,@addtime,@updatetime,@updateuser)";
    //                    cmd = new SqlCommand(sqlstr_js);
    //                    cmd.Parameters.AddWithValue("@UserGl_id", Guid.NewGuid());
    //                    cmd.Parameters.AddWithValue("@User_ID", YHID);
    //                    cmd.Parameters.AddWithValue("@JS_ID", yhjs.ToArray()[i].ToString());
    //                    cmd.Parameters.AddWithValue("@delflag", 0);
    //                    cmd.Parameters.AddWithValue("@addtime", DateTime.Now);
    //                    cmd.Parameters.AddWithValue("@updatetime", DateTime.Now);
    //                    cmd.Parameters.AddWithValue("@updateuser", EditUser.UserID);
    //                    dbc.ExecuteNonQuery(cmd);
    //                }


    //                //建立用户单位关联
    //                for (int i = 0; i < yhjsdw.ToArray().Length; i++)
    //                {
    //                    JSReader[] arr_dw = yhjsdw.ToArray()[i].ToArray();
    //                    for (int k = 0; k < arr_dw.Length; k++)
    //                    {
    //                        string sqlstr_dw = "insert into tb_b_User_Dw_Gl(UserDwGL_id,User_ID,DW_ID,delflag,addtime,updatetime,updateuser) values(@UserDwGL_id,@User_ID,@DW_ID,@delflag,@addtime,@updatetime,@updateuser)";
    //                        cmd = new SqlCommand(sqlstr_dw);
    //                        cmd.Parameters.AddWithValue("@UserDwGL_id", Guid.NewGuid());
    //                        cmd.Parameters.AddWithValue("@User_ID", YHID);
    //                        cmd.Parameters.AddWithValue("@DW_ID", arr_dw[k].ToString());
    //                        cmd.Parameters.AddWithValue("@delflag", 0);
    //                        cmd.Parameters.AddWithValue("@addtime", DateTime.Now);
    //                        cmd.Parameters.AddWithValue("@updatetime", DateTime.Now);
    //                        cmd.Parameters.AddWithValue("@updateuser", EditUser.UserID);
    //                        dbc.ExecuteNonQuery(cmd);
    //                    }
    //                }

    //                //建立用户权限关联
    //                for (int i = 0; i < qxids.ToArray().Length; i++)
    //                {
    //                    string sqlstr_qx = "insert into tb_b_YH_YHQX (PRIVILEGECODE,USERID) values(@PRIVILEGECODE,@USERID)";
    //                    cmd = new SqlCommand(sqlstr_qx);
    //                    cmd.Parameters.AddWithValue("@PRIVILEGECODE", new Guid(qxids.ToArray()[i]));
    //                    cmd.Parameters.AddWithValue("@USERID", YHID);
    //                    dbc.ExecuteNonQuery(cmd);
    //                }

    //            }
    //            else
    //            {
    //                var YHID = jsr["User_ID"].ToString();
    //                var oldname = dbc.ExecuteScalar("select LoginName from tb_b_Users where User_ID='" + YHID + "'");
    //                if (!jsr["LoginName"].ToString().Equals(oldname.ToString()))
    //                {
    //                    DataTable dt_user = dbc.ExecuteDataTable("select * from tb_b_Users where LoginName='" + jsr["LoginName"].ToString() + "' and User_DelFlag=0");
    //                    if (dt_user.Rows.Count > 0)
    //                    {
    //                        throw new Exception("该用户名已存在！");
    //                    }
    //                }
    //                var con = "";
    //                if (jsr["QY_ID"].ToString() != "")
    //                {
    //                    con = ",QY_ID='" + jsr["QY_ID"].ToString() + "'";
    //                }
    //                else
    //                {
    //                    con = ",QY_ID=null";

    //                }
    //                string sqlstr = "update tb_b_Users set LoginName=@LoginName,Password=@Password,User_DM=@User_DM,User_XM=@User_XM,User_ZW=@User_ZW,User_DH=@User_DH,User_SJ=@User_SJ,User_Email=@User_Email,User_DZ=@User_DZ,User_Enable=@User_Enable,updatetime=@updatetime,updateuser=@updateuser " + con + " where User_ID=@User_ID";
    //                SqlCommand cmd = new SqlCommand(sqlstr);
    //                cmd.Parameters.AddWithValue("@LoginName", jsr["LoginName"].ToString());
    //                cmd.Parameters.AddWithValue("@Password", jsr["Password"].ToString());
    //                cmd.Parameters.AddWithValue("@User_DM", jsr["User_DM"].ToString());
    //                cmd.Parameters.AddWithValue("@User_XM", jsr["User_XM"].ToString());
    //                cmd.Parameters.AddWithValue("@User_ZW", jsr["User_ZW"].ToString());
    //                cmd.Parameters.AddWithValue("@User_DH", jsr["User_DH"].ToString());
    //                cmd.Parameters.AddWithValue("@User_SJ", jsr["User_SJ"].ToString());
    //                cmd.Parameters.AddWithValue("@User_Email", jsr["User_Email"].ToString());
    //                cmd.Parameters.AddWithValue("@User_DZ", jsr["User_DZ"].ToString());
    //                cmd.Parameters.AddWithValue("@User_Enable", Convert.ToInt32(jsr["User_Enable"].ToString()));
    //                cmd.Parameters.AddWithValue("@updatetime", DateTime.Now);
    //                cmd.Parameters.AddWithValue("@updateuser", EditUser.UserID);
    //                cmd.Parameters.AddWithValue("@User_ID", YHID);
    //                dbc.ExecuteNonQuery(cmd);

    //                //删除用户的角色关联
    //                string del_js = "update tb_b_User_JS_Gl set delflag=1,updatetime=@updatetime,updateuser=@updateuser where User_ID=@User_ID";
    //                cmd = new SqlCommand(del_js);
    //                cmd.Parameters.AddWithValue("@updatetime", DateTime.Now);
    //                cmd.Parameters.AddWithValue("@updateuser", EditUser.UserID);
    //                cmd.Parameters.AddWithValue("@User_ID", YHID);
    //                dbc.ExecuteNonQuery(cmd);
    //                //删除用户的单位关联
    //                string del_dw = "update tb_b_User_Dw_Gl set delflag=1,updatetime=@updatetime,updateuser=@updateuser where User_ID=@User_ID";
    //                cmd = new SqlCommand(del_dw);
    //                cmd.Parameters.AddWithValue("@updatetime", DateTime.Now);
    //                cmd.Parameters.AddWithValue("@updateuser", EditUser.UserID);
    //                cmd.Parameters.AddWithValue("@User_ID", YHID);
    //                dbc.ExecuteNonQuery(cmd);
    //                //删除用户的权限关联
    //                string del_qx = "delete from tb_b_YH_YHQX where USERID=@USERID";
    //                cmd = new SqlCommand(del_qx);
    //                cmd.Parameters.AddWithValue("@USERID", YHID);
    //                dbc.ExecuteNonQuery(cmd);

    //                //建立用户角色关联
    //                for (int i = 0; i < yhjs.ToArray().Length; i++)
    //                {
    //                    string sqlstr_js = "insert into tb_b_User_JS_Gl (UserGl_id,User_ID,JS_ID,delflag,addtime,updatetime,updateuser) values(@UserGl_id,@User_ID,@JS_ID,@delflag,@addtime,@updatetime,@updateuser)";
    //                    cmd = new SqlCommand(sqlstr_js);
    //                    cmd.Parameters.AddWithValue("@UserGl_id", Guid.NewGuid());
    //                    cmd.Parameters.AddWithValue("@User_ID", YHID);
    //                    cmd.Parameters.AddWithValue("@JS_ID", yhjs.ToArray()[i].ToString());
    //                    cmd.Parameters.AddWithValue("@delflag", 0);
    //                    cmd.Parameters.AddWithValue("@addtime", DateTime.Now);
    //                    cmd.Parameters.AddWithValue("@updatetime", DateTime.Now);
    //                    cmd.Parameters.AddWithValue("@updateuser", EditUser.UserID);
    //                    dbc.ExecuteNonQuery(cmd);
    //                }


    //                //建立用户单位关联
    //                for (int i = 0; i < yhjsdw.ToArray().Length; i++)
    //                {
    //                    JSReader[] arr_dw = yhjsdw.ToArray()[i].ToArray();
    //                    for (int k = 0; k < arr_dw.Length; k++)
    //                    {
    //                        string sqlstr_dw = "insert into tb_b_User_Dw_Gl(UserDwGL_id,User_ID,DW_ID,delflag,addtime,updatetime,updateuser) values(@UserDwGL_id,@User_ID,@DW_ID,@delflag,@addtime,@updatetime,@updateuser)";
    //                        cmd = new SqlCommand(sqlstr_dw);
    //                        cmd.Parameters.AddWithValue("@UserDwGL_id", Guid.NewGuid());
    //                        cmd.Parameters.AddWithValue("@User_ID", YHID);
    //                        cmd.Parameters.AddWithValue("@DW_ID", arr_dw[k].ToString());
    //                        cmd.Parameters.AddWithValue("@delflag", 0);
    //                        cmd.Parameters.AddWithValue("@addtime", DateTime.Now);
    //                        cmd.Parameters.AddWithValue("@updatetime", DateTime.Now);
    //                        cmd.Parameters.AddWithValue("@updateuser", EditUser.UserID);
    //                        dbc.ExecuteNonQuery(cmd);
    //                    }
    //                }

    //                //建立用户权限关联
    //                for (int i = 0; i < qxids.ToArray().Length; i++)
    //                {
    //                    string sqlstr_qx = "insert into tb_b_YH_YHQX (PRIVILEGECODE,USERID) values(@PRIVILEGECODE,@USERID)";
    //                    cmd = new SqlCommand(sqlstr_qx);
    //                    cmd.Parameters.AddWithValue("@PRIVILEGECODE", new Guid(qxids.ToArray()[i]));
    //                    cmd.Parameters.AddWithValue("@USERID", YHID);
    //                    dbc.ExecuteNonQuery(cmd);
    //                }
    //            }

    //            dbc.CommitTransaction();
    //            return true;
    //        }
    //        catch (Exception ex)
    //        {
    //            dbc.RoolbackTransaction();
    //            throw ex;
    //        }
    //    }
    //}

    //[CSMethod("SaveUser")]
    //public bool SaveUser(JSReader jsr, string[] Privileges)
    //{
    //    if (jsr["YH_DLM"].IsNull || jsr["YH_DLM"].IsEmpty)
    //    {
    //        throw new Exception("登陆名不能为空");
    //    }
    //    if (jsr["YH_MIMA"].IsNull || jsr["YH_MIMA"].IsEmpty)
    //    {
    //        throw new Exception("密码不能为空");
    //    }
    //    if (jsr["DW_LX"].IsNull || jsr["DW_LX"].IsEmpty)
    //    {
    //        throw new Exception("单位类型不能为空");
    //    }
    //    if (jsr["DW_ID"].IsNull || jsr["DW_ID"].IsEmpty)
    //    {
    //        throw new Exception("密码不能为空");
    //    }
    //    bool UserEditSuccess = false;
    //    if (jsr["YH_ID"].ToString() == "")
    //    {
    //        UserEditSuccess = SystemUser.CreateUser(jsr["YH_DLM"].ToString(),"", jsr["YH_MIMA"].ToString(),"","","","","","");
    //    }
    //    else
    //    {
    //        UserEditSuccess = EditUser(jsr["YH_DLM"].ToString(), jsr["YH_MIMA"].ToString(), jsr["DW_ID"].ToString(), jsr["YH_ID"].ToString());
    //    }
    //    if (UserEditSuccess)
    //    {

    //        if (jsr["DW_LX"].ToString() == "0" && Privileges.Length > 0)
    //        {
    //            try
    //            {
    //                SystemUser.GetUserByUserName(jsr["YH_DLM"].ToString()).RemoveAllPriviledge();
    //                foreach (string Privilege in Privileges)
    //                {
    //                    SystemUser.GetUserByUserName(jsr["YH_DLM"].ToString()).AddPriviledge(new Guid(Privilege));
    //                }
    //                UserEditSuccess = true;
    //            }
    //            catch
    //            {
    //                UserEditSuccess = false;
    //            }
    //        }
    //    }
    //    return UserEditSuccess;
    //}

    //[CSMethod("DelUser")]
    //public bool DelUser(string userid)
    //{
    //    if (userid.Trim() == "")
    //        return false;
    //    using (DBConnection dbc = new DBConnection())
    //    {
    //        int retInt = dbc.ExecuteNonQuery("update tb_b_Users set User_DelFlag = 1 where user_id = '" + userid + "'");
    //        if (retInt > 0)
    //            return true;
    //        return false;
    //    }
    //}

    [CSMethod("DelUser")]
    public bool DelUser(JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                for (int i = 0; i < jsr.ToArray().Length; i++)
                {
                    string delstr = "delete from tb_b_user_role where userId=@userId";
                    SqlCommand cmd = new SqlCommand(delstr);
                    cmd.Parameters.AddWithValue("@userId", jsr.ToArray()[i].ToString());
                    dbc.ExecuteNonQuery(cmd);

                    string str = "delete from tb_b_user where UserID=@UserID and (ClientKind<>1 and ClientKind<>2)";
                    SqlCommand ucmd = new SqlCommand(str);
                    ucmd.Parameters.AddWithValue("@UserID", jsr.ToArray()[i].ToString());
                    dbc.ExecuteNonQuery(ucmd);
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


    [CSMethod("DelUserByids")]
    public bool DelUserByids(JSReader jsr)
    {
        var user = SystemUser.CurrentUser;
        string userid = user.UserID;

        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                for (int i = 0; i < jsr.ToArray().Length; i++)
                {
                    string str = "update tb_b_Users set User_DelFlag = 1,Updatetime=@Updatetime,Updateuser=@Updateuser where user_id =@user_id";
                    SqlCommand cmd = new SqlCommand(str);
                    cmd.Parameters.AddWithValue("@Updatetime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Updateuser", userid);
                    cmd.Parameters.AddWithValue("@user_id", jsr.ToArray()[i].ToString());
                    dbc.ExecuteNonQuery(cmd);

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

    [CSMethod("EnableUser")]
    public bool EnableUser(string[] userIds, bool enable)
    {
        if (userIds.Length > 0)
        {
            string userIdStr = "'" + string.Join(",", userIds).Replace(",", "','") + "'";
            var sqlStr = string.Format("update tb_b_Users set User_Enable = @YH_ENABLE where User_ID IN({0})", userIdStr);
            SqlCommand cmd = new SqlCommand(sqlStr);
            cmd.Parameters.AddWithValue("@YH_ENABLE", enable ? 0 : 1);
            using (DBConnection dbc = new DBConnection())
            {
                var retInt = dbc.ExecuteNonQuery(cmd);
                if (retInt > 0)
                    return true;
                return false;
            }
        }
        return false;
    }
    private bool EditUser(string dlm, string mima, string dwid, string userid)
    {
        string sqlStr = "update tzclz_t_yh set yh_dlm=:yh_dlm,yh_mima = :yh_mima,updatetime = sysdate,dw_id = :dw_id,XGYH_ID = :XGYH_ID where YH_ID = :YH_ID and status = 0";
        SqlCommand cmd = new SqlCommand(sqlStr);
        cmd.Parameters.AddWithValue("yh_dlm", dlm);
        cmd.Parameters.AddWithValue("yh_mima", mima);
        cmd.Parameters.AddWithValue("dw_id", dwid);
        cmd.Parameters.AddWithValue("YH_ID", userid);
        cmd.Parameters.AddWithValue("XGYH_ID", SystemUser.CurrentUser.UserID);
        using (DBConnection dbc = new DBConnection())
        {
            int retInt = dbc.ExecuteNonQuery(cmd);
            if (retInt > 0)
                return true;
            return false;
        }
    }
    [CSMethod("GetAllPrivilege")]
    public object GetAllPrivilege()
    {
        var Mod = PrivilegeDescription.PrivilegeType();

        using (DBConnection dbc = new DBConnection())
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" select t.orderno,");
            sb.Append(" substr(ModuleName, 0, instr(ModuleName, '-') -1) as Mod,");
            sb.Append(" substr(ModuleName,");
            sb.Append(" instr(ModuleName, '-') + 1,");
            sb.Append(" (length(ModuleName) - instr(ModuleName, '-'))) as Item");
            sb.Append(" ,t.privilegecode");
            sb.Append(" from TZCLZ_T_YH_QX t");
            var Items = dbc.ExecuteDataTable(sb.ToString());
            return new { Mod = Mod, Items = Items };
        }
    }
    [CSMethod("GetUserPrivileges")]
    public string[] GetAllPrivilege(string userid)
    {
        List<string> Privileges = new List<string>();

        var dtPrivilege = SystemUser.GetUserByID(userid).GetUserPriviledgeInfo();
        foreach (DataRow drPrivilege in dtPrivilege.Rows)
        {
            Privileges.Add(drPrivilege["PRIVILEGECODE"].ToString());
        }
        return Privileges.ToArray();
    }

    [CSMethod("GetDQS")]
    public DataTable GetDQS()
    {
        using (DBConnection dbc = new DBConnection())
        {
            string str = "select dq_mc,dq_bm from tb_b_dq where dq_sj='000000' and status=0 order by dq_bm";
            DataTable dt = dbc.ExecuteDataTable(str);
            return dt;
        }
    }

    [CSMethod("GetDQ")]
    public DataTable GetDQ(string dqbm)
    {
        using (DBConnection dbc = new DBConnection())
        {
            string str = "select dq_mc,dq_bm from tb_b_dq where dq_sj=" + dbc.ToSqlValue(dqbm) + " and status=0 order by dq_bm";
            DataTable dt = dbc.ExecuteDataTable(str);
            return dt;
        }
    }

    [CSMethod("GetKP")]
    public DataTable GetKP()
    {
        using (DBConnection dbc = new DBConnection())
        {
            string str = "select id,name from tb_b_carriagechb order by name";
            DataTable dt = dbc.ExecuteDataTable(str);
            return dt;
        }
    }

    [CSMethod("GetEWMToFile", 2)]
    public byte[] GetEWMToFile(string userid)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {


                Bitmap bt = QRCodeBimapForString("http://share.chahuobao.net/freight/html/htbd.html?userid=" + userid);
                MemoryStream ms = new MemoryStream();
                bt.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                byte[] bytes = ms.GetBuffer();
                return bytes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
    [CSMethod("GetEWMToFile1", 2)]
    public byte[] GetEWMToFile1(string userid)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {


                Bitmap bt = QRCodeBimapForString("http://share.chahuobao.net/freight/html/sfbd.html?userid=" + userid);
                MemoryStream ms = new MemoryStream();
                bt.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                byte[] bytes = ms.GetBuffer();
                return bytes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    public Bitmap QRCodeBimapForString(string nr)
    {
        string enCodeString = nr;
        QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
        //编码方式(注意：BYTE能支持中文，ALPHA_NUMERIC扫描出来的都是数字)
        qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
        qrCodeEncoder.QRCodeScale = 4;//大小(值越大生成的二维码图片像素越高)
        //版本(注意：设置为0主要是防止编码的字符串太长时发生错误)
        qrCodeEncoder.QRCodeVersion = 7;
        //错误效验、错误更正(有4个等级)
        qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;

        return qrCodeEncoder.Encode(enCodeString, Encoding.GetEncoding("GB2312"));
    }

    #region 分享明细
    [CSMethod("getShareList")]
    public object getShareList(int pagnum, int pagesize, string isregister, string isbuy, string start, string end, string tjr, string btjr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";

                if (!string.IsNullOrEmpty(isregister))
                {
                    where += " and " + dbc.C_EQ("a.isregister", isregister);
                }
                if (!string.IsNullOrEmpty(isbuy))
                {
                    where += " and " + dbc.C_EQ("a.isbuy", isbuy);
                }
                if (!string.IsNullOrEmpty(start))
                {
                    where += " and a.addtime>='" + Convert.ToDateTime(start).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.addtime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(tjr))
                {
                    where += " and (" + dbc.C_Like("b.UserName", tjr, LikeStyle.LeftAndRightLike) + " or " + dbc.C_Like("b.UserXM", tjr, LikeStyle.LeftAndRightLike) + " )";
                }

                if (!string.IsNullOrEmpty(btjr))
                {
                    where += " and (" + dbc.C_Like("c.UserName", btjr, LikeStyle.LeftAndRightLike) + " or " + dbc.C_Like("c.UserXM", btjr, LikeStyle.LeftAndRightLike) + " )";
                }

                string str = @" select a.*,b.UserName as tjname,b.UserXM as tjxm,c.UserName as btjname,c.UserXM as btjxm from tb_b_share a left join tb_b_user b on a.userid=b.UserID
                                left join tb_b_user c on a.tel=c.UserName   where 1=1";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order  by a.addtime ", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    [CSMethod("getShareListToFile", 2)]
    public byte[] getShareListToFile(string isregister, string isbuy, string start, string end, string tjr, string btjr)
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
                cells[0, 0].PutValue("推荐人登录名");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("推荐人姓名");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("被推荐人登录名");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);
                cells[0, 3].PutValue("被推荐人姓名");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("时间");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("是否注册");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);
                cells[0, 6].PutValue("是否购买");
                cells[0, 6].SetStyle(style2);
                cells.SetColumnWidth(6, 20);

                string where = "";

                if (!string.IsNullOrEmpty(isregister))
                {
                    where += " and " + dbc.C_EQ("a.isregister", isregister);
                }
                if (!string.IsNullOrEmpty(isbuy))
                {
                    where += " and " + dbc.C_EQ("a.isbuy", isbuy);
                }
                if (!string.IsNullOrEmpty(start))
                {
                    where += " and a.addtime>='" + Convert.ToDateTime(start).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and a.addtime<'" + Convert.ToDateTime(end).AddDays(1).ToString("yyyy-MM-dd") + "'";
                }
                if (!string.IsNullOrEmpty(tjr))
                {
                    where += " and (" + dbc.C_Like("b.UserName", tjr, LikeStyle.LeftAndRightLike) + " or " + dbc.C_Like("b.UserXM", tjr, LikeStyle.LeftAndRightLike) + " )";
                }

                if (!string.IsNullOrEmpty(btjr))
                {
                    where += " and (" + dbc.C_Like("c.UserName", btjr, LikeStyle.LeftAndRightLike) + " or " + dbc.C_Like("c.UserXM", btjr, LikeStyle.LeftAndRightLike) + " )";
                }

                string str = @" select a.*,b.UserName as tjname,b.UserXM as tjxm,c.UserName as btjname,c.UserXM as btjxm from tb_b_share a left join tb_b_user b on a.userid=b.UserID
                                left join tb_b_user c on a.tel=c.UserName   where 1=1";
                str += where;

                //开始取分页数据
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = dbc.ExecuteDataTable(str + " order  by a.addtime ");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["tjname"]);
                    cells[i + 1, 0].SetStyle(style4);
                    cells[i + 1, 1].PutValue(dt.Rows[i]["tjxm"]);
                    cells[i + 1, 1].SetStyle(style4);
                    cells[i + 1, 2].PutValue(dt.Rows[i]["btjname"]);
                    cells[i + 1, 2].SetStyle(style4);
                    cells[i + 1, 3].PutValue(dt.Rows[i]["btjxm"]);
                    cells[i + 1, 3].SetStyle(style4);
                    if (dt.Rows[i]["addtime"] != null && dt.Rows[i]["addtime"].ToString() != "")
                    {
                        cells[i + 1, 4].PutValue(Convert.ToDateTime(dt.Rows[i]["addtime"]).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    cells[i + 1, 4].SetStyle(style4);

                    string register = "";
                    if (dt.Rows[i]["isregister"] != null && dt.Rows[i]["isregister"].ToString() != "")
                    {

                        if (Convert.ToInt32(dt.Rows[i]["isregister"]) == 0)
                        {
                            register = "否";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["isregister"]) == 1)
                        {
                            register = "是";
                        }
                    }
                    cells[i + 1, 5].PutValue(register);
                    cells[i + 1, 5].SetStyle(style4);

                    string buy = "";
                    if (dt.Rows[i]["isbuy"] != null && dt.Rows[i]["isbuy"].ToString() != "")
                    {

                        if (Convert.ToInt32(dt.Rows[i]["isbuy"]) == 0)
                        {
                            buy = "否";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["isbuy"]) == 1)
                        {
                            buy = "是";
                        }
                    }
                    cells[i + 1, 6].PutValue(buy);
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

    #region 分流点管理
    [CSMethod("getNetWorkList")]
    public object getNetWorkList(int pagnum, int pagesize, string cx_zxmc)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";

                if (!string.IsNullOrEmpty(cx_zxmc))
                {
                    where += " and " + dbc.C_Like("b.UserXM", cx_zxmc, LikeStyle.LeftAndRightLike);
                }

                string str = @" select a.*,b.UserXM as zxmc from tb_b_network a 
                                left join tb_b_user b on a.userid=b.UserID
                                where status=0 " + where + " order by a.networkAddtime desc";

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

    [CSMethod("SaveNetWork")]
    public bool SaveNetWork(JSReader jsr)
    {
        if (jsr["jd"].IsNull || jsr["jd"].IsEmpty)
        {
            throw new Exception("经度不能为空");
        }
        if (jsr["wd"].IsNull || jsr["wd"].IsEmpty)
        {
            throw new Exception("纬度不能为空");
        }
        if (jsr["networkAddress"].IsNull || jsr["networkAddress"].IsEmpty)
        {
            throw new Exception("地址不能为空");
        }

        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                var networkId = jsr["networkId"].ToString();

                var dt = dbc.GetEmptyDataTable("tb_b_network");
                var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                var dr = dt.NewRow();
                dr["networkId"] = new Guid(networkId);
                dr["networkName"] = jsr["networkName"].ToString();
                dr["networkAddress"] = jsr["networkAddress"].ToString();
                dr["networkPerson"] = jsr["networkPerson"].ToString();
                dr["networkTel"] = jsr["networkTel"].ToString();
                dr["networkType"] = jsr["networkType"].ToString();
                //dr["networkAddtime"] = jsr["networkAddtime"].ToString();
                dr["status"] = 0;
                dr["userid"] = jsr["userid"].ToString();
                dr["jd"] = jsr["jd"].ToString();
                dr["wd"] = jsr["wd"].ToString();
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

    [CSMethod("getNetWorkListToFile", 2)]
    public byte[] getNetWorkListToFile(string cx_zxmc)
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
                cells[0, 0].PutValue("专线名称");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("网点类型");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("分流点/网点名称");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 30);
                cells[0, 3].PutValue("联系人");
                cells[0, 3].SetStyle(style2);
                cells.SetColumnWidth(3, 20);
                cells[0, 4].PutValue("联系电话");
                cells[0, 4].SetStyle(style2);
                cells.SetColumnWidth(4, 20);
                cells[0, 5].PutValue("经度");
                cells[0, 5].SetStyle(style2);
                cells.SetColumnWidth(5, 20);
                cells[0, 6].PutValue("纬度");
                cells[0, 6].SetStyle(style2);
                cells.SetColumnWidth(6, 20);
                cells[0, 7].PutValue("地址");
                cells[0, 7].SetStyle(style2);
                cells.SetColumnWidth(7, 20);

                string where = "";

                if (!string.IsNullOrEmpty(cx_zxmc))
                {
                    where += " and " + dbc.C_Like("b.UserXM", cx_zxmc, LikeStyle.LeftAndRightLike);
                }

                string str = @" select a.*,b.UserXM as zxmc from tb_b_network a 
                                left join tb_b_user b on a.userid=b.UserID
                                where status=0 " + where + " order by a.networkAddtime desc";

                //开始取分页数据
                System.Data.DataTable dt = new System.Data.DataTable();
                dt = dbc.ExecuteDataTable(str);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cells[i + 1, 0].PutValue(dt.Rows[i]["zxmc"]);
                    cells[i + 1, 0].SetStyle(style4);
                    string typeName = "";
                    if (dt.Rows[i]["networkType"] != null && dt.Rows[i]["networkType"].ToString() != "")
                    {

                        if (Convert.ToInt32(dt.Rows[i]["networkType"]) == 0)
                        {
                            typeName = "网点";
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["networkType"]) == 1)
                        {
                            typeName = "分流点";
                        }
                    }
                    cells[i + 1, 1].PutValue(typeName);
                    cells[i + 1, 1].SetStyle(style4);
                    cells[i + 1, 2].PutValue(dt.Rows[i]["networkName"]);
                    cells[i + 1, 2].SetStyle(style4);
                    cells[i + 1, 3].PutValue(dt.Rows[i]["networkPerson"]);
                    cells[i + 1, 3].SetStyle(style4);
                    cells[i + 1, 4].PutValue(dt.Rows[i]["networkTel"]);
                    cells[i + 1, 4].SetStyle(style4);
                    cells[i + 1, 5].PutValue(dt.Rows[i]["jd"]);
                    cells[i + 1, 5].SetStyle(style4);
                    cells[i + 1, 6].PutValue(dt.Rows[i]["wd"]);
                    cells[i + 1, 6].SetStyle(style4);
                    cells[i + 1, 7].PutValue(dt.Rows[i]["networkAddress"]);
                    cells[i + 1, 7].SetStyle(style4);
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

    [CSMethod("JudgeUser")]
    public object JudgeUser()
    {

        var privilege = new SystemUser().GetPrivilegeList(SystemUser.CurrentUser.RoleID);
        var mmck = false;
        for (int i = 0; i < privilege.Rows.Count; i++)
        {
            if (privilege.Rows[i]["privilegeName"] != null && privilege.Rows[i]["privilegeName"].ToString() != "")
            {
                if (privilege.Rows[i]["privilegeName"].ToString() == "系统维护中心_用户管理_密码查看")
                {
                    mmck = true;
                }
            }
        }

        return mmck;
    }

    [CSMethod("IsBdBf")]
    public object IsBdBf(string username)
    {
        //构造获取openid及access_token的url
        WxPayData data = new WxPayData();
        data.SetValue("loginmobile", username);
        string url = "http://47.110.134.105:8011/BAOHUTONG_H5/findCustInfo.action?" + data.ToUrl();

        //请求url以获取数据
        string json = HttpService.Get(url);
        ToJsonMy2 ToJsonMy2 = JsonConvert.DeserializeObject<ToJsonMy2>(json);
        return ToJsonMy2;
    }
    #region
    [CSMethod("GetProductImages1")]
    public object GetProductImages1(string pid)
    {

        string _url = System.Configuration.ConfigurationManager.AppSettings["ServiceURL"].ToString() + "tbbuserphoto.selectUserphoto";
        string jsonParam = new JavaScriptSerializer().Serialize(new
        {
            tradeCode = "tbbuserphoto.selectUserphoto",
            userid = pid,
            userphotogltype = 99
        });
        var request1 = (HttpWebRequest)WebRequest.Create(_url);
        request1.Method = "POST";
        request1.ContentType = "application/json;charset=UTF-8";
        var byteData = Encoding.UTF8.GetBytes(jsonParam);
        var length = byteData.Length;
        request1.ContentLength = length;
        var writer = request1.GetRequestStream();
        writer.Write(byteData, 0, length);
        writer.Close();
        var response = (HttpWebResponse)request1.GetResponse();
        var responseString = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();

        return responseString;

    }

    [CSMethod("UploadPicForProduct1", 1)]
    public object UploadPicForProduct1(FileData[] fds, string UserID)
    {
        string ServiceURL = System.Configuration.ConfigurationManager.AppSettings["ServiceURL"].ToString();
        WebRequest request = (HttpWebRequest)WebRequest.Create(ServiceURL + "uploadMultipleFiles");
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
        string _url = ServiceURL + "tbbuserphoto.update";
        string jsonParam = new JavaScriptSerializer().Serialize(new
        {
            userid = UserID,
            userphotogltype = 99,
            fileList = list
        });
        var request1 = (HttpWebRequest)WebRequest.Create(_url);
        request1.Method = "POST";
        request1.ContentType = "application/json;charset=UTF-8";
        var byteData = Encoding.UTF8.GetBytes(jsonParam);
        var length = byteData.Length;
        request1.ContentLength = length;
        var writer = request1.GetRequestStream();
        writer.Write(byteData, 0, length);
        writer.Close();
        var response = (HttpWebResponse)request1.GetResponse();
        var responseString = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")).ReadToEnd();
        return new { fileurl = list[0].fileFullUrl, isdefault = 0, fileid = list[0].fjId };

    }
    #endregion

    [CSMethod("Shpass")]
    public void Shpass(string userid, string isshpass)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                dbc.BeginTransaction();
                string sqlstr = @"update tb_b_user set IsSHPass=" + dbc.ToSqlValue(isshpass) + " where UserID=" + dbc.ToSqlValue(userid);
                dbc.ExecuteNonQuery(sqlstr);

                int type = 1;
                string nr = "专线下架";
                if (isshpass == "1")
                {
                    type = 2;
                    nr = "专线重新上架";
                }
                recordlog(dbc, userid, DateTime.Now, type, nr);

                dbc.CommitTransaction();
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }

    /// <summary>
    /// 用户管理操作日志
    /// </summary>
    /// <param name="dbc"></param>
    /// <param name="handlers"></param>
    /// <param name="createtime"></param>
    /// <param name="type">1.下架 2.重新上架  3.修改 4.下单红包额度</param>
    public void recordlog(DBConnection dbc, string handlers, DateTime createtime, int type, string content)
    {
        string leixing = "";
        switch (type)
        {
            case 1:
                leixing = "专线下架";
                break;
            case 2:
                leixing = "专线重新上架";
                break;
            case 3:
                leixing = "用户修改";
                break;
            case 4:
                leixing = "下单红包额度";
                break;
            case 5:
                leixing = "黑名单设置";
                break;
        }

        var dt = dbc.GetEmptyDataTable("tb_b_record");
        var dr = dt.NewRow();
        dr["CaoZuoJiLuID"] = Guid.NewGuid();
        dr["UserID"] = new Guid(handlers);
        dr["CaoZuoLeiXing"] = leixing;
        dr["CaoZuoNeiRong"] = content;
        dr["CaoZuoTime"] = createtime;
        dt.Rows.Add(dr);
        dbc.InsertTable(dt);
    }

    #region 分享记录
    [CSMethod("GetShareRecordByPage")]
    public object GetShareRecordByPage(int pagnum, int pagesize, string beg, string end, string username)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                string where2 = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  AddTime>='" + Convert.ToDateTime(beg).ToString() + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and AddTime<='" + Convert.ToDateTime(end).AddDays(1).ToString() + "'";
                }

                if (!string.IsNullOrEmpty(username.Trim()))
                {
                    where2 = " where b.UserName like '%" + username + "%'";
                }

                string str = @"select t.*,b.UserName,b.UserXM from(
	                                select userid,YEAR(addtime) y,month(addtime) m,day(addtime) d,COUNT(1) recordNum from tb_b_share_record
	                                where Status=0" + where + @"
	                                group by userid,YEAR(addtime),month(addtime),day(addtime)
                                )t
                                left join tb_b_user b on t.userid=b.UserID " + where2 + @"
                                order by y desc,m desc,d desc";

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

    [CSMethod("GetShareRecordToFile", 2)]
    public byte[] GetShareRecordToFile(string beg, string end, string username)
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
                cells[0, 0].PutValue("日期");
                cells[0, 0].SetStyle(style2);
                cells.SetColumnWidth(0, 20);
                cells[0, 1].PutValue("账号");
                cells[0, 1].SetStyle(style2);
                cells.SetColumnWidth(1, 20);
                cells[0, 2].PutValue("数量");
                cells[0, 2].SetStyle(style2);
                cells.SetColumnWidth(2, 20);

                string where = "";
                string where2 = "";
                if (!string.IsNullOrEmpty(beg))
                {
                    where += " and  AddTime>='" + Convert.ToDateTime(beg).ToString() + "'";
                }
                if (!string.IsNullOrEmpty(end))
                {
                    where += " and AddTime<='" + Convert.ToDateTime(end).AddDays(1).ToString() + "'";
                }

                if (!string.IsNullOrEmpty(username.Trim()))
                {
                    where2 = " where b.UserName like '%" + username + "%'";
                }

                string str = @"select t.*,b.UserName,b.UserXM from(
	                                select userid,YEAR(addtime) y,month(addtime) m,day(addtime) d,COUNT(1) recordNum from tb_b_share_record
	                                where Status=0" + where + @"
	                                group by userid,YEAR(addtime),month(addtime),day(addtime)
                                )t
                                left join tb_b_user b on t.userid=b.UserID " + where2 + @"
                                order by y desc,m desc,d desc";

                DataTable dt = dbc.ExecuteDataTable(str);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[i]["y"].ToString()) && !string.IsNullOrEmpty(dt.Rows[i]["m"].ToString()) && !string.IsNullOrEmpty(dt.Rows[i]["d"].ToString()))
                    {
                        cells[i + 1, 0].PutValue(dt.Rows[i]["y"].ToString() + "-" + dt.Rows[i]["m"].ToString() + "-" + dt.Rows[i]["d"].ToString());
                    }
                    else
                    {
                        cells[i + 1, 0].PutValue("");
                    }
                    cells[i + 1, 0].SetStyle(style4);

                    if (!string.IsNullOrEmpty(dt.Rows[i]["UserName"].ToString()))
                    {
                        cells[i + 1, 1].PutValue(dt.Rows[i]["UserName"]);
                    }
                    else
                    {
                        cells[i + 1, 1].PutValue("");
                    }
                    cells[i + 1, 1].SetStyle(style4);

                    cells[i + 1, 2].PutValue(dt.Rows[i]["recordNum"]);
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
    #endregion

}
public class ToJsonMy2
{
    public bool success { get; set; }
    public result result { get; set; }
    public string errorCode { get; set; }
    public string errorMsg { get; set; }

}

public class result
{
    public string contractNo { get; set; }
    public string platformNo { get; set; }
    public string loginMobile { get; set; }
    public string realNameFlag { get; set; }
    public string bindCardFlag { get; set; }
    public string operatorStatus { get; set; }
    public string customerName { get; set; }
    public string certificateNo { get; set; }

    public result(string contractNo, string platformNo, string loginMobile, string realNameFlag, string bindCardFlag, string operatorStatus, string customerName, string certificateNo)
    {
        this.contractNo = contractNo;
        this.platformNo = platformNo;
        this.loginMobile = loginMobile;
        this.realNameFlag = realNameFlag;
        this.bindCardFlag = bindCardFlag;
        this.operatorStatus = operatorStatus;
        this.customerName = customerName;
        this.certificateNo = certificateNo;
    }
}
