using ESRI.ArcGIS.Geodatabase;
using FSSG.EsriGIS.Extend;
using FSSG.EsriGIS.Geodatabase;
using GISETL_bg.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GISETL_bg.Node
{
    public class MergeShapefile : _BaseNode
    {
        /// <summary>
        /// 数据源id
        /// </summary>
        string dataSourceId
        {
            get
            {
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
        /// shp文件数组
        /// </summary>
        string[] shpPaths
        {
            get
            {
                return GetInput<string[]>("shp_paths");
            }
        }

        public MergeShapefile(string task_id, string model_id, string step_id) : base(task_id, model_id, step_id){ }
        public override bool Exexute()
        {
            IWorkspace workspace = GisDataHelper.OpenWorkspace(dataSourceId);
            if (workspace.HasFeatureClass(layerName) && deleteOld)
            {
                workspace.DeleteFeatureClass(layerName);
            }
            IFeatureClass targetFeatureClass = workspace.TryOpenFeatureClass(layerName);
            if (targetFeatureClass == null)
            {
                if (shpPaths.Length > 0)
                {
                    IFeatureClass shpFeatureClass = XWorkspace.OpenShapeFile(shpPaths[0]);
                    targetFeatureClass = workspace.CreateFeatureClass(layerName, shpFeatureClass.Fields);
                }
            }
            else {
                if (clearRecord) {
                    targetFeatureClass.Delete();
                }
            }
            for (int i = 0; i < shpPaths.Length; i++)
            {
                IFeatureClass shpFeatureClass = XWorkspace.OpenShapeFile(shpPaths[i]);
                shpFeatureClass.CopyTo(targetFeatureClass);
            }
            Output = targetFeatureClass;
            return Output != null;
        }
    }
}
