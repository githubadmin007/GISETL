using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSSG.EsriGIS.Extend
{
    public static class IFeatureCursorEx
    {
        /// <summary>
        /// 将IFeatureCursor保存到指定表
        /// </summary>
        /// <param name="cursor"></param>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        public static bool SaveToFeatureClass(this IFeatureCursor cursor, IFeatureClass featureClass) {
            try
            {
                featureClass.CopyFrom(cursor);
            }
            catch (Exception e ) {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 将IFeatureCursor保存到指定工作空间的指定表
        /// </summary>
        /// <param name="cursor"></param>
        /// <param name="workspace"></param>
        /// <param name="layername"></param>
        /// <param name="clearReords">是否清空原图层的数据</param>
        /// <param name="type"></param>
        /// <param name="ShapeFieldName"></param>
        /// <param name="ConfigKeyword"></param>
        /// <returns></returns>
        public static IFeatureClass SaveToWorkspace(this IFeatureCursor cursor, IFeatureWorkspace workspace, string layername
            , bool clearReords = false
            , esriFeatureType type = esriFeatureType.esriFTSimple
            , string ShapeFieldName = "SHAPE"
            , string ConfigKeyword = "")
        {
            try
            {

                IFeatureClass featureClass = workspace.TryOpenFeatureClass(layername);
                if (featureClass == null)
                {
                    IFields fields = cursor.Fields.Clone();
                    featureClass = workspace.CreateFeatureClass(layername, fields, type, ShapeFieldName, ConfigKeyword);
                }
                else {
                    if (clearReords) {
                        featureClass.Delete();
                    }
                }
                cursor.SaveToFeatureClass(featureClass);
                return featureClass;
            }
            catch
            {
                return null;
            }
        }
    }
}
