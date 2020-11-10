using ESRI.ArcGIS.Geodatabase;
using FSSG.EsriGIS.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GISETL_bg.Node
{
    class CreateGuid : _BaseNode
    {
        /// <summary>
        /// 字段名
        /// </summary>
        string fieldName
        {
            get
            {
                return GetParam("field_name");
            }
        }
        /// <summary>
        /// 输入图层
        /// </summary>
        IFeatureClass inFeatureClass
        {
            get
            {
                return GetInput<IFeatureClass>("in_feature_class");
            }
        }

        public CreateGuid(string task_id, string model_id, string step_id) : base(task_id, model_id, step_id){ }
        public override bool Exexute()
        {
            IFeatureCursor cursor = inFeatureClass.Update();
            IFeature feature;
            while (null != (feature = cursor.NextFeature())) {
                string guid = Guid.NewGuid().ToString();
                feature.SetValue(fieldName, guid);
                feature.Store();
            }
            cursor.Flush();
            cursor.Dispose();
            Output = inFeatureClass;
            return Output != null;
        }
    }
}
