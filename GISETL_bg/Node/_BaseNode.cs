using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZJH.BaseTools;
using ZJH.BaseTools.DB;

namespace GISETL_bg.Node
{
    public abstract class _BaseNode
    {
        /// <summary>
        /// 数据基础路径
        /// </summary>
        protected static string GisDataFolder = GlobalConfig.AppCfg.getValueByPath("configuration/Custom/GisDataFolder");
        /// <summary>
        /// 临时文件夹
        /// </summary>
        protected static string TempDataFolder = GlobalConfig.AppCfg.getValueByPath("configuration/Custom/TempDataFolder");
        /// <summary>
        /// 临时GDB
        /// </summary>
        protected static string TempGDB = GlobalConfig.AppCfg.getValueByPath("configuration/Custom/TempGDB");

        /// <summary>
        /// 输入数据
        /// </summary>
        public Dictionary<string, object> Input { get; set; }
        /// <summary>
        /// 输出数据
        /// </summary>
        public object Output { get; protected set; }
        /// <summary>
        /// 节点参数
        /// </summary>
        protected Dictionary<string, string> ParamsDict = new Dictionary<string, string>();
        /// <summary>
        /// 任务id
        /// </summary>
        string task_id;
        /// <summary>
        /// 模型id
        /// </summary>
        string model_id;
        /// <summary>
        /// 步骤id
        /// </summary>
        string step_id;
        /// <summary>
        /// 节点id
        /// </summary>
        string node_id;
        public _BaseNode() { }
        /// <summary>
        /// 构造函数(通过3个id获取参数)
        /// </summary>
        /// <param name="task_id">任务id</param>
        /// <param name="model_id">模型id</param>
        /// <param name="step_id">步骤id</param>
        public _BaseNode(string task_id, string model_id, string step_id)
        {
            this.task_id = task_id;
            this.model_id = model_id;
            this.step_id = step_id;
            using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
            {
                node_id = helper.ExecuteScalarToString($"select node_id from etl_step where id='{step_id}'");
                // 获取节点参数列表
                var paramLst = helper.ExecuteReader_ToList($"select param_name,final_value from v_etl_param_value where task_id = '{task_id}' and step_id = '{step_id}'");
                foreach (var param in paramLst)
                {
                    ParamsDict.Add(param["PARAM_NAME"].ToString(), param["FINAL_VALUE"].ToString());
                }
            }
            Output = null;
        }
        /// <summary>
        /// 执行函数
        /// </summary>
        /// <returns></returns>
        public abstract bool Exexute();

        /// <summary>
        /// 获取输入的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input_name"></param>
        /// <returns></returns>
        protected T GetInput<T>(string input_name)
        {
            if (Input != null)
            {
                return (T)Input[input_name];
            }
            return default(T);
        }
        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <param name="param_name"></param>
        /// <returns></returns>
        protected string GetParam(string param_name) {
            return ParamsDict[param_name];
        }
    }
}
