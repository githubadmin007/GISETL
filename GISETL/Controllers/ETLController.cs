using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ZJH.BaseTools.BasicExtend;
using ZJH.BaseTools.DB;
using ZJH.BaseTools.IO;
using ZJH.BaseTools.Text;

namespace SLWETL.Controllers
{
    /// <summary>
    /// 因为三龙湾的工程有问题导致前端无法请求到数据，所以在这里放了一套接口
    /// GISETL的接口在其他Controller里面
    /// </summary>
    public class ETLController : Controller
    {
        #region 数据源相关
        //获取数据源列表
        public ActionResult GetDataSourceList()
        {
            Result result = null;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("SLWETL"))
                {
                    string sql = "select * from etl_data_source";
                    var list = helper.ExecuteReader_ToList(sql);
                    result = Result.CreateSuccess("成功", list);
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("ETL/GetDataSourceList", ex);
            }
            return Content(result.ToString(), "application/json");
        }
        //保存新建数据源
        public ActionResult SaveDataSource(string datatsourceJSON)
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
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("SLWETL"))
                {
                    result = helper.ExecuteSqlTran(sqls) ? Result.Success : Result.Defeat;
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("ETL/SaveDataSource", ex);
            }
            return Content(result.ToString(), "application/json");
        }
        //删除数据源
        public ActionResult DeleteDataSource(string datasoureid)
        {
            Result result = null;
            try
            {
                List<string> sqls = new List<string>();
                // 删除数据源
                sqls.Add($"delete from ETL_DATA_SOURCE where id='{datasoureid}'");

                // 以数据库事务执行SQL
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("SLWETL"))
                {
                    result = helper.ExecuteSqlTran(sqls) ? Result.Success : Result.Defeat;
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("ETL/DeleteDataSource", ex);
            }
            return Content(result.ToString(), "application/json");
        }
        #endregion


        #region 节点相关
        /// <summary>
        /// 获取节点列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetNodeList()
        {
            Result result = null;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("SLWETL"))
                {
                    string sql = "select * from etl_node";
                    var list = helper.ExecuteReader_ToList(sql);
                    list.ForEach(delegate (Dictionary<string, object> dict) {
                        string node_id = dict["ID"].ToString();
                        sql = $"select * from etl_node_param where node_id='{node_id}'";
                        var paramLst = helper.ExecuteReader_ToList(sql);
                        dict.Add("PARAMS", paramLst);
                    });
                    result = Result.CreateSuccess("成功", list);
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("ETL/GetNodeList", ex);
            }
            return Content(result.ToString(), "application/json");
        }
        //保存节点
        public ActionResult SaveNode(string NodeJSON, string ParamJSON)
        {
            Result result = null;
            try
            {
                List<string> sqls = new List<string>();
                JObject newnode = JObject.Parse(NodeJSON);
                string ID = newnode["ID"].ToString();
                string NAME = newnode["NAME"].ToString();
                string CLASS_NAME = newnode["CLASS_NAME"].ToString();
                string INPUT_TYPE = newnode["INPUT_TYPE"].ToString();
                string OUTPUT_TYPE = newnode["OUTPUT_TYPE"].ToString();
                sqls.Add(string.Format("delete from etl_node where id='{0}'", ID));
                sqls.Add(string.Format("delete from etl_node_param where node_id='{0}'", ID));
                sqls.Add(string.Format("insert into etl_node(ID,NAME,CLASS_NAME,INPUT_TYPE,OUTPUT_TYPE) values('{0}','{1}','{2}','{3}','{4}')", new object[]
                {
                    ID,
                    NAME,
                    CLASS_NAME,
                    INPUT_TYPE,
                    OUTPUT_TYPE
                }));
                JArray paramArr = JArray.Parse(ParamJSON);
                sqls.AddRange(this.GetNodeParamSQL(paramArr, ID));
                JArray newnodeparam = JArray.Parse(ParamJSON);
                // 以数据库事务执行SQL
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("SLWETL"))
                {
                    result = helper.ExecuteSqlTran(sqls) ? Result.Success : Result.Defeat;
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("ETL/SaveNode", ex);
            }
            return Content(result.ToString(), "application/json");
        }
        public ActionResult DeleteNode(string NodeId)
        {
            Result result = null;
            try
            {
                List<string> list = new List<string>();
                list.Add(string.Format("delete from etl_node where id='{0}'", NodeId));
                list.Add(string.Format("delete from etl_node_param where node_id='{0}'", NodeId));
                using (DatabaseHelper databaseHelper = DatabaseHelper.CreateByConnName("SLWETL"))
                {
                    result = (databaseHelper.ExecuteSqlTran(list) ? Result.Success : Result.Defeat);
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("ETL/DeleteNode", ex);
            }
            return base.Content(result.ToString(), "application/json");
        }
        List<string> GetNodeParamSQL(JArray paramArr, string node_id)
        {
            List<string> list = new List<string>();
            using (IEnumerator<JToken> enumerator = paramArr.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    JObject jObject = (JObject)enumerator.Current;
                    string ID = jObject["ID"].ToString();
                    string NAME = jObject["NAME"].ToString();
                    string ALIAS = jObject["ALIAS"].ToString();
                    string REQUIRED = jObject["REQUIRED"].ToString();
                    list.Add(string.Format("insert into etl_node_param(ID,NODE_ID,NAME,ALIAS,REQUIRED) values('{0}','{1}','{2}','{3}','{4}')", new object[]
                    {
                        ID,
                        node_id,
                        NAME,
                        ALIAS,
                        REQUIRED
                    }));
                }
            }
            return list;
        }
        #endregion


        #region 模型相关
        /// <summary>
        /// 获取模型列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetModelList()
        {
            Result result = null;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("SLWETL"))
                {
                    string sql = "select * from etl_model";
                    var list = helper.ExecuteReader_ToList(sql);
                    result = Result.CreateSuccess("成功", list);
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("ETL/GetModelList", ex);
            }
            return Content(result.ToString(), "application/json");
        }
        /// <summary>
        /// 获取步骤列表
        /// </summary>
        /// <param name="model_id">模型id</param>
        /// <returns></returns>
        public ActionResult GetStepList(string model_id)
        {
            Result result = null;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("SLWETL"))
                {
                    string sql = $"select * from etl_step where model_id='{model_id}'";
                    var list = helper.ExecuteReader_ToList(sql);
                    list.ForEach(delegate (Dictionary<string, object> dict) {
                        string step_id = dict["ID"].ToString();
                        sql = $"select * from etl_step_param where step_id='{step_id}'";
                        var paramLst = helper.ExecuteReader_ToList(sql);
                        dict.Add("PARAMS", paramLst);
                    });
                    result = Result.CreateSuccess("成功", list);
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("ETL/GetStepList", ex);
            }
            return Content(result.ToString(), "application/json");
        }
        /// <summary>
        /// 获取步骤关联关系列表
        /// </summary>
        /// <param name="model_id">模型id</param>
        /// <returns></returns>
        public ActionResult GetRelationList(string model_id)
        {
            Result result = null;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("SLWETL"))
                {
                    string sql = $"select * from etl_step_relation where model_id='{model_id}'";
                    var list = helper.ExecuteReader_ToList(sql);
                    result = Result.CreateSuccess("成功", list);
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("ETL/GetRelationList", ex);
            }
            return Content(result.ToString(), "application/json");
        }
        /// <summary>
        /// 保存模型
        /// </summary>
        /// <param name="modelJSON"></param>
        /// <param name="stepsJSON"></param>
        /// <param name="relationsJSON"></param>
        /// <returns></returns>
        public ActionResult SaveModel(string modelJSON, string stepsJSON, string relationsJSON)
        {
            Result result = null;
            try
            {
                List<string> sqls = new List<string>();
                JObject modelObj = JObject.Parse(modelJSON);
                string ID = modelObj["ID"].ToString();
                // 删除模型及相关的表记录
                sqls.AddRange(GetDeleteModelSQL(ID));
                // 插入模型
                sqls.Add(GetModelSQL(modelObj));
                // 插入步骤
                JArray stepArr = JArray.Parse(stepsJSON);
                sqls.AddRange(GetStepSQL(stepArr));
                // 插入步骤关系
                JArray relationArr = JArray.Parse(relationsJSON);
                sqls.AddRange(GetRelationSQL(relationArr));
                // 以数据库事务执行SQL
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("SLWETL"))
                {
                    result = helper.ExecuteSqlTran(sqls) ? Result.Success : Result.CreateDefeat("保存失败");
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("ETL/SaveModel", ex);
            }
            return Content(result.ToString(), "application/json");
        }
        /// <summary>
        /// 删除模型
        /// </summary>
        /// <param name="model_id"></param>
        /// <returns></returns>
        public ActionResult DeleteModel(string model_id)
        {
            Result result = null;
            try
            {
                List<string> sqls = GetDeleteModelSQL(model_id);
                // 以数据库事务执行SQL
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("SLWETL"))
                {
                    result = helper.ExecuteSqlTran(sqls) ? Result.Success : Result.Defeat;
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("ETL/DeleteModel", ex);
            }
            return Content(result.ToString(), "application/json");
        }

        /// <summary>
        /// 获取SQL（插入模型）
        /// </summary>
        /// <param name="modelObj"></param>
        /// <returns></returns>
        string GetModelSQL(JObject modelObj)
        {
            string ID = modelObj["ID"].ToString();
            string NAME = modelObj["NAME"].ToString();
            string TIME = modelObj["TIME"].ToString();
            return $"insert into etl_model(ID,NAME,TIME) values('{ID}','{NAME}',to_date('{TIME}','yyyy/mm/dd hh24:mi:ss'))";
        }
        /// <summary>
        /// 获取SQL（插入步骤）
        /// </summary>
        /// <param name="model_id"></param>
        /// <param name="stepArr"></param>
        /// <returns></returns>
        List<string> GetStepSQL(JArray stepArr)
        {
            List<string> sqls = new List<string>();
            foreach (JObject stepObj in stepArr)
            {
                string ID = stepObj["ID"].ToString();
                string NODE_ID = stepObj["NODE_ID"].ToString();
                string MODEL_ID = stepObj["MODEL_ID"].ToString();
                string NAME = stepObj["NAME"].ToString();
                int X = stepObj["X"].ToInt32();
                int Y = stepObj["Y"].ToInt32();
                int WIDTH = stepObj["WIDTH"].ToInt32();
                int HEIGHT = stepObj["HEIGHT"].ToInt32();
                sqls.Add($"insert into etl_step(ID,NODE_ID,MODEL_ID,NAME,X,Y,WIDTH,HEIGHT) values('{ID}','{NODE_ID}','{MODEL_ID}','{NAME}',{X},{Y},{WIDTH},{HEIGHT})");
                JArray paramArr = stepObj["PARAMS"] as JArray;
                sqls.AddRange(GetStepParamSQL(paramArr));
            }
            return sqls;
        }
        /// <summary>
        /// 获取SQL（更新或插入步骤参数）
        /// </summary>
        /// <param name="step_id">步骤id</param>
        /// <param name="paramArr">参数数组</param>
        /// <returns></returns>
        List<string> GetStepParamSQL(JArray paramArr)
        {
            List<string> sqls = new List<string>();
            foreach (JObject paramObj in paramArr)
            {
                string ID = paramObj["ID"].ToString();
                string NODE_PARAM_ID = paramObj["NODE_PARAM_ID"].ToString();
                string STEP_ID = paramObj["STEP_ID"].ToString();
                string PARAM_VALUE = paramObj["PARAM_VALUE"].ToString();
                string IS_DYNAMIC = paramObj["IS_DYNAMIC"].ToString();
                sqls.Add($"insert into etl_step_param(ID,NODE_PARAM_ID,STEP_ID,PARAM_VALUE,IS_DYNAMIC) values('{ID}','{NODE_PARAM_ID}','{STEP_ID}','{PARAM_VALUE}','{IS_DYNAMIC}')");
            }
            return sqls;
        }
        /// <summary>
        /// 获取SQL（更新或插入步骤关系）
        /// </summary>
        /// <param name="model_id"></param>
        /// <param name="relationArr"></param>
        /// <returns></returns>
        List<string> GetRelationSQL(JArray relationArr)
        {
            List<string> sqls = new List<string>();
            foreach (JObject relationObj in relationArr)
            {
                string ID = relationObj["ID"].ToString();
                string MODEL_ID = relationObj["MODEL_ID"].ToString();
                string FRONT_STEP_ID = relationObj["FRONT_STEP_ID"].ToString();
                string AFTER_STEP_ID = relationObj["AFTER_STEP_ID"].ToString();
                sqls.Add($"insert into etl_step_relation(ID,MODEL_ID,FRONT_STEP_ID,AFTER_STEP_ID) values('{ID}','{MODEL_ID}','{FRONT_STEP_ID}','{AFTER_STEP_ID}')");
            }
            return sqls;
        }
        /// <summary>
        /// 获取SQL（删除模型及相关的表）
        /// </summary>
        /// <param name="model_id"></param>
        /// <returns></returns>
        List<string> GetDeleteModelSQL(string model_id)
        {
            List<string> sqls = new List<string>();
            // 删除步骤关联关系
            sqls.Add($"delete from etl_step_relation where model_id='{model_id}'");
            // 删除步骤参数
            using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("SLWETL"))
            {
                var list = helper.ExecuteReader_ToList($"select ID from etl_step where model_id='{model_id}'");
                sqls.AddRange(list.Select(dict => $"delete from etl_step_param where step_id='{dict["ID"]}'"));
            }
            // 删除步骤
            sqls.Add($"delete from etl_step where model_id='{model_id}'");
            // 删除模型
            sqls.Add($"delete from etl_model where id='{model_id}'");
            return sqls;
        }
        #endregion


        #region 任务相关
        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTaskList()
        {
            Result result = null;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("SLWETL"))
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
                Logger.log("ETL/GetTaskList", ex);
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
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("SLWETL"))
                {
                    string sql = $"select * from v_etl_dynamic_param where model_id='{model_id}'";
                    var list = helper.ExecuteReader_ToList(sql);
                    result = Result.CreateSuccess("成功", list);
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("ETL/GetTaskList", ex);
            }
            return Content(result.ToString(), "application/json");
        }
        /// <summary>
        /// 保存任务
        /// </summary>
        /// <param name="taskJSON"></param>
        /// <returns></returns>
        public ActionResult SaveTask(string taskJSON)
        {
            Result result = null;
            try
            {
                List<string> sqls = new List<string>();
                JObject taskObj = JObject.Parse(taskJSON);
                string ID = taskObj["ID"].ToString();
                // 删除任务及相关的表记录
                sqls.AddRange(GetDeleteTaskSQL(ID));
                // 插入任务
                sqls.Add(GetTaskSQL(taskObj));
                // 插入任务参数
                JArray paramArr = taskObj["PARAMS"] as JArray;
                sqls.AddRange(GetTaskParamSQL(paramArr));
                // 以数据库事务执行SQL
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("SLWETL"))
                {
                    result = helper.ExecuteSqlTran(sqls) ? Result.Success : Result.CreateDefeat("保存失败");
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("ETL/SaveTask", ex);
            }
            return Content(result.ToString(), "application/json");
        }
        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="task_id"></param>
        /// <returns></returns>
        public ActionResult DeleteTask(string task_id)
        {
            Result result = null;
            try
            {
                List<string> sqls = GetDeleteTaskSQL(task_id, true);
                // 以数据库事务执行SQL
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("SLWETL"))
                {
                    result = helper.ExecuteSqlTran(sqls) ? Result.Success : Result.Defeat;
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("ETL/DeleteTask", ex);
            }
            return Content(result.ToString(), "application/json");
        }



        /// <summary>
        /// 获得任务日志(返回结束时间最近的一条数据)
        /// </summary>
        /// <param name="task_id"></param>
        /// <returns></returns>
        public ActionResult GetTaskLog(string task_id)
        {
            Result result = null;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("SLWETL"))
                {
                    string sql = $" select * from(select * from etl_task_log where task_id = '{task_id}' order by end_time DESC) where rownum = 1";//获取任务结束时间最近的一条数据
                    var list = helper.ExecuteReader_ToList(sql);
                    result = Result.CreateSuccess("成功", list);
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("ETL/GetTaskLog", ex);
            }
            return Content(result.ToString(), "application/json");
        }



        /// <summary>
        /// 获取待执行任务列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTodoTaskList()
        {
            Result result = null;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("SLWETL"))
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
                Logger.log("ETL/GetTodoTaskList", ex);
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
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("SLWETL"))
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
                Logger.log("ETL/GetTodoTaskID", ex);
            }
            return Content(result.ToString(), "application/json");
        }





        /// <summary>
        /// 插入数据至待执行任务表
        /// </summary>
        /// <param name="task_id"></param>
        /// <returns></returns>
        public ActionResult SaveTodoTask(string todotaskJSON)
        {

            Result result = null;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("SLWETL"))
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
                Logger.log("ETL/SaveTodoTask", ex);
            }
            return Content(result.ToString(), "application/json");

        }






        /// <summary>
        /// 获取SQL（删除任务及相关的表）
        /// </summary>
        /// <param name="task_id">任务id</param>
        /// <param name="deleteLog">是否删除日志</param>
        /// <returns></returns>
        List<string> GetDeleteTaskSQL(string task_id, bool deleteLog = false)
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
        string GetTaskSQL(JObject taskObj)
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
        List<string> GetTaskParamSQL(JArray paramArr)
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
        #endregion


        #region 任务日志相关
        /// <summary>
        /// 获取任务日志视图数据（含任务名）
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTaskLogList()
        {
            Result result = null;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("SLWETL"))
                {
                    string sql = "select * from v_etl_task_log";
                    var list = helper.ExecuteReader_ToList(sql);
                    result = Result.CreateSuccess("成功", list);
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("ETL/GetTaskLogList", ex);
            }
            return Content(result.ToString(), "application/json");
        }

        #endregion
    }
}