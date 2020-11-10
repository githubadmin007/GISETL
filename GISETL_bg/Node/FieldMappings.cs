using ESRI.ArcGIS.Geodatabase;
using FSSG.EsriGIS.Extend;
using FSSG.EsriGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZJH.BaseTools.BasicExtend;
using ZJH.BaseTools.DB;

namespace GISETL_bg.Node
{
    public class FieldMappings : _BaseNode
    {
        /// <summary>
        /// 映射id
        /// </summary>
        string fieldGroupId
        {
            get
            {
                return GetParam("field_group_id");
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
        /// <summary>
        /// 字段映射关系
        /// </summary>
        List<FieldMap> fieldMaps = null;
        public FieldMappings(string task_id, string model_id, string step_id) : base(task_id, model_id, step_id){
            using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
            {
                fieldMaps = helper.ExecuteReader_ToList($"select * from node_field_mappings where group_id='{fieldGroupId}'")
                    .Select(dict => CreateFieldMap(dict))
                    .ToList();
            }
        }

        public override bool Exexute()
        {
            string tempLayerName = $"mappings{DateTime.Now.ToString("yyyyMMddHHmmss")}";
            IWorkspace tempWorkspace = XWorkspace.GetFileGDBWorkspace(TempGDB);
            inFeatureClass.CopyTo(tempWorkspace, tempLayerName, true, fieldMaps);
            Output = tempWorkspace.TryOpenFeatureClass(tempLayerName);
            return Output != null;
        }

        private FieldMap CreateFieldMap(Dictionary<string, object> dict) {
            string SourceField = dict["SOURCE_FIELD"].ToString("");
            string TargetField = dict["TARGET_FIELD"].ToString("");
            string TargetAlias = dict["TARGET_ALIAS"].ToString("");
            string TargetType = dict["TARGET_TYPE"].ToString("");
            int TargetLength = dict["TARGET_LENGTH"].ToInt32();
            return new FieldMap(SourceField, TargetField, TargetAlias, TargetType, TargetLength);
        }
    }
}
