using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ZJH.BaseTools.DB;
using ZJH.BaseTools.IO;
using ZJH.BaseTools.Text;

namespace GISETL.Controllers
{
    public class DataSourceController : Controller
    {
        //获取数据源列表
        public ActionResult GetList()
        {
            Result result = null;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
                {
                    string sql = "select * from etl_data_source";
                    var list = helper.ExecuteReader_ToList(sql);
                    result = Result.CreateSuccess("成功", list);
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("DataSource/GetList", ex);
            }
            return Content(result.ToString(), "application/json");
        }
        //保存新建数据源
        public ActionResult Save(string datatsourceJSON)
        {
            Result result = null;
            try
            {
                List<string> sqls = new List<string>();
                JObject newdatasource = JObject.Parse(datatsourceJSON);
                string ID = newdatasource["ID"].ToString();
                string NAME = newdatasource["NAME"].ToString();
                string TYPE = newdatasource["TYPE"].ToString();
                string FILEPATH = newdatasource["FILEPATH"].ToString();//SDE\GDB\MDB\SHP数据类型填写
                string CONNECT_STR = newdatasource["CONNECT_STR"].ToString();//数据库类型填写 ORACLE
                string SERVER_URL = newdatasource["SERVER_URL"].ToString();//mapserver 填写
                // 更新或插入新数据源
                sqls.Add($"delete from ETL_DATA_SOURCE where id='{ID}'");
                sqls.Add($"insert into ETL_DATA_SOURCE(ID,NAME,TYPE,FILEPATH,CONNECT_STR,SERVER_URL) values('{ID}','{NAME}','{TYPE}','{FILEPATH}','{CONNECT_STR}','{SERVER_URL}')");
                // 以数据库事务执行SQL
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
                {
                    result = helper.ExecuteSqlTran(sqls) ? Result.Success : Result.Defeat;
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("DataSource/Save", ex);
            }
            return Content(result.ToString(), "application/json");
        }
        //删除数据源
        public ActionResult Delete(string datasoureid)
        {
            Result result = null;
            try
            {
                List<string> sqls = new List<string>();
                // 删除数据源
                sqls.Add($"delete from ETL_DATA_SOURCE where id='{datasoureid}'");
                // 以数据库事务执行SQL
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
                {
                    result = helper.ExecuteSqlTran(sqls) ? Result.Success : Result.Defeat;
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("DataSource/Delete", ex);
            }
            return Content(result.ToString(), "application/json");
        }
    }
}