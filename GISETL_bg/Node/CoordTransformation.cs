using ESRI.ArcGIS.DataManagementTools;
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
    class CoordTransformation : _BaseNode
    {
        /// <summary>
        /// 输入层
        /// </summary>
        IFeatureClass inFeatureClass
        {
            get
            {
                return GetInput<IFeatureClass>("in_feature_class");
            }
        }
        public CoordTransformation(string task_id, string model_id, string step_id) : base(task_id, model_id, step_id){ }
        public override bool Exexute()
        {
            // 输出路径
            string LayerName = $"project{DateTime.Now.ToString("yyyyMMddHHmmss")}";
            // 使用GP工具执行Clip操作，会生成临时文件
            Geoprocessor mGeoprocessor = new Geoprocessor();
            mGeoprocessor.OverwriteOutput = true;
            Project mProject = new Project();
            mProject.in_dataset = inFeatureClass;
            // mProject.in_coor_system = 
            mProject.out_dataset = TempGDB + "/" + LayerName;
            mProject.out_coor_system = "PROJCS[\"CGCS2000_3_Degree_GK_CM_113E\",GEOGCS[\"GCS_China_Geodetic_Coordinate_System_2000\",DATUM[\"D_China_2000\",SPHEROID[\"CGCS2000\",6378137.0,298.257222101]],PRIMEM[\"Greenwich\",0.0],UNIT[\"Degree\",0.0174532925199433]],PROJECTION[\"Gauss_Kruger\"],PARAMETER[\"False_Easting\",700000.0],PARAMETER[\"False_Northing\",0.0],PARAMETER[\"Central_Meridian\",113.0],PARAMETER[\"Scale_Factor\",1.0],PARAMETER[\"Latitude_Of_Origin\",0.0],UNIT[\"Meter\",1.0]]";
            mGeoprocessor.Execute(mProject, null);
            // 打开结果图层
            IWorkspace tempWorkspace = XWorkspace.GetFileGDBWorkspace(TempGDB);
            Output = tempWorkspace.TryOpenFeatureClass(LayerName);
            tempWorkspace.Dispose();
            return Output != null;
        }
    }
}
