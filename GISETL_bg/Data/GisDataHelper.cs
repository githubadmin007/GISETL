using ESRI.ArcGIS.Geodatabase;
using FSSG.EsriGIS.Extend;
using FSSG.EsriGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZJH.BaseTools;
using ZJH.BaseTools.BasicExtend;
using ZJH.BaseTools.DB;

namespace GISETL_bg.Data
{
    public class GisDataHelper
    {
        static string TempGDB = GlobalConfig.AppCfg.getValueByPath("configuration/Custom/TempGDB");
        static string GisDataFolder = GlobalConfig.AppCfg.getValueByPath("configuration/Custom/GisDataFolder");

        #region 工作空间相关
        /// <summary>
        /// 打开临时数据工作空间
        /// </summary>
        /// <returns></returns>
        public static IWorkspace OpenTempWorkspace()
        {
            return XWorkspace.GetFileGDBWorkspace(TempGDB);
        }
        /// <summary>
        /// 通过数据源id打开工作空间
        /// </summary>
        /// <param name="dataSourceId">数据源id</param>
        /// <returns></returns>
        public static IWorkspace OpenWorkspace(string dataSourceId)
        {
            using (DatabaseHelper helper = DatabaseHelper.CreateByConnName("GISETL"))
            {
                string sql = $"select * from etl_data_source where id='{dataSourceId}'";
                var dict = helper.ExecuteReader_ToDict(sql);
                string type = dict["TYPE"].ToString("");
                switch (type)
                {
                    case "SDE": return OpenSDE(dict);
                    case "GDB": return OpenGDB(dict);
                    case "MDB": return OpenMDB(dict);
                    case "SHP": return OpenSHP(dict);
                }
            }
            return null;
        }
        /// <summary>
        /// 通过数据源配置信息打开SDE工作空间
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        static IWorkspace OpenSDE(Dictionary<string, object> dict)
        {
            string filePath = dict["FILEPATH"].ToString("");
            return XWorkspace.GetSdeWorkspace($"{GisDataFolder}/{filePath}");
        }
        /// <summary>
        /// 通过数据源配置信息打开GDB工作空间
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        static IWorkspace OpenGDB(Dictionary<string, object> dict)
        {
            string filePath = dict["FILEPATH"].ToString("");
            return XWorkspace.GetFileGDBWorkspace($"{GisDataFolder}/{filePath}");
        }
        /// <summary>
        /// 通过数据源配置信息打开MDB工作空间
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        static IWorkspace OpenMDB(Dictionary<string, object> dict)
        {
            string filePath = dict["FILEPATH"].ToString("");
            return XWorkspace.GetAccessWorkspace($"{GisDataFolder}/{filePath}");
        }
        /// <summary>
        /// 通过数据源配置信息打开GDB工作空间
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        static IWorkspace OpenSHP(Dictionary<string, object> dict)
        {
            string filePath = dict["FILEPATH"].ToString("");
            return XWorkspace.GetShapefileWorkspace($"{GisDataFolder}/{filePath}");
        }
        #endregion

        #region FeatureClass相关
        /// <summary>
        /// 打开FeatureClass
        /// </summary>
        /// <param name="dataSourceId">数据源id</param>
        /// <param name="layerName">图层名</param>
        /// <returns></returns>
        public static IFeatureClass OpenFeatureClass(string dataSourceId, string layerName)
        {
            IWorkspace workspace = OpenWorkspace(dataSourceId);
            return workspace.TryOpenFeatureClass(layerName);
        }
        #endregion
    }
}
