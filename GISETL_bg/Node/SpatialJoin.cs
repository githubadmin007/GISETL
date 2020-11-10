using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessor;
using FSSG.EsriGIS.Extend;
using FSSG.EsriGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GISETL_bg.Node
{
    class SpatialJoin: _BaseNode
    {
        /// <summary>
        /// 目标层
        /// </summary>
        IFeatureClass targetFeatureClass
        {
            get
            {
                return GetInput<IFeatureClass>("target_feature_class");
            }
        }
        /// <summary>
        /// 追加层
        /// </summary>
        IFeatureClass joinFeatureClass
        {
            get
            {
                return GetInput<IFeatureClass>("join_feature_class");
            }
        }
        public SpatialJoin(string task_id, string model_id, string step_id) : base(task_id, model_id, step_id){ }
        public override bool Exexute()
        {
            // 输出路径
            string LayerName = $"join{DateTime.Now.ToString("yyyyMMddHHmmss")}";
            // 使用GP工具执行Clip操作，会生成临时文件
            Geoprocessor mGeoprocessor = new Geoprocessor();
            mGeoprocessor.OverwriteOutput = true;
            ESRI.ArcGIS.AnalysisTools.SpatialJoin mJoin = new ESRI.ArcGIS.AnalysisTools.SpatialJoin();
            mJoin.target_features = targetFeatureClass;
            mJoin.join_features = joinFeatureClass;
            mJoin.join_operation = "JOIN_ONE_TO_ONE"; // JOIN_ONE_TO_MANY
            mJoin.match_option = "INTERSECTS";
            mJoin.join_type = "KEEP_ALL";
            mJoin.out_feature_class = TempGDB + "/" + LayerName;
            //mJoin.field_mapping
            mGeoprocessor.Execute(mJoin, null);
            // 打开Join的结果
            IWorkspace tempWorkspace = XWorkspace.GetFileGDBWorkspace(TempGDB);
            Output = tempWorkspace.TryOpenFeatureClass(LayerName);
            tempWorkspace.Dispose();
            return Output != null;
        }
    }
}
