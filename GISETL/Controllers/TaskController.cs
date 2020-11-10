using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZJH.BaseTools.DB;
using ZJH.BaseTools.IO;
using ZJH.BaseTools.Text;

namespace GISETL.Controllers
{
    public class TaskController : Controller
    {
        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetList()
        {
            Result result = null;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
                {
                    string sql = "select * from v_etl_task";
                    var list = helper.ExecuteReader_ToList(sql);
                    list.ForEach(delegate (Dictionary<string, object> dict) {
                        string task_id = dict["ID"].ToString();
                        sql = $"select * from etl_task_param where task_id='{task_id}'";
                        var paramLst = helper.ExecuteReader_ToList(sql);
                        dict.Add("PARAMS", paramLst);
                    });
                    result = Result.CreateSuccess("成功", list);
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("Task/GetList", ex);
            }
            return Content(result.ToString(), "application/json");
        }
        /// <summary>
        /// 获取需要填写的动态参数
        /// </summary>
        /// <param name="model_id"></param>
        /// <returns></returns>
        public ActionResult GetDynamicParams(string model_id)
        {
            Result result = null;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
                {
                    string sql = $"select * from v_etl_dynamic_param where model_id='{model_id}'";
                    var list = helper.ExecuteReader_ToList(sql);
                    result = Result.CreateSuccess("成功", list);
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("Task/GetDynamicParams", ex);
            }
            return Content(result.ToString(), "application/json");
        }
        /// <summary>
        /// 保存任务
        /// </summary>
        /// <param name="taskJSON"></param>
        /// <returns></returns>
        public ActionResult Save(string taskJSON)
        {
            Result result = null;
            try
            {
                List<string> sqls = new List<string>();
                JObject taskObj = JObject.Parse(taskJSON);
                string ID = taskObj["ID"].ToString();
                // 删除任务及相关的表记录
                sqls.AddRange(GetDeleteSQL(ID));
                // 插入任务
                sqls.Add(GetInsertSQL(taskObj));
                // 插入任务参数
                JArray paramArr = taskObj["PARAMS"] as JArray;
                sqls.AddRange(GetParamSQL(paramArr));
                // 以数据库事务执行SQL
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
                {
                    result = helper.ExecuteSqlTran(sqls) ? Result.Success : Result.CreateDefeat("保存失败");
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("Task/Save", ex);
            }
            return Content(result.ToString(), "application/json");
        }
        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="task_id"></param>
        /// <returns></returns>
        public ActionResult Delete(string task_id)
        {
            Result result = null;
            try
            {
                List<string> sqls = GetDeleteSQL(task_id, true);
                // 以数据库事务执行SQL
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
                {
                    result = helper.ExecuteSqlTran(sqls) ? Result.Success : Result.Defeat;
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("Task/Delete", ex);
            }
            return Content(result.ToString(), "application/json");
        }
        /// <summary>
        /// 获得任务日志(返回结束时间最近的一条数据)
        /// </summary>
        /// <param name="task_id"></param>
        /// <returns></returns>
        public ActionResult GetLog(string task_id)
        {
            Result result = null;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
                {
                    string sql = $" select * from(select * from etl_task_log where task_id = '{task_id}' order by end_time DESC) where rownum = 1";//获取任务结束时间最近的一条数据
                    var list = helper.ExecuteReader_ToList(sql);
                    result = Result.CreateSuccess("成功", list);
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("Task/GetLog", ex);
            }
            return Content(result.ToString(), "application/json");
        }
        /// <summary>
        /// 获取待执行任务列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTodoList()
        {
            Result result = null;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
                {
                    string sql = "";
                    sql = $"select * from ETL_TODO_TASK where FINISHED_STATE='0'";
                    var list = helper.ExecuteReader_ToList(sql);
                    result = Result.CreateSuccess("成功", list);
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("Task/GetTodoList", ex);
            }
            return Content(result.ToString(), "application/json");
        }




        /// <summary>
        /// 根据ID从TODOLIST 表中获取数据 主要用于获取该任务的状态信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTodoTaskID(string todoid)
        {
            Result result = null;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
                {
                    string sql = "";
                    sql = $"select * from ETL_TODO_TASK where ID ='{todoid}'";
                    var list = helper.ExecuteReader_ToList(sql);
                    result = Result.CreateSuccess("成功", list);
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("Task/GetTodoTaskID", ex);
            }
            return Content(result.ToString(), "application/json");
        }
        /// <summary>
        /// 插入数据至待执行任务表
        /// </summary>
        /// <param name="task_id"></param>
        /// <returns></returns>
        public ActionResult SaveTodo(string todotaskJSON)
        {
            Result result = null;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
                {
                    JObject todotaskObj = JObject.Parse(todotaskJSON);
                    string ID = todotaskObj["ID"].ToString();
                    string TASK_ID = todotaskObj["TASK_ID"].ToString();
                    string ADD_TIME = todotaskObj["ADD_TIME"].ToString();
                    string TASK_NAME = todotaskObj["TASK_NAME"].ToString();
                    string FINISHED_STATE = todotaskObj["FINISHED_STATE"].ToString();
                    //插入前先判断是否有统一任务且未执行完毕的情况
                    string judgesql = $"SELECT *  FROM ETL_TODO_TASK where  TASK_ID = '{TASK_ID}' and FINISHED_STATE='0'";
                    var judgelist = helper.ExecuteReader_ToList(judgesql);
                    if (judgelist.Count > 0)
                    {
                        result = Result.CreateDefeat("同一任务正在执行中");
                    }
                    else {
                        //插入数据库
                        string sql = $"insert into etl_todo_task(ID,TASK_ID,ADD_TIME,FINISHED_STATE,TASK_NAME) values('{ID}','{TASK_ID}',to_date('{ADD_TIME}','yyyy/mm/dd hh24:mi:ss'),'{FINISHED_STATE}','{TASK_NAME}')";
                        var list = helper.ExecuteReader_ToList(sql);
                        result = Result.CreateSuccess("成功", list);
                    }
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("Task/SaveTodo", ex);
            }
            return Content(result.ToString(), "application/json");
        }
        /// <summary>
        /// 获取SQL（删除任务及相关的表）
        /// </summary>
        /// <param name="task_id">任务id</param>
        /// <param name="deleteLog">是否删除日志</param>
        /// <returns></returns>
        List<string> GetDeleteSQL(string task_id, bool deleteLog = false)
        {
            List<string> sqls = new List<string>();
            // 删除任务日志
            if (deleteLog)
            {
                sqls.Add($"delete from etl_task_log where task_id='{task_id}'");
            }
            // 删除任务参数
            sqls.Add($"delete from etl_task_param where task_id='{task_id}'");
            // 删除任务
            sqls.Add($"delete from etl_task where id='{task_id}'");
            //删除待执行任务
            sqls.Add($"delete from etl_todo_task where task_id='{task_id}'");
            return sqls;
        }
        /// <summary>
        /// 获取SQL（插入任务）
        /// </summary>
        /// <param name="taskObj"></param>
        /// <returns></returns>
        string GetInsertSQL(JObject taskObj)
        {
            string ID = taskObj["ID"].ToString();
            string MODEL_ID = taskObj["MODEL_ID"].ToString();
            string NAME = taskObj["NAME"].ToString();
            string STATE = taskObj["STATE"].ToString();
            string REPEAT_MODE = taskObj["REPEAT_MODE"].ToString();
            string TIME_INTERVAL = taskObj["TIME_INTERVAL"].ToString();
            string TIME_REGULAR = taskObj["TIME_REGULAR"].ToString();
            return $"insert into etl_task(ID,MODEL_ID,NAME,STATE,REPEAT_MODE,TIME_INTERVAL,TIME_REGULAR) values('{ID}','{MODEL_ID}','{NAME}','{STATE}','{REPEAT_MODE}','{TIME_INTERVAL}','{TIME_REGULAR}')";
        }
        /// <summary>
        /// 获取SQL（更新或插入任务参数）
        /// </summary>
        /// <param name="paramArr">参数数组</param>
        /// <returns></returns>
        List<string> GetParamSQL(JArray paramArr)
        {
            List<string> sqls = new List<string>();
            foreach (JObject paramObj in paramArr)
            {
                string ID = paramObj["ID"].ToString();
                string STEP_PARAM_ID = paramObj["STEP_PARAM_ID"].ToString();
                string TASK_ID = paramObj["TASK_ID"].ToString();
                string PARAM_VALUE = paramObj["PARAM_VALUE"].ToString();
                sqls.Add($"insert into etl_task_param(ID,STEP_PARAM_ID,TASK_ID,PARAM_VALUE) values('{ID}','{STEP_PARAM_ID}','{TASK_ID}','{PARAM_VALUE}')");
            }
            return sqls;
        }
        /// <summary>
        /// 获取任务日志视图数据（含任务名）
        /// </summary>
        /// <returns></returns>
        public ActionResult GetLogList()
        {
            Result result = null;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
                {
                    string sql = "select * from v_etl_task_log";
                    var list = helper.ExecuteReader_ToList(sql);
                    result = Result.CreateSuccess("成功", list);
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("Task/GetLogList", ex);
            }
            return Content(result.ToString(), "application/json");
        }
    }
}