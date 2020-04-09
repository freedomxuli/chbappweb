using SmartFramework4v2.Data.MySql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// MySqlConnstr 的摘要说明
/// </summary>
public class MySqlConnstr
{
	public MySqlConnstr()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}
    public static  MySqlDbConnection GetDBConnection()
    {
        return new MySqlDbConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnStr"].ConnectionString);
    }

    
    ///使用示例
    //using (MySqlDbConnection dbc = GetDBConnection())
     //       {
     //           语句和sqlserver通用
     //       }




}