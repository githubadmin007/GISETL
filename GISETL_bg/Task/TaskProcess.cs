using GISETL_bg.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ZJH.BaseTools.DB;

namespace GISETL_bg.Task
{
    /// <summary>
    /// 任务“进程”。完成任务相关的一系列步骤并输出任务日志
    /// </summary>
    public class TaskProcess
    {
        /// <summary>
        /// 日志id
        /// </summary>
        string log_id;
        /// <summary>
        /// 任务id
        /// </summary>
        string task_id;
        /// <summary>
        /// 模型id
        /// </summary>
        string model_id;
        /// <summary>
        /// 步骤总数
        /// </summary>
        int StepNum = 0;
        /// <summary>
        /// 步骤列表
        /// </summary>
        List<Dictionary<string, object>> stepLst;
        /// <summary>
        /// 步骤关联关系列表
        /// </summary>
        List<Dictionary<string, object>> relationLst;
        /// <summary>
        /// 步骤分段列表
        /// </summary>
        List<StepSegment> segmentList = new List<StepSegment>();
        /// <summary>
        /// 数据库帮助类
        /// </summary>
        DatabaseHelper helper = null;
        /// <summary>
        /// 构造函数。读取模型、步骤信息
        /// </summary>
        /// <param name="task_id"></param>
        public TaskProcess(string task_id) {
            log_id = Guid.NewGuid().ToString();
            this.task_id = task_id;
            helper = DatabaseHelper.CreateByConnName("GISETL");
            model_id = helper.ExecuteScalarToString($"select model_id from etl_task where id='{task_id}'");
            relationLst = helper.ExecuteReader_ToList($"select * from v_etl_step_relation where model_id='{model_id}'");
            stepLst = helper.ExecuteReader_ToList($"select * from v_etl_step where model_id='{model_id}'");
            StepNum = stepLst.Count;
            // 计算每个步骤的输入输出数
            CountIO();
            // 将步骤分段
            StepToSegment();
            // 将线段分级
            List<StepSegment> NoLevelLst = segmentList.Where(segment => segment.Level == int.MinValue).ToList();
            while (NoLevelLst.Count > 0) {
                NoLevelLst[0].Level = StepSegment.InitLevel;
                ClassifySegment(NoLevelLst[0]);
                NoLevelLst = segmentList.Where(segment => segment.Level == int.MinValue).ToList();
            }
            // 按分级排序
            segmentList = segmentList.OrderBy(segment => segment.Level).ToList();
        }

        /// <summary>
        /// 计算每个步骤的输入输出数
        /// </summary>
        void CountIO() {
            for (int i = stepLst.Count - 1; i >= 0; i--)
            {
                var step = stepLst[i];
                var inputs = relationLst.Where(rel => Equals(rel["AFTER_STEP_ID"], step["ID"])).Select(rel => rel["INPUT_NAME"].ToString()).ToList();
                int out_num = relationLst.Count(rel => Equals(rel["FRONT_STEP_ID"], step["ID"]));
                step.Add("IN_NUM_NOW", inputs.Count); // 每分割出一个线段，该段落的下一节点输入数实时减一
                step.Add("IN_NUM", inputs.Count);
                step.Add("OUT_NUM", out_num);
                step.Add("INPUTS", inputs);// 输入类型
            }
        }
        /// <summary>
        /// 将步骤分段
        /// </summary>
        void StepToSegment() {
            while (stepLst.Count > 0)
            {
                // 查找一个输入数为0的步骤,作为线段的开头
                var startStep = stepLst.Find(step => (int)step["IN_NUM_NOW"] == 0);
                // 创建一个线段
                StepSegment segment = new StepSegment(this); 
                AddStepToSegment(startStep, segment);
                while (!segment.IsEnd)
                {
                    // 因为segment未结尾,所以有且只有一个relation
                    var relation = relationLst.Find(rel => Equals(rel["FRONT_STEP_ID"], segment.LastStepId));
                    // 下一步骤
                    var nextStep = stepLst.Find(step => Equals(step["ID"], relation["AFTER_STEP_ID"]));
                    // 存在下一步骤，且下一步骤输入数为1
                    if (nextStep != null && (int)nextStep["IN_NUM"] == 1) 
                    {
                        AddStepToSegment(nextStep, segment);
                        relationLst.Remove(relation);
                    }
                    else {
                        break;
                    }
                }
                segmentList.Add(segment);
            }
        }
        /// <summary>
        /// 将线段分级
        /// </summary>
        void ClassifySegment(StepSegment segment) {
            // 查找所有前置线段
            var frontSegments = relationLst.Where(rel => Equals(rel["AFTER_STEP_ID"], segment.FirstStepId))
                .Select(rel => segmentList.Find(seg => Equals(rel["FRONT_STEP_ID"], seg.LastStepId)));
            foreach (var frontSegment in frontSegments) {
                // 评级以【与初始评级之差的绝对值】大的为准
                if (frontSegment.Level == int.MinValue || Math.Abs(segment.Level - 1) > Math.Abs(frontSegment.Level - StepSegment.InitLevel)) {
                    frontSegment.Level = segment.Level - 1;
                    ClassifySegment(frontSegment);
                }
            }
            // 查找所有后置线段
            var afterSegments = relationLst.Where(rel => Equals(rel["FRONT_STEP_ID"], segment.LastStepId))
                .Select(rel => segmentList.Find(seg => Equals(rel["AFTER_STEP_ID"], seg.FirstStepId)));
            foreach (var afterSegment in afterSegments)
            {
                // 评级以【与初始评级之差的绝对值】大的为准
                if (afterSegment.Level == int.MinValue || Math.Abs(segment.Level + 1) > Math.Abs(afterSegment.Level - StepSegment.InitLevel))
                {
                    afterSegment.Level = segment.Level + 1;
                    ClassifySegment(afterSegment);
                }
            }
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~TaskProcess() {
            if (helper != null) {
                helper.Dispose();
            }
        }
        /// <summary>
        /// 将步骤添加到线段中，并更新下一步骤的输入数
        /// </summary>
        /// <param name="moveStep">要添加的步骤</param>
        /// <param name="segment"></param>
        void AddStepToSegment(Dictionary<string, object> moveStep, StepSegment segment)
        {
            // 查找关联
            var relations = relationLst.Where(rel => Equals(rel["FRONT_STEP_ID"], moveStep["ID"]));
            // 查找下一步骤，并将输入数减一
            foreach (var relation in relations)
            {
                var nextStep = stepLst.Find(step => Equals(step["ID"], relation["AFTER_STEP_ID"])); // 下一步骤
                nextStep["IN_NUM_NOW"] = (int)nextStep["IN_NUM_NOW"] - 1;
            }
            // 移动步骤
            segment.Add(moveStep);
            stepLst.Remove(moveStep);
        }
        /// <summary>
        /// 按顺序执行步骤
        /// </summary>
        public bool Execute() {
            object lastOutput = null;
            try
            {
                CreateLog();
                foreach (StepSegment segment in segmentList)
                {
                    var inputDict = new Dictionary<string, object>();
                    // 查找所有输入
                    var frontRelations = relationLst.Where(rel => Equals(rel["AFTER_STEP_ID"], segment.FirstStepId));
                    foreach (var relation in frontRelations) {
                        StepSegment frontSegment = segmentList.Find(seg => Equals(relation["FRONT_STEP_ID"], seg.LastStepId));
                        string inputName = relation["INPUT_NAME"].ToString();
                        inputDict.Add(inputName, frontSegment.Output);
                    }
                    segment.Input = inputDict;
                    segment.Execute();
                    lastOutput = segment.Output;
                }
                FinishLog(1, "成功");
                return true;
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                FinishLog(-1, ex.Message);
                return false;
            }

        }

        /// <summary>
        /// 步骤进度
        /// </summary>
        int progress = 0;
        /// <summary>
        /// 创建日志
        /// </summary>
        /// <returns></returns>
        void CreateLog()
        {
            progress = 0;
            string sql = $"insert into etl_task_log(id,task_id,start_time,progress,result) values('{log_id}','{task_id}',sysdate,0,0)";
            helper.ExecuteNonQuery(sql);
        }
        /// <summary>
        /// 更新进度
        /// </summary>
        /// <param name="id"></param>
        /// <param name="progress"></param>
        void UpdateProgress()
        {
            progress++;
            string sql = $"update etl_task_log set progress={progress * 100 / StepNum} where id='{log_id}'";
            helper.ExecuteNonQuery(sql);
        }
        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="id"></param>
        /// <param name="result"></param>
        void FinishLog(int result, string result_text)
        {
            string sql = $"update etl_task_log set end_time=sysdate,result={result},result_text='{result_text}' where id='{log_id}'";
            helper.ExecuteNonQuery(sql);
        }

        class StepSegment
        {
            public static int InitLevel = 0; // 初始级别
            TaskProcess task;
            List<Dictionary<string, object>> stepLst = new List<Dictionary<string, object>>();
            /// <summary>
            /// 第一个步骤
            /// </summary>
            Dictionary<string, object> FirstStep
            {
                get
                {
                    return stepLst.Count > 0 ? stepLst[0] : null;
                }
            }
            /// <summary>
            /// 最后一个步骤
            /// </summary>
            Dictionary<string, object> LastStep
            {
                get
                {
                    return stepLst.Count > 0 ? stepLst[stepLst.Count - 1] : null;
                }
            }

            public int Level = int.MinValue;
            public Dictionary<string, object> Input { get; set; }
            public object Output { get; set; }
            /// <summary>
            /// 首个步骤的id
            /// </summary>
            public string FirstStepId
            {
                get
                {
                    return FirstStep == null ? "" : FirstStep["ID"].ToString();
                }
            }
            /// <summary>
            /// 最后一个步骤的id
            /// </summary>
            public string LastStepId
            {
                get
                {
                    return LastStep == null ? "" : LastStep["ID"].ToString();
                }
            }
            /// <summary>
            /// 是否已经结尾（最后一个步骤的OUT_NUM为0或大于1）
            /// 为true必定结尾，为false不一定未结尾（比如下一节点有一个以上的输入，也应该结尾只是在本类中无法检测）
            /// </summary>
            public bool IsEnd
            {
                get
                {
                    if (LastStep != null)
                    {
                        int out_num = (int)LastStep["OUT_NUM"];
                        return out_num == 0 || out_num > 1;
                    }
                    return false;
                }
            }
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="task"></param>
            public StepSegment(TaskProcess task) {
                this.task = task;
            }
            /// <summary>
            /// 增加新步骤
            /// </summary>
            /// <param name="step"></param>
            /// <returns></returns>
            public void Add(Dictionary<string, object> step)
            {
                stepLst.Add(step);
            }
            /// <summary>
            /// 按顺序执行步骤
            /// </summary>
            public void Execute()
            {
                object lastOutput = null;
                foreach (var step in stepLst)
                {
                    string step_id = step["ID"].ToString();
                    string stepName = step["NAME"].ToString();
                    string className = step["CLASS_NAME"].ToString();
                    Type type = Type.GetType($"GISETL_bg.Node.{className}");
                    if (type == null)
                    {
                        throw new Exception($"未找到类：“{className}”");
                    }
                    _BaseNode node = (_BaseNode)Activator.CreateInstance(type, new object[] { task.task_id, task.model_id, step_id });
                    // 获取输入参数（线段中的第一个步骤是使用线段外传入的Input，其他步骤使用上一步骤的Output）
                    if (Input == null)
                    {
                        node.Input = getInputParam(step, lastOutput);
                    }
                    else {
                        node.Input = Input;
                        Input = null;
                    }
                    // 执行节点操作
                    if (!node.Exexute())
                    {
                        throw new Exception($"步骤【{stepName}】执行失败");
                    }
                    lastOutput = node.Output;
                    task.UpdateProgress();
                }
                Output = lastOutput;
            }

            /// <summary>
            /// 获取输入参数
            /// </summary>
            /// <param name="step"></param>
            /// <param name="input"></param>
            /// <returns></returns>
            Dictionary<string, object> getInputParam(Dictionary<string,object> step,object input) {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                List<string> INPUTS = step["INPUTS"] as List<string>;
                if (INPUTS.Count > 0 && input != null) {
                    // 线段中的步骤除第一个以外输入数都为一
                    dict.Add(INPUTS[0], input);
                }
                return dict;
            }
        } // StepSegment
    }
}
