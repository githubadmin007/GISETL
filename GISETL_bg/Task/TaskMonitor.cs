using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using ZJH.BaseTools.BasicExtend;
using ZJH.BaseTools.DB;
using ZJH.BaseTools.IO;

namespace GISETL_bg.Task
{
    /// <summary>
    /// 任务监视器。定时检测任务列表，查找需要执行的任务
    /// </summary>
    public class TaskMonitor
    {
        /// <summary>
        /// 定时器
        /// </summary>
        System.Timers.Timer timer;
        /// <summary>
        /// 是否处于忙碌状态（上一次定时器事件是否处理完成）
        /// </summary>
        bool IsBusy = false;
        /// <summary>
        /// 构造函数。初始化定时器，但并不启动
        /// </summary>
        /// <param name="interval">检测间隔（毫秒）</param>
        public TaskMonitor(double interval = 1000)//20200915 60000 to 1000
        {
            // 创建定时器
            timer = new System.Timers.Timer(interval);
            timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
        }
        /// <summary>
        /// 启动监视器
        /// </summary>
        public void Start() {
            timer.Start();
        }
        /// <summary>
        /// 关闭监视器
        /// </summary>
        public void Stop()
        {
            timer.Stop();
        }
        /// <summary>
        /// 立刻进行一次检查
        /// </summary>
        public void CheckImmediately() {
            Timer_Elapsed(null, null);
        }
        /// <summary>
        /// 响应定时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Start();
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
                {

                    //20200915
                    var todotasklist = helper.ExecuteReader_ToList($"select * from etl_todo_task where FINISHED_STATE='0'");
                    foreach(var tttask in todotasklist)
                    {
                        string task_id = tttask["TASK_ID"].ToString();
                        TaskProcess process = new TaskProcess(task_id);
                        if (process.Execute())
                        {
                            string sql = $"update etl_todo_task set FINISHED_STATE='1' where TASK_ID='{task_id}'";
                            helper.ExecuteNonQuery(sql);
                        }
                        else
                        {
                            string sql = $"update etl_todo_task set FINISHED_STATE='2' where TASK_ID='{task_id}'";
                            helper.ExecuteNonQuery(sql);
                        }
                    }


                    var taskLst = helper.ExecuteReader_ToList($"select * from v_etl_task where state='1'");
                    foreach (var task in taskLst)
                    {
                        string REPEAT_MODE = task["REPEAT_MODE"].ToString();
                        if (
                            REPEAT_MODE == "1" && CheckNeedExecute_1(task)
                            || REPEAT_MODE == "2" && CheckNeedExecute_2(task)
                            || REPEAT_MODE == "3" && CheckNeedExecute_3(task)
                        )
                        {
                            Execute(task);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log("TaskMonitor/Timer_Elapsed", ex);
            }
            finally {
                IsBusy = false;
            }
        }


        string GetFormatStr(string perStr) {
            switch (perStr) {
                case "年":
                    return "MM月dd日HH时mm分";
                case "月":
                    return "dd日HH时mm分";
                case "日":
                    return "HH时mm分";
                case "时":
                    return "mm分";
                case "周":
                    return "dddHH时mm分";
            }
            return "每年08月24日16时55分";
        }
        /// <summary>
        /// 判断是否需要执行任务（按执行时间）
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        bool CheckNeedExecute_1(Dictionary<string,object> dict)
        {
            // 年月日时周
            string regular = dict["TIME_REGULAR"].ToString();
            if (regular.IsNullOrWhiteSpace() || regular.Length < 2) return false;
            string perStr = regular.Substring(1, 1);
            string timeStr = perStr == "周" ? regular.Substring(1) : regular.Substring(2);
            string formatStr = GetFormatStr(perStr);
            string nowTimeStr = DateTime.Now.ToString(formatStr);
            return timeStr == nowTimeStr;
        }
        /// <summary>
        /// 判断是否需要执行任务（按时间间隔）
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        bool CheckNeedExecute_2(Dictionary<string, object> dict)
        {
            int interval = dict["TIME_INTERVAL"].ToInt32();
            object obj = dict["LAST_TIME"];
            if (obj == DBNull.Value) return true;
            DateTime lastTime = (DateTime)obj;
            TimeSpan span = DateTime.Now - lastTime;
            return interval > 0 && span.TotalMinutes > interval;
        }
        /// <summary>
        /// 判断是否需要执行任务（只执行一次）
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        bool CheckNeedExecute_3(Dictionary<string, object> dict)
        {
            object obj = dict["LAST_TIME"];
            return obj == DBNull.Value; // 最近执行时间为空，代表任务未被执行过，需要执行
        }
        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="dict"></param>
        void Execute(Dictionary<string, object> task)
        {
            string task_id = task["ID"].ToString();
            // todo: 此处创建线程代码直接复制于网上，可能需要优化。比如进行线程管理，限制最大线程数等等
            Thread thread = new Thread(new ParameterizedThreadStart(delegate
            {
                try
                {
                    TaskProcess process = new TaskProcess(task_id);
                    process.Execute();

                }
                catch (Exception ex)
                {
                    Logger.log("TaskMonitor.Execute", ex);
                }
            }));//创建线程
            thread.Start();//启动线程
        }

    }
}
