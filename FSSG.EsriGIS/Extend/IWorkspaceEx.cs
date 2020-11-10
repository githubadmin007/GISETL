using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FSSG.EsriGIS.Extend
{
    public static class IWorkspaceEx
    {
        /// <summary>
        /// 释放COM对象
        /// </summary>
        /// <param name="workspace"></param>
        public static void Dispose(this IWorkspace workspace) {
            if (workspace != null)
            {
                Marshal.ReleaseComObject(workspace);
            }
        }

        #region FeatureWorkspace相关功能
        /// <summary>
        /// 获取IEnumDataset中的所有FeatureClass名称
        /// </summary>
        /// <param name="datasetsEnum"></param>
        /// <returns></returns>
        static List<string> GetAllFeatureClassName(IEnumDatasetName datasetsEnumName)
        {
            List<string> lst = new List<string>();
            if (datasetsEnumName == null)
            {
                return lst;
            }
            else {
                IDatasetName datasetName = datasetsEnumName.Next();
                while (datasetName != null)
                {
                    if (datasetName.Type == esriDatasetType.esriDTFeatureClass)
                    {
                        lst.Add(datasetName.Name);
                    }
                    else if (datasetName.Type == esriDatasetType.esriDTFeatureDataset)
                    {
                        lst.AddRange(GetAllFeatureClassName(datasetName.SubsetNames));
                    }
                    datasetName = datasetsEnumName.Next();
                }
            }
            return lst;
        }
        /// <summary>
        /// 获取workspace中的所有FeatureClass名称
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllFeatureClassName(this IWorkspace workspace) {
            IEnumDatasetName datasetNameEnum = workspace.DatasetNames[esriDatasetType.esriDTAny];
            return GetAllFeatureClassName(datasetNameEnum);
        }
        /// <summary>
        /// 判断FeatureClass是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool HasFeatureClass(this IWorkspace workspace, string name) {
            List<string> lst = workspace.GetAllFeatureClassName();
            return lst.Contains(name);
        }
        /// <summary>
        /// 删除图层
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool DeleteFeatureClass(this IWorkspace workspace, string name) {
            try
            {
                IEnumDatasetName pEnumDsName = workspace.get_DatasetNames(esriDatasetType.esriDTFeatureClass);
                IDatasetName datasetName = pEnumDsName.Next();
                while (datasetName != null)
                {
                    string[] name_arr = datasetName.Name.Split(new char[] { '.', '/', '\\' });
                    if (name_arr[name_arr.Length - 1].ToUpper() == (name.ToUpper()))
                    {
                        IFeatureWorkspaceManage pFWSM = workspace as IFeatureWorkspaceManage;
                        if (pFWSM.CanDelete((IName)datasetName))
                        {
                            pFWSM.DeleteByName(datasetName);
                            break;
                        }
                    }
                    datasetName = pEnumDsName.Next();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 打开FeatureClass
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IFeatureClass TryOpenFeatureClass(this IWorkspace workspace, string name) {
            IFeatureWorkspace space = workspace as IFeatureWorkspace;
            return space.TryOpenFeatureClass(name);
        }
        /// <summary>
        /// 打开FeatureClass
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IFeatureClass CreateFeatureClass(this IWorkspace workspace, string Name, IFields fields, esriFeatureType type = esriFeatureType.esriFTSimple, string ShapeFieldName = "SHAPE", string ConfigKeyword = "")
        {
            IFeatureWorkspace space = workspace as IFeatureWorkspace;
            return space.CreateFeatureClass(Name, fields, type, ShapeFieldName, ConfigKeyword);
        }
        /// <summary>
        /// 复制图层
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <param name="copyReords">思否复制记录</param>
        public static IFeatureClass CopyFeatureClass(this IWorkspace workspace, string oldName, string newName, bool copyReords = false)
        {
            IFeatureWorkspace space = workspace as IFeatureWorkspace;
            return space.CopyFeatureClass(oldName, newName, copyReords);
        }
        /// <summary>
        /// 重命名FeatureClass
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public static bool ReName(this IWorkspace workspace, string oldName, string newName)
        {
            IFeatureWorkspace space = workspace as IFeatureWorkspace;
            return space.ReName(oldName, newName);
        }
        #endregion









    }
}
