using ESRI.ArcGIS.Geodatabase;
using FSSG.EsriGIS.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZJH.BaseTools.BasicExtend;

namespace GISETL_bg.Node
{
    class AddFieldToFeatureClass : _BaseNode
    {
        /// <summary>
        /// 字段名
        /// </summary>
        string name
        {
            get
            {
                return GetParam("name");
            }
        }
        /// <summary>
        /// 字段别名
        /// </summary>
        string alias
        {
            get
            {
                return GetParam("alias");
            }
        }
        /// <summary>
        /// 字段类型
        /// </summary>
        esriFieldType type
        {
            get
            {
                return GetParam("type").ToEnum<esriFieldType>();
            }
        }
        /// <summary>
        /// 字段长度
        /// </summary>
        int length
        {
            get
            {
                return GetParam("length").ToInt32();
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
        public AddFieldToFeatureClass(string task_id, string model_id, string step_id) : base(task_id, model_id, step_id){ }
        public override bool Exexute()
        {
            inFeatureClass.AddField(name, alias, type, length);
            Output = inFeatureClass;
            return Output != null;
        }
    }
}
