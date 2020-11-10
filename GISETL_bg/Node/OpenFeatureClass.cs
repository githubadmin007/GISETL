using GISETL_bg.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GISETL_bg.Node
{
    /// <summary>
    /// 打开GIS图层
    /// </summary>
    public class OpenFeatureClass : _BaseNode
    {
        /// <summary>
        /// 数据源id
        /// </summary>
        string dataSourceId {
            get {
                return GetParam("data_source_id");
            }
        }
        /// <summary>
        /// 图层名
        /// </summary>
        string layerName {
            get
            {
                return GetParam("layer_name");
            }
        }
        public OpenFeatureClass(string task_id, string model_id, string step_id) : base(task_id, model_id, step_id) { }
        public override bool Exexute()
        {
            Output = GisDataHelper.OpenFeatureClass(dataSourceId, layerName);
            return Output != null;
        }
    }
}
