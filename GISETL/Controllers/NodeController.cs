using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ZJH.BaseTools.BasicExtend;
using ZJH.BaseTools.DB;
using ZJH.BaseTools.IO;
using ZJH.BaseTools.Text;

namespace GISETL.Controllers
{
    public class NodeController : Controller
    {
        /// <summary>
        /// 获取节点列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetList()
        {
            Result result = null;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
                {
                    string sql = "select * from etl_node";
                    var list = helper.ExecuteReader_ToList(sql);
                    list.ForEach(delegate (Dictionary<string, object> dict) {
                        string node_id = dict["ID"].ToString();
                        sql = $"select * from etl_node_param where node_id='{node_id}'";
                        var paramLst = helper.ExecuteReader_ToList(sql);
                        dict.Add("PARAMS", paramLst);
                        sql = $"select * from etl_node_input where node_id='{node_id}'";
                        var inputLst = helper.ExecuteReader_ToList(sql);
                        dict.Add("INPUTS", inputLst);
                    });
                    result = Result.CreateSuccess("成功", list);
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("Node/GetList", ex);
            }
            return Content(result.ToString(), "application/json");
        }
        /// <summary>
        /// 保存节点
        /// </summary>
        /// <param name="nodeJSON"></param>
        /// <returns></returns>
        public ActionResult Save(string nodeJSON)
        {
            Result result = null;
            try
            {
                List<string> sqls = new List<string>();
                JObject nodeObj = JObject.Parse(nodeJSON);
                string ID = nodeObj["ID"].ToString();
                // 删除节点及相关的表记录
                sqls.AddRange(GetDeleteNodeSQL(ID));
                // 保存节点
                sqls.Add(GetNodeSQL(nodeObj));
                // 保存参数
                JArray paramArr = nodeObj["PARAMS"] as JArray;
                sqls.AddRange(GetParamSQL(paramArr, ID));
                // 保存输入
                JArray inputArr = nodeObj["INPUTS"] as JArray;
                sqls.AddRange(GetInputSQL(inputArr, ID));
                // 以数据库事务执行SQL
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
                {
                    result = helper.ExecuteSqlTran(sqls) ? Result.Success : Result.Defeat;
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("Node/Save", ex);
            }
            return Content(result.ToString(), "application/json");
        }
        /// <summary>
        ///  删除节点
        /// </summary>
        /// <param name="NodeId"></param>
        /// <returns></returns>
        public ActionResult Delete(string nodeId)
        {
            Result result = null;
            try
            {
                List<string> list = GetDeleteNodeSQL(nodeId);
                using (DatabaseHelper databaseHelper = DatabaseHelper.CreateByConnName("GISETL"))
                {
                    result = (databaseHelper.ExecuteSqlTran(list) ? Result.Success : Result.Defeat);
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("Node/Delete", ex);
            }
            return base.Content(result.ToString(), "application/json");
        }


        /// <summary>
        /// 获取SQL（删除节点及相关的表）
        /// </summary>
        /// <param name="node_id">节点id</param>
        /// <returns></returns>
        List<string> GetDeleteNodeSQL(string node_id)
        {
            List<string> sqls = new List<string>();
            // 删除参数
            sqls.Add($"delete from etl_node_param where node_id='{node_id}'");
            // 删除输入
            sqls.Add($"delete from etl_node_input where node_id='{node_id}'");
            // 删除节点
            sqls.Add($"delete from etl_node where id='{node_id}'");
            return sqls;
        }
        /// <summary>
        /// 获取SQL（保存节点）
        /// </summary>
        /// <param name="nodeObj"></param>
        /// <returns></returns>
        string GetNodeSQL(JObject nodeObj)
        {
            string ID = nodeObj["ID"].ToString();
            string NAME = nodeObj["NAME"].ToString();
            string CLASS_NAME = nodeObj["CLASS_NAME"].ToString();
            string OUTPUT_TYPE = nodeObj["OUTPUT_TYPE"].ToString();
            return $"insert into etl_node(ID,NAME,CLASS_NAME,OUTPUT_TYPE) values('{ID}','{NAME}','{CLASS_NAME}','{OUTPUT_TYPE}')";
        }
        /// <summary>
        /// 获取SQL（保存参数）
        /// </summary>
        /// <param name="paramArr"></param>
        /// <param name="node_id"></param>
        /// <returns></returns>
        List<string> GetParamSQL(JArray paramArr, string node_id)
        {
            List<string> sqls = new List<string>();
            foreach (JObject paramObj in paramArr)
            {
                string ID = paramObj["ID"].ToString();
                string NAME = paramObj["NAME"].ToString();
                string ALIAS = paramObj["ALIAS"].ToString();
                string REQUIRED = paramObj["REQUIRED"].ToString();
                sqls.Add($"insert into etl_node_param(ID,NODE_ID,NAME,ALIAS,REQUIRED) values('{ID}','{node_id}','{NAME}','{ALIAS}','{REQUIRED}')");
            }
            return sqls;
        }
        /// <summary>
        /// 获取SQL（保存输入）
        /// </summary>
        /// <param name="inputArr"></param>
        /// <param name="node_id"></param>
        /// <returns></returns>
        List<string> GetInputSQL(JArray inputArr, string node_id)
        {
            List<string> sqls = new List<string>();
            foreach (JObject inputObj in inputArr)
            {
                string ID = inputObj["ID"].ToString();
                string NAME = inputObj["NAME"].ToString();
                string ALIAS = inputObj["ALIAS"].ToString();
                string TYPE = inputObj["TYPE"].ToString();
                sqls.Add($"insert into etl_node_input(ID,NODE_ID,NAME,ALIAS,TYPE) values('{ID}','{node_id}','{NAME}','{ALIAS}','{TYPE}')");
            }
            return sqls;
        }











        /// <summary>
        /// 获取字段映射表
        /// </summary>
        /// <param name="group_id"></param>
        /// <returns></returns>
        public ActionResult GetFieldMappingsList(string group_id) {
            Result result = null;
            try
            {
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
                {
                    string sql = $"select * from node_field_mappings where group_id='{group_id}'";
                    var list = helper.ExecuteReader_ToList(sql);
                    result = Result.CreateSuccess("成功", list);
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("Node/GetFieldMappingsList", ex);
            }
            return Content(result.ToString(), "application/json");
        }
        /// <summary>
        /// 保存字段映射表
        /// </summary>
        /// <param name="mappingsJSON"></param>
        /// <returns></returns>
        public ActionResult SaveFieldMappingsList(string group_id, string mappingsJSON)
        {
            Result result = null;
            try
            {
                List<string> sqls = new List<string>();
                JArray mappingArr = JArray.Parse(mappingsJSON);
                sqls.Add($"delete from node_field_mappings where group_id='{group_id}'");
                foreach (JObject mappingObj in mappingArr) {
                    string ID = mappingObj["ID"].ToString("");
                    string SOURCE_FIELD = mappingObj["SOURCE_FIELD"].ToString("");
                    string TARGET_FIELD = mappingObj["TARGET_FIELD"].ToString("");
                    string TARGET_ALIAS = mappingObj["TARGET_ALIAS"].ToString("");
                    string TARGET_TYPE = mappingObj["TARGET_TYPE"].ToString("");
                    int TARGET_LENGTH = mappingObj["TARGET_LENGTH"].ToInt32();
                    sqls.Add($"insert into node_field_mappings(ID,GROUP_ID,SOURCE_FIELD,TARGET_FIELD,TARGET_ALIAS,TARGET_TYPE,TARGET_LENGTH) values('{ID}','{group_id}','{SOURCE_FIELD}','{TARGET_FIELD}','{TARGET_ALIAS}','{TARGET_TYPE}',{TARGET_LENGTH})");
                }
                // 以数据库事务执行SQL
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
                {
                    result = helper.ExecuteSqlTran(sqls) ? Result.Success : Result.Defeat;
                }
            }
            catch (Exception ex)
            {
                result = Result.CreateFromException(ex);
                Logger.log("Node/SaveFieldMappingsList", ex);
            }
            return Content(result.ToString(), "application/json");
        }

    }
}