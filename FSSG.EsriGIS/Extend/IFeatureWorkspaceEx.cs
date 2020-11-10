using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSSG.EsriGIS.Extend
{
    public static class IFeatureWorkspaceEx
    {
        /// <summary>
        /// 打开FeatureClass
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IFeatureClass TryOpenFeatureClass(this IFeatureWorkspace workspace, string name)
        {
            try
            {
                return workspace.OpenFeatureClass(name);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// 创建FeatureClass
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="fieldsHelper"></param>
        /// <param name="esriFeatureType"></param>
        /// <param name="ShapeFieldName"></param>
        /// <param name="ConfigKeyword"></param>
        public static IFeatureClass CreateFeatureClass(this IFeatureWorkspace workspace, string Name, IFields fields, esriFeatureType type = esriFeatureType.esriFTSimple, string ShapeFieldName = "SHAPE", string ConfigKeyword = "") {
            try
            {
                return workspace.CreateFeatureClass(Name, fields, null, null, type, ShapeFieldName, ConfigKeyword);
            }
            catch (Exception ex)
            {
                throw new Exception("创建FeatureClass失败", ex);
            }
        }
        /// <summary>
        /// 复制图层
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <param name="copyReords">思否复制记录</param>
        public static IFeatureClass CopyFeatureClass(this IFeatureWorkspace workspace, string oldName, string newName, bool copyReords = false) {
            IFeatureClass oldFClass = workspace.OpenFeatureClass(oldName);
            if (oldFClass != null)
            {
                IFields fields = oldFClass.Fields.Clone();
                IFeatureClass newFClass = workspace.CreateFeatureClass(newName, fields, oldFClass.FeatureType, oldFClass.ShapeFieldName);
                if (copyReords)
                {
                    newFClass.CopyFrom(oldFClass);
                }
                return newFClass;
            }
            return null;
        }
        /// <summary>
        /// 重命名FeatureClass
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public static bool ReName(this IFeatureWorkspace workspace, string oldName, string newName) {
            try
            {
                IFeatureClass fc = workspace.OpenFeatureClass(oldName);
                fc.ReName(newName);
            }
            catch
            {
                return false;
            }
            return true;
        }







        public static void ssssss(this IFeatureWorkspace workspace) { }
    }
}
