using ESRI.ArcGIS.AnalysisTools;
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
    public class ClipFeatureClass : _BaseNode
    {
        /// <summary>
        /// 被裁剪层
        /// </summary>
        IFeatureClass inFeatureClass {
            get {
                return GetInput<IFeatureClass>("in_feature_class");
            }
        }
        /// <summary>
        /// 裁剪层
        /// </summary>
        IFeatureClass clipFeatureClass
        {
            get
            {
                return GetInput<IFeatureClass>("clip_feature_class");
            }
        }
        public ClipFeatureClass(string task_id, string model_id, string step_id) : base(task_id, model_id, step_id){}
        /// <summary>
        /// 执行函数
        /// </summary>
        /// <returns></returns>
        public override bool Exexute()
        {
            // 输出路径
            string LayerName = $"clip{DateTime.Now.ToString("yyyyMMddHHmmss")}";
            // 使用GP工具执行Clip操作，会生成临时文件
            Geoprocessor mGeoprocessor = new Geoprocessor();
            mGeoprocessor.OverwriteOutput = true;
            Clip mClip = new Clip();
            mClip.in_features = inFeatureClass;
            mClip.clip_features = clipFeatureClass;
            mClip.out_feature_class = TempGDB + "/" + LayerName;
            mGeoprocessor.Execute(mClip, null);
            // 打开Clip的结果
            IWorkspace tempWorkspace = XWorkspace.GetFileGDBWorkspace(TempGDB);
            Output = tempWorkspace.TryOpenFeatureClass(LayerName);
            tempWorkspace.Dispose();
            return Output != null;
        }
    }
}
