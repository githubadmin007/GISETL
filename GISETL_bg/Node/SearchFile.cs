using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GISETL_bg.Node
{
    public class SearchFile: _BaseNode
    {
        /// <summary>
        /// 文件夹
        /// </summary>
        string folder
        {
            get
            {
                return GetParam("folder");
            }
        }
        /// <summary>
        /// 通配符
        /// </summary>
        string wildcard {
            get {
                return GetParam("wildcard");
            }
        }
        /// <summary>
        /// 是否深度搜索
        /// </summary>
        bool deep {
            get {
                return GetParam("deep") == "是";
            }
        }

        public SearchFile(string task_id, string model_id, string step_id) : base(task_id, model_id, step_id){ }

        public override bool Exexute()
        {
            string searchFolder = $"{GisDataFolder}/{folder}";
            Output = Directory.GetFiles(searchFolder, wildcard, SearchOption.AllDirectories);
            return Output != null;
        }
    }
}
