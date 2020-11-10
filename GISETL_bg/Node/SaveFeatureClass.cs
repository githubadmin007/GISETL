using ESRI.ArcGIS.Geodatabase;
using FSSG.EsriGIS.Extend;
using GISETL_bg.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GISETL_bg.Node
{
    public class SaveFeatureClass : _BaseNode
    {
        /// <summary>
        /// 数据源id
        /// </summary>
        string dataSourceId
        {
            get {
                return GetParam("data_source_id");
            }
        }
        /// <summary>
        /// 图层名
        /// </summary>
        string layerName
        {
            get
            {
                return GetParam("layer_name");
            }
        }
        /// <summary>
        /// 是否删除旧图层
        /// </summary>
        bool deleteOld
        {
            get
            {
                return GetParam("delete_old") == "是";
            }
        }
        /// <summary>
        /// 是否清空记录
        /// </summary>
        bool clearRecord
        {
            get
            {
                return GetParam("clear_record") == "是";
            }
        }
        /// <summary>
        /// 要保存的FeatureClass
        /// </summary>
        IFeatureClass saveFeatureClass
        {
            get
            {
                return GetInput<IFeatureClass>("save_feature_class");
            }
        }
        public SaveFeatureClass(string task_id, string model_id, string step_id) : base(task_id, model_id, step_id){}


        public override bool Exexute()
        {
            IWorkspace workspace = GisDataHelper.OpenWorkspace(dataSourceId);
            if (workspace.HasFeatureClass(layerName) && deleteOld) {
                workspace.DeleteFeatureClass(layerName);
            }
            saveFeatureClass.CopyTo(workspace, layerName, clearRecord);
            Output = saveFeatureClass;
            return true;
        }
    }
}
