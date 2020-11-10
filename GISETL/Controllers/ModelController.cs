using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZJH.BaseTools.BasicExtend;
using ZJH.BaseTools.DB;
using ZJH.BaseTools.IO;
using ZJH.BaseTools.Text;

namespace GISETL.Controllers
{
    public class ModelController : Controller
    {
        /*
        * 关于步骤参数
        */

        /// <summary>
        /// 获取模型列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetList()
        {
            Result result = null;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
                {
                    string sql = "select * from etl_model";
                    var list = helper.ExecuteReader_ToList(sql);
                    result = Result.CreateSuccess("成功", list);
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("Model/GetList", ex);
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
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
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
                Logger.log("Model/GetStepList", ex);
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
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
                {
                    string sql = $"select * from etl_step_relation where model_id='{model_id}'";
                    var list = helper.ExecuteReader_ToList(sql);
                    result = Result.CreateSuccess("成功", list);
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("Model/GetRelationList", ex);
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
        public ActionResult Save(string modelJSON, string stepsJSON, string relationsJSON)
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
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
                {
                    result = helper.ExecuteSqlTran(sqls) ? Result.Success : Result.CreateDefeat("保存失败");
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("Model/Save", ex);
            }
            return Content(result.ToString(), "application/json");
        }
        /// <summary>
        /// 删除模型
        /// </summary>
        /// <param name="model_id"></param>
        /// <returns></returns>
        public ActionResult Delete(string model_id)
        {
            Result result = null;
            try
            {
                List<string> sqls = GetDeleteModelSQL(model_id);
                // 以数据库事务执行SQL
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
                {
                    result = helper.ExecuteSqlTran(sqls) ? Result.Success : Result.Defeat;
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("Model/Delete", ex);
            }
            return Content(result.ToString(), "application/json");
        }
        /// <summary>
        /// 复制模型
        /// </summary>
        /// <param name="model_id">模型id</param>
        /// <returns></returns>
        public ActionResult Copy(string model_id, string name) {
            Result result = null;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
                {
                    string new_model_id = Guid.NewGuid().ToString();
                    List<DataTable> tables = new List<DataTable>();
                    // 复制模型
                    DataTable modelTable = helper.GetDataTable($"select * from etl_model where id='{model_id}'", "etl_model");
                    UpdateDataTable(modelTable, "ID", new_model_id);
                    UpdateDataTable(modelTable, "NAME", name);
                    tables.Add(modelTable);
                    // 复制连接
                    DataTable relationTable = helper.GetDataTable($"select * from etl_step_relation where model_id='{model_id}'", "etl_step_relation");
                    UpdateDataTable(relationTable, "MODEL_ID", new_model_id);
                    DataTableCreateID(relationTable);
                    tables.Add(relationTable);
                    // 复制步骤
                    DataTable stepTable = helper.GetDataTable($"select * from etl_step where model_id='{model_id}'", "etl_step");
                    UpdateDataTable(stepTable, "MODEL_ID", new_model_id);
                    tables.Add(stepTable);
                    foreach (DataRow stepRow in stepTable.Rows) {
                        string step_id = stepRow["ID"].ToString();
                        string new_step_id = Guid.NewGuid().ToString();
                        stepRow["ID"] = new_step_id;
                        // 修改连接中的step_id
                        foreach (DataRow relRow in relationTable.Rows) {
                            if (relRow["FRONT_STEP_ID"].ToString() == step_id) {
                                relRow["FRONT_STEP_ID"] = new_step_id;
                            }
                            if (relRow["AFTER_STEP_ID"].ToString() == step_id)
                            {
                                relRow["AFTER_STEP_ID"] = new_step_id;
                            }
                        }
                        // 复制步骤参数
                        DataTable paramTable = helper.GetDataTable($"select * from etl_step_param where step_id='{step_id}'", "etl_step_param");
                        UpdateDataTable(paramTable, "STEP_ID", new_step_id);
                        DataTableCreateID(paramTable);
                        tables.Add(paramTable);
                        // 复制etl_step_param_array。。。。暂不处理
                    }
                    helper.InsertDataSet(tables);
                    result = Result.CreateSuccess("成功");
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("Model/Copy", ex);
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
                string NODE_INPUT_ID = relationObj["NODE_INPUT_ID"].ToString();
                sqls.Add($"insert into etl_step_relation(ID,MODEL_ID,FRONT_STEP_ID,AFTER_STEP_ID,NODE_INPUT_ID) values('{ID}','{MODEL_ID}','{FRONT_STEP_ID}','{AFTER_STEP_ID}','{NODE_INPUT_ID}')");
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
            using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
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


        /// <summary>
        /// 创建新的id
        /// </summary>
        /// <param name="table"></param>
        /// <param name="IdColumnName"></param>
        void DataTableCreateID(DataTable table,string IdColumnName = "ID") {
            if (table == null || table.Columns.IndexOf(IdColumnName) < 0)
            {
                return;
            }
            foreach (DataRow row in table.Rows)
            {
                row[IdColumnName] = Guid.NewGuid().ToString();
            }
        }
        /// <summary>
        /// 设置整列的值
        /// </summary>
        /// <param name="table"></param>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        void UpdateDataTable(DataTable table, string columnName, object value) {
            if (table == null || table.Columns.IndexOf(columnName) < 0) {
                return;
            }
            foreach (DataRow row in table.Rows) {
                row[columnName] = value;
            }
        }
    }
}